using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.Month;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Database.ClsData.UFK;
using Krista.FM.Client.Reports.Database.ClsData;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 004_ИСПОЛНЕНИЕ МЕСТНОГО БЮДЖЕТА В РАЗРЕЗЕ МУНИЦИПАЛЬНЫХ ОБРАЗОВАНИЙ
        /// </summary>
        public DataTable[] GetMonthReport004ExecutingMunicipalBudgetData(Dictionary<string, string> reportParams)
        {
            var reportHelper = new ReportMonthMethods(scheme);
            var filterHelper = new QFilterHelper();
            var tablesResult = new DataTable[5];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramLvl = reportParams[ReportConsts.ParamRegionListType];
            var filterKD = reportParams[ReportConsts.ParamKDComparable];
            var year = ReportMonthMethods.GetSelectedYear(reportParams[ReportConsts.ParamYear]);
            var yearLoBound = GetUNVYearStart(year);
            var yearHiBound = GetUNVYearEnd(year);
            var yearPeriod = filterHelper.GetYearFilter(yearLoBound, yearHiBound);
            var dbHelper = new ReportDBHelper(scheme);
            // МесОтч
            var qMonthParams = new QMonthRepParams
            {
                Period = yearPeriod,
                KD = filterKD,
                Lvl = ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.Summary),
                DocType = ReportMonthConsts.DocTypeFull
            };
            var groupMonthInfo = new List<QMonthRepGroup>
                                     {
                                         QMonthRepGroup.Period, 
                                         QMonthRepGroup.Lvl,
                                         QMonthRepGroup.DocType,
                                         QMonthRepGroup.Region
                                     };
            var queryMonText = QMonthRepIncomes.GroupBy(qMonthParams, groupMonthInfo);
            var tblMonthRepData = dbHelper.GetTableData(queryMonText);
            // РезультатДоходовСРАсщеплением
            var qParams = new QPlanIncomeParams
            {
                Period = yearPeriod,
                KD = filterKD,
                Lvl = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Summary)
            };

            var groupInfo = new List<QPlanIncomeGroup>
                                {
                                    QPlanIncomeGroup.Period, 
                                    QPlanIncomeGroup.Region, 
                                    QPlanIncomeGroup.Lvl
                                };
            var queryIncomeData = QPlanIncomeDivide.GroupBy(qParams, groupInfo);
            var tblIncomeDivData = dbHelper.GetTableData(queryIncomeData);
            // ГодОтч
            var yearList = new List<int> { year - 2, year - 1, year };
            var yearPeriodFilter = filterHelper.GetMultiYearFilter(yearList);
            var qFOYRParams = new QFOYRParams
            {
                Period = yearPeriodFilter,
                KD = filterKD,
                Lvl = ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.Summary),
                DocType = ReportMonthConsts.DocTypeFull
            };
            var groupFOYRInfo = new List<QFOYRGroup>
                                    {
                                        QFOYRGroup.Period, 
                                        QFOYRGroup.DocType, 
                                        QFOYRGroup.Lvl, 
                                        QFOYRGroup.Region
                                    };
            var queryYrText = QFOYRIncomes.GroupBy(qFOYRParams, groupFOYRInfo);
            var tblYearData = dbHelper.GetTableData(queryYrText);

            var tblResult = reportHelper.CreateRegionList(1 + 13 * 2);
            var tblYearResult = reportHelper.CreateRegionList(2);

            const int summColumn = 0;

            var splitParams = new RegionSplitParams
            {
                UseLvlDepencity = true,
                TblResult = tblResult
            };

            for (var i = 1; i < 13; i++)
            {
                var periodStart = GetUNVMonthStart(year, i);
                var filterMonth = filterHelper.EqualIntFilter(f_F_MonthRepIncomes.RefYearDayUNV, periodStart);
                var rowsMonthData = tblMonthRepData.Select(filterMonth);
                var rowsIncomeDiv = tblIncomeDivData.Select(filterMonth);
                var planColumn = i * 2 - 1;
                var factColumn = planColumn + 1;
                splitParams.DstColumnIndex = summColumn;
                splitParams.RowsData = rowsIncomeDiv;
                splitParams.SrcColumnIndex = GetColumnIndex(tblIncomeDivData, f_D_FOPlanIncDivide.YearPlan);
                splitParams.UseDocumentTypes = false;
                splitParams.IsSkifLevels = false;
                splitParams.IsFractional = false;
                splitParams.LvlValIndex = GetColumnIndex(tblIncomeDivData, f_D_FOPlanIncDivide.RefBudLevel);
                splitParams.KeyValIndex = GetColumnIndex(tblIncomeDivData, d_Regions_Plan.RefBridge);
                splitParams.DocValIndex = splitParams.LvlValIndex;

                if (i == 1 || rowsIncomeDiv.Length > 0)
                {
                    reportHelper.SplitRegionData(splitParams);
                }

                splitParams.UseDocumentTypes = true;
                splitParams.IsSkifLevels = true;
                splitParams.IsFractional = true;
                splitParams.KeyValIndex = GetColumnIndex(tblMonthRepData, d_Regions_MonthRep.RefRegionsBridge);
                splitParams.DocValIndex = GetColumnIndex(tblMonthRepData, d_Regions_MonthRep.RefDocType);
                splitParams.LvlValIndex = GetColumnIndex(tblMonthRepData, f_F_MonthRepIncomes.RefBdgtLevels);
                splitParams.SrcColumnIndex = GetColumnIndex(tblMonthRepData, f_F_MonthRepIncomes.YearPlan);
                splitParams.DstColumnIndex = planColumn;
                splitParams.RowsData = rowsMonthData;
                reportHelper.SplitRegionData(splitParams);
                splitParams.DstColumnIndex = factColumn;
                splitParams.SrcColumnIndex = GetColumnIndex(tblMonthRepData, f_F_MonthRepIncomes.Fact);
                reportHelper.SplitRegionData(splitParams);
                var planAbsIndex = ReportMonthMethods.AbsColumnIndex(planColumn);
                var factAbsIndex = ReportMonthMethods.AbsColumnIndex(factColumn);
                DivideColumn(tblResult, planAbsIndex, divider);
                RoundColumn(tblResult, planAbsIndex, precision);
                DivideColumn(tblResult, factAbsIndex, divider);
                RoundColumn(tblResult, factAbsIndex, precision);
            }

            splitParams.TblResult = tblYearResult;
            splitParams.KeyValIndex = GetColumnIndex(tblYearData, d_Regions_FOYR.RefRegionsBridge);
            splitParams.DocValIndex = GetColumnIndex(tblYearData, d_Regions_FOYR.RefDocType);
            splitParams.LvlValIndex = GetColumnIndex(tblYearData, f_D_FOYRIncomes.RefBdgtLevels);
            splitParams.SrcColumnIndex = GetColumnIndex(tblYearData, f_D_FOYRIncomes.Performed);

            for (var i = 0; i < yearList.Count - 1; i++)
            {
                var yearFilter = filterHelper.GetYearFilter(yearList[i], false);
                var rowsYearData = tblYearData.Select(yearFilter);
                splitParams.RowsData = rowsYearData;
                splitParams.DstColumnIndex = i;
                reportHelper.SplitRegionData(splitParams);
                var absColumnIndex = ReportMonthMethods.AbsColumnIndex(i);
                DivideColumn(tblYearResult, absColumnIndex, divider);
                RoundColumn(tblYearResult, absColumnIndex, precision);
            }

            var fltYear = filterHelper.GetYearFilter(year, false);
            var rowsYear = tblYearData.Select(fltYear);
            splitParams.RowsData = rowsYear;
            var resultColumn = 25;
            splitParams.SrcColumnIndex = GetColumnIndex(tblYearData, f_D_FOYRIncomes.Assigned);
            splitParams.DstColumnIndex = resultColumn;
            splitParams.TblResult = tblResult;
            reportHelper.SplitRegionData(splitParams);
            var asbResultIndex = ReportMonthMethods.AbsColumnIndex(resultColumn);
            DivideColumn(tblResult, asbResultIndex, divider);
            RoundColumn(tblResult, asbResultIndex, precision);
            splitParams.SrcColumnIndex = GetColumnIndex(tblYearData, f_D_FOYRIncomes.Performed);
            resultColumn++;
            splitParams.DstColumnIndex = resultColumn;
            reportHelper.SplitRegionData(splitParams);
            asbResultIndex = ReportMonthMethods.AbsColumnIndex(resultColumn);
            DivideColumn(tblResult, asbResultIndex, divider);
            RoundColumn(tblResult, asbResultIndex, precision);
            var asbSummIndex = ReportMonthMethods.AbsColumnIndex(summColumn);
            DivideColumn(tblResult, asbSummIndex, divider);
            RoundColumn(tblResult, asbSummIndex, precision);
            var tblSubjectMN = ReportMonthMethods.CreateSubjectTable(tblResult);
            var tblSubjectYR = ReportMonthMethods.CreateSubjectTable(tblYearResult);
            reportHelper.ClearSettleRows(tblResult, paramLvl);
            reportHelper.ClearSettleRows(tblYearResult, paramLvl);
            tablesResult[0] = tblResult;
            tablesResult[1] = tblYearResult;
            tablesResult[2] = tblSubjectMN;
            tablesResult[3] = tblSubjectYR;
            rowCaption[0] = year;
            rowCaption[1] = reportHelper.GetKDBridgeCaptionText(filterKD);
            rowCaption[2] = ReportMonthMethods.WriteSettles(paramLvl);
            rowCaption[4] = ReportMonthMethods.GetDividerDescr(divider);
            rowCaption[7] = ReportMonthMethods.GetPrecisionIndex(precision);
            return tablesResult;
        }
    }
}
