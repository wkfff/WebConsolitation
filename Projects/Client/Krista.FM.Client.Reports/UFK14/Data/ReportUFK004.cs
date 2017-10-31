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
        /// ОТЧЕТ 004 ПОСТУПЛЕНИЕ НАЛОГА НА ПРИБЫЛЬ И НДФЛ С ТЕРРИТОРИИ СУБЪЕКТА РФ ОТ СТРУКТУРНЫХ ПОДРАЗДЕЛЕНИЙ
        /// </summary>
        public DataTable[] GetUFKReport004(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramShowOrg = Convert.ToBoolean(reportParams[ReportConsts.ParamOutputMode]);
            var paramStructure = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamStructure]);
            var paramStartDate = reportParams[ReportConsts.ParamStartDate];
            var paramEndDate = reportParams[ReportConsts.ParamEndDate];
            var startDate = GetUNVDate(paramStartDate);
            var endDate = GetUNVDate(paramEndDate);
            var year = Convert.ToDateTime(paramEndDate).Year;
            var filterPeriod = filterHelper.PeriodFilter(f_D_UFK14.RefYearDayUNV, startDate, endDate, true);
            
            // КД
            var kds = new Dictionary<string, string>
            {
                {b_KD_Bridge.CodeNP, reportHelper.GetKDNestedID(b_KD_Bridge.CodeNP)},
                {b_KD_Bridge.CodeNDFL, reportHelper.GetKDNestedID(b_KD_Bridge.CodeNDFL)}
            };

            // Уровни
            var levels = new Dictionary<string, List<string>>
            {
                {
                    b_KD_Bridge.CodeNP, new List<string>
                    {
                        ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Federal),
                        ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject)
                    }
                },
                {
                    b_KD_Bridge.CodeNDFL, new List<string>
                    {
                        ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject),
                        ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Municipal),
                        ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Settle)
                    }
                }
            };

            var lvl = levels.Values.SelectMany(level => level).Distinct();

            // получаем данные из т.ф.
            var filterList = new List<QFilter>
            {
                new QFilter(QUFK14.Keys.Period, filterPeriod),
                new QFilter(QUFK14.Keys.Lvl, ConvertToString(lvl)),
                new QFilter(QUFK14.Keys.KD, ConvertToString(kds.Values)),
                new QFilter(QUFK14.Keys.Struc, paramStructure)
            };

            var groupFields = new List<Enum> {QUFK14.Keys.Okato, QUFK14.Keys.KD, QUFK14.Keys.Lvl};
            if (paramShowOrg)
            {
                groupFields.Add(QUFK14.Keys.Org);
            }
            var query = new QUFK14().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(query);

            // параметры отчета
            var rep = new Report(f_D_UFK14.InternalKey) {Divider = GetDividerValue(divider)};

            // группировка по АТЕ
            var ateGrouping = rep.AddGrouping(d_OKATO_UFK.RefRegionsBridge,
                                              new AteGrouping(1) {HideConsBudjetRow = true});

            // группировка по организациям
            const string orgTableKey = b_Org_PayersBridge.InternalKey;
            var orgGrouping = rep.AddGrouping(d_Org_UFKPayers.RefOrgPayersBridge);
            orgGrouping.AddLookupField(orgTableKey, b_Org_PayersBridge.INN, b_Org_PayersBridge.Name);
            orgGrouping.AddSortField(orgTableKey, b_Org_PayersBridge.INN);
            orgGrouping.ViewParams[0].Style = 0;

            // настраиваем колонки отчета
            var captionColumn = rep.AddCaptionColumn();
            captionColumn.SetMask(orgGrouping, 0, orgTableKey, b_Org_PayersBridge.INN, b_Org_PayersBridge.Name);
            captionColumn.SetMasks(ateGrouping, new AteOutMasks());

            foreach (var kd in kds)
            {
                var kdFilter = filterHelper.RangeFilter(d_KD_UFK.RefKDBridge, kd.Value);

                foreach (var level in levels[kd.Key])
                {
                    var lvlFilter = filterHelper.RangeFilter(f_D_UFK14.RefFX, level);
                    rep.AddValueColumn(f_D_UFK14.Credit, CombineAnd(kdFilter, lvlFilter));
                }
            }

            rep.ProcessTable(tblData);
            tablesResult[0] = rep.GetReportData();

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.YEAR, year);
            paramHelper.SetParamValue(ParamUFKHelper.STARTDATE, paramStartDate);
            paramHelper.SetParamValue(ParamUFKHelper.ENDDATE, paramEndDate);
            paramHelper.SetParamValue(ParamUFKHelper.STRUCTURE, reportHelper.GetStructureCharacterCaptionText(paramStructure));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
