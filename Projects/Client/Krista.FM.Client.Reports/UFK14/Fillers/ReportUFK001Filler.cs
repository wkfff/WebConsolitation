using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Month.Fillers;
using Krista.FM.Client.Reports.UFK14.Helpers;
using NPOI.HSSF.UserModel;


namespace Krista.FM.Client.Reports.UFK.ReportFillers
{
    class ReportUFK001Filler : CommonMonthReportFiller
    {
        protected override int[] GetHeaderColumns()
        {
            return new[] { 0, 1, 2 };
        }

        protected override Dictionary<int, List<int>> GetHeaderPatterns()
        {
            var patterns = new Dictionary<int, List<int>>
                               {
                                   {0, new List<int>() {0}},
                                   {1, new List<int>() { ReportMonthMethods.RegionTerrIndex }},
                                   {2, new List<int>() { ReportMonthMethods.RegionNameIndex }},
                               };

            return patterns;
        }

        public virtual void FillUFKReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            const int startRow = 10;
            const int firstDataCol = ReportMonthMethods.RegionHeaderColumnCnt;
            const int FirstCol = 3;
            const int ColumnCount = 8;
            var sheet = wb.GetSheetAt(0);
            var tblData = tableList[0];
            var tblCaptions = tableList[tableList.Length - 1];
            var paramHelper = new ParamUFKHelper(tblCaptions.Rows[0]);
            FillCaptionParams(wb, sheet, tblCaptions, ParamUFKHelper.GetParamList());

            if (tableList[1].Rows.Count > 0)
            {
                var subjectRow = tableList[1].Rows[0];
                
                for (var n = 0; n < ColumnCount - FirstCol; n++)
                {
                    var subjectValue = ReportDataServer.GetDecimal(subjectRow[firstDataCol + n]);
                    NPOIHelper.SetCellValue(sheet, startRow + 3, FirstCol + n, subjectValue);
                }
            }

            for (var i = 0; i < tblData.Rows.Count - 3; i++)
            {
                NPOIHelper.CopyRow(wb, sheet, startRow, startRow + 1);
            }

            // заполняем таблицу наименованиями административных единиц
            var writeSettles = Convert.ToBoolean(paramHelper.GetParamValue(ParamUFKHelper.REGION_LIST_TYPE));
            CreateRegionHeaders(wb, sheet, tblData, startRow, ColumnCount, writeSettles);

            // заполняем таблицу данными
            for (var i = 0; i < tblData.Rows.Count; i++)
            {
                var row = tblData.Rows[i];
                var rowIndex = startRow + i;

                for (var n = 0; n < ColumnCount - FirstCol; n++)
                {
                    NPOIHelper.SetCellValue(sheet, rowIndex, FirstCol + n, ReportDataServer.GetDecimal(row[firstDataCol + n]));
                }
            }

            // устанавливаем в колонках с суммами формат чисел
            var cols = new[] { 3, 4, 5, 6, 7 };
            var rows = new int[tblData.Rows.Count + 2]; // включая две строки после итого
            for (var i = 0; i < tblData.Rows.Count + 2; i++)
            {
                rows[i] = startRow + i;
            }

            SetRangeDataFormat(wb, sheet, GetPrecisionFormat(paramHelper.GetParamValue(ParamUFKHelper.PRECISION)), rows, cols);

            // скрываем ненужные колонки
            var paramLvl = paramHelper.GetParamValue(ParamUFKHelper.BDGT_LEVEL);
            var bdgtLevel = (string) paramLvl != String.Empty ? Convert.ToInt32(paramLvl) : 0;
            var hiddenColumns = new Dictionary<int, int[]>
                                    {
                                        {0, new int[] {}},
                                        {1, new    [] {3, 5, 6, 7}},
                                        {2, new    [] {3, 4}},
                                        {3, new    [] {3, 4, 5, 7}},
                                        {4, new    [] {3, 4, 5, 6}}
                                    };

            foreach (var column in hiddenColumns[bdgtLevel])
            {
                sheet.SetColumnHidden(column, true);
            }

            // заполняем номера колонок начиная с 3
            var colNum = 3;
            for (var column = FirstCol; column < ColumnCount; column++)
            {
                if (!hiddenColumns[bdgtLevel].Contains(column))
                {
                    NPOIHelper.SetCellValue(sheet, startRow - 1, column, colNum++);
                }
            }

            sheet.ForceFormulaRecalculation = true;
        }
    }
}
