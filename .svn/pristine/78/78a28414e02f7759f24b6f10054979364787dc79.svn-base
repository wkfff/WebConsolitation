using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData.UFK;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK.ReportMaster;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{

    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 024 ДИНАМИКА ПОСТУПЛЕНИЯ НАЛОГОВЫХ И НЕНАЛОГОВЫХ ДОХОДОВ В БC – ОПЕРАТИВНАЯ ИНФОРМАЦИЯ
        /// </summary>
        public DataTable[] GetUFK22Report024Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramRegion = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamRegionComparable]);
            var paramMarks = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMark]);
            var paramGroupKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamGroupKD]);
            var startDate = Convert.ToDateTime(reportParams[ReportConsts.ParamStartDate]);
            var endDate = Convert.ToDateTime(reportParams[ReportConsts.ParamEndDate]);
            if (startDate.Year != endDate.Year)
            {
                startDate = new DateTime(endDate.Year, 1, 1);
            }

            // периоды
            var prevStartDate = startDate.AddYears(-1);
            var prevEndDate = endDate.AddYears(-1);
            var unvEndDate = endDate.Month != 12 || endDate.Day != 31
                                 ? GetUNVDate(endDate)
                                 : GetUNVDate(endDate.Year, 12, 32);
            var unvPrevEndDate = prevEndDate.Month != 12 || prevEndDate.Day != 31
                                 ? GetUNVDate(prevEndDate)
                                 : GetUNVDate(prevEndDate.Year, 12, 32);
            var periods = new[]
            {
                 filterHelper.PeriodFilter(f_D_UFK22.RefYearDayUNV, GetUNVYearStart(prevEndDate.Year), unvPrevEndDate, true),
                 filterHelper.PeriodFilter(f_D_UFK22.RefYearDayUNV, GetUNVYearStart(endDate.Year), unvEndDate, true),
                 filterHelper.PeriodFilter(f_D_UFK22.RefYearDayUNV, GetUNVDate(prevStartDate), unvPrevEndDate, true),
                 filterHelper.PeriodFilter(f_D_UFK22.RefYearDayUNV, GetUNVDate(startDate), unvEndDate, true)
            };

            // группы КД
            var selectedGroupKD = ReportUFKHelper.GetSelectedID(paramGroupKD, b_D_GroupKD.InternalKey);
            var nestedKD = (from id in selectedGroupKD
                            let groupId = Convert.ToString(id)
                            let nestedGroupId = reportHelper.GetNestedIDByField(b_D_GroupKD.InternalKey, b_D_GroupKD.ID, groupId)
                            let nestedKDId = reportHelper.GetNestedIDByField(b_KD_Bridge.InternalKey, b_KD_Bridge.RefDGroupKD, nestedGroupId)
                            select new { groupId, nestedKDId }).ToDictionary(e => e.groupId, e => e.nestedKDId);
            var filterKD = GetDistinctCodes(ConvertToString(nestedKD.Values));

            // параметры отчета
            var rep = new Report(f_D_UFK22.InternalKey) {Divider = GetDividerValue(divider)};
            // группировка по группам КД
            var kdGrouping = rep.AddGrouping(b_KD_Bridge.RefDGroupKD);
            kdGrouping.HierarchyTable = dbHelper.GetEntityData(b_D_GroupKD.InternalKey);
            kdGrouping.HideHierarchyLevels = true;
            kdGrouping.AddLookupField(b_D_GroupKD.InternalKey, b_D_GroupKD.Name);
            kdGrouping.AddLookupField(b_D_GroupKD.InternalKey, b_D_GroupKD.Code).Type = typeof(int);
            kdGrouping.ViewParams.Add(1, new RowViewParams {BreakSumming = true, Style = 1});
            kdGrouping.ViewParams.Add(2, new RowViewParams {BreakSumming = true, Style = 2, ForAllLevels = true});
            kdGrouping.AddSortField(b_D_GroupKD.InternalKey, b_D_GroupKD.Code);
            kdGrouping.SetFixedValues(paramGroupKD);
            // сортируем и настраиваем колонки отчета
            var captionColumn = rep.AddCaptionColumn();
            captionColumn.SetMask(kdGrouping, 0, b_D_GroupKD.InternalKey, b_D_GroupKD.Name).ForAllLevels = true;
            foreach (var period in periods)
            {
                rep.AddValueColumn(SUM);
            }

            // получаем данные
            var filterList = new List<QFilter>
                {
                    new QFilter(QdUFK22.Keys.Period, String.Empty),
                    new QFilter(QdUFK22.Keys.KD, filterKD),
                    new QFilter(QdUFK22.Keys.Okato, paramRegion),
                    new QFilter(QdUFK22.Keys.Marks, paramMarks)
                };

            var groupFields = new List<Enum> { QdUFK22.Keys.KD };

            var dt = new DataTable();
            dt.Columns.Add(b_KD_Bridge.RefDGroupKD, typeof (int));
            dt.Columns.Add(SUM, typeof (decimal));

            for (var i = 0; i < periods.Length; i++)
            {
                filterList[0].Filter = periods[i];
                var query = new QdUFK22().GetQueryText(filterList, groupFields);
                var tblData = dbHelper.GetTableData(query);
                dt.Rows.Clear();

                foreach (var groupKD in nestedKD.Where(e => e.Value != String.Empty))
                {
                    var filter = filterHelper.RangeFilter(d_KD_UFK.RefKDBridge, groupKD.Value);
                    dt.Rows.Add(groupKD.Key, GetSumFieldValue(tblData, SUM, filter));
                }

                rep.ProcessDataRows(dt.Select(), captionColumn.Index + 1 + i);
            }

            tablesResult[0] = rep.GetReportData();

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.STARTDATE, startDate.ToShortDateString());
            paramHelper.SetParamValue(ParamUFKHelper.ENDDATE, endDate.ToShortDateString());
            paramHelper.SetParamValue(ParamUFKHelper.SETTLEMENT, reportHelper.GetNotNestedRegionCaptionText(paramRegion));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
