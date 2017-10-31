using System;
using System.Data;
using System.Windows.Forms;
using Infragistics.Excel;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Common.Forms
{
    public partial class FormPackageConflicts : Form
    {
        private IScheme scheme = null;

        public FormPackageConflicts()
        {
            InitializeComponent();
        }

        public FormPackageConflicts(IScheme scheme)
            : this()
        {
            this.scheme = scheme;

            ultraGrid.InitializeLayout += new InitializeLayoutEventHandler(ultraGrid_InitializeLayout);

            InfragisticComponentsCustomize.CustomizeUltraGridParams(ultraGrid);

            ultraGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
            ultraGrid.DisplayLayout.Override.AllowColSizing = AllowColSizing.Free;
            ultraGrid.DisplayLayout.Override.ColumnSizingArea = ColumnSizingArea.CellsOnly;
        }

        static void ultraGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGrid gr = sender as UltraGrid;

            if (gr != null)
            {
                foreach (UltraGridBand band in gr.DisplayLayout.Bands)
                {
                    UltraGridColumn clmn = band.Columns["number"];
                    clmn.Header.Caption = "ID конфликтной ситуации";
                    clmn.Hidden = true;

                    clmn = band.Columns["path"];
                    clmn.Header.Caption = "Зависимость между пакетами";
                    clmn.Hidden = false;

                    clmn = band.Columns["dependent"];
                    clmn.Header.Caption = "Зависимость";
                    clmn.Width = 50;

                    clmn = band.Columns["association"];
                    clmn.Header.Caption = "Ассоциация";
                }
            }
        }


        private void FormPackageConflicts_Load(object sender, EventArgs e)
        {
            Operation operation = new Operation();
            try
            {
                operation.Text = "Поиск конфликтов";
                operation.StartOperation();

                DataTable table = scheme.RootPackage.GetConflictPackageDependents();

                ultraStatusBar1.Text = String.Format("Найдено конфликтных ситуаций : {0}", DefineCountOfDependents(table.Rows));

                ultraGrid.DataSource = table;

                ultraGrid.DisplayLayout.Bands[0].SortedColumns.Add("number", false, true);
            }
            finally
            {
                operation.StopOperation();
                operation.ReleaseThread();
            }
        }

        private static int DefineCountOfDependents(DataRowCollection rows)
        {
            int i = 0;
            foreach (DataRow row in rows)
            {
                if (int.Parse(row["number"].ToString()) > i)
                    i = int.Parse(row["number"].ToString());
            }

            return i;
        }

        private void ultraToolbarsManager1_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch(e.Tool.Key)
            {
                case "Excel":
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.FileName = "Таблица конфликтов в схеме";
                    sfd.Filter = "Excel Files *.xls|*.xls";

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        Workbook wb = new Workbook();
                        UltraGridExcelExporter excelExpoter = new UltraGridExcelExporter();

                        Worksheet wsAll = wb.Worksheets.Add("Dependents");

                        excelExpoter.Export(ultraGrid, wsAll, 3, 0);
                        //wb.Save(sfd.FileName);
                        BIFF8Writer.WriteWorkbookToFile(wb, sfd.FileName);

                        excelExpoter.Dispose();
                    }
                    break;
            }
        }
    }
}