using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.Balance
{
    public class BalanceView : BaseViewObject.BaseView
    {
        private Infragistics.Win.Misc.UltraPanel BaseView_Fill_Panel;
        internal Infragistics.Win.UltraWinGrid.UltraGrid ugData;
        internal Infragistics.Win.UltraWinToolbars.UltraToolbarsManager tbManager;
        private System.ComponentModel.IContainer components;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Bottom;
        private Infragistics.Win.Misc.UltraPanel ultraPanel1;
    
        public BalanceView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance52 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance53 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance55 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance54 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance60 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance56 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance63 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance59 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance57 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance58 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance61 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance62 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("IncomesVariant");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool25 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnIncomesVariant");
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar2 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("ChargeVariant");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool27 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnChargeVariant");
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar3 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("IFVariant");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool29 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnIFVariant");
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar4 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("Operations");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool31 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnGetBalance");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnExport");
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar5 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("Year");
            Infragistics.Win.UltraWinToolbars.LabelTool labelTool5 = new Infragistics.Win.UltraWinToolbars.LabelTool("lYear");
            Infragistics.Win.UltraWinToolbars.ComboBoxTool comboBoxTool1 = new Infragistics.Win.UltraWinToolbars.ComboBoxTool("cbYear");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool26 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnIncomesVariant");
            Infragistics.Win.Appearance appearance48 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BalanceView));
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool28 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnChargeVariant");
            Infragistics.Win.Appearance appearance49 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool30 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnIFVariant");
            Infragistics.Win.Appearance appearance50 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool32 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnGetBalance");
            Infragistics.Win.Appearance appearance51 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.LabelTool labelTool6 = new Infragistics.Win.UltraWinToolbars.LabelTool("lYear");
            Infragistics.Win.Appearance appearance64 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ComboBoxTool comboBoxTool2 = new Infragistics.Win.UltraWinToolbars.ComboBoxTool("cbYear");
            Infragistics.Win.ValueList valueList1 = new Infragistics.Win.ValueList(0);
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnExport");
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            this.BaseView_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            this.ugData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.tbManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._BaseView_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseView_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseView_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseView_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.BaseView_Fill_Panel.ClientArea.SuspendLayout();
            this.BaseView_Fill_Panel.SuspendLayout();
            this.ultraPanel1.ClientArea.SuspendLayout();
            this.ultraPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbManager)).BeginInit();
            this.SuspendLayout();
            // 
            // BaseView_Fill_Panel
            // 
            // 
            // BaseView_Fill_Panel.ClientArea
            // 
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.ultraPanel1);
            this.BaseView_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.BaseView_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BaseView_Fill_Panel.Location = new System.Drawing.Point(0, 131);
            this.BaseView_Fill_Panel.Name = "BaseView_Fill_Panel";
            this.BaseView_Fill_Panel.Size = new System.Drawing.Size(730, 405);
            this.BaseView_Fill_Panel.TabIndex = 0;
            // 
            // ultraPanel1
            // 
            // 
            // ultraPanel1.ClientArea
            // 
            this.ultraPanel1.ClientArea.Controls.Add(this.ugData);
            this.ultraPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraPanel1.Location = new System.Drawing.Point(0, 0);
            this.ultraPanel1.Name = "ultraPanel1";
            this.ultraPanel1.Size = new System.Drawing.Size(730, 405);
            this.ultraPanel1.TabIndex = 0;
            // 
            // ugData
            // 
            appearance52.BackColor = System.Drawing.SystemColors.Window;
            appearance52.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.ugData.DisplayLayout.Appearance = appearance52;
            this.ugData.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ugData.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance53.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance53.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance53.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance53.BorderColor = System.Drawing.SystemColors.Window;
            this.ugData.DisplayLayout.GroupByBox.Appearance = appearance53;
            appearance55.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ugData.DisplayLayout.GroupByBox.BandLabelAppearance = appearance55;
            this.ugData.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance54.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance54.BackColor2 = System.Drawing.SystemColors.Control;
            appearance54.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance54.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ugData.DisplayLayout.GroupByBox.PromptAppearance = appearance54;
            this.ugData.DisplayLayout.MaxColScrollRegions = 1;
            this.ugData.DisplayLayout.MaxRowScrollRegions = 1;
            appearance60.BackColor = System.Drawing.SystemColors.Window;
            appearance60.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ugData.DisplayLayout.Override.ActiveCellAppearance = appearance60;
            appearance56.BackColor = System.Drawing.SystemColors.Highlight;
            appearance56.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.ugData.DisplayLayout.Override.ActiveRowAppearance = appearance56;
            this.ugData.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed;
            this.ugData.DisplayLayout.Override.AllowColSizing = Infragistics.Win.UltraWinGrid.AllowColSizing.None;
            this.ugData.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            this.ugData.DisplayLayout.Override.AllowGroupMoving = Infragistics.Win.UltraWinGrid.AllowGroupMoving.NotAllowed;
            this.ugData.DisplayLayout.Override.AllowGroupSwapping = Infragistics.Win.UltraWinGrid.AllowGroupSwapping.NotAllowed;
            this.ugData.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.ugData.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance63.BackColor = System.Drawing.SystemColors.Window;
            this.ugData.DisplayLayout.Override.CardAreaAppearance = appearance63;
            appearance59.BorderColor = System.Drawing.Color.Silver;
            appearance59.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.ugData.DisplayLayout.Override.CellAppearance = appearance59;
            this.ugData.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.ugData.DisplayLayout.Override.CellPadding = 0;
            appearance57.BackColor = System.Drawing.SystemColors.Control;
            appearance57.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance57.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance57.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance57.BorderColor = System.Drawing.SystemColors.Window;
            this.ugData.DisplayLayout.Override.GroupByRowAppearance = appearance57;
            appearance58.TextHAlignAsString = "Left";
            this.ugData.DisplayLayout.Override.HeaderAppearance = appearance58;
            this.ugData.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.ugData.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance61.BackColor = System.Drawing.SystemColors.Window;
            appearance61.BorderColor = System.Drawing.Color.Silver;
            this.ugData.DisplayLayout.Override.RowAppearance = appearance61;
            this.ugData.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance62.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ugData.DisplayLayout.Override.TemplateAddRowAppearance = appearance62;
            this.ugData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ugData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.ugData.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ugData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugData.Location = new System.Drawing.Point(0, 0);
            this.ugData.Name = "ugData";
            this.ugData.Size = new System.Drawing.Size(730, 405);
            this.ugData.TabIndex = 0;
            this.ugData.Text = "ultraGrid1";
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
            buttonTool25});
            ultraToolbar1.Text = "IncomesVariant";
            ultraToolbar2.DockedColumn = 0;
            ultraToolbar2.DockedRow = 1;
            ultraToolbar2.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool27});
            ultraToolbar2.Text = "ChargeVariant";
            ultraToolbar3.DockedColumn = 0;
            ultraToolbar3.DockedRow = 2;
            ultraToolbar3.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool29});
            ultraToolbar3.Text = "IFVariant";
            ultraToolbar4.DockedColumn = 0;
            ultraToolbar4.DockedRow = 4;
            ultraToolbar4.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool31,
            buttonTool1});
            ultraToolbar4.Text = "Operations";
            ultraToolbar5.DockedColumn = 0;
            ultraToolbar5.DockedRow = 3;
            comboBoxTool1.InstanceProps.Width = 57;
            ultraToolbar5.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            labelTool5,
            comboBoxTool1});
            ultraToolbar5.Text = "Year";
            this.tbManager.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1,
            ultraToolbar2,
            ultraToolbar3,
            ultraToolbar4,
            ultraToolbar5});
            appearance48.Image = ((object)(resources.GetObject("appearance48.Image")));
            buttonTool26.SharedPropsInternal.AppearancesSmall.Appearance = appearance48;
            buttonTool26.SharedPropsInternal.Caption = "Вариант доходов не выбран";
            buttonTool26.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            appearance49.Image = ((object)(resources.GetObject("appearance49.Image")));
            buttonTool28.SharedPropsInternal.AppearancesSmall.Appearance = appearance49;
            buttonTool28.SharedPropsInternal.Caption = "Вариант расходов не выбран";
            buttonTool28.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            appearance50.Image = ((object)(resources.GetObject("appearance50.Image")));
            buttonTool30.SharedPropsInternal.AppearancesSmall.Appearance = appearance50;
            buttonTool30.SharedPropsInternal.Caption = "Вариант ИФ не выбран";
            buttonTool30.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            appearance51.Image = ((object)(resources.GetObject("appearance51.Image")));
            buttonTool32.SharedPropsInternal.AppearancesSmall.Appearance = appearance51;
            buttonTool32.SharedPropsInternal.Caption = "Получить данные по балансировке";
            appearance64.TextHAlignAsString = "Left";
            labelTool6.SharedPropsInternal.AppearancesSmall.Appearance = appearance64;
            labelTool6.SharedPropsInternal.Caption = "Год";
            comboBoxTool2.SharedPropsInternal.Caption = "Год";
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            comboBoxTool2.ValueList = valueList1;
            appearance11.Image = global::Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Properties.Resources.Document_Microsoft_Excel_icon;
            buttonTool2.SharedPropsInternal.AppearancesSmall.Appearance = appearance11;
            buttonTool2.SharedPropsInternal.Caption = "Сохранить отчет";
            this.tbManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool26,
            buttonTool28,
            buttonTool30,
            buttonTool32,
            labelTool6,
            comboBoxTool2,
            buttonTool2});
            // 
            // _BaseView_Toolbars_Dock_Area_Top
            // 
            this._BaseView_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._BaseView_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._BaseView_Toolbars_Dock_Area_Top.Name = "_BaseView_Toolbars_Dock_Area_Top";
            this._BaseView_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(730, 131);
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
            this._BaseView_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(730, 0);
            this._BaseView_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.tbManager;
            // 
            // _BaseView_Toolbars_Dock_Area_Left
            // 
            this._BaseView_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._BaseView_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 131);
            this._BaseView_Toolbars_Dock_Area_Left.Name = "_BaseView_Toolbars_Dock_Area_Left";
            this._BaseView_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 405);
            this._BaseView_Toolbars_Dock_Area_Left.ToolbarsManager = this.tbManager;
            // 
            // _BaseView_Toolbars_Dock_Area_Right
            // 
            this._BaseView_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._BaseView_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(730, 131);
            this._BaseView_Toolbars_Dock_Area_Right.Name = "_BaseView_Toolbars_Dock_Area_Right";
            this._BaseView_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 405);
            this._BaseView_Toolbars_Dock_Area_Right.ToolbarsManager = this.tbManager;
            // 
            // BalanceView
            // 
            this.Controls.Add(this.BaseView_Fill_Panel);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Bottom);
            this.Name = "BalanceView";
            this.Size = new System.Drawing.Size(730, 536);
            this.BaseView_Fill_Panel.ClientArea.ResumeLayout(false);
            this.BaseView_Fill_Panel.ResumeLayout(false);
            this.ultraPanel1.ClientArea.ResumeLayout(false);
            this.ultraPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ugData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbManager)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
