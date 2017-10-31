namespace Krista.FM.Client.ViewObjects.ReportsUI.Gui
{
    partial class ReportTree
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
            this.tReports = new Infragistics.Win.UltraWinTree.UltraTree();
            ((System.ComponentModel.ISupportInitialize)(this.tReports)).BeginInit();
            this.SuspendLayout();
            // 
            // tReports
            // 
            this.tReports.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.tReports.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tReports.Location = new System.Drawing.Point(0, 0);
            this.tReports.Name = "tReports";
            this.tReports.Size = new System.Drawing.Size(599, 519);
            this.tReports.TabIndex = 1;
            // 
            // ReportTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tReports);
            this.Name = "ReportTree";
            this.Size = new System.Drawing.Size(599, 519);
            ((System.ComponentModel.ISupportInitialize)(this.tReports)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal Infragistics.Win.UltraWinTree.UltraTree tReports;
    }
}
