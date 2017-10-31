using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Services;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Controllers
{
    public class EO15AIPDetailExpertiseController : SchemeBoundController
    {
        private readonly IRepository<D_ExcCosts_WorkType> workTypeRepository;
        private readonly ILinqRepository<F_ExcCosts_AIP> factsAipRepository;
        private readonly IConstructionService constructRepository;
        private readonly ILinqRepository<D_ExcCosts_AIPMark> markRepository;
        private readonly IEO15ExcCostsAIPExtension extension;
        private readonly IRepository<D_ExcCosts_Audits> auditRepository;
        private readonly IRepository<D_ExcCosts_Contract> contractRepository;
        private readonly IRepository<D_ExcCosts_Finances> financesRepository;
        private readonly IRepository<D_ExcCosts_StatusD> statusDRepository;
        private readonly IRepository<FX_Date_YearDayUNV> periodRepository;

        public EO15AIPDetailExpertiseController(
            IEO15ExcCostsAIPExtension extension,
            ILinqRepository<F_ExcCosts_AIP> factsAipRepository, 
            IRepository<D_ExcCosts_WorkType> workTypeRepository,
            IConstructionService constructRepository,
            ILinqRepository<D_ExcCosts_AIPMark> markRepository,
            IRepository<D_ExcCosts_Audits> auditRepository,
            IRepository<D_ExcCosts_Contract> contractRepository,
            IRepository<D_ExcCosts_Finances> financesRepository,
            IRepository<D_ExcCosts_StatusD> statusDRepository,
            IRepository<FX_Date_YearDayUNV> periodRepository)
        {
            this.extension = extension;
            this.factsAipRepository = factsAipRepository;
            this.workTypeRepository = workTypeRepository;
            this.constructRepository = constructRepository;
            this.markRepository = markRepository;
            this.auditRepository = auditRepository;
            this.contractRepository = contractRepository;
            this.financesRepository = financesRepository;
            this.statusDRepository = statusDRepository;
            this.periodRepository = periodRepository;
        }

        /// <summary>
        /// Чтение данных "Лимит бюджетных ассигнований"
        /// </summary>
        /// <param name="filter">Фильтры по статусам данных</param>
        /// <param name="objectId">иднтификатор объекта строительства</param>
        /// <param name="expertiseId">идентификатор экспертизы</param>
        public AjaxStoreResult Read(bool[] filter, int objectId, int? expertiseId)
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

                var statusDDefault = statusDRepository.Get((int)AIPStatusD.Edit);

                // если период не определен, возвращаем 0 записей
                if (expertiseId == null)
                {
                    expertiseId = -1;
                }

                var expertise = auditRepository.Get((int)expertiseId);
                if (expertise == null || expertiseId < 1)
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
                                                      x.RefPeriod.ID == -1 &&
                                                      x.SourceID == extension.DataSource.ID &&
                                                      x.RefTypeWork.ID == type.ID &&
                                                      x.RefAudits.ID == expertiseId).ToList();
                    var fact2001 = facts.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkSmeta2001);
                    var factCur = facts.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkSmeta);
                    var factToUse = factCur ?? fact2001;
                    var statusD = factToUse == null ? statusDDefault : factToUse.RefStatusD;
                    if (!filters.Contains(statusD.ID))
                    {
                        data.Add(new
                                     {
                                         CObjectId = objectId,
                                         TypeWorkId = type.ID,
                                         TypeWorkName = type.Name,
                                         AuditId = expertiseId,
                                         Fact2001Id = fact2001 == null ? -1 : fact2001.ID,
                                         Fact2001Value = fact2001 == null ? 0 : fact2001.Value,
                                         FactCurId = factCur == null ? -1 : factCur.ID,
                                         FactCurValue = factCur == null ? 0 : factCur.Value,
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
        /// Чтение списка экспертиз
        /// </summary>
        /// <returns>список экспертиз</returns>
        public ActionResult LookupExpertise()
        {
            try
            {
                var data = new List<object>();
                var expertises = auditRepository.GetAll().Where(x => x.ID > 0);
                foreach (var expertise in expertises)
                {
                    data.Add(new
                    {
                        expertise.ID,
                        Name = expertise.Property
                    });
                }

                return new AjaxStoreResult(data, data.Count);
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        /// <summary>
        /// Чтение статусов
        /// </summary>
        /// <returns>список статусов</returns>
        public ActionResult LookupStatusD()
        {
            try
            {
                var data = (from s in statusDRepository.GetAll().Where(x => x.ID > 0)
                            select new
                            {
                                s.ID,
                                s.Name
                            }).ToList();

                return new AjaxStoreResult(data, data.Count);
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [Transaction]
        private void SaveChanges(IEnumerable<Dictionary<string, object>> table)
        {
            var defaultMark = markRepository.FindAll().FirstOrDefault(x => x.Code == AIPMarks.MarkLimit);
            var mark3 = markRepository.FindAll().FirstOrDefault(x => x.Code == AIPMarks.MarkSmeta2001);
            var mark4 = markRepository.FindAll().FirstOrDefault(x => x.Code == AIPMarks.MarkSmeta);
            var contractDefault = contractRepository.Get(-1);
            var periodDefault = periodRepository.Get(-1);
            var financeDefault = financesRepository.Get(-1);
            var statusDDefault = statusDRepository.Get((int)AIPStatusD.Edit);

            foreach (var record in table)
            {
                var typeId = CommonService.GetIntFromRecord(record, "TypeWorkId", "Вид работ");
                var type = workTypeRepository.Get(typeId);
                var objId = CommonService.GetIntFromRecord(record, "CObjectId", "Объект строительства");
                var obj = constructRepository.GetOne(objId);
                var auditId = CommonService.GetIntFromRecord(record, "AuditId", "Экспертиза");
                var audit = auditRepository.Get(auditId);

                var statusDId = CommonService.GetIntFromRecord(record, "StatusDId", "Статус данных");
                var statusD = statusDRepository.Get(statusDId);

                F_ExcCosts_AIP fact2001;
                try
                {
                    // Сметная стоимость по экспертизе в ценах 2001 года 
                    var fact2001Id = CommonService.GetIntFromRecord(record, "Fact2001Id", "Сметная стоимость по экспертизе в ценах 2001 года", false);
                    var value2001 = CommonService.GetDecimalFromRecord(record, "Fact2001Value", "Сметная стоимость по экспертизе в ценах 2001 года");
                    fact2001 = factsAipRepository.FindOne(fact2001Id) ?? CreateDefaultFact(obj, type, defaultMark, contractDefault, periodDefault, financeDefault, statusDDefault);
                    fact2001.RefAIPMark = mark3;
                    fact2001.RefAudits = audit;
                    fact2001.Value = value2001;
                }
                catch (Exception)
                {
                    fact2001 = null;
                }

                // Сметная стоимость по экспертизе в текущих ценах с НДС
                F_ExcCosts_AIP factCur;
                try
                {
                    var factCurId = CommonService.GetIntFromRecord(record, "FactCurId", "Сметная стоимость по экспертизе в текущих ценах с НДС", false);
                    var valueCur = CommonService.GetDecimalFromRecord(record, "FactCurValue", "Сметная стоимость по экспертизе в текущих ценах с НДС");
                    factCur = factsAipRepository.FindOne(factCurId) ?? CreateDefaultFact(obj, type, defaultMark, contractDefault, periodDefault, financeDefault, statusDDefault);
                    factCur.RefAIPMark = mark4;
                    factCur.Value = valueCur;
                    factCur.RefAudits = audit;
                }
                catch (Exception)
                {
                    factCur = null;
                }

                if (fact2001 != null)
                {
                    fact2001.RefStatusD = statusD;
                    factsAipRepository.Save(fact2001);
                }

                if (factCur != null)
                {
                    factCur.RefStatusD = statusD;
                    factsAipRepository.Save(factCur);
                }
            }
        }

        private F_ExcCosts_AIP CreateDefaultFact(D_ExcCosts_CObject obj, D_ExcCosts_WorkType type, D_ExcCosts_AIPMark defaultMark, D_ExcCosts_Contract contractDefault, FX_Date_YearDayUNV periodDefault, D_ExcCosts_Finances financeDefault, D_ExcCosts_StatusD statusDDefault)
        {
            return new F_ExcCosts_AIP
            {
                RefAIPMark = defaultMark,
                RefPeriod = periodDefault,
                RefCObject = obj,
                RefTypeWork = type,
                RefContract = contractDefault,
                RefStatusD = statusDDefault,
                RefFinances = financeDefault,
                SourceID = extension.DataSource.ID,
            };
        }
    }
}
