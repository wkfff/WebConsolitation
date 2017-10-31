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
        /// ОТЧЕТ 028 ОЖИДАЕМОЕ ПОСТУПЛЕНИЕ НАЛОГОВЫХ И НЕНАЛОГОВЫХ ДОХОДОВ В КБС В ТЕКУЩЕМ ГОДУ
        /// </summary>
        public DataTable[] GetMonthReport028Data(Dictionary<string, string> reportParams)
        {
            const int sumColumnsCount = 6;
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramMeans = reportParams[ReportConsts.ParamMeans];
            var paramDocType = reportParams[ReportConsts.ParamDocType];
            var paramLvl = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamBdgtLevels]);
            var paramGroupKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamGroupKD]);
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;
            var paramMonth = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMonth]);
            var month = GetEnumItemIndex(new MonthEnum(), paramMonth) + 1;

            // периоды
            var yearPeriods = new[] {GetUNVYearPlanLoBound(year - 2), GetUNVYearPlanLoBound(year - 1)};

            var monthPeriods = new[]
            {
                GetUNVMonthStart(year - 1, month),
                GetUNVMonthStart(year - 1, 12),
                GetUNVMonthStart(year, month)
            };

            var fltPeriods = monthPeriods.Select(e => filterHelper.EqualIntFilter(f_D_FOYRIncomes.RefYearDayUNV, e)).ToList();

            // уровни
            var levels = new List<string>
            {
                ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.ConsSubject),
                ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.Subject),
                ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.ConsMunicipal)
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
            var rep = new Report(f_D_FOYRIncomes.InternalKey)
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
                rep.AddValueColumn(SUM);
            }

            // временная таблица
            var dt = new DataTable();
            dt.Columns.Add(b_KD_Bridge.RefDGroupKD, typeof(int));
            dt.Columns.Add(SUM, typeof(decimal));
            var index = captionColumn.Index;

            // получаем данные из т. ф. "Доходы.ФО_ГодОтч_Доходы" (f.D.FOYRIncomes)
            var filterFOYR = new List<QFilter>
            {
                new QFilter(QdFOYRIncomes.Keys.Day, ConvertToString(yearPeriods)),
                new QFilter(QdFOYRIncomes.Keys.Lvl, ConvertToString(levels)),
                new QFilter(QdFOYRIncomes.Keys.KD, filterKD),
                new QFilter(QdFOYRIncomes.Keys.Means, paramMeans),
                new QFilter(QdFOYRIncomes.Keys.DocTyp, paramDocType)
            };

            var groupFOYR = new List<Enum> { QdFOYRIncomes.Keys.Day, QdFOYRIncomes.Keys.KD };
            var queryFOYR = new QdFOYRIncomes().GetQueryText(filterFOYR, groupFOYR);
            var tblFOYR = dbHelper.GetTableData(queryFOYR);

            foreach (var period in yearPeriods)
            {
                dt.Rows.Clear();
                var fltPeriod = filterHelper.EqualIntFilter(f_D_FOYRIncomes.RefYearDayUNV, period);

                foreach (var groupKD in nestedKD.Where(e => e.Value != String.Empty))
                {
                    var fltKD = filterHelper.RangeFilter(d_KD_UFK.RefKDBridge, groupKD.Value);
                    dt.Rows.Add(groupKD.Key,
                                GetSumFieldValue(tblFOYR, f_D_FOYRIncomes.Performed, CombineAnd(fltPeriod, fltKD)));
                }

                rep.ProcessDataRows(dt.Select(), ++index);
            }

            // получаем данные из т. ф. "Факт.ФО_МесОтч_Доходы" (f.F.MonthRepIncomes)
            var filterMonthRep = new List<QFilter>
            {
                new QFilter(QMonthRep.Keys.Day, ConvertToString(monthPeriods.Distinct())),
                new QFilter(QMonthRep.Keys.Lvl, ConvertToString(levels)),
                new QFilter(QMonthRep.Keys.KD, filterKD),
                new QFilter(QMonthRep.Keys.Means, paramMeans),
                new QFilter(QMonthRep.Keys.DocTyp, paramDocType)
            };

            var groupMonthRep = new List<Enum> { QMonthRep.Keys.Day, QMonthRep.Keys.KD };
            var queryMonthRep = new QMonthRep().GetQueryText(filterMonthRep, groupMonthRep);
            var tblMonthRep = dbHelper.GetTableData(queryMonthRep);
            dt.Rows.Clear();
            var row = dt.Rows.Add();

            foreach (var groupKD in nestedKD.Where(e => e.Value != String.Empty))
            {
                var fltKD = filterHelper.RangeFilter(d_KD_UFK.RefKDBridge, groupKD.Value);
                row[b_KD_Bridge.RefDGroupKD] = groupKD.Key;

                row[SUM] = GetSumFieldValue(tblMonthRep, SUM1, CombineAnd(fltPeriods[2], fltKD));
                rep.ProcessDataRows(new [] {row}, index + 1);

                var sumPrevPeriod = GetSumFieldValue(tblMonthRep, SUM, CombineAnd(fltPeriods[0], fltKD));
                row[SUM] = sumPrevPeriod;
                rep.ProcessDataRows(new[] { row }, index + 2);

                row[SUM] = GetSumFieldValue(tblMonthRep, SUM, CombineAnd(fltPeriods[2], fltKD));
                rep.ProcessDataRows(new[] { row }, index + 3);

                if (month < 12)
                {
                    var sumPrevYear = GetSumFieldValue(tblMonthRep, SUM, CombineAnd(fltPeriods[1], fltKD));
                    row[SUM] = GetNotNullSumDifference(sumPrevYear, sumPrevPeriod);
                    rep.ProcessDataRows(new[] {row}, index + 4);
                }
            }

            tablesResult[0] = rep.GetReportData();

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.YEAR, year);
            paramHelper.SetParamValue(ParamUFKHelper.MONTH, month);
            paramHelper.SetParamValue(ParamUFKHelper.BDGT_NAME, ReportMonthMethods.GetSelectedBudgetLvlFull(paramLvl));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
