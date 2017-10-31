using System.Windows.Forms;
namespace Krista.FM.Client.iMonitoringWM.Controls
{
    partial class SearchBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchBox));
            this.gradient = new Resco.Controls.OutlookControls.ImageButton();
            this.textBox = new Krista.FM.Client.iMonitoringWM.Controls.WMTextBox();
            this.findLeftPart = new Resco.Controls.OutlookControls.ImageButton();
            this.findRightPart = new Resco.Controls.OutlookControls.ImageButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.gradient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.findLeftPart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.findRightPart)).BeginInit();
            this.SuspendLayout();
            // 
            // gradient
            // 
            this.gradient.AutoTransparent = false;
            this.gradient.BackColor = System.Drawing.Color.Black;
            this.gradient.BorderColor = System.Drawing.Color.Black;
            this.gradient.ButtonStyle = Resco.Controls.OutlookControls.ImageButton.ButtonType.PictureBox;
            this.gradient.Checked = false;
            this.gradient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gradient.FocusedColor = System.Drawing.Color.Black;
            this.gradient.GradientColors = new Resco.Controls.OutlookControls.GradientColor(System.Drawing.Color.LightGray, System.Drawing.Color.Black, Resco.Controls.OutlookControls.FillDirection.Vertical);
            this.gradient.GradientColorsPressed = new Resco.Controls.OutlookControls.GradientColor(System.Drawing.Color.Black, System.Drawing.Color.LightGray, Resco.Controls.OutlookControls.FillDirection.Vertical);
            this.gradient.ImageDefault = ((System.Drawing.Image)(resources.GetObject("gradient.ImageDefault")));
            this.gradient.ImageLocation = new System.Drawing.Point(-1, -1);
            this.gradient.ImageVgaDefault = ((System.Drawing.Image)(resources.GetObject("gradient.ImageVgaDefault")));
            this.gradient.Location = new System.Drawing.Point(0, 0);
            this.gradient.MaxStretchImageSize = new System.Drawing.Size(-1, -1);
            this.gradient.Name = "gradient";
            this.gradient.PressedBackColor = System.Drawing.Color.PowderBlue;
            this.gradient.PressedBorderColor = System.Drawing.Color.SteelBlue;
            this.gradient.Size = new System.Drawing.Size(219, 30);
            this.gradient.TabIndex = 0;
            this.gradient.TextLocation = new System.Drawing.Point(-1, -1);
            this.gradient.VistaButtonInflate = new System.Drawing.Size(0, 0);
            // 
            // textBox
            // 
            this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular);
            this.textBox.Location = new System.Drawing.Point(30, 7);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(159, 20);
            this.textBox.TabIndex = 1;
            // 
            // findLeftPart
            // 
            this.findLeftPart.AutoTransparent = false;
            this.findLeftPart.BackColor = System.Drawing.Color.Black;
            this.findLeftPart.BorderColor = System.Drawing.Color.Black;
            this.findLeftPart.ButtonStyle = Resco.Controls.OutlookControls.ImageButton.ButtonType.ImageButton;
            this.findLeftPart.Checked = false;
            this.findLeftPart.FocusedColor = System.Drawing.Color.Black;
            this.findLeftPart.GradientColors = new Resco.Controls.OutlookControls.GradientColor(System.Drawing.Color.LightGray, System.Drawing.Color.Black, Resco.Controls.OutlookControls.FillDirection.Vertical);
            this.findLeftPart.GradientColorsPressed = new Resco.Controls.OutlookControls.GradientColor(System.Drawing.Color.Black, System.Drawing.Color.LightGray, Resco.Controls.OutlookControls.FillDirection.Vertical);
            this.findLeftPart.ImageDefault = ((System.Drawing.Image)(resources.GetObject("findLeftPart.ImageDefault")));
            this.findLeftPart.ImageLocation = new System.Drawing.Point(-1, -1);
            this.findLeftPart.ImageVgaDefault = ((System.Drawing.Image)(resources.GetObject("findLeftPart.ImageVgaDefault")));
            this.findLeftPart.Location = new System.Drawing.Point(0, 0);
            this.findLeftPart.MaxStretchImageSize = new System.Drawing.Size(-1, -1);
            this.findLeftPart.Name = "findLeftPart";
            this.findLeftPart.PressedBackColor = System.Drawing.Color.PowderBlue;
            this.findLeftPart.PressedBorderColor = System.Drawing.Color.Black;
            this.findLeftPart.Size = new System.Drawing.Size(30, 30);
            this.findLeftPart.TabIndex = 2;
            this.findLeftPart.TextLocation = new System.Drawing.Point(-1, -1);
            this.findLeftPart.VistaButtonInflate = new System.Drawing.Size(-4, -4);
            this.findLeftPart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.findLeftPart_MouseDown);
            // 
            // findRightPart
            // 
            this.findRightPart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findRightPart.AutoTransparent = false;
            this.findRightPart.BackColor = System.Drawing.Color.Black;
            this.findRightPart.BorderColor = System.Drawing.Color.Black;
            this.findRightPart.ButtonStyle = Resco.Controls.OutlookControls.ImageButton.ButtonType.ImageButton;
            this.findRightPart.Checked = false;
            this.findRightPart.FocusedColor = System.Drawing.Color.Black;
            this.findRightPart.GradientColors = new Resco.Controls.OutlookControls.GradientColor(System.Drawing.Color.LightGray, System.Drawing.Color.Black, Resco.Controls.OutlookControls.FillDirection.Vertical);
            this.findRightPart.GradientColorsPressed = new Resco.Controls.OutlookControls.GradientColor(System.Drawing.Color.Black, System.Drawing.Color.LightGray, Resco.Controls.OutlookControls.FillDirection.Vertical);
            this.findRightPart.ImageDefault = ((System.Drawing.Image)(resources.GetObject("findRightPart.ImageDefault")));
            this.findRightPart.ImageLocation = new System.Drawing.Point(-1, -1);
            this.findRightPart.ImageVgaDefault = ((System.Drawing.Image)(resources.GetObject("findRightPart.ImageVgaDefault")));
            this.findRightPart.Location = new System.Drawing.Point(189, 0);
            this.findRightPart.MaxStretchImageSize = new System.Drawing.Size(-1, -1);
            this.findRightPart.Name = "findRightPart";
            this.findRightPart.PressedBackColor = System.Drawing.Color.PowderBlue;
            this.findRightPart.PressedBorderColor = System.Drawing.Color.Black;
            this.findRightPart.Size = new System.Drawing.Size(30, 30);
            this.findRightPart.TabIndex = 3;
            this.findRightPart.TextLocation = new System.Drawing.Point(-1, -1);
            this.findRightPart.VistaButtonInflate = new System.Drawing.Size(-4, -4);
            this.findRightPart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.findRightPart_MouseDown);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular);
            this.textBox1.Location = new System.Drawing.Point(30, 4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(159, 23);
            this.textBox1.TabIndex = 4;
            // 
            // SearchBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.findRightPart);
            this.Controls.Add(this.findLeftPart);
            this.Controls.Add(this.gradient);
            this.Name = "SearchBox";
            this.Size = new System.Drawing.Size(219, 30);
            ((System.ComponentModel.ISupportInitialize)(this.gradient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.findLeftPart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.findRightPart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Resco.Controls.OutlookControls.ImageButton gradient;
        //private TextBox textBox;
        private Resco.Controls.OutlookControls.ImageButton findLeftPart;
        private Resco.Controls.OutlookControls.ImageButton findRightPart;
        private TextBox textBox1;
        private Krista.FM.Client.iMonitoringWM.Controls.WMTextBox textBox;
    }
}
