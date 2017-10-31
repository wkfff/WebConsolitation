using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI
{
    public partial class frmSetParentValueToChild : Form
    {

        public static bool GetSetValuesParam(ref bool allChilds, string fieldCaption)
        {
            frmSetParentValueToChild frm = new frmSetParentValueToChild();
            frm.lNoFilledFields.Text = String.Format("Только если значение поля '{0}' не заполнено", fieldCaption);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                allChilds = frm.radioButton2.Checked;
                return true;
            }
            else
                return false;
        }

        public frmSetParentValueToChild()
        {
            InitializeComponent();
        }
    }
}