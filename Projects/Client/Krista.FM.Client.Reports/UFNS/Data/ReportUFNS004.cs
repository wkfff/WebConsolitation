using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData.FNS;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 004_АНАЛИЗ ПОКАЗАТЕЛЕЙ В РАЗРЕЗЕ ОМСУ ПО 65-Н 
        /// </summary>
        public DataTable[] GetUFNSReport004Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var reportHelper = new ReportMonthMethods(scheme);
            var tablesResult = new DataTable[3];
            var paramRgnListType = reportParams[ReportConsts.ParamRegionListType];
            var writeSettles = ReportMonthMethods.WriteSettles(paramRgnListType);
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramKD = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamKDComparable]);
            var paramOKVED = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamOKVED]);
            var paramMark = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMark]);
            var okved = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamOKVED]);
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty
                                ? ReportMonthMethods.GetSelectedYear(paramYear)
                                : DateTime.Now.Year;
            var paramMonth = reportParams[ReportConsts.ParamMonth];
            var month = GetEnumItemIndex(new MonthEnum(), paramMonth) + 1;
            var filterPeriod = filterHelper.EqualIntFilter(f_F_DirtyUMNS28n.RefYearDayUNV, GetUNVMonthStart(year, month), true);

            // получаем коды показателей
            var entity = ConvertorSchemeLink.GetEntity(fx_FX_DataMarks65n.InternalKey);
            var fltID = paramMark != String.Empty
                            ? filterHelper.RangeFilter(fx_FX_DataMarks65n.ID, paramMark)
                            : String.Empty;
            var tblID = dbHelper.GetEntityData(entity, fltID);
            var markID = (from DataRow row in tblID.Rows
                          let id = row[fx_FX_DataMarks65n.ID]
                          let code = row[fx_FX_DataMarks65n.Code]
                          select Convert.ToString(id)).ToList();

            var fltCode = paramMark != String.Empty
                            ? filterHelper.RangeFilter(fx_FX_DataMarks65n.Code, paramMark)
                            : String.Empty;
            var tblCode = dbHelper.GetEntityData(entity, fltCode);
            var markCode = (from DataRow row in tblCode.Rows
                            let code = row[fx_FX_DataMarks65n.Code]
                            select Convert.ToString(code)).ToList();

            var codes = new Dictionary<string, string>();
            foreach (var id in markID.Where(id => !codes.Keys.Contains(id)))
            {
                codes.Add(id, GetCalculatedCode(id));
            }
            foreach (var code in markCode.Where(code => !codes.Keys.Contains(code)))
            {
                codes.Add(code, GetCalculatedCode(code));
            }

            var codeFilter = String.Join(",", codes.Values.ToArray());
            codeFilter = GetDistinctCodes(GetAbsCodes(codeFilter));
            
            // получаем данные из т.ф. «ФНС.28н.без расщепления» (f_F_DirtyUMNS28n)
            var filterList = new List<QFilter> 
            {
                new QFilter(QDirtyUMNS28n.Keys.Mark,  codeFilter),
                new QFilter(QDirtyUMNS28n.Keys.Period,  filterPeriod),
                new QFilter(QDirtyUMNS28n.Keys.KD,  paramKD),
                new QFilter(QDirtyUMNS28n.Keys.Okved,  paramOKVED)
            };
            var groupList = new List<Enum> { QDirtyUMNS28n.Keys.Okato, QDirtyUMNS28n.Keys.Mark };
            var queryText = new QDirtyUMNS28n().GetQueryText(filterList, groupList);
            var tblData = dbHelper.GetTableData(queryText);
            var columnsCount = codes.Count;

            // заполняем таблицу АТЕ суммами
            var tblATE = reportHelper.CreateRegionList(columnsCount);
            var columnClsIndex = GetColumnIndex(tblData, d_OKATO_A28N.RefRegionsBridge);
            var columnSumIndex = GetColumnIndex(tblData, SUM);
            var splitParams = new RegionSplitParams
            {
                KeyValIndex = 0,
                DocValIndex = 0,
                LvlValIndex = 0,
                SrcColumnIndex = 1,
                TblResult = tblATE,
            };

            var dt = new DataTable();
            dt.Columns.Add(d_OKATO_A28N.RefRegionsBridge, tblData.Columns[columnClsIndex].DataType);
            dt.Columns.Add(SUM, tblData.Columns[columnSumIndex].DataType);

            for (var i = 0; i < columnsCount; i++)
            {
                dt.Rows.Clear();
                var intCodes = ConvertToIntList(codes.Values.ElementAt(i));

                var positiveCodes = intCodes.Where(code => code >= 0);
                var positiveFilter = ConvertToString(positiveCodes);
                if (positiveFilter != String.Empty)
                {
                    var filter = filterHelper.RangeFilter(f_F_DirtyUMNS28n.RefDataMarks65n, positiveFilter);
                    var rows = tblData.Select(filter);

                    foreach (var row in rows)
                    {
                        dt.Rows.Add(row[columnClsIndex], row[columnSumIndex]);
                    }
                }

                var negativeCodes = intCodes.Where(code => code < 0);
                var negativeFilter = GetAbsCodes(ConvertToString(negativeCodes));
                if (negativeFilter != String.Empty)
                {
                    var filter = filterHelper.RangeFilter(f_F_DirtyUMNS28n.RefDataMarks65n, negativeFilter);
                    var rows = tblData.Select(filter);

                    foreach (var row in rows)
                    {
                        var sum = -GetDecimal(row[columnSumIndex]);
                        dt.Rows.Add(row[columnClsIndex], sum);
                    }
                }

                splitParams.RowsData = dt.Select();
                splitParams.DstColumnIndex = i;
                reportHelper.SplitRegionData(splitParams);

            }

            const int firstATESumColumn = ReportMonthMethods.RegionHeaderColumnCnt;

            // заполняем таблицу отчета
            var repTable = new DataTable();
            AddColumnToReport(repTable, typeof(int), "Num");
            AddColumnToReport(repTable, typeof(string), "Type");
            AddColumnToReport(repTable, typeof(string), "Name");
            var firstSumColumn = AddColumnToReport(repTable, typeof(decimal), "Sum", columnsCount);
            AddColumnToReport(repTable, typeof(int), STYLE);
            var regionRow = repTable.NewRow();

            foreach (DataRow row in tblATE.Rows)
            {
                var lvl = Convert.ToInt32(row[ReportMonthMethods.RegionLvlIndex]);
                var flg = Convert.ToInt32(row[ReportMonthMethods.RegionFlgIndex]);
                var ter = Convert.ToString(row[ReportMonthMethods.RegionTerrIndex]);
                var typ = Convert.ToInt32(row[ReportMonthMethods.RegionTypIndex]);
                if (!writeSettles && (lvl >= 4 || lvl == 3 && flg == ReportMonthMethods.RowTypeRegionSummary))
                {
                    continue;
                }

                var style = 1;
                switch (lvl)
                {
                    case 2:
                        style = 0;
                        break;
                    case 3:
                        if (flg != ReportMonthMethods.RowTypeRegionSummary && ter != "ГО" && writeSettles)
                        {
                            style = 0;
                        }
                        break;
                    case 4:
                        style = 2;
                        break;
                }

                var repRow = typ == 3 ? regionRow : repTable.Rows.Add();
                repRow["Num"] = row[ReportMonthMethods.RegionCodeIndex];
                repRow["Type"] = ter;
                repRow["Name"] = row[ReportMonthMethods.RegionNameIndex];
                repRow[STYLE] = style;

                for (var i = 0; i < columnsCount; i++)
                {
                    repRow[firstSumColumn + i] = row[firstATESumColumn + i];
                }
            }

            // заполняем итоговые строки
            var lastRow = repTable.Rows[repTable.Rows.Count - 1]; // итого
            repTable.Rows.Add(regionRow); // регион
            var totalRow = repTable.Rows.Add(); // всего

            for (var i = 0; i < columnsCount; i++)
            {
                var sum = GetDecimal(lastRow[firstSumColumn + i]);
                sum += GetDecimal(regionRow[firstSumColumn + i]);
                totalRow[firstSumColumn + i] = sum;
            }

            // делим суммы в зависимости от выбранных единиц измерения
            DivideSum(repTable, 3, columnsCount, divider);

            // заполняем таблицу колонок
            var dtColumns = new DataTable();
            dtColumns.Columns.Add("code");

            foreach (var code in codes)
            {
                dtColumns.Rows.Add(code.Key);
            }

            tablesResult[0] = repTable;
            tablesResult[1] = dtColumns;
            
            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);

            var okvedValue = reportHelper.GetBridgeCaptionText(
                b_OKVED_Bridge.InternalKey,
                okved,
                b_OKVED_Bridge.Code,
                b_OKVED_Bridge.Name);

            month++;
            if (month == 13)
            {
                month = 1;
                year = year + 1;
            }
            paramHelper.SetParamValue(ParamUFKHelper.MONTH, GetMonthText2(month));
            paramHelper.SetParamValue(ParamUFKHelper.YEARS, year);
            paramHelper.SetParamValue(ParamUFKHelper.OKVED, FormatOkved(okvedValue));
            paramHelper.SetParamValue(ParamUFKHelper.KD, reportHelper.GetNotNestedKDCaptionText(paramKD));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));

            return tablesResult;
        }
    }
}
