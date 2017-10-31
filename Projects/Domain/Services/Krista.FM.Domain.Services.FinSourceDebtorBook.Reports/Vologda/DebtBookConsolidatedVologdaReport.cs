using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public class DebtBookConsolidatedReport : Report
    {
        private readonly ReportsDataService reportsDataService;
        private DateTime reportDate;

        public DebtBookConsolidatedReport(ReportsDataService reportsDataService)
        {
            this.reportsDataService = reportsDataService;
        }

        public override string TemplateName
        {
            get { return "ConsolidatedReport"; }
        }

        public override void Create(string templateDocumentName, int currentVariantId, int regionId, DateTime calculateDate, D_S_TitleReport titleReport)
        {
            DataTable[] tables = new DataTable[4];
            PrepareData(currentVariantId, calculateDate, ref tables);
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
            int writeColumnOffset, 
            int columnCount,
            int dataStartColumn,
            bool insertNewRows,
            Collection<int> skipColumns)
        {
            int flagCaption = -10;
            int rowCount = tblData.Rows.Count;
            int regionCounter = 1;
            foreach (DataRow row in tblData.Rows)
            {
                rowCount--;
                if (insertNewRows && rowCount > 1)
                {
                    NPOIHelper.CopyRow(wb, sheet, rowIndex, rowIndex + 1);
                }

                int realColumnIndex = 0;
                for (int i = 0; i < columnCount; i++)
                {
                    object cellValue = "x";
                    if (!skipColumns.Contains(i))
                    {
                        cellValue = row[dataStartColumn + realColumnIndex++];
                    }

                    if (insertNewRows)
                    {
                        if (rowCount == 0)
                        {
                            if (i == 0)
                            {
                                cellValue = null;
                            }

                            if (i == 1)
                            {
                                cellValue = "Итого";
                            }
                        }

                        int rowType = Convert.ToInt32(row[0]);

                        if (i == 0)
                        {
                            if (rowType <= flagCaption)
                            {
                                cellValue = String.Format("{0}.", regionCounter++);
                            }
                            else
                            {
                                cellValue = null;
                            }
                        }

                        int cellStyleIndex = 0;
                        if (i < 2)
                        {
                            cellStyleIndex = 1;
                        }

                        if (skipColumns.Contains(i))
                        {
                            cellStyleIndex = 2;
                        }

                        HSSFCell cellDst = sheet.GetRow(rowIndex).GetCell(writeColumnOffset + i);
                        int rowStyleIndex = 4;

                        if (rowType > flagCaption && rowCount != 0)
                        {
                            rowStyleIndex = 3;
                        }

                        HSSFCell cellSrc = sheet.GetRow(rowStyleIndex).GetCell(cellStyleIndex);

                        if (cellDst != null)
                        {
                            cellDst.CellStyle = cellSrc.CellStyle;
                        }
                    }

                    NPOIHelper.SetCellValue(sheet, rowIndex, writeColumnOffset + i, cellValue);
                }

                rowIndex++;

                if (insertNewRows && Convert.ToInt32(row[0]) == flagCaption - 4)
                {
                    NPOIHelper.CopyRow(wb, sheet, rowIndex, rowIndex + 1);
                    NPOIHelper.SetCellValue(sheet, rowIndex, 1, "в том числе");
                    HSSFCell cellDst = sheet.GetRow(rowIndex).GetCell(1);
                    HSSFCell cellSrc = sheet.GetRow(3).GetCell(1);
                    cellDst.CellStyle = cellSrc.CellStyle;
                    rowIndex++;
                }
            }
        }

        private void PrepareData(int currentVariantId, DateTime calculateDate, ref DataTable[] tables)
        {
            reportDate = calculateDate;
            tables[0] = reportsDataService.GetTotalSubjectData(currentVariantId);
            tables[1] = reportsDataService.GetOrgCreditsFullData(currentVariantId);
            tables[2] = reportsDataService.GetBudCreditsFullData(currentVariantId);
            tables[3] = reportsDataService.GetGuaranteeTotalData(currentVariantId);
        }

        private void Render(string templateDocumentName, IList<DataTable> tables, D_S_TitleReport titleReport)
        {
            HSSFWorkbook wb = CreateDocument(templateDocumentName);
            FillDocumentVariables(wb, titleReport);

            HSSFSheet sheet = wb.GetSheetAt(0);

            Collection<int> emptyColumns = new Collection<int>();
            Collection<int> skipColumnsCrd = new Collection<int> { 6 };
            Collection<int> skipColumnsTtl = new Collection<int> { 4 };

            FillData(wb, sheet, tables[3], 45, 0, 11, 1, true, emptyColumns);
            FillData(wb, sheet, tables[2], 37, 0, 10, 1, true, skipColumnsCrd);
            FillData(wb, sheet, tables[1], 33, 0, 10, 1, true, skipColumnsCrd);
            FillData(wb, sheet, tables[0], 17, 2, 8, 0, false, skipColumnsTtl);

            for (int i = 0; i < 3; i++)
            {
                NPOIHelper.SetCellValue(sheet, 3, i, String.Empty);
                NPOIHelper.SetCellValue(sheet, 4, i, String.Empty);
            }

            sheet.GetRow(4).ZeroHeight = true;
            sheet.GetRow(3).ZeroHeight = true;
            sheet.ForceFormulaRecalculation = true;
            
            // закрываем и уходим
            SaveDocument(wb, templateDocumentName);
        }
    }
}
