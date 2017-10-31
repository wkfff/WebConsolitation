using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData.UFK;
using Krista.FM.Client.Reports.Database.ClsFx;
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
        /// ОТЧЕТ 022 АНАЛИЗ ПОСТУПЛЕНИЙ НАЛОГА НА ПРИБЫЛЬ
        /// </summary>
        public DataTable[] GetUFKReport022(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[3];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var filterKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var paramDate = Convert.ToDateTime(reportParams[ReportConsts.ParamEndDate]);
            var paramINN = reportParams[ReportConsts.ParamINN];
            var bdgtLevel = ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject); // только областной бюджет
            var paramHideEmptyStr = Convert.ToBoolean(reportParams[ReportConsts.ParamHideEmptyStr]);

            // периоды
            var yearStart = GetUNVYearStart(paramDate.Year);
            var date = GetUNVDate(paramDate.AddDays(-1));
            var yearEnd = GetUNVYearEnd(paramDate.Year);
            var prevYearStart = GetUNVYearStart(paramDate.Year - 1);
            var prevDate = GetUNVDate(paramDate.AddYears(-1).AddDays(-1));
            var prevYearEnd = GetUNVYearEnd(paramDate.Year - 1);
            var periods = new[]
            {
                filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV, prevYearStart, prevDate, true),
                filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV, prevYearStart, prevYearEnd, true),
                filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV, yearStart, date, true),
                filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV, yearStart, yearEnd, true)
            };

            // организации
            var filterOrg = ReportConsts.UndefinedKey;
            if (paramINN != String.Empty)
            {
                const string indexRegExp = @"(^|\s|,)(?<inn>\d+)(\s|,|$)";
                var innList = Regex.Matches(paramINN, indexRegExp).Cast<Match>().Select(m => m.Groups["inn"].Value);
                if (innList.Count() > 0)
                {
                    var filterINN = filterHelper.RangeFilter(b_Org_PayersBridge.INN, ConvertToString(innList));
                    var tblOrg = dbHelper.GetEntityData(b_Org_PayersBridge.InternalKey, filterINN);
                    if (tblOrg.Rows.Count > 0)
                    {
                        var idList = tblOrg.Rows.Cast<DataRow>().Select(row => Convert.ToInt32(row[b_Org_PayersBridge.ID]));
                        filterOrg = ConvertToString(idList);
                    }
                }
            }

            // фильтры фактов
            var filterList = new List<QFilter>
            {
                new QFilter(QUFK14.Keys.Period, String.Empty),
                new QFilter(QUFK14.Keys.Lvl, bdgtLevel),
                new QFilter(QUFK14.Keys.Org, filterOrg),
                new QFilter(QUFK14.Keys.Struc, ReportUFKHelper.StructureMO),
                new QFilter(QUFK14.Keys.KD, filterKD)
            };

            var rep = new Report(f_D_UFK14.InternalKey) {Divider = GetDividerValue(divider), AddNumColumn = true};

            // группировка по организациям
            const string orgTableKey = b_Org_PayersBridge.InternalKey;
            var grColumnOrg = rep.AddGrouping(d_Org_UFKPayers.RefOrgPayersBridge);
            grColumnOrg.AddLookupField(orgTableKey, b_Org_PayersBridge.INN, b_Org_PayersBridge.Name);
            grColumnOrg.AddSortField(orgTableKey, b_Org_PayersBridge.INN);
            if (!paramHideEmptyStr && filterOrg != ReportConsts.UndefinedKey)
            {
                grColumnOrg.SetFixedValues(filterOrg);
            }

            // колонки отчета
            var nameColumn = rep.AddCaptionColumn(grColumnOrg, orgTableKey, b_Org_PayersBridge.INN, b_Org_PayersBridge.Name);
            rep.AddValueColumn(f_D_UFK14.Credit);
            rep.AddValueColumn(f_D_UFK14.Credit);
            rep.AddValueColumn(f_D_UFK14.Credit);
            rep.AddValueColumn(f_D_UFK14.Credit);
            var returnColumn = rep.AddValueColumn(f_D_UFK14.Credit);
            returnColumn.K = -1;
            var fltOper = filterHelper.EqualIntFilter(f_D_UFK14.RefOpertnTypes, fx_FX_OpertnTypes.Return);
            var groupOper = new List<Enum> { QUFK14.Keys.Org, QUFK14.Keys.Oper };

            for (var i = 0; i < periods.Length; i++)
            {
                filterList[0].Filter = periods[i];
                var query = new QUFK14().GetQueryText(filterList, groupOper);
                var tblData = dbHelper.GetTableData(query);

                rep.ProcessDataRows(tblData.Select(), nameColumn.Index + i);
                if (i == 1 || i == 3)
                {
                    rep.ProcessDataRows(tblData.Select(fltOper), returnColumn.Index);
                }
            }

            var repDt = rep.GetReportData();

            // заполняем таблицу итоговых значений
            var groupFields = new List<Enum> { QUFK14.Keys.Struc, QUFK14.Keys.Oper };
            filterList[2].Filter = String.Empty;
            filterList[3].Filter = String.Empty;

            var totalDt = new DataTable();
            AddColumnToReport(totalDt, typeof(decimal), SUM, periods.Length + 1);
            var strucFilters = new List<string>
                                   {
                                       String.Empty,
                                       filterHelper.EqualIntFilter(d_Org_UFKPayers.RefFX, ReportUFKHelper.StructureNot),
                                       filterHelper.EqualIntFilter(d_Org_UFKPayers.RefFX, ReportUFKHelper.StructureMO)
                                   };
            foreach (var strucFilter in strucFilters)
            {
                totalDt.Rows.Add();
            }

            for (var i = 0; i < periods.Length; i++)
            {
                filterList[0].Filter = periods[i];
                var query = new QUFK14().GetQueryText(filterList, groupFields);
                var tblData = dbHelper.GetTableData(query);

                for (var j = 0; j < strucFilters.Count; j++)
                {
                    totalDt.Rows[j][i] = GetSumFieldValue(tblData, f_D_UFK14.Credit, strucFilters[j]);
                    if (i == 1 || i == 3)
                    {
                        var sum = GetSumFieldValue(tblData, f_D_UFK14.Credit, CombineAnd(strucFilters[j], fltOper));
                        if (sum != DBNull.Value)
                        {
                            totalDt.Rows[j][periods.Length] = GetDecimal(totalDt.Rows[j][periods.Length]) + -1*GetDecimal(sum);
                        }
                    }
                }
            }

            // делим суммы в таблице итоговых значений зависимости от выбранных единиц измерения
            DivideSum(totalDt, 0, totalDt.Columns.Count, divider);

            var totalRow = totalDt.Rows.Add();
            for (var i = 0; i < totalDt.Columns.Count; i++)
            {
                var row = repDt.Rows[repDt.Rows.Count - 1];
                totalRow[i] = row[nameColumn.Index + 1 + i];
            }

            repDt.Rows.RemoveAt(repDt.Rows.Count - 1);

            tablesResult[0] = repDt;
            tablesResult[1] = totalDt;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.ENDDATE, paramDate);
            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(filterKD));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
