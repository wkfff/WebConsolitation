using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK;
using Krista.FM.Client.Reports.Month;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Database.ClsData.UFK;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsBridge;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 012_АНАЛИЗ ПОСТУПЛЕНИЙ ПО НДФЛ
        /// </summary>
        public DataTable[] GetMonthReport012AnalisysIncomeNDFLData(Dictionary<string, string> reportParams)
        {
            const int GroupCount = 4;
            const int ColumnCount = 8 + GroupCount * 13;
            var reportHelper = new ReportMonthMethods(scheme);
            var filterHelper = new QFilterHelper();
            var tablesResult = new DataTable[3];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var rowCaption = CreateReportParamsRow(tablesResult);
            var kdBridge = ConvertorSchemeLink.GetEntity(b_KD_Bridge.InternalKey);
            var rowNDFL = reportHelper.GetBookRow(kdBridge, b_KD_Bridge.CodeStr, "00010102000010000110");
            var rowPtnt = reportHelper.GetBookRow(kdBridge, b_KD_Bridge.CodeStr, "00010102040010000110");
            var paramLvl = reportParams[ReportConsts.ParamRegionListType];
            var year = ReportMonthMethods.GetSelectedYear(reportParams[ReportConsts.ParamYear]);
            var yearLoBound = GetUNVYearStart(year);
            var yearHiBound = GetUNVYearEnd(year);
            var yearPeriod = filterHelper.GetYearFilter(yearLoBound, yearHiBound);
            var yearUfkPeriod = filterHelper.GetYearFilter(yearLoBound, GetUNVAbsYearEnd(year));
            var dbHelper = new ReportDBHelper(scheme);
            var fltPtnt = Convert.ToString(rowPtnt[b_KD_Bridge.ID]);
            var fltNDFL = reportHelper.GetKDHierarchyFilter(Convert.ToString(rowNDFL[b_KD_Bridge.ID]));
            var yearList = new List<int> { year - 1, year };
            var yearPeriodFilter = filterHelper.GetMultiYearFilter(yearList);

            var qMonthParams = new QMonthRepParams
            {
                Period = yearPeriodFilter,
                KD = fltNDFL,
                Lvl = ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.ConsMunicipal)
            };

            var groupMonthInfo = new List<QMonthRepGroup>
                                {
                                    QMonthRepGroup.Period,
                                    QMonthRepGroup.Lvl,
                                    QMonthRepGroup.Region,
                                    QMonthRepGroup.DocType
                                };

            var queryYearNDFLIncText = QMonthRepIncomes.GroupBy(qMonthParams, groupMonthInfo);
            qMonthParams.KD = fltPtnt;
            var queryYearPtntIncText = QMonthRepIncomes.GroupBy(qMonthParams, groupMonthInfo);
            var tblYearNDFLData = dbHelper.GetTableData(queryYearNDFLIncText);
            var tblYearPtntData = dbHelper.GetTableData(queryYearPtntIncText);
            qMonthParams.Period = yearPeriod;
            var queryMonthRepText = QMonthRepIncomes.GroupBy(qMonthParams, groupMonthInfo);
            var tblMonthRepPtntData = dbHelper.GetTableData(queryMonthRepText);
            qMonthParams.KD = fltNDFL;
            queryMonthRepText = QMonthRepIncomes.GroupBy(qMonthParams, groupMonthInfo);
            var tblMonthRepNDFLData = dbHelper.GetTableData(queryMonthRepText);

            var groupInfo = new List<QPlanIncomeGroup>
                                {
                                    QPlanIncomeGroup.Period,
                                    QPlanIncomeGroup.Lvl,
                                    QPlanIncomeGroup.Region,
                                    QPlanIncomeGroup.Kd
                                };

            var qParams = new QPlanIncomeParams
            {
                Period = yearPeriod,
                Lvl = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Summary),
                KD = fltNDFL
            };

            var queryIncomeData = QPlanIncomeDivide.GroupBy(qParams, groupInfo);
            var tblIncomeDivData = dbHelper.GetTableData(queryIncomeData);

            var tblResult = reportHelper.CreateRegionList(ColumnCount);

            var subjectCodeDiv = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject);
            var fltMBDiv = filterHelper.NotEqualIntFilter(f_D_FOPlanIncDivide.RefBudLevel, subjectCodeDiv);
            var fltOBDiv = filterHelper.EqualIntFilter(f_D_FOPlanIncDivide.RefBudLevel, subjectCodeDiv);

            var subjectCodeMon = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject);
            var fltMBMon = filterHelper.NotEqualIntFilter(f_F_MonthRepIncomes.RefBdgtLevels, subjectCodeMon);

            // не знаю что делать с одноименными колонками выборки
            var cnKD = tblIncomeDivData.Columns[4].ColumnName;
            var fltNDFLValue = filterHelper.RangeFilter(cnKD, fltNDFL);

            var qUfkParams = new QUFK22Params
            {
                KD = fltNDFL,
                Period = yearUfkPeriod,
                Mark = ReportMonthConsts.MarkUfkIncome,
            };

            var groupFields = new List<QUFK22Group> { QUFK22Group.Region, QUFK22Group.Period };
            var queryUFK22NDFLText = QUFK22.GroupBy(qUfkParams, groupFields);
            qUfkParams.KD = fltPtnt;
            var queryUFK22PtntText = QUFK22.GroupBy(qUfkParams, groupFields);
            var tblUKF22NDFLData = dbHelper.GetTableData(queryUFK22NDFLText);
            var tblUKF22PtntData = dbHelper.GetTableData(queryUFK22PtntText);

            var splitParamsDiv = new RegionSplitParams
                                     {
                                         TblResult = tblResult,
                                         KeyValIndex = GetColumnIndex(tblIncomeDivData, d_Regions_Plan.RefBridge),
                                         LvlValIndex = GetColumnIndex(tblIncomeDivData, f_D_FOPlanIncDivide.RefBudLevel),
                                         DocValIndex = GetColumnIndex(tblIncomeDivData, f_D_FOPlanIncDivide.RefBudLevel),
                                         UseDocumentTypes = false,
                                         IsSkifLevels = false
                                     };

            var splitParamsMon = new RegionSplitParams
            {
                TblResult = tblResult,
                KeyValIndex = GetColumnIndex(tblMonthRepNDFLData, d_Regions_MonthRep.RefRegionsBridge),
                DocValIndex = GetColumnIndex(tblMonthRepNDFLData, d_Regions_MonthRep.RefDocType),
                LvlValIndex = GetColumnIndex(tblMonthRepNDFLData, f_F_MonthRepIncomes.RefBdgtLevels),
                IsSkifLevels = true,
                UseDocumentTypes = true,
                UseLvlDepencity = true
            };

            var splitParamsUfk = new RegionSplitParams
            {
                TblResult = tblResult,
                KeyValIndex = GetColumnIndex(tblUKF22NDFLData, d_OKATO_UFK.RefRegionsBridge),
                DocValIndex = GetColumnIndex(tblUKF22NDFLData, d_OKATO_UFK.RefRegionsBridge),
                LvlValIndex = GetColumnIndex(tblUKF22NDFLData, d_OKATO_UFK.RefRegionsBridge),
                SrcColumnIndex = GetColumnIndex(tblUKF22NDFLData, f_D_UFK22.ForPeriod)
            };

            var planLastDate = String.Empty;

            for (var i = 1; i < 13; i++)
            {
                var periodStart = GetUNVMonthStart(year, i);
                var filterMonth = filterHelper.EqualIntFilter(f_D_FOPlanIncDivide.RefYearDayUNV, periodStart);
                var tblDivIncData = DataTableUtils.FilterDataSet(tblIncomeDivData, filterMonth);
                var tblDivIncNDFLData = DataTableUtils.FilterDataSet(tblDivIncData, fltNDFLValue);
                // ндфл
                var rowsNDFLIncomeOBDiv = tblDivIncNDFLData.Select(fltOBDiv);
                var rowsNDFLIncomeMBDiv = tblDivIncNDFLData.Select(fltMBDiv);
                // ОБ план НДФЛ
                splitParamsDiv.SrcColumnIndex = GetColumnIndex(tblIncomeDivData, f_D_FOPlanIncDivide.YearPlan);
                splitParamsDiv.UseLvlDepencity = false;
                splitParamsDiv.RowsData = rowsNDFLIncomeOBDiv;
                splitParamsDiv.DstColumnIndex = 4;

                if (rowsNDFLIncomeOBDiv.Length > 0)
                {
                    planLastDate = new DateTime(year, i, 1).AddMonths(1).ToShortDateString();
                    reportHelper.SplitRegionData(splitParamsDiv);

                    if (i == 1)
                    {
                        var columnIndex = ReportMonthMethods.AbsColumnIndex(splitParamsDiv.DstColumnIndex);
                        SumColumns(splitParamsDiv.TblResult, columnIndex, columnIndex + 1, columnIndex + 1);
                    }
                }

                // МБ план НДФЛ
                splitParamsDiv.UseLvlDepencity = true;
                splitParamsDiv.RowsData = rowsNDFLIncomeMBDiv;
                splitParamsDiv.DstColumnIndex = 6;

                if (rowsNDFLIncomeOBDiv.Length > 0)
                {
                    reportHelper.SplitRegionData(splitParamsDiv);

                    if (i == 1)
                    {
                        var columnIndex = ReportMonthMethods.AbsColumnIndex(splitParamsDiv.DstColumnIndex);
                        SumColumns(splitParamsDiv.TblResult, columnIndex, columnIndex + 1, columnIndex + 1);
                    }
                }
            }

            for (var i = 1; i < 12; i++)
            {
                var currentColumn = 8 + (i - 1) * GroupCount;
                var periodStart = GetUNVMonthStart(year, i);
                var filterMonth = filterHelper.EqualIntFilter(f_F_MonthRepIncomes.RefYearDayUNV, periodStart);
                var tblMonIncNDFLData = DataTableUtils.FilterDataSet(tblMonthRepNDFLData, filterMonth);
                var tblMonIncPtntData = DataTableUtils.FilterDataSet(tblMonthRepPtntData, filterMonth);
                // ндфл
                var rowsNDFLMonthMBData = tblMonIncNDFLData.Select(fltMBMon);
                // патенты
                var rowsPtntMonthMBData = tblMonIncPtntData.Select(fltMBMon);

                // МБ факт НДФЛ           
                splitParamsMon.SrcColumnIndex = GetColumnIndex(tblMonIncNDFLData, f_F_MonthRepIncomes.Fact);
                splitParamsMon.DstColumnIndex = currentColumn + 2;
                splitParamsMon.RowsData = rowsNDFLMonthMBData;
                reportHelper.SplitRegionData(splitParamsMon);
                // МБ факт патенты
                splitParamsMon.DstColumnIndex = currentColumn + 3;
                splitParamsMon.RowsData = rowsPtntMonthMBData;
                reportHelper.SplitRegionData(splitParamsMon);
                // ОБ факт ндфл
                var fltUfkPeriod = filterHelper.PeriodFilter(
                    f_D_UFK22.RefYearDayUNV,
                    GetUNVMonthStart(year, i),
                    GetUNVAbsMonthEnd(year, i));
                splitParamsUfk.DstColumnIndex = currentColumn;
                splitParamsUfk.RowsData = tblUKF22NDFLData.Select(fltUfkPeriod);
                reportHelper.SplitRegionData(splitParamsUfk);
                // об факт патенты
                splitParamsUfk.DstColumnIndex = currentColumn + 1;
                splitParamsUfk.RowsData = tblUKF22PtntData.Select(fltUfkPeriod);
                reportHelper.SplitRegionData(splitParamsUfk);
                
                if (i > 1)
                {
                    var idx = ReportMonthMethods.AbsColumnIndex(currentColumn);
                    SumColumns(splitParamsUfk.TblResult, idx, idx - GroupCount, idx);
                    idx++;
                    SumColumns(splitParamsUfk.TblResult, idx, idx - GroupCount, idx);
                }
            }

            const int YearColumn = 8 + 11 * 4;

            groupFields = new List<QUFK22Group> { QUFK22Group.Region };

            var splitParamsYr = new RegionSplitParams
            {
                TblResult = tblResult,
                UseDocumentTypes = true,
                UseLvlDepencity = true,
                KeyValIndex = GetColumnIndex(tblYearNDFLData, d_Regions_MonthRep.RefRegionsBridge),
                DocValIndex = GetColumnIndex(tblYearNDFLData, d_Regions_MonthRep.RefDocType),
                LvlValIndex = GetColumnIndex(tblYearNDFLData, f_F_MonthRepIncomes.RefBdgtLevels),
                SrcColumnIndex = GetColumnIndex(tblYearNDFLData, f_F_MonthRepIncomes.Fact)
            };


            var qFOYRParams = new QFOYRParams
            {
                Period = yearPeriodFilter,
                KD = fltNDFL,
                Lvl = ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.ConsMunicipal)
            };

            var groupFOYRInfo = new List<QFOYRGroup>
                                    {
                                        QFOYRGroup.DocType, QFOYRGroup.Region, QFOYRGroup.Lvl, QFOYRGroup.Period
                                    };
            var queryYrText = QFOYRIncomes.GroupBy(qFOYRParams, groupFOYRInfo);
            var tblDataNDFLFOYR = dbHelper.GetTableData(queryYrText);
            qFOYRParams.KD = fltPtnt;
            queryYrText = QFOYRIncomes.GroupBy(qFOYRParams, groupFOYRInfo);
            var tblDataPtntFOYR = dbHelper.GetTableData(queryYrText);

            var splitParamsFOYR = new RegionSplitParams
            {
                TblResult = tblResult,
                UseDocumentTypes = true,
                UseLvlDepencity = true,
                KeyValIndex = GetColumnIndex(tblDataNDFLFOYR, d_Regions_FOYR.RefRegionsBridge),
                DocValIndex = GetColumnIndex(tblDataNDFLFOYR, d_Regions_FOYR.RefDocType),
                LvlValIndex = GetColumnIndex(tblDataNDFLFOYR, f_D_FOYRIncomes.RefBdgtLevels),
                SrcColumnIndex = GetColumnIndex(tblDataNDFLFOYR, f_D_FOYRIncomes.Performed)
            };

            for (var i = 0; i < yearList.Count; i++)
            {
                var isPrevYear = i == 0;
                var yearFilter = filterHelper.GetYearFilter(yearList[i], false);
                var yearUfkFilter = filterHelper.GetAbsYearFilter(yearList[i], false);
                
                if (!isPrevYear)
                {
                    var monthFilter = filterHelper.EqualIntFilter(
                        f_F_MonthRepIncomes.RefYearDayUNV,
                        GetUNVMonthStart(yearList[i], 12));
                    // мб факт ндфл
                    splitParamsYr.RowsData = tblYearNDFLData.Select(monthFilter);
                    splitParamsYr.DstColumnIndex = YearColumn + 2;
                    reportHelper.SplitRegionData(splitParamsYr);
                    // мб факт патенты
                    splitParamsYr.RowsData = tblYearPtntData.Select(monthFilter);
                    splitParamsYr.DstColumnIndex = YearColumn + 3;
                    reportHelper.SplitRegionData(splitParamsYr);

                    // мб факт ндфл
                    splitParamsFOYR.RowsData = tblDataNDFLFOYR.Select(yearFilter);
                    splitParamsFOYR.DstColumnIndex = YearColumn + 2 + GroupCount;
                    reportHelper.SplitRegionData(splitParamsFOYR);
                    // мб факт патенты
                    splitParamsFOYR.RowsData = tblDataPtntFOYR.Select(yearFilter);
                    splitParamsFOYR.DstColumnIndex = YearColumn + 3 + GroupCount;
                    reportHelper.SplitRegionData(splitParamsFOYR); 
                }
                else
                {
                    // мб факт ндфл
                    splitParamsFOYR.RowsData = tblDataNDFLFOYR.Select(yearFilter);
                    splitParamsFOYR.DstColumnIndex = 2;
                    reportHelper.SplitRegionData(splitParamsFOYR);
                    // мб факт патенты
                    splitParamsFOYR.RowsData = tblDataPtntFOYR.Select(yearFilter);
                    splitParamsFOYR.DstColumnIndex = 3;
                    reportHelper.SplitRegionData(splitParamsFOYR);                    
                }

                // настройка выборки уфк 22
                qUfkParams.Period = yearUfkFilter;
                qUfkParams.KD = fltNDFL;
                queryUFK22NDFLText = QUFK22.GroupBy(qUfkParams, groupFields);
                qUfkParams.KD = fltPtnt;
                queryUFK22PtntText = QUFK22.GroupBy(qUfkParams, groupFields);
                tblUKF22NDFLData = dbHelper.GetTableData(queryUFK22NDFLText);
                tblUKF22PtntData = dbHelper.GetTableData(queryUFK22PtntText);
                // об факт ндфл
                splitParamsUfk.RowsData = tblUKF22NDFLData.Select();
                splitParamsUfk.DstColumnIndex = isPrevYear ? 0 : YearColumn;
                reportHelper.SplitRegionData(splitParamsUfk);
                // об факт патенты
                splitParamsUfk.RowsData = tblUKF22PtntData.Select();
                splitParamsUfk.DstColumnIndex = isPrevYear ? 1 : YearColumn + 1;
                reportHelper.SplitRegionData(splitParamsUfk);

                if (!isPrevYear)
                {
                    var sumColumn1 = ReportMonthMethods.AbsColumnIndex(YearColumn);
                    var sumColumn2 = ReportMonthMethods.AbsColumnIndex(YearColumn + GroupCount);
                    SumColumns(tblResult, sumColumn2, sumColumn1, sumColumn2);
                    sumColumn1++;
                    sumColumn2++;
                    SumColumns(tblResult, sumColumn2, sumColumn1, sumColumn2);
                }
            }

            for (var i = 0; i < ColumnCount; i++)
            {
                var absColumnIndex = ReportMonthMethods.AbsColumnIndex(i);
                DivideColumn(tblResult, absColumnIndex, divider);
                RoundColumn(tblResult, absColumnIndex, precision);
            }

            var tblSubject = ReportMonthMethods.CreateSubjectTable(tblResult);
            reportHelper.ClearSettleRows(tblResult, paramLvl);
            tablesResult[0] = tblResult;
            tablesResult[1] = tblSubject;
            rowCaption[0] = year;
            rowCaption[1] = reportHelper.GetKDBridgeCaptionText(fltNDFL);
            rowCaption[2] = ReportMonthMethods.WriteSettles(paramLvl);
            rowCaption[4] = ReportMonthMethods.GetDividerDescr(divider);
            rowCaption[7] = ReportMonthMethods.GetPrecisionIndex(precision);
            rowCaption[8] = planLastDate;
            return tablesResult;
        }
    }
}
