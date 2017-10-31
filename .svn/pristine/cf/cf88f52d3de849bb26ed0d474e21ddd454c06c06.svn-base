using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData.FNS;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK.ReportMaster;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// 011 ПОКАЗАТЕЛИ ФОРМЫ 5-НДФЛ
        /// </summary>
        public DataTable[] GetUFNSReport011Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[3];
            var paramRegion = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamRegionComparable]);
            var paramMark = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMark]);
            var paramHalfYear = reportParams[ReportConsts.ParamHalfYear];
            var halfYear = paramHalfYear != String.Empty ? ConvertToIntList(paramHalfYear) : GetColumnsList(0, 2);
            var paramRgnListType = reportParams[ReportConsts.ParamRegionListType];
            var hideSettlement = !ReportMonthMethods.WriteSettles(paramRgnListType);
            var paramHideEmptyStr = Convert.ToBoolean(reportParams[ReportConsts.ParamHideEmptyStr]);
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var years = GetSelectedYears(reportParams[ReportConsts.ParamYear], false);
            var marksTable = new MarksNDFLTable(QFNS5NDFLRegions.Keys.Mark, QFNS5NDFLRegions.Keys.MarkBridge);

            // периоды
            var periods = (from year in years from i in halfYear select GetUNVMonthStart(year, (i + 1)*6)).ToList();

            // показатели
            var filterMark = paramMark != String.Empty
                ? filterHelper.RangeFilter(marksTable.Id, paramMark)
                : marksTable.GetRowsFilter();
            var hMarks = new Hierarchy(marksTable.TableKey, filterMark);
            hMarks.Sort(marksTable.Code);
            var marks = (from mark in hMarks.ChildOfLastLevel()
                         let isRub = ReportUFNSHelper.IsRubFNS5NDFLMark(mark.Row[marksTable.Name])
                         select new {mark.Id, isRub}).ToDictionary(e => e.Id, e => e.isRub);
            var filterMarks = marks.Count > 0 ? ConvertToString(marks.Keys) : ReportConsts.UndefinedKey;

            // получаем данные из т.ф. «ФНС_5 НДФЛ_Районы» (f.D.FNS5NDFLRegions)
            var filterList = new List<QFilter>
            {
                new QFilter(QFNS5NDFLRegions.Keys.Day,  ConvertToString(periods)),
                new QFilter(QFNS5NDFLRegions.Keys.Okato,  paramRegion),
                new QFilter(marksTable.FilterKey,  filterMarks)
            };
            var groupList = new List<Enum>
            {
                QFNS5NDFLRegions.Keys.Okato,
                marksTable.FilterKey,
                QFNS5NDFLRegions.Keys.Day
            };
            var queryText = new QFNS5NDFLRegions().GetQueryText(filterList, groupList);
            var tblData = dbHelper.GetTableData(queryText);

            // параметры отчета
            var rep = new Report(f_D_FNS5NDFLRegions.InternalKey)
            {
                Divider = GetDividerValue(divider),
                RowFilter = paramHideEmptyStr ? Functions.IsNotNullRow : (RowViewParams.Function)null
            };

            // группировка по АТЕ
            var ateGrouping = new AteGrouping(0, hideSettlement);
            rep.AddGrouping(d_Regions_FNS.RefBridge, ateGrouping);
            var fixedId = paramHideEmptyStr
                  ? ateGrouping.AteMainId
                  : paramRegion != String.Empty
                        ? Combine(paramRegion, ateGrouping.AteMainId)
                        : ateGrouping.GetRegionsId();
            ateGrouping.SetFixedValues(fixedId);
            const string regionsTableKey = b_Regions_Bridge.InternalKey;

            // настраиваем колонки отчета
            const string terTypeTableKey = fx_FX_TerritorialPartitionType.InternalKey;
            var masks = new AteOutMasks(new TableField(regionsTableKey, b_Regions_Bridge.CodeLine));
            rep.AddCaptionColumn().SetMasks(ateGrouping, masks);
            masks = new AteOutMasks(new TableField(terTypeTableKey, fx_FX_TerritorialPartitionType.Name));
            rep.AddCaptionColumn().SetMasks(ateGrouping, masks);
            masks = new AteOutMasks(new TableField(regionsTableKey, b_Regions_Bridge.Name));
            rep.AddCaptionColumn().SetMasks(ateGrouping, masks);

            foreach (var mark in marks)
            {
                var fltMark = filterHelper.EqualIntFilter(f_D_FNS5NDFLRegions.RefMarks, mark.Key);

                foreach (
                    var fltPeriod in
                        periods.Select(p => filterHelper.EqualIntFilter(f_D_FNS5NDFLRegions.RefYearDayUNV, p)))
                {
                    var column = rep.AddValueColumn(f_D_FNS5NDFLRegions.ValueReport, CombineAnd(fltMark, fltPeriod));
                    if (!mark.Value)
                    {
                        column.Divider = 1;
                    }
                }
            }

            rep.ProcessTable(tblData);
            tablesResult[0] = rep.GetReportData();

            // заполняем таблицу колонок
            var dtMarks = new DataTable();
            dtMarks.Columns.Add(NAME);
            dtMarks.Columns.Add(STYLE, typeof(int));
            dtMarks.Columns.Add(LEVEL, typeof(int));
            dtMarks.Columns.Add(MERGED, typeof(bool));
            dtMarks.Columns.Add(IsRUB, typeof(bool));
            dtMarks.Columns.Add(IsDATA, typeof(bool));

            foreach (var mark in hMarks.ChildAll())
            {
                var name = String.Format("{0} {1}",
                                         ReportUFNSHelper.GetFNS5NDFLMarkShortCode(mark.Row[marksTable.Code]),
                                         mark.Row[marksTable.Name]).Trim();
                var isRubColumn = marks.ContainsKey(mark.Id) && marks[mark.Id];
                dtMarks.Rows.Add(name, 3, mark.Level, mark.Child.Count > 0, isRubColumn, true);
            }

            tablesResult[1] = dtMarks;
            
            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.YEARS, ConvertToString(years));
            paramHelper.SetParamValue(ParamUFKHelper.HALF_YEAR, ConvertToString(halfYear));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
