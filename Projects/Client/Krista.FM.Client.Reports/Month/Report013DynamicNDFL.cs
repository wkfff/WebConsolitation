using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK;
using Krista.FM.Client.Reports.Month;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Database.ClsBridge;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 013_АНАЛИЗ КОНТИНГЕНТА ПО НДФЛ В ЦЕЛОМ
        /// </summary>
        public DataTable[] GetMonthReport013DynamicNDFLData(Dictionary<string, string> reportParams)
        {
            var dbHelper = new ReportDBHelper(scheme);
            var filterHelper = new QFilterHelper();
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var rowCaption = CreateReportParamsRow(tablesResult);
            var filterLvl = ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.ConsSubject);
            var yearParam = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var yearCodes = yearParam.Split(',');
            var tblResult = CreateReportCaptionTable(2 * yearCodes.Length, 12);
            var yearList = new List<int>(yearCodes.Select(ReportMonthMethods.GetSelectedYear));
            var filterPeriod = filterHelper.GetMultiYearFilter(yearList);

            if (yearParam.Length == 0)
            {
                yearList.Clear();
            }
            else
            {
                yearList.Sort();
            }

            var kdBridge = ConvertorSchemeLink.GetEntity(b_KD_Bridge.InternalKey);
            var rowNDFL = reportHelper.GetBookRow(kdBridge, b_KD_Bridge.CodeStr, "00010102000010000110");
            var rowPtnt = reportHelper.GetBookRow(kdBridge, b_KD_Bridge.CodeStr, "00010102040010000110");
            var fltPtnt =  Convert.ToString(rowPtnt[b_KD_Bridge.ID]);
            var fltNDFL = reportHelper.GetKDHierarchyFilter(Convert.ToString(rowNDFL[b_KD_Bridge.ID]));
            var yearCounter = 0;
            // общий по ндфл
            var qMonthParams = new QMonthRepParams
            {
                Period = filterPeriod,
                KD = fltNDFL,
                Lvl = filterLvl,
                DocType = ReportMonthConsts.DocTypeConsolidate
            };
            var groupMonthInfo = new List<QMonthRepGroup> { QMonthRepGroup.Period };
            var queryMonText = QMonthRepIncomes.GroupBy(qMonthParams, groupMonthInfo);
            var tblNDFLDataMon = dbHelper.GetTableData(queryMonText);
            // по патентам
            qMonthParams.KD = fltPtnt;
            var queryPtntMonText = QMonthRepIncomes.GroupBy(qMonthParams, groupMonthInfo);
            var tblPtntDataMon = dbHelper.GetTableData(queryPtntMonText);


            var qFOYRParams = new QFOYRParams
            {
                Period = filterPeriod,
                KD = fltNDFL,
                Lvl = filterLvl,
                DocType = ReportMonthConsts.DocTypeConsolidate
            };

            var groupFOYRInfo = new List<QFOYRGroup> { QFOYRGroup.Period };
            var queryYrText = QFOYRIncomes.GroupBy(qFOYRParams, groupFOYRInfo);
            var tblNDFLDataYr = dbHelper.GetTableData(queryYrText);
            // по патентам
            qFOYRParams.KD = fltPtnt;
            var queryPtntYrText = QFOYRIncomes.GroupBy(qFOYRParams, groupFOYRInfo);
            var tblPtntDataYr = dbHelper.GetTableData(queryPtntYrText);

            foreach (var year in yearList)
            {
                rowCaption[10 + yearCounter] = year;
                
                var filterYearPeriod = filterHelper.PeriodFilter(
                    f_D_FOYRIncomes.RefYearDayUNV, 
                    GetUNVYearLoBound(year),
                    GetUNVYearEnd(year));

                var hasYearData = tblNDFLDataYr.Select(filterYearPeriod).Length > 0;

                for (var i = 1; i < 13; i++)
                {
                    var rowIndex = i - 1;
                    var periodStart = GetUNVMonthStart(year, i);
                    var filterMonth = filterHelper.EqualIntFilter(f_F_MonthRepIncomes.RefYearDayUNV, periodStart);
                    var rowsNDFLMonth = tblNDFLDataMon.Select(filterMonth);
                    var rowsPtntMonth = tblPtntDataMon.Select(filterMonth);

                    var columnName = f_F_MonthRepIncomes.Fact;

                    if (i == 12 && hasYearData)
                    {
                        columnName = f_D_FOYRIncomes.Performed;
                        rowsNDFLMonth = tblNDFLDataYr.Select(filterYearPeriod);
                        rowsPtntMonth = tblPtntDataYr.Select(filterYearPeriod);
                    }

                    var sumNDFL = rowsNDFLMonth.Sum(dataRow => GetDecimal(dataRow[columnName]));
                    var sumPtnt = rowsPtntMonth.Sum(dataRow => GetDecimal(dataRow[columnName]));

                    var colIndex = yearCounter * 2;
                    tblResult.Rows[rowIndex][colIndex + 0] = sumNDFL;
                    tblResult.Rows[rowIndex][colIndex + 1] = sumPtnt;
                }

                yearCounter++;
            }

            for (var i = 0; i < tblResult.Columns.Count; i++)
            {
                DivideColumn(tblResult, i, divider);
                RoundColumn(tblResult, i, precision);
            }

            tablesResult[0] = tblResult;
            rowCaption[0] = yearList.Count;
            rowCaption[6] = ReportMonthMethods.GetDividerDescr(divider);
            rowCaption[7] = ReportMonthMethods.GetPrecisionIndex(precision);
            return tablesResult;
        }
    }
}
