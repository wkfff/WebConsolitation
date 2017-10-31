using System;
using System.ComponentModel;
using System.Windows.Forms;

using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinToolbars;

using Krista.FM.Client.Common;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
	public class TasksView : BaseViewObject.BaseView
	{
		public UltraTabControl utcPages;
		private UltraTabSharedControlsPage ultraTabSharedControlsPage1;
		public UltraTabPageControl utpTaskInfo;
		public UltraTabPageControl utpDocuments;
		public UltraTabPageControl utpHistory;
        private UltraDataSource udsDocuments;
		public UltraGrid ugDocuments;
        public UltraGrid ugHistory;
        private UltraLabel laParameters;
		public UltraToolbarsManager utbDocuments;
		private Panel utpDocuments_Fill_Panel;
		private UltraToolbarsDockArea _utpDocuments_Toolbars_Dock_Area_Left;
		private UltraToolbarsDockArea _utpDocuments_Toolbars_Dock_Area_Right;
		private UltraToolbarsDockArea _utpDocuments_Toolbars_Dock_Area_Top;
        private UltraToolbarsDockArea _utpDocuments_Toolbars_Dock_Area_Bottom;
		public UltraPopupControlContainer upcContainer;
        private ImageList ilImages16;
        public SaveFileDialog sdSave;
        private ImageList ilImages12;
        public UltraTabPageControl utpcGroupsPermissions;
        public UltraTabPageControl utpcUsersPermissions;
        public UltraLabel laContainerInfo;
        private Panel pnContainer1;
        private UltraLabel laTaskKind;
        public Button btnSelectTaskType;
        public TextBox tbTaskKind;
        private UltraLabel laCurator;
        public Button btnSelectCurator;
        public TextBox tbCurator;
        public Button btnSelectDoer;
        public Button btnSelectOwner;
        public TextBox tbDoer;
        public TextBox tbOwner;
        private UltraLabel laDoer;
        private UltraLabel laOwner;
        private UltraLabel laEndDate;
        public DateTimePicker dtpDateTo;
        public DateTimePicker dtpFromDate;
        private UltraLabel laStartDate;
        public Button btnApply;
        public Button btnCancel;
        public UltraDropDownButton uddbDoAction;
        public TextBox tbState;
        public ListBox lbActions;
        public TextBox tbAction;
        private Panel pnContainer2;
        private Splitter spSplitter1;
        public TextBox tbDescription;
        private UltraLabel laDescription;
        private Panel pnContainer4;
        public TextBox tbComment;
        private UltraLabel laCommemt;
        private Panel pnContainer3;
        private UltraLabel laJob;
        private Splitter spSplitter2;
        public TextBox tbJob;
        private UltraTabPageControl utpcConsts;
        private UltraTabPageControl utpcParameters;
        public FolderBrowserDialog fbSelectDir;
        public Components.UltraGridEx ugeParams;
        public Components.UltraGridEx ugeConsts;
		private IContainer components;

		public TasksView()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TasksView));
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DocumentType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Version");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SourceFileName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RefTasks");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Description");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Ownership");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RefTasksTemp");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("clmnEdit", 0);
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("clmnDocumentTypePic", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("clmnFileExt", 2);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("clmnSave", 3);
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("clmnOwnershipName", 4, 6548954);
            Infragistics.Win.ValueList valueList1 = new Infragistics.Win.ValueList(6548954);
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("ID");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("DocumentType");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Name");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Version");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("SourceFileName");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn6 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("RefTasks");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn7 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Description");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn8 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Ownership");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn9 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("RefTasksTemp");
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("utbActions");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("pmAddDocument");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnDelDocument");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnOpenAllDocuments");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnSaveAllDocuments");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnCutDocuments");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnCopyDocuments");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnPasteDocuments");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool7 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("pmRefresh");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnWriteBackAll");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("pmAddDocument");
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnSelectDocumentFile");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnSelectDocumentsFiles");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnNewCalcSheetExcel");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnNewCalcSheetWord");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnNewMdxExpertDocument");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnNewExcelDocument");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnNewWordDocument");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnDelDocument");
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnSelectDocumentFile");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnNewCalcSheetExcel");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnNewMdxExpertDocument");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnNewWordDocument");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool21 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnNewExcelDocument");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnSelectDocumentsFiles");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool23 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnSaveAllDocuments");
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool24 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnOpenAllDocuments");
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool25 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnCutDocuments");
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool26 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnCopyDocuments");
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool27 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnPasteDocuments");
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool28 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnRefreshAll");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool29 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnWriteBackAll");
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool30 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnNewCalcSheetWord");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool3 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("pmRefresh");
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool35 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnRefreshSelect");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool31 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnRefreshAll");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool4 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("pmWriteBack");
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool36 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnWriteBackSelect");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool32 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnWriteBackAll");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool33 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnWriteBackSelect");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool34 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnRefreshSelect");
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab4 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab5 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab6 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab7 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            this.utpTaskInfo = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.spSplitter2 = new System.Windows.Forms.Splitter();
            this.pnContainer4 = new System.Windows.Forms.Panel();
            this.tbComment = new System.Windows.Forms.TextBox();
            this.laCommemt = new Infragistics.Win.Misc.UltraLabel();
            this.pnContainer3 = new System.Windows.Forms.Panel();
            this.tbJob = new System.Windows.Forms.TextBox();
            this.laJob = new Infragistics.Win.Misc.UltraLabel();
            this.spSplitter1 = new System.Windows.Forms.Splitter();
            this.pnContainer2 = new System.Windows.Forms.Panel();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.laDescription = new Infragistics.Win.Misc.UltraLabel();
            this.pnContainer1 = new System.Windows.Forms.Panel();
            this.laTaskKind = new Infragistics.Win.Misc.UltraLabel();
            this.btnSelectTaskType = new System.Windows.Forms.Button();
            this.ilImages12 = new System.Windows.Forms.ImageList(this.components);
            this.tbTaskKind = new System.Windows.Forms.TextBox();
            this.laCurator = new Infragistics.Win.Misc.UltraLabel();
            this.btnSelectCurator = new System.Windows.Forms.Button();
            this.tbCurator = new System.Windows.Forms.TextBox();
            this.btnSelectDoer = new System.Windows.Forms.Button();
            this.btnSelectOwner = new System.Windows.Forms.Button();
            this.tbDoer = new System.Windows.Forms.TextBox();
            this.tbOwner = new System.Windows.Forms.TextBox();
            this.laDoer = new Infragistics.Win.Misc.UltraLabel();
            this.laOwner = new Infragistics.Win.Misc.UltraLabel();
            this.laEndDate = new Infragistics.Win.Misc.UltraLabel();
            this.dtpDateTo = new System.Windows.Forms.DateTimePicker();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.laStartDate = new Infragistics.Win.Misc.UltraLabel();
            this.btnApply = new System.Windows.Forms.Button();
            this.ilImages16 = new System.Windows.Forms.ImageList(this.components);
            this.btnCancel = new System.Windows.Forms.Button();
            this.uddbDoAction = new Infragistics.Win.Misc.UltraDropDownButton();
            this.upcContainer = new Infragistics.Win.Misc.UltraPopupControlContainer(this.components);
            this.lbActions = new System.Windows.Forms.ListBox();
            this.tbState = new System.Windows.Forms.TextBox();
            this.tbAction = new System.Windows.Forms.TextBox();
            this.laParameters = new Infragistics.Win.Misc.UltraLabel();
            this.utpDocuments = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.utpDocuments_Fill_Panel = new System.Windows.Forms.Panel();
            this.ugDocuments = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.udsDocuments = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            this._utpDocuments_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.utbDocuments = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._utpDocuments_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._utpDocuments_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._utpDocuments_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.utpcParameters = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ugeParams = new Krista.FM.Client.Components.UltraGridEx();
            this.utpcConsts = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ugeConsts = new Krista.FM.Client.Components.UltraGridEx();
            this.utpHistory = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ugHistory = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.utpcGroupsPermissions = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.utpcUsersPermissions = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.utcPages = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.sdSave = new System.Windows.Forms.SaveFileDialog();
            this.laContainerInfo = new Infragistics.Win.Misc.UltraLabel();
            this.fbSelectDir = new System.Windows.Forms.FolderBrowserDialog();
            this.utpTaskInfo.SuspendLayout();
            this.pnContainer4.SuspendLayout();
            this.pnContainer3.SuspendLayout();
            this.pnContainer2.SuspendLayout();
            this.pnContainer1.SuspendLayout();
            this.utpDocuments.SuspendLayout();
            this.utpDocuments_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugDocuments)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udsDocuments)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.utbDocuments)).BeginInit();
            this.utpcParameters.SuspendLayout();
            this.utpcConsts.SuspendLayout();
            this.utpHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.utcPages)).BeginInit();
            this.utcPages.SuspendLayout();
            this.SuspendLayout();
            // 
            // utpTaskInfo
            // 
            this.utpTaskInfo.Controls.Add(this.spSplitter2);
            this.utpTaskInfo.Controls.Add(this.pnContainer4);
            this.utpTaskInfo.Controls.Add(this.pnContainer3);
            this.utpTaskInfo.Controls.Add(this.spSplitter1);
            this.utpTaskInfo.Controls.Add(this.pnContainer2);
            this.utpTaskInfo.Controls.Add(this.pnContainer1);
            this.utpTaskInfo.Controls.Add(this.laParameters);
            this.utpTaskInfo.Location = new System.Drawing.Point(-10000, -10000);
            this.utpTaskInfo.Name = "utpTaskInfo";
            this.utpTaskInfo.Size = new System.Drawing.Size(636, 454);
            this.utpTaskInfo.Tag = "";
            // 
            // spSplitter2
            // 
            this.spSplitter2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.spSplitter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.spSplitter2.Location = new System.Drawing.Point(0, 395);
            this.spSplitter2.Name = "spSplitter2";
            this.spSplitter2.Size = new System.Drawing.Size(636, 3);
            this.spSplitter2.TabIndex = 40;
            this.spSplitter2.TabStop = false;
            // 
            // pnContainer4
            // 
            this.pnContainer4.Controls.Add(this.tbComment);
            this.pnContainer4.Controls.Add(this.laCommemt);
            this.pnContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnContainer4.Location = new System.Drawing.Point(0, 395);
            this.pnContainer4.Name = "pnContainer4";
            this.pnContainer4.Size = new System.Drawing.Size(636, 59);
            this.pnContainer4.TabIndex = 39;
            // 
            // tbComment
            // 
            this.tbComment.AcceptsReturn = true;
            this.tbComment.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbComment.BackColor = System.Drawing.SystemColors.Info;
            this.tbComment.Location = new System.Drawing.Point(3, 16);
            this.tbComment.MaxLength = 4000;
            this.tbComment.Multiline = true;
            this.tbComment.Name = "tbComment";
            this.tbComment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbComment.Size = new System.Drawing.Size(630, 38);
            this.tbComment.TabIndex = 18;
            // 
            // laCommemt
            // 
            this.laCommemt.AutoSize = true;
            this.laCommemt.Location = new System.Drawing.Point(4, 1);
            this.laCommemt.Name = "laCommemt";
            this.laCommemt.Size = new System.Drawing.Size(77, 14);
            this.laCommemt.TabIndex = 17;
            this.laCommemt.Text = "Комментарий";
            // 
            // pnContainer3
            // 
            this.pnContainer3.Controls.Add(this.tbJob);
            this.pnContainer3.Controls.Add(this.laJob);
            this.pnContainer3.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnContainer3.Location = new System.Drawing.Point(0, 248);
            this.pnContainer3.Name = "pnContainer3";
            this.pnContainer3.Size = new System.Drawing.Size(636, 147);
            this.pnContainer3.TabIndex = 37;
            // 
            // tbJob
            // 
            this.tbJob.AcceptsReturn = true;
            this.tbJob.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbJob.Location = new System.Drawing.Point(3, 16);
            this.tbJob.MaxLength = 4000;
            this.tbJob.Multiline = true;
            this.tbJob.Name = "tbJob";
            this.tbJob.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbJob.Size = new System.Drawing.Size(630, 127);
            this.tbJob.TabIndex = 33;
            this.tbJob.Leave += new System.EventHandler(this.tbNotNullTextBoxControl_Leave);
            // 
            // laJob
            // 
            this.laJob.AutoSize = true;
            this.laJob.Location = new System.Drawing.Point(4, 1);
            this.laJob.Name = "laJob";
            this.laJob.Size = new System.Drawing.Size(49, 14);
            this.laJob.TabIndex = 32;
            this.laJob.Text = "Задание";
            // 
            // spSplitter1
            // 
            this.spSplitter1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.spSplitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.spSplitter1.Location = new System.Drawing.Point(0, 245);
            this.spSplitter1.Name = "spSplitter1";
            this.spSplitter1.Size = new System.Drawing.Size(636, 3);
            this.spSplitter1.TabIndex = 36;
            this.spSplitter1.TabStop = false;
            // 
            // pnContainer2
            // 
            this.pnContainer2.Controls.Add(this.tbDescription);
            this.pnContainer2.Controls.Add(this.laDescription);
            this.pnContainer2.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnContainer2.Location = new System.Drawing.Point(0, 162);
            this.pnContainer2.Name = "pnContainer2";
            this.pnContainer2.Size = new System.Drawing.Size(636, 83);
            this.pnContainer2.TabIndex = 35;
            // 
            // tbDescription
            // 
            this.tbDescription.AcceptsReturn = true;
            this.tbDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDescription.Location = new System.Drawing.Point(3, 16);
            this.tbDescription.MaxLength = 255;
            this.tbDescription.Multiline = true;
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbDescription.Size = new System.Drawing.Size(630, 63);
            this.tbDescription.TabIndex = 17;
            this.tbDescription.Leave += new System.EventHandler(this.tbNotNullTextBoxControl_Leave);
            // 
            // laDescription
            // 
            this.laDescription.AutoSize = true;
            this.laDescription.Location = new System.Drawing.Point(4, 1);
            this.laDescription.Name = "laDescription";
            this.laDescription.Size = new System.Drawing.Size(83, 14);
            this.laDescription.TabIndex = 16;
            this.laDescription.Text = "Наименование";
            // 
            // pnContainer1
            // 
            this.pnContainer1.Controls.Add(this.laTaskKind);
            this.pnContainer1.Controls.Add(this.btnSelectTaskType);
            this.pnContainer1.Controls.Add(this.tbTaskKind);
            this.pnContainer1.Controls.Add(this.laCurator);
            this.pnContainer1.Controls.Add(this.btnSelectCurator);
            this.pnContainer1.Controls.Add(this.tbCurator);
            this.pnContainer1.Controls.Add(this.btnSelectDoer);
            this.pnContainer1.Controls.Add(this.btnSelectOwner);
            this.pnContainer1.Controls.Add(this.tbDoer);
            this.pnContainer1.Controls.Add(this.tbOwner);
            this.pnContainer1.Controls.Add(this.laDoer);
            this.pnContainer1.Controls.Add(this.laOwner);
            this.pnContainer1.Controls.Add(this.laEndDate);
            this.pnContainer1.Controls.Add(this.dtpDateTo);
            this.pnContainer1.Controls.Add(this.dtpFromDate);
            this.pnContainer1.Controls.Add(this.laStartDate);
            this.pnContainer1.Controls.Add(this.btnApply);
            this.pnContainer1.Controls.Add(this.btnCancel);
            this.pnContainer1.Controls.Add(this.uddbDoAction);
            this.pnContainer1.Controls.Add(this.tbState);
            this.pnContainer1.Controls.Add(this.lbActions);
            this.pnContainer1.Controls.Add(this.tbAction);
            this.pnContainer1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnContainer1.Location = new System.Drawing.Point(0, 0);
            this.pnContainer1.Name = "pnContainer1";
            this.pnContainer1.Size = new System.Drawing.Size(636, 162);
            this.pnContainer1.TabIndex = 34;
            // 
            // laTaskKind
            // 
            this.laTaskKind.AutoSize = true;
            this.laTaskKind.Location = new System.Drawing.Point(4, 137);
            this.laTaskKind.Name = "laTaskKind";
            this.laTaskKind.Size = new System.Drawing.Size(65, 14);
            this.laTaskKind.TabIndex = 55;
            this.laTaskKind.Text = "Вид задачи";
            // 
            // btnSelectTaskType
            // 
            this.btnSelectTaskType.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnSelectTaskType.ImageIndex = 1;
            this.btnSelectTaskType.ImageList = this.ilImages12;
            this.btnSelectTaskType.Location = new System.Drawing.Point(379, 136);
            this.btnSelectTaskType.Name = "btnSelectTaskType";
            this.btnSelectTaskType.Size = new System.Drawing.Size(20, 20);
            this.btnSelectTaskType.TabIndex = 54;
            // 
            // ilImages12
            // 
            this.ilImages12.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilImages12.ImageStream")));
            this.ilImages12.TransparentColor = System.Drawing.Color.Fuchsia;
            this.ilImages12.Images.SetKeyName(0, "User12.bmp");
            this.ilImages12.Images.SetKeyName(1, "general_3_point.bmp");
            // 
            // tbTaskKind
            // 
            this.tbTaskKind.Enabled = false;
            this.tbTaskKind.Location = new System.Drawing.Point(126, 136);
            this.tbTaskKind.Name = "tbTaskKind";
            this.tbTaskKind.ReadOnly = true;
            this.tbTaskKind.Size = new System.Drawing.Size(248, 20);
            this.tbTaskKind.TabIndex = 53;
            // 
            // laCurator
            // 
            this.laCurator.AutoSize = true;
            this.laCurator.Location = new System.Drawing.Point(4, 111);
            this.laCurator.Name = "laCurator";
            this.laCurator.Size = new System.Drawing.Size(48, 14);
            this.laCurator.TabIndex = 52;
            this.laCurator.Text = "Куратор";
            // 
            // btnSelectCurator
            // 
            this.btnSelectCurator.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnSelectCurator.ImageKey = "User12.bmp";
            this.btnSelectCurator.ImageList = this.ilImages12;
            this.btnSelectCurator.Location = new System.Drawing.Point(379, 110);
            this.btnSelectCurator.Name = "btnSelectCurator";
            this.btnSelectCurator.Size = new System.Drawing.Size(20, 20);
            this.btnSelectCurator.TabIndex = 51;
            // 
            // tbCurator
            // 
            this.tbCurator.Enabled = false;
            this.tbCurator.Location = new System.Drawing.Point(126, 110);
            this.tbCurator.Name = "tbCurator";
            this.tbCurator.ReadOnly = true;
            this.tbCurator.Size = new System.Drawing.Size(248, 20);
            this.tbCurator.TabIndex = 50;
            // 
            // btnSelectDoer
            // 
            this.btnSelectDoer.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnSelectDoer.ImageIndex = 0;
            this.btnSelectDoer.ImageList = this.ilImages12;
            this.btnSelectDoer.Location = new System.Drawing.Point(379, 84);
            this.btnSelectDoer.Name = "btnSelectDoer";
            this.btnSelectDoer.Size = new System.Drawing.Size(20, 20);
            this.btnSelectDoer.TabIndex = 49;
            // 
            // btnSelectOwner
            // 
            this.btnSelectOwner.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnSelectOwner.ImageIndex = 0;
            this.btnSelectOwner.ImageList = this.ilImages12;
            this.btnSelectOwner.Location = new System.Drawing.Point(379, 58);
            this.btnSelectOwner.Name = "btnSelectOwner";
            this.btnSelectOwner.Size = new System.Drawing.Size(20, 20);
            this.btnSelectOwner.TabIndex = 48;
            // 
            // tbDoer
            // 
            this.tbDoer.Enabled = false;
            this.tbDoer.Location = new System.Drawing.Point(126, 84);
            this.tbDoer.Name = "tbDoer";
            this.tbDoer.ReadOnly = true;
            this.tbDoer.Size = new System.Drawing.Size(248, 20);
            this.tbDoer.TabIndex = 45;
            // 
            // tbOwner
            // 
            this.tbOwner.Enabled = false;
            this.tbOwner.Location = new System.Drawing.Point(126, 58);
            this.tbOwner.Name = "tbOwner";
            this.tbOwner.ReadOnly = true;
            this.tbOwner.Size = new System.Drawing.Size(248, 20);
            this.tbOwner.TabIndex = 44;
            // 
            // laDoer
            // 
            this.laDoer.AutoSize = true;
            this.laDoer.Location = new System.Drawing.Point(4, 85);
            this.laDoer.Name = "laDoer";
            this.laDoer.Size = new System.Drawing.Size(74, 14);
            this.laDoer.TabIndex = 43;
            this.laDoer.Text = "Исполнитель";
            // 
            // laOwner
            // 
            this.laOwner.AutoSize = true;
            this.laOwner.Location = new System.Drawing.Point(4, 59);
            this.laOwner.Name = "laOwner";
            this.laOwner.Size = new System.Drawing.Size(57, 14);
            this.laOwner.TabIndex = 42;
            this.laOwner.Text = "Владелец";
            // 
            // laEndDate
            // 
            this.laEndDate.AutoSize = true;
            this.laEndDate.Location = new System.Drawing.Point(4, 34);
            this.laEndDate.Name = "laEndDate";
            this.laEndDate.Size = new System.Drawing.Size(98, 14);
            this.laEndDate.TabIndex = 41;
            this.laEndDate.Text = "Дата завершения";
            // 
            // dtpDateTo
            // 
            this.dtpDateTo.CustomFormat = "dd.MM.yyyy HH:mm";
            this.dtpDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDateTo.Location = new System.Drawing.Point(126, 33);
            this.dtpDateTo.Name = "dtpDateTo";
            this.dtpDateTo.Size = new System.Drawing.Size(110, 20);
            this.dtpDateTo.TabIndex = 40;
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.CustomFormat = "dd.MM.yyyy HH:mm";
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDate.Location = new System.Drawing.Point(126, 7);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(110, 20);
            this.dtpFromDate.TabIndex = 39;
            // 
            // laStartDate
            // 
            this.laStartDate.AutoSize = true;
            this.laStartDate.Location = new System.Drawing.Point(4, 8);
            this.laStartDate.Name = "laStartDate";
            this.laStartDate.Size = new System.Drawing.Size(71, 14);
            this.laStartDate.TabIndex = 38;
            this.laStartDate.Text = "Дата начала";
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnApply.ImageIndex = 5;
            this.btnApply.ImageList = this.ilImages16;
            this.btnApply.Location = new System.Drawing.Point(406, 33);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(106, 23);
            this.btnApply.TabIndex = 2;
            this.btnApply.Text = "Применить";
            this.btnApply.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            // 
            // ilImages16
            // 
            this.ilImages16.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilImages16.ImageStream")));
            this.ilImages16.TransparentColor = System.Drawing.Color.Fuchsia;
            this.ilImages16.Images.SetKeyName(0, "");
            this.ilImages16.Images.SetKeyName(1, "");
            this.ilImages16.Images.SetKeyName(2, "task_document_open.bmp");
            this.ilImages16.Images.SetKeyName(3, "task_document_save.bmp");
            this.ilImages16.Images.SetKeyName(4, "");
            this.ilImages16.Images.SetKeyName(5, "");
            this.ilImages16.Images.SetKeyName(6, "");
            this.ilImages16.Images.SetKeyName(7, "User.bmp");
            this.ilImages16.Images.SetKeyName(8, "Copy.bmp");
            this.ilImages16.Images.SetKeyName(9, "Cut.bmp");
            this.ilImages16.Images.SetKeyName(10, "Paste.bmp");
            this.ilImages16.Images.SetKeyName(11, "task_document_refresh_all.bmp");
            this.ilImages16.Images.SetKeyName(12, "task_document_write_back_all.bmp");
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.ImageIndex = 4;
            this.btnCancel.ImageList = this.ilImages16;
            this.btnCancel.Location = new System.Drawing.Point(516, 33);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(106, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Отменить";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            // 
            // uddbDoAction
            // 
            this.uddbDoAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = 6;
            this.uddbDoAction.Appearance = appearance1;
            this.uddbDoAction.ImageList = this.ilImages16;
            this.uddbDoAction.Location = new System.Drawing.Point(406, 7);
            this.uddbDoAction.Name = "uddbDoAction";
            this.uddbDoAction.PopupItemKey = "lbActions";
            this.uddbDoAction.PopupItemProvider = this.upcContainer;
            this.uddbDoAction.Size = new System.Drawing.Size(216, 23);
            this.uddbDoAction.Style = Infragistics.Win.Misc.SplitButtonDisplayStyle.DropDownButtonOnly;
            this.uddbDoAction.TabIndex = 1;
            this.uddbDoAction.Text = "Выполнить";
            // 
            // upcContainer
            // 
            this.upcContainer.PopupControl = this.lbActions;
            // 
            // lbActions
            // 
            this.lbActions.BackColor = System.Drawing.SystemColors.Window;
            this.lbActions.FormattingEnabled = true;
            this.lbActions.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"});
            this.lbActions.Location = new System.Drawing.Point(406, 33);
            this.lbActions.Name = "lbActions";
            this.lbActions.ScrollAlwaysVisible = true;
            this.lbActions.Size = new System.Drawing.Size(216, 121);
            this.lbActions.TabIndex = 46;
            this.lbActions.Visible = false;
            this.lbActions.SelectedIndexChanged += new System.EventHandler(this.lbActions_SelectedIndexChanged);
            // 
            // tbState
            // 
            this.tbState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbState.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbState.Location = new System.Drawing.Point(78, 12);
            this.tbState.Name = "tbState";
            this.tbState.ReadOnly = true;
            this.tbState.Size = new System.Drawing.Size(319, 13);
            this.tbState.TabIndex = 0;
            this.tbState.TabStop = false;
            this.tbState.Text = "Состояние";
            this.tbState.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbAction
            // 
            this.tbAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAction.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbAction.Location = new System.Drawing.Point(78, 38);
            this.tbAction.Name = "tbAction";
            this.tbAction.ReadOnly = true;
            this.tbAction.Size = new System.Drawing.Size(319, 13);
            this.tbAction.TabIndex = 47;
            this.tbAction.TabStop = false;
            this.tbAction.Text = "Действие";
            this.tbAction.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // laParameters
            // 
            this.laParameters.AutoSize = true;
            this.laParameters.Location = new System.Drawing.Point(12, 550);
            this.laParameters.Name = "laParameters";
            this.laParameters.Size = new System.Drawing.Size(65, 14);
            this.laParameters.TabIndex = 17;
            this.laParameters.Text = "Параметры";
            this.laParameters.Visible = false;
            // 
            // utpDocuments
            // 
            this.utpDocuments.Controls.Add(this.utpDocuments_Fill_Panel);
            this.utpDocuments.Controls.Add(this._utpDocuments_Toolbars_Dock_Area_Left);
            this.utpDocuments.Controls.Add(this._utpDocuments_Toolbars_Dock_Area_Right);
            this.utpDocuments.Controls.Add(this._utpDocuments_Toolbars_Dock_Area_Top);
            this.utpDocuments.Controls.Add(this._utpDocuments_Toolbars_Dock_Area_Bottom);
            this.utpDocuments.Location = new System.Drawing.Point(2, 24);
            this.utpDocuments.Name = "utpDocuments";
            this.utpDocuments.Size = new System.Drawing.Size(636, 454);
            // 
            // utpDocuments_Fill_Panel
            // 
            this.utpDocuments_Fill_Panel.Controls.Add(this.ugDocuments);
            this.utpDocuments_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.utpDocuments_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.utpDocuments_Fill_Panel.Location = new System.Drawing.Point(0, 52);
            this.utpDocuments_Fill_Panel.Name = "utpDocuments_Fill_Panel";
            this.utpDocuments_Fill_Panel.Size = new System.Drawing.Size(636, 402);
            this.utpDocuments_Fill_Panel.TabIndex = 0;
            // 
            // ugDocuments
            // 
            this.ugDocuments.AllowDrop = true;
            this.ugDocuments.Cursor = System.Windows.Forms.Cursors.Default;
            this.ugDocuments.DataSource = this.udsDocuments;
            this.ugDocuments.DisplayLayout.AddNewBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.ugDocuments.DisplayLayout.AddNewBox.Prompt = "Добавить запись";
            ultraGridColumn1.AllowGroupBy = Infragistics.Win.DefaultableBoolean.False;
            ultraGridColumn1.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            ultraGridColumn1.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn1.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn1.Header.VisiblePosition = 2;
            ultraGridColumn1.Width = 37;
            ultraGridColumn2.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn2.Header.Caption = "Тип документа";
            ultraGridColumn2.Header.VisiblePosition = 4;
            ultraGridColumn2.Hidden = true;
            ultraGridColumn3.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn3.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn3.Header.Caption = "Название";
            ultraGridColumn3.Header.VisiblePosition = 5;
            ultraGridColumn3.Width = 344;
            ultraGridColumn4.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn4.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn4.Header.Caption = "Версия";
            ultraGridColumn4.Header.VisiblePosition = 8;
            ultraGridColumn5.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn5.Header.VisiblePosition = 12;
            ultraGridColumn5.Hidden = true;
            ultraGridColumn6.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn6.Header.VisiblePosition = 13;
            ultraGridColumn6.Hidden = true;
            ultraGridColumn7.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn7.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn7.Header.Caption = "Комментарий";
            ultraGridColumn7.Header.VisiblePosition = 11;
            ultraGridColumn8.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn8.Header.VisiblePosition = 10;
            ultraGridColumn8.Hidden = true;
            ultraGridColumn9.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn9.Header.VisiblePosition = 9;
            ultraGridColumn9.Hidden = true;
            ultraGridColumn10.AllowGroupBy = Infragistics.Win.DefaultableBoolean.False;
            ultraGridColumn10.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            ultraGridColumn10.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            ultraGridColumn10.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.None;
            ultraGridColumn10.AutoSizeEdit = Infragistics.Win.DefaultableBoolean.False;
            ultraGridColumn10.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
            ultraGridColumn10.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            appearance16.ForeColor = System.Drawing.Color.Transparent;
            appearance16.ForeColorDisabled = System.Drawing.Color.Transparent;
            appearance16.ForegroundAlpha = Infragistics.Win.Alpha.Transparent;
            appearance16.Image = 2;
            appearance16.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance16.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn10.CellAppearance = appearance16;
            appearance17.BackColor = System.Drawing.Color.White;
            appearance17.ForeColor = System.Drawing.Color.Transparent;
            appearance17.ForeColorDisabled = System.Drawing.Color.Transparent;
            appearance17.ForegroundAlpha = Infragistics.Win.Alpha.Transparent;
            appearance17.Image = 2;
            appearance17.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance17.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn10.CellButtonAppearance = appearance17;
            ultraGridColumn10.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
            ultraGridColumn10.DefaultCellValue = "Открыть";
            ultraGridColumn10.Header.Caption = "";
            ultraGridColumn10.Header.VisiblePosition = 0;
            ultraGridColumn10.LockedWidth = true;
            ultraGridColumn10.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            ultraGridColumn10.Width = 21;
            ultraGridColumn11.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn11.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn11.Header.Caption = "Тип документа";
            ultraGridColumn11.Header.VisiblePosition = 3;
            ultraGridColumn11.Width = 130;
            ultraGridColumn12.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn12.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn12.Header.Caption = "Тип файла";
            ultraGridColumn12.Header.VisiblePosition = 6;
            ultraGridColumn12.Width = 76;
            ultraGridColumn13.AllowGroupBy = Infragistics.Win.DefaultableBoolean.False;
            ultraGridColumn13.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            ultraGridColumn13.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            ultraGridColumn13.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.None;
            ultraGridColumn13.AutoSizeEdit = Infragistics.Win.DefaultableBoolean.False;
            ultraGridColumn13.AutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
            ultraGridColumn13.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            appearance18.ForeColor = System.Drawing.Color.Transparent;
            appearance18.ForeColorDisabled = System.Drawing.Color.Transparent;
            appearance18.ForegroundAlpha = Infragistics.Win.Alpha.Transparent;
            appearance18.Image = 3;
            appearance18.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance18.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn13.CellAppearance = appearance18;
            appearance19.BackColor = System.Drawing.Color.White;
            appearance19.ForeColor = System.Drawing.Color.Transparent;
            appearance19.ForeColorDisabled = System.Drawing.Color.Transparent;
            appearance19.ForegroundAlpha = Infragistics.Win.Alpha.Transparent;
            appearance19.Image = 3;
            appearance19.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance19.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn13.CellButtonAppearance = appearance19;
            ultraGridColumn13.CellDisplayStyle = Infragistics.Win.UltraWinGrid.CellDisplayStyle.FullEditorDisplay;
            ultraGridColumn13.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
            ultraGridColumn13.Header.Caption = "";
            ultraGridColumn13.Header.VisiblePosition = 1;
            ultraGridColumn13.LockedWidth = true;
            ultraGridColumn13.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            ultraGridColumn13.Width = 21;
            ultraGridColumn14.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn14.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            ultraGridColumn14.Header.Caption = "Принадлежность";
            ultraGridColumn14.Header.VisiblePosition = 7;
            ultraGridColumn14.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            ultraGridColumn14.Width = 200;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn10,
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn13,
            ultraGridColumn14});
            ultraGridBand1.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.Edit;
            this.ugDocuments.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.ugDocuments.DisplayLayout.LoadStyle = Infragistics.Win.UltraWinGrid.LoadStyle.LoadOnDemand;
            this.ugDocuments.DisplayLayout.MaxBandDepth = 1;
            this.ugDocuments.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.Yes;
            this.ugDocuments.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.ugDocuments.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.Edit;
            valueList1.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayText;
            valueList1.Key = "clmnOwnershipNameDropDownList";
            valueList1.MaxDropDownItems = 3;
            valueList1.ScaleItemImage = Infragistics.Win.ScaleImage.Never;
            this.ugDocuments.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1});
            this.ugDocuments.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ugDocuments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugDocuments.ImageList = this.ilImages16;
            this.ugDocuments.Location = new System.Drawing.Point(0, 0);
            this.ugDocuments.Name = "ugDocuments";
            this.ugDocuments.Size = new System.Drawing.Size(636, 402);
            this.ugDocuments.SyncWithCurrencyManager = false;
            this.ugDocuments.TabIndex = 8;
            // 
            // udsDocuments
            // 
            ultraDataColumn1.DataType = typeof(int);
            ultraDataColumn2.DataType = typeof(int);
            ultraDataColumn4.DataType = typeof(int);
            ultraDataColumn6.DataType = typeof(int);
            ultraDataColumn8.DataType = typeof(int);
            ultraDataColumn9.DataType = typeof(int);
            this.udsDocuments.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4,
            ultraDataColumn5,
            ultraDataColumn6,
            ultraDataColumn7,
            ultraDataColumn8,
            ultraDataColumn9});
            // 
            // _utpDocuments_Toolbars_Dock_Area_Left
            // 
            this._utpDocuments_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._utpDocuments_Toolbars_Dock_Area_Left.AlphaBlendMode = Infragistics.Win.AlphaBlendMode.Disabled;
            this._utpDocuments_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._utpDocuments_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._utpDocuments_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._utpDocuments_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 52);
            this._utpDocuments_Toolbars_Dock_Area_Left.Name = "_utpDocuments_Toolbars_Dock_Area_Left";
            this._utpDocuments_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 402);
            this._utpDocuments_Toolbars_Dock_Area_Left.ToolbarsManager = this.utbDocuments;
            // 
            // utbDocuments
            // 
            this.utbDocuments.AlphaBlendMode = Infragistics.Win.AlphaBlendMode.Disabled;
            this.utbDocuments.DesignerFlags = 1;
            this.utbDocuments.DockWithinContainer = this.utpDocuments;
            this.utbDocuments.ImageListSmall = this.ilImages16;
            this.utbDocuments.ImageTransparentColor = System.Drawing.Color.White;
            this.utbDocuments.ShowFullMenusDelay = 500;
            ultraToolbar1.DockedColumn = 0;
            ultraToolbar1.DockedRow = 0;
            ultraToolbar1.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool1,
            buttonTool2,
            buttonTool3,
            buttonTool4,
            buttonTool5,
            buttonTool6,
            popupMenuTool7,
            buttonTool7});
            ultraToolbar1.Settings.AllowCustomize = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowDockBottom = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowDockLeft = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowDockRight = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowDockTop = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowFloating = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowHiding = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Text = "Действия";
            this.utbDocuments.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1});
            this.utbDocuments.ToolbarSettings.AllowCustomize = Infragistics.Win.DefaultableBoolean.False;
            appearance6.Image = 0;
            popupMenuTool2.SharedProps.AppearancesSmall.Appearance = appearance6;
            popupMenuTool2.SharedProps.Caption = "Добавить документ";
            popupMenuTool2.SharedProps.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageOnlyOnToolbars;
            buttonTool9.InstanceProps.IsFirstInGroup = true;
            buttonTool11.InstanceProps.IsFirstInGroup = true;
            buttonTool14.InstanceProps.IsFirstInGroup = true;
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool9,
            buttonTool10,
            buttonTool11,
            buttonTool12,
            buttonTool13,
            buttonTool14,
            buttonTool15});
            appearance7.Image = 1;
            buttonTool16.SharedProps.AppearancesSmall.Appearance = appearance7;
            buttonTool16.SharedProps.Caption = "Удалить документ";
            buttonTool17.SharedProps.Caption = "Выбрать файл документа";
            buttonTool18.SharedProps.Caption = "Создать документ надстройки MS Excel";
            buttonTool19.SharedProps.Caption = "Создать документ MDX Эксперт";
            buttonTool20.SharedProps.Caption = "Создать документ MS Word";
            buttonTool21.SharedProps.Caption = "Создать документ MS Excel";
            buttonTool22.SharedProps.Caption = "Выбрать файлы документов";
            appearance8.Image = 3;
            buttonTool23.SharedProps.AppearancesSmall.Appearance = appearance8;
            buttonTool23.SharedProps.Caption = "Сохранить все документы";
            appearance9.Image = 2;
            buttonTool24.SharedProps.AppearancesSmall.Appearance = appearance9;
            buttonTool24.SharedProps.Caption = "Открыть все документы";
            appearance10.Image = "Cut.bmp";
            buttonTool25.SharedProps.AppearancesSmall.Appearance = appearance10;
            buttonTool25.SharedProps.Caption = "Вырезать документы";
            buttonTool25.SharedProps.ToolTipText = "Вырезать документы";
            appearance11.Image = "Copy.bmp";
            buttonTool26.SharedProps.AppearancesSmall.Appearance = appearance11;
            buttonTool26.SharedProps.Caption = "Копировать документы";
            buttonTool26.SharedProps.ToolTipText = "Копировать документы";
            appearance12.Image = "Paste.bmp";
            buttonTool27.SharedProps.AppearancesSmall.Appearance = appearance12;
            buttonTool27.SharedProps.Caption = "Вставить документы";
            buttonTool27.SharedProps.ToolTipText = "Вставить документы";
            appearance13.Image = "task_document_refresh_all.bmp";
            buttonTool28.SharedProps.AppearancesSmall.Appearance = appearance13;
            buttonTool28.SharedProps.Caption = "Обновить все листы";
            buttonTool28.SharedProps.ToolTipText = "Обновить все листы";
            appearance14.Image = "task_document_write_back_all.bmp";
            buttonTool29.SharedProps.AppearancesSmall.Appearance = appearance14;
            buttonTool29.SharedProps.Caption = "Обратная запись";
            buttonTool29.SharedProps.ToolTipText = "Обратная запись";
            buttonTool30.SharedProps.Caption = "Создать документ надстройки MS Word";
            appearance33.Image = "task_document_refresh_all.bmp";
            popupMenuTool3.Settings.Appearance = appearance33;
            popupMenuTool3.Settings.ToolDisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            appearance30.Image = "task_document_refresh_all.bmp";
            popupMenuTool3.SharedProps.AppearancesSmall.Appearance = appearance30;
            popupMenuTool3.SharedProps.Caption = "Обновить данные";
            popupMenuTool3.SharedProps.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageOnlyOnToolbars;
            popupMenuTool3.SharedProps.ToolTipText = "Обновить данные";
            popupMenuTool3.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool35,
            buttonTool31});
            appearance32.Image = "task_document_write_back_all.bmp";
            popupMenuTool4.Settings.Appearance = appearance32;
            popupMenuTool4.Settings.ToolDisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            appearance31.Image = "task_document_write_back_all.bmp";
            popupMenuTool4.SharedProps.AppearancesSmall.Appearance = appearance31;
            popupMenuTool4.SharedProps.Caption = "Передать данные листов на сервер";
            popupMenuTool4.SharedProps.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageOnlyOnToolbars;
            popupMenuTool4.SharedProps.ToolTipText = "Передать данные листов на сервер";
            popupMenuTool4.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool36,
            buttonTool32});
            buttonTool33.SharedProps.Caption = "Передать данные  выделенных листов";
            buttonTool34.SharedProps.Caption = "Обновить выделенные листы";
            this.utbDocuments.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool2,
            buttonTool16,
            buttonTool17,
            buttonTool18,
            buttonTool19,
            buttonTool20,
            buttonTool21,
            buttonTool22,
            buttonTool23,
            buttonTool24,
            buttonTool25,
            buttonTool26,
            buttonTool27,
            buttonTool28,
            buttonTool29,
            buttonTool30,
            popupMenuTool3,
            popupMenuTool4,
            buttonTool33,
            buttonTool34});
            // 
            // _utpDocuments_Toolbars_Dock_Area_Right
            // 
            this._utpDocuments_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._utpDocuments_Toolbars_Dock_Area_Right.AlphaBlendMode = Infragistics.Win.AlphaBlendMode.Disabled;
            this._utpDocuments_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._utpDocuments_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._utpDocuments_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._utpDocuments_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(636, 52);
            this._utpDocuments_Toolbars_Dock_Area_Right.Name = "_utpDocuments_Toolbars_Dock_Area_Right";
            this._utpDocuments_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 402);
            this._utpDocuments_Toolbars_Dock_Area_Right.ToolbarsManager = this.utbDocuments;
            // 
            // _utpDocuments_Toolbars_Dock_Area_Top
            // 
            this._utpDocuments_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._utpDocuments_Toolbars_Dock_Area_Top.AlphaBlendMode = Infragistics.Win.AlphaBlendMode.Disabled;
            this._utpDocuments_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._utpDocuments_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._utpDocuments_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._utpDocuments_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._utpDocuments_Toolbars_Dock_Area_Top.Name = "_utpDocuments_Toolbars_Dock_Area_Top";
            this._utpDocuments_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(636, 52);
            this._utpDocuments_Toolbars_Dock_Area_Top.ToolbarsManager = this.utbDocuments;
            // 
            // _utpDocuments_Toolbars_Dock_Area_Bottom
            // 
            this._utpDocuments_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._utpDocuments_Toolbars_Dock_Area_Bottom.AlphaBlendMode = Infragistics.Win.AlphaBlendMode.Disabled;
            this._utpDocuments_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._utpDocuments_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._utpDocuments_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._utpDocuments_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 454);
            this._utpDocuments_Toolbars_Dock_Area_Bottom.Name = "_utpDocuments_Toolbars_Dock_Area_Bottom";
            this._utpDocuments_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(636, 0);
            this._utpDocuments_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.utbDocuments;
            // 
            // utpcParameters
            // 
            this.utpcParameters.Controls.Add(this.ugeParams);
            this.utpcParameters.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcParameters.Name = "utpcParameters";
            this.utpcParameters.Size = new System.Drawing.Size(636, 454);
            // 
            // ugeParams
            // 
            this.ugeParams.AllowAddNewRecords = true;
            this.ugeParams.AllowClearTable = true;
            this.ugeParams.Caption = "";
            this.ugeParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugeParams.InDebugMode = false;
            this.ugeParams.LoadMenuVisible = false;
            this.ugeParams.Location = new System.Drawing.Point(0, 0);
            this.ugeParams.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ugeParams.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ugeParams.Name = "ugeParams";
            this.ugeParams.SaveLoadFileName = "";
            this.ugeParams.SaveMenuVisible = false;
            this.ugeParams.ServerFilterEnabled = false;
            this.ugeParams.SingleBandLevelName = "Добавить запись...";
            this.ugeParams.Size = new System.Drawing.Size(636, 454);
            this.ugeParams.sortColumnName = "";
            this.ugeParams.StateRowEnable = false;
            this.ugeParams.TabIndex = 10;
            // 
            // utpcConsts
            // 
            this.utpcConsts.Controls.Add(this.ugeConsts);
            this.utpcConsts.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcConsts.Name = "utpcConsts";
            this.utpcConsts.Size = new System.Drawing.Size(636, 454);
            // 
            // ugeConsts
            // 
            this.ugeConsts.AllowAddNewRecords = true;
            this.ugeConsts.AllowClearTable = true;
            this.ugeConsts.Caption = "";
            this.ugeConsts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugeConsts.InDebugMode = false;
            this.ugeConsts.LoadMenuVisible = false;
            this.ugeConsts.Location = new System.Drawing.Point(0, 0);
            this.ugeConsts.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ugeConsts.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ugeConsts.Name = "ugeConsts";
            this.ugeConsts.SaveLoadFileName = "";
            this.ugeConsts.SaveMenuVisible = false;
            this.ugeConsts.ServerFilterEnabled = false;
            this.ugeConsts.SingleBandLevelName = "Добавить запись...";
            this.ugeConsts.Size = new System.Drawing.Size(636, 454);
            this.ugeConsts.sortColumnName = "";
            this.ugeConsts.StateRowEnable = false;
            this.ugeConsts.TabIndex = 11;
            // 
            // utpHistory
            // 
            this.utpHistory.Controls.Add(this.ugHistory);
            this.utpHistory.Location = new System.Drawing.Point(-10000, -10000);
            this.utpHistory.Name = "utpHistory";
            this.utpHistory.Size = new System.Drawing.Size(636, 454);
            // 
            // ugHistory
            // 
            this.ugHistory.AllowDrop = true;
            this.ugHistory.Cursor = System.Windows.Forms.Cursors.Default;
            this.ugHistory.DisplayLayout.AddNewBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.ugHistory.DisplayLayout.AddNewBox.Prompt = "Добавить запись";
            this.ugHistory.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.ugHistory.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.ugHistory.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.ugHistory.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.ugHistory.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ugHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugHistory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ugHistory.Location = new System.Drawing.Point(0, 0);
            this.ugHistory.Name = "ugHistory";
            this.ugHistory.Size = new System.Drawing.Size(636, 454);
            this.ugHistory.TabIndex = 8;
            this.ugHistory.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ugHistory_InitializeLayout);
            // 
            // utpcGroupsPermissions
            // 
            this.utpcGroupsPermissions.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcGroupsPermissions.Name = "utpcGroupsPermissions";
            this.utpcGroupsPermissions.Size = new System.Drawing.Size(636, 454);
            // 
            // utpcUsersPermissions
            // 
            this.utpcUsersPermissions.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcUsersPermissions.Name = "utpcUsersPermissions";
            this.utpcUsersPermissions.Size = new System.Drawing.Size(636, 454);
            // 
            // utcPages
            // 
            this.utcPages.Controls.Add(this.ultraTabSharedControlsPage1);
            this.utcPages.Controls.Add(this.utpTaskInfo);
            this.utcPages.Controls.Add(this.utpDocuments);
            this.utcPages.Controls.Add(this.utpHistory);
            this.utcPages.Controls.Add(this.utpcGroupsPermissions);
            this.utcPages.Controls.Add(this.utpcUsersPermissions);
            this.utcPages.Controls.Add(this.utpcConsts);
            this.utcPages.Controls.Add(this.utpcParameters);
            this.utcPages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.utcPages.Location = new System.Drawing.Point(0, 0);
            this.utcPages.Name = "utcPages";
            this.utcPages.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.utcPages.Size = new System.Drawing.Size(640, 480);
            appearance15.BackColor = System.Drawing.SystemColors.Control;
            this.utcPages.TabHeaderAreaAppearance = appearance15;
            this.utcPages.TabIndex = 0;
            ultraTab1.Key = "utpTaskInfo";
            ultraTab1.TabPage = this.utpTaskInfo;
            ultraTab1.Text = "Задача";
            ultraTab2.Key = "utpDocuments";
            ultraTab2.TabPage = this.utpDocuments;
            ultraTab2.Text = "Документы";
            ultraTab3.Key = "utpParameters";
            ultraTab3.TabPage = this.utpcParameters;
            ultraTab3.Text = "Параметры";
            ultraTab4.Key = "utpConsts";
            ultraTab4.TabPage = this.utpcConsts;
            ultraTab4.Text = "Константы";
            ultraTab5.Key = "utpHistory";
            ultraTab5.TabPage = this.utpHistory;
            ultraTab5.Text = "История";
            ultraTab6.Key = "utpGroupsPermissions";
            ultraTab6.TabPage = this.utpcGroupsPermissions;
            ultraTab6.Text = "Права групп";
            ultraTab7.Key = "utpUsersPermissions";
            ultraTab7.TabPage = this.utpcUsersPermissions;
            ultraTab7.Text = "Права пользователей";
            this.utcPages.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2,
            ultraTab3,
            ultraTab4,
            ultraTab5,
            ultraTab6,
            ultraTab7});
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(636, 454);
            // 
            // laContainerInfo
            // 
            appearance34.TextHAlignAsString = "Center";
            this.laContainerInfo.Appearance = appearance34;
            this.laContainerInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.laContainerInfo.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.laContainerInfo.Location = new System.Drawing.Point(0, 0);
            this.laContainerInfo.Name = "laContainerInfo";
            this.laContainerInfo.Size = new System.Drawing.Size(640, 480);
            this.laContainerInfo.TabIndex = 1;
            this.laContainerInfo.Text = "Недостаточно прав для просмотра задачи";
            // 
            // fbSelectDir
            // 
            this.fbSelectDir.Description = "Выберите каталог для сохранения документов";
            this.fbSelectDir.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // TasksView
            // 
            this.Controls.Add(this.utcPages);
            this.Controls.Add(this.laContainerInfo);
            this.Name = "TasksView";
            this.Size = new System.Drawing.Size(640, 480);
            this.utpTaskInfo.ResumeLayout(false);
            this.utpTaskInfo.PerformLayout();
            this.pnContainer4.ResumeLayout(false);
            this.pnContainer4.PerformLayout();
            this.pnContainer3.ResumeLayout(false);
            this.pnContainer3.PerformLayout();
            this.pnContainer2.ResumeLayout(false);
            this.pnContainer2.PerformLayout();
            this.pnContainer1.ResumeLayout(false);
            this.pnContainer1.PerformLayout();
            this.utpDocuments.ResumeLayout(false);
            this.utpDocuments_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ugDocuments)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udsDocuments)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.utbDocuments)).EndInit();
            this.utpcParameters.ResumeLayout(false);
            this.utpcConsts.ResumeLayout(false);
            this.utpHistory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ugHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.utcPages)).EndInit();
            this.utcPages.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void lbActions_SelectedIndexChanged(object sender, EventArgs e)
		{
			upcContainer.Close();
		}

		//private void uddbDoAction_DroppingDown(object sender, CancelEventArgs e)
		//{
		//	this.lbActions.Items.Clear();
		//}

		public override void Customize()
		{
            //.ResetDisplayLayout();
            //ugConsts.ResetDisplayLayout();
            ComponentCustomizer.CustomizeInfragisticsComponents(components);
			base.Customize();
		}

        private void ugHistory_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            for (int i = 0; i < e.Layout.Bands.Count; i++)
            {
                UltraGridBand band = e.Layout.Bands[i];

                // ID
                UltraGridColumn clmn = band.Columns["ID"];
                clmn.AllowGroupBy = Infragistics.Win.DefaultableBoolean.False;
                clmn.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
                clmn.Header.VisiblePosition = 0;
                clmn.Width = 37;
                // ActionDate
                clmn = band.Columns["ActionDate"];
                clmn.Header.Caption = "Дата/время изменения";
                clmn.Header.VisiblePosition = 1;
                clmn.MaskClipMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiteralsWithPadding;
                clmn.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiteralsWithPadding;
                clmn.MaskInput = "{LOC}dd/mm/yyyy hh:mm:ss";
                // Action
                clmn = band.Columns["Action"];
                clmn.Header.Caption = "Действие";
                clmn.Header.VisiblePosition = 2;
                clmn.Width = 170;
                // OldState
                clmn = band.Columns["OldState"];
                clmn.Header.Caption = "Старое состояние";
                clmn.Header.VisiblePosition = 4;
                clmn.Width = 132;
                // NewState
                clmn = band.Columns["NewState"];
                clmn.Header.Caption = "Новое состояние";
                clmn.Header.VisiblePosition = 5;
                clmn.Width = 132;
                // RefUsers
                clmn = band.Columns["RefUsers"];
                clmn.Header.Caption = "Пользователь";
                clmn.Header.VisiblePosition = 3;
                clmn.Hidden = true;
                // RefUsers_Name
                clmn = band.Columns["RefUsers_Name"];
                clmn.Header.Caption = "Пользователь";
                clmn.Header.VisiblePosition = 6;
                clmn.Width = 130;

            }
        }

        private void tbNotNullTextBoxControl_Leave(object sender, EventArgs e)
        {
            TextBox _sender = (TextBox)sender;
            if (_sender.Enabled)
            {
                if (_sender.Text == String.Empty)
                    _sender.Text = "Не указано";
            }
        }

	}
}

