using System.Web.Mvc;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Mrot
{
    public class ExportMrotController : SchemeBoundController
    {
        private readonly IExportService exportService;
        private readonly IUserSessionState extension;

        public ExportMrotController(
            IExportService exportService,
            IUserSessionState extension)
        {
            this.exportService = exportService;
            this.extension = extension;
        }

        public ActionResult Executers(int taskId)
        {
            var stream = exportService.ExportExecuters(taskId);
            var docName = exportService.GetDocumentName(taskId, "Должности", true);
            return File(stream, "application/vnd.ms-excel", "{0}.xls".FormatWith(docName));
        }

        public ActionResult FormCollection(int taskId)
        {
            var stream = exportService.ExportFormCollection(taskId);
            var docName = exportService.GetDocumentName(taskId, "Форма сбора", false);
            return File(stream, "application/vnd.ms-excel", "{0}.xls".FormatWith(docName));
        }

        public ActionResult UnactedRegions(int taskId)
        {
            var stream = exportService.ExportUnactedRegions(taskId);
            var docName = exportService.GetDocumentName(taskId, "Не поступившие данные", true);
            return File(stream, "application/vnd.ms-excel", "{0}.xls".FormatWith(docName));
        }

        public ActionResult SubjectTrihedrData(int taskId)
        {
            var stream = exportService.ExportMOReport(taskId);
            var docName = exportService.GetDocumentName(taskId, "Отчет", true);
            return File(stream, "application/vnd.ms-excel", "{0}.xls".FormatWith(docName));
        }
    }
}
