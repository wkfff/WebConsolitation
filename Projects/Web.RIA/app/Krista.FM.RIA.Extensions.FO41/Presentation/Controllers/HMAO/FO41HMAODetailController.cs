using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.FO41.Services;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controllers.HMAO
{
    public class FO41HMAODetailController : SchemeBoundController
    {
        private readonly CategoryTaxpayerService categoryRepository;

        private readonly NHibernateLinqRepository<D_Org_Privilege> orgPrivilegeRepository;

        private readonly AppPrivilegeService requestsRepository;

        private readonly ILinqRepository<B_Regions_Bridge> regionsBridgeRepository;

        public FO41HMAODetailController(
            CategoryTaxpayerService categoryRepository, 
            NHibernateLinqRepository<D_Org_Privilege> orgPrivilegeRepository,
            NHibernateLinqRepository<T_Doc_ApplicationOrg> docAppOrgRepository,
            AppPrivilegeService requestsRepository,
            ILinqRepository<B_Regions_Bridge> regionsBridgeRepository)
        {
            this.categoryRepository = categoryRepository;
            this.orgPrivilegeRepository = orgPrivilegeRepository;
            this.requestsRepository = requestsRepository;
            this.regionsBridgeRepository = regionsBridgeRepository;
        }

        /// <summary>
        /// Возвращает список всех категорий
        /// </summary>
        /// <returns>Список категорий</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult LookupCategoryTaxPayerHMAO(int taxTypeId, int periodId, int orgId, int curCategoryId)
        {
            var orgsWithRequests = requestsRepository.FindAll()
                .Where(x => x.RefOrgPrivilege.ID == orgId && x.RefYearDayUNV.ID == periodId)
                .Select(x => x.RefOrgCategory.ID).ToList();
                
            var data = (from c in categoryRepository.FindAll()
                        where c.RefTypeTax != null && 
                            c.RefTypeTax.ID == taxTypeId && 
                            (!orgsWithRequests.Contains(c.ID) || c.ID == curCategoryId)
                        select new
                        {
                            c.ID,
                            c.Name,
                            c.RowType,
                            RefTypeTax = c.RefTypeTax.ID,
                            RefTypeTaxName = c.RefTypeTax.Name,
                            c.ShortName,
                            c.CorrectIndex
                        }).ToList();
            return new AjaxStoreResult(data, data.Count);
        }

        /// <summary>
        /// Возвращает список всех территорий
        /// </summary>
        /// <returns>Список территорий</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult LookupRegionsHMAO()
        {
            var data = (from r in regionsBridgeRepository.FindAll()
                       where (r.RefTerrType != null && (r.RefTerrType.ID == 4 || r.RefTerrType.ID == 7))
                       select new
                                  {
                                      r.ID,
                                      r.Name,
                                      r.ShortName
                                  }).ToList();

            return new AjaxStoreResult(data, data.Count);
        }

        /// <summary>
        /// Возвращает список всех организаций, если параметры не заданы или
        /// список организаций, по которым нет еще заявок по категории categoryId и периоду periodId плюс организация с идентфикатором orgId
        /// </summary>
        /// <param name="categoryId">Идентификатор категории или null</param>
        /// <param name="periodId">Идентификатор периода или null</param>
        /// <param name="curOrgId">Идентификатор организации или null (-1)</param>
        /// <returns>Список организаций</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult LookupOrgsHMAO(int? categoryId, int? periodId, int? curOrgId)
        {
            var orgsWithRequests = (categoryId != null && periodId != null)
                ? requestsRepository.FindAll()
                    .Where(x => x.RefOrgCategory.ID == categoryId && x.RefYearDayUNV.ID == periodId)
                    .Select(x => x.RefOrgPrivilege.ID).ToList()
                : new List<int>();
            
            var data = (from r in orgPrivilegeRepository.FindAll()
                            .Where(x => !orgsWithRequests.Contains(x.ID) || x.ID == curOrgId).ToList()
                        select new
                        {
                            r.ID,
                            r.Name,
                            INN = r.Code,
                            KPP = r.INN20,
                            r.LegalAddress,
                            BridgeRegion = r.RefBridgeRegions.Name,
                            BridgeRegionID = r.RefBridgeRegions.ID,
                            r.Address,
                            r.Unit,
                            r.OKATO
                        }).ToList();

            return new AjaxStoreResult(data, data.Count);
        }
    }
}
