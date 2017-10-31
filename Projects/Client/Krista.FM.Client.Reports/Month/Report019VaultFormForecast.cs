using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 019_СВОДНАЯ ФОРМА ВЫВОДА СПРОГНОЗИРОВАННЫХ СУММ
        /// </summary>
        public DataTable[] GetMonthReport019VaultFormForecastData(Dictionary<string, string> reportParams)
        {
            var tablesResult = new DataTable[2];
            var reportHelper = new ReportMonthMethods(scheme);
            var dbHelper = new ReportDBHelper(scheme);
            var filterHelper = new QFilterHelper();
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var variantId = reportParams[ReportConsts.ParamVariantDID];
            var fltKVSR = reportParams[ReportConsts.ParamKVSRComparable];
            var filterKD = reportParams[ReportConsts.ParamKDComparable];
            fltKVSR = ReportMonthMethods.CheckBookValue(fltKVSR);
            var entKdBridge = ConvertorSchemeLink.GetEntity(b_KD_Bridge.InternalKey);
            const string fieldPeriod = f_D_FOPlanIncDivide.RefYearDayUNV;
            var listKBK = filterKD.Split(',');

            for (var i = 0; i < tablesResult.Length; i++)
            {
                tablesResult[i] = CreateReportCaptionTable(50);
            }

            var entDVariant = ConvertorSchemeLink.GetEntity(d_Variant_PlanIncomes.InternalKey);
            var rowVariant = reportHelper.GetBookRow(entDVariant, variantId);

            if (rowVariant == null)
            {
                return tablesResult;
            }

            var year = Convert.ToInt32(rowVariant[d_Variant_PlanIncomes.RefYear]) - 1;
            var unvYear1 = GetUNVYearPlanLoBound(year);
            var unvYear2 = GetUNVYearPlanLoBound(year + 3);

            var fltPeriod = filterHelper.PeriodFilter(fieldPeriod, unvYear1, unvYear2, true);

            var qParams = new QPlanIncomeParams()
            {
                Period = fltPeriod,
                Lvl = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Summary),
                Variant = variantId,
                KVSR = fltKVSR,
                KD = filterKD
            };

            var groupInfo = new List<QPlanIncomeGroup>()
                                {
                                    QPlanIncomeGroup.Period,
                                    QPlanIncomeGroup.Lvl,
                                    QPlanIncomeGroup.Kd
                                };

            var queryIncomeData = QPlanIncomeDivide.GroupBy(qParams, groupInfo);
            var tblIncomeDivData = dbHelper.GetTableData(queryIncomeData);
            var tblResult = CreateReportCaptionTable(14);
            var subjectCode = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject);
            var fltMB = filterHelper.NotEqualIntFilter(f_D_FOPlanIncDivide.RefBudLevel, subjectCode);
            var fltOB = filterHelper.EqualIntFilter(f_D_FOPlanIncDivide.RefBudLevel, subjectCode);

            var cutParams = reportHelper.CreateCutKDParams();
            var kbkSummary = new List<string>();
            var summary = new decimal[tblResult.Columns.Count];

            foreach (var refKBK in listKBK)
            {
                var isNonEmpty = false;
                var rowResult = tblResult.NewRow();
                var rowTitle = reportHelper.GetBookRow(entKdBridge, refKBK);

                if (rowTitle == null)
                {
                    continue;
                }

                rowResult[0] = rowTitle[b_KD_Bridge.CodeStr];
                rowResult[1] = rowTitle[b_KD_Bridge.Name];

                cutParams.Key = refKBK;
                var childs = reportHelper.GetChildClsRecord(cutParams);
                var childFilter = refKBK;

                if (childs.Count() > 0)
                {
                    childFilter = childs.Aggregate(childFilter, (current, clsRec) => Combine(current, clsRec.KeyValue, ","));
                    childFilter = childFilter.Trim(',');
                }

                var fltKBK = filterHelper.RangeFilter(d_KD_PlanIncomes.RefBridge, childFilter);
                var tblKBK = DataTableUtils.FilterDataSet(tblIncomeDivData, fltKBK);

                for (var i = 0; i < 4; i++)
                {
                    var unvCurrentYear = GetUNVYearPlanLoBound(year + i);
                    var fltCurrentYear = filterHelper.EqualIntFilter(fieldPeriod, unvCurrentYear);
                    var tblYear = DataTableUtils.FilterDataSet(tblKBK, fltCurrentYear);
                    var sumField = i == 0 ? f_D_FOPlanIncDivide.Estimate : f_D_FOPlanIncDivide.Forecast;
                    var rowsKB = tblYear.Select();
                    var rowsOB = tblYear.Select(fltOB);
                    var rowsMB = tblYear.Select(fltMB);
                    var dstColumnIndex = 2 + i * 3;
                    var srcColumnIndex = tblIncomeDivData.Columns[sumField].Ordinal;
                    var sumKB = rowsKB.Sum(dataRow => GetDecimal(dataRow[srcColumnIndex]));
                    var sumOB = rowsOB.Sum(dataRow => GetDecimal(dataRow[srcColumnIndex]));
                    var sumMB = rowsMB.Sum(dataRow => GetDecimal(dataRow[srcColumnIndex]));
                    rowResult[dstColumnIndex + 0] = sumKB;
                    rowResult[dstColumnIndex + 1] = sumOB;
                    rowResult[dstColumnIndex + 2] = sumMB;
                    isNonEmpty = isNonEmpty || sumKB != 0 || sumOB != 0 || sumMB != 0;

                    if (!kbkSummary.Contains(refKBK))
                    {
                        summary[dstColumnIndex + 0] += sumKB;
                        summary[dstColumnIndex + 1] += sumOB;
                        summary[dstColumnIndex + 2] += sumMB;
                    }
                }

                if (isNonEmpty)
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

            for (var j = 2; j < tblResult.Columns.Count; j++)
            {
                DivideColumn(tblResult, j, divider);
            }

            tablesResult[0] = tblResult;
            var rowCaption = CreateReportParamsRow(tablesResult);
            rowCaption[0] = year;
            rowCaption[1] = rowVariant[d_Variant_PlanIncomes.Name];
            rowCaption[2] = rowVariant[d_Variant_PlanIncomes.RefYear];
            rowCaption[4] = ReportMonthMethods.GetDividerDescr(divider);
            rowCaption[6] = DateTime.Now.ToShortDateString();
            rowCaption[8] = ReportMonthMethods.GetSelectedKVSR(reportParams[ReportConsts.ParamKVSRComparable]);
            return tablesResult;
        }
    }
}

