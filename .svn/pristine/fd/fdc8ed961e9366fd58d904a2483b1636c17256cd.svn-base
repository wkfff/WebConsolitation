using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Common;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Credits
{
    public partial class CreditDiagramForm : Form
    {
        private DataTable dtDiagram;

        public CreditDiagramForm()
        {
            InitializeComponent();
            InfragisticComponentsCustomize.CustomizeInfragisticsControl(ultraGrid1);
        }

        public static bool ShowDiagram(DataTable diagramData)
        {
            CreditDiagramForm diagramForm = new CreditDiagramForm();
            diagramForm.dtDiagram = diagramData.Copy();
            diagramForm.uchDiagram.DataSource = diagramForm.dtDiagram;
            diagramForm.ultraGrid1.DataSource = diagramForm.dtDiagram;
            if (diagramForm.ShowDialog() == DialogResult.OK)
                return true;
            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string fileName = "Отчет по кредитной линии";
            if (ExportImportHelper.GetFileName(fileName, ExportImportHelper.fileExtensions.xls, true, ref fileName))
            {
                Infragistics.Excel.Workbook wb = new Infragistics.Excel.Workbook();
                Infragistics.Excel.Worksheet ws = wb.Worksheets.Add("Data");
                Infragistics.Excel.WorksheetShape sheetImage = new Infragistics.Excel.WorksheetImage(uchDiagram.Image);
                sheetImage.PositioningMode = Infragistics.Excel.ShapePositioningMode.DontMoveOrSizeWithCells;
                sheetImage.TopLeftCornerCell = ws.Rows[15].Cells[1];
                //sheetImage.BottomRightCornerCell = ws.Rows[41].Cells[11];
                sheetImage.BottomRightCornerCell = ws.Rows[40].Cells[6];
                sheetImage.BottomRightCornerPosition = new PointF(73, 55);
                sheetImage.SetBoundsInTwips(ws, sheetImage.GetBoundsInTwips());
                ws.Shapes.Add(sheetImage);
                Infragistics.Excel.WorksheetCell cell = ws.GetCell("D4");
                cell.Value = "Лимит кредитной линии";
                cell = ws.GetCell("B9");
                cell.Value = "Дата заключения договора";
                cell = ws.GetCell("B11");
                cell.Value = "Дата закрытия договора";
                cell = ws.GetCell("B13");
                cell.Value = "Лимит единовременной задолженности";
                UltraGridExcelExporter gridExpoter = new UltraGridExcelExporter();
                gridExpoter.Export(ultraGrid1, ws, 43, 1);
                wb.Save(fileName);
            }
        }
    }
}