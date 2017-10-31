using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using Krista.FM.Client.Common;
using Krista.FM.Common.OfficeHelpers;
using InitializeRowEventHandler=Infragistics.Win.UltraWinGrid.InitializeRowEventHandler;

namespace Krista.FM.Client.HelpGenerator
{
    public partial class CompareDescriptionForm : Form
    {
        CompareDescription compareDesc = new CompareDescription();

        public CompareDescriptionForm()
        {
            InitializeComponent();
            ultraGrid.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(ultraGrid_InitializeLayout);
            InfragisticComponentsCustomize.CustomizeUltraGridParams(ultraGrid);

            ultraGrid.DataSource = compareDesc.CompareDataSet;

            //this.ultraGrid.DisplayLayout.Bands[0].SortedColumns.Add("package", false, true);
            ultraGrid.InitializeRow += new InitializeRowEventHandler(ultraGrid_InitializeRow);
            ultraGrid.KeyPress += new KeyPressEventHandler(ultraGrid_KeyPress);
        }

        void ultraGrid_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 19 && Control.ModifierKeys==Keys.Control)
            {
                Excel();
            }
        }

        private void Excel()
        {
            string fileName = "Сравнительная таблица";
            ExportImportHelper.GetFileName(fileName, ExportImportHelper.fileExtensions.xls, ref fileName);

            Infragistics.Excel.Workbook wb = new Infragistics.Excel.Workbook();
            UltraGridExcelExporter excelExpoter = new UltraGridExcelExporter();

            Infragistics.Excel.Worksheet wsAll = wb.Worksheets.Add("Compere");

            excelExpoter.Export(ultraGrid, wsAll, 0, 0);
            //wb.Save(fileName);
            Infragistics.Excel.BIFF8Writer.WriteWorkbookToFile(wb, fileName);

            excelExpoter.Dispose();

            using (OfficeApplication officeApplication = OfficeHelper.CreateExcelApplication())
            {
                officeApplication.OpenFile(fileName, true, true);
            }
        }

        void ultraGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (string.Compare(e.Row.Cells["schemeDescription"].Value.ToString(), e.Row.Cells["fmmdallDescription"].Value.ToString())==0)
            {
                e.Row.Appearance.ForeColor = Color.Green;
            }
        }

        void ultraGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            foreach (UltraGridBand _band in ultraGrid.DisplayLayout.Bands)
            {
                UltraGridColumn clmn = _band.Columns["package"];
                clmn.Header.Caption = "Имя gпакета";

                clmn = _band.Columns["entityOLAPName"];
                clmn.Header.Caption = "имя объекта";
                clmn.Width = 400;
                clmn.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;

                clmn = _band.Columns["schemeDescription"];
                clmn.Header.Caption = "Описание разработчика";
                clmn.Width = 400;
                clmn.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;

                clmn = _band.Columns["fmmdallDescription"];
                clmn.Header.Caption = "Описание в FMMD_All";
                clmn.Width = 400;
                clmn.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                clmn.Hidden = true;
            }
        }
    }
}