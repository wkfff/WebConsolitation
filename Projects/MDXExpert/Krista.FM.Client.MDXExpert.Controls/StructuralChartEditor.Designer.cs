namespace Krista.FM.Client.MDXExpert.Controls
{
    partial class StructuralChartEditor
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
            this.lbLayerMinProc = new Infragistics.Win.Misc.UltraLabel();
            this.tbLayerMinProc = new System.Windows.Forms.TrackBar();
            this.cbIsConcentricAppear = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tbLayerMinProc)).BeginInit();
            this.SuspendLayout();
            // 
            // lbLayerMinProc
            // 
            this.lbLayerMinProc.AutoSize = true;
            this.lbLayerMinProc.Location = new System.Drawing.Point(4, 50);
            this.lbLayerMinProc.Name = "lbLayerMinProc";
            this.lbLayerMinProc.Size = new System.Drawing.Size(103, 14);
            this.lbLayerMinProc.TabIndex = 10;
            this.lbLayerMinProc.Text = "Мин. процент слоя";
            // 
            // tbLayerMinProc
            // 
            this.tbLayerMinProc.AutoSize = false;
            this.tbLayerMinProc.Location = new System.Drawing.Point(0, 27);
            this.tbLayerMinProc.Maximum = 30;
            this.tbLayerMinProc.Name = "tbLayerMinProc";
            this.tbLayerMinProc.Size = new System.Drawing.Size(142, 20);
            this.tbLayerMinProc.TabIndex = 9;
            this.tbLayerMinProc.TickFrequency = 3;
            // 
            // cbIsConcentricAppear
            // 
            this.cbIsConcentricAppear.Location = new System.Drawing.Point(6, 6);
            this.cbIsConcentricAppear.Name = "cbIsConcentricAppear";
            this.cbIsConcentricAppear.Size = new System.Drawing.Size(173, 17);
            this.cbIsConcentricAppear.TabIndex = 11;
            this.cbIsConcentricAppear.Text = "Концентрический вид";
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 2000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.ReshowDelay = 100;
            // 
            //  StructuralChartEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbIsConcentricAppear);
            this.Controls.Add(this.lbLayerMinProc);
            this.Controls.Add(this.tbLayerMinProc);
            this.Name = "StructuralChartEditor";
            this.Size = new System.Drawing.Size(160, 66);
            ((System.ComponentModel.ISupportInitialize)(this.tbLayerMinProc)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel lbLayerMinProc;
        private System.Windows.Forms.TrackBar tbLayerMinProc;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbIsConcentricAppear;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
