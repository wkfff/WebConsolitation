using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsData.MOFO;
using Krista.FM.Client.Reports.MOFO.Helpers;
using Krista.FM.Client.Reports.MOFO.Queries;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{

    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 002 СПИСОК МО, ОТ КОТОРЫХ НЕ ПОСТУПИЛИ ДАННЫЕ О НАЧИСЛЕННЫХ СУММАХ НАЛОГА НА ИМУЩЕСТВО ФИЗИЧЕСКИХ ЛИЦ
        /// </summary>
        public DataTable[] GetMOFO0029Report002Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[2];
            var paramVariant = reportParams[ReportConsts.ParamVariantID];
            var dtVariant = dbHelper.GetEntityData(d_Variant_PropertyTax.InternalKey,
                       filterHelper.RangeFilter(d_Variant_PropertyTax.ID, paramVariant));
            var year = dtVariant.Rows.Count > 0
                ? Convert.ToString(Convert.ToInt32(dtVariant.Rows[0][d_Variant_PropertyTax.Year]) - 1)
                : String.Empty;
            var filterVariant = dtVariant.Rows.Count > 0
                ? Convert.ToString(dtVariant.Rows[0][d_Variant_PropertyTax.ID])
                : ReportConsts.UndefinedKey;

            // получаем данные по АТЕ
            var filterList = new List<QFilter>
            {
                new QFilter(QFPropertyTax.Keys.Variant, filterVariant)
            };

            var groupFields = new List<Enum> { QFPropertyTax.Keys.Okato };
            var query = new QFPropertyTax().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(query);
            var id = (from DataRow row in tblData.Rows
                      select Convert.ToInt32(row[d_Regions_Plan.RefBridge])).Distinct();

            tablesResult[0] = ReportMOFOHelper.GetAbsentAteTable(id);

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.NOW, DateTime.Now.ToShortDateString());
            paramHelper.SetParamValue(ParamUFKHelper.YEAR, year);
            return tablesResult;
        }
    }
}
