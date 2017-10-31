namespace Krista.FM.Client.MDXExpert.BrowseAdapters.MapReportAdapters.MapEditors
{
    partial class MapSyncForm
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
            this.gbObjectsPlacing = new Infragistics.Win.Misc.UltraGroupBox();
            this.osObjectsPlacing = new Infragistics.Win.UltraWinEditors.UltraOptionSet();
            this.btOK = new Infragistics.Win.Misc.UltraButton();
            this.ultraPanel1.ClientArea.SuspendLayout();
            this.ultraPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbSyncEnabled)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbObjectsPlacing)).BeginInit();
            this.gbObjectsPlacing.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.osObjectsPlacing)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraPanel1
            // 
            // 
            // ultraPanel1.ClientArea
            // 
            this.ultraPanel1.ClientArea.Controls.Add(this.btCancel);
            this.ultraPanel1.ClientArea.Controls.Add(this.cbSyncEnabled);
            this.ultraPanel1.ClientArea.Controls.Add(this.gbObjectsPlacing);
            this.ultraPanel1.ClientArea.Controls.Add(this.btOK);
            this.ultraPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraPanel1.Location = new System.Drawing.Point(0, 0);
            this.ultraPanel1.Name = "ultraPanel1";
            this.ultraPanel1.Size = new System.Drawing.Size(250, 157);
            this.ultraPanel1.TabIndex = 1;
            // 
            // btCancel
            // 
            this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(185, 129);
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
            this.cbSyncEnabled.Location = new System.Drawing.Point(12, 84);
            this.cbSyncEnabled.Name = "cbSyncEnabled";
            this.cbSyncEnabled.Size = new System.Drawing.Size(126, 20);
            this.cbSyncEnabled.TabIndex = 6;
            this.cbSyncEnabled.Text = "Синхронизировать";
            // 
            // gbObjectsPlacing
            // 
            this.gbObjectsPlacing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbObjectsPlacing.Controls.Add(this.osObjectsPlacing);
            this.gbObjectsPlacing.Location = new System.Drawing.Point(13, 12);
            this.gbObjectsPlacing.Name = "gbObjectsPlacing";
            this.gbObjectsPlacing.Size = new System.Drawing.Size(225, 66);
            this.gbObjectsPlacing.TabIndex = 4;
            this.gbObjectsPlacing.Text = "Размещение объектов";
            // 
            // osObjectsPlacing
            // 
            this.osObjectsPlacing.BackColor = System.Drawing.Color.Transparent;
            this.osObjectsPlacing.BackColorInternal = System.Drawing.Color.Transparent;
            this.osObjectsPlacing.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.osObjectsPlacing.CheckedIndex = 0;
            valueListItem1.DataValue = "rows";
            valueListItem1.DisplayText = "В строках";
            valueListItem2.DataValue = "columns";
            valueListItem2.DisplayText = "В столбцах";
            this.osObjectsPlacing.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2});
            this.osObjectsPlacing.ItemSpacingVertical = 5;
            this.osObjectsPlacing.Location = new System.Drawing.Point(6, 19);
            this.osObjectsPlacing.Name = "osObjectsPlacing";
            this.osObjectsPlacing.Size = new System.Drawing.Size(154, 42);
            this.osObjectsPlacing.TabIndex = 0;
            this.osObjectsPlacing.Text = "В строках";
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.Location = new System.Drawing.Point(124, 129);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(55, 23);
            this.btOK.TabIndex = 0;
            this.btOK.Text = "OK";
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // MapSyncForm
            // 
            this.AcceptButton = this.btOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btCancel;
            this.ClientSize = new System.Drawing.Size(250, 157);
            this.ControlBox = false;
            this.Controls.Add(this.ultraPanel1);
            this.Name = "MapSyncForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Создание карты по таблице";
            this.ultraPanel1.ClientArea.ResumeLayout(false);
            this.ultraPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cbSyncEnabled)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbObjectsPlacing)).EndInit();
            this.gbObjectsPlacing.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.osObjectsPlacing)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraPanel ultraPanel1;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbSyncEnabled;
        private Infragistics.Win.Misc.UltraGroupBox gbObjectsPlacing;
        private Infragistics.Win.UltraWinEditors.UltraOptionSet osObjectsPlacing;
        private Infragistics.Win.Misc.UltraButton btOK;
        private Infragistics.Win.Misc.UltraButton btCancel;
    }
}