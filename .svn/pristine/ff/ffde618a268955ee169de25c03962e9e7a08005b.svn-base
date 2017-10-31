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
using Krista.FM.Common;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms
{
    public partial class BorrowingVolumeForm : Form
    {
        private const string variantIncomeKey = "1525f07f-8a60-47af-9b80-7200e74956bc";
        private const string variantOutcomeKey = "e8cb8e78-f486-46c1-800f-284eb791d95a";
        private const string variantIFKey = "94a75cd5-351c-41dc-8962-a9ffc80c4419";

        public static bool ShowVariantForm(IWin32Window parent, string variantIfCaption, ref int variantIncome, ref int variantOutcome, ref int variantIF)
        {
            BorrowingVolumeBudgetType tmpBool = BorrowingVolumeBudgetType.BudgetList;
            decimal euroRate = 0;
            decimal dollarRate = 0;
            return ShowVariantForm(parent, false, variantIfCaption, ref variantIncome, ref variantOutcome, ref variantIF, ref tmpBool, ref euroRate, ref dollarRate);
        }

        public static bool ShowVariantForm(IWin32Window parent, bool showBudParams, string variantIfCaption,
            ref int variantIncome, ref int variantOutcome, ref int variantIF, ref BorrowingVolumeBudgetType budgetDataType,
            ref decimal euroRate, ref decimal dollarRate)
        {
            var form = new BorrowingVolumeForm();
            form.cbBudgetDataType.SelectedIndex = 0;
            form.EuroRate.Value = 0;
            form.DollarRate.Value = 0;
            form.DollarRate.Enabled = form.CurrencyContractsExist(840, -1);
            form.EuroRate.Enabled = form.CurrencyContractsExist(978, -1);
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
                euroRate = Convert.ToDecimal(form.EuroRate.Value);
                dollarRate = Convert.ToDecimal(form.DollarRate.Value);
                return true;
            }
            return false;
        }

        public BorrowingVolumeForm()
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

            if (variant != -1)
            {
                DollarRate.Enabled = CurrencyContractsExist(840, variant);
                EuroRate.Enabled = CurrencyContractsExist(978, variant);
            }
            else
            {
                DollarRate.Enabled = false;
                EuroRate.Enabled = false;
            }
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

        private bool CurrencyContractsExist(int currencyCode, int variant)
        {
            using (IDatabase db = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                object queryResult =
                    db.ExecQuery(
                        "select count(id) from f_S_Creditincome where( RefVariant = ? or RefVariant = 0) and RefOKV in (select id from d_OKV_Currency where Code = ?)",
                        QueryResultTypes.Scalar,
                        new DbParameterDescriptor("p0", variant),
                        new DbParameterDescriptor("p1", currencyCode));
                if (queryResult != null && queryResult != DBNull.Value)
                {
                    if (Convert.ToInt32(queryResult) > 0)
                        return true;
                }

                queryResult =
                    db.ExecQuery(
                        "select count(id) from f_S_Guarantissued where ( RefVariant = ? or RefVariant = 0) and RefOKV in (select id from d_OKV_Currency where Code = ?)",
                        QueryResultTypes.Scalar,
                        new DbParameterDescriptor("p0", variant),
                        new DbParameterDescriptor("p1", currencyCode));
                if (queryResult != null && queryResult != DBNull.Value)
                {
                    if (Convert.ToInt32(queryResult) > 0)
                        return true;
                }

                queryResult =
                    db.ExecQuery(
                        "select count(id) from f_S_Creditissued where ( RefVariant = ? or RefVariant = 0) and RefOKV in (select id from d_OKV_Currency where Code = ?)",
                        QueryResultTypes.Scalar,
                        new DbParameterDescriptor("p0", variant),
                        new DbParameterDescriptor("p1", currencyCode));
                if (queryResult != null && queryResult != DBNull.Value)
                {
                    if (Convert.ToInt32(queryResult) > 0)
                        return true;
                }

                queryResult =
                    db.ExecQuery(
                        "select count(id) from f_S_Capital where ( RefVariant = ? or RefVariant = 0) and RefOKV in (select id from d_OKV_Currency where Code = ?)",
                        QueryResultTypes.Scalar,
                        new DbParameterDescriptor("p0", variant),
                        new DbParameterDescriptor("p1", currencyCode));
                if (queryResult != null && queryResult != DBNull.Value)
                {
                    if (Convert.ToInt32(queryResult) > 0)
                        return true;
                }
                return false;
            }
        }
    }
}