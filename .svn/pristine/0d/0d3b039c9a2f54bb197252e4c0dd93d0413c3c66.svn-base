using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{

    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 008 - АНАЛИЗ СОБИРАЕМОСТИ И ЗАДОЛЖЕННОСТИ В КОНСОЛИДИРОВАННЫЙ БЮДЖЕТ МОСКОВСКОЙ ОБЛАСТИ 
        /// </summary>
        public DataTable[] GetUFNSReport008Data(Dictionary<string, string> reportParams)
        {
            const int sumColumnsCount = 7;
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var paramArrears = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamArrearsFNS]);
            var yearList = GetSelectedYears(reportParams[ReportConsts.ParamYear]);
            var filterPeriod = GetYearsBoundsFilter(yearList, true);
            var currentYear = DateTime.Now.Year;
            yearList = yearList.Where(year => year <= currentYear).ToList();

            // получаем данные из т.ф. «Доходы.ФНС.1 НМ Сводный» (f_D_FNS3Cons).
            var filterList = new List<QFilter>
            {
                new QFilter(QFNS3Cons.Keys.Period, filterPeriod),
                new QFilter(QFNS3Cons.Keys.KD, paramKD)
            };
            var groupList = new List<Enum> { QFNS3Cons.Keys.Day, QFNS3Cons.Keys.Lvl };
            var queryText = new QFNS3Cons().GetQueryText(filterList, groupList);
            var tblFNS3Data = dbHelper.GetTableData(queryText);

            // получаем данные из т.ф. «Факт.ФО.МесОтч.Доходы» (f_F_MonthRepIncomes)
            var lvlSkif = ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.ConsSubject); // только Консолидированный бюджет субъекта
            var docTypeSkif = ReportMonthMethods.GetDocumentTypesSkif(SettleLvl.ConsSubject);
            filterList = new List<QFilter>
            {
                new QFilter(QMonthRep.Keys.Period, filterPeriod),
                new QFilter(QMonthRep.Keys.KD, paramKD),
                new QFilter(QMonthRep.Keys.Lvl, lvlSkif),
                new QFilter(QMonthRep.Keys.DocTyp, Convert.ToString(docTypeSkif))
            };
            groupList = new List<Enum> { QMonthRep.Keys.Day };
            queryText = new QMonthRep().GetQueryText(filterList, groupList);
            var tblMonthRepData = dbHelper.GetTableData(queryText);

            // получаем данные из т.ф. «Доходы.ФНС.4 НМ.Сводный» (f_D_FNS4NMTotal)
            filterList = new List<QFilter>
            {
                new QFilter(QFNS4NMTotal.Keys.Period, filterPeriod),
                new QFilter(QFNS4NMTotal.Keys.Arrears, paramArrears),
                new QFilter(QFNS4NMTotal.Keys.RefD, GetRefDGroupByKD(paramKD))
            };
            groupList = new List<Enum> { QFNS4NMTotal.Keys.Day };
            queryText = new QFNS4NMTotal().GetQueryText(filterList, groupList);
            var tblFNS4Data = dbHelper.GetTableData(queryText);

            // получаем данные из т.ф. «ФНС.28н.без расщепления» (f_F_DirtyUMNS28n)
            filterList = new List<QFilter>
            {
                new QFilter(QDirtyUMNS28n.Keys.Period, filterPeriod),
                new QFilter(QDirtyUMNS28n.Keys.Mark, GetUFNSMarkCodes(UFNSMark.Debts)),
                new QFilter(QDirtyUMNS28n.Keys.KD, paramKD)
            };
            groupList = new List<Enum> { QDirtyUMNS28n.Keys.Day };
            queryText = new QDirtyUMNS28n().GetQueryText(filterList, groupList);
            var tblUMNSData = dbHelper.GetTableData(queryText);

            // заполняем таблицу отчета
            var repTable = new DataTable();
            AddColumnToReport(repTable, typeof(string), "Period");
            AddColumnToReport(repTable, typeof(decimal), "Sum", sumColumnsCount);
            AddColumnToReport(repTable, typeof(int), STYLE);
            var lvl = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsSubject); // только Консолидированный бюджет субъекта
            var fltLvl = filterHelper.RangeFilter(f_D_FNS3Cons.RefBudgetLevels, lvl);

            foreach (var year in yearList)
            {
                var lastMonth = year < currentYear ? 12 : DateTime.Now.Month;
                for (var month = 1; month <= lastMonth; month++)
                {
                    var dateFilter = filterHelper.EqualIntFilter(f_D_FNS3Cons.RefYearDayUNV, GetUNVMonthStart(year, month));
                    var period = month < lastMonth
                                    ? String.Format("на {0}", GetMonthStart(year, month + 1))
                                    : String.Format("{0} год", year);
                    var sum0 = GetSumFieldValue(tblFNS3Data, "Sum0", dateFilter);
                    var sum1 = GetSumFieldValue(tblFNS3Data, "Sum1", String.Format("{0} and {1}", dateFilter, fltLvl));
                    var row = repTable.Rows.Add();
                    row["Period"] = period;
                    row[STYLE] = 0;
                    row["Sum0"] = sum0;
                    row["Sum1"] = sum1;
                    row["Sum2"] = GetNotNullSumDifference(sum1, sum0);
                    row["Sum3"] = GetSumFieldValue(tblMonthRepData, SUM, dateFilter);
                    row["Sum5"] = GetSumFieldValue(tblFNS4Data, SUM, dateFilter);
                    row["Sum6"] = GetSumFieldValue(tblUMNSData, SUM, dateFilter);

                    if (sum0 != DBNull.Value && sum1 != DBNull.Value)
                    {
                        var x = GetDecimal(sum0);
                        var y = GetDecimal(sum1);
                        if (x != 0)
                        {
                            row["Sum4"] = (y / x) * 100;
                        }
                    }
                }
            }

            // делим суммы в зависимости от выбранных единиц измерения
            DivideSum(repTable, 1, 4, divider);
            DivideSum(repTable, 6, 2, divider);

            tablesResult[0] = repTable;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(paramKD));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));

            return tablesResult;
        }
    }
}
