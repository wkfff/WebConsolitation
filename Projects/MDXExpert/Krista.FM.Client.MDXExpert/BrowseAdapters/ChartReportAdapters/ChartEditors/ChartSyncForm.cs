using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.MDXExpert.BrowseAdapters.ChartReportAdapters.ChartEditors
{
    public partial class ChartSyncForm : Form
    {
        private bool measureInRows;
        private bool isSyncronize;

        public bool IsSyncronize
        {
            get { return this.isSyncronize; }
        }

        public bool MeasureInRows
        {
            get { return measureInRows; }
            set { measureInRows = value; }
        }

        public ChartSyncForm()
        {
            InitializeComponent();
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            this.MeasureInRows = (osMeasurePlacing.CheckedIndex == 0);
            this.isSyncronize = cbSyncEnabled.Checked;
        }
    }
}
