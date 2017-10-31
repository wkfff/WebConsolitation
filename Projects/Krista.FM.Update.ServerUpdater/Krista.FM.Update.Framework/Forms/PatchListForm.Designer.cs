using Krista.FM.Update.Framework.Controls;

namespace Krista.FM.Update.Framework.Forms
{
    partial class PatchListForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PatchListForm));
            
            this.SuspendLayout();
            // 
            // patchListControl
            // 
            this.patchListControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.patchListControl.Location = new System.Drawing.Point(0, 0);
            this.patchListControl.Name = "patchListControl";
            this.patchListControl.Patches = ((System.Collections.Generic.IList<Krista.FM.Update.Framework.IUpdatePatch>)(resources.GetObject("patchListControl.Patches")));
            this.patchListControl.ReadOnlyMode = false;
            this.patchListControl.Size = new System.Drawing.Size(545, 432);
            this.patchListControl.TabIndex = 0;
            // 
            // PatchListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.patchListControl);
            this.MinimumSize = new System.Drawing.Size(362, 459);
            this.Name = "PatchListForm";
            this.Text = "Список установленных обновлений";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.PatchListControl patchListControl;

        public PatchListControl PatchListControl
        {
            get { return patchListControl; }
            set { patchListControl = value; }
        }
    }
}