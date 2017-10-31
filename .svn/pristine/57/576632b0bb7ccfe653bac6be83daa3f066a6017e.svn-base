using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Krista.FM.Client.MDXExpert
{
    public partial class SyncControl : UserControl
    {
        private string boundTo;
        private bool measureInRows;
        private bool changed = false;
        private ChartSynchronization sync;

        public string BoundTo
        {
            get { return boundTo; }
            set
            {
                boundTo = value;

            }
        }

        public bool MeasureInRows
        {
            get { return measureInRows; }
            set { measureInRows = value; }
        }

        public bool Changed
        {
            get { return changed; }
        }

        public SyncControl(ChartSynchronization sync)
        {
            InitializeComponent();

            this.sync = sync;
            this.boundTo = sync.BoundTo;
            this.measureInRows = sync.MeasureInRows;

            FillTableList(sync.ChartElement);
            osMeasurePlacing.CheckedIndex = sync.MeasureInRows ? 0 : 1;

            cbSyncEnabled.Checked = !String.IsNullOrEmpty(this.boundTo);
        }

        private void FillTableList(ChartReportElement chartElem)
        {
            cbTables.Clear();
            cbTables.Items.Add(null, "");

            foreach(string tableKey in chartElem.MainForm.GetAvialableTables())
            {
                cbTables.Items.Add(tableKey, chartElem.MainForm.GetReportElementText(tableKey));
            }

            if (!String.IsNullOrEmpty(chartElem.Synchronization.BoundTo))
            {
                cbTables.SelectedItem = cbTables.Items.ValueList.FindByDataValue(chartElem.Synchronization.BoundTo);
            }
            else
            {
                if (cbTables.Items.Count > 1)
                {
                    cbTables.SelectedItem = cbTables.Items[1];
                }
            }
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if (Tag != null)
            {
                this.MeasureInRows = (osMeasurePlacing.CheckedIndex == 0);
                if (cbSyncEnabled.Checked)
                {
                    if (cbTables.SelectedItem != null)
                        this.BoundTo = (string) cbTables.SelectedItem.DataValue;
                }
                else
                {
                    this.BoundTo = "";
                }
                this.changed = true;

                ((IWindowsFormsEditorService)Tag).CloseDropDown();
            }
            else
            {
                this.Hide();
            }

        }

        private void btDataReceive_Click(object sender, EventArgs e)
        {
            this.sync.MeasureInRows = (osMeasurePlacing.CheckedIndex == 0);

            if (cbTables.SelectedItem != null)
            {
                this.sync.BoundTo = (string)cbTables.SelectedItem.DataValue;
            }
            else
            {
                this.sync.BoundTo = "";
            }
            this.sync.ChartElement.Synchronize(true);

            if (!cbSyncEnabled.Checked)
            {
                this.sync.BoundTo = "";
                this.sync.ChartElement.MainForm.FieldListEditor.InitEditor(this.sync.ChartElement);

            }
        }
    }
}
