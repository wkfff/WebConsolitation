using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.Database.ClsData.UFK;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Month;
using Krista.FM.Client.Reports.Month.Queries;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 006_ДАННЫЕ ПО ФАКТИЧЕСКОМУ ИСПОЛНЕНИЮ И ПЛАНОВЫМ НАЗНАЧЕНИЯМ
        /// </summary>
        public DataTable[] GetMonthReport006BudgetAppointmentData(Dictionary<string, string> reportParams)
        {
            var dbHelper = new ReportDBHelper(scheme);
            const int ResultColumnCount = 2;
            var filterHelper = new QFilterHelper();
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[3];
            var columnList = reportParams[ReportConsts.ParamOutputMode];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var monthItem = reportParams[ReportConsts.ParamMonth];
            var monthEnum = new MonthEnum();
            var monthNum = GetEnumItemIndex(monthEnum, monthItem) + 1;
            var paramLvl = reportParams[ReportConsts.ParamRegionListType];
            var filterKD = reportParams[ReportConsts.ParamKDComparable];
            var year = ReportMonthMethods.GetSelectedYear(reportParams[ReportConsts.ParamYear]);
            var monthPeriod = GetUNVMonthStart(year, monthNum);
            var fltPeriod = filterHelper.EqualIntFilter(f_F_MonthRepIncomes.RefYearDayUNV, monthPeriod, true);

            var qMonthParams = new QMonthRepParams
            {
                Period = fltPeriod,
                KD = filterKD,
                Lvl = ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.Summary),
                DocType = ReportMonthConsts.DocTypeFull
            };

            var groupMonthInfo = new List<QMonthRepGroup> { QMonthRepGroup.Region, QMonthRepGroup.DocType, QMonthRepGroup.Lvl };
            var queryMonText = QMonthRepIncomes.GroupBy(qMonthParams, groupMonthInfo);
            var tblMonthRepData = dbHelper.GetTableData(queryMonText);      

            var tblResult = reportHelper.CreateRegionList(ResultColumnCount);

            var splitParams = new RegionSplitParams
            {
                KeyValIndex = GetColumnIndex(tblMonthRepData, d_Regions_MonthRep.RefRegionsBridge),
                DocValIndex = GetColumnIndex(tblMonthRepData, d_Regions_MonthRep.RefDocType),
                LvlValIndex = GetColumnIndex(tblMonthRepData, f_F_MonthRepIncomes.RefBdgtLevels),
                RowsData = tblMonthRepData.Select(),
                UseDocumentTypes = true,
                UseLvlDepencity = true,
                IsFractional = true,
                TblResult = tblResult,
                SrcColumnIndex = GetColumnIndex(tblMonthRepData, f_F_MonthRepIncomes.YearPlan),
                DstColumnIndex = 0
            };

            reportHelper.SplitRegionData(splitParams);

            splitParams.DstColumnIndex = 1;
            splitParams.SrcColumnIndex = GetColumnIndex(tblMonthRepData, f_F_MonthRepIncomes.Fact);

            reportHelper.SplitRegionData(splitParams);
            reportHelper.ClearSettleRows(tblResult, paramLvl);

            for (var i = 0; i < ResultColumnCount; i++)
            {
                var asbColumnIndex = ReportMonthMethods.AbsColumnIndex(i);
                DivideColumn(tblResult, asbColumnIndex, divider);
                RoundColumn(tblResult, asbColumnIndex, precision);
            }

            tablesResult[1] = ReportMonthMethods.CreateSubjectTable(tblResult);

            var nextMonth = new DateTime(year, monthNum, 1).AddMonths(1);

            tablesResult[0] = tblResult;
            var rowCaption = CreateReportParamsRow(tablesResult);
            rowCaption[0] = nextMonth.Year;
            rowCaption[1] = reportHelper.GetKDBridgeCaptionText(filterKD);
            rowCaption[2] = ReportMonthMethods.WriteSettles(paramLvl);
            rowCaption[4] = ReportMonthMethods.GetDividerDescr(divider);
            rowCaption[5] = GetMonthText2(nextMonth.Month);
            rowCaption[6] = columnList.Contains("0");
            rowCaption[7] = columnList.Contains("1");
            rowCaption[8] = ReportMonthMethods.GetPrecisionIndex(precision);
            return tablesResult;
        }
    }
}
