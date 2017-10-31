using System.Data;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0022.ReportFillers
{
    class ReportMOFO0022_006Filler : CommonUFNSReportFiller
    {
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 6,
                StyleRowsCount = 1,
                TotalRowsCount = 0,
            };

            FillSheet(wb, tableList, sheetParams);
        }
    }
}
