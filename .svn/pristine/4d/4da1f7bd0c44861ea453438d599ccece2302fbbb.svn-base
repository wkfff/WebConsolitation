using System;
using System.Linq;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.PfhdService
{
    public class Pfhd2017Service : NewRestService, IPfhd2017Service
    {
        /// <summary>
        ///   Удаление документа
        /// </summary>
        /// <param name="docId">ID документа</param>
        public void DeleteDoc(int docId)
        {
            try
            {
                // todo реализовать метод с параметром - массивом типов таблиц из которых удалять данные необходимо
                GetItems<F_F_FinancialIndex>().Where(x => x.RefParametr.ID == docId).Each(Delete);
                GetItems<F_F_PlanPaymentIndex>().Where(x => x.RefParametr.ID == docId).Each(Delete);
                GetItems<F_Fin_ExpensePaymentIndex>().Where(x => x.RefParametr.ID == docId).Each(Delete);
                GetItems<F_F_TemporaryResources>().Where(x => x.RefParametr.ID == docId).Each(Delete);
                GetItems<F_F_Reference>().Where(x => x.RefParametr.ID == docId).Each(Delete);
                GetItems<F_Fin_CapFunds>().Where(x => x.RefParametr.ID == docId).Each(Delete);
                GetItems<F_Fin_realAssFunds>().Where(x => x.RefParametr.ID == docId).Each(Delete);
                GetItems<F_Fin_othGrantFunds>().Where(x => x.RefParametr.ID == docId).Each(Delete);
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка удаления документа \"ПФХД 2017\": " + e.Message, e);
            }
        }

        /// <summary>
        /// Проверка на пустой документ
        /// </summary>
        /// <param name="docId">ID документа</param>
        public bool CheckDocEmpty(int docId)
        {
            if (GetItems<F_F_FinancialIndex>().Any(x => x.RefParametr.ID == docId)
                || GetItems<F_F_PlanPaymentIndex>().Any(x => x.RefParametr.ID == docId)
                || GetItems<F_Fin_CapFunds>().Any(x => x.RefParametr.ID == docId) 
                || GetItems<F_Fin_realAssFunds>().Any(x => x.RefParametr.ID == docId)
                || GetItems<F_Fin_othGrantFunds>().Any(x => x.RefParametr.ID == docId) 
                || GetItems<F_F_TemporaryResources>().Any(x => x.RefParametr.ID == docId)
                || GetItems<F_F_Reference>().Any(x => x.RefParametr.ID == docId) 
                || GetItems<F_Fin_ExpensePaymentIndex>().Any(x => x.RefParametr.ID == docId))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Копирование контента документа в новый документ
        /// </summary>
        /// <param name="docId">ID документа</param>
        public void CopyContent(int docId)
        {
            var idOfLastDoc = Resolver.Get<IVersioningService>().GetDocumentForCopy(docId).ID;
            var parameterDoc = GetItem<F_F_ParameterDoc>(docId);

            GetItems<F_F_FinancialIndex>().Where(x => x.RefParametr.ID == idOfLastDoc).Each(
                x =>
                    {
                        var row = new F_F_FinancialIndex
                                      {
                                          AccountsPayable = x.AccountsPayable,
                                          Debentures = x.Debentures,
                                          DebitExpense = x.DebitExpense,
                                          DebitIncome = x.DebitIncome,
                                          FinancialAssets = x.FinancialAssets,
                                          FinancialCircumstanc = x.FinancialCircumstanc,
                                          FundsAccountsInstitution = x.FundsAccountsInstitution,
                                          FundsPlacedOnDeposits = x.FundsPlacedOnDeposits,
                                          HighValuePADepreciatedCost = x.HighValuePADepreciatedCost,
                                          HighValuePersonalAssets = x.HighValuePersonalAssets,
                                          KreditExpired = x.KreditExpired,
                                          MoneyInstitutions = x.MoneyInstitutions,
                                          NonFinancialAssets = x.NonFinancialAssets,
                                          OtherFinancialInstruments = x.OtherFinancialInstruments,
                                          RealAssets = x.RealAssets,
                                          RealAssetsDepreciatedCost = x.RealAssetsDepreciatedCost,
                                          RefParametr = parameterDoc
                                      };
                        Save(row);
                    });

            GetItems<F_F_PlanPaymentIndex>().Where(x => x.RefParametr.ID == idOfLastDoc).Each(
                x =>
                    {
                        var row = new F_F_PlanPaymentIndex
                                    {
                                        RefParametr = parameterDoc,
                                        LineCode = x.LineCode,
                                        CapitalInvestment = x.CapitalInvestment,
                                        FinancialProvision = x.FinancialProvision,
                                        HealthInsurance = x.HealthInsurance,
                                        Kbk = x.Kbk,
                                        Name = x.Name,
                                        Period = x.Period,
                                        ServiceGrant = x.ServiceGrant,
                                        ServiceTotal = x.ServiceTotal,
                                        SubsidyFinSupportFfoms = x.SubsidyFinSupportFfoms,
                                        SubsidyOtherPurposes = x.SubsidyOtherPurposes,
                                        Total = x.Total
                                    };
                        Save(row);
                    });

            GetItems<F_F_TemporaryResources>().Where(x => x.RefParametr.ID == idOfLastDoc).Each(
                x =>
                    {
                        var row = new F_F_TemporaryResources
                                    {
                                            RefParametr = parameterDoc,
                                            BalanceBeginningYear = x.BalanceBeginningYear,
                                            BalanceEndYear = x.BalanceEndYear,
                                            Disposals = x.Disposals,
                                            Income = x.Income
                                    };
                        Save(row);
                    });

            GetItems<F_F_Reference>().Where(x => x.RefParametr.ID == idOfLastDoc).Each(
                x =>
                    {
                        var row = new F_F_Reference
                                      {
                                          RefParametr = parameterDoc,
                                          AmountPublicLiabilities = x.AmountPublicLiabilities,
                                          AmountTemporaryOrder = x.AmountTemporaryOrder,
                                          VolumeBudgetIinvestments = x.VolumeBudgetIinvestments
                                      };
                        
                        Save(row);
                    });

            GetItems<F_Fin_ExpensePaymentIndex>().Where(x => x.RefParametr.ID == idOfLastDoc).Each(
                x =>
                    {
                        var row = new F_Fin_ExpensePaymentIndex
                                      {
                                          RefParametr = parameterDoc,
                                          LineCode = x.LineCode,
                                          Fz223SumFirstPlanYear = x.Fz223SumFirstPlanYear,
                                          Fz223SumNextYear = x.Fz223SumNextYear,
                                          Fz223SumSecondPlanYear = x.Fz223SumSecondPlanYear,
                                          Fz44SumFirstPlanYear = x.Fz44SumFirstPlanYear,
                                          Fz44SumNextYear = x.Fz44SumNextYear,
                                          Fz44SumSecondPlanYear = x.Fz44SumSecondPlanYear,
                                          Name = x.Name,
                                          TotalSumFirstPlanYear = x.TotalSumFirstPlanYear,
                                          TotalSumNextYear = x.TotalSumNextYear,
                                          TotalSumSecondPlanYear = x.TotalSumSecondPlanYear,
                                          Year = x.Year
                                      };
                        Save(row);
                    });

            GetItems<F_Fin_CapFunds>().Where(x => x.RefParametr.ID == idOfLastDoc).Each(
                x =>
                    {
                        var row = new F_Fin_CapFunds
                                      {
                                          RefParametr = parameterDoc,
                                          Name = x.Name,
                                          funds = x.funds
                                      };
                        Save(row);
                    });

            GetItems<F_Fin_realAssFunds>().Where(x => x.RefParametr.ID == idOfLastDoc).Each(
                x =>
                    {
                        var row = new F_Fin_realAssFunds
                                      {
                                          RefParametr = parameterDoc,
                                          Name = x.Name,
                                          funds = x.funds
                                      };
                        Save(row);
                    });

            GetItems<F_Fin_othGrantFunds>().Where(x => x.RefParametr.ID == idOfLastDoc).Each(
                x =>
                    {
                        var row = new F_Fin_othGrantFunds
                                      {
                                          RefParametr = parameterDoc,
                                          funds = x.funds,
                                          KOSGY = x.KOSGY,
                                          RefOtherGrant = x.RefOtherGrant
                                      };
                        Save(row);
                    });
        }
    }
}
