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
    public class EO15AIPDetailReviewController : SchemeBoundController
    {
        private readonly ILinqRepository<D_ExcCosts_Review> reviewRepository;
        private readonly IConstructionService constructRepository;
        private readonly IRepository<FX_Date_YearDayUNV> quarterRepository;
        private readonly IRepository<D_ExcCosts_StatusD> statusDRepository;

        public EO15AIPDetailReviewController(
            ILinqRepository<D_ExcCosts_Review> reviewRepository, 
            IConstructionService constructRepository,
            IRepository<FX_Date_YearDayUNV> quarterRepository,
            IRepository<D_ExcCosts_StatusD> statusDRepository)
        {
            this.reviewRepository = reviewRepository;
            this.constructRepository = constructRepository;
            this.quarterRepository = quarterRepository;
            this.statusDRepository = statusDRepository;
        }

        /// <summary>
        /// Чтение данных "конъюнктурный обзор".
        /// </summary>
        /// <param name="filter">Фильтры статусов данных.</param>
        /// <param name="objectId">иднтификатор объекта строительства.</param>
        public AjaxStoreResult Read(bool[] filter, int objectId)
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

                var data = (from r in reviewRepository.FindAll().Where(x => 
                    !filters.Contains(x.RefStatusD.ID) &&
                    x.RefCObject1.ID == objectId).OrderBy(x => x.RefYearDayUNV.ID).ToList()
                              select new
                                         {
                                             r.ID,
                                             r.Result,
                                             CObjectId = objectId,
                                             PeriodId = r.RefYearDayUNV.ID,
                                             StatusDId = r.RefStatusD.ID,
                                             StatusDName = r.RefStatusD.Name
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

        /// <summary>
        /// Сохранение изменений в данных "Лимит бюджетных ассигнований".
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

                if (dataSet.ContainsKey("Deleted"))
                {
                    var table = dataSet["Deleted"];
                    foreach (var record in table)
                    {
                        try
                        {
                            var reviewId = CommonService.GetIntFromRecord(record, "ID", "Идентификатор обзора", false);
                            var review = reviewRepository.FindOne(reviewId);
                            if (review != null)
                            {
                                reviewRepository.Delete(review);
                            }
                        }
                        catch (Exception)
                        {
                        }
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

        [Transaction]
        private void SaveChanges(IEnumerable<Dictionary<string, object>> table)
        {
            foreach (var record in table)
            {
                object result;
                if (!record.ContainsKey("Result"))
                {
                    throw new Exception("Параметр 'Конъюнктурный обзор' не должен быть пустым");
                }

                record.TryGetValue("Result", out result);
                var objId = CommonService.GetIntFromRecord(record, "CObjectId", "Объект строительства");
                var obj = constructRepository.GetOne(objId);
                var periodId = CommonService.GetIntFromRecord(record, "PeriodId", "Квартал");
                var period = quarterRepository.Get(periodId);
                var statusDId = CommonService.GetIntFromRecord(record, "StatusDId", "Статус данных");
                var statusD = statusDRepository.Get(statusDId);

                var reviewId = CommonService.GetIntFromRecord(record, "ID", "Идентификатор обзора", false);
                var review = reviewRepository.FindOne(reviewId) ?? new D_ExcCosts_Review
                                                                       {
                                                                           RefCObject1 = obj,
                                                                           RowType = 0,
                                                                           Code = 0,
                                                                           RefYearDayUNV = period
                                                                       };
                review.Result = result.ToString();
                review.RefStatusD = statusD;

                if (reviewRepository.FindAll().Any(x =>
                                                   x.RefYearDayUNV.ID == periodId &&
                                                   x.RefCObject1.ID == objId &&
                                                   x.ID != review.ID))
                {
                    throw new Exception("На {0} данные уже существуют.".FormatWith((periodId % 10) + " квартал " + (periodId / 10000) + " года"));
                }

                reviewRepository.Save(review);
            }
        }
    }
}
