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
using Krista.FM.Client.Reports.UFK22.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{

    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 027 ДИНАМИКА ПОСТУПЛЕНИЯ НАЛОГОВЫХ И НЕНАЛОГОВЫХ ДОХОДОВ В БC
        /// </summary>
        public DataTable[] GetUFK22Report027Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramMarks = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMark]);
            var paramGroupKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamGroupKD]);
            var paramDate = Convert.ToDateTime(reportParams[ReportConsts.ParamEndDate]);

            // периоды
            var maxMonth = reportHelper.GetMaxMonth(f_F_MonthRepIncomes.InternalKey, f_F_MonthRepIncomes.RefYearDayUNV, paramDate.Year);
            var unvMaxMonth = maxMonth > 0 ? GetUNVMonthStart(paramDate.Year, maxMonth) : ReportConsts.UndefinedKey;
            var dates = new[]
            {
                new DateTime(paramDate.Year - 2, 12, 31),
                paramDate.AddYears(-2),
                new DateTime(paramDate.Year - 1, 12, 31),
                paramDate.AddYears(-1),
                paramDate
            };
            var periods = new List<string>();
            foreach (var date in dates)
            {
                var unvDate = date.Month != 12 || date.Day != 31
                                 ? GetUNVDate(date)
                                 : GetUNVDate(date.Year, 12, 32);
                periods.Add(filterHelper.PeriodFilter(f_D_UFK22.RefYearDayUNV, GetUNVYearStart(date.Year), unvDate, true));
            }

            // группы КД
            var selectedGroupKD = ReportUFKHelper.GetSelectedID(paramGroupKD, b_D_GroupKD.InternalKey);
            var nestedKD = (from id in selectedGroupKD
                            let groupId = Convert.ToString(id)
                            let nestedGroupId = reportHelper.GetNestedIDByField(b_D_GroupKD.InternalKey, b_D_GroupKD.ID, groupId)
                            let nestedKDId = reportHelper.GetNestedIDByField(b_KD_Bridge.InternalKey, b_KD_Bridge.RefDGroupKD, nestedGroupId)
                            select new { groupId, nestedKDId }).ToDictionary(e => e.groupId, e => e.nestedKDId);
            var filterKD = GetDistinctCodes(ConvertToString(nestedKD.Values));

            // параметры отчета
            var rep = new Report(f_D_UFK22.InternalKey) { Divider = GetDividerValue(divider) };
            // группировка по группам КД
            var kdGrouping = rep.AddGrouping(b_KD_Bridge.RefDGroupKD);
            kdGrouping.HierarchyTable = dbHelper.GetEntityData(b_D_GroupKD.InternalKey);
            kdGrouping.HideHierarchyLevels = true;
            kdGrouping.AddLookupField(b_D_GroupKD.InternalKey, b_D_GroupKD.Name);
            kdGrouping.AddLookupField(b_D_GroupKD.InternalKey, b_D_GroupKD.Code).Type = typeof(int);
            kdGrouping.ViewParams.Add(1, new RowViewParams { BreakSumming = true, Style = 1 });
            kdGrouping.ViewParams.Add(2, new RowViewParams { BreakSumming = true, Style = 2, ForAllLevels = true });
            kdGrouping.AddSortField(b_D_GroupKD.InternalKey, b_D_GroupKD.Code);
            kdGrouping.SetFixedValues(paramGroupKD);
            // сортируем и настраиваем колонки отчета
            var captionColumn = rep.AddCaptionColumn();
            captionColumn.SetMask(kdGrouping, 0, b_D_GroupKD.InternalKey, b_D_GroupKD.Name).ForAllLevels = true;
            var planColumn = rep.AddValueColumn(SUM); // колонка План года
            foreach (var period in periods)
            {
                rep.AddValueColumn(SUM);
            }

            // промежуточная таблица
            var dt = new DataTable();
            dt.Columns.Add(b_KD_Bridge.RefDGroupKD, typeof(int));
            dt.Columns.Add(SUM, typeof(decimal));

            // получаем данные из т.ф. ФО_МесОтч_Доходы (f.F.MonthRepIncomes)
            {
                var filterQMonthRep = new List<QFilter>
                {
                    new QFilter(QMonthRep.Keys.Day, unvMaxMonth),
                    new QFilter(QMonthRep.Keys.KD, filterKD),
                    new QFilter(QMonthRep.Keys.Means, ReportUFK22Helper.MeansTypeBudget),
                    new QFilter(QMonthRep.Keys.Lvl, ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.Subject)),
                    new QFilter(QMonthRep.Keys.DocTyp, ReportMonthMethods.GetDocumentTypesSkif(SettleLvl.ConsSubject))
                };

                var groupQMonthRep = new List<Enum> { QMonthRep.Keys.KD };
                var query = new QMonthRep().GetQueryText(filterQMonthRep, groupQMonthRep);
                var tblData = dbHelper.GetTableData(query);
                foreach (var groupKD in nestedKD.Where(e => e.Value != String.Empty))
                {
                    var filter = filterHelper.RangeFilter(d_KD_MonthRep.RefKDBridge, groupKD.Value);
                    dt.Rows.Add(groupKD.Key, GetSumFieldValue(tblData, SUM1, filter));
                }

                rep.ProcessDataRows(dt.Select(), planColumn.Index);
            }

            // получаем данные из т.ф. УФК_Справка о перечисленных поступлениях в бюджет (f.D.UFK22)
            {
                var filterQUFK22 = new List<QFilter>
                {
                    new QFilter(QdUFK22.Keys.Period, String.Empty),
                    new QFilter(QdUFK22.Keys.KD, filterKD),
                    new QFilter(QdUFK22.Keys.Marks, paramMarks)
                };

                var groupQUFK22 = new List<Enum> {QdUFK22.Keys.KD};

                for (var i = 0; i < periods.Count; i++)
                {
                    filterQUFK22[0].Filter = periods[i];
                    var query = new QdUFK22().GetQueryText(filterQUFK22, groupQUFK22);
                    var tblData = dbHelper.GetTableData(query);
                    dt.Rows.Clear();

                    foreach (var groupKD in nestedKD.Where(e => e.Value != String.Empty))
                    {
                        var filter = filterHelper.RangeFilter(d_KD_UFK.RefKDBridge, groupKD.Value);
                        dt.Rows.Add(groupKD.Key, GetSumFieldValue(tblData, SUM, filter));
                    }

                    rep.ProcessDataRows(dt.Select(), planColumn.Index + 1 + i);
                }
            }

            tablesResult[0] = rep.GetReportData();

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.ENDDATE, paramDate.ToShortDateString());
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
