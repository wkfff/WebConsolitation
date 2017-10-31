using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.Common;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.DisintRulesUI.Forms
{
    public partial class frmTransfertNormatives : Form
    {
        public frmTransfertNormatives()
        {
            InitializeComponent();

            KdCode = "00010102000010000110";
            KdName = "Налог на доходы физических лиц";
        }

        public static bool ShowTransfertParams(ref int fundVariant, ref int fundMarks, ref string kdCode, ref string kdName)
        {
            var form = new frmTransfertNormatives();
            if (form.ShowDialog() == DialogResult.OK)
            {
                fundVariant = form.FundVariantId;
                fundMarks = form.FundMarks;
                kdCode = form.KdCode;
                kdName = form.KdName;
                return true;
            }
            return false;
        }

        private const string d_Variant_FO9Fund = "97f35885-6dd9-4e18-a604-96274f6d8f11";
        private const string d_Marks_FO9Fund = "12aa18cd-a5a2-4112-9967-2dceecf599a3";
        private const string b_KD_Bridge = "5cd4f631-6276-4a9f-b466-980282500b50";

        private int FundVariantId
        {
            get; set;
        }

        private int FundMarks
        {
            get; set;
        }

        private string KdCode
        {
            get; set;
        }

        private string KdName
        {
            get; set;
        }

        private int ChooseKd(string clsKey, ref string kdCode, ref string kdName)
        {
            frmModalTemplate tmpClsForm = new frmModalTemplate();

            IEntity cls = DisintRulesNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(clsKey);
            BaseClsUI clsUI = new DataClsUI(cls);
            clsUI.Workplace = DisintRulesNavigation.Instance.Workplace;
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
                kdCode = string.Format("{0}", clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["CodeStr"].Value);
                kdName = clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["Name"].Value.ToString();
                return Convert.ToInt32(clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["ID"].Value);
            }
            return -1;
        }

        private int ChooseVariant(string clsKey, ref string variantCaption)
        {
            frmModalTemplate tmpClsForm = new frmModalTemplate();

            IEntity cls = DisintRulesNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(clsKey);
            BaseClsUI clsUI = new DataClsUI(cls);
            clsUI.Workplace = DisintRulesNavigation.Instance.Workplace;
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

        private void uteFondVariant_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            string variantCaption = uteFondVariant.Text;
            int variant = ChooseVariant(d_Variant_FO9Fund, ref variantCaption);
            uteFondVariant.Text = variantCaption;
            uteFondVariant.Tag = variant != -1 ? (object)variant : null;
            FundVariantId = variant;
            btnOK.Enabled = uteFondVariant.Tag != null && uteFondMarks.Tag != null && uteKD.Tag != null;
        }

        private void uteFondMarks_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            string variantCaption = uteFondMarks.Text;
            int variant = ChooseVariant(d_Marks_FO9Fund, ref variantCaption);
            uteFondMarks.Text = variantCaption;
            uteFondMarks.Tag = variant != -1 ? (object)variant : null;
            FundMarks = variant;
            btnOK.Enabled = uteFondVariant.Tag != null && uteFondMarks.Tag != null && uteKD.Tag != null;
        }

        private void uteKD_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            string kdCode = string.Empty;
            string kdName = string.Empty;
            int variant = ChooseKd(b_KD_Bridge, ref kdCode, ref kdName);
            if (variant != -1)
            {
                uteKD.Text = string.Format("{0} ({1})", kdCode, kdName);
                KdCode = kdCode;
                KdName = kdName;
            }
            uteKD.Text = variant == -1 ? string.Empty : string.Format("{0} ({1})", kdCode, kdName);
            uteKD.Tag = variant != -1 ? (object)variant : null;
            btnOK.Enabled = uteFondVariant.Tag != null && uteFondMarks.Tag != null && uteKD.Tag != null;
        }
    }
}
