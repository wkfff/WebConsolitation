using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData.UFK;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 021_АНАЛИЗ ПОСТУПЛЕНИЙ В БЮДЖЕТ (КБС, ОБ, МБ) СПРОГНОЗИРОВАННЫХ СУММ
        /// </summary>
        public DataTable[] GetMonthReport021AnalysIncomeData(Dictionary<string, string> reportParams)
        {
            var tablesResult = new DataTable[2];
            var tblResult = CreateReportCaptionTable(9);
            var reportHelper = new ReportMonthMethods(scheme);
            var dbHelper = new ReportDBHelper(scheme);
            var filterHelper = new QFilterHelper();
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var variantId = reportParams[ReportConsts.ParamVariantDID];
            var filterKD = reportParams[ReportConsts.ParamKDComparable];
            filterKD = ReportMonthMethods.CheckBookValue(filterKD);
            var clearKD = filterKD;
            var filterGroup = reportParams[ReportConsts.ParamGroupKD];
            filterGroup = ReportMonthMethods.CheckBookValue(filterGroup);
            var entKdBridge = ConvertorSchemeLink.GetEntity(b_KD_Bridge.InternalKey);
            var lvl = Convert.ToInt32(reportParams[ReportConsts.ParamBdgtLevels]);
            var lvlMon = ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.MRTotal);
            var lvlInc = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Summary);

            switch (lvl)
            {
                case 1:
                    lvlMon = ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.Subject);
                    lvlInc = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject);
                    break;
                case 2:
                    lvlMon = ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.ConsMunicipal);
                    lvlInc = String.Join(",", new []
                                                   {
                                                       ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Municipal),
                                                       ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.TownSettle),
                                                       ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.VillageSettle)
                                                   });
                    break;
            }

            clearKD = reportHelper.CreateGroupKBKFilters(ref filterKD, filterGroup);
            var cutParams = reportHelper.CreateCutKDParams();
            var listKBK = clearKD.Split(',');

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

            var year = Convert.ToInt32(rowVariant[d_Variant_PlanIncomes.RefYear]);

            var qParams = new QPlanIncomeParams()
            {
                Lvl = lvlInc,
                Variant = variantId,
                KD = filterKD
            };

            var groupInfo = new List<QPlanIncomeGroup>()
                                {
                                    QPlanIncomeGroup.Kd
                                };

            var tblIncData = new DataTable[4];
            var tblMonData = new DataTable[1];
            var tblFOYData = new DataTable[2];

            for (var i = -1; i < 3; i++)
            {
                qParams.Period = filterHelper.EqualIntFilter(f_D_FOPlanIncDivide.RefYearDayUNV,  GetUNVYearPlanLoBound(year + i),  true);
                var queryIncomeData = QPlanIncomeDivide.GroupBy(qParams, groupInfo);
                tblIncData[i + 1] = dbHelper.GetTableData(queryIncomeData);
            }

            var qMonthParams = new QMonthRepParams
            {
                KD = filterKD,
                Lvl = lvlMon,
                DocType = "3"
            };

            var qFOYRParams = new QFOYRParams
            {
                KD = filterKD,
                Lvl = lvlMon,
                DocType = "3"
            };

            var groupMonthInfo = new List<QMonthRepGroup> { QMonthRepGroup.Kd };
            var groupFOYRInfo = new List<QFOYRGroup> { QFOYRGroup.Kd };

            var maxMon1 = reportHelper.GetMaxMonth(f_F_MonthRepIncomes.InternalKey, f_F_MonthRepIncomes.RefYearDayUNV, year - 1);
            var maxMon2 = reportHelper.GetMaxMonth(f_D_FOPlanIncDivide.InternalKey, f_D_FOPlanIncDivide.RefYearDayUNV, year - 1);

            qMonthParams.Period = filterHelper.EqualIntFilter(f_F_MonthRepIncomes.RefYearDayUNV, GetUNVMonthStart(year - 1, maxMon1), true);
            var queryMonthData = QMonthRepIncomes.GroupBy(qMonthParams, groupMonthInfo);
            tblMonData[0] = dbHelper.GetTableData(queryMonthData);
            
            for (var i = -2; i < 0; i++)
            {
                var curYear = year + i;
                qFOYRParams.Period = filterHelper.EqualIntFilter(f_D_FOYRIncomes.RefYearDayUNV, GetUNVYearPlanLoBound(curYear), true);
                var queryYrText = QFOYRIncomes.GroupBy(qFOYRParams, groupFOYRInfo);
                tblFOYData[i + 2] = dbHelper.GetTableData(queryYrText);
            }

            var kbkSummary = new List<string>();
            var summary = new decimal[tblResult.Columns.Count];

            foreach (var refKBK in listKBK)
            {
                var rowTitle = reportHelper.GetBookRow(entKdBridge, refKBK);

                if (rowTitle == null)
                {
                    continue;
                }

                var rowResult = tblResult.NewRow();
                rowResult[0] = rowTitle[b_KD_Bridge.CodeStr];
                rowResult[1] = rowTitle[b_KD_Bridge.Name];

                cutParams.Key = refKBK;
                var childs = reportHelper.GetChildClsRecord(cutParams);
                var childFilter = refKBK;

                if (childs.Count() > 0)
                {
                    childFilter = childs.Aggregate(childFilter,
                                                   (current, clsRec) => Combine(current, clsRec.KeyValue, ","));
                    childFilter = childFilter.Trim(',');
                }

                var fltMonKBK = filterHelper.RangeFilter(d_KD_MonthRep.RefKDBridge, childFilter);
                var rowsMonYr = tblMonData[0].Select(fltMonKBK);

                var fltFOYKBK = filterHelper.RangeFilter(d_KD_FOYR.RefKDBridge, childFilter);
                var rowsFOYYr1 = tblFOYData[0].Select(fltFOYKBK);
                var rowsFOYYr2 = tblFOYData[1].Select(fltFOYKBK);

                var sumMon1 = rowsFOYYr1.Sum(dataRow => GetDecimal(dataRow[f_D_FOYRIncomes.Performed]));

                var sumMon2 = rowsMonYr.Sum(dataRow => GetDecimal(dataRow[f_F_MonthRepIncomes.YearPlan]));
                var sumMon3 = rowsMonYr.Sum(dataRow => GetDecimal(dataRow[f_F_MonthRepIncomes.Fact]));

                if (tblFOYData[1].Rows.Count > 0)
                {
                    sumMon2 = rowsFOYYr2.Sum(dataRow => GetDecimal(dataRow[f_D_FOYRIncomes.Assigned]));
                }

                if (tblFOYData[1].Rows.Count > 0)
                {
                    sumMon3 = rowsFOYYr2.Sum(dataRow => GetDecimal(dataRow[f_D_FOYRIncomes.Performed]));
                }

                var fltIncKBK = filterHelper.RangeFilter(d_KD_PlanIncomes.RefBridge, childFilter);
                var rowsIncYrB1 = tblIncData[0].Select(fltIncKBK);
                var rowsIncYrB0 = tblIncData[1].Select(fltIncKBK);
                var rowsIncYrA1 = tblIncData[2].Select(fltIncKBK);
                var rowsIncYrA2 = tblIncData[3].Select(fltIncKBK);
                var sumInc1 = rowsIncYrB1.Sum(dataRow => GetDecimal(dataRow[f_D_FOPlanIncDivide.Estimate]));
                var sumInc2 = rowsIncYrB0.Sum(dataRow => GetDecimal(dataRow[f_D_FOPlanIncDivide.Forecast]));
                var sumInc3 = rowsIncYrA1.Sum(dataRow => GetDecimal(dataRow[f_D_FOPlanIncDivide.Forecast]));
                var sumInc4 = rowsIncYrA2.Sum(dataRow => GetDecimal(dataRow[f_D_FOPlanIncDivide.Forecast]));

                rowResult[2] = sumMon1;
                rowResult[3] = sumMon2;
                rowResult[4] = sumMon3;
                rowResult[5] = sumInc1;
                rowResult[6] = sumInc2;
                rowResult[7] = sumInc3;
                rowResult[8] = sumInc4;

                tblResult.Rows.Add(rowResult);

                if (!kbkSummary.Contains(refKBK))
                {
                    summary[2] += sumMon1;
                    summary[3] += sumMon2;
                    summary[4] += sumMon3;
                    summary[5] += sumInc1;
                    summary[6] += sumInc2;
                    summary[7] += sumInc3;
                    summary[8] += sumInc4;
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
                RoundColumn(tblResult, j, precision);
            }

            tablesResult[0] = tblResult;
            var rowCaption = CreateReportParamsRow(tablesResult);
            rowCaption[0] = year;
            rowCaption[1] = rowVariant[d_Variant_PlanIncomes.Name];
            rowCaption[2] = rowVariant[d_Variant_PlanIncomes.RefYear];
            rowCaption[4] = ReportMonthMethods.GetDividerDescr(divider);
            rowCaption[6] = DateTime.Now.ToShortDateString();
            rowCaption[10] = maxMon1;
            rowCaption[11] = maxMon2;
            return tablesResult;
        }
    }
}

