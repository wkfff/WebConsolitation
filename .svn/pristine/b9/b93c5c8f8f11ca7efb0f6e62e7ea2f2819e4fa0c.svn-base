using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{

    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 020 ПОСТУПЛЕНИЯ ДОХОДОВ В БC. ФОРМА ЕЖЕДНЕВНОГО АНАЛИЗА ПОСТУПЛЕНИЙ ПО ВЫБРАННОМУ КД
        /// </summary>
        public DataTable[] GetUFKReport020(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;
            var prevYear = year - 1;
            var paramMonth = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMonth]);
            var month = GetEnumItemIndex(new MonthEnum(), paramMonth) + 1;
            var filterKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);

            // периоды
            var prevMonthStart = new DateTime(prevYear, month, 1).ToShortDateString();
            var prevMonthEnd = new DateTime(prevYear, month, DateTime.DaysInMonth(prevYear, month)).ToShortDateString();
            var monthStart = new DateTime(year, month, 1).ToShortDateString();
            var monthEnd = new DateTime(year, month, DateTime.DaysInMonth(year, month)).ToShortDateString();
            var periods = new []
            {
                 filterHelper.PeriodFilter(d_Date_WorkingDays.DaysWorking, GetUNVDate(prevMonthStart), GetUNVDate(prevMonthEnd), true),
                 filterHelper.PeriodFilter(d_Date_WorkingDays.DaysWorking, GetUNVDate(monthStart), GetUNVDate(monthEnd), true)
            };

            // получаем рабочие дни
            var filterListWorkingDays = new List<QFilter> { new QFilter(QWorkingDays.Keys.Period, String.Join(" or ", periods)) };
            var queryWorkingDays = new QWorkingDays().GetQueryText(filterListWorkingDays);
            var tblWorkingDays = dbHelper.GetTableData(queryWorkingDays).Rows.Cast<DataRow>();
            var workingDays = tblWorkingDays.Select(row => Convert.ToInt32(row[d_Date_WorkingDays.DaysWorking]));
            workingDays = workingDays.OrderBy(day => day);
            var filterDays = workingDays.Count() > 0 ? ConvertToString(workingDays) : ReportConsts.UndefinedKey;

            // показатели
            var fltMarkReturn = filterHelper.EqualIntFilter(f_D_UFK22.RefMarks, ReportUFKHelper.MarksInpaymentsReturn);
            var fltMarkTransfer = filterHelper.EqualIntFilter(f_D_UFK22.RefMarks, ReportUFKHelper.MarksInpaymentsTransfer);
            var filterMark = Combine(ReportUFKHelper.MarksInpaymentsReturn, ReportUFKHelper.MarksInpaymentsTransfer);
            // получаем данные за рабочие дни
            var filterList = new List<QFilter>
            {
                new QFilter(QdUFK22.Keys.Day,  filterDays),
                new QFilter(QdUFK22.Keys.Marks,  filterMark),
                new QFilter(QdUFK22.Keys.KD,  filterKD)
            };

            var groupFields = new List<Enum> { QdUFK22.Keys.Day, QdUFK22.Keys.Marks };
            var query = new QdUFK22().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(query);

            // заполняем таблицу отчета
            var repDt = new DataTable();
            AddColumnToReport(repDt, typeof (string), "Date1");
            var sumFactInd1 = AddColumnToReport(repDt, typeof (decimal), "SumFact1");
            var sumReturnInd1 = AddColumnToReport(repDt, typeof(decimal), "SumReturn1");
            AddColumnToReport(repDt, typeof(string), "Date");
            var sumWaitInd = AddColumnToReport(repDt, typeof(decimal), "SumWait");
            var sumFactInd = AddColumnToReport(repDt, typeof(decimal), "SumFact");
            var sumReturnInd = AddColumnToReport(repDt, typeof(decimal), "SumReturn");
            AddColumnToReport(repDt, typeof(int), STYLE);

            var days = workingDays.Where(day => day >= Convert.ToInt32(GetUNVDate(monthStart))).ToList();
            var prevDays = workingDays.Where(day => day <= Convert.ToInt32(GetUNVDate(prevMonthEnd))).ToList();
            var count = Math.Max(days.Count, prevDays.Count);

            for (var i = 0; i < count; i++)
            {
                var row = repDt.Rows.Add();
                row[sumWaitInd] = DBNull.Value;
                row[STYLE] = 0;

                if (i < prevDays.Count)
                {
                    var fltDay = filterHelper.EqualIntFilter(f_D_UFK22.RefYearDayUNV, prevDays[i]);
                    row["Date1"] = GetNormalDate(prevDays[i]).ToShortDateString();
                    row[sumFactInd1] = GetSumFieldValue(tblData, SUM, CombineAnd(fltDay, fltMarkTransfer));
                    row[sumReturnInd1] = GetSumFieldValue(tblData, SUM, CombineAnd(fltDay, fltMarkReturn));
                }

                if (i < days.Count)
                {
                    var fltDay = filterHelper.EqualIntFilter(f_D_UFK22.RefYearDayUNV, days[i]);
                    row["Date"] = GetNormalDate(days[i]).ToShortDateString();
                    row[sumFactInd] = GetSumFieldValue(tblData, SUM, CombineAnd(fltDay, fltMarkTransfer));
                    row[sumReturnInd] = GetSumFieldValue(tblData, SUM, CombineAnd(fltDay, fltMarkReturn));
                }
            }

            // заключительные обороты
            if (month == 12)
            {
                var zoDays = new[] { GetUNVDate(prevYear, month, 32), GetUNVDate(year, month, 32) };
                filterList[0] = new QFilter(QdUFK22.Keys.Day, ConvertToString(zoDays));
                query = new QdUFK22().GetQueryText(filterList, groupFields);
                tblData = dbHelper.GetTableData(query);
                var row = repDt.Rows.Add();
                var fltDay1 = filterHelper.EqualIntFilter(f_D_UFK22.RefYearDayUNV, zoDays[0]);
                row[sumFactInd1] = GetSumFieldValue(tblData, SUM, CombineAnd(fltDay1, fltMarkTransfer));
                row[sumReturnInd1] = GetSumFieldValue(tblData, SUM, CombineAnd(fltDay1, fltMarkReturn));
                var fltDay = filterHelper.EqualIntFilter(f_D_UFK22.RefYearDayUNV, zoDays[1]);
                row[sumFactInd] = GetSumFieldValue(tblData, SUM, CombineAnd(fltDay, fltMarkTransfer));
                row[sumReturnInd] = GetSumFieldValue(tblData, SUM, CombineAnd(fltDay, fltMarkReturn));
            }

            // Итого
            var totalRow = repDt.Rows.Add();
            totalRow[sumFactInd] = GetSumFieldValue(repDt, sumFactInd, String.Empty);
            totalRow[sumReturnInd] = GetSumFieldValue(repDt, sumReturnInd, String.Empty);
            totalRow[sumFactInd1] = GetSumFieldValue(repDt, sumFactInd1, String.Empty);
            totalRow[sumReturnInd1] = GetSumFieldValue(repDt, sumReturnInd1, String.Empty);
            totalRow[sumWaitInd] = GetSumFieldValue(repDt, sumWaitInd, String.Empty);

            // делим суммы в зависимости от выбранных единиц измерения
            DivideSum(repDt, sumFactInd, 1, divider);
            DivideSum(repDt, sumReturnInd, 1, divider);
            DivideSum(repDt, sumFactInd1, 1, divider);
            DivideSum(repDt, sumReturnInd1, 1, divider);
            DivideSum(repDt, sumWaitInd, 1, divider);

            tablesResult[0] = repDt;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.YEARS, year);
            paramHelper.SetParamValue(ParamUFKHelper.MONTH, month);
            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(filterKD));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
