using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Common.Forms
{

    public enum ObjectTypeParams { DivSourceClassifier, NonDivSourceClassifier, FactTable, ConversionTable };

    public partial class FormImportParameters : Form
    {
        public FormImportParameters()
        {
            InitializeComponent();
        }

        private static bool _inDeveloperMode;

        public static bool ShowImportParams(IWin32Window parentWindow, string uniqueAttributesNames,
            ObjectTypeParams objectType, bool inDeveloperMode,
            ref ImportPatams importParams, ref bool exportAllRows, ref bool exportHierarchy)
        {
            FormImportParameters frm = new FormImportParameters(uniqueAttributesNames);
            frm.Size = new Size(430, 228);
            _inDeveloperMode = inDeveloperMode;
            switch (objectType)
            {
                case ObjectTypeParams.NonDivSourceClassifier:
                    frm.cbRestoreDataSource.Enabled = false;
                    break;
                case ObjectTypeParams.ConversionTable:
                    frm.cbGenerateNewID.Enabled = false;
                    frm.cblUniqueAttributesList.Enabled = false;
                    frm.cbRefreshDataByUnique.Enabled = false;
                    frm.cbRefreshDataByAttributes.Enabled = false;
                    frm.cbRestoreDataSource.Enabled = false;
                    break;
                case ObjectTypeParams.FactTable:
                    frm.cbGenerateNewID.Enabled = false;
                    break;
            }

            if (!exportHierarchy)
                frm.rbSelectedHierarchy.Enabled = false;

            // настройка вида формы в зависимости от того, в режиме разработчика находимся или нет
            if (!_inDeveloperMode)
            {
                frm.cbDeleteDevelop.Visible = false;
                frm.cbGenerateNewID.Location = new Point(frm.cbGenerateNewID.Location.X, frm.cbGenerateNewID.Location.Y - 23);
                frm.cbRestoreDataSource.Location = new Point(frm.cbRestoreDataSource.Location.X, frm.cbRestoreDataSource.Location.Y - 23);
                frm.cbRefreshDataByUnique.Location = new Point(frm.cbRefreshDataByUnique.Location.X, frm.cbRefreshDataByUnique.Location.Y - 23);
                frm.cbRefreshDataByAttributes.Location = new Point(frm.cbRefreshDataByAttributes.Location.X, frm.cbRefreshDataByAttributes.Location.Y - 23);

                frm.cblUniqueAttributesList.Location = new Point(frm.cblUniqueAttributesList.Location.X, frm.cblUniqueAttributesList.Location.Y - 23);
                frm.panelParametrs.Size = new Size(frm.panelParametrs.Size.Width, frm.panelParametrs.Size.Height - 23);

                frm.button4.Location = new Point(frm.button4.Location.X, frm.button4.Location.Y - 23);
                frm.button5.Location = new Point(frm.button5.Location.X, frm.button5.Location.Y - 23);
                frm.button6.Location = new Point(frm.button6.Location.X, frm.button6.Location.Y - 23);
            }

            if (frm.ShowDialog(parentWindow) == DialogResult.OK)
            {
                importParams.deleteDataBeforeImport = _deleteDataBeforeImport;
                importParams.deleteDeveloperData = _deleteDevelopData;
                importParams.useOldID = _useOldID;
                importParams.restoreDataSource = _restoreDataSource;
                importParams.refreshDataByUnique = _refreshDataByUnique;
                importParams.refreshDataByAttributes = _refreshDataByAttributes;
                importParams.uniqueAttributesNames = _uniqueAttributesNames;
                exportAllRows = _exportAllRows;
                exportHierarchy = _exportHierarchyRows;
                return true;
            }
            else
                return false;
        }

        public static bool ShowImportParams(IWin32Window parentWindow,
            ref bool exportAllRows, ref bool exportHierarchy)
        {
            var frm = new FormImportParameters();
            frm.Size = new Size(300, 178);
            frm.rbSelectedRecords.Checked = true;
            frm.lCaption.Visible = false;
            frm.cbGenerateNewID.Enabled = false;
            frm.cblUniqueAttributesList.Enabled = false;
            frm.cbRefreshDataByUnique.Enabled = false;
            frm.cbRefreshDataByAttributes.Enabled = false;
            frm.cbRestoreDataSource.Enabled = false;
            frm.button4.Visible = false;
            frm.button3.Visible = false;
            frm.button1.Location = new Point(frm.button1.Location.X - 130, frm.button1.Location.Y - 50);
            frm.button2.Location = new Point(frm.button2.Location.X - 130, frm.button2.Location.Y - 50);
            if (!exportHierarchy)
                frm.rbSelectedHierarchy.Enabled = false;

            if (frm.ShowDialog(parentWindow) == DialogResult.OK)
            {
                exportAllRows = _exportAllRows;
                exportHierarchy = _exportHierarchyRows;
                return true;
            }
            else
                return false;
        }

        public FormImportParameters(string uniqueAttributes)
        {
            InitializeComponent();

            foreach (string item in uniqueAttributes.Split(','))
            {
                cblUniqueAttributesList.Items.Add(item, false);
            }
        }

        private static bool _exportHierarchyRows;
        private static bool _exportAllRows;
        private static bool _deleteDataBeforeImport;
        private static bool _deleteDevelopData;
        private static bool _useOldID;
        private static bool _restoreDataSource;
        private static bool _refreshDataByUnique;
        private static bool _refreshDataByAttributes;
        private static string _uniqueAttributesNames;

        private void button2_Click(object sender, EventArgs e)
        {
            _deleteDataBeforeImport = cbDeleteDataBeforeImport.Checked;
            _deleteDevelopData = cbDeleteDevelop.Checked;
            _useOldID = cbGenerateNewID.Checked;
            _restoreDataSource = cbRestoreDataSource.Checked;
            _refreshDataByUnique = cbRefreshDataByUnique.Checked;
            _refreshDataByAttributes = cbRefreshDataByAttributes.Checked;
            _exportAllRows = rbAllRecords.Checked;
            _exportHierarchyRows = rbSelectedHierarchy.Checked;
            if (_refreshDataByAttributes)
            {
                List<string> attributes = new List<string>();
                foreach (object item in cblUniqueAttributesList.CheckedItems)
                {
                    attributes.Add(item.ToString().Split('(')[1].Replace(")", string.Empty));
                }
                _uniqueAttributesNames = String.Join(",", attributes.ToArray());
            }
        }

        private void cbRefreshDataByAttributes_CheckedChanged(object sender, EventArgs e)
        {
            if (cbRefreshDataByAttributes.Checked)
            {
                cblUniqueAttributesList.Enabled = true;
                if (cblUniqueAttributesList.CheckedItems.Count == 0)
                    button2.Enabled = false;
                else
                    button2.Enabled = true;
            }
            else
            {
                cblUniqueAttributesList.Enabled = false;
                button2.Enabled = true;
            }
        }

        private void cblUniqueAttributesList_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cblUniqueAttributesList.CheckedItems.Count == 0)
                button2.Enabled = false;
            else
                button2.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_inDeveloperMode)
                this.Size = new Size(430, 458);
            else
                this.Size = new Size(430, 434);
            button3.Visible = false;
            button1.Visible = false;
            button2.Visible = false;
            panelParametrs.Visible = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Size = new Size(430, 228);
            panelParametrs.Visible = false;
            button3.Visible = true;
            button1.Visible = true;
            button2.Visible = true;
        }

        private void cbRefreshDataByUnique_CheckedChanged(object sender, EventArgs e)
        {
            //this.cbRefreshDataByUnique.Enabled = !((CheckBox)sender).Checked;
        }

        private void cbDeleteDataBeforeImport_CheckStateChanged(object sender, EventArgs e)
        {
            this.cbDeleteDevelop.Enabled = ((CheckBox)sender).Checked;
            this.cbRefreshDataByAttributes.Enabled = !((CheckBox)sender).Checked;
        }
    }
}