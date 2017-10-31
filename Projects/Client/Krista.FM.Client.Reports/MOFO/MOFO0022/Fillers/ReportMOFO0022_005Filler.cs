using System;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.MOFO0022.ReportFillers
{
    class ReportMOFO0022_005Filler : CommonUFNSReportFiller
    {
        
        public void FillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var sheet = wb.GetSheetAt(0);
            var dtColumns = tableList[1];
            var columns = dtColumns.Rows.Cast<DataRow>().Select(row => Convert.ToInt32(row[0]));
            var sumColumns = from DataRow row in dtColumns.Rows
                             where Convert.ToBoolean(row[1])
                             select Convert.ToInt32(row[0]);

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 7,
                StyleRowsCount = 5,
                TotalRowsCount = 3,
                FirstTotalColumn = 3,
                Columns = columns.ToList(),
                SumColumns = sumColumns.ToList()
            };

            FillSheet(wb, tableList, sheetParams);
            sheet.ForceFormulaRecalculation = true;
        }
    }
}
