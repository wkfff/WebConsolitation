using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData;
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
        /// ОТЧЕТ 020 УТОЧНЕНИЕ ПЛАНОВЫХ ПОКАЗАТЕЛЕЙ ПО НАЛОГОВЫМ И НЕ НАЛОГОВЫМ ДОХОДАМ
        /// </summary>
        public DataTable[] GetMonthReport020Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramLvl = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamBdgtLevels]);
            var paramGroupKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamGroupKD]);
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;
            var paramMonth = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMonth]);
            var months = paramMonth != String.Empty ? ConvertToIntList(paramMonth) : GetColumnsList(0, 12);
            months = months.Select(month => month + 1).OrderBy(month => month).ToList();

            // периоды
            var periods = months.Select(month => GetUNVMonthStart(year, month)).ToList();

            // уровни
            var levels = new List<string>
            {
                ConvertToString(new []
                {
                    ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject),
                    ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Municipal),
                    ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.SettleFractional)
                }),

                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject),

                ConvertToString(new []
                {
                    ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Municipal),
                    ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.SettleFractional)
                }),
            };
            var intLvl = paramLvl != String.Empty ? ConvertToIntList(paramLvl) : new List<int>();
            levels = levels.Where((t, i) => intLvl.Contains(i)).ToList();

            // группы КД
            var selectedGroupKD = ReportUFKHelper.GetSelectedID(paramGroupKD, b_D_GroupKD.InternalKey);
            var nestedKD = (from id in selectedGroupKD
                            let groupId = Convert.ToString(id)
                            let nestedGroupId = reportHelper.GetNestedIDByField(b_D_GroupKD.InternalKey, b_D_GroupKD.ID, groupId)
                            let nestedKDId = reportHelper.GetNestedIDByField(b_KD_Bridge.InternalKey, b_KD_Bridge.RefDGroupKD, nestedGroupId)
                            select new { groupId, nestedKDId }).ToDictionary(e => e.groupId, e => e.nestedKDId);
            var filterKD = GetDistinctCodes(ConvertToString(nestedKD.Values));

            // параметры отчета
            var rep = new Report(f_D_FOPlanIncDivide.InternalKey)
            {
                Divider = GetDividerValue(divider),
                AddTotalRow = false
            };
            // группировка по группам КД
            var kdGrouping = rep.AddGrouping(b_KD_Bridge.RefDGroupKD);
            kdGrouping.HierarchyTable = dbHelper.GetEntityData(b_D_GroupKD.InternalKey);
            kdGrouping.HideHierarchyLevels = true;
            kdGrouping.AddLookupField(b_D_GroupKD.InternalKey, b_D_GroupKD.Name);
            kdGrouping.AddLookupField(b_D_GroupKD.InternalKey, b_D_GroupKD.Code).Type = typeof(int);
            kdGrouping.ViewParams.Add(1, new RowViewParams { BreakSumming = true, Style = 1 });
            kdGrouping.ViewParams.Add(2, new RowViewParams { BreakSumming = true, Style = 2, ForAllLevels = true });
            kdGrouping.AddSortField(b_D_GroupKD.InternalKey, b_D_GroupKD.Code);
            kdGrouping.SetFixedValues(paramGroupKD);
            // сортируем и настраиваем колонки отчета
            var captionColumn = rep.AddCaptionColumn();
            captionColumn.SetMask(kdGrouping, 0, b_D_GroupKD.InternalKey, b_D_GroupKD.Name).ForAllLevels = true;

            foreach (var period in periods)
            {
                rep.AddValueColumn(SUM);
            }

            // временная таблица
            var dt = new DataTable();
            dt.Columns.Add(b_KD_Bridge.RefDGroupKD, typeof(int));
            dt.Columns.Add(SUM, typeof(decimal));

            // получаем данные из т. ф. "ФО_Результат доходов с расщеплением" (f.D.FOPlanIncDivide)
            var filterList = new List<QFilter>
            {
                new QFilter(QFOPlanIncDivide.Keys.Day, ConvertToString(periods)),
                new QFilter(QFOPlanIncDivide.Keys.Lvl, ConvertToString(levels)),
                new QFilter(QFOPlanIncDivide.Keys.KD, filterKD),
                new QFilter(QFOPlanIncDivide.Keys.Variant, ReportMonthConsts.VariantPlan)
            };

            var groupFields = new List<Enum> { QFOPlanIncDivide.Keys.Day, QFOPlanIncDivide.Keys.KD };
            var query = new QFOPlanIncDivide().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(query);

            for (var i = 0; i < periods.Count; i++)
            {
                dt.Rows.Clear();
                var fltPeriod = filterHelper.EqualIntFilter(f_D_FOPlanIncDivide.RefYearDayUNV, periods[i]);

                foreach (var groupKD in nestedKD.Where(e => e.Value != String.Empty))
                {
                    var fltKD = filterHelper.RangeFilter(d_KD_PlanIncomes.RefBridge, groupKD.Value);
                    dt.Rows.Add(groupKD.Key,
                                GetSumFieldValue(tblData, f_D_FOPlanIncDivide.YearPlan, CombineAnd(fltPeriod, fltKD)));
                }

                rep.ProcessDataRows(dt.Select(), captionColumn.Index + 1 + i);
            }

            tablesResult[0] = rep.GetReportData();

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.YEAR, year);
            paramHelper.SetParamValue(ParamUFKHelper.MONTH, ConvertToString(months));
            paramHelper.SetParamValue(ParamUFKHelper.BDGT_NAME, ReportMonthMethods.GetSelectedBudgetLvlFull(paramLvl));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
