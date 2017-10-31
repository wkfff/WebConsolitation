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
    public partial class MapSyncControl : UserControl
    {
        private string boundTo;
        private bool objectsInRows;
        private bool changed = false;
        private MapSynchronization sync;

        public string BoundTo
        {
            get { return boundTo; }
            set
            {
                boundTo = value;

            }
        }

        public bool ObjectsInRows
        {
            get { return objectsInRows; }
            set { objectsInRows = value; }
        }

        public bool Changed
        {
            get { return changed; }
        }

        public MapSyncControl(MapSynchronization sync)
        {
            InitializeComponent();

            this.sync = sync;
            this.boundTo = sync.BoundTo;
            this.objectsInRows = sync.ObjectsInRows;

            FillTableList(sync.MapElement);
            osObjectsPlacing.CheckedIndex = sync.ObjectsInRows ? 0 : 1;

            cbSyncEnabled.Checked = !String.IsNullOrEmpty(this.boundTo);
        }

        private void FillTableList(MapReportElement mapElem)
        {
            cbTables.Clear();
            cbTables.Items.Add(null, "");

            foreach(string tableKey in mapElem.MainForm.GetAvialableTables())
            {
                cbTables.Items.Add(tableKey, mapElem.MainForm.GetReportElementText(tableKey));
            }

            if (!String.IsNullOrEmpty(mapElem.Synchronization.BoundTo))
            {
                cbTables.SelectedItem = cbTables.Items.ValueList.FindByDataValue(mapElem.Synchronization.BoundTo);
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
                this.ObjectsInRows = (osObjectsPlacing.CheckedIndex == 0);
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
            this.sync.ObjectsInRows = (osObjectsPlacing.CheckedIndex == 0);

            if (cbTables.SelectedItem != null)
            {
                this.sync.BoundTo = (string)cbTables.SelectedItem.DataValue;
            }
            else
            {
                this.sync.BoundTo = "";
            }

            this.sync.MapElement.Synchronize(true);

            if (!cbSyncEnabled.Checked)
            {
                this.sync.BoundTo = "";
                this.sync.MapElement.MainForm.FieldListEditor.InitEditor(this.sync.MapElement);
            }

        }
    }
}
