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
        /// ОТЧЕТ 023 ДИНАМИКА ПОСТУПЛЕНИЙ ПО КД В РАЗРЕЗЕ АДМИНИСТРАТОРОВ
        /// </summary>
        public DataTable[] GetUFKReport023(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[3];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var filterKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var filterKVSR = reportParams[ReportConsts.ParamKVSRComparable];
            var paramLvl = SortCodes(ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamBdgtLevels]));
            var yearList = GetSelectedYears(reportParams[ReportConsts.ParamYear]);

            // администраторы
            var kvsrs = new Dictionary<int, string>();
            if (filterKVSR != ReportConsts.UndefinedKey)
            {
                var filter = filterHelper.RangeFilter(b_KVSR_Bridge.ID, filterKVSR);
                var dt = dbHelper.GetEntityData(b_KVSR_Bridge.InternalKey, filter);
                kvsrs = (from DataRow row in dt.Rows
                         let id = Convert.ToInt32(row[b_KVSR_Bridge.ID])
                         let code = Convert.ToInt32(row[b_KVSR_Bridge.Code])
                         let name = Convert.ToString(row[b_KVSR_Bridge.Name])
                         orderby code
                         select new {id, name}).ToDictionary(e => e.id, e => e.name);
            }

            // уровни
            var levels = new List<string>
            {
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Federal),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsSubjectFractional),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsMunicipalFractional),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Municipal),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Settle),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Fonds),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.PurposeFonds)
            };

            var intLvl = paramLvl != String.Empty ? ConvertToIntList(paramLvl) : new List<int>();
            levels = levels.Where((t, i) => intLvl.Contains(i)).ToList();

            // фильтры фактов
            var filterList = new List<QFilter>
            {
                new QFilter(QUFK14.Keys.Period,  String.Empty),
                new QFilter(QUFK14.Keys.KVSR,  filterKVSR),
                new QFilter(QUFK14.Keys.Lvl,  ConvertToString(levels)),
                new QFilter(QUFK14.Keys.KD,  filterKD)
            };

            var groupFields = new List<Enum> { QUFK14.Keys.KVSR };

            // заполняем таблицу отчета
            var columnsInYear = kvsrs.Count > 1 ? kvsrs.Count + 1 : kvsrs.Count;
            var sumColumnsCount = yearList.Count * columnsInYear;
            var repTable = new DataTable();
            AddColumnToReport(repTable, typeof(decimal), SUM, sumColumnsCount);
            AddColumnToReport(repTable, typeof(int), STYLE);
            var yearSumColumnsCount = kvsrs.Count > 1 ? 1 : 0;

            for (var month = 1; month <= 12; month++)
            {
                var row = repTable.Rows.Add();
                row[STYLE] = month - 1;
                var i = 0;

                foreach (var year in yearList)
                {
                    i += yearSumColumnsCount;
                    var monthStart = GetUNVMonthStart(year, month);
                    var monthEnd = GetUNVMonthEnd(year, month);
                    var period = filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV, monthStart, monthEnd, true);
                    filterList[0] = new QFilter(QUFK14.Keys.Period, period);
                    var query = new QUFK14().GetQueryText(filterList, groupFields);
                    var tblData = dbHelper.GetTableData(query);
                    var values = new List<object>();

                    foreach (var kvsr in kvsrs)
                    {
                        var filter = filterHelper.EqualIntFilter(d_KD_UFK.RefKVSRBridge, kvsr.Key);
                        row[i] = GetSumFieldValue(tblData, f_D_UFK14.Credit, filter);
                        values.Add(row[i++]);
                    }

                    if (kvsrs.Count > 1)
                    {
                        var yearSumColumn = i - kvsrs.Count - 1;
                        row[yearSumColumn] = Functions.Sum(values);
                    }
                }
            }

            // делим суммы в зависимости от выбранных единиц измерения
            DivideSum(repTable, 0, sumColumnsCount, divider);

            tablesResult[0] = repTable;

            // заполняем таблицу колонок
            var columnsDt = new DataTable();
            AddColumnToReport(columnsDt, typeof(string), NAME);
            AddColumnToReport(columnsDt, typeof(string), STYLE);
            columnsDt.Rows.Add(String.Empty, 0);
            var firstYearStyles = new[]
                                      {
                                          1,
                                          1 + columnsInYear,
                                          1 + columnsInYear * 3
                                      };

            for (var i = 0; i < yearList.Count; i++)
            {
                var firstYearStyle = i < 2 ? firstYearStyles[i] : firstYearStyles[2];

                for (var l = 0; l < columnsInYear; l++)
                {
                    var index = i == 0 ? firstYearStyle + l : firstYearStyle + l * 2;
                    var name = l == 0 && kvsrs.Count > 1
                                   ? (object) DBNull.Value
                                   : kvsrs.Count > 1 
                                         ? kvsrs.ElementAt(l - 1).Value
                                         : kvsrs.ElementAt(l).Value;
                    columnsDt.Rows.Add(name, index);
                    if (i > 0)
                    {
                        columnsDt.Rows.Add(DBNull.Value, index + 1);
                    }
                }
            }

            tablesResult[1] = columnsDt;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            var bdgtName = paramLvl != String.Empty
                   ? ReportMonthMethods.GetSelectedBudgetLvlFull(paramLvl)
                   : String.Empty;
            paramHelper.SetParamValue(ParamUFKHelper.YEARS, ConvertToString(yearList));
            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(filterKD));
            paramHelper.SetParamValue(ParamUFKHelper.KVSR, ConvertToString(kvsrs.Keys));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            paramHelper.SetParamValue(ParamUFKHelper.BDGT_NAME, bdgtName);
            return tablesResult;
        }
    }
}
