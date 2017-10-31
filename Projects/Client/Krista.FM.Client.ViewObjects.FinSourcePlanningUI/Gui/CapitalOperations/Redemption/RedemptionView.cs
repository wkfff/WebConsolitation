using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Client.ViewObjects.BaseViewObject;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalOperations.Redemption
{
    public class RedemptionView : BaseView
    {
        internal Infragistics.Win.UltraWinEditors.UltraDateTimeEditor RedemptionDate;
        private System.Windows.Forms.Label label3;
        internal Infragistics.Win.UltraWinEditors.UltraDateTimeEditor StartCpnDate;
        private System.Windows.Forms.Label label2;
        internal Infragistics.Win.UltraWinToolbars.UltraToolbarsManager ToolbarsManager;
        private System.ComponentModel.IContainer components;
        private Infragistics.Win.Misc.UltraPanel BaseView_Fill_Panel;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Top;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        internal Infragistics.Win.UltraWinEditors.UltraNumericEditor YTM;
        internal Infragistics.Win.UltraWinEditors.UltraNumericEditor CP;
        internal Infragistics.Win.UltraWinEditors.UltraNumericEditor CPRub;
        internal System.Windows.Forms.RadioButton rb1;
        internal System.Windows.Forms.RadioButton rb2;
        internal Infragistics.Win.UltraWinEditors.UltraNumericEditor Nom;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        internal Infragistics.Win.UltraWinEditors.UltraNumericEditor Cpn;
        internal Infragistics.Win.UltraWinEditors.UltraNumericEditor CostServLn;
        internal Infragistics.Win.UltraWinEditors.UltraNumericEditor AI;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        internal Infragistics.Win.UltraWinEditors.UltraNumericEditor TotalCount;
        internal Infragistics.Win.UltraWinEditors.UltraNumericEditor TotalSum;
        internal System.Windows.Forms.RadioButton rb3;
        internal System.Windows.Forms.RadioButton rb4;
        private System.Windows.Forms.GroupBox groupBox1;
        internal Infragistics.Win.UltraWinEditors.UltraNumericEditor Economy;
        private System.Windows.Forms.Label label13;
        internal Infragistics.Win.UltraWinEditors.UltraNumericEditor TotalCpn;
        private System.Windows.Forms.Label label12;
        internal Infragistics.Win.UltraWinEditors.UltraNumericEditor TotalAI;
        private System.Windows.Forms.Label label11;
        internal Infragistics.Win.UltraWinEditors.UltraNumericEditor TotalCostServLn;
        private System.Windows.Forms.Label label10;
        internal Infragistics.Win.UltraWinEditors.UltraNumericEditor TotalDiffPCNom;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        internal Infragistics.Win.UltraWinEditors.UltraNumericEditor CouponRate;
        private System.Windows.Forms.Label label14;
        internal System.Windows.Forms.RadioButton rbRub;
        internal Infragistics.Win.UltraWinEditors.UltraNumericEditor TotalNom;
        internal Infragistics.Win.UltraWinEditors.UltraNumericEditor MaxCapitalsCount;
        private System.Windows.Forms.Label label1;
        internal Infragistics.Win.UltraWinEditors.UltraNumericEditor DiffPCNom;
        internal System.Windows.Forms.CheckBox IsCalcCP;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Bottom;

        public RedemptionView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("UltraToolbar1");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("NewCalculation");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SaveCalculation");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CreateReport");
            Infragistics.Win.UltraWinToolbars.ComboBoxTool comboBoxTool1 = new Infragistics.Win.UltraWinToolbars.ComboBoxTool("Calculations");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("DeleteCalculation");
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar2 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("UltraToolbar2");
            Infragistics.Win.UltraWinToolbars.ComboBoxTool comboBoxTool3 = new Infragistics.Win.UltraWinToolbars.ComboBoxTool("OfficialNumber");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Calculate");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("NewCalculation");
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RedemptionView));
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SaveCalculation");
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CreateReport");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ComboBoxTool comboBoxTool2 = new Infragistics.Win.UltraWinToolbars.ComboBoxTool("Calculations");
            Infragistics.Win.ValueList valueList1 = new Infragistics.Win.ValueList(0);
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("DeleteCalculation");
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ComboBoxTool comboBoxTool4 = new Infragistics.Win.UltraWinToolbars.ComboBoxTool("OfficialNumber");
            Infragistics.Win.ValueList valueList2 = new Infragistics.Win.ValueList(0);
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Calculate");
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            this.RedemptionDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.label3 = new System.Windows.Forms.Label();
            this.StartCpnDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.label2 = new System.Windows.Forms.Label();
            this.BaseView_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.IsCalcCP = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.MaxCapitalsCount = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.TotalCount = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.TotalSum = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.rb3 = new System.Windows.Forms.RadioButton();
            this.rb4 = new System.Windows.Forms.RadioButton();
            this.Economy = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.label13 = new System.Windows.Forms.Label();
            this.TotalCpn = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.label12 = new System.Windows.Forms.Label();
            this.TotalAI = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.label11 = new System.Windows.Forms.Label();
            this.TotalCostServLn = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.label10 = new System.Windows.Forms.Label();
            this.TotalDiffPCNom = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.label9 = new System.Windows.Forms.Label();
            this.TotalNom = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.Cpn = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.CostServLn = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.DiffPCNom = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.AI = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.CouponRate = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.Nom = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.label14 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.YTM = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.CP = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.CPRub = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.rb1 = new System.Windows.Forms.RadioButton();
            this.rbRub = new System.Windows.Forms.RadioButton();
            this.rb2 = new System.Windows.Forms.RadioButton();
            this._BaseView_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.ToolbarsManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._BaseView_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseView_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseView_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.RedemptionDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StartCpnDate)).BeginInit();
            this.BaseView_Fill_Panel.ClientArea.SuspendLayout();
            this.BaseView_Fill_Panel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MaxCapitalsCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TotalCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalSum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Economy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalCpn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalAI)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalCostServLn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalDiffPCNom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalNom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Cpn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CostServLn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiffPCNom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AI)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CouponRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Nom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.YTM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CPRub)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ToolbarsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // RedemptionDate
            // 
            this.RedemptionDate.Location = new System.Drawing.Point(322, 15);
            this.RedemptionDate.Name = "RedemptionDate";
            this.RedemptionDate.Size = new System.Drawing.Size(93, 21);
            this.RedemptionDate.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Дата выкупа";
            // 
            // StartCpnDate
            // 
            this.StartCpnDate.Location = new System.Drawing.Point(322, 43);
            this.StartCpnDate.Name = "StartCpnDate";
            this.StartCpnDate.ReadOnly = true;
            this.StartCpnDate.Size = new System.Drawing.Size(93, 21);
            this.StartCpnDate.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(13, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(227, 28);
            this.label2.TabIndex = 9;
            this.label2.Text = "Дата начала купонного периода, предшествующая дате выкупа";
            // 
            // BaseView_Fill_Panel
            // 
            this.BaseView_Fill_Panel.AlphaBlendMode = Infragistics.Win.AlphaBlendMode.Standard;
            // 
            // BaseView_Fill_Panel.ClientArea
            // 
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.IsCalcCP);
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.groupBox1);
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.label6);
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.label5);
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.label4);
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.label8);
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.Cpn);
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.CostServLn);
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.DiffPCNom);
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.AI);
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.CouponRate);
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.Nom);
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.label14);
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.label21);
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.ultraGroupBox2);
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.RedemptionDate);
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.label3);
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.StartCpnDate);
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.label2);
            this.BaseView_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.BaseView_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BaseView_Fill_Panel.Location = new System.Drawing.Point(0, 54);
            this.BaseView_Fill_Panel.Name = "BaseView_Fill_Panel";
            this.BaseView_Fill_Panel.Size = new System.Drawing.Size(895, 482);
            this.BaseView_Fill_Panel.TabIndex = 0;
            this.BaseView_Fill_Panel.PaintClient += new System.Windows.Forms.PaintEventHandler(this.BaseView_Fill_Panel_PaintClient);
            // 
            // IsCalcCP
            // 
            this.IsCalcCP.Location = new System.Drawing.Point(16, 133);
            this.IsCalcCP.Name = "IsCalcCP";
            this.IsCalcCP.Size = new System.Drawing.Size(402, 34);
            this.IsCalcCP.TabIndex = 36;
            this.IsCalcCP.Text = "Рассчитать цену выкупа (без НКД) по расходам на обслуживание при выкупе 1 облигац" +
                "ии";
            this.IsCalcCP.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.MaxCapitalsCount);
            this.groupBox1.Controls.Add(this.ultraGroupBox1);
            this.groupBox1.Controls.Add(this.Economy);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.TotalCpn);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.TotalAI);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.TotalCostServLn);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.TotalDiffPCNom);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.TotalNom);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(433, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(448, 370);
            this.groupBox1.TabIndex = 34;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Расчет на выкупаемый объем облигаций";
            // 
            // MaxCapitalsCount
            // 
            this.MaxCapitalsCount.Location = new System.Drawing.Point(308, 17);
            this.MaxCapitalsCount.MaskInput = "nnn,nnn,nnn,nnn,nnn";
            this.MaxCapitalsCount.MaxValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.MaxCapitalsCount.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.MaxCapitalsCount.Name = "MaxCapitalsCount";
            this.MaxCapitalsCount.Nullable = true;
            this.MaxCapitalsCount.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.MaxCapitalsCount.ReadOnly = true;
            this.MaxCapitalsCount.Size = new System.Drawing.Size(125, 21);
            this.MaxCapitalsCount.TabIndex = 9;
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.TotalCount);
            this.ultraGroupBox1.Controls.Add(this.TotalSum);
            this.ultraGroupBox1.Controls.Add(this.rb3);
            this.ultraGroupBox1.Controls.Add(this.rb4);
            this.ultraGroupBox1.Location = new System.Drawing.Point(16, 51);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(426, 68);
            this.ultraGroupBox1.TabIndex = 23;
            // 
            // TotalCount
            // 
            this.TotalCount.Location = new System.Drawing.Point(291, 9);
            this.TotalCount.MaskInput = "nnn,nnn,nnn,nnn,nnn";
            this.TotalCount.MaxValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.TotalCount.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.TotalCount.Name = "TotalCount";
            this.TotalCount.Nullable = true;
            this.TotalCount.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.TotalCount.Size = new System.Drawing.Size(125, 21);
            this.TotalCount.TabIndex = 9;
            // 
            // TotalSum
            // 
            this.TotalSum.Location = new System.Drawing.Point(291, 36);
            this.TotalSum.MaskInput = "-nnn,nnn,nnn,nnn,nnn";
            this.TotalSum.MaxValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.TotalSum.MinValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            -2147483648});
            this.TotalSum.Name = "TotalSum";
            this.TotalSum.Nullable = true;
            this.TotalSum.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.TotalSum.Size = new System.Drawing.Size(125, 21);
            this.TotalSum.TabIndex = 9;
            this.TotalSum.Tag = "999999999999";
            // 
            // rb3
            // 
            this.rb3.AutoSize = true;
            this.rb3.Location = new System.Drawing.Point(6, 11);
            this.rb3.Name = "rb3";
            this.rb3.Size = new System.Drawing.Size(229, 17);
            this.rb3.TabIndex = 0;
            this.rb3.Text = "Количество выкупаемых облигаций, шт.";
            this.rb3.UseVisualStyleBackColor = true;
            // 
            // rb4
            // 
            this.rb4.AutoSize = true;
            this.rb4.Location = new System.Drawing.Point(6, 38);
            this.rb4.Name = "rb4";
            this.rb4.Size = new System.Drawing.Size(241, 17);
            this.rb4.TabIndex = 0;
            this.rb4.Text = "Объем временно свободных средств, руб.";
            this.rb4.UseVisualStyleBackColor = true;
            // 
            // Economy
            // 
            this.Economy.Location = new System.Drawing.Point(307, 332);
            this.Economy.MaskInput = "-nnn,nnn,nnn,nnn,nnn";
            this.Economy.MaxValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.Economy.MinValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            -2147483648});
            this.Economy.Name = "Economy";
            this.Economy.Nullable = true;
            this.Economy.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.Economy.ReadOnly = true;
            this.Economy.Size = new System.Drawing.Size(125, 21);
            this.Economy.TabIndex = 25;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(13, 336);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(84, 13);
            this.label13.TabIndex = 33;
            this.label13.Text = "Экономия, руб.";
            // 
            // TotalCpn
            // 
            this.TotalCpn.Location = new System.Drawing.Point(307, 285);
            this.TotalCpn.MaskInput = "-nnn,nnn,nnn,nnn,nnn";
            this.TotalCpn.MaxValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.TotalCpn.MinValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            -2147483648});
            this.TotalCpn.Name = "TotalCpn";
            this.TotalCpn.Nullable = true;
            this.TotalCpn.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.TotalCpn.ReadOnly = true;
            this.TotalCpn.Size = new System.Drawing.Size(125, 21);
            this.TotalCpn.TabIndex = 25;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(13, 269);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(259, 59);
            this.label12.TabIndex = 33;
            this.label12.Text = "Запланированные расходы на выплату купонов (с даты начала купонного периода, пред" +
                "шествующей дате выкупа, по дату погашения займа), руб.";
            // 
            // TotalAI
            // 
            this.TotalAI.Location = new System.Drawing.Point(307, 208);
            this.TotalAI.MaskInput = "-nnn,nnn,nnn,nnn,nnn";
            this.TotalAI.MaxValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.TotalAI.MinValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            -2147483648});
            this.TotalAI.Name = "TotalAI";
            this.TotalAI.Nullable = true;
            this.TotalAI.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.TotalAI.ReadOnly = true;
            this.TotalAI.Size = new System.Drawing.Size(125, 21);
            this.TotalAI.TabIndex = 25;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(13, 212);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(137, 13);
            this.label11.TabIndex = 33;
            this.label11.Text = "НКД на дату выкупа, руб.";
            // 
            // TotalCostServLn
            // 
            this.TotalCostServLn.Location = new System.Drawing.Point(308, 239);
            this.TotalCostServLn.MaskInput = "-nnn,nnn,nnn,nnn,nnn";
            this.TotalCostServLn.MaxValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.TotalCostServLn.MinValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            -2147483648});
            this.TotalCostServLn.Name = "TotalCostServLn";
            this.TotalCostServLn.Nullable = true;
            this.TotalCostServLn.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.TotalCostServLn.ReadOnly = true;
            this.TotalCostServLn.Size = new System.Drawing.Size(125, 21);
            this.TotalCostServLn.TabIndex = 25;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(13, 235);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(216, 31);
            this.label10.TabIndex = 33;
            this.label10.Text = "Итого расходы на обслуживание при выкупе облигаций, руб.";
            // 
            // TotalDiffPCNom
            // 
            this.TotalDiffPCNom.Location = new System.Drawing.Point(308, 170);
            this.TotalDiffPCNom.MaskInput = "-nnn,nnn,nnn,nnn,nnn";
            this.TotalDiffPCNom.MaxValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.TotalDiffPCNom.MinValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            -2147483648});
            this.TotalDiffPCNom.Name = "TotalDiffPCNom";
            this.TotalDiffPCNom.Nullable = true;
            this.TotalDiffPCNom.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.TotalDiffPCNom.ReadOnly = true;
            this.TotalDiffPCNom.Size = new System.Drawing.Size(125, 21);
            this.TotalDiffPCNom.TabIndex = 25;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(13, 165);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(250, 44);
            this.label9.TabIndex = 33;
            this.label9.Text = "Разница между ценой выкупа (без НКД) и непогашенной частью номинальной стоимости " +
                "облигаций (+/-), руб. ";
            // 
            // TotalNom
            // 
            this.TotalNom.Location = new System.Drawing.Point(307, 132);
            this.TotalNom.MaskInput = "-nnn,nnn,nnn,nnn,nnn";
            this.TotalNom.MaxValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.TotalNom.MinValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            -2147483648});
            this.TotalNom.Name = "TotalNom";
            this.TotalNom.Nullable = true;
            this.TotalNom.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.TotalNom.ReadOnly = true;
            this.TotalNom.Size = new System.Drawing.Size(125, 21);
            this.TotalNom.TabIndex = 25;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(13, 129);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(247, 29);
            this.label7.TabIndex = 33;
            this.label7.Text = "Непогашенная часть  номинальной стоимости облигаций, руб.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(219, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Количество размещенных облигаций, шт.";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(13, 377);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(259, 49);
            this.label6.TabIndex = 33;
            this.label6.Text = "Запланированные расходы на выплату купона, рассчитанные на 1 облигацию за купонны" +
                "й период, в котором осуществляется выкуп, руб.";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(13, 342);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(216, 30);
            this.label5.TabIndex = 33;
            this.label5.Text = "Итого расходы на обслуживание при выкупе 1 облигации, руб.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 319);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(219, 13);
            this.label4.TabIndex = 33;
            this.label4.Text = "НКД на дату выкупа на 1 облигацию, руб.";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(13, 270);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(293, 46);
            this.label8.TabIndex = 31;
            this.label8.Text = "Разница между ценой выкупа (без НКД) и непогашенной частью номинальной стоимости " +
                "1 облигации (+/-), руб. ";
            // 
            // Cpn
            // 
            this.Cpn.Location = new System.Drawing.Point(353, 387);
            this.Cpn.MaskInput = "-nn,nnn.nn";
            this.Cpn.Name = "Cpn";
            this.Cpn.Nullable = true;
            this.Cpn.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.Cpn.ReadOnly = true;
            this.Cpn.Size = new System.Drawing.Size(65, 21);
            this.Cpn.TabIndex = 25;
            // 
            // CostServLn
            // 
            this.CostServLn.Location = new System.Drawing.Point(353, 347);
            this.CostServLn.MaskInput = "-nnn.nn";
            this.CostServLn.Name = "CostServLn";
            this.CostServLn.Nullable = true;
            this.CostServLn.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.CostServLn.ReadOnly = true;
            this.CostServLn.Size = new System.Drawing.Size(65, 21);
            this.CostServLn.TabIndex = 25;
            this.CostServLn.Tag = "99,99";
            // 
            // DiffPCNom
            // 
            this.DiffPCNom.Location = new System.Drawing.Point(353, 279);
            this.DiffPCNom.MaskInput = "-nn,nnn.nn";
            this.DiffPCNom.Name = "DiffPCNom";
            this.DiffPCNom.Nullable = true;
            this.DiffPCNom.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.DiffPCNom.ReadOnly = true;
            this.DiffPCNom.Size = new System.Drawing.Size(65, 21);
            this.DiffPCNom.TabIndex = 25;
            // 
            // AI
            // 
            this.AI.Location = new System.Drawing.Point(353, 315);
            this.AI.MaskInput = "nn,nnn.nn";
            this.AI.Name = "AI";
            this.AI.Nullable = true;
            this.AI.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.AI.ReadOnly = true;
            this.AI.Size = new System.Drawing.Size(65, 21);
            this.AI.TabIndex = 25;
            // 
            // CouponRate
            // 
            this.CouponRate.Location = new System.Drawing.Point(350, 71);
            this.CouponRate.MaskInput = "-nn,nnn.nn";
            this.CouponRate.Name = "CouponRate";
            this.CouponRate.Nullable = true;
            this.CouponRate.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.CouponRate.ReadOnly = true;
            this.CouponRate.Size = new System.Drawing.Size(65, 21);
            this.CouponRate.TabIndex = 25;
            // 
            // Nom
            // 
            this.Nom.Location = new System.Drawing.Point(350, 104);
            this.Nom.MaskInput = "-nn,nnn.nn";
            this.Nom.Name = "Nom";
            this.Nom.Nullable = true;
            this.Nom.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.Nom.ReadOnly = true;
            this.Nom.Size = new System.Drawing.Size(65, 21);
            this.Nom.TabIndex = 25;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(13, 75);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(195, 13);
            this.label14.TabIndex = 24;
            this.label14.Text = "Ставка купонного дохода, % годовых";
            // 
            // label21
            // 
            this.label21.Location = new System.Drawing.Point(13, 102);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(253, 28);
            this.label21.TabIndex = 24;
            this.label21.Text = "Непогашенная часть  номинальной стоимости облигаций, руб.";
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.Controls.Add(this.label16);
            this.ultraGroupBox2.Controls.Add(this.label15);
            this.ultraGroupBox2.Controls.Add(this.YTM);
            this.ultraGroupBox2.Controls.Add(this.CP);
            this.ultraGroupBox2.Controls.Add(this.CPRub);
            this.ultraGroupBox2.Controls.Add(this.rb1);
            this.ultraGroupBox2.Controls.Add(this.rbRub);
            this.ultraGroupBox2.Controls.Add(this.rb2);
            this.ultraGroupBox2.Location = new System.Drawing.Point(8, 172);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(419, 93);
            this.ultraGroupBox2.TabIndex = 23;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(23, 65);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(235, 13);
            this.label16.TabIndex = 11;
            this.label16.Text = "Цена выкупа на 1 облигацию (без НКД), руб.";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(23, 38);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(290, 13);
            this.label15.TabIndex = 10;
            this.label15.Text = "Цена выкупа на 1 облигацию (без НКД), % от номинала";
            // 
            // YTM
            // 
            this.YTM.Location = new System.Drawing.Point(345, 9);
            this.YTM.MaskInput = "-nnn.nnnn";
            this.YTM.Name = "YTM";
            this.YTM.Nullable = true;
            this.YTM.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.YTM.Size = new System.Drawing.Size(65, 21);
            this.YTM.TabIndex = 9;
            this.YTM.Tag = "99,9999";
            // 
            // CP
            // 
            this.CP.Location = new System.Drawing.Point(345, 36);
            this.CP.MaskInput = "nnn.nnnn";
            this.CP.MaxValue = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.CP.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.CP.Name = "CP";
            this.CP.Nullable = true;
            this.CP.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.CP.Size = new System.Drawing.Size(65, 21);
            this.CP.TabIndex = 9;
            this.CP.Tag = "2000";
            // 
            // CPRub
            // 
            appearance16.BackColor = System.Drawing.Color.White;
            this.CPRub.Appearance = appearance16;
            this.CPRub.BackColor = System.Drawing.Color.White;
            this.CPRub.Location = new System.Drawing.Point(345, 63);
            this.CPRub.MaskInput = "-n,nnn.nn";
            this.CPRub.MaxValue = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.CPRub.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.CPRub.Name = "CPRub";
            this.CPRub.Nullable = true;
            this.CPRub.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.CPRub.Size = new System.Drawing.Size(65, 21);
            this.CPRub.TabIndex = 9;
            this.CPRub.Tag = "2000";
            // 
            // rb1
            // 
            this.rb1.AutoSize = true;
            this.rb1.Location = new System.Drawing.Point(6, 11);
            this.rb1.Name = "rb1";
            this.rb1.Size = new System.Drawing.Size(284, 17);
            this.rb1.TabIndex = 0;
            this.rb1.Text = "Эффективная доходность к погашению, % годовых";
            this.rb1.UseVisualStyleBackColor = true;
            // 
            // rbRub
            // 
            this.rbRub.AutoSize = true;
            this.rbRub.Location = new System.Drawing.Point(6, 65);
            this.rbRub.Name = "rbRub";
            this.rbRub.Size = new System.Drawing.Size(14, 13);
            this.rbRub.TabIndex = 0;
            this.rbRub.UseVisualStyleBackColor = true;
            // 
            // rb2
            // 
            this.rb2.AutoSize = true;
            this.rb2.Location = new System.Drawing.Point(6, 38);
            this.rb2.Name = "rb2";
            this.rb2.Size = new System.Drawing.Size(14, 13);
            this.rb2.TabIndex = 0;
            this.rb2.UseVisualStyleBackColor = true;
            // 
            // _BaseView_Toolbars_Dock_Area_Left
            // 
            this._BaseView_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._BaseView_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 54);
            this._BaseView_Toolbars_Dock_Area_Left.Name = "_BaseView_Toolbars_Dock_Area_Left";
            this._BaseView_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 482);
            this._BaseView_Toolbars_Dock_Area_Left.ToolbarsManager = this.ToolbarsManager;
            // 
            // ToolbarsManager
            // 
            this.ToolbarsManager.DesignerFlags = 1;
            this.ToolbarsManager.DockWithinContainer = this;
            this.ToolbarsManager.ShowFullMenusDelay = 500;
            ultraToolbar1.DockedColumn = 0;
            ultraToolbar1.DockedRow = 0;
            ultraToolbar1.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool2,
            buttonTool3,
            comboBoxTool1,
            buttonTool4});
            ultraToolbar1.Text = "UltraToolbar1";
            ultraToolbar2.DockedColumn = 0;
            ultraToolbar2.DockedRow = 1;
            ultraToolbar2.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            comboBoxTool3,
            buttonTool9});
            ultraToolbar2.Text = "UltraToolbar2";
            this.ToolbarsManager.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1,
            ultraToolbar2});
            appearance11.Image = ((object)(resources.GetObject("appearance11.Image")));
            buttonTool5.SharedPropsInternal.AppearancesSmall.Appearance = appearance11;
            buttonTool5.SharedPropsInternal.Caption = "Новый расчет";
            appearance12.Image = ((object)(resources.GetObject("appearance12.Image")));
            buttonTool6.SharedPropsInternal.AppearancesSmall.Appearance = appearance12;
            buttonTool6.SharedPropsInternal.Caption = "Сохранить расчет";
            appearance13.Image = ((object)(resources.GetObject("appearance13.Image")));
            buttonTool7.SharedPropsInternal.AppearancesSmall.Appearance = appearance13;
            buttonTool7.SharedPropsInternal.Caption = "Создать отчет";
            comboBoxTool2.SharedPropsInternal.Caption = "Сохраненные расчеты";
            comboBoxTool2.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            comboBoxTool2.ValueList = valueList1;
            appearance14.Image = ((object)(resources.GetObject("appearance14.Image")));
            buttonTool8.SharedPropsInternal.AppearancesSmall.Appearance = appearance14;
            buttonTool8.SharedPropsInternal.Caption = "Удалить расчет";
            comboBoxTool4.SharedPropsInternal.Caption = "Государственный регистрационный номер";
            comboBoxTool4.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            valueList2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            comboBoxTool4.ValueList = valueList2;
            appearance15.Image = ((object)(resources.GetObject("appearance15.Image")));
            buttonTool10.SharedPropsInternal.AppearancesSmall.Appearance = appearance15;
            buttonTool10.SharedPropsInternal.Caption = "Расcчитать";
            buttonTool10.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            this.ToolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool5,
            buttonTool6,
            buttonTool7,
            comboBoxTool2,
            buttonTool8,
            comboBoxTool4,
            buttonTool10});
            // 
            // _BaseView_Toolbars_Dock_Area_Right
            // 
            this._BaseView_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._BaseView_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(895, 54);
            this._BaseView_Toolbars_Dock_Area_Right.Name = "_BaseView_Toolbars_Dock_Area_Right";
            this._BaseView_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 482);
            this._BaseView_Toolbars_Dock_Area_Right.ToolbarsManager = this.ToolbarsManager;
            // 
            // _BaseView_Toolbars_Dock_Area_Top
            // 
            this._BaseView_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._BaseView_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._BaseView_Toolbars_Dock_Area_Top.Name = "_BaseView_Toolbars_Dock_Area_Top";
            this._BaseView_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(895, 54);
            this._BaseView_Toolbars_Dock_Area_Top.ToolbarsManager = this.ToolbarsManager;
            // 
            // _BaseView_Toolbars_Dock_Area_Bottom
            // 
            this._BaseView_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._BaseView_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 536);
            this._BaseView_Toolbars_Dock_Area_Bottom.Name = "_BaseView_Toolbars_Dock_Area_Bottom";
            this._BaseView_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(895, 0);
            this._BaseView_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.ToolbarsManager;
            // 
            // RedemptionView
            // 
            this.Controls.Add(this.BaseView_Fill_Panel);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Bottom);
            this.Name = "RedemptionView";
            this.Size = new System.Drawing.Size(895, 536);
            ((System.ComponentModel.ISupportInitialize)(this.RedemptionDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StartCpnDate)).EndInit();
            this.BaseView_Fill_Panel.ClientArea.ResumeLayout(false);
            this.BaseView_Fill_Panel.ClientArea.PerformLayout();
            this.BaseView_Fill_Panel.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MaxCapitalsCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TotalCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalSum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Economy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalCpn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalAI)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalCostServLn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalDiffPCNom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalNom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Cpn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CostServLn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiffPCNom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AI)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CouponRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Nom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            this.ultraGroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.YTM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CPRub)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ToolbarsManager)).EndInit();
            this.ResumeLayout(false);

        }

        private void BaseView_Fill_Panel_PaintClient(object sender, System.Windows.Forms.PaintEventArgs e)
        {

        }
    }
}
