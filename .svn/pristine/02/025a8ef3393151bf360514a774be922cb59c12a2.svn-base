using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Month;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.Database.FactTables;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 005_ДИНАМИКА ЕЖЕМЕСЯЧНЫХ  ПОСТУПЛЕНИЙ В КБС ПО НЕСКОЛЬКИМ КБК
        /// </summary>
        public DataTable[] GetMonthReport005DynamicIncomeKDData(Dictionary<string, string> reportParams)
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
            var yearParam = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var yearCodes = yearParam.Split(',');
            var filterLvl = ReportMonthMethods.GetBdgtLvlSKIFCodes(Convert.ToInt32(paramLvl));
            var lstYears = yearCodes.Select(ReportMonthMethods.GetSelectedYear).ToList();
            var fltYearPeriod = filterHelper.GetMultiYearFilter(lstYears);
            var clearKD = reportHelper.ClearKDValues(filterKD, true);
            var kdCodes = clearKD.Split(',');
            var tblResult = CreateReportCaptionTable(lstYears.Count * kdCodes.Length, 12);
            var yearCounter = 0;
            var dataList = new Dictionary<string, DataTable>();
            var dataYearList = new Dictionary<string, DataTable>();

            if (yearParam.Length == 0)
            {
                lstYears.Clear();
            }
            else
            {
                lstYears.Sort();
            }

            for (var j = 0; j < kdCodes.Length; j++)
            {
                var kdValue = kdCodes[j];
                var kdFilter = reportHelper.GetKDHierarchyFilter(kdValue);

                var qMonthParams = new QMonthRepParams
                {
                    Period = fltYearPeriod,
                    KD = kdFilter,
                    Lvl = filterLvl,
                    DocType = ReportMonthConsts.DocTypeConsolidate
                };

                var groupMonthInfo = new List<QMonthRepGroup> { QMonthRepGroup.Period };
                var queryMonText = QMonthRepIncomes.GroupBy(qMonthParams, groupMonthInfo);
                var tblMonData = dbHelper.GetTableData(queryMonText);

                var qFOYRParams = new QFOYRParams
                {
                    Period = fltYearPeriod,
                    KD = kdFilter,
                    Lvl = filterLvl,
                    DocType = ReportMonthConsts.DocTypeConsolidate
                };

                var groupFOYRInfo = new List<QFOYRGroup> { QFOYRGroup.Period, QFOYRGroup.Kd };
                var queryYrText = QFOYRIncomes.GroupBy(qFOYRParams, groupFOYRInfo);
                var tblDataYr = dbHelper.GetTableData(queryYrText);

                dataYearList.Add(kdValue, tblDataYr);
                dataList.Add(kdValue, tblMonData);
                rowCaption[30 + j] = reportHelper.GetKDBridgeCaptionText(kdValue);
            }

            foreach (var year in lstYears)
            {
                rowCaption[ReportMonthMethods.RegionHeaderColumnCnt + yearCounter] = year;
                var yearStart = GetUNVYearPlanLoBound(year);

                for (var i = 1; i < 13; i++)
                {
                    var rowIndex = i - 1;
                    var monthStart = GetUNVMonthStart(year, i);

                    for (var j = 0; j < kdCodes.Length; j++)
                    {
                        var tblData = dataList[kdCodes[j]];
                        var fltMonthKD = filterHelper.EqualIntFilter(f_F_MonthRepIncomes.RefYearDayUNV, monthStart);
                        var rowsMonth = tblData.Select(fltMonthKD);
                        var sum = rowsMonth.Sum(dataRow => GetDecimal(dataRow[f_F_MonthRepIncomes.Fact]));
                        var colIndex = yearCounter * kdCodes.Length + j;
                        tblResult.Rows[rowIndex][colIndex] = sum;

                        if (i != 12)
                        {
                            continue;
                        }

                        tblData = dataYearList[kdCodes[j]];
                        var fltYear = filterHelper.EqualIntFilter(f_D_FOYRIncomes.RefYearDayUNV, yearStart);
                        var rowsYear = tblData.Select(fltYear);

                        if (rowsYear.Length == 0)
                        {
                            continue;
                        }

                        sum = rowsYear.Sum(dataRow => GetDecimal(dataRow[f_D_FOYRIncomes.Performed]));
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
            rowCaption[0] = lstYears.Count;
            rowCaption[4] = reportHelper.GetKDBridgeCaptionText(filterKD);
            rowCaption[5] = ReportMonthMethods.GetSelectedBudgetLvl(paramLvl);
            rowCaption[6] = ReportMonthMethods.GetDividerDescr(divider);
            rowCaption[7] = kdCodes.Length;
            rowCaption[8] = ReportMonthMethods.GetPrecisionIndex(precision);
            return tablesResult;
        }
    }
}
