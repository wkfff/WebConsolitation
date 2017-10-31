using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public class DebtBookKalmykiaReport : Report
    {
        private const int ServiceColumnCount = 5;
        private readonly IDebtBookExportServiceKalmykia reportsDataService;

        public DebtBookKalmykiaReport(IDebtBookExportServiceKalmykia reportsDataService)
        {
            this.reportsDataService = reportsDataService;
        }

        public override string TemplateName
        {
            get { return "DebtorBookKalmykiaReport"; }
        }

        public override void Create(string templateDocumentName, int currentVariantId, int regionId, DateTime calculateDate, D_S_TitleReport titleReport)
        {
            var tables = PrepareData(currentVariantId, regionId, calculateDate);
            Render(templateDocumentName, tables, titleReport);
        }

        private DataTable[] PrepareData(int currentVariantId, int regionId, DateTime calculateDate)
        {
            return reportsDataService.GetDebtBookKalmykiaData(currentVariantId, regionId, calculateDate);
        }

        private void Render(string templateDocumentName, IList<DataTable> tables, D_S_TitleReport titleReport)
        {
            HSSFWorkbook wb;
            using (var fs = new FileStream(templateDocumentName, FileMode.Open, FileAccess.Read))
            {
                wb = new HSSFWorkbook(fs, true);
            }

            var tblCaptions = tables[tables.Count - 1];
            FillDocumentVariables(wb, titleReport, tblCaptions);

            var lstStartRows = new List<int> { 21, 19, 22, 22 };

            for (var i = 0; i < 4; i++)
            {
                FillTableData(wb, i, tables[i], lstStartRows[i]);
            }

            wb.GetSheetAt(4).ForceFormulaRecalculation = true;

            // закрываем и уходим
            SaveDocument(wb, templateDocumentName);
        }

        private void FillTableData(HSSFWorkbook wb, int sheetIndex, DataTable tblData, int rowIndex)
        {
            var sheet = wb.GetSheetAt(sheetIndex);

            for (var i = 0; i < tblData.Rows.Count - 2; i++)
            {
                NPOIHelper.CopyRow(wb, sheet, rowIndex, rowIndex + 1);
            }

            for (var i = 0; i < tblData.Rows.Count; i++)
            {
                NPOIHelper.SetCellValue(sheet, rowIndex, 0, i + 1);

                var row = tblData.Rows[i];
                sheet.GetRow(rowIndex).RowRecord.BadFontHeight = false;

                for (var j = ServiceColumnCount; j < tblData.Columns.Count; j++)
                {
                    var value = row[j];
                    var columnIndex = j - ServiceColumnCount + 1;

                    var cell = NPOIHelper.GetCellByXY(sheet, rowIndex, columnIndex);

                    if (cell == null || cell.CellType == HSSFCell.CELL_TYPE_FORMULA)
                    {
                        continue;
                    }

                    decimal numValue;

                    if (Decimal.TryParse(Convert.ToString(value), out numValue))
                    {
                        value = numValue;
                    }

                    NPOIHelper.SetCellValue(sheet, rowIndex, columnIndex, value);
                }

                rowIndex++;
            }

            sheet.ForceFormulaRecalculation = true;
        }

        private void FillDocumentVariables(HSSFWorkbook wb, D_S_TitleReport titleReport, DataTable tblParams)
        {
            var directorName = String.Empty;
            var accountantName = String.Empty;
            var directorTitle = String.Empty;
            var accountantTitle = String.Empty;
            var contact = String.Empty;

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

                if (titleReport.Contacts != null)
                {
                    contact = titleReport.Contacts;
                }
            }

            var rowCaptions = tblParams.Rows[0];
            var variantDate = ReportsDataService.GetDateValue(rowCaptions[6]);

            for (var k = 0; k < wb.NumberOfSheets; k++)
            {
                var sheet = wb.GetSheetAt(k);

                for (var i = 0; i < sheet.LastRowNum + 1; i++)
                {
                    for (var j = 0; j < 255; j++)
                    {
                        var row = sheet.GetRow(i);

                        if (row == null)
                        {
                            continue;
                        }

                        var cell = row.GetCell(j);

                        if (cell == null || cell.CellType != 1)
                        {
                            continue;
                        }

                        var cellValue = cell.StringCellValue;
                        var oldCellValue = cellValue;
                        cellValue = ReplaceTemplate(cellValue, "REPORTDATE", variantDate);
                        cellValue = ReplaceTemplate(cellValue, "DIRECTORNAME", directorName);
                        cellValue = ReplaceTemplate(cellValue, "DIRECTORTITLE", directorTitle);
                        cellValue = ReplaceTemplate(cellValue, "ACCOUNTANTNAME", accountantName);
                        cellValue = ReplaceTemplate(cellValue, "ACCOUNTANTTITLE", accountantTitle);
                        cellValue = ReplaceTemplate(cellValue, "CONTACTS", contact);
                        cellValue = ReplaceTemplate(cellValue, "REGION", rowCaptions[3]);
                        
                        if (String.Compare(oldCellValue, cellValue, true) != 0)
                        {
                            cell.SetCellValue(cellValue);
                        }
                    }
                }
            }
        }

        private string ReplaceTemplate(string value, string templateStr, object replaceValue)
        {
            if (value.Contains(templateStr))
            {
                value = value.Replace(templateStr, replaceValue.ToString());
            }

            return value;
        }

        private void SaveDocument(HSSFWorkbook wb, string templateDocumentName)
        {
            // Сохраняем книгу
            var dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI";
            wb.DocumentSummaryInformation = dsi;
            var si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI";
            wb.SummaryInformation = si;
            
            using (var file = new FileStream(templateDocumentName, FileMode.Create))
            {
                wb.Write(file);
                file.Close();
            }
        }
    }
}
