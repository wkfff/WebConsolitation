using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public class DebtBookMoscowReport : Report
    {
        private readonly IDebtBookExportServiceMoscow reportsDataService;
        private int regionNameIndex = 0;

        public DebtBookMoscowReport(IDebtBookExportServiceMoscow reportsDataService)
        {
            this.reportsDataService = reportsDataService;
        }

        public override string TemplateName
        {
            get { return "DebtorBookMoscowReport"; }
        }

        public override void Create(string templateDocumentName, int currentVariantId, int regionId, DateTime calculateDate, D_S_TitleReport titleReport)
        {
            var tables = PrepareData(currentVariantId, regionId, calculateDate);
            Render(templateDocumentName, tables, titleReport);
        }

        private DataTable[] PrepareData(int currentVariantId, int regionId, DateTime calculateDate)
        {
            return reportsDataService.GetDebtBookMoscowData(currentVariantId, regionId, calculateDate);
        }

        private void FillTableData(HSSFWorkbook wb, int sheetIndex, DataTable tblData, int rowIndex, bool isSettle = false)
        {
            const string MARK_COLUMN = "COLUMN";
            var markLength = MARK_COLUMN.Length;
            var sheet = wb.GetSheetAt(sheetIndex);

            var columnList = new Dictionary<int, int>();

            for (var i = 0; i < 250; i++)
            {
                var cellValue = NPOIHelper.GetCellStringValue(sheet, rowIndex, i);

                if (!cellValue.ToUpper().StartsWith(MARK_COLUMN))
                {
                    continue;
                }

                var dataIndex = Convert.ToInt32(cellValue.Remove(0, markLength));
                columnList.Add(i, dataIndex);
                NPOIHelper.SetCellValue(sheet, rowIndex, i, String.Empty);
            }

            var summaryRow = tblData.Rows[tblData.Rows.Count - 1];

            foreach (var columnInfo in columnList)
            {
                if (summaryRow[columnInfo.Value] != DBNull.Value)
                {
                    NPOIHelper.SetCellValue(sheet, rowIndex + 1, columnInfo.Key, summaryRow[columnInfo.Value]);
                }
            }

            for (var i = 0; i < tblData.Rows.Count - 1; i++)
            {
                NPOIHelper.CopyRow(wb, sheet, rowIndex, rowIndex + 1);
                NPOIHelper.SetCellValue(sheet, rowIndex + 1, 0, String.Empty);
            }

            rowIndex++;

            for (var i = 0; i < tblData.Rows.Count - 1; i++)
            {
                var row = tblData.Rows[i];

                foreach (var columnInfo in columnList)
                {
                    NPOIHelper.SetCellValue(sheet, rowIndex, columnInfo.Key, row[columnInfo.Value]);
                }

                if (isSettle)
                {
                    NPOIHelper.SetCellValue(sheet, rowIndex, 0, row[regionNameIndex]);                    
                }

                rowIndex++;
            }

            sheet.ForceFormulaRecalculation = true;
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
            
            // ЦБ с поселениями
            regionNameIndex = 29;
            FillTableData(wb, 0, tables[1], 25, true);
            FillTableData(wb, 0, tables[0], 22);
            FillTableData(wb, 0, tables[1], 15, true);
            FillTableData(wb, 0, tables[0], 12);
            
            // ЦБ без поселений
            FillTableData(wb, 1, tables[0], 16);
            FillTableData(wb, 1, tables[0], 11);

            // 2 Кредиты организаций с поселениями
            regionNameIndex = 17;
            FillTableData(wb, 2, tables[7], 21, true);
            FillTableData(wb, 2, tables[5], 19, true);
            FillTableData(wb, 2, tables[6], 15);
            FillTableData(wb, 2, tables[4], 13);

            // 2.1 Кредиты организаций без поселений
            FillTableData(wb, 3, tables[6], 14);
            FillTableData(wb, 3, tables[4], 12);

            // 3 Гарантии с поселениями
            regionNameIndex = 19;
            FillTableData(wb, 4, tables[11], 21, true);
            FillTableData(wb, 4, tables[09], 19, true);
            FillTableData(wb, 4, tables[10], 15);
            FillTableData(wb, 4, tables[08], 13);

            // 3.1 Гарантии без поселений
            FillTableData(wb, 5, tables[10], 14);
            FillTableData(wb, 5, tables[08], 12);

            // 4 Бюджетные кредиты с поселениями
            regionNameIndex = 14;
            FillTableData(wb, 6, tables[15], 21, true);
            FillTableData(wb, 6, tables[13], 19, true);
            FillTableData(wb, 6, tables[14], 15);
            FillTableData(wb, 6, tables[12], 13);

            // 4.1 Бюджетные кредиты без поселений
            FillTableData(wb, 7, tables[14], 14);
            FillTableData(wb, 7, tables[12], 12);

            // 5 Иные с поселениями
            regionNameIndex = 14;
            FillTableData(wb, 8, tables[19], 20, true);
            FillTableData(wb, 8, tables[17], 18, true);
            FillTableData(wb, 8, tables[18], 14);
            FillTableData(wb, 8, tables[16], 12);

            // 5.1 Иные без поселений
            FillTableData(wb, 9, tables[18], 13);
            FillTableData(wb, 9, tables[16], 11);

            if (tblCaptions.Rows.Count > 0)
            {
                var rowCaption = tblCaptions.Rows[0];
                var hasSettles = Convert.ToBoolean(rowCaption[0]);

                for (var i = wb.NumberOfSheets - 1; i >= 0; i--)
                {
                    var saveList = i % 2 == 0 == hasSettles;
                    wb.SetSheetHidden(i, !saveList);
                }
            }

            // закрываем и уходим
            SaveDocument(wb, templateDocumentName);
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
            var variantDate = Convert.ToDateTime(rowCaptions[6]);
            var monthText = GetMonthRusName(variantDate.Month);

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
                        cellValue = ReplaceTemplate(cellValue, "REPORTDAY", variantDate.Day);
                        cellValue = ReplaceTemplate(cellValue, "REPORTMONTHTEXT", monthText);
                        cellValue = ReplaceTemplate(cellValue, "REPORTYEAR", variantDate.Year);

                        cellValue = ReplaceTemplate(cellValue, "DIRECTORNAME", directorName);
                        cellValue = ReplaceTemplate(cellValue, "DIRECTORTITLE", directorTitle);
                        cellValue = ReplaceTemplate(cellValue, "ACCOUNTANTNAME", accountantName);
                        cellValue = ReplaceTemplate(cellValue, "ACCOUNTANTTITLE", accountantTitle);
                        cellValue = ReplaceTemplate(cellValue, "CONTACTS", contact);
                        
                        if (String.Compare(oldCellValue, cellValue, true) != 0)
                        {
                            cell.SetCellValue(cellValue);
                        }
                    }
                }
            }
        }

        private string GetMonthRusName(int monthNum)
        {
            switch (monthNum)
            {
                case 1:
                    return "января";
                case 2:
                    return "февраля";
                case 3:
                    return "марта";
                case 4:
                    return "апреля";
                case 5:
                    return "мая";
                case 6:
                    return "июня";
                case 7:
                    return "июля";
                case 8:
                    return "августа";
                case 9:
                    return "сентября";
                case 10:
                    return "октября";
                case 11:
                    return "ноября";
                case 12:
                    return "декабря";
            }

            return String.Empty;
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
