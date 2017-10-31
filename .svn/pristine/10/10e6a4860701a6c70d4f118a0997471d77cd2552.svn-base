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
    public class EO15AIPController : SchemeBoundController
    {
        private readonly IConstructionService constructRepository;
        private readonly ILinqRepository<D_ExcCosts_ListAIP> programs;
        private readonly ILinqRepository<D_ExcCosts_ObjctAIP> objAIPRepository;
        private readonly IRepository<FX_Date_YearDayUNV> periodRepository;

        public EO15AIPController(
            IConstructionService constructRepository,
            ILinqRepository<D_ExcCosts_ListAIP> programs,
            ILinqRepository<D_ExcCosts_ObjctAIP> objAIPRepository,
            IRepository<FX_Date_YearDayUNV> periodRepository)
        {
            this.constructRepository = constructRepository;
            this.programs = programs;
            this.objAIPRepository = objAIPRepository;
            this.periodRepository = periodRepository;
        }

        public AjaxStoreResult ReadAIP()
        {
            try
            {
                var data =
                    (from program in programs.FindAll().Where(x => x.ID > 0).OrderBy(x => x.Name).ToList()
                     select new
                     {
                         AIPId = program.ID,
                         AIPName = program.Name,
                         AIPYearId = program.RefYearDayUNV.ID
                     }).ToList();

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

        public AjaxStoreResult ReadCObjectsAIP(int? aipId)
        {
            try
            {
                var data =
                    (from relation in objAIPRepository.FindAll().Where(x => x.RefListAIP.ID == aipId).OrderBy(x => x.RefCObject.Name).ToList()
                     select new
                     {
                         AIPId = aipId,
                         CObjectId = relation.RefCObject.ID,
                         CObjectName = relation.RefCObject.Name,
                         ClientName = relation.RefCObject.RefClients.ID == -1 ? string.Empty : relation.RefCObject.RefClients.Name,
                         ClientId = relation.RefCObject.RefClients.ID == -1 ? 0 : relation.RefCObject.RefClients.ID
                     }).ToList();

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

        public AjaxStoreResult ReadCObjects()
        {
            try
            {
                // В перечень объектов не попадают объекты со статусом "введен в эксплуатацию" (Code = 2 (operated))
                var data =
                    (from obj in constructRepository.FindAll()
                         .Where(x => x.ID > 0 && x.RefStat.Code != (int)AIPStatus.Operated)
                         .OrderBy(x => x.Name)
                         .ToList()
                            select new
                            {
                                CObjectId = obj.ID,
                                CObjectName = obj.Name,
                                ClientName = obj.RefClients.ID == -1 ? string.Empty : obj.RefClients.Name,
                                ClientId = obj.RefClients.ID == -1 ? 0 : obj.RefClients.ID
                            }).ToList();

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

        [Transaction(RollbackOnModelStateError = true)]
        public ActionResult SaveCObject()
        {
            try
            {
                var dataHandler = new StoreDataHandler(HttpContext.Request["data"]);
                var dataSet = JsonDataSetParser.Parse(dataHandler.JsonData);

                if (dataSet.ContainsKey("Deleted"))
                {
                    var table = dataSet["Deleted"];
                    foreach (var record in table)
                    {
                        try
                        {
                            var objId = CommonService.GetIntFromRecord(record, "CObjectId", "Идентификатор объекта строительства");
                            var aipId = CommonService.GetIntFromRecord(record, "AIPId", "Идентификатор АИП");
                            var objAip = objAIPRepository.FindAll().FirstOrDefault(x => x.RefListAIP.ID == aipId && x.RefCObject.ID == objId);
                            if (objAip != null)
                            {
                                objAIPRepository.Delete(objAip);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                if (dataSet.ContainsKey("Updated"))
                {
                    var table = dataSet["Updated"];
                    foreach (var record in table)
                    {
                        UpdateCreateRelation(record);
                    }
                }

                if (dataSet.ContainsKey("Created"))
                {
                    var table = dataSet["Created"];
                    foreach (var record in table)
                    {
                        UpdateCreateRelation(record);
                    }
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

        [Transaction(RollbackOnModelStateError = true)]
        public ActionResult SaveAIP()
        {
            try
            {
                var dataHandler = new StoreDataHandler(HttpContext.Request["data"]);
                var dataSet = JsonDataSetParser.Parse(dataHandler.JsonData);

                if (dataSet.ContainsKey("Deleted"))
                {
                    var table = dataSet["Deleted"];
                    foreach (var record in table)
                    {
                        try
                        {
                            var aipId = CommonService.GetIntFromRecord(record, "AIPId", "Идентификатор АИП");
                            var aip = programs.FindOne(aipId);
                            if (aip != null)
                            {
                                // Сначала нужно удалить все связи АИП-Объект.
                                var relationsToDelete = objAIPRepository.FindAll().Where(x => x.RefListAIP.ID == aipId);
                                foreach (var relation in relationsToDelete)
                                {
                                    objAIPRepository.Delete(relation);
                                }

                                // Затем удаляется сама программа.
                                programs.Delete(aip);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                if (dataSet.ContainsKey("Updated"))
                {
                    var table = dataSet["Updated"];
                    foreach (var record in table)
                    {
                        UpdateCreateAIP(record);
                    }
                }

                if (dataSet.ContainsKey("Created"))
                {
                    var table = dataSet["Created"];
                    foreach (var record in table)
                    {
                        UpdateCreateAIP(record);
                    }
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

        private void UpdateCreateRelation(Dictionary<string, object> record)
        {
            var objId = CommonService.GetIntFromRecord(record, "CObjectId", "Идентификатор объекта строительства");
            var aipId = CommonService.GetIntFromRecord(record, "AIPId", "Идентификатор АИП");
            var objAip = objAIPRepository.FindAll().FirstOrDefault(x => x.RefListAIP.ID == aipId && x.RefCObject.ID == objId) ??
                         new D_ExcCosts_ObjctAIP
                            {
                                RefCObject = constructRepository.GetOne(objId),
                                RefListAIP = programs.FindOne(aipId)
                            };

            objAIPRepository.Save(objAip);
        }

        private void UpdateCreateAIP(Dictionary<string, object> record)
        {
            var aipId = CommonService.GetIntFromRecord(record, "AIPId", "Идентификатор АИП", false);
            var aipPeriod = CommonService.GetIntFromRecord(record, "AIPYearId", "Период АИП");
            var aipName = CommonService.GetStringFromRecord(record, "AIPName", "Наименование АИП", true);
            var aip = programs.FindOne(aipId) ?? new D_ExcCosts_ListAIP();
            aip.Name = aipName;
            aip.RefYearDayUNV = periodRepository.Get(aipPeriod);
            
            programs.Save(aip);
        }
    }
}
