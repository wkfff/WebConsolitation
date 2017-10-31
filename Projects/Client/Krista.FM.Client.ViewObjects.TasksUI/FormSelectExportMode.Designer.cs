using Infragistics.Win.Misc;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    partial class FormSelectExportMode
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSelectExportMode));
            this.laInfo = new UltraLabel();
            this.uosExportMode = new Infragistics.Win.UltraWinEditors.UltraOptionSet();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.uosExportMode)).BeginInit();
            this.SuspendLayout();
            // 
            // laInfo
            // 
            this.laInfo.Location = new System.Drawing.Point(12, 9);
            this.laInfo.Name = "laInfo";
            this.laInfo.Size = new System.Drawing.Size(335, 17);
            this.laInfo.TabIndex = 0;
            this.laInfo.Text = "Обнаружены подчиненные задачи. \r\n";
            // 
            // uosExportMode
            // 
            this.uosExportMode.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.uosExportMode.CheckedIndex = 0;
            this.uosExportMode.ItemAppearance = appearance1;
            valueListItem1.DataValue = "Default Item";
            valueListItem1.DisplayText = "Экспортрировать только выделеные";
            valueListItem2.DataValue = "ValueListItem1";
            valueListItem2.DisplayText = "Экспортировать выделенные и подчиненные";
            this.uosExportMode.Items.Add(valueListItem1);
            this.uosExportMode.Items.Add(valueListItem2);
            this.uosExportMode.ItemSpacingVertical = 10;
            this.uosExportMode.Location = new System.Drawing.Point(31, 29);
            this.uosExportMode.Name = "uosExportMode";
            this.uosExportMode.Size = new System.Drawing.Size(289, 52);
            this.uosExportMode.TabIndex = 1;
            this.uosExportMode.Text = "Экспортрировать только выделеные";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(191, 87);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "ОК";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(272, 87);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // FormSelectExportMode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 120);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.uosExportMode);
            this.Controls.Add(this.laInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSelectExportMode";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Выбор режим экспорта задач";
            ((System.ComponentModel.ISupportInitialize)(this.uosExportMode)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private UltraLabel laInfo;
        private Infragistics.Win.UltraWinEditors.UltraOptionSet uosExportMode;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;

    }
}