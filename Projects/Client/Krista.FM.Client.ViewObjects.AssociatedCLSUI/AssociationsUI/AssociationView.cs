using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.Misc;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Association
{
	public class AssociationView : Krista.FM.Client.ViewObjects.BaseViewObject.BaseView
    {
		//private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Left2;
		//private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Right2;
		//private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Top2;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Bottom2;
        internal ImageList ilTools;
        internal Infragistics.Win.UltraWinTabControl.UltraTabControl ultraTabControl1;
        //internal Infragistics.Win.UltraWinStatusBar.UltraStatusBar usbAssociateCount;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private Panel panel4;
        private Panel BaseView_Fill_Panel;
        internal SplitContainer scAssociation;
        internal Panel pDataCls;
        private Panel panel2;
        private Panel panel3;
        internal UltraLabel lAllDataClsAllSources;
        internal UltraLabel lAllDataCls;
        internal Panel panel1;
        internal UltraLabel lUnAssociateAllSources;
        internal UltraLabel lNotClsData;
        internal RadioButton rbCurrentAssociate;
        internal RadioButton rbUnAssociate;
        internal RadioButton rbAllRecords;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Top;
        internal Infragistics.Win.UltraWinToolbars.UltraToolbarsManager utbmAssociate;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Right;
        internal Infragistics.Win.UltraWinGrid.UltraGrid ultraGrid1;
		private System.ComponentModel.IContainer components = null;

		public AssociationView()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("Associations");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CreateBridge");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("AssociateMaster");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("HandAssociate");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ClearAllBridgeRef");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ClearCurentBridgeRef");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("AddToBridge");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Infragistics.Win.UltraWinToolbars.ButtonTool("AddToBridgeAll");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ShowBridgeRow");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool21 = new Infragistics.Win.UltraWinToolbars.ButtonTool("GetAssociationExcelReport");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ReplaceLink");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CreateBridge");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("AssociateMaster");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("HandAssociate");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ClearAllBridgeRef");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ClearCurentBridgeRef");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Infragistics.Win.UltraWinToolbars.ButtonTool("AddToBridge");
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ShowBridgeRow");
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Infragistics.Win.UltraWinToolbars.ButtonTool("GetAssociationExcelReport");
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Infragistics.Win.UltraWinToolbars.ButtonTool("AddToBridgeAll");
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ReplaceLink");
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssociationView));
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.panel4 = new System.Windows.Forms.Panel();
            this.BaseView_Fill_Panel = new System.Windows.Forms.Panel();
            this.scAssociation = new System.Windows.Forms.SplitContainer();
            this.pDataCls = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lAllDataClsAllSources = new Infragistics.Win.Misc.UltraLabel();
            this.lAllDataCls = new Infragistics.Win.Misc.UltraLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lUnAssociateAllSources = new Infragistics.Win.Misc.UltraLabel();
            this.lNotClsData = new Infragistics.Win.Misc.UltraLabel();
            this.rbCurrentAssociate = new System.Windows.Forms.RadioButton();
            this.rbUnAssociate = new System.Windows.Forms.RadioButton();
            this.rbAllRecords = new System.Windows.Forms.RadioButton();
            this._BaseView_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.utbmAssociate = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this.ilTools = new System.Windows.Forms.ImageList(this.components);
            this._BaseView_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseView_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseView_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.ultraGrid1 = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this._BaseView_Toolbars_Dock_Area_Bottom2 = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.ultraTabControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ultraTabPageControl1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.BaseView_Fill_Panel.SuspendLayout();
            this.scAssociation.Panel1.SuspendLayout();
            this.scAssociation.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.utbmAssociate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGrid1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl1)).BeginInit();
            this.ultraTabControl1.SuspendLayout();
            this.ultraTabSharedControlsPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.panel4);
            this.ultraTabPageControl1.Controls.Add(this.ultraGrid1);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(2, 21);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(660, 520);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.BaseView_Fill_Panel);
            this.panel4.Controls.Add(this._BaseView_Toolbars_Dock_Area_Top);
            this.panel4.Controls.Add(this._BaseView_Toolbars_Dock_Area_Bottom);
            this.panel4.Controls.Add(this._BaseView_Toolbars_Dock_Area_Left);
            this.panel4.Controls.Add(this._BaseView_Toolbars_Dock_Area_Right);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(660, 520);
            this.panel4.TabIndex = 0;
            // 
            // BaseView_Fill_Panel
            // 
            this.BaseView_Fill_Panel.Controls.Add(this.scAssociation);
            this.BaseView_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.BaseView_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BaseView_Fill_Panel.Location = new System.Drawing.Point(0, 27);
            this.BaseView_Fill_Panel.Name = "BaseView_Fill_Panel";
            this.BaseView_Fill_Panel.Size = new System.Drawing.Size(660, 493);
            this.BaseView_Fill_Panel.TabIndex = 8;
            // 
            // scAssociation
            // 
            this.scAssociation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scAssociation.Location = new System.Drawing.Point(0, 0);
            this.scAssociation.Name = "scAssociation";
            // 
            // scAssociation.Panel1
            // 
            this.scAssociation.Panel1.Controls.Add(this.pDataCls);
            this.scAssociation.Panel1.Controls.Add(this.panel2);
            this.scAssociation.Size = new System.Drawing.Size(660, 493);
            this.scAssociation.SplitterDistance = 320;
            this.scAssociation.TabIndex = 3;
            this.scAssociation.Text = "splitContainer1";
            // 
            // pDataCls
            // 
            this.pDataCls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pDataCls.Location = new System.Drawing.Point(0, 0);
            this.pDataCls.Name = "pDataCls";
            this.pDataCls.Size = new System.Drawing.Size(320, 421);
            this.pDataCls.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.rbCurrentAssociate);
            this.panel2.Controls.Add(this.rbUnAssociate);
            this.panel2.Controls.Add(this.rbAllRecords);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 421);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(320, 72);
            this.panel2.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.lAllDataClsAllSources);
            this.panel3.Controls.Add(this.lAllDataCls);
            this.panel3.Location = new System.Drawing.Point(160, 9);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(160, 32);
            this.panel3.TabIndex = 0;
            // 
            // lAllDataClsAllSources
            // 
            appearance9.TextHAlignAsString = "Left";
            appearance9.TextVAlignAsString = "Middle";
            this.lAllDataClsAllSources.Appearance = appearance9;
            this.lAllDataClsAllSources.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lAllDataClsAllSources.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lAllDataClsAllSources.Location = new System.Drawing.Point(0, 14);
            this.lAllDataClsAllSources.Name = "lAllDataClsAllSources";
            this.lAllDataClsAllSources.Size = new System.Drawing.Size(158, 16);
            this.lAllDataClsAllSources.TabIndex = 12;
            this.lAllDataClsAllSources.Text = "по всем источникам";
            // 
            // lAllDataCls
            // 
            appearance10.TextHAlignAsString = "Left";
            appearance10.TextVAlignAsString = "Middle";
            this.lAllDataCls.Appearance = appearance10;
            this.lAllDataCls.Dock = System.Windows.Forms.DockStyle.Top;
            this.lAllDataCls.Location = new System.Drawing.Point(0, 0);
            this.lAllDataCls.Name = "lAllDataCls";
            this.lAllDataCls.Size = new System.Drawing.Size(158, 16);
            this.lAllDataCls.TabIndex = 9;
            this.lAllDataCls.Text = "Всего записей";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.lUnAssociateAllSources);
            this.panel1.Controls.Add(this.lNotClsData);
            this.panel1.Location = new System.Drawing.Point(160, 40);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(160, 32);
            this.panel1.TabIndex = 9;
            // 
            // lUnAssociateAllSources
            // 
            appearance11.TextHAlignAsString = "Left";
            appearance11.TextVAlignAsString = "Middle";
            this.lUnAssociateAllSources.Appearance = appearance11;
            this.lUnAssociateAllSources.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lUnAssociateAllSources.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lUnAssociateAllSources.Location = new System.Drawing.Point(0, 14);
            this.lUnAssociateAllSources.Name = "lUnAssociateAllSources";
            this.lUnAssociateAllSources.Size = new System.Drawing.Size(158, 16);
            this.lUnAssociateAllSources.TabIndex = 11;
            this.lUnAssociateAllSources.Text = "по всем источникам";
            // 
            // lNotClsData
            // 
            appearance12.TextHAlignAsString = "Left";
            appearance12.TextVAlignAsString = "Middle";
            this.lNotClsData.Appearance = appearance12;
            this.lNotClsData.Dock = System.Windows.Forms.DockStyle.Top;
            this.lNotClsData.Location = new System.Drawing.Point(0, 0);
            this.lNotClsData.Name = "lNotClsData";
            this.lNotClsData.Size = new System.Drawing.Size(158, 16);
            this.lNotClsData.TabIndex = 10;
            this.lNotClsData.Text = "Не сопоставлено";
            // 
            // rbCurrentAssociate
            // 
            this.rbCurrentAssociate.AutoSize = true;
            this.rbCurrentAssociate.Location = new System.Drawing.Point(3, 49);
            this.rbCurrentAssociate.Name = "rbCurrentAssociate";
            this.rbCurrentAssociate.Size = new System.Drawing.Size(157, 17);
            this.rbCurrentAssociate.TabIndex = 8;
            this.rbCurrentAssociate.Text = "Сопоставленные текущей";
            // 
            // rbUnAssociate
            // 
            this.rbUnAssociate.AutoSize = true;
            this.rbUnAssociate.Location = new System.Drawing.Point(3, 26);
            this.rbUnAssociate.Name = "rbUnAssociate";
            this.rbUnAssociate.Size = new System.Drawing.Size(124, 17);
            this.rbUnAssociate.TabIndex = 7;
            this.rbUnAssociate.Text = "Несопоставленные";
            // 
            // rbAllRecords
            // 
            this.rbAllRecords.AutoSize = true;
            this.rbAllRecords.Checked = true;
            this.rbAllRecords.Location = new System.Drawing.Point(3, 3);
            this.rbAllRecords.Name = "rbAllRecords";
            this.rbAllRecords.Size = new System.Drawing.Size(83, 17);
            this.rbAllRecords.TabIndex = 6;
            this.rbAllRecords.TabStop = true;
            this.rbAllRecords.Text = "Все записи";
            // 
            // _BaseView_Toolbars_Dock_Area_Top
            // 
            this._BaseView_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._BaseView_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._BaseView_Toolbars_Dock_Area_Top.Name = "_BaseView_Toolbars_Dock_Area_Top";
            this._BaseView_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(660, 27);
            this._BaseView_Toolbars_Dock_Area_Top.ToolbarsManager = this.utbmAssociate;
            // 
            // utbmAssociate
            // 
            this.utbmAssociate.DesignerFlags = 0;
            this.utbmAssociate.DockWithinContainer = this;
            this.utbmAssociate.ImageListSmall = this.ilTools;
            this.utbmAssociate.ImageTransparentColor = System.Drawing.Color.White;
            this.utbmAssociate.MenuSettings.IsSideStripVisible = Infragistics.Win.DefaultableBoolean.False;
            this.utbmAssociate.RuntimeCustomizationOptions = Infragistics.Win.UltraWinToolbars.RuntimeCustomizationOptions.None;
            this.utbmAssociate.ShowFullMenusDelay = 500;
            this.utbmAssociate.ShowQuickCustomizeButton = false;
            ultraToolbar1.DockedColumn = 0;
            ultraToolbar1.DockedRow = 0;
            ultraToolbar1.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool2,
            buttonTool3,
            buttonTool4,
            buttonTool5,
            buttonTool6,
            buttonTool17,
            buttonTool20,
            buttonTool21,
            buttonTool22});
            ultraToolbar1.Text = "Сопоставление";
            this.utbmAssociate.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1});
            this.utbmAssociate.ToolbarSettings.AllowCustomize = Infragistics.Win.DefaultableBoolean.False;
            this.utbmAssociate.ToolbarSettings.AllowFloating = Infragistics.Win.DefaultableBoolean.False;
            this.utbmAssociate.ToolbarSettings.AllowHiding = Infragistics.Win.DefaultableBoolean.False;
            appearance1.Image = 2;
            buttonTool9.SharedPropsInternal.AppearancesSmall.Appearance = appearance1;
            buttonTool9.SharedPropsInternal.Caption = "HandAssociate";
            buttonTool9.SharedPropsInternal.ToolTipText = "Формирование сопоставимого классификатора";
            appearance2.Image = 0;
            buttonTool10.SharedPropsInternal.AppearancesSmall.Appearance = appearance2;
            buttonTool10.SharedPropsInternal.Caption = "CreateBridge";
            buttonTool10.SharedPropsInternal.ToolTipText = "Мастер автоматического сопоставления";
            appearance3.Image = 1;
            buttonTool11.SharedPropsInternal.AppearancesSmall.Appearance = appearance3;
            buttonTool11.SharedPropsInternal.Caption = "ClearAllBridgeRef";
            buttonTool11.SharedPropsInternal.ToolTipText = "Мастер ручного сопоставления";
            appearance4.Image = 3;
            buttonTool12.SharedPropsInternal.AppearancesSmall.Appearance = appearance4;
            buttonTool12.SharedPropsInternal.Caption = "ClearCurentBridgeRef";
            buttonTool12.SharedPropsInternal.ToolTipText = "Очистка сопоставления по текущему источнику";
            appearance5.Image = 4;
            buttonTool13.SharedPropsInternal.AppearancesSmall.Appearance = appearance5;
            buttonTool13.SharedPropsInternal.Caption = "ShowBridgeRow";
            buttonTool13.SharedPropsInternal.ToolTipText = "Очистка сопоставления текущей записи";
            appearance6.Image = 7;
            buttonTool14.SharedPropsInternal.AppearancesSmall.Appearance = appearance6;
            buttonTool14.SharedPropsInternal.Caption = "AddToBridge";
            buttonTool14.SharedPropsInternal.ToolTipText = "Добавить запись в сопоставимый классификатор";
            appearance7.Image = 6;
            buttonTool15.SharedPropsInternal.AppearancesSmall.Appearance = appearance7;
            buttonTool15.SharedPropsInternal.Caption = "AssociateMaster";
            buttonTool15.SharedPropsInternal.ToolTipText = "Поиск записи, которой сопоставлена текущая";
            appearance8.Image = 9;
            buttonTool16.SharedPropsInternal.AppearancesSmall.Appearance = appearance8;
            buttonTool16.SharedPropsInternal.ToolTipText = "Сохранить отчет по сопоставлению";
            appearance23.Image = 11;
            buttonTool19.SharedPropsInternal.AppearancesSmall.Appearance = appearance23;
            buttonTool19.SharedPropsInternal.Caption = "AddToBridgeAll";
            buttonTool19.SharedPropsInternal.ToolTipText = "Добавить все записи в сопоставимый классификатор";
            appearance24.Image = ((object)(resources.GetObject("appearance24.Image")));
            buttonTool18.SharedPropsInternal.AppearancesSmall.Appearance = appearance24;
            buttonTool18.SharedPropsInternal.Caption = "ReplaceLink";
            buttonTool18.SharedPropsInternal.ToolTipText = "Перенести ссылки на новую версию классификатора";
            this.utbmAssociate.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool9,
            buttonTool10,
            buttonTool11,
            buttonTool12,
            buttonTool13,
            buttonTool14,
            buttonTool15,
            buttonTool16,
            buttonTool19,
            buttonTool18});
            this.utbmAssociate.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.utbmAssociate_ToolClick);
            // 
            // ilTools
            // 
            this.ilTools.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilTools.ImageStream")));
            this.ilTools.TransparentColor = System.Drawing.Color.Magenta;
            this.ilTools.Images.SetKeyName(0, "AssociateClassifier.bmp");
            this.ilTools.Images.SetKeyName(1, "AssociateSingleRecord.bmp");
            this.ilTools.Images.SetKeyName(2, "CreateBridgeCls.bmp");
            this.ilTools.Images.SetKeyName(3, "ClearAllAssociate.bmp");
            this.ilTools.Images.SetKeyName(4, "ClearAssociate.bmp");
            this.ilTools.Images.SetKeyName(5, "RunFilter.bmp");
            this.ilTools.Images.SetKeyName(6, "FindBridgeAssociation.bmp");
            this.ilTools.Images.SetKeyName(7, "AddToBridge.bmp");
            this.ilTools.Images.SetKeyName(8, "AddToTransl.bmp");
            this.ilTools.Images.SetKeyName(9, "ExportExcel1.bmp");
            this.ilTools.Images.SetKeyName(10, "GotoAssociation.bmp");
            this.ilTools.Images.SetKeyName(11, "AddToBridgeAll.Bmp");
            // 
            // _BaseView_Toolbars_Dock_Area_Bottom
            // 
            this._BaseView_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._BaseView_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 520);
            this._BaseView_Toolbars_Dock_Area_Bottom.Name = "_BaseView_Toolbars_Dock_Area_Bottom";
            this._BaseView_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(660, 0);
            this._BaseView_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.utbmAssociate;
            // 
            // _BaseView_Toolbars_Dock_Area_Left
            // 
            this._BaseView_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._BaseView_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._BaseView_Toolbars_Dock_Area_Left.Name = "_BaseView_Toolbars_Dock_Area_Left";
            this._BaseView_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 520);
            this._BaseView_Toolbars_Dock_Area_Left.ToolbarsManager = this.utbmAssociate;
            // 
            // _BaseView_Toolbars_Dock_Area_Right
            // 
            this._BaseView_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._BaseView_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(660, 0);
            this._BaseView_Toolbars_Dock_Area_Right.Name = "_BaseView_Toolbars_Dock_Area_Right";
            this._BaseView_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 520);
            this._BaseView_Toolbars_Dock_Area_Right.ToolbarsManager = this.utbmAssociate;
            // 
            // ultraGrid1
            // 
            this.ultraGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGrid1.Location = new System.Drawing.Point(0, 0);
            this.ultraGrid1.Name = "ultraGrid1";
            this.ultraGrid1.Size = new System.Drawing.Size(660, 520);
            this.ultraGrid1.TabIndex = 0;
            this.ultraGrid1.Text = "ultraGrid1";
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(660, 520);
            // 
            // _BaseView_Toolbars_Dock_Area_Bottom2
            // 
            this._BaseView_Toolbars_Dock_Area_Bottom2.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Bottom2.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Bottom2.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._BaseView_Toolbars_Dock_Area_Bottom2.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Bottom2.Location = new System.Drawing.Point(0, 543);
            this._BaseView_Toolbars_Dock_Area_Bottom2.Name = "_BaseView_Toolbars_Dock_Area_Bottom2";
            this._BaseView_Toolbars_Dock_Area_Bottom2.Size = new System.Drawing.Size(664, 0);
            this._BaseView_Toolbars_Dock_Area_Bottom2.ToolbarsManager = this.utbmAssociate;
            // 
            // ultraTabControl1
            // 
            this.ultraTabControl1.Controls.Add(this.ultraTabSharedControlsPage1);
            this.ultraTabControl1.Controls.Add(this.ultraTabPageControl1);
            this.ultraTabControl1.Controls.Add(this.ultraTabPageControl2);
            this.ultraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraTabControl1.Location = new System.Drawing.Point(0, 0);
            this.ultraTabControl1.Name = "ultraTabControl1";
            this.ultraTabControl1.SharedControls.AddRange(new System.Windows.Forms.Control[] {
            this.ultraGrid1});
            this.ultraTabControl1.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.ultraTabControl1.Size = new System.Drawing.Size(664, 543);
            this.ultraTabControl1.TabIndex = 6;
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "Данные";
            ultraTab2.TabPage = this.ultraTabPageControl2;
            ultraTab2.Text = "Таблицы перекодировок";
            this.ultraTabControl1.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2});
            this.ultraTabControl1.ViewStyle = Infragistics.Win.UltraWinTabControl.ViewStyle.VisualStudio2005;
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Controls.Add(this.ultraGrid1);
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(660, 520);
            // 
            // AssociationView
            // 
            this.Controls.Add(this.ultraTabControl1);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Bottom2);
            this.Name = "AssociationView";
            this.Size = new System.Drawing.Size(664, 543);
            this.ultraTabPageControl1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.BaseView_Fill_Panel.ResumeLayout(false);
            this.scAssociation.Panel1.ResumeLayout(false);
            this.scAssociation.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.utbmAssociate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGrid1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl1)).EndInit();
            this.ultraTabControl1.ResumeLayout(false);
            this.ultraTabSharedControlsPage1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void lNotClsData_Click(object sender, EventArgs e)
		{

		}

        private void utbmAssociate_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {

        }
	}
}

