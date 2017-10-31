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
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.References;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.IncomesYearPlan
{
    public partial class TransfertDataForm : Form
    {
        public long SelectedVariantId
        {
            get; set;
        }

        public int VariantYear
        {
            get; set;
        }

        public TransfertDataForm()
        {
            InitializeComponent();
        }

        public static bool ShowTransfertData(int year, ref long variantId)
        {
            var frm = new TransfertDataForm();
            frm.VariantYear = year;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                variantId = frm.SelectedVariantId;
                return true;
            }
            return false;
        }

        private bool ChooseVariant(int soirceId, string codeName, List<string> columns, ref List<object> values)
        {
            var tmpClsForm = new frmModalTemplate();
            IEntity cls = ConsBudgetForecastNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(ObjectKeys.d_Variant_PlanIncomes);
            var clsUI = new DataClsUI(cls); 
            if (VariantYear != 0)
                clsUI.AdditionalFilter = string.Format(" and RefYear = {0}", VariantYear);
            clsUI.Workplace = ConsBudgetForecastNavigation.Instance.Workplace;
            clsUI.Initialize();
            clsUI.InitModalCls(-1);
            if (soirceId >= 0)
                clsUI.RefreshAttachedData(soirceId);
            else
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
            clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].Columns[codeName].SortIndicator =
                SortIndicator.Ascending;
            if (tmpClsForm.ShowDialog() == DialogResult.OK)
            {
                UltraGridRow activeRow = UltraGridHelper.GetActiveRowCells(clsUI.UltraGridExComponent.ugData);
                foreach (string columnName in columns)
                {
                    values.Add(activeRow.Cells[columnName].Value);
                }
                return true;
            }
            return false;
        }

        private void uteVariant_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            List<string> columns = new List<string>(new string[] { "ID", "Name", "RefYear", "VariantDate" });
            List<object> values = new List<object>();
            if (ChooseVariant(-1, "Code", columns, ref values))
            {
                uteVariant.Text = values[1].ToString();
                SelectedVariantId = Convert.ToInt64(values[0]);
                ultraButton1.Enabled = true;
            }
        }
    }
}
