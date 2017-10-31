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
        /// ОТЧЕТ 018_ВЫВОД СПРОГНОЗИРОВАННЫХ СУММ НА ТЕКУЩИЙ ГОД И ПЛАНОВЫЙ ПЕРИОД ПО ДОХОДНЫМ ИСТОЧНИКАМ
        /// </summary>
        public DataTable[] GetMonthReport018ForecastKDIncomeData(Dictionary<string, string> reportParams)
        {
            const int GroupColumnSize = 5;
            var reportHelper = new ReportMonthMethods(scheme);
            var filterHelper = new QFilterHelper();
            var tablesResult = new DataTable[4];
            var rowCaption = CreateReportParamsRow(tablesResult);
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var variantId = reportParams[ReportConsts.ParamVariantDID];
            var paramLvl = reportParams[ReportConsts.ParamRegionListType];
            var filterKD = reportParams[ReportConsts.ParamKDComparable];
            var sumFieldType =
                (PlanSumFieldEnum) Enum.Parse(typeof (PlanSumFieldEnum), reportParams[ReportConsts.ParamSum]);
            var paramBgtLvls = reportParams[ReportConsts.ParamBdgtLevels];
            var paramRgnLvls = reportParams[ReportConsts.ParamRegionLevels];
            var dbHelper = new ReportDBHelper(scheme);
            var year = ReportMonthMethods.GetSelectedYear(reportParams[ReportConsts.ParamYear]);
            var entKdBridge = ConvertorSchemeLink.GetEntity(b_KD_Bridge.InternalKey);
            var tblCaptions = CreateReportCaptionTable(2);
            var listKBK = filterKD.Split(',');
            var listFullKBK = new string[listKBK.Length];
            var filterFullKD = filterKD;
            var idxKBK = 0;
            var fltKVSR = reportParams[ReportConsts.ParamKVSRComparable];
            fltKVSR = ReportMonthMethods.CheckBookValue(fltKVSR);
            var unvYear = Convert.ToInt32(GetUNVYearLoBound(year)) + 1;
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


            var fltPeriod = filterHelper.EqualIntFilter(f_D_FOPlanIncDivide.RefYearDayUNV, unvYear, true);

            var qParams = new QPlanIncomeParams
                              {
                                  KD = filterFullKD,
                                  Period = fltPeriod,
                                  Lvl = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Summary),
                                  Variant = variantId,
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
            qParams.Lvl = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Settle);
            var querySettleData = QPlanIncomeDivide.GroupBy(qParams, groupInfo);
            var tblSettleDivData = dbHelper.GetTableData(querySettleData);

            var tblResult = reportHelper.CreateRegionList(6 + GroupColumnSize*listKBK.Length);

            var sumField = String.Empty;

            switch (sumFieldType)
            {
                case PlanSumFieldEnum.i1:
                    sumField = f_D_FOPlanIncDivide.Estimate;
                    break;
                case PlanSumFieldEnum.i2:
                    sumField = f_D_FOPlanIncDivide.Forecast;
                    break;
                case PlanSumFieldEnum.i3:
                    sumField = f_D_FOPlanIncDivide.TaxResource;
                    break;
            }

            var splitParams = new RegionSplitParams
                                  {
                                      KeyValIndex = 4,
                                      LvlValIndex = tblIncomeDivData.Columns[f_D_FOPlanIncDivide.RefBudLevel].Ordinal,
                                      TblResult = tblResult,
                                      IsSkifLevels = false,
                                      UseDocumentTypes = false,
                                      SrcColumnIndex = tblIncomeDivData.Columns[sumField].Ordinal
                                  };

            for (var j = 0; j < listKBK.Length; j++)
            {
                var kbk = Convert.ToInt32(listKBK[j]);
                var sumColumn = (j + 1)*GroupColumnSize;
                var fltKBK = filterHelper.RangeFilter(d_KD_PlanIncomes.RefBridge, listFullKBK[j]);
                var tblKBK = DataTableUtils.FilterDataSet(tblIncomeDivData, fltKBK);
                var tblSettleKBK = DataTableUtils.FilterDataSet(tblSettleDivData, fltKBK);
                // МБ
                splitParams.UseLvlDepencity = true;
                splitParams.DstColumnIndex = sumColumn + 2;
                splitParams.RowsData = tblKBK.Select(
                    filterHelper.NotEqualIntFilter(
                        f_D_FOPlanIncDivide.RefBudLevel, 
                        ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject)));
                reportHelper.SplitRegionData(splitParams);
                // ОБ
                splitParams.DstColumnIndex = sumColumn + 1;
                splitParams.UseLvlDepencity = false;
                var fltLvl = filterHelper.RangeFilter(
                    f_D_FOPlanIncDivide.RefBudLevel,
                    ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject));
                splitParams.RowsData = tblKBK.Select(fltLvl);
                reportHelper.SplitRegionData(splitParams);
                // КБ
                splitParams.RowsData = tblKBK.Select();
                splitParams.DstColumnIndex = sumColumn;
                reportHelper.SplitRegionData(splitParams);
                // МР
                fltLvl = filterHelper.RangeFilter(
                    f_D_FOPlanIncDivide.RefBudLevel,
                    ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.MR));
                splitParams.RowsData = tblKBK.Select(fltLvl);
                splitParams.DstColumnIndex = sumColumn + 3;
                reportHelper.SplitRegionData(splitParams);
                // Поселения
                splitParams.RowsData = tblSettleKBK.Select();
                splitParams.DstColumnIndex = sumColumn + 4;
                reportHelper.SplitRegionData(splitParams);

                var rowTitle = reportHelper.GetBookRow(entKdBridge, kbk);

                if (rowTitle != null)
                {
                    var rowKBK = tblCaptions.Rows.Add();
                    rowKBK[0] = rowTitle[b_KD_Bridge.CodeStr];
                    rowKBK[1] = rowTitle[b_KD_Bridge.Name];
                }
            }

            var idxResultColumn = ReportMonthMethods.AbsColumnIndex(0);
            var excludedColumns = new List<int>();

            for (var j = 0; j < listKBK.Length; j++)
            {
                var alreadyExist = false;

                foreach (var kbkLink in kbkRelation)
                {
                    alreadyExist = alreadyExist || kbkLink.Value.Contains(listKBK[j]);
                }

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

                    var idxDataColumn = ReportMonthMethods.AbsColumnIndex(j*GroupColumnSize);

                    for (var i = 0; i < GroupColumnSize; i++)
                    {
                        row[idxResultColumn + i] = GetDecimal(row[idxResultColumn + i]) +
                                                   GetDecimal(row[idxDataColumn + i]);
                    }
                }
            }

            for (var j = 0; j <= listKBK.Length; j++)
            {
                for (var i = 0; i < GroupColumnSize; i++)
                {
                    var asbColumnIndex = ReportMonthMethods.AbsColumnIndex(j * GroupColumnSize + i);
                    DivideColumn(tblResult, asbColumnIndex, divider);
                    RoundColumn(tblResult, asbColumnIndex, precision);
                }
            }

            var tblSubject = ReportMonthMethods.CreateSubjectTable(tblResult);
            reportHelper.ClearSettleRows(tblResult, paramLvl);
            var isSubject = sumFieldType == PlanSumFieldEnum.i1 || sumFieldType == PlanSumFieldEnum.i2;
            tablesResult[0] = tblResult;
            tablesResult[1] = tblCaptions;
            tablesResult[2] = tblSubject;
            rowCaption[0] = year;
            rowCaption[1] = isSubject;
            rowCaption[2] = ReportMonthMethods.WriteSettles(paramLvl);
            rowCaption[3] = reportHelper.GetKDBridgeCaptionText(filterKD);
            rowCaption[4] = ReportMonthMethods.GetDividerDescr(divider);
            rowCaption[5] = listKBK.Count();
            rowCaption[6] = DateTime.Now.ToShortDateString();
            rowCaption[7] = reportParams[ReportConsts.ParamBdgtLevels];
            rowCaption[8] = ReportMonthMethods.GetSelectedKVSR(reportParams[ReportConsts.ParamKVSRComparable]);
            rowCaption[9] = ReportMonthMethods.GetSelectedVariant(reportParams[ReportConsts.ParamVariantDID]);
            // видимость колонок
            rowCaption[10] = isSubject && paramBgtLvls.Contains("0");
            rowCaption[11] = isSubject && paramBgtLvls.Contains("1");
            rowCaption[12] = isSubject && paramBgtLvls.Contains("2");
            rowCaption[13] = !isSubject && paramRgnLvls.Contains("0");
            rowCaption[14] = !isSubject && paramRgnLvls.Contains("1");
            rowCaption[20] = ReportMonthMethods.GetSelectedBudgetLvl(paramBgtLvls);
            rowCaption[21] = ReportMonthMethods.GetSelectedRegionLvl(paramRgnLvls);
            rowCaption[50] = ReportMonthMethods.GetPrecisionIndex(precision);
            return tablesResult;
        }
    }
}
