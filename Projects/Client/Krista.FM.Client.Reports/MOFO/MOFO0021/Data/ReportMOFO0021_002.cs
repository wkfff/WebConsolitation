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
        /// ОТЧЕТ 002 СПИСОК МО, ОТ КОТОРЫХ НЕ ПОСТУПИЛИ ДАННЫЕ О СУММАХ ПРОГНОЗА ОМСУ ПО ДОХОДНЫМ ИСТОЧНИКАМ
        /// </summary>
        public DataTable[] GetMOFO0021Report002Data(Dictionary<string, string> reportParams)
        {
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[2];
            var paramVariant = reportParams[ReportConsts.ParamVariantID];
            var nameVariant = Convert.ToString(ReportMonthMethods.GetSelectedVariantMOFOMarks(paramVariant));

            // получаем данные по АТЕ
            var filterList = new List<QFilter>
            {
                new QFilter(QFMarksForecast.Keys.Variant, paramVariant)
            };

            var groupFields = new List<Enum> {QFMarksForecast.Keys.Okato};
            var query = new QFMarksForecast().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(query);

            var id = (from DataRow row in tblData.Rows
                      select Convert.ToInt32(row[d_Regions_Plan.RefBridge])).Distinct();

            tablesResult[0] = ReportMOFOHelper.GetAbsentAteTable(id);

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.NOW, DateTime.Now.ToShortDateString());
            paramHelper.SetParamValue(ParamUFKHelper.VARIANT, nameVariant);
            return tablesResult;
        }
    }
}
