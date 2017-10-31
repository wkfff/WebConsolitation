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
        /// ОТЧЕТ 009 - АНАЛИЗ ЗАДОЛЖЕННОСТИ И ПЕРЕПЛАТЫ В КОНСОЛИДИРОВАННЫЙ БЮДЖЕТ МОСКОВСКОЙ ОБЛАСТИ 
        /// </summary>
        public DataTable[] GetUFNSReport009Data(Dictionary<string, string> reportParams)
        {
            const int periodColumn = 0;
            const int sumColumn = 1;
            const int sumColumnsCount = 8;
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var paramArrears = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamArrearsFNS]);
            var yearList = GetSelectedYears(reportParams[ReportConsts.ParamYear]);
            var filterPeriod = GetYearsBoundsFilter(yearList, true);
            var currentYear = DateTime.Now.Year;
            yearList = yearList.Where(year => year <= currentYear).ToList();

            // получаем данные из т.ф. «Доходы.ФНС.4 НМ.Сводный» (f_D_FNS4NMTotal)
            var filterList = new List<QFilter>
            {
                new QFilter(QFNS4NMTotal.Keys.Period, filterPeriod),
                new QFilter(QFNS4NMTotal.Keys.Arrears, paramArrears),
                new QFilter(QFNS4NMTotal.Keys.RefD, GetRefDGroupByKD(paramKD))
            };
            var groupList = new List<Enum> { QFNS4NMTotal.Keys.Day };
            var queryText = new QFNS4NMTotal().GetQueryText(filterList, groupList);
            var tblFNS4Data = dbHelper.GetTableData(queryText);

            // получаем данные из т.ф. «ФНС.28н.без расщепления» (f_F_DirtyUMNS28n)
            var debtsFilter = GetUFNSMarkCodes(UFNSMark.Debts);
            var arrearsFilter = GetUFNSMarkCodes(UFNSMark.Arrears);
            var overpaymentFilter = GetUFNSMarkCodes(UFNSMark.Overpayment);
            var filterMark = String.Format("{0}, {1}, {2}", debtsFilter, arrearsFilter, overpaymentFilter);
            filterList = new List<QFilter>
            {
                new QFilter(QDirtyUMNS28n.Keys.Period, filterPeriod),
                new QFilter(QDirtyUMNS28n.Keys.Mark, filterMark),
                new QFilter(QDirtyUMNS28n.Keys.KD, paramKD)
            };
            groupList = new List<Enum> { QDirtyUMNS28n.Keys.Day, QDirtyUMNS28n.Keys.Mark };
            queryText = new QDirtyUMNS28n().GetQueryText(filterList, groupList);
            var tblUMNSData = dbHelper.GetTableData(queryText);

            // заполняем таблицу отчета
            var repTable = new DataTable();
            AddColumnToReport(repTable, typeof(string), "Period");
            AddColumnToReport(repTable, typeof(decimal), "Sum", sumColumnsCount);
            AddColumnToReport(repTable, typeof(int), STYLE);

            var debtsRangeFlt = filterHelper.RangeFilter(f_F_DirtyUMNS28n.RefDataMarks65n, debtsFilter);
            var arrearsRangeFlt = filterHelper.RangeFilter(f_F_DirtyUMNS28n.RefDataMarks65n, arrearsFilter);
            var overpaymentRangeFlt = filterHelper.RangeFilter(f_F_DirtyUMNS28n.RefDataMarks65n, overpaymentFilter);
            var difColumns = new []
            {
                sumColumn + 0,
                sumColumn + 2,
                sumColumn + 4,
                sumColumn + 6
            };

            foreach (var year in yearList)
            {
                var lastMonth = year < currentYear ? 12 : DateTime.Now.Month;
                for (var month = 1; month <= lastMonth; month++)
                {
                    var row = repTable.Rows.Add();
                    var dateFilter = filterHelper.EqualIntFilter(f_D_FNS4NMTotal.RefYearDayUNV, GetUNVMonthStart(year, month));
                    row[STYLE] = 0;
                    row[periodColumn] = month < 12
                                        ? String.Format("на {0}", GetMonthStart(year, month + 1))
                                        : String.Format("на {0}", GetMonthStart(year + 1, 1));

                    row[sumColumn + 0] = GetSumFieldValue(tblFNS4Data, SUM, dateFilter);
                    row[sumColumn + 2] = GetSumFieldValue(tblUMNSData, SUM, String.Format("{0} and {1}", dateFilter, debtsRangeFlt));
                    row[sumColumn + 4] = GetSumFieldValue(tblUMNSData, SUM, String.Format("{0} and {1}", dateFilter, arrearsRangeFlt));
                    row[sumColumn + 6] = GetSumFieldValue(tblUMNSData, SUM, String.Format("{0} and {1}", dateFilter, overpaymentRangeFlt));

                    if (repTable.Rows.Count < 2)
                    {
                        continue;
                    }

                    var prevRow = repTable.Rows[repTable.Rows.Count - 2];

                    foreach (var difColumn in difColumns)
                    {
                        row[difColumn + 1] = GetNotNullSumDifference(row[difColumn], prevRow[difColumn]);
                    }
                }
            }

            // делим суммы в зависимости от выбранных единиц измерения
            DivideSum(repTable, 1, sumColumnsCount, divider);

            tablesResult[0] = repTable;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(paramKD));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));

            return tablesResult;
        }
    }
}
