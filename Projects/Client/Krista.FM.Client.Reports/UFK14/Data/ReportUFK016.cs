using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData.UFK;
using Krista.FM.Client.Reports.Database.ClsFx;
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
        /// ОТЧЕТ 016 ПОСТУПЛЕНИЯ В КБС, БС, КМБ И ВОЗВРАТЫ ПО ПЛАТЕЛЬЩИКАМ БЕЗ РАЗБИВКИ ПО КБК
        /// </summary>
        public DataTable[] GetUFKReport016(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var filterKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var paramStartDate = Convert.ToDateTime(reportParams[ReportConsts.ParamStartDate]);
            var paramEndDate = Convert.ToDateTime(reportParams[ReportConsts.ParamEndDate]);
            var fltOrg = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamOrgID]);
            var limitSum = Convert.ToDecimal(reportParams[ReportConsts.ParamSum]) * ReportUFKHelper.LimitSumRate;

            // уровни
            var levels = new List<string>
            {
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsSubjectFractional),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsMunicipalFractional),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsSubjectFractional)
            };

            // типы операций
            var opers = new List<string>
            {
                String.Empty,
                String.Empty,
                String.Empty,
                Convert.ToString(fx_FX_OpertnTypes.Return)
            };

            // коэффициенты
            var coefs = new List<int>{1, 1, 1, -1};

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
                new QFilter(QUFK14.Keys.Lvl, String.Empty), // зарезервировано
                new QFilter(QUFK14.Keys.Oper,  String.Empty), // зарезервировано
                new QFilter(QUFK14.Keys.KD, filterKD),
                new QFilter(QUFK14.Keys.Org, fltOrg)
            };

            var groupFields = new List<Enum> { QUFK14.Keys.Org };

            if (limitSum != 0)
            {
                // выбираем организации с суммой платежей за год свыше лимита
                var orgList = new List<int>();
                var strSum = Convert.ToString(limitSum, CultureInfo.InvariantCulture);

                foreach (var period in periods.Where((p, i) => i > 0)) // только большие периоды
                {
                    for (var i = 0; i < levels.Count; i++)
                    {
                        filterList[0].Filter = period;
                        filterList[1].Filter = levels[i];
                        filterList[2].Filter = opers[i];
                        var havingFilter = filterHelper.MoreEqualFilter(QFactTable.Having(f_D_UFK14.Credit, coefs[i]),
                                                                        strSum);
                        var queryOrg = new QUFK14().GetQueryText(filterList, groupFields, havingFilter);
                        var tblPayerData = dbHelper.GetTableData(queryOrg);
                        orgList.AddRange(from DataRow row in tblPayerData.Rows
                                         select Convert.ToInt32(row[d_Org_UFKPayers.RefOrgPayersBridge]));
                    }
                }

                orgList = orgList.Distinct().ToList();
                var fltPayers = orgList.Count > 0 ? ConvertToString(orgList) : ReportConsts.UndefinedKey;
                filterList[filterList.Count - 1] = new QFilter(QUFK14.Keys.Org, fltPayers);
            }

            // параметры отчета
            var rep = new Report(f_D_UFK14.InternalKey)
            {
                AddNumColumn = true,
                AddTotalRow = false,
                Divider = GetDividerValue(divider)
            };

            // группировка по организациям
            const string payersTableKey = b_Org_PayersBridge.InternalKey;
            var grColumn = rep.AddGrouping(d_Org_UFKPayers.RefOrgPayersBridge);
            grColumn.AddLookupField(payersTableKey, b_Org_PayersBridge.INN, b_Org_PayersBridge.Name);
            grColumn.AddSortField(payersTableKey, b_Org_PayersBridge.INN);
            grColumn.SetFixedValues(fltOrg);

            rep.AddCaptionColumn(grColumn, payersTableKey, b_Org_PayersBridge.INN);
            var nameColumn = rep.AddCaptionColumn(grColumn, payersTableKey, b_Org_PayersBridge.Name);

            foreach (var k in periods.SelectMany(period => coefs))
            {
                rep.AddValueColumn(f_D_UFK14.Credit).K = k;
            }

            var columnIndex = nameColumn.Index;

            foreach (var period in periods)
            {
                for (var i = 0; i < levels.Count; i++)
                {
                    filterList[0].Filter = period;
                    filterList[1].Filter = levels[i];
                    filterList[2].Filter = opers[i];
                    var query = new QUFK14().GetQueryText(filterList, groupFields);
                    var tblData = dbHelper.GetTableData(query);
                    rep.ProcessDataRows(tblData.Select(), ++columnIndex);
                }
            }

            tablesResult[0] = rep.GetReportData();

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.STARTDATE, paramStartDate);
            paramHelper.SetParamValue(ParamUFKHelper.ENDDATE, paramEndDate);
            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(filterKD));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            paramHelper.SetParamValue(ParamUFKHelper.SUMM, reportParams[ReportConsts.ParamSum]);
            return tablesResult;
        }
    }
}
