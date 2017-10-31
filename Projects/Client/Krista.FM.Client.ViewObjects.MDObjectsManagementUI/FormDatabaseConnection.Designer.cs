using Infragistics.Win.Misc;

namespace Krista.FM.Client.ViewObjects.MDObjectsManagementUI
{
    partial class FormDatabaseConnection
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
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.layoutPanelOptions = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxServerName = new System.Windows.Forms.TextBox();
            this.textBoxDatabaseName = new System.Windows.Forms.TextBox();
            this.textBoxDataSourceName = new System.Windows.Forms.TextBox();
            this.lbServerName = new Infragistics.Win.Misc.UltraLabel();
            this.lbDatabaseName = new Infragistics.Win.Misc.UltraLabel();
            this.lbDataSourceName = new Infragistics.Win.Misc.UltraLabel();
            this.pnlButtons.SuspendLayout();
            this.layoutPanelOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Controls.Add(this.btnOk);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(0, 78);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(451, 32);
            this.pnlButtons.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(292, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(373, 6);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Ок";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // layoutPanelOptions
            // 
            this.layoutPanelOptions.ColumnCount = 2;
            this.layoutPanelOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.layoutPanelOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.layoutPanelOptions.Controls.Add(this.textBoxServerName, 1, 0);
            this.layoutPanelOptions.Controls.Add(this.textBoxDatabaseName, 1, 1);
            this.layoutPanelOptions.Controls.Add(this.textBoxDataSourceName, 1, 2);
            this.layoutPanelOptions.Controls.Add(this.lbServerName, 0, 0);
            this.layoutPanelOptions.Controls.Add(this.lbDatabaseName, 0, 1);
            this.layoutPanelOptions.Controls.Add(this.lbDataSourceName, 0, 2);
            this.layoutPanelOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutPanelOptions.Location = new System.Drawing.Point(0, 0);
            this.layoutPanelOptions.Name = "layoutPanelOptions";
            this.layoutPanelOptions.RowCount = 3;
            this.layoutPanelOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutPanelOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutPanelOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutPanelOptions.Size = new System.Drawing.Size(451, 78);
            this.layoutPanelOptions.TabIndex = 1;
            // 
            // textBoxServerName
            // 
            this.textBoxServerName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxServerName.Location = new System.Drawing.Point(138, 3);
            this.textBoxServerName.Name = "textBoxServerName";
            this.textBoxServerName.Size = new System.Drawing.Size(310, 20);
            this.textBoxServerName.TabIndex = 0;
            // 
            // textBoxDatabaseName
            // 
            this.textBoxDatabaseName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDatabaseName.Location = new System.Drawing.Point(138, 29);
            this.textBoxDatabaseName.Name = "textBoxDatabaseName";
            this.textBoxDatabaseName.Size = new System.Drawing.Size(310, 20);
            this.textBoxDatabaseName.TabIndex = 1;
            // 
            // textBoxDataSourceName
            // 
            this.textBoxDataSourceName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDataSourceName.Location = new System.Drawing.Point(138, 55);
            this.textBoxDataSourceName.Name = "textBoxDataSourceName";
            this.textBoxDataSourceName.Size = new System.Drawing.Size(310, 20);
            this.textBoxDataSourceName.TabIndex = 2;
            // 
            // lbServerName
            // 
            appearance1.TextHAlignAsString = "Center";
            this.lbServerName.Appearance = appearance1;
            this.lbServerName.AutoSize = true;
            this.lbServerName.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbServerName.Location = new System.Drawing.Point(3, 3);
            this.lbServerName.Name = "lbServerName";
            this.lbServerName.Size = new System.Drawing.Size(76, 20);
            this.lbServerName.TabIndex = 3;
            this.lbServerName.Text = "Имя сервера:";
            // 
            // lbDatabaseName
            // 
            appearance2.TextHAlignAsString = "Left";
            appearance2.TextVAlignAsString = "Middle";
            this.lbDatabaseName.Appearance = appearance2;
            this.lbDatabaseName.AutoSize = true;
            this.lbDatabaseName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbDatabaseName.Location = new System.Drawing.Point(3, 29);
            this.lbDatabaseName.Name = "lbDatabaseName";
            this.lbDatabaseName.Size = new System.Drawing.Size(129, 20);
            this.lbDatabaseName.TabIndex = 4;
            this.lbDatabaseName.Text = "Имя базы данных:";
            // 
            // lbDataSourceName
            // 
            appearance3.TextHAlignAsString = "Left";
            appearance3.TextVAlignAsString = "Middle";
            this.lbDataSourceName.Appearance = appearance3;
            this.lbDataSourceName.AutoSize = true;
            this.lbDataSourceName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbDataSourceName.Location = new System.Drawing.Point(3, 55);
            this.lbDataSourceName.Name = "lbDataSourceName";
            this.lbDataSourceName.Size = new System.Drawing.Size(129, 20);
            this.lbDataSourceName.TabIndex = 5;
            this.lbDataSourceName.Text = "Имя источника данных:";
            // 
            // FormDatabaseConnection
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(451, 110);
            this.ControlBox = false;
            this.Controls.Add(this.layoutPanelOptions);
            this.Controls.Add(this.pnlButtons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormDatabaseConnection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Введите параметры подключения к многомерной базе";
            this.pnlButtons.ResumeLayout(false);
            this.layoutPanelOptions.ResumeLayout(false);
            this.layoutPanelOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.TableLayoutPanel layoutPanelOptions;
        private System.Windows.Forms.TextBox textBoxServerName;
        private System.Windows.Forms.TextBox textBoxDatabaseName;
        private System.Windows.Forms.TextBox textBoxDataSourceName;
        private UltraLabel lbServerName;
        private UltraLabel lbDatabaseName;
        private UltraLabel lbDataSourceName;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
    }
}