using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{

    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 021 ДИНАМИКА ПОСТУПЛЕНИЙ ПО КД
        /// </summary>
        public DataTable[] GetUFKReport021(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[3];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var filterKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var filterKVSR = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKVSRComparable]);
            var paramLvl = SortCodes(ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamBdgtLevels]));
            var yearList = GetSelectedYears(reportParams[ReportConsts.ParamYear]);

            // уровни
            var levels = new List<string>
            {
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsSubjectFractional),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsMunicipalFractional)
            };

            var intLvl = paramLvl != String.Empty ? ConvertToIntList(paramLvl) : GetColumnsList(0, levels.Count);
            levels = levels.Where((t, i) => intLvl.Contains(i)).ToList();

            // фильтры фактов
            var filterList = new List<QFilter>
            {
                new QFilter(QUFK14.Keys.Period, String.Empty),
                new QFilter(QUFK14.Keys.Lvl, String.Empty),
                new QFilter(QUFK14.Keys.KD, filterKD),
                new QFilter(QUFK14.Keys.KVSR, filterKVSR)
            };

            // заполняем таблицу отчета
            var sumColumnsCount = yearList.Count*levels.Count;
            var repTable = new DataTable();
            AddColumnToReport(repTable, typeof(decimal), SUM, sumColumnsCount);
            AddColumnToReport(repTable, typeof(int), STYLE);

            for (var month = 1; month <= 12; month++)
            {
                var row = repTable.Rows.Add();
                row[STYLE] = month - 1;
                var i = 0;

                foreach (var year in yearList)
                {
                    var monthStart = GetUNVMonthStart(year, month);
                    var monthEnd = GetUNVMonthEnd(year, month);
                    var period = filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV, monthStart, monthEnd, true);
                    filterList[0] = new QFilter(QUFK14.Keys.Period, period);

                    foreach (var level in levels)
                    {
                        filterList[1] = new QFilter(QUFK14.Keys.Lvl, level);
                        var query = new QUFK14().GetQueryText(filterList);
                        var tblData = dbHelper.GetTableData(query);
                        row[i++] = GetSumFieldValue(tblData, f_D_UFK14.Credit, String.Empty);
                    }
                }
            }
            
            // делим суммы в зависимости от выбранных единиц измерения
            DivideSum(repTable, 0, sumColumnsCount, divider);

            tablesResult[0] = repTable;

            // заполняем таблицу колонок
            var columnsDt = new DataTable();
            AddColumnToReport(columnsDt, typeof(string), NAME);
            AddColumnToReport(columnsDt, typeof(string), STYLE);
            columnsDt.Rows.Add(String.Empty, 0);
            var firstYearStyles = new[] {1, 1 + levels.Count, 1 + levels.Count*3};

            for (var i = 0; i < yearList.Count; i++ )
            {
                var firstYearStyle = i < 2 ? firstYearStyles[i] : firstYearStyles[2];

                for (var l = 0; l < levels.Count; l++)
                {
                    var index = i == 0 ? firstYearStyle + l : firstYearStyle + l * 2;
                    columnsDt.Rows.Add(DBNull.Value, index);
                    if (i > 0)
                    {
                        columnsDt.Rows.Add(DBNull.Value, index + 1);
                    }
                }
            }

            tablesResult[1] = columnsDt;
            
            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.YEARS, ConvertToString(yearList));
            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(filterKD));
            paramHelper.SetParamValue(ParamUFKHelper.KVSR, reportHelper.GetKVSRBridgeCaptionText(filterKVSR));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            paramHelper.SetParamValue(ParamUFKHelper.BDGT_LEVEL, ConvertToString(intLvl));
            return tablesResult;
        }
    }
}
