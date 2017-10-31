using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData.UFK;
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
        /// ОТЧЕТ 008 ПОСТУПЛЕНИЯ ОТ ОРГАНИЗАЦИЙ ПО УРОВНЯМ БЮДЖЕТА
        /// </summary>
        public DataTable[] GetUFKReport008(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[5];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramRgnListType = reportParams[ReportConsts.ParamRegionListType];
            var hideSettlements = String.Compare(paramRgnListType, RegionListTypeEnum.i1.ToString(), true) != 0;
            var filterKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var filterKVSR = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKVSRComparable]);
            var startDate = GetUNVDate(reportParams[ReportConsts.ParamStartDate]);
            var endDate = GetUNVDate(reportParams[ReportConsts.ParamEndDate]);
            var paramOrg = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamOrgID]);
            var paramLvl = SortCodes(ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamBdgtLevels]));
            var limitSum = Convert.ToDecimal(reportParams[ReportConsts.ParamSum]) * ReportUFKHelper.LimitSumRate;
            
            // уровни
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
            var fltPeriod = filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV, startDate, endDate, true);

            // организации
            var filterOrg = paramOrg == String.Empty && limitSum == 0 ? ReportConsts.UndefinedKey : paramOrg;

            // получаем данные из т.ф. «УФК_Выписка из сводного реестра_c расщеплением» (f.D.UFK14)
            var filterList = new List<QFilter>
            {
                new QFilter(QUFK14.Keys.Period,  fltPeriod),
                new QFilter(QUFK14.Keys.Lvl,  ConvertToString(levelsAll)),
                new QFilter(QUFK14.Keys.KD,  filterKD),
                new QFilter(QUFK14.Keys.KVSR,  filterKVSR),
                new QFilter(QUFK14.Keys.Org,  filterOrg)
            };

            if (limitSum != 0)
            {
                // выбираем организации с суммой платежей за год свыше лимита
                var groupOrg = new List<Enum> { QUFK14.Keys.Org };
                var strSum = Convert.ToString(limitSum, CultureInfo.InvariantCulture);
                var havingFilter = filterHelper.MoreEqualFilter(QFactTable.Having(f_D_UFK14.Credit), strSum);
                var queryOrg = new QUFK14().GetQueryText(filterList, groupOrg, havingFilter);
                var tblOrg = dbHelper.GetTableData(queryOrg);
                var orgList = (from DataRow row in tblOrg.Rows
                               select Convert.ToInt32(row[d_Org_UFKPayers.RefOrgPayersBridge])).ToList();
                filterOrg = orgList.Count > 0 ? ConvertToString(orgList) : ReportConsts.UndefinedKey;
                filterList[filterList.Count - 1] = new QFilter(QUFK14.Keys.Org, filterOrg);
            }

            var groupFields = new List<Enum> {QUFK14.Keys.Org, QUFK14.Keys.Okato, QUFK14.Keys.KD, QUFK14.Keys.Lvl};
            var query = new QUFK14().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(query);

            // параметры отчета
            const string tableKey = f_D_UFK14.InternalKey;
            var rep = new Report(tableKey) {Divider = GetDividerValue(divider)};

            // Лист 1
            // группировка по организациям
            const string orgTableKey = b_Org_PayersBridge.InternalKey;
            var orgGrouping = rep.AddGrouping(d_Org_UFKPayers.RefOrgPayersBridge);
            orgGrouping.AddLookupField(orgTableKey, b_Org_PayersBridge.INN, b_Org_PayersBridge.Name);
            orgGrouping.AddSortField(orgTableKey, b_Org_PayersBridge.INN);
            orgGrouping.SetFixedValues(ReportMonthMethods.CheckBookValue(filterOrg));
            // колонки Наименование организации и ИНН
            var nameColumn = rep.AddCaptionColumn(orgGrouping, orgTableKey, b_Org_PayersBridge.Name);
            var innColumn = rep.AddCaptionColumn(orgGrouping, orgTableKey, b_Org_PayersBridge.INN);

            if (needSumColumn)
            {
                rep.AddValueColumn(f_D_UFK14.Credit, filterHelper.RangeFilter(f_D_UFK14.RefFX, ConvertToString(levelsAll)));
            }

            foreach (var level in intLevels)
            {
                rep.AddValueColumn(f_D_UFK14.Credit, filterHelper.RangeFilter(f_D_UFK14.RefFX, ConvertToString(level)));
            }

            rep.ProcessTable(tableKey, tblData);
            tablesResult[0] = rep.GetReportData();

            // Лист 2
            rep.Clear();
            // группировка по АТЕ
            var ateGrouping = new AteGrouping(1, hideSettlements) {HideConsBudjetRow = true};
            rep.AddGrouping(d_OKATO_UFK.RefRegionsBridge, ateGrouping);

            // колонка Наименование АТЕ
            var ateColumn = rep.InsertCaptionColumn(innColumn.Index + 1);
            ateColumn.SetMasks(ateGrouping, new AteOutMasks());
            rep.ProcessTable(tableKey, tblData);
            tablesResult[1] = rep.GetReportData();

            // Лист 4
            rep.Clear();
            // группировка по КД
            const string kdTableKey = b_KD_Bridge.InternalKey;
            var kdGrouping = rep.AddGrouping(d_KD_UFK.RefKDBridge);
            kdGrouping.AddLookupField(kdTableKey, b_KD_Bridge.CodeStr, b_KD_Bridge.Name);
            kdGrouping.AddSortField(kdTableKey, b_KD_Bridge.CodeStr);
            kdGrouping.ViewParams[0].Style = 6;
            // настраиваем колонки
            rep.RemoveCaptionColumn(ateColumn.Index);
            rep.RemoveCaptionColumn(innColumn.Index);
            nameColumn.SetMask(orgGrouping, 0, orgTableKey, b_Org_PayersBridge.Name, b_Org_PayersBridge.INN);
            nameColumn.SetMasks(ateGrouping, new AteOutMasks());
            nameColumn.SetMask(kdGrouping, 0, kdTableKey, b_KD_Bridge.CodeStr, b_KD_Bridge.Name);
            rep.ProcessTable(tableKey, tblData);
            tablesResult[3] = rep.GetReportData();

            // Лист 3
            rep.Clear();
            rep.RemoveGrouping(ateGrouping.Index);
            kdGrouping.ViewParams[0].Style = 1;
            rep.ProcessTable(tableKey, tblData);
            tablesResult[2] = rep.GetReportData();

            var columns = needSumColumn ? new List<int> { 0 } : new List<int> ();
            columns.AddRange(intParamLvl.Select(i => i + 1));

            // заполняем таблицу параметров
            var paramHelper = new ParamUFKHelper(CreateReportParamsRow(tablesResult));
            paramHelper.SetParamValue(ParamUFKHelper.STARTDATE, reportParams[ReportConsts.ParamStartDate]);
            paramHelper.SetParamValue(ParamUFKHelper.ENDDATE, reportParams[ReportConsts.ParamEndDate]);
            paramHelper.SetParamValue(ParamUFKHelper.COLUMNS, ConvertToString(columns));
            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(filterKD));
            paramHelper.SetParamValue(ParamUFKHelper.KVSR, reportHelper.GetKVSRBridgeCaptionText(filterKVSR));
            paramHelper.SetParamValue(ParamUFKHelper.NOW, DateTime.Now.ToShortDateString());
            paramHelper.SetParamValue(ParamUFKHelper.SUMM, reportParams[ReportConsts.ParamSum]);
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
