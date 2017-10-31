using System;
using System.Linq;
using System.Web.Mvc;

using Ext.Net.MVC;

using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Models;
using Krista.FM.RIA.Extensions.E86N.Services;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.RestControllers
{
    public class CommonDataController : SchemeBoundController
    {
        private readonly ICommonDataService commonDataService;

        public CommonDataController(ICommonDataService commonDataService)
        {
            this.commonDataService = commonDataService;
        }

        public ActionResult GetYearFormList()
        {
            var data = commonDataService.GetYearFormList().ToList();
            return new AjaxStoreResult(data, data.Count);
        }

        public ActionResult GetPartDocList(int masterId, DateTime? formationDate)
        {
            if (formationDate == null)
            {
                return new AjaxStoreResult(Enumerable.Empty<Domain.FX_FX_PartDoc>());
            }

            var data = commonDataService.GetPartDocList(masterId, formationDate.Value).ToList();
            return new AjaxStoreResult(data, data.Count);
        }

        public ActionResult GetTipFilList()
        {
            var data = commonDataService.GetTipFilList().ToList();
            return new AjaxStoreResult(data, data.Count);
        }

        public ActionResult GetOkvedList()
        {
            var data = commonDataService.GetOkvedList().ToList();
            return new AjaxStoreResult(data, data.Count);
        }

        public ActionResult GetOkvedListExt(int? limit, int? start, string query, DateTime? dateBegin, DateTime? dateEnd)
        {
            var data = commonDataService.GetOkvedList(query, dateBegin, dateEnd).ToList();
            if (limit.HasValue && start.HasValue)
            {
                return new AjaxStoreResult(data.Skip(start.Value).Take(limit.Value), data.Count);    
            }

            return new AjaxStoreResult(data, data.Count);
        }

        public ActionResult GetPrOkvedList()
        {
            var data = commonDataService.GetPrOkvedList().ToList();
            return new AjaxStoreResult(data, data.Count);
        }

        public ActionResult GetTypeDocList()
        {
            var data = commonDataService.GetTypeDocList().ToList();
            return new AjaxStoreResult(data, data.Count);
        }

        public ActionResult GetOrgCategoryList()
        {
            var data = commonDataService.GetOrgCategoryList().ToList();
            return new AjaxStoreResult(data, data.Count);
        }

        public ActionResult GetFinPeriodList()
        {
            var data = commonDataService.GetFinPeriodList().ToList();
            return new AjaxStoreResult(data, data.Count);
        }

        public ActionResult GetSostDataList()
        {
            var data = commonDataService.GetSostDataList().ToList();
            return new AjaxStoreResult(data, data.Count);
        }

        public ActionResult GetTipUchList()
        {
            var data = commonDataService.GetTipUchList().ToList();
            return new AjaxStoreResult(data, data.Count);
        }

        public ActionResult GetGRBSList()
        {
            var data = commonDataService.GetGRBSList().ToList();
            return new AjaxStoreResult(data, data.Count);
        }

        public ActionResult GetOkei(int limit, int start, string query)
        {
            var data = OKEIModel.GetList(limit, start, query);
            return new AjaxStoreResult(data.Skip(start).Take(limit), data.Count);
        }
    }
}