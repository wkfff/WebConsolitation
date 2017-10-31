using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI
{
    public partial class frmDeleteRows : Form
    {
        public frmDeleteRows()
        {
            InitializeComponent();
        }

        public static bool DeleteRowsMode(ref bool DeleteChildRows, IWin32Window parent)
        {
            frmDeleteRows deleteFrm = new frmDeleteRows();
            if (deleteFrm.ShowDialog(parent) == DialogResult.OK)
            {
                DeleteChildRows = deleteFrm.radioButton1.Checked;
                return true;
            }
            return false;
        }

        public int DeleteActionType;

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                DeleteActionType = 0;
            if (radioButton2.Checked)
                DeleteActionType = 1;
        }


    }
}