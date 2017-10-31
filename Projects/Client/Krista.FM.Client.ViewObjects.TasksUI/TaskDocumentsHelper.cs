using System;
using System.IO;
using System.Reflection;
using System.Data;
using System.Windows.Forms;

using Krista.FM.Common;
using Krista.FM.Common.FileUtils;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Common.MdxExpertHelpers;
using Krista.FM.Common.OfficeHelpers;
using Krista.FM.Common.OfficePluginServices.FMOfficeAddin;
using Krista.FM.Common.TaskDocuments;
using Krista.FM.Common.TaskDocuments.OfficeCustomPropertiesAdapters;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
	/// <summary>
	/// ��������������� ����� ��� ������ � ����������� �����
	/// </summary>
	internal class DocumentsHelper
	{
		/// <summary>
		/// �������� ����� ����������
		/// </summary>
		private const string DocsFolderName = "TasksDocuments";

		/// <summary>
		/// ���� � ��������� ����� ����������
		/// </summary>
		public static string LocalDocsPath = String.Empty;

        static DocumentsHelper()
        {
            // ��������� ������� ����� ��������� ���������� (���� ��� - �������)
            LocalDocsPath = Assembly.GetExecutingAssembly().Location;
            LocalDocsPath = Path.GetDirectoryName(LocalDocsPath);
            LocalDocsPath = LocalDocsPath + Path.DirectorySeparatorChar + DocsFolderName;
            // ���� �� ���������� - �������
            if (!Directory.Exists(LocalDocsPath)) 
                Directory.CreateDirectory(LocalDocsPath);
        }

        private static void GetDocumentTypeFromHelper(string fileName, ref TaskDocumentType dt)
        {
            string docTypeStr = string.Empty;
            using (OfficeCustomPropertiesAdapter propertiesAdapter = OfficeCustomPropertiesFactory.Create(fileName))
            {
                var prop = propertiesAdapter.GetProperty(FMOfficeAddinConsts.pspSheetType);
                if (prop == null)
                    prop = propertiesAdapter.GetProperty("PlanningSheetType");
                if (prop != null)
                    docTypeStr = prop.ToString();
            }
            // ���� ������� - �������� ��� � ������
            if (!string.IsNullOrEmpty(docTypeStr) && (docTypeStr != "null"))
                try
                {   // ��� �������� ���������� ������ ����������� ������ 
                    dt = (TaskDocumentType)Convert.ToInt32(docTypeStr);
                }
                catch
                {
                }
        }

        /// <summary>
		/// ���������� ��� ��������� �� ����� �����
		/// </summary>
		/// <param name="fileName">��� �����</param>
		/// <returns>��� ���������</returns>
		public TaskDocumentType ResolveDocumentType(string fileName)
		{
			TaskDocumentType dt = TaskDocumentType.dtArbitraryDocument;
			string fileExt = Path.GetExtension(fileName);
			switch (fileExt)
			{
				case ".doc":
					dt = TaskDocumentType.dtWordDocument;
                    break;
				case ".exd":
					dt = TaskDocumentType.dtMDXExpertDocument;
					break;
				// ����� Excel, � ���� �������, ����� �������� ����������� ������������
				case ".xls":
					dt = TaskDocumentType.dtExcelDocument;
					break;
                case ".docx":
                    //  ���� ����-��-��������� ������, �� �� ������������ �� ��������� ������
                    if (OfficeHelper.GetWordVersionNumber() < 12)
                        break;
                    dt = TaskDocumentType.dtWordDocument;
                    break;
                case ".xlsx":
                    //  ���� ����-��-��������� ������, �� �� ������������ �� ��������� ������
                    if (OfficeHelper.GetExcelVersionNumber() < 12)
                        break;
                    dt = TaskDocumentType.dtExcelDocument;
			        break;
            }
            if ((dt == TaskDocumentType.dtExcelDocument) || (dt == TaskDocumentType.dtWordDocument))
                GetDocumentTypeFromHelper(fileName, ref dt);
			return dt;
		}

		/// <summary>
		/// ���������������� ��������� �������� � ���������
		/// </summary>
		/// <param name="progressObj">������-��������</param>
		/// <param name="activeTask">�������� ������</param>
		/// <param name="row">������ � ����������� ���������</param>
		public void CheckDocumentTypeAndCRC(Progress progressObj, ITask activeTask, DataRow row)
		{
			// �������� ��������� ��������� ���������
            string fmtStr = "{0} ({1} ��)";
			int taskID = activeTask.ID;
			int documentID = Convert.ToInt32(row["ID"]);
			string documentName = Convert.ToString(row["Name"]);
			string sourceFileName = Convert.ToString(row["SourceFileName"]);
			string sourceFileExt = Path.GetExtension(sourceFileName).ToLower();
			string localDocumentName = GetLocalDocumentName(taskID, documentID,
				documentName,sourceFileName);

			if (File.Exists(localDocumentName))
			{
                if (!FileHelper.FileIsVacant(localDocumentName))
                    return;

				// ���� ��� �������� ����� � ������ �������� - ��������� �� ��������� �� ��� ���������
                if ((sourceFileExt == ".xls") || (sourceFileExt == ".doc"))
				{
					TaskDocumentType oldType = (TaskDocumentType)Convert.ToInt32(row["DocumentType"]);
					TaskDocumentType newType = ResolveDocumentType(localDocumentName);
					if (oldType != newType)
						row["DocumentType"] = (int)newType;
				}

				// ������ ��������� �� ���������� �� ���������� �����
				ulong localFileCRC32;
                byte[] fileData = FileHelper.ReadFileData(localDocumentName);
                progressObj.Text = String.Format(fmtStr, documentName, fileData.Length / 1024);
                // ��������� CRC ��������� ������
                localFileCRC32 = CRCHelper.CRC32(fileData, 0, fileData.Length);
				// ��������� CRC ��������� ������
				ulong serverFileCRC32 = activeTask.GetDocumentCRC32(documentID);
				// ���� CRC ���������� - ��������� ��������� ������
				if (localFileCRC32 != serverFileCRC32)
				{
                    activeTask.SetDocumentData(documentID, fileData);
				}
                GC.GetTotalMemory(true);
			}
		}

		/// <summary>
		/// ������������ ������ �������� ���������� ���������
		/// </summary>
		/// <param name="taskID">ID ������</param>
		/// <param name="documentID">ID ���������</param>
		/// <param name="documentName">�������� ���������</param>
		/// <param name="sourceFileName">�������� ��������� ����� (������ ������� ����������)</param>
		/// <returns>������ ���� � ���������� ���������</returns>
		public static string GetLocalDocumentName(int taskID, int documentID, string documentName, string sourceFileName)
		{
			// �������� ������������ � ������ ������ ������� �� �������������
			char[] invalidChars = Path.GetInvalidFileNameChars();
			string normDocName = documentName; 
			foreach (char chr in invalidChars)
			{
				if (normDocName.IndexOf(chr) != -1)
					normDocName = normDocName.Replace(chr, '_');
			}
			// ��������� ��� �����
			return String.Format("{0}\\{1}_{2}_{3}{4}", LocalDocsPath, taskID, documentID, normDocName, Path.GetExtension(sourceFileName));
		}

		/// <summary>
		/// �������� ������ ���������� ��������� �������������� ����������
		/// </summary>
		/// <param name="operationObj">������-��������</param>
		/// <param name="activeTask">�������� ������</param>
		/// <param name="taskID">ID ������</param>
		/// <param name="documentID">ID ���������</param>
		/// <param name="documentName">�������� ���������</param>
		/// <param name="sourceFileName">�������� ��������� �����</param>
		/// <returns></returns>
		public string CheckLocalTaskDocument(Operation operationObj, TaskStub activeTask, int taskID, int documentID, string documentName, string sourceFileName)
		{
			string localDocumentName = GetLocalDocumentName(taskID, documentID, documentName, sourceFileName);
			FileStream fs = null;
			byte[] fileData;
			// ����-�� ����� ���� � ��������� ����?
			if (!File.Exists(localDocumentName))
			{
                if (operationObj != null)
				    operationObj.Text = "�������� ���������";
				// ���� ��� - �������� � �������

                fileData = activeTask.GetDocumentData(documentID);
                if (fileData.Length == 0)
                    return String.Empty;
				try
				{
					fs = new FileStream(localDocumentName, FileMode.CreateNew, FileAccess.Write);
					fs.Write(fileData, 0, fileData.Length);
				}
				finally
				{
                    if (fs != null)
                    {
                        fs.Flush();
                        fs.Close();
                    }
				}
			}
			else
			{
				// ���� ��������� � ������ �������������� - ������ �� ���������
				if (!activeTask.InEdit)
				{
                    if (operationObj != null)
					    operationObj.Text = "���������� ���������� ������";
					// ���� ���� - ���������� ����������� ����� ���������� ����� � ����� �� �������
					ulong serverFileCRC32 = activeTask.GetDocumentCRC32(documentID);
					ulong localFileCRC32 = 0;
					if (File.Exists(localDocumentName))
						localFileCRC32 = CRCHelper.CRC32(localDocumentName);
					// ���� �� ��������� - ��������� ��������� ������ �����
					if ((serverFileCRC32 != 0) && (serverFileCRC32 != localFileCRC32))
					{
                        // ���� ���� ����� - �������� ��� �� ���������
                        if (!FileHelper.FileIsVacant(localDocumentName))
                        {
                            if (operationObj != null)
                                operationObj.StopOperation();
                            string errStr = String.Format("���������� ���������� ���������� '{0}'. ���� ������������ ������ ���������.", localDocumentName);
                            MessageBox.Show(errStr, "������ ���������� �����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            if (operationObj != null)
                                operationObj.Text = "���������� ���������";

                            fileData = activeTask.GetDocumentData(documentID);
                            try
                            {
                                fs = new FileStream(localDocumentName, FileMode.OpenOrCreate, FileAccess.Write);
                                fs.SetLength(fileData.Length);
                                fs.Write(fileData, 0, fileData.Length);
                            }
                            finally
                            {
                                if (fs != null)
                                {
                                    fs.Flush();
                                    fs.Close();
                                }
                            }
                        }
					}
				}
			}
			return localDocumentName;
		}

        /// <summary>
        /// ������� ��������
        /// </summary>
        /// <param name="operationObj">������-��������</param>
        /// <param name="activeTask">�������� ������</param>
        /// <param name="activeScheme">�������� �����</param>
        /// <param name="documentID"></param>
        /// <param name="documentName"></param>
        /// <param name="sourceFileName"></param>
        /// <param name="docType"></param>
        /// <param name="showPromp"></param>
		public void OpenTaskDocument(Operation operationObj, TaskStub activeTask, 
            IScheme activeScheme, int documentID, string documentName, string sourceFileName,
            int docType, bool showPromp)
		{
			// ���� �������� ������ ��������� �� � ������ �������������� - ���������� ������������, ����� �� ��
			// ������� �������� ������ ��� ������
			if ((showPromp) && ((!activeTask.InEdit) &&
				(MessageBox.Show("������ �� ��������� � ������ ��������������. ������� �������� ������ ��� ������?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)))
				return;
			operationObj.Text = "����� ���������";
			operationObj.StartOperation();
			try
			{
				// �������� ���������
				int taskID = activeTask.ID;

                string localDocumentName = CheckLocalTaskDocument(operationObj, activeTask, 
					taskID, documentID, documentName, sourceFileName);

                switch ((TaskDocumentType)docType)
                {
                    case TaskDocumentType.dtMDXExpertDocument:
                        MdxExpertHelper.OpenDocument(
                            localDocumentName, 
                            activeScheme.SchemeMDStore.ServerName, 
                            activeScheme.SchemeMDStore.CatalogName);
                        break;
                    //case TaskDocumentType.dtArbitraryDocument:
                    case TaskDocumentType.dtCalcSheet:
                    case TaskDocumentType.dtDataCaptureList: 
                    case TaskDocumentType.dtExcelDocument:
                    case TaskDocumentType.dtInputForm:
                    case TaskDocumentType.dtPlanningSheet:
                    case TaskDocumentType.dtReport:
                        TaskDocumentHelper.OfficeMessageDelegate messageDelegate;
                        if (showPromp)
                            messageDelegate = OfficeMessageHandler;
                        else
                            messageDelegate = OfficeMessageSilentHandler;

                        new TaskDocumentHelper().OpenDocumentFromTask(
                            localDocumentName,
                            true,
                            false,
                            activeTask.InEdit,
                            activeScheme,
                            activeTask.Headline,
                            activeTask.ID.ToString(),
                            documentName, 
                            documentID.ToString(),
                            activeScheme.UsersManager.GetUserNameByID(activeTask.Doer),
                            docType, 
                            activeTask,
                            messageDelegate
                        );
                        break;
                    default:
                        System.Diagnostics.Process.Start(localDocumentName);
                        break;
                }
			}
			finally
			{
				operationObj.StopOperation();
			}
        }

	    private void OfficeMessageHandler(string message)
	    {
	        MessageBox.Show(message, "��������������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
	    }

        private void OfficeMessageSilentHandler(string message)
        {
        }

        #region ������� ������ � ����������� 

        #region ���������� ���������� �� ����

        /// <summary>
        /// �������� ������ ���������� ��������� �������������� ����������
        /// </summary>
        /// <param name="activeTask">�������� ������</param>
        /// <param name="taskID">ID ������</param>
        /// <param name="documentID">ID ���������</param>
        /// <param name="documentName">�������� ���������</param>
        /// <param name="sourceFileName">�������� ��������� �����</param>
        /// <returns></returns>
        private string CheckLocalTaskDocument(ITask activeTask, int taskID, int documentID, string documentName, string sourceFileName)
        {
            string localDocumentName = GetLocalDocumentName(taskID, documentID, documentName, sourceFileName);
            FileStream fs = null;
            byte[] fileData;
            // ����-�� ����� ���� � ��������� ����?
            if (!File.Exists(localDocumentName))
            {
                fileData = activeTask.GetDocumentData(documentID);
                if (fileData.Length == 0)
                    return String.Empty;
                try
                {
                    fs = new FileStream(localDocumentName, FileMode.CreateNew, FileAccess.Write);
                    fs.Write(fileData, 0, fileData.Length);
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Flush();
                        fs.Close();
                    }
                }
            }
            else
            {
                // ���� ��������� � ������ �������������� - ������ �� ���������
                if (!activeTask.InEdit)
                {
                    // ���� ���� - ���������� ����������� ����� ���������� ����� � ����� �� �������
                    ulong serverFileCRC32 = activeTask.GetDocumentCRC32(documentID);
                    ulong localFileCRC32 = 0;
                    if (File.Exists(localDocumentName))
                        localFileCRC32 = CRCHelper.CRC32(localDocumentName);
                    // ���� �� ��������� - ��������� ��������� ������ �����
                    if ((serverFileCRC32 != 0) && (serverFileCRC32 != localFileCRC32))
                    {
                        // ���� ���� ����� - �������� ��� �� ���������
                        if (FileHelper.FileIsVacant(localDocumentName))
                        {
                            fileData = activeTask.GetDocumentData(documentID);
                            try
                            {
                                fs = new FileStream(localDocumentName, FileMode.OpenOrCreate, FileAccess.Write);
                                fs.Write(fileData, 0, fileData.Length);
                            }
                            finally
                            {
                                if (fs != null)
                                {
                                    fs.Flush();
                                    fs.Close();
                                }
                            }
                        }
                    }
                }
            }
            return localDocumentName;
        }

        public string InternalSaveTaskDocument(ITask activeTask, int documentID, string documentName, string sourceFileName)
        {
            int taskID = activeTask.ID;
            string localDocumentName = CheckLocalTaskDocument(activeTask, taskID, documentID, documentName, sourceFileName);
            //string newFileName = String.Concat(dirName, "\\", Path.GetFileName(localDocumentName));

            // ������ ��������� ��� ������� ���� �� ��������� ��������
            byte[] fileData = FileHelper.ReadFileData(localDocumentName);
            FileStream fs = new FileStream(localDocumentName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            fs.Write(fileData, 0, fileData.Length);
            fs.Flush();
            fs.Close();
            fileData = null;

            // ���� ��� �������� ������� - ������������� ��� ��������
            TaskDocumentType dt = ResolveDocumentType(localDocumentName);
            if (TaskDocumentHelper.IsPlanningDocument(dt))
            {
                using (OfficeApplication officeApp = OfficeHelper.GetOfficeAppForFile(localDocumentName))
                {
                    officeApp.OpenFile(localDocumentName, false, false);
                    officeApp.SaveChanges(null, localDocumentName);
                    officeApp.Quit();
                }
            }
            return localDocumentName;
        }

        public string GetDocumentName(ITask task, DataRow documentRow)
        {
            return string.Empty;
        }

        #endregion

        #region ���������� ���������� � ����

        /// <summary>
        /// ���������������� ��������� �������� � ���������
        /// </summary>
        /// <param name="activeTask">�������� ������</param>
        /// <param name="row">������ � ����������� ���������</param>
        public void CheckDocumentTypeAndCRC(ITask activeTask, DataRow row)
        {
            // �������� ��������� ��������� ���������
            int taskID = activeTask.ID;
            int documentID = Convert.ToInt32(row["ID"]);
            string documentName = Convert.ToString(row["Name"]);
            string sourceFileName = Convert.ToString(row["SourceFileName"]);
            string sourceFileExt = Path.GetExtension(sourceFileName).ToLower();
            string localDocumentName = GetLocalDocumentName(taskID, documentID,
                documentName, sourceFileName);

            if (File.Exists(localDocumentName))
            {
                // ���� ��� �������� ����� � ������ �������� - ��������� �� ��������� �� ��� ���������
                if ((sourceFileExt == ".xls") || (sourceFileExt == ".doc"))
                {
                    TaskDocumentType oldType = (TaskDocumentType)Convert.ToInt32(row["DocumentType"]);
                    TaskDocumentType newType = ResolveDocumentType(localDocumentName);
                    if (oldType != newType)
                        row["DocumentType"] = (int)newType;
                }

                // ������ ��������� �� ���������� �� ���������� �����
                ulong localFileCRC32;
                byte[] fileData = FileHelper.ReadFileData(localDocumentName);
                // ��������� CRC ��������� ������
                localFileCRC32 = CRCHelper.CRC32(fileData, 0, fileData.Length);
                // ��������� CRC ��������� ������
                ulong serverFileCRC32 = activeTask.GetDocumentCRC32(documentID);
                // ���� CRC ���������� - ��������� ��������� ������
                if (localFileCRC32 != serverFileCRC32)
                {
                    activeTask.SetDocumentData(documentID, fileData);
                }
                GC.GetTotalMemory(true);
            }
        }

        #endregion

        #endregion
    }
}