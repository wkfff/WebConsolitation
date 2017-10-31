using System.Web.Mvc;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Org3PricesAndTariffs.Consolidated
{
    public class Org3GasolineConsController : SchemeBoundController
    {
        private IExportService exportService;
        
        public Org3GasolineConsController(IExportService exportService)
        {
            this.exportService = exportService;
        }

        public ActionResult ExportGas(int taskId)
        {
            var stream = exportService.GetReportGas(taskId);
            string fileName = "Отчет.xls";
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        public ActionResult ExportFood(int taskId)
        {
            var stream = exportService.GetReportFood(taskId);
            string fileName = "Отчет.xls";
            return File(stream, "application/vnd.ms-excel", fileName);
        }
    }
}
