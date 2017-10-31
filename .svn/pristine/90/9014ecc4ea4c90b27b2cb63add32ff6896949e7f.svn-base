using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Database.FactTables.MOFO;
using Krista.FM.Client.Reports.MOFO.Queries;
using Krista.FM.Client.Reports.MOFO0024.Helpers;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 005 ПОКВАРТАЛЬНЫЙ АНАЛИЗ ПОСТУПЛЕНИЙ НАЛОГА НА ПРИБЫЛЬ ОРГАНИЗАЦИЙ
        /// </summary>
        public DataTable[] GetMOFO0024Report005Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[2];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var yearList = GetSelectedYears(reportParams[ReportConsts.ParamYear]);

            // фильтры фактов
            var filterQMarks = new List<QFilter>
            {
                new QFilter(QMarksTaxBenPay.Keys.Day, String.Empty),
                new QFilter(QMarksTaxBenPay.Keys.Lvl, ReportMOFO0024Helper.BdgtLevelSubject),
                new QFilter(QMarksTaxBenPay.Keys.Mark, ReportMOFO0024Helper.MarkAll),
                new QFilter(QMarksTaxBenPay.Keys.Org, ReportMOFO0024Helper.OrgRefUndefined)
            };

            var filterQMonthRep = new List<QFilter>
            {
                new QFilter(QMonthRep.Keys.Day, String.Empty),
                new QFilter(QMonthRep.Keys.KD, paramKD),
                new QFilter(QMonthRep.Keys.Lvl, ReportMonthMethods.GetBdgtLvlSKIFCodes(SettleLvl.Subject)),
                new QFilter(QMonthRep.Keys.DocTyp, ReportMonthMethods.GetDocumentTypesSkif(SettleLvl.ConsSubject))
            };

            var filterQFOPlan = new List<QFilter>
            {
                new QFilter(QFOPlanIncDivide.Keys.Period, String.Empty),
                new QFilter(QFOPlanIncDivide.Keys.KD, paramKD),
                new QFilter(QFOPlanIncDivide.Keys.Lvl, ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject)),
                new QFilter(QFOPlanIncDivide.Keys.Variant, ReportMOFO0024Helper.VariantPlanIncomesMonth)
            };

            var repDt = new DataTable();
            AddColumnToReport(repDt, typeof (decimal), SUM, 4);

            foreach (var year in yearList)
            {
                var planRow = repDt.Rows.Add();
                var factRow = repDt.Rows.Add();
                var paymentRow = repDt.Rows.Add();
                var reductionRow = repDt.Rows.Add();

                // получаем данные из т.ф. «ФО_Результат доходов с расщеплением» (f_D_FOPlanIncDivide)
                var period = filterHelper.PeriodFilter(f_D_FOPlanIncDivide.RefYearDayUNV, GetUNVYearStart(year), GetUNVYearEnd(year), true);
                filterQFOPlan[0] = new QFilter(QFOPlanIncDivide.Keys.Period, period);
                var query = new QFOPlanIncDivide().GetQueryText(filterQFOPlan, new List<Enum> {QFOPlanIncDivide.Keys.Day});
                query = String.Format("{0}\r\nORDER BY\r\n\t{1}{2} DESC",
                                        query,
                                        QFilterHelper.fltPrefix,
                                        f_D_FOPlanIncDivide.RefYearDayUNV);
                var dt = dbHelper.GetTableData(query);
                if (dt.Rows.Count > 0)
                {
                    planRow[0] = dt.Rows[0][f_D_FOPlanIncDivide.YearPlan];
                }

                for (var i = 1; i <= 4; i++)
                {
                    // получаем данные из т.ф. «МОФО_НП к доплате_уменьшению» (f_Marks_TaxBenPay).
                    var unvQuarter = i > 1 ? GetUNVYearQuarter(year, i - 1) : GetUNVYearQuarter(year - 1, 4);
                    filterQMarks[0] = new QFilter(QMarksTaxBenPay.Keys.Day, unvQuarter);
                    query = new QMarksTaxBenPay().GetQueryText(filterQMarks);
                    dt = dbHelper.GetTableData(query);
                    if (dt.Rows.Count > 0)
                    {
                        paymentRow[i - 1] = dt.Rows[0][f_Marks_TaxBenPay.SumPayment];
                        reductionRow[i - 1] = dt.Rows[0][f_Marks_TaxBenPay.SumReduction];
                    }

                    // получаем данные из т.ф. «Факт.ФО.МесОтч.Доходы» (f_F_MonthRepIncomes)
                    filterQMonthRep[0] = new QFilter(QMonthRep.Keys.Day, GetUNVMonthStart(year, i * 3));
                    query = new QMonthRep().GetQueryText(filterQMonthRep);
                    dt = dbHelper.GetTableData(query);
                    if (dt.Rows.Count > 0)
                    {
                        factRow[i - 1] = dt.Rows[0][SUM];
                    }
                }
            }

            // делим суммы в зависимости от выбранных единиц измерения
            DivideSum(repDt, 0, 4, divider);

            tablesResult[0] = repDt;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.NOW, DateTime.Now.ToShortDateString());
            paramHelper.SetParamValue(ParamUFKHelper.YEARS, ConvertToString(yearList));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
