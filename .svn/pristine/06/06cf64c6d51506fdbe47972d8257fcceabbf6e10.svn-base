using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Infragistics.Excel;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.ExcelExport;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI
{
    internal class ReportBuilder
    {
        //private 


        internal ReportBuilder(DataSet dsData)
        {
            
        }

        internal bool CreateReport(UltraGrid dataGrid)
        {
            MemoryStream stream = new MemoryStream();
            dataGrid.DisplayLayout.Save(stream);
            stream.Position = 0;
            UltraGrid reportGrid = new UltraGrid();
            reportGrid.DisplayLayout.Load(stream);
            Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter excelExporter = new UltraGridExcelExporter();
            Infragistics.Excel.Workbook wb = new Workbook();
            Infragistics.Excel.Worksheet ws = wb.Worksheets.Add("test");
            excelExporter.Export(dataGrid, ws, 5, 0);

            int refKD = 0;
            Int32.TryParse(dataGrid.Rows[0].Cells["RefKD"].Value.ToString(), out refKD);
            int mergeRowsCount = 0;
            int rowIndex = 7;
            
            foreach (UltraGridRow gridRow in dataGrid.Rows)
            {
                int newRefKD;
                Int32.TryParse(gridRow.Cells["RefKD"].Value.ToString(), out newRefKD);
                if (refKD == newRefKD)
                {
                    mergeRowsCount++;
                }
                else
                {
                    ws.MergedCellsRegions.Add(rowIndex - mergeRowsCount, 0, rowIndex, 0);
                    ws.MergedCellsRegions.Add(rowIndex - mergeRowsCount, 1, rowIndex, 1);
                    mergeRowsCount = 0;
                }
                refKD = newRefKD;
                rowIndex++;
            }
            
            //ws.MergedCellsRegions.Add()
            Infragistics.Excel.BIFF8Writer.WriteWorkbookToFile(wb, @"d:\test.xls");
            return true;
        }
    }
}
