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
        /// ОТЧЕТ 005_ПОСТУПЛЕНИЕ ДОХОДОВ В РАЗРЕЗЕ АТЕ И УБ ПО КД И ИНН
        /// </summary>
        public DataTable[] GetUFKReport005Income(Dictionary<string, string> reportParams)
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
            var paramOrg = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamOrgID]);
            var paramKVSR = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKVSRComparable]);
            var paramLvl = SortCodes(ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamBdgtLevels]));
            var paramHideEmptyStr = Convert.ToBoolean(reportParams[ReportConsts.ParamHideEmptyStr]);

            var levels = new List<string> 
            {
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Federal),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsSubjectFractional),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsMunicipalFractional),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Municipal),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Settle),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Fonds),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.PurposeFonds)
            };

            var intParamLvl = paramLvl != String.Empty ? ConvertToIntList(paramLvl) : GetColumnsList(0, levels.Count);
            var intLevels = levels.Select(ConvertToIntList).ToList();
            intLevels = intLevels.Where((t, i) => intParamLvl.Contains(i)).ToList();
            var levelsAll = intLevels.SelectMany(e => e).Distinct().ToList();
            var needSumColumn = intLevels.Count > 1;
            
            // период
            var fltPeriod = filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV, GetUNVDate(startDate),
                                                      GetUNVDate(endDate), true);

            // получаем данные из т.ф. «УФК_Выписка из сводного реестра_c расщеплением» (f.D.UFK14)
            var filterList = new List<QFilter>
            {
                new QFilter(QUFK14.Keys.Period, fltPeriod),
                new QFilter(QUFK14.Keys.Lvl, ConvertToString(levelsAll)),
                new QFilter(QUFK14.Keys.KVSR, paramKVSR),
                new QFilter(QUFK14.Keys.Org, paramOrg),
                new QFilter(QUFK14.Keys.KD, filterKD),
                new QFilter(QUFK14.Keys.Okato, paramRegion)
            };

            var groupFields = new List<Enum> { QUFK14.Keys.Okato, QUFK14.Keys.Lvl };
            var queryText = new QUFK14().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(queryText);

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
            rep.AddCaptionColumn().SetMasks(ateGrouping, masks);

            if (needSumColumn)
            {
                rep.AddValueColumn(f_D_UFK14.Credit, filterHelper.RangeFilter(f_D_UFK14.RefFX, ConvertToString(levelsAll)));
            }

            foreach (var level in intLevels)
            {
                rep.AddValueColumn(f_D_UFK14.Credit, filterHelper.RangeFilter(f_D_UFK14.RefFX, ConvertToString(level)));
            }

            rep.ProcessTable(tblData);
            tablesResult[0] = rep.GetReportData();

            var columns = needSumColumn ? new List<int> {0, 1, 2, 3} : new List<int> {0, 1, 2};
            columns.AddRange(intParamLvl.Select(i => i + 4));

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.STARTDATE, reportParams[ReportConsts.ParamStartDate]);
            paramHelper.SetParamValue(ParamUFKHelper.ENDDATE, reportParams[ReportConsts.ParamEndDate]);
            paramHelper.SetParamValue(ParamUFKHelper.NOW, DateTime.Now.ToShortDateString());
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.COLUMNS, ConvertToString(columns));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(filterKD));
            paramHelper.SetParamValue(ParamUFKHelper.KVSR, reportHelper.GetKVSRBridgeCaptionText(paramKVSR));
            paramHelper.SetParamValue(ParamUFKHelper.INN, reportHelper.GetPayerText(paramOrg));
            return tablesResult;
        }
    }
}
