using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK;
using Krista.FM.Client.Reports.Month;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData.UFK;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        private class ReportMonth25RowInfo
        {
            public bool IsCalcRow { get; set; }
            public int RowIndex { get; set; }
            public string ID { get; set; }
            public List<string> Childs { get; set; }
        }

        /// <summary>
        /// ОТЧЕТ 025_Расчетные показатели
        /// </summary>
        public DataTable[] GetMonthReport025CalcMeasuresData(Dictionary<string, string> reportParams)
        {
            var lstRowsInfo = new List<ReportMonth25RowInfo>[4];
            var tablesResult = new DataTable[4];
            var reportHelper = new ReportMonthMethods(scheme);
            var dbHelper = new ReportDBHelper(scheme);
            var filterHelper = new QFilterHelper();
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var variantId = reportParams[ReportConsts.ParamVariantDID];
            var filterKD = reportParams[ReportConsts.ParamKDComparable];
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

            var year = Convert.ToInt32(rowVariant[d_Variant_PlanIncomes.RefYear]);

            var fltPeriod = filterHelper.PeriodFilter(
                fieldPeriod,
                GetUNVYearPlanLoBound(year), 
                GetUNVYearPlanLoBound(year + 2), 
                true);
            var fltCurYear = filterHelper.PeriodFilter(
                fieldPeriod, 
                GetUNVYearLoBound(year - 1),
                GetUNVYearEnd(year - 1), 
                true);

            var qMonthParams = new QMonthRepParams
            {
                Period = fltCurYear,
                KD = filterKD,
                Lvl = String.Join(",", new[]
                                                   {
                                                       ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.ConsMunicipal),
                                                       ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.Subject)
                                                   }),
                DocType = ReportMonthConsts.DocTypeConsolidate
            };

            var groupMonthInfo = new List<QMonthRepGroup> { QMonthRepGroup.Kd, QMonthRepGroup.Period };
            var queryMonthData = QMonthRepIncomes.GroupBy(qMonthParams, groupMonthInfo);
            var tblMonthData = dbHelper.GetTableData(queryMonthData);
            tblMonthData = ReportMonthMethods.FilterLastMonth(tblMonthData, f_F_MonthRepIncomes.RefYearDayUNV);

            var qParams = new QPlanIncomeParams
            {
                Period = fltPeriod,
                Lvl = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Summary),
                Variant = variantId,
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
            qParams.Period = fltCurYear;
            queryIncomeData = QPlanIncomeDivide.GroupBy(qParams, groupInfo);
            var tblIncomeCurData = dbHelper.GetTableData(queryIncomeData);
            qParams.Variant = ReportMonthConsts.VariantPlan;
            queryIncomeData = QPlanIncomeDivide.GroupBy(qParams, groupInfo);
            var tblIncomeData = dbHelper.GetTableData(queryIncomeData);
            tblIncomeData = ReportMonthMethods.FilterLastMonth(tblIncomeData, f_D_FOPlanIncDivide.RefYearDayUNV);

            var subCode = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject);
            var munCode = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.MR);
            var gooCode = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Town);
            var stlCode = String.Join(",", new[]
                                                   {
                                                       ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.TownSettle),
                                                       ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.VillageSettle)
                                                   });

            var fltIncSub = filterHelper.EqualIntFilter(f_D_FOPlanIncDivide.RefBudLevel, subCode);
            var fltIncMun = filterHelper.EqualIntFilter(f_D_FOPlanIncDivide.RefBudLevel, munCode);
            var fltIncStl = filterHelper.RangeFilter(f_D_FOPlanIncDivide.RefBudLevel, stlCode);
            var fltIncGoo = filterHelper.EqualIntFilter(f_D_FOPlanIncDivide.RefBudLevel, gooCode);

            var cutParams = reportHelper.CreateCutKDParams();
            var kbkSummary = new List<string>();
            var summary = new decimal[4, 50];

            for (var i = 0; i < 4; i++ )
            {
                lstRowsInfo[i] = new List<ReportMonth25RowInfo>();
            }

            foreach (var refKBK in listKBK)
            {
                var rowTitle = reportHelper.GetBookRow(entKdBridge, refKBK);

                cutParams.Key = refKBK;
                var childs = reportHelper.GetChildClsRecord(cutParams);
                var childFilter = refKBK;

                if (childs.Count() > 0)
                {
                    childFilter = childs.Aggregate(childFilter, (current, clsRec) => Combine(current, clsRec.KeyValue, ","));
                    childFilter = childFilter.Trim(',');
                }

                var fltKBKIncome = filterHelper.RangeFilter(d_KD_PlanIncomes.RefBridge, childFilter);
                var tblKBKIncome = DataTableUtils.FilterDataSet(tblIncomeDivData, fltKBKIncome);
                var tblKBKIncCur = DataTableUtils.FilterDataSet(tblIncomeData, fltKBKIncome);
                var fltKBKMonth = filterHelper.RangeFilter(d_KD_MonthRep.RefKDBridge, childFilter);
                var tblKBKMonth = DataTableUtils.FilterDataSet(tblMonthData, fltKBKMonth);
                var tblKBKIncomeCurData = DataTableUtils.FilterDataSet(tblIncomeCurData, fltKBKIncome);

                for (var y = -1; y < 3; y++)
                {
                    var tblResult = tablesResult[y + 1];
                    var rowResult = tblResult.NewRow();
                    rowResult[0] = CombineFieldText(rowTitle, b_KD_Bridge.CodeStr, b_KD_Bridge.Name);
                    var columnOffset = 1;
                    decimal sumIncCurYear = 0;
                    decimal sumMonCurYear = 0;

                    if (y < 0)
                    {
                        var sumColumnIndex = GetColumnIndex(tblKBKIncCur, f_D_FOPlanIncDivide.YearPlan);
                        sumIncCurYear = tblKBKIncCur.Select().Sum(dataRow => GetDecimal(dataRow[sumColumnIndex]));
                        rowResult[columnOffset + 0] = sumIncCurYear;
                        sumColumnIndex = GetColumnIndex(tblKBKMonth, f_F_MonthRepIncomes.YearPlan);
                        sumMonCurYear = tblKBKMonth.Select().Sum(dataRow => GetDecimal(dataRow[sumColumnIndex]));
                        rowResult[columnOffset + 1] = sumMonCurYear;
                        columnOffset = 3;
                    }

                    // ожидаемое
                    var unvYear = GetUNVYearPlanLoBound(year + y);
                    var fltYear = filterHelper.EqualIntFilter(fieldPeriod, unvYear);
                    var tblYear = DataTableUtils.FilterDataSet(tblKBKIncome, fltYear);
                    var sumField = y < 0 ? f_D_FOPlanIncDivide.Estimate : f_D_FOPlanIncDivide.Forecast;
                    var srcColumnIndex = GetColumnIndex(tblYear, sumField);

                    var rowsInc = tblYear.Select(fltYear);

                    if (y < 0)
                    {
                        rowsInc = tblKBKIncomeCurData.Select(filterHelper.EqualIntFilter(f_D_FOPlanIncDivide.RefYearDayUNV, unvYear));
                    }

                    var sum = rowsInc.Sum(dataRow => GetDecimal(dataRow[srcColumnIndex]));
                    rowResult[columnOffset + 0] = sum;
                    // прогноз ОБ
                    unvYear = GetUNVYearPlanLoBound(year + y + 1);
                    fltYear = filterHelper.EqualIntFilter(fieldPeriod, unvYear);
                    tblYear = DataTableUtils.FilterDataSet(tblKBKIncome, fltYear);
                    sumField = f_D_FOPlanIncDivide.Forecast;
                    srcColumnIndex = GetColumnIndex(tblYear, sumField);
                    var sumSub = tblYear.Select(fltIncSub).Sum(dataRow => GetDecimal(dataRow[srcColumnIndex]));
                    var sumMun = tblYear.Select(fltIncMun).Sum(dataRow => GetDecimal(dataRow[srcColumnIndex]));
                    var sumStl = tblYear.Select(fltIncStl).Sum(dataRow => GetDecimal(dataRow[srcColumnIndex]));
                    var sumGoo = tblYear.Select(fltIncGoo).Sum(dataRow => GetDecimal(dataRow[srcColumnIndex]));
                    rowResult[columnOffset + 1] = sumSub;
                    rowResult[columnOffset + 2] = sumMun;
                    rowResult[columnOffset + 3] = sumGoo;
                    rowResult[columnOffset + 4] = sumStl;

                    if (sumSub != 0 || sumMun != 0 || sumStl != 0 || sumGoo != 0)
                    {
                        var rowInfo = new ReportMonth25RowInfo
                                          {
                                              ID = refKBK,
                                              RowIndex = tblResult.Rows.Count,
                                              Childs = new List<string>(childs.Select(child => child.KeyValue))
                                          };

                        lstRowsInfo[y + 1].Add(rowInfo);
                        tblResult.Rows.Add(rowResult);
                    }
                }

                kbkSummary.AddRange(childs.Select(child => child.KeyValue));
            }

            for (var j = 0; j < lstRowsInfo.Length; j++)
            {
                var curInfoList = lstRowsInfo[j];

                for (var i = 0; i < curInfoList.Count; i++)
                {
                    var parentsVisible = curInfoList.Where(f => f.Childs.Contains(curInfoList[i].ID));

                    foreach (var parentInfo in parentsVisible)
                    {
                        parentInfo.IsCalcRow = true;
                    }
                }
            }

            for (var i = 0; i < 3; i++)
            {
                var tblResult = tablesResult[i];
                var curInfoList = lstRowsInfo[i];

                for (var j = 0; j < tblResult.Rows.Count; j++)
                {
                    var curRow = tblResult.Rows[j];

                    if (!curInfoList[j].IsCalcRow)
                    {
                        continue;
                    }

                    for (var k = 1; k < 20; k++)
                    {
                        curRow[k] = 0;
                    }
                }

                foreach (var rowInfo in curInfoList.Where(f => f.IsCalcRow == false))
                {
                    var rowsParent = curInfoList.Where(f => f.Childs.Contains(rowInfo.ID));
                    var curRow = tblResult.Rows[rowInfo.RowIndex];

                    foreach (var rowParent in rowsParent)
                    {
                        var parentRow = tblResult.Rows[rowParent.RowIndex];

                        for (var k = 1; k < 20; k++)
                        {
                            parentRow[k] = GetDecimal(curRow[k]) + GetDecimal(parentRow[k]);
                        }
                    }

                    for (var k = 1; k < 20; k++)
                    {
                        summary[i, k] += GetDecimal(curRow[k]);
                    }
                }

                tblResult = DataTableUtils.SortDataSet(tblResult, tblResult.Columns[0].ColumnName);
                var rowSummary = tblResult.Rows.Add();

                for (var j = 0; j < tblResult.Columns.Count; j++)
                {
                    rowSummary[j] = summary[i, j];
                }

                for (var j = 1; j < tblResult.Columns.Count; j++)
                {
                    DivideColumn(tblResult, j, divider);
                    RoundColumn(tblResult, j, precision);
                }

                tablesResult[i] = tblResult;
            }

            var rowCaption = CreateReportParamsRow(tablesResult);
            rowCaption[0] = year;
            rowCaption[1] = rowVariant[d_Variant_PlanIncomes.Name];
            rowCaption[2] = year;
            rowCaption[4] = ReportMonthMethods.GetDividerDescr(divider);
            rowCaption[6] = DateTime.Now.ToShortDateString();
            rowCaption[7] = ReportMonthMethods.GetPrecisionIndex(precision);
            return tablesResult;
        }
    }
}

