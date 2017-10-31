using System;
using System.Collections.Generic;
using System.Data;
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
        /// ОТЧЕТ 000 КОНТРОЛЬНЫЙ ОТЧЕТ
        /// </summary>
        public DataTable[] GetUFK22Report000Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var paramLvl = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamBdgtLevels]);
            var startDate = Convert.ToDateTime(reportParams[ReportConsts.ParamStartDate]);
            var endDate = Convert.ToDateTime(reportParams[ReportConsts.ParamEndDate]);

            // периоды
            var days = new List<string>();
            var daysCount = (endDate - startDate).Days;

            for (var i = 0; i <= daysCount; i++)
            {
                days.Add(GetUNVDate(startDate.AddDays(i)));
            }

            var fltDays = ConvertToString(days);
            var fltDaysFO = filterHelper.RangeFilter(f_D_UFK14.RefYearDayUNV, fltDays);
            var fltDaysFK = filterHelper.RangeFilter(f_D_UFK14.RefFKDay, fltDays);
            var fltDaysAll = String.Format("({0})",
                                           CombineOr(filterHelper.RangeFilter(f_D_UFK14.RefYearDayUNV, fltDays, true),
                                                     filterHelper.RangeFilter(f_D_UFK14.RefFKDay, fltDays, true)));
            // получаем данные из т.ф. f.D.UFK22 (УФК_Справка о перечисленных поступлениях в бюджет)
            var filterUFK22 = new List<QFilter>
                {
                    new QFilter(QdUFK22.Keys.Day, fltDays),
                    new QFilter(QdUFK22.Keys.KD, paramKD)
                };

            var groupUFK22 = new List<Enum> { QdUFK22.Keys.Okato, QdUFK22.Keys.Day, QdUFK22.Keys.Marks };
            var queryUFK22 = new QdUFK22().GetQueryText(filterUFK22, groupUFK22);
            var tblUFK22 = dbHelper.GetTableData(queryUFK22);

            // получаем данные по из т.ф. f.D.UFK14 (УФК_Выписка из сводного реестра с расщепления)
            var filterUFK14 = new List<QFilter>
                {
                    new QFilter(QUFK14.Keys.Period, fltDaysAll),
                    new QFilter(QUFK14.Keys.Lvl, paramLvl),
                    new QFilter(QUFK14.Keys.KD, paramKD)
                };

            var groupUFK14 = new List<Enum> { QUFK14.Keys.Okato, QUFK14.Keys.Day, QUFK14.Keys.DayFK, QUFK14.Keys.Oper };
            var queryUFK14 = new QUFK14().GetQueryText(filterUFK14, groupUFK14);
            var tblUFK14 = dbHelper.GetTableData(queryUFK14);

            // получаем данные из т.ф. f.D.UFK14dirty (УФК_Выписка из сводного реестра_без расщепления)
            var filterUFK14dirty = new List<QFilter>
                {
                    new QFilter(QUFK14dirty.Keys.DayFK, fltDays),
                    new QFilter(QUFK14dirty.Keys.KD, paramKD)
                };

            var groupUFK14dirty = new List<Enum> { QUFK14dirty.Keys.Okato, QUFK14dirty.Keys.DayFK };
            var queryUFK14dirty = new QUFK14dirty().GetQueryText(filterUFK14dirty, groupUFK14dirty);
            var tblUFK14dirty = dbHelper.GetTableData(queryUFK14dirty);

            // параметры отчета
            var rep = new Report(f_D_UFK22.InternalKey) { Divider = GetDividerValue(divider) };
            // группировка по АТЕ
            var ateGrouping = rep.AddGrouping(d_OKATO_UFK.RefRegionsBridge, new AteGrouping(0));
            // группировка по дате
            var dateGrouping = rep.AddGrouping(f_D_UFK22.RefYearDayUNV);
            dateGrouping.AddSortField(f_D_UFK22.InternalKey, f_D_UFK22.RefYearDayUNV);

            // настраиваем колонки отчета
            rep.AddCaptionColumn().SetMasks(ateGrouping, new AteOutMasks());
            var dateColumn = rep.AddCaptionColumn(dateGrouping, f_D_UFK22.InternalKey, f_D_UFK22.RefYearDayUNV);
            dateColumn.Type = typeof (int);
            dateColumn.SumNestedRows = false;
                
            rep.AddValueColumn(SUM);
            rep.AddValueColumn(SUM);
            rep.AddValueColumn(SUM);
            rep.AddValueColumn(SUM);
            rep.AddValueColumn(SUM);
            rep.AddValueColumn(f_D_UFK14dirty.Debit);
            rep.AddValueColumn(f_D_UFK14dirty.Credit);
            rep.AddValueColumn(f_D_UFK14.Credit);
            rep.AddValueColumn(f_D_UFK14.Credit);
            rep.AddValueColumn(f_D_UFK14.Credit);
            rep.AddValueColumn(f_D_UFK14.Debit);

            // обрабатываем таблицы фактов
            var fltMaskMark = filterHelper.EqualIntFilter(f_D_UFK22.RefMarks, "{0}");
            rep.ProcessDataRows(tblUFK22.Select(String.Format(fltMaskMark, 1)), dateColumn.Index + 1);
            rep.ProcessDataRows(tblUFK22.Select(String.Format(fltMaskMark, 2)), dateColumn.Index + 2);
            rep.ProcessDataRows(tblUFK22.Select(String.Format(fltMaskMark, 3)), dateColumn.Index + 3);
            rep.ProcessDataRows(tblUFK22.Select(String.Format(fltMaskMark, 4)), dateColumn.Index + 4);
            rep.ProcessDataRows(tblUFK22.Select(String.Format(fltMaskMark, 5)), dateColumn.Index + 5);

            ateGrouping.Field = new TableField(f_D_UFK14dirty.InternalKey, d_OKATO_UFK.RefRegionsBridge);
            dateGrouping.Field = new TableField(f_D_UFK14dirty.InternalKey, f_D_UFK14dirty.RefFKDay);
            rep.ProcessDataRows(tblUFK14dirty.Select(), dateColumn.Index + 6);
            rep.ProcessDataRows(tblUFK14dirty.Select(), dateColumn.Index + 7);

            ateGrouping.Field = new TableField(f_D_UFK14.InternalKey, d_OKATO_UFK.RefRegionsBridge);
            dateGrouping.Field = new TableField(f_D_UFK14.InternalKey, f_D_UFK14.RefYearDayUNV);
            var fltMaskFO = CombineAnd(filterHelper.EqualIntFilter(f_D_UFK14.RefOpertnTypes, "{0}"), fltDaysFO);
            rep.ProcessDataRows(tblUFK14.Select(String.Format(fltMaskFO, 1)), dateColumn.Index + 8);
            rep.ProcessDataRows(tblUFK14.Select(String.Format(fltMaskFO, 3)), dateColumn.Index + 9);
            rep.ProcessDataRows(tblUFK14.Select(String.Format(fltMaskFO, 2)), dateColumn.Index + 10);
            dateGrouping.Field = new TableField(f_D_UFK14.InternalKey, f_D_UFK14.RefFKDay);
            var fltMaskFK = CombineAnd(filterHelper.EqualIntFilter(f_D_UFK14.RefOpertnTypes, "{0}"), fltDaysFK);
            rep.ProcessDataRows(tblUFK14.Select(String.Format(fltMaskFK, 1)), dateColumn.Index + 11);

            dateGrouping.Field = new TableField(f_D_UFK22.InternalKey, f_D_UFK22.RefYearDayUNV);

            tablesResult[0] = rep.GetReportData();

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.STARTDATE, startDate.ToShortDateString());
            paramHelper.SetParamValue(ParamUFKHelper.ENDDATE, endDate.ToShortDateString());
            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(paramKD));
            paramHelper.SetParamValue(ParamUFKHelper.BDGT_LEVEL, reportHelper.GetBudgetLevelCaptionText(paramLvl));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
