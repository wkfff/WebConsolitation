using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsData.UFK;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK.ReportQueries;
using Krista.FM.Client.Reports.UFK14.Helpers;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 001_ПОСТУПЛЕНИЕ ДОХОДОВ В КОНСОЛИДИРОВАННЫЙ БЮДЖЕТ МОСКОВСКОЙ ОБЛАСТИ 
        /// </summary>
        public DataTable[] GetUFKReport001IncomeConsBudgetData(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            const int ResultColumnCount = 5;
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[3];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramLvl = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamBdgtLevels]);
            var paramKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var paramKVSR = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKVSRComparable]);
            var paramRgnListType = reportParams[ReportConsts.ParamRegionListType];
            var tblResult = reportHelper.CreateRegionList(ResultColumnCount);
            var loUnvDate = GetUNVDate(reportParams[ReportConsts.ParamStartDate]);
            var hiUnvDate = GetUNVDate(reportParams[ReportConsts.ParamEndDate]);
            var fltPeriod = filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV, loUnvDate, hiUnvDate);
            var paramHideEmptyStr = Convert.ToBoolean(reportParams[ReportConsts.ParamHideEmptyStr]);
            // уровни
            var levels = new List<string>
            {
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsSubject),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsMunicipal),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Municipal),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Settle)
            };

            var qUfkParams = new QUFK14Params
            {
                KD = paramKD,
                KVSR = paramKVSR,
                Period = fltPeriod,
                Lvl = String.Join(", ", levels.ToArray()),
            };

            var groupFields = new List<QUFK14Group>() { QUFK14Group.Region, QUFK14Group.Period, QUFK14Group.Lvl };
            var queryText = QUFK14.GroupBy(qUfkParams, groupFields);
            var tblData = dbHelper.GetTableData(queryText);

            // заполняем таблицу суммами
            var columnClsIndex = GetColumnIndex(tblData, d_OKATO_UFK.RefRegionsBridge);
            var splitParams = new RegionSplitParams
            {
                KeyValIndex = columnClsIndex,
                DocValIndex = columnClsIndex,
                LvlValIndex = columnClsIndex,
                TblResult = tblResult,
                SrcColumnIndex = GetColumnIndex(tblData, f_D_UFK14.Credit)
            };

            for (var i = 0; i < ResultColumnCount; i++) // по всем урованям бюджета
            {
                var filter = filterHelper.RangeFilter(f_D_UFK14.RefFX, levels[i]);
                splitParams.RowsData = tblData.Select(filter);
                splitParams.DstColumnIndex = i;
                reportHelper.SplitRegionData(splitParams);
            }

            // удаляем пустые строки
            if (paramHideEmptyStr)
            {
                var bdgtLevel = paramLvl != String.Empty ? Convert.ToInt32(paramLvl) : 0;
                var columnsDic = new Dictionary<int, int[]>
                {
                    {0, new [] {0, 1, 2, 3, 4}},
                    {1, new [] {1}},
                    {2, new [] {2, 3, 4}},
                    {3, new [] {3}},
                    {4, new [] {4}}
                };

                var columns = columnsDic[bdgtLevel].Select(col => ReportMonthMethods.RegionHeaderColumnCnt + col);
                tblResult = FilterNotNullData(tblResult, columns);
            }

            // делим суммы в зависимости от выбранных единиц измерения
            for (var i = 0; i < ResultColumnCount; i++)
            {
                DivideColumn(tblResult, ReportMonthMethods.AbsColumnIndex(i), divider);
            }

            var tblSubject = ReportMonthMethods.CreateSubjectTable(tblResult);

            reportHelper.ClearSettleRows(tblResult, paramRgnListType);

            tablesResult[0] = tblResult;
            tablesResult[1] = tblSubject;
            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.STARTDATE, reportParams[ReportConsts.ParamStartDate]);
            paramHelper.SetParamValue(ParamUFKHelper.ENDDATE, reportParams[ReportConsts.ParamEndDate]);
            paramHelper.SetParamValue(ParamUFKHelper.NOW, DateTime.Now.ToShortDateString());
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.REGION_LIST_TYPE, ReportMonthMethods.WriteSettles(paramRgnListType));
            paramHelper.SetParamValue(ParamUFKHelper.BDGT_LEVEL, paramLvl);
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetKDBridgeCaptionText(paramKD));
            paramHelper.SetParamValue(ParamUFKHelper.KVSR, reportHelper.GetKVSRBridgeCaptionText(paramKVSR));
            return tablesResult;
        }
    }
}
