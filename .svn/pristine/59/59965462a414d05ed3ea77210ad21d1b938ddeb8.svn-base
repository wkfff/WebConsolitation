using System;
using System.Collections.Generic;
using Krista.FM.Client.Reports.Common;
using Krista.FM.Client.Reports.Database.FactTables.Planning.Capital;
using Krista.FM.Client.Reports.Database.FactTables.Planning.CreditIncome;
using Krista.FM.Client.Reports.Database.FactTables.Planning.Garant;
using Krista.FM.Client.Reports.Planning.Data;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        public static string CheckMOCreditDateData(Dictionary<string, string> reportParams)
        {
            var regNum = reportParams[ReportConsts.ParamRegNum];
            return CheckMODocumentDateData(
                new CreditDataObject(),
                reportParams,
                f_S_Creditincome.ContractDate,
                f_S_Creditincome.RegNum,
                regNum);
        }

        public static string CheckMOCapitalDateData(Dictionary<string, string> reportParams)
        {
            var regNum = reportParams[ReportConsts.ParamRegNum];
            return CheckMODocumentDateData(
                new CapitalDataObject(),
                reportParams,
                f_S_Capital.StartDate,
                f_S_Capital.OfficialNumber,
                regNum);
        }

        public static string CheckMOGarantDateData(Dictionary<string, string> reportParams)
        {
            var regNum = reportParams[ReportConsts.ParamRegNum];
            return CheckMODocumentDateData(
                new GarantDataObject(), 
                reportParams,
                f_S_Guarantissued.StartDate,
                f_S_Guarantissued.RegNum,
                regNum);
        }

        /// <summary>
        /// МО - Карточка учета долга по гарантии
        /// </summary>
        private static string CheckMODocumentDateData(
            CommonDataObject cdo,
            Dictionary<string, string> reportParams,
            string fieldDate,
            string fieldFilter,
            string filterRegNum
            )
        {
            cdo.InitObject(ConvertorSchemeLink.scheme);
            cdo.mainFilter[fieldFilter] = GetMOSelectedRegNumFilter(filterRegNum);
            cdo.useSummaryRow = false;
            cdo.AddDataColumn(fieldDate);
            var tblData = cdo.FillData();

            var rowData = GetLastRow(tblData);

            if (rowData != null)
            {
                var contractDate = rowData[fieldDate];

                if (contractDate == DBNull.Value || contractDate == null)
                {
                    return "Дата заключения договора не заполнена";
                }

                var date1 = Convert.ToDateTime(contractDate);
                var date2 = Convert.ToDateTime(reportParams[ReportConsts.ParamEndDate]);

                if (DateTime.Compare(date1, date2) > 0)
                {
                    return "Данный договор на данную отчетную дату выведен быть не может";
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// Московская область - Сведения о договоре (долговые обязательства)
        /// </summary>
        public static string CheckParamsMOContractInfoData(Dictionary<string, string> reportParams)
        {
            var regNumValue = reportParams[ReportConsts.ParamRegNum];

            var strCrdResult = CheckMODocumentDateData(
                new CreditDataObject(),
                reportParams,
                f_S_Creditincome.ContractDate,
                f_S_Creditincome.RegNum,
                regNumValue);

            if (strCrdResult.Length > 0)
            {
                return strCrdResult;
            }

            var strCapResult = CheckMODocumentDateData(
                new CapitalDataObject(),
                reportParams,
                f_S_Capital.StartDate,
                f_S_Capital.OfficialNumber,
                regNumValue);

            if (strCapResult.Length > 0)
            {
                return strCapResult;
            }

            var strGrnResult = CheckMODocumentDateData(
                new GarantDataObject(),
                reportParams,
                f_S_Guarantissued.StartDate,
                f_S_Guarantissued.RegNum,
                regNumValue);

            return strGrnResult.Length > 0 ? strGrnResult : String.Empty;
        }
    }
}
