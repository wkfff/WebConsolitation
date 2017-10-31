using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData.UFK;
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
        /// ОТЧЕТ 009 ПОСТУПЛЕНИЕ ДОХОДОВ С ПОКВАРТАЛЬНОЙ РАЗБИВКОЙ ПО КБК
        /// </summary>
        public DataTable[] GetUFKReport009QuarterPayments(Dictionary<string, string> reportParams)
        {
            const int orgStyle = 5;
            const int firstSumColumn = 1;
            const int firstCalcColumn = 13;
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var filterKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;
            var paramQuarter = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamQuarter]);
            var paramShowOrg = Convert.ToBoolean(reportParams[ReportConsts.ParamOutputMode]);
            var paramSum = paramShowOrg ? Convert.ToDecimal(reportParams[ReportConsts.ParamSum]) : 0;
            var limitSum = paramSum * ReportUFKHelper.LimitSumRate;
            var paramLvl = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamBdgtLevels]);

            // уровни
            var levels = new List<string>
            {
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsSubjectFractional),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsMunicipalFractional)
            };

            var intParamLvl = paramLvl != String.Empty ? ConvertToIntList(paramLvl) : GetColumnsList(0, levels.Count);
            levels = levels.Where((t, i) => intParamLvl.Contains(i)).ToList();
            var intParamQuarters = paramQuarter != String.Empty ? ConvertToIntList(paramQuarter) : GetColumnsList(0, 4);
            var quarters = (from i in intParamQuarters
                            let qStartDate = GetUNVDate(Convert.ToDateTime(GetMonthStart(year, 1 + i*3)))
                            let qEndDate = GetUNVDate(Convert.ToDateTime(GetMonthEnd(year, 3 + i*3)))
                            select filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV, qStartDate, qEndDate, true)).ToList();

            // фильтры фактов
            var filterList = new List<QFilter>
            {
                new QFilter(QUFK14.Keys.Period,  String.Empty), // зарезервированио
                new QFilter(QUFK14.Keys.Lvl,  String.Empty),    // зарезервированио
                new QFilter(QUFK14.Keys.KD,  filterKD)
            };

            // параметры отчета
            var rep = new Report(f_D_UFK14.InternalKey) {Divider = GetDividerValue(divider)};
            var dividedLimitSum = limitSum / rep.Divider;

            // группировка по АТЕ
            var ateGrouping = rep.AddGrouping(d_OKATO_UFK.RefRegionsBridge,
                                              new AteGrouping(0) {HideConsBudjetRow = true});
            // группировка по организациям
            const string orgTableKey = b_Org_PayersBridge.InternalKey;
            var orgGrouping = rep.AddGrouping(d_Org_UFKPayers.RefOrgPayersBridge);
            orgGrouping.AddLookupField(orgTableKey, b_Org_PayersBridge.INN, b_Org_PayersBridge.Name);
            orgGrouping.AddSortField(orgTableKey, b_Org_PayersBridge.INN);
            orgGrouping.ViewParams[0].Style = orgStyle;
            orgGrouping.ViewParams[0].BreakSumming = true; // исключаем строки с организациями из суммирования, т.к. суммы по районам получим отдельным запросом
            // показывать будем только строки, где сумма выше пороговой
            orgGrouping.ViewParams[0].Filter = delegate(ReportRow row, List<ReportColumn> columns)
            {
                return columns.Where(column => column.ColumnType == ReportColumnType.Value)
                              .Any(column => GetDecimal(row.Values[column.Index]) >= dividedLimitSum);
            };

            // сортируем и настраиваем колонки отчета
            var captionColumn = rep.AddCaptionColumn();
            captionColumn.Style = 0;
            captionColumn.SetMask(orgGrouping, 0, orgTableKey, b_Org_PayersBridge.INN, b_Org_PayersBridge.Name);
            captionColumn.SetMasks(ateGrouping, new AteOutMasks());
            var quarterFilters = new List<string>();
            var levelFilters = new List<string>();

            foreach (var quarter in quarters)
            {
                foreach (var level in levels)
                {
                    rep.AddValueColumn(f_D_UFK14.Credit);
                    quarterFilters.Add(quarter);
                    levelFilters.Add(level);
                }
            }

            // вычисляемые колонки
            if (quarters.Count > 1)
            {
                for (var l = 0; l < levels.Count; l++)
                {
                    var levelColumns = quarters.Select((t, i) => firstSumColumn + i*levels.Count + l).ToList();
                    rep.AddCalcColumn((row, index) => Functions.Sum(levelColumns.Select(i => row.Values[i])));
                }
            }

            // получаем данные с группировкой по АТЕ
            var groupFields = new List<Enum> { QUFK14.Keys.Okato };
            var existData = false;
            var sumColumnsCount = quarters.Count * levels.Count;

            for (var i = 0; i < sumColumnsCount; i++)
            {
                filterList[0] = new QFilter(QUFK14.Keys.Period, quarterFilters[i]);
                filterList[1] = new QFilter(QUFK14.Keys.Lvl, levelFilters[i]);
                var query = new QUFK14().GetQueryText(filterList, groupFields);
                var tblData = dbHelper.GetTableData(query);
                rep.ProcessDataRows(tblData.Select(), firstSumColumn + i);
                existData = existData || tblData.Rows.Count > 0;
            }

            // определяем организации, показываемые в отчете
            if (paramShowOrg && existData)
            {
                var groupOrg = new List<Enum> { QUFK14.Keys.Okato, QUFK14.Keys.Org };
                var orgList = new List<int>();
                var strSum = Convert.ToString(limitSum, CultureInfo.InvariantCulture);
                var havingFilter = filterHelper.MoreEqualFilter(QFactTable.Having(f_D_UFK14.Credit), strSum);

                for (var i = 0; i < sumColumnsCount; i++)
                {
                    filterList[0] = new QFilter(QUFK14.Keys.Period, quarterFilters[i]);
                    filterList[1] = new QFilter(QUFK14.Keys.Lvl, levelFilters[i]);
                    var queryOrg = new QUFK14().GetQueryText(filterList, groupOrg, havingFilter);
                    var tblPayerData = dbHelper.GetTableData(queryOrg);
                    orgList.AddRange(from DataRow row in tblPayerData.Rows
                                     select Convert.ToInt32(row[d_Org_UFKPayers.RefOrgPayersBridge]));
                }

                orgList = orgList.Distinct().ToList();
                var fltPayers = orgList.Count > 0
                                    ? ReportMonthMethods.CreateRangeFilter(orgList)
                                    : ReportConsts.UndefinedKey;
                filterList.Add(new QFilter(QUFK14.Keys.Org, fltPayers));

                // получаем данные с группировкой по организациям
                groupFields = new List<Enum> { QUFK14.Keys.Okato, QUFK14.Keys.Org };

                for (var i = 0; i < sumColumnsCount; i++)
                {
                    filterList[0] = new QFilter(QUFK14.Keys.Period, quarterFilters[i]);
                    filterList[1] = new QFilter(QUFK14.Keys.Lvl, levelFilters[i]);
                    var query = new QUFK14().GetQueryText(filterList, groupFields);
                    var tblData = dbHelper.GetTableData(query);
                    rep.ProcessDataRows(tblData.Select(), firstSumColumn + i);
                }
            }

            tablesResult[0] = rep.GetReportData();

            // колонки
            var repColumns = new List<int> {0};
            repColumns.AddRange(from i in intParamQuarters from n in intParamLvl
                                select firstSumColumn + i * intParamLvl.Count + n);
            if (quarters.Count > 1)
            {
                repColumns.AddRange(from n in intParamLvl select firstCalcColumn + n);
            }

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            var bdgtName = paramLvl != String.Empty
                               ? ReportMonthMethods.GetSelectedBudgetLvlFull(paramLvl)
                               : String.Empty;
            paramHelper.SetParamValue(ParamUFKHelper.YEAR, year);
            paramHelper.SetParamValue(ParamUFKHelper.COLUMNS, ConvertToString(repColumns));
            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(filterKD));
            paramHelper.SetParamValue(ParamUFKHelper.BDGT_NAME, bdgtName);
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            paramHelper.SetParamValue(ParamUFKHelper.SUMM, paramSum);
            return tablesResult;
        }
    }
}
