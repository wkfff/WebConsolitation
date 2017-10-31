using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsData.FNS;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// 010_ПЗ_ПОКАЗАТЕЛИ Ф.5-МН В РАЗРЕЗЕ ОМСУ 
        /// </summary>
        public DataTable[] GetUFNSReport010Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[3];
            var paramMark = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMark]);
            var paramRgnListType = reportParams[ReportConsts.ParamRegionListType];
            var writeSettles = ReportMonthMethods.WriteSettles(paramRgnListType);
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;

            // показатели
            var tblSources = dbHelper.GetTableSources(d_Marks_FNS5MN.InternalKey);
            var source = (from DataRow row in tblSources.Select(filterHelper.EqualIntFilter(HUB_Datasources.YEAR, year))
                          select Convert.ToInt32(row[HUB_Datasources.id])).ToList();
            
            var fltMark = source.Count > 0
                                ? filterHelper.RangeFilter(d_Marks_FNS5MN.SourceID, ConvertToString(source))
                                : filterHelper.RangeFilter(d_Marks_FNS5MN.SourceID, ReportConsts.UndefinedKey);
            fltMark = CombineAnd(fltMark, filterHelper.EqualIntFilter(d_Marks_FNS5MN.RowType, ReportConsts.RowType));
            if (paramMark != String.Empty)
            {
                fltMark = CombineAnd(fltMark, filterHelper.RangeFilter(d_Marks_FNS5MN.ID, paramMark));
            }

            var hMarks = new Hierarchy(d_Marks_FNS5MN.InternalKey, fltMark);
            hMarks.Sort(d_Marks_FNS5MN.Code);
            var marks = hMarks.ChildAll();
            var marksId = (from mark in marks
                           where Convert.ToInt32(mark.Row[d_Marks_FNS5MN.Code]) > 0
                           select mark.Id).ToList();
            var filterMarks = marksId.Count > 0 ? ConvertToString(marksId) : ReportConsts.UndefinedKey;
            var filterPeriod = filterHelper.PeriodFilter(f_D_FNS5MNRegions.RefYearDayUNV, GetUNVYearStart(year), GetUNVYearEnd(year), true);

            // получаем данные из т.ф. «Доходы.ФНС_5 МН.Районы» (f_D_FNS5MNRegions)
            var filterList = new List<QFilter>
            {
                new QFilter(QFNS5MNRegions.Keys.Period,  filterPeriod),
                new QFilter(QFNS5MNRegions.Keys.Mark,  filterMarks)
            };
            var groupList = new List<Enum> { QFNS5MNRegions.Keys.Okato, QFNS5MNRegions.Keys.Mark };
            var queryText = new QFNS5MNRegions().GetQueryText(filterList, groupList);
            var tblData = dbHelper.GetTableData(queryText);
            
            // заполняем таблицу АТЕ суммами
            var tblATE = reportHelper.CreateRegionList(marksId.Count);
            var columnClsIndex = GetColumnIndex(tblData, d_Regions_FNS.RefBridge);
            var splitParams = new RegionSplitParams
            {
                KeyValIndex = columnClsIndex,
                DocValIndex = columnClsIndex,
                LvlValIndex = columnClsIndex,
                TblResult = tblATE,
                SrcColumnIndex = GetColumnIndex(tblData, SUM),
            };

            for (var i = 0; i < marksId.Count; i++)
            {
                var filter = filterHelper.EqualIntFilter(f_D_FNS5MNRegions.RefMarks, marksId[i]);
                splitParams.RowsData = tblData.Select(filter);
                splitParams.DstColumnIndex = i;
                reportHelper.SplitRegionData(splitParams);
            }

            reportHelper.ClearSettleRows(tblATE, paramRgnListType);
            const int firstATESumColumn = ReportMonthMethods.RegionHeaderColumnCnt;

            // заполняем таблицу отчета
            var repTable = new DataTable();
            AddColumnToReport(repTable, typeof(int), "Num");
            AddColumnToReport(repTable, typeof(string), "Type");
            AddColumnToReport(repTable, typeof(string), "Name");
            var firstSumColumn = AddColumnToReport(repTable, typeof(decimal), "Sum", marksId.Count);
            AddColumnToReport(repTable, typeof(int), STYLE);
            var regionRow = repTable.NewRow();

            foreach (DataRow row in tblATE.Rows)
            {
                var lvl = Convert.ToInt32(row[ReportMonthMethods.RegionLvlIndex]);
                var flg = Convert.ToInt32(row[ReportMonthMethods.RegionFlgIndex]);
                var ter = Convert.ToString(row[ReportMonthMethods.RegionTerrIndex]);
                var typ = Convert.ToInt32(row[ReportMonthMethods.RegionTypIndex]);
                var style = 1;
                switch (lvl)
                {
                    case 2:
                        style = 0;
                        break;
                    case 3:
                        if (flg != ReportMonthMethods.RowTypeRegionSummary && ter != "ГО" && writeSettles)
                        {
                            style = 0;
                        }
                        break;
                    case 4:
                        style = 2;
                        break;
                }

                var repRow = typ == 3 ? regionRow : repTable.Rows.Add();
                repRow["Num"] = row[ReportMonthMethods.RegionCodeIndex];
                repRow["Type"] = ter;
                repRow["Name"] = row[ReportMonthMethods.RegionNameIndex];
                repRow[STYLE] = style;

                for (var i = 0; i < marksId.Count; i++)
                {
                    repRow[firstSumColumn + i] = row[firstATESumColumn + i];
                }
            }
            
            // заполняем итоговые строки
            var lastRow = repTable.Rows[repTable.Rows.Count - 1]; // итого
            repTable.Rows.Add(regionRow); // регион
            var totalRow = repTable.Rows.Add(); // всего

            for (var i = 0; i < marksId.Count; i++)
            {
                var sum = GetDecimal(lastRow[firstSumColumn + i]);
                sum += GetDecimal(regionRow[firstSumColumn + i]);
                totalRow[firstSumColumn + i] = sum;
            }

            // заполняем таблицу колонок
            var dtColumns = new DataTable();
            dtColumns.Columns.Add(NAME);
            dtColumns.Columns.Add(STYLE, typeof(int));
            dtColumns.Columns.Add(LEVEL, typeof(int));
            dtColumns.Columns.Add(MERGED, typeof(bool));
            dtColumns.Columns.Add(IsRUB, typeof(bool));
            dtColumns.Columns.Add(IsDATA, typeof(bool));

            dtColumns.Rows.Add(DBNull.Value, 0, 2, false, false, true);
            dtColumns.Rows.Add(DBNull.Value, 1, 2, false, false, true);
            dtColumns.Rows.Add(DBNull.Value, 2, 2, false, false, true);

            foreach (var mark in marks)
            {
                var code = Convert.ToInt32(mark.Row[d_Marks_FNS5MN.Code]);
                var name = code > 0
                               ? String.Format("{0}\r\n({1})", mark.Row[d_Marks_FNS5MN.Name], code)
                               : mark.Row[d_Marks_FNS5MN.Name];
                var merged = mark.Child.Count > 0;
                var isRubColumn = IsRubRef(mark.Row[d_Marks_FNS5MN.RefUnits]);
                var style = isRubColumn ? 3 : 4;

                dtColumns.Rows.Add(name, style, mark.Level, merged, isRubColumn, !merged);

                if (merged && code > 0)
                {
                    dtColumns.Rows.Add(DBNull.Value, style, mark.Level + 1, false, isRubColumn, true);
                }
            }

            // делим суммы в зависимости от выбранных единиц измерения
            var index = 0;
            foreach (var row in dtColumns.Rows.Cast<DataRow>().Where(row => !Convert.ToBoolean(row[MERGED])))
            {
                if (Convert.ToBoolean(row[IsRUB]))
                {
                    DivideSum(repTable, index, 1, divider);
                }
                index++;
            }

            tablesResult[0] = repTable;
            tablesResult[1] = dtColumns;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.YEAR, year);
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
