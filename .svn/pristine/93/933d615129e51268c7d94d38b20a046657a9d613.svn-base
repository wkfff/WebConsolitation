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
        /// ОТЧЕТ 010 ПОСТУПЛЕНИЕ ДОХОДОВ КБС (БС, МБ) В РАЗРЕЗЕ ДОХОДНЫХ ИСТОЧНИКОВ И МУНИЦИПАЛЬНЫХ ОБРАЗОВАНИЙ ПО НАЛОГОПЛАТЕЛЬЩИКАМ
        /// </summary>
        public DataTable[] GetUFKReport010(Dictionary<string, string> reportParams)
        {
            const int styleOrg = 5;
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[3];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var filterKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var filterKVSR = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKVSRComparable]);
            var startDate = GetUNVDate(reportParams[ReportConsts.ParamStartDate]);
            var endDate = GetUNVDate(reportParams[ReportConsts.ParamEndDate]);
            var paramShowOrg = Convert.ToBoolean(reportParams[ReportConsts.ParamOutputMode]);
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

            if (paramLvl != String.Empty)
            {
                var intLvl = ConvertToIntList(paramLvl);
                var tmpLevels = levels.Where((t, i) => intLvl.Contains(i)).ToList();
                levels = tmpLevels;
            }

            var fltLevels = String.Join(",", levels.ToArray());

            var fltPeriod = filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV, startDate, endDate, true);

            // разбиваем выбранные КД на невложенные группы
            const string keyKD = b_KD_Bridge.InternalKey;
            var selectedKD = ReportUFKHelper.GetSelectedID(filterKD, keyKD);
            var notNestedKD = ConvertToIntList(reportHelper.GetNotNestedID(keyKD, ConvertToString(selectedKD)));
            var nestedKD = new Dictionary<int, string>();
            foreach (var id in notNestedKD)
            {
                var nestedKDAll = ConvertToIntList((reportHelper.GetNestedIDByField(keyKD, b_KD_Bridge.ID, Convert.ToString(id))));
                var selectedNestedKD = nestedKDAll.Where(selectedKD.Contains);
                nestedKD.Add(id, ConvertToString(selectedNestedKD));
            }

            // фильтры фактов
            var filterList = new List<QFilter>
            {
                new QFilter(QUFK14.Keys.Period,  fltPeriod),
                new QFilter(QUFK14.Keys.Lvl,  fltLevels),
                new QFilter(QUFK14.Keys.KD,  filterKD),
                new QFilter(QUFK14.Keys.KVSR,  filterKVSR)
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
            orgGrouping.ViewParams[0].Style = styleOrg;
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

            var repColumnHeaders = new Dictionary<int, string>();
            foreach (var kd in nestedKD)
            {
                var header = reportHelper.GetKDBridgeCaptionText(Convert.ToString(kd.Key));
                repColumnHeaders.Add(kd.Key, Convert.ToString(header));
            }

            var headers = repColumnHeaders.OrderBy(item => item.Value);
            foreach (var header in headers)
            {
                var filter = filterHelper.RangeFilter(d_KD_UFK.RefKDBridge, nestedKD[header.Key]);
                var repColumn = rep.AddValueColumn(f_D_UFK14.Credit, filter);
                repColumn.Header = header.Value;
                repColumn.Style = 1;
            }

            if (nestedKD.Count > 1)
            {
                ReportCalcColumn.Calculate calculate = delegate(ReportRow row, int index)
                {
                    var values = new List<object>();
                    for (var i = 1; i <= nestedKD.Count; i++)
                    {
                        values.Add(row.Values[index - i]);
                    }

                    return Functions.Sum(values);
                };

                var calcColumn = rep.AddCalcColumn(calculate);
                calcColumn.Style = 2;
            }

            // получаем данные с группировкой по АТЕ
            var groupFields = new List<Enum> { QUFK14.Keys.Okato, QUFK14.Keys.KD };
            var query = new QUFK14().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(query);
            rep.ProcessTable(f_D_UFK14.InternalKey, tblData);
            
            // определяем организации, показываемые в отчете
            if (paramShowOrg && tblData.Rows.Count > 0)
            {
                var groupOrg = new List<Enum> {QUFK14.Keys.Okato, QUFK14.Keys.Org};
                var strSum = Convert.ToString(limitSum, CultureInfo.InvariantCulture);
                var havingFilter = filterHelper.MoreEqualFilter(QFactTable.Having(f_D_UFK14.Credit), strSum);
                var orgList = new List<int>();

                foreach (var header in headers)
                {
                    filterList[2] = new QFilter(QUFK14.Keys.KD, nestedKD[header.Key]);
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
                groupFields = new List<Enum> {QUFK14.Keys.Okato, QUFK14.Keys.Org, QUFK14.Keys.KD};
                filterList[2] = new QFilter(QUFK14.Keys.KD, filterKD);
                query = new QUFK14().GetQueryText(filterList, groupFields);
                tblData = dbHelper.GetTableData(query);
                rep.ProcessTable(f_D_UFK14.InternalKey, tblData);
            }

            tablesResult[0] = rep.GetReportData();

            // заполняем таблицу колонок
            var columnsDt = new DataTable();
            AddColumnToReport(columnsDt, typeof(string), "Name");
            AddColumnToReport(columnsDt, typeof(string), STYLE);

            foreach (var column in rep.Columns)
            {
                columnsDt.Rows.Add(column.Header, column.Style);
            }

            tablesResult[1] = columnsDt;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            var bdgtName = paramLvl != String.Empty
                               ? ReportMonthMethods.GetSelectedBudgetLvlFull(paramLvl)
                               : String.Empty;
            paramHelper.SetParamValue(ParamUFKHelper.STARTDATE, reportParams[ReportConsts.ParamStartDate]);
            paramHelper.SetParamValue(ParamUFKHelper.ENDDATE, reportParams[ReportConsts.ParamEndDate]);
            paramHelper.SetParamValue(ParamUFKHelper.KVSR, reportHelper.GetKVSRBridgeCaptionText(filterKVSR));
            paramHelper.SetParamValue(ParamUFKHelper.BDGT_NAME, bdgtName);
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            paramHelper.SetParamValue(ParamUFKHelper.SUMM, paramSum);
            return tablesResult;
        }
    }
}
