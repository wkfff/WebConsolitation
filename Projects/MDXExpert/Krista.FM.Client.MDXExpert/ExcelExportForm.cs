using System;
using System.ComponentModel;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Exporter;
using System.IO;
using Infragistics.Win;

namespace Krista.FM.Client.MDXExpert.Controls
{
    public partial class ExcelExportForm : Form
    {
        private ReportExporter _exporter;

        public ExcelExportForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Показывает форму экспорта, параметром передается значение по умолчанию, 
        /// что экспортировать, либо отчет (тогда параметр false) либо активный элемент (тогда параметр true)
        /// </summary>
        /// <param name="isExportActiveElement"></param>
        public void ShowDialog(bool isExportActiveElement)
        {
            this.uosExportElement.CheckedIndex = isExportActiveElement ? 1 : 0;
            this.InitializeOpenedWorkbook();
            base.ShowDialog();
        }

        /// <summary>
        /// Проинициализировать список отрытых книг
        /// </summary>
        private void InitializeOpenedWorkbook()
        {
            string selectedItemText = string.Empty;
            if (this.ceOpenedWorkbook.SelectedItem != null)
                selectedItemText = this.ceOpenedWorkbook.SelectedItem.DisplayText;

            this.ceOpenedWorkbook.Items.Clear();
            Excel.Application excel = Common.ExcelUtils.GetActiveExcel();
            if (excel != null)
            {
                foreach (Excel.Workbook book in excel.Workbooks)
                {
                    this.ceOpenedWorkbook.Items.Add(book.FullName, book.Name);
                }
            }
            this.ceOpenedWorkbook.NullText = (this.ceOpenedWorkbook.Items.Count == 0) ? "Нет открытых книг" : 
                string.Empty;

            foreach (ValueListItem item in this.ceOpenedWorkbook.Items)
            {
                if (item.DisplayText == selectedItemText)
                {
                    this.ceOpenedWorkbook.SelectedItem = item;
                    break;
                }
            }
            //Если книги есть, но ни одна из них не выделена, выбираем первую.
            //if ((this.ceOpenedWorkbook.Items.Count > 0) && (this.ceOpenedWorkbook.SelectedItem == null))
            //    this.ceOpenedWorkbook.SelectedIndex = 0;
        }

        /// <summary>
        /// Экспорт
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ubExport_Click(object sender, EventArgs e)
        {
            string bookPath = string.Empty;

            //Экспортируем в существующию книгу
            switch ((string)this.uosExportExcelBook.CheckedItem.DataValue)
            {
                case "Opened":
                    {
                        if (this.ceOpenedWorkbook.SelectedItem == null)
                            bookPath = string.Empty;
                        else
                            bookPath = (string)this.ceOpenedWorkbook.SelectedItem.DataValue;

                        if (bookPath == string.Empty)
                        {
                            MessageBox.Show("Не указано имя открытой книги.", "Ошибка", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        break;
                    }
                case "Exist":
                    {
                        bookPath = this.teExistBookPatch.Text;
                        //Проверим наличие файла
                        if (!this.IsValidPath(bookPath))
                        {
                            MessageBox.Show("Указанный файл не существует, или не является книгой Excel.", 
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        break;
                    }
            }

            this.Hide();

            if ((string)uosExportElement.CheckedItem.DataValue == "Report")
            {
                //Экспортируем отчет целиком
                this.Exporter.ToExcel(bookPath, this.cePrintVersion.Checked, this.cbUnionExport.Checked, this.ceIsSeparateProperties.Checked);
            }
            else
            {
                //Экспортируем активный элемент отчета
                this.Exporter.ActiveElementToExcel(bookPath, this.cePrintVersion.Checked, this.ceIsSeparateProperties.Checked);
            }
        }

        private void ubCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ceOpenedWorkbook_BeforeDropDown(object sender, CancelEventArgs e)
        {
            this.uosExportExcelBook.CheckedIndex = 1;
            this.InitializeOpenedWorkbook();
        }

        void teExistBookPatch_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.teExistBookPatch.Text = this.openFileDialog.FileName;
                this.uosExportExcelBook.CheckedIndex = 2;
            }
        }

        void teExistBookPatch_Click(object sender, EventArgs e)
        {
            this.uosExportExcelBook.CheckedIndex = 2;
        }

        /// <summary>
        /// Проверяет правильность пути к книге Excel
        /// </summary>
        /// <returns></returns>
        private bool IsValidPath(string bookPath)
        {
            if (bookPath == string.Empty)
                return false;

            FileInfo fileInfo = new FileInfo(bookPath);
            return fileInfo.Exists && (fileInfo.Extension == ".xls");
        }

        /// <summary>
        /// Экспортер отчета
        /// </summary>
        public ReportExporter Exporter
        {
            get { return _exporter; }
            set { _exporter = value; }
        }

    }
}