namespace Krista.FM.Client.Design.Editors
{
    partial class SessionGridControl
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
            
           
            this.sessionGrid = new Krista.FM.Client.Components.UltraGridEx();
            this.SuspendLayout();
         
            this.sessionGrid.ColumnsToolbarVisible = false;
            this.sessionGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sessionGrid.ExportImportToolbarVisible = false;
            this.sessionGrid.Location = new System.Drawing.Point(0, 0);
            this.sessionGrid.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.sessionGrid.MinCalendarDate = new System.DateTime(((long)(0)));
            this.sessionGrid.Name = "sessionGrid";
            //this.sessionGrid.ParentColumnName = "";
            //this.sessionGrid.RefColumnName = "";
            this.sessionGrid.SaveLoadFileName = "";
            this.sessionGrid.SingleBandLevelName = "Добавить запись";
            this.sessionGrid.Size = new System.Drawing.Size(489, 328);
            this.sessionGrid.StateRowEnable = false;
            this.sessionGrid.TabIndex = 2;
            
            // 
            // SessionGridControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.sessionGrid);
            this.Name = "SessionGridControl";
            this.Size = new System.Drawing.Size(489, 328);
            this.ResumeLayout(false);
            
        }

        #endregion

        private System.Data.DataTable dataTable1;
        private System.Data.DataColumn dataColumn1;
        private System.Data.DataColumn dataColumn2;
        private System.Data.DataColumn dataColumn3;
        private System.Data.DataColumn dataColumn4;
        private System.Data.DataColumn dataColumn5;
        private Krista.FM.Client.Components.UltraGridEx sessionGrid;
    }
}

