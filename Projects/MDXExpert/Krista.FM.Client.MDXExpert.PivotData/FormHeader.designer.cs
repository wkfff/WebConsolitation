namespace Krista.FM.Client.MDXExpert.Data
{
    partial class FormHeader
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
            this.SuspendLayout();
            // 
            // FormHeader
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Grip;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Name = "FormHeader";
            this.Size = new System.Drawing.Size(239, 16);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormHeader_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormHeader_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormHeader_MouseUp);
            this.SizeChanged += new System.EventHandler(this.Gripper_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
