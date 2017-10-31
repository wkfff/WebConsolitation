using Krista.FM.Client.MDXExpert.Controls;
namespace Krista.FM.Client.MDXExpert.FieldList
{
    partial class FieldListEditor
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FieldListEditor));
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint1 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.utpStructure = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.tvFields = new Infragistics.Win.UltraWinTree.UltraTree();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.utpQuery = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.customMDXEditor = new Krista.FM.Client.MDXExpert.Controls.MdxQueryControl();
            this.parentPanel = new Infragistics.Win.Misc.UltraGroupBox();
            this.fieldListContainer = new Infragistics.Win.Misc.UltraGridBagLayoutPanel();
            this.utStructure = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.splitter = new System.Windows.Forms.Splitter();
            this.fieldsContainerPanel = new Infragistics.Win.Misc.UltraGridBagLayoutPanel();
            this.dataUpdatePanel = new Infragistics.Win.Misc.UltraGroupBox();
            this.btUpdate = new Infragistics.Win.Misc.UltraButton();
            this.cbDeferUpdating = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.utpStructure.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tvFields)).BeginInit();
            this.utpQuery.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.parentPanel)).BeginInit();
            this.parentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fieldListContainer)).BeginInit();
            this.fieldListContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.utStructure)).BeginInit();
            this.utStructure.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fieldsContainerPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataUpdatePanel)).BeginInit();
            this.dataUpdatePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbDeferUpdating)).BeginInit();
            this.SuspendLayout();
            // 
            // utpStructure
            // 
            this.utpStructure.Controls.Add(this.tvFields);
            this.utpStructure.Location = new System.Drawing.Point(2, 24);
            this.utpStructure.Name = "utpStructure";
            this.utpStructure.Size = new System.Drawing.Size(256, 421);
            // 
            // tvFields
            // 
            appearance13.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(157)))), ((int)(((byte)(185)))));
            this.tvFields.Appearance = appearance13;
            this.tvFields.BorderStyle = Infragistics.Win.UIElementBorderStyle.Rounded1;
            this.tvFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvFields.FullRowSelect = true;
            this.tvFields.ImageList = this.imageList;
            this.tvFields.Location = new System.Drawing.Point(0, 0);
            this.tvFields.Name = "tvFields";
            _override1.HotTracking = Infragistics.Win.DefaultableBoolean.False;
            _override1.NodeDoubleClickAction = Infragistics.Win.UltraWinTree.NodeDoubleClickAction.None;
            _override1.SelectionType = Infragistics.Win.UltraWinTree.SelectType.SingleAutoDrag;
            _override1.ShowColumns = Infragistics.Win.DefaultableBoolean.True;
            this.tvFields.Override = _override1;
            this.tvFields.ScrollBounds = Infragistics.Win.UltraWinTree.ScrollBounds.ScrollToFill;
            this.tvFields.ShowRootLines = false;
            this.tvFields.Size = new System.Drawing.Size(256, 421);
            this.tvFields.TabIndex = 9;
            this.tvFields.AfterSelect += new Infragistics.Win.UltraWinTree.AfterNodeSelectEventHandler(this.tvFields_AfterSelect);
            this.tvFields.SelectionDragStart += new System.EventHandler(this.tvFields_SelectionDragStart);
            this.tvFields.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tvFields_MouseClick);
            this.tvFields.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tvFields_MouseMove);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.White;
            this.imageList.Images.SetKeyName(0, "cube.bmp");
            this.imageList.Images.SetKeyName(1, "dimension.bmp");
            this.imageList.Images.SetKeyName(2, "hierarch.bmp");
            this.imageList.Images.SetKeyName(3, "measure.bmp");
            this.imageList.Images.SetKeyName(4, "mesuremember.bmp");
            this.imageList.Images.SetKeyName(5, "1.bmp");
            this.imageList.Images.SetKeyName(6, "2.bmp");
            this.imageList.Images.SetKeyName(7, "3.bmp");
            this.imageList.Images.SetKeyName(8, "4.bmp");
            this.imageList.Images.SetKeyName(9, "5.bmp");
            this.imageList.Images.SetKeyName(10, "6.bmp");
            this.imageList.Images.SetKeyName(11, "7.bmp");
            this.imageList.Images.SetKeyName(12, "8.bmp");
            this.imageList.Images.SetKeyName(13, "9.bmp");
            this.imageList.Images.SetKeyName(14, "10.bmp");
            this.imageList.Images.SetKeyName(15, "11.bmp");
            this.imageList.Images.SetKeyName(16, "12.bmp");
            this.imageList.Images.SetKeyName(17, "13.bmp");
            this.imageList.Images.SetKeyName(18, "14.bmp");
            this.imageList.Images.SetKeyName(19, "15.bmp");
            this.imageList.Images.SetKeyName(20, "16.bmp");
            this.imageList.Images.SetKeyName(21, "17.bmp");
            this.imageList.Images.SetKeyName(22, "1.bmp");
            this.imageList.Images.SetKeyName(23, "callcmembers.bmp");
            // 
            // utpQuery
            // 
            this.utpQuery.Controls.Add(this.customMDXEditor);
            this.utpQuery.Location = new System.Drawing.Point(-10000, -10000);
            this.utpQuery.Name = "utpQuery";
            this.utpQuery.Size = new System.Drawing.Size(256, 421);
            // 
            // customMDXEditor
            // 
            this.customMDXEditor.AutoSaveQuery = true;
            this.customMDXEditor.CurrentPivotData = null;
            this.customMDXEditor.DisplayMode = Krista.FM.Client.MDXExpert.Controls.ControlDisplayMode.Simple;
            this.customMDXEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customMDXEditor.Location = new System.Drawing.Point(0, 0);
            this.customMDXEditor.Name = "customMDXEditor";
            this.customMDXEditor.Size = new System.Drawing.Size(256, 421);
            this.customMDXEditor.TabIndex = 2;
            // 
            // parentPanel
            // 
            this.parentPanel.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.None;
            this.parentPanel.Controls.Add(this.fieldListContainer);
            this.parentPanel.Controls.Add(this.splitter);
            this.parentPanel.Controls.Add(this.fieldsContainerPanel);
            this.parentPanel.Controls.Add(this.dataUpdatePanel);
            this.parentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parentPanel.Location = new System.Drawing.Point(0, 0);
            this.parentPanel.Name = "parentPanel";
            this.parentPanel.Size = new System.Drawing.Size(272, 497);
            this.parentPanel.TabIndex = 12;
            this.parentPanel.Resize += new System.EventHandler(this.parentPanel_Resize);
            // 
            // fieldListContainer
            // 
            this.fieldListContainer.BackColor = System.Drawing.Color.Transparent;
            this.fieldListContainer.Controls.Add(this.utStructure);
            this.fieldListContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldListContainer.ExpandToFitHeight = true;
            this.fieldListContainer.ExpandToFitWidth = true;
            this.fieldListContainer.Location = new System.Drawing.Point(1, 0);
            this.fieldListContainer.Name = "fieldListContainer";
            this.fieldListContainer.Padding = new System.Windows.Forms.Padding(5);
            this.fieldListContainer.Size = new System.Drawing.Size(270, 457);
            this.fieldListContainer.TabIndex = 18;
            // 
            // utStructure
            // 
            this.utStructure.Controls.Add(this.ultraTabSharedControlsPage1);
            this.utStructure.Controls.Add(this.utpStructure);
            this.utStructure.Controls.Add(this.utpQuery);
            this.utStructure.Dock = System.Windows.Forms.DockStyle.Fill;
            gridBagConstraint1.Fill = Infragistics.Win.Layout.FillType.Both;
            this.fieldListContainer.SetGridBagConstraint(this.utStructure, gridBagConstraint1);
            this.utStructure.Location = new System.Drawing.Point(5, 5);
            this.utStructure.Name = "utStructure";
            this.fieldListContainer.SetPreferredSize(this.utStructure, new System.Drawing.Size(381, 244));
            this.utStructure.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.utStructure.Size = new System.Drawing.Size(260, 447);
            this.utStructure.TabIndex = 11;
            this.utStructure.TabOrientation = Infragistics.Win.UltraWinTabs.TabOrientation.TopLeft;
            ultraTab1.TabPage = this.utpStructure;
            ultraTab1.Text = "Список полей";
            ultraTab2.TabPage = this.utpQuery;
            ultraTab2.Text = "Запрос";
            this.utStructure.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2});
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(256, 421);
            // 
            // splitter
            // 
            this.splitter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(178)))), ((int)(((byte)(227)))));
            this.splitter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter.Location = new System.Drawing.Point(1, 457);
            this.splitter.MinExtra = 200;
            this.splitter.MinSize = 0;
            this.splitter.Name = "splitter";
            this.splitter.Size = new System.Drawing.Size(270, 5);
            this.splitter.TabIndex = 20;
            this.splitter.TabStop = false;
            // 
            // fieldsContainerPanel
            // 
            this.fieldsContainerPanel.BackColor = System.Drawing.Color.Transparent;
            this.fieldsContainerPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.fieldsContainerPanel.Location = new System.Drawing.Point(1, 462);
            this.fieldsContainerPanel.Name = "fieldsContainerPanel";
            this.fieldsContainerPanel.Size = new System.Drawing.Size(270, 0);
            this.fieldsContainerPanel.TabIndex = 19;
            // 
            // dataUpdatePanel
            // 
            this.dataUpdatePanel.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.None;
            this.dataUpdatePanel.Controls.Add(this.btUpdate);
            this.dataUpdatePanel.Controls.Add(this.cbDeferUpdating);
            this.dataUpdatePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataUpdatePanel.Location = new System.Drawing.Point(1, 462);
            this.dataUpdatePanel.Name = "dataUpdatePanel";
            this.dataUpdatePanel.Size = new System.Drawing.Size(270, 34);
            this.dataUpdatePanel.TabIndex = 21;
            // 
            // btUpdate
            // 
            this.btUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btUpdate.Location = new System.Drawing.Point(191, 5);
            this.btUpdate.Name = "btUpdate";
            this.btUpdate.Size = new System.Drawing.Size(72, 23);
            this.btUpdate.TabIndex = 0;
            this.btUpdate.Text = "Обновить";
            this.btUpdate.Click += new System.EventHandler(this.btUpdate_Click);
            // 
            // cbDeferUpdating
            // 
            this.cbDeferUpdating.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.BackColor = System.Drawing.Color.Transparent;
            appearance2.BackColor2 = System.Drawing.Color.Transparent;
            appearance2.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
            appearance2.BackColorDisabled = System.Drawing.Color.Transparent;
            appearance2.BackColorDisabled2 = System.Drawing.Color.Transparent;
            appearance2.BorderColor = System.Drawing.Color.Transparent;
            appearance2.BorderColor2 = System.Drawing.Color.Transparent;
            this.cbDeferUpdating.Appearance = appearance2;
            this.cbDeferUpdating.BackColor = System.Drawing.Color.Transparent;
            this.cbDeferUpdating.BackColorInternal = System.Drawing.Color.Transparent;
            this.cbDeferUpdating.Location = new System.Drawing.Point(3, 3);
            this.cbDeferUpdating.Name = "cbDeferUpdating";
            this.cbDeferUpdating.Size = new System.Drawing.Size(182, 28);
            this.cbDeferUpdating.TabIndex = 1;
            this.cbDeferUpdating.Text = "Отложить обновление макета";
            this.cbDeferUpdating.CheckedChanged += new System.EventHandler(this.cbDeferUpdating_CheckedChanged);
            // 
            // FieldListEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(216)))), ((int)(((byte)(240)))));
            this.Controls.Add(this.parentPanel);
            this.Name = "FieldListEditor";
            this.Size = new System.Drawing.Size(272, 497);
            this.utpStructure.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tvFields)).EndInit();
            this.utpQuery.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.parentPanel)).EndInit();
            this.parentPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fieldListContainer)).EndInit();
            this.fieldListContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.utStructure)).EndInit();
            this.utStructure.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fieldsContainerPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataUpdatePanel)).EndInit();
            this.dataUpdatePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cbDeferUpdating)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox parentPanel;
        private System.Windows.Forms.ImageList imageList;
        private Infragistics.Win.Misc.UltraGridBagLayoutPanel fieldListContainer;
        private System.Windows.Forms.Splitter splitter;
        private Infragistics.Win.Misc.UltraGridBagLayoutPanel fieldsContainerPanel;
        private Infragistics.Win.Misc.UltraGroupBox dataUpdatePanel;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbDeferUpdating;
        private Infragistics.Win.Misc.UltraButton btUpdate;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl utStructure;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpStructure;
        private Infragistics.Win.UltraWinTree.UltraTree tvFields;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpQuery;
        private MdxQueryControl customMDXEditor;

    }
}
