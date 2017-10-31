using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.SchemeEditor
{
    public partial class FixedRowsGridControl : UserControl
    {
        Krista.FM.ServerLibrary.IClassifier classifier;

        public FixedRowsGridControl()
        {
            InitializeComponent();
        }

        private void RefreshData()
        {
            if (classifier != null)
            {
                fixedGrid.DataSource = classifier.GetFixedRowsTable();
            }
        }

        internal Krista.FM.ServerLibrary.IClassifier Classifier
        {
            set 
            {
                classifier = value;
                if (classifier != null)
                    fixedGrid.IsReadOnly = (classifier.ParentPackage.IsLocked) ? false : true;

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
            DialogResult dr = MessageBox.Show("Сохранить изменения?", "Сохранение изменений", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                classifier.SetFixedRowsTable((DataTable)fixedGrid.DataSource);
                return true;
            }
            else if (dr == DialogResult.No || dr == DialogResult.Cancel)
            {
                return false;
            }
            return false;
        }
    }
}
