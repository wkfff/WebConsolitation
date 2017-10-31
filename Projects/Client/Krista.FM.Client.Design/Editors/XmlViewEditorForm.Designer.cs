namespace Krista.FM.Client.Design.Editors
{
    partial class XmlViewEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XmlViewEditorForm));
            this.panel2 = new System.Windows.Forms.Panel();
            this.xmlBrowser = new System.Windows.Forms.WebBrowser();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.xmlBrowser);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // xmlBrowser
            // 
            resources.ApplyResources(this.xmlBrowser, "xmlBrowser");
            this.xmlBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.xmlBrowser.Name = "xmlBrowser";
            this.xmlBrowser.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.xmlBrowser_PreviewKeyDown);
            // 
            // XmlViewEditorForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.MinimizeBox = false;
            this.Name = "XmlViewEditorForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.XmlViewEditorForm_FormClosed);
            this.Load += new System.EventHandler(this.XmlViewEditorForm_Load);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.WebBrowser xmlBrowser;
    }
}