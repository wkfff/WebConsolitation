using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    public partial class GaugeSyncControl : UserControl
    {
        private string boundTo;
        private bool isCurrentColumnValues;
        private string measure;
        private bool changed = false;
        private GaugeSynchronization sync;

        public string BoundTo
        {
            get { return boundTo; }
            set
            {
                boundTo = value;

            }
        }

        public bool IsCurrentColumnValues
        {
            get { return isCurrentColumnValues; }
            set
            {
                isCurrentColumnValues = value;

            }
        }


        public bool Changed
        {
            get { return changed; }
        }

        public GaugeSyncControl(GaugeSynchronization sync)
        {
            InitializeComponent();

            this.sync = sync;
            this.boundTo = sync.BoundTo;
            this.isCurrentColumnValues = sync.IsCurrentColumnValues;
            this.measure = sync.Measure;

            FillTableList(sync.GaugeElement);
            //osMeasurePlacing.CheckedIndex = sync.MeasureInRows ? 0 : 1;

            cbSyncEnabled.Checked = !String.IsNullOrEmpty(this.boundTo);
            ceIsCurrentColumnValues.Checked = this.isCurrentColumnValues;
        }

        private void FillTableList(GaugeReportElement gaugeElem)
        {
            cbTables.Clear();
            cbTables.Items.Add(null, "");

            foreach(string tableKey in gaugeElem.MainForm.GetAvialableTables())
            {
                cbTables.Items.Add(tableKey, gaugeElem.MainForm.GetReportElementText(tableKey));
            }

            if (!String.IsNullOrEmpty(gaugeElem.Synchronization.BoundTo))
            {
                cbTables.SelectedItem = cbTables.Items.ValueList.FindByDataValue(gaugeElem.Synchronization.BoundTo);
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
                if (cbSyncEnabled.Checked)
                {
                    if (cbTables.SelectedItem != null)
                    {
                        this.BoundTo = (string) cbTables.SelectedItem.DataValue;
                    }
                }
                else
                {
                    this.BoundTo = "";
                }

                this.IsCurrentColumnValues = ceIsCurrentColumnValues.Checked;
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

            if (cbTables.SelectedItem != null)
            {
                this.sync.BoundTo = (string)cbTables.SelectedItem.DataValue;
            }
            else
            {
                this.sync.BoundTo = "";
            }
            this.sync.GaugeElement.Synchronize();

            if (!cbSyncEnabled.Checked)
            {
                this.sync.BoundTo = "";
                this.sync.GaugeElement.MainForm.FieldListEditor.InitEditor(this.sync.GaugeElement);

            }
        }

        private void cbSyncEnabled_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ceIsCurrentColumnValues_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
