namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI
{
    partial class frmMergingDuplicates
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.scChoseDuplicates = new System.Windows.Forms.SplitContainer();
            this.ugeMergingDuplicates = new Krista.FM.Client.Components.UltraGridEx();
            this.ugeMainRecord = new Krista.FM.Client.Components.UltraGridEx();
            this.btnStartMerging = new System.Windows.Forms.Button();
            this.btnCancelMerging = new System.Windows.Forms.Button();
            this.lbDuplicatesCount = new System.Windows.Forms.Label();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.scMergingResults = new System.Windows.Forms.SplitContainer();
            this.tbResults = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.pbMergingProgress = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.utcMerging = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.ultraTabPageControl1.SuspendLayout();
            this.scChoseDuplicates.Panel1.SuspendLayout();
            this.scChoseDuplicates.Panel2.SuspendLayout();
            this.scChoseDuplicates.SuspendLayout();
            this.ultraTabPageControl2.SuspendLayout();
            this.scMergingResults.Panel1.SuspendLayout();
            this.scMergingResults.Panel2.SuspendLayout();
            this.scMergingResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbResults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.utcMerging)).BeginInit();
            this.utcMerging.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.scChoseDuplicates);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(632, 453);
            // 
            // scChoseDuplicates
            // 
            this.scChoseDuplicates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scChoseDuplicates.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.scChoseDuplicates.Location = new System.Drawing.Point(0, 0);
            this.scChoseDuplicates.Name = "scChoseDuplicates";
            this.scChoseDuplicates.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scChoseDuplicates.Panel1
            // 
            this.scChoseDuplicates.Panel1.Controls.Add(this.ugeMergingDuplicates);
            this.scChoseDuplicates.Panel1.Controls.Add(this.ugeMainRecord);
            // 
            // scChoseDuplicates.Panel2
            // 
            this.scChoseDuplicates.Panel2.Controls.Add(this.btnStartMerging);
            this.scChoseDuplicates.Panel2.Controls.Add(this.btnCancelMerging);
            this.scChoseDuplicates.Panel2.Controls.Add(this.lbDuplicatesCount);
            this.scChoseDuplicates.Panel2MinSize = 70;
            this.scChoseDuplicates.Size = new System.Drawing.Size(632, 453);
            this.scChoseDuplicates.SplitterDistance = 379;
            this.scChoseDuplicates.TabIndex = 2;
            // 
            // ugeMergingDuplicates
            // 
            this.ugeMergingDuplicates.AllowAddNewRecords = true;
            this.ugeMergingDuplicates.AllowClearTable = true;
            this.ugeMergingDuplicates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugeMergingDuplicates.InDebugMode = false;
            this.ugeMergingDuplicates.LoadMenuVisible = false;
            this.ugeMergingDuplicates.Location = new System.Drawing.Point(0, 95);
            this.ugeMergingDuplicates.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ugeMergingDuplicates.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ugeMergingDuplicates.Name = "ugeMergingDuplicates";
            this.ugeMergingDuplicates.SaveLoadFileName = "";
            this.ugeMergingDuplicates.SaveMenuVisible = false;
            this.ugeMergingDuplicates.ServerFilterEnabled = false;
            this.ugeMergingDuplicates.SingleBandLevelName = "Добавить запись...";
            this.ugeMergingDuplicates.Size = new System.Drawing.Size(632, 284);
            this.ugeMergingDuplicates.sortColumnName = "";
            this.ugeMergingDuplicates.StateRowEnable = false;
            this.ugeMergingDuplicates.TabIndex = 2;
            // 
            // ugeMainRecord
            // 
            this.ugeMainRecord.AllowAddNewRecords = true;
            this.ugeMainRecord.AllowClearTable = true;
            this.ugeMainRecord.Dock = System.Windows.Forms.DockStyle.Top;
            this.ugeMainRecord.InDebugMode = false;
            this.ugeMainRecord.LoadMenuVisible = false;
            this.ugeMainRecord.Location = new System.Drawing.Point(0, 0);
            this.ugeMainRecord.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ugeMainRecord.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ugeMainRecord.Name = "ugeMainRecord";
            this.ugeMainRecord.SaveLoadFileName = "";
            this.ugeMainRecord.SaveMenuVisible = false;
            this.ugeMainRecord.ServerFilterEnabled = false;
            this.ugeMainRecord.SingleBandLevelName = "Добавить запись...";
            this.ugeMainRecord.Size = new System.Drawing.Size(632, 95);
            this.ugeMainRecord.sortColumnName = "";
            this.ugeMainRecord.StateRowEnable = false;
            this.ugeMainRecord.TabIndex = 1;
            // 
            // btnStartMerging
            // 
            this.btnStartMerging.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartMerging.Location = new System.Drawing.Point(530, 26);
            this.btnStartMerging.Name = "btnStartMerging";
            this.btnStartMerging.Size = new System.Drawing.Size(90, 23);
            this.btnStartMerging.TabIndex = 9;
            this.btnStartMerging.Text = "Объединить";
            this.btnStartMerging.UseVisualStyleBackColor = true;
            this.btnStartMerging.Click += new System.EventHandler(this.btnStartMerging_Click);
            // 
            // btnCancelMerging
            // 
            this.btnCancelMerging.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelMerging.Location = new System.Drawing.Point(436, 26);
            this.btnCancelMerging.Name = "btnCancelMerging";
            this.btnCancelMerging.Size = new System.Drawing.Size(90, 23);
            this.btnCancelMerging.TabIndex = 8;
            this.btnCancelMerging.Text = "Отмена";
            this.btnCancelMerging.UseVisualStyleBackColor = true;
            this.btnCancelMerging.Click += new System.EventHandler(this.btnCancelMerging_Click);
            // 
            // lbDuplicatesCount
            // 
            this.lbDuplicatesCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbDuplicatesCount.AutoSize = true;
            this.lbDuplicatesCount.Location = new System.Drawing.Point(12, 31);
            this.lbDuplicatesCount.Name = "lbDuplicatesCount";
            this.lbDuplicatesCount.Size = new System.Drawing.Size(0, 13);
            this.lbDuplicatesCount.TabIndex = 7;
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.scMergingResults);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(0, 0);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(632, 453);
            // 
            // scMergingResults
            // 
            this.scMergingResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scMergingResults.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.scMergingResults.Location = new System.Drawing.Point(0, 0);
            this.scMergingResults.Name = "scMergingResults";
            this.scMergingResults.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scMergingResults.Panel1
            // 
            this.scMergingResults.Panel1.Controls.Add(this.tbResults);
            // 
            // scMergingResults.Panel2
            // 
            this.scMergingResults.Panel2.Controls.Add(this.pbMergingProgress);
            this.scMergingResults.Panel2.Controls.Add(this.btnSave);
            this.scMergingResults.Panel2.Controls.Add(this.btnOK);
            this.scMergingResults.Panel2MinSize = 70;
            this.scMergingResults.Size = new System.Drawing.Size(632, 453);
            this.scMergingResults.SplitterDistance = 379;
            this.scMergingResults.TabIndex = 0;
            // 
            // tbResults
            // 
            this.tbResults.AlwaysInEditMode = true;
            this.tbResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbResults.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.tbResults.Location = new System.Drawing.Point(12, 3);
            this.tbResults.Multiline = true;
            this.tbResults.Name = "tbResults";
            this.tbResults.ReadOnly = true;
            this.tbResults.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbResults.Size = new System.Drawing.Size(620, 373);
            this.tbResults.TabIndex = 0;
            // 
            // pbMergingProgress
            // 
            this.pbMergingProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbMergingProgress.BorderStyle = Infragistics.Win.UIElementBorderStyle.InsetSoft;
            this.pbMergingProgress.Location = new System.Drawing.Point(12, 26);
            this.pbMergingProgress.Name = "pbMergingProgress";
            this.pbMergingProgress.Size = new System.Drawing.Size(384, 23);
            this.pbMergingProgress.TabIndex = 2;
            this.pbMergingProgress.Text = "[Formatted]";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(406, 26);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Сохранить протокол";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(530, 26);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // utcMerging
            // 
            this.utcMerging.Controls.Add(this.ultraTabSharedControlsPage1);
            this.utcMerging.Controls.Add(this.ultraTabPageControl1);
            this.utcMerging.Controls.Add(this.ultraTabPageControl2);
            this.utcMerging.Dock = System.Windows.Forms.DockStyle.Fill;
            this.utcMerging.Location = new System.Drawing.Point(0, 0);
            this.utcMerging.Name = "utcMerging";
            this.utcMerging.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.utcMerging.Size = new System.Drawing.Size(632, 453);
            this.utcMerging.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Wizard;
            this.utcMerging.TabIndex = 1;
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "tab1";
            ultraTab2.TabPage = this.ultraTabPageControl2;
            ultraTab2.Text = "tab2";
            this.utcMerging.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2});
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(632, 453);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "txt";
            this.saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt";
            // 
            // frmMergingDuplicates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 453);
            this.Controls.Add(this.utcMerging);
            this.Name = "frmMergingDuplicates";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MergingDuplicatesWizard";
            this.ultraTabPageControl1.ResumeLayout(false);
            this.scChoseDuplicates.Panel1.ResumeLayout(false);
            this.scChoseDuplicates.Panel2.ResumeLayout(false);
            this.scChoseDuplicates.Panel2.PerformLayout();
            this.scChoseDuplicates.ResumeLayout(false);
            this.ultraTabPageControl2.ResumeLayout(false);
            this.scMergingResults.Panel1.ResumeLayout(false);
            this.scMergingResults.Panel1.PerformLayout();
            this.scMergingResults.Panel2.ResumeLayout(false);
            this.scMergingResults.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbResults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.utcMerging)).EndInit();
            this.utcMerging.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinTabControl.UltraTabControl utcMerging;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private System.Windows.Forms.SplitContainer scMergingResults;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar pbMergingProgress;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor tbResults;
        private System.Windows.Forms.SplitContainer scChoseDuplicates;
        private Krista.FM.Client.Components.UltraGridEx ugeMergingDuplicates;
        private Krista.FM.Client.Components.UltraGridEx ugeMainRecord;
        private System.Windows.Forms.Button btnStartMerging;
        private System.Windows.Forms.Button btnCancelMerging;
        private System.Windows.Forms.Label lbDuplicatesCount;


    }
}