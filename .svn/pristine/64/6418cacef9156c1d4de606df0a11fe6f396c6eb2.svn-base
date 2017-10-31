using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsBridge;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 017_ВЫВОД СУММ ПЛАНА НА ТЕКУЩИЙ ГОД ПО ДОХОДНЫМ ИСТОЧНИКАМ
        /// </summary>
        public DataTable[] GetMonthReport017PlanKBKData(Dictionary<string, string> reportParams)
        {
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var filterHelper = new QFilterHelper();
            var tablesResult = new DataTable[4];
            var rowCaption = CreateReportParamsRow(tablesResult);
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramLvl = reportParams[ReportConsts.ParamRegionListType];
            var filterKD = reportParams[ReportConsts.ParamKDComparable];
            var fltKVSR = reportParams[ReportConsts.ParamKVSRComparable];
            fltKVSR = ReportMonthMethods.CheckBookValue(fltKVSR);
            var year = ReportMonthMethods.GetSelectedYear(reportParams[ReportConsts.ParamYear]);
            var entKdBridge = ConvertorSchemeLink.GetEntity(b_KD_Bridge.InternalKey);
            var tblCaptions = CreateReportCaptionTable(2);
            var monthItem = reportParams[ReportConsts.ParamMonth];
            var monthNum = GetEnumItemIndex(new MonthEnum(), monthItem) + 1;
            var listKBK = filterKD.Split(',');
            var listFullKBK = new string[listKBK.Length];
            var filterFullKD = filterKD;
            var idxKBK = 0;

            rowCaption[0] = String.Empty;
            rowCaption[1] = String.Empty;

            var cutParams = reportHelper.CreateCutKDParams();
            var kbkRelation = new Dictionary<string, IEnumerable<string>>();

            foreach (var kbk in listKBK)
            {
                cutParams.Key = kbk;
                var childsKBK = reportHelper.GetChildClsRecord(cutParams);
                listFullKBK[idxKBK] = kbk;
                var listChild = new List<string>();

                foreach (var childKBK in childsKBK)
                {
                    listFullKBK[idxKBK] = Combine(listFullKBK[idxKBK], childKBK.KeyValue);
                    filterFullKD = Combine(filterFullKD, childKBK.KeyValue);
                    listChild.Add(childKBK.KeyValue);
                }

                kbkRelation.Add(kbk, listChild);
                idxKBK++;
            }


            var fltPeriod = filterHelper.PeriodFilter(
                f_D_FOPlanIncDivide.RefYearDayUNV,
                GetUNVYearStart(year),
                GetUNVMonthStart(year, monthNum),
                true);

            var qParams = new QPlanIncomeParams
                              {
                                  KD = filterFullKD,
                                  Period = fltPeriod,
                                  Lvl = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Summary),
                                  KVSR = fltKVSR
                              };

            var groupInfo = new List<QPlanIncomeGroup>
                                {
                                    QPlanIncomeGroup.Kd,
                                    QPlanIncomeGroup.Period,
                                    QPlanIncomeGroup.Lvl,
                                    QPlanIncomeGroup.Region
                                };
            var queryIncomeData = QPlanIncomeDivide.GroupBy(qParams, groupInfo);
            var tblIncomeDivData = dbHelper.GetTableData(queryIncomeData);
            var rowData = tblIncomeDivData.Select();

            var tblResult = reportHelper.CreateRegionList(3 + 3 * listKBK.Length);

            var splitParams = new RegionSplitParams
                                  {
                                      KeyValIndex = 4,
                                      LvlValIndex = GetColumnIndex(tblIncomeDivData, f_D_FOPlanIncDivide.RefBudLevel),
                                      TblResult = tblResult,
                                      IsSkifLevels = false,
                                      UseDocumentTypes = false,
                                      SrcColumnIndex = 0
                                  };

            var actualMonth = -1;

            for (var i = 1; i <= monthNum; i++)
            {
                var periodStart = Convert.ToInt32(GetUNVMonthStart(year, i));

                var monthData =
                    from row in rowData
                    let dateVal = Convert.ToInt32(row[f_D_FOPlanIncDivide.RefYearDayUNV])
                    where dateVal == periodStart
                    select row;

                var currentData = monthData.ToList();

                if (currentData.Count > 0)
                {
                    actualMonth = i;
                }
            }

            var tblLastData = DataTableUtils.FilterDataSet(tblIncomeDivData,
                                                           filterHelper.EqualIntFilter(
                                                               f_D_FOPlanIncDivide.RefYearDayUNV,
                                                               GetUNVMonthStart(year, actualMonth)));

            if (actualMonth > 0)
            {
                for (var j = 0; j < listKBK.Length; j++)
                {
                    var fltKBK = filterHelper.RangeFilter(d_KD_PlanIncomes.RefBridge, listFullKBK[j]);
                    var tblKBK = DataTableUtils.FilterDataSet(tblLastData, fltKBK);

                    var kbk = Convert.ToInt32(listKBK[j]);
                    var sumColumn = (j + 1) * 3;
                    splitParams.DstColumnIndex = sumColumn + 1;
                    splitParams.UseLvlDepencity = false;

                    var fltLvl = filterHelper.RangeFilter(
                        f_D_FOPlanIncDivide.RefBudLevel,
                        ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject));

                    splitParams.RowsData = tblKBK.Select(fltLvl);
                    reportHelper.SplitRegionData(splitParams);
                    splitParams.UseLvlDepencity = true;
                    splitParams.DstColumnIndex = sumColumn + 2;
                    splitParams.RowsData = tblKBK.Select(filterHelper.NotEqualIntFilter(
                        f_D_FOPlanIncDivide.RefBudLevel,
                        ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject)));
                    reportHelper.SplitRegionData(splitParams);
                    splitParams.UseLvlDepencity = false;
                    splitParams.RowsData = tblKBK.Select();
                    splitParams.DstColumnIndex = sumColumn;
                    reportHelper.SplitRegionData(splitParams);
                    var rowTitle = reportHelper.GetBookRow(entKdBridge, kbk);

                    if (rowTitle == null)
                    {
                        continue;
                    }

                    var rowKBK = tblCaptions.Rows.Add();
                    rowKBK[0] = rowTitle[b_KD_Bridge.CodeStr];
                    rowKBK[1] = rowTitle[b_KD_Bridge.Name];
                }

                var idxResultColumn = ReportMonthMethods.AbsColumnIndex(0);
                var excludedColumns = new List<int>();

                for (var j = 0; j < listKBK.Length; j++)
                {
                    var alreadyExist = kbkRelation.Aggregate(false, (current, kbkLink) => 
                        current || kbkLink.Value.Contains(listKBK[j]));

                    if (alreadyExist)
                    {
                        excludedColumns.Add(j);
                    }
                }

                foreach (DataRow row in tblResult.Rows)
                {
                    for (var j = 1; j <= listKBK.Length; j++)
                    {
                        if (excludedColumns.Contains(j - 1))
                        {
                            continue;
                        }

                        var idxDataColumn = ReportMonthMethods.AbsColumnIndex(j * 3);

                        for (var i = 0; i < 3; i++)
                        {
                            var val1 = row[idxResultColumn + i];
                            var val2 = row[idxDataColumn + i];
                            row[idxResultColumn + i] = GetDecimal(val1) + GetDecimal(val2);
                        }
                    }
                }

                for (var j = 0; j <= listKBK.Length; j++)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        var absColumnIndex = ReportMonthMethods.AbsColumnIndex(j * 3 + i);
                        DivideColumn(tblResult, absColumnIndex, divider);
                        RoundColumn(tblResult, absColumnIndex, precision);
                    }
                }

                var nxtMonth = new DateTime(year, actualMonth, 1).AddMonths(1);
                rowCaption[0] = nxtMonth.Year;
                rowCaption[1] = GetMonthText2(nxtMonth.Month);
            }

            var tblSubject = ReportMonthMethods.CreateSubjectTable(tblResult);
            reportHelper.ClearSettleRows(tblResult, paramLvl);

            tablesResult[0] = tblResult;
            tablesResult[1] = tblCaptions;
            tablesResult[2] = tblSubject;
            rowCaption[2] = ReportMonthMethods.WriteSettles(paramLvl);
            rowCaption[3] = reportHelper.GetKDBridgeCaptionText(filterKD);
            rowCaption[4] = ReportMonthMethods.GetDividerDescr(divider);
            rowCaption[5] = listKBK.Count();
            rowCaption[6] = DateTime.Now.ToShortDateString();
            rowCaption[7] = reportParams[ReportConsts.ParamBdgtLevels];
            rowCaption[8] = ReportMonthMethods.GetPrecisionIndex(precision);
            rowCaption[9] = ReportMonthMethods.GetSelectedKVSR(fltKVSR);
            return tablesResult;
        }
    }
}
