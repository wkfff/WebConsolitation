using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.Month;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Database.ClsData.UFK;
using Krista.FM.Client.Reports.Database.ClsData;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 008_ИСПОЛНЕНИЕ МЕСТНОГО БЮДЖЕТА В РАЗРЕЗЕ МУНИЦИПАЛЬНЫХ ОБРАЗОВАНИЙ
        /// </summary>
        public DataTable[] GetMonthReport008ExecutingMunicipalBudgetData(Dictionary<string, string> reportParams)
        {
            var dbHelper = new ReportDBHelper(scheme);
            const int GroupCount = 5;
            const int ColumnCount = GroupCount * 12;
            var reportHelper = new ReportMonthMethods(scheme);
            var filterHelper = new QFilterHelper();
            var tablesResult = new DataTable[5];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramLvl = reportParams[ReportConsts.ParamRegionListType];
            var filterKD = reportParams[ReportConsts.ParamKDComparable];
            var year = ReportMonthMethods.GetSelectedYear(reportParams[ReportConsts.ParamYear]);
            var yearLoBound = GetUNVYearStart(year);
            var yearHiBound = GetUNVYearEnd(year);
            var yearPeriod = filterHelper.GetYearFilter(yearLoBound, yearHiBound);

            // МесОтч
            var qMonthParams = new QMonthRepParams
            {
                Period = yearPeriod,
                KD = filterKD,
                Lvl = ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.Summary),
                DocType = ReportMonthConsts.DocTypeFull
            };
            var groupMonthInfo = new List<QMonthRepGroup>
                                     {
                                         QMonthRepGroup.Period, 
                                         QMonthRepGroup.Lvl,
                                         QMonthRepGroup.DocType,
                                         QMonthRepGroup.Region
                                     };
            var queryMonText = QMonthRepIncomes.GroupBy(qMonthParams, groupMonthInfo);
            var tblMonthRepData = dbHelper.GetTableData(queryMonText);
            var yearList = new List<int> { year - 2, year - 1, year };
            var yearPeriodFilter = filterHelper.GetMultiYearFilter(yearList);
            qMonthParams.Period = yearPeriodFilter;
            var queryYearIncText = QMonthRepIncomes.GroupBy(qMonthParams, groupMonthInfo);
            var tblYearData = dbHelper.GetTableData(queryYearIncText);
            // ГодОтч
            var foyrPeriod = filterHelper.GetYearFilter(GetUNVYearLoBound(year), yearHiBound);
            var qFOYRParams = new QFOYRParams
            {
                Period = foyrPeriod,
                KD = filterKD,
                Lvl = ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.Summary),
                DocType = ReportMonthConsts.DocTypeFull
            };
            var groupFOYRInfo = new List<QFOYRGroup>
                                    {
                                        QFOYRGroup.Period, 
                                        QFOYRGroup.DocType, 
                                        QFOYRGroup.Lvl, 
                                        QFOYRGroup.Region
                                    };
            var queryFOYRText = QFOYRIncomes.GroupBy(qFOYRParams, groupFOYRInfo);
            var tblFOYRData = dbHelper.GetTableData(queryFOYRText);
            // РезультатДоходовСРасщеплением
            var qParams = new QPlanIncomeParams
            {
                Period = yearPeriod,
                Lvl = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Summary),
                KD = filterKD
            };
            var groupInfo = new List<QPlanIncomeGroup>
                                {
                                    QPlanIncomeGroup.Period,
                                    QPlanIncomeGroup.Lvl,
                                    QPlanIncomeGroup.Region
                                };
            var queryIncomeData = QPlanIncomeDivide.GroupBy(qParams, groupInfo);
            var tblIncomeDivData = dbHelper.GetTableData(queryIncomeData);

            var tblResult = reportHelper.CreateRegionList(ColumnCount);
            var tblYearResult = reportHelper.CreateRegionList(12);

            var splitParams = new RegionSplitParams
            {
                TblResult = tblResult
            };

            var subjectCodeDiv = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject);
            var fltMBDiv = filterHelper.NotEqualIntFilter(f_D_FOPlanIncDivide.RefBudLevel, subjectCodeDiv);
            var fltOBDiv = filterHelper.EqualIntFilter(f_D_FOPlanIncDivide.RefBudLevel, subjectCodeDiv);

            var subjectCodeMon = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject);
            var fltMBMon = filterHelper.NotEqualIntFilter(f_F_MonthRepIncomes.RefBdgtLevels, subjectCodeMon);

            var yearUfkPeriodFilter = filterHelper.GetAbsMultiYearFilter(yearList);
            var qUfkParams = new QUFK22Params
            {
                KD = filterKD,
                Period = yearUfkPeriodFilter,
                Mark = ReportMonthConsts.MarkUfkIncome
            };

            var groupFields = new List<QUFK22Group> { QUFK22Group.Region, QUFK22Group.Period };
            var queryUFK22Text = QUFK22.GroupBy(qUfkParams, groupFields);
            var tblUKF22Data = dbHelper.GetTableData(queryUFK22Text);

            var maxMonth = 0;

            for (var i = 1; i < 13; i++)
            {
                var currentColumn = (i - 1) * GroupCount;
                var periodStart = GetUNVMonthStart(year, i);
                var filterMonth = filterHelper.EqualIntFilter(f_F_MonthRepIncomes.RefYearDayUNV, periodStart);
                var tblMonIncData = DataTableUtils.FilterDataSet(tblMonthRepData, filterMonth);
                var tblDivIncData = DataTableUtils.FilterDataSet(tblIncomeDivData, filterMonth);
                var rowsMonthMBData = tblMonIncData.Select(fltMBMon);
                var rowsIncomeOBDiv = tblDivIncData.Select(fltOBDiv);
                var rowsIncomeMBDiv = tblDivIncData.Select(fltMBDiv);
                // ОБ план
                splitParams.KeyValIndex = GetColumnIndex(tblIncomeDivData, d_Regions_Plan.RefBridge);
                splitParams.LvlValIndex = GetColumnIndex(tblIncomeDivData, f_D_FOPlanIncDivide.RefBudLevel);
                splitParams.DocValIndex = splitParams.LvlValIndex;
                splitParams.SrcColumnIndex = GetColumnIndex(tblIncomeDivData, f_D_FOPlanIncDivide.YearPlan);
                splitParams.UseLvlDepencity = false;
                splitParams.UseDocumentTypes = false;
                splitParams.IsSkifLevels = false;
                splitParams.IsFractional = false;
                splitParams.RowsData = rowsIncomeOBDiv;
                splitParams.DstColumnIndex = currentColumn;
                reportHelper.SplitRegionData(splitParams);
                // МБ план
                splitParams.UseLvlDepencity = true;
                splitParams.RowsData = rowsIncomeMBDiv;
                splitParams.DstColumnIndex = currentColumn + 1;
                reportHelper.SplitRegionData(splitParams);

                if (rowsIncomeMBDiv.Length > 0 || rowsIncomeOBDiv.Length > 0)
                {
                    maxMonth = Math.Max(maxMonth, i);
                }

                // МБ план омсу
                splitParams.KeyValIndex = GetColumnIndex(tblMonthRepData, d_Regions_MonthRep.RefRegionsBridge);
                splitParams.DocValIndex = GetColumnIndex(tblMonthRepData, d_Regions_MonthRep.RefDocType);
                splitParams.LvlValIndex = GetColumnIndex(tblMonthRepData, f_F_MonthRepIncomes.RefBdgtLevels);
                splitParams.UseDocumentTypes = true;
                splitParams.IsSkifLevels = true;
                splitParams.IsFractional = true;
                splitParams.SrcColumnIndex = GetColumnIndex(tblMonthRepData, f_F_MonthRepIncomes.YearPlan);
                splitParams.DstColumnIndex = currentColumn + 2;
                splitParams.RowsData = rowsMonthMBData;
                reportHelper.SplitRegionData(splitParams);
                // МБ факт                
                splitParams.SrcColumnIndex = GetColumnIndex(tblMonthRepData, f_F_MonthRepIncomes.Fact);
                splitParams.DstColumnIndex = currentColumn + 4;
                splitParams.RowsData = rowsMonthMBData;
                reportHelper.SplitRegionData(splitParams);
                // ОБ факт
                splitParams.SrcColumnIndex = GetColumnIndex(tblUKF22Data, f_D_UFK22.ForPeriod);
                splitParams.KeyValIndex = GetColumnIndex(tblUKF22Data, d_OKATO_UFK.RefRegionsBridge);
                splitParams.DocValIndex = splitParams.KeyValIndex;
                splitParams.LvlValIndex = splitParams.KeyValIndex;
                var fltOBFact = filterHelper.PeriodFilter(
                    f_D_UFK22.RefYearDayUNV,
                    periodStart,
                    i == 12 ? GetUNVAbsMonthEnd(year, i) : GetUNVMonthEnd(year, i));
                splitParams.UseLvlDepencity = false;
                splitParams.UseDocumentTypes = false;
                splitParams.IsFractional = false;
                splitParams.DstColumnIndex = currentColumn + 3;
                splitParams.RowsData = tblUKF22Data.Select(fltOBFact);
                reportHelper.SplitRegionData(splitParams);

                if (i <= 1)
                {
                    continue;
                }

                var asbCurFactOBIndex = ReportMonthMethods.AbsColumnIndex(splitParams.DstColumnIndex);
                var asbPrvFactOBIndex = ReportMonthMethods.AbsColumnIndex(splitParams.DstColumnIndex - GroupCount);
                SumColumns(tblResult, asbCurFactOBIndex, asbPrvFactOBIndex, asbCurFactOBIndex);
            }

            for (var i = 0; i < ColumnCount; i++)
            {
                var asbColumnIndex = ReportMonthMethods.AbsColumnIndex(i);
                DivideColumn(tblResult, asbColumnIndex, divider);
                RoundColumn(tblResult, asbColumnIndex, precision);
            }

            qFOYRParams.Period = yearPeriodFilter;
            queryFOYRText = QFOYRIncomes.GroupBy(qFOYRParams, groupFOYRInfo);
            var tblFullTearData = dbHelper.GetTableData(queryFOYRText);

            splitParams.TblResult = tblYearResult;
            splitParams.UseLvlDepencity = true;
            splitParams.UseDocumentTypes = true;
            splitParams.IsSkifLevels = true;
            splitParams.KeyValIndex = GetColumnIndex(tblFullTearData, d_Regions_FOYR.RefRegionsBridge);
            splitParams.DocValIndex = GetColumnIndex(tblFullTearData, d_Regions_FOYR.RefDocType);
            splitParams.LvlValIndex = GetColumnIndex(tblFullTearData, f_D_FOYRIncomes.RefBdgtLevels);
            splitParams.SrcColumnIndex = GetColumnIndex(tblFullTearData, f_D_FOYRIncomes.Performed);

            var factColumnIndex = GetColumnIndex(tblUKF22Data, d_OKATO_UFK.RefRegionsBridge);
            var splitUfkSettings = new RegionSplitParams
            {
                KeyValIndex = factColumnIndex,
                DocValIndex = factColumnIndex,
                LvlValIndex = factColumnIndex,
                TblResult = tblYearResult,
                SrcColumnIndex = GetColumnIndex(tblUKF22Data, f_D_UFK22.ForPeriod)
            };

            for (var i = 0; i < yearList.Count; i++)
            {
                var yearIncColumn = 3 + i * 2;
                var yearUfkColumn = yearIncColumn + 1;
                var yearUnvYearEnd = GetUNVYearPlanLoBound(yearList[i]);
                var yearFilter = filterHelper.EqualIntFilter(f_D_FOYRIncomes.RefYearDayUNV, yearUnvYearEnd);
                var rowsYearData = tblFullTearData.Select(yearFilter);
                splitParams.RowsData = rowsYearData;
                splitParams.IsFractional = true;
                splitParams.DstColumnIndex = yearIncColumn;
                reportHelper.SplitRegionData(splitParams);

                splitParams.IsFractional = false;
                splitUfkSettings.DstColumnIndex = yearUfkColumn;
                splitUfkSettings.RowsData = tblUKF22Data.Select(
                    filterHelper.PeriodFilter(
                        f_D_UFK22.RefYearDayUNV,
                        GetUNVYearLoBound(yearList[i]),
                        GetUNVAbsYearEnd(yearList[i])));
                reportHelper.SplitRegionData(splitUfkSettings);

                var incAbsIndex = ReportMonthMethods.AbsColumnIndex(yearIncColumn);
                var ufkAbsIndex = ReportMonthMethods.AbsColumnIndex(yearUfkColumn);

                DivideColumn(tblYearResult, incAbsIndex, divider);
                RoundColumn(tblYearResult, incAbsIndex, precision);
                DivideColumn(tblYearResult, ufkAbsIndex, divider);
                RoundColumn(tblYearResult, ufkAbsIndex, precision);

                var ttlAbsIndex = ReportMonthMethods.AbsColumnIndex(i);

                foreach (DataRow rowYear in tblYearResult.Rows)
                {
                    rowYear[ttlAbsIndex] = GetDecimal(rowYear[incAbsIndex]) + GetDecimal(rowYear[ufkAbsIndex]);
                }

                var rowSubject = tblYearResult.Rows[tblYearResult.Rows.Count - 2];
                rowSubject[ttlAbsIndex] = GetDecimal(rowSubject[ufkAbsIndex]); 
            }

            const int PlanOMSU = 9;
            const int FactOMSU = 10;
            const int PlanSumm = 11;
            splitParams.KeyValIndex = GetColumnIndex(tblFOYRData, d_Regions_FOYR.RefRegionsBridge);
            splitParams.DocValIndex = GetColumnIndex(tblFOYRData, d_Regions_FOYR.RefDocType);
            splitParams.LvlValIndex = GetColumnIndex(tblFOYRData, f_D_FOYRIncomes.RefBdgtLevels);
            splitParams.IsFractional = true;
            splitParams.RowsData = tblFOYRData.Select();
            splitParams.DstColumnIndex = PlanOMSU;
            splitParams.SrcColumnIndex = GetColumnIndex(tblFOYRData, f_D_FOYRIncomes.Assigned);
            reportHelper.SplitRegionData(splitParams);
            splitParams.DstColumnIndex = FactOMSU;
            splitParams.SrcColumnIndex = GetColumnIndex(tblFOYRData, f_D_FOYRIncomes.Performed);
            reportHelper.SplitRegionData(splitParams);

            var planPeriod = GetUNVMonthStart(year, maxMonth);
            var filterLastMonth = filterHelper.EqualIntFilter(f_F_MonthRepIncomes.RefYearDayUNV, planPeriod);
            var tblPlanDivIncData = DataTableUtils.FilterDataSet(tblIncomeDivData, filterLastMonth);
            splitParams.KeyValIndex = GetColumnIndex(tblPlanDivIncData, d_Regions_Plan.RefBridge);
            splitParams.LvlValIndex = GetColumnIndex(tblPlanDivIncData, f_D_FOPlanIncDivide.RefBudLevel);
            splitParams.DocValIndex = splitParams.LvlValIndex;
            splitParams.SrcColumnIndex = GetColumnIndex(tblPlanDivIncData, f_D_FOPlanIncDivide.YearPlan);
            splitParams.UseLvlDepencity = false;
            splitParams.UseDocumentTypes = false;
            splitParams.IsSkifLevels = false;
            splitParams.IsFractional = false;
            splitParams.RowsData = tblPlanDivIncData.Select();
            splitParams.DstColumnIndex = PlanSumm;
            reportHelper.SplitRegionData(splitParams);

            var absPlanOMSU = ReportMonthMethods.AbsColumnIndex(PlanOMSU);
            DivideColumn(tblYearResult, absPlanOMSU, divider);
            RoundColumn(tblYearResult, absPlanOMSU, precision);
            var absFactOMSU = ReportMonthMethods.AbsColumnIndex(FactOMSU);
            DivideColumn(tblYearResult, absFactOMSU, divider);
            RoundColumn(tblYearResult, absFactOMSU, precision);
            var absPlanSumm = ReportMonthMethods.AbsColumnIndex(PlanSumm);
            DivideColumn(tblYearResult, absPlanSumm, divider);
            RoundColumn(tblYearResult, absPlanSumm, precision);

            var tblSubjectMN = ReportMonthMethods.CreateSubjectTable(tblResult);
            var tblSubjectYR = ReportMonthMethods.CreateSubjectTable(tblYearResult);

            reportHelper.ClearSettleRows(tblResult, paramLvl);
            reportHelper.ClearSettleRows(tblYearResult, paramLvl);

            tablesResult[0] = tblResult;
            tablesResult[1] = tblYearResult;
            tablesResult[2] = tblSubjectMN;
            tablesResult[3] = tblSubjectYR;
            rowCaption[0] = year;
            rowCaption[1] = reportHelper.GetKDBridgeCaptionText(filterKD);
            rowCaption[2] = ReportMonthMethods.WriteSettles(paramLvl);
            rowCaption[4] = ReportMonthMethods.GetDividerDescr(divider);
            rowCaption[5] = new DateTime(year, Math.Max(1, maxMonth), 1).AddMonths(1).ToShortDateString();
            rowCaption[7] = ReportMonthMethods.GetPrecisionIndex(precision);
            return tablesResult;
        }
    }
}
