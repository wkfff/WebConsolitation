using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Database.ClsData.UFK;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 010_ПОСТУПЛЕНИЯ В ОБЛАСТНОЙ БЮДЖЕТ ПО КБК И АДМИНИСТРАТОРАМ В РАЗРЕЗЕ ОМСУ
        /// </summary>
        public DataTable[] GetMonthReport010IncomeSubjectBudgetData(Dictionary<string, string> reportParams)
        {
            const int ColumnCount = 5;
            var filterHelper = new QFilterHelper();
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[3];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramLvl = reportParams[ReportConsts.ParamRegionListType];
            var fltMark = reportParams[ReportConsts.ParamMark];
            var loDate = reportParams[ReportConsts.ParamStartDate];
            var loDateUnv = GetUNVDate(loDate);
            var hiDate = reportParams[ReportConsts.ParamEndDate];
            var hiDateUnv = GetUNVDate(hiDate);
            var endDate = Convert.ToDateTime(hiDate);
            
            if (endDate.Month == 12 && endDate.Day == 31)
            {
                hiDateUnv = GetUNVAbsYearEnd(endDate.Year);
            }

            var fltKD = reportParams[ReportConsts.ParamKDComparable];
            var fltKVSR = reportParams[ReportConsts.ParamKVSRComparable];
            fltKVSR = ReportMonthMethods.CheckBookValue(fltKVSR);
            var fltPeriod = filterHelper.PeriodFilter(f_D_UFK22.RefYearDayUNV, loDateUnv, hiDateUnv, true);
            var dbHelper = new ReportDBHelper(scheme);

            // План, утвержденный органами госвласти
            var tblResult = reportHelper.CreateRegionList(ColumnCount);

            var qParams = new QUFK22Params
                              {
                                  KD = fltKD, 
                                  Period = fltPeriod, 
                                  Mark = fltMark,
                                  KVSR = fltKVSR
                              };

            var groupFields = new List<QUFK22Group> { QUFK22Group.Region, QUFK22Group.Mark };
            var queryUFK22Text = QUFK22.GroupBy(qParams, groupFields);
            var tblUKF22Data = dbHelper.GetTableData(queryUFK22Text);

            var splitParams = new RegionSplitParams
            {
                KeyValIndex = GetColumnIndex(tblUKF22Data, d_OKATO_UFK.RefRegionsBridge),
                TblResult = tblResult,
                SrcColumnIndex = GetColumnIndex(tblUKF22Data, f_D_UFK22.ForPeriod)
            };
            splitParams.LvlValIndex = splitParams.KeyValIndex;
            splitParams.DocValIndex = splitParams.KeyValIndex;

            for (var i = 0; i < ColumnCount; i++)
            {
                var fltColumn = filterHelper.EqualIntFilter(f_D_UFK22.RefMarks, i + 1);
                splitParams.RowsData = tblUKF22Data.Select(fltColumn);
                splitParams.DstColumnIndex = i;
                reportHelper.SplitRegionData(splitParams);
                var absColumnIndex = ReportMonthMethods.AbsColumnIndex(i);
                DivideColumn(tblResult, absColumnIndex, divider);
                RoundColumn(tblResult, absColumnIndex, precision);
            }

            tablesResult[1] = ReportMonthMethods.CreateSubjectTable(tblResult);
            reportHelper.ClearSettleRows(tblResult, paramLvl);
            tablesResult[0] = tblResult;
            var rowCaption = CreateReportParamsRow(tablesResult);
            rowCaption[0] = loDate;
            rowCaption[1] = hiDate;
            rowCaption[2] = ReportMonthMethods.WriteSettles(paramLvl);
            rowCaption[3] = reportHelper.GetKDBridgeCaptionText(fltKD);
            rowCaption[4] = ReportMonthMethods.GetDividerDescr(divider);
            rowCaption[5] = ReportMonthMethods.GetSelectedKVSR(fltKVSR);
            rowCaption[7] = ReportMonthMethods.GetPrecisionIndex(precision);
            rowCaption[10] = fltMark;

            for (var i = 0; i < ColumnCount; i++)
            {
                rowCaption[11 + i] = fltMark.Length == 0 || fltMark.Contains(Convert.ToString(i + 1));
            }

            return tablesResult;
        }
    }
}

