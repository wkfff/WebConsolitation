using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.Database.FactTables.MOFO;
using Krista.FM.Client.Reports.MOFO.Queries;
using Krista.FM.Client.Reports.MOFO0022.Helpers;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK.ReportMaster;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{

    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 004 ОТЧЕТ О ЗАДОЛЖЕННОСТИ В МЕСТНЫЙ БЮДЖЕТ ПО АРЕНДНОЙ ПЛАТЕ НА ИМУЩЕСТВО
        /// </summary>
        public DataTable[] GetMOFO0022Report004Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramRegion = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamRegionComparable]);
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;
            var paramHideEmptyStr = Convert.ToBoolean(reportParams[ReportConsts.ParamHideEmptyStr]);
            var filterMark = Convert.ToString(ReportMOFO0022Helper.AssetRentIncome);

            // кварталы
            var quarters = new[]
            {
                GetUNVYearQuarter(year, 1),
                GetUNVYearQuarter(year, 2),
                GetUNVYearQuarter(year, 3),
                GetUNVYearQuarter(year, 4)
            };

            // уровни
            var levels = new[]
            {
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Municipal),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Settle)
            };

            // фильтры фактов
            var filterList = new List<QFilter>
            {
                new QFilter(QRent.Keys.Day, ConvertToString(quarters)),
                new QFilter(QRent.Keys.Org, ReportMOFO0022Helper.FixOrgRef),
                new QFilter(QRent.Keys.Lvl, String.Join(",", levels)),
                new QFilter(QRent.Keys.Mark, filterMark),
                new QFilter(QRent.Keys.Okato, paramRegion)
            };

            var groupFields = new List<Enum> { QRent.Keys.Day, QRent.Keys.Lvl, QRent.Keys.Okato };
            var query = new QRent().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(query);


            // устанавливаем параметры отчета
            var rep = new Report(f_F_Rent.TableKey)
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
            rep.AddCaptionColumn().SetMasks(ateGrouping, masks);

            foreach (var quarter in quarters)
            {
                var fltQuarter = filterHelper.EqualIntFilter(f_F_Rent.RefYearDayUNV, quarter);
                rep.AddValueColumn(SUM, fltQuarter);
                rep.AddCalcColumn();
            }

            rep.ProcessTable(tblData);
            var dt = rep.GetReportData();

            // вычисляем проценты
            if (dt.Rows.Count > 0)
            {
                var totalRow = dt.Rows[dt.Rows.Count - 1];
                var indexies = from column in rep.Columns
                               where column.ColumnType == ReportColumnType.Value
                               select column.Index;

                foreach (var i in indexies)
                {
                    var sum = GetDecimal(totalRow[i]);
                    if (sum == 0)
                    {
                        continue;
                    }

                    foreach (var row in dt.Rows.Cast<DataRow>().Where(row => row[i] != DBNull.Value))
                    {
                        row[i + 1] = GetDecimal(row[i])/sum*100;
                    }
                }
            }

            tablesResult[0] = dt;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.YEARS, year);
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
