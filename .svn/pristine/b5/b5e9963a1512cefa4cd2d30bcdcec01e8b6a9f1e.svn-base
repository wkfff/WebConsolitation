using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalOperations.DataWarning;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Reports;
using CalculatedValue = Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalPlanningOperations.CalculatedValue;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalOperations.Redemption
{
    public class RedemptionUI : BaseCapitalOperationsUI
    {
        public RedemptionUI(string key)
            : base(key)
        {
            Caption = "Выкуп облигаций";
        }

        protected override void SetViewCtrl()
        {
            fViewCtrl = new RedemptionView();
            fViewCtrl.ViewContent = this;
        }

        public RedemptionView ViewObject
        {
            get; set;
        }

        private DataTable Capitals
        {
            get; set;
        }

        private Int64 CapitalId
        {
            get;
            set;
        }

        private int PaimentId
        {
            get; set;
        }

        private string RedemptionDateFilter
        {
            get;
            set;
        }

        private bool IsLoadData
        {
            get;
            set;
        }

        private long Id
        {
            get;
            set;
        }

        private string CapitalOfficialNumber
        {
            get;
            set;
        }

        private DateTime EndCoupunDate
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

        private decimal CouponSum
        {
            get; set;
        }

        private decimal NKD
        {
            get; set;
        }

        public override void Initialize()
        {
            base.Initialize();

            ViewObject = (RedemptionView)fViewCtrl;

            ViewObject.ToolbarsManager.ToolValueChanged += ToolbarsManager_ToolValueChanged;
            ViewObject.ToolbarsManager.ToolClick += ToolbarsManager_ToolClick;

            ViewObject.RedemptionDate.ValueChanged += RedemptionDate_ValueChanged;
            ViewObject.Nom.ValueChanged += Nom_ValueChanged;
            ViewObject.CP.ValueChanged += CP_ValueChanged;
            ViewObject.CPRub.ValueChanged += CPRub_ValueChanged;
            ViewObject.TotalCount.ValueChanged += TotalCount_ValueChanged;
            ViewObject.TotalCount.MouseEnterElement += Count_MouseEnterElement;
            ViewObject.TotalCount.MouseLeave += Count_MouseLeave;

            ViewObject.rb1.CheckedChanged += rb1_CheckedChanged;
            ViewObject.rb3.CheckedChanged += rb3_CheckedChanged;
            ViewObject.rbRub.CheckedChanged += rb1_CheckedChanged;

            ViewObject.Economy.ValueChanged += Economy_ValueChanged;

            ViewObject.TotalCount.ValueChanged += ValueChanged;
            ViewObject.TotalSum.ValueChanged += ValueChanged;

            ViewObject.IsCalcCP.CheckedChanged += IsCalcCP_CheckedChanged;

            #region тултипы

            ViewObject.RedemptionDate.MouseEnterElement += RedemptionDate_MouseEnterElement;
            ViewObject.RedemptionDate.MouseLeave += RedemptionDate_MouseLeave;

            #endregion

            SetEditorCheck(ViewObject.TotalSum);
            SetEditorCheck(ViewObject.TotalNom);
            SetEditorCheck(ViewObject.TotalDiffPCNom);
            SetEditorCheck(ViewObject.TotalAI);
            SetEditorCheck(ViewObject.TotalCostServLn);
            SetEditorCheck(ViewObject.TotalCpn);
            SetEditorCheck(ViewObject.YTM);
            SetEditorCheck(ViewObject.CPRub);
            SetEditorCheck(ViewObject.CP);
            SetEditorCheck(ViewObject.CostServLn);

            ViewObject.rb3.Checked = true;
            ViewObject.rb1.Checked = true;

            FillRegNumData();
            SetCalculations();

            bool enableCalculation = CheckCalculatePermission();
            BurnCalculationButton(enableCalculation);
        }

        void Nom_ValueChanged(object sender, EventArgs e)
        {
            var editor = sender as UltraNumericEditor;
            if (editor == null)
                return;
            if (editor.Value == DBNull.Value || editor.Value == null)
                return;
            decimal rubMaxValue = Convert.ToDecimal(editor.Value) * 2;
            ViewObject.CPRub.Tag = rubMaxValue;
            ViewObject.CP.Tag = 200;//rubMaxValue/10;
            //ViewObject.CPRub.MaxValue = rubMaxValue;
        }

        static void ValueChanged(object sender, EventArgs e)
        {
            var editor = sender as UltraNumericEditor;
            if (editor == null)
                return;
            if (editor.Value == DBNull.Value || editor.Value == null)
                editor.Value = 0;
        }

        #region обработчики тулбара и других компонент интерфейса

        void RedemptionDate_ValueChanged(object sender, EventArgs e)
        {
            if (ViewObject.RedemptionDate.Value == null)
            {
                ViewObject.MaxCapitalsCount.Value = null;
                return;
            }
            DateTime redemptionDate = Convert.ToDateTime(ViewObject.RedemptionDate.Value);

            if (ViewObject.RedemptionDate.Value != null && ViewObject.RedemptionDate.Value != DBNull.Value)
            {
                if (redemptionDate.Year >= 1980)
                {
                    long maxcapitalCount = CapitalOperationsServer.GetCapitalCount(CapitalId,
                                                                                   redemptionDate);
                    ViewObject.MaxCapitalsCount.Value = maxcapitalCount;
                }
            }

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
                        /*
                        query = @"select Coupon from t_S_CPJournalPercent where RefCap = ? and ChargeDate = 
                            (select max(ChargeDate) from t_S_CPJournalPercent where RefCap = ? and ChargeDate <= ?)";
                        object coupon = db.ExecQuery(query, QueryResultTypes.Scalar,
                                                     new DbParameterDescriptor("p0", CapitalId),
                                                     new DbParameterDescriptor("p1", CapitalId),
                                                     new DbParameterDescriptor("p2", EndCoupunDate));
                        if (coupon != null && coupon != DBNull.Value)
                        {
                            ViewObject.Cpn.Value = coupon;
                            ViewObject.CostServLn.Value = Math.Round(Convert.ToDecimal(coupon), 2, MidpointRounding.ToEven);
                        }*/
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
                    nom = 1000 - (1000 * Convert.ToDecimal(percentSum) / 100);
                }
                else
                {
                    nom = 1000;
                }
                ViewObject.Nom.Value = Math.Round(Convert.ToDecimal(nom), 2, MidpointRounding.AwayFromZero);

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
            }
        }

        void Economy_ValueChanged(object sender, EventArgs e)
        {
            if (ViewObject.Economy.Value != null && ViewObject.Economy.Value != DBNull.Value)
            {
                RedeptionParams calcParams = GetCalculationParams();
                if (calcParams.Economy != 0)
                    BurnEconomy(true, calcParams.Economy > 0);
                else
                    BurnEconomy(false, false);
            }
            else
                BurnEconomy(false, false);
        }

        void CP_ValueChanged(object sender, EventArgs e)
        {
            if (ViewObject.CP.ReadOnly)
                return;
            object value = ViewObject.CP.Value;
            if (value == DBNull.Value || value == null)
                ViewObject.CP.Value = 0;
            if (ViewObject.CP.Value != DBNull.Value)
            {
                RedeptionParams calcParams = GetCalculationParams();
                if (calcParams.Cpn != 0)
                    ViewObject.CPRub.Value = Math.Round((calcParams.CP * calcParams.Nom) / 100, 2, MidpointRounding.AwayFromZero);
            }
        }

        void CPRub_ValueChanged(object sender, EventArgs e)
        {
            if (ViewObject.CPRub.ReadOnly)
                return;
            object value = ViewObject.CPRub.Value;
            if (value == DBNull.Value || value == null)
                ViewObject.CPRub.Value = 0;
            if (ViewObject.CPRub.Value != DBNull.Value)
            {
                RedeptionParams calcParams = GetCalculationParams();
                if (calcParams.Cpn != 0 && calcParams.Nom != 0)
                    ViewObject.CP.Value = Math.Round((calcParams.CPRub * 100) / calcParams.Nom, 3, MidpointRounding.AwayFromZero);
            }
        }

        void TotalCount_ValueChanged(object sender, EventArgs e)
        {
            BurnEditor(false, ViewObject.TotalCount);
            if (ViewObject.TotalCount.ReadOnly)
            {
                RedeptionParams calcParams = GetCalculationParams();
                if (calcParams.TotalCount > calcParams.MaxCapitalsCount)
                {
                    BurnEditor(true, ViewObject.TotalCount);
                }
            }
        }

        void rb3_CheckedChanged(object sender, EventArgs e)
        {
            if (ViewObject.rb3.Checked)
            {
                ViewObject.TotalCount.ReadOnly = true;
                ViewObject.TotalSum.ReadOnly = false;
                ViewObject.TotalSum.Appearance.ResetBackColor();
            }
            else
            {
                ViewObject.TotalCount.ReadOnly = false;
                ViewObject.TotalCount.Appearance.ResetBackColor();
                ViewObject.TotalSum.ReadOnly = true;
            }
        }

        void rb1_CheckedChanged(object sender, EventArgs e)
        {
            if (ViewObject.rb1.Checked)
            {
                ViewObject.YTM.ReadOnly = true;
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
                ViewObject.CPRub.ReadOnly = true;
            }
        }

        void IsCalcCP_CheckedChanged(object sender, EventArgs e)
        {
            RedeptionParams calculationParams = GetCalculationParams();
            if (ViewObject.IsCalcCP.Checked)
            {
                ViewObject.CostServLn.ReadOnly = false;
                ViewObject.CP.ReadOnly = true;
                ViewObject.CPRub.ReadOnly = true;
                ViewObject.rb2.Enabled = false;
                ViewObject.rbRub.Enabled = false;
                ViewObject.CostServLn.Value = calculationParams.Cpn;
            }
            else
            {
                ViewObject.CostServLn.ReadOnly = true;
                ViewObject.CP.ReadOnly = false;
                ViewObject.CPRub.ReadOnly = false;
                ViewObject.rb2.Enabled = true;
                ViewObject.rbRub.Enabled = true;
                ViewObject.CostServLn.Value = 0;
            }
        }

        void ToolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            string toolKey = e.Tool.Key;
            switch (toolKey)
            {
                case "NewCalculation":
                    SetDefaultValues();
                    CleanWarnings();
                    ComboBoxTool cb = (ComboBoxTool)e.Tool.ToolbarsManager.Tools["Calculations"];
                    cb.SelectedIndex = -1;
                    SetRedemptionDate();
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
                    SetRedemptionDate();
                    if (!CalculationParamsValidation())
                    {
                        MessageBox.Show("Не все параметры заполнены для расчета или заполнены не корректно", "Расчет", MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        return;
                    }
                    Calculate();
                    BurnSaveDataButton(true);
                    break;
                case "CreateReport":
                    if (Id > 0)
                    {
                        var reportCommand = new ReportMOCapitalRedemptionCommand
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
                case "OfficialNumber":
                    SetDefaultValues();
                    if (cb.SelectedIndex == -1)
                    {
                        return;
                    }
                    CapitalId = Convert.ToInt64(cb.ValueList.ValueListItems[cb.SelectedIndex].DataValue);
                    DataRow capitalRow = Capitals.Select(string.Format("ID = {0}", CapitalId))[0];
                    if (capitalRow.IsNull(2) || capitalRow.IsNull(3))
                        break;
                    CapitalOfficialNumber = capitalRow[1].ToString();
                    StartPeriod = Convert.ToDateTime(capitalRow[2]);
                    EndPeriod = Convert.ToDateTime(capitalRow[3]);
                    RedemptionDateFilter =
                        string.Format("Дата выкупа должна входить в диапазон от {0} до {1} не включая границы интервала",
                                     Convert.ToDateTime(capitalRow[2]).ToShortDateString(), Convert.ToDateTime(capitalRow[3]).ToShortDateString());
                    SetRedemptionDate();
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

        #endregion

        #region параметры и расчет

        private void SetDefaultValues()
        {
            IsLoadData = true;

            ViewObject.StartCpnDate.Value = null;
            ViewObject.RedemptionDate.Value = DateTime.Today;
            ViewObject.CouponRate.Value = 0;
            ViewObject.Nom.Value = 0;
            ViewObject.Cpn.Value = 0;
            ViewObject.AI.Value = 0;
            ViewObject.YTM.Value = 0;
            ViewObject.CP.Value = 0;
            ViewObject.CPRub.Value = 0;
            ViewObject.DiffPCNom.Value = 0;

            ViewObject.CostServLn.Value = 0;
            ViewObject.TotalNom.Value = 0;
            ViewObject.Cpn.Value = 0;

            ViewObject.TotalCount.Value = 0;
            ViewObject.TotalSum.Value = 0;
            ViewObject.TotalNom.Value = 0;
            ViewObject.TotalDiffPCNom.Value = 0;
            ViewObject.TotalCostServLn.Value = 0;
            ViewObject.TotalAI.Value = 0;

            ViewObject.TotalCpn.Value = 0;
            ViewObject.Economy.Value = 0;

            ViewObject.rb1.Checked = true;
            ViewObject.rb3.Checked = true;

            ViewObject.IsCalcCP.Checked = false;

            IsLoadData = false;
        }

        private RedeptionParams GetCalculationParams()
        {
            RedeptionParams calculationParams = new RedeptionParams();
            calculationParams.CalcDate = CalculationDate;
            calculationParams.OfficialNumber = CapitalOfficialNumber;
            calculationParams.Name = Name;
            if (ViewObject.StartCpnDate.Value != null)
                calculationParams.StartCpnDate = Convert.ToDateTime(ViewObject.StartCpnDate.Value);
            calculationParams.RedemptionDate = Convert.ToDateTime(ViewObject.RedemptionDate.Value);
            calculationParams.CapitalId = CapitalId;
            calculationParams.PaimentId = PaimentId;
            calculationParams.EndCpnDate = EndCoupunDate;
            calculationParams.CouponRate = Convert.ToDecimal(ViewObject.CouponRate.Value);
            calculationParams.Cpn = Convert.ToDecimal(ViewObject.Cpn.Value);
            calculationParams.AI = Convert.ToDecimal(ViewObject.AI.Value);
            calculationParams.Nom = Convert.ToDecimal(ViewObject.Nom.Value);
            calculationParams.YTM = Convert.ToDecimal(ViewObject.YTM.Value);
            calculationParams.CPRub = Convert.ToDecimal(ViewObject.CPRub.Value);
            calculationParams.CP = Convert.ToDecimal(ViewObject.CP.Value);
            calculationParams.DiffPCNom = ViewObject.DiffPCNom.Value != null ? Convert.ToDecimal(ViewObject.DiffPCNom.Value) : 0;
            calculationParams.Cpn = Convert.ToDecimal(ViewObject.Cpn.Value);
            calculationParams.CostServLn = Convert.ToDecimal(ViewObject.CostServLn.Value);
            calculationParams.TotalSum = Convert.ToDecimal(ViewObject.TotalSum.Value);
            calculationParams.TotalNom = Convert.ToDecimal(ViewObject.TotalNom.Value);
            calculationParams.TotalCount = Convert.ToInt64(ViewObject.TotalCount.Value);
            calculationParams.TotalDiffPCNom = Convert.ToDecimal(ViewObject.TotalDiffPCNom.Value);
            calculationParams.TotalCostServLn = Convert.ToDecimal(ViewObject.TotalCostServLn.Value);
            calculationParams.TotalAI = Convert.ToDecimal(ViewObject.TotalAI.Value);
            calculationParams.TotalCpn = Convert.ToDecimal(ViewObject.TotalCpn.Value);
            calculationParams.Economy = Convert.ToDecimal(ViewObject.Economy.Value);
            calculationParams.MaxCapitalsCount = ViewObject.MaxCapitalsCount.Value != null
                                                     ? Convert.ToInt64(ViewObject.MaxCapitalsCount.Value)
                                                     : 0;

            calculationParams.IsCalcCP = ViewObject.IsCalcCP.Checked;

            return calculationParams;
        }

        private bool Calculate()
        {
            CalculationDate = DateTime.Today;
            object redemptionDate = ViewObject.RedemptionDate.Value;
            if (redemptionDate == DBNull.Value || redemptionDate == null)
                return false;

            RedeptionParams calculationParams = GetCalculationParams();
            if (calculationParams.CouponRate == 0 || calculationParams.Nom == 0 ||
                calculationParams.Cpn == 0 || calculationParams.AI == 0)
            {
                return false;
            }

            RedeptionService calcService = new RedeptionService(Workplace.ActiveScheme);
            string messages = string.Empty;
            if (!calcService.CheckDetails(CapitalId, CapitalOfficialNumber, ref messages))
            {
                MessageBox.Show(messages, "Расчет", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (calculationParams.IsCalcCP)
            {
                ViewObject.DiffPCNom.Value =
                    Math.Round(calculationParams.CostServLn - calculationParams.AI, 2, MidpointRounding.AwayFromZero);

                calculationParams = GetCalculationParams();

                decimal cpRub =
                        Math.Round(calculationParams.Nom + calculationParams.DiffPCNom, 3, MidpointRounding.AwayFromZero);
                ViewObject.CPRub.Value = Math.Round(cpRub, 2, MidpointRounding.AwayFromZero);
                ViewObject.CP.Value =
                    Math.Abs(Math.Round((cpRub / calculationParams.Nom) * 100, 3, MidpointRounding.AwayFromZero));

                if (ViewObject.rb1.Checked)
                {
                    calculationParams = GetCalculationParams();
                    cpRub = Math.Round(calculationParams.Nom * calculationParams.CP / 100, 4, MidpointRounding.AwayFromZero);
                    ViewObject.YTM.Value =
                        Math.Round(CapitalOperationsServer.Calculate(calculationParams.RedemptionDate, CapitalId,
                        cpRub + calculationParams.AI, CalculatedValue.YTM), 4, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                if (ViewObject.rb1.Checked)
                {
                    decimal cpRub = (calculationParams.Nom * calculationParams.CP / 100) + NKD;
                    ViewObject.YTM.Value =
                        Math.Round(CapitalOperationsServer.Calculate(calculationParams.RedemptionDate, CapitalId,
                        cpRub, CalculatedValue.YTM), 4, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decimal d = Math.Round(CapitalOperationsServer.Calculate(calculationParams.RedemptionDate, CapitalId, calculationParams.YTM,
                            CalculatedValue.CurrPrice), 4, MidpointRounding.AwayFromZero);
                    decimal cpRub = d - calculationParams.AI;
                    ViewObject.CPRub.Value = cpRub;
                    ViewObject.CP.Value =
                        Math.Abs(Math.Round((cpRub / calculationParams.Nom) * 100, 4, MidpointRounding.AwayFromZero));
                }
                calculationParams = GetCalculationParams();
                ViewObject.DiffPCNom.Value =
                    Math.Round(calculationParams.CPRub - calculationParams.Nom, 2, MidpointRounding.AwayFromZero);
                calculationParams = GetCalculationParams();
                ViewObject.CostServLn.Value =
                    Math.Round(calculationParams.DiffPCNom + calculationParams.AI, 2, MidpointRounding.AwayFromZero);
            }

            calculationParams = GetCalculationParams();
            if (ViewObject.rb3.Checked)
            {
                ViewObject.TotalCount.Value = Math.Truncate(calculationParams.TotalSum/
                                              (calculationParams.CPRub + calculationParams.AI));
            }
            else
            {
                ViewObject.TotalSum.Value = Math.Round(calculationParams.TotalCount*
                                            (calculationParams.CPRub + calculationParams.AI), 0, MidpointRounding.AwayFromZero);
            }
            calculationParams = GetCalculationParams();
            ViewObject.TotalNom.Value = Math.Round(calculationParams.Nom * calculationParams.TotalCount, 0, MidpointRounding.AwayFromZero);
            calculationParams = GetCalculationParams();
            ViewObject.TotalDiffPCNom.Value = Math.Round(calculationParams.DiffPCNom * calculationParams.TotalCount, 0, MidpointRounding.AwayFromZero);
            ViewObject.TotalAI.Value = Math.Round(calculationParams.AI * calculationParams.TotalCount, 0, MidpointRounding.AwayFromZero);
            ViewObject.TotalCostServLn.Value = Math.Round(calculationParams.CostServLn * calculationParams.TotalCount, 0, MidpointRounding.AwayFromZero);

            ViewObject.TotalCpn.Value = Math.Round((calculationParams.Cpn + CouponSum) * calculationParams.TotalCount, 0, MidpointRounding.AwayFromZero);

            calculationParams = GetCalculationParams();
            ViewObject.Economy.Value = Math.Round(calculationParams.TotalCpn - calculationParams.TotalCostServLn, 0, MidpointRounding.AwayFromZero);

            return true;
        }

        #endregion

        #region загрузка данных

        private void SetCalculations()
        {
            List<Calculation> calculations = GetCalculationsList();
            ComboBoxTool cb = (ComboBoxTool)ViewObject.ToolbarsManager.Tools["Calculations"];
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
            ComboBoxTool cb = (ComboBoxTool)ViewObject.ToolbarsManager.Tools["OfficialNumber"];
            cb.SelectedItem = cb.ValueList.FindByDataValue(capitalId);
        }

        private void LoadData(string calculationKey)
        {
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                string calcName = string.Empty;
                DateTime calcDate = DateTime.MinValue;
                GetCalculationUniqueParams(calculationKey, ref calcDate, ref calcName);
                DataTable dt = (DataTable)db.ExecQuery("select * from f_S_BondBuyback where Name = ? and CalcDate = ?", QueryResultTypes.DataTable,
                             new DbParameterDescriptor("p0", calcName),
                             new DbParameterDescriptor("p1", calcDate));
                if (dt.Rows.Count > 0)
                {
                    IsLoadData = true;
                    DataRow row = dt.Rows[0];
                    Id = Convert.ToInt64(row["Id"]);
                    Name = row["Name"].ToString();
                    CalculationDate = Convert.ToDateTime(row["CalcDate"]);
                    CapitalId = Convert.ToInt64(row["CapId"]);
                    SetCapital(CapitalId);
                    PaimentId = Convert.ToInt32(row["NumCpn"]);
                    ViewObject.StartCpnDate.Value = row["StartCpnDate"];
                    ViewObject.RedemptionDate.Value = row["RedemptionDate"];
                    ViewObject.CouponRate.Value = row["CouponRate"];
                    ViewObject.Cpn.Value = row["Cpn"];
                    ViewObject.AI.Value = row["AI"];
                    ViewObject.Nom.Value = row["Nom"];
                    ViewObject.CP.Value = row["CP"];
                    ViewObject.CPRub.Value = row["CPRub"];
                    ViewObject.DiffPCNom.Value = row["DiffPCNom"];
                    ViewObject.YTM.Value = row["YTM"];
                    ViewObject.CostServLn.Value = row["CostServLn"];
                    ViewObject.TotalCount.Value = row["TotalCount"];
                    ViewObject.TotalSum.Value = row["TotalSum"];
                    ViewObject.TotalNom.Value = row["TotalNom"];
                    ViewObject.TotalDiffPCNom.Value = row["TotalDiffPCNom"];
                    ViewObject.TotalCostServLn.Value = row["TotalCostServLn"];
                    ViewObject.TotalAI.Value = row["TotalAI"];
                    ViewObject.TotalCpn.Value = row["TotalCpn"];
                    ViewObject.Economy.Value = row["Economy"];
                    ViewObject.IsCalcCP.Checked = Convert.ToBoolean(row["IsCalcCP"]);
                    IsLoadData = false;
                }
            }
        }

        private void FillRegNumData()
        {
            ComboBoxTool cb = (ComboBoxTool)ViewObject.ToolbarsManager.Tools["OfficialNumber"];
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

        private void SetRedemptionDate()
        {
            // заполнение данными после изменения даты погашения
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
                        query = @"select Coupon from t_S_CPJournalPercent where RefCap = ? and ChargeDate = 
                            (select max(ChargeDate) from t_S_CPJournalPercent where RefCap = ? and ChargeDate <= ?)";
                        object coupon = db.ExecQuery(query, QueryResultTypes.Scalar,
                                                     new DbParameterDescriptor("p0", CapitalId),
                                                     new DbParameterDescriptor("p1", CapitalId),
                                                     new DbParameterDescriptor("p2", EndCoupunDate));
                        if (coupon != null && coupon != DBNull.Value)
                        {
                            ViewObject.Cpn.Value = coupon;
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
                    ViewObject.CouponRate.Value = Math.Round(Convert.ToDecimal(couponRate), 2, MidpointRounding.ToEven);

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
                    nom = 1000 - (1000 * Convert.ToDecimal(percentSum) / 100);
                }
                else
                {
                    nom = 1000;
                }
                ViewObject.Nom.Value = Math.Round(Convert.ToDecimal(nom), 2, MidpointRounding.ToEven);
                //ViewObject.CPRub.MaxValue = Convert.ToDecimal(nom) * 2;
                if ((couponRate != null && couponRate != DBNull.Value) &&
                   (startcpnDate != null && startcpnDate != DBNull.Value))
                {
                    DateTime startCouponDate = Convert.ToDateTime(startcpnDate);
                    decimal daysYearCount = decimal.Divide((redemptionDate - startCouponDate).Days, 365);
                    decimal nkd = Convert.ToDecimal(nom)*Convert.ToDecimal(couponRate)*daysYearCount/100;

                    /*decimal nkd = Convert.ToDecimal(nom) * Convert.ToDecimal(couponRate) *
                        (redemptionDate - Convert.ToDateTime(startcpnDate)).Days / 365 / 100;*/
                    ViewObject.AI.Value = Math.Round(nkd, 2, MidpointRounding.AwayFromZero);
                    NKD = Math.Round(nkd, 4, MidpointRounding.AwayFromZero);
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

                query = "select Sum(Coupon) from t_S_CPJournalPercent where RefCap = ? and ChargeDate > ?";
                object couponSum = db.ExecQuery(query, QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", CapitalId),
                             new DbParameterDescriptor("p1", EndCoupunDate));
                if (couponSum != DBNull.Value && couponSum != null)
                {
                    CouponSum = Convert.ToDecimal(couponSum);
                }
                else
                {
                    CouponSum = 0;
                }
            }
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
                get;
                set;
            }

            internal DateTime CalculationDate
            {
                get;
                set;
            }

            internal string Comment
            {
                get;
                set;
            }
        }

        private List<Calculation> GetCalculationsList()
        {
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                List<Calculation> list = new List<Calculation>();
                string query = "select Distinct CalcDate, Name, id from f_S_BondBuyback order by CalcDate desc, name";
                DataTable dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
                foreach (DataRow row in dt.Rows)
                {
                    Calculation calculation = new Calculation(Convert.ToInt32(row[2]), Convert.ToDateTime(row[0]), row[1].ToString());
                    list.Add(calculation);
                }
                return list;
            }
        }

        private bool GetCalculationSaveName(ref string calculationName)
        {
            List<string> comments = new List<string>();
            ComboBoxTool cb = (ComboBoxTool)ViewObject.ToolbarsManager.Tools["Calculations"];
            foreach (var obj in cb.ValueList.ValueListItems)
            {
                comments.Add(obj.DisplayText.Split('(')[0]);
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
                RedeptionParams calculationParams = GetCalculationParams();
                IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_BondBuyback);
                using (IDataUpdater du = entity.GetDataUpdater())
                {
                    DataTable dt = new DataTable();
                    du.Fill(ref dt);
                    DataRow newRow = GetRow(dt, calculationName, calculationParams.CalcDate);
                    newRow.BeginEdit();
                    newRow["SourceID"] = FinSourcePlanningNavigation.Instance.CurrentSourceID;
                    newRow["TaskID"] = -1;
                    newRow["CapID"] = CapitalId;
                    newRow["CalcDate"] = DateTime.Today;
                    newRow["Name"] = calculationName;
                    newRow["OfficialNumber"] = calculationParams.OfficialNumber;
                    newRow["NumCpn"] = calculationParams.PaimentId;
                    newRow["StartCpnDate"] = calculationParams.StartCpnDate;

                    newRow["EndCpnDate"] = calculationParams.EndCpnDate;
                    newRow["RedemptionDate"] = calculationParams.RedemptionDate;
                    newRow["CouponRate"] = calculationParams.CouponRate;
                    newRow["Nom"] = calculationParams.Nom;
                    newRow["YTM"] = calculationParams.YTM;
                    newRow["CPRub"] = calculationParams.CPRub;
                    newRow["CP"] = calculationParams.CP;

                    newRow["DiffPCNom"] = calculationParams.DiffPCNom;
                    newRow["AI"] = calculationParams.AI;
                    newRow["CostServLn"] = calculationParams.CostServLn;
                    newRow["Cpn"] = calculationParams.Cpn;

                    newRow["TotalCount"] = calculationParams.TotalCount;
                    newRow["TotalSum"] = calculationParams.TotalSum;
                    newRow["TotalNom"] = calculationParams.TotalNom;
                    newRow["TotalDiffPCNom"] = calculationParams.TotalDiffPCNom;
                    newRow["TotalAI"] = calculationParams.TotalAI;

                    newRow["TotalCostServLn"] = calculationParams.TotalCostServLn;
                    newRow["TotalCpn"] = calculationParams.TotalCpn;
                    newRow["Economy"] = calculationParams.Economy;

                    newRow["IsCalcCP"] = calculationParams.IsCalcCP;

                    newRow.EndEdit();
                    if (newRow.RowState == DataRowState.Added || newRow.RowState == DataRowState.Detached)
                        dt.Rows.Add(newRow);
                    du.Update(ref dt);
                    dt.AcceptChanges();
                    Name = calculationName;
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
                RedeptionParams calculationParams = GetCalculationParams();
                using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
                {
                    db.ExecQuery("delete from f_S_BondBuyback where CalcDate = ? and Name = ?",
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
                SetRedemptionDate();
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

        private void BurnEconomy(bool burn, bool goodEconomy)
        {
            Color color = goodEconomy
                              ? Color.FromKnownColor(KnownColor.Chartreuse)
                              : Color.FromKnownColor(KnownColor.Red);
            if (!burn)
            {
                ViewObject.Economy.Appearance.BackColor = Color.Empty;
                ViewObject.Economy.Appearance.BackColor2 = Color.Empty;
                ViewObject.Economy.Appearance.BackGradientStyle = GradientStyle.None;
            }
            else
            {
                ViewObject.Economy.Appearance.BackColor = Color.FromKnownColor(KnownColor.Control);
                ViewObject.Economy.Appearance.BackColor2 = color;
                ViewObject.Economy.Appearance.BackGradientStyle = GradientStyle.VerticalBump;
            }
        }

        #endregion

        private bool CalculationParamsValidation()
        {
            CleanWarnings();

            RedeptionParams calculationParams = GetCalculationParams();

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

            if (!ViewObject.IsCalcCP.Checked && ViewObject.rb1.Checked && calculationParams.CP == 0)
            {
                WarningList.Add(ViewObject.CP.Name, new DataWarningNotifier(ViewObject.CP, "Не заполнена цена выкупа (без НКД), % от номинала"));
                WarningList.Add(ViewObject.CPRub.Name, new DataWarningNotifier(ViewObject.CPRub, "Не заполнена цена выкупа на 1 облигацию (без НКД)"));
            }

            if (ViewObject.rb3.Checked)
            {
                if (calculationParams.TotalSum == 0)
                {
                    WarningList.Add(ViewObject.TotalSum.Name, new DataWarningNotifier(ViewObject.TotalSum, "Не заполнен объем временно свободных средств"));
                }
            }
            else
            {
                if (calculationParams.TotalCount == 0)
                {
                    WarningList.Add(ViewObject.TotalCount.Name, new DataWarningNotifier(ViewObject.TotalCount, "Не заполнено количество выкупаемых облигаций"));
                }
            }

            long maxcapitalCount = CapitalOperationsServer.GetCapitalCount(CapitalId, calculationParams.RedemptionDate);
            if (ViewObject.rb4.Checked && maxcapitalCount < calculationParams.TotalCount)
            {
                WarningList.Add(ViewObject.TotalCount.Name, new DataWarningNotifier(ViewObject.TotalCount,
                    string.Format("Количество выкупаемых ценных бумаг не должно быть больше количества размещенных ценных бумаг ({0})", maxcapitalCount)));
            }
            if (maxcapitalCount == 0)
            {
                WarningList.Add(ViewObject.MaxCapitalsCount.Name, new DataWarningNotifier(ViewObject.MaxCapitalsCount,
                    "Проверьте правильность заполнения поля 'Количество ценных бумаг' в деталях 'Итоги размещения' и 'Факт погашения номинальной стоимости'"));
            }

            if (calculationParams.RedemptionDate >= EndPeriod || calculationParams.RedemptionDate <= StartPeriod)
            {
                WarningList.Add(ViewObject.RedemptionDate.Name, new DataWarningNotifier(ViewObject.RedemptionDate,
                    string.Format("Дата выкупа должна входить в диапазон от {0} до {1} не включая границы интервала", StartPeriod.ToShortDateString(), EndPeriod.ToShortDateString())));
            }

            if (calculationParams.IsCalcCP && calculationParams.CostServLn == 0)
            {
                WarningList.Add(ViewObject.CostServLn.Name, new DataWarningNotifier(ViewObject.CostServLn, "Не заполнены итого расходы на обслуживание при выкупе 1 облигации"));
            }

            CheckMaxEditorValue(ViewObject.TotalSum);
            //CheckMaxEditorValue(ViewObject.YTM, Convert.ToDecimal(99.9999));
            if (!calculationParams.IsCalcCP)
            {
                CheckMaxEditorValue(ViewObject.CPRub, Convert.ToDecimal(2000));
                CheckMaxEditorValue(ViewObject.CP, Convert.ToDecimal(200));
            }
            else
                CheckMaxEditorValue(ViewObject.CostServLn, Convert.ToDecimal(99.99));

            return WarningList.Count == 0;
        }

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
            RedeptionParams calculationParams = GetCalculationParams();
            if (calculationParams.TotalCount <= calculationParams.MaxCapitalsCount)
                return;
            ToolTip.ToolTipText = string.Format("Количество выкупаемых ценных бумаг не должно быть больше количества размещенных ценных бумаг ({0})", calculationParams.MaxCapitalsCount);
            Point tooltipPos = new Point(ViewObject.TotalCount.UIElement.Rect.Left, ViewObject.TotalCount.UIElement.Rect.Bottom);
            ToolTip.Show(ViewObject.TotalCount.PointToScreen(tooltipPos));
        }

        void Count_MouseLeave(object sender, EventArgs e)
        {
            ToolTip.Hide();
        }

        #endregion

    }
}
