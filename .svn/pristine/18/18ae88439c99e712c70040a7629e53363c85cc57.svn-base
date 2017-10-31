using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Database.ClsData;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 026_ВЫВОД СУММ ПО РАСЧЕТНОМУ НАЛОГОВОМУ ПОТЕНЦИАЛУ
        /// </summary>
        public DataTable[] GetMonthReport026TaxPotentionData(Dictionary<string, string> reportParams)
        {
            const int DataColumnIndex = 2;
            const int ColumnCount = 5;
            var tablesResult = new DataTable[3];
            var reportHelper = new ReportMonthMethods(scheme);
            var dbHelper = new ReportDBHelper(scheme);
            var filterHelper = new QFilterHelper();
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var variantId = reportParams[ReportConsts.ParamVariantDID];
            var outType = reportParams[ReportConsts.ParamOutputMode];
            outType = ReportMonthMethods.CheckBookValue(outType);
            var fltKD = reportParams[ReportConsts.ParamKDComparable];
            fltKD = ReportMonthMethods.CheckBookValue(fltKD);

            if (fltKD.Length == 0)
            {
                var kdBridge = ConvertorSchemeLink.GetEntity(b_KD_Bridge.InternalKey);
                var rowKD = reportHelper.GetBookRow(kdBridge, b_KD_Bridge.CodeStr, "00010000000000000000");
                
                if (rowKD != null)
                {
                    fltKD = reportHelper.GetKDHierarchyFilter(Convert.ToString(rowKD[b_KD_Bridge.ID]));
                }
            }

            var yearType = (VariantYearEnum)Enum.Parse(typeof(VariantYearEnum), reportParams[ReportConsts.ParamYear]);
            var year = reportHelper.GetIncomeVariantYear(variantId);

            switch (yearType)
            {
                case VariantYearEnum.i2:
                    year += 1;
                    break;
                case VariantYearEnum.i3:
                    year += 2;
                    break;
            }

            var loYearBound = GetUNVYearPlanLoBound(year);
            var fltPeriod = filterHelper.EqualIntFilter(f_D_FOPlanIncDivide.RefYearDayUNV, loYearBound, true);
            var codeRegion = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.MR);
            var codeSettle = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Settle);
            var fltDataRegion = filterHelper.EqualIntFilter(f_D_FOPlanIncDivide.RefBudLevel, codeRegion);
            var fltDataSettle = filterHelper.EqualIntFilter(f_D_FOPlanIncDivide.RefBudLevel, codeSettle);
            var codeClsRegion = ReportMonthMethods.GetTerritoryCode(TerritoryType.MrGo);
            var codeClsSettle = ReportMonthMethods.GetTerritoryCode(TerritoryType.Settle);
            var codeGooSettle = ReportMonthMethods.GetTerritoryCode(TerritoryType.Town);
            var gooId = Convert.ToInt32(codeGooSettle);
            var lstSettle = codeClsSettle.Split(',').ToList();
            var fltClsRegion = filterHelper.RangeFilter(b_Regions_Bridge.RefTerrType, codeClsRegion);
            var fltClsSettle = filterHelper.RangeFilter(b_Regions_Bridge.RefTerrType, codeClsSettle);
            var fltGooSettle = filterHelper.RangeFilter(b_Regions_Bridge.RefTerrType, codeGooSettle);
            var rgnKey = b_Regions_Bridge.InternalKey;
            var clsRegn = dbHelper.GetEntityData(rgnKey);
            var terrTypesKey = fx_FX_TerritorialPartitionType.InternalKey;
            var clsTerrType = dbHelper.GetEntityData(terrTypesKey);
            var clsGoo = dbHelper.GetEntityData(rgnKey, fltGooSettle);
            var clsTables = new []
                                {
                                    dbHelper.GetEntityData(rgnKey, fltClsRegion),
                                    dbHelper.GetEntityData(rgnKey, fltClsSettle)
                                };

            clsGoo = DataTableUtils.SortDataSet(clsGoo, b_Regions_Bridge.CodeLine);
            clsTables[1] = DataTableUtils.SortDataSet(clsTables[1], b_Regions_Bridge.Code);

            foreach (DataRow rowGoo in clsGoo.Rows)
            {
                clsTables[1].ImportRow(rowGoo);
            }

            var fltLvl = String.Join(",", new[] { codeRegion, codeSettle });

            var qParams = new QPlanIncomeParams
                              {
                                  Lvl = fltLvl,
                                  Variant = variantId,
                                  KD = fltKD,
                                  Period = fltPeriod
                              };

            var groupInfo = new List<QPlanIncomeGroup>
                                {
                                    QPlanIncomeGroup.Region, QPlanIncomeGroup.Lvl
                                };

            var queryIncomeData = QPlanIncomeDivide.GroupBy(qParams, groupInfo);
            var tblIncData = dbHelper.GetTableData(queryIncomeData);

            var dataTables = new[]
                                {
                                    DataTableUtils.FilterDataSet(tblIncData, fltDataRegion),
                                    DataTableUtils.FilterDataSet(tblIncData, fltDataSettle)
                                };

            var counter = 1;

            for (var i = 0; i < clsTables.Length; i++)
            {
                var tblResult = CreateReportCaptionTable(ColumnCount);

                if (i == 0)
                {
                    clsTables[i] = DataTableUtils.SortDataSet(clsTables[i], b_Regions_Bridge.CodeLine);
                }

                foreach (DataRow rowRegion in clsTables[i].Rows)
                {
                    var isSettle = lstSettle.Contains(Convert.ToString(rowRegion[b_Regions_Bridge.RefTerrType]));
                    var rowResult = tblResult.Rows.Add();
                    rowResult[0] = i > 0 ? counter++ : rowRegion[b_Regions_Bridge.CodeLine];
                    rowResult[1] = rowRegion[b_Regions_Bridge.Name];
                    rowResult[3] = rowRegion[b_Regions_Bridge.CodeLine];
                    rowResult[4] = Convert.ToInt32(rowRegion[b_Regions_Bridge.RefTerrType]) == gooId;

                    if (isSettle)
                    {
                        var fltParent = filterHelper.EqualIntFilter(
                            b_Regions_Bridge.ID,
                            rowRegion[b_Regions_Bridge.ParentID]);
                        var rowsParent = clsRegn.Select(fltParent);
                        fltParent = filterHelper.EqualIntFilter(
                            b_Regions_Bridge.ID,
                            rowsParent[0][b_Regions_Bridge.ParentID]);
                        rowsParent = clsRegn.Select(fltParent);

                        if (rowsParent.Length > 0)
                        {
                            var rowParent = rowsParent[0];
                            var terrFilter = filterHelper.EqualIntFilter(
                                fx_FX_TerritorialPartitionType.ID,
                                rowParent[b_Regions_Bridge.RefTerrType]);
                            var rowsTerrType = clsTerrType.Select(terrFilter);
                            rowResult[1] = String.Format("{0} ({1} {2})", 
                                rowResult[1],
                                rowParent[b_Regions_Bridge.Name],
                                rowsTerrType[0][fx_FX_TerritorialPartitionType.Name]);
                            rowResult[3] = rowParent[b_Regions_Bridge.CodeLine];
                        }
                    }

                    var fltRegion = filterHelper.EqualIntFilter(d_Regions_Plan.RefBridge, rowRegion[b_Regions_Bridge.ID]);
                    var rowsData = dataTables[i].Select(fltRegion);
                    rowResult[2] = rowsData.Sum(dataRow => GetDecimal(dataRow[f_D_FOPlanIncDivide.TaxResource]));
                }

                DivideColumn(tblResult, DataColumnIndex, divider);
                RoundColumn(tblResult, DataColumnIndex, precision);
                tablesResult[i] = tblResult;
            }

            var rowCaption = CreateReportParamsRow(tablesResult);
            rowCaption[0] = year;
            rowCaption[1] = reportHelper.GetKDBridgeCaptionText(fltKD);
            rowCaption[2] = reportHelper.GetIncomeVariantName(variantId);
            rowCaption[4] = ReportMonthMethods.GetDividerDescr(divider);
            rowCaption[5] = outType.Length == 0 || outType.Contains("0");
            rowCaption[6] = outType.Length == 0 || outType.Contains("1");
            rowCaption[7] = ReportMonthMethods.GetPrecisionIndex(precision);
            return tablesResult;
        }
    }
}

