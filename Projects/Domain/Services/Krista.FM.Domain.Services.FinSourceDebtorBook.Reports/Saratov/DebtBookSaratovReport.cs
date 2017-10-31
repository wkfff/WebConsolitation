using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public class DebtBookSaratovReport : Report
    {
        private readonly ReportsDataService reportsDataService;

        public DebtBookSaratovReport(ReportsDataService reportsDataService)
        {
            this.reportsDataService = reportsDataService;
        }

        public override string TemplateName
        {
            get { return "DebtorBookSaratovReport"; }
        }

        public override void Create(string templateDocumentName, int currentVariantId, int regionId, DateTime calculateDate, D_S_TitleReport titleReport)
        {
            var tables = new DataTable[12];
            PrepareData(currentVariantId, regionId, calculateDate, ref tables);
            Render(templateDocumentName, tables, titleReport);
        }

        private void PrepareData(int currentVariantId, int regionId, DateTime calculateDate, ref DataTable[] tables)
        {
            reportsDataService.GetDebtBookSaratovData(currentVariantId, regionId, ref tables, calculateDate);
        }

        private string ReplaceTemplate(string value, string templateStr, object replaceValue)
        {
            if (value.Contains(templateStr))
            {
                value = value.Replace(templateStr, replaceValue.ToString());
            }

            return value;
        }

        private double GetNumber(object obj)
        {
            return reportsDataService.GetNumber(obj);
        }

        private void FillDocumentVariables(HSSFWorkbook wb, D_S_TitleReport titleReport, DataTable tblParams)
        {
            var directorName = String.Empty;
            var accountantName = String.Empty;
            var directorTitle = String.Empty;
            var accountantTitle = String.Empty;

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

            var rowCaptions = tblParams.Rows[0];
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
                        cellValue = ReplaceTemplate(cellValue, "REPORTDATE", rowCaptions[3]);
                        cellValue = ReplaceTemplate(cellValue, "REGIONNAME", rowCaptions[4]);
                        cellValue = ReplaceTemplate(cellValue, "DIRECTORNAME", directorName);
                        cellValue = ReplaceTemplate(cellValue, "ACCOUNTANTNAME", accountantName);
                        cellValue = ReplaceTemplate(cellValue, "ACCOUNTANTTITLE", accountantTitle);
                        cellValue = ReplaceTemplate(cellValue, "DIRECTORTITLE", directorTitle);
                        cellValue = ReplaceTemplate(cellValue, "YEARSTART", rowCaptions[1]);
                        cellValue = ReplaceTemplate(cellValue, "YEAR", rowCaptions[0]);

                        var monthDataIndex = -1;
                        if (cellValue.Contains("MO_ORG_CREDIT"))
                        {
                            monthDataIndex = 8;
                        }

                        if (cellValue.Contains("MO_BUDGET_CREDIT"))
                        {
                            monthDataIndex = 7;
                        }

                        if (cellValue.Contains("MO_GARANT"))
                        {
                            monthDataIndex = 9;
                        }

                        if (cellValue.Contains("MO_OTHER"))
                        {
                            monthDataIndex = 11;
                        }

                        if (monthDataIndex != -1)
                        {
                            cell.SetCellValue(GetNumber(rowCaptions[monthDataIndex]));
                        }
                        else
                        {
                            cell.SetCellValue(cellValue);
                        }
                    }
                }
            }
        }

        private void FillMOData(
            HSSFWorkbook wb, 
            HSSFSheet sheet, 
            DataTable tblData, 
            int rowIndex, 
            int colIndex, 
            int serviceColumns,
            bool copyRows,
            Collection<int> excludedColumns)
        {
            if (tblData == null)
            {
                return;
            }

            var rowCount = tblData.Rows.Count;
            var columnCount = tblData.Columns.Count - serviceColumns;
            DataRow summaryRow = null;

            foreach (DataRow row in tblData.Rows)
            {
                if (rowCount == 1 && copyRows)
                {
                    summaryRow = row;
                    break;
                }

                rowCount--;
                if (rowCount > 2)
                {
                    if (copyRows)
                    {
                        NPOIHelper.CopyRow(wb, sheet, rowIndex, rowIndex + 1);
                    }
                }

                for (var i = colIndex; i < columnCount; i++)
                {
                    if (excludedColumns.Contains(i))
                    {
                        continue;
                    }

                    double testNum;
                    var cellValue = Double.TryParse(row[i].ToString(), out testNum) ? testNum : row[i];
                    NPOIHelper.SetCellValue(sheet, rowIndex, i, cellValue);
                }

                sheet.GetRow(rowIndex).RowRecord.BadFontHeight = false;

                if (copyRows && row[0] == DBNull.Value)
                {
                    NPOIHelper.SetCellValue(sheet, rowIndex, 0, row[1]);
                    NPOIHelper.SetCellValue(sheet, rowIndex, 1, String.Empty);
                    var cellRange = new CellRangeAddress(rowIndex, rowIndex, 0, 5);
                    sheet.GetRow(rowIndex).GetCell(0).CellStyle.Alignment = HSSFCellStyle.ALIGN_LEFT;
                    sheet.AddMergedRegion(cellRange);
                }

                rowIndex++;
            }

            if (tblData.Rows.Count < 3)
            {
                rowIndex = rowIndex + 3 - tblData.Rows.Count;
            }

            if (summaryRow == null)
            {
                return;
            }

            for (var i = 1; i < columnCount; i++)
            {
                if (excludedColumns.Contains(i))
                {
                    continue;
                }

                double testNum;
                var cellValue = Double.TryParse(summaryRow[i].ToString(), out testNum) ? testNum : summaryRow[i];
                NPOIHelper.SetCellValue(sheet, rowIndex, i, cellValue);
            }
        }

        private void FillControlData(HSSFWorkbook wb, DataTable tblData)
        {
            if (tblData.Rows.Count <= 0)
            {
                return;
            }

            var sheet = wb.GetSheetAt(5);
            var rowIndex = 18;
            var colIndex = 1;
            var rowData = tblData.Rows[0];
            NPOIHelper.SetCellValue(sheet, rowIndex, colIndex + 0, GetNumber(rowData[0]));
            NPOIHelper.SetCellValue(sheet, rowIndex, colIndex + 1, GetNumber(rowData[1]));
            NPOIHelper.SetCellValue(sheet, rowIndex, colIndex + 2, GetNumber(rowData[2]));
            sheet = wb.GetSheetAt(6);
            rowIndex = 20;
            colIndex = 0;
            NPOIHelper.SetCellValue(sheet, rowIndex, colIndex + 0, GetNumber(rowData[3]));
            NPOIHelper.SetCellValue(sheet, rowIndex, colIndex + 1, GetNumber(rowData[4]));
            NPOIHelper.SetCellValue(sheet, rowIndex, colIndex + 2, GetNumber(rowData[5]));
        }

        private void FillServiceData(HSSFWorkbook wb, DataTable tblData)
        {
            var excludedColumns = new Collection<int> { 4, 5, 6 };
            FillMOData(wb, wb.GetSheetAt(6), tblData, 7, 1, 0, false, excludedColumns);
        }

        private void FillStructureData(HSSFWorkbook wb, DataTable tblData)
        {
            var excludedColumns = new Collection<int> { 1, 2, 5, 8 };
            FillMOData(wb, wb.GetSheetAt(5), tblData, 7, 1, 0, false, excludedColumns);
        }

        private void Render(string templateDocumentName, IList<DataTable> tables, D_S_TitleReport titleReport)
        {
            HSSFWorkbook wb;
            using (var fs = new FileStream(templateDocumentName, FileMode.Open, FileAccess.Read))
            {
                wb = new HSSFWorkbook(fs, true);
            }

            FillDocumentVariables(wb, titleReport, tables[10]);
            FillControlData(wb, tables[6]);
            FillServiceData(wb, tables[7]);

            FillStructureData(wb, tables[5]);

            var excludedColumns = new Collection<int>();

            // ЦБ
            var sheet = wb.GetSheetAt(0);
            FillMOData(wb, sheet, tables[0], 8, 0, 1, true, excludedColumns);

            // БЮДЖЕТНЫЕ КРЕДИТЫ
            sheet = wb.GetSheetAt(1);
            FillMOData(wb, sheet, tables[1], 7, 0, 1, true, excludedColumns);
            
            // КРЕДИТЫ ОРГАНИЗАЦИЙ
            sheet = wb.GetSheetAt(2);
            FillMOData(wb, sheet, tables[2], 10, 0, 1, true, excludedColumns);
            
            // ГАРАНТИИ
            sheet = wb.GetSheetAt(3);
            FillMOData(wb, sheet, tables[3], 10, 0, 1, true, excludedColumns);

            // ИНЫЕ
            sheet = wb.GetSheetAt(4);
            FillMOData(wb, sheet, tables[4], 10, 0, 1, true, excludedColumns);

            // СВОД
            if (tables[8] != null)
            {
                tables[8].Rows.Add();
            }

            if (tables[9] != null)
            {
                tables[9].Rows.Add();
            }

            sheet = wb.GetSheetAt(7);
            FillMOData(wb, sheet, tables[9], 14, 0, 0, true, excludedColumns);

            FillMOData(wb, sheet, tables[8], 06, 0, 0, true, excludedColumns);

            for (int i = 0; i < 8; i++)
            {
                wb.GetSheetAt(i).ForceFormulaRecalculation = true;
            }            

            ClearVaultList(wb, tables);

            // закрываем и уходим
            SaveDocument(wb, templateDocumentName);
        }

        private void ClearVaultList(HSSFWorkbook wb, IList<DataTable> tables)
        {
            var isSubject = Convert.ToBoolean(tables[tables.Count - 1].Rows[0][6]);
            
            if (!isSubject)
            {
                wb.RemoveSheetAt(7);
            }
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
