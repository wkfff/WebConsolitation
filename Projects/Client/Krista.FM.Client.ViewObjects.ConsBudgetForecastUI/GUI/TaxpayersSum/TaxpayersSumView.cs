using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Client.ViewObjects.BaseViewObject;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.TaxpayersSum
{
    public class TaxpayersSumView : BaseView
    {
        internal Infragistics.Win.UltraWinToolbars.UltraToolbarsManager tbManager;
        private System.ComponentModel.IContainer components;
        private Infragistics.Win.Misc.UltraPanel BaseView_Fill_Panel;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Top;
        private Infragistics.Win.Misc.UltraSplitter ultraSplitter1;
        private Infragistics.Win.Misc.UltraPanel ultraPanel1;
        internal System.Windows.Forms.ComboBox cbYears;
        private System.Windows.Forms.Label lYear;
        internal System.Windows.Forms.ComboBox Quarter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Infragistics.Win.Misc.UltraPanel ultraPanel2;
        private Infragistics.Win.Misc.UltraPanel ultraPanel4;
        internal Infragistics.Win.UltraWinGrid.UltraGrid TaxpayerDataGrid;
        private Infragistics.Win.Misc.UltraPanel ultraPanel3;
        internal Infragistics.Win.UltraWinGrid.UltraGrid ResultsGrid;
        internal Infragistics.Win.UltraWinEditors.UltraTextEditor Municipals;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Bottom;

        public TaxpayersSumView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("DataOperations");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("RefreshData");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("AddNew");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SaveChanges");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CancelChanges");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("RefreshData");
            Infragistics.Win.Appearance appearance53 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TaxpayersSumView));
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
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Infragistics.Win.UltraWinToolbars.ButtonTool("AddNew");
            Infragistics.Win.Appearance appearance37 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Delete");
            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            this._BaseView_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.tbManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._BaseView_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseView_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseView_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.BaseView_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.ultraPanel2 = new Infragistics.Win.Misc.UltraPanel();
            this.ultraPanel4 = new Infragistics.Win.Misc.UltraPanel();
            this.TaxpayerDataGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraPanel3 = new Infragistics.Win.Misc.UltraPanel();
            this.ResultsGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraSplitter1 = new Infragistics.Win.Misc.UltraSplitter();
            this.ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            this.Municipals = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.Quarter = new System.Windows.Forms.ComboBox();
            this.cbYears = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lYear = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tbManager)).BeginInit();
            this.BaseView_Fill_Panel.ClientArea.SuspendLayout();
            this.BaseView_Fill_Panel.SuspendLayout();
            this.ultraPanel2.ClientArea.SuspendLayout();
            this.ultraPanel2.SuspendLayout();
            this.ultraPanel4.ClientArea.SuspendLayout();
            this.ultraPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TaxpayerDataGrid)).BeginInit();
            this.ultraPanel3.ClientArea.SuspendLayout();
            this.ultraPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResultsGrid)).BeginInit();
            this.ultraPanel1.ClientArea.SuspendLayout();
            this.ultraPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Municipals)).BeginInit();
            this.SuspendLayout();
            // 
            // _BaseView_Toolbars_Dock_Area_Top
            // 
            this._BaseView_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._BaseView_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._BaseView_Toolbars_Dock_Area_Top.Name = "_BaseView_Toolbars_Dock_Area_Top";
            this._BaseView_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(716, 27);
            this._BaseView_Toolbars_Dock_Area_Top.ToolbarsManager = this.tbManager;
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
            buttonTool4,
            buttonTool5,
            buttonTool3,
            buttonTool2});
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
            appearance37.Image = ((object)(resources.GetObject("appearance37.Image")));
            buttonTool15.SharedPropsInternal.AppearancesSmall.Appearance = appearance37;
            buttonTool15.SharedPropsInternal.Caption = "Добавить налогоплательщика";
            appearance38.Image = ((object)(resources.GetObject("appearance38.Image")));
            buttonTool16.SharedPropsInternal.AppearancesSmall.Appearance = appearance38;
            buttonTool16.SharedPropsInternal.Caption = "Удалить данные налогоплательщика";
            this.tbManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool7,
            buttonTool8,
            buttonTool9,
            buttonTool10,
            buttonTool11,
            buttonTool12,
            buttonTool13,
            buttonTool15,
            buttonTool16});
            // 
            // _BaseView_Toolbars_Dock_Area_Bottom
            // 
            this._BaseView_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._BaseView_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 536);
            this._BaseView_Toolbars_Dock_Area_Bottom.Name = "_BaseView_Toolbars_Dock_Area_Bottom";
            this._BaseView_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(716, 0);
            this._BaseView_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.tbManager;
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
            // _BaseView_Toolbars_Dock_Area_Right
            // 
            this._BaseView_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._BaseView_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(716, 27);
            this._BaseView_Toolbars_Dock_Area_Right.Name = "_BaseView_Toolbars_Dock_Area_Right";
            this._BaseView_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 509);
            this._BaseView_Toolbars_Dock_Area_Right.ToolbarsManager = this.tbManager;
            // 
            // BaseView_Fill_Panel
            // 
            // 
            // BaseView_Fill_Panel.ClientArea
            // 
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.ultraPanel2);
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.ultraSplitter1);
            this.BaseView_Fill_Panel.ClientArea.Controls.Add(this.ultraPanel1);
            this.BaseView_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.BaseView_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BaseView_Fill_Panel.Location = new System.Drawing.Point(0, 27);
            this.BaseView_Fill_Panel.Name = "BaseView_Fill_Panel";
            this.BaseView_Fill_Panel.Size = new System.Drawing.Size(716, 509);
            this.BaseView_Fill_Panel.TabIndex = 8;
            // 
            // ultraPanel2
            // 
            // 
            // ultraPanel2.ClientArea
            // 
            this.ultraPanel2.ClientArea.Controls.Add(this.ultraPanel4);
            this.ultraPanel2.ClientArea.Controls.Add(this.ultraPanel3);
            this.ultraPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraPanel2.Location = new System.Drawing.Point(0, 83);
            this.ultraPanel2.Name = "ultraPanel2";
            this.ultraPanel2.Size = new System.Drawing.Size(716, 426);
            this.ultraPanel2.TabIndex = 5;
            // 
            // ultraPanel4
            // 
            // 
            // ultraPanel4.ClientArea
            // 
            this.ultraPanel4.ClientArea.Controls.Add(this.TaxpayerDataGrid);
            this.ultraPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraPanel4.Location = new System.Drawing.Point(0, 100);
            this.ultraPanel4.Name = "ultraPanel4";
            this.ultraPanel4.Size = new System.Drawing.Size(716, 326);
            this.ultraPanel4.TabIndex = 1;
            // 
            // TaxpayerDataGrid
            // 
            appearance12.BackColor = System.Drawing.SystemColors.Window;
            appearance12.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.TaxpayerDataGrid.DisplayLayout.Appearance = appearance12;
            this.TaxpayerDataGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.TaxpayerDataGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance13.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance13.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance13.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance13.BorderColor = System.Drawing.SystemColors.Window;
            this.TaxpayerDataGrid.DisplayLayout.GroupByBox.Appearance = appearance13;
            appearance28.ForeColor = System.Drawing.SystemColors.GrayText;
            this.TaxpayerDataGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance28;
            this.TaxpayerDataGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.TaxpayerDataGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance16.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance16.BackColor2 = System.Drawing.SystemColors.Control;
            appearance16.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance16.ForeColor = System.Drawing.SystemColors.GrayText;
            this.TaxpayerDataGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance16;
            this.TaxpayerDataGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.TaxpayerDataGrid.DisplayLayout.MaxRowScrollRegions = 1;
            appearance33.BackColor = System.Drawing.SystemColors.Window;
            appearance33.ForeColor = System.Drawing.SystemColors.ControlText;
            this.TaxpayerDataGrid.DisplayLayout.Override.ActiveCellAppearance = appearance33;
            appearance29.BackColor = System.Drawing.SystemColors.Highlight;
            appearance29.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.TaxpayerDataGrid.DisplayLayout.Override.ActiveRowAppearance = appearance29;
            this.TaxpayerDataGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.TaxpayerDataGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.TaxpayerDataGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance36.BackColor = System.Drawing.SystemColors.Window;
            this.TaxpayerDataGrid.DisplayLayout.Override.CardAreaAppearance = appearance36;
            appearance32.BorderColor = System.Drawing.Color.Silver;
            appearance32.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.TaxpayerDataGrid.DisplayLayout.Override.CellAppearance = appearance32;
            this.TaxpayerDataGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.TaxpayerDataGrid.DisplayLayout.Override.CellPadding = 0;
            appearance30.BackColor = System.Drawing.SystemColors.Control;
            appearance30.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance30.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance30.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance30.BorderColor = System.Drawing.SystemColors.Window;
            this.TaxpayerDataGrid.DisplayLayout.Override.GroupByRowAppearance = appearance30;
            appearance31.TextHAlignAsString = "Left";
            this.TaxpayerDataGrid.DisplayLayout.Override.HeaderAppearance = appearance31;
            this.TaxpayerDataGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.TaxpayerDataGrid.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance34.BackColor = System.Drawing.SystemColors.Window;
            appearance34.BorderColor = System.Drawing.Color.Silver;
            this.TaxpayerDataGrid.DisplayLayout.Override.RowAppearance = appearance34;
            this.TaxpayerDataGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance35.BackColor = System.Drawing.SystemColors.ControlLight;
            this.TaxpayerDataGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance35;
            this.TaxpayerDataGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.TaxpayerDataGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.TaxpayerDataGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.TaxpayerDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TaxpayerDataGrid.Location = new System.Drawing.Point(0, 0);
            this.TaxpayerDataGrid.Name = "TaxpayerDataGrid";
            this.TaxpayerDataGrid.Size = new System.Drawing.Size(716, 326);
            this.TaxpayerDataGrid.TabIndex = 0;
            this.TaxpayerDataGrid.Text = "ultraGrid2";
            // 
            // ultraPanel3
            // 
            // 
            // ultraPanel3.ClientArea
            // 
            this.ultraPanel3.ClientArea.Controls.Add(this.ResultsGrid);
            this.ultraPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraPanel3.Location = new System.Drawing.Point(0, 0);
            this.ultraPanel3.Name = "ultraPanel3";
            this.ultraPanel3.Size = new System.Drawing.Size(716, 100);
            this.ultraPanel3.TabIndex = 0;
            // 
            // ResultsGrid
            // 
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.ResultsGrid.DisplayLayout.Appearance = appearance11;
            this.ResultsGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ResultsGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance17.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance17.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance17.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance17.BorderColor = System.Drawing.SystemColors.Window;
            this.ResultsGrid.DisplayLayout.GroupByBox.Appearance = appearance17;
            appearance19.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ResultsGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance19;
            this.ResultsGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ResultsGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance18.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance18.BackColor2 = System.Drawing.SystemColors.Control;
            appearance18.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance18.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ResultsGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance18;
            this.ResultsGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.ResultsGrid.DisplayLayout.MaxRowScrollRegions = 1;
            appearance24.BackColor = System.Drawing.SystemColors.Window;
            appearance24.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ResultsGrid.DisplayLayout.Override.ActiveCellAppearance = appearance24;
            appearance20.BackColor = System.Drawing.SystemColors.Highlight;
            appearance20.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.ResultsGrid.DisplayLayout.Override.ActiveRowAppearance = appearance20;
            this.ResultsGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.ResultsGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.ResultsGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance27.BackColor = System.Drawing.SystemColors.Window;
            this.ResultsGrid.DisplayLayout.Override.CardAreaAppearance = appearance27;
            appearance23.BorderColor = System.Drawing.Color.Silver;
            appearance23.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.ResultsGrid.DisplayLayout.Override.CellAppearance = appearance23;
            this.ResultsGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.ResultsGrid.DisplayLayout.Override.CellPadding = 0;
            appearance21.BackColor = System.Drawing.SystemColors.Control;
            appearance21.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance21.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance21.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance21.BorderColor = System.Drawing.SystemColors.Window;
            this.ResultsGrid.DisplayLayout.Override.GroupByRowAppearance = appearance21;
            appearance22.TextHAlignAsString = "Left";
            this.ResultsGrid.DisplayLayout.Override.HeaderAppearance = appearance22;
            this.ResultsGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.ResultsGrid.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance25.BackColor = System.Drawing.SystemColors.Window;
            appearance25.BorderColor = System.Drawing.Color.Silver;
            this.ResultsGrid.DisplayLayout.Override.RowAppearance = appearance25;
            this.ResultsGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance26.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ResultsGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance26;
            this.ResultsGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ResultsGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.ResultsGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ResultsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResultsGrid.Location = new System.Drawing.Point(0, 0);
            this.ResultsGrid.Name = "ResultsGrid";
            this.ResultsGrid.Size = new System.Drawing.Size(716, 100);
            this.ResultsGrid.TabIndex = 0;
            this.ResultsGrid.Text = "ultraGrid1";
            // 
            // ultraSplitter1
            // 
            this.ultraSplitter1.BackColor = System.Drawing.SystemColors.Control;
            this.ultraSplitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraSplitter1.Location = new System.Drawing.Point(0, 73);
            this.ultraSplitter1.Name = "ultraSplitter1";
            this.ultraSplitter1.RestoreExtent = 73;
            this.ultraSplitter1.Size = new System.Drawing.Size(716, 10);
            this.ultraSplitter1.TabIndex = 4;
            // 
            // ultraPanel1
            // 
            // 
            // ultraPanel1.ClientArea
            // 
            this.ultraPanel1.ClientArea.Controls.Add(this.Municipals);
            this.ultraPanel1.ClientArea.Controls.Add(this.Quarter);
            this.ultraPanel1.ClientArea.Controls.Add(this.cbYears);
            this.ultraPanel1.ClientArea.Controls.Add(this.label1);
            this.ultraPanel1.ClientArea.Controls.Add(this.label2);
            this.ultraPanel1.ClientArea.Controls.Add(this.lYear);
            this.ultraPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraPanel1.Location = new System.Drawing.Point(0, 0);
            this.ultraPanel1.Name = "ultraPanel1";
            this.ultraPanel1.Size = new System.Drawing.Size(716, 73);
            this.ultraPanel1.TabIndex = 0;
            // 
            // Municipals
            // 
            appearance14.BackColor = System.Drawing.SystemColors.Window;
            this.Municipals.Appearance = appearance14;
            this.Municipals.BackColor = System.Drawing.SystemColors.Window;
            appearance15.FontData.BoldAsString = "True";
            editorButton1.Appearance = appearance15;
            editorButton1.Text = "...";
            this.Municipals.ButtonsRight.Add(editorButton1);
            this.Municipals.Location = new System.Drawing.Point(176, 43);
            this.Municipals.Name = "Municipals";
            this.Municipals.ReadOnly = true;
            this.Municipals.Size = new System.Drawing.Size(229, 19);
            this.Municipals.TabIndex = 10;
            this.Municipals.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            // 
            // Quarter
            // 
            this.Quarter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Quarter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Quarter.FormattingEnabled = true;
            this.Quarter.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.Quarter.Location = new System.Drawing.Point(211, 10);
            this.Quarter.Name = "Quarter";
            this.Quarter.Size = new System.Drawing.Size(56, 21);
            this.Quarter.TabIndex = 9;
            // 
            // cbYears
            // 
            this.cbYears.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbYears.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbYears.FormattingEnabled = true;
            this.cbYears.Items.AddRange(new object[] {
            "2000",
            "2001",
            "2002",
            "2003",
            "2004",
            "2005",
            "2006",
            "2007",
            "2008",
            "2009",
            "2010",
            "2011",
            "2012",
            "2013",
            "2014",
            "2015",
            "2016",
            "2017",
            "2018",
            "2019",
            "2020",
            "2021",
            "2022",
            "2023",
            "2024",
            "2025"});
            this.cbYears.Location = new System.Drawing.Point(43, 10);
            this.cbYears.Name = "cbYears";
            this.cbYears.Size = new System.Drawing.Size(101, 21);
            this.cbYears.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(156, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Квартал";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(158, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Муниципальные образования";
            // 
            // lYear
            // 
            this.lYear.AutoSize = true;
            this.lYear.Location = new System.Drawing.Point(12, 15);
            this.lYear.Name = "lYear";
            this.lYear.Size = new System.Drawing.Size(25, 13);
            this.lYear.TabIndex = 8;
            this.lYear.Text = "Год";
            // 
            // TaxpayersSumView
            // 
            this.Controls.Add(this.BaseView_Fill_Panel);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Bottom);
            this.Name = "TaxpayersSumView";
            this.Size = new System.Drawing.Size(716, 536);
            ((System.ComponentModel.ISupportInitialize)(this.tbManager)).EndInit();
            this.BaseView_Fill_Panel.ClientArea.ResumeLayout(false);
            this.BaseView_Fill_Panel.ResumeLayout(false);
            this.ultraPanel2.ClientArea.ResumeLayout(false);
            this.ultraPanel2.ResumeLayout(false);
            this.ultraPanel4.ClientArea.ResumeLayout(false);
            this.ultraPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TaxpayerDataGrid)).EndInit();
            this.ultraPanel3.ClientArea.ResumeLayout(false);
            this.ultraPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ResultsGrid)).EndInit();
            this.ultraPanel1.ClientArea.ResumeLayout(false);
            this.ultraPanel1.ClientArea.PerformLayout();
            this.ultraPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Municipals)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
