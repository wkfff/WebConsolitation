using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

using Krista.FM.Client.Common;


namespace Krista.FM.Client.SchemeEditor.Gui.ViewControls
{
    public partial class ValidationErrorListControl : UserControl
    {
        public ValidationErrorListControl()
            : this(null)
        {
        }

        public ValidationErrorListControl(DataTable errorListTable)
        {
            InitializeComponent();

            //инициализация грида
            grid.StateRowEnable = true;
            grid.ugData.DisplayLayout.GroupByBox.Hidden = false;
            grid.ugData.Text = "Список ошибок";
            grid.IsReadOnly = true;
            grid.utmMain.Tools[8].SharedProps.Visible = false;

            grid.OnGridInitializeLayout += new Krista.FM.Client.Components.GridInitializeLayout(grid_OnGridInitializeLayout);

            InfragisticComponentsCustomize.CustomizeUltraGridParams(grid.ugData);

            grid.DataSource = errorListTable;
        }

        void grid_OnGridInitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];

            UltraGridColumn clmn;

            clmn = band.Columns["Message"];
            clmn.CellActivation = Activation.AllowEdit;
            clmn.AllowGroupBy = DefaultableBoolean.False;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 1;
            clmn.Hidden = false;
            clmn.Header.Caption = "Описание";
            clmn.Width = 350;
            
            clmn = band.Columns["Object"];
            clmn.CellActivation = Activation.AllowEdit;
            clmn.AllowGroupBy = DefaultableBoolean.True;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 2;
            clmn.Hidden = false;
            clmn.Header.Caption = "Объект";
            clmn.Width = 200;

            clmn = band.Columns["Package"];
            clmn.CellActivation = Activation.AllowEdit;
            clmn.AllowGroupBy = DefaultableBoolean.True;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 3;
            clmn.Hidden = false;
            clmn.Header.Caption = "Пакет";
            clmn.Width = 250;
        }

        public DataTable DataSource
        {
            set { grid.DataSource = value; }
        }
    }
}
