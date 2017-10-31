using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.Database.ClsBridge;
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
        /// ОТЧЕТ 001 НАЛОГ НА ПРИБЫЛЬ К ДОПЛАТЕ (УМЕНЬШЕНИЮ) ИТОГОВЫЙ СВОД
        /// </summary>
        public DataTable[] GetMOFO0024Report001Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;
            var paramQuarter = reportParams[ReportConsts.ParamQuarter];
            var quarter = Convert.ToInt32(GetEnumItemValue(new QuarterEnum(), paramQuarter)) + 1;
            var filterQuarter = GetUNVYearQuarter(year, quarter);
            // фильтры
            var levelFilters = new[]
            {
                filterHelper.EqualIntFilter(f_Marks_TaxBenPay.RefBdgLevels, ReportMOFO0024Helper.BdgtLevelAll),
                filterHelper.EqualIntFilter(f_Marks_TaxBenPay.RefBdgLevels, ReportMOFO0024Helper.BdgtLevelFederal),
                filterHelper.EqualIntFilter(f_Marks_TaxBenPay.RefBdgLevels, ReportMOFO0024Helper.BdgtLevelSubject),
            };

            var markFilterAll = filterHelper.EqualIntFilter(f_Marks_TaxBenPay.RefMarks, ReportMOFO0024Helper.MarkAll);
            var markFilterPayment = filterHelper.EqualIntFilter(f_Marks_TaxBenPay.RefMarks, ReportMOFO0024Helper.MarkPayment);
            var markFilterReduction = filterHelper.EqualIntFilter(f_Marks_TaxBenPay.RefMarks, ReportMOFO0024Helper.MarkReduction);

            var filterAte = ReportConsts.UndefinedKey;
            var dt = dbHelper.GetEntityData(b_Regions_Bridge.InternalKey);
            var subjectRow = AteGrouping.GetSubjectRow(dt);
            if (subjectRow != null)
            {
                var id = Convert.ToString(subjectRow[b_Regions_Bridge.ParentID]);
                filterAte = reportHelper.GetNestedIDByField(b_Regions_Bridge.InternalKey, b_Regions_Bridge.ID, id);
            }

            // создаем таблицу отчета
            var repTable = new DataTable();
            repTable.Columns.Add();
            repTable.Columns.Add(SUM, typeof(decimal));
            repTable.Columns.Add(SUM1, typeof(decimal));
            repTable.Columns.Add(STYLE, typeof(int));

            // получаем данные "Всего"
            var filterList = new List<QFilter>
            {
                new QFilter(QMarksTaxBenPay.Keys.Day, filterQuarter),
                new QFilter(QMarksTaxBenPay.Keys.Okato, filterAte),
                new QFilter(QMarksTaxBenPay.Keys.Org, ReportMOFO0024Helper.OrgRefUndefined)
            };

            var groupFields = new List<Enum> { QMarksTaxBenPay.Keys.Lvl, QMarksTaxBenPay.Keys.Mark };
            var query = new QMarksTaxBenPay().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(query);

            foreach (var levelFilter in levelFilters)
            {
                var row = repTable.Rows.Add();
                row[SUM] = GetSumFieldValue(tblData, f_Marks_TaxBenPay.SumPayment, CombineAnd(levelFilter, markFilterAll));
                row[SUM1] = GetSumFieldValue(tblData, f_Marks_TaxBenPay.SumReduction, CombineAnd(levelFilter, markFilterAll));
            }

            // получаем данные по крупнейшим плательщикам
            filterList[2] = new QFilter(QMarksTaxBenPay.Keys.Org, ReportMOFO0024Helper.OrgRefUndefined, true);
            query = new QMarksTaxBenPay().GetQueryText(filterList, groupFields);
            tblData = dbHelper.GetTableData(query);

            foreach (var levelFilter in levelFilters)
            {
                var row = repTable.Rows.Add();
                row[SUM] = GetSumFieldValue(tblData, f_Marks_TaxBenPay.SumPayment, CombineAnd(levelFilter, markFilterPayment));
                row[SUM1] = GetSumFieldValue(tblData, f_Marks_TaxBenPay.SumReduction, CombineAnd(levelFilter, markFilterReduction));
            }

            // делим суммы в зависимости от выбранных единиц измерения
            DivideSum(repTable, 1, 2, divider);

            tablesResult[0] = repTable;
            
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
