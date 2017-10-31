namespace Krista.FM.Client.SchemeEditor.Services.SearchService
{
    partial class SearchTabControl
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
            this.components = new System.ComponentModel.Container();
            this.ultraTabControl = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolMenuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMenuCloseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl)).BeginInit();
            this.ultraTabControl.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraTabControl
            // 
            this.ultraTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraTabControl.ContextMenuStrip = this.contextMenuStrip;
            this.ultraTabControl.Controls.Add(this.ultraTabSharedControlsPage1);
            this.ultraTabControl.FlatMode = true;//Infragistics.Win.DefaultableBoolean.True;
            this.ultraTabControl.Location = new System.Drawing.Point(3, 3);
            this.ultraTabControl.Name = "ultraTabControl";
            this.ultraTabControl.NavigationStyle = Infragistics.Win.UltraWinTabControl.NavigationStyle.Activate;
            this.ultraTabControl.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.ultraTabControl.Size = new System.Drawing.Size(409, 318);
            this.ultraTabControl.TabIndex = 0;
            this.ultraTabControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ultraTabControl_MouseDown);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolMenuClose,
            this.toolMenuCloseAll});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.ShowImageMargin = false;
            this.contextMenuStrip.Size = new System.Drawing.Size(128, 70);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // toolMenuClose
            // 
            this.toolMenuClose.Name = "toolMenuClose";
            this.toolMenuClose.Size = new System.Drawing.Size(127, 22);
            this.toolMenuClose.Text = "Закрыть";
            this.toolMenuClose.Click += new System.EventHandler(this.toolMenuClose_Click);
            // 
            // toolMenuCloseAll
            // 
            this.toolMenuCloseAll.Name = "toolMenuCloseAll";
            this.toolMenuCloseAll.Size = new System.Drawing.Size(127, 22);
            this.toolMenuCloseAll.Text = "Закрыть все";
            this.toolMenuCloseAll.Click += new System.EventHandler(this.toolMenuCloseAll_Click);
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(2, 21);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(405, 295);
            // 
            // SearchTabControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraTabControl);
            this.Name = "SearchTabControl";
            this.Size = new System.Drawing.Size(415, 323);
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl)).EndInit();
            this.ultraTabControl.ResumeLayout(false);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinTabControl.UltraTabControl ultraTabControl;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolMenuClose;
        private System.Windows.Forms.ToolStripMenuItem toolMenuCloseAll;
    }
}
