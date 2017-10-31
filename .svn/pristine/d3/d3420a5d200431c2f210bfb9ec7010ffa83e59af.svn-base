using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public class DebtorBookYarReport : Report
    {
        protected readonly ReportsDataService DataService;
        private static DateTime calcDate = DateTime.Now;
        private static DateTime originCalcDate = DateTime.Now;

        public DebtorBookYarReport(ReportsDataService reportsDataService)
        {
            DataService = reportsDataService;
        }

        public override string TemplateName
        {
            get { return "DebtorBookYarReport"; }
        }

        public static void HideRows(HSSFSheet sheet, bool isMR)
        {
            for (var delIndex = sheet.LastRowNum; delIndex > 0; delIndex--)
            {
                var rowScan = sheet.GetRow(delIndex);

                if (rowScan == null)
                {
                    continue;
                }

                var cell = rowScan.GetCell(1);

                if (cell == null || cell.CellType != 1)
                {
                    continue;
                }

                var cellValue = cell.StringCellValue;

                if (isMR || (!cellValue.Contains("(поселений)") && !cellValue.Contains("(консолидированный)")))
                {
                    continue;
                }

                for (var i = 15; i >= 0; i--)
                {
                    var row = sheet.GetRow(delIndex + i);
                    
                    if (row == null)
                    {
                        continue;
                    }

                    row.RemoveAllCells();
                    row.Height = 1;
                    row.ZeroHeight = true;
                }
            }
        }

        public override void Create(string templateDocumentName, int currentVariantId, int regionId, DateTime calculateDate, D_S_TitleReport titleReport)
        {
            calcDate = calculateDate;
            originCalcDate = calculateDate;
            if (calcDate.Day == 1)
            {
                calcDate = calcDate.AddDays(-1);
            }

            var tables = new DataTable[1];
            var titles = new Dictionary<string, string>();
            PrepareData(currentVariantId, regionId, calculateDate, ref tables, ref titles);
            Render(templateDocumentName, tables, titleReport, titles);
        }

        private static void AddEmptyRow(HSSFSheet sheet, int rowIndex)
        {
            sheet.ShiftRows(rowIndex, sheet.LastRowNum, 1);
            var row1 = sheet.GetRow(rowIndex);

            for (var ii = 0; ii < 22; ii++)
            {
                row1.CreateCell(ii);
            }
        }

        private static void WriteSummaryTable(HSSFSheet sheet, DataTable tblData, int startRowIndex)
        {
            var rowIndex = startRowIndex;

            foreach (DataRow row in tblData.Rows)
            {
                for (var j = 1; j < row.ItemArray.Length - 1; j++)
                {
                    if (j != 6 && j != 12)
                    {
                        NPOIHelper.SetCellValue(sheet, rowIndex - 1, j, Convert.ToDouble(row[j]));
                    }
                }

                rowIndex++;
            }

            var resultRowIndex = rowIndex - 3;
            var restRowIndex = resultRowIndex + 1;

            for (var j = 0; j < 3; j++)
            {
                var offset = j * 6;
                NPOIHelper.SetCellValue(sheet, resultRowIndex, 01 + offset, "X");
                NPOIHelper.SetCellValue(sheet, restRowIndex, 01 + offset, "X");
            }
        }

        private static void WriteMonthData(HSSFSheet sheet, DataRow rowData)
        {
            var lstLimits = new Dictionary<string, int>
                                {
                                    { "МД_КРЕД_ОРГ", 0 },
                                    { "МД_КРЕД_БЮДЖ", 1 },
                                    { "МД_КРЕД_ГАР", 2 },
                                    { "МД_КРЕД_ЦБ", 3 }
                                };

            for (var i = 0; i < 250; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    var cellValue = NPOIHelper.GetCellStringValue(sheet, i, j);

                    foreach (var limitInfo in lstLimits.Where(limitInfo => cellValue.Contains(limitInfo.Key)))
                    {
                        cellValue = cellValue.Replace(limitInfo.Key, rowData[limitInfo.Value].ToString());
                        NPOIHelper.SetCellValue(sheet, i, j, cellValue);
                    }
                }
            }
        }

        private static void Render(
            string templateDocumentName, 
            IList<DataTable> tables, 
            D_S_TitleReport titleReport,
            Dictionary<string, string> titles)
        {
            HSSFWorkbook wb;
            
            using (var fs = new FileStream(templateDocumentName, FileMode.Open, FileAccess.Read))
            {
                wb = new HSSFWorkbook(fs, true);
            }

            /* 0-3 основные, 4-7 их итоги, 8-9 поселения кредитов, 10-11 консолидированные кредитов
                12 прямой долг мо, 13 прямой долг послений, 14 прямой долг конс, 15 муниц долг мо
                16 муниц долг послений, 17 муниц долг конс */
            const int ServiceTableCount = 1;
            var tableIndexes = new Dictionary<int, int>();

            tableIndexes.Add(00, 10);
            tableIndexes.Add(01, 60);
            tableIndexes.Add(02, 111);
            tableIndexes.Add(03, 177);

            tableIndexes.Add(04, 12);
            tableIndexes.Add(05, 62);
            tableIndexes.Add(06, 113);
            tableIndexes.Add(07, 179);
            tableIndexes.Add(08, 28);
            tableIndexes.Add(09, 78);
            tableIndexes.Add(10, 44);
            tableIndexes.Add(11, 94);
            tableIndexes.Add(12, 129);
            tableIndexes.Add(13, 145);
            tableIndexes.Add(14, 161);
            tableIndexes.Add(15, 195);
            tableIndexes.Add(16, 211);
            tableIndexes.Add(17, 228);

            var tblMonthData = tables[tables.Count - 1];
            var rowFlagEmptyDS = tables[tables.Count - 2].Rows[0];
            var ci = new CultureInfo("RU-ru");
            var fullMonthList = ci.DateTimeFormat.MonthNames;
            var notUseMonths = new Collection<string>();

            for (var i = calcDate.Month; i < 12; i++)
            {
                notUseMonths.Add(fullMonthList[i].ToLower());                
            }

            var sheetCount = (tables.Count - ServiceTableCount - 7) / tableIndexes.Count;

            for (var sheetNum = 0; sheetNum < sheetCount; sheetNum++)
            {
                var isMR = Convert.ToInt32(rowFlagEmptyDS[sheetNum]) > 0;
                var sheet = wb.CloneSheet(1);

                var deltaTable = sheetNum * 18;
                var regionName = tables[4 + deltaTable].Rows[0][tables[4 + deltaTable].Columns.Count - 1].ToString();

                // Исполнитель + телефон на листе МР\ГО
                var rowTitle1 = sheet.CreateRow(243);
                rowTitle1.CreateCell(0);
                rowTitle1.CreateCell(3);
                var currentTitle = titleReport.LastName;

                if (titles.ContainsKey(regionName))
                {
                    currentTitle = titles[regionName];
                }

                NPOIHelper.SetCellValue(sheet, 243, 0, currentTitle);

                WriteMonthData(sheet, tblMonthData.Rows[sheetNum + 1]);
                var offset = sheetNum * 18;

                for (var i = 4; i < 18; i++)
                {
                    WriteSummaryTable(sheet, tables[i + offset], tableIndexes[i]);
                }

                for (var i = 3; i >= 0; i--)
                {
                    var recCounter = new Dictionary<int, int>();
                    var rowIndex = tableIndexes[i] - 1;
                    var tableIndex = 0;
                    var recCount = 0;
                    var delta = sheetNum * 18;

                    foreach (DataRow row in tables[i + delta].Rows)
                    {
                        var rowType = Convert.ToInt32(row[row.ItemArray.Length - 1]);

                        if (rowType == 1)
                        {
                            AddEmptyRow(sheet, rowIndex++);
                        }

                        if (rowType == 2)
                        {
                            recCount++;
                        }

                        if (rowType == 3)
                        {
                            recCounter.Add(tableIndex, recCount);
                            recCount = 0;
                            tableIndex++;
                        }
                    }

                    tableIndex = 0;
                    rowIndex = tableIndexes[i] - 1;
                    recCount = 0;
                    var tableOffset = sheetNum * 18;

                    foreach (DataRow row in tables[i + tableOffset].Rows)
                    {
                        var rowType = Convert.ToInt32(row[row.ItemArray.Length - 1]);

                        if (rowType == 1)
                        {
                            NPOIHelper.CopyRows(wb, sheet, 6, 9, rowIndex + 1);
                            sheet.GetRow(rowIndex).GetCell(0).SetCellValue(row[0].ToString());
                            row[0] = "На начало года";
                            rowIndex++;
                        }
                        else
                        {
                            if (rowType != 3)
                            {
                                recCount++;
                                if (recCounter[tableIndex] > recCount)
                                {
                                    NPOIHelper.CopyRow(wb, sheet, rowIndex, rowIndex + 1);
                                }
                            }
                            else
                            {
                                tableIndex++;
                                recCount = 0;
                            }
                        }

                        var outRowIndex = rowIndex;

                        if (rowType == 3)
                        {
                            if (recCounter[tableIndex - 1] == 0)
                            {
                                outRowIndex++;
                            }
                        }

                        for (var j = 0; j < row.ItemArray.Length - 1; j++)
                        {
                            if (row[j] == DBNull.Value)
                            {
                                continue;
                            }

                            if (j > 0 && j != 6 && j != 12)
                            {
                                NPOIHelper.SetCellValue(sheet, outRowIndex, j, Convert.ToDouble(row[j]));
                            }
                            else
                            {
                                if (!(rowType == 3 && (j == 6 || j == 12)))
                                {
                                    NPOIHelper.SetCellValue(sheet, outRowIndex, j, row[j]);
                                }
                            }
                        }

                        if (rowType == 3)
                        {
                            NPOIHelper.SetCellValue(sheet, outRowIndex, 01, "X");
                            NPOIHelper.SetCellValue(sheet, outRowIndex, 07, "X");
                            NPOIHelper.SetCellValue(sheet, outRowIndex, 13, "X");
                            rowIndex = outRowIndex + 1;
                        }
                        else
                        {
                            rowIndex++;
                        }
                    }
                }

                NPOIHelper.SetCellValue(sheet, 0, 0, string.Format("{0} по состоянию на {1}", regionName, originCalcDate.ToShortDateString()));
                regionName = regionName.Replace(" муниципальный район", string.Empty).Replace(" район", String.Empty);

                if (regionName == string.Empty)
                {
                    regionName = "Неизвестно";
                }

                sheet.book.SetSheetName(sheetNum + 2, regionName);
                NPOIHelper.DeleteRows(sheet, 5, 8);
                ClearEmptyRows(sheet, notUseMonths);
                HideRows(sheet, isMR);
            }

            var sheetSummary = wb.GetSheetAt(0);
            WriteMonthData(sheetSummary, tblMonthData.Rows[0]);
            tableIndexes.Clear();
            tableIndexes.Add(01, 6);
            tableIndexes.Add(02, 22);
            tableIndexes.Add(03, 41);
            tableIndexes.Add(04, 57);
            tableIndexes.Add(05, 76);
            tableIndexes.Add(06, 92);

            // Исполнитель + телефон на листе сводных итогов
            var rowTitle = sheetSummary.CreateRow(108);
            rowTitle.CreateCell(0);
            rowTitle.CreateCell(3);
            NPOIHelper.SetCellValue(sheetSummary, 108, 0, titleReport.LastName);
            var cellHeader = sheetSummary.GetRow(0).GetCell(0);
            NPOIHelper.SetCellValue(sheetSummary, 0, 0, String.Format("{0} {1}", cellHeader.StringCellValue, originCalcDate.ToShortDateString()));

            for (var i = 1; i < 7; i++)
            {
                WriteSummaryTable(sheetSummary, tables[tables.Count - 7 + i - ServiceTableCount - 1], tableIndexes[i]);
            }

            ClearEmptyRows(sheetSummary, notUseMonths);
            wb.RemoveSheetAt(1);

            if (sheetCount == 1)
            {
                wb.RemoveSheetAt(0);
            }

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

        private static void ClearEmptyRows(HSSFSheet sheet, Collection<string> notUseMonths)
        {
            // Поехали грохать лишние строчки (итоги по ненужным месяцам и просроченную задолженность)
            for (var delIndex = sheet.LastRowNum; delIndex > 0; delIndex--)
            {
                var row = sheet.GetRow(delIndex);

                if (row == null)
                {
                    continue;
                }

                if (delIndex > 10)
                {
                    row.RowRecord.BadFontHeight = false;
                }

                var cell = row.GetCell(0);
                
                if (cell == null || cell.CellType != 1)
                {
                    continue;
                }

                var cellValue = cell.StringCellValue;
                var notUsedRow = notUseMonths.Contains(cellValue.ToLower());
                var borderTop = cell.CellStyle.BorderTop;
                var borderBottom = cell.CellStyle.BorderBottom;

                if (!notUsedRow)
                {
                    continue;
                }

                row.RemoveAllCells();
                row.Height = 1;
                row.ZeroHeight = true;
                var rowNext = sheet.GetRow(delIndex + 1);
                
                if (rowNext == null)
                {
                    continue;
                }

                for (var i = 0; i < rowNext.PhysicalNumberOfCells; i++)
                {
                    cell.CellStyle.BorderBottom = borderBottom;
                    cell.CellStyle.BorderTop = borderTop;
                }
            }
        }

        private void PrepareData(
            int currentVariantId, 
            int regionId, 
            DateTime calculateDate, 
            ref DataTable[] tables,
            ref Dictionary<string, string> titles)
        {
            DataService.GetDebtorBookYarData(currentVariantId, regionId, ref tables, calculateDate, ref titles);
        }
    }
}
