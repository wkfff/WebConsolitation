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
        /// ОТЧЕТ 016_ДИНАМИКА ЕЖЕМЕСЯЧНЫХ ПОСТУПЛЕНИЙ В БЮДЖЕТЫ
        /// </summary>
        public DataTable[] GetMonthReport016AnalisysDynamicIncomeData(Dictionary<string, string> reportParams)
        {
            var dbHelper = new ReportDBHelper(scheme);
            var filterHelper = new QFilterHelper();
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramLvl = reportParams[ReportConsts.ParamBdgtLevels];
            var filterLvl = String.Join(",", new[]
                                                 {
                                                     ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.MRTotal),
                                                     ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.Subject),
                                                     ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.ConsMunicipal),
                                                 });
            var filterKD = reportParams[ReportConsts.ParamKDComparable];
            var yearCodes = reportParams[ReportConsts.ParamYear].Split(',');
            var lstYears = yearCodes.Select(ReportMonthMethods.GetSelectedYear).ToList();
            var filterPeriod = filterHelper.GetMultiYearFilter(lstYears);
            var tblResult = CreateReportCaptionTable(3 * lstYears.Count, 12);
            var yearCounter = 0;
            lstYears.Sort();

            var qMonthParams = new QMonthRepParams
                                   {
                                       Period = filterPeriod,
                                       KD = filterKD,
                                       Lvl = filterLvl,
                                       DocType = ReportMonthConsts.DocTypeConsolidate
                                   };

            var groupMonthInfo = new List<QMonthRepGroup> { QMonthRepGroup.Period, QMonthRepGroup.Lvl };
            var queryIncomeData = QMonthRepIncomes.GroupBy(qMonthParams, groupMonthInfo);
            var tblData = dbHelper.GetTableData(queryIncomeData);
            var columnDataIndex = GetColumnIndex(tblData, f_F_MonthRepIncomes.Fact);

            var qFOYRParams = new QFOYRParams
            {
                KD = filterKD,
                Lvl = filterLvl,
                DocType = ReportMonthConsts.DocTypeConsolidate
            };

            var groupFOYRInfo = new List<QFOYRGroup> { QFOYRGroup.Period, QFOYRGroup.Lvl };

            foreach (var year in lstYears)
            {
                rowCaption[10 + yearCounter] = year;
                var fltYear = filterHelper.GetYearFilter(year);
                qFOYRParams.Period = fltYear;
                var queryYrText = QFOYRIncomes.GroupBy(qFOYRParams, groupFOYRInfo);
                var tblDataYr = dbHelper.GetTableData(queryYrText);
                var columnYearDataIndex = GetColumnIndex(tblDataYr, f_D_FOYRIncomes.Performed);

                for (var i = 1; i < 13; i++)
                {
                    var rowIndex = i - 1;
                    var periodStart = GetUNVMonthStart(year, i);
                    var fltPeriod = filterHelper.EqualIntFilter(f_F_MonthRepIncomes.RefYearDayUNV, periodStart);
                    var tblMonth = DataTableUtils.FilterDataSet(tblData, fltPeriod);

                    for (var j = 0; j < 3; j++)
                    {
                        var lvlCode = ReportMonthMethods.GetBdgtLvlSKIFCodes(j);
                        var filterMonth = filterHelper.RangeFilter(f_F_MonthRepIncomes.RefBdgtLevels, lvlCode);
                        var sum = tblMonth.Select(filterMonth)
                            .Sum(dataRow => GetDecimal(dataRow[columnDataIndex]));
                        var colIndex = yearCounter * 3 + j;
                        tblResult.Rows[rowIndex][colIndex] = sum;
                    }
                }

                if (tblDataYr.Rows.Count > 0)
                {
                    for (var j = 0; j < 3; j++)
                    {
                        var lvlCode = ReportMonthMethods.GetBdgtLvlSKIFCodes(j);
                        var filter = filterHelper.RangeFilter(f_D_FOYRIncomes.RefBdgtLevels, lvlCode);
                        var sum = tblDataYr.Select(filter).Sum(dataRow => GetDecimal(dataRow[columnYearDataIndex]));
                        tblResult.Rows[11][yearCounter * 3 + j] = sum;
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
