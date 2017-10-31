using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK;
using Krista.FM.Client.Reports.Month;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData.UFK;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 022_ИСПОЛНЕНИЕ БЮДЖЕТА ЗА МЕСЯЦ В РАЗРЕЗЕ ДОХОДНЫХ ИСТОЧНИКОВ  
        /// </summary>
        public DataTable[] GetMonthReport022ExecuteMonthIncomeData(Dictionary<string, string> reportParams)
        {
            var tablesResult = new DataTable[2];
            var tblResult = CreateReportCaptionTable(10);
            var reportHelper = new ReportMonthMethods(scheme);
            var dbHelper = new ReportDBHelper(scheme);
            var filterHelper = new QFilterHelper();
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var filterKD = reportParams[ReportConsts.ParamKDComparable];
            filterKD = ReportMonthMethods.CheckBookValue(filterKD);
            var filterGroup = reportParams[ReportConsts.ParamGroupKD];
            filterGroup = ReportMonthMethods.CheckBookValue(filterGroup);
            var monthNum = GetEnumItemIndex(new MonthEnum(), reportParams[ReportConsts.ParamMonth]) + 1;
            var entKdBridge = ConvertorSchemeLink.GetEntity(b_KD_Bridge.InternalKey);
            var lvlMon = ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.Summary);
            var year = ReportMonthMethods.GetSelectedYear(reportParams[ReportConsts.ParamYear]);
            var clearKD = reportHelper.CreateGroupKBKFilters(ref filterKD, filterGroup);
            var cutParams = reportHelper.CreateCutKDParams();
            var listKBK = clearKD.Split(',');

            for (var i = 0; i < tablesResult.Length; i++)
            {
                tablesResult[i] = CreateReportCaptionTable(50);
            }

            var tblMonData = new DataTable[2];
            var qMonthParams = new QMonthRepParams
                                   {
                                       KD = filterKD,
                                       Lvl = lvlMon,
                                       DocType = ReportMonthConsts.DocTypeConsolidate
                                   };

            var groupMonthInfo = new List<QMonthRepGroup> { QMonthRepGroup.Kd, QMonthRepGroup.Lvl };

            for (var i = -1; i < 1; i++)
            {
                var curYear = year + i;
                qMonthParams.Period = filterHelper.EqualIntFilter(
                    f_F_MonthRepIncomes.RefYearDayUNV,
                    GetUNVMonthStart(curYear, monthNum), true);
                var queryMonthData = QMonthRepIncomes.GroupBy(qMonthParams, groupMonthInfo);
                tblMonData[i + 1] = dbHelper.GetTableData(queryMonthData);
            }

            var kbkSummary = new List<string>();
            var summary = new decimal[tblResult.Columns.Count];
            const string fieldPlan = f_F_MonthRepIncomes.YearPlan;
            const string fieldFact = f_F_MonthRepIncomes.Fact;

            var kbCode = ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.ConsSubject);
            var obCode = ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.Subject);
            var mbCode = ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.ConsMunicipal);

            var fltKB = filterHelper.RangeFilter(f_F_MonthRepIncomes.RefBdgtLevels, kbCode);
            var fltOB = filterHelper.RangeFilter(f_F_MonthRepIncomes.RefBdgtLevels, obCode);
            var fltMB = filterHelper.RangeFilter(f_F_MonthRepIncomes.RefBdgtLevels, mbCode);

            var tblPrvData = tblMonData[0];
            var tblCurData = tblMonData[1];

            var tblPrvKBData = DataTableUtils.FilterDataSet(tblPrvData, fltKB);
            var tblPrvOBData = DataTableUtils.FilterDataSet(tblPrvData, fltOB);
            var tblPrvMBData = DataTableUtils.FilterDataSet(tblPrvData, fltMB);

            var tblCurKBData = DataTableUtils.FilterDataSet(tblCurData, fltKB);
            var tblCurOBData = DataTableUtils.FilterDataSet(tblCurData, fltOB);
            var tblCurMBData = DataTableUtils.FilterDataSet(tblCurData, fltMB);

            foreach (var refKBK in listKBK)
            {
                var rowTitle = reportHelper.GetBookRow(entKdBridge, refKBK);

                if (rowTitle == null)
                {
                    continue;
                }

                var rowResult = tblResult.NewRow();
                rowResult[0] = rowTitle[b_KD_Bridge.Name];

                cutParams.Key = refKBK;
                var childs = reportHelper.GetChildClsRecord(cutParams);
                var childFilter = refKBK;

                if (childs.Count() > 0)
                {
                    childFilter = childs.Aggregate(childFilter,
                                                   (current, clsRec) => Combine(current, clsRec.KeyValue, ","));
                    childFilter = childFilter.Trim(',');
                }

                var fltKBK = filterHelper.RangeFilter(d_KD_MonthRep.RefKDBridge, childFilter);

                var rowsKBPrv = tblPrvKBData.Select(fltKBK);
                var rowsOBPrv = tblPrvOBData.Select(fltKBK);
                var rowsMBPrv = tblPrvMBData.Select(fltKBK);

                var sumKBPrvFact = rowsKBPrv.Sum(dataRow => GetDecimal(dataRow[fieldFact]));
                var sumOBPrvFact = rowsOBPrv.Sum(dataRow => GetDecimal(dataRow[fieldFact]));
                var sumMBPrvFact = rowsMBPrv.Sum(dataRow => GetDecimal(dataRow[fieldFact]));

                rowResult[1] = sumKBPrvFact;
                rowResult[2] = sumOBPrvFact;
                rowResult[3] = sumMBPrvFact;

                var rowsKBCur = tblCurKBData.Select(fltKBK);
                var rowsOBCur = tblCurOBData.Select(fltKBK);
                var rowsMBCur = tblCurMBData.Select(fltKBK);

                var sumKBCurPlan = rowsKBCur.Sum(dataRow => GetDecimal(dataRow[fieldPlan]));
                var sumOBCurPlan = rowsOBCur.Sum(dataRow => GetDecimal(dataRow[fieldPlan]));
                var sumMBCurPlan = rowsMBCur.Sum(dataRow => GetDecimal(dataRow[fieldPlan]));

                rowResult[4] = sumKBCurPlan;
                rowResult[5] = sumOBCurPlan;
                rowResult[6] = sumMBCurPlan;

                var sumKBCurFact = rowsKBCur.Sum(dataRow => GetDecimal(dataRow[fieldFact]));
                var sumOBCurFact = rowsOBCur.Sum(dataRow => GetDecimal(dataRow[fieldFact]));
                var sumMBCurFact = rowsMBCur.Sum(dataRow => GetDecimal(dataRow[fieldFact]));

                rowResult[7] = sumKBCurFact;
                rowResult[8] = sumOBCurFact;
                rowResult[9] = sumMBCurFact;

                var isNonEmpty = false;

                for (var i = 1; i < 10; i++)
                {
                    isNonEmpty = isNonEmpty || GetDecimal(rowResult[i]) != 0;
                }

                if (isNonEmpty)
                {
                    tblResult.Rows.Add(rowResult);
                }

                if (!kbkSummary.Contains(refKBK))
                {
                    summary[1] += sumKBPrvFact;
                    summary[2] += sumOBPrvFact;
                    summary[3] += sumMBPrvFact;

                    summary[4] += sumKBCurPlan;
                    summary[5] += sumOBCurPlan;
                    summary[6] += sumMBCurPlan;

                    summary[7] += sumKBCurFact;
                    summary[8] += sumOBCurFact;
                    summary[9] += sumMBCurFact;
                }

                kbkSummary.AddRange(childs.Select(child => child.KeyValue));
            }

            var rowSummary = tblResult.Rows.Add();

            for (var i = 0; i < summary.Length; i++)
            {
                rowSummary[i] = summary[i];
            }

            for (var j = 1; j < tblResult.Columns.Count; j++)
            {
                DivideColumn(tblResult, j, divider);
                RoundColumn(tblResult, j, precision);
            }

            var nextMonth = new DateTime(year, monthNum, 1).AddMonths(1);
            tablesResult[0] = tblResult;
            var rowCaption = CreateReportParamsRow(tablesResult);
            rowCaption[0] = year;
            rowCaption[1] = nextMonth.Year;
            rowCaption[2] = GetMonthRusNames()[monthNum - 1];
            rowCaption[3] = GetMonthText2(nextMonth.Month);
            rowCaption[4] = ReportMonthMethods.GetDividerDescr(divider);
            rowCaption[5] = reportHelper.GetKDBridgeCaptionText(clearKD);
            rowCaption[6] = DateTime.Now.ToShortDateString();
            rowCaption[7] = ReportMonthMethods.GetPrecisionIndex(precision);
            return tablesResult;
        }
    }
}

