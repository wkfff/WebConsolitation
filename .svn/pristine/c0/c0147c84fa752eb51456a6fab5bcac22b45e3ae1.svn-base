using System;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportFillers;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.UFK.ReportFillers
{
    class ReportUFK009Filler : CommonUFNSReportFiller
    {
        public virtual void FillUFKReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var paramColumns = Convert.ToString(ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.COLUMNS));
            var columns = ReportDataServer.ConvertToIntList(paramColumns);
            var sheet = wb.GetSheetAt(0);
            var paramSum = ParamUFKHelper.GetParamValue(tableList, ParamUFKHelper.SUMM);
            if (Convert.ToDecimal(paramSum) == 0)
            {
                DeleteParamCell(sheet, ParamUFKHelper.SUMM);
            }

            var sheetParams = new SheetParams
            {
                SheetIndex = 0,
                FirstRow = 6,
                StyleRowsCount = 6,
                FirstTotalColumn = 1,
                HeaderRowsCount = 3,
                Columns = columns,
                SumColumns = columns.Where(i => i > 0).ToList(),
            };

            FillSheet(wb, tableList, sheetParams);
            sheet.ForceFormulaRecalculation = true;
        }
    }
}
