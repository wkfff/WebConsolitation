using System.Data;
using System.Windows.Forms;
using Krista.FM.Client.Common;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Credits
{
    public partial class TranshDataForm : frmModalTemplate
    {
        public TranshDataForm()
        {
            InitializeComponent(); 
            InfragisticComponentsCustomize.CustomizeInfragisticsControl(ultraGrid1);
        }

        public static bool ShowTranshData(IWin32Window parent, DataTable dtTransh, ref object selectedID)
        {
            TranshDataForm form = new TranshDataForm();
            form.ultraGrid1.DataSource = dtTransh;
            if (form.ShowDialog(parent) == DialogResult.OK)
            {
                if (form.ultraGrid1.ActiveRow != null)
                {
                    selectedID = form.ultraGrid1.ActiveRow.Cells[0].Value;
                }
                return true;
            }
            return false;
        }
    }
}