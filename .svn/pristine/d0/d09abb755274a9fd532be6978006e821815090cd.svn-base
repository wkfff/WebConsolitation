using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.FO41.Presentation.Models;

namespace Krista.FM.RIA.Extensions.FO41.Services
{
    public class AppPrivilegeService : NHibernateLinqRepository<D_Application_Privilege>, IAppPrivilegeService
    {
        private readonly IRepository<FX_FX_TypeTax> typeTaxRepository;
        private readonly IFO41Extension extension;
        private readonly ILinqRepository<B_Regions_Bridge> regionsBridgeRepository;
        private readonly ILinqRepository<D_Org_Privilege> orgRepository;
        private readonly ILinqRepository<T_Note_ApplicationOrg> commentsOrgVRepository;
        private readonly ILinqRepository<T_Doc_ApplicationOrg> filesRepository;
        private readonly ILinqRepository<F_Marks_DataPrivilege> factsRepository;

        public AppPrivilegeService(
            IFO41Extension extension,
            IRepository<FX_FX_TypeTax> typeTaxRepository,
            ILinqRepository<B_Regions_Bridge> regionsBridgeRepository,
            ILinqRepository<D_Org_Privilege> orgRepository,
            ILinqRepository<T_Note_ApplicationOrg> commentsOrgVRepository,
            ILinqRepository<T_Doc_ApplicationOrg> filesRepository,
            ILinqRepository<F_Marks_DataPrivilege> factsRepository)
        {
            this.typeTaxRepository = typeTaxRepository;
            this.extension = extension;
            this.regionsBridgeRepository = regionsBridgeRepository;
            this.orgRepository = orgRepository;
            this.commentsOrgVRepository = commentsOrgVRepository;
            this.filesRepository = filesRepository;
            this.factsRepository = factsRepository;
        }

        public IQueryable GetQueryByCategory(int categoryId)
        {
            var data = from r in FindAll()
                       where (r.RefOrgCategory.ID == categoryId)
                       select new
                       {
                           r.ID,
                           r.Executor,
                           RequestDate = r.RequestDate.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                           RefCategory = r.RefOrgCategory.Name,
                           RefCategoryShort = r.RefOrgCategory.ShortName,
                           r.RowType,
                           RefOrgName = r.RefOrgPrivilege.Name,
                           RefState = r.RefStateOrg.Name,
                           RefStateID = r.RefStateOrg.ID,
                           RefOGVId = r.RefOrgCategory.RefOGV.ID,
                           RefOGVName = r.RefOrgCategory.RefOGV.Name,
                           Included = r.RefApplicOGV.ID > 0,
                           CopiesCnt = FindAll().Count(f => 
                               f.RowType == r.RowType && 
                               f.RefOrgPrivilege.ID == r.RefOrgPrivilege.ID && 
                               f.RefYearDayUNV.ID == r.RefYearDayUNV.ID) - 1
                       };
            return data;
        }

        public IQueryable GetRequestToInputData(int categoryId, int periodId)
        {
            var data = from r in FindAll()
                       where (r.RefOrgCategory.ID == categoryId && 
                                r.RefYearDayUNV.ID == periodId && 
                                r.RefStateOrg.ID > 1)
                       select new
                       {
                           r.ID,
                           r.Executor,
                           RequestDate = r.RequestDate.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                           RefCategory = r.RefOrgCategory.Name,
                           RefCategoryShort = r.RefOrgCategory.ShortName,
                           r.RowType,
                           RefOrgName = r.RefOrgPrivilege.Name,
                           RefState = r.RefStateOrg.Name,
                           RefStateID = r.RefStateOrg.ID,
                           RefOGVId = r.RefOrgCategory.RefOGV.ID,
                           RefOGVName = r.RefOrgCategory.RefOGV.Name,
                           Included = r.RefApplicOGV.ID > 0,
                           CopiesCnt = FindAll().Count(f => 
                               f.RowType == r.RowType && 
                               f.RefOrgPrivilege.ID == r.RefOrgPrivilege.ID && 
                               f.RefYearDayUNV.ID == r.RefYearDayUNV.ID) - 1
                       };
            return data;
        }

        public IQueryable GetReqPrevPeriods(int orgId, int periodId)
        {
            var data = from r in FindAll()
                       where (r.RefOrgPrivilege.ID == orgId && r.RefYearDayUNV.ID < periodId)
                       orderby r.RefYearDayUNV.ID
                       select new
                       {
                           r.ID,
                           Name = "№ {0}: {1} - {2} - {3}".FormatWith(
                                r.ID, 
                                r.RefOrgCategory.ShortName, 
                                extension.GetTextForPeriod(r.RefYearDayUNV.ID), 
                                r.RefOrgPrivilege.Name)
                       };
            return data;
        }

        public object GetQueryOne(int id)
        {
            var r = FindOne(id);
            return new
                       {
                           Id = r.ID,
                           r.Executor,
                           RequestDate = r.RequestDate.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                           CategoryId = r.RefOrgCategory.ID,
                           CategoryName = r.RefOrgCategory.Name,
                           OrgName = r.RefOrgPrivilege.Name,
                           OrgId = r.RefOrgPrivilege.ID,
                           RegionId = r.RefOrgPrivilege.RefBridgeRegions.ID,
                           RegionName = r.RefOrgPrivilege.RefBridgeRegions.Name,
                           PeriodId = r.RefYearDayUNV.ID,
                           RefStateID = r.RefStateOrg.ID,
                           RefOGVId = r.RefOrgCategory.RefOGV.ID,
                           RefOGVName = r.RefOrgCategory.RefOGV.Name
                       };
        }

        public int GetYear(int id)
        {
            var r = FindOne(id);
            return r.RefYearDayUNV.ID / 10000;
        }

        /// <summary>
        /// Удаление заявки
        /// </summary>
        /// <param name="requestId">Идентификатор заявки</param>
        [Transaction]
        public string RemoveRequest(int requestId)
        {
            // Проверка, что заявка с указанным идентификаторм существует
            var request = Get(requestId);
            if (request == null)
            {
                return "Заявка №{0} не найдена".FormatWith(requestId);
            }

            // Проверка, что заявка находится в состоянии "Создана" == 1
            if (request.RefStateOrg.ID != 1)
            {
                return "Только заявки в состоянии 'Создана' могут быть удалены";
            }

            try
            {
                // Удаление комментариев
                var commentsOrg = commentsOrgVRepository.FindAll().Where(x => x.RefApplicOrg.ID == requestId);
                foreach (var comment in commentsOrg)
                {
                    commentsOrgVRepository.Delete(comment);
                }

                // Удаление документов
                var files = filesRepository.FindAll().Where(x => x.RefApplicOrg.ID == requestId);
                foreach (var file in files)
                {
                    filesRepository.Delete(file);
                }

                // Удаление показателей
                var facts = factsRepository.FindAll().Where(x => x.RefApplication.ID == requestId).ToList();
                foreach (var fact in facts)
                {
                    factsRepository.Delete(fact);
                }

                // Удаление собственно заявки
                Delete(request);

                return "Заявка №{0} удалена".FormatWith(requestId);
            }
            catch (Exception)
            {
                return @"Возможно, заявка №{0} не была удалена корректно".FormatWith(requestId);
            }
        }

        public DetailsViewModel GetDetailsViewModel(int id)
        {
            var queryable = from t in FindAll()
                            where t.ID == id
                            select new DetailsViewModel
                            {
                                Id = t.ID,
                                CategoryId = t.RefOrgCategory.ID,
                                CategoryName = t.RefOrgCategory.Name,
                                Executor = t.Executor,
                                OrgName = t.RefOrgPrivilege.Name,
                                OrgId = t.RefOrgPrivilege.ID,
                                RegionId = t.RefOrgPrivilege.RefBridgeRegions.ID,
                                RegionName = t.RefOrgPrivilege.RefBridgeRegions.Name,
                                RequestDate = t.RequestDate.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                                PeriodId = t.RefYearDayUNV.ID,
                                StateId = t.RefStateOrg.ID,
                                StateName = t.RefStateOrg.Name
                            };

            var copiesText = string.Empty;
            var curRequest = (queryable.Count() > 0) ? queryable.First() : null;
            if (curRequest != null)
            {
                var requests = from r in FindAll()
                               where r.RefOrgPrivilege.ID == curRequest.OrgId &&
                                     r.RefYearDayUNV.ID == curRequest.PeriodId && 
                                     r.RefOrgCategory.ID != curRequest.CategoryId
                               select new
                                          {
                                              r.RefOrgCategory.ShortName
                                          };

                    var requestsList = requests.ToList().Distinct();

                    var reqList = requestsList.Aggregate(
                        string.Empty, 
                        (current, request) => current + (request.ShortName + ", "));

                if (reqList.Length > 2)
                    {
                        reqList = reqList.Remove(reqList.Length - 2, 2);
                    }

                    copiesText = (reqList.Length > 0)
                                     ? "По данному налогоплательщику уже имеются заявки по следующим категориям: {0}"
                                           .FormatWith(reqList)
                                     : string.Empty;
            }

            var detailsViewModel = (queryable.Count() > 0) ? queryable.First() : new DetailsViewModel
            {
                CategoryId = 0,
                CategoryName = string.Empty,
                Executor = string.Empty,
                OrgName = string.Empty,
                OrgId = 0,
                RegionId = 0,
                RegionName = string.Empty,
                RequestDate = DateTime.Today.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                PeriodId = extension.GetPrevPeriod()
            };

            detailsViewModel.CopiesText = copiesText;

            if (extension.ResponsOrg != null)
            {
                detailsViewModel.OrgId = extension.ResponsOrg.ID;
                detailsViewModel.OrgName = extension.ResponsOrg.Name;
            }

            return detailsViewModel;
        }

        // для ХМАО

        /// <summary>
        /// Получение списка заявок по налогоплательщику
        /// </summary>
        /// <param name="taxPayerId">Идентификатор налогоплательщика</param>
        /// <param name="periodId">Идентификатор периода</param>
        /// <returns>Список заявок</returns>
        public IQueryable GetAppForTaxPayer(int taxPayerId, int periodId)
        {
            return from r in FindAll()
                   where r.RefOrgPrivilege.ID == taxPayerId && r.RefYearDayUNV.ID == periodId
                   orderby r.RequestDate
                   select new
                              {
                                  r.ID,
                                  RequestDate = r.RequestDate.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                                  CategoryShort = r.RefOrgCategory.ShortName,
                                  r.RowType,
                                  StateName = r.RefStateOrg.Name,
                                  StateID = r.RefStateOrg.ID,
                                  TaxTypeName = r.RefTypeTax.Name,
                                  TaxTypeID = r.RefTypeTax.ID,
                                  OrgName = r.RefOrgPrivilege.Name
                              };
        }

        /// <summary>
        /// Получение списка заявок по категории и периоду
        /// </summary>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <param name="periodId">Идентификатор периода</param>
        /// <returns>Список заявок</returns>
        public IQueryable GetAppForOGV(int categoryId, int periodId)
        {
            return from r in FindAll()
                   where r.RefOrgCategory.ID == categoryId && r.RefYearDayUNV.ID == periodId
                   orderby r.RequestDate
                   select new
                   {
                       r.ID,
                       RequestDate = r.RequestDate.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                       CategoryShort = r.RefOrgCategory.ShortName,
                       r.RowType,
                       StateName = r.RefStateOrg.Name,
                       StateID = r.RefStateOrg.ID,
                       TaxTypeName = r.RefTypeTax.Name,
                       TaxTypeID = r.RefTypeTax.ID,
                       OrgName = r.RefOrgPrivilege.Name
                   };
        }

        /// <summary>
        /// Получение списка заявок по категории и периоду
        /// </summary>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <param name="periodId">Идентификатор периода</param>
        /// <returns>Список заявок</returns>
        public IList<D_Application_Privilege> GetAppForOGVList(int categoryId, int periodId)
        {
            return FindAll().Where(r => r.RefOrgCategory.ID == categoryId && r.RefYearDayUNV.ID == periodId)
                .OrderBy(r => r.RequestDate).ToList();
        }

        public HMAODetailViewModel GetHMAODetailsViewModel(int id, int? typeTaxId)
        {
            var t = FindOne(id);
            var taxPayer = extension.ResponsOrg;

            HMAODetailViewModel detailsViewModel;
            if (t != null)
            {
                detailsViewModel = new HMAODetailViewModel
                {
                    Id = t.ID,
                    CategoryId = t.RefOrgCategory.ID,
                    CategoryName = t.RefOrgCategory.Name,
                    Executor = t.Executor,
                    OrgName = t.RefOrgPrivilege.Name,
                    OrgId = t.RefOrgPrivilege.ID,
                    RegionId = t.RefOrgPrivilege.RefBridgeRegions.ID,
                    RegionName = t.RefOrgPrivilege.RefBridgeRegions.Name,
                    RequestDate = t.RequestDate.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                    PeriodId = t.RefYearDayUNV.ID,
                    StateId = t.RefStateOrg.ID,
                    StateName = t.RefStateOrg.Name,

                    Address = t.RefOrgPrivilege.Address,
                    LegalAddress = t.RefOrgPrivilege.LegalAddress,
                    KPP = t.RefOrgPrivilege.INN20,
                    INN = t.RefOrgPrivilege.Code,
                    BridgeRegionId = t.RefOrgPrivilege.RefBridgeRegions == null
                        ? -1 :
                        t.RefOrgPrivilege.RefBridgeRegions.ID,
                    BridgeRegionName = t.RefOrgPrivilege.RefBridgeRegions == null
                        ? string.Empty
                        : t.RefOrgPrivilege.RefBridgeRegions.Name,
                    BridgeRegionShortName = t.RefOrgPrivilege.RefBridgeRegions == null
                        ? string.Empty
                        : t.RefOrgPrivilege.RefBridgeRegions.ShortName,
                    OKATO = t.RefOrgPrivilege.OKATO,
                    Unit = t.RefOrgPrivilege.Unit,
                    CopiesText = string.Empty,
                    TypeTax = t.RefTypeTax
                };
            }
            else
            {
                detailsViewModel = new HMAODetailViewModel
                                       {
                                           CategoryId = 0,
                                           CategoryName = string.Empty,
                                           Executor = string.Empty,
                                           OrgName = taxPayer == null ? string.Empty : taxPayer.Name,
                                           OrgId = taxPayer == null ? -1 : taxPayer.ID,
                                           RegionId = 0,
                                           RegionName = string.Empty,
                                           RequestDate =
                                               DateTime.Today.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                                           PeriodId = extension.GetPrevPeriod(),
                                           CopiesText = string.Empty,
                                           TypeTax = typeTaxId == null ? null : typeTaxRepository.Get((int)typeTaxId),
                                           StateId = 1,
                                           StateName = "Создана"
                                       };
            }

            taxPayer = taxPayer ?? orgRepository.FindOne(detailsViewModel.OrgId);

            if (taxPayer == null)
            {
                detailsViewModel.Address = string.Empty;
                detailsViewModel.LegalAddress = string.Empty;
                detailsViewModel.KPP = string.Empty;
                detailsViewModel.INN = 0;
                detailsViewModel.BridgeRegionId = -1;
                detailsViewModel.BridgeRegionName = string.Empty;
                detailsViewModel.BridgeRegionShortName = string.Empty;
                detailsViewModel.OKATO = 0;
                detailsViewModel.Unit = 0;
                detailsViewModel.OKATO = 0;
            }
            else
            {
                detailsViewModel.OrgName = taxPayer.Name;
                detailsViewModel.OrgId = taxPayer.ID;
                detailsViewModel.Address = taxPayer.Address;
                detailsViewModel.LegalAddress = taxPayer.LegalAddress;
                detailsViewModel.KPP = taxPayer.INN20;
                detailsViewModel.INN = taxPayer.Code;
                detailsViewModel.OKATO = taxPayer.OKATO;
                detailsViewModel.Unit = taxPayer.Unit;
                if (taxPayer.RefBridgeRegions == null)
                {
                    detailsViewModel.BridgeRegionId = -1;
                    detailsViewModel.BridgeRegionName = string.Empty;
                    detailsViewModel.BridgeRegionShortName = string.Empty;
                }
                else
                {
                    detailsViewModel.BridgeRegionId = taxPayer.RefBridgeRegions.ID;
                    var region = regionsBridgeRepository.FindOne(taxPayer.RefBridgeRegions.ID);
                    if (region == null)
                    {
                        detailsViewModel.BridgeRegionName = string.Empty;
                        detailsViewModel.BridgeRegionShortName = string.Empty;
                    }
                    else
                    {
                        detailsViewModel.BridgeRegionName = region.Name;
                        detailsViewModel.BridgeRegionShortName = region.ShortName;
                    }
                }
            }

            return detailsViewModel;
        }

        public IQueryable GetQueryAll()
        {
            return from r in FindAll()
                   orderby r.RefOrgCategory.Name
                   select new
                   {
                       r.ID,
                       r.Executor,
                       RequestDate = r.RequestDate.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                       RefCategory = r.RefOrgCategory.Name,
                       RefCategoryShort = r.RefOrgCategory.ShortName,
                       r.RowType,
                       RefOrgName = r.RefOrgPrivilege.Name,
                       RefState = r.RefStateOrg.Name,
                       RefStateID = r.RefStateOrg.ID,
                       RefOGVId = r.RefOrgCategory.RefOGV.ID,
                       RefOGVName = r.RefOrgCategory.RefOGV.Name,
                       CopiesCnt = FindAll().Count(f =>
                           f.RowType == r.RowType &&
                           f.RefOrgPrivilege.ID == r.RefOrgPrivilege.ID &&
                           f.RefYearDayUNV.ID == r.RefYearDayUNV.ID) - 1
                   };
        }

        public IQueryable GetQueryByCategory(int? categoryId, int? ogvId, int? periodId, List<int> filters)
        {
            var requests = from r in FindAll()
                           where ((categoryId == null || r.RefOrgCategory.ID == categoryId) &&
                                  !filters.Contains(r.RefStateOrg.ID)) &&
                                 (ogvId == null || r.RefOrgCategory.RefOGV.ID == ogvId) &&
                                  (r.RefYearDayUNV.ID == periodId || periodId == null)
                           orderby r.RefOrgCategory.RefOGV.ID, r.RefOrgCategory.Name
                           select r;

            return GetRequestsModelList(requests);
        }

        public IQueryable GetQueryByTaxPayer(int taxPayerId, int? periodId, List<int> filters)
        {
            var requests = from r in FindAll()
                           where (r.RefOrgPrivilege.ID == taxPayerId) &&
                                  !filters.Contains(r.RefStateOrg.ID) &&
                                  (r.RefYearDayUNV.ID == periodId || periodId == null)
                           orderby r.RefYearDayUNV.ID, r.RequestDate
                           select r;

            return GetRequestsModelList(requests);
        }

        private IQueryable GetRequestsModelList(IOrderedQueryable<D_Application_Privilege> requests)
        {
            var data = from r in requests
                       select new
                       {
                           r.ID,
                           r.Executor,
                           RequestDate = r.RequestDate.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                           RefCategory = r.RefOrgCategory.Name,
                           RefCategoryShort = r.RefOrgCategory.ShortName,
                           r.RowType,
                           RefOrgName = r.RefOrgPrivilege.Name,
                           RefState = r.RefStateOrg.Name,
                           RefStateID = r.RefStateOrg.ID,
                           RefOGVId = r.RefOrgCategory.RefOGV.ID,
                           RefOGVName = r.RefOrgCategory.RefOGV.Name,
                           CopiesCnt = FindAll().Count(f =>
                                                       f.RowType == r.RowType &&
                                                       f.RefOrgPrivilege.ID == r.RefOrgPrivilege.ID &&
                                                       f.RefYearDayUNV.ID == r.RefYearDayUNV.ID) - 1
                       };
            return data;
        }
    }
}
