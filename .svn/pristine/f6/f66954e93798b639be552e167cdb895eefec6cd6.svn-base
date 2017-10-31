using System;
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
        /// ОТЧЕТ 009_ДАННЫЕ ПО ФАКТИЧЕСКОМУ ИСПОЛНЕНИЮ И ПЛАНОВЫМ НАЗНАЧЕНИЯМ
        /// </summary>
        public DataTable[] GetMonthReport009ExecutionYearConsBudgetData(Dictionary<string, string> reportParams)
        {
            const int ResultColumnCount = 5;
            var dbHelper = new ReportDBHelper(scheme);
            var filterHelper = new QFilterHelper();
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[3];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramLvl = reportParams[ReportConsts.ParamRegionListType];
            var filterKD = reportParams[ReportConsts.ParamKDComparable];
            var filterMark = reportParams[ReportConsts.ParamMark];
            var year = ReportMonthMethods.GetSelectedYear(reportParams[ReportConsts.ParamYear]);
            var fltPeriod = filterHelper.GetYearFilter(year);

            // Местный бюджет - плавн омсу и факт
            var qFOYRParams = new QFOYRParams
            {
                Period = fltPeriod,
                KD = filterKD,
                Lvl = ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.Summary),
                DocType = ReportMonthConsts.DocTypeFull
            };

            var groupFOYRInfo = new List<QFOYRGroup> { QFOYRGroup.Lvl, QFOYRGroup.Period, QFOYRGroup.DocType, QFOYRGroup.Region };
            var queryYrText = QFOYRIncomes.GroupBy(qFOYRParams, groupFOYRInfo);
            var tblDataYr = dbHelper.GetTableData(queryYrText);

            var tblResult = reportHelper.CreateRegionList(ResultColumnCount);
            var splitParams = new RegionSplitParams
            {
                UseLvlDepencity = true,
                UseDocumentTypes = true,
                TblResult = tblResult,
                DstColumnIndex = 3
            };

            if (tblDataYr.Rows.Count > 0)
            {
                splitParams.KeyValIndex = GetColumnIndex(tblDataYr, d_Regions_FOYR.RefRegionsBridge);
                splitParams.DocValIndex = GetColumnIndex(tblDataYr, d_Regions_FOYR.RefDocType);
                splitParams.LvlValIndex = GetColumnIndex(tblDataYr, f_D_FOYRIncomes.RefBdgtLevels);
                splitParams.RowsData = tblDataYr.Select();
                splitParams.IsFractional = true;
                //МБ - План, утвержденный ОМСУ
                splitParams.SrcColumnIndex = GetColumnIndex(tblDataYr, f_D_FOYRIncomes.Assigned);
                splitParams.DstColumnIndex = 3;
                reportHelper.SplitRegionData(splitParams);
                //МБ - Факт
                splitParams.SrcColumnIndex = GetColumnIndex(tblDataYr, f_D_FOYRIncomes.Performed);
                splitParams.DstColumnIndex = 4;
                reportHelper.SplitRegionData(splitParams);
            }
            else
            {
                var qMonthParams = new QMonthRepParams
                                       {
                                           Period = fltPeriod,
                                           KD = filterKD,
                                           Lvl = ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.Summary)
                                       };

                var groupMonthInfo = new List<QMonthRepGroup>
                                         {
                                             QMonthRepGroup.Period,
                                             QMonthRepGroup.Lvl,
                                             QMonthRepGroup.Region,
                                             QMonthRepGroup.DocType
                                         };

                var queryIncomeData = QMonthRepIncomes.GroupBy(qMonthParams, groupMonthInfo);
                var tblYearData = dbHelper.GetTableData(queryIncomeData);
                tblYearData = ReportMonthMethods.FilterLastMonth(tblYearData, f_F_MonthRepIncomes.RefYearDayUNV);

                splitParams.KeyValIndex = GetColumnIndex(tblYearData, d_Regions_MonthRep.RefRegionsBridge);
                splitParams.DocValIndex = GetColumnIndex(tblYearData, d_Regions_MonthRep.RefDocType);
                splitParams.LvlValIndex = GetColumnIndex(tblYearData, f_F_MonthRepIncomes.RefBdgtLevels);
                splitParams.RowsData = tblYearData.Select();
                splitParams.IsFractional = true;
                //МБ - План, утвержденный ОМСУ
                splitParams.SrcColumnIndex = GetColumnIndex(tblYearData, f_F_MonthRepIncomes.YearPlan);
                splitParams.DstColumnIndex = 3;
                reportHelper.SplitRegionData(splitParams);
                //МБ - Факт
                splitParams.SrcColumnIndex = GetColumnIndex(tblYearData, f_F_MonthRepIncomes.Fact);
                splitParams.DstColumnIndex = 4;
                reportHelper.SplitRegionData(splitParams);
            }

            // План, утвержденный органами госвласти
            var codesSB = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject);

            var qIncDivParams = new QPlanIncomeParams
            {
                KD = filterKD,
                Period = fltPeriod,
                Lvl = codesSB
            };

            var groupInfo = new List<QPlanIncomeGroup>
                                {
                                    QPlanIncomeGroup.Period,
                                    QPlanIncomeGroup.Region
                                };

            var queryPlanIncSBText = QPlanIncomeDivide.GroupBy(qIncDivParams, groupInfo);
            var tblPlanSBData = dbHelper.GetTableData(queryPlanIncSBText);

            tblPlanSBData = ReportMonthMethods.FilterLastMonth(tblPlanSBData, f_D_FOPlanIncDivide.RefYearDayUNV);
            splitParams = new RegionSplitParams
            {
                KeyValIndex = GetColumnIndex(tblPlanSBData, d_Regions_Plan.RefBridge),
                RowsData = tblPlanSBData.Select(),
                TblResult = tblResult,
                SrcColumnIndex = GetColumnIndex(tblPlanSBData, f_D_FOPlanIncDivide.YearPlan),
                DstColumnIndex = 0,
                UseLvlDepencity = false,
                IsSkifLevels = false
            };
            splitParams.LvlValIndex = splitParams.KeyValIndex;
            splitParams.DocValIndex = splitParams.KeyValIndex;
            splitParams.IsFractional = false;
            reportHelper.SplitRegionData(splitParams);
            var codesMB = String.Join(",", new[]
            {
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.MRTotal),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Town)
            });

            qIncDivParams.Lvl = codesMB;
            var queryPlanIncMBText = QPlanIncomeDivide.GroupBy(qIncDivParams, groupInfo);
            var tblPlanMBData = dbHelper.GetTableData(queryPlanIncMBText);
            tblPlanMBData = ReportMonthMethods.FilterLastMonth(tblPlanMBData, f_D_FOPlanIncDivide.RefYearDayUNV);
            splitParams.DstColumnIndex = 2;
            splitParams.RowsData = tblPlanMBData.Select();
            reportHelper.SplitRegionData(splitParams);
            // ОБ факт
            var fltUfkPeriod = filterHelper.GetAbsYearFilter(year);
            var qUfkParams = new QUFK22Params
            {
                KD = filterKD,
                Period = fltUfkPeriod,
                Mark = filterMark,
            };

            var groupFields = new List<QUFK22Group> { QUFK22Group.Region, QUFK22Group.Period };
            var queryUFK22Text = QUFK22.GroupBy(qUfkParams, groupFields);
            var tblUKF22Data = dbHelper.GetTableData(queryUFK22Text);

            splitParams = new RegionSplitParams
            {
                KeyValIndex = GetColumnIndex(tblUKF22Data, d_OKATO_UFK.RefRegionsBridge),
                RowsData = tblUKF22Data.Select(),
                TblResult = tblResult,
                SrcColumnIndex = GetColumnIndex(tblUKF22Data, f_D_UFK22.ForPeriod),
                DstColumnIndex = 1
            };

            splitParams.LvlValIndex = splitParams.KeyValIndex;
            splitParams.DocValIndex = splitParams.KeyValIndex;
            reportHelper.SplitRegionData(splitParams);

            for (var i = 0; i < ResultColumnCount; i++)
            {
                var absColumnIndex = ReportMonthMethods.AbsColumnIndex(i);
                DivideColumn(tblResult, absColumnIndex, divider);
                RoundColumn(tblResult, absColumnIndex, precision);
            }

            reportHelper.ClearSettleRows(tblResult, paramLvl);

            tablesResult[1] = ReportMonthMethods.CreateSubjectTable(tblResult);

            tablesResult[0] = tblResult;
            var rowCaption = CreateReportParamsRow(tablesResult);
            rowCaption[0] = year;
            rowCaption[1] = reportHelper.GetKDBridgeCaptionText(filterKD);
            rowCaption[2] = ReportMonthMethods.WriteSettles(paramLvl);
            rowCaption[4] = ReportMonthMethods.GetDividerDescr(divider);
            rowCaption[7] = ReportMonthMethods.GetPrecisionIndex(precision);
            return tablesResult;
        }
    }
}
