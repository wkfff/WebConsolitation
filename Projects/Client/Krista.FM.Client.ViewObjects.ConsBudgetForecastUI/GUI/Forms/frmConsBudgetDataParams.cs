using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Krista.FM.Client.Common;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;

using System.Windows.Forms;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI
{
    public partial class frmConsBudgetDataParams : Form
    {
        public static bool ShowDataParams(IWin32Window parent, ref int year)
        {
            frmConsBudgetDataParams frm = new frmConsBudgetDataParams();
            frm.Year = year;
            if (frm.ShowDialog(parent) == DialogResult.OK)
            {
                year = frm.Year;
                return true;
            }

            return false;
        }

        public frmConsBudgetDataParams()
        {
            InitializeComponent();
            cbYears.Items.Add(Year);
            cbYears.Items.Add(Year + 1);
            cbYears.Items.Add(Year + 2);
            cbYears.SelectedIndex = 0;
        }

        private int ChooseVariant(string clsKey, ref string variantCaption, ref int variantType)
        {
            frmModalTemplate tmpClsForm = new frmModalTemplate();

            IClassifier cls = ConsBudgetForecastNavigation.Instance.Workplace.ActiveScheme.Classifiers[clsKey];
            BaseClsUI clsUI = new DataClsUI(cls);
            clsUI.AdditionalFilter = string.Format(" and RefYear = {0}", Year);
            clsUI.Workplace = ConsBudgetForecastNavigation.Instance.Workplace;
            clsUI.Initialize();
            clsUI.InitModalCls(-1);
            clsUI.RefreshAttachedData();
            tmpClsForm.SuspendLayout();
            try
            {
                tmpClsForm.AttachCls(clsUI);
                ComponentCustomizer.CustomizeInfragisticsControls(tmpClsForm);
            }
            finally
            {
                tmpClsForm.ResumeLayout();
            }
            if (tmpClsForm.ShowDialog(this) == DialogResult.OK)
            {
                variantCaption = string.Format("{0}", clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["NAME"].Value);
                variantType = Convert.ToInt32(clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["RefVarD"].Value);
                return Convert.ToInt32(clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["ID"].Value);
            }
            return -1;
        }

        private int year = -1;
        internal  int Year
        {
            get { return year; }
            set { year = value; }
        }

        private void cbYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            Year = Convert.ToInt32(cbYears.SelectedItem);
        }
    }
}
