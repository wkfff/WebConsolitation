namespace Krista.FM.Client.iMonitoringWM.Controls
{
    partial class ScrollView
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
            this.ribbon = new System.Windows.Forms.Panel();
            this._scrollIndicator = new Krista.FM.Client.iMonitoringWM.Controls.ScrollIndicator();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.ribbon.BackColor = System.Drawing.Color.Black;
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.Name = "ribbon";
            this.ribbon.Size = new System.Drawing.Size(200, 195);
            this.ribbon.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ribbon_MouseMove);
            this.ribbon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ribbon_MouseDown);
            this.ribbon.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ribbon_MouseUp);
            // 
            // _scrollIndicator
            // 
            this._scrollIndicator.BackColor = System.Drawing.Color.Black;
            this._scrollIndicator.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._scrollIndicator.Location = new System.Drawing.Point(0, 195);
            this._scrollIndicator.Name = "_scrollIndicator";
            this._scrollIndicator.Size = new System.Drawing.Size(200, 10);
            this._scrollIndicator.TabIndex = 0;
            this._scrollIndicator.MouseDown += new System.Windows.Forms.MouseEventHandler(this._scrollIndicator_MouseDown);
            // 
            // ScrollView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this._scrollIndicator);
            this.Controls.Add(this.ribbon);
            this.Name = "ScrollView";
            this.Size = new System.Drawing.Size(200, 205);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel ribbon;
        private Krista.FM.Client.iMonitoringWM.Controls.ScrollIndicator _scrollIndicator;
    }
}
