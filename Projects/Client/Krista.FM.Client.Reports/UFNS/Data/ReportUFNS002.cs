using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{
    internal class UFNSColumn
    {
        internal string Name { get; set; }
        internal string Codes { get; set; }
        internal object Sum { get; set; }
    }

    public partial class ReportDataServer
    {
        private Dictionary<int, UFNSColumn> GetMarksColumns(string columnsCodes)
        {
            var columns = new Dictionary<int, UFNSColumn>();
            var codes = new List<int>();
            var dbHelper = new ReportDBHelper(ConvertorSchemeLink.scheme);
            var dtMarks = dbHelper.GetEntityData(fx_FX_DataMarks65n.InternalKey, "id <> 0");
            
            if (columnsCodes != String.Empty)
            {
                var strCodes = columnsCodes.Split(',');
                codes = strCodes.Select(column => Convert.ToInt32(column)).ToList();
            }
            else
            {
                codes.AddRange(from DataRow row in dtMarks.Rows
                               select Convert.ToInt32(row[fx_FX_DataMarks65n.ID]));
            }

            codes.Sort();

            foreach (var code in codes)
            {
                var rows = dtMarks.Select(String.Format("{0} = {1}", fx_FX_DataMarks65n.ID, code));
                if (rows.Length > 0)
                {
                    var key = Convert.ToInt32(rows[0][fx_FX_DataMarks65n.Code]);
                    var calcCode = GetCalculatedCode(Convert.ToString(code));
                    if (!columns.Keys.Contains(key))
                    {
                        var column = new UFNSColumn
                                         {
                                             Name = Convert.ToString(rows[0][fx_FX_DataMarks65n.Name]),
                                             Codes = calcCode,
                                             Sum = DBNull.Value
                                         };
                        columns.Add(key, column);
                    }
                    else
                    {
                        columns[key].Codes = String.Format("{0}, {1}", columns[key].Codes, calcCode);
                    }
                }
            }

            return columns;
        }

        /// <summary>
        /// ОТЧЕТ 002_ЕЖЕМЕСЯЧНЫЙ АНАЛИЗ ПОКАЗАТЕЛЕЙ 65Н 
        /// </summary>
        public DataTable[] GetUFNSReport002Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[3];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var paramOKVED = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamOKVED]);
            var paramRegion = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamRegionComparable]);
            var okved = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamOKVED]);
            var paramMarks = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMark]);
            var sumColumns = GetMarksColumns(paramMarks);
            var yearList = GetSelectedYears(reportParams[ReportConsts.ParamYear]);
            var filterPeriod = GetYearsBoundsFilter(yearList, true);
            var currentYear = DateTime.Now.Year;
            yearList = yearList.Where(year => year <= currentYear).ToList();
            
            // фильтр кодов
            var codes = (from column in sumColumns
                         where column.Value.Codes != String.Empty
                         select GetAbsCodes(column.Value.Codes)
                        ).ToArray();
            var codeFilter = GetDistinctCodes(String.Join(",", codes));

            // получаем данные из т.ф. «ФНС.28н.без расщепления» (f_F_DirtyUMNS28n)
            var filterList = new List<QFilter> 
            {
                new QFilter(QDirtyUMNS28n.Keys.Period,  filterPeriod),
                new QFilter(QDirtyUMNS28n.Keys.KD,  paramKD),
                new QFilter(QDirtyUMNS28n.Keys.Okved,  paramOKVED),
                new QFilter(QDirtyUMNS28n.Keys.Okato,  paramRegion),
                new QFilter(QDirtyUMNS28n.Keys.Mark,  codeFilter),
            };
            var groupList = new List<Enum> { QDirtyUMNS28n.Keys.Day,  QDirtyUMNS28n.Keys.Mark };
            var queryText = new QDirtyUMNS28n().GetQueryText(filterList, groupList);
            var tblData = dbHelper.GetTableData(queryText);

            // заполняем таблицу отчета
            var repTable = new DataTable();
            AddColumnToReport(repTable, typeof(string), "Period");
            var sum0Column = AddColumnToReport(repTable, typeof(decimal), "Sum", sumColumns.Count);
            AddColumnToReport(repTable, typeof(int), STYLE);

            foreach (var year in yearList)
            {
                var lastMonth = year < currentYear ? 12 : DateTime.Now.Month;

                for (var month = 1; month <= lastMonth; month++)
                {
                    foreach (var sumColumn in sumColumns)
                    {
                        var filter = String.Format("{0} and {1}",
                                                    filterHelper.EqualIntFilter(f_F_DirtyUMNS28n.RefYearDayUNV, GetUNVMonthStart(year, month)),
                                                    filterHelper.RangeFilter(f_F_DirtyUMNS28n.RefDataMarks65n, "{0}")
                                                  );
                        sumColumn.Value.Sum = GetCalculatedSumFieldValue(tblData, SUM, filter, sumColumn.Value.Codes);
                    }

                    var row = repTable.Rows.Add();
                    row["Period"] = month < 12
                                     ? String.Format("на {0}", GetMonthStart(year, month + 1))
                                     : String.Format("на {0}", GetMonthStart(year + 1, 1));
                    row[STYLE] = month % 3 > 0 ? 0 : 1;
                    var column = sum0Column;
                    foreach (var sumColumn in sumColumns)
                    {
                        row[column++] = sumColumn.Value.Sum;
                    }
                }
            }

            // убираем строки без данных
            repTable = FilterNotExistData(repTable, GetColumnsList(1, sumColumns.Count));

            // делим суммы в зависимости от выбранных единиц измерения
            DivideSum(repTable, 1, sumColumns.Count, divider);

            // заполняем таблицу колонок
            var columnsDt = new DataTable();
            AddColumnToReport(columnsDt, typeof(string), "Code");

            foreach (var column in sumColumns)
            {
                columnsDt.Rows.Add(column.Key);
            }

            tablesResult[0] = repTable;
            tablesResult[1] = columnsDt;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            var regionValue = reportHelper.GetNotNestedRegionCaptionText(paramRegion);
            var okvedValue = reportHelper.GetBridgeCaptionText(
                b_OKVED_Bridge.InternalKey,
                okved,
                b_OKVED_Bridge.Code,
                b_OKVED_Bridge.Name
                );

            paramHelper.SetParamValue(ParamUFKHelper.OKVED, FormatOkved(okvedValue));
            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(paramKD));
            paramHelper.SetParamValue(ParamUFKHelper.SETTLEMENT, regionValue);
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
