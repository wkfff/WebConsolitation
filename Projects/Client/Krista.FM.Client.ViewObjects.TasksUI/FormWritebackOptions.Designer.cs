namespace Krista.FM.Client.ViewObjects.TasksUI
{
    partial class FormWritebackOptions
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
            Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem5 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            this.uosDataWrite = new Infragistics.Win.UltraWinEditors.UltraOptionSet();
            this.uosProcessDocuments = new Infragistics.Win.UltraWinEditors.UltraOptionSet();
            this.uceProcessMD = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraButton1 = new Infragistics.Win.Misc.UltraButton();
            this.ultraButton2 = new Infragistics.Win.Misc.UltraButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.uosDataWrite)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uosProcessDocuments)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uceProcessMD)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // uosDataWrite
            // 
            this.uosDataWrite.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.uosDataWrite.CheckedIndex = 0;
            this.uosDataWrite.Dock = System.Windows.Forms.DockStyle.Fill;
            valueListItem4.DataValue = false;
            valueListItem4.DisplayText = "Дописать данные";
            valueListItem5.DataValue = true;
            valueListItem5.DisplayText = "Переписать данные";
            this.uosDataWrite.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem4,
            valueListItem5});
            this.uosDataWrite.ItemSpacingVertical = 10;
            this.uosDataWrite.Location = new System.Drawing.Point(3, 16);
            this.uosDataWrite.Name = "uosDataWrite";
            this.uosDataWrite.Size = new System.Drawing.Size(264, 52);
            this.uosDataWrite.TabIndex = 0;
            this.uosDataWrite.Text = "Дописать данные";
            // 
            // uosProcessDocuments
            // 
            this.uosProcessDocuments.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.uosProcessDocuments.CheckedIndex = 0;
            this.uosProcessDocuments.Dock = System.Windows.Forms.DockStyle.Fill;
            valueListItem2.DataValue = true;
            valueListItem2.DisplayText = "Передать данные выбранных документов";
            valueListItem3.DataValue = false;
            valueListItem3.DisplayText = "Передать данные всех документов задачи";
            this.uosProcessDocuments.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem2,
            valueListItem3});
            this.uosProcessDocuments.ItemSpacingVertical = 10;
            this.uosProcessDocuments.Location = new System.Drawing.Point(3, 16);
            this.uosProcessDocuments.Name = "uosProcessDocuments";
            this.uosProcessDocuments.Size = new System.Drawing.Size(264, 52);
            this.uosProcessDocuments.TabIndex = 0;
            this.uosProcessDocuments.Text = "Передать данные выбранных документов";
            // 
            // uceProcessMD
            // 
            this.uceProcessMD.Checked = true;
            this.uceProcessMD.CheckState = System.Windows.Forms.CheckState.Checked;
            this.uceProcessMD.Location = new System.Drawing.Point(15, 195);
            this.uceProcessMD.Name = "uceProcessMD";
            this.uceProcessMD.Size = new System.Drawing.Size(158, 20);
            this.uceProcessMD.TabIndex = 1;
            this.uceProcessMD.Tag = "";
            this.uceProcessMD.Text = "Выполнить расчет кубов";
            // 
            // ultraButton1
            // 
            this.ultraButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ultraButton1.Location = new System.Drawing.Point(212, 229);
            this.ultraButton1.Name = "ultraButton1";
            this.ultraButton1.Size = new System.Drawing.Size(75, 23);
            this.ultraButton1.TabIndex = 3;
            this.ultraButton1.Text = "Отмена";
            // 
            // ultraButton2
            // 
            this.ultraButton2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ultraButton2.Location = new System.Drawing.Point(131, 229);
            this.ultraButton2.Name = "ultraButton2";
            this.ultraButton2.Size = new System.Drawing.Size(75, 23);
            this.ultraButton2.TabIndex = 3;
            this.ultraButton2.Text = "ОК";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.uosProcessDocuments);
            this.groupBox1.Location = new System.Drawing.Point(15, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(270, 71);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Обработка документов";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.uosDataWrite);
            this.groupBox2.Location = new System.Drawing.Point(15, 105);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(270, 71);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Запись данных";
            // 
            // FormWritebackOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(299, 264);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ultraButton2);
            this.Controls.Add(this.ultraButton1);
            this.Controls.Add(this.uceProcessMD);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormWritebackOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Параметры обратной записи";
            this.Load += new System.EventHandler(this.FormWritebackOptions_Load);
            ((System.ComponentModel.ISupportInitialize)(this.uosDataWrite)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uosProcessDocuments)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uceProcessMD)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraOptionSet uosDataWrite;
        private Infragistics.Win.UltraWinEditors.UltraOptionSet uosProcessDocuments;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor uceProcessMD;
        private Infragistics.Win.Misc.UltraButton ultraButton1;
        private Infragistics.Win.Misc.UltraButton ultraButton2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}