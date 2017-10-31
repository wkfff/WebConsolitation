using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Common.CommonParamForm;
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
        /// 016 ПОКАЗАТЕЛИ ФОРМЫ 5-УСН В РАЗРЕЗЕ МУНИЦИПАЛЬНЫХ ОБРАЗОВАНИЙ
        /// </summary>
        public DataTable[] GetUFNSReport016Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[3];
            var paramMark = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMark]);
            var paramRegion = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamRegionComparable]);
            var paramPersons = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamPersons]);
            var paramRgnListType = reportParams[ReportConsts.ParamRegionListType];
            var paramHideEmptyStr = Convert.ToBoolean(reportParams[ReportConsts.ParamHideEmptyStr]);
            var hideSettlement = !ReportMonthMethods.WriteSettles(paramRgnListType);
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var marksTable = new MarksYSNTable(QFNS5YSNRegions.Keys.Mark, QFNS5YSNRegions.Keys.MarkBridge);

            // периоды
            var years = GetSelectedYears(reportParams[ReportConsts.ParamYear], false);
            var periods = years.Select(year => GetUNVMonthStart(year, 12)).ToList();

            // показатели
            var filterMark = paramMark != String.Empty
                ? filterHelper.RangeFilter(marksTable.Id, paramMark)
                : marksTable.GetRowsFilter();
            var hMarks = new Hierarchy(marksTable.TableKey, filterMark);
            hMarks.Sort(marksTable.Code);
            var marksAll = hMarks.ChildAll();
            var marks = (from mark in marksAll
                         where mark.Level > 0
                         let isRub = IsRubRef(mark.Row[marksTable.RefUnits])
                         select new { mark.Id, isRub }).ToDictionary(e => e.Id, e => e.isRub);
            var filterMarks = marks.Count > 0 ? ConvertToString(marks.Keys) : ReportConsts.UndefinedKey;

            // виды лиц
            var personsFilter = ParamStore.container[ReportConsts.ParamPersons].BookInfo.GetRowsFilter();
            var tblPersons = dbHelper.GetEntityData(fx_Types_Persons.InternalKey, personsFilter);
            var hPersons = new Hierarchy(tblPersons, fx_Types_Persons.InternalKey);
            var persons = paramPersons == String.Empty
                ? hPersons.ChildAll()
                : hPersons.ChildAll().Where(e => ConvertToIntList(paramPersons).Contains(e.Id)).ToList();
            var allPersonsId = persons.SelectMany(person => person.ToList().Select(e => e.Id)).ToList();


            // получаем данные из т.ф. «ФНС_5 УСН_Районы» (f.D.FNS5YSNRegions)
            var filterList = new List<QFilter>
            {
                new QFilter(QFNS5YSNRegions.Keys.Day,  ConvertToString(periods)),
                new QFilter(QFNS5YSNRegions.Keys.Person,  ConvertToString(allPersonsId)),
                new QFilter(QFNS5YSNRegions.Keys.Okato,  paramRegion),
                new QFilter(marksTable.FilterKey,  filterMarks)
            };
            var groupList = new List<Enum>
            {
                QFNS5YSNRegions.Keys.Okato,
                marksTable.FilterKey,
                QFNS5YSNRegions.Keys.Day
            };
            var queryText = new QFNS5YSNRegions().GetQueryText(filterList, groupList);
            var tblData = dbHelper.GetTableData(queryText);

            // параметры отчета
            var rep = new Report(f_D_FNS5YSNRegions.InternalKey)
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
                var fltMark = filterHelper.EqualIntFilter(f_D_FNS5YSNRegions.RefMarks, mark.Key);

                foreach (
                    var fltPeriod in
                        periods.Select(p => filterHelper.EqualIntFilter(f_D_FNS5YSNRegions.RefYearDayUNV, p)))
                {
                    var column = rep.AddValueColumn(f_D_FNS5YSNRegions.ValueReport, CombineAnd(fltMark, fltPeriod));
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
            dtMarks.Columns.Add(LEVEL, typeof(int));
            dtMarks.Columns.Add(MERGED, typeof(bool));
            dtMarks.Columns.Add(IsRUB, typeof(bool));

            foreach (var mark in marksAll)
            {
                var code = Convert.ToInt32(mark.Row[marksTable.Code]);
                var strCode = Convert.ToString(code).PadLeft(3, '0');
                var name = code != 0
                               ? String.Format("{0}\r\n(стр. {1})", mark.Row[marksTable.Name], strCode)
                               : mark.Row[marksTable.Name];
                var isRubColumn = marks.ContainsKey(mark.Id) && marks[mark.Id];
                dtMarks.Rows.Add(name, mark.Level, mark.HasChild, isRubColumn);
            }

            tablesResult[1] = dtMarks;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.YEARS, ConvertToString(years));
            paramHelper.SetParamValue(ParamUFKHelper.PERSON, ReportMonthMethods.GetSelectedPersons(paramPersons));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
