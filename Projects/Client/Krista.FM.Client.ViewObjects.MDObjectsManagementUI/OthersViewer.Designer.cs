namespace Krista.FM.Client.ViewObjects.MDObjectsManagementUI
{
    partial class OthersViewer
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Semantic");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Caption");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Description");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("FullName");
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.btnRefreshPlanningsheetMetadata = new System.Windows.Forms.Button();
            this.btnCreateMDBase = new System.Windows.Forms.Button();
            this.btnSetCubesHierarchy = new System.Windows.Forms.Button();
            this.udsDataCls = new Infragistics.Win.UltraWinDataSource.UltraDataSource();
            this.spSplitter = new System.Windows.Forms.Splitter();
            this.tbInfo = new System.Windows.Forms.TextBox();
            this.utcMain = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.btnCreateCustomDimensions = new System.Windows.Forms.Button();
            this.ultraTabPageControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udsDataCls)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.utcMain)).BeginInit();
            this.utcMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.btnCreateCustomDimensions);
            this.ultraTabPageControl1.Controls.Add(this.btnRefreshPlanningsheetMetadata);
            this.ultraTabPageControl1.Controls.Add(this.btnCreateMDBase);
            this.ultraTabPageControl1.Controls.Add(this.btnSetCubesHierarchy);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(722, 346);
            // 
            // btnRefreshPlanningsheetMetadata
            // 
            this.btnRefreshPlanningsheetMetadata.Location = new System.Drawing.Point(19, 131);
            this.btnRefreshPlanningsheetMetadata.Name = "btnRefreshPlanningsheetMetadata";
            this.btnRefreshPlanningsheetMetadata.Size = new System.Drawing.Size(320, 23);
            this.btnRefreshPlanningsheetMetadata.TabIndex = 2;
            this.btnRefreshPlanningsheetMetadata.Text = "Обновить метаданные для надстройки MS Office";
            this.btnRefreshPlanningsheetMetadata.UseVisualStyleBackColor = true;
            // 
            // btnCreateMDBase
            // 
            this.btnCreateMDBase.Location = new System.Drawing.Point(19, 57);
            this.btnCreateMDBase.Name = "btnCreateMDBase";
            this.btnCreateMDBase.Size = new System.Drawing.Size(320, 23);
            this.btnCreateMDBase.TabIndex = 1;
            this.btnCreateMDBase.Text = "Сгенерировать многомерную базу";
            this.btnCreateMDBase.UseVisualStyleBackColor = true;
            // 
            // btnSetCubesHierarchy
            // 
            this.btnSetCubesHierarchy.Location = new System.Drawing.Point(19, 20);
            this.btnSetCubesHierarchy.Name = "btnSetCubesHierarchy";
            this.btnSetCubesHierarchy.Size = new System.Drawing.Size(320, 23);
            this.btnSetCubesHierarchy.TabIndex = 0;
            this.btnSetCubesHierarchy.Text = "Установить иерархию для кубов";
            this.btnSetCubesHierarchy.UseVisualStyleBackColor = true;

            // 
            // udsDataCls
            // 
            this.udsDataCls.AllowAdd = false;
            this.udsDataCls.AllowDelete = false;
            ultraDataColumn1.ReadOnly = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn2.ReadOnly = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn3.ReadOnly = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn4.ReadOnly = Infragistics.Win.DefaultableBoolean.True;
            this.udsDataCls.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4});
            this.udsDataCls.Band.Key = "Main";
            this.udsDataCls.ReadOnly = true;

            // 
            // spSplitter
            // 
            this.spSplitter.Dock = System.Windows.Forms.DockStyle.Top;
            this.spSplitter.Location = new System.Drawing.Point(0, 372);
            this.spSplitter.Name = "spSplitter";
            this.spSplitter.Size = new System.Drawing.Size(726, 3);
            this.spSplitter.TabIndex = 8;
            this.spSplitter.TabStop = false;
            // 
            // tbInfo
            // 
            this.tbInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbInfo.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbInfo.Location = new System.Drawing.Point(0, 372);
            this.tbInfo.Multiline = true;
            this.tbInfo.Name = "tbInfo";
            this.tbInfo.ReadOnly = true;
            this.tbInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbInfo.Size = new System.Drawing.Size(726, 180);
            this.tbInfo.TabIndex = 7;
            // 
            // utcMain
            // 
            this.utcMain.Controls.Add(this.ultraTabSharedControlsPage1);
            this.utcMain.Controls.Add(this.ultraTabPageControl1);
            this.utcMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.utcMain.Location = new System.Drawing.Point(0, 0);
            this.utcMain.Name = "utcMain";
            this.utcMain.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.utcMain.Size = new System.Drawing.Size(726, 372);
            this.utcMain.TabIndex = 6;
            ultraTab1.Key = "utManagement";
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "Управление";
            this.utcMain.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1});
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(722, 346);
            // 
            // btnCreateCustomDimensions
            // 
            this.btnCreateCustomDimensions.Location = new System.Drawing.Point(19, 94);
            this.btnCreateCustomDimensions.Name = "btnCreateCustomDimensions";
            this.btnCreateCustomDimensions.Size = new System.Drawing.Size(320, 23);
            this.btnCreateCustomDimensions.TabIndex = 3;
            this.btnCreateCustomDimensions.Text = "Сгенерировать специальные измерения";
            this.btnCreateCustomDimensions.UseVisualStyleBackColor = true;
            // 
            // OthersViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.spSplitter);
            this.Controls.Add(this.tbInfo);
            this.Controls.Add(this.utcMain);
            this.Name = "OthersViewer";
            this.Size = new System.Drawing.Size(726, 552);
            this.ultraTabPageControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.udsDataCls)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.utcMain)).EndInit();
            this.utcMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Splitter spSplitter;
        public System.Windows.Forms.TextBox tbInfo;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl utcMain;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        public System.Windows.Forms.Button btnRefreshPlanningsheetMetadata;
        public System.Windows.Forms.Button btnCreateMDBase;
        public System.Windows.Forms.Button btnSetCubesHierarchy;
        public Infragistics.Win.UltraWinDataSource.UltraDataSource udsDataCls;
        public System.Windows.Forms.Button btnCreateCustomDimensions;
    }
}
