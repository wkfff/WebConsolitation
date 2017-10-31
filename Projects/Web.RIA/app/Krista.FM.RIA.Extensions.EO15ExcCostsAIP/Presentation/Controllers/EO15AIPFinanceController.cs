using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Services;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Controllers
{
    public class EO15AIPFinanceController : SchemeBoundController
    {
        private readonly IConstructionService constructRepository;
        private readonly ILinqRepository<D_ExcCosts_AIPMark> markRepository;
        private readonly IRepository<D_ExcCosts_Audits> auditRepository;
        private readonly IRepository<D_ExcCosts_Contract> contractRepository;
        private readonly ILinqRepository<D_ExcCosts_Finances> financesRepository;
        private readonly IRepository<D_ExcCosts_StatusD> statusDRepository;
        private readonly ILinqRepository<F_ExcCosts_AIP> factsAipRepository;
        private readonly IRepository<D_ExcCosts_WorkType> workTypeRepository;
        private readonly IEO15ExcCostsAIPExtension extension;
        private readonly IRepository<FX_Date_YearDayUNV> periodRepository;
        
        public EO15AIPFinanceController(
            IEO15ExcCostsAIPExtension extension,
            ILinqRepository<F_ExcCosts_AIP> factsAipRepository, 
            IRepository<D_ExcCosts_WorkType> workTypeRepository,
            IConstructionService constructRepository,
            ILinqRepository<D_ExcCosts_AIPMark> markRepository,
            IRepository<D_ExcCosts_Audits> auditRepository,
            IRepository<D_ExcCosts_Contract> contractRepository,
            ILinqRepository<D_ExcCosts_Finances> financesRepository,
            IRepository<D_ExcCosts_StatusD> statusDRepository,
            IRepository<FX_Date_YearDayUNV> periodRepository)
        {
            this.extension = extension;
            this.periodRepository = periodRepository;
            this.factsAipRepository = factsAipRepository;
            this.workTypeRepository = workTypeRepository;
            this.constructRepository = constructRepository;
            this.markRepository = markRepository;
            this.auditRepository = auditRepository;
            this.contractRepository = contractRepository;
            this.financesRepository = financesRepository;
            this.statusDRepository = statusDRepository;
        }

        /// <summary>
        /// Чтение данных "Финансирование"
        /// </summary>
        /// <param name="objectId">иднтификатор объекта строительства</param>
        /// <param name="periodId">идентификатор отчетного периода</param>
        public AjaxStoreResult Read(int objectId, int[] periodId)
        {
            try
            {
                var data = new List<object>();

                foreach (var curPeriod in periodId)
                {
                    if (curPeriod <= 0)
                    {
                        throw new Exception();
                    }

                    var period = periodRepository.Get(curPeriod);
                    if (period == null || curPeriod < 1)
                    {
                        return new AjaxStoreResult
                                   {
                                       ResponseFormat = StoreResponseFormat.Load,
                                       Data = new List<int>(),
                                       Total = 0
                                   };
                    }

                    var subMOVisible = User.IsInRole(AIPRoles.Coordinator) ||
                                       User.IsInRole(AIPRoles.MOClient) ||
                                       User.IsInRole(AIPRoles.User);
                    var investAOVisible = User.IsInRole(AIPRoles.Coordinator) ||
                                          User.IsInRole(AIPRoles.GovClient) ||
                                          User.IsInRole(AIPRoles.User);

                    var typeWorkDefault = workTypeRepository.Get(-1);

                    var year = curPeriod / 10000;
                    for (var quarter = 1; quarter <= 4; quarter++)
                    {
                        var curQuarterPeriod = (year * 10000) + 9990 + quarter;
                        var financeOnPeriod = GetFinanceOnPeriod(
                            curQuarterPeriod,
                            objectId,
                            typeWorkDefault,
                            investAOVisible,
                            subMOVisible,
                            true);

                        data.Add(financeOnPeriod);
                    }
                }

                return new AjaxStoreResult
                           {
                               ResponseFormat = StoreResponseFormat.Load,
                               Data = data,
                               Total = data.Count
                           };
            }
            catch (Exception)
            {
                return new AjaxStoreResult
                           {
                               ResponseFormat = StoreResponseFormat.Load,
                               Data = new List<string>(),
                               Total = 0
                           };
            }
        }

        /// <summary>
        /// Сохранение изменений в данных "Финансирование"
        /// </summary>
        [Transaction]
        public ActionResult Save()
        {
            try
            {
                var finances = financesRepository.FindAll();
                var financesDefault = financesRepository.FindOne(-1);
                var financeBudgetInvestAO = finances.FirstOrDefault(x => x.Code == AIPFinances.FinanceBudgetInvestAO);
                var financeSubMO = finances.FirstOrDefault(x => x.Code == AIPFinances.FinanceSubMO);
                var financeBudgetMO = finances.FirstOrDefault(x => x.Code == AIPFinances.FinanceBudgetMO);
                var financeOther = finances.FirstOrDefault(x => x.Code == AIPFinances.FinanceOther);
                var financeCooperation = finances.FirstOrDefault(x => x.Code == AIPFinances.FinanceCooperation);
                var financeBudgetRF = finances.FirstOrDefault(x => x.Code == AIPFinances.FinanceBudgetRF);
                var financeOutOfBudget = finances.FirstOrDefault(x => x.Code == AIPFinances.FinanceOutOfBudget);

                var auditDefault = auditRepository.Get(-1);
                var contractDefault = contractRepository.Get(-1);
                var typeWorkDefault = workTypeRepository.Get(-1);

                var dataHandler = new StoreDataHandler(HttpContext.Request["data"]);
                var dataSet = JsonDataSetParser.Parse(dataHandler.JsonData);
                if (dataSet.ContainsKey("Updated"))
                {
                    var markFinance = markRepository.FindAll().FirstOrDefault(x => x.Code == AIPMarks.MarkFinance);
                    var markFinanceSubBudgetAO = markRepository.FindAll().FirstOrDefault(x => x.Code == AIPMarks.MarkFinanceSubBudgetAO);
                    var markUtilizedInCurPrice = markRepository.FindAll().FirstOrDefault(x => x.Code == AIPMarks.MarkUtilizedInCurPrice);
                    var markUtilizedInBasePrice = markRepository.FindAll().FirstOrDefault(x => x.Code == AIPMarks.MarkUtilizedInBasePrice);
                    var markBalanceInBasePrice = markRepository.FindAll().FirstOrDefault(x => x.Code == AIPMarks.MarkBalanceBasePrice);
                    
                    var table = dataSet["Updated"];
                    foreach (var record in table)
                    {
                        var periodId = CommonService.GetIntFromRecord(record, "PeriodId", "Период");
                        var period = periodRepository.Get(periodId);
                        if (period == null)
                        {
                            throw new Exception("Выберите период");
                        }

                        var objId = CommonService.GetIntFromRecord(record, "CObjectId", "Объект строительства");
                        var obj = constructRepository.GetOne(objId);

                        var statusDId = CommonService.GetIntFromRecord(record, "StatusDId", "Статус данных");
                        var statusD = statusDRepository.Get(statusDId);

                        var investAOVisible = User.IsInRole(AIPRoles.Coordinator) || 
                            User.IsInRole(AIPRoles.GovClient) || 
                            User.IsInRole(AIPRoles.User);
                        if (investAOVisible)
                        {
                            SaveFact(record, period, obj, markFinance, financeBudgetInvestAO, auditDefault, contractDefault, statusD, typeWorkDefault, "FactAOID", "FactAOValue", "бюджетные инвестиции автономного округа");
                        }

                        var subMOVisible = User.IsInRole(AIPRoles.Coordinator) || User.IsInRole(AIPRoles.MOClient) || User.IsInRole(AIPRoles.User);
                        if (subMOVisible)
                        {
                            SaveFact(record, period, obj, markFinance, financeSubMO, auditDefault, contractDefault, statusD, typeWorkDefault, "FactSubMOID", "FactSubMOValue", "субсидии муниципальным образованиям");
                        }

                        SaveFact(record, period, obj, markFinance, financeBudgetMO, auditDefault, contractDefault, statusD, typeWorkDefault, "FactMOID", "FactMOValue", "От бюджетов мундругихиципальных образований");
                        SaveFact(record, period, obj, markFinance, financeOther, auditDefault, contractDefault, statusD, typeWorkDefault, "FactOtherID", "FactOtherValue", "От других источников");
                        SaveFact(record, period, obj, markFinance, financeOutOfBudget, auditDefault, contractDefault, statusD, typeWorkDefault, "FactOutOfBudgetID", "FactOutOfBudgetValue", "Объем средств из внебюджетных источников");
                        SaveFact(record, period, obj, markFinanceSubBudgetAO, financesDefault, auditDefault, contractDefault, statusD, typeWorkDefault, "FactSubAOID", "FactSubAOValue", "Профинансировано за счет субсидий АО");
                        SaveFact(record, period, obj, markUtilizedInCurPrice, financesDefault, auditDefault, contractDefault, statusD, typeWorkDefault, "FactMasterID", "FactMasterValue", "Освоено за отчетный период в текущих ценах");
                        SaveFact(record, period, obj, markUtilizedInBasePrice, financesDefault, auditDefault, contractDefault, statusD, typeWorkDefault, "FactMasterBasePriceID", "FactMasterBasePriceValue", "Освоено за отчетный период в базовых ценах");
                        SaveFact(record, period, obj, markBalanceInBasePrice, financesDefault, auditDefault, contractDefault, statusD, typeWorkDefault, "FactBalanceBasePriceID", "FactBalanceBasePriceValue", "Остаток средств в базовых ценах");

                        SaveFact(record, period, obj, markFinance, financeBudgetRF, auditDefault, contractDefault, statusD, typeWorkDefault, "FactBudgetRFID", "FactBudgetRFValue", "Бюджет РФ");
                        SaveFact(record, period, obj, markFinance, financeCooperation, auditDefault, contractDefault, statusD, typeWorkDefault, "FactProgramCooperationID", "FactProgramCooperationValue", "Программа ССотрудничество");
                    }
                }

                return new RestResult
                {
                    Success = true,
                    Message = "Объекты строительства сохранены",
                    Data = new List<object>()
                };
            }
            catch (Exception e)
            {
                return CommonService.ErrorResult(e.Message);
            }
        }

        private object GetFinanceOnPeriod(int? periodId, int objectId, D_ExcCosts_WorkType typeWorkDefault, bool investAOVisible, bool subMOVisible, bool editable)
        {
            var facts = factsAipRepository.FindAll().Where(x =>
                                                          x.RefCObject.ID == objectId &&
                                                          x.RefPeriod.ID == periodId &&
                                                          x.SourceID == extension.DataSource.ID &&
                                                          x.RefTypeWork.ID == typeWorkDefault.ID);

            var factAO = facts.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkFinance && x.RefFinances.Code == AIPFinances.FinanceBudgetInvestAO);
            var factSubMO = facts.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkFinance && x.RefFinances.Code == AIPFinances.FinanceSubMO);
            var factMO = facts.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkFinance && x.RefFinances.Code == AIPFinances.FinanceBudgetMO);
            var factOther = facts.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkFinance && x.RefFinances.Code == AIPFinances.FinanceOther);
            var factOutOfBudget = facts.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkFinance && x.RefFinances.Code == AIPFinances.FinanceOutOfBudget);
            var factSubAO = facts.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkFinanceSubBudgetAO);
            var factMaster = facts.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkUtilizedInCurPrice);
            var factMasterBasePrice = facts.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkUtilizedInBasePrice);
            var factBalanceBasePrice = facts.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkBalanceBasePrice);
            
            var factBudgetRF = facts.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkFinance && x.RefFinances.Code == AIPFinances.FinanceBudgetRF);
            var factProgram = facts.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkFinance && x.RefFinances.Code == AIPFinances.FinanceCooperation);

            var factToUse = factAO ??
                            factSubMO ??
                            factMO ??
                            factOther ??
                            factOutOfBudget ??
                            factSubAO ??
                            factMaster ??
                            factMasterBasePrice ?? 
                            factBalanceBasePrice ?? 
                            factBudgetRF ?? 
                            factProgram;

            var statusDId = factToUse == null ? (int)AIPStatusD.Edit : factToUse.RefStatusD.ID;
            var statusD = statusDRepository.Get(statusDId);

            var year = periodId / 10000;
            var quarter = periodId % 10;
            return new
            {
                CObjectId = objectId,
                PeriodId = periodId,
                PeriodName = "{0} квартал {1} года".FormatWith(quarter, year),
                FactAOID = factAO == null ? -1 : factAO.ID,
                FactAOValue = factAO == null ? 0 : factAO.Value,
                FactSubMOID = factSubMO == null ? -1 : factSubMO.ID,
                FactSubMOValue = factSubMO == null ? 0 : factSubMO.Value,
                FactMOID = factMO == null ? -1 : factMO.ID,
                FactMOValue = factMO == null ? 0 : factMO.Value,
                FactOtherID = factOther == null ? -1 : factOther.ID,
                FactOtherValue = factOther == null ? 0 : factOther.Value,
                FactOutOfBudgetID = factOutOfBudget == null ? -1 : factOutOfBudget.ID,
                FactOutOfBudgetValue = factOutOfBudget == null ? 0 : factOutOfBudget.Value,
                FactAll = (factAO == null || !investAOVisible ? 0 : factAO.Value) +
                          (factSubMO == null || !subMOVisible ? 0 : factSubMO.Value) +
                          (factMO == null ? 0 : factMO.Value) +
                          (factOther == null ? 0 : factOther.Value) + 
                          (factOutOfBudget == null ? 0 : factOutOfBudget.Value) +
                          (factBudgetRF == null ? 0 : factBudgetRF.Value) + 
                          (factProgram == null ? 0 : factProgram.Value),
                FactSubAOID = factSubAO == null ? 0 : factSubAO.ID,
                FactSubAOValue = factSubAO == null ? 0 : factSubAO.Value,
                FactMasterID = factMaster == null ? 0 : factMaster.ID,
                FactMasterValue = factMaster == null ? 0 : factMaster.Value,
                FactMasterBasePriceID = factMasterBasePrice == null ? 0 : factMasterBasePrice.ID,
                FactMasterBasePriceValue = factMasterBasePrice == null ? 0 : factMasterBasePrice.Value,
                FactBalanceBasePriceID = factBalanceBasePrice == null ? 0 : factBalanceBasePrice.ID,
                FactBalanceBasePriceValue = factBalanceBasePrice == null ? 0 : factBalanceBasePrice.Value,
                FactBudgetRFID = factBudgetRF == null ? 0 : factBudgetRF.ID,
                FactBudgetRFValue = factBudgetRF == null ? 0 : factBudgetRF.Value,
                FactProgramCooperationID = factProgram == null ? 0 : factProgram.ID,
                FactProgramCooperationValue = factProgram == null ? 0 : factProgram.Value,
                Editable = editable,
                StatusDId = statusD.ID,
                StatusDName = statusD.Name
            };
        }
        
        private void SaveFact(
            IDictionary<string, object> record, 
            FX_Date_YearDayUNV period, 
            D_ExcCosts_CObject obj, 
            D_ExcCosts_AIPMark mark, 
            D_ExcCosts_Finances finance, 
            D_ExcCosts_Audits auditDefault, 
            D_ExcCosts_Contract contractDefault, 
            D_ExcCosts_StatusD statusD, 
            D_ExcCosts_WorkType typeWorkDefault, 
            string paramIdName, 
            string paramValueName, 
            string paramMinning)
        {
            var factSubMOId = CommonService.GetIntFromRecord(record, paramIdName, "{0} (ID)".FormatWith(paramMinning), false);
            var factSubMO = factsAipRepository.FindOne(factSubMOId) ??
                            new F_ExcCosts_AIP
                                {
                                    RefCObject = obj,
                                    RefPeriod = period,
                                    RefAIPMark = mark,
                                    SourceID = extension.DataSource.ID,
                                    RefTypeWork = typeWorkDefault,
                                    RefFinances = finance,
                                    RefAudits = auditDefault,
                                    RefContract = contractDefault,
                                    RefStatusD = statusD
                                };

            factSubMO.RefStatusD = statusD;

            factSubMO.Value = CommonService.GetDecimalFromRecord(record, paramValueName, paramMinning, false);
            factsAipRepository.Save(factSubMO);
        }
    }
}
