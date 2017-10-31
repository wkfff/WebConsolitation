using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public class DebtBookStavropolReport : Report
    {
        private const int ServiceColumnCount = 5;
        private readonly IDebtBookExportServiceStavropol reportsDataService;

        public DebtBookStavropolReport(IDebtBookExportServiceStavropol reportsDataService)
        {
            this.reportsDataService = reportsDataService;
        }

        public override string TemplateName
        {
            get { return "DebtorBookStavropolReport"; }
        }

        public override void Create(string templateDocumentName, int currentVariantId, int regionId, DateTime calculateDate, D_S_TitleReport titleReport)
        {
            var tables = PrepareData(currentVariantId, regionId, calculateDate);
            Render(templateDocumentName, tables, titleReport);
        }

        private DataTable[] PrepareData(int currentVariantId, int regionId, DateTime calculateDate)
        {
            return reportsDataService.GetDebtBookStavropolData(currentVariantId, regionId, calculateDate);
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

            for (var i = 0; i < tables.Count - 2; i++)
            {
                FillTableData(wb, i, tables[i], 11);
            }

            FillVaultSheet(wb, 5, tables[5]);

            var isNotSubject = Convert.ToBoolean(tblCaptions.Rows[0][0]);

            if (isNotSubject)
            {
                wb.SetSheetHidden(5, true);
            }

            // закрываем и уходим
            SaveDocument(wb, templateDocumentName);
        }

        private void SetRowsOrder(DataTable tbl, int offset)
        {
            for (var i = 0; i < tbl.Rows.Count; i++)
            {
                tbl.Rows[i][ServiceColumnCount] = offset + i + 1;
            }
        }

        private void FillVaultSheet(HSSFWorkbook wb, int sheetIndex, DataTable tblData)
        {
            const string TemplateSelect = "{0} = '{1}'";
            var sheet = wb.GetSheetAt(sheetIndex);
            var columnState = tblData.Columns[1].ColumnName;
            var columnOrder = tblData.Columns[ServiceColumnCount].ColumnName;

            var tblMR = ReportsDataService.FilterDataSet(tblData, String.Format(TemplateSelect, columnState, 0));
            var tblGO = ReportsDataService.FilterDataSet(tblData, String.Format(TemplateSelect, columnState, 1));
            var tblSB = ReportsDataService.FilterDataSet(tblData, String.Format(TemplateSelect, columnState, 2));
            tblMR = ReportsDataService.SortDataSet(tblMR, columnOrder);
            tblGO = ReportsDataService.SortDataSet(tblGO, columnOrder);
            tblSB = ReportsDataService.SortDataSet(tblSB, columnOrder);

            SetRowsOrder(tblMR, 0);
            SetRowsOrder(tblGO, tblMR.Rows.Count);
            SetRowsOrder(tblSB, tblMR.Rows.Count + tblGO.Rows.Count);

            const int IdxStartSB = 21;
            const int IdxStartMR = 12;
            const int IdxStartGO = 16;

            var lstCells = new Dictionary<int, int>
                               {
                                   { IdxStartGO, tblGO.Rows.Count }, 
                                   { IdxStartMR, tblMR.Rows.Count }
                               };

            foreach (var cellInfo in lstCells)
            {
                var rowIndex = cellInfo.Key + 2;
                var cell = NPOIHelper.GetCellByXY(sheet, rowIndex, 0);
                var cellValue = cell.StringCellValue;
                NPOIHelper.SetCellValue(sheet, rowIndex, 0, cellValue.Replace("COUNT", Convert.ToString(cellInfo.Value)));                
            }

            FillTableData(wb, sheetIndex, tblSB, IdxStartSB);
            FillTableData(wb, sheetIndex, tblGO, IdxStartGO);
            FillTableData(wb, sheetIndex, tblMR, IdxStartMR);
        }

        private HSSFCellStyle CreateSummaryStyle(HSSFWorkbook wb, bool isCaption)
        {
            const string FormatStr = "#,##0.00"; 
            var font = wb.CreateFont();
            font.FontName = "Arial Narrow";
            font.FontHeightInPoints = 14;
            font.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;

            var cellStyle = wb.CreateCellStyle();
            cellStyle.BorderTop = 1;
            cellStyle.BorderLeft = 1;
            cellStyle.BorderRight = 1;
            cellStyle.BorderBottom = 1;
            cellStyle.Alignment = isCaption ? HSSFCellStyle.ALIGN_LEFT : HSSFCellStyle.ALIGN_RIGHT;
            cellStyle.SetFont(font);
            cellStyle.GetFont(wb).Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            cellStyle.WrapText = true;

            if (!isCaption)
            {
                var format = HSSFDataFormat.GetBuiltinFormat(FormatStr);
                
                if (format < 0)
                {
                    format = wb.CreateDataFormat().GetFormat(FormatStr);
                }

                cellStyle.DataFormat = cellStyle.DataFormat = format;
            }

            return cellStyle;
        }

        private void FillTableData(HSSFWorkbook wb, int sheetIndex, DataTable tblData, int rowIndex)
        {
            var sheet = wb.GetSheetAt(sheetIndex);

            var styleSummaryLbl = CreateSummaryStyle(wb, true);
            var styleSummaryVal = CreateSummaryStyle(wb, false);

            for (var i = 0; i < tblData.Rows.Count - 2; i++)
            {
                NPOIHelper.CopyRow(wb, sheet, rowIndex, rowIndex + 1);
            }

            for (var i = 0; i < tblData.Rows.Count; i++)
            {
                var row = tblData.Rows[i];
                var recType = (RecordType)Convert.ToInt32(row[0]);
                sheet.GetRow(rowIndex).RowRecord.BadFontHeight = false;

                for (var j = ServiceColumnCount; j < tblData.Columns.Count; j++)
                {
                    var value = row[j];
                    var columnIndex = j - ServiceColumnCount;
                    decimal numValue = 0;

                    var cell = NPOIHelper.GetCellByXY(sheet, rowIndex, columnIndex);

                    if (cell != null && cell.CellType != HSSFCell.CELL_TYPE_FORMULA)
                    {
                        if (Decimal.TryParse(Convert.ToString(value), out numValue))
                        {
                            value = numValue;
                        }

                        NPOIHelper.SetCellValue(sheet, rowIndex, columnIndex, value);

                        if (recType != RecordType.Data)
                        {
                            cell.CellStyle = columnIndex == 0 ? styleSummaryLbl : styleSummaryVal;
                        }
                    }
                }

                rowIndex++;
            }

            sheet.FitToPage = true;
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
                        cellValue = ReplaceTemplate(cellValue, "REPORTDATE", ReportsDataService.GetDateValue(variantDate));
                        cellValue = ReplaceTemplate(cellValue, "DIRECTORNAME", directorName);
                        cellValue = ReplaceTemplate(cellValue, "DIRECTORTITLE", directorTitle);
                        cellValue = ReplaceTemplate(cellValue, "ACCOUNTANTNAME", accountantName);
                        cellValue = ReplaceTemplate(cellValue, "ACCOUNTANTTITLE", accountantTitle);
                        cellValue = ReplaceTemplate(cellValue, "CONTACTS", contact);

                        cellValue = ReplaceTemplate(cellValue, "REGION1", rowCaptions[3]);
                        cellValue = ReplaceTemplate(cellValue, "REGION2", rowCaptions[4]);

                        cellValue = ReplaceTemplate(cellValue, "INFOTEXT", rowCaptions[8]);
                        cellValue = ReplaceTemplate(cellValue, "CREDITAPPENDIX", rowCaptions[9]);
                        
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
