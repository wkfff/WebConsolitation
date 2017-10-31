namespace Krista.FM.Client.MDXExpert.BrowseAdapters.ChartReportAdapters.ChartEditors
{
    partial class ChartSyncForm
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            this.ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            this.btCancel = new Infragistics.Win.Misc.UltraButton();
            this.cbSyncEnabled = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.gbMeasurePlacing = new Infragistics.Win.Misc.UltraGroupBox();
            this.osMeasurePlacing = new Infragistics.Win.UltraWinEditors.UltraOptionSet();
            this.btOK = new Infragistics.Win.Misc.UltraButton();
            this.ultraPanel1.ClientArea.SuspendLayout();
            this.ultraPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbSyncEnabled)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbMeasurePlacing)).BeginInit();
            this.gbMeasurePlacing.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.osMeasurePlacing)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraPanel1
            // 
            // 
            // ultraPanel1.ClientArea
            // 
            this.ultraPanel1.ClientArea.Controls.Add(this.btCancel);
            this.ultraPanel1.ClientArea.Controls.Add(this.cbSyncEnabled);
            this.ultraPanel1.ClientArea.Controls.Add(this.gbMeasurePlacing);
            this.ultraPanel1.ClientArea.Controls.Add(this.btOK);
            this.ultraPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraPanel1.Location = new System.Drawing.Point(0, 0);
            this.ultraPanel1.Name = "ultraPanel1";
            this.ultraPanel1.Size = new System.Drawing.Size(253, 156);
            this.ultraPanel1.TabIndex = 1;
            // 
            // btCancel
            // 
            this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(191, 126);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(55, 23);
            this.btCancel.TabIndex = 7;
            this.btCancel.Text = "Отмена";
            // 
            // cbSyncEnabled
            // 
            appearance1.BackColor = System.Drawing.Color.Transparent;
            this.cbSyncEnabled.Appearance = appearance1;
            this.cbSyncEnabled.BackColor = System.Drawing.Color.Transparent;
            this.cbSyncEnabled.BackColorInternal = System.Drawing.Color.Transparent;
            this.cbSyncEnabled.Location = new System.Drawing.Point(12, 85);
            this.cbSyncEnabled.Name = "cbSyncEnabled";
            this.cbSyncEnabled.Size = new System.Drawing.Size(126, 20);
            this.cbSyncEnabled.TabIndex = 6;
            this.cbSyncEnabled.Text = "Синхронизировать";
            // 
            // gbMeasurePlacing
            // 
            this.gbMeasurePlacing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbMeasurePlacing.Controls.Add(this.osMeasurePlacing);
            this.gbMeasurePlacing.Location = new System.Drawing.Point(12, 12);
            this.gbMeasurePlacing.Name = "gbMeasurePlacing";
            this.gbMeasurePlacing.Size = new System.Drawing.Size(232, 64);
            this.gbMeasurePlacing.TabIndex = 4;
            this.gbMeasurePlacing.Text = "Размещение мер";
            // 
            // osMeasurePlacing
            // 
            this.osMeasurePlacing.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.osMeasurePlacing.BackColor = System.Drawing.Color.Transparent;
            this.osMeasurePlacing.BackColorInternal = System.Drawing.Color.Transparent;
            this.osMeasurePlacing.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.osMeasurePlacing.CheckedIndex = 0;
            valueListItem1.DataValue = "rows";
            valueListItem1.DisplayText = "В рядах";
            valueListItem2.DataValue = "columns";
            valueListItem2.DisplayText = "В категориях";
            this.osMeasurePlacing.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2});
            this.osMeasurePlacing.ItemSpacingVertical = 5;
            this.osMeasurePlacing.Location = new System.Drawing.Point(6, 19);
            this.osMeasurePlacing.Name = "osMeasurePlacing";
            this.osMeasurePlacing.Size = new System.Drawing.Size(211, 39);
            this.osMeasurePlacing.TabIndex = 0;
            this.osMeasurePlacing.Text = "В рядах";
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.Location = new System.Drawing.Point(132, 126);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(55, 23);
            this.btOK.TabIndex = 0;
            this.btOK.Text = "OK";
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // ChartSyncForm
            // 
            this.AcceptButton = this.btOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btCancel;
            this.ClientSize = new System.Drawing.Size(253, 156);
            this.ControlBox = false;
            this.Controls.Add(this.ultraPanel1);
            this.Name = "ChartSyncForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Создание диаграммы по таблице";
            this.ultraPanel1.ClientArea.ResumeLayout(false);
            this.ultraPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cbSyncEnabled)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbMeasurePlacing)).EndInit();
            this.gbMeasurePlacing.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.osMeasurePlacing)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraPanel ultraPanel1;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbSyncEnabled;
        private Infragistics.Win.Misc.UltraGroupBox gbMeasurePlacing;
        private Infragistics.Win.UltraWinEditors.UltraOptionSet osMeasurePlacing;
        private Infragistics.Win.Misc.UltraButton btOK;
        private Infragistics.Win.Misc.UltraButton btCancel;
    }
}