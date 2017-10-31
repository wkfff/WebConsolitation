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
        /// ОТЧЕТ 007 - АНАЛИЗ НАЧИСЛЕНИЙ И ПОСТУПЛЕНИЙ ПО 1-НМ, 65Н И МЕСОТЧ
        /// </summary>
        public DataTable[] GetUFNSReport007Data(Dictionary<string, string> reportParams)
        {
            var reportHelper = new ReportMonthMethods(scheme);
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
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

            // получаем данные из т.ф. «ФНС.28н.без расщепления» (f_F_DirtyUMNS28n)
            var charge = GetUFNSMarkCodes(UFNSMark.Charge);
            var income = GetUFNSMarkCodes(UFNSMark.Income);
            var compens = GetUFNSMarkCodes(UFNSMark.Compens);
            filterList = new List<QFilter>
            {
                new QFilter(QDirtyUMNS28n.Keys.Period, filterPeriod),
                new QFilter(QDirtyUMNS28n.Keys.Mark, String.Format("{0},{1},{2}", charge, income, compens)),
                new QFilter(QDirtyUMNS28n.Keys.KD, paramKD)
            };
            groupList = new List<Enum> { QDirtyUMNS28n.Keys.Day, QDirtyUMNS28n.Keys.Mark };
            queryText = new QDirtyUMNS28n().GetQueryText(filterList, groupList);
            var tblUMNSData = dbHelper.GetTableData(queryText);

            // заполняем таблицу отчета
            var repTable = new DataTable();
            AddColumnToReport(repTable, typeof(string), "Period");
            AddColumnToReport(repTable, typeof(decimal), "Sum", 9);
            AddColumnToReport(repTable, typeof(int), STYLE);

            var fltCharge = filterHelper.RangeFilter(f_F_DirtyUMNS28n.RefDataMarks65n, charge);
            var fltIncome = filterHelper.RangeFilter(f_F_DirtyUMNS28n.RefDataMarks65n, income);
            var fltCompens = filterHelper.RangeFilter(f_F_DirtyUMNS28n.RefDataMarks65n, compens);
            var lvl = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsSubject); // только Консолидированный бюджет субъекта
            var fltLvl = filterHelper.RangeFilter(f_D_FNS3Cons.RefBudgetLevels, lvl);

            foreach (var year in yearList)
            {
                var lastMonth = year < currentYear ? 12 : DateTime.Now.Month;

                for (var month = 1; month <= lastMonth; month++)
                {
                    var monthFilter = filterHelper.EqualIntFilter(f_D_FNS3Cons.RefYearDayUNV, GetUNVMonthStart(year, month));
                    var sum0 = GetSumFieldValue(tblUMNSData, "Sum", String.Format("{0} and {1}", monthFilter, fltCharge));
                    var sum1 = GetSumFieldValue(tblFNS3Data, "Sum0", monthFilter);
                    var sum4 = GetSumFieldValue(tblFNS3Data, "Sum1", String.Format("{0} and {1}", monthFilter, fltLvl));
                    var sum6 = GetSumFieldValue(tblMonthRepData, "Sum", monthFilter);
                    var sumIncome = GetSumFieldValue(tblUMNSData, "Sum", String.Format("{0} and {1}", monthFilter, fltIncome));
                    var sumCompens = GetSumFieldValue(tblUMNSData, "Sum", String.Format("{0} and {1}", monthFilter, fltCompens));

                    var sum3 = GetNotNullSumDifference(sumIncome, sumCompens);

                    var row = repTable.Rows.Add();
                    row["Period"] = month < 12
                                    ? String.Format("на {0}", GetMonthStart(year, month + 1))
                                    : String.Format("на {0}", GetMonthStart(year + 1, 1));

                    row[STYLE] = 0;
                    row["Sum0"] = sum0;
                    row["Sum1"] = sum1;
                    row["Sum2"] = GetNotNullSumDifference(sum1, sum0);
                    row["Sum3"] = sum3;
                    row["Sum4"] = sum4;
                    row["Sum5"] = GetNotNullSumDifference(sum4, sum3);
                    row["Sum6"] = sum6;
                    row["Sum7"] = GetNotNullSumDifference(sum6, sum3);
                    row["Sum8"] = GetNotNullSumDifference(sum6, sum4);
                }
            }

            // делим суммы в зависимости от выбранных единиц измерения
            DivideSum(repTable, 1, 9, divider);

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
