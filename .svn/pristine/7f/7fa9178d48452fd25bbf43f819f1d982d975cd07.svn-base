using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsData.MOFO;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.Database.FactTables.MOFO;
using Krista.FM.Client.Reports.MOFO.Queries;
using Krista.FM.Client.Reports.MOFO0029.Helpers;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK.ReportMaster;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{

    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 001 ОТЧЕТ О НАЧИСЛЕННЫХ СУММАХ НАЛОГА НА ИМУЩЕСТВО ФИЗИЧЕСКИХ ЛИЦ В ОТЧЕТНОМ ГОДУ И НАЧИСЛЕНИИ НА ТЕКУЩИЙ ГОД
        /// </summary>
        public DataTable[] GetMOFO0029Report001Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramVariant = reportParams[ReportConsts.ParamVariantID];
            var paramRegion = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamRegionComparable]);
            var paramHideEmptyStr = Convert.ToBoolean(reportParams[ReportConsts.ParamHideEmptyStr]);

            var dtVariant = dbHelper.GetEntityData(d_Variant_PropertyTax.InternalKey,
                                   filterHelper.RangeFilter(d_Variant_PropertyTax.ID, paramVariant));
            var year = dtVariant.Rows.Count > 0
                ? Convert.ToInt32(dtVariant.Rows[0][d_Variant_PropertyTax.Year])
                : DateTime.Now.Year;

            var periods = new List<string>
                              {
                                  GetUNVYearPlanLoBound(year - 1),
                                  GetUNVYearPlanLoBound(year)
                              };
            var fltPrevYear = filterHelper.EqualIntFilter(f_F_PropertyTax.RefYearDayUNV, periods[0]);
            var fltYear = filterHelper.EqualIntFilter(f_F_PropertyTax.RefYearDayUNV, periods[1]);

            var filterVariant = dtVariant.Rows.Count > 0
                ? Convert.ToString(dtVariant.Rows[0][d_Variant_PropertyTax.ID])
                : ReportConsts.UndefinedKey;

            var marks = new List<int>
            {
                ReportMOFO0029Helper.MarkLess300,
                ReportMOFO0029Helper.MarkOver300Less500,
                ReportMOFO0029Helper.MarkOver500
            };

            // получаем данные из т.ф. МОФО_Начисления по НИФЛ (f.F.PropertyTax)
            var filterList = new List<QFilter>
            {
                new QFilter(QFPropertyTax.Keys.Day, ConvertToString(periods)),
                new QFilter(QFPropertyTax.Keys.Variant, filterVariant),
                new QFilter(QFPropertyTax.Keys.Okato, paramRegion),
                new QFilter(QFPropertyTax.Keys.Mark, ConvertToString(marks))
            };

            var groupFields = new List<Enum>
            {
                QFPropertyTax.Keys.Okato,
                QFPropertyTax.Keys.Mark,
                QFPropertyTax.Keys.Day
            };
            var query = new QFPropertyTax().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(query);

            // устанавливаем параметры отчета
            var rep = new Report(f_F_PropertyTax.TableKey)
            {
                Divider = GetDividerValue(divider),
                RowFilter = paramHideEmptyStr ? Functions.IsNotNullRow : (RowViewParams.Function) null
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
                var fltMark = filterHelper.EqualIntFilter(f_F_PropertyTax.RefMarks, mark);
                // ставка налога (YEAR-1)
                var rateColumn = rep.AddValueColumn(f_F_PropertyTax.RateTax, CombineAnd(fltPrevYear, fltMark));
                rateColumn.SumNestedRows = false;
                rateColumn.Divider = 1;
                // ставка налога (YEAR)
                rateColumn = rep.AddValueColumn(f_F_PropertyTax.RateTax, CombineAnd(fltYear, fltMark));
                rateColumn.SumNestedRows = false;
                rateColumn.Divider = 1;
                // сумма налога (YEAR-1)
                rep.AddValueColumn(f_F_PropertyTax.AddTax, CombineAnd(fltPrevYear, fltMark));
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
