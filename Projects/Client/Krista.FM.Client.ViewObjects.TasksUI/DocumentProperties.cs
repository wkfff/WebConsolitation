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
			propFrm.ofdSelectFile.Filter = "��� ����� (*.*)|*.*";
			propFrm.tbFileName.Enabled = documentType == AddedTaskDocumentType.ndtExisting;
			propFrm.btnFileSelect.Enabled = documentType == AddedTaskDocumentType.ndtExisting;
			switch (documentType)
			{
				case AddedTaskDocumentType.ndtExisting:
					propFrm.Text = "���������� ������������� ���������";
					break;
				case AddedTaskDocumentType.ndtNewCalcSheetExcel:
                    propFrm.Text = "�������� ��������� ���������� MS Excel";
					//propFrm.ofdSelectFile.Filter = "����� Excel (*.xls)|*.xls";
					break;
                case AddedTaskDocumentType.ndtNewCalcSheetWord:
                    propFrm.Text = "�������� ��������� ���������� MS Word";
                    //propFrm.ofdSelectFile.Filter = "����� Excel (*.xls)|*.xls";
                    break;
				case AddedTaskDocumentType.ndtNewExcel:
					propFrm.Text = "�������� ������ ��������� MS Excel";
					//propFrm.ofdSelectFile.Filter = "����� Excel (*.xls)|*.xls";
					break;
				case AddedTaskDocumentType.ndtNewMDXExpert:
					propFrm.Text = "�������� ������ ��������� MDXExpert";
					//propFrm.ofdSelectFile.Filter = "����� MDXExpert (*.exd)|*.exd";
					break;
				case AddedTaskDocumentType.ndtNewWord:
					propFrm.Text = "�������� ������ ��������� MS Word";
					//propFrm.ofdSelectFile.Filter = "����� Word (*.doc)|*.doc";
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
        private static string BadFileName = "�������� ��� �����: {0}";
        private static string BadFileSize = "������ ����� '{0}' ��������� {1} ����." + Environment.NewLine + 
            "���������� ����������.";

        public static bool CheckFileNameAndSize(string fileName)
        {
            // ��������� ������������ ����� �����
            if (!File.Exists(fileName))
            {
                MessageBox.Show(String.Format(BadFileName, fileName), "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // ��������� ������ ����� 
            FileInfo fi = new FileInfo(fileName);
            if (fi.Length > MaxFileSize)
            {
                MessageBox.Show(String.Format(BadFileSize, fileName, MaxFileSize), "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

		private void btnOK_Click(object sender, EventArgs e)
		{
            if ((DocumentType == AddedTaskDocumentType.ndtExisting) && (!CheckFileNameAndSize(tbFileName.Text)))
                return;

			// ��� ��������� �� ������ ���� ������
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
					tbDocumentName.Text = "����� ��������";
				}
			}
			// ����� ����� ��������� �� ������ ��������� 255 ��������
			if (tbDocumentName.Text.Length > 255)
			{
				MessageBox.Show("����� ����� ��������� �� ����� ��������� 255 ��������", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}