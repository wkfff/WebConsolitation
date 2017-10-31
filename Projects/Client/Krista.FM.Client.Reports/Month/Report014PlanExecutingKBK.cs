using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Month;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsData.UFK;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 014_ИСПОЛНЕНИЕ ПЛАНА КОНСОЛИДИРОВАННОГО БЮДЖЕТА ОБЛАСТИ
        /// </summary>
        public DataTable[] GetMonthReport014PlanExecutingKBKData(Dictionary<string, string> reportParams)
        {
            var tblResult = CreateReportCaptionTable(16);
            var filterHelper = new QFilterHelper();
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramLvl = reportParams[ReportConsts.ParamBdgtLevels];
            var filterLvl = ReportMonthMethods.GetBdgtLvlSKIFCodes(paramLvl);
            var filterKD = reportParams[ReportConsts.ParamKDComparable];
            var year = ReportMonthMethods.GetSelectedYear(reportParams[ReportConsts.ParamYear]);
            var yearLoBound = GetUNVYearLoBound(year);
            var yearHiBound = GetUNVYearEnd(year);
            var yearPeriod = filterHelper.GetYearFilter(yearLoBound, yearHiBound);
            var dbHelper = new ReportDBHelper(scheme);
            var entKdBridge = ConvertorSchemeLink.GetEntity(b_KD_Bridge.InternalKey);

            var qParams = new QMonthRepParams
            {
                Period = yearPeriod,
                Lvl = filterLvl,
                KD = filterKD,
                DocType = ReportMonthConsts.DocTypeConsolidate
            };

            var groupInfo = new List<QMonthRepGroup>
                                {
                                    QMonthRepGroup.Period,
                                    QMonthRepGroup.Kd
                                };

            var queryIncomeData = QMonthRepIncomes.GroupBy(qParams, groupInfo);
            var tblDataMonth = dbHelper.GetTableData(queryIncomeData);
            var kbkList = filterKD.Split(',').ToList();
            var factIndex = GetColumnIndex(tblDataMonth, f_F_MonthRepIncomes.Fact);
            var cutParams = reportHelper.CreateCutKDParams();
            var summary = new decimal[16];
            var kbkSummary = new List<string>();

            // РезультатДоходовСРасщеплением
            var qIncParams = new QPlanIncomeParams
            {
                Period = yearPeriod,
                Lvl = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Summary),
                KD = filterKD
            };
            var groupIncInfo = new List<QPlanIncomeGroup>
                                {
                                    QPlanIncomeGroup.Period,
                                    QPlanIncomeGroup.Kd
                                };
            var queryIncDivData = QPlanIncomeDivide.GroupBy(qIncParams, groupIncInfo);
            var tblIncomeDivData = dbHelper.GetTableData(queryIncDivData);
            var planIndex = GetColumnIndex(tblIncomeDivData, f_D_FOPlanIncDivide.YearPlan);

            var planLoPeriod = GetUNVMonthStart(year, 1);
            var planHiPeriod = Convert.ToInt32(planLoPeriod);
            if (tblIncomeDivData.Rows.Count > 0)
            {
                planHiPeriod = tblIncomeDivData.Select()
                    .Max(dataRow => Convert.ToInt32(dataRow[f_D_FOPlanIncDivide.RefYearDayUNV]));
            }

            var planLastDate = String.Empty;

            if (planHiPeriod > 0)
            {
                planLastDate = GetNormalDate(Convert.ToString(planHiPeriod)).AddMonths(1).ToShortDateString();
            }

            var fltLoPlanPeriod = filterHelper.EqualIntFilter(f_D_FOPlanIncDivide.RefYearDayUNV, planLoPeriod);
            var fltHiPlanPeriod = filterHelper.EqualIntFilter(f_D_FOPlanIncDivide.RefYearDayUNV, planHiPeriod);

            var tblPlanLo = DataTableUtils.FilterDataSet(tblIncomeDivData, fltLoPlanPeriod);
            var tblPlanHi = DataTableUtils.FilterDataSet(tblIncomeDivData, fltHiPlanPeriod);

            foreach (var kbk in kbkList)
            {
                var nonEmpty = false;
                var rowResult = tblResult.NewRow();
                var rowTitle = reportHelper.GetBookRow(entKdBridge, kbk);

                if (rowTitle == null)
                {
                    continue;
                }

                rowResult[0] = rowTitle[b_KD_Bridge.CodeStr];
                rowResult[1] = rowTitle[b_KD_Bridge.Name];

                cutParams.Key = kbk;
                var childs = reportHelper.GetChildClsRecord(cutParams);
                var childFilter = kbk;
                var childIncFilter = kbk;

                if (childs.Count() > 0)
                {
                    childFilter = childs.Aggregate(String.Empty, (current, clsRec) => Combine(current, clsRec.KeyValue, ","));
                    childFilter = childFilter.Trim(',');
                    childIncFilter = String.Join(",", new string[] { childIncFilter, childFilter });
                }

                var fltIncDivKBK = filterHelper.RangeFilter(d_KD_PlanIncomes.RefBridge, childIncFilter);
                var sumLoPlan = tblPlanLo.Select(fltIncDivKBK).Sum(dataRow => GetDecimal(dataRow[planIndex]));
                var sumHiPlan = tblPlanHi.Select(fltIncDivKBK).Sum(dataRow => GetDecimal(dataRow[planIndex]));

                rowResult[2] = sumLoPlan;
                rowResult[3] = sumHiPlan;
                var fltKBK = filterHelper.RangeFilter(d_KD_MonthRep.RefKDBridge, childFilter);
                var tblKBK = DataTableUtils.FilterDataSet(tblDataMonth, fltKBK);

                nonEmpty = sumLoPlan != 0 || sumHiPlan != 0;

                if (!kbkSummary.Contains(kbk))
                {
                    summary[2] += sumLoPlan;
                    summary[3] += sumHiPlan;
                }

                for (var i = 1; i < 13; i++)
                {
                    var periodStart = GetUNVMonthStart(year, i);
                    var filterMonth = filterHelper.EqualIntFilter(f_F_MonthRepIncomes.RefYearDayUNV, periodStart);
                    var rowsMonth = tblKBK.Select(filterMonth);
                    var sum = rowsMonth.Sum(dataRow => GetDecimal(dataRow[factIndex]));
                    rowResult[3 + i] = sum;
                    nonEmpty = nonEmpty || sum != 0;

                    if (!kbkSummary.Contains(kbk))
                    {
                        summary[3 + i] += sum;
                    }
                }

                if (nonEmpty)
                {
                    tblResult.Rows.Add(rowResult);
                }

                kbkSummary.AddRange(childs.Select(child => child.KeyValue));
            }

            tblResult = DataTableUtils.SortDataSet(tblResult, tblResult.Columns[0].ColumnName);
            var rowSummary = tblResult.Rows.Add();

            for (var i = 0; i < summary.Length; i++)
            {
                rowSummary[i] = summary[i];
            }

            for (var i = 2; i < tblResult.Columns.Count; i++)
            {
                DivideColumn(tblResult, i, divider);
                RoundColumn(tblResult, i, precision);
            }

            tablesResult[0] = tblResult;
            rowCaption[0] = year;
            rowCaption[1] = reportHelper.GetKDBridgeCaptionText(filterKD);
            rowCaption[2] = ReportMonthMethods.GetSelectedBudgetLvl(paramLvl);
            rowCaption[4] = ReportMonthMethods.GetDividerDescr(divider);
            rowCaption[5] = planLastDate;
            rowCaption[7] = ReportMonthMethods.GetPrecisionIndex(precision);
            return tablesResult;
        }
    }
}
