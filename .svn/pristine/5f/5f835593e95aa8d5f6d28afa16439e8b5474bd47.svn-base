using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalPlanningOperations
{
    public partial class frmCouponParams : Form
    {
        public frmCouponParams()
        {
            InitializeComponent();
            ne1.Value = 0;
        }

        public static bool ShowCouponParams(ref int periodsCount, ref decimal couponRate)
        {
            frmCouponParams frm = new frmCouponParams();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                periodsCount = Convert.ToInt32(frm.ne2.Value);
                return true;
            }
            return false;
        }
    }
}
