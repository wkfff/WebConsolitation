namespace Krista.FM.Client.MDXExpert.Controls
{
    partial class RangeValueCtrl
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.SpinEditorButton spinEditorButton1 = new Infragistics.Win.UltraWinEditors.SpinEditorButton();
            this.lRangeNumber = new Infragistics.Win.Misc.UltraLabel();
            this.teRangeValue = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            ((System.ComponentModel.ISupportInitialize)(this.teRangeValue)).BeginInit();
            this.SuspendLayout();
            // 
            // lRangeNumber
            // 
            appearance1.TextHAlignAsString = "Right";
            this.lRangeNumber.Appearance = appearance1;
            this.lRangeNumber.Location = new System.Drawing.Point(3, 12);
            this.lRangeNumber.Name = "lRangeNumber";
            this.lRangeNumber.Size = new System.Drawing.Size(23, 16);
            this.lRangeNumber.TabIndex = 0;
            this.lRangeNumber.Text = "0";
            this.lRangeNumber.WrapText = false;
            // 
            // teRangeValue
            // 
            appearance2.TextHAlignAsString = "Right";
            this.teRangeValue.Appearance = appearance2;
            this.teRangeValue.ButtonsRight.Add(spinEditorButton1);
            this.teRangeValue.Location = new System.Drawing.Point(32, 8);
            this.teRangeValue.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            this.teRangeValue.MaskInput = "{double:-15.2:c}";
            this.teRangeValue.Name = "teRangeValue";
            this.teRangeValue.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double;
            this.teRangeValue.PromptChar = ' ';
            this.teRangeValue.Size = new System.Drawing.Size(218, 21);
            this.teRangeValue.TabIndex = 1;
            this.teRangeValue.ValueChanged += new System.EventHandler(this.teRangeValue_ValueChanged);
            this.teRangeValue.EditorSpinButtonClick += new Infragistics.Win.UltraWinEditors.SpinButtonClickEventHandler(this.teRangeValue_EditorSpinButtonClick);
            this.teRangeValue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.teRangeValue_KeyDown);
            this.teRangeValue.Leave += new System.EventHandler(this.teRangeValue_Leave);
            // 
            // RangeValueCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.teRangeValue);
            this.Controls.Add(this.lRangeNumber);
            this.Name = "RangeValueCtrl";
            this.Size = new System.Drawing.Size(273, 38);
            ((System.ComponentModel.ISupportInitialize)(this.teRangeValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel lRangeNumber;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor teRangeValue;
    }
}
