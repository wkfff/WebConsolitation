using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.RIA.Extensions.FO41.Presentation.Models;

namespace Krista.FM.RIA.Extensions.FO41.Services
{
    public class AppFromOGVService : NHibernateLinqRepository<D_Application_FromOGV>, IAppFromOGVService
    {
        public IQueryable GetQueryAll()
        {
            return from r in FindAll()
                   orderby r.RefOGV.Name
                   select new
                   {
                       r.ID,
                       r.Executor,
                       r.RowType,
                       RequestDate = r.RequestDate == null 
                            ? string.Empty 
                            : ((DateTime)r.RequestDate).ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                       RefCategory = r.RefOrgCategory.Name,
                       RefCategoryShort = r.RefOrgCategory.ShortName,
                       RefStateID = r.RefStateOGV.ID,
                       RefState = r.RefStateOGV.Name,
                       RefOGVId = r.RefOGV.ID,
                       RefOGVName = r.RefOGV.Name
                   };
        }

        public IQueryable GetQueryByOGV(int? ogvId, List<int> filters, int periodId)
        {
            var data = from r in FindAll()
                       where (!filters.Contains(r.RefStateOGV.ID)) &&
                             r.ID != -1 &&   
                             (ogvId == null || r.RefOGV.ID == ogvId) && (r.RefYearDayUNV.ID == periodId)
                       orderby r.RefOrgCategory.RefOGV.ID
                       select new
                       {
                           r.ID,
                           r.Executor,
                           r.RowType,
                           RequestDate = r.RequestDate == null 
                                ? string.Empty 
                                : ((DateTime)r.RequestDate).ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                           RefCategory = r.RefOrgCategory.Name,
                           RefCategoryId = r.RefOrgCategory.ID,
                           RefCategoryShort = r.RefOrgCategory.ShortName,
                           RefStateID = r.RefStateOGV.ID,
                           RefState = r.RefStateOGV.Name,
                           RefOGVId = r.RefOGV.ID,
                           RefOGVName = r.RefOGV.Name,
                           PeriodId = r.RefYearDayUNV.ID
                       };
            return data;
        }

        /// <summary>
        /// Получает номер заявки от ОГВ по категории
        /// </summary>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <param name="periodId">Идентификатор периода</param>
        /// <returns>Идентификатор заявки</returns>
        public int GetAppIdByCategory(int categoryId, int periodId)
        {
            var data = from r in FindAll()
                       where (r.RefOrgCategory.ID == categoryId && r.RefYearDayUNV.ID == periodId)
                       orderby r.RefOrgCategory.RefOGV.ID
                       select new
                       {
                           r.ID
                       };
            return (data.ToList().Count > 0) ? data.ToList().First().ID : -1;
        }

        public new D_Application_FromOGV Get(int id)
        {
            return FindOne(id);
        }

        public object GetQueryOne(int id)
        {
            var r = FindOne(id);
            return new
            {
                r.ID,
                r.Executor,
                r.RowType,
                RequestDate = r.RequestDate == null 
                    ? string.Empty 
                    : ((DateTime)r.RequestDate).ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                RefCategory = r.RefOrgCategory.Name,
                RefCategoryShort = r.RefOrgCategory.ShortName,
                RefStateID = r.RefStateOGV.ID,
                RefState = r.RefStateOGV.Name,
                RefOGVId = r.RefOGV.ID,
                RefOGVName = r.RefOGV.Name
            };
        }

        public DetailsEstimateModel GetDetailsViewModel(int id)
        {
            var queryable = from t in FindAll()
                            where t.ID == id
                            select new DetailsEstimateModel
                            {
                                ID = t.ID,
                                CategoryID = t.RefOrgCategory.ID,
                                CategoryName = t.RefOrgCategory.Name,
                                Date = t.RequestDate == null 
                                    ? string.Empty 
                                    : ((DateTime)t.RequestDate).ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                                PeriodId = t.RefYearDayUNV.ID,
                                OGVID = t.RefOGV.ID,
                                OGVName = t.RefOGV.Name,
                                StateOGV = t.RefStateOGV.ID,
                                StateNameOGV = t.RefStateOGV.Name
                            };

            var detailsViewModel = (queryable.Count() > 0) 
                ? queryable.First() 
                : new DetailsEstimateModel
            {
                                CategoryID = 0,
                                CategoryName = string.Empty,
                                Date = DateTime.Today.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                                PeriodId = ((DateTime.Today.Year - 1) * 10000) + 1,
                                OGVID = 0,
                                OGVName = string.Empty,
                                StateOGV = 1,
                                StateNameOGV = "Создана"
            };

            return detailsViewModel;
        }

        public DetailsEstimateModel GetDetailsViewModelByCategory(int categoryId, int periodId)
        {
            var queryable = from t in FindAll()
                            where t.RefOrgCategory.ID == categoryId && t.RefYearDayUNV.ID == periodId
                            select new DetailsEstimateModel
                            {
                                ID = t.ID,
                                CategoryID = t.RefOrgCategory.ID,
                                CategoryName = t.RefOrgCategory.Name,
                                Date = t.RequestDate == null 
                                    ? string.Empty 
                                    : ((DateTime)t.RequestDate).ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                                PeriodId = t.RefYearDayUNV.ID,
                                OGVID = t.RefOGV.ID,
                                OGVName = t.RefOGV.Name,
                                StateOGV = t.RefStateOGV.ID,
                                StateNameOGV = t.RefStateOGV.Name
                            };

            var detailsViewModel = (queryable.Count() > 0)
                ? queryable.First()
                : new DetailsEstimateModel
                {
                    CategoryID = categoryId,
                    CategoryName = new CategoryTaxpayerService().FindOne(categoryId).Name,
                    Date = DateTime.Today.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                    PeriodId = ((DateTime.Today.Year - 1) * 10000) + 1,
                    OGVID = 0,
                    OGVName = string.Empty,
                    StateOGV = 1,
                    StateNameOGV = "Создана"
                };

            return detailsViewModel;
        }

        public DetailsEstimateModel GetDetailsViewModelByID(int appFromOGVId)
        {
            var queryable = from t in FindAll()
                            where t.ID == appFromOGVId
                            select new DetailsEstimateModel
                            {
                                ID = t.ID,
                                CategoryID = t.RefOrgCategory.ID,
                                CategoryName = t.RefOrgCategory.Name,
                                Date = t.RequestDate == null 
                                    ? string.Empty 
                                    : ((DateTime)t.RequestDate).ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                                PeriodId = t.RefYearDayUNV.ID,
                                OGVID = t.RefOGV.ID,
                                OGVName = t.RefOGV.Name,
                                StateOGV = t.RefStateOGV.ID,
                                StateNameOGV = t.RefStateOGV.Name
                            };

            var detailsViewModel = (queryable.Count() > 0)
                ? queryable.First()
                : null;

            return detailsViewModel;
        }
    }
}
