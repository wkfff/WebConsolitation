using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsData.MOFO;
using Krista.FM.Client.Reports.Database.FactTables.MOFO;
using Krista.FM.Client.Reports.MOFO.Queries;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK.ReportMaster;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 001 ПОСТУПЛЕНИЯ ДОХОДОВ ОТ ПРОДАЖИ ПРАВА НА ЗАКЛЮЧЕНИЕ ДОГОВОРОВ АРЕНДЫ ЗА ЗЕМЕЛЬНЫЕ УЧАСТКИ И ПОСТУПЛЕНИЙ ОТ РЕАЛИЗАЦИИ ИНВЕСТИЦИОННЫХ КОНТРАКТОВ
        /// </summary>
        public DataTable[] GetMOFO0023Report001Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[3];
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;
            var paramMonth = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMonth]);
            var month = GetEnumItemIndex(new MonthEnum(), paramMonth) + 1;
            var paramMarks = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMark]);
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramRegion = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamRegionComparable]);
            var paramRgnListType = reportParams[ReportConsts.ParamRegionListType];
            var paramHideEmptyStr = Convert.ToBoolean(reportParams[ReportConsts.ParamHideEmptyStr]);
            var hideSettlements = String.Compare(paramRgnListType, RegionListTypeEnum.i1.ToString(), true) != 0;

            // уровни
            var levels = new List<string>
            {
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Municipal),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.SettleFractional)
            };

            var fltLevels = levels.Select(l => filterHelper.RangeFilter(f_F_Rental.RefBudLevel, l)).ToList();

            // показатели
            var marksFilter = paramMarks != String.Empty
                                  ? filterHelper.RangeFilter(d_Marks_Receipt.ID, paramMarks)
                                  : String.Empty;
            var tblMarks = dbHelper.GetEntityData(d_Marks_Receipt.InternalKey, marksFilter);
            var marks = (from DataRow row in tblMarks.Rows
                         let id = Convert.ToInt32(row[d_Marks_Receipt.ID])
                         let codeInd = Convert.ToInt32(row[d_Marks_Receipt.CodeInd])
                         let name = Convert.ToString(row[d_Marks_Receipt.Name])
                         orderby codeInd
                         select new { id, name }).ToDictionary(e => e.id, e => e.name);

            // получаем данные
            var filterList = new List<QFilter>
            {
                new QFilter(QFRental.Keys.Day, GetUNVMonthStart(year, month)),
                new QFilter(QFRental.Keys.Mark, ConvertToString(marks.Keys)),
                new QFilter(QFRental.Keys.Lvl, ConvertToString(levels)),
                new QFilter(QFRental.Keys.Okato, paramRegion)
            };

            var groupFields = new List<Enum>
            {
                QFRental.Keys.Okato,
                QFRental.Keys.Mark,
                QFRental.Keys.Lvl
            };
            var query = new QFRental().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(query);
           
            // параметры отчета
            var rep = new Report(f_F_Rental.TableKey)
            {
                Divider = GetDividerValue(divider),
                RowFilter = paramHideEmptyStr ? Functions.IsNotNullRow : (RowViewParams.Function) null
            };

            // группировка по АТЕ
            var ateGrouping = new AteGrouping(0, hideSettlements);
            rep.AddGrouping(d_Regions_Plan.RefBridge, ateGrouping);
            var fixedId = paramHideEmptyStr
                              ? ateGrouping.AteMainId
                              : paramRegion != String.Empty
                                    ? Combine(paramRegion, ateGrouping.AteMainId)
                                    : ateGrouping.GetRegionsId();
            ateGrouping.SetFixedValues(fixedId);

            // сортируем и настраиваем колонки отчета
            var captionColumn = rep.AddCaptionColumn();
            captionColumn.SetMasks(ateGrouping, new AteOutMasks());

            foreach (var mark in marks.Keys)
            {
                var fltMark = filterHelper.EqualIntFilter(f_F_Rental.RefReceipt, mark);

                foreach (var fltLevel in fltLevels)
                {
                    rep.AddValueColumn(SUM, CombineAnd(fltMark, fltLevel));
                }
            }

            rep.ProcessTable(tblData);
            var repDt = rep.GetReportData();

            // заполняем таблицу колонок
            var columnsDt = new DataTable();
            AddColumnToReport(columnsDt, typeof(string), NAME);

            foreach (var mark in marks.Values)
            {
                columnsDt.Rows.Add(mark);
            }

            tablesResult[0] = repDt;
            tablesResult[1] = columnsDt;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.YEARS, month < 12 ? year : year + 1);
            paramHelper.SetParamValue(ParamUFKHelper.MONTH, GetMonthText2(month < 12 ? month + 1 : 1));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
