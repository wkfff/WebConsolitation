using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public class DebtBookVologdaReport : Report
    {
        private readonly ReportsDataService reportsDataService;
        private DateTime reportDate;

        public DebtBookVologdaReport(ReportsDataService reportsDataService)
        {
            this.reportsDataService = reportsDataService;
        }

        public override string TemplateName
        {
            get { return "DebtorBookVologdaReport"; }
        }

        public override void Create(string templateDocumentName, int currentVariantId, int regionId, DateTime calculateDate, D_S_TitleReport titleReport)
        {
            var tables = new DataTable[12];
            reportDate = calculateDate;
            PrepareData(currentVariantId, regionId, calculateDate, ref tables);
            Render(templateDocumentName, tables, titleReport);
        }

        protected virtual void DeleteSheets(HSSFWorkbook wb)
        {
        }

        private void PrepareData(int currentVariantId, int regionId, DateTime calculateDate, ref DataTable[] tables)
        {
            var tblCreditOrg = new DataTable[3];
            var tblCreditBud = new DataTable[3];
            var tblGarant = new DataTable[3];
            reportsDataService.GetOrganizationCreditsData(currentVariantId, regionId, ref tblCreditOrg, calculateDate);
            reportsDataService.GetBudgetCreditsData(currentVariantId, regionId, ref tblCreditBud, calculateDate);
            reportsDataService.GetGuaranteeData(currentVariantId, regionId, ref tblGarant, calculateDate);
            tables[0] = tblCreditOrg[0];
            tables[1] = tblCreditOrg[1];
            tables[2] = tblCreditOrg[2];
            tables[3] = tblCreditBud[0];
            tables[4] = tblCreditBud[1];
            tables[5] = tblCreditBud[2];
            tables[6] = tblGarant[0];
            tables[7] = tblGarant[1];
            tables[8] = tblGarant[2];
        }

        private string ReplaceTemplate(string value, string templateStr, object replaceValue)
        {
            if (value.Contains(templateStr))
            {
                value = value.Replace(templateStr, replaceValue.ToString());
            }

            return value;
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
            var reportDateText = reportDate.ToShortDateString();

            for (var k = 0; k < wb.NumberOfSheets; k++)
            {
                var sheet = wb.GetSheetAt(k);

                for (var i = 0; i < sheet.LastRowNum; i++)
                {
                    for (var j = 0; j < 250; j++)
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
                        cellValue = ReplaceTemplate(cellValue, "REPORTDATE", reportDateText);
                        cellValue = ReplaceTemplate(cellValue, "REGIONNAME", rowCaptions[0]);
                        cellValue = ReplaceTemplate(cellValue, "DIRECTORNAME", directorName);
                        cellValue = ReplaceTemplate(cellValue, "ACCOUNTANTNAME", accountantName);
                        cellValue = ReplaceTemplate(cellValue, "ACCOUNTANTTITLE", accountantTitle);
                        cellValue = ReplaceTemplate(cellValue, "DIRECTORTITLE", directorTitle);
                        cell.SetCellValue(cellValue);
                    }
                }
            }
        }

        private void FillMOData(
            HSSFWorkbook wb, 
            HSSFSheet sheet,
            DataTable tblData, 
            int rowIndex, 
            int columnCount, 
            int startSummaryColumn)
        {
            var rowCount = tblData.Rows.Count;
            DataRow summaryRow = null;

            foreach (DataRow row in tblData.Rows)
            {
                if (rowCount == 1)
                {
                    summaryRow = row;
                    break;
                }

                rowCount--;
                if (rowCount > 1)
                {
                    NPOIHelper.CopyRow(wb, sheet, rowIndex, rowIndex + 1);
                }

                for (var i = 0; i < columnCount; i++)
                {
                    var cellValue = row[i];
                    double numberValue;

                    if (cellValue != DBNull.Value && Double.TryParse(Convert.ToString(cellValue), out numberValue))
                    {
                        cellValue = numberValue;
                    }
                    
                    NPOIHelper.SetCellValue(sheet, rowIndex, i, cellValue);
                }

                rowIndex++;
            }

            if (summaryRow == null)
            {
                return;
            }

            for (var i = startSummaryColumn; i < columnCount; i++)
            {
                NPOIHelper.SetCellValue(sheet, rowIndex, i, summaryRow[i]);
            }
        }

        private void FillSettleData(
            HSSFWorkbook wb, 
            HSSFSheet sheet,
            DataTable tblData, 
            int rowIndex,
            int startSummaryIndex, 
            int endSummaryIndex)
        {
            var rowCount = tblData.Rows.Count;
            DataRow summaryRow = null;
            var offsetCells = startSummaryIndex - 11;

            foreach (DataRow row in tblData.Rows)
            {
                if (rowCount == 1)
                {
                    summaryRow = row;
                    break;
                }

                rowCount--;

                if (rowCount > 1)
                {
                    NPOIHelper.CopyRow(wb, sheet, rowIndex, rowIndex + 1);
                }

                NPOIHelper.SetCellValue(sheet, rowIndex, 00, row[0]);
                NPOIHelper.SetCellValue(sheet, rowIndex, 01, row[1]);

                for (var i = startSummaryIndex; i < endSummaryIndex; i++)
                {
                    NPOIHelper.SetCellValue(sheet, rowIndex, i, row[i - 9 - offsetCells]);
                }

                rowIndex++;
            }

            if (summaryRow == null)
            {
                return;
            }

            for (var i = startSummaryIndex; i < endSummaryIndex; i++)
            {
                NPOIHelper.SetCellValue(sheet, rowIndex, i, summaryRow[i - 9 - offsetCells]);
            }
        }

        private void CalcTotalSum(DataTable tblTable, ref double[] sum, int startIndex)
        {
            for (var j = 0; j < tblTable.Rows.Count - 1; j++)
            {
                for (var i = startIndex; i < startIndex + 10; i++)
                {
                    sum[i - startIndex] += Convert.ToDouble(tblTable.Rows[j][i]);
                }
            }
        }

        private void FillGarantTotalRow(HSSFSheet sheet, IList<DataTable> tables)
        {
            var sum = new double[10];
            CalcTotalSum(tables[6], ref sum, 12);
            CalcTotalSum(tables[7], ref sum, 2);

            for (var i = 0; i < 10; i++)
            {
                NPOIHelper.SetCellValue(sheet, 30, i + 12, sum[i]);
            }
        }

        private void Render(string templateDocumentName, IList<DataTable> tables, D_S_TitleReport titleReport)
        {
            HSSFWorkbook wb;

            using (var fs = new FileStream(templateDocumentName, FileMode.Open, FileAccess.Read))
            {
                wb = new HSSFWorkbook(fs, true);
            }

            FillDocumentVariables(wb, titleReport, tables[2]);
            
            // КОММЕРЧЕСКИЕ КРЕДИТЫ
            var sheet = wb.GetSheetAt(0);
            FillSettleData(wb, sheet, tables[1], 28, 11, 18);
            FillMOData(wb, sheet, tables[0], 20, 18, 10);
            sheet.ForceFormulaRecalculation = true;

            // БЮДЖЕТНЫЕ КРЕДИТЫ
            sheet = wb.GetSheetAt(1);
            FillSettleData(wb, sheet, tables[4], 28, 11, 18);
            FillMOData(wb, sheet, tables[3], 20, 18, 10);
            sheet.ForceFormulaRecalculation = true;            

            // ГАРАНТИИ
            sheet = wb.GetSheetAt(2);
            FillGarantTotalRow(sheet, tables);
            FillSettleData(wb, sheet, tables[7], 28, 12, 22);
            FillMOData(wb, sheet, tables[6], 20, 22, 12);
            sheet.ForceFormulaRecalculation = true;
            
            // ЦЕННЫЕ БУМАГИ

            // удаление ненужных шаблонов
            DeleteSheets(wb);

            // закрываем и уходим
            SaveDocument(wb, templateDocumentName);
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
