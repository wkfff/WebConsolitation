using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Month;
using Krista.FM.Client.Reports.Month.Queries;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 015_АНАЛИЗ ЕЖЕМЕСЯЧНЫХ ПОСТУПЛЕНИЙ ПО АКЦИЗАМ
        /// </summary>
        public DataTable[] GetMonthReport015DynamicExciseData(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var rowCaption = CreateReportParamsRow(tablesResult);
            var yearCodes = reportParams[ReportConsts.ParamYear].Split(',');
            var kdBridge = ConvertorSchemeLink.GetEntity(b_KD_Bridge.InternalKey);
            var rowExcise = reportHelper.GetBookRow(kdBridge, b_KD_Bridge.CodeStr, "00010302000010000110");
            var lstExciseUfk = new List<string>
                                   {
                                       "00010302190010000110",
                                       "00010302200010000110",
                                       "00010302150010000110",
                                       "00010302160010000110",
                                       "00010302170010000110",
                                       "00010302180010000110"
                                   };
            
            var fltUfk = lstExciseUfk.Select(exciseUfkCode => 
                reportHelper.GetBookRow(kdBridge, b_KD_Bridge.CodeStr, exciseUfkCode))
                .Where(rowUfk => rowUfk != null)
                .Aggregate(String.Empty, (current, rowUfk) => Combine(current, rowUfk[b_KD_Bridge.ID], ","));

            fltUfk = fltUfk.Trim(',');

            var clsParams = reportHelper.CreateCutKDParams();
            clsParams.Key = Convert.ToString(rowExcise[b_KD_Bridge.ID]);
            var rowsExcise = reportHelper.GetChildClsRecord(clsParams);
            var fltExcise = String.Empty;
            var lstYears = yearCodes.Select(ReportMonthMethods.GetSelectedYear).ToList();
            var filterPeriod = filterHelper.GetMultiYearFilter(lstYears);
            lstYears.Sort();

            fltExcise = rowsExcise.Aggregate(fltExcise, (current, clsRow) => 
                Combine(current, clsRow.KeyValue, ","));

            fltExcise = fltExcise.Trim(',');

            var dbHelper = new ReportDBHelper(scheme);
            var tblResult = CreateReportCaptionTable(2 * lstYears.Count, 12);
            var yearCounter = 0;

            var qMonthParams = new QMonthRepParams
                                   {
                                       Period = filterPeriod,
                                       KD = fltExcise,
                                       Lvl = ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.MRTotal),
                                       DocType = ReportMonthConsts.DocTypeConsolidate
                                   };

            var groupMonthInfo = new List<QMonthRepGroup> { QMonthRepGroup.Period };
            var queryIncomeData = QMonthRepIncomes.GroupBy(qMonthParams, groupMonthInfo);
            var tblExcDataMon = dbHelper.GetTableData(queryIncomeData);
            qMonthParams.KD = fltUfk;
            queryIncomeData = QMonthRepIncomes.GroupBy(qMonthParams, groupMonthInfo);
            var tblUfkDataMon = dbHelper.GetTableData(queryIncomeData);
            var columnDataIndex = GetColumnIndex(tblExcDataMon, f_F_MonthRepIncomes.Fact);

            var qFOYRParams = new QFOYRParams
                                  {
                                      Period = filterPeriod,
                                      KD = fltExcise,
                                      Lvl = ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.MRTotal),
                                      DocType = ReportMonthConsts.DocTypeConsolidate
                                  };

            var groupFOYRInfo = new List<QFOYRGroup> { QFOYRGroup.Period };
            var queryYrData = QFOYRIncomes.GroupBy(qFOYRParams, groupFOYRInfo);
            var tblExcDataYr = dbHelper.GetTableData(queryYrData);
            qFOYRParams.KD = fltUfk;
            queryYrData = QFOYRIncomes.GroupBy(qFOYRParams, groupFOYRInfo);
            var tblUfkDataYr = dbHelper.GetTableData(queryYrData);
            var columnDataIndexYr = GetColumnIndex(tblExcDataYr, f_D_FOYRIncomes.Performed);

            foreach (var year in lstYears)
            {
                var filterYear = filterHelper.EqualIntFilter(f_D_FOYRIncomes.RefYearDayUNV, GetUNVYearPlanLoBound(year));
                var rowsExcYear = tblExcDataYr.Select(filterYear);
                var rowsUfkYear = tblUfkDataYr.Select(filterYear);

                for (var i = 1; i < 13; i++)
                {
                    var rowIndex = i - 1;
                    var periodStart = GetUNVMonthStart(year, i);
                    var filterMonth = filterHelper.EqualIntFilter(f_F_MonthRepIncomes.RefYearDayUNV, periodStart);
                    var rowsExcMonth = tblExcDataMon.Select(filterMonth);
                    var rowsUfkMonth = tblUfkDataMon.Select(filterMonth);
                    var sumExc = i == 12 && rowsExcYear.Length > 0
                                     ? rowsExcYear.Sum(dataRow => GetDecimal(dataRow[columnDataIndexYr]))
                                     : rowsExcMonth.Sum(dataRow => GetDecimal(dataRow[columnDataIndex]));
                    var sumUfk = i == 12 && rowsUfkYear.Length > 0
                                     ? rowsUfkYear.Sum(dataRow => GetDecimal(dataRow[columnDataIndexYr]))
                                     : rowsUfkMonth.Sum(dataRow => GetDecimal(dataRow[columnDataIndex]));
                    var colIndex = yearCounter * 2;
                    tblResult.Rows[rowIndex][colIndex + 0] = sumExc;
                    tblResult.Rows[rowIndex][colIndex + 1] = sumUfk;
                }

                rowCaption[10 + yearCounter] = year;
                yearCounter++;
            }

            for (var i = 0; i < tblResult.Columns.Count; i++)
            {
                DivideColumn(tblResult, i, divider);
                RoundColumn(tblResult, i, precision);
            }

            tablesResult[0] = tblResult;
            rowCaption[0] = lstYears.Count;
            rowCaption[6] = ReportMonthMethods.GetDividerDescr(divider);
            rowCaption[7] = ReportMonthMethods.GetPrecisionIndex(precision);
            return tablesResult;
        }
    }
}
