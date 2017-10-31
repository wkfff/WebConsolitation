using System;
using System.Collections.Generic;
using System.Data;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.Month.Fillers
{
    class Report009Filler : CommonMonthReportFiller
    {
        private string precision;
        const int columnOffset = ReportMonthMethods.RegionHeaderColumnCnt;
        private decimal planOBValue;
        private decimal factOBValue;
        private decimal planMBValue;
        private decimal planOMValue;
        private decimal factMBValue;
        private decimal consPlan;
        private decimal consFact;

        private void FillRowData(HSSFWorkbook wb, HSSFSheet sheet, int rowIndex, DataRow row)
        {
            planOBValue = ReportDataServer.GetDecimal(row[columnOffset + 0]);
            factOBValue = ReportDataServer.GetDecimal(row[columnOffset + 1]);
            planMBValue = ReportDataServer.GetDecimal(row[columnOffset + 2]);
            planOMValue = ReportDataServer.GetDecimal(row[columnOffset + 3]);
            factMBValue = ReportDataServer.GetDecimal(row[columnOffset + 4]);
            consPlan = planOBValue + planMBValue;
            consFact = factOBValue + factMBValue;

            NPOIHelper.SetCellValue(sheet, rowIndex, 03, consPlan);
            NPOIHelper.SetCellValue(sheet, rowIndex, 04, planOMValue);
            NPOIHelper.SetCellValue(sheet, rowIndex, 05, consFact);
            NPOIHelper.SetCellValue(sheet, rowIndex, 08, planOBValue);
            NPOIHelper.SetCellValue(sheet, rowIndex, 09, factOBValue);
            NPOIHelper.SetCellValue(sheet, rowIndex, 11, planMBValue);
            NPOIHelper.SetCellValue(sheet, rowIndex, 12, planOMValue);
            NPOIHelper.SetCellValue(sheet, rowIndex, 13, factMBValue);

            SetRangeDataFormat(wb, sheet, precision, new List<int> { rowIndex, rowIndex + 1, rowIndex + 2 }, new List<int> { 3, 4, 5, 8, 9, 11, 12, 13 });
        }

        private void FillRelations(HSSFSheet sheet, int rowIndex)
        {
            NPOIHelper.SetCellValue(sheet, rowIndex, 06, GetPercent(consFact, consPlan));
            NPOIHelper.SetCellValue(sheet, rowIndex, 07, GetPercent(factMBValue, planOMValue));
            NPOIHelper.SetCellValue(sheet, rowIndex, 10, GetPercent(factOBValue, planOBValue));
            NPOIHelper.SetCellValue(sheet, rowIndex, 14, GetPercent(factMBValue, planMBValue));
            NPOIHelper.SetCellValue(sheet, rowIndex, 15, GetPercent(factMBValue, planOMValue));
        }

        public virtual void FillMonthReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var tblData = tableList[0];
            var tblSubject = tableList[1];
            var tblCaptions = tableList[tableList.Length - 1];
            var rowCaption = tblCaptions.Rows[0];
            precision = GetPrecisionFormat(rowCaption[7]);
            var writeSettles = Convert.ToBoolean(rowCaption[2]);
            const int startRow = 9;
            const int columnCount = 16;

            var paramList = GetParamDictionary();

            FillCaptionParams(wb, sheet, tblCaptions, paramList);

            var columnList = new List<int>();

            for (var i = 0; i < columnCount; i++)
            {
                columnList.Add(i);
            }

            if (tblSubject.Rows.Count > 0)
            {
                var rowSubject = tblSubject.Rows[0];
                planOBValue = ReportDataServer.GetDecimal(rowSubject[columnOffset + 0]);
                factOBValue = ReportDataServer.GetDecimal(rowSubject[columnOffset + 1]);
                NPOIHelper.SetCellValue(sheet, 12, 08, planOBValue);
                NPOIHelper.SetCellValue(sheet, 12, 09, factOBValue);

                SetRangeDataFormat(wb, sheet, precision, new List<int> { 12 }, new List<int> { 8, 9 });
            }

            for (var i = 0; i < tblData.Rows.Count - 3; i++)
            {
                NPOIHelper.CopyRow(wb, sheet, startRow, startRow + 1);
            }

            CreateRegionHeaders(wb, sheet, tblData, startRow, columnCount, writeSettles);

            for (var i = 0; i < tblData.Rows.Count; i++)
            {
                FillRowData(wb, sheet, startRow + i, tblData.Rows[i]);
                FillRelations(sheet, startRow + i);
            }

            sheet.ForceFormulaRecalculation = true;
        }

        private decimal? GetPercent(decimal val1, decimal val2)
        {
            if (val2 != 0)
            {
                return 100 * val1 / val2;
            }

            return null;
        }
    }
}
