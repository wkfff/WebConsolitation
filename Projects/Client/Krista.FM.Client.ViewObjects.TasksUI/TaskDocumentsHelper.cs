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
	/// Вспомогательный класс для работы с документами задач
	/// </summary>
	internal class DocumentsHelper
	{
		/// <summary>
		/// Название папки документов
		/// </summary>
		private const string DocsFolderName = "TasksDocuments";

		/// <summary>
		/// Путь к локальной папке документов
		/// </summary>
		public static string LocalDocsPath = String.Empty;

        static DocumentsHelper()
        {
            // проверяем наличие папки локальных документов (если нет - создаем)
            LocalDocsPath = Assembly.GetExecutingAssembly().Location;
            LocalDocsPath = Path.GetDirectoryName(LocalDocsPath);
            LocalDocsPath = LocalDocsPath + Path.DirectorySeparatorChar + DocsFolderName;
            // если не существует - создаем
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
            // если удалось - приводим тип к нашему
            if (!string.IsNullOrEmpty(docTypeStr) && (docTypeStr != "null"))
                try
                {   // это свойство возвращает индекс выпадающего списка 
                    dt = (TaskDocumentType)Convert.ToInt32(docTypeStr);
                }
                catch
                {
                }
        }

        /// <summary>
		/// Определить тип документа по имени файла
		/// </summary>
		/// <param name="fileName">имя файла</param>
		/// <returns>тип документа</returns>
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
				// файлы Excel, в свою очередь, могут являться документами планирования
				case ".xls":
					dt = TaskDocumentType.dtExcelDocument;
					break;
                case ".docx":
                    //  если офис-по-умолчанию старый, то не обрабатываем им документы нового
                    if (OfficeHelper.GetWordVersionNumber() < 12)
                        break;
                    dt = TaskDocumentType.dtWordDocument;
                    break;
                case ".xlsx":
                    //  если офис-по-умолчанию старый, то не обрабатываем им документы нового
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
		/// Синхронизировать локальный документ с серверным
		/// </summary>
		/// <param name="progressObj">объект-прогресс</param>
		/// <param name="activeTask">активная задача</param>
		/// <param name="row">строка с параметрами документа</param>
		public void CheckDocumentTypeAndCRC(Progress progressObj, ITask activeTask, DataRow row)
		{
			// получаем различные параметры документа
            string fmtStr = "{0} ({1} КБ)";
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

				// если это документ Ексел и плагин доступен - проверяем не изменился ли тип документа
                if ((sourceFileExt == ".xls") || (sourceFileExt == ".doc"))
				{
					TaskDocumentType oldType = (TaskDocumentType)Convert.ToInt32(row["DocumentType"]);
					TaskDocumentType newType = ResolveDocumentType(localDocumentName);
					if (oldType != newType)
						row["DocumentType"] = (int)newType;
				}

				// теперь проверяем не изменилось ли содержимое файла
				ulong localFileCRC32;
                byte[] fileData = FileHelper.ReadFileData(localDocumentName);
                progressObj.Text = String.Format(fmtStr, documentName, fileData.Length / 1024);
                // вычисляем CRC локальной версии
                localFileCRC32 = CRCHelper.CRC32(fileData, 0, fileData.Length);
				// вычисляем CRC серверной версии
				ulong serverFileCRC32 = activeTask.GetDocumentCRC32(documentID);
				// если CRC изменилось - обновляем серверную версию
				if (localFileCRC32 != serverFileCRC32)
				{
                    activeTask.SetDocumentData(documentID, fileData);
				}
                GC.GetTotalMemory(true);
			}
		}

		/// <summary>
		/// Сформировать полное название локального документа
		/// </summary>
		/// <param name="taskID">ID задачи</param>
		/// <param name="documentID">ID документа</param>
		/// <param name="documentName">название документа</param>
		/// <param name="sourceFileName">название исходного файла (оттуда берется расширение)</param>
		/// <returns>полный путь к локальному документу</returns>
		public static string GetLocalDocumentName(int taskID, int documentID, string documentName, string sourceFileName)
		{
			// заменяем недопустимые в именах файлов символы на подчеркивание
			char[] invalidChars = Path.GetInvalidFileNameChars();
			string normDocName = documentName; 
			foreach (char chr in invalidChars)
			{
				if (normDocName.IndexOf(chr) != -1)
					normDocName = normDocName.Replace(chr, '_');
			}
			// формируем имя файла
			return String.Format("{0}\\{1}_{2}_{3}{4}", LocalDocsPath, taskID, documentID, normDocName, Path.GetExtension(sourceFileName));
		}

		/// <summary>
		/// Проверка версии локального документа относительного серверного
		/// </summary>
		/// <param name="operationObj">объект-операция</param>
		/// <param name="activeTask">активная задача</param>
		/// <param name="taskID">ID задачи</param>
		/// <param name="documentID">ID документа</param>
		/// <param name="documentName">название документа</param>
		/// <param name="sourceFileName">название исходного файла</param>
		/// <returns></returns>
		public string CheckLocalTaskDocument(Operation operationObj, TaskStub activeTask, int taskID, int documentID, string documentName, string sourceFileName)
		{
			string localDocumentName = GetLocalDocumentName(taskID, documentID, documentName, sourceFileName);
			FileStream fs = null;
			byte[] fileData;
			// есть-ли такой файл в локальном кэше?
			if (!File.Exists(localDocumentName))
			{
                if (operationObj != null)
				    operationObj.Text = "Загрузка документа";
				// если нет - копируем с сервера

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
				// если находимся в режиме редактирования - ничего не проверяем
				if (!activeTask.InEdit)
				{
                    if (operationObj != null)
					    operationObj.Text = "Сохранение документов задачи";
					// если есть - сравниваем контрольную сумму локального файла и файла на сервере
					ulong serverFileCRC32 = activeTask.GetDocumentCRC32(documentID);
					ulong localFileCRC32 = 0;
					if (File.Exists(localDocumentName))
						localFileCRC32 = CRCHelper.CRC32(localDocumentName);
					// если не совпадают - обновляем локальную версию файла
					if ((serverFileCRC32 != 0) && (serverFileCRC32 != localFileCRC32))
					{
                        // если файл занят - обновить его не получится
                        if (!FileHelper.FileIsVacant(localDocumentName))
                        {
                            if (operationObj != null)
                                operationObj.StopOperation();
                            string errStr = String.Format("Невозможно произвести обновление '{0}'. Файл используется другим процессом.", localDocumentName);
                            MessageBox.Show(errStr, "Ошибка обновления файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            if (operationObj != null)
                                operationObj.Text = "Обновление документа";

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
        /// Открыть документ
        /// </summary>
        /// <param name="operationObj">Объект-операция</param>
        /// <param name="activeTask">активная задача</param>
        /// <param name="activeScheme">активная схема</param>
        /// <param name="documentID"></param>
        /// <param name="documentName"></param>
        /// <param name="sourceFileName"></param>
        /// <param name="docType"></param>
        /// <param name="showPromp"></param>
		public void OpenTaskDocument(Operation operationObj, TaskStub activeTask, 
            IScheme activeScheme, int documentID, string documentName, string sourceFileName,
            int docType, bool showPromp)
		{
			// если активная задача находится не в режиме редактирования - спрашиваем пользователя, хочет ли он
			// открыть документ только для чтения
			if ((showPromp) && ((!activeTask.InEdit) &&
				(MessageBox.Show("Задача не находится в режиме редактирования. Открыть документ только для чтения?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)))
				return;
			operationObj.Text = "Поиск документа";
			operationObj.StartOperation();
			try
			{
				// получаем параметры
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
	        MessageBox.Show(message, "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
	    }

        private void OfficeMessageSilentHandler(string message)
        {
        }

        #region скрытая работа с документами 

        #region Сохранение документов на диск

        /// <summary>
        /// Проверка версии локального документа относительного серверного
        /// </summary>
        /// <param name="activeTask">активная задача</param>
        /// <param name="taskID">ID задачи</param>
        /// <param name="documentID">ID документа</param>
        /// <param name="documentName">название документа</param>
        /// <param name="sourceFileName">название исходного файла</param>
        /// <returns></returns>
        private string CheckLocalTaskDocument(ITask activeTask, int taskID, int documentID, string documentName, string sourceFileName)
        {
            string localDocumentName = GetLocalDocumentName(taskID, documentID, documentName, sourceFileName);
            FileStream fs = null;
            byte[] fileData;
            // есть-ли такой файл в локальном кэше?
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
                // если находимся в режиме редактирования - ничего не проверяем
                if (!activeTask.InEdit)
                {
                    // если есть - сравниваем контрольную сумму локального файла и файла на сервере
                    ulong serverFileCRC32 = activeTask.GetDocumentCRC32(documentID);
                    ulong localFileCRC32 = 0;
                    if (File.Exists(localDocumentName))
                        localFileCRC32 = CRCHelper.CRC32(localDocumentName);
                    // если не совпадают - обновляем локальную версию файла
                    if ((serverFileCRC32 != 0) && (serverFileCRC32 != localFileCRC32))
                    {
                        // если файл занят - обновить его не получится
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

            // всегда сохраняем или создаем файл во временном каталоге
            byte[] fileData = FileHelper.ReadFileData(localDocumentName);
            FileStream fs = new FileStream(localDocumentName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            fs.Write(fileData, 0, fileData.Length);
            fs.Flush();
            fs.Close();
            fileData = null;

            // если это документ плагина - устанавливаем ему свойства
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

        #region сохранение документов в базу

        /// <summary>
        /// Синхронизировать локальный документ с серверным
        /// </summary>
        /// <param name="activeTask">активная задача</param>
        /// <param name="row">строка с параметрами документа</param>
        public void CheckDocumentTypeAndCRC(ITask activeTask, DataRow row)
        {
            // получаем различные параметры документа
            int taskID = activeTask.ID;
            int documentID = Convert.ToInt32(row["ID"]);
            string documentName = Convert.ToString(row["Name"]);
            string sourceFileName = Convert.ToString(row["SourceFileName"]);
            string sourceFileExt = Path.GetExtension(sourceFileName).ToLower();
            string localDocumentName = GetLocalDocumentName(taskID, documentID,
                documentName, sourceFileName);

            if (File.Exists(localDocumentName))
            {
                // если это документ Ексел и плагин доступен - проверяем не изменился ли тип документа
                if ((sourceFileExt == ".xls") || (sourceFileExt == ".doc"))
                {
                    TaskDocumentType oldType = (TaskDocumentType)Convert.ToInt32(row["DocumentType"]);
                    TaskDocumentType newType = ResolveDocumentType(localDocumentName);
                    if (oldType != newType)
                        row["DocumentType"] = (int)newType;
                }

                // теперь проверяем не изменилось ли содержимое файла
                ulong localFileCRC32;
                byte[] fileData = FileHelper.ReadFileData(localDocumentName);
                // вычисляем CRC локальной версии
                localFileCRC32 = CRCHelper.CRC32(fileData, 0, fileData.Length);
                // вычисляем CRC серверной версии
                ulong serverFileCRC32 = activeTask.GetDocumentCRC32(documentID);
                // если CRC изменилось - обновляем серверную версию
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