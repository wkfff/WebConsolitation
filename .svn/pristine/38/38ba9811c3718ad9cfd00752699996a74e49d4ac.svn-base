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
        /// 014 ПОКАЗАТЕЛИ ФОРМЫ 5-ДДК В РАЗРЕЗЕ МУНИЦИПАЛЬНЫХ ОБРАЗОВАНИЙ
        /// </summary>
        public DataTable[] GetUFNSReport014Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[3];
            var paramMark = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMark]);
            var paramRegion = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamRegionComparable]);
            var paramIncomes = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamIncomes]);
            var paramPersons = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamPersons]);
            var paramRgnListType = reportParams[ReportConsts.ParamRegionListType];
            var hideSettlement = !ReportMonthMethods.WriteSettles(paramRgnListType);
            var paramHideEmptyStr = Convert.ToBoolean(reportParams[ReportConsts.ParamHideEmptyStr]);
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var marksTable = new MarksDDKTable(QFNS5DDKRegions.Keys.Mark, QFNS5DDKRegions.Keys.MarkBridge);

            // периоды
            var years = GetSelectedYears(reportParams[ReportConsts.ParamYear], false);
            var periods = years.Select(year => GetUNVYearPlanLoBound(year)).ToList();

            // показатели
            var filterMark = paramMark != String.Empty
                ? filterHelper.RangeFilter(marksTable.Id, paramMark)
                : marksTable.GetRowsFilter();
            var hMarks = new Hierarchy(marksTable.TableKey, filterMark);
            hMarks.Sort(marksTable.Code);
            var marks = (from mark in hMarks.ChildOfLastLevel()
                         let isRub = IsRubRef(mark.Row[marksTable.RefUnits])
                         select new { mark.Id, isRub }).ToDictionary(e => e.Id, e => e.isRub);
            var filterMarks = marks.Count > 0 ? ConvertToString(marks.Keys) : ReportConsts.UndefinedKey;

            // виды доходов
            var filterIncomes = ConvertToIntList(paramIncomes);
            if (filterIncomes.Count > 0)
            {
                filterIncomes.Add(fx_Types_Incomes.All);
            }

            // виды лиц
            var filterPersons = ConvertToIntList(paramPersons);
            if (filterPersons.Count > 0)
            {
                filterPersons.Add(fx_Types_Persons.People);
            }

            // получаем данные из т.ф. «ФНС_5 ДДК_Районы» (f.D.FNS5DDKRegions)
            var filterList = new List<QFilter>
            {
                new QFilter(QFNS5DDKRegions.Keys.Day,  ConvertToString(periods)),
                new QFilter(QFNS5DDKRegions.Keys.Income,  ConvertToString(filterIncomes)),
                new QFilter(QFNS5DDKRegions.Keys.Person,  ConvertToString(filterPersons)),
                new QFilter(QFNS5DDKRegions.Keys.Okato,  paramRegion),
                new QFilter(marksTable.FilterKey,  filterMarks)
            };
            var groupList = new List<Enum>
            {
                QFNS5DDKRegions.Keys.Okato,
                marksTable.FilterKey,
                QFNS5DDKRegions.Keys.Day
            };
            var queryText = new QFNS5DDKRegions().GetQueryText(filterList, groupList);
            var tblData = dbHelper.GetTableData(queryText);

            // параметры отчета
            var rep = new Report(f_D_FNS5DDKRegions.InternalKey)
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
                var fltMark = filterHelper.EqualIntFilter(f_D_FNS5DDKRegions.RefMarks, mark.Key);

                foreach (
                    var fltPeriod in
                        periods.Select(p => filterHelper.EqualIntFilter(f_D_FNS5DDKRegions.RefYearDayUNV, p)))
                {
                    var column = rep.AddValueColumn(f_D_FNS5DDKRegions.Value, CombineAnd(fltMark, fltPeriod));
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
                var code = Convert.ToInt32(mark.Row[marksTable.Code]);
                var name = code != 0
                               ? String.Format("{0} {1}", code, mark.Row[marksTable.Name])
                               : mark.Row[marksTable.Name];
                var isRubColumn = marks.ContainsKey(mark.Id) && marks[mark.Id];
                dtMarks.Rows.Add(name, 3, mark.Level, mark.Child.Count > 0, isRubColumn, true);
            }

            tablesResult[1] = dtMarks;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.YEARS, ConvertToString(years));
            paramHelper.SetParamValue(ParamUFKHelper.PROFIT, ReportMonthMethods.GetSelectedIncomes(paramIncomes));
            paramHelper.SetParamValue(ParamUFKHelper.PERSON, ReportMonthMethods.GetSelectedPersons(paramPersons));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
