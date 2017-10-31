using System.Globalization;
using System.IO;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Org3PricesAndTariffs.Gasoline
{
    public class ExportService : IExportService
    {
        private readonly ILinqRepository<T_Org_CPrice> factRepository;
        private readonly ILinqRepository<D_CD_Task> taskRepository;

        public ExportService(ILinqRepository<T_Org_CPrice> factRepository, ILinqRepository<D_CD_Task> taskRepository)
        {
            this.factRepository = factRepository;
            this.taskRepository = taskRepository;
        }

        public Stream GetExcelReport(int taskId)
        {
            Stream template = new MemoryStream(Resource.TemplateGasoline);

            var wb = new HSSFWorkbook(template);
            var sheet = wb.GetSheetAt(0);

            var task = taskRepository.FindOne(taskId);
            var facts = factRepository.FindAll().Where(x => x.RefReport.RefTask.ID == taskId).ToList();

            var goods = facts.Select(x => x.RefGood).Distinct().OrderBy(x => x.Code).ToList();

            // Рисуем шапку
            // Столбцы по количеству товаров
            int currentColumnIndex = 1;
            for (int i = 0; i < goods.Count; i++)
            {
                NPOIHelper.CopyColumn(sheet, currentColumnIndex, currentColumnIndex + i);
            }

            NPOIHelper.SetCellValue(sheet, 0, 1, task.RefSubject.Name);
            NPOIHelper.SetCellValue(sheet, 1, 1, task.EndDate.ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("ru-RU")));

            currentColumnIndex = 1;
            foreach (D_Org_Good good in goods)
            {
                NPOIHelper.SetCellValue(sheet, 2, currentColumnIndex, good.Name);
                currentColumnIndex++;
            }

            var orgs = facts.Select(x => x.RefRegistrOrg).Distinct().OrderBy(x => x.Code).ToList();
            int currentRowIndex = 3;
            foreach (D_Org_RegistrOrg org in orgs)
            {
                NPOIHelper.CopyRow(wb, sheet, currentRowIndex, currentRowIndex + 1);

                currentColumnIndex = 0;
                NPOIHelper.SetCellValue(sheet, currentRowIndex, currentColumnIndex, org.NameOrg);
                currentColumnIndex++;
                foreach (D_Org_Good good in goods)
                {
                    var value = facts.FirstOrDefault(x => x.RefGood == good && x.RefRegistrOrg == org);
                    if (value != null)
                    {
                        NPOIHelper.SetCellValue(sheet, currentRowIndex, currentColumnIndex, value.Price);
                    }

                    currentColumnIndex++;
                }

                currentRowIndex++;
            }

            NPOIHelper.DeleteRows(sheet, currentRowIndex, currentRowIndex);

            return WriteToStream(wb);
        }

        private static MemoryStream WriteToStream(HSSFWorkbook wb)
        {
            var dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI";
            wb.DocumentSummaryInformation = dsi;
            var si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI";
            wb.SummaryInformation = si;
            var resultStream = new MemoryStream();
            wb.Write(resultStream);
            resultStream.Seek(0, SeekOrigin.Begin);
            return resultStream;
        }
    }
}
