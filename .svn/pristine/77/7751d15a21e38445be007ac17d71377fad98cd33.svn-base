using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolTip;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    public partial class AcquittancePercentsWizard : Form
    {
        private DataSet dsCustomPeriod;

        internal PayPeriodicity payPeriodicity;

        private Dictionary<string, PercentCalculationParams> percentSchemes;

        public AcquittancePercentsWizard(PayPeriodicity payPeriodicity)
        {
            InitializeComponent(); 
            this.payPeriodicity = payPeriodicity;
            percentSchemes = new Dictionary<string, PercentCalculationParams>();
            GetPercentSchemes(WorkplaceSingleton.Workplace.ActiveScheme);

            dsCustomPeriod = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(Int32));
            dt.Columns.Add("StartDate", typeof(DateTime));
            dt.Columns.Add("EndDate", typeof(DateTime));
            dsCustomPeriod.Tables.Add(dt);
            // метод с единовременным погашением долга
            AddEnumItemsToCombo(new PayPeriodicity[6]
            {
                PayPeriodicity.Month, PayPeriodicity.Quarter, PayPeriodicity.HalfYear,
                PayPeriodicity.Year, PayPeriodicity.Single, PayPeriodicity.Other
            });

            ugeCustomPeriods.ugData.DisplayLayout.GroupByBox.Hidden = true;
            ugeCustomPeriods.BorderStyle = BorderStyle.Fixed3D;
            ugeCustomPeriods.OnSaveChanges += new Krista.FM.Client.Components.SaveChanges(ugeCustomPeriods_OnSaveChanges);
            ugeCustomPeriods.OnCancelChanges += new Krista.FM.Client.Components.DataWorking(ugeCustomPeriods_OnCancelChanges);
            ugeCustomPeriods.OnClearCurrentTable += new Krista.FM.Client.Components.DataWorking(ugeCustomPeriods_OnClearCurrentTable);
            ugeCustomPeriods.OnGetGridColumnsState += new Krista.FM.Client.Components.GetGridColumnsState(ugeCustomPeriods_OnGetGridColumnsState);
            ugeCustomPeriods.OnAfterRowInsert += new AfterRowInsert(ugeCustomPeriods_OnAfterRowInsert);
            ugeCustomPeriods.DataSource = dsCustomPeriod;

            InitializWizardData();
            uceVersionName.SelectedIndex = 0;

            cePayPeriodicity.ValueChanged += new System.EventHandler(this.ParametersPage_ValueChanged);
            ccEndDate.ValueChanged += new System.EventHandler(this.ParametersPage_ValueChanged);
            ccStartDate.ValueChanged += new System.EventHandler(this.ParametersPage_ValueChanged);

            if (payPeriodicity == PayPeriodicity.Single)
            {
                lPeriodsCount.Text = "Количество периодов - 1";
                cbPaymentDate.SelectedIndex = 31;
            }
            ToolTipManager.DisplayStyle = ToolTipDisplayStyle.Standard;

            ccStartDate.MouseEnter += Control_MouseEnter;
            ccEndDate.MouseEnter += Control_MouseEnter;
            cePayPeriodicity.MouseEnter += Control_MouseEnter;
            uceEndPeriodDay.MouseEnter += Control_MouseEnter;
            cbPayDayCorrection.MouseEnter += Control_MouseEnter;
            cbEndDayShift.MouseEnter += Control_MouseEnter;
            cbPaymentDate.MouseEnter += Control_MouseEnter;
            cbFirstDayPayment.MouseEnter += Control_MouseEnter;
            cbPretermDischarge.MouseEnter += Control_MouseEnter;
            uceRoundResultParam.MouseEnter += Control_MouseEnter;

            cbEndDayShift.CheckedChanged += cbEndDayShift_CheckedChanged;

            dteFormDate.Value = DateTime.Today;
        }

        void cbEndDayShift_CheckedChanged(object sender, EventArgs e)
        {
            cbPaymentDate.Enabled = !cbEndDayShift.Checked;
        }

        private void Control_MouseEnter(object sender, EventArgs e)
        {
            Control control = sender as Control;
            if (control != null)
            {
                ToolTipManager.SetUltraToolTip(control, new UltraToolTipInfo(GetControlComment(control), ToolTipImage.None, string.Empty, DefaultableBoolean.True));
                ToolTipManager.ShowToolTip(control, wizardParametersPage.PointToScreen(new Point(control.Left, control.Bottom)));
            }
}

        /// <summary>
        /// создание списка отображения по всем значениям перечисления
        /// </summary>
        /// <param name="enumType"></param>
        private void AddEnumItemsToCombo(Type enumType)
        {
            cePayPeriodicity.Items.Clear();

            foreach (FieldInfo fi in enumType.GetFields())
            {
                if (fi.IsLiteral)
                {
                    DescriptionAttribute da = (DescriptionAttribute)Attribute.GetCustomAttribute(
                        fi, typeof(DescriptionAttribute));

                    ValueListItem valueListItem = new ValueListItem();
                    valueListItem.DataValue = fi.GetRawConstantValue();
                    valueListItem.DisplayText = da != null ? da.Description : fi.Name;
                    cePayPeriodicity.Items.Add(valueListItem);
                }
            }
            cePayPeriodicity.SelectedIndex = 0;
        }

        /// <summary>
        /// создание списка отображения из списка значений перечисления
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumItems"></param>
        private void AddEnumItemsToCombo<T>(IEnumerable<T> enumItems)
        {
            Type enumType = typeof(T);
            // если тип не перечисление, выходим, ничего не делаем
            if (!enumType.IsEnum)
                return;

            foreach (T enumItem in enumItems)
            {
                FieldInfo fldInfo = enumType.GetField(enumItem.ToString());
                DescriptionAttribute da = (DescriptionAttribute)Attribute.GetCustomAttribute(
                    fldInfo, typeof(DescriptionAttribute));
                string displayText = da != null ? da.Description : fldInfo.Name;
                cePayPeriodicity.Items.Add(fldInfo.GetRawConstantValue(), displayText);
            }
            cePayPeriodicity.SelectedIndex = 0;
        }

        void ugeCustomPeriods_OnAfterRowInsert(object sender, UltraGridRow row)
        {
            row.Cells["ID"].Value = dsCustomPeriod.Tables[0].Rows.Count + 1;
        }

        GridColumnsStates ugeCustomPeriods_OnGetGridColumnsState(object sender)
        {
            GridColumnsStates states = new GridColumnsStates();

            GridColumnState state = new GridColumnState();
            state.ColumnName = "ID";
            state.ColumnCaption = "Номер периода";
            state.ColumnWidth = 100;
            state.IsReadOnly = true;
            states.Add("ID", state);

            state = new GridColumnState();
            state.ColumnName = "StartDate";
            state.ColumnCaption = "Начало периода";
            state.ColumnWidth = 120;
            states.Add("StartDate", state);

            state = new GridColumnState();
            state.ColumnName = "EndDate";
            state.ColumnCaption = "Конец периода";
            state.ColumnWidth = 120;
            states.Add("EndDate", state);

            return states;
        }

        void ugeCustomPeriods_OnClearCurrentTable(object sender)
        {
            dsCustomPeriod.Clear();
            dsCustomPeriod.AcceptChanges();
        }

        void ugeCustomPeriods_OnCancelChanges(object sender)
        {
            dsCustomPeriod.Tables[0].RejectChanges();
        }

        bool ugeCustomPeriods_OnSaveChanges(object sender)
        {
            dsCustomPeriod.Tables[0].AcceptChanges();
            return true;
        }

        internal virtual void InitializWizardData()
        {

        }

        internal virtual void InitializWizardData(PercentCalculationParams percentScheme)
        {

        }

        private void ParametersPage_ValueChanged(object sender, EventArgs e)
        {
            DateTime startDate;
            DateTime endDate;
            if (!DateTime.TryParse(ccStartDate.Value.ToString(), out startDate) ||
                !DateTime.TryParse(ccEndDate.Value.ToString(), out endDate))
                return;

            int periodsCount = 0;
            payPeriodicity = (PayPeriodicity)cePayPeriodicity.Value;

            switch (payPeriodicity)
            {
                case PayPeriodicity.Other:
                    lPeriodsCount.Text = string.Empty;
                    cePayPeriodicity.SelectedIndex = GetIndex(PayPeriodicity.Other);
                    break;
                case PayPeriodicity.Single:
                    periodsCount = 1;
                    cePayPeriodicity.SelectedIndex = GetIndex(PayPeriodicity.Single);
                    break;
                default:
                    cePayPeriodicity.SelectedIndex = (int)payPeriodicity - 1;
                    periodsCount = Utils.GetPeriodCount(startDate, endDate,
                        (PayPeriodicity)Enum.Parse(typeof(PayPeriodicity),
                        (cePayPeriodicity.SelectedItem.DataValue).ToString()), Convert.ToInt32(uceEndPeriodDay.Value), dataSet.Tables[0], true);
                    break;
            }
            if (periodsCount > 0)
                lPeriodsCount.Text = string.Format("Количество периодов - {0}", periodsCount);
        }

        internal virtual void wizard_Next(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventNextArgs e)
        {
            if (e.CurrentPage == wizardWelcomePage1 && wizard.Pages[1] == wizardParametersPage)
            {
                string selectedScheme = cePercentSchemes.SelectedItem.DataValue.ToString();
                if (percentSchemes.ContainsKey(selectedScheme))
                {
                    InitializWizardData(percentSchemes[selectedScheme]);
                }
                else
                {
                    InitializWizardData();
                }
            }

            if (e.CurrentPage == wizardPageBase1)
            {
                if (uceVersionName.Enabled && string.IsNullOrEmpty(uceVersionName.Text))
                {
                    MessageBox.Show("Необходимо указать наименование версии расчета плана обслуживания", "План обслуживания долга", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    e.Step = 0;
                    return;
                }

                string selectedScheme = cePercentSchemes.SelectedItem.DataValue.ToString();
                if (percentSchemes.ContainsKey(selectedScheme))
                {
                    InitializWizardData(percentSchemes[selectedScheme]);
                }
                else
                {
                    InitializWizardData();
                }
            }

            if (e.CurrentPage == wizardParametersPage)
            {
				PayPeriodicity pp = (Server.PayPeriodicity)cePayPeriodicity.SelectedItem.DataValue;
				if (pp != Server.PayPeriodicity.Other)
                {
                    wizardFinalPage.Description2 = CalculateAcquittanceMainPlan();
                    e.Step = 2;
                    wizard.WizardButtons = Common.Wizards.WizardForm.TWizardsButtons.Next;
                }
                else
                {
                    e.Step = 1;
                }
            }
            if (e.CurrentPage == wizardCustomPeriodsPage)
            {
                if ((PayPeriodicity)cePayPeriodicity.SelectedItem.DataValue == PayPeriodicity.Other)
                {
                    if (dsCustomPeriod.Tables[0].Rows.Count == 0)
                    {
                        MessageBox.Show("Добавте хотя бы один период", "План обслуживания долга", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        e.Step = 0;
                        return;
                    }
                    foreach (DataRow row in dsCustomPeriod.Tables[0].Rows)
                    {
                        if (row.IsNull("StartDate") || row.IsNull("EndDate"))
                        {
                            MessageBox.Show("В периодах не все данные заполнены", "План погашения основного долга",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            e.Step = 0;
                            return;
                        }
                    }
                }
                wizardFinalPage.Description2 = CalculateAcquittanceMainPlan();
                wizard.WizardButtons = Common.Wizards.WizardForm.TWizardsButtons.Next;
            }
        }

        private void wizard_Back(object sender, Common.Wizards.WizardForm.EventNextArgs e)
        {
            if (e.CurrentPage.FinishPage)
            {
                PayPeriodicity pp = (PayPeriodicity)cePayPeriodicity.SelectedItem.DataValue;
                e.Step = pp == PayPeriodicity.Other ? 1 : 2;
            }
        }

        internal virtual string CalculateAcquittanceMainPlan()
        {
            return string.Empty;
        }

        private void wizard_Cancel(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventWizardCancelArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void wizard_Finish(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventNextArgs e)
        {
            Close();
        }

        private void wizard_WizardClosed(object sender, EventArgs e)
        {
            Close();
        }

        private void AcquittanceMainPlanWizard_Load(object sender, EventArgs e)
        {
            //DefaultFormState.Load(this);
        }

        private void AcquittanceMainPlanWizard_FormClosed(object sender, FormClosedEventArgs e)
        {
            //DefaultFormState.Save(this);
        }

        internal int GetIndex(PayPeriodicity period)
        {
            foreach (ValueListItem item in cePayPeriodicity.Items)
            {
                PayPeriodicity itemPeriod = (PayPeriodicity)Enum.Parse(typeof(PayPeriodicity), item.DataValue.ToString());
                if (itemPeriod == period)
                {
                    cePayPeriodicity.SelectedItem = item;
                    return cePayPeriodicity.SelectedIndex;
                }
            }
            return -1;
        }

        private string GetControlComment(Control control)
        {
            switch (control.Name)
            {
                case "ccStartDate":
                    return "Дата возникновения долгового обязательства";
                case "ccEndDate":
                    return "Дата окончательного погашения основного долга по договору";
                case "cePayPeriodicity":
                    return "Периодичность погашения процентов";
                case "uceEndPeriodDay":
                    return "День, до которого осуществляется начисление процентов в каждом периоде";
                case "cbPayDayCorrection":
                    {
                        int correctionIndex = cbPayDayCorrection.SelectedIndex;
                        switch (correctionIndex)
                        {
                            case 0:
                                return "При попадании на выходной или праздничный день дата выплаты не переносится";
                            case 1:
                                return "При попадании на выходной или праздничный день дата выплаты переносится на первый рабочий день," +
                                    Environment.NewLine + "следующий за нерабочим днем (днями)";
                            case 2:
                                return "При попадании на выходной или праздничный день дата выплаты переносится на рабочий день," +
                                    Environment.NewLine + "предшествующий нерабочему дню (дням)";
                        }
                        break;
                    }
                case "cbEndDayShift":
                    return "День окончания периода смещается с учетом корректировки на праздничные и выходные дни";
                case "cbPaymentDate":
                    return "День, до которого осуществляется выплата процентов в каждом периоде";
                case "cbFirstDayPayment":
                    return "Начисление процентов с 1-го дня возникновения долгового обязательства";
                case "cbPretermDischarge":
                    return "Возможность досрочного погашения по договору";
                case "uceRoundResultParam":
                    return "Расчет процентов с учетом остатков округлений (банковский расчет)";
            }
            return string.Empty;
        }

        /// <summary>
        /// получение схем расчета детали обслуживания долга
        /// </summary>
        /// <param name="scheme"></param>
        private void GetPercentSchemes(IScheme scheme)
        {
            // получение xml из репозитория
            DataRow[] schemesRows = scheme.TemplatesService.Repository.GetTemplatesInfo(TemplateTypes.System).Select(string.Format(
                "Code = '{0}'", "PercentSchemes"));
            if (schemesRows == null || schemesRows.Length == 0)
                return;
            byte[] tmpDocData = scheme.TemplatesService.Repository.GetDocument(Convert.ToInt32(schemesRows[0]["ID"]));
            tmpDocData = DocumentsHelper.DecompressFile(tmpDocData);
            MemoryStream stream = new MemoryStream(tmpDocData);
            XDocument xmlDoc = XDocument.Load(XmlReader.Create(stream));
            
            foreach (var node in xmlDoc.Descendants())
            {
                PercentCalculationParams percentScheme = new PercentCalculationParams();
                string schemeKey = string.Empty;
                foreach (var attribute in node.Attributes())
                {
                    if (string.Compare(attribute.Name.ToString(), "name", true) == 0)
                    {
                        cePercentSchemes.Items.Add(attribute.Value);
                        schemeKey = attribute.Value;
                    }

                    if (string.Compare(attribute.Name.ToString(), "payDayCorrection", true) == 0)
                    {
                        percentScheme.PaymentDayCorrection = (DayCorrection)Enum.Parse(typeof(DayCorrection), attribute.Value);
                    }

                    if (string.Compare(attribute.Name.ToString(), "endPeriodShift", true) == 0)
                    {
                        percentScheme.EndPeriodDayShift = Convert.ToBoolean(attribute.Value);
                    }

                    if (string.Compare(attribute.Name.ToString(), "endPeriodDay", true) == 0)
                    {
                        percentScheme.EndPeriodDay = Convert.ToInt32(attribute.Value);
                    }

                    if (string.Compare(attribute.Name.ToString(), "payDay", true) == 0)
                    {
                        percentScheme.PaymentDay = Convert.ToInt32(attribute.Value);
                    }

                    if (string.Compare(attribute.Name.ToString(), "allowRound", true) == 0)
                    {
                        percentScheme.RestRound = (PercentRestRound)Convert.ToInt32(attribute.Value);
                    }
                }
                if (!percentSchemes.ContainsKey(schemeKey))
                    percentSchemes.Add(schemeKey, percentScheme);
            }
        }

        private void cbSplitPercentPeriods_CheckedChanged(object sender, EventArgs e)
        {

        }

        public DateTime FormDate
        {
            get; set;
        }

        public string FormComment
        {
            get; set;
        }
    }
}