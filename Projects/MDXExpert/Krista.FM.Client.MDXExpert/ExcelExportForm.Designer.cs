namespace Krista.FM.Client.MDXExpert.Controls
{
    partial class ExcelExportForm
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
            Infragistics.Win.ValueListItem valueListItem6 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem7 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem8 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton2 = new Infragistics.Win.UltraWinEditors.EditorButton("ChangeBookPath");
            Infragistics.Win.ValueListItem valueListItem9 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem10 = new Infragistics.Win.ValueListItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.cbUnionExport = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ubExport = new Infragistics.Win.Misc.UltraButton();
            this.cePrintVersion = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ugbExportExcelBook = new Infragistics.Win.Misc.UltraGroupBox();
            this.ceOpenedWorkbook = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.uosExportExcelBook = new Infragistics.Win.UltraWinEditors.UltraOptionSet();
            this.teExistBookPatch = new Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor();
            this.ubCancel = new Infragistics.Win.Misc.UltraButton();
            this.ugbExportElement = new Infragistics.Win.Misc.UltraGroupBox();
            this.uosExportElement = new Infragistics.Win.UltraWinEditors.UltraOptionSet();
            this.ceIsSeparateProperties = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbUnionExport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cePrintVersion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ugbExportExcelBook)).BeginInit();
            this.ugbExportExcelBook.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ceOpenedWorkbook)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uosExportExcelBook)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ugbExportElement)).BeginInit();
            this.ugbExportElement.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uosExportElement)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceIsSeparateProperties)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Книги Microsoft Offise Excel (*.xls)|*.xls";
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.None;
            this.ultraGroupBox1.Controls.Add(this.ceIsSeparateProperties);
            this.ultraGroupBox1.Controls.Add(this.cbUnionExport);
            this.ultraGroupBox1.Controls.Add(this.ubExport);
            this.ultraGroupBox1.Controls.Add(this.cePrintVersion);
            this.ultraGroupBox1.Controls.Add(this.ugbExportExcelBook);
            this.ultraGroupBox1.Controls.Add(this.ubCancel);
            this.ultraGroupBox1.Controls.Add(this.ugbExportElement);
            this.ultraGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(313, 320);
            this.ultraGroupBox1.TabIndex = 8;
            // 
            // cbUnionExport
            // 
            this.cbUnionExport.BackColor = System.Drawing.Color.Transparent;
            this.cbUnionExport.BackColorInternal = System.Drawing.Color.Transparent;
            this.cbUnionExport.Location = new System.Drawing.Point(19, 256);
            this.cbUnionExport.Name = "cbUnionExport";
            this.cbUnionExport.Size = new System.Drawing.Size(267, 20);
            this.cbUnionExport.TabIndex = 13;
            this.cbUnionExport.Text = "Экспортировать все элементы на один лист";
            // 
            // ubExport
            // 
            this.ubExport.Location = new System.Drawing.Point(140, 286);
            this.ubExport.Name = "ubExport";
            this.ubExport.Size = new System.Drawing.Size(72, 22);
            this.ubExport.TabIndex = 10;
            this.ubExport.Text = "Экспорт";
            this.ubExport.Click += new System.EventHandler(this.ubExport_Click);
            // 
            // cePrintVersion
            // 
            this.cePrintVersion.BackColor = System.Drawing.Color.Transparent;
            this.cePrintVersion.BackColorInternal = System.Drawing.Color.Transparent;
            this.cePrintVersion.Checked = true;
            this.cePrintVersion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cePrintVersion.Location = new System.Drawing.Point(19, 200);
            this.cePrintVersion.Name = "cePrintVersion";
            this.cePrintVersion.Size = new System.Drawing.Size(236, 20);
            this.cePrintVersion.TabIndex = 9;
            this.cePrintVersion.Text = "Не экспортировать цвет и заливку";
            // 
            // ugbExportExcelBook
            // 
            this.ugbExportExcelBook.Controls.Add(this.ceOpenedWorkbook);
            this.ugbExportExcelBook.Controls.Add(this.uosExportExcelBook);
            this.ugbExportExcelBook.Controls.Add(this.teExistBookPatch);
            this.ugbExportExcelBook.Location = new System.Drawing.Point(12, 90);
            this.ugbExportExcelBook.Name = "ugbExportExcelBook";
            this.ugbExportExcelBook.Size = new System.Drawing.Size(287, 104);
            this.ugbExportExcelBook.TabIndex = 12;
            this.ugbExportExcelBook.Text = "Книга";
            // 
            // ceOpenedWorkbook
            // 
            this.ceOpenedWorkbook.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.ceOpenedWorkbook.Location = new System.Drawing.Point(114, 44);
            this.ceOpenedWorkbook.Name = "ceOpenedWorkbook";
            this.ceOpenedWorkbook.Size = new System.Drawing.Size(160, 21);
            this.ceOpenedWorkbook.TabIndex = 5;
            this.ceOpenedWorkbook.BeforeDropDown += new System.ComponentModel.CancelEventHandler(this.ceOpenedWorkbook_BeforeDropDown);
            // 
            // uosExportExcelBook
            // 
            this.uosExportExcelBook.BackColor = System.Drawing.Color.Transparent;
            this.uosExportExcelBook.BackColorInternal = System.Drawing.Color.Transparent;
            this.uosExportExcelBook.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.uosExportExcelBook.CheckedIndex = 0;
            valueListItem6.DataValue = "New";
            valueListItem6.DisplayText = "Новая";
            valueListItem7.DataValue = "Opened";
            valueListItem7.DisplayText = "Открытая";
            valueListItem8.DataValue = "Exist";
            valueListItem8.DisplayText = "Существующая";
            this.uosExportExcelBook.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem6,
            valueListItem7,
            valueListItem8});
            this.uosExportExcelBook.ItemSpacingVertical = 10;
            this.uosExportExcelBook.Location = new System.Drawing.Point(7, 19);
            this.uosExportExcelBook.Name = "uosExportExcelBook";
            this.uosExportExcelBook.Size = new System.Drawing.Size(101, 72);
            this.uosExportExcelBook.TabIndex = 3;
            this.uosExportExcelBook.Text = "Новая";
            // 
            // teExistBookPatch
            // 
            editorButton2.Key = "ChangeBookPath";
            editorButton2.Text = "...";
            this.teExistBookPatch.ButtonsRight.Add(editorButton2);
            this.teExistBookPatch.Location = new System.Drawing.Point(114, 71);
            this.teExistBookPatch.Name = "teExistBookPatch";
            this.teExistBookPatch.ScrollBarDisplayStyle = Infragistics.Win.UltraWinScrollBar.ScrollBarDisplayStyle.Never;
            this.teExistBookPatch.Size = new System.Drawing.Size(160, 20);
            this.teExistBookPatch.TabIndex = 4;
            this.teExistBookPatch.Value = "";
            this.teExistBookPatch.WrapText = false;
            this.teExistBookPatch.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.teExistBookPatch_EditorButtonClick);
            this.teExistBookPatch.Click += new System.EventHandler(this.teExistBookPatch_Click);
            // 
            // ubCancel
            // 
            this.ubCancel.Location = new System.Drawing.Point(224, 286);
            this.ubCancel.Name = "ubCancel";
            this.ubCancel.Size = new System.Drawing.Size(75, 22);
            this.ubCancel.TabIndex = 11;
            this.ubCancel.Text = "Отмена";
            this.ubCancel.Click += new System.EventHandler(this.ubCancel_Click);
            // 
            // ugbExportElement
            // 
            this.ugbExportElement.Controls.Add(this.uosExportElement);
            this.ugbExportElement.Location = new System.Drawing.Point(12, 10);
            this.ugbExportElement.Name = "ugbExportElement";
            this.ugbExportElement.Size = new System.Drawing.Size(200, 74);
            this.ugbExportElement.TabIndex = 8;
            this.ugbExportElement.Text = "Экспорт";
            // 
            // uosExportElement
            // 
            this.uosExportElement.BackColor = System.Drawing.Color.Transparent;
            this.uosExportElement.BackColorInternal = System.Drawing.Color.Transparent;
            this.uosExportElement.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.uosExportElement.CheckedIndex = 0;
            valueListItem9.DataValue = "Report";
            valueListItem9.DisplayText = "Отчета";
            valueListItem10.DataValue = "ActiveElement";
            valueListItem10.DisplayText = "Активного элемента";
            this.uosExportElement.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem9,
            valueListItem10});
            this.uosExportElement.ItemSpacingVertical = 10;
            this.uosExportElement.Location = new System.Drawing.Point(7, 19);
            this.uosExportElement.Name = "uosExportElement";
            this.uosExportElement.Size = new System.Drawing.Size(157, 45);
            this.uosExportElement.TabIndex = 0;
            this.uosExportElement.Text = "Отчета";
            // 
            // ceIsSeparateProperties
            // 
            this.ceIsSeparateProperties.BackColor = System.Drawing.Color.Transparent;
            this.ceIsSeparateProperties.BackColorInternal = System.Drawing.Color.Transparent;
            this.ceIsSeparateProperties.Location = new System.Drawing.Point(19, 222);
            this.ceIsSeparateProperties.Name = "ceIsSeparateProperties";
            this.ceIsSeparateProperties.Size = new System.Drawing.Size(280, 31);
            this.ceIsSeparateProperties.TabIndex = 14;
            this.ceIsSeparateProperties.Text = "Экспортировать свойства элементов в отдельные ячейки";
            // 
            // ExcelExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(313, 320);
            this.Controls.Add(this.ultraGroupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ExcelExportForm";
            this.ShowInTaskbar = false;
            this.Text = "Экспорт в Excel";
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cbUnionExport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cePrintVersion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ugbExportExcelBook)).EndInit();
            this.ugbExportExcelBook.ResumeLayout(false);
            this.ugbExportExcelBook.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ceOpenedWorkbook)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uosExportExcelBook)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ugbExportElement)).EndInit();
            this.ugbExportElement.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uosExportElement)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceIsSeparateProperties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.Misc.UltraButton ubExport;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor cePrintVersion;
        private Infragistics.Win.Misc.UltraGroupBox ugbExportExcelBook;
        private Infragistics.Win.UltraWinEditors.UltraOptionSet uosExportExcelBook;
        private Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor teExistBookPatch;
        private Infragistics.Win.Misc.UltraButton ubCancel;
        private Infragistics.Win.Misc.UltraGroupBox ugbExportElement;
        private Infragistics.Win.UltraWinEditors.UltraOptionSet uosExportElement;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor ceOpenedWorkbook;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbUnionExport;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor ceIsSeparateProperties;
    }
}