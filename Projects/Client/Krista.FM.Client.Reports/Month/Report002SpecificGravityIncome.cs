using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Month;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK;
using Krista.FM.Client.Reports.Database.FactTables;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 002_УДЕЛЬНЫЙ ВЕС ЕЖЕМЕСЯЧНЫХ И КВАРТАЛЬНЫХ ПОСТУПЛЕНИЙ
        /// </summary>
        public DataTable[] GetMonthReport002SpecificGravityData(Dictionary<string, string> reportParams)
        {
            var dbHelper = new ReportDBHelper(scheme);
            var filterHelper = new QFilterHelper();
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramLvl = reportParams[ReportConsts.ParamBdgtLevels];
            var filterLvl = ReportMonthMethods.GetBdgtLvlSKIFCodes(paramLvl);
            var filterKD = reportParams[ReportConsts.ParamKDComparable];
            var yearParam = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var yearList = new List<string>(yearParam.Split(','));
            var tblResult = CreateReportCaptionTable(yearList.Count, 12);
            var currentColumn = 0;
            var lstYearValues = yearList.Select(ReportMonthMethods.GetSelectedYear).ToList();

            if (yearParam.Length == 0)
            {
                lstYearValues.Clear();
            }
            else
            {
                lstYearValues.Sort();
            }

            var qMonthParams = new QMonthRepParams
            {
                KD = filterKD,
                Lvl = filterLvl,
                DocType = ReportMonthConsts.DocTypeConsolidate
            };

            var groupMonthInfo = new List<QMonthRepGroup> { QMonthRepGroup.Lvl, QMonthRepGroup.Period };

            var qFOYRParams = new QFOYRParams
            {
                KD = filterKD,
                Lvl = filterLvl,
                DocType = ReportMonthConsts.DocTypeConsolidate
            };

            var groupFOYRInfo = new List<QFOYRGroup> { QFOYRGroup.Lvl, QFOYRGroup.Period };

            foreach (var currentYear in lstYearValues)
            {
                var yearLoBound = GetUNVYearLoBound(currentYear);
                var yearHiBound = GetUNVYearEnd(currentYear);
                var yearPeriod = filterHelper.PeriodFilter(
                    f_F_MonthRepIncomes.RefYearDayUNV,
                    yearLoBound,
                    yearHiBound,
                    true);
                qMonthParams.Period = yearPeriod;
                var queryMonText = QMonthRepIncomes.GroupBy(qMonthParams, groupMonthInfo);
                var tblDataMonth = dbHelper.GetTableData(queryMonText);
                qFOYRParams.Period = yearPeriod;
                var queryYrText = QFOYRIncomes.GroupBy(qFOYRParams, groupFOYRInfo);
                var tblDataYr = dbHelper.GetTableData(queryYrText);

                var filterYearPeriod = filterHelper.PeriodFilter(
                    f_D_FOYRIncomes.RefYearDayUNV,
                    GetUNVYearLoBound(currentYear),
                    GetUNVYearEnd(currentYear));

                var rowsYear = tblDataYr.Select(filterYearPeriod);
                var hasYearData = rowsYear.Length > 0;

                for (var i = 1; i < 13; i++)
                {
                    var rowIndex = i - 1;
                    var periodStart = GetUNVMonthStart(currentYear, i);

                    var filterMonth = filterHelper.EqualIntFilter(f_F_MonthRepIncomes.RefYearDayUNV, periodStart);
                    var rowsMonth = tblDataMonth.Select(filterMonth);
                    var columnDataIndex = GetColumnIndex(tblDataMonth, f_F_MonthRepIncomes.Fact);

                    if (i == 12 && hasYearData)
                    {
                        columnDataIndex = GetColumnIndex(tblDataYr, f_D_FOYRIncomes.Performed);
                        rowsMonth = rowsYear;
                    }

                    var sum = rowsMonth.Sum(dataRow => GetDecimal(dataRow[columnDataIndex]));
                    tblResult.Rows[rowIndex][currentColumn] = sum;
                }

                DivideColumn(tblResult, currentColumn, divider);
                RoundColumn(tblResult, currentColumn, precision);
                rowCaption[10 + currentColumn] = currentYear;
                currentColumn++;
            }

            tablesResult[0] = tblResult;
            rowCaption[0] = lstYearValues.Count;
            rowCaption[1] = reportHelper.GetKDBridgeCaptionText(filterKD);
            rowCaption[2] = ReportMonthMethods.GetSelectedBudgetLvl(paramLvl);
            rowCaption[4] = ReportMonthMethods.GetDividerDescr(divider);
            rowCaption[7] = ReportMonthMethods.GetPrecisionIndex(precision);
            return tablesResult;
        }
    }
}
