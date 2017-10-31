namespace Krista.FM.Client.ViewObjects.DataSourcesUI
{
    partial class FrmSourceDeleteError
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.ultraGridEx = new Krista.FM.Client.Components.UltraGridEx();
            this.btnOk = new System.Windows.Forms.Button();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.ultraGridEx);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.btnOk);
            this.splitContainer.Size = new System.Drawing.Size(632, 453);
            this.splitContainer.SplitterDistance = 394;
            this.splitContainer.TabIndex = 0;
            // 
            // ultraGridEx
            // 
            this.ultraGridEx.AllowAddNewRecords = true;
            this.ultraGridEx.AllowClearTable = true;
            this.ultraGridEx.AllowDeleteRows = false;
            this.ultraGridEx.AllowEditRows = false;
            this.ultraGridEx.AllowImportFromXML = false;
            this.ultraGridEx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGridEx.InDebugMode = false;
            this.ultraGridEx.IsReadOnly = true;
            this.ultraGridEx.LoadMenuVisible = false;
            this.ultraGridEx.Location = new System.Drawing.Point(0, 0);
            this.ultraGridEx.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ultraGridEx.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ultraGridEx.Name = "ultraGridEx";
            this.ultraGridEx.SaveLoadFileName = "";
            this.ultraGridEx.SaveMenuVisible = false;
            this.ultraGridEx.ServerFilterEnabled = false;
            this.ultraGridEx.SingleBandLevelName = "Добавить запись...";
            this.ultraGridEx.Size = new System.Drawing.Size(632, 394);
            this.ultraGridEx.sortColumnName = "";
            this.ultraGridEx.StateRowEnable = false;
            this.ultraGridEx.TabIndex = 0;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(545, 16);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // FrmSourceDeleteError
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 453);
            this.Controls.Add(this.splitContainer);
            this.MinimizeBox = false;
            this.Name = "FrmSourceDeleteError";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Невозможно удалить следующие данные";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Button btnOk;
        private Krista.FM.Client.Components.UltraGridEx ultraGridEx;
    }
}