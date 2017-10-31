using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsData.UFK;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.MOFO.Queries;
using Krista.FM.Client.Reports.Month;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK.ReportMaster;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{

    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 024 ИСПОЛНЕНИЕ БЮДЖЕТА МО ПО ДОХОДНОМУ ИСТОЧНИКУ ЗА ОТЧЕТНЫЙ ГОД
        /// </summary>
        public DataTable[] GetUFKReport024(Dictionary<string, string> reportParams)
        {
            const string kdId = "KDID";
            const string lvlId = "LVLID";
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var paramKVSR = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKVSRComparable]);
            var year = ReportMonthMethods.GetSelectedYear(reportParams[ReportConsts.ParamYear]);

            // КД
            var selectedKD = ReportUFKHelper.GetSelectedID(paramKD, b_KD_Bridge.InternalKey);
            var nestedKD = (from id in selectedKD
                            let filter = Convert.ToString(id)
                            let nestedId = reportHelper.GetNestedIDByField(b_KD_Bridge.InternalKey, b_KD_Bridge.ID, filter)
                            select new { id, nestedId }).ToDictionary(e => e.id, e => e.nestedId);
            var filterKD = GetDistinctCodes(ConvertToString(nestedKD.Values));

            // уровни
            var levelsUfk = new List<string>
            {
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsMunicipalFractional)
            };

            var levelsPlan = new List<string>
            {
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject),
                Combine(ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Municipal),
                        ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.SettleFractional))
            };

            // период
            var fltPeriodUfk = filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV,
                                                         GetUNVYearStart(year),
                                                         GetUNVYearEnd(year), true);

            var lastPlanMonth = reportHelper.GetMaxMonth(f_D_FOPlanIncDivide.InternalKey,
                                                         f_D_FOPlanIncDivide.RefYearDayUNV, year);
            var unvMonth = lastPlanMonth > 0 ? GetUNVMonthStart(year, lastPlanMonth) : ReportConsts.UndefinedKey;

            // параметры отчета
            var rep = new Report(String.Empty)
            {
                Divider = GetDividerValue(divider),
                AddTotalRow = false
            };
            // группировка по КД
            var kdGrouping = rep.AddGrouping(kdId);
            kdGrouping.AddLookupField(b_KD_Bridge.InternalKey, b_KD_Bridge.Name, b_KD_Bridge.CodeStr);
            kdGrouping.AddSortField(b_KD_Bridge.InternalKey, b_KD_Bridge.CodeStr);
            kdGrouping.SetFixedValues(paramKD);
            // сортируем и настраиваем колонки отчета
            rep.AddCaptionColumn().SetMask(kdGrouping, 0, b_KD_Bridge.InternalKey, b_KD_Bridge.CodeStr);
            rep.AddCaptionColumn().SetMask(kdGrouping, 0, b_KD_Bridge.InternalKey, b_KD_Bridge.Name);

            foreach (var level in levelsPlan)
            {
                rep.AddValueColumn(f_D_FOPlanIncDivide.InternalKey, SUM, filterHelper.RangeFilter(lvlId, level));
            }

            foreach (var level in levelsUfk)
            {
                rep.AddValueColumn(f_D_UFK14.InternalKey, SUM, filterHelper.RangeFilter(lvlId, level));
            }

            // временная таблица
            var dt = new DataTable();
            dt.Columns.Add(kdId, typeof(int));
            dt.Columns.Add(lvlId, typeof(int));
            dt.Columns.Add(SUM, typeof(decimal));

            // получаем данные (оценка) из т. ф. "ФО_Результат доходов с расщеплением" (f.D.FOPlanIncDivide)
            var filterPlan = new List<QFilter>
            {
                new QFilter(QFOPlanIncDivide.Keys.Day, unvMonth),
                new QFilter(QFOPlanIncDivide.Keys.Variant, ReportMonthConsts.VariantPlan),
                new QFilter(QFOPlanIncDivide.Keys.Lvl, GetDistinctCodes(ConvertToString(levelsPlan))),
                new QFilter(QFOPlanIncDivide.Keys.KD, filterKD),
                new QFilter(QFOPlanIncDivide.Keys.KVSR, paramKVSR)
            };

            var groupPlan = new List<Enum> {QFOPlanIncDivide.Keys.KD, QFOPlanIncDivide.Keys.Lvl};
            var queryPlan = new QFOPlanIncDivide().GetQueryText(filterPlan, groupPlan);
            var tblPlan = dbHelper.GetTableData(queryPlan);

            foreach (var groupKD in nestedKD.Where(e => e.Value != String.Empty))
            {
                var rows = tblPlan.Select(filterHelper.RangeFilter(d_KD_PlanIncomes.RefBridge, groupKD.Value));

                foreach (var row in rows)
                {
                    dt.Rows.Add(groupKD.Key, row[f_D_FOPlanIncDivide.RefBudLevel], row[f_D_FOPlanIncDivide.YearPlan]);
                }
            }

            rep.ProcessTable(f_D_FOPlanIncDivide.InternalKey, dt);

            // получаем данные из т. ф. "Доходы.УФК_Выписка из сводного реестра_c расщеплением" (f.D.UFK14)
            var filterUfk = new List<QFilter>
            {
                new QFilter(QUFK14.Keys.Period, fltPeriodUfk),
                new QFilter(QUFK14.Keys.Lvl, GetDistinctCodes(ConvertToString(levelsUfk))),
                new QFilter(QUFK14.Keys.KD, filterKD),
                new QFilter(QUFK14.Keys.KVSR, paramKVSR)
            };

            var groupUfk = new List<Enum> { QUFK14.Keys.KD, QUFK14.Keys.Lvl };
            var queryUfk = new QUFK14().GetQueryText(filterUfk, groupUfk);
            var tblUfk = dbHelper.GetTableData(queryUfk);
            dt.Rows.Clear();

            foreach (var groupKD in nestedKD.Where(e => e.Value != String.Empty))
            {
                var rows = tblUfk.Select(filterHelper.RangeFilter(d_KD_UFK.RefKDBridge, groupKD.Value));

                foreach (var row in rows)
                {
                    dt.Rows.Add(groupKD.Key, row[f_D_UFK14.RefFX], row[f_D_UFK14.Credit]);
                }
            }

            rep.ProcessTable(f_D_UFK14.InternalKey, dt);

            tablesResult[0] = rep.GetReportData();

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.YEAR, year);
            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(filterKD));
            paramHelper.SetParamValue(ParamUFKHelper.KVSR, reportHelper.GetKVSRBridgeCaptionText(paramKVSR));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
