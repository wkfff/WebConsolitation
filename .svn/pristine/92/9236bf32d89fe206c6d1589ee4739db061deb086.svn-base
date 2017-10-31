using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.MOFO.Helpers;
using Krista.FM.Client.Reports.MOFO.Queries;
using Krista.FM.Client.Reports.MOFO0022.Helpers;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{

    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 006 СПИСОК МО, ОТ КОТОРЫХ НЕ ПОСТУПИЛИ ДАННЫЕ О ЗАДОЛЖЕННОСТИ ПО АРЕНДНОЙ ПЛАТЕ НА ЗЕМЛЮ И ИМУЩЕСТВО
        /// </summary>
        public DataTable[] GetMOFO0022Report006Data(Dictionary<string, string> reportParams)
        {
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[2];
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;
            var paramQuarter = reportParams[ReportConsts.ParamQuarter];
            var quarter = Convert.ToInt32(GetEnumItemValue(new QuarterEnum(), paramQuarter)) + 1;
            var filterQuarter = GetUNVYearQuarter(year, quarter);

            // фильтры фактов
            var filterList = new List<QFilter>
            {
                new QFilter(QRent.Keys.Day, filterQuarter),
                new QFilter(QRent.Keys.Org, ReportMOFO0022Helper.FixOrgRef),
            };

            var groupFields = new List<Enum> {QRent.Keys.Okato};
            var query = new QRent().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(query);
            var id = (from DataRow row in tblData.Rows
                     select Convert.ToInt32(row[d_Regions_Plan.RefBridge])).Distinct();

            tablesResult[0] = ReportMOFOHelper.GetAbsentAteTable(id);

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.YEARS, year);
            paramHelper.SetParamValue(ParamUFKHelper.QUARTER, quarter);
            return tablesResult;
        }
    }
}
