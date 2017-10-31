using System;
using System.Data;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0025.ReportFillers
{
    class ReportMOFO0025_002Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var year = Convert.ToInt32(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.YEAR));
            SetParamValue(sheet, "YEAR+2", year + 2);
            SetParamValue(sheet, "YEAR", year);

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 6 - 1,
                StyleRowsCount = 1,
                TotalRowsCount = 0,
            };

            FillSheet(wb, tableList, sheetParams);
        }
    }
}
