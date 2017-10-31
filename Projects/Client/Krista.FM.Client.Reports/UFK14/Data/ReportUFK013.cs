using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
        /// ОТЧЕТ 013 ПОСТУПЛЕНИЕ ДОХОДНОГО ИСТОЧНИКА В КБС (ОБ, МБ) ПО ОПРЕДЕЛЕННОМУ НАЛОГОПЛАТЕЛЬЩИКУ И ПО ОПРЕДЕЛЕННОМУ МУНИЦИПАЛЬНОМУ ОБРАЗОВАНИЮ
        /// </summary>
        public DataTable[] GetUFKReport013(Dictionary<string, string> reportParams)
        {
            const int yearsCount = 3;
            const int styleOrg = 5;
            const int styleDate = 6;
            const int styleKd = 7;
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramRegion = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamRegionComparable]);
            var paramOrg = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamOrgID]);
            var paramLvl = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamBdgtLevels]);
            var paramKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var limitSum = Convert.ToDecimal(reportParams[ReportConsts.ParamSum]) * ReportUFKHelper.LimitSumRate;
            var endDate = Convert.ToDateTime(reportParams[ReportConsts.ParamEndDate]);
            var startDate = Convert.ToDateTime(reportParams[ReportConsts.ParamStartDate]);
            if (startDate.Year != endDate.Year)
            {
                startDate = new DateTime(endDate.Year, 1, 1);
            }

            // уровни
            var levels = new List<string>
            {
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsSubjectFractional),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Municipal),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Settle)
            };

            var intParamLvl = paramLvl != String.Empty ? ConvertToIntList(paramLvl) : GetColumnsList(0, levels.Count);
            intParamLvl.Sort();
            levels = levels.Where((t, i) => intParamLvl.Contains(i)).ToList();

            // периоды выборки
            var periods = new List<string>();

            for (var i = yearsCount - 1; i >= 0; i--)
            {
                periods.Add(filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV, GetUNVDate(startDate.AddYears(-i)),
                                                       GetUNVDate(endDate.AddYears(-i)), true));
            }

            // получаем данные из т.ф. «УФК_Выписка из сводного реестра_c расщеплением» (f.D.UFK14)
            var filterList = new List<QFilter>
            {
                new QFilter(QUFK14.Keys.Period, String.Empty), // зарезервиовано
                new QFilter(QUFK14.Keys.Lvl, GetDistinctCodes(ConvertToString(levels))),
                new QFilter(QUFK14.Keys.KD, paramKD),
                new QFilter(QUFK14.Keys.Okato, paramRegion),
                new QFilter(QUFK14.Keys.Org, paramOrg)
            };

            if (limitSum != 0)
            {
                // выбираем организации с суммой платежей за год свыше лимита
                var groupOrg = new List<Enum> { QUFK14.Keys.Org };
                var strSum = Convert.ToString(limitSum, CultureInfo.InvariantCulture);
                var havingFilter = filterHelper.MoreEqualFilter(QFactTable.Having(f_D_UFK14.Credit), strSum);
                var orgList = new List<int>();

                foreach (var period in periods)
                {
                    filterList[0] = new QFilter(QUFK14.Keys.Period, period);
                    var queryOrg = new QUFK14().GetQueryText(filterList, groupOrg, havingFilter);
                    var tblOrg = dbHelper.GetTableData(queryOrg);
                    orgList.AddRange(from DataRow row in tblOrg.Rows
                                     select Convert.ToInt32(row[d_Org_UFKPayers.RefOrgPayersBridge]));
                }

                orgList = orgList.Distinct().ToList();
                var fltOrg = orgList.Count > 0 ? ConvertToString(orgList) : ReportConsts.UndefinedKey;
                filterList[filterList.Count - 1] = new QFilter(QUFK14.Keys.Org, fltOrg);
            }

            var groupFields = new List<Enum>
            {
                QUFK14.Keys.Okato,
                QUFK14.Keys.Org,
                QUFK14.Keys.Day,
                QUFK14.Keys.KD,
                QUFK14.Keys.Lvl
            };

            // параметры отчета
            var rep = new Report(f_D_UFK14.InternalKey) { Divider = GetDividerValue(divider) };

            // группировка по АТЕ
            var ateGrouping = rep.AddGrouping(d_OKATO_UFK.RefRegionsBridge,
                                              new AteGrouping(0) { HideConsBudjetRow = true });
            // группировка по организациям
            const string orgTableKey = b_Org_PayersBridge.InternalKey;
            var orgGrouping = rep.AddGrouping(d_Org_UFKPayers.RefOrgPayersBridge);
            orgGrouping.AddLookupField(orgTableKey, b_Org_PayersBridge.INN, b_Org_PayersBridge.Name);
            orgGrouping.AddSortField(orgTableKey, b_Org_PayersBridge.INN);
            orgGrouping.ViewParams[0].Style = styleOrg;
            // группировка по дате
            var dateGrouping = rep.AddGrouping(f_D_UFK14.RefYearDayUNV);
            dateGrouping.AddSortField(f_D_UFK14.InternalKey, f_D_UFK14.RefYearDayUNV);
            dateGrouping.ViewParams[0].Style = styleDate;
            // группировка по КД
            const string kdTableKey = b_KD_Bridge.InternalKey;
            var kdGrouping = rep.AddGrouping(d_KD_UFK.RefKDBridge);
            kdGrouping.AddLookupField(kdTableKey, b_KD_Bridge.CodeStr, b_KD_Bridge.Name);
            kdGrouping.AddSortField(kdTableKey, b_KD_Bridge.CodeStr);
            kdGrouping.ViewParams[0].Style = styleKd;

            // сортируем и настраиваем колонки отчета
            var captionColumn = rep.AddCaptionColumn();
            captionColumn.SetMasks(ateGrouping, new AteOutMasks());
            captionColumn.SetMask(orgGrouping, 0, orgTableKey, b_Org_PayersBridge.INN, b_Org_PayersBridge.Name);
            captionColumn.SetMask(dateGrouping, 0, f_D_UFK14.InternalKey, f_D_UFK14.RefYearDayUNV);
            captionColumn.SetMask(kdGrouping, 0, kdTableKey, b_KD_Bridge.CodeStr, b_KD_Bridge.Name);

            for (var i = 0; i < levels.Count * yearsCount; i++)
            {
                rep.AddValueColumn(f_D_UFK14.Credit);
            }

            var column = captionColumn.Index;

            foreach (var level in levels)
            {
                filterList[1] = new QFilter(QUFK14.Keys.Lvl, level);

                foreach (var period in periods)
                {
                    filterList[0] = new QFilter(QUFK14.Keys.Period, period);
                    var queryText = new QUFK14().GetQueryText(filterList, groupFields);
                    var tblData = dbHelper.GetTableData(queryText);
                    rep.ProcessDataRows(tblData.Select(), ++column);
                }
            }

            var dt = rep.GetReportData();
            foreach (DataRow row in dt.Rows)
            {
                if (row[STYLE] != DBNull.Value && Convert.ToInt32(row[STYLE]) == styleDate)
                {
                    row[captionColumn.Index] =
                        GetNormalDate(Convert.ToString(row[captionColumn.Index])).ToShortDateString();
                }
            }

            tablesResult[0] = dt;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.STARTDATE, reportParams[ReportConsts.ParamStartDate]);
            paramHelper.SetParamValue(ParamUFKHelper.ENDDATE, reportParams[ReportConsts.ParamEndDate]);
            paramHelper.SetParamValue(ParamUFKHelper.BDGT_LEVEL, ConvertToString(intParamLvl));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            paramHelper.SetParamValue(ParamUFKHelper.SUMM, reportParams[ReportConsts.ParamSum]);
            return tablesResult;
        }
    }
}
