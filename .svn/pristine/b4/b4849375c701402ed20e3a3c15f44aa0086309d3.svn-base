using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.MDXExpert
{
    public partial class RelationalTable : UserControl
    {
        public delegate void DataSourceEventHandler(DataTable dt);

        private DataSourceEventHandler _dataSourceUpdate;

        public event DataSourceEventHandler AfterDataSourceUpdate
        {
            add { _dataSourceUpdate += value; }
            remove { _dataSourceUpdate -= value; }
        }

        private void DoAfterDataSourceUpdate(DataTable dt)
        {
            if (_dataSourceUpdate != null)
                _dataSourceUpdate(dt);
        }

        public RelationalTable()
        {
            InitializeComponent();
            this.grid.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(grid_AfterCellUpdate); 
        }

        public void SetDataSource(DataTable dt)
        {
            this.grid.DataSource = dt;
        }

        void grid_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            this.DoAfterDataSourceUpdate((DataTable)this.grid.DataSource);
        }
    }
}
