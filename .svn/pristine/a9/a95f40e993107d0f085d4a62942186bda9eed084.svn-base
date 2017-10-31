using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsData.MOFO;
using Krista.FM.Client.Reports.Database.ClsFx;
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
        /// ОТЧЕТ 001 НАЧИСЛЕННЫЕ СУММЫ АРЕНДНОЙ ПЛАТЫ ЗА ЗЕМЛЮ И ИМУЩЕСТВО
        /// </summary>
        public DataTable[] GetMOFO0028Report001Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[3];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramMarks = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMark]);
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;
            var paramHideEmptyStr = Convert.ToBoolean(reportParams[ReportConsts.ParamHideEmptyStr]);

            // показатели
            var marksFilter = paramMarks != String.Empty
                                  ? filterHelper.RangeFilter(d_Marks_Sourse.ID, paramMarks)
                                  : String.Empty;
            var tblMarks = dbHelper.GetEntityData(d_Marks_Sourse.InternalKey, marksFilter);
            var marks = (from DataRow row in tblMarks.Rows
                         let id = Convert.ToInt32(row[d_Marks_Sourse.ID])
                         let code = Convert.ToString(row[d_Marks_Sourse.Code])
                         let codeStr = Convert.ToInt64(row[d_Marks_Sourse.CodeStr])
                         let name = code != "0"
                                        ? String.Format("{0} {1}", row[d_Marks_Sourse.Code], row[d_Marks_Sourse.Name])
                                        : Convert.ToString(row[d_Marks_Sourse.Name]) 
                         orderby codeStr
                         select new { id, name }).ToDictionary(e => e.id, e => e.name);
           
            // получаем данные из т.ф. МОФО_Начисление арендной платы (f.Marks.ChargeRent)
            var filterList = new List<QFilter>
            {
                new QFilter(QFMarksChargeRent.Keys.Day, GetUNVYearPlanLoBound(year)),
                new QFilter(QFMarksChargeRent.Keys.Mark, ConvertToString(marks.Keys))
            };

            var groupFields = new List<Enum> {QFMarksChargeRent.Keys.Okato, QFMarksChargeRent.Keys.Mark};
            var query = new QFMarksChargeRent().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(query);

            // устанавливаем параметры отчета
            var rep = new Report(f_Marks_ChargeRent.TableKey)
            {
                Divider = GetDividerValue(divider),
                RowFilter = paramHideEmptyStr ? Functions.IsNotNullRow : (RowViewParams.Function) null
            };

            // группировка по АТЕ
            var ateGrouping = new AteGrouping(0);
            rep.AddGrouping(d_Regions_Plan.RefBridge, ateGrouping);
            var fixedId = paramHideEmptyStr ? ateGrouping.AteMainId : ateGrouping.GetRegionsId();
            ateGrouping.SetFixedValues(fixedId);

            // настраиваем колонки отчета
            const string regionsTableKey = b_Regions_Bridge.InternalKey;
            const string terTypeTableKey = fx_FX_TerritorialPartitionType.InternalKey;
            var masks = new AteOutMasks(new TableField(regionsTableKey, b_Regions_Bridge.CodeLine));
            rep.AddCaptionColumn().SetMasks(ateGrouping, masks);
            masks = new AteOutMasks(new TableField(terTypeTableKey, fx_FX_TerritorialPartitionType.Name));
            rep.AddCaptionColumn().SetMasks(ateGrouping, masks);
            masks = new AteOutMasks(new TableField(regionsTableKey, b_Regions_Bridge.Name));
            var nameColumn = rep.AddCaptionColumn();
            nameColumn.SetMasks(ateGrouping, masks);

            foreach (var mark in marks.Keys)
            {
                var fltMark = filterHelper.EqualIntFilter(f_Marks_ChargeRent.RefMarks, mark);
                var amountColumn = rep.AddValueColumn(f_Marks_ChargeRent.Amount, fltMark);
                amountColumn.SumNestedRows = false;
                amountColumn.Divider = 1;
                rep.AddValueColumn(f_Marks_ChargeRent.RentArea, fltMark);
                rep.AddValueColumn(f_Marks_ChargeRent.BorrowArea, fltMark);
                rep.AddValueColumn(f_Marks_ChargeRent.ChargeAnnual, fltMark);
                rep.AddValueColumn(f_Marks_ChargeRent.Facility, fltMark);
                rep.AddValueColumn(f_Marks_ChargeRent.FactArrivalAnnual, fltMark);
                rep.AddValueColumn(f_Marks_ChargeRent.FactArrivalOMSY, fltMark);
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
            var paramHelper = new ParamUFKHelper(CreateReportParamsRow(tablesResult));
            paramHelper.SetParamValue(ParamUFKHelper.YEARS, year);
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
