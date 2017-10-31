using System;
using System.Collections.Generic;
using System.Linq;

using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;

using GlobalConsts = Krista.FM.RIA.Extensions.E86N.Utils.GlobalConsts;

namespace Krista.FM.RIA.Extensions.E86N.Services.Export
{
    public static class ExportFinancialActivityPlan2017Service
    {
        public static byte[] Serialize(F_F_ParameterDoc header)
        {
            var authService = Resolver.Get<IAuthService>();

            D_Org_Structure target = header.RefUchr;
            D_Org_UserProfile placerProfile = authService.Profile;
            if (placerProfile == null)
            {
                throw new InvalidOperationException(GlobalConsts.NullProfile);
            }

            int year = header.RefYearForm.ID;

            List<F_Doc_Docum> documents = header.Documents.Where(docum => docum.RefTypeDoc.Code.Equals("P")).ToList();
            
            var financialIndex = header.FinancialIndex.FirstOrDefault();

            var financialActivityPlan2017Content =
                new financialActivityPlan2017Type
                    {
                        positionId = Guid.NewGuid().ToString(),
                        changeDate = DateTime.Now,
                        placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                        initiator = target.ID != placerProfile.RefUchr.ID
                                        ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                                        : null,
                        financialYear = year,
                        planFirstYear = year + 1,
                        planLastYear = year + 2,
                        financialIndex =
                            new financialActivityPlan2017Type.financialIndexLocalType
                                {
                                    nonfinancialAssets =
                                        new financialActivityPlan2017Type.financialIndexLocalType
                                        .nonfinancialAssetsLocalType
                                            {
                                                realAssets = financialIndex == null ? 0 : financialIndex.RealAssets.GetValueOrDefault(),
                                                realAssetsResidual = financialIndex == null ? 0 : financialIndex.RealAssetsDepreciatedCost.GetValueOrDefault(),
                                                highValuePersonalAssets = financialIndex == null ? 0 : financialIndex.HighValuePersonalAssets.GetValueOrDefault(),
                                                highValuePersonalAssetsResidual = financialIndex == null ? 0 : financialIndex.HighValuePADepreciatedCost.GetValueOrDefault(),
                                                total = financialIndex == null ? 0 : financialIndex.NonFinancialAssets.GetValueOrDefault()
                                        },
                                    financialAssets =
                                        new financialActivityPlan2017Type.financialIndexLocalType.financialAssetsLocalType
                                            {
                                                cash = financialIndex == null ? 0 : financialIndex.MoneyInstitutions.GetValueOrDefault(),
                                                accountsCash = financialIndex == null ? 0 : financialIndex.FundsAccountsInstitution.GetValueOrDefault(),
                                                depositCash = financialIndex == null ? 0 : financialIndex.FundsPlacedOnDeposits.GetValueOrDefault(),
                                                others = financialIndex == null ? 0 : financialIndex.OtherFinancialInstruments.GetValueOrDefault(),
                                                debit =
                                                    new financialActivityPlan2017Type.financialIndexLocalType
                                                    .financialAssetsLocalType.debitLocalType
                                                        {
                                                            income = financialIndex == null ? 0 : financialIndex.DebitIncome.GetValueOrDefault(),
                                                            expense = financialIndex == null ? 0 : financialIndex.DebitExpense.GetValueOrDefault()
                                                    },
                                                total = financialIndex == null ? 0 : financialIndex.FinancialAssets.GetValueOrDefault()
                                            },
                                    financialCircumstances =
                                        new financialActivityPlan2017Type.financialIndexLocalType
                                        .financialCircumstancesLocalType
                                            {
                                                debentures = financialIndex == null ? 0 : financialIndex.Debentures.GetValueOrDefault(),
                                                kredit = financialIndex == null ? 0 : financialIndex.AccountsPayable.GetValueOrDefault(),
                                                kreditExpired = financialIndex == null ? 0 : financialIndex.KreditExpired.GetValueOrDefault(),
                                                total = financialIndex == null ? 0 : financialIndex.FinancialCircumstanc.GetValueOrDefault()
                                        }
                                },
                        planPaymentIndex = GetPlanPaymentIndex(header.PlanPaymentIndex.Where(x => x.Period.Equals(0))),
                        expensePaymentIndex = GetExpensePaymentIndex(header.ExpensePaymentIndex),
                        temporaryResources = GetTemporaryResources(header.TemporaryResources.FirstOrDefault()),
                        reference = GetReference(header.Reference.FirstOrDefault()),
                        document = ExportServiceHelper.Documents(documents)
                    };

            return ExportServiceHelper.Serialize(
                new financialActivityPlan2017
                    {
                        header = ExportServiceHelper.HeaderType(),
                        body = new financialActivityPlan2017.bodyLocalType
                                   {
                                       position = financialActivityPlan2017Content
                                   }
                    }.Save);
        }

        private static List<planPaymentIndexItem> GetPlanPaymentIndex(IEnumerable<F_F_PlanPaymentIndex> planPaymentIndex)
        {
            var result = new List<planPaymentIndexItem>();
            planPaymentIndex.Each(
                x =>
                    {
                        var item = new planPaymentIndexItem
                                        {
                                            name = x.Name,
                                            lineCode = x.LineCode,
                                            total = x.Total
                                        };

                        if (x.Kbk.IsNotNullOrEmpty())
                        {
                            item.kbk = x.Kbk;
                        }
                        
                        item.financialProvision = x.FinancialProvision.GetValueOrDefault();
                        item.capitalInvestment = x.CapitalInvestment.GetValueOrDefault();
                        item.healthInsurance = x.HealthInsurance.GetValueOrDefault();
                        item.serviceTotal = x.ServiceTotal.GetValueOrDefault();
                        item.serviceGrant = x.ServiceGrant.GetValueOrDefault();
                        item.item781 = x.SubsidyOtherPurposes.GetValueOrDefault();
                        
                        result.Add(item);
                    });

            return result;
        }

        private static List<expensePaymentIndexItem> GetExpensePaymentIndex(IList<F_Fin_ExpensePaymentIndex> expensePaymentIndex)
        {
            var result = new List<expensePaymentIndexItem>();
            expensePaymentIndex.Each(
                x =>
                    {
                        var item = new expensePaymentIndexItem
                                       {
                                           name = x.Name,
                                           lineCode = x.LineCode,
                                           totalSum =
                                               new expenseSum
                                                   {
                                                       nextYear = x.TotalSumNextYear,
                                                       firstPlanYear = x.TotalSumFirstPlanYear,
                                                       secondPlanYear = x.TotalSumSecondPlanYear
                                                   },
                                           fz44Sum =
                                               new expenseSum
                                                   {
                                                       nextYear = x.Fz44SumNextYear,
                                                       firstPlanYear = x.Fz44SumFirstPlanYear,
                                                       secondPlanYear = x.Fz44SumSecondPlanYear
                                                   },
                                           fz223Sum =
                                               new expenseSum
                                                   {
                                                       nextYear = x.Fz223SumNextYear,
                                                       firstPlanYear = x.Fz223SumFirstPlanYear,
                                                       secondPlanYear = x.Fz223SumSecondPlanYear
                                                   }
                                       };

                        if (x.Year.HasValue)
                        {
                            item.year = x.Year;
                        }

                        result.Add(item);
                    });
            return result;
        }

        private static List<indexSum> GetTemporaryResources(F_F_TemporaryResources temporaryResources)
        {
            var result = new List<indexSum>
                             {
                                 new indexSum
                                     {
                                        name = "Остаток средств на конец года",
                                        lineCode = "010",
                                        total = temporaryResources == null ? 0 : temporaryResources.BalanceBeginningYear.GetValueOrDefault()
                                     },
                                 new indexSum
                                     {
                                        name = "Остаток средств на начало года",
                                        lineCode = "020",
                                        total = temporaryResources == null ? 0 : temporaryResources.BalanceEndYear.GetValueOrDefault()
                                     },
                                 new indexSum
                                     {
                                        name = "Поступление",
                                        lineCode = "030",
                                        total = temporaryResources == null ? 0 : temporaryResources.Income.GetValueOrDefault()
                                     },
                                 new indexSum
                                     {
                                        name = "Выбытие",
                                        lineCode = "040",
                                        total = temporaryResources == null ? 0 : temporaryResources.Disposals.GetValueOrDefault()
                                     }
                             };

            return result;
        }

        private static List<indexSum> GetReference(F_F_Reference reference)
        {
            var result = new List<indexSum>
                             {
                                 new indexSum
                                        {
                                            name = "Объем публичных обязательств. Всего",
                                            lineCode = "010",
                                            total = reference == null ? 0 : decimal.Round(reference.AmountPublicLiabilities.GetValueOrDefault() / 1000, 2)
                                        },
                                 new indexSum
                                        {
                                            name = "Объем бюджетных инвестиций",
                                            lineCode = "020",
                                            total = reference == null ? 0 : decimal.Round(reference.VolumeBudgetIinvestments.GetValueOrDefault() / 1000, 2)
                                        },
                                 new indexSum
                                        {
                                            name = "Объем средств поступивших во временное распоряжение. Всего",
                                            lineCode = "030",
                                            total = reference == null ? 0 : decimal.Round(reference.AmountTemporaryOrder.GetValueOrDefault() / 1000, 2)
                                        }
                             };
            return result;
        }
    }
}