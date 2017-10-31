namespace Krista.FM.Client.MDXExpert.Controls
{
    partial class DrillThroughForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DrillThroughForm));
            this.ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            this.btExcelExport = new Infragistics.Win.Misc.UltraButton();
            this.btOK = new Infragistics.Win.Misc.UltraButton();
            this.gDrillThroughData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraPanel1.ClientArea.SuspendLayout();
            this.ultraPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gDrillThroughData)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraPanel1
            // 
            // 
            // ultraPanel1.ClientArea
            // 
            this.ultraPanel1.ClientArea.Controls.Add(this.btExcelExport);
            this.ultraPanel1.ClientArea.Controls.Add(this.btOK);
            this.ultraPanel1.ClientArea.Controls.Add(this.gDrillThroughData);
            this.ultraPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraPanel1.Location = new System.Drawing.Point(0, 0);
            this.ultraPanel1.Name = "ultraPanel1";
            this.ultraPanel1.Size = new System.Drawing.Size(640, 462);
            this.ultraPanel1.TabIndex = 0;
            // 
            // btExcelExport
            // 
            this.btExcelExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = ((object)(resources.GetObject("appearance1.Image")));
            this.btExcelExport.Appearance = appearance1;
            this.btExcelExport.Location = new System.Drawing.Point(446, 433);
            this.btExcelExport.Name = "btExcelExport";
            this.btExcelExport.Size = new System.Drawing.Size(91, 23);
            this.btExcelExport.TabIndex = 2;
            this.btExcelExport.Text = "Экспорт...";
            this.btExcelExport.Click += new System.EventHandler(this.btExcelExport_Click);
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.Location = new System.Drawing.Point(543, 433);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(85, 23);
            this.btOK.TabIndex = 1;
            this.btOK.Text = "OK";
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // gDrillThroughData
            // 
            this.gDrillThroughData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gDrillThroughData.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.gDrillThroughData.Location = new System.Drawing.Point(3, 3);
            this.gDrillThroughData.Name = "gDrillThroughData";
            this.gDrillThroughData.Size = new System.Drawing.Size(634, 424);
            this.gDrillThroughData.TabIndex = 0;
            // 
            // DrillThroughForm
            // 
            this.AcceptButton = this.btOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 462);
            this.ControlBox = false;
            this.Controls.Add(this.ultraPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DrillThroughForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Детальные данные";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DrillThroughForm_FormClosed);
            this.ultraPanel1.ClientArea.ResumeLayout(false);
            this.ultraPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gDrillThroughData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraPanel ultraPanel1;
        private Infragistics.Win.Misc.UltraButton btOK;
        private Infragistics.Win.UltraWinGrid.UltraGrid gDrillThroughData;
        private Infragistics.Win.Misc.UltraButton btExcelExport;
    }
}