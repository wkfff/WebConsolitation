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
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms
{
    public partial class VariantSelectForm : Form
    {
        private const string variantIncomeKey = "1525f07f-8a60-47af-9b80-7200e74956bc";
        private const string variantOutcomeKey = "e8cb8e78-f486-46c1-800f-284eb791d95a";
        private const string variantIFKey = "94a75cd5-351c-41dc-8962-a9ffc80c4419";

        public static bool ShowVariantForm(IWin32Window parent, string variantIfCaption, ref int variantIncome, ref int variantOutcome, ref int variantIF)
        {
            BorrowingVolumeBudgetType tmpBool = BorrowingVolumeBudgetType.BudgetList;
            return ShowVariantForm(parent, false, variantIfCaption, ref variantIncome, ref variantOutcome, ref variantIF, ref tmpBool);
        }

        public static bool ShowVariantForm(IWin32Window parent, bool showBudParams, string variantIfCaption,
            ref int variantIncome, ref int variantOutcome, ref int variantIF, ref BorrowingVolumeBudgetType budgetDataType)
        {
            VariantSelectForm form = new VariantSelectForm();
            form.cbBudgetDataType.SelectedIndex = 0;
            if (!(FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_D_IncomesData) == null ||
                FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_R_BudgetData) == null)
                && showBudParams)
                form.groupBox1.Visible = true;
            else
                form.groupBox1.Visible = false;

            if (!string.IsNullOrEmpty(variantIfCaption))
            {
                form.uteIFVariant.Text = variantIfCaption;
                form.uteIFVariant.Tag = variantIF == -1 ? null : (object)variantIF;
            }
            if (form.ShowDialog(parent) == DialogResult.OK)
            {
                variantIncome = Convert.ToInt32(form.uteIncomeVariant.Tag);
                variantOutcome = Convert.ToInt32(form.uteOutcomeVariant.Tag);
                variantIF = Convert.ToInt32(form.uteIFVariant.Tag);
                budgetDataType = (BorrowingVolumeBudgetType)form.cbBudgetDataType.SelectedIndex;
                return true;
            }
            return false;
        }

        public VariantSelectForm()
        {
            InitializeComponent();
        }

        private void uteVariantOutcome_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            string variantCaption = uteIFVariant.Text;
            int variant = ChooseVariant(variantOutcomeKey, ref variantCaption);
            uteOutcomeVariant.Text = variantCaption;
            uteOutcomeVariant.Tag = variant != -1 ? (object)variant : null;
            btnOK.Enabled = uteOutcomeVariant.Tag != null && uteIFVariant.Tag != null && uteIncomeVariant.Tag != null;
        }

        void uteVariantIncome_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            string variantCaption = uteOutcomeVariant.Text;
            int variant = ChooseVariant(variantIncomeKey, ref variantCaption);
            uteIncomeVariant.Text = variantCaption;
            uteIncomeVariant.Tag = variant != -1 ? (object)variant : null;
            btnOK.Enabled = uteOutcomeVariant.Tag != null && uteIFVariant.Tag != null && uteIncomeVariant.Tag != null;
        }

        void uteVariantIF_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            string variantCaption = uteIncomeVariant.Text;
            int variant = ChooseVariant(variantIFKey, ref variantCaption);
            uteIFVariant.Text = variantCaption;
            uteIFVariant.Tag = variant != -1 ? (object)variant : null;
            btnOK.Enabled = uteOutcomeVariant.Tag != null && uteIFVariant.Tag != null && uteIncomeVariant.Tag != null;
        }

        private static int ChooseVariant(string clsKey, ref string variantCaption)
        {
            frmModalTemplate tmpClsForm = new frmModalTemplate();

            IClassifier cls = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.Classifiers[clsKey];
            BaseClsUI clsUI = new DataClsUI(cls);
            clsUI.AdditionalFilter = " and ID > 0";
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
            if (tmpClsForm.ShowDialog(FinSourcePlanningNavigation.Instance.Workplace.WindowHandle) == DialogResult.OK)
            {
                if (clsUI.UltraGridExComponent.ugData.ActiveRow == null)
                {
                    variantCaption = string.Empty;
                    return -1;
                }
                variantCaption = string.Format("({0}) {1}", clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["ID"].Value,
                    clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["NAME"].Value);
                return Convert.ToInt32(clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["ID"].Value);
            }
            variantCaption = string.Empty;
            return -1;
        }
    }
}