using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsData.MOFO;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.Database.FactTables.MOFO;
using Krista.FM.Client.Reports.MOFO.Queries;
using Krista.FM.Client.Reports.MOFO0024.Helpers;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK.ReportMaster;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{

    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 003 НАЛОГ НА ПРИБЫЛЬ К ДОПЛАТЕ (УМЕНЬШЕНИЮ)
        /// </summary>
        public DataTable[] GetMOFO0024Report003Data(Dictionary<string, string> reportParams)
        {
            const int ateFirstStyle = 1;
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[2];
            var paramShowOrg = Convert.ToBoolean(reportParams[ReportConsts.ParamOutputMode]);
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;
            var paramQuarter = reportParams[ReportConsts.ParamQuarter];
            var quarter = Convert.ToInt32(GetEnumItemValue(new QuarterEnum(), paramQuarter)) + 1;
            var filterQuarter = GetUNVYearQuarter(year, quarter);

            // фильтры
            var fltLvlFed = filterHelper.EqualIntFilter(f_Marks_TaxBenPay.RefBdgLevels, ReportMOFO0024Helper.BdgtLevelFederal);
            var fltLvlSub = filterHelper.EqualIntFilter(f_Marks_TaxBenPay.RefBdgLevels, ReportMOFO0024Helper.BdgtLevelSubject);
            var fltLvlAll = Combine(ReportMOFO0024Helper.BdgtLevelFederal, ReportMOFO0024Helper.BdgtLevelSubject);
            var fltMarkAll = Combine(ReportMOFO0024Helper.MarkPayment, ReportMOFO0024Helper.MarkReduction);
            var fltMarkPayment = filterHelper.EqualIntFilter(f_Marks_TaxBenPay.RefMarks, ReportMOFO0024Helper.MarkPayment);
            var fltMarkReduction = filterHelper.EqualIntFilter(f_Marks_TaxBenPay.RefMarks, ReportMOFO0024Helper.MarkReduction);

            // устанавливаем параметры отчета
            var rep = new Report(f_Marks_TaxBenPay.TableKey) {AddTotalRow = false, Divider = GetDividerValue(divider)};

            // группировка по АТЕ
            var ateGrouping = new MOFO0024AteGrouping(ateFirstStyle);
            rep.AddGrouping(d_Regions_Plan.RefBridge, ateGrouping);

            // группировка по организации
            const string orgTableKey = d_Org_TaxBenPay.TableKey;
            var orgGrouping = rep.AddGrouping(f_Marks_TaxBenPay.RefOrg);
            orgGrouping.AddLookupField(orgTableKey, d_Org_TaxBenPay.INN, d_Org_TaxBenPay.Name);
            orgGrouping.AddSortField(orgTableKey, d_Org_TaxBenPay.INN);
            orgGrouping.ViewParams[0].Style = 0;

            // настраиваем колонки отчета
            const string regionsTableKey = b_Regions_Bridge.InternalKey;
            const string terTypeTableKey = fx_FX_TerritorialPartitionType.InternalKey;
            // колонка Код АТЕ, ИНН
            var codeColumn = rep.AddCaptionColumn();
            var masks = new AteOutMasks(new TableField(regionsTableKey, b_Regions_Bridge.CodeLine));
            codeColumn.SetMasks(ateGrouping, masks);
            codeColumn.SetMask(orgGrouping, 0, orgTableKey, d_Org_TaxBenPay.INN);
            // колонка Наименование АТЕ, налогоплательщика
            var nameColumn = rep.AddCaptionColumn();
            masks = new AteOutMasks(new TableField(terTypeTableKey, fx_FX_TerritorialPartitionType.Name),
                                    new TableField(regionsTableKey, b_Regions_Bridge.Name));
            nameColumn.SetMasks(ateGrouping, masks);
            nameColumn.SetMask(ateGrouping, ateGrouping.LastLevel, MOFO0024AteGrouping.Title);
            nameColumn.SetMask(orgGrouping, 0, orgTableKey, d_Org_TaxBenPay.Name);
            // колонки сумм
            rep.AddValueColumn(f_Marks_TaxBenPay.SumPayment, fltLvlFed);
            rep.AddValueColumn(f_Marks_TaxBenPay.SumPayment, fltLvlSub);
            rep.AddValueColumn(f_Marks_TaxBenPay.SumReduction, fltLvlFed);
            rep.AddValueColumn(f_Marks_TaxBenPay.SumReduction, fltLvlSub);

            // получаем данные по АТЕ
            var filterList = new List<QFilter>
            {
                new QFilter(QMarksTaxBenPay.Keys.Day, filterQuarter),
                new QFilter(QMarksTaxBenPay.Keys.Lvl, fltLvlAll),
                new QFilter(QMarksTaxBenPay.Keys.Mark, ReportMOFO0024Helper.MarkAll),
                new QFilter(QMarksTaxBenPay.Keys.Org, ReportMOFO0024Helper.OrgRefUndefined)
            };

            var groupAte = new List<Enum> { QMarksTaxBenPay.Keys.Okato, QMarksTaxBenPay.Keys.Lvl };
            var query = new QMarksTaxBenPay().GetQueryText(filterList, groupAte);
            var tblData = dbHelper.GetTableData(query);
            rep.ProcessTable(tblData);

            // получаем данные по организациям
            if (paramShowOrg)
            {
                filterList[2] = new QFilter(QMarksTaxBenPay.Keys.Mark, fltMarkAll);
                filterList[3] = new QFilter(QMarksTaxBenPay.Keys.Org, ReportMOFO0024Helper.OrgRefUndefined, true);

                var groupOrg = new List<Enum>
                {
                    QMarksTaxBenPay.Keys.Okato,
                    QMarksTaxBenPay.Keys.Org,
                    QMarksTaxBenPay.Keys.Mark,
                    QMarksTaxBenPay.Keys.Lvl
                };

                var queryOrg = new QMarksTaxBenPay().GetQueryText(filterList, groupOrg);
                var tblDataOrg = dbHelper.GetTableData(queryOrg);
                var i = nameColumn.Index;
                rep.ProcessDataRows(tblDataOrg.Select(CombineAnd(fltLvlFed, fltMarkPayment)), i + 1);
                rep.ProcessDataRows(tblDataOrg.Select(CombineAnd(fltLvlSub, fltMarkPayment)), i + 2);
                rep.ProcessDataRows(tblDataOrg.Select(CombineAnd(fltLvlFed, fltMarkReduction)), i + 3);
                rep.ProcessDataRows(tblDataOrg.Select(CombineAnd(fltLvlSub, fltMarkReduction)), i + 4);
            }

            tablesResult[0] = rep.GetReportData();

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.NOW, DateTime.Now.ToShortDateString());
            paramHelper.SetParamValue(ParamUFKHelper.YEARS, year);
            paramHelper.SetParamValue(ParamUFKHelper.QUARTER, quarter);
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
