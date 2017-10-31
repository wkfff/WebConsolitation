using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI
{
    public partial class frmSplitDataParams : Form
    {

        public static bool ShowSplitDataParams(IWin32Window parentWindow, ref bool checkNormativeType)
        {
            frmSplitDataParams form = new frmSplitDataParams();
            if (form.ShowDialog(parentWindow) == DialogResult.OK)
            {
                checkNormativeType = form.radioButton2.Checked;
                return true;
            }
            return false;
        }

        public frmSplitDataParams()
        {
            InitializeComponent();
        }
    }
}
