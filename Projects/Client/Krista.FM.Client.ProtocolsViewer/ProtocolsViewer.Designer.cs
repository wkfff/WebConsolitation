namespace Krista.FM.Client.ProtocolsViewer
{
    partial class ProtocolsViewForm
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
            this.protocolsObject = new Krista.FM.Client.Common.ProtocolsObject();
            this.statusStripPV = new System.Windows.Forms.StatusStrip();
            this.statuslbFileName = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStripPV.SuspendLayout();
            this.SuspendLayout();
            // 
            // protocolsObject
            // 
            this.protocolsObject.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.protocolsObject.Location = new System.Drawing.Point(0, 0);
            this.protocolsObject.Name = "protocolsObject";
            this.protocolsObject.Size = new System.Drawing.Size(783, 477);
            this.protocolsObject.TabIndex = 0;
            // 
            // statusStripPV
            // 
            this.statusStripPV.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statuslbFileName});
            this.statusStripPV.Location = new System.Drawing.Point(0, 476);
            this.statusStripPV.Name = "statusStripPV";
            this.statusStripPV.Size = new System.Drawing.Size(783, 22);
            this.statusStripPV.SizingGrip = false;
            this.statusStripPV.TabIndex = 2;
            this.statusStripPV.Text = "statusStrip1";
            // 
            // statuslbFileName
            // 
            this.statuslbFileName.Name = "statuslbFileName";
            this.statuslbFileName.Size = new System.Drawing.Size(0, 17);
            this.statuslbFileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ProtocolsViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(783, 498);
            this.Controls.Add(this.statusStripPV);
            this.Controls.Add(this.protocolsObject);
            this.Name = "ProtocolsViewForm";
            this.Text = "Просмотр протоколов";
            this.statusStripPV.ResumeLayout(false);
            this.statusStripPV.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Krista.FM.Client.Common.ProtocolsObject protocolsObject;
        private System.Windows.Forms.StatusStrip statusStripPV;
        private System.Windows.Forms.ToolStripStatusLabel statuslbFileName;
    }
}

