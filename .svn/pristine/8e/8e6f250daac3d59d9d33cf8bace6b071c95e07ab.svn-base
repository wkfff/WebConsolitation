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
        /// ОТЧЕТ 015 ПОСТУПЛЕНИЯ В БЮДЖЕТ МОСКОВСКОЙ ОБЛАСТИ И ВОЗВРАТЫ ИЗ БЮДЖЕТА МОСКОВСКОЙ ОБЛАСТИ ПО ПЛАТЕЛЬЩИКАМ И ВИДУ ДОХОДОВ
        /// </summary>
        public DataTable[] GetUFKReport015(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[3];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var filterKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var paramStartDate = Convert.ToDateTime(reportParams[ReportConsts.ParamStartDate]);
            var paramEndDate = Convert.ToDateTime(reportParams[ReportConsts.ParamEndDate]);
            var limitSum = Convert.ToDecimal(reportParams[ReportConsts.ParamSum]) * ReportUFKHelper.LimitSumRate;
            var paramLvl = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamBdgtLevels]);

            // уровни
            var levels = new List<string>
            {
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Federal),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsSubjectFractional),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsMunicipalFractional),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Municipal),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Settle),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Fonds),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.PurposeFonds)
            };

            var intParamLvl = paramLvl != String.Empty ? ConvertToIntList(paramLvl) : GetColumnsList(0, levels.Count);
            levels = levels.Where((t, i) => intParamLvl.Contains(i)).ToList();

            // периоды
            if (paramStartDate.Year != paramEndDate.Year)
            {
                paramStartDate = new DateTime(paramEndDate.Year, 1, 1);
            }
            var startDate = GetUNVDate(paramStartDate);
            var endDate = GetUNVDate(paramEndDate);
            var prevStartDate = GetUNVDate(paramStartDate.AddYears(-1));
            var prevEndDate = GetUNVDate(paramEndDate.AddYears(-1));
            var prevYearStart = GetUNVDate(GetYearStart(paramEndDate.Year - 1));
            var prevYearEnd = GetUNVDate(GetYearEnd(paramEndDate.Year - 1));
            var periods = new[]
            {
                filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV, prevStartDate, prevEndDate, true),
                filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV, prevYearStart, prevYearEnd, true),
                filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV, startDate, endDate, true)
            };

            // фильтры фактов
            var filterList = new List<QFilter>
            {
                new QFilter(QUFK14.Keys.Period, String.Empty), // зарезервировано
                new QFilter(QUFK14.Keys.Lvl, GetDistinctCodes(ConvertToString(levels))),
                new QFilter(QUFK14.Keys.KD, filterKD)
            };

            var filterListAllOrg = new List<QFilter>(filterList);
            var groupFields = new List<Enum> { QUFK14.Keys.Org };

            // выбираем организации с суммой платежей за год свыше лимита
            if (limitSum != 0)
            {
                var orgList = new List<int>();
                var strSum = Convert.ToString(limitSum, CultureInfo.InvariantCulture);

                foreach (var period in periods.Where((p, i) => i > 0)) // только большие периоды
                {
                    filterList[0] = new QFilter(QUFK14.Keys.Period, period);
                    var havingFilter = filterHelper.MoreEqualFilter(QFactTable.Having(f_D_UFK14.Credit), strSum);
                    var queryOrg = new QUFK14().GetQueryText(filterList, groupFields, havingFilter);
                    var tblPayerData = dbHelper.GetTableData(queryOrg);
                    orgList.AddRange(from DataRow row in tblPayerData.Rows
                                     select Convert.ToInt32(row[d_Org_UFKPayers.RefOrgPayersBridge]));
                }

                orgList = orgList.Distinct().ToList();
                var fltPayers = orgList.Count > 0 ? ConvertToString(orgList) : ReportConsts.UndefinedKey;
                filterList.Add(new QFilter(QUFK14.Keys.Org, fltPayers));
            }

            var rep = new Report(f_D_UFK14.InternalKey) {AddNumColumn = true, Divider = GetDividerValue(divider)};

            // группировка по организациям
            const string orgTableKey = b_Org_PayersBridge.InternalKey;
            var grColumn = rep.AddGrouping(d_Org_UFKPayers.RefOrgPayersBridge);
            grColumn.AddLookupField(orgTableKey, b_Org_PayersBridge.INN, b_Org_PayersBridge.Name);
            grColumn.AddSortField(orgTableKey, b_Org_PayersBridge.INN);

            rep.AddCaptionColumn(grColumn, orgTableKey, b_Org_PayersBridge.INN);
            var nameColumn = rep.AddCaptionColumn(grColumn, orgTableKey, b_Org_PayersBridge.Name);
            var firstSumColumn = nameColumn.Index + 1;

            // получаем детальные данные
            foreach (var period in periods)
            {
                rep.AddValueColumn(f_D_UFK14.Credit);
            }

            for (var i = 0; i < periods.Length; i++)
            {
                filterList[0].Filter = periods[i];
                var query = new QUFK14().GetQueryText(filterList, groupFields);
                var tblData = dbHelper.GetTableData(query);
                rep.ProcessDataRows(tblData.Select(), firstSumColumn + i);
            }

            var mainOrgDt = rep.GetReportData();

            // получаем данные по всем организациям (включая мелкие)
            var allOrgDt = new DataTable();
            AddColumnToReport(allOrgDt, typeof (decimal), "column", periods.Length);
            var allOrgRow = allOrgDt.Rows.Add();

            for (var i = 0; i < periods.Length; i++)
            {
                filterListAllOrg[0].Filter = periods[i];
                var query = new QUFK14().GetQueryText(filterListAllOrg);
                var tblData = dbHelper.GetTableData(query);
                if (tblData.Rows.Count > 0)
                {
                    allOrgRow[i] = tblData.Rows[0][f_D_UFK14.Credit];
                }
            }

            // копируем итоги по крупным организациям
            DivideSum(allOrgDt, 0, periods.Length, divider);
            if (mainOrgDt.Rows.Count > 0)
            {
                var mainOrgRow = mainOrgDt.Rows[mainOrgDt.Rows.Count - 1];
                var row = allOrgDt.Rows.Add();

                for (var i = 0; i < periods.Length; i++)
                {
                    row[i] = mainOrgRow[firstSumColumn + i + 1]; // +1 т.к. добавилась нумерация строк
                }

                mainOrgDt.Rows.Remove(mainOrgRow);
            }

            tablesResult[0] = mainOrgDt;
            tablesResult[1] = allOrgDt;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            var bdgtName = paramLvl != String.Empty
                               ? ReportMonthMethods.GetSelectedBudgetLvlFull(paramLvl)
                               : String.Empty;
            paramHelper.SetParamValue(ParamUFKHelper.STARTDATE, paramStartDate);
            paramHelper.SetParamValue(ParamUFKHelper.ENDDATE, paramEndDate);
            paramHelper.SetParamValue(ParamUFKHelper.BDGT_NAME, bdgtName);
            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(filterKD));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            paramHelper.SetParamValue(ParamUFKHelper.SUMM, reportParams[ReportConsts.ParamSum]);
            return tablesResult;
        }
    }
}
