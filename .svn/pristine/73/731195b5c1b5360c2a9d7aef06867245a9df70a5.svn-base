using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Components;
using Krista.FM.Client.Reports;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalOperations.DataWarning;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalOperations.LoanToCredit
{
    public class LoanToCreditUI : BaseCapitalOperationsUI
    {
        public LoanToCreditUI(string key)
            : base(key)
        {
            Caption = "Планирование операций с ценными бумагами";
        }

        protected override void SetViewCtrl()
        {
            fViewCtrl = new LoanToCreditView();
            fViewCtrl.ViewContent = this;
        }

        public LoanToCreditView ViewObject
        {
            get; set;
        }

        private long Id
        {
            get;
            set;
        }

        private long CapitalId
        {
            get; set;
        }

        private string CapitalOfficialNumber
        {
            get; set;
        }

        private int PaimentId
        {
            get; set;
        }

        private DateTime EndCoupunDate
        {
            get; set;
        }

        private LoanToCreditService CalculateService
        {
            get; set;
        }

        private DataTable Capitals
        {
            get; set;
        }

        private string RedemptionDateFilter
        {
            get; set;
        }

        private bool IsLoadData
        {
            get; set;
        }

        private string Name
        {
            get; set;
        }

        private DateTime CalculationDate
        {
            get; set;
        }

        private DateTime StartPeriod
        {
            get; set;
        }

        private DateTime EndPeriod
        {
            get; set;
        }

        private DataTable CreditServicePlan
        {
            get; set;
        }

        public override void Initialize()
        {
            base.Initialize();
            ViewObject = (LoanToCreditView) fViewCtrl;

            CalculateService = new LoanToCreditService(Workplace.ActiveScheme);

            SetDefaultValues();

            ViewObject.ToolbarsManager.ToolClick += ToolbarsManager_ToolClick;
            ViewObject.ToolbarsManager.ToolValueChanged += ToolbarsManager_ToolValueChanged;

            ViewObject.RedemptionDate.ValueChanged += RedemptionDate_ValueChanged;
            ViewObject.DateDischarge.ValueChanged += DateDischarge_ValueChanged;
            ViewObject.Nom.ValueChanged += Nom_ValueChanged;
            ViewObject.CP.ValueChanged += CP_ValueChanged;
            ViewObject.CPRub.ValueChanged += CPRub_ValueChanged;
            ViewObject.Count.ValueChanged += TotalCount_ValueChanged;
            ViewObject.Count.MouseEnterElement += Count_MouseEnterElement;
            ViewObject.Count.MouseLeave += Count_MouseLeave;

            ViewObject.CreditServisePlanGrid.InitializeLayout += CreditServisePlanGrid_InitializeLayout;
            ViewObject.CreditServisePlanGrid.SummaryValueChanged += new SummaryValueChangedEventHandler(CreditServisePlanGrid_SummaryValueChanged);

            List<string> creditAttractionList = new List<string>(new string[] { "на покрытие рыночной стоимости ценных бумаг и НКД", "на покрытие номинальной стоимости ценных бумаг" });
            ViewObject.CreditAttraction.DataSource = creditAttractionList;
            ViewObject.CreditAttraction.BeforeDropDown += BeforeDropDown;
            ViewObject.CreditAttraction.Rows[0].Activate();

            ViewObject.rb1.CheckedChanged += rb1_CheckedChanged;
            ViewObject.rb1.Checked = true;

            ViewObject.rb3.CheckedChanged += rb3_CheckedChanged;
            ViewObject.rb4.CheckedChanged += rb3_CheckedChanged;
            ViewObject.rb3.Checked = true;

            ViewObject.IsCalcRate.CheckedChanged += IsCalcRate_CheckedChanged;
            ViewObject.IsCalcRate.Checked = false;

            ViewObject.IsCalcFrmRSum.CheckedChanged += IsCalcFrmRSum_CheckedChanged;
            ViewObject.l1.Click += l1_Click;
            ViewObject.l2.Click += l1_Click;

            ViewObject.CrdtRate.ValueChanged += ValueChanged;
            ViewObject.CostServCrdt.ValueChanged += ValueChanged;
            ViewObject.Count.ValueChanged += ValueChanged;
            ViewObject.CrdtSum.ValueChanged += ValueChanged;
            ViewObject.RepaymentSum.ValueChanged += ValueChanged;

            SetEditorCheck(ViewObject.CrdtSum);
            SetEditorCheck(ViewObject.RepaymentSum);
            SetEditorCheck(ViewObject.TotalNom);
            SetEditorCheck(ViewObject.TotalCpnInc);
            SetEditorCheck(ViewObject.TotalDiffPCNom);
            SetEditorCheck(ViewObject.TotalServCrdt);
            SetEditorCheck(ViewObject.TotalCostServ);
            SetEditorCheck(ViewObject.CostServCrdt);
            SetEditorCheck(ViewObject.YTM);
            SetEditorCheck(ViewObject.CrdtRate);

            #region тултипы

            ViewObject.RedemptionDate.MouseEnterElement += RedemptionDate_MouseEnterElement;
            ViewObject.RedemptionDate.MouseLeave += RedemptionDate_MouseLeave;

            #endregion

            ViewObject.IsCalcFrmRSum.Checked = false;
            ViewObject.IsCalcFrmRSum.Checked = true;

            FillRegNumData();
            SetCalculations();

            SetRedemptionDate(true);

            bool enableCalculation = CheckCalculatePermission();
            BurnCalculationButton(enableCalculation);
        }

        #region настройка грида детали обслуживания кредита

        void CreditServisePlanGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            var layout = e.Layout;

            layout.Override.AllowColMoving = AllowColMoving.NotAllowed;
            layout.Override.SummaryDisplayArea = SummaryDisplayAreas.TopFixed;

            layout.Bands[0].Columns["ID"].SortIndicator = SortIndicator.Ascending;
            layout.Bands[0].Columns["ID"].Hidden = true;
            layout.Bands[0].Columns["StartDate"].Header.Caption = "Дата начала периода";
            layout.Bands[0].Columns["StartDate"].Width = 120;
            layout.Bands[0].Columns["StartDate"].CellActivation = Activation.NoEdit;

            layout.Bands[0].Columns["EndDate"].Header.Caption = "Дата окончания периода";
            layout.Bands[0].Columns["EndDate"].Width = 120;
            layout.Bands[0].Columns["EndDate"].CellActivation = Activation.NoEdit;

            layout.Bands[0].Columns["Sum"].Header.Caption = "Сумма процентов по кредиту";
            layout.Bands[0].Columns["Sum"].Width = 150;
            layout.Bands[0].Columns["Sum"].CellMultiLine = DefaultableBoolean.False;
            layout.Bands[0].Columns["Sum"].MaskDataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            layout.Bands[0].Columns["Sum"].MaskClipMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            layout.Bands[0].Columns["Sum"].MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            layout.Bands[0].Columns["Sum"].CellAppearance.TextHAlign = HAlign.Right;
            layout.Bands[0].Columns["Sum"].PadChar = '_';
            layout.Bands[0].Columns["Sum"].MaskInput = "-nnn,nnn,nnn,nnn.nn";
            layout.Bands[0].Columns["Sum"].CellActivation = Activation.NoEdit;

            if (!layout.Bands[0].Summaries.Exists("Sum"))
            {
                var s = layout.Bands[0].Summaries.Add("Sum", SummaryType.Sum, layout.Bands[0].Columns["Sum"]);
                s.DisplayFormat = "{0:##,##0.00#}";
                s.Appearance.TextHAlign = HAlign.Right;
            }

            layout.Bands[0].Columns["RefReplaceIssFLn"].Hidden = true;

        }

        void CreditServisePlanGrid_SummaryValueChanged(object sender, SummaryValueChangedEventArgs e)
        {
            if (e.SummaryValue.Key == "Sum")
            {
                //e.SummaryValue. = "{0:##,##0.00#}";
                e.SummaryValue.Appearance.TextHAlign = HAlign.Right;
            }
        }

        private void FillCreditServisePlan()
        {
            var calcParams = GetCalculationParams();
            // в случае если сумма кредита равна нулю, деталь не заполняем
            if (calcParams.CrdtSum == 0)
                return;

            decimal crdtSum = 0;

            CreditServicePlan.Clear();
            var serviceEntity =
                        Workplace.ActiveScheme.RootPackage.FindEntityByName("4810b086-68c1-48a4-a4f8-290d7e0692fb");
            var startDate = calcParams.StartCrdtDate.AddDays(1);
            var endDate = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));
            Decimal roundRest = 0;
            while (endDate < calcParams.EndCrdtDate)
            {
                var newRow = CreditServicePlan.NewRow();
                newRow["ID"] = serviceEntity.GetGeneratorNextValue;
                newRow["StartDate"] = startDate;
                newRow["EndDate"] = endDate;
                var sum = ((endDate - startDate).Days + 1) * calcParams.CrdtRate * calcParams.CrdtSum / 100 /
                          DaysInAYear(endDate.Year);
                roundRest += sum - Math.Round(sum, 2, MidpointRounding.ToEven);
                crdtSum += sum;
                newRow["Sum"] = Math.Round(sum, 2, MidpointRounding.AwayFromZero);

                CreditServicePlan.Rows.Add(newRow);

                startDate = endDate.AddDays(1);
                endDate = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));
            }
            {
                var newRow = CreditServicePlan.NewRow();
                newRow["ID"] = serviceEntity.GetGeneratorNextValue;
                newRow["StartDate"] = startDate;
                newRow["EndDate"] = calcParams.EndCrdtDate;
                var sum = ((calcParams.EndCrdtDate - startDate).Days + 1) * calcParams.CrdtRate * calcParams.CrdtSum / 100 /
                          DaysInAYear(endDate.Year);
                crdtSum += sum;
                newRow["Sum"] = Math.Round(sum + roundRest, 2, MidpointRounding.AwayFromZero);
                CreditServicePlan.Rows.Add(newRow);
            }

            ViewObject.CreditServisePlanGrid.DataSource = CreditServicePlan;
        }

        #endregion

        void Nom_ValueChanged(object sender, EventArgs e)
        {
            var editor = sender as UltraNumericEditor;
            if (editor == null)
                return;
            if (editor.Value == DBNull.Value || editor.Value == null)
                return;
            decimal rubMaxValue = Convert.ToDecimal(editor.Value) * 2;
            ViewObject.CPRub.MaxValue = rubMaxValue;
        }

        static void ValueChanged(object sender, EventArgs e)
        {
            var editor = sender as UltraNumericEditor;
            if (editor == null)
                return;
            if (editor.Value == DBNull.Value || editor.Value == null)
                editor.Value = 0;
        }

        void l1_Click(object sender, EventArgs e)
        {
            ViewObject.rb4.Checked = true;
        }

        void IsCalcFrmRSum_CheckedChanged(object sender, EventArgs e)
        {
            if (ViewObject.IsCalcFrmRSum.Checked)
            {
                ViewObject.CrdtSum.ReadOnly = true;
                //ViewObject.CrdtSum.Appearance.BackColor = Color.Khaki;
                ViewObject.RepaymentSum.ReadOnly = false;
                ViewObject.RepaymentSum.Appearance.ResetBackColor();
            }
            else
            {
                ViewObject.RepaymentSum.ReadOnly = true;
                //ViewObject.RepaymentSum.Appearance.BackColor = Color.Khaki;
                ViewObject.CrdtSum.ReadOnly = false;
                ViewObject.CrdtSum.Appearance.ResetBackColor();
            }
        }

        void IsCalcRate_CheckedChanged(object sender, EventArgs e)
        {
            if (ViewObject.IsCalcRate.Checked)
            {
                ViewObject.CrdtRate.ReadOnly = true;
                //ViewObject.CrdtRate.Appearance.BackColor = Color.Khaki;

                ViewObject.CostServCrdt.ReadOnly = false;
                ViewObject.CostServCrdt.Appearance.ResetBackColor();

                ViewObject.CostServCrdt.Value = ViewObject.CostServLn.Value;
            }
            else
            {
                ViewObject.CrdtRate.ReadOnly = false;
                ViewObject.CrdtRate.Appearance.ResetBackColor();

                ViewObject.CostServCrdt.ReadOnly = true;
                //ViewObject.CostServCrdt.Appearance.BackColor = Color.Khaki;

                ViewObject.CostServCrdt.Value = 0;
            }
        }

        void rb3_CheckedChanged(object sender, EventArgs e)
        {
            ViewObject.IsCalcFrmRSum.Enabled = ViewObject.rb3.Checked;
            if (ViewObject.rb3.Checked)
            {
                ViewObject.Count.ReadOnly = true;
                //ViewObject.Count.Appearance.BackColor = Color.Khaki;
                if (ViewObject.IsCalcFrmRSum.Checked)
                {
                    ViewObject.RepaymentSum.ReadOnly = false;
                    ViewObject.RepaymentSum.Appearance.ResetBackColor();
                }
                else
                {
                    ViewObject.CrdtSum.ReadOnly = false;
                    ViewObject.CrdtSum.Appearance.ResetBackColor();
                }
            }
            if (ViewObject.rb4.Checked)
            {
                ViewObject.Count.ReadOnly = false;
                ViewObject.Count.Appearance.ResetBackColor();
                ViewObject.CrdtSum.ReadOnly = true;
                //ViewObject.CrdtSum.Appearance.BackColor = Color.Khaki;
                ViewObject.RepaymentSum.ReadOnly = true;
                //ViewObject.RepaymentSum.Appearance.BackColor = Color.Khaki;
            }
        }

        void rb1_CheckedChanged(object sender, EventArgs e)
        {
            if (ViewObject.rb1.Checked)
            {
                ViewObject.YTM.ReadOnly = true;
                //ViewObject.YTM.Appearance.BackColor = Color.Khaki;
                ViewObject.CP.ReadOnly = false;
                ViewObject.CP.Appearance.ResetBackColor();
                ViewObject.CPRub.ReadOnly = false;
                ViewObject.CPRub.Appearance.ResetBackColor();
            }
            else
            {
                ViewObject.YTM.ReadOnly = false;
                ViewObject.YTM.Appearance.ResetBackColor();
                ViewObject.CP.ReadOnly = true;
                //ViewObject.CP.Appearance.BackColor = Color.Khaki;
                ViewObject.CPRub.ReadOnly = true;
                //ViewObject.CPRub.Appearance.BackColor = Color.Khaki;
            }
        }

        #region настройки отображения компонентов интерфейса

        void BeforeDropDown(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UltraCombo combo = (UltraCombo)sender;
            combo.DisplayLayout.Bands[0].ColHeadersVisible = false;
        }

        private void SetDefaultValues()
        {
            IsLoadData = true;
            Id = -1;

            ViewObject.StartCpnDate.Value = null;
            ViewObject.RedemptionDate.Value = DateTime.Today;
            //ViewObject.DateDischarge.Value = null;
            ViewObject.StartCrdtDate.Value = null;
            //ViewObject.EndCrdtDate.Value = null;
            ViewObject.CouponRate.Value = 0;
            ViewObject.Nom.Value = 0;
            ViewObject.Cpn.Value = 0;
            ViewObject.AI.Value = 0;
            ViewObject.YTM.Value = 0;
            ViewObject.CP.Value = 0;
            ViewObject.CPRub.Value = 0;
            ViewObject.DiffPCNom.Value = 0;
            ViewObject.CrdtRate.Value = 0;
            ViewObject.ServCrdt.Value = 0;
            ViewObject.CostServLn.Value = 0;
            ViewObject.CostServCrdt.Value = 0;
            ViewObject.Count.Value = 0;
            ViewObject.CrdtSum.Value = 0;
            ViewObject.RepaymentSum.Value = 0;
            ViewObject.TotalNom.Value = 0;
            ViewObject.TotalCpnInc.Value = 0;
            ViewObject.TotalDiffPCNom.Value = 0;
            ViewObject.TotalServCrdt.Value = 0;
            ViewObject.TotalCostServ.Value = 0;

            GetEmptyCreditDetailData();
            ViewObject.CreditServisePlanGrid.DataSource = CreditServicePlan;

            IsLoadData = false;
        }

        #endregion

        #region обработчики тулбара

        void ToolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            string toolKey = e.Tool.Key;
            switch (toolKey)
            {
                case "NewCalculation":
                    SetDefaultValues();
                    CleanWarnings();
                    ComboBoxTool cb = (ComboBoxTool) e.Tool.ToolbarsManager.Tools["Calculations"];
                    cb.SelectedIndex = -1;
                    SetRedemptionDate(true);
                    break;
                case "SaveCalculation":
                    if (SaveData())
                        BurnSaveDataButton(false);
                    break;
                case "DeleteCalculation":
                    cb = (ComboBoxTool)e.Tool.ToolbarsManager.Tools["Calculations"];
                    if (cb.SelectedIndex == -1)
                        return;
                    if (MessageBox.Show(string.Format("Удалить расчет '{0}'?", CurrentCalculationCaption),
                        "Удаление расчета", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DeleteData();
                        DeleteCalculation();
                    }
                    break;
                case "Calculate":
                    // перед расчетом загружаем данные, актуальные для выбранного выпуска ценных бумаг
                    SetRedemptionDate(false);
                    if (CalculationParamsValidation())
                    {
                        Calculate();
                        FillCreditServisePlan();
                        BurnSaveDataButton(true);
                    }
                    else
                    {
                        MessageBox.Show("Не все параметры заполнены для расчета или заполнены не корректно", "Расчет", MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    }
                    break;
                case "CreateReport":
                    if (Id > 0)
                    {
                        var reportCommand = new ReportMOExchangeCapitalCreditCommand
                                                {
                                                    window = Workplace.WindowHandle,
                                                    operationObj = Workplace.OperationObj,
                                                    scheme = Workplace.ActiveScheme
                                                };
                        reportCommand.SetReportParamValue(ReportConsts.ParamMasterFilter, Id);
                        reportCommand.Run();
                    }
                    break;
            }
        }

        void ToolbarsManager_ToolValueChanged(object sender, ToolEventArgs e)
        {
            string toolKey = e.Tool.Key;
            ComboBoxTool cb = (ComboBoxTool)e.Tool;
            CleanWarnings();
            switch (toolKey)
            {
                case "RegNum":
                    SetDefaultValues();
                    CapitalId = Convert.ToInt64(cb.ValueList.ValueListItems[cb.SelectedIndex].DataValue);
                    DataRow capitalRow = Capitals.Select(string.Format("ID = {0}", CapitalId))[0];
                    if (capitalRow.IsNull(2) || capitalRow.IsNull(3))
                        break;
                    ViewObject.DateDischarge.Value = capitalRow[3];
                    CapitalOfficialNumber = capitalRow[1].ToString();
                    StartPeriod = Convert.ToDateTime(capitalRow[2]);
                    EndPeriod = Convert.ToDateTime(capitalRow[3]);
                    RedemptionDateFilter =
                        string.Format("Дата выкупа должна входить в диапазон от {0} до {1} не включая границы интервала",
                                     Convert.ToDateTime(capitalRow[2]).ToShortDateString(), Convert.ToDateTime(capitalRow[3]).ToShortDateString());
                    SetRedemptionDate(true);
                    break;
                case "Calculations":
                    if (cb.SelectedIndex == -1)
                    {
                        SetDefaultValues();
                        return;
                    }
                    string calculationKey = cb.ValueList.ValueListItems[cb.SelectedIndex].DataValue.ToString();
                    CurrentCalculationCaption = cb.ValueList.ValueListItems[cb.SelectedIndex].DisplayText;
                    LoadData(calculationKey);
                    break;
            }
        }

        private void FillRegNumData()
        {
            ComboBoxTool cb = (ComboBoxTool)ViewObject.ToolbarsManager.Tools["RegNum"];
            Dictionary<long, string> regNumList = GetRegNumList();
            foreach (KeyValuePair<long, string> kvp in regNumList)
            {
                cb.ValueList.ValueListItems.Add(kvp.Key, kvp.Value);
            }
            if (regNumList.Count > 0)
                cb.SelectedIndex = 0;
        }

        private Dictionary<long, string> GetRegNumList()
        {
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                Dictionary<long, string> regNumList = new Dictionary<long, string>();
                Capitals = new DataTable();
                Capitals = (DataTable)db.ExecQuery("select Id, OfficialNumber, StartDate, DateDischarge from f_S_Capital where ExstraIssue = 0 and RefVariant = 0 and DateDischarge > ? order by OfficialNumber",
                             QueryResultTypes.DataTable,
                             new DbParameterDescriptor("p0", DateTime.Today));

                foreach (DataRow row in Capitals.Rows)
                {
                    regNumList.Add(Convert.ToInt64(row[0]), row[1].ToString());
                }
                return regNumList;
            }
        }

        #endregion

        #region расчеты

        void CP_ValueChanged(object sender, EventArgs e)
        {
            if (IsLoadData)
                return;

            if (ViewObject.CP.ReadOnly)
                return;
            object value = ViewObject.CP.Value;
            if (value == DBNull.Value || value == null)
            {
                ViewObject.CP.Value = 0;
            }
            object nom = ViewObject.Nom.Value;
            if (CheckValue(value) && CheckValue(nom))
                ViewObject.CPRub.Value = Math.Round(Convert.ToDecimal(nom) * Convert.ToDecimal(value) / 100, 2, MidpointRounding.AwayFromZero);
        }

        void CPRub_ValueChanged(object sender, EventArgs e)
        {
            if (IsLoadData)
                return;
            if (ViewObject.CPRub.ReadOnly)
                return;

            var calcParams = GetCalculationParams();
            if (calcParams.Nom == 0)
                return;

            object value = ViewObject.CPRub.Value;
            if (value == DBNull.Value || value == null)
            {
                ViewObject.CPRub.Value = 0;
            }
            if (CheckValue(value))
                ViewObject.CP.Value = Math.Round(Convert.ToDecimal(value) * 100 / calcParams.Nom, 4, MidpointRounding.AwayFromZero);
        }

        void DateDischarge_ValueChanged(object sender, EventArgs e)
        {
            if (IsLoadData)
                return;
            DateTime dateDischarge = Convert.ToDateTime(ViewObject.DateDischarge.Value);
            ViewObject.EndCrdtDate.Value = dateDischarge;
        }

        void RedemptionDate_ValueChanged(object sender, EventArgs e)
        {
            if (ViewObject.RedemptionDate.Value == null || ViewObject.RedemptionDate.Value == DBNull.Value)
            {
                ViewObject.MaxCapitalsCount.Value = 0;
                return;
            }
               
            DateTime redemptionDate = Convert.ToDateTime(ViewObject.RedemptionDate.Value);
            if (redemptionDate.Year >= 1980)
            {
                long maxcapitalCount = CapitalOperationsServer.GetCapitalCount(CapitalId,
                                                                                redemptionDate);
                ViewObject.MaxCapitalsCount.Value = maxcapitalCount;
            }
            // если мы загружаем сохраненные данные, ничего пересчитывать не будем)
            if (IsLoadData)
                return;
            SetRedemptionDate(true);
        }

        void TotalCount_ValueChanged(object sender, EventArgs e)
        {
            BurnEditor(false, ViewObject.Count);
            if (ViewObject.Count.ReadOnly)
            {
                LoanToCreditParams calcParams = GetCalculationParams();
                if (calcParams.Count > calcParams.MaxCapitalsCount)
                {
                    BurnEditor(true, ViewObject.Count);
                }
            }
        }

        private void SetRedemptionDate(bool fillCreditDates)
        {
            // заполнение данными по ценным бумагам после изменения даты погашения
            DateTime redemptionDate = Convert.ToDateTime(ViewObject.RedemptionDate.Value);
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                string query =
                    @"select Id, EndDate, StartDate from t_S_CPPlanService where RefCap = ? and 
                    StartDate = (select max(StartDate) from t_S_CPPlanService where RefCap = ? and StartDate <= ?)";
                DataTable dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable,
                    new DbParameterDescriptor("p0", CapitalId),
                    new DbParameterDescriptor("p1", CapitalId),
                    new DbParameterDescriptor("p2", redemptionDate));
                object startcpnDate = null;
                if (dt.Rows.Count > 0)
                {
                    if (!dt.Rows[0].IsNull(0))
                        PaimentId = Convert.ToInt32(dt.Rows[0][0]);
                    if (!dt.Rows[0].IsNull(1))
                    {
                        EndCoupunDate = Convert.ToDateTime(dt.Rows[0][1]);
                        query = @"select sum(Coupon) from t_S_CPJournalPercent where RefCap = ? and ChargeDate >= ?";
                        object coupon = db.ExecQuery(query, QueryResultTypes.Scalar,
                                                     new DbParameterDescriptor("p0", CapitalId),
                                                     new DbParameterDescriptor("p1", EndCoupunDate));
                        if (coupon != null && coupon != DBNull.Value)
                        {
                            ViewObject.Cpn.Value = coupon;
                            ViewObject.CostServLn.Value = Math.Round(Convert.ToDecimal(coupon), 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    if (!dt.Rows[0].IsNull(2))
                    {
                        ViewObject.StartCpnDate.Value = Convert.ToDateTime(dt.Rows[0][2]);
                        startcpnDate = Convert.ToDateTime(dt.Rows[0][2]);
                    }
                }
                else
                {
                    ViewObject.StartCpnDate.Value = null;
                }

                query =
                    @"select CreditPercent from t_S_CPJournalPercent where RefCap = ? and 
                    ChargeDate = (select min(ChargeDate) from t_S_CPJournalPercent where RefCap = ? and ChargeDate >= ?)";
                object couponRate = db.ExecQuery(query, QueryResultTypes.Scalar,
                    new DbParameterDescriptor("p0", CapitalId),
                    new DbParameterDescriptor("p1", CapitalId),
                    new DbParameterDescriptor("p2", redemptionDate));
                if (couponRate != null && couponRate != DBNull.Value)
                    ViewObject.CouponRate.Value = Math.Round(Convert.ToDecimal(couponRate), 2, MidpointRounding.AwayFromZero);

                query =
                    @"select sum(PercentNom) from t_S_CPPlanDebt where RefCap = ? and 
                    EndDate in (select EndDate from t_S_CPPlanDebt where RefCap = ? and EndDate <= ?)";
                object percentSum = db.ExecQuery(query, QueryResultTypes.Scalar,
                    new DbParameterDescriptor("p0", CapitalId),
                    new DbParameterDescriptor("p1", CapitalId),
                    new DbParameterDescriptor("p2", redemptionDate));
                object nom = null;
                if (percentSum != null && percentSum != DBNull.Value)
                {
                    nom = Math.Round(1000 - (1000 * Convert.ToDecimal(percentSum) / 100), 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    nom = 1000;
                }
                ViewObject.Nom.Value = Math.Round(Convert.ToDecimal(nom), 2, MidpointRounding.AwayFromZero);
                ViewObject.CPRub.MaxValue = Convert.ToDecimal(nom)*2;
                if ((couponRate != null && couponRate != DBNull.Value) &&
                   (startcpnDate != null && startcpnDate != DBNull.Value))
                {
                    decimal nkd = Convert.ToDecimal(nom) * Convert.ToDecimal(couponRate) *
                        (redemptionDate - Convert.ToDateTime(startcpnDate)).Days / 365 / 100;
                    ViewObject.AI.Value = Math.Round(nkd, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    ViewObject.AI.Value = 0;
                }

                if (redemptionDate.Year >= 1980)
                {
                    long maxcapitalCount = CapitalOperationsServer.GetCapitalCount(CapitalId, redemptionDate);
                    ViewObject.MaxCapitalsCount.Value = maxcapitalCount;
                }
            }

            if (fillCreditDates || ViewObject.StartCrdtDate.Value == null)
                ViewObject.StartCrdtDate.Value = redemptionDate;
            if (fillCreditDates || ViewObject.EndCrdtDate.Value == null)
            {
                DateTime dateDischarge = Convert.ToDateTime(ViewObject.DateDischarge.Value);
                ViewObject.EndCrdtDate.Value = dateDischarge;
            }
        }

        private LoanToCreditParams GetCalculationParams()
        {
            LoanToCreditParams calculationParams = new LoanToCreditParams();
            calculationParams.CalcDate = DateTime.Today;
            calculationParams.Name = Name;
            calculationParams.CapitalId = CapitalId;
            calculationParams.OfficialNumber = CapitalOfficialNumber;
            if (ViewObject.StartCpnDate.Value != null)
                calculationParams.StartCpnDate = Convert.ToDateTime(ViewObject.StartCpnDate.Value);
            calculationParams.RedemptionDate = Convert.ToDateTime(ViewObject.RedemptionDate.Value);
            calculationParams.PaimentId = PaimentId;
            calculationParams.EndCpnDate = EndCoupunDate;
            calculationParams.DateDischarge = Convert.ToDateTime(ViewObject.DateDischarge.Value);
            calculationParams.CouponRate = Convert.ToDecimal(ViewObject.CouponRate.Value);
            calculationParams.Cpn = Convert.ToDecimal(ViewObject.Cpn.Value);
            calculationParams.AI = Convert.ToDecimal(ViewObject.AI.Value);
            calculationParams.Nom = Convert.ToDecimal(ViewObject.Nom.Value);
            calculationParams.YTM = Convert.ToDecimal(ViewObject.YTM.Value);
            calculationParams.CPRub = Convert.ToDecimal(ViewObject.CPRub.Value);
            calculationParams.CP = Convert.ToDecimal(ViewObject.CP.Value);
            calculationParams.DiffPCNom = ViewObject.DiffPCNom.Value != null ? Convert.ToDecimal(ViewObject.DiffPCNom.Value) : 0;
            calculationParams.CreditAttraction = Convert.ToInt32(ViewObject.CreditAttraction.ActiveRow.Index);
            calculationParams.StartCrdtDate = Convert.ToDateTime(ViewObject.StartCrdtDate.Value);
            calculationParams.EndCrdtDate = Convert.ToDateTime(ViewObject.EndCrdtDate.Value);
            calculationParams.CostServCrdt = Convert.ToDecimal(ViewObject.CostServCrdt.Value);
            calculationParams.IsCalcRate = ViewObject.IsCalcRate.Checked;
            calculationParams.CrdtRate = Convert.ToDecimal(ViewObject.CrdtRate.Value);
            calculationParams.ServCrdt = Convert.ToDecimal(ViewObject.ServCrdt.Value);
            calculationParams.CostServLn = Convert.ToDecimal(ViewObject.CostServLn.Value);
            calculationParams.IsCalcFrmRSum = ViewObject.IsCalcFrmRSum.Checked;
            calculationParams.Count = Convert.ToInt64(ViewObject.Count.Value);
            calculationParams.CrdtSum = Convert.ToDecimal(ViewObject.CrdtSum.Value);
            calculationParams.RepaymentSum = Convert.ToDecimal(ViewObject.RepaymentSum.Value);
            calculationParams.TotalNom = Convert.ToDecimal(ViewObject.TotalNom.Value);
            calculationParams.TotalCpnInc = Convert.ToDecimal(ViewObject.TotalCpnInc.Value);
            calculationParams.TotalDiffPCNom = Convert.ToDecimal(ViewObject.TotalDiffPCNom.Value);
            calculationParams.TotalServCrdt = Convert.ToDecimal(ViewObject.TotalServCrdt.Value);
            calculationParams.TotalCostServ = Convert.ToDecimal(ViewObject.TotalCostServ.Value);
            calculationParams.MaxCapitalsCount = ViewObject.MaxCapitalsCount.Value != null
                                                     ? Convert.ToInt64(ViewObject.MaxCapitalsCount.Value)
                                                     : 0;
            return calculationParams;
        }

        private bool Calculate()
        {
            object redemptionDate = ViewObject.RedemptionDate.Value;
            if (!CheckValue(redemptionDate))
                return false;

            LoanToCreditParams calculationParams = GetCalculationParams();
            if (calculationParams.CouponRate == 0 || calculationParams.Nom == 0 || 
                calculationParams.Cpn == 0 || calculationParams.AI == 0)
            {
                return false;
            }

            CalculatedValue calculatedValue = ViewObject.rb1.Checked ? CalculatedValue.YTM : CalculatedValue.CurrPrice;

            object nkd = ViewObject.AI.Value;
            object dateDischarge = ViewObject.DateDischarge.Value;
            object couponRate = ViewObject.CouponRate.Value;
            object cpn = ViewObject.Cpn.Value;
            object ai = ViewObject.AI.Value;
            object nom = ViewObject.Nom.Value;
            object ytm = ViewObject.YTM.Value;
            object cPRub = ViewObject.CPRub.Value;

            if (CheckValue(nkd) && CheckValue(dateDischarge) && CheckValue(couponRate)
                && CheckValue(cpn) && CheckValue(ai) && CheckValue(nom) &&
                ((calculatedValue == CalculatedValue.YTM && CheckValue(cPRub)) ||
                (calculatedValue == CalculatedValue.CurrPrice && CheckValue(ytm))))
            {
                switch (calculatedValue)
                {
                    case CalculatedValue.CurrPrice:
                        cPRub = Math.Round(CapitalOperationsServer.Calculate(calculationParams.RedemptionDate, CapitalId,
                            calculationParams.YTM, calculatedValue) - calculationParams.AI, 3, MidpointRounding.AwayFromZero);
                        ViewObject.CPRub.Value = Math.Round(Convert.ToDecimal(cPRub), 2, MidpointRounding.AwayFromZero);
                        ViewObject.CP.Value = Math.Round((Convert.ToDecimal(cPRub)/calculationParams.Nom)*100, 3,
                                                         MidpointRounding.AwayFromZero);
                        break;
                    case CalculatedValue.YTM:
                        decimal cpRub = Math.Round(calculationParams.Nom * calculationParams.CP / 100, 4, MidpointRounding.AwayFromZero);
                        ytm = CapitalOperationsServer.Calculate(Convert.ToDateTime(redemptionDate), CapitalId,
                            cpRub + calculationParams.AI, calculatedValue);
                        ViewObject.YTM.Value = Math.Round(Convert.ToDecimal(ytm), 4, MidpointRounding.AwayFromZero);
                        break;
                }

                ViewObject.DiffPCNom.Value = Math.Round(Convert.ToDecimal(cPRub) - Convert.ToDecimal(nom), 2, MidpointRounding.AwayFromZero);

                calculationParams = GetCalculationParams();

                decimal sum = ViewObject.CreditAttraction.SelectedRow.Index == 1
                                    ? calculationParams.Nom
                                    : calculationParams.AI + calculationParams.CPRub;
                DateTime startDate = calculationParams.StartCrdtDate;
                DateTime endYearDate = new DateTime(startDate.Year, 12, 31);
                if (ViewObject.IsCalcRate.Checked)
                {
                    startDate = calculationParams.StartCrdtDate.AddDays(1);
                    endYearDate = new DateTime(startDate.Year, 12, 31);
                    decimal daysCount = 0;
                    decimal daysInYear = 0;
                    while (endYearDate < calculationParams.EndCrdtDate)
                    {
                        daysInYear = DaysInAYear(startDate.Year);
                        daysCount += decimal.Divide((endYearDate - startDate).Days + 1, daysInYear);
                        startDate = endYearDate.AddDays(1);
                        endYearDate = new DateTime(startDate.Year, 12, 31);
                    }
                    daysInYear = DaysInAYear(startDate.Year);
                    daysCount += decimal.Divide((calculationParams.EndCrdtDate - startDate).Days + 1, daysInYear);
                    decimal crdtRate = ((calculationParams.CostServCrdt - calculationParams.AI - calculationParams.DiffPCNom) * 100) / Math.Abs(sum) / daysCount;
                    ViewObject.CrdtRate.Value = Math.Round(crdtRate, 2, MidpointRounding.AwayFromZero);
                }
                calculationParams = GetCalculationParams();

                decimal servCrdt = 0;
                startDate = calculationParams.StartCrdtDate.AddDays(1);
                endYearDate = new DateTime(startDate.Year, 12, 31);
                while (endYearDate < calculationParams.EndCrdtDate)
                {
                    servCrdt += sum * ((endYearDate - startDate).Days + 1) * calculationParams.CrdtRate / DaysInAYear(startDate.Year) / 100;
                    startDate = endYearDate.AddDays(1);
                    endYearDate = new DateTime(startDate.Year, 12, 31);
                }
                servCrdt += sum * ((calculationParams.EndCrdtDate - startDate).Days + 1) *
                                    calculationParams.CrdtRate / DaysInAYear(startDate.Year) / 100;
                ViewObject.ServCrdt.Value = Math.Round(servCrdt, 2, MidpointRounding.AwayFromZero);

                calculationParams = GetCalculationParams();
                if (!ViewObject.IsCalcRate.Checked)
                    ViewObject.CostServCrdt.Value = Math.Round(calculationParams.AI + calculationParams.DiffPCNom +
                                                calculationParams.ServCrdt, 2, MidpointRounding.AwayFromZero);

                calculationParams = GetCalculationParams();

                if (ViewObject.rb3.Checked)
                {
                    if (calculationParams.IsCalcFrmRSum)
                    {
                        ViewObject.Count.Value =
                            Math.Truncate(
                                calculationParams.RepaymentSum / (calculationParams.CPRub + calculationParams.AI));
                    }
                    else
                    {
                        if (calculationParams.CreditAttraction == 1)
                            ViewObject.Count.Value = Math.Round(calculationParams.CrdtSum / (calculationParams.Nom), 2, MidpointRounding.AwayFromZero);
                        else
                        {
                            ViewObject.Count.Value =
                                Math.Round(
                                    calculationParams.CrdtSum / (calculationParams.CPRub + calculationParams.AI), 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    calculationParams = GetCalculationParams();

                    if (calculationParams.IsCalcFrmRSum)
                    {
                        if (calculationParams.CreditAttraction == 1)
                            ViewObject.CrdtSum.Value = Math.Round(calculationParams.Count*calculationParams.Nom, 2,
                                                                  MidpointRounding.AwayFromZero);
                        else
                        {
                            ViewObject.CrdtSum.Value =
                                Math.Round(calculationParams.Count * (calculationParams.CPRub + calculationParams.AI), 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        ViewObject.RepaymentSum.Value = Math.Round(calculationParams.Count *
                                                      (calculationParams.CPRub + calculationParams.AI), 2, MidpointRounding.AwayFromZero);  
                    }
                }

                if (ViewObject.rb4.Checked)
                {
                    if (calculationParams.CreditAttraction == 1)
                        ViewObject.CrdtSum.Value = Math.Round(calculationParams.Count * calculationParams.Nom, 2, MidpointRounding.AwayFromZero);
                    else
                    {
                        ViewObject.CrdtSum.Value =
                            Math.Round(calculationParams.Count * (calculationParams.CPRub + calculationParams.AI), 2, MidpointRounding.AwayFromZero);
                    }

                    ViewObject.RepaymentSum.Value = Math.Round(calculationParams.Count *
                                                    (calculationParams.CPRub + calculationParams.AI), 2, MidpointRounding.AwayFromZero);
                }

                calculationParams = GetCalculationParams();

                ViewObject.TotalNom.Value = Math.Round(calculationParams.Nom * calculationParams.Count, 2, MidpointRounding.AwayFromZero);
                ViewObject.TotalCpnInc.Value = Math.Round(calculationParams.AI * calculationParams.Count, 2, MidpointRounding.AwayFromZero);
                ViewObject.TotalDiffPCNom.Value = Math.Round(calculationParams.DiffPCNom * calculationParams.Count, 2, MidpointRounding.AwayFromZero);
                ViewObject.TotalServCrdt.Value = Math.Round(calculationParams.ServCrdt * calculationParams.Count, 2, MidpointRounding.AwayFromZero);
                ViewObject.TotalCostServ.Value = Math.Round(calculationParams.CostServCrdt * calculationParams.Count, 2, MidpointRounding.AwayFromZero);

                return true;
            }
            return false;
        }

        private bool CheckValue(object value)
        {
            return value != null && value != DBNull.Value;
        }

        #endregion

        #region тултипы

        void RedemptionDate_MouseEnterElement(object sender, UIElementEventArgs e)
        {
            if (string.IsNullOrEmpty(RedemptionDateFilter))
                return;

            ToolTip.ToolTipText = RedemptionDateFilter;
            Point tooltipPos = new Point(ViewObject.RedemptionDate.UIElement.Rect.Left, ViewObject.RedemptionDate.UIElement.Rect.Bottom);
            ToolTip.Show(ViewObject.RedemptionDate.PointToScreen(tooltipPos));
        }

        void RedemptionDate_MouseLeave(object sender, EventArgs e)
        {
            ToolTip.Hide();
        }

        void Count_MouseEnterElement(object sender, UIElementEventArgs e)
        {
            LoanToCreditParams calculationParams = GetCalculationParams();
            if (calculationParams.Count <= calculationParams.MaxCapitalsCount)
                return;
            ToolTip.ToolTipText = string.Format("Количество выкупаемых ценных бумаг не должно быть больше количества размещенных ценных бумаг ({0})", calculationParams.MaxCapitalsCount);
            Point tooltipPos = new Point(ViewObject.Count.UIElement.Rect.Left, ViewObject.Count.UIElement.Rect.Bottom);
            ToolTip.Show(ViewObject.Count.PointToScreen(tooltipPos));
        }

        void Count_MouseLeave(object sender, EventArgs e)
        {
            ToolTip.Hide();
        }

        #endregion

        #region загрузка и сохранение данных

        private class Calculation
        {
            internal Calculation(int id, DateTime calculationDate, string comment)
            {
                CalculationDate = calculationDate;
                Comment = comment;
                Id = id;
            }

            internal int Id
            {
                get; set;
            }

            internal DateTime CalculationDate
            {
                get; set;
            }

            internal string Comment
            {
                get; set;
            }
        }

        private List<Calculation> GetCalculationsList()
        {
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                List<Calculation> list = new List<Calculation>();
                string query = "select Distinct CalcDate, Name, id from f_S_ReplaceIssForLn order by CalcDate desc, Name";
                DataTable dt = (DataTable) db.ExecQuery(query, QueryResultTypes.DataTable);
                foreach (DataRow row in dt.Rows)
                {
                    Calculation calculation = new Calculation(Convert.ToInt32(row[2]), Convert.ToDateTime(row[0]), row[1].ToString());
                    list.Add(calculation);
                }
                return list;
            }
        }

        private void SetCalculations()
        {
            List<Calculation> calculations = GetCalculationsList();
            ComboBoxTool cb = (ComboBoxTool) ViewObject.ToolbarsManager.Tools["Calculations"];
            cb.ValueList.ValueListItems.Clear();
            foreach (Calculation calculation in calculations)
            {
                cb.ValueList.ValueListItems.Add(GetCalculationKey(calculation.Comment, calculation.CalculationDate),
                                                string.Format("{0} ({1})", calculation.Comment,
                                                              calculation.CalculationDate.ToShortDateString()));
            }
            if (cb.ValueList.ValueListItems.Count > 0)
                cb.SelectedIndex = 0;
        }

        private void SetCapital(long capitalId)
        {
            ComboBoxTool cb = (ComboBoxTool)ViewObject.ToolbarsManager.Tools["RegNum"];
            cb.SelectedItem = cb.ValueList.FindByDataValue(capitalId);
        }

        private void LoadData(string calculationKey)
        {
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                string calcName = string.Empty;
                DateTime calcDate = DateTime.MinValue;
                GetCalculationUniqueParams(calculationKey, ref calcDate, ref calcName);
                DataTable dt = (DataTable)db.ExecQuery("select * from f_S_ReplaceIssForLn where Name = ? and CalcDate = ?", QueryResultTypes.DataTable,
                             new DbParameterDescriptor("p0", calcName),
                             new DbParameterDescriptor("p1", calcDate));
                if (dt.Rows.Count > 0)
                {
                    IsLoadData = true;
                    DataRow row = dt.Rows[0];
                    Id = Convert.ToInt64(row["Id"]);
                    CapitalId = Convert.ToInt64(row["CapId"]);
                    SetCapital(CapitalId);
                    PaimentId = Convert.ToInt32(row["NumCpn"]);
                    Name = row["Name"].ToString();
                    CalculationDate = Convert.ToDateTime(row["CalcDate"]);
                    ViewObject.StartCpnDate.Value = row["StartCpnDate"];
                    EndCoupunDate = Convert.ToDateTime(row["EndCpnDate"]);
                    ViewObject.RedemptionDate.Value = row["RedemptionDate"];
                    ViewObject.DateDischarge.Value = row["DateDischarge"];
                    ViewObject.CouponRate.Value = row["CouponRate"];
                    ViewObject.Cpn.Value = row["Cpn"];
                    ViewObject.AI.Value = row["AI"];
                    ViewObject.Nom.Value = row["Nom"]; 
                    ViewObject.CP.Value = row["CP"];
                    ViewObject.CPRub.Value = row["CPRub"];
                    ViewObject.DiffPCNom.Value = row["DiffPCNom"];
                    ViewObject.YTM.Value = row["YTM"];
                    ViewObject.StartCrdtDate.Value = row["StartCrdtDate"];
                    ViewObject.EndCrdtDate.Value = row["EndCrdtDate"];
                    ViewObject.CrdtRate.Value = row["CrdtRate"];
                    ViewObject.ServCrdt.Value = row["ServCrdt"];
                    ViewObject.CostServLn.Value = row["CostServLn"];
                    ViewObject.CostServCrdt.Value = row["CostServCrdt"];
                    ViewObject.CreditAttraction.Rows[Convert.ToInt32(row["IsCoverNom"])].Activate();
                    ViewObject.Count.Value = row["Count"];
                    ViewObject.CrdtSum.Value = row["CrdtSum"];
                    ViewObject.RepaymentSum.Value = row["RepaymentSum"];
                    ViewObject.TotalNom.Value = row["TotalNom"];
                    ViewObject.TotalCpnInc.Value = row["TotalCpnInc"];
                    ViewObject.TotalDiffPCNom.Value = row["TotalDiffPCNom"];
                    ViewObject.TotalServCrdt.Value = row["TotalServCrdt"];
                    ViewObject.TotalCostServ.Value = row["TotalCostServ"];
                    IsLoadData = false;

                    var serviceEntity =
                        Workplace.ActiveScheme.RootPackage.FindEntityByName("4810b086-68c1-48a4-a4f8-290d7e0692fb");
                    using (IDataUpdater du = serviceEntity.GetDataUpdater(string.Format("RefReplaceIssFLn = {0}", Id), null))
                    {
                        var dt1 = new DataTable();
                        du.Fill(ref dt1);
                        CreditServicePlan = dt1.Copy();
                    }
                    ViewObject.CreditServisePlanGrid.DataSource = CreditServicePlan;
                }
            }
        }

        private void GetEmptyCreditDetailData()
        {
            var serviceEntity =
                        Workplace.ActiveScheme.RootPackage.FindEntityByName("4810b086-68c1-48a4-a4f8-290d7e0692fb");
            using (IDataUpdater du = serviceEntity.GetDataUpdater("1 = 2", null))
            {
                var dt1 = new DataTable();
                du.Fill(ref dt1);
                CreditServicePlan = dt1.Copy();
            }
        }

        private bool GetCalculationSaveName(ref string calculationName)
        {
            List<string> comments = new List<string>();
            ComboBoxTool cb = (ComboBoxTool)ViewObject.ToolbarsManager.Tools["Calculations"];
            foreach (var obj in cb.ValueList.ValueListItems)
            {
                comments.Add(obj.DisplayText.Split('(')[0].Trim());
            }
            return SelectCommentForm.ShowSaveCalcResultsForm(comments, ref calculationName);
        }

        private bool SaveData()
        {
            if (InvalidDataEditors.Count > 0)
            {
                MessageBox.Show("Сохранение расчета невозможно. Параметры расчета введены неверно", "Сохранение данных",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            string calculationName = string.Empty;
            if (!GetCalculationSaveName(ref calculationName))
                return false;
            Workplace.OperationObj.Text = "Сохранение данных";
            Workplace.OperationObj.StartOperation();
            try
            {
                Name = calculationName;
                DeleteData();
                LoanToCreditParams calculationParams = GetCalculationParams();
                IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_ReplaceIssForLn);
                long currentId = -1;
                using (IDataUpdater du = entity.GetDataUpdater("1 = 2", null))
                {
                    DataTable dt = new DataTable();
                    du.Fill(ref dt);
                    DataRow newRow = GetRow(dt,calculationName, calculationParams.CalcDate);
                    newRow.BeginEdit();
                    newRow["SourceID"] = FinSourcePlanningNavigation.Instance.CurrentSourceID;
                    newRow["TaskID"] = -1;
                    newRow["CapID"] = CapitalId;
                    newRow["CalcDate"] = calculationParams.CalcDate;
                    newRow["Name"] = calculationName;
                    newRow["OfficialNumber"] = calculationParams.OfficialNumber;
                    newRow["NumCpn"] = calculationParams.PaimentId;
                    newRow["StartCpnDate"] = calculationParams.StartCpnDate;
                    newRow["EndCpnDate"] = calculationParams.EndCpnDate;
                    newRow["RedemptionDate"] = calculationParams.RedemptionDate;
                    newRow["DateDischarge"] = calculationParams.DateDischarge;
                    newRow["Cpn"] = calculationParams.Cpn;
                    newRow["AI"] = calculationParams.AI;
                    newRow["Nom"] = calculationParams.Nom;
                    newRow["CP"] = calculationParams.   CP;
                    newRow["CPRub"] = calculationParams.CPRub;
                    newRow["DiffPCNom"] = calculationParams.DiffPCNom;
                    newRow["YTM"] = calculationParams.YTM;
                    newRow["CouponRate"] = calculationParams.CouponRate;
                    newRow["StartCrdtDate"] = calculationParams.StartCrdtDate;
                    newRow["EndCrdtDate"] = calculationParams.EndCrdtDate;
                    newRow["IsCalcRate"] = calculationParams.IsCalcRate;
                    newRow["CrdtRate"] = calculationParams.CrdtRate;
                    newRow["ServCrdt"] = calculationParams.ServCrdt;
                    newRow["CostServLn"] = calculationParams.CostServLn;
                    newRow["CostServCrdt"] = calculationParams.CostServCrdt;
                    newRow["IsCoverNom"] = calculationParams.CreditAttraction;
                    newRow["Count"] = calculationParams.Count;
                    newRow["CrdtSum"] = calculationParams.CrdtSum;
                    newRow["RepaymentSum"] = calculationParams.RepaymentSum;
                    newRow["TotalNom"] = calculationParams.TotalNom;
                    newRow["TotalCpnInc"] = calculationParams.TotalCpnInc;
                    newRow["TotalDiffPCNom"] = calculationParams.TotalDiffPCNom;
                    newRow["TotalServCrdt"] = calculationParams.TotalServCrdt;
                    newRow["TotalCostServ"] = calculationParams.TotalCostServ;
                    newRow.EndEdit();
                    if (newRow.RowState == DataRowState.Added || newRow.RowState == DataRowState.Detached)
                        dt.Rows.Add(newRow);
                    du.Update(ref dt);
                    dt.AcceptChanges();
                    currentId = Convert.ToInt64(dt.Rows[0]["ID"]);
                    Name = calculationName;
                }

                using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
                {
                    db.ExecQuery(string.Format("delete from t_S_ReIssForLnServ where RefReplaceIssFLn = {0}", currentId), 
                        QueryResultTypes.NonQuery);
                }
                var serviceEntity =
                        Workplace.ActiveScheme.RootPackage.FindEntityByName("4810b086-68c1-48a4-a4f8-290d7e0692fb");
                using (IDataUpdater du = serviceEntity.GetDataUpdater(string.Format("RefReplaceIssFLn = {0}", currentId), null))
                {
                    foreach (DataRow row in CreditServicePlan.Rows)
                    {
                        row["RefReplaceIssFLn"] = currentId;
                    }

                    var dt1 = CreditServicePlan.Copy();
                    du.Update(ref dt1);
                    CreditServicePlan = dt1.Copy();
                }

                AddCalculation(calculationName, calculationParams.CalcDate);
                Workplace.OperationObj.StopOperation();
                return true;
            }
            catch (Exception)
            {
                Workplace.OperationObj.StopOperation();
                throw;
            }
        }

        private DataRow GetRow(DataTable dtData, string calculationName, DateTime calculationDate)
        {
            DataRow[] rows = dtData.Select(string.Format("CalcDate = '{0}' and Name = '{1}'",
                                                         calculationDate, calculationName));
            if (rows.Length > 0)
                return rows[0];
            return dtData.NewRow();
        }

        private void DeleteData()
        {
            Workplace.OperationObj.Text = "Удаление данных";
            Workplace.OperationObj.StartOperation();
            try
            {
                LoanToCreditParams calculationParams = GetCalculationParams();
                using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
                {
                    db.ExecQuery("delete from t_S_ReIssForLnServ where RefReplaceIssFLn = ?",
                        QueryResultTypes.NonQuery, new DbParameterDescriptor("p0", Id));

                    db.ExecQuery("delete from f_S_ReplaceIssForLn where CalcDate = ? and Name = ?",
                                 QueryResultTypes.NonQuery,
                                 new DbParameterDescriptor("p0", calculationParams.CalcDate),
                                 new DbParameterDescriptor("p1", calculationParams.Name));
                }
                Workplace.OperationObj.StopOperation();
            }
            catch
            {
                Workplace.OperationObj.StopOperation();
                throw;
            }
        }

        private void DeleteCalculation()
        {
            // удаляем расчет из списка сохраненных расчетов
            ComboBoxTool cb = (ComboBoxTool)ViewObject.ToolbarsManager.Tools["Calculations"];
            if (cb.SelectedItem != null)
                cb.ValueList.ValueListItems.Remove(cb.SelectedItem);
            if (cb.ValueList.ValueListItems.Count > 0)
                cb.SelectedIndex = 0;
            else
            {
                cb.SelectedIndex = -1;
                SetDefaultValues();
                SetRedemptionDate(true);
            }
        }

        private void AddCalculation(string calculationName, DateTime calculationDate)
        {
            // добавляем сохраненный расчет в список расчетов
            ComboBoxTool cb = (ComboBoxTool)ViewObject.ToolbarsManager.Tools["Calculations"];
            string calculationKey = GetCalculationKey(calculationName, calculationDate);
            ValueListItem item = cb.ValueList.FindByDataValue(calculationKey);
            if (item == null)
                item = cb.ValueList.ValueListItems.Add(calculationKey, string.Format("{0} ({1})", calculationName,
                                                                         calculationDate.ToShortDateString()));
            cb.SelectedItem = item;
        }

        private string GetCalculationKey(string calculationName, DateTime calculationDate)
        {
            return string.Format("{0}_{1}", calculationName, calculationDate.ToShortDateString());
        }

        private void BurnSaveDataButton(bool burn)
        {
            InfragisticsHelper.BurnTool(ViewObject.ToolbarsManager.Tools["SaveCalculation"], burn);
        }

        private void BurnCalculationButton(bool burn)
        {
            InfragisticsHelper.BurnTool(ViewObject.ToolbarsManager.Tools["Calculate"], burn);
            ViewObject.ToolbarsManager.Tools["Calculate"].SharedProps.Enabled = burn;
        }

        #endregion

        private bool CalculationParamsValidation()
        {
            CleanWarnings();

            LoanToCreditParams calculationParams = GetCalculationParams();

            if (calculationParams.RedemptionDate.Year < 1980)
            {
                WarningList.Add(ViewObject.RedemptionDate.Name, new DataWarningNotifier(ViewObject.RedemptionDate, "Не заполнена дата выкупа"));
            }

            if (calculationParams.StartCpnDate.Year < 1980)
            {
                string message = string.Format(
                    "Дата выкупа не попадает ни в один купонный период облигационного займа '{0}'.{1}Проверьте правильность заполнения даты выкупа или детали «План выплаты дохода» в интерфейсе 'Ценные бумаги'",
                    CapitalOfficialNumber, Environment.NewLine);
                WarningList.Add(ViewObject.StartCpnDate.Name, new DataWarningNotifier(ViewObject.StartCpnDate, message));
            }

            if (calculationParams.DateDischarge.Year < 1980)
            {

                WarningList.Add(ViewObject.DateDischarge.Name, new DataWarningNotifier(ViewObject.DateDischarge, "Не заполнена дата погашения займа"));
            }

            if (calculationParams.StartCpnDate.Year >= 1980 && calculationParams.CouponRate == 0)
            {
                string message = string.Format(
                    "В «Журнале ставок процентов облигационного займа '{0}' не найдено ставки купонного дохода для данного купонного периода. {1}Проверьте правильность заполнения детали в интерфейсе 'Ценные бумаги'",
                    CapitalOfficialNumber, Environment.NewLine);
                WarningList.Add(ViewObject.CouponRate.Name, new DataWarningNotifier(ViewObject.CouponRate, message));
            }

            if (calculationParams.Nom == 0)
            {
                string message = string.Format(
                    "Проверьте правильность заполнения детали «План погашения номинальной стоимости» облигационного займа {0} в интерфейсе 'Ценные бумаги'. {1}Возможно деталь не заполнена или не заполнено значения поле 'Процент погашения номинальной стоимости' в записях детали",
                    CapitalOfficialNumber, Environment.NewLine);

                WarningList.Add(ViewObject.Nom.Name, new DataWarningNotifier(ViewObject.Nom, message));
            }

            if ((ViewObject.rb2.Checked || ViewObject.rbRub.Checked) &&
                calculationParams.YTM == 0)
            {
                WarningList.Add(ViewObject.YTM.Name, new DataWarningNotifier(ViewObject.YTM, "Не заполнена эффективная доходность к погашению"));
            }

            if (ViewObject.rb1.Checked && calculationParams.CP == 0)
            {
                WarningList.Add(ViewObject.CP.Name, new DataWarningNotifier(ViewObject.CP, "Не заполнена цена выкупа (без НКД), % от номинала"));
                WarningList.Add(ViewObject.CPRub.Name, new DataWarningNotifier(ViewObject.CPRub, "Не заполнена цена выкупа на 1 облигацию (без НКД)"));
            }

            if (ViewObject.IsCalcRate.Checked)
            {
                if (calculationParams.CostServCrdt == 0)
                    WarningList.Add(ViewObject.CostServCrdt.Name, new DataWarningNotifier(ViewObject.CostServCrdt, "Не заполнен кредит расхода на обслуживании 1 облигации"));
            }
            else if (calculationParams.CrdtRate == 0)
            {
                WarningList.Add(ViewObject.CrdtRate.Name, new DataWarningNotifier(ViewObject.CrdtRate, "Не заполнена ставка по кредиту"));
            }

            if (ViewObject.TabControl.ActiveTab == ViewObject.TabControl.Tabs[1])
            {
                if (ViewObject.rb3.Checked)
                {
                    if (ViewObject.IsCalcFrmRSum.Checked)
                    {
                        if (calculationParams.RepaymentSum == 0)
                        {
                            WarningList.Add(ViewObject.RepaymentSum.Name, new DataWarningNotifier(ViewObject.RepaymentSum, "Не заполнены денежные средства на выкуп"));
                        }
                    }
                    else
                    {
                        if (calculationParams.CrdtSum == 0)
                        {
                            WarningList.Add(ViewObject.CrdtSum.Name, new DataWarningNotifier(ViewObject.CrdtSum, "Не заполнен размер кредита"));
                        }
                    }
                }
                else
                {
                    if (calculationParams.Count == 0)
                    {
                        WarningList.Add(ViewObject.Count.Name, new DataWarningNotifier(ViewObject.Count, "Не заполнено количество выкупаемых облигаций"));
                    }
                }

                long maxcapitalCount = CapitalOperationsServer.GetCapitalCount(CapitalId,
                                                                               calculationParams.RedemptionDate);
                if (ViewObject.rb4.Checked && maxcapitalCount < calculationParams.Count)
                {
                    WarningList.Add(ViewObject.Count.Name, new DataWarningNotifier(ViewObject.Count,
                                                                                   string.Format(
                                                                                       "Количество выкупаемых ценных бумаг не должно быть больше количества размещенных ценных бумаг ({0})",
                                                                                       maxcapitalCount)));
                }

                if (maxcapitalCount == 0)
                {
                    WarningList.Add(ViewObject.MaxCapitalsCount.Name,
                                    new DataWarningNotifier(ViewObject.MaxCapitalsCount,
                                                            "Проверьте правильность заполнения поля 'Количество ценных бумаг' в деталях 'Итоги размещения' и 'Факт погашения номинальной стоимости'"));
                }

                if (calculationParams.RedemptionDate >= EndPeriod || calculationParams.RedemptionDate <= StartPeriod)
                {
                    WarningList.Add(ViewObject.RedemptionDate.Name, new DataWarningNotifier(ViewObject.RedemptionDate,
                                                                                            string.Format(
                                                                                                "Дата выкупа должна входить в диапазон от {0} до {1} не включая границы интервала",
                                                                                                StartPeriod.
                                                                                                    ToShortDateString(),
                                                                                                EndPeriod.
                                                                                                    ToShortDateString())));
                }
            }

            CheckMaxEditorValue(ViewObject.CrdtSum);
            CheckMaxEditorValue(ViewObject.RepaymentSum);
            CheckMaxEditorValue(ViewObject.CostServCrdt, 1000);
            CheckMaxEditorValue(ViewObject.YTM, Convert.ToDecimal(99.9999));

            if (!ViewObject.IsCalcRate.Checked)
                CheckMaxEditorValue(ViewObject.CrdtRate, Convert.ToDecimal(100));

            return WarningList.Count == 0;
        }

    }
}
