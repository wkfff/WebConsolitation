using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsData.UFK;
using Krista.FM.Client.Reports.Database.ClsFx;
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
        /// ОТЧЕТ 029 АНАЛИЗ ИСПОЛНЕНИЯ БЮДЖЕТОВ МУНИЦИПАЛЬНЫХ ОБРАЗОВАНИЙ
        /// </summary>
        public DataTable[] GetMonthReport029Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var reportHelper = new ReportMonthMethods(scheme);
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[2];
            var paramHideEmptyStr = Convert.ToBoolean(reportParams[ReportConsts.ParamHideEmptyStr]);
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramVariant = reportParams[ReportConsts.ParamVariantID];
            var paramKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var paramRgnListType = reportParams[ReportConsts.ParamRegionListType];
            var hideSettlement = !ReportMonthMethods.WriteSettles(paramRgnListType);
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;
            var paramMonth = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMonth]);
            var month = GetEnumItemIndex(new MonthEnum(), paramMonth) + 1;

            // периоды
            var unvMonth = GetUNVMonthStart(year, month);
            var unvPrevYearMonth = GetUNVMonthStart(year - 1, month);
            var fltMonth = filterHelper.EqualIntFilter(f_F_MonthRepIncomes.RefYearDayUNV, unvMonth);
            var fltPrevYearMonth = filterHelper.EqualIntFilter(f_F_MonthRepIncomes.RefYearDayUNV, unvPrevYearMonth);

            // уровни
            var levels = new[]
            {
                SettleLvl.Town,
                SettleLvl.MR,
                SettleLvl.SettleFractional
            };

            var levelsId = levels.Select(ReportMonthMethods.GetBdgtLvlCodes).ToArray();
            var levelsSkifId = levels.Select(ReportMonthMethods.GetBdgtLvlSKIFCodes).ToArray();
            var docTypesId = levels.Select(ReportMonthMethods.GetDocumentTypesSkif).ToArray();
            var filterLevels = ConvertToString(levelsId);
            var filterLevelsSkif = ConvertToString(levelsSkifId);
            var filterDocTypes = ConvertToString(docTypesId);

            // получаем данные из т. ф. "Факт.ФО_МесОтч_Доходы" (f.F.MonthRepIncomes)
            var filterMonthRep = new List<QFilter>
            {
                new QFilter(QMonthRep.Keys.Day, Combine(unvMonth, unvPrevYearMonth)),
                new QFilter(QMonthRep.Keys.KD, paramKD),
                new QFilter(QMonthRep.Keys.Means, ReportMonthConsts.MeansTypeBudget),
                new QFilter(QMonthRep.Keys.DocTyp, filterDocTypes),
                new QFilter(QMonthRep.Keys.Lvl, filterLevelsSkif)
            };
            var groupMonthRep = new List<Enum>
            {
                QMonthRep.Keys.Day,
                QMonthRep.Keys.Okato,
                QMonthRep.Keys.Lvl,
                QMonthRep.Keys.DocTyp
            };
            var queryMonthRep = new QMonthRep().GetQueryText(filterMonthRep, groupMonthRep);
            var tblMonthRep = dbHelper.GetTableData(queryMonthRep);

            // получаем данные из т. ф. "Доходы.ФО_ГодОтч_Доходы" (f.D.FOYRIncomes)
            var filterFOYR = new List<QFilter>
            {
                new QFilter(QdFOYRIncomes.Keys.Day, GetUNVYearPlanLoBound(year - 1)),
                new QFilter(QdFOYRIncomes.Keys.KD, paramKD),
                new QFilter(QdFOYRIncomes.Keys.Means, ReportMonthConsts.MeansTypeBudget),
                new QFilter(QdFOYRIncomes.Keys.DocTyp, filterDocTypes),
                new QFilter(QdFOYRIncomes.Keys.Lvl, filterLevelsSkif)
            };
            var groupFOYR = new List<Enum>
            {
                QdFOYRIncomes.Keys.Okato,
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
                new QFilter(QFOPlanIncDivide.Keys.KD, paramKD)
            };
            var groupPlan = new List<Enum> { QFOPlanIncDivide.Keys.Okato };
            var queryPlan = new QFOPlanIncDivide().GetQueryText(filterPlan, groupPlan);
            var tblPlan = dbHelper.GetTableData(queryPlan);

            // получаем данные (оценка) из т. ф. "ФО_Результат доходов с расщеплением" (f.D.FOPlanIncDivide)
            var filterEstimate = new List<QFilter>
            {
                new QFilter(QFOPlanIncDivide.Keys.Variant, paramVariant),
                new QFilter(QFOPlanIncDivide.Keys.Lvl, filterLevels),
                new QFilter(QFOPlanIncDivide.Keys.KD, paramKD)
            };
            var groupEstimate = new List<Enum> { QFOPlanIncDivide.Keys.Okato };
            var queryEstimate = new QFOPlanIncDivide().GetQueryText(filterEstimate, groupEstimate);
            var tblEstimate = dbHelper.GetTableData(queryEstimate);

            // параметры отчета
            var rep = new Report(f_F_MonthRepIncomes.InternalKey)
            {
                Divider = GetDividerValue(divider),
                RowFilter = paramHideEmptyStr ? Functions.IsNotNullRow : (RowViewParams.Function) null
            };

            // группировка по АТЕ
            var ateGrouping = new AteGrouping(0, hideSettlement);
            rep.AddGrouping(d_Regions_MonthRep.RefRegionsBridge, ateGrouping);
            var fixedId = paramHideEmptyStr ? ateGrouping.AteMainId : ateGrouping.GetRegionsId();
            ateGrouping.SetFixedValues(fixedId);

            // настраиваем колонки отчета
            const string regionsTableKey = b_Regions_Bridge.InternalKey;
            const string terTypeTableKey = fx_FX_TerritorialPartitionType.InternalKey;
            var masks = new AteOutMasks(new TableField(regionsTableKey, b_Regions_Bridge.CodeLine));
            rep.AddCaptionColumn().SetMasks(ateGrouping, masks);
            masks = new AteOutMasks(new TableField(terTypeTableKey, fx_FX_TerritorialPartitionType.Name));
            rep.AddCaptionColumn().SetMasks(ateGrouping, masks);
            masks = new AteOutMasks(new TableField(regionsTableKey, b_Regions_Bridge.Name));
            rep.AddCaptionColumn().SetMasks(ateGrouping, masks);
            var prevYearFactColumn = rep.AddValueColumn(SUM);
            var foyrColumn = rep.AddValueColumn(f_D_FOYRIncomes.Performed);
            var yearPlanColumn = rep.AddValueColumn(f_D_FOPlanIncDivide.YearPlan);
            var monthPlanColumn = rep.AddValueColumn(SUM1);
            var factColumn = rep.AddValueColumn(SUM);
            var estimateColumn = rep.AddValueColumn(f_D_FOPlanIncDivide.Estimate);

            // обрабатываем таблицы фактов
            for (var i = 0; i < levels.Length; i++)
            {
                var fltMonthRep = CombineAnd(filterHelper.RangeFilter(f_F_MonthRepIncomes.RefBdgtLevels, levelsSkifId[i]),
                                             filterHelper.RangeFilter(d_Regions_MonthRep.RefDocType, docTypesId[i]));
                var prevYearMonthRows = tblMonthRep.Select(CombineAnd(fltPrevYearMonth, fltMonthRep));
                var monthRows = tblMonthRep.Select(CombineAnd(fltMonth, fltMonthRep));
                ateGrouping.Field = new TableField(f_F_MonthRepIncomes.InternalKey, d_Regions_MonthRep.RefRegionsBridge);
                rep.ProcessDataRows(prevYearMonthRows, prevYearFactColumn.Index);
                rep.ProcessDataRows(monthRows, monthPlanColumn.Index);
                rep.ProcessDataRows(monthRows, factColumn.Index);

                var fltFOYR = CombineAnd(filterHelper.RangeFilter(f_D_FOYRIncomes.RefBdgtLevels, levelsSkifId[i]),
                                         filterHelper.RangeFilter(d_Regions_FOYR.RefDocType, docTypesId[i]));
                ateGrouping.Field = new TableField(f_D_FOYRIncomes.InternalKey, d_Regions_FOYR.RefRegionsBridge);
                rep.ProcessDataRows(tblFOYR.Select(fltFOYR), foyrColumn.Index);
            }

            ateGrouping.Field = new TableField(f_D_FOPlanIncDivide.InternalKey, d_Regions_Plan.RefBridge);
            rep.ProcessDataRows(tblPlan.Select(), yearPlanColumn.Index);
            rep.ProcessDataRows(tblEstimate.Select(), estimateColumn.Index);
            tablesResult[0] = rep.GetReportData();

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.YEAR, year);
            paramHelper.SetParamValue(ParamUFKHelper.MONTH, month);
            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(paramKD));
            paramHelper.SetParamValue(ParamUFKHelper.VARIANT, ReportMonthMethods.GetSelectedVariant(paramVariant));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
