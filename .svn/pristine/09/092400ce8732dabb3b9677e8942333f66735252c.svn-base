using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Client.Common;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Calculations.DDE;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    public partial class DDEIndicatorsView : BaseView
    {
        public IScheme Scheme
        {
            get; set;
        }

        public DDEIndicatorsView()
        {
            InitializeComponent();
            SetDefaultParams();
			//InfragisticComponentsCustomize.CustomizeUltraGridParams(this.ugeIndicators._ugData);
            btnCalculate.Appearance.Image = Resources.ru.calculator_icon_32;
		}

        public void SetDefaultParams()
        {
            nudYear.Value = DateTime.Today.Year;
            formDate.Value = DateTime.Today;
            cbMonthSelect.SelectedIndex = 0;
            groupBox3.Enabled = false;
            cbMonthSelect.Enabled = false;
            cbSplitQuarter.Enabled = false;
            cbCalculationPeriod.SelectedIndex = 0;
            cbPlaningLevel.SelectedIndex = 0;
            cb2.Checked = true;
        }

        public string Comment
        {
            get { return tbSaveComment.Text; }
            set { tbSaveComment.Text = value; }
        }

        public override string Text
        {
            get { return "Доступная долговая емкость"; }
        }

        public DdeCalculationParams GetCalculationParams()
        {
            DdeCalculationParams calculationParams = new DdeCalculationParams();
            calculationParams.PlaningLevel = (DdePlaningLevel)cbPlaningLevel.SelectedIndex;
            calculationParams.CalculationPeriod = (DdeCalculationPeriod) cbCalculationPeriod.SelectedIndex;
            calculationParams.FormDate = Convert.ToDateTime(formDate.Value);
            calculationParams.Month = cbMonthSelect.SelectedIndex + 1;
            calculationParams.Quarter = rb1q.Checked ? 1 : rb2q.Checked ? 2 : rb3q.Checked ? 3 : 4;
            calculationParams.Year = Convert.ToInt32(nudYear.Value);
            calculationParams.ConsiderRestChange = cb2.Checked;
            calculationParams.ReduceCharge = cb1.Checked;
            calculationParams.EurRate = Convert.ToDecimal(eur.Value);
            calculationParams.UsdRate = Convert.ToDecimal(usd.Value);
            calculationParams.SplitQuarterToMonths = cbSplitQuarter.Checked;
            calculationParams.PlaningVariant = Variant.Tag == null ? -1 : Convert.ToInt32(Variant.Tag);
            calculationParams.Comment = Comment;
            return calculationParams;
        }

        public DataRow GetCurrentCalculationParamsRow(DataTable dtParams)
        {
            DdeCalculationParams calculationParams = GetCalculationParams();
            DataRow newParams = dtParams.NewRow();
            newParams.BeginEdit();
            newParams["SourceID"] = FinSourcePlanningNavigation.Instance.CurrentSourceID;
            newParams["TaskID"] = -1;
            newParams["FlagPeriod"] = (int)calculationParams.PlaningLevel;
            newParams["CalcPeriod"] = (int)calculationParams.CalculationPeriod;
            newParams["Year"] = calculationParams.Year;
            newParams["Quarter"] = calculationParams.Quarter;
            newParams["Month"] = calculationParams.Month;
            newParams["ToReduceExpInv"] = calculationParams.ReduceCharge;
            newParams["ToCnsdrChngRmns"] = calculationParams.ConsiderRestChange;
            newParams["USDExchR"] = calculationParams.UsdRate;
            newParams["EURExchR"] = calculationParams.EurRate;
            newParams["RefBrwVariant"] = calculationParams.PlaningVariant;
            newParams["ToBreakQOnM"] = calculationParams.SplitQuarterToMonths;
            newParams.EndEdit();
            return newParams;
        }

        public void SetCalculationParams(DataRow calculationRow)
        {
            DdeCalculationParams calculationParams = GetCalculationParams(calculationRow);
            switch (calculationParams.PlaningLevel)
            {
                case DdePlaningLevel.CurrentYear:
                    cbPlaningLevel.SelectedIndex = 0;
                    break;
                case DdePlaningLevel.NextYear:
                    cbPlaningLevel.SelectedIndex = 1;
                    break;
            }
            
            switch (calculationParams.CalculationPeriod)
            {
                case DdeCalculationPeriod.Year:
                    cbCalculationPeriod.SelectedIndex = 0;
                    break;
                case DdeCalculationPeriod.Quarter:
                    cbCalculationPeriod.SelectedIndex = 1;
                    break;
                case DdeCalculationPeriod.Month:
                    cbCalculationPeriod.SelectedIndex = 2;
                    break;
            }

            nudYear.Value = calculationParams.Year;
            cbMonthSelect.SelectedIndex = calculationParams.Month - 1;
            rb1q.Checked = calculationParams.Quarter == 1;
            rb2q.Checked = calculationParams.Quarter == 2;
            rb3q.Checked = calculationParams.Quarter == 3;
            rb4q.Checked = calculationParams.Quarter == 4;
            cb2.Checked = calculationParams.ConsiderRestChange;
            cb1.Checked = calculationParams.ReduceCharge;
            eur.Value = calculationParams.EurRate;
            usd.Value = calculationParams.UsdRate;
            cbSplitQuarter.Checked = calculationParams.SplitQuarterToMonths;
            tbSaveComment.Text = calculationParams.Comment;
            Variant.Text = GetVariantName(calculationParams.PlaningVariant);
            Variant.Tag = calculationParams.PlaningVariant;

        }

        public DdeCalculationParams GetCalculationParams(DataRow calculationRow)
        {
            DdeCalculationParams calculationParams = new DdeCalculationParams();
            int planingLevel = Convert.ToInt32(calculationRow["FlagPeriod"]);
            calculationParams.PlaningLevel = planingLevel == 0 ? DdePlaningLevel.CurrentYear : DdePlaningLevel.NextYear;
            int calcPeriod = Convert.ToInt32(calculationRow["CalcPeriod"]);
            calculationParams.CalculationPeriod = calcPeriod == 0
                                                      ? DdeCalculationPeriod.Year
                                                      : calcPeriod == 1
                                                            ? DdeCalculationPeriod.Quarter
                                                            : DdeCalculationPeriod.Month;
            calculationParams.Year = Convert.ToInt32(calculationRow["Year"]);
            calculationParams.Quarter = Convert.ToInt32(calculationRow["Quarter"]);
            calculationParams.Month = Convert.ToInt32(calculationRow["Month"]);
            calculationParams.ReduceCharge = Convert.ToBoolean(calculationRow["ToReduceExpInv"]);
            calculationParams.ConsiderRestChange = Convert.ToBoolean(calculationRow["ToCnsdrChngRmns"]);
            calculationParams.PlaningVariant = Convert.ToInt32(calculationRow["RefBrwVariant"]);
            calculationParams.UsdRate = Convert.ToInt32(calculationRow["USDExchR"]);
            calculationParams.EurRate = Convert.ToInt32(calculationRow["EURExchR"]);
            calculationParams.SplitQuarterToMonths = Convert.ToBoolean(calculationRow["ToBreakQOnM"]);
            calculationParams.Comment = calculationRow["CalcComment"].ToString();
            return calculationParams;
        }

        private int ChooseVariant(string clsKey, ref string variantCaption)
        {
            frmModalTemplate tmpClsForm = new frmModalTemplate();
            IEntity cls = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(clsKey);
            BaseClsUI clsUI = new DataClsUI(cls);
            clsUI.Workplace = FinSourcePlanningNavigation.Instance.Workplace;
            clsUI.AdditionalFilter = " and ID > 0";
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

        private void Variant_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            string variantCaption = string.Empty;
            int variant = ChooseVariant(SchemeObjectsKeys.d_Variant_Borrow_Key, ref variantCaption);
            Variant.Text = variantCaption;
            Variant.Tag = variant != -1 ? (object)variant : null;
        }

        internal void SetCurrencyRates(IScheme scheme)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                bool eurCredits = Convert.ToInt32(db.ExecQuery("select Count(id) from f_S_Creditincome where RefOKV in (select id from d_OKV_Currency where Code = 978) and RefVariant = 0", QueryResultTypes.Scalar)) > 0;
                bool usdCredits = Convert.ToInt32(db.ExecQuery("select Count(id) from f_S_Creditincome where RefOKV in (select id from d_OKV_Currency where Code = 840) and RefVariant = 0", QueryResultTypes.Scalar)) > 0;

                eur.Visible = eurCredits;
                label6.Visible = eurCredits;
                if (eurCredits)
                {
                    object queryResult = db.ExecQuery(
                        @"select ExchangeRate from d_S_ExchangeRate where RefOKV in (select id from d_OKV_Currency where Code = 978) and 
                        DateFixing = (select max(DateFixing) from d_S_ExchangeRate where DateFixing <= ?)",
                        QueryResultTypes.Scalar,
                        new DbParameterDescriptor("p0", DateTime.Today));
                    eur.Value = queryResult;
                }

                usd.Visible = usdCredits;
                label5.Visible = usdCredits;
                if (usdCredits)
                {
                    object queryResult = db.ExecQuery(
                        @"select ExchangeRate from d_S_ExchangeRate where RefOKV in (select id from d_OKV_Currency where Code = 840) and 
                        DateFixing = (select max(DateFixing) from d_S_ExchangeRate where DateFixing <= ?)",
                        QueryResultTypes.Scalar,
                        new DbParameterDescriptor("p0", DateTime.Today));
                    usd.Value = queryResult;
                }
            }
        }

        private void tbSaveComment_TextChanged(object sender, EventArgs e)
        {
            //Comment = tbSaveComment.Text;
        }

        public string ResultComment
        {
            get
            {
                return tbResultComment.Text;
            }
            set
            {
                tbResultComment.Text = value;
            }
        }

        private string GetVariantName(int variant)
        {
            using (IDatabase db = Scheme.SchemeDWH.DB)
            {
                return db.ExecQuery("select name from d_Variant_Borrow where id = ?", QueryResultTypes.Scalar,
                                    new DbParameterDescriptor("p0", variant)).ToString();
            }
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {

        }

        private void cbCalculationPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCalculationPeriod.SelectedIndex == 0)
            {
                groupBox3.Enabled = false;
                cbMonthSelect.Enabled = false;
                cbSplitQuarter.Enabled = false;
            }

            if (cbCalculationPeriod.SelectedIndex == 1)
            {
                groupBox3.Enabled = true;
                cbMonthSelect.Enabled = false;
                cbSplitQuarter.Enabled = true;
            }

            if (cbCalculationPeriod.SelectedIndex == 2)
            {
                groupBox3.Enabled = false;
                cbMonthSelect.Enabled = true;
                cbSplitQuarter.Enabled = false;
            }
        }

        internal void SetCalculationComment()
        {
            string calculationComment = string.Empty;
            DdeCalculationParams calculationParams = GetCalculationParams();
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Year)
                calculationComment = string.Format("Расчет долговой емкости {0} г.", calculationParams.Year);
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Month)
                calculationComment = string.Format("Расчет долговой емкости за {0} {1} г.", cbMonthSelect.Text, calculationParams.Year);
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Quarter)
            {
                calculationComment = !calculationParams.SplitQuarterToMonths
                           ? string.Format("Расчет долговой емкости за {0} квартал {1} г.", calculationParams.Quarter,
                           calculationParams.Year) :
                           string.Format("Расчет долговой емкости за {0} квартал {1} г. (детальный)", calculationParams.Quarter,
                           calculationParams.Year);
            }
            Comment = calculationComment;
        }

        internal string GetReportPeriod()
        {
            string calculationComment = string.Empty;
            DdeCalculationParams calculationParams = GetCalculationParams();
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Year)
                calculationComment = string.Format("{0} г.", calculationParams.Year);
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Month)
                calculationComment = string.Format("{0} {1} г.", cbMonthSelect.Text, calculationParams.Year);
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Quarter)
            {
                calculationComment = !calculationParams.SplitQuarterToMonths
                           ? string.Format("{0} квартал {1} г.", calculationParams.Quarter,
                           calculationParams.Year) :
                           string.Format("{0} квартал {1} г. (детальный)", calculationParams.Quarter,
                           calculationParams.Year);
            }
            return calculationComment;
        }

        private void cbPlaningLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPlaningLevel.SelectedIndex == 1)
            {
                cbCalculationPeriod.SelectedIndex = 0;
                cbCalculationPeriod.Enabled = false;
            }
            else
            {
                cbCalculationPeriod.Enabled = true;
            }
        }
    }
}
