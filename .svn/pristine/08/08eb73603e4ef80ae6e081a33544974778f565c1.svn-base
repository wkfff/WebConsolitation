using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
        /// ОТЧЕТ 017 ПОСТУПЛЕНИЯ ДОХОДОВ ОТ ОРГАНИЗАЦИЙ, РАССМАТРИВАЕМЫХ НА МЕЖВЕДОМСТВЕННОЙ РАБОЧЕЙ ГРУППЕ
        /// </summary>
        public DataTable[] GetUFKReport017(Dictionary<string, string> reportParams)
        {
            const int yearsCount = 4;
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[3];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var filterKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var paramStartDate = Convert.ToDateTime(reportParams[ReportConsts.ParamStartDate]);
            var paramEndDate = Convert.ToDateTime(reportParams[ReportConsts.ParamEndDate]);
            var fltOrg = reportParams[ReportConsts.ParamOrgID];
            var paramLvl = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamBdgtLevels]);

            // уровни
            var levels = new List<string>
            {
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Federal),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsSubjectFractional),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.ConsMunicipalFractional),
            };

            var intLvl = paramLvl != String.Empty ? ConvertToIntList(paramLvl) : GetColumnsList(0, levels.Count);
            intLvl.Sort();
            levels = levels.Where((t, i) => intLvl.Contains(i)).ToList();
            var levelsAll = GetDistinctCodes(ConvertToString(levels));
            if (intLvl.Count > 1)
            {
                levels.Insert(0, levelsAll); // уровни колонка "Всего"
            }

            var fltLevels = levels.Select(l => filterHelper.RangeFilter(f_D_UFK14.RefFX, l));

            // периоды
            if (paramStartDate.Year != paramEndDate.Year)
            {
                paramStartDate = new DateTime(paramEndDate.Year, 1, 1);
            }

            var periods = new List<string>();

            for (var i = yearsCount - 1; i >= 0; i--)
            {
                var startDate = GetUNVDate(paramStartDate.AddYears(-i));
                var endDate = GetUNVDate(paramEndDate.AddYears(-i));
                periods.Add(filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV, startDate, endDate, true));
                if (i > 0)
                {
                    var startYearDate = GetUNVYearStart(paramEndDate.Year - i);
                    var endYearDate = GetUNVYearEnd(paramEndDate.Year - i);
                    periods.Add(filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV, startYearDate, endYearDate, true));
                }
            }

            // разбиваем выбранные КД на невложенные группы
            const string kdTableKey = b_KD_Bridge.InternalKey;
            var selectedKD = ReportUFKHelper.GetSelectedID(filterKD, kdTableKey);
            var notNestedKD = ConvertToIntList(reportHelper.GetNotNestedID(kdTableKey, ConvertToString(selectedKD)));
            var nestedKD = new Dictionary<int, string>();
            foreach (var id in notNestedKD)
            {
                var nestedKDAll = ConvertToIntList((reportHelper.GetNestedIDByField(kdTableKey, b_KD_Bridge.ID, Convert.ToString(id))));
                var selectedNestedKD = nestedKDAll.Where(selectedKD.Contains);
                nestedKD.Add(id, ConvertToString(selectedNestedKD));
            }

            // параметры отчета
            const string tableKey = f_D_UFK14.InternalKey;
            var rep = new Report(tableKey) { AddTotalRow = false, Divider = GetDividerValue(divider) };

            // группировка по организациям
            const string orgTableKey = b_Org_PayersBridge.InternalKey;
            var grColumn = rep.AddGrouping(d_Org_UFKPayers.RefOrgPayersBridge);
            grColumn.AddLookupField(orgTableKey, b_Org_PayersBridge.INN, b_Org_PayersBridge.Name);
            grColumn.AddSortField(orgTableKey, b_Org_PayersBridge.INN);
            if (fltOrg != ReportConsts.UndefinedKey)
            {
                grColumn.SetFixedValues(fltOrg);
            }
            // группировка по КД
            var kdGrouping = rep.AddGrouping(d_KD_UFK.RefKDBridge);
            kdGrouping.AddLookupField(kdTableKey, b_KD_Bridge.CodeStr, b_KD_Bridge.Name);
            kdGrouping.AddSortField(kdTableKey, b_KD_Bridge.CodeStr);

            // настраиваем колонки отчета
            var nameColumn = rep.AddCaptionColumn(grColumn, orgTableKey, b_Org_PayersBridge.INN, b_Org_PayersBridge.Name);
            nameColumn.SetMask(kdGrouping, 0, kdTableKey, b_KD_Bridge.CodeStr, b_KD_Bridge.Name);

            var yearColumns = new List<int>{nameColumn.Index};
            for (var i = 0; i < periods.Count; i++)
            {
                foreach (var level in levels)
                {
                    var column = rep.AddValueColumn(f_D_UFK14.Credit);

                    if (i % 2 == 1)
                    {
                        yearColumns.Add(column.Index);
                    }
                }
            }

            // получаем данные
            var filterList = new List<QFilter>
            {
                new QFilter(QUFK14.Keys.Period, String.Empty), // зарезервировано
                new QFilter(QUFK14.Keys.KD, String.Empty), // зарезервировано
                new QFilter(QUFK14.Keys.Org, fltOrg),
                new QFilter(QUFK14.Keys.Lvl, levelsAll)
            };

            var groupFields = new List<Enum> { QUFK14.Keys.Org, QUFK14.Keys.Lvl };

            foreach (var kd in nestedKD)
            {
                var columnIndex = nameColumn.Index;
                var qufk = new QUFK14
                {
                    Result = String.Format("Sum({0}.{1}) as {1}, {2} as {3}",
                                           QFilterHelper.fltPrefixName, f_D_UFK14.Credit, kd.Key, d_KD_UFK.RefKDBridge)
                };

                foreach (var period in periods)
                {
                    filterList[0].Filter = period;
                    filterList[1].Filter = kd.Value;
                    var query = qufk.GetQueryText(filterList, groupFields);
                    var tblData = dbHelper.GetTableData(query);

                    foreach (var fltLevel in fltLevels)
                    {
                        rep.ProcessDataRows(tblData.Select(fltLevel), ++columnIndex);
                    }
                }
            }

            var dt = rep.GetReportData();
            yearColumns.Add(dt.Columns.Count - 1); // колонка Style
            tablesResult[0] = dt;
            tablesResult[1] = GetTableCopy(dt, yearColumns);

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.STARTDATE, paramStartDate);
            paramHelper.SetParamValue(ParamUFKHelper.ENDDATE, paramEndDate);
            paramHelper.SetParamValue(ParamUFKHelper.BDGT_LEVEL, ConvertToString(intLvl));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
