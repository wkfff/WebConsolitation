using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.MOFO.Helpers;
using Krista.FM.Client.Reports.MOFO.Queries;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{

    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 002 СПИСОК МО, ОТ КОТОРЫХ НЕ ПОСТУПИЛИ ДАННЫЕ ПО ПОСТУПЛЕНИЯМ ПО АРЕНДНОЙ ПЛАТЕ
        /// </summary>
        public DataTable[] GetMOFO0023Report002Data(Dictionary<string, string> reportParams)
        {
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[2];
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;
            var paramMonth = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMonth]);
            var month = GetEnumItemIndex(new MonthEnum(), paramMonth) + 1;

            // получаем данные
            var filterList = new List<QFilter>
            {
                new QFilter(QFRental.Keys.Day, GetUNVMonthStart(year, month)),
            };

            var groupFields = new List<Enum> { QFRental.Keys.Okato };
            var query = new QFRental().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(query);
            var id = (from DataRow row in tblData.Rows
                      select Convert.ToInt32(row[d_Regions_Plan.RefBridge])).Distinct();

            tablesResult[0] = ReportMOFOHelper.GetAbsentAteTable(id);
            
            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.YEAR, month < 12 ? year : year + 1);
            paramHelper.SetParamValue(ParamUFKHelper.MONTH, GetMonthText2(month < 12 ? month + 1 : 1).ToUpper());

            return tablesResult;
        }
    }
}
