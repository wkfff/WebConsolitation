using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Infragistics.Win.UltraWinDataSource;

using Krista.FM.Client.Common;
using CC = Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Infragistics.Win.Misc;


namespace Krista.FM.Client.ViewObjects.ProtocolsUI
{
    public class ProtocolsView : Krista.FM.Client.ViewObjects.BaseViewObject.BaseView
	{
        internal ImageList ilColumns;
        internal ImageList ilFilter;
        public Infragistics.Win.UltraWinTabControl.UltraTabControl utcProtocols;
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
        public Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpAssociate;
        public Infragistics.Win.UltraWinGrid.UltraGrid ugAssociations;
        internal Krista.FM.Client.Components.UltraGridEx ugeAudit;
        internal Panel pnControls;
        internal PictureBox pbEventImage;
        internal Infragistics.Win.UltraWinGrid.UltraCombo ucEvents;
        internal CheckBox cbUseEventType;
        internal CheckBox cbUsePeriod;
        private UltraLabel laEndPeriod;
        private UltraLabel laBeginPeriod;
        public Infragistics.Win.UltraWinEditors.UltraDateTimeEditor udteEndPeriod;
        public Infragistics.Win.UltraWinEditors.UltraDateTimeEditor udteBeginPeriod;
        internal Krista.FM.Client.Components.UltraGridEx ugex1;
        internal ImageList ilObjectType;
		private IContainer components;
    
        public ProtocolsView()
        {
            InitializeComponent();
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Caption");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("EventCode");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProtocolsView));
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.utpData = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.pnDataTemplate = new System.Windows.Forms.Panel();
            this.pnDataTemplate_Fill_Panel = new System.Windows.Forms.Panel();
            this.ugex1 = new Krista.FM.Client.Components.UltraGridEx();
            this.pnControls = new System.Windows.Forms.Panel();
            this.pbEventImage = new System.Windows.Forms.PictureBox();
            this.ucEvents = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.ilColumns = new System.Windows.Forms.ImageList(this.components);
            this.cbUseEventType = new System.Windows.Forms.CheckBox();
            this.cbUsePeriod = new System.Windows.Forms.CheckBox();
            this.laEndPeriod = new UltraLabel();
            this.laBeginPeriod = new UltraLabel();
            this.udteEndPeriod = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.udteBeginPeriod = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this._pnDataTemplate_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._pnDataTemplate_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._pnDataTemplate_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._utpData_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._utpData_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._utpData_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._utpData_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.utpAssociate = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ugeAudit = new Krista.FM.Client.Components.UltraGridEx();
            this.ugAssociations = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ilFilter = new System.Windows.Forms.ImageList(this.components);
            this.utcProtocols = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ultraTabSharedControlsPage2 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ilObjectType = new System.Windows.Forms.ImageList(this.components);
            this.utpData.SuspendLayout();
            this.pnDataTemplate.SuspendLayout();
            this.pnDataTemplate_Fill_Panel.SuspendLayout();
            this.pnControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbEventImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ucEvents)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udteEndPeriod)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udteBeginPeriod)).BeginInit();
            this.utpAssociate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugAssociations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.utcProtocols)).BeginInit();
            this.utcProtocols.SuspendLayout();
            this.SuspendLayout();
            // 
            // utpData
            // 
            this.utpData.Controls.Add(this.pnDataTemplate);
            this.utpData.Controls.Add(this._utpData_Toolbars_Dock_Area_Left);
            this.utpData.Controls.Add(this._utpData_Toolbars_Dock_Area_Right);
            this.utpData.Controls.Add(this._utpData_Toolbars_Dock_Area_Top);
            this.utpData.Controls.Add(this._utpData_Toolbars_Dock_Area_Bottom);
            this.utpData.Location = new System.Drawing.Point(0, 0);
            this.utpData.Name = "utpData";
            this.utpData.Size = new System.Drawing.Size(671, 511);
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
            this.pnDataTemplate.Size = new System.Drawing.Size(671, 511);
            this.pnDataTemplate.TabIndex = 10;
            // 
            // pnDataTemplate_Fill_Panel
            // 
            this.pnDataTemplate_Fill_Panel.Controls.Add(this.ugex1);
            this.pnDataTemplate_Fill_Panel.Controls.Add(this.pnControls);
            this.pnDataTemplate_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.pnDataTemplate_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnDataTemplate_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.pnDataTemplate_Fill_Panel.Name = "pnDataTemplate_Fill_Panel";
            this.pnDataTemplate_Fill_Panel.Size = new System.Drawing.Size(671, 511);
            this.pnDataTemplate_Fill_Panel.TabIndex = 0;
            // 
            // ugex1
            // 
            this.ugex1.AllowAddNewRecords = true;
            this.ugex1.AllowClearTable = true;
            this.ugex1.Cursor = System.Windows.Forms.Cursors.Default;
            this.ugex1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugex1.InDebugMode = false;
            this.ugex1.IsReadOnly = true;
            this.ugex1.LoadMenuVisible = false;
            this.ugex1.Location = new System.Drawing.Point(0, 83);
            this.ugex1.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ugex1.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ugex1.Name = "ugex1";
            this.ugex1.SaveLoadFileName = "";
            this.ugex1.SaveMenuVisible = false;
            this.ugex1.ServerFilterEnabled = false;
            this.ugex1.SingleBandLevelName = "";
            this.ugex1.Size = new System.Drawing.Size(671, 428);
            this.ugex1.sortColumnName = "";
            this.ugex1.StateRowEnable = true;
            this.ugex1.TabIndex = 19;
            // 
            // pnControls
            // 
            this.pnControls.Controls.Add(this.pbEventImage);
            this.pnControls.Controls.Add(this.ucEvents);
            this.pnControls.Controls.Add(this.cbUseEventType);
            this.pnControls.Controls.Add(this.cbUsePeriod);
            this.pnControls.Controls.Add(this.laEndPeriod);
            this.pnControls.Controls.Add(this.laBeginPeriod);
            this.pnControls.Controls.Add(this.udteEndPeriod);
            this.pnControls.Controls.Add(this.udteBeginPeriod);
            this.pnControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnControls.Location = new System.Drawing.Point(0, 0);
            this.pnControls.Name = "pnControls";
            this.pnControls.Size = new System.Drawing.Size(671, 83);
            this.pnControls.TabIndex = 18;
            // 
            // pbEventImage
            // 
            this.pbEventImage.Location = new System.Drawing.Point(117, 52);
            this.pbEventImage.Name = "pbEventImage";
            this.pbEventImage.Size = new System.Drawing.Size(16, 16);
            this.pbEventImage.TabIndex = 25;
            this.pbEventImage.TabStop = false;
            // 
            // ucEvents
            // 
            this.ucEvents.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.ucEvents.Cursor = System.Windows.Forms.Cursors.Default;
            this.ucEvents.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.Header.Caption = "Событие";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 360;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Hidden = true;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2});
            this.ucEvents.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.ucEvents.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ucEvents.DisplayMember = "";
            this.ucEvents.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2003;
            this.ucEvents.DropDownStyle = Infragistics.Win.UltraWinGrid.UltraComboStyle.DropDownList;
            this.ucEvents.Enabled = false;
            this.ucEvents.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ucEvents.ImageList = this.ilColumns;
            this.ucEvents.Location = new System.Drawing.Point(140, 50);
            this.ucEvents.Name = "ucEvents";
            this.ucEvents.Size = new System.Drawing.Size(378, 21);
            this.ucEvents.TabIndex = 24;
            this.ucEvents.ValueMember = "Caption";
            // 
            // ilColumns
            // 
            this.ilColumns.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilColumns.ImageStream")));
            this.ilColumns.TransparentColor = System.Drawing.Color.Magenta;
            this.ilColumns.Images.SetKeyName(0, "BeginWork.bmp");
            this.ilColumns.Images.SetKeyName(1, "Message.bmp");
            this.ilColumns.Images.SetKeyName(2, "Warning.bmp");
            this.ilColumns.Images.SetKeyName(3, "SuccefullFinish.bmp");
            this.ilColumns.Images.SetKeyName(4, "Error.bmp");
            this.ilColumns.Images.SetKeyName(5, "CriticalError.bmp");
            this.ilColumns.Images.SetKeyName(6, "UserBeginWork.bmp");
            this.ilColumns.Images.SetKeyName(7, "UserEndWork.bmp");
            this.ilColumns.Images.SetKeyName(8, "FilePumpBegin.bmp");
            this.ilColumns.Images.SetKeyName(9, "FilePumpSucces.bmp");
            this.ilColumns.Images.SetKeyName(10, "FilePumpError.bmp");
            this.ilColumns.Images.SetKeyName(11, "GoTo.bmp");
            this.ilColumns.Images.SetKeyName(12, "User.bmp");
            this.ilColumns.Images.SetKeyName(13, "users.bmp");
            this.ilColumns.Images.SetKeyName(14, "systemObjects.bmp");
            this.ilColumns.Images.SetKeyName(15, "reference.bmp");
            this.ilColumns.Images.SetKeyName(16, "changeUsersInGroup.bmp");
            this.ilColumns.Images.SetKeyName(17, "Copy.bmp");
            this.ilColumns.Images.SetKeyName(18, "Check.bmp");
            this.ilColumns.Images.SetKeyName(19, "ProtectForm.bmp");
            this.ilColumns.Images.SetKeyName(20, "deleteRows.bmp");
            this.ilColumns.Images.SetKeyName(21, "SchemeUpdate.bmp");
            // 
            // cbUseEventType
            // 
            this.cbUseEventType.AutoSize = true;
            this.cbUseEventType.Location = new System.Drawing.Point(18, 52);
            this.cbUseEventType.Name = "cbUseEventType";
            this.cbUseEventType.Size = new System.Drawing.Size(91, 17);
            this.cbUseEventType.TabIndex = 20;
            this.cbUseEventType.Text = "Тип события";
            // 
            // cbUsePeriod
            // 
            this.cbUsePeriod.AutoSize = true;
            this.cbUsePeriod.Checked = true;
            this.cbUsePeriod.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbUsePeriod.Location = new System.Drawing.Point(18, 17);
            this.cbUsePeriod.Name = "cbUsePeriod";
            this.cbUsePeriod.Size = new System.Drawing.Size(64, 17);
            this.cbUsePeriod.TabIndex = 19;
            this.cbUsePeriod.Text = "Период";
            // 
            // laEndPeriod
            // 
            this.laEndPeriod.AutoSize = true;
            this.laEndPeriod.Location = new System.Drawing.Point(211, 19);
            this.laEndPeriod.Name = "laEndPeriod";
            this.laEndPeriod.Size = new System.Drawing.Size(19, 13);
            this.laEndPeriod.TabIndex = 17;
            this.laEndPeriod.Text = "по";
            // 
            // laBeginPeriod
            // 
            this.laBeginPeriod.AutoSize = true;
            this.laBeginPeriod.Location = new System.Drawing.Point(102, 19);
            this.laBeginPeriod.Name = "laBeginPeriod";
            this.laBeginPeriod.Size = new System.Drawing.Size(13, 13);
            this.laBeginPeriod.TabIndex = 16;
            this.laBeginPeriod.Text = "с";
            // 
            // udteEndPeriod
            // 
            this.udteEndPeriod.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.VisualStudio2005;
            this.udteEndPeriod.Location = new System.Drawing.Point(230, 15);
            this.udteEndPeriod.Name = "udteEndPeriod";
            this.udteEndPeriod.Size = new System.Drawing.Size(88, 21);
            this.udteEndPeriod.TabIndex = 15;
            // 
            // udteBeginPeriod
            // 
            this.udteBeginPeriod.ButtonStyle = Infragistics.Win.UIElementButtonStyle.ButtonSoft;
            this.udteBeginPeriod.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.VisualStudio2005;
            this.udteBeginPeriod.Location = new System.Drawing.Point(117, 15);
            this.udteBeginPeriod.Name = "udteBeginPeriod";
            this.udteBeginPeriod.Size = new System.Drawing.Size(88, 21);
            this.udteBeginPeriod.TabIndex = 14;
            // 
            // _pnDataTemplate_Toolbars_Dock_Area_Left
            // 
            this._pnDataTemplate_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._pnDataTemplate_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._pnDataTemplate_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._pnDataTemplate_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._pnDataTemplate_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._pnDataTemplate_Toolbars_Dock_Area_Left.Name = "_pnDataTemplate_Toolbars_Dock_Area_Left";
            this._pnDataTemplate_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 511);
            // 
            // _pnDataTemplate_Toolbars_Dock_Area_Right
            // 
            this._pnDataTemplate_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._pnDataTemplate_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._pnDataTemplate_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._pnDataTemplate_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._pnDataTemplate_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(671, 0);
            this._pnDataTemplate_Toolbars_Dock_Area_Right.Name = "_pnDataTemplate_Toolbars_Dock_Area_Right";
            this._pnDataTemplate_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 511);
            // 
            // _pnDataTemplate_Toolbars_Dock_Area_Top
            // 
            this._pnDataTemplate_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._pnDataTemplate_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._pnDataTemplate_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._pnDataTemplate_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._pnDataTemplate_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._pnDataTemplate_Toolbars_Dock_Area_Top.Name = "_pnDataTemplate_Toolbars_Dock_Area_Top";
            this._pnDataTemplate_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(671, 0);
            // 
            // _pnDataTemplate_Toolbars_Dock_Area_Bottom
            // 
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 511);
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom.Name = "_pnDataTemplate_Toolbars_Dock_Area_Bottom";
            this._pnDataTemplate_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(671, 0);
            // 
            // _utpData_Toolbars_Dock_Area_Left
            // 
            this._utpData_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._utpData_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._utpData_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._utpData_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._utpData_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._utpData_Toolbars_Dock_Area_Left.Name = "_utpData_Toolbars_Dock_Area_Left";
            this._utpData_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 511);
            // 
            // _utpData_Toolbars_Dock_Area_Right
            // 
            this._utpData_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._utpData_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._utpData_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._utpData_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._utpData_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(671, 0);
            this._utpData_Toolbars_Dock_Area_Right.Name = "_utpData_Toolbars_Dock_Area_Right";
            this._utpData_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 511);
            // 
            // _utpData_Toolbars_Dock_Area_Top
            // 
            this._utpData_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._utpData_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._utpData_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._utpData_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._utpData_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._utpData_Toolbars_Dock_Area_Top.Name = "_utpData_Toolbars_Dock_Area_Top";
            this._utpData_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(671, 0);
            // 
            // _utpData_Toolbars_Dock_Area_Bottom
            // 
            this._utpData_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._utpData_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._utpData_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._utpData_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._utpData_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 511);
            this._utpData_Toolbars_Dock_Area_Bottom.Name = "_utpData_Toolbars_Dock_Area_Bottom";
            this._utpData_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(671, 0);
            // 
            // utpAssociate
            // 
            this.utpAssociate.Controls.Add(this.ugeAudit);
            this.utpAssociate.Controls.Add(this.ugAssociations);
            this.utpAssociate.Location = new System.Drawing.Point(-10000, -10000);
            this.utpAssociate.Name = "utpAssociate";
            this.utpAssociate.Size = new System.Drawing.Size(671, 511);
            // 
            // ugeAudit
            // 
            this.ugeAudit.AllowAddNewRecords = true;
            this.ugeAudit.AllowClearTable = true;
            this.ugeAudit.Cursor = System.Windows.Forms.Cursors.Default;
            this.ugeAudit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugeAudit.InDebugMode = false;
            this.ugeAudit.IsReadOnly = true;
            this.ugeAudit.LoadMenuVisible = false;
            this.ugeAudit.Location = new System.Drawing.Point(0, 0);
            this.ugeAudit.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ugeAudit.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ugeAudit.Name = "ugeAudit";
            this.ugeAudit.SaveLoadFileName = "";
            this.ugeAudit.SaveMenuVisible = false;
            this.ugeAudit.ServerFilterEnabled = false;
            this.ugeAudit.SingleBandLevelName = "";
            this.ugeAudit.Size = new System.Drawing.Size(671, 511);
            this.ugeAudit.sortColumnName = "";
            this.ugeAudit.StateRowEnable = true;
            this.ugeAudit.TabIndex = 18;
            // 
            // ugAssociations
            // 
            this.ugAssociations.Cursor = System.Windows.Forms.Cursors.Default;
            this.ugAssociations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugAssociations.Location = new System.Drawing.Point(0, 0);
            this.ugAssociations.Name = "ugAssociations";
            this.ugAssociations.Size = new System.Drawing.Size(671, 511);
            this.ugAssociations.TabIndex = 1;
            // 
            // ilFilter
            // 
            this.ilFilter.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilFilter.ImageStream")));
            this.ilFilter.TransparentColor = System.Drawing.Color.Magenta;
            this.ilFilter.Images.SetKeyName(0, "runFilter.bmp");
            this.ilFilter.Images.SetKeyName(1, "RefreshData.bmp");
            this.ilFilter.Images.SetKeyName(2, "GotoAssociation.bmp");
            // 
            // utcProtocols
            // 
            this.utcProtocols.Controls.Add(this.ultraTabSharedControlsPage1);
            this.utcProtocols.Controls.Add(this.ultraTabSharedControlsPage2);
            this.utcProtocols.Controls.Add(this.utpData);
            this.utcProtocols.Controls.Add(this.utpAssociate);
            this.utcProtocols.Dock = System.Windows.Forms.DockStyle.Fill;
            this.utcProtocols.Location = new System.Drawing.Point(0, 0);
            this.utcProtocols.Name = "utcProtocols";
            this.utcProtocols.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.utcProtocols.ShowTabListButton = Infragistics.Win.DefaultableBoolean.True;
            this.utcProtocols.Size = new System.Drawing.Size(671, 511);
            this.utcProtocols.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Wizard;
            this.utcProtocols.TabIndex = 17;
            ultraTab1.TabPage = this.utpData;
            ultraTab1.Text = "Протоколы";
            ultraTab2.TabPage = this.utpAssociate;
            ultraTab2.Text = "Аудит";
            this.utcProtocols.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2});
            this.utcProtocols.ViewStyle = Infragistics.Win.UltraWinTabControl.ViewStyle.VisualStudio2005;
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(671, 511);
            // 
            // ultraTabSharedControlsPage2
            // 
            this.ultraTabSharedControlsPage2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage2.Name = "ultraTabSharedControlsPage2";
            this.ultraTabSharedControlsPage2.Size = new System.Drawing.Size(747, 614);
            // 
            // ilObjectType
            // 
            this.ilObjectType.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilObjectType.ImageStream")));
            this.ilObjectType.TransparentColor = System.Drawing.Color.Magenta;
            this.ilObjectType.Images.SetKeyName(0, "cls_Bridge_16.gif");
            this.ilObjectType.Images.SetKeyName(1, "cls_Data_16.gif");
            this.ilObjectType.Images.SetKeyName(2, "cls_Fixed_16.gif");
            this.ilObjectType.Images.SetKeyName(3, "cls_FactTable_16.gif");
            this.ilObjectType.Images.SetKeyName(4, "Table.bmp");
            // 
            // ProtocolsView
            // 
            this.AutoSize = true;
            this.Controls.Add(this.utcProtocols);
            this.Name = "ProtocolsView";
            this.Size = new System.Drawing.Size(671, 511);
            this.utpData.ResumeLayout(false);
            this.pnDataTemplate.ResumeLayout(false);
            this.pnDataTemplate_Fill_Panel.ResumeLayout(false);
            this.pnControls.ResumeLayout(false);
            this.pnControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbEventImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ucEvents)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udteEndPeriod)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udteBeginPeriod)).EndInit();
            this.utpAssociate.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ugAssociations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.utcProtocols)).EndInit();
            this.utcProtocols.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

		public override void Customize()
		{
			ComponentCustomizer.CustomizeInfragisticsComponents(this.components);
			base.Customize();
		}

		private Infragistics.Win.ToolTip toolTipValue = null;

		public Infragistics.Win.ToolTip pToolTip
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
    }
}
