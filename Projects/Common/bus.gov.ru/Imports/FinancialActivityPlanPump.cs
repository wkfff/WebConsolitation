using System;
using System.Linq;
using System.Xml.Schema;
using bus.gov.ru;
using bus.gov.ru.types.Item1;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Extensions;

namespace Bus.Gov.Ru.Imports
{
    // TODO прикрутить этот класс к закачке ProcessBudgetaryCircumstancesPumpFile
    // TODO что бы не дублировать и использовать одно поведение
    
    /// <summary>
    /// Закачка ПФХД
    /// </summary>
    public class FinancialActivityPlanPump
    {
        private readonly CommonPump commonPump;
        
        public FinancialActivityPlanPump()
        {
            commonPump = CommonPump.GetCommonPump;

            State = FX_Org_SostD.CreatedStateID;
            PlanThreeYear = false;
        }

        public int State { get; set; }

        public bool PlanThreeYear { get; set; }

        /// <summary>
        /// Выполняет закачку из файла
        /// </summary>
        /// <param name="pumpData">
        /// The pump Data.
        /// </param>
        public void PumpFile(financialActivityPlanType pumpData)
        {
            string result;

            // Проверка по схеме
            if (!pumpData.Validate(Resolver.Get<XmlSchemaSet>(), out result))
            {
                throw new Exception(result);
            }

            refNsiConsRegExtendedStrongType targetOrg = pumpData.initiator ?? pumpData.placer;

            // Проверка учреждения
            if (!commonPump.CheckInstTarget(targetOrg))
            {
                /*return;*/
                throw new Exception(
                    "Учреждение regnum={0}{1} не найдено, требуется обновление nsiOgs"
                        .FormatWith(
                            targetOrg.regNum,
                            targetOrg.fullName.Return(
                                s => string.Format("({0})", s),
                                string.Empty)));
            }

            var year = Convert.ToInt32(pumpData.financialYear);
            var uchr = commonPump.OrgStructuresByRegnumCache[targetOrg.regNum];

            // Проверка предыдущих документов
            commonPump.CheckDocs(uchr.ID, FX_FX_PartDoc.PfhdDocTypeID, year, new[] { FX_Org_SostD.ExportedStateID, State });

            commonPump.HeaderRepository.DbContext.BeginTransaction();

            var header = commonPump.HeaderRepository.FindAll().SingleOrDefault(
                                                                                x => x.RefSost.ID == State
                                                                                        && x.RefPartDoc.ID == FX_FX_PartDoc.PfhdDocTypeID
                                                                                        && x.RefYearForm.ID == year
                                                                                        && x.PlanThreeYear == PlanThreeYear
                                                                                        && x.RefUchr.ID == uchr.ID)
                                                              ?? new F_F_ParameterDoc
                                                              {
                                                                  PlanThreeYear = PlanThreeYear,
                                                                  RefPartDoc = commonPump.TypeDocCache.FindOne(FX_FX_PartDoc.PfhdDocTypeID),
                                                                  RefSost = commonPump.StateDocCache.FindOne(State),
                                                                  RefUchr = uchr,
                                                                  RefYearForm = commonPump.YearFormCache.FindOne(year),
                                                                  OpeningDate = DateTime.Now
                                                              };

            var newFinActPlan = header.FinancialActivityPlan.SingleOrDefault(x => x.RefParametr.ID == header.ID && x.NumberStr == 0) ??
                                    new F_Fin_finActPlan
                                    {
                                        NumberStr = 0,
                                        RefParametr = header,
                                    };
           
            newFinActPlan.realAssets = pumpData.financialIndex.nonfinancialAssets.realAssets;
            newFinActPlan.highValPersAssets = pumpData.financialIndex.nonfinancialAssets.highValuePersonalAssets;
            newFinActPlan.totnonfinAssets = pumpData.financialIndex.nonfinancialAssets.total;

            newFinActPlan.income = pumpData.financialIndex.financialAssets.debit.income;
            newFinActPlan.expense = pumpData.financialIndex.financialAssets.debit.expense;
            newFinActPlan.finAssets = pumpData.financialIndex.financialAssets.total;

            newFinActPlan.kreditExpir = pumpData.financialIndex.financialCircumstances.kreditExpired;
            newFinActPlan.finCircum = pumpData.financialIndex.financialCircumstances.total;

            newFinActPlan.stateTaskGrant = pumpData.planPaymentIndex.planInpayments.stateTaskGrant;
            newFinActPlan.actionGrant = pumpData.planPaymentIndex.planInpayments.actionGrant;
            newFinActPlan.budgetaryFunds = pumpData.planPaymentIndex.planInpayments.budgetaryFunds;
            newFinActPlan.paidServices = pumpData.planPaymentIndex.planInpayments.paidServices;
            newFinActPlan.planInpayments = pumpData.planPaymentIndex.planInpayments.total;

            newFinActPlan.labourRemuneration = pumpData.planPaymentIndex.planPayments.labourRemuneration;
            newFinActPlan.telephoneServices = pumpData.planPaymentIndex.planPayments.telephoneServices;
            newFinActPlan.freightServices = pumpData.planPaymentIndex.planPayments.freightServices;
            newFinActPlan.publicServeces = pumpData.planPaymentIndex.planPayments.publicServices;
            newFinActPlan.rental = pumpData.planPaymentIndex.planPayments.rental;
            newFinActPlan.maintenanceCosts = pumpData.planPaymentIndex.planPayments.maintenanceCosts;
            newFinActPlan.mainFunds = pumpData.planPaymentIndex.planPayments.mainFunds;
            newFinActPlan.fictitiousAssets = pumpData.planPaymentIndex.planPayments.fictitiousAssets;
            newFinActPlan.tangibleAssets = pumpData.planPaymentIndex.planPayments.tangibleAssets;
            newFinActPlan.planPayments = pumpData.planPaymentIndex.planPayments.total;

            newFinActPlan.publish = pumpData.planPublicCircumstances;

            if (newFinActPlan.ID <= 0)
            {
                header.FinancialActivityPlan.Add(newFinActPlan);    
            }
            
            commonPump.ProcessDocumentsHeader(
                                                header,
                                                pumpData.document,
                                                type => type.With(x => commonPump.DocumentTypesCache.FindAll().First(doc => doc.Code.Equals("B"))));

            commonPump.HeaderRepository.Save(header);
            commonPump.HeaderRepository.DbContext.CommitChanges();
            commonPump.HeaderRepository.DbContext.CommitTransaction();
        }
    }
}
