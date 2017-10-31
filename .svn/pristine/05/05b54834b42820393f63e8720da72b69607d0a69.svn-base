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
        /// 005_ПЗ_ЕЖЕМЕС. АНАЛИЗ НАЧИСЛЕНИЙ ПО НАЛОГАМ ПО ФОРМЕ 1-НМ 
        /// </summary>
        public DataTable[] GetUFNSReport005Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[3];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var selectedYear = paramYear != String.Empty
                                ? ReportMonthMethods.GetSelectedYear(paramYear)
                                : DateTime.Now.Year;
            var yearList = new List<int>
            {
                selectedYear - 3,
                selectedYear - 2,
                selectedYear - 1,
                selectedYear - 0
            };
  
            var filterPeriod = GetYearsBoundsFilter(yearList, true);

            // получаем данные из т.ф. «Доходы.ФНС.1 НМ Сводный» (f_D_FNS3Cons).
            var filterList = new List<QFilter> 
            {
                new QFilter(QFNS3Cons.Keys.Period,  filterPeriod),
                new QFilter(QFNS3Cons.Keys.KD,  paramKD)
            };
            var groupList = new List<Enum> { QFNS3Cons.Keys.Day };
            var queryText = new QFNS3Cons().GetQueryText(filterList, groupList);
            var tblData = dbHelper.GetTableData(queryText);

            // заполняем таблицу отчета
            var repTable = new DataTable();
            AddColumnToReport(repTable, typeof(string), "Period");
            AddColumnToReport(repTable, typeof(decimal), String.Format("Sum{0}", yearList[0]));
            var sumColumns = new List<int> { 1 };

            for (var i = 1; i < yearList.Count; i++)
            {
                AddColumnToReport(repTable, typeof(decimal), String.Format("Sum{0}", yearList[i]));
                AddColumnToReport(repTable, typeof(decimal), String.Format("Temp{0}", yearList[i]));
                sumColumns.Add(2 * i);
            }

            AddColumnToReport(repTable, typeof(int), STYLE);

            var jan = GetMonthRusNames()[0];

            for (var month = 1; month < 13; month++)
            {
                var row = repTable.Rows.Add();
                for (var i = 0; i < yearList.Count; i++)
                {
                    var year = yearList[i];
                    var filter = filterHelper.EqualIntFilter(f_D_FNS3Cons.RefYearDayUNV, GetUNVMonthStart(year, month));
                    var rows = tblData.Select(filter);
                    var sum = rows.Sum(r => GetDecimal(r["Sum0"]));

                    row["Period"] = month == 1 ? jan : String.Format("{0} - {1}", jan, GetMonthRusNames()[month - 1]);
                    row[sumColumns[i]] = sum;
                    if (i > 0)
                    {
                        var prevSum = GetDecimal(row[sumColumns[i - 1]]);
                        if (prevSum > 0)
                        {
                            row[sumColumns[i] + 1] = sum / prevSum * 100;
                        }
                    }
                    row[STYLE] = 0;
                }
            }

            // делим суммы в зависимости от выбранных единиц измерения
            foreach (var column in sumColumns)
            {
                DivideSum(repTable, column, 1, divider);
            }

            // заполняем таблицу колонок
            var columnsDt = new DataTable();
            AddColumnToReport(columnsDt, typeof(string), "Name");
            AddColumnToReport(columnsDt, typeof(string), STYLE);
            columnsDt.Rows.Add(String.Empty, 0);
            columnsDt.Rows.Add(yearList[0], 1);

            for (var i = 1; i < yearList.Count; i++)
            {
                columnsDt.Rows.Add(yearList[i], 1);
                columnsDt.Rows.Add("Темп роста в %", 2);
            }


            tablesResult[0] = repTable;
            tablesResult[1] = columnsDt;

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
