using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        /// 012 СВОДНЫЙ ОТЧЕТ ПО ФОРМЕ 5-НДФЛ
        /// </summary>
        public DataTable[] GetUFNSReport012Data(Dictionary<string, string> reportParams)
        {
            const int stylesCount = 3;
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[2];
            var paramMark = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMark]);
            var paramHalfYear = reportParams[ReportConsts.ParamHalfYear];
            var halfYear = paramHalfYear != String.Empty ? ConvertToIntList(paramHalfYear) : GetColumnsList(0, 2);
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var years = GetSelectedYears(reportParams[ReportConsts.ParamYear], false);
            var marksTable = new MarksNDFLTable(QFNS5NDFLTotal.Keys.Mark, QFNS5NDFLTotal.Keys.MarkBridge);

            // периоды
            var periods = (from year in years from i in halfYear select GetUNVMonthStart(year, (i + 1) * 6)).ToList();
            var filterPeriods = periods.Count > 0 ? ConvertToString(periods) : ReportConsts.UndefinedKey;

            // получаем данные из т.ф. «ФНС_5 НДФЛ_Районы» (f.D.FNS5NDFLRegions)
            var filterList = new List<QFilter>
            {
                new QFilter(QFNS5NDFLTotal.Keys.Day, filterPeriods),
                new QFilter(marksTable.FilterKey,  paramMark)
            };
            var groupList = new List<Enum>
            {
                marksTable.FilterKey,
                QFNS5NDFLTotal.Keys.Day
            };
            var queryText = new QFNS5NDFLTotal().GetQueryText(filterList, groupList);
            var tblData = dbHelper.GetTableData(queryText);

            // параметры отчета
            var rep = new Report(f_D_FNS5NDFLTotal.InternalKey)
            {
                AddTotalRow = false,
                RowFilter = null
            };

            // группировка по показателям
            var markGrouping = rep.AddGrouping(f_D_FNS5NDFLTotal.RefMarks);
            markGrouping.HierarchyTable = dbHelper.GetEntityData(marksTable.TableKey);
            markGrouping.HideHierarchyLevels = true;
            markGrouping.AddLookupField(marksTable.TableKey, marksTable.Id).Type = typeof(int);
            markGrouping.AddLookupField(marksTable.TableKey, marksTable.Name);
            markGrouping.AddLookupField(marksTable.TableKey, marksTable.Code).Type = typeof(int);
            markGrouping.ViewParams.Add(1, new RowViewParams { BreakSumming = true, Style = 1 });
            markGrouping.ViewParams.Add(2, new RowViewParams { BreakSumming = true, Style = 2, ForAllLevels = true });
            markGrouping.AddSortField(marksTable.TableKey, marksTable.Code);
            markGrouping.AddSortField(marksTable.TableKey, marksTable.Id);
            markGrouping.SetFixedValues(paramMark);

            // настраиваем колонки отчета
            var codeColumn = rep.AddCaptionColumn();
            codeColumn.SetMask(markGrouping, 0, String.Empty);
            codeColumn.SetMask(markGrouping, 1, marksTable.TableKey, marksTable.Code).ForAllLevels = true;
            var nameColumn = rep.AddCaptionColumn();
            nameColumn.SetMask(markGrouping, 0, marksTable.TableKey, marksTable.Name).ForAllLevels = true;
            var sumColumnsIndexies = new List<int>();

            foreach (var period in periods)
            {
                var fltPeriod = filterHelper.EqualIntFilter(f_D_FNS5NDFLTotal.RefYearDayUNV, period);
                sumColumnsIndexies.Add(rep.AddValueColumn(f_D_FNS5NDFLTotal.ValueReport, fltPeriod).Index);
                rep.AddValueColumn(f_D_FNS5NDFLTotal.TaxpayersNumberReport, fltPeriod);
            }

            // формируем таблицу отчета
            rep.ProcessTable(tblData);
            var dt = rep.GetReportData();
            var dividerValue = GetDividerValue(divider);
            foreach (DataRow row in dt.Rows)
            {
                row[codeColumn.Index] = ReportUFNSHelper.GetFNS5NDFLMarkShortCode(row[codeColumn.Index]); // обрезаем код

                if (!ReportUFNSHelper.IsRubFNS5NDFLMark(row[nameColumn.Index]))
                {
                    row[STYLE] = Convert.ToInt32(row[STYLE]) + stylesCount; // меняем стиль у нерублевых величин
                }
                else if (dividerValue != 1)
                {
                    foreach (var i in sumColumnsIndexies.Where(i => row[i] != DBNull.Value))
                    {
                        row[i] = GetDecimal(row[i]) / dividerValue; // делим рублевые величины
                    }
                }
            }

            tablesResult[0] = dt;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.YEARS, ConvertToString(years));
            paramHelper.SetParamValue(ParamUFKHelper.HALF_YEAR, ConvertToString(halfYear));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
