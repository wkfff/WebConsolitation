using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsData.UFK;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Month;
using Krista.FM.Client.Reports.Month.Queries;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 007_ИСПОЛНЕНИЕ ОБЛАСТНОГО БЮДЖЕТА В РАЗРЕЗЕ МУНИЦИПАЛЬНЫХ ОБРАЗОВАНИЙ
        /// </summary>
        public DataTable[] GetMonth007ExecutingSubjectBudgetData(Dictionary<string, string> reportParams)
        {
            const int GroupColumnSize = 2;
            var reportHelper = new ReportMonthMethods(scheme);
            var filterHelper = new QFilterHelper();
            var tablesResult = new DataTable[5];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramLvl = reportParams[ReportConsts.ParamRegionListType];
            var fltKD = reportParams[ReportConsts.ParamKDComparable];
            var fltKVSR = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKVSRComparable]);
            var year = ReportMonthMethods.GetSelectedYear(reportParams[ReportConsts.ParamYear]);
            var yearLoBound = GetUNVYearStart(year);
            var yearHiBound = GetUNVYearEnd(year);
            var fltPeriod = filterHelper.GetYearFilter(yearLoBound, yearHiBound);
            var fltUfkPeriod = filterHelper.GetYearFilter(yearLoBound, GetUNVAbsYearEnd(year));
            var dbHelper = new ReportDBHelper(scheme);
            var tblYearResult = reportHelper.CreateRegionList(3);
            var yearList = new List<int> { year - 2, year - 1, year };

            var qUfkYearParams = new QUFK22Params
            {
                KD = fltKD,
                Mark = ReportMonthConsts.MarkUfkIncome,
                KVSR = fltKVSR
            };

            var groupYearFields = new List<QUFK22Group> { QUFK22Group.Region };

            for (var i = 0; i < yearList.Count; i++)
            {
                qUfkYearParams.Period = filterHelper.GetAbsYearFilter(yearList[i], false);
                var queryYearText = QUFK22.GroupBy(qUfkYearParams, groupYearFields);
                var tblYearData = dbHelper.GetTableData(queryYearText);
                var keyIndex = GetColumnIndex(tblYearData, d_OKATO_UFK.RefRegionsBridge);

                var splitUfkSettings = new RegionSplitParams
                {
                    RowsData = tblYearData.Select(),
                    KeyValIndex = keyIndex,
                    DocValIndex = keyIndex,
                    LvlValIndex = keyIndex,
                    TblResult = tblYearResult,
                    SrcColumnIndex = GetColumnIndex(tblYearData, f_D_UFK22.ForPeriod),
                    DstColumnIndex = i
                };

                reportHelper.SplitRegionData(splitUfkSettings);
                var asbColumnIndex = ReportMonthMethods.AbsColumnIndex(i);
                DivideColumn(tblYearResult, asbColumnIndex, divider);
                RoundColumn(tblYearResult, asbColumnIndex, precision);
            }

            var qParams = new QPlanIncomeParams
            {
                KD = fltKD,
                Period = fltPeriod,
                Lvl = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject),
                KVSR = fltKVSR
            };

            var groupInfo = new List<QPlanIncomeGroup>
                                {
                                    QPlanIncomeGroup.Period,
                                    QPlanIncomeGroup.Region
                                };

            var queryIncomeData = QPlanIncomeDivide.GroupBy(qParams, groupInfo);            
            var tblIncomeDivData = dbHelper.GetTableData(queryIncomeData);

            var qUfkParams = new QUFK22Params
            {
                KD = fltKD,
                Period = fltUfkPeriod,
                Mark = ReportMonthConsts.MarkUfkIncome,
                KVSR = fltKVSR
            };

            var groupFields = new List<QUFK22Group> { QUFK22Group.Region, QUFK22Group.Period };
            var queryUFK22Text = QUFK22.GroupBy(qUfkParams, groupFields);
            var tblUKF22Data = dbHelper.GetTableData(queryUFK22Text);

            var tblResult = reportHelper.CreateRegionList(12 * GroupColumnSize);

            var planColumnIndex = GetColumnIndex(tblIncomeDivData, d_Regions_Plan.RefBridge);
            var splitPlanParams = new RegionSplitParams
            {
                KeyValIndex = planColumnIndex,
                DocValIndex = planColumnIndex,
                LvlValIndex = planColumnIndex,
                TblResult = tblResult,
                SrcColumnIndex = GetColumnIndex(tblIncomeDivData, f_D_FOPlanIncDivide.YearPlan),
            };

            var factColumnIndex = GetColumnIndex(tblUKF22Data, d_OKATO_UFK.RefRegionsBridge);
            var splitFactParams = new RegionSplitParams
            {
                KeyValIndex = factColumnIndex,
                DocValIndex = factColumnIndex,
                LvlValIndex = factColumnIndex,
                TblResult = tblResult,
                SrcColumnIndex = GetColumnIndex(tblUKF22Data, f_D_UFK22.ForPeriod)
            };

            var lastPlanMonth = 1;

            for (var i = 1; i < 13; i++)
            {
                var periodStart = GetUNVMonthStart(year, i);
                var fltMonPlan = filterHelper.EqualIntFilter(f_D_FOPlanIncDivide.RefYearDayUNV, periodStart);
                var rowsIncomeDiv = tblIncomeDivData.Select(fltMonPlan);
                var planColumn = (i - 1) * GroupColumnSize;
                var factColumn = planColumn + 1;
                splitPlanParams.RowsData = rowsIncomeDiv;
                splitPlanParams.DstColumnIndex = planColumn;

                if (i == 1 || rowsIncomeDiv.Length > 0)
                {
                    reportHelper.SplitRegionData(splitPlanParams);
                    lastPlanMonth = i;
                }

                var periodUfkEnd = i == 12 ? GetUNVAbsMonthEnd(year, i) : GetUNVMonthEnd(year, i);
                var fltMonFact = filterHelper.PeriodFilter(f_D_UFK22.RefYearDayUNV, periodStart, periodUfkEnd);
                splitFactParams.RowsData = tblUKF22Data.Select(fltMonFact);
                splitFactParams.DstColumnIndex = factColumn;
                reportHelper.SplitRegionData(splitFactParams);

                if (i > 1)
                {
                    var absFactColumnIndex = ReportMonthMethods.AbsColumnIndex(factColumn);
                    var absPrevFactColumn = ReportMonthMethods.AbsColumnIndex(factColumn - GroupColumnSize);
                    SumColumns(tblResult, absFactColumnIndex, absPrevFactColumn, absFactColumnIndex);
                }
            }

            for (var i = 1; i < 13; i++)
            {
                var planColumn = (i - 1) * GroupColumnSize;
                var factColumn = planColumn + 1;
                var absPlanColumnIndex = ReportMonthMethods.AbsColumnIndex(planColumn);
                DivideColumn(tblResult, absPlanColumnIndex, divider);
                RoundColumn(tblResult, absPlanColumnIndex, precision);
                var absFactColumnIndex = ReportMonthMethods.AbsColumnIndex(factColumn);
                DivideColumn(tblResult, absFactColumnIndex, divider);
                RoundColumn(tblResult, absFactColumnIndex, precision);
            }

            tablesResult[1] = ReportMonthMethods.CreateSubjectTable(tblResult);
            tablesResult[3] = ReportMonthMethods.CreateSubjectTable(tblYearResult);
            reportHelper.ClearSettleRows(tblResult, paramLvl);
            tablesResult[0] = tblResult;
            tablesResult[2] = tblYearResult;
            rowCaption[0] = year;
            rowCaption[1] = reportHelper.GetKDBridgeCaptionText(fltKD);
            rowCaption[2] = ReportMonthMethods.WriteSettles(paramLvl);
            rowCaption[4] = ReportMonthMethods.GetDividerDescr(divider);
            rowCaption[5] = lastPlanMonth;
            rowCaption[7] = ReportMonthMethods.GetPrecisionIndex(precision);
            rowCaption[8] = ReportMonthMethods.GetSelectedKVSR(fltKVSR);
            return tablesResult;
        }
    }
}
