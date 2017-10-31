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
    public class EO15AIPInfoController : SchemeBoundController
    {
        private readonly ILinqRepository<D_ExcCosts_Input> inputRepository;
        private readonly IRepository<FX_Date_YearDayUNV> periodRepository;
        private readonly IConstructionService constructRepository;
        private readonly IRepository<D_ExcCosts_AIPMark> markRepository;
        private readonly IRepository<D_ExcCosts_StatusD> statusDRepository;

        public EO15AIPInfoController(
            ILinqRepository<D_ExcCosts_Input> inputRepository, 
            IRepository<FX_Date_YearDayUNV> periodRepository,
            IConstructionService constructRepository,
            IRepository<D_ExcCosts_AIPMark> markRepository,
            IRepository<D_ExcCosts_StatusD> statusDRepository)
        {
            this.inputRepository = inputRepository;
            this.periodRepository = periodRepository;
            this.constructRepository = constructRepository;
            this.markRepository = markRepository;
            this.statusDRepository = statusDRepository;
        }

        /// <summary>
        /// Чтение данных "Справка объекта строительства".
        /// </summary>
        public AjaxStoreResult Read(int objectId, int[] periodId)
        {
            try
            {
                var data = new List<object>();

                if (periodId == null)
                {
                    throw new Exception("Выберите период");
                }

                var infoAll = inputRepository.FindAll().Where(x => x.RefCObject.ID == objectId).ToList();
                foreach (var period in periodId)
                {
                    if (period > 0)
                    {
                        var year = period / 10000;
                        for (var quarter = 1; quarter <= 4; quarter++)
                        {
                            var curPeriod = (year * 10000) + 9990 + quarter;
                            var infoCur = infoAll.Where(x => x.RefYearDayUNV.ID == curPeriod).ToList();

                            var marks = new List<D_ExcCosts_Input>
                            {
                                infoCur.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkPermissionCapacity),
                                infoCur.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkPermissionGeneralFund),
                                infoCur.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkPermissionGeneralFundAO),
                                infoCur.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkPermissionExploitation),
                                infoCur.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkRetireCapacity),
                                infoCur.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkRedesignCapacity),
                                infoCur.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkPlanStaff),
                                infoCur.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkPlanStaffNew)
                            };

                            var markReasonsNotCom = infoCur.FirstOrDefault(x => x.RefAIPMark.Code == AIPMarks.MarkReasonsNotCommissioning);

                            var markToUse = marks[0] ?? marks[1] ?? marks[2] ?? marks[3] ?? marks[4] ?? marks[5] ?? marks[6] ?? marks[7] ?? markReasonsNotCom;
                            
                            var statusDId = markToUse == null ? (int)AIPStatusD.Edit : markToUse.RefStatusD.ID;
                            var statusD = statusDRepository.Get(statusDId);
                            
                            data.Add(new
                                    {
                                        CObjectId = objectId,
                                        PeriodId = curPeriod,
                                        PeriodName = "{0} квартал {1} года".FormatWith(quarter, year),
                                        Mark11 = marks[0] != null ? marks[0].Value : null,
                                        Mark11Id = marks[0] != null ? marks[0].ID : 0,
                                        Mark12 = marks[1] != null ? marks[1].Value : null,
                                        Mark12Id = marks[1] != null ? marks[1].ID : 0,
                                        Mark13 = marks[2] != null ? marks[2].Value : null,
                                        Mark13Id = marks[2] != null ? marks[2].ID : 0,
                                        Mark14 = marks[3] != null ? marks[3].Value : null,
                                        Mark14Id = marks[3] != null ? marks[3].ID : 0,
                                        Mark15 = marks[4] != null ? marks[4].Value : null,
                                        Mark15Id = marks[4] != null ? marks[4].ID : 0,
                                        Mark16 = marks[5] != null ? marks[5].Value : null,
                                        Mark16Id = marks[5] != null ? marks[5].ID : 0,
                                        Mark17 = marks[6] != null ? marks[6].Value : null,
                                        Mark17Id = marks[6] != null ? marks[6].ID : 0,
                                        Mark18 = marks[7] != null ? marks[7].Value : null,
                                        Mark18Id = marks[7] != null ? marks[7].ID : 0,
                                        MarkReasonsNotComId = markReasonsNotCom != null ? markReasonsNotCom.ID : 0,
                                        MarkReasonsNotCom = markReasonsNotCom != null ? markReasonsNotCom.Value : null,
                                        StatusDId = statusD.ID,
                                        StatusDName = statusD.Name
                                    });
                        }
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
        /// Сохранение изменений в данных "Справка объекта строительства".
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
                    foreach (var record in table)
                    {
                        var periodId = CommonService.GetIntFromRecord(record, "PeriodId", "Период");
                        var period = periodRepository.Get(periodId);

                        var statusDId = CommonService.GetIntFromRecord(record, "StatusDId", "Статус данных");
                        var statusD = statusDRepository.Get(statusDId);

                        if (period == null)
                        {
                            throw new Exception("Выберите период");
                        }

                        var objId = CommonService.GetIntFromRecord(record, "CObjectId", "Объект строительства");
                        var obj = constructRepository.GetOne(objId);

                        var factsId = new List<int>();
                        var facts = new List<D_ExcCosts_Input>();
                        for (var i = 0; i < 8; i++)
                        {
                            factsId.Add(CommonService.GetIntFromRecord(record, "Mark{0}Id".FormatWith(i + 11), "Идентификатор факта", false));
                            var markCode = 800000000 + (10000 * (i + 1));
                            facts.Add(inputRepository.FindOne(factsId[i])
                                ?? new D_ExcCosts_Input
                                       {
                                           RefYearDayUNV = period, 
                                           RefCObject = obj, 
                                           RefAIPMark = markRepository.GetAll().FirstOrDefault(x => x.Code == markCode)
                                       });
                            facts[i].Value = CommonService.GetStringFromRecord(record, "Mark{0}".FormatWith(i + 11), "Значение факта", true);
                            facts[i].RefStatusD = statusD;
                            inputRepository.Save(facts[i]);
                         }

                        var factReasonsNotComId = CommonService.GetIntFromRecord(record, "MarkReasonsNotComId", "Идентификатор факта", false);
                        var fact = inputRepository.FindOne(factReasonsNotComId)
                            ?? new D_ExcCosts_Input
                                   {
                                       RefYearDayUNV = period, 
                                       RefCObject = obj,
                                       RefAIPMark = markRepository.GetAll().FirstOrDefault(x => x.Code == AIPMarks.MarkReasonsNotCommissioning)
                                   };
                        fact.Value = CommonService.GetStringFromRecord(record, "MarkReasonsNotCom", "Значение факта", true);
                        fact.RefStatusD = statusD;
                        inputRepository.Save(fact);
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
    }
}
