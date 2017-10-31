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
    public partial class MultiGaugeSyncControl : UserControl
    {
        private string boundTo;
        private bool changed = false;
        private MultipleGaugeSynchronization sync;

        public string BoundTo
        {
            get { return boundTo; }
            set
            {
                boundTo = value;

            }
        }

        public bool Changed
        {
            get { return changed; }
        }

        public MultiGaugeSyncControl(MultipleGaugeSynchronization sync)
        {
            InitializeComponent();

            this.sync = sync;
            this.boundTo = sync.BoundTo;

            FillTableList(sync.GaugeElement);

            cbSyncEnabled.Checked = !String.IsNullOrEmpty(this.boundTo);
        }

        private void FillTableList(MultipleGaugeReportElement gaugeElem)
        {
            cbTables.Clear();
            cbTables.Items.Add(null, "");

            foreach (string tableKey in gaugeElem.MainForm.GetAvialableTables())
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
            if (cbTables.SelectedItem != null)
            {
                this.sync.BoundTo = (string)cbTables.SelectedItem.DataValue;
            }
            else
            {
                this.sync.BoundTo = "";
            }
            this.sync.GaugeElement.Synchronize(true);

            if (!cbSyncEnabled.Checked)
            {
                this.sync.BoundTo = "";
                this.sync.GaugeElement.MainForm.FieldListEditor.InitEditor(this.sync.GaugeElement);

            }
        }
    }
}
