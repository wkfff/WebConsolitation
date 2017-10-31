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
        /// ОТЧЕТ 007 АНАЛИЗ ПОСТУПЛЕНИЯ ПО НДФЛ И НП ПО СТРУКТУРАМ ЗА ДВА ПРОШЛЫХ ГОДА И ТЕКУЩИЙ ГО
        /// </summary>
        public DataTable[] GetUFKReport007(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var lastYear = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;
            var years = new List<int> {lastYear - 2, lastYear - 1, lastYear};

            // периоды
            var periods = new List<string>();
            foreach (var year in years)
            {
                var janStart = GetUNVMonthStart(year, 1);
                var junEnd = GetUNVMonthEnd(year, 6);
                var julStart = GetUNVMonthStart(year, 7);
                var decEnd = GetUNVMonthEnd(year, 12);
                periods.Add(filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV, janStart, junEnd, true));
                periods.Add(filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV, julStart, decEnd, true));
            }

            // параметры отчета
            var rep = new Report(f_D_UFK14.InternalKey) {Divider = GetDividerValue(divider)};

            // группировка по АТЕ
            var ateGrouping = rep.AddGrouping(d_OKATO_UFK.RefRegionsBridge,
                                              new AteGrouping(0) {HideConsBudjetRow = true});
            // настраиваем колонки отчета
            var captionColumn = rep.AddCaptionColumn();
            captionColumn.SetMasks(ateGrouping, new AteOutMasks());

            foreach (var period in periods)
            {
                rep.AddValueColumn(f_D_UFK14.Credit);
                rep.AddValueColumn(f_D_UFK14.Credit);
                rep.AddValueColumn(f_D_UFK14.Credit);
            }

            // получаем данные из т.ф.
            var filterList = new List<QFilter>
            {
                new QFilter(QUFK14.Keys.Period, String.Empty), // зарезервировано
                new QFilter(QUFK14.Keys.KD, paramKD)
            };

            var groupFields = new List<Enum> { QUFK14.Keys.Okato, QUFK14.Keys.Struc };
            var column = captionColumn.Index;
            var filterMO = filterHelper.EqualIntFilter(d_Org_UFKPayers.RefFX, ReportUFKHelper.StructureMO);
            var filterNotMO = filterHelper.EqualIntFilter(d_Org_UFKPayers.RefFX, ReportUFKHelper.StructureNotMO);

            foreach (var period in periods)
            {
                filterList[0] = new QFilter(QUFK14.Keys.Period, period);
                var query = new QUFK14().GetQueryText(filterList, groupFields);
                var tblData = dbHelper.GetTableData(query);
                rep.ProcessDataRows(tblData.Select(filterNotMO), ++column);
                rep.ProcessDataRows(tblData.Select(filterMO), ++column);
                rep.ProcessDataRows(tblData.Select(), ++column);
            }

            tablesResult[0] = rep.GetReportData();

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.YEARS, lastYear);
            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(paramKD));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
