using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI
{
    public partial class frmSetReserveRows : Form
    {
        public frmSetReserveRows()
        {
            InitializeComponent();
        }

        public static bool ReserveRowsMode(ref bool reserveRows, ref bool reserveAllRows, ref bool reserveChildRows, int levelsCount)
        {
            frmSetReserveRows frm = new frmSetReserveRows();
            if (levelsCount == 1)
                frm.rbSererveChildRows.Enabled = false;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                reserveRows = frm.rbReserveRows.Checked;
                reserveAllRows = frm.rbReserveAllRows.Checked;
                reserveChildRows = frm.rbSererveChildRows.Checked;
                return true;
            }
            return false;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}