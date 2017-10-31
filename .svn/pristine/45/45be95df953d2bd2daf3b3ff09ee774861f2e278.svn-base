using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsData.MOFO;
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
        /// ОТЧЕТ 002 АНАЛИЗ ИНФОРМАЦИИ ПО ПЕРЕРАСЧЕТАМ ПО НАЛОГУ НА ПРИБЫЛЬ ОРГАНИЗАЦИЙ
        /// </summary>
        public DataTable[] GetMOFO0024Report002Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[3];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramSum = Convert.ToDecimal(reportParams[ReportConsts.ParamSum]);
            var limitSum = paramSum * ReportMOFO0024Helper.LimitSumRate;
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;
            var paramQuarter = reportParams[ReportConsts.ParamQuarter];
            var quarter = Convert.ToInt32(GetEnumItemValue(new QuarterEnum(), paramQuarter)) + 1;

            // фильтры
            var filterQuarter = GetUNVYearQuarter(year, quarter);
            var markFilterPayment = filterHelper.EqualIntFilter(f_Marks_TaxBenPay.RefMarks, ReportMOFO0024Helper.MarkPayment);
            var markFilterReduction = filterHelper.EqualIntFilter(f_Marks_TaxBenPay.RefMarks, ReportMOFO0024Helper.MarkReduction);
            var markFilter = ConvertToString(new [] {ReportMOFO0024Helper.MarkPayment, ReportMOFO0024Helper.MarkReduction});
            
            // получаем данные по плательщикам
            var filterListOrg = new List<QFilter>
            {
                new QFilter(QMarksTaxBenPay.Keys.Day, filterQuarter),
                new QFilter(QMarksTaxBenPay.Keys.Lvl, ReportMOFO0024Helper.BdgtLevelSubject),
                new QFilter(QMarksTaxBenPay.Keys.Org, ReportMOFO0024Helper.OrgRefUndefined, true),
                new QFilter(QMarksTaxBenPay.Keys.Mark, markFilter)
            };

            var groupFieldsOrg = new List<Enum>
            {
                QMarksTaxBenPay.Keys.Org,
                QMarksTaxBenPay.Keys.Okato,
                QMarksTaxBenPay.Keys.Mark
            };
            var queryOrg = new QMarksTaxBenPay().GetQueryText(filterListOrg, groupFieldsOrg);
            var tblDataOrg = dbHelper.GetTableData(queryOrg);
            // устанавливаем параметры отчета
            var rep = new Report(f_Marks_TaxBenPay.TableKey)
            {
                AddNumColumn = true,
                Divider = GetDividerValue(divider),
                AddTotalRow = false
            };
            var dividedLimitSum = limitSum / rep.Divider;

            // группировка по организации
            const string orgTableKey = d_Org_TaxBenPay.TableKey;
            var orgGrouping = rep.AddGrouping(f_Marks_TaxBenPay.RefOrg);
            orgGrouping.AddLookupField(orgTableKey, d_Org_TaxBenPay.INN, d_Org_TaxBenPay.Name);
            orgGrouping.AddSortField(orgTableKey, d_Org_TaxBenPay.INN);
            orgGrouping.ViewParams[0].ShowOrder = RowViewParams.ShowType.SkipBeforeChild;
            // группировка по АТЕ
            var ateGrouping = new AteGrouping(1, true){HideConsBudjetRow = true};
            ateGrouping.ViewParams[0].Style = 0; // уровень области
            ateGrouping.ViewParams[1].ShowOrder = RowViewParams.ShowType.SkipAfterChild;  // уровень МР и ГО сводный
            ateGrouping.ViewParams[2].Style = 0; // уровень МР и ГО
            rep.AddGrouping(d_Regions_Plan.RefBridge, ateGrouping);

            // настраиваем колонки отчета
            // ИНН
            var innColumn = rep.AddCaptionColumn(orgGrouping);
            var innMask = innColumn.SetMask(ateGrouping, 2, orgTableKey, d_Org_TaxBenPay.INN);
            innMask.GroupingIndex = orgGrouping.Index;
            innMask.Level = 0;
            innMask = innColumn.SetMask(ateGrouping, 0, orgTableKey, d_Org_TaxBenPay.INN);
            innMask.GroupingIndex = orgGrouping.Index;
            innMask.Level = 0;
            // Наименование организации
            var nameColumn = rep.AddCaptionColumn(orgGrouping);
            var nameMask = nameColumn.SetMask(ateGrouping, 2, orgTableKey, d_Org_TaxBenPay.Name);
            nameMask.GroupingIndex = orgGrouping.Index;
            nameMask.Level = 0;
            nameMask = nameColumn.SetMask(ateGrouping, 0, orgTableKey, d_Org_TaxBenPay.Name);
            nameMask.GroupingIndex = orgGrouping.Index;
            nameMask.Level = 0;
            // Наименование АТЕ
            var masks = new AteOutMasks();
            rep.AddCaptionColumn().SetMasks(ateGrouping, masks);
            // Суммы
            var paymentColumn = rep.AddValueColumn(f_Marks_TaxBenPay.SumPayment, markFilterPayment);
            var reductionColumn = rep.AddValueColumn(f_Marks_TaxBenPay.SumReduction, markFilterReduction);
            // Получаем сгруппированные данные
            rep.ProcessTable(tblDataOrg);
            var dt = rep.GetReportData();

            var paymentColumnName = dt.Columns[paymentColumn.Index + 1].ColumnName;
            var reductionColumnName = dt.Columns[reductionColumn.Index + 1].ColumnName;

            // записываем в отчет строки по убыванию суммы к доплате
            var repDt = dt.Clone();
            var num = 0;
            decimal paymentSum = 0;
            var paymentRows = dt.Select(String.Format("{0} <> 0", paymentColumnName),
                                        String.Format("{0} DESC", paymentColumnName));
            foreach (var row in paymentRows)
            {
                var sum = GetDecimal(row[paymentColumnName]);
                if (Math.Abs(sum) >= dividedLimitSum)
                {
                    paymentSum += sum;
                    var repRow = repDt.Rows.Add(row.ItemArray);
                    repRow[0] = ++num;
                    repRow[reductionColumnName] = DBNull.Value;
                }
            }

            // записываем в отчет строки по возрастанию суммы к уменьшению
            decimal reductionSum = 0;
            var reductionRows = dt.Select(String.Format("{0} <> 0", reductionColumnName),
                                          String.Format("{0}", reductionColumnName));
            foreach (var row in reductionRows)
            {
                var sum = GetDecimal(row[reductionColumnName]);
                if (Math.Abs(sum) >= dividedLimitSum)
                {
                    reductionSum += sum;
                    var repRow = repDt.Rows.Add(row.ItemArray);
                    repRow[0] = ++num;
                    repRow[paymentColumnName] = DBNull.Value;
                }
            }

            // получаем данные по области в целом
            var filterList = new List<QFilter>
            {
                new QFilter(QMarksTaxBenPay.Keys.Day, filterQuarter),
                new QFilter(QMarksTaxBenPay.Keys.Lvl, ReportMOFO0024Helper.BdgtLevelSubject),
                new QFilter(QMarksTaxBenPay.Keys.Org, ReportMOFO0024Helper.OrgRefUndefined),
                new QFilter(QMarksTaxBenPay.Keys.Mark, ReportMOFO0024Helper.MarkAll)
            };

            var query = new QMarksTaxBenPay().GetQueryText(filterList);
            var tblData = dbHelper.GetTableData(query);

            // делим суммы в зависимости от выбранных единиц измерения
            var totalPayment = tblData.Rows.Count > 0
                ? DivideSumValue(tblData.Rows[0][f_Marks_TaxBenPay.SumPayment], divider)
                : (object) DBNull.Value;
            var totalReduction = tblData.Rows.Count > 0
                ? DivideSumValue(tblData.Rows[0][f_Marks_TaxBenPay.SumReduction], divider)
                : (object) DBNull.Value;

            // заполняем таблицу итогов
            var totalDt = new DataTable();
            AddColumnToReport(totalDt, typeof(decimal), SUM, 2);
            totalDt.Rows.Add(totalPayment, totalReduction); // Всего по Московской области
            totalDt.Rows.Add(paymentSum, reductionSum); // Итого по крупным плательщикам

            tablesResult[0] = repDt;
            tablesResult[1] = totalDt;

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
