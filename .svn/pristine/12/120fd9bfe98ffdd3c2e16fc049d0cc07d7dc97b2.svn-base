using System.Web.Mvc;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.MarksOMSU.Services;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Controllers
{
    public class ExportController : SchemeBoundController
    {
        private readonly IExportService exportService;
        private readonly IMarksOmsuExtension extension;
        private readonly IMarksRepository marksRepository;

        public ExportController(
            IExportService exportService, 
            IMarksOmsuExtension extension,
            IMarksRepository marksRepository)
        {
            this.exportService = exportService;
            this.extension = extension;
            this.marksRepository = marksRepository;
        }

        public ActionResult Omsu()
        {
            var stream = exportService.ExportForOmsu(extension.DataSourceOmsu.ID, extension.UserRegionCurrent.ID);

            return File(stream, "application/vnd.ms-excel", "{0}.xls".FormatWith(extension.UserRegionCurrent.Name));
        }

        public ActionResult Oiv(int markId)
        {
            var stream = exportService.ExportForOiv(extension.DataSourceOmsu.ID, markId);

            var mark = marksRepository.FindOne(markId);

            return File(stream, "application/vnd.ms-excel", "Показатель код {0}.xls".FormatWith(mark.Code));
        }

        public ActionResult OivInputData(int markId)
        {
            var stream = exportService.ExportOivInputData(extension.DataSourceOmsu.ID, markId);

            var mark = marksRepository.FindOne(markId);

            return File(stream, "application/vnd.ms-excel", "Ввод данных ОИВ, показатель {0}.xls".FormatWith(mark.Code));
        }
    }
}
