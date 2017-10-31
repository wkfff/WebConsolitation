using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public class CommonSamaraReport : Report
    {
        private static int columnCount;
        private static DateTime calcDate = DateTime.Now;
        private ReportsDataService dataService;

        public static int ColumnCount
        {
            get { return columnCount; }
            set { columnCount = value; }
        }
        
        public ReportsDataService DataService
        {
            get { return dataService; }
            set { dataService = value; }
        }

        public override string TemplateName
        {
            get { return string.Empty; }
        }

        public virtual void PrepareData(int currentVariantId, int regionId, DateTime calculateDate, ref DataTable[] tables)
        {
        }

        public override void Create(string templateDocumentName, int currentVariantId, int regionId, DateTime calculateDate, D_S_TitleReport titleReport)
        {
            calcDate = calculateDate;
            DataTable[] tables = new DataTable[1];
            PrepareData(currentVariantId, regionId, calculateDate, ref tables);
            Render(templateDocumentName, tables, titleReport);
        }

        private static void FillDataTable(HSSFWorkbook wb, HSSFSheet sheet, DataTable tableData, int startIndex)
        {
            int rowIndex = startIndex;
            int rowCount = tableData.Rows.Count;
            
            // Погнали заполнять
            foreach (DataRow row in tableData.Rows)
            {
                rowCount--;
                if (rowCount > 1)
                {
                    NPOIHelper.CopyRow(wb, sheet, rowIndex, rowIndex + 1);
                }

                for (int i = 0; i < columnCount; i++)
                {
                    NPOIHelper.SetCellValue(sheet, rowIndex, i, row[i]);
                }

                rowIndex++;
            }
        }

        private static void Render(string templateDocumentName, IList<DataTable> tables, D_S_TitleReport titleReport)
        {
            HSSFWorkbook wb;
            using (FileStream fs = new FileStream(templateDocumentName, FileMode.Open, FileAccess.Read))
            {
                wb = new HSSFWorkbook(fs, true);
            }

            HSSFSheet sheet = wb.GetSheetAt(0);
            
            // Итог внутреннего долга
            NPOIHelper.SetCellValue(sheet, 08, columnCount - 1, tables[2].Rows[0][0]);
            
            // Итог внешнего долга
            NPOIHelper.SetCellValue(sheet, 12, columnCount - 1, tables[2].Rows[0][1]);
            
            // Итог общего долга
            NPOIHelper.SetCellValue(sheet, 13, columnCount - 1, tables[2].Rows[0][2]);

            // Заполняем табличку по районам
            FillDataTable(wb, sheet, tables[1], 10);
            FillDataTable(wb, sheet, tables[0], 06);

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
    }
}
