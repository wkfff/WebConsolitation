namespace Krista.FM.Client.MDXExpert
{
    partial class RelationalTable
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.gbMain = new Infragistics.Win.Misc.UltraGroupBox();
            this.grid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.gbTop = new Infragistics.Win.Misc.UltraGroupBox();
            this.formHeader1 = new Krista.FM.Client.MDXExpert.Data.FormHeader();
            this.gbBottom = new Infragistics.Win.Misc.UltraGroupBox();
            this.gripper1 = new Krista.FM.Client.MDXExpert.Data.Gripper();
            ((System.ComponentModel.ISupportInitialize)(this.gbMain)).BeginInit();
            this.gbMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbTop)).BeginInit();
            this.gbTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbBottom)).BeginInit();
            this.gbBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbMain
            // 
            this.gbMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbMain.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.Parallel3D;
            this.gbMain.Controls.Add(this.grid);
            this.gbMain.Location = new System.Drawing.Point(0, 15);
            this.gbMain.Name = "gbMain";
            this.gbMain.Size = new System.Drawing.Size(467, 405);
            this.gbMain.TabIndex = 0;
            this.gbMain.UseAppStyling = false;
            this.gbMain.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2000;
            // 
            // grid
            // 
            this.grid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.Yes;
            this.grid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
            this.grid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.grid.DisplayLayout.Override.MergedCellStyle = Infragistics.Win.UltraWinGrid.MergedCellStyle.Always;
            this.grid.DisplayLayout.Override.RowFilterAction = Infragistics.Win.UltraWinGrid.RowFilterAction.AppearancesOnly;
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Location = new System.Drawing.Point(1, 0);
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(465, 403);
            this.grid.TabIndex = 2;
            this.grid.UseFlatMode = Infragistics.Win.DefaultableBoolean.False;
            // 
            // gbTop
            // 
            this.gbTop.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.None;
            this.gbTop.Controls.Add(this.formHeader1);
            this.gbTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbTop.Location = new System.Drawing.Point(0, 0);
            this.gbTop.Name = "gbTop";
            this.gbTop.Size = new System.Drawing.Size(465, 15);
            this.gbTop.TabIndex = 1;
            this.gbTop.UseAppStyling = false;
            this.gbTop.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2000;
            // 
            // formHeader1
            // 
            this.formHeader1.AccessibleRole = System.Windows.Forms.AccessibleRole.Grip;
            this.formHeader1.BackColor = System.Drawing.Color.White;
            this.formHeader1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formHeader1.Location = new System.Drawing.Point(1, 0);
            this.formHeader1.Name = "formHeader1";
            this.formHeader1.Size = new System.Drawing.Size(463, 14);
            this.formHeader1.TabIndex = 0;
            // 
            // gbBottom
            // 
            appearance1.BackColor = System.Drawing.Color.White;
            this.gbBottom.Appearance = appearance1;
            this.gbBottom.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.None;
            this.gbBottom.Controls.Add(this.gripper1);
            this.gbBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gbBottom.Location = new System.Drawing.Point(0, 421);
            this.gbBottom.Name = "gbBottom";
            this.gbBottom.Size = new System.Drawing.Size(465, 22);
            this.gbBottom.TabIndex = 2;
            this.gbBottom.UseAppStyling = false;
            this.gbBottom.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2000;
            // 
            // gripper1
            // 
            this.gripper1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.gripper1.BackColor = System.Drawing.Color.White;
            this.gripper1.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.gripper1.Location = new System.Drawing.Point(448, 5);
            this.gripper1.Name = "gripper1";
            this.gripper1.Size = new System.Drawing.Size(19, 17);
            this.gripper1.TabIndex = 0;
            // 
            // RelationalTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.gbBottom);
            this.Controls.Add(this.gbTop);
            this.Controls.Add(this.gbMain);
            this.Name = "RelationalTable";
            this.Size = new System.Drawing.Size(465, 443);
            ((System.ComponentModel.ISupportInitialize)(this.gbMain)).EndInit();
            this.gbMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbTop)).EndInit();
            this.gbTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gbBottom)).EndInit();
            this.gbBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox gbMain;
        private Infragistics.Win.Misc.UltraGroupBox gbTop;
        private Krista.FM.Client.MDXExpert.Data.FormHeader formHeader1;
        private Infragistics.Win.Misc.UltraGroupBox gbBottom;
        private Krista.FM.Client.MDXExpert.Data.Gripper gripper1;
        private Infragistics.Win.UltraWinGrid.UltraGrid grid;
    }
}
