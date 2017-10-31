using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public class DebtBookBudCreditSamaraReport : Report
    {
        protected readonly ReportsDataService DataService;
        private static DateTime calcDate = DateTime.Now;

        public DebtBookBudCreditSamaraReport(ReportsDataService reportsDataService)
        {
            this.DataService = reportsDataService;
        }

        public override string TemplateName
        {
            get { return "ReportDebtBookBudCreditSamara"; }
        }

        public override void Create(string templateDocumentName, int currentVariantId, int regionId, DateTime calculateDate, D_S_TitleReport titleReport)
        {
            calcDate = calculateDate;
            DataTable[] tables = new DataTable[1];
            PrepareData(currentVariantId, regionId, calculateDate, ref tables);
            Render(templateDocumentName, tables, titleReport);
        }

        private static void Render(string templateDocumentName, IList<DataTable> tables, D_S_TitleReport titleReport)
        {
            HSSFWorkbook wb;
            using (FileStream fs = new FileStream(templateDocumentName, FileMode.Open, FileAccess.Read))
            {
                wb = new HSSFWorkbook(fs, true);
            }

            HSSFSheet sheet = wb.GetSheetAt(0);

            // Заполняем табличку по районам
            int rowIndex = 3;
            int rowCount = tables[0].Rows.Count;
            Dictionary<int, int> colIndexes = new Dictionary<int, int>();

            // Это чтобы универсальный заполнитель датасета сделать по кредитам
            colIndexes.Add(0, 0);
            colIndexes.Add(1, 1);
            colIndexes.Add(2, 2);
            colIndexes.Add(3, 4);
            colIndexes.Add(4, 6);
            colIndexes.Add(5, 7);
            foreach (DataRow row in tables[0].Rows)
            {
                rowCount--;
                if (rowCount > 1)
                {
                    NPOIHelper.CopyRow(wb, sheet, rowIndex, rowIndex + 1);
                }

                for (int i = 0; i < 6; i++)
                {
                    NPOIHelper.SetCellValue(sheet, rowIndex, i, row[colIndexes[i]]);
                }

                rowIndex++;
            }

            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI";
            wb.DocumentSummaryInformation = dsi;
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI";
            wb.SummaryInformation = si;
            using (FileStream file = new FileStream(templateDocumentName, FileMode.Create))
            {
                wb.Write(file);
                file.Close();
            }
        }

        private void PrepareData(int currentVariantId, int regionId, DateTime calculateDate, ref DataTable[] tables)
        {
            DataService.GetBudCreditSamaraData(
                 currentVariantId, regionId, ref tables, calculateDate);
        }
    }
}
