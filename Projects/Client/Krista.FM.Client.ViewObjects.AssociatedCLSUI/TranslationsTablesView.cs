using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Krista.FM.Client.Common;
using CC = Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;


namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.TranslationsTables
{
	public class TranslationsTablesView : Krista.FM.Client.ViewObjects.BaseViewObject.BaseView
	{
		private ImageList ilRecords;
		internal CC.UltraGridEx ugeTranslTables;
		private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Top;
        internal ContextMenuStrip cmsAudit;
        internal ToolStripMenuItem auditToolStripMenuItem;
        public Infragistics.Win.UltraWinTabControl.UltraTabControl utcDataCls;
        public Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        public Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage2;
        public Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpData;
        public Panel pnDataTemplate;
        private Panel pnDataTemplate_Fill_Panel;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _pnDataTemplate_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _pnDataTemplate_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _pnDataTemplate_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _pnDataTemplate_Toolbars_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _utpData_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _utpData_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _utpData_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _utpData_Toolbars_Dock_Area_Bottom;
        public Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpProtocol;
        private Panel panel1;
        internal Infragistics.Win.UltraWinTabControl.UltraTabControl utcLogSwitcher;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage3;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage4;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        internal Panel pClassifiers;
		private IContainer components;

		public TranslationsTablesView()
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
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TranslationsTablesView));
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.utpData = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.pnDataTemplate = new System.Windows.Forms.Panel();
            this.pnDataTemplate_Fill_Panel = new System.Windows.Forms.Panel();
            this.ugeTranslTables = new Krista.FM.Client.Components.UltraGridEx();
            this._pnDataTemplate_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._pnDataTemplate_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._pnDataTemplate_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._utpData_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._utpData_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._utpData_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._utpData_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.utpProtocol = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.utcLogSwitcher = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage3 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ultraTabSharedControlsPage4 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.pClassifiers = new System.Windows.Forms.Panel();
            this.ilRecords = new System.Windows.Forms.ImageList(this.components);
            this._BaseView_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.cmsAudit = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.auditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utcDataCls = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ultraTabSharedControlsPage2 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.utpData.SuspendLayout();
            this.pnDataTemplate.SuspendLayout();
            this.pnDataTemplate_Fill_Panel.SuspendLayout();
            this.utpProtocol.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.utcLogSwitcher)).BeginInit();
            this.utcLogSwitcher.SuspendLayout();
            this.ultraTabPageControl1.SuspendLayout();
            this.cmsAudit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.utcDataCls)).BeginInit();
            this.utcDataCls.SuspendLayout();
            this.SuspendLayout();
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
            this.utpData.Size = new System.Drawing.Size(625, 442);
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
            this.pnDataTemplate.Size = new System.Drawing.Size(625, 442);
            this.pnDataTemplate.TabIndex = 10;
            // 
            // pnDataTemplate_Fill_Panel
            // 
            this.pnDataTemplate_Fill_Panel.Controls.Add(this.ugeTranslTables);
            this.pnDataTemplate_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.pnDataTemplate_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnDataTemplate_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.pnDataTemplate_Fill_Panel.Name = "pnDataTemplate_Fill_Panel";
            this.pnDataTemplate_Fill_Panel.Size = new System.Drawing.Size(625, 442);
            this.pnDataTemplate_Fill_Panel.TabIndex = 0;
            // 
            // ugeTranslTables
            // 
            this.ugeTranslTables.AllowAddNewRecords = true;
            this.ugeTranslTables.AllowClearTable = true;
            this.ugeTranslTables.Cursor = System.Windows.Forms.Cursors.Default;
            this.ugeTranslTables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugeTranslTables.InDebugMode = false;
            this.ugeTranslTables.LoadMenuVisible = false;
            this.ugeTranslTables.Location = new System.Drawing.Point(0, 0);
            this.ugeTranslTables.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ugeTranslTables.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ugeTranslTables.Name = "ugeTranslTables";
            this.ugeTranslTables.SaveLoadFileName = "";
            this.ugeTranslTables.SaveMenuVisible = true;
            this.ugeTranslTables.ServerFilterEnabled = false;
            this.ugeTranslTables.SingleBandLevelName = "Добавить запись...";
            this.ugeTranslTables.Size = new System.Drawing.Size(625, 442);
            this.ugeTranslTables.sortColumnName = "";
            this.ugeTranslTables.StateRowEnable = false;
            this.ugeTranslTables.TabIndex = 1;
            this.ugeTranslTables.Load += new System.EventHandler(this.ugeTranslTables_Load);
            // 
            // _pnDataTemplate_Toolbars_Dock_Area_Left
            // 
            this._pnDataTemplate_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._pnDataTemplate_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._pnDataTemplate_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._pnDataTemplate_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._pnDataTemplate_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._pnDataTemplate_Toolbars_Dock_Area_Left.Name = "_pnDataTemplate_Toolbars_Dock_Area_Left";
            this._pnDataTemplate_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 442);
            // 
            // _pnDataTemplate_Toolbars_Dock_Area_Right
            // 
            this._pnDataTemplate_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._pnDataTemplate_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._pnDataTemplate_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._pnDataTemplate_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._pnDataTemplate_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(625, 0);
            this._pnDataTemplate_Toolbars_Dock_Area_Right.Name = "_pnDataTemplate_Toolbars_Dock_Area_Right";
            this._pnDataTemplate_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 442);
            // 
            // _pnDataTemplate_Toolbars_Dock_Area_Top
            // 
            this._pnDataTemplate_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._pnDataTemplate_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._pnDataTemplate_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._pnDataTemplate_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._pnDataTemplate_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._pnDataTemplate_Toolbars_Dock_Area_Top.Name = "_pnDataTemplate_Toolbars_Dock_Area_Top";
            this._pnDataTemplate_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(625, 0);
            // 
            // _pnDataTemplate_Toolbars_Dock_Area_Bottom
            // 
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 442);
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom.Name = "_pnDataTemplate_Toolbars_Dock_Area_Bottom";
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(625, 0);
            // 
            // _utpData_Toolbars_Dock_Area_Left
            // 
            this._utpData_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._utpData_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._utpData_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._utpData_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._utpData_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._utpData_Toolbars_Dock_Area_Left.Name = "_utpData_Toolbars_Dock_Area_Left";
            this._utpData_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 442);
            // 
            // _utpData_Toolbars_Dock_Area_Right
            // 
            this._utpData_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._utpData_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._utpData_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._utpData_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._utpData_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(625, 0);
            this._utpData_Toolbars_Dock_Area_Right.Name = "_utpData_Toolbars_Dock_Area_Right";
            this._utpData_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 442);
            // 
            // _utpData_Toolbars_Dock_Area_Top
            // 
            this._utpData_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._utpData_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._utpData_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._utpData_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._utpData_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._utpData_Toolbars_Dock_Area_Top.Name = "_utpData_Toolbars_Dock_Area_Top";
            this._utpData_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(625, 0);
            // 
            // _utpData_Toolbars_Dock_Area_Bottom
            // 
            this._utpData_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._utpData_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._utpData_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._utpData_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._utpData_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 442);
            this._utpData_Toolbars_Dock_Area_Bottom.Name = "_utpData_Toolbars_Dock_Area_Bottom";
            this._utpData_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(625, 0);
            // 
            // utpProtocol
            // 
            this.utpProtocol.Controls.Add(this.panel1);
            this.utpProtocol.Location = new System.Drawing.Point(-10000, -10000);
            this.utpProtocol.Name = "utpProtocol";
            this.utpProtocol.Size = new System.Drawing.Size(625, 442);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.utcLogSwitcher);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(625, 442);
            this.panel1.TabIndex = 0;
            // 
            // utcLogSwitcher
            // 
            this.utcLogSwitcher.Controls.Add(this.ultraTabSharedControlsPage3);
            this.utcLogSwitcher.Controls.Add(this.ultraTabSharedControlsPage4);
            this.utcLogSwitcher.Controls.Add(this.ultraTabPageControl1);
            this.utcLogSwitcher.Dock = System.Windows.Forms.DockStyle.Fill;
            this.utcLogSwitcher.FlatMode = true;//Infragistics.Win.DefaultableBoolean.True;
            this.utcLogSwitcher.Location = new System.Drawing.Point(0, 0);
            this.utcLogSwitcher.Name = "utcLogSwitcher";
            this.utcLogSwitcher.SharedControlsPage = this.ultraTabSharedControlsPage3;
            this.utcLogSwitcher.Size = new System.Drawing.Size(625, 442);
            this.utcLogSwitcher.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Excel;
            this.utcLogSwitcher.TabIndex = 14;
            this.utcLogSwitcher.TabOrientation = Infragistics.Win.UltraWinTabs.TabOrientation.BottomLeft;
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "Классификаторы и таблицы";
            this.utcLogSwitcher.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1});
            // 
            // ultraTabSharedControlsPage3
            // 
            this.ultraTabSharedControlsPage3.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage3.Name = "ultraTabSharedControlsPage3";
            this.ultraTabSharedControlsPage3.Size = new System.Drawing.Size(623, 421);
            // 
            // ultraTabSharedControlsPage4
            // 
            this.ultraTabSharedControlsPage4.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage4.Name = "ultraTabSharedControlsPage4";
            this.ultraTabSharedControlsPage4.Size = new System.Drawing.Size(413, 444);
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.pClassifiers);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(1, 1);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(623, 421);
            // 
            // pClassifiers
            // 
            this.pClassifiers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pClassifiers.Location = new System.Drawing.Point(0, 0);
            this.pClassifiers.Name = "pClassifiers";
            this.pClassifiers.Size = new System.Drawing.Size(623, 421);
            this.pClassifiers.TabIndex = 0;
            // 
            // ilRecords
            // 
            this.ilRecords.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilRecords.ImageStream")));
            this.ilRecords.TransparentColor = System.Drawing.Color.Transparent;
            this.ilRecords.Images.SetKeyName(0, "Inserted.bmp");
            this.ilRecords.Images.SetKeyName(1, "Edited.bmp");
            this.ilRecords.Images.SetKeyName(2, "Deleted.bmp");
            this.ilRecords.Images.SetKeyName(3, "EraseDisinRules.bmp");
            // 
            // _BaseView_Toolbars_Dock_Area_Top
            // 
            this._BaseView_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._BaseView_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._BaseView_Toolbars_Dock_Area_Top.Name = "_BaseView_Toolbars_Dock_Area_Top";
            this._BaseView_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(629, 0);
            // 
            // cmsAudit
            // 
            this.cmsAudit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.auditToolStripMenuItem});
            this.cmsAudit.Name = "cmsAudit";
            this.cmsAudit.Size = new System.Drawing.Size(184, 26);
            // 
            // auditToolStripMenuItem
            // 
            this.auditToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("auditToolStripMenuItem.Image")));
            this.auditToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.auditToolStripMenuItem.Name = "auditToolStripMenuItem";
            this.auditToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.auditToolStripMenuItem.Text = "История изменений";
            // 
            // utcDataCls
            // 
            this.utcDataCls.Controls.Add(this.ultraTabSharedControlsPage1);
            this.utcDataCls.Controls.Add(this.ultraTabSharedControlsPage2);
            this.utcDataCls.Controls.Add(this.utpData);
            this.utcDataCls.Controls.Add(this.utpProtocol);
            this.utcDataCls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.utcDataCls.Location = new System.Drawing.Point(0, 0);
            this.utcDataCls.Name = "utcDataCls";
            this.utcDataCls.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.utcDataCls.Size = new System.Drawing.Size(629, 465);
            this.utcDataCls.TabIndex = 3;
            ultraTab2.TabPage = this.utpData;
            ultraTab2.Text = "Данные";
            ultraTab3.TabPage = this.utpProtocol;
            ultraTab3.Text = "Протоколы";
            this.utcDataCls.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab2,
            ultraTab3});
            this.utcDataCls.ViewStyle = Infragistics.Win.UltraWinTabControl.ViewStyle.VisualStudio2005;
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(625, 442);
            // 
            // ultraTabSharedControlsPage2
            // 
            this.ultraTabSharedControlsPage2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage2.Name = "ultraTabSharedControlsPage2";
            this.ultraTabSharedControlsPage2.Size = new System.Drawing.Size(747, 614);
            // 
            // TranslationsTablesView
            // 
            this.Controls.Add(this.utcDataCls);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Top);
            this.Name = "TranslationsTablesView";
            this.Size = new System.Drawing.Size(629, 465);
            this.utpData.ResumeLayout(false);
            this.pnDataTemplate.ResumeLayout(false);
            this.pnDataTemplate_Fill_Panel.ResumeLayout(false);
            this.utpProtocol.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.utcLogSwitcher)).EndInit();
            this.utcLogSwitcher.ResumeLayout(false);
            this.ultraTabPageControl1.ResumeLayout(false);
            this.cmsAudit.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.utcDataCls)).EndInit();
            this.utcDataCls.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		public override void Customize()
		{
			ComponentCustomizer.CustomizeInfragisticsComponents(this.components);
			base.Customize();
		}

		private void ugTranslation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
		{

		}

		private void ugeTranslTables_Load(object sender, EventArgs e)
		{

		}

	}
}

