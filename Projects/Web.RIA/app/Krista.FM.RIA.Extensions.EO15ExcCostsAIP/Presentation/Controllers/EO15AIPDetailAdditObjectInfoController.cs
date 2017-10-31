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
    public class EO15AIPDetailAdditObjectInfoController : SchemeBoundController
    {
         private readonly IConstructionService constrRepository;
        private readonly ILinqRepository<FX_Date_YearDayUNV> periodRepository;
        private readonly ILinqRepository<D_ExcCosts_CharObj> marksObjectRepository;
        private readonly ILinqRepository<D_ExcCosts_AIPMark> markRepository;
        private readonly int marksObjectCodeGroup = 6;

        public EO15AIPDetailAdditObjectInfoController(
            IConstructionService constrRepository,
            ILinqRepository<FX_Date_YearDayUNV> periodRepository,
            ILinqRepository<D_ExcCosts_AIPMark> markRepository,
            ILinqRepository<D_ExcCosts_CharObj> marksObjectRepository)
        {
            this.constrRepository = constrRepository;
            this.periodRepository = periodRepository;
            this.marksObjectRepository = marksObjectRepository;
            this.markRepository = markRepository;
        }

        public AjaxStoreResult Read(int objectId, int? periodId)
        {
            try
            {
                var curObject = constrRepository.GetOne(objectId);
                if (curObject.StartConstruction == null || curObject.EndConstruction == null || periodId == null)
                {
                    return new AjaxStoreResult
                    {
                        ResponseFormat = StoreResponseFormat.Load,
                        Data = new List<string>(),
                        Total = 0
                    };  
                }

                var data = new List<object>();
                
                var marks = markRepository.FindAll().Where(x => 
                    (x.Code / 100000000 > marksObjectCodeGroup) && 
                    (x.Code / 100000000 < marksObjectCodeGroup + 1))
                    .ToList();
                var values = marksObjectRepository.FindAll()
                    .Where(x => x.RefCObject.ID == objectId && x.RefYearDayUNV.ID == periodId)
                    .ToList();
                foreach (var mark in marks)
                {
                    var markValues = values.FirstOrDefault(x => x.RefAIPMark.ID == mark.ID);
                    data.Add(new
                                 {
                                     MarkId = mark.ID, 
                                     MarkName = mark.Name, 
                                     MarkValue = markValues == null ? String.Empty : markValues.Value,
                                     CObjectId = objectId,
                                     PeriodId = periodId
                                 });
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
                var dataHandler = new StoreDataHandler(HttpContext.Request["data"]);
                var dataSet = JsonDataSetParser.Parse(dataHandler.JsonData);

                if (dataSet.ContainsKey("Deleted"))
                {
                    var table = dataSet["Deleted"];
                    DeleteObjectMarks(table);
                }

                var markWithMaxCode = markRepository.FindAll().Where(x =>
                                                     (x.Code / 100000000 > marksObjectCodeGroup) &&
                                                     (x.Code / 100000000 < marksObjectCodeGroup + 1)).
                        OrderByDescending(x => x.Code).FirstOrDefault();
                /* Для вновь создаваемых показателей в поле «Код (Code)» 
                 * в таблице «Исполнение расходов.Показатели АИП (d.ExcCosts.AIPMark)» 
                 * присваивать значения >56 по порядку (+1 к последнему значению).
                 * */
                var maxMarkCode = markWithMaxCode == null ? 57 : markWithMaxCode.Code;
                var markObjectWithMaxCode = marksObjectRepository.FindAll().OrderByDescending(x => x.Code).FirstOrDefault();
                var maxMarkObjectCode = markObjectWithMaxCode == null ? 1 : markObjectWithMaxCode.Code;

                if (dataSet.ContainsKey("Updated"))
                {
                    var table = dataSet["Updated"];
                    UpdateObjectMarks(table, maxMarkCode, maxMarkObjectCode);
                }

                if (dataSet.ContainsKey("Created"))
                {
                    var table = dataSet["Created"];
                    UpdateObjectMarks(table, maxMarkCode, maxMarkObjectCode);
                }

                return new RestResult
                {
                    Success = true,
                    Message = "Данные сохранены",
                    Data = dataSet.ToString()
                };
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return CommonService.ErrorResult(e.Message);
            }
        }

        /// <summary>
        /// Чтение периодов
        /// </summary>
        /// <returns>список годов с начала по окончание строительства</returns>
        public ActionResult LookupPeriods(int objectId)
        {
            try
            {
                var data = new List<object>();

                var constrObject = constrRepository.GetOne(objectId);

                if (constrObject.EndConstruction != null && constrObject.StartConstruction != null)
                {
                    var year = constrObject.EndConstruction.Value.Year;
                    var startYear = constrObject.StartConstruction.Value.Year;
                    for (var i = year; i >= startYear; i--)
                    {
                        data.Add(new
                                     {
                                         ID = (i * 10000) + 1,
                                         Name = i.ToString()
                                     });
                    }
                }

                return new AjaxStoreResult(data, data.Count);
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        private void UpdateObjectMarks(IEnumerable<Dictionary<string, object>> table, int maxMarkCode, int maxMarkObjectCode)
        {
            foreach (var record in table)
            {
                var markId = CommonService.GetIntFromRecord(record, "MarkId", "Идентификатор показателя", false);
                var mark = markRepository.FindOne(markId);

                // Если показатель новый, создаем запись в классификаторе.
                if (mark == null)
                {
                    mark = new D_ExcCosts_AIPMark
                    {
                        Name = CommonService.GetStringFromRecord(record, "MarkName", "Наименование показателя", false),
                        Code = maxMarkCode + 10000,
                        RowType = 0
                    };
                    maxMarkCode++;
                    markRepository.Save(mark);
                }
                else
                {
                    var newName = CommonService.GetStringFromRecord(record, "MarkName", "Наименование показателя", false);
                    if (!mark.Name.Equals(newName))
                    {
                        mark.Name = newName;
                        markRepository.Save(mark);
                    }
                }

                var objectId = CommonService.GetIntFromRecord(record, "CObjectId", "Идентификатор объекта", true);
                var periodId = CommonService.GetIntFromRecord(record, "PeriodId", "Период", true);
                var markValueObj =
                    marksObjectRepository.FindAll().FirstOrDefault(
                        x =>
                        x.RefAIPMark.ID == mark.ID && x.RefCObject.ID == objectId &&
                        x.RefYearDayUNV.ID == periodId);

                if (markValueObj == null) 
                {
                    markValueObj = new D_ExcCosts_CharObj
                    {
                        RefCObject = constrRepository.GetOne(objectId),
                        RefYearDayUNV = periodRepository.FindOne(periodId),
                        RefAIPMark = markRepository.FindOne(mark.ID),
                        Code = maxMarkObjectCode + 1
                    };
                    maxMarkObjectCode++;
                }

                markValueObj.Value = CommonService.GetStringFromRecord(record, "MarkValue", "Значение показателя", false);
                if (String.IsNullOrEmpty(markValueObj.Value) && markValueObj.ID > 0)
                {
                    marksObjectRepository.Delete(markValueObj);
                }
                else
                {
                    if (!String.IsNullOrEmpty(markValueObj.Value))
                    {
                        marksObjectRepository.Save(markValueObj);
                    }
                }
            }
        }

        private void DeleteObjectMarks(IEnumerable<Dictionary<string, object>> table)
        {
            foreach (var record in table)
            {
                var markId = CommonService.GetIntFromRecord(record, "MarkId", "Идентификатор показателя", false);
                var mark = markRepository.FindOne(markId);

                if (mark != null)
                {
                    var objectId = CommonService.GetIntFromRecord(record, "CObjectId", "Идентификатор объекта", true);
                    var periodId = CommonService.GetIntFromRecord(record, "PeriodId", "Период", true);
                    var markValueObj = marksObjectRepository.FindAll().FirstOrDefault(x =>
                                                                                      x.RefAIPMark.ID == mark.ID &&
                                                                                      x.RefCObject.ID == objectId &&
                                                                                      x.RefYearDayUNV.ID == periodId);

                    if (markValueObj != null)
                    {
                        marksObjectRepository.Delete(markValueObj);
                    }

                    var existsValueObjsOnMark = marksObjectRepository.FindAll().Any(x => x.RefAIPMark.ID == mark.ID);
                    if (!existsValueObjsOnMark)
                    {
                        markRepository.Delete(mark);
                    }
                    else
                    {
                        throw new Exception("Удаление показателя невозможно, поскольку на него записаны данные в базе данных");
                    }
                }
            }
        }
    }
}
