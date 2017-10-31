namespace Krista.FM.Client.SchemeEditor
{
    partial class FixedRowsGridForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FixedRowsGridForm));
            this.fixedRowsGridControl = new Krista.FM.Client.SchemeEditor.FixedRowsGridControl();
            this.SuspendLayout();
            // 
            // fixedRowsGridControl
            // 
            this.fixedRowsGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fixedRowsGridControl.Location = new System.Drawing.Point(0, 0);
            this.fixedRowsGridControl.Name = "fixedRowsGridControl";
            this.fixedRowsGridControl.Size = new System.Drawing.Size(498, 272);
            this.fixedRowsGridControl.TabIndex = 0;
            // 
            // FixedRowsGridForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 272);
            this.Controls.Add(this.fixedRowsGridControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "FixedRowsGridForm";
            this.ShowInTaskbar = false;
            this.Text = "Фиксированные значения";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FixedRowsGridForm_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private FixedRowsGridControl fixedRowsGridControl;

    }
}