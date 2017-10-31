namespace Krista.FM.Client.MDXExpert.Controls
{
    partial class RangeColorCtrl
    {
        /// <summary> 
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.teRangeText = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.cpColor = new Infragistics.Win.UltraWinEditors.UltraColorPicker();
            ((System.ComponentModel.ISupportInitialize)(this.teRangeText)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cpColor)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(12, 11);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(33, 14);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "Цвет:";
            // 
            // teRangeText
            // 
            this.teRangeText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.teRangeText.Location = new System.Drawing.Point(133, 8);
            this.teRangeText.Name = "teRangeText";
            this.teRangeText.Size = new System.Drawing.Size(115, 21);
            this.teRangeText.TabIndex = 2;
            this.teRangeText.ValueChanged += new System.EventHandler(this.teRangeText_ValueChanged);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(90, 12);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(37, 14);
            this.ultraLabel2.TabIndex = 3;
            this.ultraLabel2.Text = "Текст:";
            // 
            // cpColor
            // 
            this.cpColor.Color = System.Drawing.Color.Empty;
            this.cpColor.Location = new System.Drawing.Point(44, 8);
            this.cpColor.Name = "cpColor";
            this.cpColor.Size = new System.Drawing.Size(42, 21);
            this.cpColor.TabIndex = 4;
            this.cpColor.ColorChanged += new System.EventHandler(this.cpColor_ColorChanged);
            // 
            // RangeColorCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cpColor);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.teRangeText);
            this.Controls.Add(this.ultraLabel1);
            this.Name = "RangeColorCtrl";
            this.Size = new System.Drawing.Size(271, 34);
            ((System.ComponentModel.ISupportInitialize)(this.teRangeText)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cpColor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor teRangeText;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraColorPicker cpColor;
    }
}
