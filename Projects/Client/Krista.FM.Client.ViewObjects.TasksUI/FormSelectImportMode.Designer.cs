using Infragistics.Win.Misc;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    partial class FormSelectImportMode
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.uosImportMode = new Infragistics.Win.UltraWinEditors.UltraOptionSet();
            this.laInfo = new UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.uosImportMode)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(272, 88);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(191, 88);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "ОК";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // uosImportMode
            // 
            this.uosImportMode.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.uosImportMode.CheckedIndex = 0;
            this.uosImportMode.ItemAppearance = appearance1;
            valueListItem1.DataValue = "Default Item";
            valueListItem1.DisplayText = "Корневые задачи";
            valueListItem2.DataValue = "ValueListItem1";
            valueListItem2.DisplayText = "Подчиненные выделенной";
            this.uosImportMode.Items.Add(valueListItem1);
            this.uosImportMode.Items.Add(valueListItem2);
            this.uosImportMode.ItemSpacingVertical = 10;
            this.uosImportMode.Location = new System.Drawing.Point(31, 30);
            this.uosImportMode.Name = "uosImportMode";
            this.uosImportMode.Size = new System.Drawing.Size(289, 52);
            this.uosImportMode.TabIndex = 5;
            this.uosImportMode.Text = "Корневые задачи";
            // 
            // laInfo
            // 
            this.laInfo.Location = new System.Drawing.Point(12, 10);
            this.laInfo.Name = "laInfo";
            this.laInfo.Size = new System.Drawing.Size(335, 17);
            this.laInfo.TabIndex = 4;
            this.laInfo.Text = "Импортировать задачи верхнего уровня как";
            // 
            // FormSelectImportMode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 120);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.uosImportMode);
            this.Controls.Add(this.laInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSelectImportMode";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Выбор режима импорта задач";
            ((System.ComponentModel.ISupportInitialize)(this.uosImportMode)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private Infragistics.Win.UltraWinEditors.UltraOptionSet uosImportMode;
        private UltraLabel laInfo;
    }
}