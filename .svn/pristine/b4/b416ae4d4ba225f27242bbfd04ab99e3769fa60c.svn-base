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
        /// ОТЧЕТ 001_ДИНАМИКА ЕЖЕМЕСЯЧНЫХ ПОСТУПЛЕНИЙ В БЮДЖЕТЫ
        /// </summary>
        public DataTable[] GetMonthReport001DynamicIncomeData(Dictionary<string, string> reportParams)
        {
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var filterHelper = new QFilterHelper();
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramLvl = reportParams[ReportConsts.ParamBdgtLevels];
            var filterLvl = ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.Summary);
            var filterKD = reportParams[ReportConsts.ParamKDComparable];
            var yearParam = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var yearCodes = yearParam.Split(',');
            var yearList = new List<int>(yearCodes.Select(ReportMonthMethods.GetSelectedYear));
            var filterPeriod = filterHelper.GetMultiYearFilter(yearList);
            var tblResult = CreateReportCaptionTable(3 * yearList.Count, 12);

            if (yearParam.Length == 0)
            {
                yearList.Clear();
            }
            else
            {
                yearList.Sort();                
            }

            var yearCounter = 0;

            var qMonthParams = new QMonthRepParams
            {
                Period = filterPeriod,
                KD = filterKD,
                Lvl = filterLvl,
                DocType = ReportMonthConsts.DocTypeConsolidate
            };

            var groupMonthInfo = new List<QMonthRepGroup> { QMonthRepGroup.Lvl, QMonthRepGroup.Period };
            var queryMonText = QMonthRepIncomes.GroupBy(qMonthParams, groupMonthInfo);
            var tblDataMon = dbHelper.GetTableData(queryMonText);

            var qFOYRParams = new QFOYRParams
            {
                Period = filterPeriod,
                KD = filterKD,
                Lvl = filterLvl,
                DocType = ReportMonthConsts.DocTypeConsolidate
            };

            var groupFOYRInfo = new List<QFOYRGroup> { QFOYRGroup.Lvl, QFOYRGroup.Period };
            var queryYrText = QFOYRIncomes.GroupBy(qFOYRParams, groupFOYRInfo);
            var tblDataYr = dbHelper.GetTableData(queryYrText);

            foreach (var year in yearList)
            {
                rowCaption[10 + yearCounter] = year;
                
                var filterYearPeriod = filterHelper.PeriodFilter(
                    f_D_FOYRIncomes.RefYearDayUNV, 
                    GetUNVYearLoBound(year),
                    GetUNVYearEnd(year));

                var tblSingleYear = DataTableUtils.FilterDataSet(tblDataYr, filterYearPeriod);
                var hasYearData = tblSingleYear.Rows.Count > 0;

                for (var i = 1; i < 13; i++)
                {
                    var rowIndex = i - 1;
                    var periodStart = GetUNVMonthStart(year, i);
                    var filterMonth = filterHelper.EqualIntFilter(f_F_MonthRepIncomes.RefYearDayUNV, periodStart);
                    var tblSingleMonth = DataTableUtils.FilterDataSet(tblDataMon, filterMonth);

                    for (var j = 0; j < 3; j++)
                    {
                        var lvlCode = ReportMonthMethods.GetBdgtLvlSKIFCodes(j);
                        var filterSingleLvl =  filterHelper.RangeFilter(f_F_MonthRepIncomes.RefBdgtLevels, lvlCode);
                        var rowsMonth = tblSingleMonth.Select(filterSingleLvl);
                        var columnDataIndex = GetColumnIndex(tblDataMon, f_F_MonthRepIncomes.Fact);

                        if (i == 12 && hasYearData)
                        {
                            var filterYear = filterHelper.RangeFilter(f_F_MonthRepIncomes.RefBdgtLevels, lvlCode);
                            columnDataIndex = GetColumnIndex(tblDataYr, f_D_FOYRIncomes.Performed);
                            rowsMonth = tblSingleYear.Select(filterYear);
                        }

                        var sum = rowsMonth.Sum(dataRow => GetDecimal(dataRow[columnDataIndex]));
                        var colIndex = yearCounter * 3 + j;
                        tblResult.Rows[rowIndex][colIndex] = sum;
                    }
                }

                yearCounter++;
            }

            for (var i = 0; i < tblResult.Columns.Count; i++)
            {
                DivideColumn(tblResult, i, divider);
                RoundColumn(tblResult, i, precision);
            }

            tablesResult[0] = tblResult;
            rowCaption[0] = yearList.Count;
            rowCaption[1] = paramLvl.Contains("0");
            rowCaption[2] = paramLvl.Contains("1");
            rowCaption[3] = paramLvl.Contains("2");
            rowCaption[4] = reportHelper.GetKDBridgeCaptionText(filterKD);
            rowCaption[5] = ReportMonthMethods.GetSelectedBudgetLvl(paramLvl);
            rowCaption[6] = ReportMonthMethods.GetDividerDescr(divider);
            rowCaption[7] = ReportMonthMethods.GetPrecisionIndex(precision);
            return tablesResult;
        }
    }
}
