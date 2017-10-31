using System;
using System.Collections.Generic;
using System.Linq;
using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using GlobalConsts = Krista.FM.RIA.Extensions.E86N.Utils.GlobalConsts;

namespace Krista.FM.RIA.Extensions.E86N.Services.Export
{
    public static class ExportFinancialActivityPlanService
    {
        public static byte[] Serialize(IAuthService authService, F_F_ParameterDoc header)
        {
            // лучше тут получим ошибку
            F_Fin_finActPlan finActPlan = header.FinancialActivityPlan.Single(x => x.NumberStr == 0);

            D_Org_Structure target = header.RefUchr;
            D_Org_UserProfile placerProfile = authService.Profile;
            if (placerProfile == null)
            {
                throw new InvalidOperationException(GlobalConsts.NullProfile);
            }

            int year = header.RefYearForm.ID;

            List<F_Doc_Docum> documents = header.Documents.Where(docum => docum.RefTypeDoc.Code.Equals("P")).ToList();

            var financialActivityPlanContent =
                new financialActivityPlanType
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
                            new financialActivityPlanType.financialIndexLocalType
                                {
                                    nonfinancialAssets =
                                        new financialActivityPlanType.financialIndexLocalType.nonfinancialAssetsLocalType
                                            {
                                                realAssets = (decimal)finActPlan.realAssets,
                                                highValuePersonalAssets = (decimal)finActPlan.highValPersAssets,
                                                total = (decimal)finActPlan.totnonfinAssets
                                            },
                                    financialAssets =
                                        new financialActivityPlanType.financialIndexLocalType.financialAssetsLocalType
                                            {
                                                debit =
                                                    new financialActivityPlanType.financialIndexLocalType.
                                                    financialAssetsLocalType.debitLocalType
                                                        {
                                                            income = (decimal)finActPlan.income,
                                                            expense = (decimal)finActPlan.expense
                                                        },
                                                total = (decimal)finActPlan.finAssets
                                            },
                                    financialCircumstances =
                                        new financialActivityPlanType.financialIndexLocalType.financialCircumstancesLocalType
                                            {
                                                kreditExpired = (decimal)finActPlan.kreditExpir,
                                                total = (decimal)finActPlan.finCircum
                                            }
                                },
                        planPaymentIndex =
                            new financialActivityPlanType.planPaymentIndexLocalType
                                {
                                    planInpayments =
                                        new financialActivityPlanType.planPaymentIndexLocalType.planInpaymentsLocalType
                                            {
                                                stateTaskGrant = (decimal)finActPlan.stateTaskGrant,
                                                actionGrant = (decimal)finActPlan.actionGrant,
                                                budgetaryFunds = (decimal)finActPlan.budgetaryFunds,
                                                paidServices = (decimal)finActPlan.paidServices,
                                                total = (decimal)finActPlan.planInpayments
                                            },
                                    planPayments =
                                        new financialActivityPlanType.planPaymentIndexLocalType.planPaymentsLocalType
                                            {
                                                labourRemuneration = (decimal)finActPlan.labourRemuneration,
                                                telephoneServices = (decimal)finActPlan.telephoneServices,
                                                freightServices = (decimal)finActPlan.freightServices,
                                                publicServices = (decimal)finActPlan.publicServeces,
                                                rental = (decimal)finActPlan.rental,
                                                maintenanceCosts = (decimal)finActPlan.maintenanceCosts,
                                                mainFunds = (decimal)finActPlan.mainFunds,
                                                fictitiousAssets = (decimal)finActPlan.fictitiousAssets,
                                                tangibleAssets = (decimal)finActPlan.tangibleAssets,
                                                total = (decimal)finActPlan.planPayments
                                            }
                                },
                        planPublicCircumstances = (decimal)finActPlan.publish,
                        document = ExportServiceHelper.Documents(documents)
                    };

            return ExportServiceHelper.Serialize(
                new financialActivityPlan
                    {
                        header = ExportServiceHelper.HeaderType(),
                        body = new financialActivityPlan.bodyLocalType
                                   {
                                       position = financialActivityPlanContent
                                   }
                    }.Save);
        }
    }
}
