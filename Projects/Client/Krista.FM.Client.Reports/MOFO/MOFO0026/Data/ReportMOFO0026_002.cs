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
        /// ОТЧЕТ 002 СПИСОК МО, ОТ КОТОРЫХ НЕ ПОСТУПИЛИ ДАННЫЕ ПО ПОКАЗАТЕЛЯМ ДЕЯТЕЛЬНОСТИ АКЦИОНЕРНЫХ ОБЩЕСТВ
        /// </summary>
        public DataTable[] GetMOFO0026Report002Data(Dictionary<string, string> reportParams)
        {
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[2];
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;

            // получаем данные по АТЕ
            var filterList = new List<QFilter>
            {
                new QFilter(QFIndexCapital.Keys.Day, GetUNVYearPlanLoBound(year))
            };

            var groupFields = new List<Enum> { QFIndexCapital.Keys.Okato };
            var query = new QFIndexCapital().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(query);
            var id = (from DataRow row in tblData.Rows
                      select Convert.ToInt32(row[d_Regions_Plan.RefBridge])).Distinct();

            tablesResult[0] = ReportMOFOHelper.GetAbsentAteTable(id);

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.NOW, DateTime.Now.ToShortDateString());
            paramHelper.SetParamValue(ParamUFKHelper.YEARS, year);
            return tablesResult;
        }
    }
}
