namespace Krista.FM.Client.MDXExpert.Controls
{
    partial class ChartLegendEditor
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
            Infragistics.Win.ValueListItem valueListItem5 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem6 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem7 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem8 = new Infragistics.Win.ValueListItem();
            this.cbVisible = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ulLocation = new Infragistics.Win.Misc.UltraLabel();
            this.ucLocation = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.lbSpan = new Infragistics.Win.Misc.UltraLabel();
            this.tbSpan = new System.Windows.Forms.TrackBar();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ucLocation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSpan)).BeginInit();
            this.SuspendLayout();
            // 
            // cbVisible
            // 
            this.cbVisible.Location = new System.Drawing.Point(4, 0);
            this.cbVisible.Name = "cbVisible";
            this.cbVisible.Size = new System.Drawing.Size(98, 20);
            this.cbVisible.TabIndex = 11;
            this.cbVisible.Text = "Показывать";
            // 
            // ulLocation
            // 
            this.ulLocation.AutoSize = true;
            this.ulLocation.Location = new System.Drawing.Point(4, 25);
            this.ulLocation.Name = "ulLocation";
            this.ulLocation.Size = new System.Drawing.Size(82, 14);
            this.ulLocation.TabIndex = 13;
            this.ulLocation.Text = "Расположение";
            // 
            // ucLocation
            // 
            this.ucLocation.DisplayMember = "";
            this.ucLocation.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem5.DataValue = ((byte)(0));
            valueListItem5.DisplayText = "Сверху";
            valueListItem6.DataValue = ((byte)(1));
            valueListItem6.DisplayText = "Слева";
            valueListItem7.DataValue = ((byte)(2));
            valueListItem7.DisplayText = "Справа";
            valueListItem8.DataValue = ((byte)(3));
            valueListItem8.DisplayText = "Снизу";
            this.ucLocation.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem5,
            valueListItem6,
            valueListItem7,
            valueListItem8});
            this.ucLocation.Location = new System.Drawing.Point(102, 21);
            this.ucLocation.Name = "ucLocation";
            this.ucLocation.Size = new System.Drawing.Size(117, 21);
            this.ucLocation.TabIndex = 12;
            // 
            // lbSpan
            // 
            this.lbSpan.AutoSize = true;
            this.lbSpan.Location = new System.Drawing.Point(4, 47);
            this.lbSpan.Name = "lbSpan";
            this.lbSpan.Size = new System.Drawing.Size(44, 14);
            this.lbSpan.TabIndex = 10;
            this.lbSpan.Text = "Размер";
            // 
            // tbSpan
            // 
            this.tbSpan.AutoSize = false;
            this.tbSpan.LargeChange = 10;
            this.tbSpan.Location = new System.Drawing.Point(96, 45);
            this.tbSpan.Maximum = 100;
            this.tbSpan.Name = "tbSpan";
            this.tbSpan.Size = new System.Drawing.Size(129, 20);
            this.tbSpan.TabIndex = 9;
            this.tbSpan.TickFrequency = 10;
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 2000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.ReshowDelay = 100;
            // 
            // ChartLegendEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ulLocation);
            this.Controls.Add(this.ucLocation);
            this.Controls.Add(this.cbVisible);
            this.Controls.Add(this.lbSpan);
            this.Controls.Add(this.tbSpan);
            this.MaximumSize = new System.Drawing.Size(0, 214);
            this.MinimumSize = new System.Drawing.Size(213, 0);
            this.Name = "ChartLegendEditor";
            this.Size = new System.Drawing.Size(226, 71);
            ((System.ComponentModel.ISupportInitialize)(this.ucLocation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSpan)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbVisible;
        private Infragistics.Win.Misc.UltraLabel ulLocation;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor ucLocation;
        private Infragistics.Win.Misc.UltraLabel lbSpan;
        private System.Windows.Forms.TrackBar tbSpan;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
