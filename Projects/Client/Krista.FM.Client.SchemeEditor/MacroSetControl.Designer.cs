namespace Krista.FM.Client.SchemeEditor
{
    partial class MacroSetControl
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
            this.highlightEdit = new Krista.FM.Client.SchemeEditor.HighlightEdit();
            this.SuspendLayout();
            // 
            // highlightEdit
            // 
            this.highlightEdit.AcceptsTab = true;
            this.highlightEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.highlightEdit.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.highlightEdit.Location = new System.Drawing.Point(0, 0);
            this.highlightEdit.Name = "highlightEdit";
            this.highlightEdit.Size = new System.Drawing.Size(150, 150);
            this.highlightEdit.TabIndex = 0;
            this.highlightEdit.Text = "";
            this.highlightEdit.Leave += new System.EventHandler(this.highlightEdit_Leave);
            this.highlightEdit.TextChanged += new System.EventHandler(this.highlightEdit_TextChanged);
            this.highlightEdit.MouseDown += new System.Windows.Forms.MouseEventHandler(this.highlightEdit_MouseDown);
            // 
            // MacroSetControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.highlightEdit);
            this.Name = "MacroSetControl";
            this.ResumeLayout(false);

        }

        #endregion

        private HighlightEdit highlightEdit;
    }
}
