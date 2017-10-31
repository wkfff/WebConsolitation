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
    public class EO15AIPDetailLimitController : SchemeBoundController
    {
        private readonly IRepository<D_ExcCosts_WorkType> workTypeRepository;
        private readonly ILinqRepository<F_ExcCosts_AIP> factsAipRepository;
        private readonly IConstructionService constructRepository;
        private readonly ILinqRepository<D_ExcCosts_AIPMark> markRepository;
        private readonly IRepository<D_ExcCosts_Audits> auditRepository;
        private readonly IRepository<D_ExcCosts_Contract> contractRepository;
        private readonly ILinqRepository<D_ExcCosts_Finances> financesRepository;
        private readonly IRepository<D_ExcCosts_StatusD> statusDRepository;
        private readonly IEO15ExcCostsAIPExtension extension;
        private readonly IRepository<FX_Date_YearDayUNV> periodRepository;

        public EO15AIPDetailLimitController(
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
        /// Чтение данных "Лимит бюджетных ассигнований"
        /// </summary>
        /// <param name="filter">Фильтр статусов данных</param>
        /// <param name="objectId">иднтификатор объекта строительства</param>
        /// <param name="periodId">идентификатор отчетного периода</param>
        public AjaxStoreResult Read(bool[] filter, int objectId, int? periodId)
        {
            try
            {
                var filters = new List<int>();
                var filtersCnt = filter.Count();
                for (var indexFilter = 0; indexFilter < filtersCnt; indexFilter++)
                {
                    if (!filter[indexFilter])
                    {
                        filters.Add(indexFilter + 1);
                    }
                }

                // если период не определен, возвращаем 0 записей
                if (periodId == null)
                {
                    periodId = -1;
                }

                var period = periodRepository.Get((int)periodId);
                if (period == null || periodId < 1)
                {
                    return new AjaxStoreResult
                    {
                        ResponseFormat = StoreResponseFormat.Load,
                        Data = new List<int>(),
                        Total = 0
                    };
                }

                // для всех типов работ получаем значения фактов для всех источников
                var data = new List<object>();
                var typeWorks = workTypeRepository.GetAll().Where(x => x.ID > 0);
                foreach (var type in typeWorks)
                {
                    var facts = factsAipRepository.FindAll().Where(x =>
                                                      x.RefCObject.ID == objectId &&
                                                      x.RefPeriod.ID == periodId &&
                                                      x.SourceID == extension.DataSource.ID &&
                                                      x.RefAIPMark.Code == AIPMarks.MarkLimit &&
                                                      x.RefTypeWork.ID == type.ID).ToList();

                    var factAO = facts.FirstOrDefault(x => x.RefFinances.Code == AIPFinances.FinanceBudgetInvestAO);
                    var factSubMO = facts.FirstOrDefault(x => x.RefFinances.Code == AIPFinances.FinanceSubMO);
                    var factMO = facts.FirstOrDefault(x => x.RefFinances.Code == AIPFinances.FinanceBudgetMO);
                    var factOther = facts.FirstOrDefault(x => x.RefFinances.Code == AIPFinances.FinanceOther);
                    var factBudgetRF = facts.FirstOrDefault(x => x.RefFinances.Code == AIPFinances.FinanceBudgetRF);
                    var factProgram = facts.FirstOrDefault(x => x.RefFinances.Code == AIPFinances.FinanceCooperation);

                    var factToUse = factAO ?? factSubMO ?? factMO ?? factOther ?? factBudgetRF ?? factProgram;
                    var statusDId = factToUse == null ? (int)AIPStatusD.Edit : factToUse.RefStatusD.ID;
                    if (!filters.Contains(statusDId))
                    {
                        var statusD = statusDRepository.Get(statusDId);
                        data.Add(new
                                     {
                                         CObjectId = objectId,
                                         TypeWorkId = type.ID,
                                         PeriodId = periodId,
                                         TypeWorkName = type.Name,
                                         FactAOID = factAO == null ? -1 : factAO.ID,
                                         FactAOValue = factAO == null ? 0 : factAO.Value,
                                         FactSubMOID = factSubMO == null ? -1 : factSubMO.ID,
                                         FactSubMOValue = factSubMO == null ? 0 : factSubMO.Value,
                                         FactMOID = factMO == null ? -1 : factMO.ID,
                                         FactMOValue = factMO == null ? 0 : factMO.Value,
                                         FactOtherID = factOther == null ? -1 : factOther.ID,
                                         FactOtherValue = factOther == null ? 0 : factOther.Value,
                                         FactAll = (factAO == null ? 0 : factAO.Value) +
                                                   (factSubMO == null ? 0 : factSubMO.Value) +
                                                   (factMO == null ? 0 : factMO.Value) +
                                                   (factOther == null ? 0 : factOther.Value),
                                         FactBudgetRFID = factBudgetRF == null ? 0 : factBudgetRF.ID,
                                         FactBudgetRFValue = factBudgetRF == null ? 0 : factBudgetRF.Value,
                                         FactProgramCooperationID = factProgram == null ? 0 : factProgram.ID,
                                         FactProgramCooperationValue = factProgram == null ? 0 : factProgram.Value,
                                         StatusDId = statusD.ID,
                                         StatusDName = statusD.Name
                                     });
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
        /// Сохранение изменений в данных "Лимит бюджетных ассигнований"
        /// </summary>
        [Transaction]
        public ActionResult Save()
        {
            try
            {
                var dataHandler = new StoreDataHandler(HttpContext.Request["data"]);
                var dataSet = JsonDataSetParser.Parse(dataHandler.JsonData);
                if (dataSet.ContainsKey("Updated"))
                {
                    var table = dataSet["Updated"];
                    SaveChanges(table);
                }

                if (dataSet.ContainsKey("Created"))
                {
                    var table = dataSet["Created"];
                    SaveChanges(table);
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

        /// <summary>
        /// Чтение периодов
        /// </summary>
        /// <returns>список годов с начала строительства по текущий год + 1</returns>
        public ActionResult LookupPeriods(int objId)
        {
            try
            {
                var obj = constructRepository.GetOne(objId);
                if (obj == null)
                {
                    throw new Exception("Объект строительства с id={0} не найден".FormatWith(objId));
                }

                if (obj.StartConstruction == null)
                {
                    throw new Exception("У объекта строительства с id={0} не определена дата начала строительства".FormatWith(objId));
                }

                if (obj.EndConstruction == null)
                {
                    throw new Exception("У объекта строительства с id={0} не определена дата завершения строительства".FormatWith(objId));
                }

                var nextYear = DateTime.Today.Year + 1;
                var start = Math.Min(obj.StartConstruction.Value.Year, nextYear);
                var end = Math.Min(obj.EndConstruction.Value.Year, nextYear);

                var data = new List<object>();
                for (var i = end; i > start - 1; i--)
                {
                    data.Add(new
                    {
                        ID = (i * 10000) + 1,
                        Name = "{0} год".FormatWith(i)
                    });
                }

                return new AjaxStoreResult(data, data.Count);
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        private void SaveChanges(IEnumerable<Dictionary<string, object>> table)
        {
            var finances = financesRepository.FindAll();
            var financeBudgetInvestAO = finances.FirstOrDefault(x => x.Code == AIPFinances.FinanceBudgetInvestAO);
            var financeSubMO = finances.FirstOrDefault(x => x.Code == AIPFinances.FinanceSubMO);
            var financeBudgetMO = finances.FirstOrDefault(x => x.Code == AIPFinances.FinanceBudgetMO);
            var financeOther = finances.FirstOrDefault(x => x.Code == AIPFinances.FinanceOther);
            var financeCooperation = finances.FirstOrDefault(x => x.Code == AIPFinances.FinanceCooperation);
            var financeBudgetRF = finances.FirstOrDefault(x => x.Code == AIPFinances.FinanceBudgetRF);

            var defaultMark = markRepository.FindAll().FirstOrDefault(x => x.Code == AIPMarks.MarkLimit);
            var auditDefault = auditRepository.Get(-1);
            var contractDefault = contractRepository.Get(-1);
           
            foreach (var record in table)
            {
                var periodId = CommonService.GetIntFromRecord(record, "PeriodId", "Период");
                var period = periodRepository.Get(periodId);
                var typeId = CommonService.GetIntFromRecord(record, "TypeWorkId", "Вид работ");
                var type = workTypeRepository.Get(typeId);
                var objId = CommonService.GetIntFromRecord(record, "CObjectId", "Объект строительства");
                var obj = constructRepository.GetOne(objId);
                var statusDId = CommonService.GetIntFromRecord(record, "StatusDId", "Статус данных");
                var statusD = statusDRepository.Get(statusDId);

                try
                {
                    // бюджетные инвестиции автономного округа
                    var factAOId = CommonService.GetIntFromRecord(record, "FactAOID", "бюджетные инвестиции автономного округа", false);
                    var valueAO = CommonService.GetDecimalFromRecord(record, "FactAOValue", "бюджетные инвестиции автономного округа");
                    var factAO = factsAipRepository.FindOne(factAOId) 
                        ?? CreateDefaultFact(obj, type, period, defaultMark, auditDefault, contractDefault, statusD);
                    factAO.RefFinances = financeBudgetInvestAO;
                    factAO.RefStatusD = statusD;
                    factAO.Value = valueAO;
                    factsAipRepository.Save(factAO);
                }
                catch (Exception)
                {
                }

                try
                {
                    // субсидии муниципальным образованиям
                    var factSubMOId = CommonService.GetIntFromRecord(record, "FactSubMOID", "субсидии муниципальным образованиям", false);
                    var valueSubMO = CommonService.GetDecimalFromRecord(record, "FactSubMOValue", "субсидии муниципальным образованиям");
                    var factSubMO = factsAipRepository.FindOne(factSubMOId)
                        ?? CreateDefaultFact(obj, type, period, defaultMark, auditDefault, contractDefault, statusD);
                    factSubMO.RefFinances = financeSubMO;
                    factSubMO.RefStatusD = statusD;
                    factSubMO.Value = valueSubMO;
                    factsAipRepository.Save(factSubMO);
                }
                catch (Exception)
                {
                }

                try
                {
                    // бюджетов муниципальных образований
                    var factMoidId = CommonService.GetIntFromRecord(record, "FactMOID", "средства бюджетов муниципальных образований", false);
                    var valueMO = CommonService.GetDecimalFromRecord(record, "FactMOValue", "средства бюджетов муниципальных образований");
                    var factMO = factsAipRepository.FindOne(factMoidId)
                        ?? CreateDefaultFact(obj, type, period, defaultMark, auditDefault, contractDefault, statusD);
                    factMO.RefFinances = financeBudgetMO;
                    factMO.RefStatusD = statusD;
                    factMO.Value = valueMO;
                    factsAipRepository.Save(factMO);
                }
                catch (Exception)
                {
                }

                try
                {
                    // других источников
                    var factOtherID = CommonService.GetIntFromRecord(record, "FactOtherID", "средства других источников", false);
                    var valueOther = CommonService.GetDecimalFromRecord(record, "FactOtherValue", "средства других источников");
                    var factOther = factsAipRepository.FindOne(factOtherID)
                        ?? CreateDefaultFact(obj, type, period, defaultMark, auditDefault, contractDefault, statusD);
                    factOther.RefFinances = financeOther;
                    factOther.RefStatusD = statusD;
                    factOther.Value = valueOther;
                    factsAipRepository.Save(factOther);
                }
                catch (Exception)
                {
                }

                try
                {
                    // бюджет РФ
                    var factBudgetRFId = CommonService.GetIntFromRecord(record, "FactBudgetRFID", "бюджет РФ", false);
                    var factBudgetRFValue = CommonService.GetDecimalFromRecord(record, "FactBudgetRFValue", "бюджет РФ");
                    var factBudgetRF = factsAipRepository.FindOne(factBudgetRFId)
                        ?? CreateDefaultFact(obj, type, period, defaultMark, auditDefault, contractDefault, statusD);
                    factBudgetRF.RefFinances = financeBudgetRF;
                    factBudgetRF.RefStatusD = statusD;
                    factBudgetRF.Value = factBudgetRFValue;
                    factsAipRepository.Save(factBudgetRF);
                }
                catch (Exception)
                {
                }

                try
                {
                    // программа Сотрудничество
                    var factProgramCooperationID = CommonService.GetIntFromRecord(record, "FactProgramCooperationID", "программа Сотрудничество", false);
                    var factProgramCooperationValue = CommonService.GetDecimalFromRecord(record, "FactProgramCooperationValue", "программа Сотрудничество");
                    var factProgramCooperation = factsAipRepository.FindOne(factProgramCooperationID)
                        ?? CreateDefaultFact(obj, type, period, defaultMark, auditDefault, contractDefault, statusD);
                    factProgramCooperation.RefFinances = financeCooperation;
                    factProgramCooperation.RefStatusD = statusD;
                    factProgramCooperation.Value = factProgramCooperationValue;
                    factsAipRepository.Save(factProgramCooperation);
                }
                catch (Exception)
                {
                }
            }
        }

        private F_ExcCosts_AIP CreateDefaultFact(
            D_ExcCosts_CObject obj, 
            D_ExcCosts_WorkType type, 
            FX_Date_YearDayUNV period, 
            D_ExcCosts_AIPMark defaultMark, 
            D_ExcCosts_Audits auditDefault, 
            D_ExcCosts_Contract contractDefault, 
            D_ExcCosts_StatusD statusD)
        {
            return new F_ExcCosts_AIP
            {
                RefAIPMark = defaultMark,
                RefAudits = auditDefault,
                RefCObject = obj,
                RefTypeWork = type,
                RefContract = contractDefault,
                RefStatusD = statusD,
                SourceID = extension.DataSource.ID,
                RefPeriod = period
            };
        }
    }
}
