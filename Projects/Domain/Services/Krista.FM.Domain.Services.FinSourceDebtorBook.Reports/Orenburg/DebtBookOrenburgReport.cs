using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public class DebtorBookOrenburgReport : Report
    {
        private readonly ReportsDataService reportsDataService;
        private int formVersion = 1;

        public DebtorBookOrenburgReport(ReportsDataService reportsDataService)
        {
            this.reportsDataService = reportsDataService;
        }

        public enum ReportType
        {
            /// <summary>
            /// полная долговая
            /// </summary>
            Full,

            /// <summary>
            /// районная долговая
            /// </summary>
            Region,

            /// <summary>
            /// поселение из долговой
            /// </summary>
            Settles
        }

        public override string TemplateName
        {
            get { return GetReportVersion() == 1 ? "DebtorBookOrenburgReport" : "DebtorBookOrenburgReportNew2012"; }
        }

        public virtual int GetReportVersion()
        {
            if (ReportDate.Year > 2012 || (ReportDate.Year == 2012 && ReportDate.Month > 1))
            {
                return 2;
            }

            return 1;
        }

        public override void Create(string templateDocumentName, int currentVariantId, int regionId, DateTime calculateDate, D_S_TitleReport titleReport)
        {
            var tables = new DataTable[1];
            PrepareData(currentVariantId, regionId, calculateDate, ref tables);
            Render(templateDocumentName, tables, titleReport);
        }

        protected virtual List<int> ListLimitDeleteRows()
        {
            return new List<int>();
        }

        protected virtual List<int> ListLimitSummaryDeleteRows()
        {
            return new List<int>();
        }

        protected virtual ReportType GetReportType()
        {
            return ReportType.Full;
        }

        private string ReplaceTemplate(string value, string templateStr, object replaceValue)
        {
            if (value.Contains(templateStr))
            {
                value = value.Replace(templateStr, replaceValue.ToString());
            }

            return value;
        }

        private void FillCaptionParams(HSSFWorkbook wb, DataTable dt, D_S_TitleReport titleReport)
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

            var rowCaptions = dt.Rows[0];
            var year = Convert.ToInt32(rowCaptions[0]);

            if (Convert.ToBoolean(rowCaptions[42]))
            {
                var sheetOrgCredit = wb.GetSheetAt(0);
                var reportCaption = String.Format(
                    "Информация по долговым обязательствам поселений по состоянию на {0}г.",
                    rowCaptions[3]);
                NPOIHelper.SetCellValue(sheetOrgCredit, 5, 0, reportCaption);
                var row = sheetOrgCredit.CreateRow(6);
                var cell = row.CreateCell(0);
                cell.SetCellValue(Convert.ToString(rowCaptions[4]));
            }

            var replaceList = new Dictionary<string, object>
                                  {
                                      { "YEARSTART", rowCaptions[1] },
                                      { "MONTHNUM", rowCaptions[2] },
                                      { "REPORTDATE", rowCaptions[3] },
                                      { "MONAME", rowCaptions[4] },
                                      { "MONTHTEXT", rowCaptions[5] },
                                      { "YEARNUM1", year + 1 },
                                      { "YEARNUM2", year + 2 },
                                      { "YEARNUM3", year + 3 },
                                      { "YEARNUM", year },
                                      { "DIRECTORNAME", directorName },
                                      { "ACCOUNTANTNAME", accountantName },
                                      { "DIRECTORTITLE", directorTitle },
                                      { "ACCOUNTANTTITLE", accountantTitle }
                                  };

            for (var k = 0; k < wb.NumberOfSheets; k++)
            {
                var sheet = wb.GetSheetAt(k);

                for (var i = 0; i < sheet.LastRowNum; i++)
                {
                    for (var j = 0; j < 40; j++)
                    {
                        var cell = NPOIHelper.GetCellByXY(sheet, i, j);

                        if (cell == null || cell.CellType != 1)
                        {
                            continue;
                        }

                        var cellValue = replaceList.Aggregate(
                            cell.StringCellValue, 
                            (current, replaceInfo) => 
                                ReplaceTemplate(current, replaceInfo.Key, replaceInfo.Value));

                        cell.SetCellValue(cellValue);
                    }
                }
            }
        }

        private int GetHiddenColumnCount(int docType)
        {
            var hiddenColumnCount = 5;

            if (docType == 1)
            {
                hiddenColumnCount += 2;
            }

            if (docType == 0 || docType == 1 || docType == 4)
            {
                hiddenColumnCount += 2;
            }

            return hiddenColumnCount;
        }

        private void FillTablesData(
            HSSFWorkbook book, 
            HSSFSheet sheet, 
            DataTable dt, 
            int startRow, 
            int contractType,
            bool isSettle)
        {
            if (dt == null)
            {
                return;
            }

            var addCount = dt.Rows.Count - 2;
            var settleList = new List<string>();

            if (isSettle)
            {
                var settles = from f in dt.Select().ToList()
                                 let settleName = Convert.ToString(f[dt.Columns.Count - 1])
                                 group f by new { settleName }
                                     into g
                                     select g.Key.settleName;
                settleList = settles.ToList();
                settleList.Sort();
                settleList.Remove(String.Empty);
                addCount += settleList.Count;
            }

            for (var i = 0; i < addCount; i++)
            {
                NPOIHelper.CopyRow(book, sheet, startRow, startRow + 1);
            }

            if (dt.Rows.Count < 2)
            {
                startRow++;
            }

            var hiddenColumnCount = GetHiddenColumnCount(contractType);
            var rowIndex = startRow - 1;
            var columnCount = dt.Columns.Count - hiddenColumnCount;
            var rowCounter = 1;

            if (isSettle)
            {
                foreach (var settleName in settleList)
                {
                    var rowsSettle =
                        dt.Select(String.Format("{0}='{1}'", dt.Columns[dt.Columns.Count - 1].ColumnName, settleName));

                    var cell = NPOIHelper.GetCellByXY(sheet, rowIndex, 0);

                    if (cell != null)
                    {
                        cell.CellStyle.Alignment = HSSFCellStyle.ALIGN_LEFT;
                    }

                    NPOIHelper.SetCellValue(sheet, rowIndex, 0, settleName);

                    var rangeAddress = new CellRangeAddress(
                        rowIndex,
                        rowIndex,
                        0,
                        columnCount - 1);

                    sheet.AddMergedRegion(rangeAddress);
                    rowIndex++;

                    foreach (var dataRow in rowsSettle)
                    {
                        for (var j = 0; j < columnCount; j++)
                        {
                            var value = dataRow[j];
                            double result;

                            if (Double.TryParse(value.ToString(), out result))
                            {
                                value = result;
                            }

                            if (j == 0)
                            {
                                value = rowCounter;
                            }

                            NPOIHelper.SetCellValue(sheet, rowIndex, j, value);
                        }

                        rowIndex++;
                        rowCounter++;
                    }
                }

                var summaryRow = dt.Rows[dt.Rows.Count - 1];

                for (var j = 0; j < columnCount; j++)
                {
                    var value = summaryRow[j];
                    double result;

                    if (Double.TryParse(value.ToString(), out result))
                    {
                        value = result;
                    }

                    NPOIHelper.SetCellValue(sheet, rowIndex, j, value);
                }
            }
            else
            {
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    var rowData = dt.Rows[i];

                    for (var j = 0; j < columnCount; j++)
                    {
                        var value = rowData[j];
                        double result;

                        if (Double.TryParse(value.ToString(), out result))
                        {
                            value = result;
                        }

                        NPOIHelper.SetCellValue(sheet, rowIndex, j, value);
                    }

                    rowIndex++;
                }
            }
        }

        private void FillSubjectRows(HSSFWorkbook book, HSSFSheet sheet, DataRow[] drsSelect, int startRow, int hiddenColumnCount)
        {
            for (var i = 0; i < drsSelect.Length - 2; i++)
            {
                NPOIHelper.CopyRow(book, sheet, startRow, startRow + 1);
            }

            for (var i = 0; i < drsSelect.Length; i++)
            {
                for (var j = 1; j < drsSelect[i].Table.Columns.Count - hiddenColumnCount; j++)
                {
                    var value = drsSelect[i][j];
                    double result;

                    if (value == DBNull.Value || value == null)
                    {
                        value = 0;
                    }

                    if (Double.TryParse(value.ToString(), out result))
                    {
                        value = result;
                    }

                    NPOIHelper.SetCellValue(sheet, startRow + i, j - 1, value);
                }
            }
        }

        private void FillRegion(HSSFSheet sheet, DataRow[] drsSelect, int startRow, int column)
        {
            for (var i = 0; i < drsSelect.Length; i++)
            {
                if (drsSelect[i][2] != DBNull.Value)
                {
                    NPOIHelper.SetCellValue(sheet, startRow + i, column, Convert.ToDouble(drsSelect[i][2]));
                }
            }
        }

        private DataRow[] SelectRegionsByType(DataTable dt, int regionType)
        {
            return dt.Select(String.Format("{0} = '{1}'", dt.Columns[0].ColumnName, regionType));
        }

        private void CorrectSummaryFormula(HSSFSheet sheet, ICollection<DataRow> rows, int startRow, int startCol, int endCol)
        {
            // проблемка с копированием формул в объекте, он не сдвигает индексы строк как эксель
            for (var i = 0; i < rows.Count; i++)
            {
                NPOIHelper.SetCellFormula(sheet, startRow + i, startCol - 1, String.Format("SUM(C{0}:G{0})", startRow + i + 1));
                
                for (var j = startCol; j <= endCol; j++)
                {
                    var oldformula = NPOIHelper.GetCellFormula(sheet, startRow, j);

                    if (oldformula.Length <= 0)
                    {
                        continue;
                    }

                    var k = oldformula.Length - 1;

                    while (oldformula[k] >= '0' && oldformula[k] <= '9')
                    {
                        k--;
                    }

                    if (k <= 0)
                    {
                        continue;
                    }

                    k++;
                    var rowNum = Convert.ToInt32(oldformula.Substring(k, oldformula.Length - k));
                    var newFormula = oldformula.Substring(0, k) + Convert.ToString(rowNum + i);
                    NPOIHelper.SetCellFormula(sheet, startRow + i, j, newFormula);
                }
            }
        }

        private void CreateSubjectSummaryRows(
            HSSFWorkbook book, 
            HSSFSheet sheet, 
            DataTable tblRegion, 
            DataTable tblSettle, 
            int startRow)
        {
            var rowTown = SelectRegionsByType(tblRegion, 1);
            var rowSelo = SelectRegionsByType(tblRegion, 2);
            var hiddenColumnCount = tblRegion.Columns.Count - 2;

            startRow -= 2;
            FillSubjectRows(book, sheet, rowSelo, startRow, hiddenColumnCount);

            if (tblSettle != null)
            {
                var rowSeloSettle = SelectRegionsByType(tblSettle, 2);
                FillRegion(sheet, rowSeloSettle, startRow, 7);
            }

            CorrectSummaryFormula(sheet, rowSelo, startRow, 2, 7);

            startRow -= 3;
            FillSubjectRows(book, sheet, rowTown, startRow, hiddenColumnCount);
            CorrectSummaryFormula(sheet, rowTown, startRow, 2, 6);

            sheet.ForceFormulaRecalculation = true;
        }

        private void FillSubjectData(HSSFWorkbook book, HSSFSheet sheet, DataTable dt, int startRow, int contractType)
        {
            startRow--;
            var rowTown = SelectRegionsByType(dt, 1);
            var rowSelo = SelectRegionsByType(dt, 2);

            var hiddenColumnCount = 0;
            
            if (contractType == 8 || contractType == 7)
            {
                hiddenColumnCount = 2;
            }

            // итоговая строка
            var rowSummary = dt.Rows[dt.Rows.Count - 1];

            for (var j = 2; j < dt.Columns.Count - hiddenColumnCount; j++)
            {
                if (rowSummary[j - 1] != DBNull.Value)
                {
                    NPOIHelper.SetCellValue(sheet, startRow, j - 1, Convert.ToDouble(rowSummary[j - 1]));
                }
            }
            
            // селения Руси Великой
            startRow -= 2;
            FillSubjectRows(book, sheet, rowSelo, startRow, hiddenColumnCount);
            
            // города златоглавые
            startRow -= 3;
            FillSubjectRows(book, sheet, rowTown, startRow, hiddenColumnCount);
        }

        private void FillStructureTable(HSSFWorkbook book, DataTable dt)
        {
            var sheet = book.GetSheetAt(5);
            var rowNumbers = new Collection<int> { 10, 13, 17, 21, 24, 27 };

            for (var i = 0; i < dt.Rows.Count; i++)
            {
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j] != DBNull.Value)
                    {
                        NPOIHelper.SetCellValue(sheet, rowNumbers[i], j + 4, Convert.ToDouble(dt.Rows[i][j]));
                    }
                }
            }
        }

        private void FillMonthReportData(HSSFWorkbook book, DataTable dt)
        {
            var rowCaption = dt.Rows[0];
            var sheet = book.GetSheetAt(0);
            var orgMonthDataIndex = formVersion == 1 ? 52 : 37;
            NPOIHelper.SetCellValue(sheet, orgMonthDataIndex, 20, Convert.ToDouble(rowCaption[12]));
            sheet = book.GetSheetAt(1);
            NPOIHelper.SetCellValue(sheet, 13, 21, Convert.ToDouble(rowCaption[13]));
            sheet = book.GetSheetAt(2);
            var grnMonthColIndex = formVersion == 1 ? 29 : 28;
            NPOIHelper.SetCellValue(sheet, 17, grnMonthColIndex, Convert.ToDouble(rowCaption[14]));
            sheet = book.GetSheetAt(3);
            NPOIHelper.SetCellValue(sheet, 12, 22, Convert.ToDouble(rowCaption[15]));
            sheet = book.GetSheetAt(4);
            NPOIHelper.SetCellValue(sheet, 13, 25, Convert.ToDouble(rowCaption[16]));
            sheet = book.GetSheetAt(5);

            for (var i = 0; i < 5; i++)
            {
                var colIndex = formVersion == 1 ? 12 : 11;
                NPOIHelper.SetCellValue(sheet, 30 + i, colIndex, Convert.ToDouble(rowCaption[07 + i]));
                NPOIHelper.SetCellValue(sheet, 30 + i, colIndex + 1, Convert.ToDouble(rowCaption[12 + i]));
            }
        }

        private void FillLimitTable(HSSFWorkbook book, DataTable dt, bool isSubject)
        {
            if (dt.Rows.Count <= 0)
            {
                return;
            }

            var limitFields = new Collection<string>
                              {
                                  "ConsMunDebt",
                                  "MunDebt",
                                  "PosDebt",
                                  "ConsMunGrnt",
                                  "MunGrnt",
                                  "PosGrnt",
                                  "ConsMunService",
                                  "MunService",
                                  "PosService",
                                  "MunIssue"
                              };

            var sheetNumber = 0;
            var rowNumber = 7;

            if (isSubject)
            {
                sheetNumber = 6;
                rowNumber = 3;
            }

            var sheet = book.GetSheetAt(sheetNumber);
            const int ColumnNumber = 0;
            var dr = dt.Rows[0];

            // лимиты на 3 года
            for (var i = 0; i < 3; i++)
            {
                rowNumber++;

                foreach (var lf in limitFields)
                {
                    var sumValue = 0.0;

                    foreach (var singleField in lf.Split(','))
                    {
                        var cellValue = dr[String.Format("{0}{1}", singleField, i + 1)];

                        if (cellValue == null)
                        {
                            continue;
                        }

                        sumValue += Convert.ToDouble(cellValue);
                    }

                    var oldCellValue = NPOIHelper.GetCellStringValue(sheet, rowNumber, ColumnNumber);

                    if (oldCellValue.Length > 0)
                    {
                        var newSumValue = String.Format("{0:N2}", sumValue);
                        oldCellValue = oldCellValue.Replace("СУМ1", newSumValue);
                        NPOIHelper.SetCellValue(sheet, rowNumber, ColumnNumber, oldCellValue);
                    }

                    rowNumber++;
                }
            }
        }

        private void FillTotalSummaryRow(HSSFSheet sheet, DataTable tbl1, DataTable tbl2, int rowIndex, int hiddenCount)
        {
            var row1 = tbl1 != null ? tbl1.Rows[tbl1.Rows.Count - 1] : null;
            var row2 = tbl2 != null ? tbl2.Rows[tbl2.Rows.Count - 1] : null;

            var columnCount = tbl1 != null ? tbl1.Columns.Count - hiddenCount : 0;

            for (var j = 2; j < columnCount; j++)
            {
                decimal value1 = 0;
                decimal value2 = 0;

                decimal result;

                var valueObj1 = row1 != null ? row1[j] : String.Empty;
                var valueObj2 = row2 != null ? row2[j] : String.Empty;

                if (Decimal.TryParse(valueObj1.ToString(), out result))
                {
                    value1 = result;
                }

                if (Decimal.TryParse(valueObj2.ToString(), out result))
                {
                    value2 = result;
                }

                if (value1 != 0 || value2 != 0)
                {
                    NPOIHelper.SetCellValue(sheet, rowIndex, j, value1 + value2);
                }
            }
        }

        private void Render(string templateDocumentName, IList<DataTable> tables, D_S_TitleReport titleReport)
        {
            HSSFWorkbook wb;
            
            using (var fs = new FileStream(templateDocumentName, FileMode.Open, FileAccess.Read))
            {
                wb = new HSSFWorkbook(fs, true);
            }

            FillMonthReportData(wb, tables[6]);

            FillLimitTable(wb, tables[12], true);
            FillLimitTable(wb, tables[12], false);

            FillCaptionParams(wb, tables[6], titleReport);
            var startRows = new Collection<int> { 51, 12, 16, 11, 12 };

            // задолженности по всяким разным кредитам, цб, гарантиям по отдельному МО
            for (var i = 0; i < 5; i++)
            {
                var sheet = wb.GetSheetAt(i);
                var startSettleRow = startRows[i] + 4;
                var startRegionRow = startRows[i] + 0;
                var settleTable = 14 + i;
                var regionTable = i;

                FillTotalSummaryRow(sheet, tables[regionTable], tables[settleTable], startSettleRow + 1, GetHiddenColumnCount(i));

                FillTablesData(wb, sheet, tables[settleTable], startSettleRow, i, true);

                if (tables[settleTable] == null || tables[settleTable].Rows.Count == 1)
                {
                    NPOIHelper.DropRows(sheet, startSettleRow - 1, startSettleRow + 1);
                }

                FillTablesData(wb, sheet, tables[regionTable], startRegionRow, i, false);
            }            
            
            // структура долга по отдельному МО
            FillStructureTable(wb, tables[5]);
            
            // итоговые таблицы по субъекту
            startRows.Clear();
            startRows.Add(15);
            startRows.Add(14);
            startRows.Add(18);
            startRows.Add(18);
            startRows.Add(14);

            for (var i = 0; i < 5; i++)
            {
                FillSubjectData(wb, wb.GetSheetAt(7 + i), tables[i + 7], startRows[i], 7 + i);
            }

            if (formVersion == 2)
            {
                FillSubjectData(wb, wb.GetSheetAt(13), tables[25], 10, 0);
                wb.GetSheetAt(13).ForceFormulaRecalculation = true;
            }

            CreateSubjectSummaryRows(wb, wb.GetSheetAt(12), tables[7], tables[24], 9);

            var isSubject = Convert.ToBoolean(tables[6].Rows[0][6]);

            var sheetOrgCredits = wb.GetSheetAt(0);

            foreach (var removeRow in ListLimitDeleteRows().OrderByDescending(t => t))
            {
                NPOIHelper.DeleteRow(sheetOrgCredits, removeRow - 1);
            }

            var sheetLimits = wb.GetSheetAt(6);

            foreach (var removeRow in ListLimitSummaryDeleteRows().OrderByDescending(t => t))
            {
                NPOIHelper.DeleteRow(sheetLimits, removeRow - 1);
            }

            if (!isSubject)
            {
                CopyNoteSheets(wb, tables[13]);

                if (formVersion == 2)
                {
                    wb.RemoveSheetAt(13);
                }

                wb.RemoveSheetAt(12);
                wb.RemoveSheetAt(7);
                wb.RemoveSheetAt(7);
                wb.RemoveSheetAt(7);
                wb.RemoveSheetAt(7);
                wb.RemoveSheetAt(7);
                wb.RemoveSheetAt(6);
            }

            wb.SetSelectedTab(0);
            wb.ActiveSheetIndex = 0;

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

        private void CopyNoteSheets(HSSFWorkbook dstWb, DataTable tblNotes)
        {
            foreach (DataRow row in tblNotes.Rows)
            {
                var note = row[0];
                
                if (note == DBNull.Value)
                {
                    continue;
                }
                
                using (var ms = new MemoryStream((byte[])note))
                {
                    var srcWb = new HSSFWorkbook(ms, true);
                    NPOIHelper.CopySheets(srcWb, dstWb);
                    ms.Close();
                }
            }
        }

        private void PrepareData(int currentVariantId, int regionId, DateTime calculateDate, ref DataTable[] tables)
        {
            formVersion = GetReportVersion();
            reportsDataService.GetDebtBookOrenburgData(
                GetReportType(), formVersion, currentVariantId, regionId, ref tables, calculateDate);
        }
    }
}
