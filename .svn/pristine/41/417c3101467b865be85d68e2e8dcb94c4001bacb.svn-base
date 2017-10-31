using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsData.MOFO;
using Krista.FM.Client.Reports.Database.FactTables.MOFO;
using Krista.FM.Client.Reports.MOFO.Queries;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK.ReportMaster;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{
    internal class MOFO0021AteGrouping : AteGrouping
    {
        private void InsertSettleSumRow(List<ReportRow> rows, int groupingIndex, int terType, int style)
        {
            var typeField = new TableField(b_Regions_Bridge.InternalKey, b_Regions_Bridge.RefTerrType);
            var ateTypeIndex = GetLookupFieldIndex(typeField);
            var townRows = (rows.Where(row =>row.GroupingIndex == groupingIndex &&
                                             row.Level == 4 &&
                                             row.LookupFields[ateTypeIndex].Equals(terType))).ToList();
            if (townRows.Count() > 0)
            {
                var parent = townRows[0].Parent.Parent.Parent;

                var newRow = new ReportRow
                {
                    Key = parent.Key,
                    GroupingIndex = parent.GroupingIndex,
                    Style = style,
                    LookupFields = new object[parent.LookupFields.Length],
                    Values = new object[parent.Values.Length],
                    BreakSumming = true
                };

                parent.AddChild(newRow);
                var indexies = Report.SumColumnsIndexies();
                foreach (var row in townRows)
                {
                    newRow.AddValues(row.Values, indexies);
                }

                rows.Add(newRow);
            }
        }

        private void InsertSettleSumRows(List<ReportRow> rows, int groupingIndex)
        {
            InsertSettleSumRow(rows, groupingIndex, typeTownSettle, ViewParams[1].Style + 1);
            InsertSettleSumRow(rows, groupingIndex, typeVillageSettle, ViewParams[1].Style + 2);
        }

        public MOFO0021AteGrouping(int style, bool hideSettlement = false)
            : base(style + 2)
        {
            ViewParams[1].Style = style;
            if (hideSettlement)
            {
                ViewParams[3].ShowOrder = RowViewParams.ShowType.SkipBeforeChild;
                ViewParams[4].ShowOrder = RowViewParams.ShowType.SkipBeforeChild;
            }

            Function = new Control(MoveSubjectRow) + InsertMRBudjet + InsertSettleSumRows;
        }
    }

    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 001 СУММЫ ПРОГНОЗА ОМСУ ПО ДОХОДНЫМ ИСТОЧНИКАМ
        /// </summary>
        public DataTable[] GetMOFO0021Report001Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[3];
            var paramVariant = reportParams[ReportConsts.ParamVariantID];
            var paramMarks = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMark]);
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramRgnListType = reportParams[ReportConsts.ParamRegionListType];
            var paramHideEmptyStr = Convert.ToBoolean(reportParams[ReportConsts.ParamHideEmptyStr]);
            var hideSettlements = String.Compare(paramRgnListType, RegionListTypeEnum.i1.ToString(), true) != 0;

            // уровни
            var levels = new List<string>
            {
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsSubject),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Municipal),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.VillageSettle),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.TownSettle)
            };

            // года
            var nameVariant = Convert.ToString(ReportMonthMethods.GetSelectedVariantMOFOMarks(paramVariant));
            var yearMatch = Regex.Match(nameVariant, @"(?<year>\d{4})_{1}").Groups["year"].Value;
            var year = yearMatch != String.Empty ? Convert.ToInt32(yearMatch) : DateTime.Now.Year;
            var years = new[]
            {
                GetUNVYearPlanLoBound(year - 1),
                GetUNVYearPlanLoBound(year),
                GetUNVYearPlanLoBound(year + 1),
                GetUNVYearPlanLoBound(year + 2),
                GetUNVYearPlanLoBound(year + 3)
            };
            var fltYears = years.Select(y => filterHelper.EqualIntFilter(f_Marks_Forecast.RefYearDayUNV, y)).ToArray();
            
            // показатели
            var fltParamMarks = paramMarks != String.Empty
                                       ? filterHelper.RangeFilter(b_Marks_Forecast.ID, paramMarks)
                                       : dbHelper.GetSourceFilter(b_Marks_Forecast.InternalKey);
            var hMarks = new Hierarchy(b_Marks_Forecast.InternalKey, fltParamMarks);
            hMarks.Sort(b_Marks_Forecast.CodeStr);
            var marks = hMarks.ChildAll().Where(mark => mark.Level > 0)
                                         .ToDictionary(mark => mark.Id, mark => mark);
            var filterMarks = marks.Count > 0
                                  ? ConvertToString(marks.Keys)
                                  : ReportConsts.UndefinedKey;

            // получаем данные
            var filterList = new List<QFilter>
            {
                new QFilter(QFMarksForecast.Keys.Day, ConvertToString(years)),
                new QFilter(QFMarksForecast.Keys.Variant, paramVariant),
                new QFilter(QFMarksForecast.Keys.Mark, filterMarks),
                new QFilter(QFMarksForecast.Keys.Lvl, ConvertToString(levels))
            };

            var groupFields = new List<Enum>
            {
                QFMarksForecast.Keys.Day,
                QFMarksForecast.Keys.Okato,
                QFMarksForecast.Keys.Mark
            };
            var query = new QFMarksForecast().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(query);

            // параметры отчета
            var rep = new Report(f_Marks_Forecast.TableKey)
            {
                Divider = GetDividerValue(divider),
                RowFilter = paramHideEmptyStr ? Functions.IsNotNullRow : (RowViewParams.Function) null
            };

            // группировка по АТЕ
            var ateGrouping = new MOFO0021AteGrouping(0, hideSettlements);
            rep.AddGrouping(d_Regions_Plan.RefBridge, ateGrouping);
            ateGrouping.SetFixedValues(paramHideEmptyStr ? ateGrouping.AteMainId : ateGrouping.GetRegionsId());

            // сортируем и настраиваем колонки отчета
            var captionColumn = rep.AddCaptionColumn();
            captionColumn.SetMasks(ateGrouping, new AteOutMasks());

            foreach (var mark in marks)
            {
                var filterMark = filterHelper.EqualIntFilter(d_Marks_Forecast.RefMarksBridge, mark.Key);
                rep.AddValueColumn(f_Marks_Forecast.PlanMO, CombineAnd(fltYears[1], filterMark));
                rep.AddValueColumn(f_Marks_Forecast.Fact, CombineAnd(fltYears[0], filterMark));
                rep.AddValueColumn(f_Marks_Forecast.Estimate, CombineAnd(fltYears[1], filterMark));
                rep.AddValueColumn(f_Marks_Forecast.Forecast, CombineAnd(fltYears[2], filterMark));
                rep.AddValueColumn(f_Marks_Forecast.Forecast, CombineAnd(fltYears[3], filterMark));
                rep.AddValueColumn(f_Marks_Forecast.Forecast, CombineAnd(fltYears[4], filterMark));
            }

            const int columnsInYear = 6;
            var indexies = GetColumnsList(0, marks.Count);
            indexies = indexies.Where(i => !marks.ContainsKey(marks.ElementAt(i).Value.Parent.Id)).ToList();
            indexies = indexies.Select(i => captionColumn.Index + 1 + i * columnsInYear).ToList();

            if (indexies.Count > 1)
            {
                for (var i = 0; i < years.Length + 1; i++)
                {
                    var offset = i;
                    rep.AddCalcColumn((row, index) => Functions.Sum(indexies.Select(e => row.Values[e + offset])));
                }
            }

            rep.ProcessTable(tblData);
            var repDt = rep.GetReportData();

            // заполняем таблицу колонок
            var columnsDt = new DataTable();
            AddColumnToReport(columnsDt, typeof(string), NAME);

            foreach (var mark in marks.Values)
            {
                columnsDt.Rows.Add(mark.Row[b_Marks_Forecast.Name]);
            }

            if (indexies.Count > 1)
            {
                columnsDt.Rows.Add(DBNull.Value);
            }

            tablesResult[0] = repDt;
            tablesResult[1] = columnsDt;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.VARIANT, nameVariant);
            paramHelper.SetParamValue(ParamUFKHelper.YEAR, year);
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
