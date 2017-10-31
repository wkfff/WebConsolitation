using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.Database.FactTables.MOFO;
using Krista.FM.Client.Reports.MOFO.Queries;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK.ReportMaster;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{

    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 001 ОТЧЕТ О ПРОГНОЗИРУЕМЫХ НАЧИСЛЕНИЯХ ЗЕМЕЛЬНОГО НАЛОГА НА ОЧЕРЕДНОЙ ФИНАНСОВЫЙ ГОД И ПЛАНОВЫЙ ПЕРИОД
        /// </summary>
        public DataTable[] GetMOFO0025Report001Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramRegion = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamRegionComparable]);
            var paramHideEmptyStr = Convert.ToBoolean(reportParams[ReportConsts.ParamHideEmptyStr]);
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;
            var fltYear = filterHelper.EqualIntFilter(f_Marks_LandTax.RefYearDayUNV, GetUNVYearPlanLoBound(year));
            var marks = new List<int>{0, 1, 2, 3, 4, 5, 6};

            // получаем данные из т.ф. МОФО_Начисления земельного налога (f.Marks.LandTax)
            var filterList = new List<QFilter>
            {
                new QFilter(QFMarksLandTax.Keys.Day, GetUNVYearPlanLoBound(year)),
                new QFilter(QFMarksLandTax.Keys.Okato, paramRegion),
                new QFilter(QFMarksLandTax.Keys.Mark, ConvertToString(marks))
            };

            var groupFields = new List<Enum>
            {
                QFMarksLandTax.Keys.Okato,
                QFMarksLandTax.Keys.Mark,
                QFMarksLandTax.Keys.Day
            };
            var query = new QFMarksLandTax().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(query);

            // устанавливаем параметры отчета
            var rep = new Report(f_Marks_LandTax.TableKey)
            {
                Divider = GetDividerValue(divider),
                RowFilter = paramHideEmptyStr ? Functions.IsNotNullRow : (RowViewParams.Function)null
            };

            // группировка по АТЕ
            var ateGrouping = new AteGrouping(0);
            rep.AddGrouping(d_Regions_Plan.RefBridge, ateGrouping);
            var fixedId = paramHideEmptyStr
                              ? ateGrouping.AteMainId
                              : paramRegion != String.Empty
                                    ? Combine(paramRegion, ateGrouping.AteMainId)
                                    : ateGrouping.GetRegionsId();
            ateGrouping.SetFixedValues(fixedId);

            // настраиваем колонки отчета
            const string regionsTableKey = b_Regions_Bridge.InternalKey;
            const string terTypeTableKey = fx_FX_TerritorialPartitionType.InternalKey;
            var masks = new AteOutMasks(new TableField(regionsTableKey, b_Regions_Bridge.CodeLine));
            rep.AddCaptionColumn().SetMasks(ateGrouping, masks);
            masks = new AteOutMasks(new TableField(terTypeTableKey, fx_FX_TerritorialPartitionType.Name));
            rep.AddCaptionColumn().SetMasks(ateGrouping, masks);
            masks = new AteOutMasks(new TableField(regionsTableKey, b_Regions_Bridge.Name));
            var nameColumn = rep.AddCaptionColumn();
            nameColumn.SetMasks(ateGrouping, masks);

            foreach (var mark in marks)
            {
                var fltMark = filterHelper.EqualIntFilter(f_Marks_LandTax.RefMarks, mark);
                var rateColumn = rep.AddValueColumn(f_Marks_LandTax.Value, CombineAnd(fltYear, fltMark));
                if (mark == 0)
                {
                    rateColumn.Divider = 1;
                }
            }

            rep.ProcessTable(tblData);
            tablesResult[0] = rep.GetReportData();

            // заполняем таблицу параметров
            var paramHelper = new ParamUFKHelper(CreateReportParamsRow(tablesResult));
            paramHelper.SetParamValue(ParamUFKHelper.YEAR, year);
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
