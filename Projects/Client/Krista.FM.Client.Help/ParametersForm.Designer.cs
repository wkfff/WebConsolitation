namespace Krista.FM.Client.Help
{
    partial class ParametersForm
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
            this.components = new System.ComponentModel.Container();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.gbVariant = new System.Windows.Forms.GroupBox();
            this.rbDiagramFull = new System.Windows.Forms.RadioButton();
            this.rbDiagramAdd = new System.Windows.Forms.RadioButton();
            this.gbMode = new System.Windows.Forms.GroupBox();
            this.rbDeveloperMode = new System.Windows.Forms.RadioButton();
            this.rbUserMode = new System.Windows.Forms.RadioButton();
            this.btnHelpCreate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.rbLiteMode = new System.Windows.Forms.RadioButton();
            this.gbVariant.SuspendLayout();
            this.gbMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // gbVariant
            // 
            this.gbVariant.Controls.Add(this.rbDiagramFull);
            this.gbVariant.Controls.Add(this.rbDiagramAdd);
            this.gbVariant.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gbVariant.Location = new System.Drawing.Point(12, 144);
            this.gbVariant.Name = "gbVariant";
            this.gbVariant.Size = new System.Drawing.Size(364, 85);
            this.gbVariant.TabIndex = 0;
            this.gbVariant.TabStop = false;
            this.gbVariant.Text = "Режим формирования диаграмм";
            // 
            // rbDiagramFull
            // 
            this.rbDiagramFull.AutoSize = true;
            this.rbDiagramFull.Checked = true;
            this.rbDiagramFull.Location = new System.Drawing.Point(20, 24);
            this.rbDiagramFull.Name = "rbDiagramFull";
            this.rbDiagramFull.Size = new System.Drawing.Size(173, 17);
            this.rbDiagramFull.TabIndex = 1;
            this.rbDiagramFull.TabStop = true;
            this.rbDiagramFull.Text = "Полная генерация диаграмм";
            this.rbDiagramFull.UseVisualStyleBackColor = true;
            // 
            // rbDiagramAdd
            // 
            this.rbDiagramAdd.AutoSize = true;
            this.rbDiagramAdd.Location = new System.Drawing.Point(20, 53);
            this.rbDiagramAdd.Name = "rbDiagramAdd";
            this.rbDiagramAdd.Size = new System.Drawing.Size(142, 17);
            this.rbDiagramAdd.TabIndex = 0;
            this.rbDiagramAdd.Text = "Дополнение диаграмм";
            this.rbDiagramAdd.UseVisualStyleBackColor = true;
            // 
            // gbMode
            // 
            this.gbMode.Controls.Add(this.rbLiteMode);
            this.gbMode.Controls.Add(this.rbDeveloperMode);
            this.gbMode.Controls.Add(this.rbUserMode);
            this.gbMode.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gbMode.Location = new System.Drawing.Point(12, 12);
            this.gbMode.Name = "gbMode";
            this.gbMode.Size = new System.Drawing.Size(364, 116);
            this.gbMode.TabIndex = 1;
            this.gbMode.TabStop = false;
            this.gbMode.Text = "Режим генерации";
            // 
            // rbDeveloperMode
            // 
            this.rbDeveloperMode.AutoSize = true;
            this.rbDeveloperMode.Location = new System.Drawing.Point(20, 53);
            this.rbDeveloperMode.Name = "rbDeveloperMode";
            this.rbDeveloperMode.Size = new System.Drawing.Size(125, 17);
            this.rbDeveloperMode.TabIndex = 1;
            this.rbDeveloperMode.TabStop = true;
            this.rbDeveloperMode.Text = "Для разработчиков";
            this.rbDeveloperMode.UseVisualStyleBackColor = true;
            // 
            // rbUserMode
            // 
            this.rbUserMode.AutoSize = true;
            this.rbUserMode.Checked = true;
            this.rbUserMode.Location = new System.Drawing.Point(20, 24);
            this.rbUserMode.Name = "rbUserMode";
            this.rbUserMode.Size = new System.Drawing.Size(126, 17);
            this.rbUserMode.TabIndex = 0;
            this.rbUserMode.TabStop = true;
            this.rbUserMode.Text = "Для пользователей";
            this.rbUserMode.UseVisualStyleBackColor = true;
            // 
            // btnHelpCreate
            // 
            this.btnHelpCreate.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnHelpCreate.Location = new System.Drawing.Point(220, 235);
            this.btnHelpCreate.Name = "btnHelpCreate";
            this.btnHelpCreate.Size = new System.Drawing.Size(75, 23);
            this.btnHelpCreate.TabIndex = 2;
            this.btnHelpCreate.Text = "OK";
            this.btnHelpCreate.UseVisualStyleBackColor = true;
            this.btnHelpCreate.Click += new System.EventHandler(this.btnHelpCreate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCancel.Location = new System.Drawing.Point(301, 235);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // rbLiteMode
            // 
            this.rbLiteMode.AutoSize = true;
            this.rbLiteMode.Location = new System.Drawing.Point(20, 81);
            this.rbLiteMode.Name = "rbLiteMode";
            this.rbLiteMode.Size = new System.Drawing.Size(136, 17);
            this.rbLiteMode.TabIndex = 2;
            this.rbLiteMode.TabStop = true;
            this.rbLiteMode.Text = "Упрощенный вариант";
            this.rbLiteMode.UseVisualStyleBackColor = true;
            // 
            // ParametersForm
            // 
            this.AcceptButton = this.btnHelpCreate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(389, 269);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnHelpCreate);
            this.Controls.Add(this.gbMode);
            this.Controls.Add(this.gbVariant);
            this.ForeColor = System.Drawing.SystemColors.Info;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ParametersForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Параметры генерации справки";
            this.gbVariant.ResumeLayout(false);
            this.gbVariant.PerformLayout();
            this.gbMode.ResumeLayout(false);
            this.gbMode.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.GroupBox gbVariant;
        private System.Windows.Forms.RadioButton rbDiagramFull;
        private System.Windows.Forms.RadioButton rbDiagramAdd;
        private System.Windows.Forms.GroupBox gbMode;
        private System.Windows.Forms.RadioButton rbDeveloperMode;
        private System.Windows.Forms.RadioButton rbUserMode;
        private System.Windows.Forms.Button btnHelpCreate;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton rbLiteMode;
    }
}