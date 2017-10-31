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
        /// ОТЧЕТ 023 ДИНАМИКА ПОСТУПЛЕНИЯ НАЛОГОВЫХ И НЕНАЛОГОВЫХ ДОХОДОВ В БС ПО СОСТОЯНИЮ НА ОПРЕДЕЛЕННУЮ ДАТУ И ЗА УКАЗАННЫЕ ГОДЫ С УДЕЛЬНЫМИ ВЕСАМИ
        /// </summary>
        public DataTable[] GetMonthReport023Data(Dictionary<string, string> reportParams)
        {
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
            var paramMonth = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMonth]);
            var month = GetEnumItemIndex(new MonthEnum(), paramMonth) + 1;

            // периоды
            var years = (from i in ConvertToIntList(paramYear)
                         let year = ReportMonthMethods.GetSelectedYear(Convert.ToString(i))
                         orderby year
                         select year).ToList();
            var maxMonthPeriods = (from year in years
                                   let maxMonth =
                                   reportHelper.GetMaxMonth(f_F_MonthRepIncomes.InternalKey,
                                                            f_F_MonthRepIncomes.RefYearDayUNV, year)
                                   select GetUNVMonthStart(year, maxMonth)).ToList();
            var yearPeriods = years.Select(year => GetUNVYearPlanLoBound(year)).ToList();
            var monthPeriods = years.Select(year => GetUNVMonthStart(year, month)).ToList();
            var unvMonths = new List<string>(monthPeriods);
            unvMonths.AddRange(maxMonthPeriods);
            unvMonths = unvMonths.Distinct().OrderBy(e => e).ToList();

            // уровни
            var levels = new List<string>
            {
                ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.ConsSubject),
                ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.Subject),
                ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.ConsMunicipal)
            };
            var intLvl = paramLvl != String.Empty
                ? ConvertToIntList(paramLvl).OrderBy(l => l).ToList()
                : GetColumnsList(0, levels.Count);
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
                AddTotalRow = false,
                RowFilter = null
            };
            // группировка по группам КД
            var kdGrouping = rep.AddGrouping(b_KD_Bridge.RefDGroupKD);
            kdGrouping.AddLookupField(b_D_GroupKD.InternalKey, b_D_GroupKD.Name);
            kdGrouping.AddLookupField(b_D_GroupKD.InternalKey, b_D_GroupKD.Code).Type = typeof(int);
            kdGrouping.AddSortField(b_D_GroupKD.InternalKey, b_D_GroupKD.Code);
            kdGrouping.SetFixedValues(paramGroupKD);
            // группировка по уровням бюджета
            var lvlGrouping = rep.AddGrouping(f_D_FOYRIncomes.RefBdgtLevels);
            // группировка по дате
            rep.AddGrouping(f_D_FOYRIncomes.RefYearDayUNV).ViewParams[0].BreakSumming = true;

            // сортируем и настраиваем колонки отчета
            var captionColumn = rep.AddCaptionColumn();
            captionColumn.SetMask(kdGrouping, 0, b_D_GroupKD.InternalKey, b_D_GroupKD.Name);
            captionColumn.SetMask(lvlGrouping, 0, f_D_FOYRIncomes.InternalKey, f_D_FOYRIncomes.RefBdgtLevels);

            foreach (var year in years)
            {
                rep.AddValueColumn(SUM);
            }

            // получаем данные из т. ф. "Доходы.ФО_ГодОтч_Доходы" (f.D.FOYRIncomes)
            var filterFOYR = new List<QFilter>
            {
                new QFilter(QdFOYRIncomes.Keys.Day, ConvertToString(yearPeriods)),
                new QFilter(QdFOYRIncomes.Keys.Lvl, ConvertToString(levels)),
                new QFilter(QdFOYRIncomes.Keys.KD, filterKD),
                new QFilter(QdFOYRIncomes.Keys.Means, paramMeans),
                new QFilter(QdFOYRIncomes.Keys.DocTyp, paramDocType)
            };

            var groupFOYR = new List<Enum>
            {
                QdFOYRIncomes.Keys.KD,
                QdFOYRIncomes.Keys.Lvl,
                QdFOYRIncomes.Keys.Day
            };

            var queryFOYR = new QdFOYRIncomes().GetQueryText(filterFOYR, groupFOYR);
            var tblFOYR = dbHelper.GetTableData(queryFOYR);

            // получаем данные из т. ф. "Факт.ФО_МесОтч_Доходы" (f.F.MonthRepIncomes)
            var filterMonthRep = new List<QFilter>
            {
                new QFilter(QMonthRep.Keys.Day, ConvertToString(unvMonths)),
                new QFilter(QMonthRep.Keys.Lvl, ConvertToString(levels)),
                new QFilter(QMonthRep.Keys.KD, filterKD),
                new QFilter(QMonthRep.Keys.Means, paramMeans),
                new QFilter(QMonthRep.Keys.DocTyp, paramDocType)
            };

            var groupMonthRep = new List<Enum>
            {
                QMonthRep.Keys.KD,
                QMonthRep.Keys.Lvl,
                QMonthRep.Keys.Day
            };

            var queryMonthRep = new QMonthRep().GetQueryText(filterMonthRep, groupMonthRep);
            var tblMonthRep = dbHelper.GetTableData(queryMonthRep);

            // временные таблицы
            var dtYear = new DataTable();
            dtYear.Columns.Add(b_KD_Bridge.RefDGroupKD, typeof(int));
            dtYear.Columns.Add(f_D_FOYRIncomes.RefBdgtLevels, typeof(int));
            dtYear.Columns.Add(SUM, typeof(decimal));

            var dtMonth = new DataTable();
            dtMonth.Columns.Add(b_KD_Bridge.RefDGroupKD, typeof(int));
            dtMonth.Columns.Add(f_D_FOYRIncomes.RefBdgtLevels, typeof(int));
            dtMonth.Columns.Add(f_D_FOYRIncomes.RefYearDayUNV, typeof(int));
            dtMonth.Columns.Add(SUM, typeof(decimal));

            for (var i = 0; i < years.Count; i++)
            {
                dtYear.Rows.Clear();
                dtMonth.Rows.Clear();

                foreach (var groupKD in nestedKD.Where(e => e.Value != String.Empty))
                {
                    for (var l = 0; l < levels.Count; l++ )
                    {
                        var fltYear = CombineAnd(
                            filterHelper.EqualIntFilter(f_D_FOYRIncomes.RefYearDayUNV, yearPeriods[i]),
                            filterHelper.RangeFilter(d_KD_UFK.RefKDBridge, groupKD.Value),
                            filterHelper.RangeFilter(f_D_FOYRIncomes.RefBdgtLevels, levels[l])
                        );


                        var fltMonth = CombineAnd(
                            filterHelper.EqualIntFilter(f_F_MonthRepIncomes.RefYearDayUNV, monthPeriods[i]),
                            filterHelper.RangeFilter(d_KD_UFK.RefKDBridge, groupKD.Value),
                            filterHelper.RangeFilter(f_F_MonthRepIncomes.RefBdgtLevels, levels[l])
                        );

                        var sumYear = GetSumFieldValue(tblFOYR, f_D_FOYRIncomes.Performed, fltYear);
                        if (sumYear == DBNull.Value)
                        {
                            var fltMaxMonth = CombineAnd(
                                filterHelper.EqualIntFilter(f_F_MonthRepIncomes.RefYearDayUNV, maxMonthPeriods[i]),
                                filterHelper.RangeFilter(d_KD_UFK.RefKDBridge, groupKD.Value),
                                filterHelper.RangeFilter(f_F_MonthRepIncomes.RefBdgtLevels, levels[l])
                            );
                            sumYear = GetSumFieldValue(tblMonthRep, SUM1, fltMaxMonth);
                        }
                        dtYear.Rows.Add(groupKD.Key, intLvl[l], sumYear);

                        var sumMonth = GetSumFieldValue(tblMonthRep, SUM, fltMonth);
                        dtMonth.Rows.Add(groupKD.Key, intLvl[l], month, sumMonth);
                    }
                }

                rep.ProcessDataRows(dtYear.Select(), captionColumn.Index + 1 + i);
                rep.ProcessDataRows(dtMonth.Select(), captionColumn.Index + 1 + i);
            }

            tablesResult[0] = rep.GetReportData();

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.YEARS, ConvertToString(years));
            paramHelper.SetParamValue(ParamUFKHelper.MONTH, month);
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
