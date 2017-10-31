using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI
{
    public partial class frmSetHierarchyMode : Form
    {
        public enum DivideAndFormHierarchyMode { DivideOnly, FullHierarchy, WhereNotSetHierarchy, NoDivideNoFormHierarchy };

        public frmSetHierarchyMode()
        {
            InitializeComponent();
        }

        public static bool SelectSettingHierarchyMode(bool isHierarchyClassifier, ref DivideAndFormHierarchyMode mode, IWin32Window parent)
        {
            frmSetHierarchyMode frmHierarchy = new frmSetHierarchyMode();

            frmHierarchy.cbSetHierarchy.Enabled = isHierarchyClassifier;

            if (frmHierarchy.ShowDialog(parent) == DialogResult.OK)
            {
                mode = frmHierarchy.DivideFormHierarchyMode;
                frmHierarchy.Dispose();
                return true;
            }
            return false;
        }

        private DivideAndFormHierarchyMode _divideAndFormHierarchyMode = DivideAndFormHierarchyMode.DivideOnly;

        public DivideAndFormHierarchyMode DivideFormHierarchyMode
        {
            get { return _divideAndFormHierarchyMode; }
        }

        private void cbSetHierarchy_CheckedChanged(object sender, EventArgs e)
        {
            rbAllRecords.Enabled = cbSetHierarchy.Checked;
            radioButton2.Enabled = cbSetHierarchy.Checked;
            if (cbSetHierarchy.Checked)
            {
                cbDivideCode.Enabled = false;
                cbDivideCode.Checked = true;
                if (rbAllRecords.Checked)
                    _divideAndFormHierarchyMode = DivideAndFormHierarchyMode.FullHierarchy;
                else
                    _divideAndFormHierarchyMode = DivideAndFormHierarchyMode.WhereNotSetHierarchy;
            }
            else
            {
                _divideAndFormHierarchyMode = DivideAndFormHierarchyMode.DivideOnly;
                cbDivideCode.Enabled = true;
            }
        }

        private void rbAllRecords_CheckedChanged(object sender, EventArgs e)
        {
            if (rbAllRecords.Checked)
                _divideAndFormHierarchyMode = DivideAndFormHierarchyMode.FullHierarchy;
            else
                _divideAndFormHierarchyMode = DivideAndFormHierarchyMode.WhereNotSetHierarchy;
        }

        private void cbDivideCode_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbDivideCode.Checked)
                _divideAndFormHierarchyMode = DivideAndFormHierarchyMode.NoDivideNoFormHierarchy;

            if (cbSetHierarchy.Checked)
                cbDivideCode.Checked = true;
        }
    }
}