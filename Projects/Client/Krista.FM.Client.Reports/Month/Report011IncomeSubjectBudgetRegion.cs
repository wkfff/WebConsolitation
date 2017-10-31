using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData.UFK;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 011_ПОСТУПЛЕНИЯ В ОБЛАСТНОЙ БЮДЖЕТ ПО КБК И АДМИНИСТРАТОРАМ В РАЗРЕЗЕ ОМСУ
        /// </summary>
        public DataTable[] GetMonthReport011IncomeSubjectBudgetRegionData(Dictionary<string, string> reportParams)
        {
            const int ColumnCount = 5;
            var filterHelper = new QFilterHelper();
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var fltMark = reportParams[ReportConsts.ParamMark];
            var loDate = reportParams[ReportConsts.ParamStartDate];
            var loDateUnv = GetUNVDate(loDate);
            var hiDate = reportParams[ReportConsts.ParamEndDate];
            var hiDateUnv = GetUNVDate(hiDate);
            var endDate = Convert.ToDateTime(hiDate);

            if (endDate.Month == 12 && endDate.Day == 31)
            {
                hiDateUnv = GetUNVAbsYearEnd(endDate.Year);
            }

            var fltKD = reportParams[ReportConsts.ParamKDComparable];
            var fltKVSR = reportParams[ReportConsts.ParamKVSRComparable];
            fltKVSR = ReportMonthMethods.CheckBookValue(fltKVSR);
            var fltRegion = reportParams[ReportConsts.ParamRegionComparable];
            fltRegion = ReportMonthMethods.CheckBookValue(fltRegion);
            var fltPeriod = filterHelper.PeriodFilter(f_D_UFK22.RefYearDayUNV, loDateUnv, hiDateUnv, true);
            var dbHelper = new ReportDBHelper(scheme);
            var entKdBridge = ConvertorSchemeLink.GetEntity(b_KD_Bridge.InternalKey);
            var listKBK = fltKD.Split(',');

            // План, утвержденный органами госвласти
            var tblResult = CreateReportCaptionTable(7);

            var qParams = new QUFK22Params
                              {
                                  KD = fltKD,
                                  Period = fltPeriod,
                                  Mark = fltMark,
                                  KVSR = fltKVSR,
                                  Region = fltRegion
                              };

            var groupFields = new List<QUFK22Group> { QUFK22Group.Kd, QUFK22Group.Mark };
            var queryUFK22Text = QUFK22.GroupBy(qParams, groupFields);
            var tblUKF22Data = dbHelper.GetTableData(queryUFK22Text);
            var cutParams = reportHelper.CreateCutKDParams();
            var kbkSummary = new List<string>();
            var summary = new decimal[tblResult.Columns.Count];

            foreach (var kbk in listKBK)
            {
                var rowResult = tblResult.NewRow();

                cutParams.Key = kbk;
                var childs = reportHelper.GetChildClsRecord(cutParams);
                var childFilter = kbk;

                if (childs.Count() > 0)
                {
                    childFilter = childs.Aggregate(childFilter,
                                                   (current, clsRec) => Combine(current, clsRec.KeyValue, ","));
                    childFilter = childFilter.Trim(',');
                }

                var fltKBK = filterHelper.RangeFilter(d_KD_UFK.RefKDBridge, childFilter);
                var tblKBK = DataTableUtils.FilterDataSet(tblUKF22Data, fltKBK);

                var isNonEmpty = false;
                for (var i = 0; i < ColumnCount; i++)
                {
                    var fltCurrentMark = filterHelper.EqualIntFilter(f_D_UFK22.RefMarks, i + 1);
                    var sumValue = tblKBK.Select(fltCurrentMark).Sum(dataRow => GetDecimal(dataRow[f_D_UFK22.ForPeriod]));
                    rowResult[i + 2] = sumValue;

                    if (!kbkSummary.Contains(kbk))
                    {
                        summary[i + 2] += sumValue;
                    }

                    isNonEmpty = isNonEmpty || sumValue != 0;
                }

                var rowTitle = reportHelper.GetBookRow(entKdBridge, kbk);

                if (rowTitle != null)
                {
                    rowResult[0] = rowTitle[b_KD_Bridge.CodeStr];
                    rowResult[1] = rowTitle[b_KD_Bridge.Name];
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

            for (var i = 0; i < ColumnCount; i++)
            {
                DivideColumn(tblResult, i + 2, divider);
                RoundColumn(tblResult, i + 2, precision);
            }

            tablesResult[0] = tblResult;
            var rowCaption = CreateReportParamsRow(tablesResult);
            rowCaption[0] = loDate;
            rowCaption[1] = hiDate;
            rowCaption[3] = reportHelper.GetKDBridgeCaptionText(fltKD);
            rowCaption[4] = ReportMonthMethods.GetDividerDescr(divider);
            rowCaption[5] = ReportMonthMethods.GetSelectedRegion(reportParams[ReportConsts.ParamRegionComparable]);
            rowCaption[6] = ReportMonthMethods.GetSelectedKVSR(reportParams[ReportConsts.ParamKVSRComparable]);
            rowCaption[7] = ReportMonthMethods.GetPrecisionIndex(precision);
            rowCaption[10] = fltMark;

            for (var i = 0; i < ColumnCount; i++)
            {
                rowCaption[11 + i] = fltMark.Length == 0 || fltMark.Contains(Convert.ToString(i + 1));
            }

            return tablesResult;
        }
    }
}

