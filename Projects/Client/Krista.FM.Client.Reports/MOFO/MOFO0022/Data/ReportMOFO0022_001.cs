using System;
using System.Collections.Generic;
using System.Data;
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
        /// ОТЧЕТ 001 ОТЧЕТ О ЗАДОЛЖЕННОСТИ ПО АРЕНДНОЙ ПЛАТЕ НА ЗЕМЛЮ
        /// </summary>
        public DataTable[] GetMOFO0022Report001Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramRegion = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamRegionComparable]);
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;
            var paramMark = reportParams[ReportConsts.ParamMark];
            var contractType = GetEnumItemValue(new MOFOContractTypeEnum(), paramMark);
            var paramHideEmptyStr = Convert.ToBoolean(reportParams[ReportConsts.ParamHideEmptyStr]);

            var filterMarks = ReportMOFO0022Helper.GetContractTypeCodes((ContractType)contractType);

            // кварталы
            var quarters = new []
            {
                GetUNVYearQuarter(year, 1),
                GetUNVYearQuarter(year, 2),
                GetUNVYearQuarter(year, 3),
                GetUNVYearQuarter(year, 4)
            };

            // уровни
            var levels = new []
            {
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Municipal),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Settle)
            };

            // фильтры фактов
            var filterList = new List<QFilter>
            {
                new QFilter(QRent.Keys.Day, ConvertToString(quarters)),
                new QFilter(QRent.Keys.Org, ReportMOFO0022Helper.FixOrgRef),
                new QFilter(QRent.Keys.Lvl, String.Join(",", levels)),
                new QFilter(QRent.Keys.Mark, filterMarks),
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
            const string regionsTableKey = b_Regions_Bridge.InternalKey;

            // настраиваем колонки отчета
            const string terTypeTableKey = fx_FX_TerritorialPartitionType.InternalKey;
            var masks = new AteOutMasks(new TableField(regionsTableKey, b_Regions_Bridge.CodeLine));
            rep.AddCaptionColumn().SetMasks(ateGrouping, masks);
            masks = new AteOutMasks(new TableField(terTypeTableKey, fx_FX_TerritorialPartitionType.Name));
            rep.AddCaptionColumn().SetMasks(ateGrouping, masks);
            masks = new AteOutMasks(new TableField(regionsTableKey, b_Regions_Bridge.Name));
            rep.AddCaptionColumn().SetMasks(ateGrouping, masks);

            ReportCalcColumn.Calculate sumFunction =
                (row, index) =>
                Functions.Sum(new[] {row.Values[index + 1], row.Values[index + 2], row.Values[index + 3]});

            foreach (var quarter in quarters)
            {
                var fltQuarter = filterHelper.EqualIntFilter(f_F_Rent.RefYearDayUNV, quarter);
                rep.AddCalcColumn(sumFunction);

                foreach (var level in levels)
                {
                    var fltLevel = filterHelper.RangeFilter(f_F_Rent.RefBudgetLevels, level);
                    rep.AddValueColumn(SUM, String.Format("{0} and {1}", fltQuarter, fltLevel));
                }
            }

            rep.ProcessTable(tblData);
            tablesResult[0] = rep.GetReportData();

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            var contract = GetEnumItemDescription(new MOFOContractTypeEnum(), paramMark).ToUpper();
            paramHelper.SetParamValue(ParamUFKHelper.YEARS, year);
            paramHelper.SetParamValue(ParamUFKHelper.CONTRACT, contract);
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
