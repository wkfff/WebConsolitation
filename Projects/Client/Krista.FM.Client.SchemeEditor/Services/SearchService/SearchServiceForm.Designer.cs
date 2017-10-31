namespace Krista.FM.Client.SchemeEditor.Services.SearchService
{
    partial class SearchServiceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchServiceForm));
            this.FindButton = new System.Windows.Forms.Button();
            this.FindEdit = new System.Windows.Forms.TextBox();
            this.FindLabel = new System.Windows.Forms.Label();
            this.SearchParams = new System.Windows.Forms.CheckedListBox();
            this.TypeObjList = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // FindButton
            // 
            this.FindButton.Location = new System.Drawing.Point(223, 23);
            this.FindButton.Name = "FindButton";
            this.FindButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.FindButton.Size = new System.Drawing.Size(75, 23);
            this.FindButton.TabIndex = 1;
            this.FindButton.Text = "Найти";
            this.FindButton.UseVisualStyleBackColor = true;
            this.FindButton.Click += new System.EventHandler(this.FindButton_Click);
            // 
            // FindEdit
            // 
            this.FindEdit.Location = new System.Drawing.Point(12, 25);
            this.FindEdit.Name = "FindEdit";
            this.FindEdit.Size = new System.Drawing.Size(205, 20);
            this.FindEdit.TabIndex = 0;
            // 
            // FindLabel
            // 
            this.FindLabel.AutoSize = true;
            this.FindLabel.Location = new System.Drawing.Point(12, 8);
            this.FindLabel.Name = "FindLabel";
            this.FindLabel.Size = new System.Drawing.Size(106, 13);
            this.FindLabel.TabIndex = 2;
            this.FindLabel.Text = "Строка для поиска:";
            // 
            // SearchParams
            // 
            this.SearchParams.CheckOnClick = true;
            this.SearchParams.FormattingEnabled = true;
            this.SearchParams.Items.AddRange(new object[] {
            "Чувствительность к регистру",
            "Регулярные выражения",
            "Точная фраза"});
            this.SearchParams.Location = new System.Drawing.Point(12, 72);
            this.SearchParams.Name = "SearchParams";
            this.SearchParams.Size = new System.Drawing.Size(205, 79);
            this.SearchParams.TabIndex = 2;
            // 
            // TypeObjList
            // 
            this.TypeObjList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TypeObjList.CheckOnClick = true;
            this.TypeObjList.FormattingEnabled = true;
            this.TypeObjList.Items.AddRange(new object[] {
            "Пакеты",
            "Классификаторы",
            "Ассоциации",
            "Документы",
            "Атрибуты"});
            this.TypeObjList.Location = new System.Drawing.Point(223, 72);
            this.TypeObjList.Name = "TypeObjList";
            this.TypeObjList.Size = new System.Drawing.Size(409, 79);
            this.TypeObjList.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Параметры:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(220, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Искать в:";
            // 
            // SearchServiceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 163);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TypeObjList);
            this.Controls.Add(this.SearchParams);
            this.Controls.Add(this.FindLabel);
            this.Controls.Add(this.FindEdit);
            this.Controls.Add(this.FindButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(652, 190);
            this.Name = "SearchServiceForm";
            this.ShowInTaskbar = false;
            this.Text = "Поиск";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SearchServiceForm_KeyPress);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SearchServiceForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button FindButton;
        private System.Windows.Forms.TextBox FindEdit;
        private System.Windows.Forms.Label FindLabel;
        private System.Windows.Forms.CheckedListBox SearchParams;
        private System.Windows.Forms.CheckedListBox TypeObjList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}