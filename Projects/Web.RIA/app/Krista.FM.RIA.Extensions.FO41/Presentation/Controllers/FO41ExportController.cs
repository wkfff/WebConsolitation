using System.Web.Mvc;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.FO41.Services;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controllers
{
    public class FO41ExportController : SchemeBoundController
    {
        private readonly IExportService exportService;
        private readonly IFO41Extension extension;
        private readonly ICategoryTaxpayerService categoryRepository;
        private readonly IRepository<FX_FX_TypeTax> typeTaxRepository;

        public FO41ExportController(
            IFO41Extension extension, 
            IExportService exportService, 
            ICategoryTaxpayerService categoryRepository,
            IRepository<FX_FX_TypeTax> typeTaxRepository)
        {
            this.extension = extension;
            this.exportService = exportService;
            this.categoryRepository = categoryRepository;
            this.typeTaxRepository = typeTaxRepository;
        }

        public ActionResult ReportTaxpayer(int id, string name)
        {
            var stream = (extension.OKTMO == FO41Extension.OKTMOYar)
                ? exportService.ExportForTaxpayer(id)
                : exportService.ExportForTaxpayerHMAO(id);

            return File(stream, "application/vnd.ms-excel", "{0}.xls".FormatWith("Заявка от {0}".FormatWith(name)));
        }

        public ActionResult ReportOGV(int id, string name)
        {
            var stream = exportService.ExportForOGV(id);

            return File(stream, "application/vnd.ms-excel", "{0}.xls".FormatWith("Заявка от {0}".FormatWith(name)));
        }

        public ActionResult ReportResultCategoryHMAO(int categoryId, int periodId)
        {
            var category = categoryRepository.GetQueryOne(categoryId); 
            var stream = exportService.ExportResultCategoryHMAO(
                    category.RefTypeTax.ID, 
                    categoryId, 
                    category.Name, 
                    periodId);

            var name = "Итоговые показатели_{0}_{1}_{2}.xls"
                .FormatWith(category.RefTypeTax.Name, category.ShortName, ExportService.GetTextForPeriod(periodId));
            return File(stream, "application/vnd.ms-excel", name);
        }

        public ActionResult ReportEstimateCategoryHMAO(int categoryId, int periodId)
        {
            var category = categoryRepository.GetQueryOne(categoryId);
            var stream = exportService.ExportEstimateCategoryHMAO(
                    categoryId,
                    category.Name,
                    category.RefTypeTax.ID,
                    periodId);

            var name = "Оценка_{0}_{1}_{2}.xls"
                .FormatWith(category.RefTypeTax.Name, category.ShortName, ExportService.GetTextForPeriod(periodId));
            return File(stream, "application/vnd.ms-excel", name);
        }

        public ActionResult ReportResult(int appFromOGVId, int sourceId, string name)
        {
            var stream = exportService.ExportResult(appFromOGVId, sourceId);

            return File(stream, "application/vnd.ms-excel", "{0}.xls".FormatWith(name));
        }

        public ActionResult ReportResultTaxTypeHMAO(int taxTypeId, int periodId)
        {
            var stream = exportService.ExportResultTaxType(taxTypeId, periodId);
            var tax = typeTaxRepository.Get(taxTypeId);
            var name = "Итоговые показатели_{0}_{1}.xls".FormatWith(tax.Name, ExportService.GetTextForPeriod(periodId));
            return File(stream, "application/vnd.ms-excel", name);
        }
    }
}
