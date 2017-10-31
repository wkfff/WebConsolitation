namespace Krista.FM.Update.NotifyIconApp
{
    partial class NotifyIconForm
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

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.notifyIconControl = new Krista.FM.Update.Framework.Controls.NotifyIconControl();
            this.SuspendLayout();
            // 
            // notifyIconControl1
            // 
            this.notifyIconControl.Location = new System.Drawing.Point(24, 11);
            this.notifyIconControl.Name = "notifyIconControl";
            this.notifyIconControl.Size = new System.Drawing.Size(400, 150);
            this.notifyIconControl.TabIndex = 0;
            // 
            // NotifyIconForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 173);
            this.Controls.Add(this.notifyIconControl);
            this.Name = "NotifyIconForm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }
        
        #endregion

        private Framework.Controls.NotifyIconControl notifyIconControl;
    }
}

