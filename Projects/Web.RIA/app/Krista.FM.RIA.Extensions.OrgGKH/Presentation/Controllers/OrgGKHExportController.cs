using System.Web.Mvc;
using Ext.Net.Utilities;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.OrgGKH.Services;

namespace Krista.FM.RIA.Extensions.OrgGKH.Presentation.Controllers
{
    public class OrgGKHExportController : SchemeBoundController
    {
        private readonly IExportService exportService;

        public OrgGKHExportController(IExportService exportService)
        {
            this.exportService = exportService;
        }

        public ActionResult Report(int orgId, int periodId, int sourceId, string name, string terr)
        {
            var stream = periodId % 100 == 0
                ? exportService.ExportMonth(orgId, periodId, sourceId, terr) 
                : exportService.ExportWeek(orgId, periodId, sourceId, terr);

            return File(stream, "application/vnd.ms-excel", "{0}.xls".FormatWith(name));
        }
    }
}
