using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.References;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Server;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.IncomesEvalPlan
{
    public partial class frmVariantCopy : Form
    {
        private IWorkplace Workplace
        {
            get; set;
        }

        private int SourceVariantId
        {
            get; set;
        }

        private int DestVariantId
        {
            get; set;
        }

        private object SourceYear
        {
            get; set;
        }

        public frmVariantCopy()
        {
            InitializeComponent();
            SourceVariantId = -1;
            DestVariantId = -1;
        }

        public static bool CopyVariant(IWorkplace workplace, ref object sourceVariant, ref object destVariant, ref CalculateValueType[] copyValues)
        {
            frmVariantCopy form = new frmVariantCopy();
            form.Workplace = workplace;
            if (form.ShowDialog(workplace.WindowHandle) == DialogResult.OK)
            {
                List<CalculateValueType> copyList = new List<CalculateValueType>();
                if (form.checkBox1.Checked)
                {
                    copyList.Add( CalculateValueType.Estimate);
                }
                if (form.checkBox2.Checked)
                {
                    copyList.Add(CalculateValueType.Forecast);
                }
                if (form.checkBox3.Checked)
                {
                    copyList.Add(CalculateValueType.TaxResource);
                }
                copyValues = copyList.ToArray();
                sourceVariant = form.uteVariantSource.Tag;
                destVariant = form.uteVariantDest.Tag;

                return true;
            }

            return false;
        }

        private void uteVariantSource_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            List<string> columns = new List<string>(new string[] {"ID", "Name", "RefYear", "VariantDate"});
            List<object> values = new List<object>();
            if (ChooseRef(null, columns, ref values))
            {
                uteVariantSource.Text = values[1].ToString();
                uteVariantSource.Tag = values[0];
                uteVariantDest.Enabled = uteVariantSource.Tag != null;
                SourceVariantId = Convert.ToInt32(values[0]);
                SourceYear = values[2];
                if (SourceVariantId >= 0 && DestVariantId >= 0 && SourceVariantId != DestVariantId)
                    ultraButton2.Enabled = true;
            }
        }

        private void uteVariantDest_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            List<string> columns = new List<string>(new string[] { "ID", "Name", "RefYear", "VariantDate" });
            List<object> values = new List<object>();
            if (ChooseRef(SourceYear, columns, ref values))
            {
                uteVariantDest.Text = values[1].ToString();
                uteVariantDest.Tag = values[0];
                DestVariantId = Convert.ToInt32(values[0]);
                if (SourceVariantId >= 0 && DestVariantId >= 0 && SourceVariantId != DestVariantId)
                    ultraButton2.Enabled = true;
            }
        }

        private bool ChooseRef(object year, List<string> columns, ref List<object> values)
        {
            frmModalTemplate tmpClsForm = new frmModalTemplate();
            IEntity cls = ConsBudgetForecastNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(ObjectKeys.d_Variant_PlanIncomes);
            BaseClsUI clsUI = new IncomesVariantCls(cls);
            if (year != null)
                clsUI.AdditionalFilter = string.Format(" and RefYear = {0}", year);
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
            if (tmpClsForm.ShowDialog() == DialogResult.OK)
            {
                foreach (string columnName in columns)
                {
                    values.Add(clsUI.UltraGridExComponent.ugData.ActiveRow.Cells[columnName].Value);
                }
                return true;
            }
            return false;
        }
    }
}
