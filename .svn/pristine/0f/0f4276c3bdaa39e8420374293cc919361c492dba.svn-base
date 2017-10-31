using System;
using System.Collections.Generic;
using System.Data;
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
        /// ОТЧЕТ 006 ИНФОРМАЦИЯ О ПОСТУПЛЕНИЯХ ВЫБРАННОГО НАЛОГОПЛАТЕЛЬЩИКА В РАЗЛИЧНЫЕ УРОВНИ БЮДЖЕТА
        /// </summary>
        public DataTable[] GetUFKReport006SelectPayments(Dictionary<string, string> reportParams)
        {
            const int styleAte = 1;
            const int styleKd = 6;
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[3];
            var startDate = Convert.ToDateTime(reportParams[ReportConsts.ParamStartDate]);
            var endDate = Convert.ToDateTime(reportParams[ReportConsts.ParamEndDate]);
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var filterKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var paramOrg = reportParams[ReportConsts.ParamOrgID];
            var paramLvl = SortCodes(ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamBdgtLevels]));

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
                new QFilter(QUFK14.Keys.Org, paramOrg),
                new QFilter(QUFK14.Keys.KD, filterKD)
            };

            var groupFields = new List<Enum>
            {
                QUFK14.Keys.Org,
                QUFK14.Keys.Okato,
                QUFK14.Keys.KD,
                QUFK14.Keys.Day,
                QUFK14.Keys.Lvl
            };
            var queryText = new QUFK14().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(queryText);

            // параметры отчета
            var rep = new Report(f_D_UFK14.InternalKey) {Divider = GetDividerValue(divider)};
            // Лист 1
            // группировка по организациям
            const string orgTableKey = b_Org_PayersBridge.InternalKey;
            var orgGrouping = rep.AddGrouping(d_Org_UFKPayers.RefOrgPayersBridge);
            orgGrouping.AddLookupField(orgTableKey, b_Org_PayersBridge.INN, b_Org_PayersBridge.Name);
            orgGrouping.AddSortField(orgTableKey, b_Org_PayersBridge.INN);
            orgGrouping.SetFixedValues(ReportMonthMethods.CheckBookValue(paramOrg));
            // группировка по АТЕ
            var ateGrouping = rep.AddGrouping(d_OKATO_UFK.RefRegionsBridge,
                                              new AteGrouping(styleAte) {HideConsBudjetRow = true});
            // группировка по КД
            const string kdTableKey = b_KD_Bridge.InternalKey;
            var kdGrouping = rep.AddGrouping(d_KD_UFK.RefKDBridge);
            kdGrouping.AddLookupField(kdTableKey, b_KD_Bridge.CodeStr, b_KD_Bridge.Name);
            kdGrouping.AddSortField(kdTableKey, b_KD_Bridge.CodeStr);
            kdGrouping.ViewParams[0].Style = styleKd;
            
            // сортируем и настраиваем колонки отчета
            var nameColumn = rep.AddCaptionColumn();
            nameColumn.SetMask(orgGrouping, 0, orgTableKey, b_Org_PayersBridge.INN, b_Org_PayersBridge.Name);
            nameColumn.SetMasks(ateGrouping, new AteOutMasks());
            nameColumn.SetMask(kdGrouping, 0, kdTableKey, b_KD_Bridge.CodeStr);
            var nameKdColumn = rep.AddCaptionColumn();
            nameKdColumn.SetMask(kdGrouping, 0, kdTableKey, b_KD_Bridge.Name);

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

            // Лист 2
            rep.Clear();
            kdGrouping.ViewParams[0].ShowOrder = RowViewParams.ShowType.SkipBeforeChild;
            // группировка по дате
            var dateGrouping = rep.AddGrouping(f_D_UFK14.RefYearDayUNV);
            dateGrouping.AddSortField(f_D_UFK14.InternalKey, f_D_UFK14.RefYearDayUNV);
            dateGrouping.ViewParams[0].Style = styleKd;
            // привязываем код КД к группировке по дате
            nameColumn.RemoveMask(kdGrouping.Index);
            var codeKdMask = nameColumn.SetMask(dateGrouping, 0, kdTableKey, b_KD_Bridge.CodeStr);
            codeKdMask.GroupingIndex = kdGrouping.Index;
            codeKdMask.Level = 0;
            // привязываем наименование КД к группировке по дате
            nameKdColumn.RemoveMask(kdGrouping.Index);
            var nameKdMask = nameKdColumn.SetMask(dateGrouping, 0, kdTableKey, b_KD_Bridge.Name);
            nameKdMask.GroupingIndex = kdGrouping.Index;
            nameKdMask.Level = 0;
            // колонка Дата
            var dateColumn = rep.InsertCaptionColumn(nameKdColumn.Index + 1);
            dateColumn.SetMask(dateGrouping, 0, f_D_UFK14.InternalKey, f_D_UFK14.RefYearDayUNV);

            rep.ProcessTable(tblData);
            var dt = rep.GetReportData();

            // форматируем колонку Дата
            foreach (DataRow row in dt.Rows)
            {
                if (row[STYLE] != DBNull.Value && Convert.ToInt32(row[STYLE]) == styleKd)
                {
                    row[dateColumn.Index] = GetNormalDate(Convert.ToString(row[dateColumn.Index])).ToShortDateString();
                }
            }

            tablesResult[1] = dt;

            var columns = needSumColumn ? new List<int> {0} : new List<int>();
            columns.AddRange(intParamLvl.Select(i => i + 1));

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
            return tablesResult;
        }
    }
}
