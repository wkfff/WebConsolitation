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
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Views;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Services;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Controllers
{
    public class EO15AIPRegisterController : SchemeBoundController
    {
        private readonly ILinqRepository<D_ExcCosts_ListPrg> programRepository;
        private readonly IClientService clientRepository;
        private readonly ILinqRepository<F_ExcCosts_AIP> aipRepository;
        private readonly ILinqRepository<D_Territory_RF> regionRepository;
        private readonly IRepository<D_ExcCosts_Status> statusRepository;
        private readonly IRepository<D_ExcCosts_StatusD> statusDRepository;
        private readonly IEO15ExcCostsAIPExtension extension;
        private readonly IConstructionService constructRepository;
        private readonly IRepository<FX_Date_YearDayUNV> periodRepository;
        private readonly ILinqRepository<D_ExcCosts_CharObj> constrMarksRepository;

        public EO15AIPRegisterController(
            IEO15ExcCostsAIPExtension extension,
            IConstructionService constructRepository,
            ILinqRepository<D_ExcCosts_ListPrg> programRepository,
            IClientService clientRepository,
            ILinqRepository<D_Territory_RF> regionRepository,
            ILinqRepository<F_ExcCosts_AIP> aipRepository,
            IRepository<D_ExcCosts_Status> statusRepository,
            IRepository<D_ExcCosts_StatusD> statusDRepository,
            IRepository<FX_Date_YearDayUNV> periodRepository,
            ILinqRepository<D_ExcCosts_CharObj> constrMarksRepository)
        {
            this.extension = extension;
            this.constructRepository = constructRepository;
            this.programRepository = programRepository;
            this.clientRepository = clientRepository;
            this.regionRepository = regionRepository;
            this.aipRepository = aipRepository;
            this.statusRepository = statusRepository;
            this.statusDRepository = statusDRepository;
            this.periodRepository = periodRepository;
            this.constrMarksRepository = constrMarksRepository;
        }

        /// <summary>
        /// Чтение данных реестра объектов
        /// </summary>
        /// <returns>Список объектов строительства</returns>
        public AjaxStoreResult Read(bool[] filter, int? year, int? clientId, int? programId, int? regionId)
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

                var data = (from obj in constructRepository.GetByFilter(
                                filters,
                                year == -1 ? null : year, 
                                clientId == -1 ? null : clientId, 
                                programId == -1 ? null : programId, 
                                regionId == -1 ? null : regionId).Where(x => x.ID > 0).ToList()
                            select new
                                       {
                                           obj.ID,
                                           obj.Name,
                                           ClientName = obj.RefClients.ID == -1 ? string.Empty : obj.RefClients.Name,
                                           ProgramName = obj.RefListPrg.ID == -1 ? string.Empty : obj.RefListPrg.Name,
                                           RegionName = obj.RefTerritory.ShortName,
                                           RegionId = obj.RefTerritory.ID,
                                           StateName = obj.RefStat.ID == -1 ? string.Empty : obj.RefStat.Name,
                                           StateId = obj.RefStat.ID,
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
                    foreach (var record in table)
                    {
                        object idObj;
                        record.TryGetValue("ID", out idObj);
                        if (idObj != null)
                        {
                            var id = Int32.Parse(idObj.ToString());
                            if (id > 0)
                            {
                                // TODO удалить зависимые записи
                                constructRepository.Delete(constructRepository.GetOne(id));
                            }
                        }
                    }
                }

                if (dataSet.ContainsKey("Updated"))
                {
                    var table = dataSet["Updated"];
                    foreach (var record in table)
                    {
                        object idObj;
                        record.TryGetValue("ID", out idObj);
                        if (idObj != null)
                        {
                            var id = Int32.Parse(idObj.ToString());
                            if (id > 0)
                            {
                                var obj = constructRepository.GetOne(id);
                                object stateId;
                                record.TryGetValue("StateId", out stateId);
                                var stateIdValue = Int32.Parse(stateId.ToString());
                                if (stateIdValue > 0)
                                {
                                    obj.RefStat = statusRepository.Get(stateIdValue);
                                    constructRepository.Save(obj);
                                }
                            }
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
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        /// <summary>
        /// Чтение периодов
        /// </summary>
        /// <returns>список годов с 2010 по текущий</returns>
        public ActionResult LookupPeriods()
        {
            try
            {
                var data = new List<object>
                               {
                                   new
                                       {
                                           ID = -1,
                                           Name = "Значение не указано"
                                       }
                               };

                var year = DateTime.Today.Year;
                for (var i = year; i > 2009; i--)
                {
                    data.Add(new
                                 {
                                     ID = i,
                                     Name = i.ToString()
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
        /// Чтение кварталов
        /// </summary>
        /// <returns>список кварталов</returns>
        public ActionResult LookupQuarterPeriods(int yearFrom, int yearTo)
        {
            try
            {
                var data = new List<object>();

                var values = periodRepository.GetAll().Where(x => 
                    (x.ID % 10000 <= 9994) && 
                    (x.ID % 10000 >= 9991) &&
                    (x.ID / 10000 >= yearFrom) &&
                    (x.ID / 10000 <= yearTo)).OrderBy(x => x.ID);
                foreach (var quarter in values)
                {
                    data.Add(new
                    {
                        quarter.ID,
                        Name = "{0} квартал {1} года".FormatWith(quarter.ID % 10, quarter.ID / 10000)
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
        /// Чтение годов
        /// </summary>
        /// <returns>список годов</returns>
        public ActionResult LookupYearPeriods(int yearFrom, int yearTo)
        {
            try
            {
                var data = new List<object>();

                var values = periodRepository.GetAll().Where(x => 
                    (x.ID % 10000 == 1) && 
                    (x.ID / 10000 >= yearFrom) &&
                    (x.ID / 10000 <= yearTo)).OrderBy(x => x.ID);
                foreach (var quarter in values)
                {
                    data.Add(new
                                 {
                                     quarter.ID,
                                     Name = "{0} год".FormatWith(quarter.ID / 10000)
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
        public ActionResult LookupStatus()
        {
            try
            {
                var data = (from s in statusRepository.GetAll().Where(x => x.ID > 0)
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

        /// <summary>
        /// Чтение целевых программ
        /// </summary>
        /// <returns>список целевых программ</returns>
        public ActionResult LookupPrograms()
        {
            try
            {
                var data = (from p in programRepository.FindAll()
                            select new
                                       {
                                           p.ID,
                                           p.Name
                                       }).ToList();

                data.Insert(0, new { ID = -1, Name = "Не фильтровать" });
                    
                return new AjaxStoreResult(data, data.Count);
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        /// <summary>
        /// Чтение списка заказчиков
        /// </summary>
        /// <returns>список заказчиков</returns>
        public ActionResult LookupClients()
        {
            try
            {
                var data = (from p in clientRepository.GetAll()
                            select new
                            {
                                p.ID,
                                p.Name
                            }).ToList();

                data.Insert(0, new { ID = -1, Name = "Не фильтровать" });

                return new AjaxStoreResult(data, data.Count);
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        /// <summary>
        /// Чтение списка МО
        /// </summary>
        /// <returns>список территорий (МО)</returns>
        public ActionResult LookupRegions()
        {
            try
            {
                int parentTerrId = 0;
                long oktmo;
                var oktmoStr = extension.OKTMO;
                oktmoStr = oktmoStr.Replace(" ", string.Empty);
                if (long.TryParse(oktmoStr, out oktmo))
                {
                    var regions = regionRepository.FindAll().Where(x => x.Code == oktmo).ToList();
                    if (regions.Count > 1)
                    {
                        const int SubjectType = 3;
                        regions = regions.Where(x => x.RefTerritorialPartType.ID == SubjectType).ToList();
                    }

                    var region = regions.FirstOrDefault();
                    if (region == null)
                    {
                        throw new Exception("Не найдена территория с ОКТМО (Code) {0}".FormatWith(oktmo));
                    }

                    parentTerrId = region.ID;
                }
               
                var data = (from p in regionRepository.FindAll()
                                .Where(x => x.ParentID == parentTerrId)
                                .OrderBy(x => x.Code)
                            select new
                            {
                                p.ID,
                                p.Name
                            }).ToList();

                data.Insert(0, new { ID = -1, Name = "Не фильтровать" });

                return new AjaxStoreResult(data, data.Count);
            }
            catch (Exception e)
            {
                return new AjaxStoreExtraResult(new List<int>(), 0, e.Message);
            }
        }

        /// <summary>
        /// Проверка можно ли удалять объект строительства
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <returns>Признак, можно ли данные удалять</returns>
        public AjaxStoreResult CheckCanRemove(int id)
        {
            // которая находится в состоянии на оценке или дальше. За предыдущий период период
            var result = new AjaxStoreResult { ResponseFormat = StoreResponseFormat.Save };

            // проверяем, есть ли записи, на которые записаны данные 
            // в таблице фактов (ЭО_Сбор исполнения расходов_АИП (f.ExcCosts.AIP)). 
            result.SaveResponse.Success = aipRepository.FindAll().FirstOrDefault(x => x.RefCObject.ID == id) == null;
            
            return result;
        }

        /// <summary>
        /// Обработчик на открытие формы данных  и детализации объекта
        /// </summary>
        /// <returns>Представление с формой данных  и детализации объекта</returns>
        public ActionResult ShowCObjectCard(int objId)
        {
            var constrObjectCardView = new CObjectCardView(extension, constructRepository, statusRepository, statusDRepository, constrMarksRepository, objId);

            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", constrObjectCardView);
        }

        /// <summary>
        /// Обработчик на открытие формы финансирования
        /// </summary>
        /// <returns>Представление с формой финансирования объекта</returns>
        public ActionResult ShowFinance(int objId)
        {
            var financeView = new FinanceView(extension, constructRepository, objId);

            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", financeView);
        }

        /// <summary>
        /// Обработчик на открытие формы справки
        /// </summary>
        /// <returns>Представление с формой справки объекта</returns>
        public ActionResult ShowInfo(int objId)
        {
            var infoView = new InfoView(extension, constructRepository, objId);

            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", infoView);
        }
    }
}
