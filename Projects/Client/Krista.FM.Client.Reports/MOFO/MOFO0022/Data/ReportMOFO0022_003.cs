﻿using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsData.MOFO;
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
        /// ОТЧЕТ 003 ОТЧЕТ О ЗАДОЛЖЕННОСТИ В МЕСТНЫЙ БЮДЖЕТ ПО АРЕНДНОЙ ПЛАТЕ НА ИМУЩЕСТВО ПО КРУПНЕЙШИМ ПРЕДПРИЯТИЯМ - НЕДОИМЩИКАМ
        /// </summary>
        public DataTable[] GetMOFO0022Report003Data(Dictionary<string, string> reportParams)
        {
            const int ateFirstStyle = 1;
            
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[2];
            var paramRegion = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamRegionComparable]);
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
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
                new QFilter(QRent.Keys.Lvl, String.Join(",", levels)),
                new QFilter(QRent.Keys.Mark, filterMark),
                new QFilter(QRent.Keys.Okato, paramRegion)
            };

            // устанавливаем параметры отчета
            var rep = new Report(f_F_Rent.TableKey)
            {
                Divider = GetDividerValue(divider),
                RowFilter = paramHideEmptyStr ? Functions.IsNotNullRow : (RowViewParams.Function) null
            };

            // группировка по АТЕ
            var ateGrouping = new MOFO0022AteGrouping(ateFirstStyle);
            rep.AddGrouping(d_Regions_Plan.RefBridge, ateGrouping);
            var fixedId = paramHideEmptyStr
                  ? ateGrouping.AteMainId
                  : paramRegion != String.Empty
                        ? Combine(paramRegion, ateGrouping.AteMainId)
                        : ateGrouping.GetRegionsId();
            ateGrouping.SetFixedValues(fixedId);

            // группировка по организации
            const string orgTableKey = d_Organizations_Rent.TableKey;
            var orgGrouping = rep.AddGrouping(f_F_Rent.RefOrganizations);
            orgGrouping.AddLookupField(orgTableKey, d_Organizations_Rent.INN, d_Organizations_Rent.Name);
            orgGrouping.AddSortField(orgTableKey, d_Organizations_Rent.INN);
            orgGrouping.ViewParams[0].Style = 0;

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
            nameColumn.SetMask(ateGrouping, ateGrouping.LastLevel, MOFO0022AteGrouping.Title);
            nameColumn.SetMask(orgGrouping, 0, orgTableKey, d_Organizations_Rent.INN, d_Organizations_Rent.Name);

            foreach (var quarter in quarters)
            {
                var fltQuarter = filterHelper.EqualIntFilter(f_F_Rent.RefYearDayUNV, quarter);
                rep.AddValueColumn(SUM, fltQuarter);
            }

            // получаем данные по АТЕ (f_F_Rent.RefOrganizations = -1)
            const string filterOrg = ReportMOFO0022Helper.FixOrgRef;
            filterList.Add(new QFilter(QRent.Keys.Org, filterOrg));
            var groupFields = new List<Enum> { QRent.Keys.Day, QRent.Keys.Okato };
            var query = new QRent().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(query);
            rep.ProcessTable(tblData);

            // получаем данные по организациям (f_F_Rent.RefOrganizations != -1)
            filterList[filterList.Count - 1] = new QFilter(QRent.Keys.Org, filterOrg, true);
            var groupFieldsOrg = new List<Enum> { QRent.Keys.Day, QRent.Keys.Okato, QRent.Keys.Org };
            var queryOrg = new QRent().GetQueryText(filterList, groupFieldsOrg);
            var tblDataOrg = dbHelper.GetTableData(queryOrg);
            rep.ProcessTable(tblDataOrg);

            tablesResult[0] = rep.GetReportData();

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
