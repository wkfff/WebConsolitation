namespace Krista.FM.Client.HelpGenerator
{
    partial class CompareDescriptionForm
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("package", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("entityOLAPName", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("schemeDescription", 2);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("fmmdallDescription", 3);
            this.ultraGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGrid
            // 
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn4.Width = 109;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4});
            this.ultraGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.ultraGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGrid.Location = new System.Drawing.Point(0, 0);
            this.ultraGrid.Name = "ultraGrid";
            this.ultraGrid.Size = new System.Drawing.Size(617, 426);
            this.ultraGrid.TabIndex = 0;
            this.ultraGrid.Text = "ultraGrid1";
            // 
            // CompareDescriptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 426);
            this.Controls.Add(this.ultraGrid);
            this.Name = "CompareDescriptionForm";
            this.Text = "Таблица сравнения описаний";
            ((System.ComponentModel.ISupportInitialize)(this.ultraGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinGrid.UltraGrid ultraGrid;

    }
}