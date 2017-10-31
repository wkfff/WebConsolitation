using System.Web.Mvc;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.MarksOMSU.Services;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Controllers
{
    public class OmsuEstimateController : SchemeBoundController
    {
        private readonly IOmsuEstimateService omsuEstimateService;
        private readonly IRepository<FX_OMSU_StatusData> statusRepository;
        private readonly IExportService exportService;

        public OmsuEstimateController(
            IOmsuEstimateService omsuEstimateService,
            IRepository<FX_OMSU_StatusData> statusRepository,
            IExportService exportService)
        {
            this.omsuEstimateService = omsuEstimateService;
            this.statusRepository = statusRepository;
            this.exportService = exportService;
        }

        public ActionResult GetTargetMark()
        {
            return null;
        }

        public ActionResult GetTargetFacts(int targetMarkId)
        {
            return null;
        }

        public ActionResult GetSourceFacts(int targetMarkId, int regionId)
        {
            return null;
        }

        [Transaction]
        public ActionResult SaveTargetFacts(object data)
        {
            return null;
        }

        public ActionResult ExportToXls(int targetMarkId, string itfCaption)
        {
            return null;
        }
    }
}
