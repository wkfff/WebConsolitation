using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData.UFK;
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
        /// ОТЧЕТ 019 ЕЖЕМЕСЯЧНЫЕ СУММЫ ВОЗВРАТОВ НАЛОГА В БС В ТЕКУЩЕМ ГОДУ В РАЗРЕЗЕ ТЕРРИТОРИЙ И НАЛОГОПЛАТЕЛЬЩИКОВ
        /// </summary>
        public DataTable[] GetUFKReport019(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;
            var paramMonth = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMonth]);
            var month = GetEnumItemIndex(new MonthEnum(), paramMonth) + 1;
            var paramShowOrg = Convert.ToBoolean(reportParams[ReportConsts.ParamOutputMode]);
            var filterKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var paramSum = paramShowOrg ? Convert.ToDecimal(reportParams[ReportConsts.ParamSum]) : 0;
            var limitSum = paramSum * ReportUFKHelper.LimitSumRate;
            var paramLvl = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamBdgtLevels]);

            // уровни
            var levels = new List<string>
            {
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Federal),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsSubjectFractional),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsMunicipalFractional),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Municipal),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Settle),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Fonds),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.PurposeFonds)
            };

            var intParamLvl = paramLvl != String.Empty ? ConvertToIntList(paramLvl) : new List<int> {1};
            levels = levels.Where((t, i) => intParamLvl.Contains(i)).ToList();
            var levelsAll = GetDistinctCodes(ConvertToString(levels));

            // периоды
            var periods = new List<string>();

            for (var i = 1; i <= month; i++)
            {
                var monthStart = GetUNVMonthStart(year, i);
                var monthEnd = GetUNVMonthEnd(year, i);
                periods.Add(filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV, monthStart, monthEnd, true));
            }

            // фильтры фактов
            var filterList = new List<QFilter>
            {
                new QFilter(QUFK14.Keys.Period, String.Empty), // зарезервировано
                new QFilter(QUFK14.Keys.Lvl, levelsAll),
                new QFilter(QUFK14.Keys.KD, filterKD),
                new QFilter(QUFK14.Keys.Oper, fx_FX_OpertnTypes.Return),
            };

            // параметры отчета
            var rep = new Report(f_D_UFK14.InternalKey) {Divider = GetDividerValue(divider)};
            var dividedLimitSum = limitSum / rep.Divider;

            // группировка по АТЕ
            var ateGrouping = rep.AddGrouping(d_OKATO_UFK.RefRegionsBridge, new AteGrouping(1));

            // группировка по организациям
            const string orgTableKey = b_Org_PayersBridge.InternalKey;
            var orgGrouping = rep.AddGrouping(d_Org_UFKPayers.RefOrgPayersBridge);
            orgGrouping.AddLookupField(orgTableKey, b_Org_PayersBridge.INN, b_Org_PayersBridge.Name);
            orgGrouping.AddSortField(orgTableKey, b_Org_PayersBridge.INN);
            orgGrouping.ViewParams[0].Style = 0;
            orgGrouping.ViewParams[0].BreakSumming = true; // исключаем строки с организациями из суммирования, т.к. суммы по районам получим отдельным запросом
            // показывать будем только строки, где сумма выше пороговой
            orgGrouping.ViewParams[0].Filter = delegate(ReportRow row, List<ReportColumn> columns)
            {
                return columns.Where(column => column.ColumnType == ReportColumnType.Value)
                              .Any(column => Math.Abs(GetDecimal(row.Values[column.Index])) >= dividedLimitSum);
            };
            
            // сортируем и настраиваем колонки отчета
            var captionColumn = rep.AddCaptionColumn();
            captionColumn.Style = 0;
            captionColumn.SetMask(orgGrouping, 0, orgTableKey, b_Org_PayersBridge.INN, b_Org_PayersBridge.Name);
            captionColumn.SetMasks(ateGrouping, new AteOutMasks());

            foreach (var period in periods)
            {
                rep.AddValueColumn(f_D_UFK14.Credit).K = -1;
            }

            if (periods.Count > 1)
            {
                var sumColumn = rep.AddCalcColumn(
                    (row, index) => Functions.Sum(periods.Select((p, i) => row.Values[index - i - 1])));
                sumColumn.K = -1;
            }

            // получаем данные с группировкой по АТЕ
            var groupFields = new List<Enum> { QUFK14.Keys.Okato };
            var сolumnIndex = captionColumn.Index;
            var dataExist = false;

            foreach (var period in periods)
            {
                filterList[0].Filter = period;
                var query = new QUFK14().GetQueryText(filterList, groupFields);
                var tblData = dbHelper.GetTableData(query);
                rep.ProcessDataRows(tblData.Select(), ++сolumnIndex);
                dataExist = dataExist || tblData.Rows.Count > 0;
            }

            // определяем организации, показываемые в отчете
            if (paramShowOrg && dataExist)
            {
                var groupOrg = new List<Enum> { QUFK14.Keys.Okato, QUFK14.Keys.Org };
                var orgList = new List<int>();

                foreach (var period in periods)
                {
                    filterList[0].Filter = period;
                    var strSum = Convert.ToString(limitSum, CultureInfo.InvariantCulture);
                    var havingFilter = filterHelper.MoreEqualFilter(QFactTable.Having(f_D_UFK14.Credit, -1), strSum);
                    var queryOrg = new QUFK14().GetQueryText(filterList, groupOrg, havingFilter);
                    var tblPayerData = dbHelper.GetTableData(queryOrg);
                    orgList.AddRange(from DataRow row in tblPayerData.Rows
                                     select Convert.ToInt32(row[d_Org_UFKPayers.RefOrgPayersBridge]));
                }

                orgList = orgList.Distinct().ToList();
                var fltPayers = orgList.Count > 0 ? ConvertToString(orgList) : ReportConsts.UndefinedKey;
                filterList.Add(new QFilter(QUFK14.Keys.Org, fltPayers));

                // получаем данные с группировкой по организациям
                var сolumn = captionColumn.Index;
                foreach (var period in periods)
                {
                    filterList[0].Filter = period;
                    var query = new QUFK14().GetQueryText(filterList, groupOrg);
                    var tblData = dbHelper.GetTableData(query);
                    rep.ProcessDataRows(tblData.Select(), ++сolumn);
                }
            }

            tablesResult[0] = rep.GetReportData();

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            var bdgtName = paramLvl != String.Empty
                               ? ReportMonthMethods.GetSelectedBudgetLvlFull(paramLvl)
                               : String.Empty;
            paramHelper.SetParamValue(ParamUFKHelper.YEAR, year);
            paramHelper.SetParamValue(ParamUFKHelper.BDGT_NAME, bdgtName);
            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(filterKD));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            paramHelper.SetParamValue(ParamUFKHelper.SUMM, paramSum);
            return tablesResult;
        }
    }
}
