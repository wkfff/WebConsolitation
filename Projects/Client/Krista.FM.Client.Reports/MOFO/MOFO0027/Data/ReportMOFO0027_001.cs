using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsData.MOFO;
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
        /// ОТЧЕТ 001 ОТЧЕТ ПО ПОКАЗАТЕЛЯМ ДЕЯТЕЛЬНОСТИ МУНИЦИПАЛЬНЫХ УНИТАРНЫХ ПРЕДПРИЯТИЙ
        /// </summary>
        public DataTable[] GetMOFO0027Report001Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramMarks = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMark]);
            var paramRegion = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamRegionComparable]);
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;
            var paramHideEmptyStr = Convert.ToBoolean(reportParams[ReportConsts.ParamHideEmptyStr]);
            var paramQuarter = Convert.ToBoolean(reportParams[ReportConsts.ParamQuarter]);
            var paramShowOrg = Convert.ToBoolean(reportParams[ReportConsts.ParamOutputMode]);

            // периоды
            var periods = paramQuarter
                              ? new List<string>
                              {
                                  GetUNVYearQuarter(year, 1),
                                  GetUNVYearQuarter(year, 2),
                                  GetUNVYearQuarter(year, 3),
                                  GetUNVYearQuarter(year, 4),
                                  GetUNVYearPlanLoBound(year)
                              }
                              : new List<string> {GetUNVYearPlanLoBound(year)};
            var periodFilters = (from period in periods
                                 select filterHelper.EqualIntFilter(f_F_IndexProfit.RefYearDayUNV, period)).ToList();
            var yearFilter = new List<string> {periodFilters.Last()};

            var fields = new Dictionary<string, List<string>>
            {
                {f_F_IndexProfit.SaleProfit,        periodFilters},
                {f_F_IndexProfit.BaseProfit,        periodFilters},
                {f_F_IndexProfit.AddProfit,         periodFilters},
                {f_F_IndexProfit.TransferProfit,    periodFilters},
                {f_F_IndexProfit.OldTransferProfit, yearFilter},
                {f_F_IndexProfit.TotalTransfer,     yearFilter},
                {f_F_IndexProfit.DebtProfit,        periodFilters},
                {f_F_IndexProfit.OldDebtProfit,     yearFilter},
                {f_F_IndexProfit.TotalDebt,         yearFilter}
            } ;

            // получаем данные из т.ф. МОФО_Показатели деятельности МУП (f.F.IndexProfit)
            var filterList = new List<QFilter>
            {
                new QFilter(QFIndexProfit.Keys.Day, ConvertToString(periods)),
                new QFilter(QFIndexProfit.Keys.Mark, paramMarks),
                new QFilter(QFIndexProfit.Keys.Okato, paramRegion)
            };

            var groupFields = new List<Enum> {QFIndexProfit.Keys.Okato, QFIndexProfit.Keys.Day};
            if (paramShowOrg)
            {
                groupFields.Add(QFIndexProfit.Keys.Org);
            }

            var query = new QFIndexProfit().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(query);

            // устанавливаем параметры отчета
            var rep = new Report(f_F_IndexProfit.TableKey)
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

            // группировка по организации
            const string orgTableKey = d_Org_IndexProfit.InternalKey;
            var orgGrouping = rep.AddGrouping(f_F_IndexProfit.RefOrg);
            orgGrouping.AddLookupField(orgTableKey, d_Org_IndexProfit.INN, d_Org_IndexProfit.Name);
            orgGrouping.AddSortField(orgTableKey, d_Org_IndexProfit.INN);
            orgGrouping.ViewParams[0].Style = ateGrouping.LastLevel + 1;
            orgGrouping.ViewParams[0].Filter = paramHideEmptyStr
                                                   ? (RowViewParams.Function)Functions.IsNotNullRow
                                                   : Functions.IsNotUnknownKey;
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
            nameColumn.SetMask(orgGrouping, 0, orgTableKey, d_Org_IndexProfit.INN, d_Org_IndexProfit.Name);

            foreach (var field in fields)
            {
                foreach (var periodFilter in field.Value)
                {
                    rep.AddValueColumn(field.Key, periodFilter);
                }
            }

            rep.ProcessTable(tblData);

            tablesResult[0] = rep.GetReportData();
            // заполняем таблицу параметров
            var mark = ReportMonthMethods.GetSelectedActivity(paramMarks);
            var paramHelper = new ParamUFKHelper(CreateReportParamsRow(tablesResult));
            paramHelper.SetParamValue(ParamUFKHelper.YEAR, year);
            paramHelper.SetParamValue(ParamUFKHelper.MARK, mark);
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
