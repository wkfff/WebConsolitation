using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData.UFK;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK.ReportMaster;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;


namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 003_ДИНАМИКА ПОСТУПЛЕНИЯ ДОХОДОВ В КОНСОЛИДИРОВАННЫЙ БЮДЖЕТ МОСКОВСКОЙ ОБЛАСТИ 
        /// </summary>
        public DataTable[] GetUFKReport003DynamicIncome(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var startDate = Convert.ToDateTime(reportParams[ReportConsts.ParamStartDate]);
            var endDate = Convert.ToDateTime(reportParams[ReportConsts.ParamEndDate]);
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramRegion = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamRegionComparable]);
            var paramRgnListType = reportParams[ReportConsts.ParamRegionListType];
            var hideSettlement = !ReportMonthMethods.WriteSettles(paramRgnListType);
            var filterKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var paramKVSR = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKVSRComparable]);
            var paramLvl = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamBdgtLevels]) != String.Empty
                               ? reportParams[ReportConsts.ParamBdgtLevels]
                               : "0";
            var paramHideEmptyStr = Convert.ToBoolean(reportParams[ReportConsts.ParamHideEmptyStr]);

            var levels = new List<string> 
            {
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsSubjectFractional),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsMunicipalFractional),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Municipal),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Settle),
            };

            var filterLvl = paramLvl != String.Empty ? levels[Convert.ToInt32(paramLvl)] : levels[0];

            // период
            if (startDate.Year != endDate.Year)
            {
                startDate = new DateTime(endDate.Year, 1, 1);
            }
            var years = new List<int> { endDate.Year - 2, endDate.Year - 1, endDate.Year };
            var periods = (from year in years
                           let sDate = GetUNVDate(new DateTime(year, startDate.Month, startDate.Day))
                           let eDate = GetUNVDate(new DateTime(year, endDate.Month, endDate.Day))
                           select filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV, sDate, eDate, true)).ToList();

            // параметры отчета
            var rep = new Report(f_D_UFK14.InternalKey)
            {
                Divider = GetDividerValue(divider),
                RowFilter = paramHideEmptyStr ? Functions.IsNotNullRow : (RowViewParams.Function)null
            };

            // группировка по АТЕ
            var ateGrouping = new AteGrouping(0, hideSettlement);
            rep.AddGrouping(d_OKATO_UFK.RefRegionsBridge, ateGrouping);
            var fixedId = paramHideEmptyStr
                              ? ateGrouping.AteMainId
                              : paramRegion != String.Empty
                                    ? Combine(paramRegion, ateGrouping.AteMainId)
                                    : ateGrouping.GetRegionsId();
            ateGrouping.SetFixedValues(fixedId);
            const string regionsTableKey = b_Regions_Bridge.InternalKey;

            // настраиваем колонки отчета
            const string terTypeTableKey = fx_FX_TerritorialPartitionType.InternalKey;
            var masks = new AteOutMasks(new TableField(regionsTableKey, b_Regions_Bridge.CodeLine));
            rep.AddCaptionColumn().SetMasks(ateGrouping, masks);
            masks = new AteOutMasks(new TableField(terTypeTableKey, fx_FX_TerritorialPartitionType.Name));
            rep.AddCaptionColumn().SetMasks(ateGrouping, masks);
            masks = new AteOutMasks(new TableField(regionsTableKey, b_Regions_Bridge.Name));
            var nameColumn = rep.AddCaptionColumn();
            nameColumn.SetMasks(ateGrouping, masks);

            foreach (var year in years)
            {
                rep.AddValueColumn(f_D_UFK14.Credit);
            }

            // получаем данные из т.ф. «УФК_Выписка из сводного реестра_c расщеплением» (f.D.UFK14)
            var filterList = new List<QFilter>
            {
                new QFilter(QUFK14.Keys.Period, String.Empty), // зарезервировано
                new QFilter(QUFK14.Keys.Lvl, filterLvl),
                new QFilter(QUFK14.Keys.KVSR, paramKVSR),
                new QFilter(QUFK14.Keys.KD, filterKD),
                new QFilter(QUFK14.Keys.Okato, paramRegion)
            };

            var groupFields = new List<Enum> { QUFK14.Keys.Okato };

            for (var i = 0; i < periods.Count; i++)
            {
                filterList[0] = new QFilter(QUFK14.Keys.Period, periods[i]);
                var queryText = new QUFK14().GetQueryText(filterList, groupFields);
                var tblData = dbHelper.GetTableData(queryText);
                rep.ProcessDataRows(tblData.Select(), nameColumn.Index + 1 + i);
            }

            tablesResult[0] = rep.GetReportData();

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.STARTDATE, startDate.ToShortDateString());
            paramHelper.SetParamValue(ParamUFKHelper.ENDDATE, endDate.ToShortDateString());
            paramHelper.SetParamValue(ParamUFKHelper.NOW, DateTime.Now.ToShortDateString());
            paramHelper.SetParamValue(ParamUFKHelper.BDGT_NAME, ReportMonthMethods.GetSelectedBudgetLvlFull(paramLvl));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(filterKD));
            paramHelper.SetParamValue(ParamUFKHelper.KVSR, reportHelper.GetKVSRBridgeCaptionText(paramKVSR));
            paramHelper.SetParamValue(ParamUFKHelper.YEARS, ConvertToString(years));
            return tablesResult;
        }
    }
}
