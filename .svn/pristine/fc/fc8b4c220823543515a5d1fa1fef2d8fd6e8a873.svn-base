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
        /// ОТЧЕТ 003_ИСПОЛНЕНИЕ ПЛАНА КОНСОЛИДИРОВАННОГО БЮДЖЕТА ОБЛАСТИ
        /// </summary>
        public DataTable[] GetMonthReport003ExecutingBudgetPlanData(Dictionary<string, string> reportParams)
        {
            var dbHelper = new ReportDBHelper(scheme);
            var filterHelper = new QFilterHelper();
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramLvl = reportParams[ReportConsts.ParamBdgtLevels];
            var filterKD = reportParams[ReportConsts.ParamKDComparable];
            var filterSkifLvl = ReportMonthMethods.GetBdgtLvlSKIFCodes(paramLvl);
            var filterBdgtLvl = ReportMonthMethods.GetBdgtLvlCodes(paramLvl);
            var yearParam = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = ReportMonthMethods.GetSelectedYear(yearParam);
            var yearPeriod = filterHelper.GetYearFilter(year);

            var qMonthParams = new QMonthRepParams
            {
                Period = yearPeriod,
                KD = filterKD,
                Lvl = filterSkifLvl,
                DocType = ReportMonthConsts.DocTypeConsolidate
            };

            var groupMonthInfo = new List<QMonthRepGroup> { QMonthRepGroup.Period };
            var queryMonText = QMonthRepIncomes.GroupBy(qMonthParams, groupMonthInfo);
            var tblMonthRepData = dbHelper.GetTableData(queryMonText);

            var qFOYRParams = new QFOYRParams
            {
                Period = yearPeriod,
                KD = filterKD,
                Lvl = filterSkifLvl,
                DocType = ReportMonthConsts.DocTypeConsolidate
            };

            var groupFOYRInfo = new List<QFOYRGroup> { QFOYRGroup.Period };
            var queryYrText = QFOYRIncomes.GroupBy(qFOYRParams, groupFOYRInfo);
            var tblDataYr = dbHelper.GetTableData(queryYrText);

            var qParams = new QPlanIncomeParams
            {
                Period = yearPeriod,
                KD = filterKD,
                Lvl = filterBdgtLvl
            };

            var groupInfo = new List<QPlanIncomeGroup> { QPlanIncomeGroup.Period };
            var queryIncomeData = QPlanIncomeDivide.GroupBy(qParams, groupInfo);
            var tblIncomeDivData = dbHelper.GetTableData(queryIncomeData);
            
            var tblResult = CreateReportCaptionTable(3, 12);
            var filterYearPeriod = filterHelper.GetYearFilter(year, false);
            var rowsYear = tblDataYr.Select(filterYearPeriod);
            var hasYearData = rowsYear.Length > 0;

            for (var i = 1; i < 13; i++)
            {
                var rowIndex = i - 1;
                var periodStart = GetUNVMonthStart(year, i);
                var filterMonth = filterHelper.EqualIntFilter(f_F_MonthRepIncomes.RefYearDayUNV, periodStart);
                var rowsMonth = tblMonthRepData.Select(filterMonth);

                var factColumnIndex = GetColumnIndex(tblMonthRepData, f_F_MonthRepIncomes.Fact);
                var planColumnIndex = GetColumnIndex(tblMonthRepData, f_F_MonthRepIncomes.YearPlan);
                var planColumnIncome = GetColumnIndex(tblIncomeDivData, f_D_FOPlanIncDivide.YearPlan);

                if (i == 12 && hasYearData)
                {
                    factColumnIndex = GetColumnIndex(tblDataYr, f_D_FOYRIncomes.Performed);
                    planColumnIndex = GetColumnIndex(tblDataYr, f_D_FOYRIncomes.Assigned);
                    rowsMonth = rowsYear;
                }

                var rowsIncomeDiv = tblIncomeDivData.Select(filterMonth);
                var sumMonthPlan = rowsMonth.Sum(dataRow => GetDecimal(dataRow[planColumnIndex]));
                var sumMonthFact = rowsMonth.Sum(dataRow => GetDecimal(dataRow[factColumnIndex]));
                var sumIncomeDiv = rowsIncomeDiv.Sum(dataRow => GetDecimal(dataRow[planColumnIncome]));
                var rowResult = tblResult.Rows[rowIndex];
                rowResult[0] = sumIncomeDiv;
                rowResult[1] = sumMonthPlan;
                rowResult[2] = sumMonthFact;
            }

            for (var i = 0; i < 3; i++)
            {
                DivideColumn(tblResult, i, divider);
                RoundColumn(tblResult, i, precision);
            }

            tablesResult[0] = tblResult;
            rowCaption[0] = year;
            rowCaption[1] = reportHelper.GetKDBridgeCaptionText(filterKD);
            rowCaption[2] = ReportMonthMethods.GetSelectedBudgetLvl(paramLvl);
            rowCaption[3] = Convert.ToInt32(paramLvl);
            rowCaption[4] = ReportMonthMethods.GetDividerDescr(divider);
            rowCaption[7] = ReportMonthMethods.GetPrecisionIndex(precision);
            return tablesResult;
        }
    }
}
