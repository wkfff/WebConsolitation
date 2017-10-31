using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors;
using Krista.FM.Client.Common;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms
{
    public partial class BudgetTransfertParamsForm : Form
    {
        private const string variantIncomeKey = "1525f07f-8a60-47af-9b80-7200e74956bc";
        private const string variantOutcomeKey = "e8cb8e78-f486-46c1-800f-284eb791d95a";

        public static bool ShowBudgetTransfertParams(IWin32Window parent, ref int incomeVariant, ref int outcomeVariant, ref int sourceID)
        {
            BudgetTransfertParamsForm form = new BudgetTransfertParamsForm();
            if (form.ShowDialog(parent) == DialogResult.OK)
            {
                incomeVariant = Convert.ToInt32(form.uteIncomeVariant.Tag);
                outcomeVariant = Convert.ToInt32(form.uteOutcomeVariant.Tag);
                return true;
            }
            return false;
        }

        public BudgetTransfertParamsForm()
        {
            InitializeComponent();
        }

        private void uteVariantOutcome_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            string variantCaption = uteOutcomeVariant.Text;
            int variant = ChooseVariant(variantOutcomeKey, ref variantCaption);
            uteOutcomeVariant.Text = variantCaption;
            uteOutcomeVariant.Tag = variant != -1 ? (object)variant : null;
            btnOK.Enabled = uteOutcomeVariant.Tag != null && uteIncomeVariant.Tag != null;
        }

        void uteVariantIncome_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            string variantCaption = uteIncomeVariant.Text;
            int variant = ChooseVariant(variantIncomeKey, ref variantCaption);
            uteIncomeVariant.Text = variantCaption;
            uteIncomeVariant.Tag = variant != -1 ? (object)variant : null;
            btnOK.Enabled = uteOutcomeVariant.Tag != null && uteIncomeVariant.Tag != null;
        }

        private int ChooseVariant(string clsKey, ref string variantCaption)
        {
            frmModalTemplate tmpClsForm = new frmModalTemplate();

            IClassifier cls = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.Classifiers[clsKey];
            BaseClsUI clsUI = new DataClsUI(cls);
            clsUI.Workplace = FinSourcePlanningNavigation.Instance.Workplace;
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
                return Convert.ToInt32(clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["ID"].Value);
            }
            return -1;
        }
    }
}