using System;
using System.Collections.Generic;
using System.Data;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.Month.Fillers
{
    class Report006Filler : CommonMonthReportFiller
    {
        private string precision;

        public virtual void FillMonthReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var tblData = tableList[0];
            var tblSubject = tableList[1];
            var tblCaptions = tableList[tableList.Length - 1];
            var rowCaption = tblCaptions.Rows[0];
            var writeSettles = Convert.ToBoolean(rowCaption[2]);
            precision = GetPrecisionFormat(rowCaption[8]);
            const int startRow = 9;
            FillCaptionParams(wb, sheet, tblCaptions, GetParamDictionary());

            if (tblSubject.Rows.Count > 0)
            {
                FillRow(wb, sheet, tblSubject.Rows[0], 12);
            }

            for (var i = 0; i < tblData.Rows.Count - 3; i++)
            {
                NPOIHelper.CopyRow(wb, sheet, startRow, startRow + 1);
            }

            CreateRegionHeaders(wb, sheet, tblData, startRow, 6, writeSettles);

            for (var i = 0; i < tblData.Rows.Count; i++)
            {
                FillRow(wb, sheet, tblData.Rows[i], startRow + i);
            }

            var showPlan = Convert.ToBoolean(rowCaption[7]);
            var showFact = Convert.ToBoolean(rowCaption[6]);

            if (!showFact || !showPlan)
            {
                NPOIHelper.DeleteColumn(wb, sheet, 5);
            }

            if (!showFact)
            {
                NPOIHelper.DeleteColumn(wb, sheet, 4);
            }

            if (!showPlan)
            {
                var rowIndex = startRow + tblData.Rows.Count;
                NPOIHelper.DeleteColumn(wb, sheet, 3);
                NPOIHelper.SetCellFormula(sheet, rowIndex + 1, 3, String.Format("SUM(D{0}:D{1})", rowIndex, rowIndex + 1));
            }

            FillIndexRow(sheet, 8, 3, 3);

            sheet.ForceFormulaRecalculation = true;
        }

        private void FillRow(HSSFWorkbook wb, HSSFSheet sheet, DataRow row, int rowIndex)
        {
            const int columnOffset = ReportMonthMethods.RegionHeaderColumnCnt;
            var planValue = ReportDataServer.GetDecimal(row[columnOffset + 0]);
            var factValue = ReportDataServer.GetDecimal(row[columnOffset + 1]);
            NPOIHelper.SetCellValue(sheet, rowIndex, 3, planValue);
            NPOIHelper.SetCellValue(sheet, rowIndex, 4, factValue);

            SetRangeDataFormat(wb, sheet, precision, new List<int> { rowIndex }, new List<int> { 3 });
            SetRangeDataFormat(wb, sheet, precision, new List<int> { rowIndex }, new List<int> { 4 });

            decimal percent = 0;

            if (planValue != 0)
            {
                percent = 100 * factValue / planValue;
            }

            NPOIHelper.SetCellValue(sheet, rowIndex, 5, percent);            
        }
    }
}
