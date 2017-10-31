using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Itenso.Configuration;
using Krista.FM.Client.Common;
using CC = Krista.FM.Client.Components;
using Krista.FM.Client.Common.Configuration;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls
{
	public class BaseClsView : BaseViewObject.BaseView
	{
		public Infragistics.Win.UltraWinDataSource.UltraDataSource udsAssociations;
		public Infragistics.Win.UltraWinTabControl.UltraTabControl utcDataCls;
		public Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
		public Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage2;
		public Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpData;
        public Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpAssociate;
        public Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpProtocol;
        protected internal ImageList ilImages;
		public Infragistics.Win.UltraWinToolbars.UltraToolbarsManager utbToolbarManager;
		private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _utpData_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _utpData_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _utpData_Toolbars_Dock_Area_Bottom;
        public Panel pnDataTemplate;
		private Panel pnDataTemplate_Fill_Panel;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _pnDataTemplate_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _pnDataTemplate_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _pnDataTemplate_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _pnDataTemplate_Toolbars_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _utpData_Toolbars_Dock_Area_Top;
        internal ContextMenuStrip cmsAudit;
        internal ContextMenuStrip cmsAuditSchemeObject;
        internal ToolStripMenuItem ‡Û‰ËÚToolStripMenuItem;
        internal ToolStripMenuItem ‡Û‰ËÚSchemeObjectToolStripMenuItem;
        internal SplitContainer spMasterDetail;
        public Krista.FM.Client.Components.UltraGridEx ugeCls;
        public Infragistics.Win.UltraWinTabControl.UltraTabControl utcDetails;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage5;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage6;
        private Panel panel1;
        internal Infragistics.Win.UltraWinTabControl.UltraTabControl utcLogSwitcher;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage3;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage4;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        internal Panel pClassifiers;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        internal Panel pAssociate;
        internal ContextMenuStrip cmsCreateDocument;
        private ToolStripMenuItem tsmiSelectDocument;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem tsmiCreateNewExcel;
        private ToolStripMenuItem tsmiCreateNewWord;
        private ToolStripMenuItem tsmiDeleteDocument;
        private ToolStripSeparator toolStripMenuItem2;
        private Panel panel2;
        public UltraGrid ugAssociations;
        private Panel panel2_Fill_Panel;
		private IContainer components;

		public BaseClsView()
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
					SavePersistence();
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
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("utbFilters");
            Infragistics.Win.UltraWinToolbars.ComboBoxTool comboBoxTool1 = new Infragistics.Win.UltraWinToolbars.ComboBoxTool("cbDataSources");
            Infragistics.Win.UltraWinToolbars.LabelTool labelTool1 = new Infragistics.Win.UltraWinToolbars.LabelTool("laSourceID");
            Infragistics.Win.UltraWinToolbars.LabelTool labelTool8 = new Infragistics.Win.UltraWinToolbars.LabelTool("lbPresentationKey");
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar2 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("tbDisinHierarchy");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("disin");
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar3 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("SelectTask");
            Infragistics.Win.UltraWinToolbars.ComboBoxTool comboBoxTool2 = new Infragistics.Win.UltraWinToolbars.ComboBoxTool("TaskIDCombo");
            Infragistics.Win.UltraWinToolbars.LabelTool labelTool2 = new Infragistics.Win.UltraWinToolbars.LabelTool("lbTaskID");
            Infragistics.Win.UltraWinToolbars.ComboBoxTool comboBoxTool3 = new Infragistics.Win.UltraWinToolbars.ComboBoxTool("cbDataSources");
            Infragistics.Win.ValueList valueList1 = new Infragistics.Win.ValueList(0);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.UltraWinToolbars.LabelTool labelTool3 = new Infragistics.Win.UltraWinToolbars.LabelTool("laSourceID");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("disin");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ComboBoxTool comboBoxTool4 = new Infragistics.Win.UltraWinToolbars.ComboBoxTool("TaskIDCombo");
            Infragistics.Win.ValueList valueList2 = new Infragistics.Win.ValueList(0);
            Infragistics.Win.UltraWinToolbars.LabelTool labelTool4 = new Infragistics.Win.UltraWinToolbars.LabelTool("lbTaskID");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnSelectVariant");
            Infragistics.Win.UltraWinToolbars.LabelTool labelTool6 = new Infragistics.Win.UltraWinToolbars.LabelTool("lVariantCaption");
            Infragistics.Win.UltraWinToolbars.LabelTool labelTool7 = new Infragistics.Win.UltraWinToolbars.LabelTool("lbVersion");
            Infragistics.Win.UltraWinToolbars.LabelTool labelTool9 = new Infragistics.Win.UltraWinToolbars.LabelTool("lbPresentationKey");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseClsView));
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("GoToAssociation");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("RoleData");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("RoleDataCaption");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("RoleBridge");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("RoleBridgeCaption");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn6 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("AssociatedRecCnt");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn7 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("UnassociatedRecCnt");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn8 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("AssociationName");
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab4 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab5 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.pClassifiers = new System.Windows.Forms.Panel();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.pAssociate = new System.Windows.Forms.Panel();
            this.utpData = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.pnDataTemplate = new System.Windows.Forms.Panel();
            this.pnDataTemplate_Fill_Panel = new System.Windows.Forms.Panel();
            this.spMasterDetail = new System.Windows.Forms.SplitContainer();
            this.ugeCls = new Krista.FM.Client.Components.UltraGridEx();
            this.utcDetails = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage5 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ultraTabSharedControlsPage6 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this._pnDataTemplate_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.utbToolbarManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this.ilImages = new System.Windows.Forms.ImageList(this.components);
            this._pnDataTemplate_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._pnDataTemplate_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._utpData_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._utpData_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._utpData_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._utpData_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.utpAssociate = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel2_Fill_Panel = new System.Windows.Forms.Panel();
            this.ugAssociations = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.utpProtocol = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.utcLogSwitcher = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage3 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ultraTabSharedControlsPage4 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.udsAssociations = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            this.utcDataCls = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ultraTabSharedControlsPage2 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.cmsAudit = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.‡Û‰ËÚToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsAuditSchemeObject = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.‡Û‰ËÚSchemeObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCreateDocument = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiDeleteDocument = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiSelectDocument = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCreateNewExcel = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateNewWord = new System.Windows.Forms.ToolStripMenuItem();
            this.ultraTabPageControl1.SuspendLayout();
            this.ultraTabPageControl2.SuspendLayout();
            this.utpData.SuspendLayout();
            this.pnDataTemplate.SuspendLayout();
            this.pnDataTemplate_Fill_Panel.SuspendLayout();
            this.spMasterDetail.Panel1.SuspendLayout();
            this.spMasterDetail.Panel2.SuspendLayout();
            this.spMasterDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.utcDetails)).BeginInit();
            this.utcDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.utbToolbarManager)).BeginInit();
            this.utpAssociate.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel2_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugAssociations)).BeginInit();
            this.utpProtocol.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.utcLogSwitcher)).BeginInit();
            this.utcLogSwitcher.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udsAssociations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.utcDataCls)).BeginInit();
            this.utcDataCls.SuspendLayout();
            this.cmsAudit.SuspendLayout();
            this.cmsAuditSchemeObject.SuspendLayout();
            this.cmsCreateDocument.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.pClassifiers);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(1, 1);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(681, 432);
            // 
            // pClassifiers
            // 
            this.pClassifiers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pClassifiers.Location = new System.Drawing.Point(0, 0);
            this.pClassifiers.Name = "pClassifiers";
            this.pClassifiers.Size = new System.Drawing.Size(681, 432);
            this.pClassifiers.TabIndex = 0;
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.pAssociate);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(681, 432);
            // 
            // pAssociate
            // 
            this.pAssociate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pAssociate.Location = new System.Drawing.Point(0, 0);
            this.pAssociate.Name = "pAssociate";
            this.pAssociate.Size = new System.Drawing.Size(681, 432);
            this.pAssociate.TabIndex = 0;
            // 
            // utpData
            // 
            this.utpData.Controls.Add(this.pnDataTemplate);
            this.utpData.Controls.Add(this._utpData_Toolbars_Dock_Area_Left);
            this.utpData.Controls.Add(this._utpData_Toolbars_Dock_Area_Right);
            this.utpData.Controls.Add(this._utpData_Toolbars_Dock_Area_Top);
            this.utpData.Controls.Add(this._utpData_Toolbars_Dock_Area_Bottom);
            this.utpData.Location = new System.Drawing.Point(2, 21);
            this.utpData.Name = "utpData";
            this.utpData.Size = new System.Drawing.Size(683, 453);
            // 
            // pnDataTemplate
            // 
            this.pnDataTemplate.Controls.Add(this.pnDataTemplate_Fill_Panel);
            this.pnDataTemplate.Controls.Add(this._pnDataTemplate_Toolbars_Dock_Area_Left);
            this.pnDataTemplate.Controls.Add(this._pnDataTemplate_Toolbars_Dock_Area_Right);
            this.pnDataTemplate.Controls.Add(this._pnDataTemplate_Toolbars_Dock_Area_Top);
            this.pnDataTemplate.Controls.Add(this._pnDataTemplate_Toolbars_Dock_Area_Bottom);
            this.pnDataTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnDataTemplate.Location = new System.Drawing.Point(0, 0);
            this.pnDataTemplate.Name = "pnDataTemplate";
            this.pnDataTemplate.Size = new System.Drawing.Size(683, 453);
            this.pnDataTemplate.TabIndex = 10;
            // 
            // pnDataTemplate_Fill_Panel
            // 
            this.pnDataTemplate_Fill_Panel.Controls.Add(this.spMasterDetail);
            this.pnDataTemplate_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.pnDataTemplate_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnDataTemplate_Fill_Panel.Location = new System.Drawing.Point(0, 46);
            this.pnDataTemplate_Fill_Panel.Name = "pnDataTemplate_Fill_Panel";
            this.pnDataTemplate_Fill_Panel.Size = new System.Drawing.Size(683, 407);
            this.pnDataTemplate_Fill_Panel.TabIndex = 0;
            // 
            // spMasterDetail
            // 
            this.spMasterDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spMasterDetail.Location = new System.Drawing.Point(0, 0);
            this.spMasterDetail.Name = "spMasterDetail";
            this.spMasterDetail.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spMasterDetail.Panel1
            // 
            this.spMasterDetail.Panel1.Controls.Add(this.ugeCls);
            // 
            // spMasterDetail.Panel2
            // 
            this.spMasterDetail.Panel2.Controls.Add(this.utcDetails);
            this.spMasterDetail.Size = new System.Drawing.Size(683, 407);
            this.spMasterDetail.SplitterDistance = 206;
            this.spMasterDetail.TabIndex = 0;
            // 
            // ugeCls
            // 
            this.ugeCls.AllowAddNewRecords = true;
            this.ugeCls.AllowClearTable = true;
            this.ugeCls.Caption = "";
            this.ugeCls.Cursor = System.Windows.Forms.Cursors.Default;
            this.ugeCls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugeCls.InDebugMode = false;
            this.ugeCls.LoadMenuVisible = false;
            this.ugeCls.Location = new System.Drawing.Point(0, 0);
            this.ugeCls.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ugeCls.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ugeCls.Name = "ugeCls";
            this.ugeCls.SaveLoadFileName = "";
            this.ugeCls.SaveMenuVisible = false;
            this.ugeCls.ServerFilterEnabled = false;
            this.ugeCls.SingleBandLevelName = "";
            this.ugeCls.Size = new System.Drawing.Size(683, 206);
            this.ugeCls.sortColumnName = "";
            this.ugeCls.StateRowEnable = false;
            this.ugeCls.TabIndex = 1;
            this.ugeCls.Load += new System.EventHandler(this.ugeCls_Load);
            // 
            // utcDetails
            // 
            this.utcDetails.Controls.Add(this.ultraTabSharedControlsPage5);
            this.utcDetails.Controls.Add(this.ultraTabSharedControlsPage6);
            this.utcDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.utcDetails.Location = new System.Drawing.Point(0, 0);
            this.utcDetails.Name = "utcDetails";
            this.utcDetails.SharedControlsPage = this.ultraTabSharedControlsPage5;
            this.utcDetails.ShowTabListButton = Infragistics.Win.DefaultableBoolean.True;
            this.utcDetails.Size = new System.Drawing.Size(683, 197);
            this.utcDetails.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Excel;
            this.utcDetails.TabIndex = 13;
            this.utcDetails.TabLayoutStyle = Infragistics.Win.UltraWinTabs.TabLayoutStyle.MultiRowAutoSize;
            this.utcDetails.TabOrientation = Infragistics.Win.UltraWinTabs.TabOrientation.TopLeft;
            // 
            // ultraTabSharedControlsPage5
            // 
            this.ultraTabSharedControlsPage5.Location = new System.Drawing.Point(1, 20);
            this.ultraTabSharedControlsPage5.Name = "ultraTabSharedControlsPage5";
            this.ultraTabSharedControlsPage5.Size = new System.Drawing.Size(681, 176);
            // 
            // ultraTabSharedControlsPage6
            // 
            this.ultraTabSharedControlsPage6.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage6.Name = "ultraTabSharedControlsPage6";
            this.ultraTabSharedControlsPage6.Size = new System.Drawing.Size(413, 444);
            // 
            // _pnDataTemplate_Toolbars_Dock_Area_Left
            // 
            this._pnDataTemplate_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._pnDataTemplate_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._pnDataTemplate_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._pnDataTemplate_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._pnDataTemplate_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 46);
            this._pnDataTemplate_Toolbars_Dock_Area_Left.Name = "_pnDataTemplate_Toolbars_Dock_Area_Left";
            this._pnDataTemplate_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 407);
            this._pnDataTemplate_Toolbars_Dock_Area_Left.ToolbarsManager = this.utbToolbarManager;
            // 
            // utbToolbarManager
            // 
            this.utbToolbarManager.DesignerFlags = 1;
            this.utbToolbarManager.DockWithinContainer = this.pnDataTemplate;
            this.utbToolbarManager.ImageListSmall = this.ilImages;
            this.utbToolbarManager.RightAlignedMenus = Infragistics.Win.DefaultableBoolean.False;
            this.utbToolbarManager.RuntimeCustomizationOptions = Infragistics.Win.UltraWinToolbars.RuntimeCustomizationOptions.None;
            this.utbToolbarManager.ShowFullMenusDelay = 500;
            this.utbToolbarManager.ShowQuickCustomizeButton = false;
            ultraToolbar1.DockedColumn = 0;
            ultraToolbar1.DockedRow = 0;
            comboBoxTool1.InstanceProps.Width = 251;
            labelTool1.InstanceProps.Width = 106;
            ultraToolbar1.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            comboBoxTool1,
            labelTool1,
            labelTool8});
            ultraToolbar1.Settings.AllowCustomize = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowDockBottom = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowDockLeft = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowDockRight = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowDockTop = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowFloating = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowHiding = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.ShowInToolbarList = false;
            ultraToolbar1.Text = "‘ËÎ¸Ú˚";
            ultraToolbar2.DockedColumn = 0;
            ultraToolbar2.DockedRow = 0;
            ultraToolbar2.IsStockToolbar = false;
            ultraToolbar2.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1});
            ultraToolbar2.Settings.AllowCustomize = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar2.Settings.AllowDockBottom = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar2.Settings.AllowDockLeft = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar2.Settings.AllowDockRight = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar2.Settings.AllowDockTop = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar2.Settings.AllowFloating = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar2.Settings.AllowHiding = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar2.Text = "›ÍÒÔÓÚ/»ÏÔÓÚ";
            ultraToolbar2.Visible = false;
            ultraToolbar3.DockedColumn = 0;
            ultraToolbar3.DockedRow = 1;
            ultraToolbar3.FloatingLocation = new System.Drawing.Point(284, 313);
            ultraToolbar3.FloatingSize = new System.Drawing.Size(324, 22);
            comboBoxTool2.InstanceProps.Width = 252;
            labelTool2.InstanceProps.Width = 99;
            ultraToolbar3.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            comboBoxTool2,
            labelTool2});
            ultraToolbar3.Text = "¬˚·Ó Á‡‰‡˜";
            this.utbToolbarManager.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1,
            ultraToolbar2,
            ultraToolbar3});
            this.utbToolbarManager.ToolbarSettings.AllowCustomize = Infragistics.Win.DefaultableBoolean.False;
            this.utbToolbarManager.ToolbarSettings.AllowFloating = Infragistics.Win.DefaultableBoolean.False;
            this.utbToolbarManager.ToolbarSettings.AllowHiding = Infragistics.Win.DefaultableBoolean.False;
            comboBoxTool3.SharedPropsInternal.Caption = "»ÒÚÓ˜ÌËÍ ‰‡ÌÌ˚ı";
            valueListItem1.DataValue = "ValueListItem0";
            valueListItem1.DisplayText = "1212";
            valueListItem2.DataValue = "ValueListItem1";
            valueListItem2.DisplayText = "1212";
            valueListItem3.DataValue = "ValueListItem2";
            valueListItem3.DisplayText = "1211";
            valueList1.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2,
            valueListItem3});
            comboBoxTool3.ValueList = valueList1;
            labelTool3.SharedPropsInternal.Caption = "(SourceID = ????)";
            labelTool3.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.TextOnlyAlways;
            appearance1.Image = 10;
            buttonTool2.SharedPropsInternal.AppearancesSmall.Appearance = appearance1;
            buttonTool2.SharedPropsInternal.Caption = "ButtonTool1";
            buttonTool2.SharedPropsInternal.ToolTipText = "”ÒÚ‡ÌÓ‚ËÚ¸ ËÂ‡ıË˛";
            comboBoxTool4.SharedPropsInternal.ToolTipText = "¬˚·Ó Á‡‰‡˜Ë";
            comboBoxTool4.ValueList = valueList2;
            labelTool4.SharedPropsInternal.AllowMultipleInstances = false;
            labelTool4.SharedPropsInternal.Caption = "(TaskID = ????)";
            labelTool4.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.TextOnlyAlways;
            buttonTool4.SharedPropsInternal.Caption = "ButtonTool1";
            labelTool6.SharedPropsInternal.Caption = "LabelTool1";
            labelTool7.SharedPropsInternal.Caption = "(VersionID = ????)";
            labelTool9.SharedPropsInternal.Caption = "(Presentation = ????)";
            this.utbToolbarManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            comboBoxTool3,
            labelTool3,
            buttonTool2,
            comboBoxTool4,
            labelTool4,
            buttonTool4,
            labelTool6,
            labelTool7,
            labelTool9});
            // 
            // ilImages
            // 
            this.ilImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilImages.ImageStream")));
            this.ilImages.TransparentColor = System.Drawing.Color.Magenta;
            this.ilImages.Images.SetKeyName(0, "");
            this.ilImages.Images.SetKeyName(1, "");
            this.ilImages.Images.SetKeyName(2, "");
            this.ilImages.Images.SetKeyName(3, "");
            this.ilImages.Images.SetKeyName(4, "");
            this.ilImages.Images.SetKeyName(5, "");
            this.ilImages.Images.SetKeyName(6, "");
            this.ilImages.Images.SetKeyName(7, "");
            this.ilImages.Images.SetKeyName(8, "");
            this.ilImages.Images.SetKeyName(9, "");
            this.ilImages.Images.SetKeyName(10, "");
            this.ilImages.Images.SetKeyName(11, "");
            this.ilImages.Images.SetKeyName(12, "Check.bmp");
            this.ilImages.Images.SetKeyName(13, "ProtectForm.bmp");
            this.ilImages.Images.SetKeyName(14, "Copy.bmp");
            this.ilImages.Images.SetKeyName(15, "Audit.bmp");
            this.ilImages.Images.SetKeyName(16, "thumbtack.bmp");
            this.ilImages.Images.SetKeyName(17, "SaveDoc.bmp");
            this.ilImages.Images.SetKeyName(18, "OpenDoc.bmp");
            this.ilImages.Images.SetKeyName(19, "CreateDoc.bmp");
            this.ilImages.Images.SetKeyName(20, "pump_Normatives_16.bmp");
            this.ilImages.Images.SetKeyName(21, "AddTopLevel.bmp");
            this.ilImages.Images.SetKeyName(22, "AddChild.bmp");
            // 
            // _pnDataTemplate_Toolbars_Dock_Area_Right
            // 
            this._pnDataTemplate_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._pnDataTemplate_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._pnDataTemplate_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._pnDataTemplate_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._pnDataTemplate_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(683, 46);
            this._pnDataTemplate_Toolbars_Dock_Area_Right.Name = "_pnDataTemplate_Toolbars_Dock_Area_Right";
            this._pnDataTemplate_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 407);
            this._pnDataTemplate_Toolbars_Dock_Area_Right.ToolbarsManager = this.utbToolbarManager;
            // 
            // _pnDataTemplate_Toolbars_Dock_Area_Top
            // 
            this._pnDataTemplate_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._pnDataTemplate_Toolbars_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(245)))), ((int)(((byte)(244)))));
            this._pnDataTemplate_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._pnDataTemplate_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._pnDataTemplate_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._pnDataTemplate_Toolbars_Dock_Area_Top.Name = "_pnDataTemplate_Toolbars_Dock_Area_Top";
            this._pnDataTemplate_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(683, 46);
            this._pnDataTemplate_Toolbars_Dock_Area_Top.ToolbarsManager = this.utbToolbarManager;
            // 
            // _pnDataTemplate_Toolbars_Dock_Area_Bottom
            // 
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 453);
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom.Name = "_pnDataTemplate_Toolbars_Dock_Area_Bottom";
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(683, 0);
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.utbToolbarManager;
            // 
            // _utpData_Toolbars_Dock_Area_Left
            // 
            this._utpData_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._utpData_Toolbars_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(245)))), ((int)(((byte)(244)))));
            this._utpData_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._utpData_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._utpData_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._utpData_Toolbars_Dock_Area_Left.Name = "_utpData_Toolbars_Dock_Area_Left";
            this._utpData_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 453);
            this._utpData_Toolbars_Dock_Area_Left.ToolbarsManager = this.utbToolbarManager;
            // 
            // _utpData_Toolbars_Dock_Area_Right
            // 
            this._utpData_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._utpData_Toolbars_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(245)))), ((int)(((byte)(244)))));
            this._utpData_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._utpData_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._utpData_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(683, 0);
            this._utpData_Toolbars_Dock_Area_Right.Name = "_utpData_Toolbars_Dock_Area_Right";
            this._utpData_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 453);
            this._utpData_Toolbars_Dock_Area_Right.ToolbarsManager = this.utbToolbarManager;
            // 
            // _utpData_Toolbars_Dock_Area_Top
            // 
            this._utpData_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._utpData_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._utpData_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._utpData_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._utpData_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._utpData_Toolbars_Dock_Area_Top.Name = "_utpData_Toolbars_Dock_Area_Top";
            this._utpData_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(683, 0);
            // 
            // _utpData_Toolbars_Dock_Area_Bottom
            // 
            this._utpData_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._utpData_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(245)))), ((int)(((byte)(244)))));
            this._utpData_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._utpData_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._utpData_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 453);
            this._utpData_Toolbars_Dock_Area_Bottom.Name = "_utpData_Toolbars_Dock_Area_Bottom";
            this._utpData_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(683, 0);
            this._utpData_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.utbToolbarManager;
            // 
            // utpAssociate
            // 
            this.utpAssociate.Controls.Add(this.panel2);
            this.utpAssociate.Location = new System.Drawing.Point(-10000, -10000);
            this.utpAssociate.Name = "utpAssociate";
            this.utpAssociate.Size = new System.Drawing.Size(683, 453);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel2_Fill_Panel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(683, 453);
            this.panel2.TabIndex = 0;
            // 
            // panel2_Fill_Panel
            // 
            this.panel2_Fill_Panel.Controls.Add(this.ugAssociations);
            this.panel2_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.panel2_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.panel2_Fill_Panel.Name = "panel2_Fill_Panel";
            this.panel2_Fill_Panel.Size = new System.Drawing.Size(683, 453);
            this.panel2_Fill_Panel.TabIndex = 0;
            // 
            // ugAssociations
            // 
            this.ugAssociations.Cursor = System.Windows.Forms.Cursors.Default;
            this.ugAssociations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugAssociations.Location = new System.Drawing.Point(0, 0);
            this.ugAssociations.Name = "ugAssociations";
            this.ugAssociations.Size = new System.Drawing.Size(683, 453);
            this.ugAssociations.TabIndex = 2;
            // 
            // utpProtocol
            // 
            this.utpProtocol.Controls.Add(this.panel1);
            this.utpProtocol.Location = new System.Drawing.Point(-10000, -10000);
            this.utpProtocol.Name = "utpProtocol";
            this.utpProtocol.Size = new System.Drawing.Size(683, 453);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.utcLogSwitcher);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(683, 453);
            this.panel1.TabIndex = 0;
            // 
            // utcLogSwitcher
            // 
            this.utcLogSwitcher.Controls.Add(this.ultraTabSharedControlsPage3);
            this.utcLogSwitcher.Controls.Add(this.ultraTabSharedControlsPage4);
            this.utcLogSwitcher.Controls.Add(this.ultraTabPageControl1);
            this.utcLogSwitcher.Controls.Add(this.ultraTabPageControl2);
            this.utcLogSwitcher.Dock = System.Windows.Forms.DockStyle.Fill;
            this.utcLogSwitcher.Location = new System.Drawing.Point(0, 0);
            this.utcLogSwitcher.Name = "utcLogSwitcher";
            this.utcLogSwitcher.SharedControlsPage = this.ultraTabSharedControlsPage3;
            this.utcLogSwitcher.Size = new System.Drawing.Size(683, 453);
            this.utcLogSwitcher.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Excel;
            this.utcLogSwitcher.TabIndex = 14;
            this.utcLogSwitcher.TabOrientation = Infragistics.Win.UltraWinTabs.TabOrientation.BottomLeft;
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = " Î‡ÒÒËÙËÍ‡ÚÓ˚ Ë Ú‡·ÎËˆ˚";
            ultraTab2.TabPage = this.ultraTabPageControl2;
            ultraTab2.Text = "—ÓÔÓÒÚ‡‚ÎÂÌËÂ ÍÎ‡ÒÒËÙËÍ‡ÚÓÓ‚";
            this.utcLogSwitcher.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2});
            this.utcLogSwitcher.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            // 
            // ultraTabSharedControlsPage3
            // 
            this.ultraTabSharedControlsPage3.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage3.Name = "ultraTabSharedControlsPage3";
            this.ultraTabSharedControlsPage3.Size = new System.Drawing.Size(681, 432);
            // 
            // ultraTabSharedControlsPage4
            // 
            this.ultraTabSharedControlsPage4.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage4.Name = "ultraTabSharedControlsPage4";
            this.ultraTabSharedControlsPage4.Size = new System.Drawing.Size(413, 444);
            // 
            // udsAssociations
            // 
            this.udsAssociations.AllowAdd = false;
            this.udsAssociations.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4,
            ultraDataColumn5,
            ultraDataColumn6,
            ultraDataColumn7,
            ultraDataColumn8});
            this.udsAssociations.Band.Key = "Main";
            this.udsAssociations.ReadOnly = true;
            // 
            // utcDataCls
            // 
            this.utcDataCls.Controls.Add(this.ultraTabSharedControlsPage1);
            this.utcDataCls.Controls.Add(this.ultraTabSharedControlsPage2);
            this.utcDataCls.Controls.Add(this.utpData);
            this.utcDataCls.Controls.Add(this.utpAssociate);
            this.utcDataCls.Controls.Add(this.utpProtocol);
            this.utcDataCls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.utcDataCls.Location = new System.Drawing.Point(0, 0);
            this.utcDataCls.Name = "utcDataCls";
            this.utcDataCls.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.utcDataCls.Size = new System.Drawing.Size(687, 476);
            this.utcDataCls.TabIndex = 1;
            ultraTab3.TabPage = this.utpData;
            ultraTab3.Text = "ƒ‡ÌÌ˚Â";
            ultraTab4.TabPage = this.utpAssociate;
            ultraTab4.Text = "—ÓÔÓÒÚ‡‚ÎÂÌËÂ";
            ultraTab5.TabPage = this.utpProtocol;
            ultraTab5.Text = "œÓÚÓÍÓÎ˚";
            this.utcDataCls.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab3,
            ultraTab4,
            ultraTab5});
            this.utcDataCls.ViewStyle = Infragistics.Win.UltraWinTabControl.ViewStyle.VisualStudio2005;
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(683, 453);
            // 
            // ultraTabSharedControlsPage2
            // 
            this.ultraTabSharedControlsPage2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage2.Name = "ultraTabSharedControlsPage2";
            this.ultraTabSharedControlsPage2.Size = new System.Drawing.Size(747, 614);
            // 
            // cmsAudit
            // 
            this.cmsAudit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.‡Û‰ËÚToolStripMenuItem});
            this.cmsAudit.Name = "cmsAudit";
            this.cmsAudit.Size = new System.Drawing.Size(184, 26);
            // 
            // ‡Û‰ËÚToolStripMenuItem
            // 
            this.‡Û‰ËÚToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("‡Û‰ËÚToolStripMenuItem.Image")));
            this.‡Û‰ËÚToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.‡Û‰ËÚToolStripMenuItem.Name = "‡Û‰ËÚToolStripMenuItem";
            this.‡Û‰ËÚToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.‡Û‰ËÚToolStripMenuItem.Text = "»ÒÚÓËˇ ËÁÏÂÌÂÌËÈ";
            // 
            // cmsAuditSchemeObject
            // 
            this.cmsAuditSchemeObject.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.‡Û‰ËÚSchemeObjectToolStripMenuItem});
            this.cmsAuditSchemeObject.Name = "cmsAuditSchemeObject";
            this.cmsAuditSchemeObject.Size = new System.Drawing.Size(271, 26);
            // 
            // ‡Û‰ËÚSchemeObjectToolStripMenuItem
            // 
            this.‡Û‰ËÚSchemeObjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("‡Û‰ËÚSchemeObjectToolStripMenuItem.Image")));
            this.‡Û‰ËÚSchemeObjectToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.‡Û‰ËÚSchemeObjectToolStripMenuItem.Name = "‡Û‰ËÚSchemeObjectToolStripMenuItem";
            this.‡Û‰ËÚSchemeObjectToolStripMenuItem.Size = new System.Drawing.Size(270, 22);
            this.‡Û‰ËÚSchemeObjectToolStripMenuItem.Text = "»ÒÚÓËˇ ËÁÏÂÌÂÌËÈ ÍÎ‡ÒÒËÙËÍ‡ÚÓ‡";
            // 
            // cmsCreateDocument
            // 
            this.cmsCreateDocument.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiDeleteDocument,
            this.toolStripMenuItem1,
            this.tsmiSelectDocument,
            this.toolStripMenuItem2,
            this.tsmiCreateNewExcel,
            this.tsmiCreateNewWord});
            this.cmsCreateDocument.Name = "cmsCreateDocument";
            this.cmsCreateDocument.Size = new System.Drawing.Size(227, 104);
            // 
            // tsmiDeleteDocument
            // 
            this.tsmiDeleteDocument.Name = "tsmiDeleteDocument";
            this.tsmiDeleteDocument.Size = new System.Drawing.Size(226, 22);
            this.tsmiDeleteDocument.Text = "”‰‡ÎËÚ¸ ‰ÓÍÛÏÂÌÚ";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(223, 6);
            // 
            // tsmiSelectDocument
            // 
            this.tsmiSelectDocument.Name = "tsmiSelectDocument";
            this.tsmiSelectDocument.Size = new System.Drawing.Size(226, 22);
            this.tsmiSelectDocument.Text = "¬˚·‡Ú¸ Ù‡ÈÎ ‰ÓÍÛÏÂÌÚ‡";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(223, 6);
            // 
            // tsmiCreateNewExcel
            // 
            this.tsmiCreateNewExcel.Name = "tsmiCreateNewExcel";
            this.tsmiCreateNewExcel.Size = new System.Drawing.Size(226, 22);
            this.tsmiCreateNewExcel.Text = "—ÓÁ‰‡Ú¸ ‰ÓÍÛÏÂÌÚ MS Excel";
            // 
            // tsmiCreateNewWord
            // 
            this.tsmiCreateNewWord.Name = "tsmiCreateNewWord";
            this.tsmiCreateNewWord.Size = new System.Drawing.Size(226, 22);
            this.tsmiCreateNewWord.Text = "—ÓÁ‰‡Ú¸ ‰ÓÍÛÏÂÌÚ MS Word";
            // 
            // BaseClsView
            // 
            this.Controls.Add(this.utcDataCls);
            this.Name = "BaseClsView";
            this.Size = new System.Drawing.Size(687, 476);
            this.ultraTabPageControl1.ResumeLayout(false);
            this.ultraTabPageControl2.ResumeLayout(false);
            this.utpData.ResumeLayout(false);
            this.pnDataTemplate.ResumeLayout(false);
            this.pnDataTemplate_Fill_Panel.ResumeLayout(false);
            this.spMasterDetail.Panel1.ResumeLayout(false);
            this.spMasterDetail.Panel2.ResumeLayout(false);
            this.spMasterDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.utcDetails)).EndInit();
            this.utcDetails.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.utbToolbarManager)).EndInit();
            this.utpAssociate.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ugAssociations)).EndInit();
            this.utpProtocol.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.utcLogSwitcher)).EndInit();
            this.utcLogSwitcher.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.udsAssociations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.utcDataCls)).EndInit();
            this.utcDataCls.ResumeLayout(false);
            this.cmsAudit.ResumeLayout(false);
            this.cmsAuditSchemeObject.ResumeLayout(false);
            this.cmsCreateDocument.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private Infragistics.Win.ToolTip toolTipValue = null;

		public Infragistics.Win.ToolTip ToolTip
		{
			get
			{
				if (null == this.toolTipValue)
				{
					this.toolTipValue = new Infragistics.Win.ToolTip(this);
					this.toolTipValue.DisplayShadow = true;
					this.toolTipValue.AutoPopDelay = 0;
					this.toolTipValue.InitialDelay = 0;
				}
				return this.toolTipValue;
			}
		}

		private void ugClsData_CellDataError_1(object sender, CellDataErrorEventArgs e)
		{
			MessageBox.Show("ÕÂ‚ÂÌÓÂ ÁÌ‡˜ÂÌËÂ", "Œ¯Ë·Í‡ ÔÂÓ·‡ÁÓ‚‡ÌËˇ ÚËÔÓ‚", MessageBoxButtons.OK, MessageBoxIcon.Error);
			e.RaiseErrorEvent = false;
			e.RestoreOriginalValue = true;
			e.StayInEditMode = true;
		}

		public override void Customize()
		{
			ComponentCustomizer.CustomizeInfragisticsComponents(this.components);
			base.Customize();
		}

        public void ShowIncorrect(DataTable table)
        {
            frmIncorrectXmlStructure frm = new frmIncorrectXmlStructure();
            frm.ug.DataSource = table;
            frm.ShowDialog();
        }

        private void ugeCls_Load(object sender, EventArgs e)
        {

        }
	}
}

