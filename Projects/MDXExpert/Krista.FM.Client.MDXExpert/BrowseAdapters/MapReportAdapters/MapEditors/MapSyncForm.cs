using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.MDXExpert.BrowseAdapters.MapReportAdapters.MapEditors
{
    public partial class MapSyncForm : Form
    {
        private bool objectsInRows;
        private bool isSyncronize;

        public bool IsSyncronize
        {
            get { return this.isSyncronize; }
        }

        public bool ObjectsInRows
        {
            get { return objectsInRows; }
            set { objectsInRows = value; }
        }

        public MapSyncForm()
        {
            InitializeComponent();
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            this.ObjectsInRows = (osObjectsPlacing.CheckedIndex == 0);
            this.isSyncronize = cbSyncEnabled.Checked;
        }
    }
}
