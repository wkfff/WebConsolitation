using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 001_ЕЖЕМЕСЯЧНЫЙ АНАЛИЗ ЗАДОЛЖЕННОСТИ И НЕДОИМКИ ПО КБК_65Н 
        /// </summary>
        public DataTable[] GetUFNSReport001Data(Dictionary<string, string> reportParams)
        {
            const int columnsCount = 3;
            var markDebts = GetUFNSMarkCodes(UFNSMark.Debts);
            var markArrears = GetUFNSMarkCodes(UFNSMark.Arrears);
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var paramOKVED = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamOKVED]);
            var paramRegion = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamRegionComparable]);
            var okved = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamOKVED]);
            var yearList = GetSelectedYears(reportParams[ReportConsts.ParamYear]);
            var filterPeriod = GetYearsBoundsFilter(yearList, true);
            var currentYear = DateTime.Now.Year;
            yearList = yearList.Where(year => year <= currentYear).ToList();
            
            // получаем данные из т.ф. «ФНС.28н.без расщепления» (f_F_DirtyUMNS28n)
            var filterList = new List<QFilter> 
            {
                new QFilter(QDirtyUMNS28n.Keys.Period,  filterPeriod),
                new QFilter(QDirtyUMNS28n.Keys.KD,  paramKD),
                new QFilter(QDirtyUMNS28n.Keys.Okved,  paramOKVED),
                new QFilter(QDirtyUMNS28n.Keys.Okato,  paramRegion),
                new QFilter(QDirtyUMNS28n.Keys.Mark,  String.Format("{0}, {1}", markDebts, markArrears)),
            };
            var groupList = new List<Enum> { QDirtyUMNS28n.Keys.Day, QDirtyUMNS28n.Keys.Mark };
            var queryText = new QDirtyUMNS28n().GetQueryText(filterList, groupList);
            var tblData = dbHelper.GetTableData(queryText);

            // заполняем таблицу отчета
            var repTable = new DataTable();
            AddColumnToReport(repTable, typeof(string), "Period");
            AddColumnToReport(repTable, typeof(decimal), "Sum", columnsCount);
            AddColumnToReport(repTable, typeof(int), STYLE);
            var debtsFilter = filterHelper.RangeFilter(f_F_DirtyUMNS28n.RefDataMarks65n, markDebts);
            var arrearsFilter = filterHelper.RangeFilter(f_F_DirtyUMNS28n.RefDataMarks65n, markArrears);

            foreach (var year in yearList)
            {
                var lastMonth = year < currentYear ? 12 : DateTime.Now.Month;

                for (var month = 1; month <= lastMonth; month++)
                {
                    var row = repTable.Rows.Add();
                    row["Period"] = month < 12
                                     ? String.Format("на {0}", GetMonthStart(year, month + 1))
                                     : String.Format("на {0}", GetMonthStart(year + 1, 1));
                    row[STYLE] = month % 3 > 0 ? 0 : 1;
                    var monthFilter = filterHelper.EqualIntFilter(f_F_DirtyUMNS28n.RefYearDayUNV, GetUNVMonthStart(year, month));
                    var filter = String.Format("{0} and {1}", monthFilter, debtsFilter);
                    var sum0 = GetSumFieldValue(tblData, SUM, filter);
                    filter = String.Format("{0} and {1}", monthFilter, arrearsFilter);
                    var sum1 = GetSumFieldValue(tblData, SUM, filter);

                    row["Sum0"] = sum0;
                    row["Sum1"] = sum1;
                    row["Sum2"] = GetNotNullSumDifference(sum0, sum1);
                }
            }

            // убираем строки без данных
            repTable = FilterNotExistData(repTable, GetColumnsList(1, columnsCount));

            // делим суммы в зависимости от выбранных единиц измерения
            DivideSum(repTable, 1, columnsCount, divider);

            tablesResult[0] = repTable;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            var regionValue = reportHelper.GetNotNestedRegionCaptionText(paramRegion);
            var okvedValue = reportHelper.GetBridgeCaptionText(
                b_OKVED_Bridge.InternalKey,
                okved,
                b_OKVED_Bridge.Code,
                b_OKVED_Bridge.Name
                );

            paramHelper.SetParamValue(ParamUFKHelper.OKVED, FormatOkved(okvedValue));
            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(paramKD));
            paramHelper.SetParamValue(ParamUFKHelper.SETTLEMENT, regionValue);
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
