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
        /// ОТЧЕТ 030 СПОЛНЕНИЕ БЮДЖЕТА МО ПО НАЛОГОВЫМ И НЕ НАЛОГОВЫМ ДОХОДАМ
        /// </summary>
        public DataTable[] GetMonthReport030Data(Dictionary<string, string> reportParams)
        {
            const int sumColumnsCount = 6;
            var reportHelper = new ReportMonthMethods(scheme);
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramVariant = reportParams[ReportConsts.ParamVariantID];
            var paramLvl = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamBdgtLevels]);
            var level = paramLvl != String.Empty ? Convert.ToInt32(paramLvl) : 0;
            var paramGroupKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamGroupKD]);
            var paramRegion = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamRegionComparable]);
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;
            var paramMonth = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMonth]);
            var month = GetEnumItemIndex(new MonthEnum(), paramMonth) + 1;

            // периоды
            var unvMonth = GetUNVMonthStart(year, month);
            var unvPrevYearMonth = GetUNVMonthStart(year - 1, month);
            var fltMonth = filterHelper.EqualIntFilter(f_F_MonthRepIncomes.RefYearDayUNV, unvMonth);
            var fltPrevYearMonth = filterHelper.EqualIntFilter(f_F_MonthRepIncomes.RefYearDayUNV, unvPrevYearMonth);

            // группы КД
            var selectedGroupKD = ReportUFKHelper.GetSelectedID(paramGroupKD, b_D_GroupKD.InternalKey);
            var nestedKD = (from id in selectedGroupKD
                            let groupId = Convert.ToString(id)
                            let nestedGroupId = reportHelper.GetNestedIDByField(b_D_GroupKD.InternalKey, b_D_GroupKD.ID, groupId)
                            let nestedKDId = reportHelper.GetNestedIDByField(b_KD_Bridge.InternalKey, b_KD_Bridge.RefDGroupKD, nestedGroupId)
                            select new { groupId, nestedKDId }).ToDictionary(e => e.groupId, e => e.nestedKDId);
            var filterKD = GetDistinctCodes(ConvertToString(nestedKD.Values));

            // уровни
            var levels = new[]
            {
                new[] {SettleLvl.Town, SettleLvl.MR, SettleLvl.SettleFractional},
                new[] {SettleLvl.Town, SettleLvl.MR},
                new[] {SettleLvl.SettleFractional}
            };

            var levelsId = levels[level].Select(ReportMonthMethods.GetBdgtLvlCodes).ToArray();
            var levelsSkifId = levels[level].Select(ReportMonthMethods.GetBdgtLvlSKIFCodes).ToArray();
            var docTypesId = levels[level].Select(ReportMonthMethods.GetDocumentTypesSkif).ToArray();
            var filterLevels = ConvertToString(levelsId);
            var filterLevelsSkif = ConvertToString(levelsSkifId);
            var filterDocTypes = ConvertToString(docTypesId);

            // получаем данные из т. ф. "Факт.ФО_МесОтч_Доходы" (f.F.MonthRepIncomes)
            var filterMonthRep = new List<QFilter>
            {
                new QFilter(QMonthRep.Keys.Day, Combine(unvMonth, unvPrevYearMonth)),
                new QFilter(QMonthRep.Keys.KD, filterKD),
                new QFilter(QMonthRep.Keys.Means, ReportMonthConsts.MeansTypeBudget),
                new QFilter(QMonthRep.Keys.DocTyp, filterDocTypes),
                new QFilter(QMonthRep.Keys.Lvl, filterLevelsSkif),
                new QFilter(QMonthRep.Keys.Okato, paramRegion)
            };
            var groupMonthRep = new List<Enum>
            {
                QMonthRep.Keys.Day,
                QMonthRep.Keys.KD,
                QMonthRep.Keys.Lvl,
                QMonthRep.Keys.DocTyp
            };
            var queryMonthRep = new QMonthRep().GetQueryText(filterMonthRep, groupMonthRep);
            var tblMonthRep = dbHelper.GetTableData(queryMonthRep);

            // получаем данные из т. ф. "Доходы.ФО_ГодОтч_Доходы" (f.D.FOYRIncomes)
            var filterFOYR = new List<QFilter>
            {
                new QFilter(QdFOYRIncomes.Keys.Day, GetUNVYearPlanLoBound(year - 1)),
                new QFilter(QdFOYRIncomes.Keys.KD, filterKD),
                new QFilter(QdFOYRIncomes.Keys.Means, ReportMonthConsts.MeansTypeBudget),
                new QFilter(QdFOYRIncomes.Keys.DocTyp, filterDocTypes),
                new QFilter(QdFOYRIncomes.Keys.Lvl, filterLevelsSkif),
                new QFilter(QdFOYRIncomes.Keys.Okato, paramRegion)
            };
            var groupFOYR = new List<Enum>
            {
                QdFOYRIncomes.Keys.KD,
                QdFOYRIncomes.Keys.Lvl,
                QdFOYRIncomes.Keys.DocTyp
            };
            var queryFOYR = new QdFOYRIncomes().GetQueryText(filterFOYR, groupFOYR);
            var tblFOYR = dbHelper.GetTableData(queryFOYR);

            // получаем данные (план на год) из т. ф. "ФО_Результат доходов с расщеплением" (f.D.FOPlanIncDivide)
            var filterPlan = new List<QFilter>
            {
                new QFilter(QFOPlanIncDivide.Keys.Day, unvMonth),
                new QFilter(QFOPlanIncDivide.Keys.Variant, ReportMonthConsts.VariantPlan),
                new QFilter(QFOPlanIncDivide.Keys.Lvl, filterLevels),
                new QFilter(QFOPlanIncDivide.Keys.KD, filterKD),
                new QFilter(QFOPlanIncDivide.Keys.Okato, paramRegion)
            };
            var groupPlan = new List<Enum> { QFOPlanIncDivide.Keys.KD };
            var queryPlan = new QFOPlanIncDivide().GetQueryText(filterPlan, groupPlan);
            var tblPlan = dbHelper.GetTableData(queryPlan);

            // получаем данные (оценка) из т. ф. "ФО_Результат доходов с расщеплением" (f.D.FOPlanIncDivide)
            var filterEstimate = new List<QFilter>
            {
                new QFilter(QFOPlanIncDivide.Keys.Variant, paramVariant),
                new QFilter(QFOPlanIncDivide.Keys.Lvl, filterLevels),
                new QFilter(QFOPlanIncDivide.Keys.KD, filterKD),
                new QFilter(QFOPlanIncDivide.Keys.Okato, paramRegion)
            };
            var groupEstimate = new List<Enum> { QFOPlanIncDivide.Keys.KD };
            var queryEstimate = new QFOPlanIncDivide().GetQueryText(filterEstimate, groupEstimate);
            var tblEstimate = dbHelper.GetTableData(queryEstimate);

            // параметры отчета
            var rep = new Report(f_F_MonthRepIncomes.InternalKey)
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

            for (var i = 0; i < sumColumnsCount; i++)
            {
                rep.AddValueColumn(String.Format("{0}{1}", SUM, i));
            }

            // временная таблица
            var dt = new DataTable();
            AddColumnToReport(dt, typeof(int), b_KD_Bridge.RefDGroupKD);
            AddColumnToReport(dt, typeof(decimal), SUM, sumColumnsCount);

            foreach (var groupKD in nestedKD.Where(e => e.Value != String.Empty))
            {
                var fltKDMonthRep = filterHelper.RangeFilter(d_KD_MonthRep.RefKDBridge, groupKD.Value);
                var fltKDFOYR = filterHelper.RangeFilter(d_KD_FOYR.RefKDBridge, groupKD.Value);
                var fltKDPlanIncomes = filterHelper.RangeFilter(d_KD_PlanIncomes.RefBridge, groupKD.Value);

                for (var i = 0; i < levels[level].Length; i++)
                {
                    var fltMonthRep = CombineAnd(fltKDMonthRep,
                                                 filterHelper.RangeFilter(f_F_MonthRepIncomes.RefBdgtLevels, levelsSkifId[i]),
                                                 filterHelper.RangeFilter(d_Regions_MonthRep.RefDocType, docTypesId[i]));
                    var fltFOYR = CombineAnd(fltKDFOYR,
                                             filterHelper.RangeFilter(f_D_FOYRIncomes.RefBdgtLevels, levelsSkifId[i]),
                                             filterHelper.RangeFilter(d_Regions_FOYR.RefDocType, docTypesId[i]));
                    var row = dt.Rows.Add();
                    row[b_KD_Bridge.RefDGroupKD] = groupKD.Key;
                    row["Sum0"] = GetSumFieldValue(tblMonthRep, SUM, CombineAnd(fltPrevYearMonth, fltMonthRep));
                    row["Sum1"] = GetSumFieldValue(tblFOYR, f_D_FOYRIncomes.Performed, fltFOYR);
                    row["Sum3"] = GetSumFieldValue(tblMonthRep, SUM1, CombineAnd(fltMonth, fltMonthRep));
                    row["Sum4"] = GetSumFieldValue(tblMonthRep, SUM, CombineAnd(fltMonth, fltMonthRep));
                    if (i == 0)
                    {
                        row["Sum2"] = GetSumFieldValue(tblPlan, f_D_FOPlanIncDivide.YearPlan, fltKDPlanIncomes);
                        row["Sum5"] = GetSumFieldValue(tblEstimate, f_D_FOPlanIncDivide.Estimate, fltKDPlanIncomes);
                    }
                }
            }

            rep.ProcessTable(dt);
            tablesResult[0] = rep.GetReportData();

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.YEAR, year);
            paramHelper.SetParamValue(ParamUFKHelper.VARIANT, ReportMonthMethods.GetSelectedVariant(paramVariant));
            paramHelper.SetParamValue(ParamUFKHelper.MONTH, month);
            paramHelper.SetParamValue(ParamUFKHelper.SETTLEMENT, reportHelper.GetNotNestedRegionCaptionText(paramRegion));
            paramHelper.SetParamValue(ParamUFKHelper.BDGT_NAME, ReportMonthMethods.GetSelectedBudgetLvlFull(paramLvl));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
