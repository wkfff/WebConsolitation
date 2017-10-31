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
    public class EO15AIPDetailPlanController : SchemeBoundController 
    {
        private readonly IConstructionService constrRepository;
        private readonly ILinqRepository<F_ExcCosts_AIP> factsAipRepository;
        private readonly IRepository<D_ExcCosts_WorkType> workTypeRepository;
        private readonly ILinqRepository<FX_Date_YearDayUNV> periodRepository;
        private readonly ILinqRepository<D_ExcCosts_AIPMark> markRepository;
        private readonly IRepository<D_ExcCosts_Audits> auditRepository;
        private readonly IRepository<D_ExcCosts_Contract> contractRepository;
        private readonly IRepository<D_ExcCosts_Finances> financesRepository;
        private readonly IRepository<D_ExcCosts_StatusD> statusDRepository;
        private readonly IEO15ExcCostsAIPExtension extension;

        public EO15AIPDetailPlanController(
            IEO15ExcCostsAIPExtension extension,
            IConstructionService constrRepository,
            ILinqRepository<F_ExcCosts_AIP> factsAipRepository,
            IRepository<D_ExcCosts_WorkType> workTypeRepository,
            ILinqRepository<FX_Date_YearDayUNV> periodRepository,
            ILinqRepository<D_ExcCosts_AIPMark> markRepository,
            IRepository<D_ExcCosts_Audits> auditRepository,
            IRepository<D_ExcCosts_Contract> contractRepository,
            IRepository<D_ExcCosts_Finances> financesRepository,
            IRepository<D_ExcCosts_StatusD> statusDRepository)
        {
            this.constrRepository = constrRepository;
            this.factsAipRepository = factsAipRepository;
            this.workTypeRepository = workTypeRepository;
            this.periodRepository = periodRepository;
            this.markRepository = markRepository;
            this.auditRepository = auditRepository;
            this.contractRepository = contractRepository;
            this.financesRepository = financesRepository;
            this.statusDRepository = statusDRepository;
            this.extension = extension;
        }

        public AjaxStoreResult Read(int objectId)
        {
            try
            {
                var curObject = constrRepository.GetOne(objectId);
                if (curObject.StartConstruction == null || curObject.EndConstruction == null)
                {
                    return new AjaxStoreResult
                    {
                        ResponseFormat = StoreResponseFormat.Load,
                        Data = new List<string>(),
                        Total = 0
                    };  
                }

                var start = curObject.StartConstruction.Value.Year;
                var end = curObject.EndConstruction.Value.Year;
                var facts = factsAipRepository.FindAll().Where(x => 
                    x.RefCObject.ID == objectId &&
                    x.RefPeriod.ID >= (start * 10000) + 1 &&
                    x.RefPeriod.ID <= (end * 10000) + 1 && 
                    x.SourceID == extension.DataSource.ID &&
                    x.RefTypeWork.ID == -1 &&
                    x.RefAIPMark.Code == AIPMarks.MarkPlan).ToList();

                var data = new List<Dictionary<string, object>>();

                // Выбираем источники финансирования, на которые есть записанные факты:
                var sourcesFinance = facts.Select(x => x.RefFinances.ID).Distinct().ToList();

                var statusDDefault = statusDRepository.Get((int)AIPStatusD.Edit);

                // Для каждого источника финансирования формируем запись.
                foreach (var sourceFinance in sourcesFinance)
                {
                    var factsForSourceFinance = facts.Where(x => x.RefFinances.ID == sourceFinance);
                    var factToUse = factsForSourceFinance.FirstOrDefault();
                    var statusD = factToUse == null ? statusDDefault : factToUse.RefStatusD;
                    var elem = new Dictionary<string, object> { { "CObjectId", objectId } };
                    D_ExcCosts_Finances finance = null;
                    for (var i = start; i <= end; i++)
                    {
                        var fact = factsForSourceFinance.FirstOrDefault(x => x.RefPeriod != null && x.RefPeriod.ID == (i * 10000) + 1);
                        elem.Add("Year{0}".FormatWith(i), fact == null ? 0 : fact.Value);
                        elem.Add("Fact{0}Id".FormatWith(i), fact == null ? -1 : fact.ID);
                        if (fact != null && fact.RefFinances != null)
                        {
                            finance = fact.RefFinances;
                        }
                    }

                    if (finance != null)
                    {
                        elem.Add("SourceFinanceId", finance.ID);
                        elem.Add("SourceFinance", finance.Name);
                    }

                    elem.Add("StatusDId", statusD.ID);
                    elem.Add("StatusDName", statusD.Name);

                    data.Add(elem);
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

        [Transaction]
        public ActionResult Save()
        {
            try
            {
                var markDefault = markRepository.FindAll().FirstOrDefault(x => x.Code == AIPMarks.MarkPlan);
                var auditDefault = auditRepository.Get(-1);
                var contractDefault = contractRepository.Get(-1);
                var typeDefault = workTypeRepository.Get(-1);
                
                var dataHandler = new StoreDataHandler(HttpContext.Request["data"]);
                var dataSet = JsonDataSetParser.Parse(dataHandler.JsonData);

                if (dataSet.ContainsKey("Deleted"))
                {
                    var table = dataSet["Deleted"];
                    foreach (var record in table)
                    {
                        var objId = CommonService.GetIntFromRecord(record, "CObjectId", "Идетификатор объекта");
                        var sourceFinanceId = CommonService.GetIntFromRecord(record, "SourceFinanceId", string.Empty, false);
                        var facts = factsAipRepository.FindAll().Where(
                                x => x.RefFinances.ID == sourceFinanceId && x.RefCObject.ID == objId);
                        foreach (var fact in facts)
                        {
                            factsAipRepository.Delete(fact);
                        }
                    }
                }

                if (dataSet.ContainsKey("Updated"))
                {
                    var table = dataSet["Updated"];
                    foreach (var record in table)
                    {
                        var statusDId = CommonService.GetIntFromRecord(record, "StatusDId", "Статус данных");
                        var statusD = statusDRepository.Get(statusDId);

                        var objId = CommonService.GetIntFromRecord(record, "CObjectId", "Идетификатор объекта");
                        var curObject = constrRepository.GetOne(objId);
                        if (curObject.StartConstruction == null || curObject.EndConstruction == null)
                        {
                             return CommonService.ErrorResult("Дата начала и/или окончания строительства не определена");
                        }

                        var sourceFinanceId = CommonService.GetIntFromRecord(record, "SourceFinanceId", string.Empty, false);
                        var finance = financesRepository.Get(sourceFinanceId);

                        var start = curObject.StartConstruction.Value.Year;
                        var end = curObject.EndConstruction.Value.Year;
                        
                        // проверить, есть ли другие записи по этому истточнику !!! другие!!!
                        var factsForSourceFinance = factsAipRepository.FindAll()
                            .Where(x => x.RefFinances.ID == sourceFinanceId && x.RefCObject.ID == curObject.ID);
                        if (factsForSourceFinance.Count() > 0)
                        {
                            for (var period = start; period <= end; period++)
                            {
                                var factId = CommonService.GetIntFromRecord(record, "Fact{0}Id".FormatWith(period), string.Empty, false);
                                if (
                                    factsForSourceFinance.Any(x => x.RefPeriod.ID == (period * 10000) + 1 && x.ID != factId))
                                {
                                    throw new Exception("Данные на источник '{0}' уже записаны".FormatWith(finance.Name));
                                }
                            }
                        }
                        
                        // если нет, сохранить данные
                        for (var period = start; period <= end; period++)
                        {
                            var factId = CommonService.GetIntFromRecord(record, "Fact{0}Id".FormatWith(period), string.Empty, false);
                            if (factId > 0)
                            {
                                var fact = factsAipRepository.FindOne(factId);
                                if (fact != null)
                                {
                                    try
                                    {
                                        var value = CommonService.GetDecimalNullFromRecord(
                                            record,
                                            "Year{0}".FormatWith(period),
                                            string.Empty, 
                                            true);

                                        if (value != null)
                                        {
                                            fact.Value = (decimal)value;
                                            fact.RefStatusD = statusD;
                                            fact.RefFinances = finance;
                                            factsAipRepository.Save(fact);
                                        }
                                        else
                                        {
                                            factsAipRepository.Delete(fact);
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                            else
                            {
                                CreateFact(record, objId, (period * 10000) + 1, markDefault, auditDefault, finance, contractDefault, statusD, typeDefault);
                            }
                       }
                    }
                }

                if (dataSet.ContainsKey("Created"))
                {
                    var table = dataSet["Created"];
                    foreach (var record in table)
                    {
                        var id = CommonService.GetIntFromRecord(record, "CObjectId", "Идетификатор объекта");
                        var curObject = constrRepository.GetOne(id);
                        if (curObject.StartConstruction == null || curObject.EndConstruction == null)
                        {
                             return CommonService.ErrorResult("Дата начала и/или окончания строительства не определена");
                        }

                        var sourceFinanceId = CommonService.GetIntFromRecord(record, "SourceFinanceId", string.Empty, false);
                        var finance = financesRepository.Get(sourceFinanceId);

                        if (factsAipRepository.FindAll().Any(x => x.RefFinances.ID == sourceFinanceId && x.RefCObject.ID == curObject.ID))
                        {
                            throw new Exception("Данные на источник '{0}' уже записаны".FormatWith(finance.Name));    
                        }

                        var statusDId = CommonService.GetIntFromRecord(record, "StatusDId", "Статус данных");
                        var statusD = statusDRepository.Get(statusDId);

                        var start = curObject.StartConstruction.Value.Year;
                        var end = curObject.EndConstruction.Value.Year;
                        for (var period = start; period <= end; period++)
                        {
                            CreateFact(record, id, (period * 10000) + 1, markDefault, auditDefault, finance, contractDefault, statusD, typeDefault);
                        }
                    }
                }

                return new RestResult
                {
                    Success = true,
                    Message = "Объекты строительства сохранены",
                    Data = dataSet.ToString()
                };
            }
            catch (Exception e)
            {
                return CommonService.ErrorResult(e.Message);
            }
        }

        private void CreateFact(Dictionary<string, object> record, int objId, int periodId, D_ExcCosts_AIPMark markDefault, D_ExcCosts_Audits auditDefault, D_ExcCosts_Finances financesDefault, D_ExcCosts_Contract contractDefault, D_ExcCosts_StatusD statusDDefault, D_ExcCosts_WorkType typeDefault)
        {
            var fact = new F_ExcCosts_AIP
                           {
                               RefAIPMark = markDefault,
                               RefAudits = auditDefault,
                               RefCObject = constrRepository.GetOne(objId),
                               RefContract = contractDefault,
                               RefFinances = financesDefault,
                               RefStatusD = statusDDefault,
                               SourceID = extension.DataSource.ID,
                               RefTypeWork = typeDefault
                           };
            try
            {
                fact.Value = CommonService.GetDecimalFromRecord(
                    record, 
                    "Year{0}".FormatWith(periodId / 10000),
                    string.Empty, 
                    true);

                SetPeriod(fact, periodId);
                factsAipRepository.Save(fact);
            }
            catch (Exception)
            {
            }
        }

        private void SetPeriod(F_ExcCosts_AIP fact, int periodId)
        {
            var period = periodRepository.FindOne(periodId);
            if (period == null)
            {
                throw new Exception("Для {0} года не создан период".FormatWith(periodId / 10000));
            }

            fact.RefPeriod = period;
        }

        public class EqualityComparerFactByWorkType : IEqualityComparer<F_ExcCosts_AIP>
        {
            public bool Equals(F_ExcCosts_AIP x, F_ExcCosts_AIP y)
            {
                return x.RefTypeWork.ID == y.RefTypeWork.ID;
            }

            public int GetHashCode(F_ExcCosts_AIP obj)
            {
                return base.GetHashCode();
            }
        }
    }
}
