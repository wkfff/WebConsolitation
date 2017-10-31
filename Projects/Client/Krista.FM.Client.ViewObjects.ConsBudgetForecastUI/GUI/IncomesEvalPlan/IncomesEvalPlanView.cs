using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Client.ViewObjects.BaseViewObject;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.IncomesEvalPlan
{
    public class IncomesEvalPlanView : BaseView
    {
        internal Infragistics.Win.UltraWinToolbars.UltraToolbarsManager tbManager;
        private System.ComponentModel.IContainer components;
        private Infragistics.Win.Misc.UltraPanel BaseView_Fill_Panel;
        private System.Windows.Forms.Panel panel2;
        private Infragistics.Win.Misc.UltraSplitter ultraSplitter1;
        private System.Windows.Forms.Panel panel1;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Top;
        internal Infragistics.Win.UltraWinGrid.UltraGrid ugData;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        internal Infragistics.Win.UltraWinGrid.UltraCombo ucMunicipal;
        internal Infragistics.Win.UltraWinGrid.UltraCombo ucBudgetLevel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        internal Infragistics.Win.UltraWinEditors.UltraTextEditor uteVariant;
        internal Infragistics.Win.UltraWinEditors.UltraTextEditor uteAdministrator;
        internal Infragistics.Win.UltraWinEditors.UltraTextEditor uteIncomesSource;
        internal System.Windows.Forms.CheckBox cbForecast;
        internal System.Windows.Forms.CheckBox cbEstimate;
        internal System.Windows.Forms.CheckBox cbKmbResults;
        internal System.Windows.Forms.CheckBox cbTaxResource;
        internal Infragistics.Win.Misc.UltraButton btbKvsrClear;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Bottom;

        public IncomesEvalPlanView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IncomesEvalPlanView));
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance37 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance39 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance40 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance41 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance42 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance43 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance44 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance45 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance46 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance47 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance48 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton2 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance51 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton3 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance52 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("DataOperations");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("RefreshData");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SaveChanges");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CancelChanges");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SaveToExcel");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("LoadDataFromExcel");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CreateReport");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CopyVariant");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("RefreshData");
            Infragistics.Win.Appearance appearance53 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CancelChanges");
            Infragistics.Win.Appearance appearance54 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SaveChanges");
            Infragistics.Win.Appearance appearance55 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SaveToExcel");
            Infragistics.Win.Appearance appearance56 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("LoadDataFromExcel");
            Infragistics.Win.Appearance appearance57 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CreateReport");
            Infragistics.Win.Appearance appearance58 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CopyVariant");
            Infragistics.Win.Appearance appearance60 = new Infragistics.Win.Appearance();
            this.BaseView_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ugData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraSplitter1 = new Infragistics.Win.Misc.UltraSplitter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btbKvsrClear = new Infragistics.Win.Misc.UltraButton();
            this.cbKmbResults = new System.Windows.Forms.CheckBox();
            this.cbTaxResource = new System.Windows.Forms.CheckBox();
            this.cbForecast = new System.Windows.Forms.CheckBox();
            this.cbEstimate = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ucMunicipal = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.ucBudgetLevel = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.uteVariant = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.uteAdministrator = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.uteIncomesSource = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this._BaseView_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.tbManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._BaseView_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseView_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseView_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.BaseView_Fill_Panel.ClientArea.SuspendLayout();
            this.BaseView_Fill_Panel.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugData)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ucMunicipal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ucBudgetLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uteVariant)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uteAdministrator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uteIncomesSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbManager)).BeginInit();
            this.SuspendLayout();
            // 
            // BaseView_Fill_Panel
            // 
            // 
            // BaseView_Fill_Panel.ClientArea
            // 
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.panel2);
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.ultraSplitter1);
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.panel1);
            this.BaseView_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.BaseView_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BaseView_Fill_Panel.Location = new System.Drawing.Point(0, 27);
            this.BaseView_Fill_Panel.Name = "BaseView_Fill_Panel";
            this.BaseView_Fill_Panel.Size = new System.Drawing.Size(775, 509);
            this.BaseView_Fill_Panel.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.ugData);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 163);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(775, 346);
            this.panel2.TabIndex = 3;
            // 
            // ugData
            // 
            appearance13.BackColor = System.Drawing.SystemColors.Window;
            appearance13.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.ugData.DisplayLayout.Appearance = appearance13;
            this.ugData.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ugData.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance14.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance14.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance14.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance14.BorderColor = System.Drawing.SystemColors.Window;
            this.ugData.DisplayLayout.GroupByBox.Appearance = appearance14;
            appearance16.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ugData.DisplayLayout.GroupByBox.BandLabelAppearance = appearance16;
            this.ugData.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance15.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance15.BackColor2 = System.Drawing.SystemColors.Control;
            appearance15.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance15.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ugData.DisplayLayout.GroupByBox.PromptAppearance = appearance15;
            this.ugData.DisplayLayout.MaxColScrollRegions = 1;
            this.ugData.DisplayLayout.MaxRowScrollRegions = 1;
            appearance21.BackColor = System.Drawing.SystemColors.Window;
            appearance21.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ugData.DisplayLayout.Override.ActiveCellAppearance = appearance21;
            appearance17.BackColor = System.Drawing.SystemColors.Highlight;
            appearance17.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.ugData.DisplayLayout.Override.ActiveRowAppearance = appearance17;
            this.ugData.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.ugData.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance36.BackColor = System.Drawing.SystemColors.Window;
            this.ugData.DisplayLayout.Override.CardAreaAppearance = appearance36;
            appearance20.BorderColor = System.Drawing.Color.Silver;
            appearance20.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.ugData.DisplayLayout.Override.CellAppearance = appearance20;
            this.ugData.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.ugData.DisplayLayout.Override.CellPadding = 0;
            appearance18.BackColor = System.Drawing.SystemColors.Control;
            appearance18.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance18.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance18.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance18.BorderColor = System.Drawing.SystemColors.Window;
            this.ugData.DisplayLayout.Override.GroupByRowAppearance = appearance18;
            appearance19.TextHAlignAsString = "Left";
            this.ugData.DisplayLayout.Override.HeaderAppearance = appearance19;
            this.ugData.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.ugData.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance22.BackColor = System.Drawing.SystemColors.Window;
            appearance22.BorderColor = System.Drawing.Color.Silver;
            this.ugData.DisplayLayout.Override.RowAppearance = appearance22;
            this.ugData.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance35.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ugData.DisplayLayout.Override.TemplateAddRowAppearance = appearance35;
            this.ugData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ugData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.ugData.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ugData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugData.Location = new System.Drawing.Point(0, 0);
            this.ugData.Name = "ugData";
            this.ugData.Size = new System.Drawing.Size(775, 346);
            this.ugData.TabIndex = 0;
            // 
            // ultraSplitter1
            // 
            this.ultraSplitter1.BackColor = System.Drawing.SystemColors.Control;
            this.ultraSplitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraSplitter1.Location = new System.Drawing.Point(0, 153);
            this.ultraSplitter1.Name = "ultraSplitter1";
            this.ultraSplitter1.RestoreExtent = 153;
            this.ultraSplitter1.Size = new System.Drawing.Size(775, 10);
            this.ultraSplitter1.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btbKvsrClear);
            this.panel1.Controls.Add(this.cbKmbResults);
            this.panel1.Controls.Add(this.cbTaxResource);
            this.panel1.Controls.Add(this.cbForecast);
            this.panel1.Controls.Add(this.cbEstimate);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.ucMunicipal);
            this.panel1.Controls.Add(this.ucBudgetLevel);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.uteVariant);
            this.panel1.Controls.Add(this.uteAdministrator);
            this.panel1.Controls.Add(this.uteIncomesSource);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(775, 153);
            this.panel1.TabIndex = 1;
            // 
            // btbKvsrClear
            // 
            appearance3.Image = ((object)(resources.GetObject("appearance3.Image")));
            appearance3.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance3.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.btbKvsrClear.Appearance = appearance3;
            this.btbKvsrClear.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Popup;
            this.btbKvsrClear.Location = new System.Drawing.Point(252, 59);
            this.btbKvsrClear.Name = "btbKvsrClear";
            this.btbKvsrClear.Size = new System.Drawing.Size(24, 24);
            this.btbKvsrClear.TabIndex = 11;
            // 
            // cbKmbResults
            // 
            this.cbKmbResults.AutoSize = true;
            this.cbKmbResults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbKmbResults.Location = new System.Drawing.Point(424, 117);
            this.cbKmbResults.Name = "cbKmbResults";
            this.cbKmbResults.Size = new System.Drawing.Size(94, 17);
            this.cbKmbResults.TabIndex = 10;
            this.cbKmbResults.Text = "Итоги по КМБ";
            this.cbKmbResults.UseVisualStyleBackColor = true;
            this.cbKmbResults.Visible = false;
            // 
            // cbTaxResource
            // 
            this.cbTaxResource.AutoSize = true;
            this.cbTaxResource.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbTaxResource.Location = new System.Drawing.Point(424, 60);
            this.cbTaxResource.Name = "cbTaxResource";
            this.cbTaxResource.Size = new System.Drawing.Size(136, 17);
            this.cbTaxResource.TabIndex = 10;
            this.cbTaxResource.Text = "Налоговый потенциал";
            this.cbTaxResource.UseVisualStyleBackColor = true;
            // 
            // cbForecast
            // 
            this.cbForecast.AutoSize = true;
            this.cbForecast.Checked = true;
            this.cbForecast.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbForecast.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbForecast.Location = new System.Drawing.Point(424, 37);
            this.cbForecast.Name = "cbForecast";
            this.cbForecast.Size = new System.Drawing.Size(66, 17);
            this.cbForecast.TabIndex = 10;
            this.cbForecast.Text = "Прогноз";
            this.cbForecast.UseVisualStyleBackColor = true;
            // 
            // cbEstimate
            // 
            this.cbEstimate.AutoSize = true;
            this.cbEstimate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbEstimate.Location = new System.Drawing.Point(424, 15);
            this.cbEstimate.Name = "cbEstimate";
            this.cbEstimate.Size = new System.Drawing.Size(61, 17);
            this.cbEstimate.TabIndex = 9;
            this.cbEstimate.Text = "Оценка";
            this.cbEstimate.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 119);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(158, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Муниципальные образования";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Уровни бюджета";
            // 
            // ucMunicipal
            // 
            this.ucMunicipal.CheckedListSettings.CheckStateMember = "";
            appearance23.BackColor = System.Drawing.SystemColors.Window;
            appearance23.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.ucMunicipal.DisplayLayout.Appearance = appearance23;
            this.ucMunicipal.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            this.ucMunicipal.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ucMunicipal.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance24.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance24.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance24.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance24.BorderColor = System.Drawing.SystemColors.Window;
            this.ucMunicipal.DisplayLayout.GroupByBox.Appearance = appearance24;
            appearance26.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ucMunicipal.DisplayLayout.GroupByBox.BandLabelAppearance = appearance26;
            this.ucMunicipal.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance25.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance25.BackColor2 = System.Drawing.SystemColors.Control;
            appearance25.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance25.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ucMunicipal.DisplayLayout.GroupByBox.PromptAppearance = appearance25;
            this.ucMunicipal.DisplayLayout.MaxColScrollRegions = 1;
            this.ucMunicipal.DisplayLayout.MaxRowScrollRegions = 1;
            appearance31.BackColor = System.Drawing.SystemColors.Window;
            appearance31.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ucMunicipal.DisplayLayout.Override.ActiveCellAppearance = appearance31;
            appearance27.BackColor = System.Drawing.SystemColors.Highlight;
            appearance27.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.ucMunicipal.DisplayLayout.Override.ActiveRowAppearance = appearance27;
            this.ucMunicipal.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.ucMunicipal.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance34.BackColor = System.Drawing.SystemColors.Window;
            this.ucMunicipal.DisplayLayout.Override.CardAreaAppearance = appearance34;
            appearance30.BorderColor = System.Drawing.Color.Silver;
            appearance30.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.ucMunicipal.DisplayLayout.Override.CellAppearance = appearance30;
            this.ucMunicipal.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.ucMunicipal.DisplayLayout.Override.CellPadding = 0;
            appearance28.BackColor = System.Drawing.SystemColors.Control;
            appearance28.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance28.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance28.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance28.BorderColor = System.Drawing.SystemColors.Window;
            this.ucMunicipal.DisplayLayout.Override.GroupByRowAppearance = appearance28;
            appearance29.TextHAlignAsString = "Left";
            this.ucMunicipal.DisplayLayout.Override.HeaderAppearance = appearance29;
            this.ucMunicipal.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.ucMunicipal.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance32.BackColor = System.Drawing.SystemColors.Window;
            appearance32.BorderColor = System.Drawing.Color.Silver;
            this.ucMunicipal.DisplayLayout.Override.RowAppearance = appearance32;
            this.ucMunicipal.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance33.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ucMunicipal.DisplayLayout.Override.TemplateAddRowAppearance = appearance33;
            this.ucMunicipal.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ucMunicipal.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.ucMunicipal.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ucMunicipal.DropDownStyle = Infragistics.Win.UltraWinGrid.UltraComboStyle.DropDownList;
            this.ucMunicipal.Location = new System.Drawing.Point(180, 115);
            this.ucMunicipal.Name = "ucMunicipal";
            this.ucMunicipal.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.ucMunicipal.Size = new System.Drawing.Size(229, 20);
            this.ucMunicipal.TabIndex = 7;
            this.ucMunicipal.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            // 
            // ucBudgetLevel
            // 
            this.ucBudgetLevel.CheckedListSettings.CheckStateMember = "";
            appearance37.BackColor = System.Drawing.SystemColors.Window;
            appearance37.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.ucBudgetLevel.DisplayLayout.Appearance = appearance37;
            this.ucBudgetLevel.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            this.ucBudgetLevel.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ucBudgetLevel.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance38.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance38.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance38.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance38.BorderColor = System.Drawing.SystemColors.Window;
            this.ucBudgetLevel.DisplayLayout.GroupByBox.Appearance = appearance38;
            appearance39.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ucBudgetLevel.DisplayLayout.GroupByBox.BandLabelAppearance = appearance39;
            this.ucBudgetLevel.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance40.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance40.BackColor2 = System.Drawing.SystemColors.Control;
            appearance40.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance40.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ucBudgetLevel.DisplayLayout.GroupByBox.PromptAppearance = appearance40;
            this.ucBudgetLevel.DisplayLayout.MaxColScrollRegions = 1;
            this.ucBudgetLevel.DisplayLayout.MaxRowScrollRegions = 1;
            appearance41.BackColor = System.Drawing.SystemColors.Window;
            appearance41.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ucBudgetLevel.DisplayLayout.Override.ActiveCellAppearance = appearance41;
            appearance42.BackColor = System.Drawing.SystemColors.Highlight;
            appearance42.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.ucBudgetLevel.DisplayLayout.Override.ActiveRowAppearance = appearance42;
            this.ucBudgetLevel.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.ucBudgetLevel.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance43.BackColor = System.Drawing.SystemColors.Window;
            this.ucBudgetLevel.DisplayLayout.Override.CardAreaAppearance = appearance43;
            appearance44.BorderColor = System.Drawing.Color.Silver;
            appearance44.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.ucBudgetLevel.DisplayLayout.Override.CellAppearance = appearance44;
            this.ucBudgetLevel.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.ucBudgetLevel.DisplayLayout.Override.CellPadding = 0;
            appearance45.BackColor = System.Drawing.SystemColors.Control;
            appearance45.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance45.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance45.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance45.BorderColor = System.Drawing.SystemColors.Window;
            this.ucBudgetLevel.DisplayLayout.Override.GroupByRowAppearance = appearance45;
            appearance46.TextHAlignAsString = "Left";
            this.ucBudgetLevel.DisplayLayout.Override.HeaderAppearance = appearance46;
            this.ucBudgetLevel.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.ucBudgetLevel.DisplayLayout.Override.HeaderPlacement = Infragistics.Win.UltraWinGrid.HeaderPlacement.OncePerRowIsland;
            this.ucBudgetLevel.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance47.BackColor = System.Drawing.SystemColors.Window;
            appearance47.BorderColor = System.Drawing.Color.Silver;
            this.ucBudgetLevel.DisplayLayout.Override.RowAppearance = appearance47;
            this.ucBudgetLevel.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance48.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ucBudgetLevel.DisplayLayout.Override.TemplateAddRowAppearance = appearance48;
            this.ucBudgetLevel.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ucBudgetLevel.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.ucBudgetLevel.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.ucBudgetLevel.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ucBudgetLevel.DropDownStyle = Infragistics.Win.UltraWinGrid.UltraComboStyle.DropDownList;
            this.ucBudgetLevel.Location = new System.Drawing.Point(180, 87);
            this.ucBudgetLevel.Name = "ucBudgetLevel";
            this.ucBudgetLevel.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.ucBudgetLevel.Size = new System.Drawing.Size(229, 20);
            this.ucBudgetLevel.TabIndex = 7;
            this.ucBudgetLevel.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Вариант";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Администратор";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Доходный источник";
            // 
            // uteVariant
            // 
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            this.uteVariant.Appearance = appearance1;
            this.uteVariant.BackColor = System.Drawing.SystemColors.Window;
            appearance2.FontData.BoldAsString = "True";
            editorButton1.Appearance = appearance2;
            editorButton1.Text = "...";
            this.uteVariant.ButtonsRight.Add(editorButton1);
            this.uteVariant.Location = new System.Drawing.Point(180, 12);
            this.uteVariant.Name = "uteVariant";
            this.uteVariant.ReadOnly = true;
            this.uteVariant.Size = new System.Drawing.Size(229, 19);
            this.uteVariant.TabIndex = 5;
            this.uteVariant.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            // 
            // uteAdministrator
            // 
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            this.uteAdministrator.Appearance = appearance11;
            this.uteAdministrator.BackColor = System.Drawing.SystemColors.Window;
            appearance12.FontData.BoldAsString = "True";
            editorButton2.Appearance = appearance12;
            editorButton2.Text = "...";
            this.uteAdministrator.ButtonsRight.Add(editorButton2);
            this.uteAdministrator.Location = new System.Drawing.Point(180, 62);
            this.uteAdministrator.Name = "uteAdministrator";
            this.uteAdministrator.ReadOnly = true;
            this.uteAdministrator.Size = new System.Drawing.Size(64, 19);
            this.uteAdministrator.TabIndex = 5;
            this.uteAdministrator.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            // 
            // uteIncomesSource
            // 
            appearance51.BackColor = System.Drawing.SystemColors.Window;
            this.uteIncomesSource.Appearance = appearance51;
            this.uteIncomesSource.BackColor = System.Drawing.SystemColors.Window;
            appearance52.FontData.BoldAsString = "True";
            editorButton3.Appearance = appearance52;
            editorButton3.Text = "...";
            this.uteIncomesSource.ButtonsRight.Add(editorButton3);
            this.uteIncomesSource.Location = new System.Drawing.Point(180, 37);
            this.uteIncomesSource.Name = "uteIncomesSource";
            this.uteIncomesSource.ReadOnly = true;
            this.uteIncomesSource.Size = new System.Drawing.Size(229, 19);
            this.uteIncomesSource.TabIndex = 5;
            this.uteIncomesSource.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            // 
            // _BaseView_Toolbars_Dock_Area_Left
            // 
            this._BaseView_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._BaseView_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 27);
            this._BaseView_Toolbars_Dock_Area_Left.Name = "_BaseView_Toolbars_Dock_Area_Left";
            this._BaseView_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 509);
            this._BaseView_Toolbars_Dock_Area_Left.ToolbarsManager = this.tbManager;
            // 
            // tbManager
            // 
            this.tbManager.DesignerFlags = 1;
            this.tbManager.DockWithinContainer = this;
            this.tbManager.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbManager.ShowFullMenusDelay = 500;
            ultraToolbar1.DockedColumn = 0;
            ultraToolbar1.DockedRow = 0;
            ultraToolbar1.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool3,
            buttonTool2,
            buttonTool4,
            buttonTool5,
            buttonTool6,
            buttonTool14});
            ultraToolbar1.Text = "DataOperations";
            this.tbManager.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1});
            appearance53.Image = ((object)(resources.GetObject("appearance53.Image")));
            buttonTool7.SharedPropsInternal.AppearancesSmall.Appearance = appearance53;
            buttonTool7.SharedPropsInternal.Caption = "Обновить данные";
            appearance54.Image = ((object)(resources.GetObject("appearance54.Image")));
            buttonTool8.SharedPropsInternal.AppearancesSmall.Appearance = appearance54;
            buttonTool8.SharedPropsInternal.Caption = "Отменить изменения";
            appearance55.Image = ((object)(resources.GetObject("appearance55.Image")));
            buttonTool9.SharedPropsInternal.AppearancesSmall.Appearance = appearance55;
            buttonTool9.SharedPropsInternal.Caption = "Сохранить изменения";
            appearance56.Image = ((object)(resources.GetObject("appearance56.Image")));
            buttonTool10.SharedPropsInternal.AppearancesSmall.Appearance = appearance56;
            buttonTool10.SharedPropsInternal.Caption = "Сохранить данные в Excel";
            appearance57.Image = ((object)(resources.GetObject("appearance57.Image")));
            buttonTool11.SharedPropsInternal.AppearancesSmall.Appearance = appearance57;
            buttonTool11.SharedPropsInternal.Caption = "Загрузить данные из Excel";
            appearance58.Image = ((object)(resources.GetObject("appearance58.Image")));
            buttonTool12.SharedPropsInternal.AppearancesSmall.Appearance = appearance58;
            buttonTool12.SharedPropsInternal.Caption = "Создать отчет";
            appearance60.Image = ((object)(resources.GetObject("appearance60.Image")));
            buttonTool13.SharedPropsInternal.AppearancesSmall.Appearance = appearance60;
            buttonTool13.SharedPropsInternal.Caption = "Копирование варианта";
            this.tbManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool7,
            buttonTool8,
            buttonTool9,
            buttonTool10,
            buttonTool11,
            buttonTool12,
            buttonTool13});
            // 
            // _BaseView_Toolbars_Dock_Area_Right
            // 
            this._BaseView_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._BaseView_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(775, 27);
            this._BaseView_Toolbars_Dock_Area_Right.Name = "_BaseView_Toolbars_Dock_Area_Right";
            this._BaseView_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 509);
            this._BaseView_Toolbars_Dock_Area_Right.ToolbarsManager = this.tbManager;
            // 
            // _BaseView_Toolbars_Dock_Area_Top
            // 
            this._BaseView_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._BaseView_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._BaseView_Toolbars_Dock_Area_Top.Name = "_BaseView_Toolbars_Dock_Area_Top";
            this._BaseView_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(775, 27);
            this._BaseView_Toolbars_Dock_Area_Top.ToolbarsManager = this.tbManager;
            // 
            // _BaseView_Toolbars_Dock_Area_Bottom
            // 
            this._BaseView_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._BaseView_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 536);
            this._BaseView_Toolbars_Dock_Area_Bottom.Name = "_BaseView_Toolbars_Dock_Area_Bottom";
            this._BaseView_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(775, 0);
            this._BaseView_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.tbManager;
            // 
            // IncomesEvalPlanView
            // 
            this.Controls.Add(this.BaseView_Fill_Panel);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Bottom);
            this.Name = "IncomesEvalPlanView";
            this.Size = new System.Drawing.Size(775, 536);
            this.BaseView_Fill_Panel.ClientArea.ResumeLayout(false);
            this.BaseView_Fill_Panel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ugData)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ucMunicipal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ucBudgetLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uteVariant)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uteAdministrator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uteIncomesSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbManager)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
