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
        /// ОТЧЕТ 003_АНАЛИЗ ПОСТУПЛЕНИЙ ПО 1-НМ 
        /// </summary>
        public DataTable[] GetUFNSReport003Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var yearList = GetSelectedYears(reportParams[ReportConsts.ParamYear]);
            var filterPeriod = GetYearsBoundsFilter(yearList, true);
            var currentYear = DateTime.Now.Year;
            yearList = yearList.Where(year => year <= currentYear).ToList();

            var paramLvl = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamBdgtLevels]);
            if (paramLvl == String.Empty)
            {
                paramLvl = "1";
            }
            var levels = new List<string> 
            {
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Federal),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsSubject),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsMunicipal),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Municipal),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Settle),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.FondsFractional),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.PurposeFonds)
            };
            var lvl = levels[Convert.ToInt32(paramLvl)];

            // получаем данные из т.ф. «Доходы.ФНС.1 НМ Сводный» (f_D_FNS3Cons).
            var filterList = new List<QFilter> 
            {
                new QFilter(QFNS3Cons.Keys.Period,  filterPeriod),
                new QFilter(QFNS3Cons.Keys.KD,  paramKD)
            };
            var groupList = new List<Enum> { QFNS3Cons.Keys.Day, QFNS3Cons.Keys.Lvl };
            var queryText = new QFNS3Cons().GetQueryText(filterList, groupList);
            var tblData = dbHelper.GetTableData(queryText);

            // заполняем таблицу отчета
            var repTable = new DataTable();
            AddColumnToReport(repTable, typeof(string), "Period");
            AddColumnToReport(repTable, typeof(decimal), "Sum", 4);
            AddColumnToReport(repTable, typeof(int), STYLE);

            foreach (var year in yearList)
            {
                var lastMonth = year < currentYear ? 12 : DateTime.Now.Month;

                for (var month = 1; month <= lastMonth; month++)
                {
                    var filter = filterHelper.EqualIntFilter(f_D_FNS3Cons.RefYearDayUNV, GetUNVMonthStart(year, month));
                    var sum0 = GetSumFieldValue(tblData, "Sum0", filter);

                    filter = String.Format("{0} and {1}",
                                            filterHelper.EqualIntFilter(f_D_FNS3Cons.RefYearDayUNV, GetUNVMonthStart(year, month)),
                                            filterHelper.RangeFilter(f_D_FNS3Cons.RefBudgetLevels, lvl)
                                          );
                    var sum1 = GetSumFieldValue(tblData, "Sum1", filter);

                    var row = repTable.Rows.Add();
                    row["Period"] = month < 12
                                        ? String.Format("на {0}", GetMonthStart(year, month + 1))
                                        : String.Format("на {0}", GetMonthStart(year + 1, 1));
                    row["Sum0"] = sum0;
                    row["Sum1"] = sum1;
                    row["Sum2"] = GetNotNullSumDifference(sum1, sum0);

                    if (sum0 != DBNull.Value && sum1 != DBNull.Value)
                    {
                        var dSum0 = GetDecimal(sum0);
                        var dSum1 = GetDecimal(sum1);
                        if (dSum0 != 0)
                        {
                            row["Sum3"] = (dSum1 / dSum0) * 100;
                        }
                    }

                    row[STYLE] = 0;
                }
            }


            // убираем строки без данных
            repTable = FilterNotExistData(repTable, GetColumnsList(1, 4));

            // делим суммы в зависимости от выбранных единиц измерения
            DivideSum(repTable, 1, 3, divider);

            tablesResult[0] = repTable;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.BDGT_LEVEL, ReportMonthMethods.GetSelectedBudgetLvlFull(paramLvl));
            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(paramKD));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));

            return tablesResult;
        }
    }
}
