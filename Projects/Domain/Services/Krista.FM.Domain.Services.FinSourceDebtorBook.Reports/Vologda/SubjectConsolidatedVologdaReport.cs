using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public class SubjectConsolidatedVologdaReport : Report
    {
        private readonly ReportsDataService reportsDataService;
        private DateTime reportDate;

        public SubjectConsolidatedVologdaReport(ReportsDataService reportsDataService)
        {
            this.reportsDataService = reportsDataService;
        }

        public override string TemplateName
        {
            get { return "SubjectConsolidatedReport"; }
        }

        public override void Create(string templateDocumentName, int currentVariantId, int regionId, DateTime calculateDate, D_S_TitleReport titleReport)
        {
            DataTable[] tables = new DataTable[4];
            PrepareData(currentVariantId, regionId, calculateDate, ref tables);
            Render(templateDocumentName, tables, titleReport);
        }

        private HSSFWorkbook CreateDocument(string templateDocumentName)
        {
            HSSFWorkbook wb;
            using (FileStream fs = new FileStream(templateDocumentName, FileMode.Open, FileAccess.Read))
            {
                wb = new HSSFWorkbook(fs, true);
                return wb;
            }
        }

        private void SaveDocument(HSSFWorkbook wb, string templateDocumentName)
        {
            // Сохраняем книгу
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
            reportDate = calculateDate;
            tables = reportsDataService.GetSubjectReportData(
                regionId, currentVariantId, calculateDate.AddDays(1).ToShortDateString());
        }

        private string ReplaceTemplate(string value, string templateStr, object replaceValue)
        {
            if (value.Contains(templateStr))
            {
                value = value.Replace(templateStr, replaceValue.ToString());
            }

            return value;
        }

        private void FillDocumentVariables(HSSFWorkbook wb, D_S_TitleReport titleReport)
        {
            int maxRowNum = 150;
            int maxColNum = 40;
            string directorName = string.Empty;
            string accountantName = string.Empty;
            string directorTitle = string.Empty;
            string accountantTitle = string.Empty;
            if (titleReport != null)
            {
                if (titleReport.LastName != null)
                {
                    directorName = titleReport.LastName;
                }

                if (titleReport.LastAccountant != null)
                {
                    accountantName = titleReport.LastAccountant;
                }

                if (titleReport.TitleAccountant != null)
                {
                    accountantTitle = titleReport.TitleAccountant;
                }

                if (titleReport.TitleManager != null)
                {
                    directorTitle = titleReport.TitleManager;
                }
            }

            for (int k = 0; k < wb.NumberOfSheets; k++)
            {
                HSSFSheet sheet = wb.GetSheetAt(k);
                for (int i = 0; i < maxRowNum; i++)
                {
                    for (int j = 0; j < maxColNum; j++)
                    {
                        HSSFRow row = sheet.GetRow(i);
                        if (row != null)
                        {
                            HSSFCell cell = row.GetCell(j);
                            if (cell != null)
                            {
                                if (cell.CellType == 1)
                                {
                                    string cellValue = cell.StringCellValue;
                                    cellValue = ReplaceTemplate(cellValue, "REPORTDATE", reportDate.ToShortDateString());
                                    cellValue = ReplaceTemplate(cellValue, "DIRECTORNAME", directorName);
                                    cellValue = ReplaceTemplate(cellValue, "ACCOUNTANTNAME", accountantName);
                                    cellValue = ReplaceTemplate(cellValue, "ACCOUNTANTTITLE", accountantTitle);
                                    cellValue = ReplaceTemplate(cellValue, "DIRECTORTITLE", directorTitle);
                                    cell.SetCellValue(cellValue);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void FillData(
            HSSFWorkbook wb, 
            HSSFSheet sheet,
            DataTable tblData,
            int rowIndex, 
            int columnCount)
        {
            int rowCount = tblData.Rows.Count;
            DataRow summaryRow = null;
            foreach (DataRow row in tblData.Rows)
            {
                if (rowCount == 1)
                {
                    summaryRow = row;
                    break;
                }

                rowCount--;
                if (rowCount > 2)
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

        private void Render(string templateDocumentName, IList<DataTable> tables, D_S_TitleReport titleReport)
        {
            HSSFWorkbook wb = CreateDocument(templateDocumentName);
            FillDocumentVariables(wb, titleReport);

            HSSFSheet sheet = wb.GetSheetAt(0);

            FillData(wb, sheet, tables[2], 28, 23);
            FillData(wb, sheet, tables[1], 18, 18);
            FillData(wb, sheet, tables[0], 7, 18);

            // закрываем и уходим
            SaveDocument(wb, templateDocumentName);
        }
    }
}
