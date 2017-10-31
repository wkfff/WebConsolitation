namespace Krista.FM.Client.SchemeEditor.Gui.ViewControls
{
    partial class ValidationErrorListControl
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
            this.grid = new Krista.FM.Client.Components.UltraGridEx();
            this.SuspendLayout();
            // 
            // grid
            // 
            this.grid.AllowAddNewRecords = true;
            this.grid.AllowClearTable = true;
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.InDebugMode = false;
            this.grid.LoadMenuVisible = false;
            this.grid.Location = new System.Drawing.Point(0, 0);
            this.grid.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.grid.MinCalendarDate = new System.DateTime(((long)(0)));
            this.grid.Name = "grid";
            this.grid.SaveLoadFileName = "";
            this.grid.SaveMenuVisible = false;
            this.grid.ServerFilterEnabled = false;
            this.grid.SingleBandLevelName = "Добавить запись...";
            this.grid.Size = new System.Drawing.Size(459, 324);
            this.grid.sortColumnName = "";
            this.grid.StateRowEnable = false;
            this.grid.TabIndex = 0;
            // 
            // ValidationErrorListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grid);
            this.Name = "ValidationErrorListControl";
            this.Size = new System.Drawing.Size(459, 324);
            this.ResumeLayout(false);

        }

        #endregion

        private Krista.FM.Client.Components.UltraGridEx grid;
    }
}
