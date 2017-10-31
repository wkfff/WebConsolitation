namespace Krista.FM.Client.SchemeEditor
{
    partial class EntityGridForm
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
            this.entityGridControl = new Krista.FM.Client.SchemeEditor.EntityGridControl();
            this.SuspendLayout();
            // 
            // entityGridControl1
            // 
            this.entityGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityGridControl.Location = new System.Drawing.Point(0, 0);
            this.entityGridControl.Name = "entityGridControl1";
            this.entityGridControl.Size = new System.Drawing.Size(530, 273);
            this.entityGridControl.TabIndex = 0;
            // 
            // EntityGridForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 273);
            this.Controls.Add(this.entityGridControl);
            this.Name = "EntityGridForm";
            this.Text = "EntityGridForm";
            this.ResumeLayout(false);

        }

        #endregion

        private EntityGridControl entityGridControl;
    }
}