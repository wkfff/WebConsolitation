﻿using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK.ReportFillers
{
    class ReportUFK013Filler : CommonUFNSReportFiller
    {
        private const int MainColumnLevel = 0;
        private const int YearLevel = 1;
        private const int FirstDataColumn = 1;
        private const int YearsInPattern = 3;

        private DataTable GetColumnsTable(IEnumerable<int> levels)
        {
            var dt = new DataTable();
            dt.Columns.Add(ReportDataServer.NAME, typeof(string));
            dt.Columns.Add(ReportDataServer.STYLE, typeof(int));
            dt.Columns.Add(ReportDataServer.LEVEL, typeof(int));
            dt.Columns.Add(ReportDataServer.MERGED, typeof(bool));
            dt.Columns.Add(ReportDataServer.IsRUB, typeof(bool));
            dt.Columns.Add(ReportDataServer.IsDATA, typeof(bool));
            for (var i = 0; i < FirstDataColumn; i++)
            {
                dt.Rows.Add(DBNull.Value, i, YearLevel, false, false, true);
            }

            foreach (var l in levels)
            {
                var firstLevelColumn = FirstDataColumn + l*(YearsInPattern*2 - 1);
                dt.Rows.Add(DBNull.Value, firstLevelColumn, MainColumnLevel, true, false, false);

                for (var i = 0; i < YearsInPattern; i++)
                {
                    var style = i == 0 ? firstLevelColumn : firstLevelColumn + i * 2 - 1;
                    dt.Rows.Add(DBNull.Value, style, YearLevel, false, true, true);
                    if (i > 0)
                    {
                        dt.Rows.Add(DBNull.Value, style + 1, YearLevel, false, false, false);
                    }
                }
            }

            return dt;
        }

        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            const int sheetIndex = 0;
            var sheet = wb.GetSheetAt(sheetIndex);
            var paramLevels = ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.BDGT_LEVEL);
            var levels = ReportDataServer.ConvertToIntList(Convert.ToString(paramLevels));
            var paramEndDate = ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.ENDDATE);
            var year = Convert.ToDateTime(paramEndDate).Year;
            SetParamValue(sheet, "YEAR-2", year - 2);
            SetParamValue(sheet, "YEAR-1", year - 1);
            SetParamValue(sheet, "YEAR", year);
            var paramSum = ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.SUMM);
            if (Convert.ToDecimal(paramSum) == 0)
            {
                DeleteParamCell(sheet, ParamUFKHelper.SUMM);
            }

            var dtColumns = GetColumnsTable(levels);

            var sheetParams = new SheetParams
            {
                SheetIndex = sheetIndex,
                FirstRow = 8 - 1,
                StyleRowsCount = 8,
                StyleColumnsCount = 21,
                HeaderRowsCount = 3,
                FirstTotalColumn = FirstDataColumn,
                ExistHeaderNumColumn = true,
                RemoveUnusedColumns = false,
                Columns = GetDataColumnsIndexies(dtColumns),
                SumColumns = GetSumColumnsIndexies(dtColumns)
            };

            AddColumns(wb, sheetParams, dtColumns);
            FillSheet(wb, tableList, sheetParams);
        }
    }
}