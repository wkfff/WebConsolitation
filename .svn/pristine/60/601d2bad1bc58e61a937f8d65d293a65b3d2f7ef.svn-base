using System.Windows.Forms;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.Forms
{
    public partial class frmMonthCopy : Form
    {
        public frmMonthCopy()
        {
            InitializeComponent();
        }

        public static bool ShowMonthCopyForm(ref int sourceMonth, ref int destMonth)
        {
            frmMonthCopy frm = new frmMonthCopy();
            frm.cbSource.SelectedIndex = 0;
            frm.cbDest.SelectedIndex = 1;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                sourceMonth = frm.cbSource.SelectedIndex + 1;
                destMonth = frm.cbDest.SelectedIndex + 1;
                return true;
            }
            return false;
        }
    }
}
