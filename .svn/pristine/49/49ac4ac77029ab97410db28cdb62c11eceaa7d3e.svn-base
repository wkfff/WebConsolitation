using Infragistics.Win.Misc;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    partial class FormTaskParamsResult
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("User");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("ID");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Name");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("User");
            this.ugResults = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.udsLockTasks = new Infragistics.Win.UltraWinDataSource.UltraDataSource(/*this.components*/);
            this.lResults = new UltraLabel();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ugResults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udsLockTasks)).BeginInit();
            this.SuspendLayout();
            // 
            // ugResults
            // 
            this.ugResults.DataSource = this.udsLockTasks;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 53;
            ultraGridColumn2.Header.Caption = "Наименование";
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 193;
            ultraGridColumn3.Header.Caption = "Пользователь";
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 193;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3});
            this.ugResults.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.ugResults.DisplayLayout.BorderStyleCaption = Infragistics.Win.UIElementBorderStyle.None;
            this.ugResults.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.ugResults.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.ugResults.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.ugResults.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.ugResults.Location = new System.Drawing.Point(12, 72);
            this.ugResults.Name = "ugResults";
            this.ugResults.Size = new System.Drawing.Size(477, 113);
            this.ugResults.TabIndex = 0;
            this.ugResults.Text = "ultraGrid1";
            this.ugResults.Visible = false;
            this.ugResults.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ugResults_InitializeLayout);
            // 
            // udsLockTasks
            // 
            this.udsLockTasks.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3});
            this.udsLockTasks.ReadOnly = true;
            // 
            // lResults
            // 
            this.lResults.Location = new System.Drawing.Point(12, 9);
            this.lResults.Name = "lResults";
            this.lResults.Size = new System.Drawing.Size(477, 60);
            this.lResults.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button2.Location = new System.Drawing.Point(414, 191);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "ОК";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // FormTaskParamsResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 225);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.lResults);
            this.Controls.Add(this.ugResults);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormTaskParamsResult";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Результаты выполнения";
            ((System.ComponentModel.ISupportInitialize)(this.ugResults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udsLockTasks)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal UltraLabel lResults;
        internal Infragistics.Win.UltraWinGrid.UltraGrid ugResults;
        internal Infragistics.Win.UltraWinDataSource.UltraDataSource udsLockTasks;
        internal System.Windows.Forms.Button button2;
    }
}