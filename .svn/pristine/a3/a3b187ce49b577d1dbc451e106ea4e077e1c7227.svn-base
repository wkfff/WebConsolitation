using System.Web.Mvc;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.Region10MarksOIV.Services;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Presentation.Controllers
{
    public class Region10ExportController : SchemeBoundController
    {
        private readonly IExportService exportService;
        private readonly IRegion10MarksOivExtension extension;
        
        public Region10ExportController(
            IExportService exportService, 
            IRegion10MarksOivExtension extension)
        {
            this.exportService = exportService;
            this.extension = extension;
        }

        public ActionResult Oiv()
        {
            var stream = exportService.ExportForOiv(extension.DataSourceOiv.ID, extension.RootTerritoryRf.ID, User.IsInRole(OivRoles.OivApprove));

            return File(stream, "application/vnd.ms-excel", "{0}.xls".FormatWith(extension.UserResponseOiv.Name));
        }

        public ActionResult Omsu()
        {
            var stream = exportService.ExportForOmsu(extension.DataSourceOiv.ID, extension.UserTerritoryRf.ID);

            return File(stream, "application/vnd.ms-excel", "{0}.xls".FormatWith(extension.UserTerritoryRf.Name));
        }

        public ActionResult OmsuCompare(int? markId)
        {
            var stream = exportService.ExportForOmsuCompare(extension.DataSourceOiv.ID, markId);

            return File(stream, "application/vnd.ms-excel", "{0}.xls".FormatWith(extension.UserResponseOiv.Name));
        }
    }
}
