using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData.FNS;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{

    public partial class ReportDataServer
    {
        //        private DataTable[] GetUFNSReport006DataCommon(Dictionary<string, string> reportParams, string filterRefDGroup)

        /// <summary>
        /// ОТЧЕТ 006 - АНАЛИЗ ПЕРЕПЛАТЫ ПО НАЛОГАМ В КОНСОЛИДИРОВАННЫЙ БЮДЖЕТ СУБЪЕКТА ПО ФОРМАМ 1-НМ, 4-НМ И 65Н 
        /// </summary>
        public DataTable[] GetUFNSReport006Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var paramArrears = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamArrearsFNS]);
            var yearList = GetSelectedYears(reportParams[ReportConsts.ParamYear]);
            var filterPeriod = GetYearsBoundsFilter(yearList, true);
            var currentYear = DateTime.Now.Year;
            yearList = yearList.Where(year => year <= currentYear).ToList();
            var filterRefDGroup = GetRefDGroupByKD(paramKD);

            // получаем данные из т.ф. «Доходы.ФНС.1 НМ Сводный» (f_D_FNS3Cons).
            var filterList = new List<QFilter>
            {
                new QFilter(QFNS3Cons.Keys.Period, filterPeriod),
                new QFilter(QFNS3Cons.Keys.KD, paramKD)
            };
            var groupList = new List<Enum> { QFNS3Cons.Keys.Day, QFNS3Cons.Keys.Lvl };
            var queryText = new QFNS3Cons().GetQueryText(filterList, groupList);
            var tblFNS3Data = dbHelper.GetTableData(queryText);

            // получаем данные из т.ф. «Доходы.ФНС.4 НМ.Сводный» (f_D_FNS4NMTotal)
            filterList = new List<QFilter>
            {
                new QFilter(QFNS4NMTotal.Keys.Period, filterPeriod),
                new QFilter(QFNS4NMTotal.Keys.Arrears, paramArrears),
                new QFilter(QFNS4NMTotal.Keys.RefD, filterRefDGroup)
            };
            groupList = new List<Enum> { QFNS4NMTotal.Keys.Day };
            queryText = new QFNS4NMTotal().GetQueryText(filterList, groupList);
            var tblFNS4Data = dbHelper.GetTableData(queryText);

            // получаем данные из т.ф. «ФНС.28н.без расщепления» (f_F_DirtyUMNS28n)
            filterList = new List<QFilter>
            {
                new QFilter(QDirtyUMNS28n.Keys.Period, filterPeriod),
                new QFilter(QDirtyUMNS28n.Keys.Mark, GetUFNSMarkCodes(UFNSMark.Overpayment)),
                new QFilter(QDirtyUMNS28n.Keys.KD, paramKD)
            };
            groupList = new List<Enum> { QDirtyUMNS28n.Keys.Day };
            queryText = new QDirtyUMNS28n().GetQueryText(filterList, groupList);
            var tblUMNSData = dbHelper.GetTableData(queryText);

            // заполняем таблицу отчета
            var repTable = new DataTable();
            AddColumnToReport(repTable, typeof(string), "Period");
            AddColumnToReport(repTable, typeof(decimal), "Sum", 7);
            AddColumnToReport(repTable, typeof(int), STYLE);
            var lvl = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsSubject); // только Консолидированный бюджет субъекта
            var lvlFilter = filterHelper.RangeFilter(f_D_FNS3Cons.RefBudgetLevels, lvl);
            object prevSum3 = DBNull.Value;
            object prevSum5 = DBNull.Value;

            foreach (var year in yearList)
            {
                var lastMonth = year < currentYear ? 12 : DateTime.Now.Month;
                for (var month = 1; month <= lastMonth; month++)
                {
                    var dateFilter = filterHelper.EqualIntFilter(f_D_FNS3Cons.RefYearDayUNV, GetUNVMonthStart(year, month));
                    var sum0 = GetSumFieldValue(tblFNS3Data, "Sum0", dateFilter);
                    var sum1 = GetSumFieldValue(tblFNS3Data, "Sum1", String.Format("{0} and {1}", dateFilter, lvlFilter));
                    var sum3 = GetSumFieldValue(tblFNS4Data, SUM, dateFilter);
                    var sum5 = GetSumFieldValue(tblUMNSData, SUM, dateFilter);

                    var row = repTable.Rows.Add();
                    row["Period"] = month < lastMonth
                                        ? String.Format("на {0}", GetMonthStart(year, month + 1))
                                        : String.Format("на {0}", GetMonthStart(year + 1, 1));

                    row[STYLE] = 0;
                    row["Sum0"] = sum0;
                    row["Sum1"] = sum1;
                    row["Sum2"] = GetNotNullSumDifference(sum1, sum0);
                    row["Sum3"] = sum3;
                    row["Sum4"] = GetNotNullSumDifference(sum3, prevSum3);
                    row["Sum5"] = sum5;
                    row["Sum6"] = GetNotNullSumDifference(sum5, prevSum5);

                    prevSum3 = sum3;
                    prevSum5 = sum5;
                }
            }

            // делим суммы в зависимости от выбранных единиц измерения
            DivideSum(repTable, 1, 7, divider);

            tablesResult[0] = repTable;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            var reportHelper = new ReportMonthMethods(scheme);

            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(paramKD));
            paramHelper.SetParamValue(ParamUFKHelper.DGROUP, reportHelper.GetDGroupCaptionText(filterRefDGroup));
            paramHelper.SetParamValue(ParamUFKHelper.ARREARS, reportHelper.GetArrearsFNSCaptionText(paramArrears));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }


        /// <summary>
        /// ОТЧЕТ 006_1 - АНАЛИЗ ПЕРЕПЛАТЫ ПО НАЛОГУ НА ПРИБЫЛЬ В КОНСОЛИДИРОВАННЫЙ БЮДЖЕТ СУБЪЕКТА 
        /// </summary>
        public DataTable[] GetUFNSReport006_1Data(Dictionary<string, string> reportParams)
        {
            const int sumColumnsCount = 13;
            const string arrearsCode1020 = "1020"; // недоимка
            const string taxCode = "101000000"; // налог на прибыль организаций
            var reportHelper = new ReportMonthMethods(scheme);
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var paramArrears = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamArrearsFNS]);
            var paramArrears1020 = reportHelper.GetArrearsFNSNestedID(arrearsCode1020);
            var yearList = GetSelectedYears(reportParams[ReportConsts.ParamYear]);
            var filterPeriod = GetYearsBoundsFilter(yearList, true);
            var currentYear = DateTime.Now.Year;
            yearList = yearList.Where(year => year <= currentYear).ToList();
            var filterRefDGroup = reportHelper.GetIDByField(b_D_Group.InternalKey, b_D_Group.Code, taxCode);

            // получаем данные из т.ф. «Доходы.ФНС.1 НМ Сводный» (f_D_FNS3Cons).
            var filterList = new List<QFilter>
            {
                new QFilter(QFNS3Cons.Keys.Period, filterPeriod),
                new QFilter(QFNS3Cons.Keys.KD, paramKD)
            };
            var groupList = new List<Enum> { QFNS3Cons.Keys.Day, QFNS3Cons.Keys.Lvl };
            var queryText = new QFNS3Cons().GetQueryText(filterList, groupList);
            var tblFNS3Data = dbHelper.GetTableData(queryText);

            // получаем данные из т.ф. «Доходы.ФНС.4 НМ.Сводный» (f_D_FNS4NMTotal)
            var fltArrears1020 = paramArrears1020 != String.Empty
                                     ? filterHelper.RangeFilter(d_Arrears_FNS.RefArrearsFNSBridge, paramArrears1020)
                                     : filterHelper.EqualIntFilter(d_Arrears_FNS.RefArrearsFNSBridge, ReportConsts.UndefinedKey);

            var fltArrearsAll = paramArrears != String.Empty
                                    ? ConvertToString(new List<string> {paramArrears, paramArrears1020})
                                    : String.Empty;

            filterList = new List<QFilter>
            {
                new QFilter(QFNS4NMTotal.Keys.Period, filterPeriod),
                new QFilter(QFNS4NMTotal.Keys.Arrears,  fltArrearsAll),
                new QFilter(QFNS4NMTotal.Keys.RefD, filterRefDGroup)
            };
            groupList = new List<Enum> { QFNS4NMTotal.Keys.Day, QFNS4NMTotal.Keys.Arrears };
            queryText = new QFNS4NMTotal().GetQueryText(filterList, groupList);
            var tblFNS4Data = dbHelper.GetTableData(queryText);

            // получаем данные из т.ф. «ФНС.28н.без расщепления» (f_F_DirtyUMNS28n)
            var markDebts = GetUFNSMarkCodes(UFNSMark.Debts);
            var markArrears = GetUFNSMarkCodes(UFNSMark.Arrears);
            var markOverpayment = GetUFNSMarkCodes(UFNSMark.Overpayment);
            var marksAll = ConvertToString(new List<string> {markDebts, markArrears, markOverpayment});

            var fltMarkDebts = markDebts != String.Empty
                                     ? filterHelper.RangeFilter(f_F_DirtyUMNS28n.RefDataMarks65n, markDebts)
                                     : filterHelper.EqualIntFilter(f_F_DirtyUMNS28n.RefDataMarks65n, ReportConsts.UndefinedKey);
            var fltMarkArrears = markArrears != String.Empty
                                     ? filterHelper.RangeFilter(f_F_DirtyUMNS28n.RefDataMarks65n, markArrears)
                                     : filterHelper.EqualIntFilter(f_F_DirtyUMNS28n.RefDataMarks65n, ReportConsts.UndefinedKey);
            var fltMarkOverpayment = markDebts != String.Empty
                                     ? filterHelper.RangeFilter(f_F_DirtyUMNS28n.RefDataMarks65n, markOverpayment)
                                     : filterHelper.EqualIntFilter(f_F_DirtyUMNS28n.RefDataMarks65n, ReportConsts.UndefinedKey);

            filterList = new List<QFilter>
            {
                new QFilter(QDirtyUMNS28n.Keys.Period, filterPeriod),
                new QFilter(QDirtyUMNS28n.Keys.Mark, marksAll),
                new QFilter(QDirtyUMNS28n.Keys.KD, paramKD)
            };
            groupList = new List<Enum> {QDirtyUMNS28n.Keys.Day, QDirtyUMNS28n.Keys.Mark};
            queryText = new QDirtyUMNS28n().GetQueryText(filterList, groupList);
            var tblUMNSData = dbHelper.GetTableData(queryText);

            // заполняем таблицу отчета
            var repTable = new DataTable();
            AddColumnToReport(repTable, typeof(string), "Period");
            AddColumnToReport(repTable, typeof(decimal), "Sum", sumColumnsCount);
            AddColumnToReport(repTable, typeof(int), STYLE);
            var lvl = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsSubject); // только Консолидированный бюджет субъекта
            var lvlFilter = filterHelper.RangeFilter(f_D_FNS3Cons.RefBudgetLevels, lvl);
            object prevSum3 = DBNull.Value;
            object prevSum5 = DBNull.Value;
            object prevSum7 = DBNull.Value;
            object prevSum9 = DBNull.Value;
            object prevSum11 = DBNull.Value;

            foreach (var year in yearList)
            {
                var lastMonth = year < currentYear ? 12 : DateTime.Now.Month;
                for (var month = 1; month <= lastMonth; month++)
                {
                    var dateFilter = filterHelper.EqualIntFilter(f_D_FNS3Cons.RefYearDayUNV, GetUNVMonthStart(year, month));
                    var filter = paramArrears != String.Empty
                        ? CombineAnd(dateFilter, filterHelper.RangeFilter(d_Arrears_FNS.RefArrearsFNSBridge, paramArrears))
                        : dateFilter;
                    var sum0 = GetSumFieldValue(tblFNS3Data, SUM0, dateFilter);
                    var sum1 = GetSumFieldValue(tblFNS3Data, SUM1, CombineAnd(dateFilter, lvlFilter));
                    var sum3 = GetSumFieldValue(tblFNS4Data, SUM, filter);
                    var sum5 = GetSumFieldValue(tblUMNSData, SUM, CombineAnd(dateFilter, fltMarkDebts));
                    var sum7 = GetSumFieldValue(tblFNS4Data, SUM, CombineAnd(dateFilter, fltArrears1020));
                    var sum9 = GetSumFieldValue(tblUMNSData, SUM, CombineAnd(dateFilter, fltMarkArrears));
                    var sum11 = GetSumFieldValue(tblUMNSData, SUM, CombineAnd(dateFilter, fltMarkOverpayment));

                    var row = repTable.Rows.Add();
                    row["Period"] = month < lastMonth
                                        ? String.Format("на {0}", GetMonthStart(year, month + 1))
                                        : String.Format("на {0}", GetMonthStart(year + 1, 1));

                    row[STYLE] = 0;
                    row["Sum0"] = sum0;
                    row["Sum1"] = sum1;
                    row["Sum2"] = GetNotNullSumDifference(sum1, sum0);
                    row["Sum3"] = sum3;
                    row["Sum4"] = GetNotNullSumDifference(sum3, prevSum3);
                    row["Sum5"] = sum5;
                    row["Sum6"] = GetNotNullSumDifference(sum5, prevSum5);
                    row["Sum7"] = sum7;
                    row["Sum8"] = GetNotNullSumDifference(sum7, prevSum7);
                    row["Sum9"] = sum9;
                    row["Sum10"] = GetNotNullSumDifference(sum9, prevSum9);
                    row["Sum11"] = sum11;
                    row["Sum12"] = GetNotNullSumDifference(sum11, prevSum11);

                    prevSum3 = sum3;
                    prevSum5 = sum5;
                    prevSum7 = sum7;
                    prevSum9 = sum9;
                    prevSum11 = sum11;
                }
            }

            // делим суммы в зависимости от выбранных единиц измерения
            DivideSum(repTable, 1, sumColumnsCount, divider);

            tablesResult[0] = repTable;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);

            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(paramKD));
            paramHelper.SetParamValue(ParamUFKHelper.DGROUP, reportHelper.GetDGroupCaptionText(filterRefDGroup));
            paramHelper.SetParamValue(ParamUFKHelper.ARREARS, reportHelper.GetArrearsFNSCaptionText(paramArrears));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;


        }
    }
}
