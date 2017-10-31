namespace Krista.FM.Client.MDXExpert.Controls
{
    partial class MemberPropertiesControl
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
            this.lvMemberProperties = new Infragistics.Win.UltraWinListView.UltraListView();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.btOK = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.lvMemberProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvMemberProperties
            // 
            this.lvMemberProperties.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.lvMemberProperties.Dock = System.Windows.Forms.DockStyle.Top;
            this.lvMemberProperties.Location = new System.Drawing.Point(0, 0);
            this.lvMemberProperties.Name = "lvMemberProperties";
            this.lvMemberProperties.Size = new System.Drawing.Size(211, 180);
            this.lvMemberProperties.TabIndex = 0;
            this.lvMemberProperties.Text = "ultraListView1";
            this.lvMemberProperties.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.List;
            this.lvMemberProperties.ViewSettingsList.CheckBoxStyle = Infragistics.Win.UltraWinListView.CheckBoxStyle.CheckBox;
            this.lvMemberProperties.ViewSettingsList.ImageSize = new System.Drawing.Size(0, 0);
            this.lvMemberProperties.ViewSettingsList.MultiColumn = false;
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.None;
            this.ultraGroupBox1.Controls.Add(this.btOK);
            this.ultraGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(211, 218);
            this.ultraGroupBox1.TabIndex = 1;
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.Location = new System.Drawing.Point(132, 188);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 0;
            this.btOK.Text = "OK";
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // MemberPropertiesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvMemberProperties);
            this.Controls.Add(this.ultraGroupBox1);
            this.Name = "MemberPropertiesControl";
            this.Size = new System.Drawing.Size(211, 218);
            ((System.ComponentModel.ISupportInitialize)(this.lvMemberProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinListView.UltraListView lvMemberProperties;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.Misc.UltraButton btOK;
    }
}
