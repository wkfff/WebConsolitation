using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsData.MOFO;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.Database.FactTables.MOFO;
using Krista.FM.Client.Reports.MOFO.Queries;
using Krista.FM.Client.Reports.UFK.ReportMaster;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{

    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 001 ОТЧЕТ ПО ПОКАЗАТЕЛЯМ ДЕЯТЕЛЬНОСТИ АКЦИОНЕРНЫХ ОБЩЕСТВ
        /// </summary>
        public DataTable[] GetMOFO0026Report001Data(Dictionary<string, string> reportParams)
        {
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramRegion = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamRegionComparable]);
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;
            var paramHideEmptyStr = Convert.ToBoolean(reportParams[ReportConsts.ParamHideEmptyStr]);
            var paramShowOrg = Convert.ToBoolean(reportParams[ReportConsts.ParamOutputMode]);

            // получаем данные из т.ф. МОФО_Показатели деятельности АО (f.F.IndexCapital)

            var filterList = new List<QFilter>
            {
                new QFilter(QFIndexCapital.Keys.Day, GetUNVYearPlanLoBound(year)),
                new QFilter(QFIndexCapital.Keys.Okato, paramRegion)
            };

            var groupFields = paramShowOrg
                                  ? new List<Enum> {QFIndexCapital.Keys.Okato, QFIndexCapital.Keys.Org}
                                  : new List<Enum> {QFIndexCapital.Keys.Okato};
            var query = new QFIndexCapital().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(query);

            // устанавливаем параметры отчета
            var rep = new Report(f_F_IndexCapital.TableKey)
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
            const string orgTableKey = d_Org_IndexCapital.TableKey;
            var orgGrouping = rep.AddGrouping(f_F_IndexCapital.RefOrg);
            orgGrouping.AddLookupField(orgTableKey, d_Org_IndexCapital.INN, d_Org_IndexCapital.Name);
            orgGrouping.AddSortField(orgTableKey, d_Org_IndexCapital.INN);
            orgGrouping.ViewParams[0].Style = ateGrouping.LastLevel + 1;
            orgGrouping.ViewParams[0].Filter = paramHideEmptyStr
                                                   ? (RowViewParams.Function) Functions.IsNotNullRow
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
            nameColumn.SetMask(orgGrouping, 0, orgTableKey, d_Org_IndexCapital.INN, d_Org_IndexCapital.Name);

            rep.AddValueColumn(f_F_IndexCapital.Capital);
            rep.AddValueColumn(f_F_IndexCapital.ClearAsset);
            rep.AddValueColumn(f_F_IndexCapital.ClearProfit);
            rep.AddValueColumn(f_F_IndexCapital.DeductResFund);
            rep.AddValueColumn(f_F_IndexCapital.DeductSpecFund);
            rep.AddValueColumn(f_F_IndexCapital.AddDividend);
            rep.AddValueColumn(f_F_IndexCapital.TransferDividend);
            rep.AddValueColumn(f_F_IndexCapital.DebtDividend);
            rep.ProcessTable(tblData);

            tablesResult[0] = rep.GetReportData();

            // заполняем таблицу параметров
            var paramHelper = new ParamUFKHelper(CreateReportParamsRow(tablesResult));
            paramHelper.SetParamValue(ParamUFKHelper.YEARS, year);
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
