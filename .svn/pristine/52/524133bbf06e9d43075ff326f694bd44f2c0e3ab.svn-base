using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.FO41.Services;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controllers
{
    public class FO41DetailController : SchemeBoundController
    {
        private readonly CategoryTaxpayerService categoryRepository;

        private readonly RegionsService regionsService;

        private readonly NHibernateLinqRepository<D_Org_Privilege> orgPrivilegeRepository;

        private readonly AppPrivilegeService requestsRepository;

        public FO41DetailController(
            CategoryTaxpayerService categoryRepository, 
            RegionsService regionsService,
            NHibernateLinqRepository<D_Org_Privilege> orgPrivilegeRepository,
            NHibernateLinqRepository<T_Doc_ApplicationOrg> docAppOrgRepository,
            AppPrivilegeService requestsRepository)
        {
            this.categoryRepository = categoryRepository;
            this.regionsService = regionsService;
            this.orgPrivilegeRepository = orgPrivilegeRepository;
            this.requestsRepository = requestsRepository;
        }

        /// <summary>
        /// Возвращает список категорий по ОГВ/ДФ
        /// </summary>
        /// <returns>Список категорий</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult LookupCategoryTaxPayer(int ogv, string exceptIDs)
        {
            var data = categoryRepository.GetByOGV(ogv, exceptIDs);
            return new AjaxStoreResult(data);
        }

        /// <summary>
        /// Возвращает список категорий по налогоплательщику
        /// </summary>
        /// <returns>Список категорий</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult LookupCategoryTaxPayerByOrg(string exceptIDs)
        {
            var data = categoryRepository.Get(exceptIDs);
            return new AjaxStoreResult(data);
        }
        
        /// <summary>
        /// Возвращает регион по налогоплательщику
        /// </summary>
        /// <param name="orgValue">Идентификатор налогоплательщика</param>
        /// <returns>Регион налогоплательщика</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult LookupRegions(int orgValue)
        {
            var data = regionsService.GetQueryOne(orgValue);
            return new AjaxStoreResult(new List<object> { data }, 1);
        }

        /// <summary>
        /// формирует скрипт, устанавливающий значение контрола равное имени региона налогоплательщика
        /// </summary>
        /// <param name="orgId">Идентификатор налогоплательщика</param>
        /// <param name="textField">Имя контрола</param>
        /// <returns>Скрипт присваивающий имя региона</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public JavaScriptResult GetRegionByOrg(int orgId, string textField)
        {
            var regionName = orgPrivilegeRepository.FindOne(orgId).RefBridgeRegions.Name;
            if (regionName.Equals("Несопоставленные данные"))
            {
                regionName = string.Empty;
            }

            var x = new JavaScriptResult
                        {
                            Script = "{0}.setValue('{1}')".FormatWith(textField, regionName)
                        };
            return x;
        }

        /// <summary>
        /// формирует скрипт, устанавливающий сообщение о копиях заявки
        /// </summary>
        /// <param name="orgId">Идентификатор налогоплательщика</param>
        /// <param name="periodId">Идентификатор периода</param>
        /// <param name="indicatorField">Куда записывать сообщение о копиях заявок</param>
        /// <returns>Скрипт присваивающий сообщение о копиях заявки</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public JavaScriptResult GetCopiesMsg(int orgId, int periodId, string indicatorField)
        {
            var requests = from r in requestsRepository.FindAll().Where(f =>
                                                                        f.RefOrgPrivilege.ID == orgId &&
                                                                        f.RefYearDayUNV.ID == periodId)
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

            var x = new JavaScriptResult
            {   
                Script = (reqList.Length > 0) 
                    ? "{0}.setValue('По данному налогоплательщику уже имеются заявки по следующим категориям: {1}')"
                        .FormatWith(indicatorField, reqList)
                    : "{0}.setValue('')".FormatWith(indicatorField)
            };
            return x;
        }

        /// <summary>
        /// Возвращает список всех налогоплательщиков
        /// </summary>
        /// <returns>Список налогоплательщиков</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult LookupOrgPrivilege(int org, int category, int period)
        {
            var orgToDecludeList = from x in requestsRepository.FindAll()
                .Where(x => x.RefOrgCategory.ID == category && x.RefYearDayUNV.ID == period)
                select new { x.RefOrgPrivilege.ID };

            var orgToDeclude = orgToDecludeList.Select(x => x.ID).ToList();

            var data = (from r in orgPrivilegeRepository.FindAll()
                        where !orgToDeclude.Contains(r.ID) || r.ID == org
                        select new
                        {
                            r.ID,
                            r.Name,
                            RefRegionsId = r.RefBridgeRegions.ID
                        }).ToList();

            return new AjaxStoreResult(data.ToList(), data.ToList().Count);
        }
    }
}