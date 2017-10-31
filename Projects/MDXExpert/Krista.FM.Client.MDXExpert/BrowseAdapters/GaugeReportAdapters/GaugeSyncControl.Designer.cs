namespace Krista.FM.Client.MDXExpert
{
    partial class GaugeSyncControl
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
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            this.cbSyncEnabled = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.cbTables = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.btOK = new Infragistics.Win.Misc.UltraButton();
            this.ceIsCurrentColumnValues = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraPanel1.ClientArea.SuspendLayout();
            this.ultraPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbSyncEnabled)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTables)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceIsCurrentColumnValues)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraPanel1
            // 
            // 
            // ultraPanel1.ClientArea
            // 
            this.ultraPanel1.ClientArea.Controls.Add(this.ceIsCurrentColumnValues);
            this.ultraPanel1.ClientArea.Controls.Add(this.cbSyncEnabled);
            this.ultraPanel1.ClientArea.Controls.Add(this.cbTables);
            this.ultraPanel1.ClientArea.Controls.Add(this.ultraLabel1);
            this.ultraPanel1.ClientArea.Controls.Add(this.btOK);
            this.ultraPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraPanel1.Location = new System.Drawing.Point(0, 0);
            this.ultraPanel1.Name = "ultraPanel1";
            this.ultraPanel1.Size = new System.Drawing.Size(229, 132);
            this.ultraPanel1.TabIndex = 0;
            // 
            // cbSyncEnabled
            // 
            appearance2.BackColor = System.Drawing.Color.Transparent;
            this.cbSyncEnabled.Appearance = appearance2;
            this.cbSyncEnabled.BackColor = System.Drawing.Color.Transparent;
            this.cbSyncEnabled.BackColorInternal = System.Drawing.Color.Transparent;
            this.cbSyncEnabled.Location = new System.Drawing.Point(13, 87);
            this.cbSyncEnabled.Name = "cbSyncEnabled";
            this.cbSyncEnabled.Size = new System.Drawing.Size(126, 20);
            this.cbSyncEnabled.TabIndex = 6;
            this.cbSyncEnabled.Text = "Синхронизировать";
            this.cbSyncEnabled.CheckedChanged += new System.EventHandler(this.cbSyncEnabled_CheckedChanged);
            // 
            // cbTables
            // 
            this.cbTables.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cbTables.Location = new System.Drawing.Point(13, 22);
            this.cbTables.Name = "cbTables";
            this.cbTables.Size = new System.Drawing.Size(209, 21);
            this.cbTables.TabIndex = 5;
            // 
            // ultraLabel1
            // 
            appearance3.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel1.Appearance = appearance3;
            this.ultraLabel1.Location = new System.Drawing.Point(13, 7);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(190, 13);
            this.ultraLabel1.TabIndex = 3;
            this.ultraLabel1.Text = "Таблица:";
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.Location = new System.Drawing.Point(167, 104);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(55, 23);
            this.btOK.TabIndex = 0;
            this.btOK.Text = "OK";
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // ceIsCurrentColumnValues
            // 
            this.ceIsCurrentColumnValues.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.BackColor = System.Drawing.Color.Transparent;
            this.ceIsCurrentColumnValues.Appearance = appearance1;
            this.ceIsCurrentColumnValues.BackColor = System.Drawing.Color.Transparent;
            this.ceIsCurrentColumnValues.BackColorInternal = System.Drawing.Color.Transparent;
            this.ceIsCurrentColumnValues.Location = new System.Drawing.Point(13, 49);
            this.ceIsCurrentColumnValues.Name = "ceIsCurrentColumnValues";
            this.ceIsCurrentColumnValues.Size = new System.Drawing.Size(209, 32);
            this.ceIsCurrentColumnValues.TabIndex = 7;
            this.ceIsCurrentColumnValues.Text = "Учитывать значения только текущего столбца";
            this.ceIsCurrentColumnValues.CheckedChanged += new System.EventHandler(this.ceIsCurrentColumnValues_CheckedChanged);
            // 
            // GaugeSyncControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraPanel1);
            this.Name = "GaugeSyncControl";
            this.Size = new System.Drawing.Size(229, 132);
            this.ultraPanel1.ClientArea.ResumeLayout(false);
            this.ultraPanel1.ClientArea.PerformLayout();
            this.ultraPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cbSyncEnabled)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTables)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceIsCurrentColumnValues)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraPanel ultraPanel1;
        private Infragistics.Win.Misc.UltraButton btOK;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cbTables;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbSyncEnabled;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor ceIsCurrentColumnValues;
    }
}
