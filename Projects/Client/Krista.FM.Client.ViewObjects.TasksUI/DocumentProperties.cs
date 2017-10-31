using System;
using System.IO;
using System.Windows.Forms;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
	public partial class DocumentProperties : Form
	{
		public DocumentProperties()
		{
			InitializeComponent();
		}

		private AddedTaskDocumentType DocumentType = AddedTaskDocumentType.ndtExisting;

		public static bool SelectDocument(AddedTaskDocumentType documentType, ref string documentName, ref string filePath, ref string comment)
		{
			DocumentProperties propFrm = new DocumentProperties();
			propFrm.DocumentType = documentType;
			propFrm.ofdSelectFile.Filter = "Все файлы (*.*)|*.*";
			propFrm.tbFileName.Enabled = documentType == AddedTaskDocumentType.ndtExisting;
			propFrm.btnFileSelect.Enabled = documentType == AddedTaskDocumentType.ndtExisting;
			switch (documentType)
			{
				case AddedTaskDocumentType.ndtExisting:
					propFrm.Text = "Добавление произвольного документа";
					break;
				case AddedTaskDocumentType.ndtNewCalcSheetExcel:
                    propFrm.Text = "Создание документа надстройки MS Excel";
					//propFrm.ofdSelectFile.Filter = "Файлы Excel (*.xls)|*.xls";
					break;
                case AddedTaskDocumentType.ndtNewCalcSheetWord:
                    propFrm.Text = "Создание документа надстройки MS Word";
                    //propFrm.ofdSelectFile.Filter = "Файлы Excel (*.xls)|*.xls";
                    break;
				case AddedTaskDocumentType.ndtNewExcel:
					propFrm.Text = "Создание нового документа MS Excel";
					//propFrm.ofdSelectFile.Filter = "Файлы Excel (*.xls)|*.xls";
					break;
				case AddedTaskDocumentType.ndtNewMDXExpert:
					propFrm.Text = "Создание нового документа MDXExpert";
					//propFrm.ofdSelectFile.Filter = "Файлы MDXExpert (*.exd)|*.exd";
					break;
				case AddedTaskDocumentType.ndtNewWord:
					propFrm.Text = "Создание нового документа MS Word";
					//propFrm.ofdSelectFile.Filter = "Файлы Word (*.doc)|*.doc";
					break;
			}
			propFrm.ShowDialog();
			if (propFrm.DialogResult == DialogResult.OK)
			{
				documentName = propFrm.tbDocumentName.Text;
				filePath = propFrm.tbFileName.Text;
				comment = propFrm.tbComment.Text;
				return true;
			}
			else return false;
		}

		private void btnFileSelect_Click(object sender, EventArgs e)
		{
			if (ofdSelectFile.ShowDialog() == DialogResult.OK)
				tbFileName.Text = ofdSelectFile.FileName;
		}

		public const int MaxFileSize = 50 * 1024 * 1024;
        private static string BadFileName = "Неверное имя файла: {0}";
        private static string BadFileSize = "Размер файла '{0}' превышает {1} байт." + Environment.NewLine + 
            "Добавление невозможно.";

        public static bool CheckFileNameAndSize(string fileName)
        {
            // проверяем корректность имени файла
            if (!File.Exists(fileName))
            {
                MessageBox.Show(String.Format(BadFileName, fileName), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // проверяем размер файла 
            FileInfo fi = new FileInfo(fileName);
            if (fi.Length > MaxFileSize)
            {
                MessageBox.Show(String.Format(BadFileSize, fileName, MaxFileSize), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

		private void btnOK_Click(object sender, EventArgs e)
		{
            if ((DocumentType == AddedTaskDocumentType.ndtExisting) && (!CheckFileNameAndSize(tbFileName.Text)))
                return;

			// имя документа не должно быть пустым
			if (tbDocumentName.Text.Length == 0)
			{
				if (DocumentType == AddedTaskDocumentType.ndtExisting)
				{
					string fileName = Path.GetFileName(tbFileName.Text);
					string fileExt = Path.GetExtension(fileName);
					fileName = fileName.Substring(0, fileName.Length - fileExt.Length);
					tbDocumentName.Text = fileName;
				}
				else
				{
					tbDocumentName.Text = "Новый документ";
				}
			}
			// длина имени документа не должна превышать 255 символов
			if (tbDocumentName.Text.Length > 255)
			{
				MessageBox.Show("Длина имени документа не может превышать 255 символов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}