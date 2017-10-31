namespace Krista.FM.Client.SchemeEditor
{
    partial class HashCodeRowsGridControl
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
            this.fixedGrid = new Krista.FM.Client.Components.UltraGridEx();
            this.SuspendLayout();
            // 
            // fixedGrid
            // 
            this.fixedGrid.AllowAddNewRecords = true;
            this.fixedGrid.AllowClearTable = true;
            this.fixedGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fixedGrid.ExportImportToolbarVisible = false;
            this.fixedGrid.InDebugMode = false;
            this.fixedGrid.IsReadOnly = true;
            this.fixedGrid.LoadMenuVisible = false;
            this.fixedGrid.Location = new System.Drawing.Point(0, 0);
            this.fixedGrid.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.fixedGrid.MinCalendarDate = new System.DateTime(((long)(0)));
            this.fixedGrid.Name = "fixedGrid";
            this.fixedGrid.SaveLoadFileName = "";
            this.fixedGrid.SaveMenuVisible = false;
            this.fixedGrid.ServerFilterEnabled = false;
            this.fixedGrid.SingleBandLevelName = "Добавить запись...";
            this.fixedGrid.Size = new System.Drawing.Size(644, 390);
            this.fixedGrid.sortColumnName = "";
            this.fixedGrid.StateRowEnable = false;
            this.fixedGrid.TabIndex = 0;
            this.fixedGrid.OnRefreshData += new Krista.FM.Client.Components.RefreshData(this.ultraGridEx_OnRefreshData);
            this.fixedGrid.OnCancelChanges += new Krista.FM.Client.Components.DataWorking(this.ultraGridEx_OnCancelChanges);
            this.fixedGrid.OnSaveChanges += new Krista.FM.Client.Components.SaveChanges(this.ultraGridEx_OnSaveChanges);
            // 
            // FixedRowsGridControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fixedGrid);
            this.Name = "FixedRowsGridControl";
            this.Size = new System.Drawing.Size(644, 390);
            this.ResumeLayout(false);

        }

        #endregion

        private Krista.FM.Client.Components.UltraGridEx fixedGrid;
    }
}
