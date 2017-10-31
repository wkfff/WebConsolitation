using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.SchemeEditor
{
    public partial class HashCodeRowsGridControl : UserControl
    {
        DataTable table;

        public HashCodeRowsGridControl()
        {
            InitializeComponent();
        }

        private void RefreshData()
        {
            if (table != null)
            {
                fixedGrid.DataSource = table;
            }
        }

        internal DataTable Table
        {
            set 
            {
                table = value;
                fixedGrid.IsReadOnly = true;
                RefreshData();
            }
        }

        private bool ultraGridEx_OnRefreshData(object sender)
        {
            RefreshData();
            return default(bool);
        }

        private void ultraGridEx_OnCancelChanges(object sender)
        {
            RefreshData();
        }

        private bool ultraGridEx_OnSaveChanges(object sender)
        {
            return false;
        }
    }
}
