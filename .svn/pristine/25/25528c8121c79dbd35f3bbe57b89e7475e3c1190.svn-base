using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Xml;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

using Krista.FM.Server.Common;

namespace Krista.FM.Server.Tasks
{
    internal static class XMLConsts
    {
        #region теги xml документа с задачами
        internal const string DocStart = "tasksExportXml";
        internal const string Version = "tasksExportXml";

        internal const string TaskTypes = "taskTypes";
        internal const string TaskType = "taskType";
        internal const string Tasks = "tasks";
        internal const string Task = "task";
        internal const string Documents = "documents";
        internal const string Document = "document";
        internal const string DocumentData = "documentData";
        internal const string Consts = "consts";
        internal const string Params = "params";
        internal const string Const = "const";
        internal const string Param = "param";
        internal const string Values = "values";
        #endregion

        internal const string ID = "ID";

        #region аттрибуты задачи
        internal const string TaskHeadline = "taskHeadline";
        internal const string TaskJob = "taskJob";
        internal const string TaskDescription = "taskDescription";
        internal const string TaskState = "taskState";
        internal const string TaskFromDate = "taskFromDate";
        internal const string TaskToDate = "taskToDate";
        internal const string TaskOwner = "owner";
        internal const string TaskDoer = "doer";
        internal const string TaskCurator = "curator";
        internal const string TaskLockByUser = "lockByUser";
        internal const string TaskLockedUserName = "lockedUserName";
        internal const string TaskParentID = "parentID";
        #endregion

        #region аттрибуты документа
        internal const string DocumentName = "documentName";
        internal const string DocumentSourceFileName = "documentSourceFileName";
        internal const string DocumentType = "documentType";
        internal const string DocumentDescription = "documentDescription";
        internal const string DocumentOwnership = "documentOwnership";
        internal const string DocumentCrc = "crc";
        internal const string DocumentSize = "size";
        #endregion

        #region аттрибуты типа задачи
        internal const string TaskTypesName = "taskTypesName";
        internal const string TaskTypesCode = "taskTypesCode";
        internal const string TaskTypesTaskType = "taskTypesTaskType";
        internal const string TaskTypesDescription = "taskTypesDescription";
        #endregion

        #region
        internal const string ConstParamName = "name";
        internal const string ConstParamDescription = "description";
        internal const string ConstParamAllowMultiSelect = "allowMultiSelect";
        internal const string ConstParamDimension = "dimension";
        internal const string ConstParamCrc = "crc";
        internal const string ConstParamSize = "size";
        #endregion
    }

    #region статические классы для хранения всех данных по задаче

    internal static class TaskParams
    {
       internal static int TaskID;
       internal static string TaskHeadline;
       internal static string TaskJob;
       internal static string TaskDescription;
       internal static string TaskState;
       internal static DateTime TaskFromDate;
       internal static DateTime TaskToDate;
       internal static int TaskOwner;
       internal static int TaskDoer;
       internal static int TaskCurator;
       internal static int TaskType;
       internal static string TaskLockByUser;
       internal static string TaskLockedUserName;
       internal static int? TaskParentID;
    }

    internal static class DocumentParams
    {
        internal static string DocumentName;
        internal static string DocumentSourceFileName;
        internal static int DocumentType;
        internal static string DocumentDescription;
        internal static int DocumentOwnership;
        internal static uint DocumentCrc;
        internal static int DocumentSize;
        internal static byte[] DocumentData;
    }

    internal static class ConstParamParams
    {
        internal static string Name;
        internal static string Description;
        internal static bool AllowMultiselect = false;
        internal static string Dimension;
        internal static uint Crc;
        internal static int Size;
        internal static byte[] Data;
    }

    #endregion

    public partial class TaskCollection : RealTimeDBObjectsEnumerableCollection, ITaskCollection
    {
        public void ImportTask(Stream xmlStream, object parentTaskID)
        {
            using (_db = Scheme.SchemeDWH.DB)
            {
                _db.BeginTransaction();
                IDataUpdater taskUpdater = GetTasksImportUpdater(_db);
                IDataUpdater documentUpdater = GetDocumentsImportUpdater(_db);
                IDataUpdater constUpdater = GetParamsDataUpdater(_db);
                try
                {
                    allTaskTypes = Scheme.UsersManager.GetTasksTypes();
                    ImportTasksFromXML(xmlStream, parentTaskID, taskUpdater, documentUpdater, constUpdater);
                    _db.Commit();
                }
                catch(Exception e)
                {
                    taskUpdater.Dispose();
                    documentUpdater.Dispose();
                    constUpdater.Dispose();
                    _db.Rollback();
                    throw new Exception(e.Message, e.InnerException);
                }
            }
        }

        public void ExportTask(Stream stream, int[] tasksIdList)
        {
            using (_db = Scheme.SchemeDWH.DB)
            {
                _db.BeginTransaction();
                try
                {
                    ExportTaskToXmlStream(stream, tasksIdList);
                    _db.Commit();
                }
                catch (Exception e)
                {
                    _db.Rollback();
                    throw new Exception(e.Message, e.InnerException);
                }
            }
        }

        private DataTable allTaskTypes;

        #region копировать вставить

        private Dictionary<int, string> tmpFiles;

        /// <summary>
        /// копирование задачи
        /// </summary>
        /// <param name="tasksIdList"></param>
        public void CopyTask(int[] tasksIdList)
        {
            if (tmpFiles == null)
                tmpFiles = new Dictionary<int, string>();
            DeleteTempFile();
            // создаем временный файл, туда записываем задачу в виде xml
            // имя файла сохраняем 
            string tmpFileName = Path.GetTempFileName();
            using (FileStream stream = new FileStream(tmpFileName, FileMode.Append))
            {
                ExportTask(stream, tasksIdList);
            }
            tmpFiles.Add(Scheme.UsersManager.GetCurrentUserID(), tmpFileName);
        }

        public void PasteTask(int? parentID)
        {
            if (tmpFiles == null || tmpFiles.Count == 0)
                return;
            int currentUserID = Scheme.UsersManager.GetCurrentUserID();
            if (!tmpFiles.ContainsKey(currentUserID))
                return;
            string tmpFileName = tmpFiles[currentUserID];
            using (FileStream stream = new FileStream(tmpFileName, FileMode.Open))
            {
                ImportTask(stream, parentID);
            }
        }

        public void DeleteTempFile()
        {
            int currentUserID = Scheme.UsersManager.GetCurrentUserID();
            if (tmpFiles != null && tmpFiles.ContainsKey(currentUserID))
            {
                File.Delete(tmpFiles[currentUserID]);
                tmpFiles.Remove(currentUserID);
            }
        }

        #endregion

        #region Импорт задач

        private void ImportTasksFromXML(Stream xmlStream, object parentTaskID,
            IDataUpdater taskUpdater, IDataUpdater documentUpdater, IDataUpdater constUpdater)
        {
            // закэшируем список задач, доступных пользователю
            int userID = (int)Authentication.UserID;
            DataTable userTasks = GetTasksInfo(_db);
            if (_cashedTasksInfo.ContainsKey(userID))
                _cashedTasksInfo.Remove(userID);
            _cashedTasksInfo.Add(userID, userTasks);

            Dictionary<int, int> parentTasksList = new Dictionary<int, int>();
            // список с типами задач
            Dictionary<int, int> taskTypesID = new Dictionary<int, int>();
            Dictionary<int, int> parentIDDepth = new Dictionary<int, int>();
            XmlTextReader xmlReader = new XmlTextReader(xmlStream);
            DataTable dtConstsParams = GetParamsImportTable();
            object currentTaskParent = parentTaskID;
            bool isConst = false;
            int taskID = 0;

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        switch (xmlReader.LocalName)
                        {
                            case XMLConsts.Consts:
                                ImportParams(_db, dtConstsParams, currentTaskParent, constUpdater);
                                dtConstsParams.Clear();
                                break;
                            case XMLConsts.Params:
                                ImportParams(_db, dtConstsParams, currentTaskParent, constUpdater);
                                dtConstsParams.Clear();
                                break;
                        }
                        break;
                    case XmlNodeType.Element:
                        switch (xmlReader.LocalName)
                        {
                            case XMLConsts.TaskType:
                                // сохраняем в некоторой коллекции очередной найденный тип задач
                                ReadTaskType(xmlReader, ref taskTypesID);
                                break;
                            case XMLConsts.TaskTypes:
                                // сохраняем типы задач, синхронизируя их с теми, что есть в базе на данный момент
                                break;
                            case XMLConsts.Document:
                                // сохраняем параметры документа, название и т.п.
                                ReadDocumentAttributes(xmlReader);
                                break;
                            case XMLConsts.Task:
                                // у задачи получаем теги с параметрами задачи
                                // уровень вложенности задачи (для xml старого образца)
                                int taskDepth = xmlReader.Depth;
                                GetTaskAttributes(xmlReader);
                                // сохраняем задачу
                                // для xml нового образца id родительской задачи берем непосредственно в параметрах xml
                                // для xml старого образца id родительской задачи берем id задачи уровнем выше
                                // исходя из уровня вложенности задачи
                                taskID = taskDepth == 2
                                    ? ImportTask(taskTypesID, parentTasksList, parentTaskID, taskUpdater, ref userTasks, ref currentTaskParent)
                                    : ImportTask(taskTypesID, parentIDDepth[taskDepth - 2], taskUpdater, ref userTasks);
                                // id задачи более высокого уровня для контроля параметров и констант
                                currentTaskParent = taskDepth == 2 ? currentTaskParent : parentIDDepth[taskDepth - 2];
                                parentTasksList.Add(TaskParams.TaskID, taskID);
                                if (parentIDDepth.ContainsKey(taskDepth))
                                    parentIDDepth.Remove(taskDepth);
                                parentIDDepth.Add(taskDepth, taskID);

                                break;
                            case XMLConsts.DocumentData:
                                // сохраняем данные файла, который содержится в разделе CData
                                ReadDocumentDataAttributes(xmlReader);
                                // сохраняем документ в базу со ссылкой на задачу
                                ImportTaskDocument(taskID, documentUpdater);
                                break;
                            case XMLConsts.Param:
                                isConst = false;
                                ReadTaskConstParam(xmlReader);
                                break;
                            case XMLConsts.Const:
                                isConst = true;
                                ReadTaskConstParam(xmlReader);
                                break;
                            case XMLConsts.Values:
                                ReadTaskConstParamData(xmlReader);
                                ImportTaskConstParameter(isConst, ref dtConstsParams, taskID);
                                break;
                        }
                        break;
                }
            }
        }

        #region сохранение данных по задаче

        private const string tasksQuery =
            "select ID, State, FromDate, ToDate, Doer, Owner, Curator, HeadLine, RefTasks, RefTasksTypes, LockByUser, Job, Description from tasks where 1 = 2";

        /// <summary>
        /// получение атрибута объекта схемы из XML
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        internal static void GetTaskAttributes(XmlTextReader reader)
        {
            reader.MoveToAttribute(XMLConsts.ID);
            TaskParams.TaskID = Convert.ToInt32(reader.Value);
            reader.MoveToAttribute(XMLConsts.TaskHeadline);
            TaskParams.TaskHeadline = reader.Value;
            reader.MoveToAttribute(XMLConsts.TaskJob);
            TaskParams.TaskJob = reader.Value;
            reader.MoveToAttribute(XMLConsts.TaskDescription);
            TaskParams.TaskDescription = reader.Value;
            reader.MoveToAttribute(XMLConsts.TaskState);
            TaskParams.TaskState = reader.Value;
            reader.MoveToAttribute(XMLConsts.TaskFromDate);
            TaskParams.TaskFromDate = Convert.ToDateTime(reader.Value);
            reader.MoveToAttribute(XMLConsts.TaskToDate);
            TaskParams.TaskToDate = Convert.ToDateTime(reader.Value);
            reader.MoveToAttribute(XMLConsts.TaskOwner);
            TaskParams.TaskOwner = Convert.ToInt32(reader.Value);
            reader.MoveToAttribute(XMLConsts.TaskDoer);
            TaskParams.TaskDoer = Convert.ToInt32(reader.Value);
            reader.MoveToAttribute(XMLConsts.TaskCurator);
            TaskParams.TaskCurator = Convert.ToInt32(reader.Value);
            reader.MoveToAttribute(XMLConsts.TaskType);
            TaskParams.TaskType = Convert.ToInt32(reader.Value);
            reader.MoveToAttribute(XMLConsts.TaskLockByUser);
            TaskParams.TaskLockByUser = reader.Value;
            reader.MoveToAttribute(XMLConsts.TaskLockedUserName);
            TaskParams.TaskLockedUserName = reader.Value;
            if (reader.MoveToAttribute(XMLConsts.TaskParentID))
                TaskParams.TaskParentID = string.IsNullOrEmpty(reader.Value) ? (int?)null : Convert.ToInt32(reader.Value);
            else
                TaskParams.TaskParentID = null;
        }

        /// <summary>
        /// Сохраняем задачу, с параметрами, которые мы загрузили
        /// </summary>
        /// <returns></returns>
        private int ImportTask(Dictionary<int, int> taskTypesID, Dictionary<int, int> parentTaskList,
            object parentTaskId, IDataUpdater taskUpdater, ref DataTable dtTasks, ref object currentParentTaskId)
        {
            //DataTable dtNewTasks = (DataTable)_db.ExecQuery(tasksQuery, QueryResultTypes.DataTable);
            DataRow newTaskRow = dtTasks.NewRow();
            int newTaskID = _db.GetGenerator("G_TASKS");
            newTaskRow.BeginEdit();
            newTaskRow[0] = newTaskID;
            newTaskRow["State"] = "Создана";
            newTaskRow["FromDate"] = TaskParams.TaskFromDate;
            newTaskRow["ToDate"] = TaskParams.TaskToDate;
            newTaskRow["Doer"] = Scheme.UsersManager.GetCurrentUserID();
            newTaskRow["Owner"] = Scheme.UsersManager.GetCurrentUserID();
            newTaskRow["Curator"] = Scheme.UsersManager.GetCurrentUserID();
            newTaskRow["Job"] = TaskParams.TaskJob;
            newTaskRow["Description"] = TaskParams.TaskDescription;
            newTaskRow["HeadLine"] = TaskParams.TaskHeadline;
            // заполним это поле позже
            if (TaskParams.TaskParentID != null && parentTaskList.ContainsKey((int)TaskParams.TaskParentID))
                newTaskRow["Reftasks"] = parentTaskList[(int)TaskParams.TaskParentID];
            else
                newTaskRow["Reftasks"] = parentTaskId ?? DBNull.Value;
            currentParentTaskId = newTaskRow["Reftasks"];
            newTaskRow["RefTasksTypes"] = taskTypesID[TaskParams.TaskType];
            newTaskRow[TasksNavigationTableColumns.VISIBLE_COLUMN] = 0;
            newTaskRow.EndEdit();
            dtTasks.Rows.Add(newTaskRow);
            UpdateTable(dtTasks.GetChanges(), taskUpdater);
            //ImportTable(dtTasks.GetChanges(), taskUpdater);//GetTasksImportUpdater(_db));
            dtTasks.AcceptChanges();
            return newTaskID;
        }

        /// <summary>
        /// Сохраняем задачу, с параметрами, которые мы загрузили
        /// </summary>
        /// <returns></returns>
        private int ImportTask(Dictionary<int, int> taskTypesID, int parentTaskID, IDataUpdater taskUpdater, ref DataTable dtTasks)
        {
            //DataTable dtNewTasks = (DataTable)_db.ExecQuery(tasksQuery, QueryResultTypes.DataTable);
            DataRow newTaskRow = dtTasks.NewRow();
            int newTaskID = _db.GetGenerator("G_TASKS");
            newTaskRow.BeginEdit();
            newTaskRow[0] = newTaskID;
            newTaskRow["State"] = "Создана";
            newTaskRow["FromDate"] = TaskParams.TaskFromDate;
            newTaskRow["ToDate"] = TaskParams.TaskToDate;
            newTaskRow["Doer"] = Scheme.UsersManager.GetCurrentUserID();
            newTaskRow["Owner"] = Scheme.UsersManager.GetCurrentUserID();
            newTaskRow["Curator"] = Scheme.UsersManager.GetCurrentUserID();
            newTaskRow["Job"] = TaskParams.TaskJob;
            newTaskRow["Description"] = TaskParams.TaskDescription;
            newTaskRow["HeadLine"] = TaskParams.TaskHeadline;
            // заполним это поле позже
            newTaskRow["Reftasks"] = parentTaskID;
            newTaskRow["RefTasksTypes"] = taskTypesID[TaskParams.TaskType];
            newTaskRow[TasksNavigationTableColumns.VISIBLE_COLUMN] = 0;
            newTaskRow.EndEdit();
            dtTasks.Rows.Add(newTaskRow);
            UpdateTable(dtTasks.GetChanges(), taskUpdater);// GetTasksImportUpdater(_db));
            dtTasks.AcceptChanges();
            return newTaskID;
        }

        private void UpdateTable(DataTable changes, IDataUpdater upd)
        {
            if ((changes == null) || (changes.Rows.Count == 0))
                return;
            upd.Update(ref changes);
        }

        #endregion

        #region сохранение документов

        private static void ReadDocumentAttributes(XmlTextReader reader)
        {
            reader.MoveToAttribute(XMLConsts.DocumentName);
            DocumentParams.DocumentName = reader.Value;
            reader.MoveToAttribute(XMLConsts.DocumentSourceFileName);
            DocumentParams.DocumentSourceFileName = reader.Value;
            reader.MoveToAttribute(XMLConsts.DocumentType);
            DocumentParams.DocumentType = Convert.ToInt32(reader.Value);
            reader.MoveToAttribute(XMLConsts.DocumentDescription);
            DocumentParams.DocumentDescription = reader.Value;
            reader.MoveToAttribute(XMLConsts.DocumentOwnership);
            DocumentParams.DocumentOwnership = Convert.ToInt32(reader.Value);
        }

        private void ReadDocumentDataAttributes(XmlTextReader reader)
        {
            reader.MoveToAttribute(XMLConsts.DocumentCrc);
            DocumentParams.DocumentCrc = Convert.ToUInt32(reader.Value);
            reader.MoveToAttribute(XMLConsts.DocumentSize);
            DocumentParams.DocumentSize = Convert.ToInt32(reader.Value);
            // читаем данные самого документа
            DocumentParams.DocumentData = GetUncompressedData(DocumentParams.DocumentSize, DocumentParams.DocumentCrc, Convert.FromBase64String(reader.ReadElementString()), true);
        }

        private void ImportTaskDocument(int taskID, IDataUpdater documentUpdater)
        {
            int documentID = _db.GetGenerator("G_DOCUMENTS");
            DataTable dtSocuments = GetDocumentsImportTable();
            DataRow newDocument = dtSocuments.NewRow();
            newDocument.BeginEdit();
            newDocument["ID"] = documentID;
            newDocument["DocumentType"] = DocumentParams.DocumentType;
            newDocument["Name"] = DocumentParams.DocumentName;
            newDocument["SourceFileName"] = DocumentParams.DocumentSourceFileName;
            newDocument["Version"] = 0;
            newDocument["RefTasks"] = taskID;
            newDocument["Description"] = DocumentParams.DocumentDescription;
            newDocument["Ownership"] = 0;
            newDocument.EndEdit();
            dtSocuments.Rows.Add(newDocument);
            // сохраняем в базу
            UpdateTable(dtSocuments.GetChanges(), documentUpdater);
            //ImportTable(dtSocuments, documentUpdater);// GetDocumentsImportUpdater(_db));
            SetDocumentData(documentID, DocumentParams.DocumentData, false, _db);
        }

        #endregion

        #region сохранение типов задач

        private void ReadTaskType(XmlTextReader reader, ref Dictionary<int, int> taskTypesID)
        {
            reader.MoveToAttribute(XMLConsts.ID);
            int taskTypeID = Convert.ToInt32(reader.Value);
            reader.MoveToAttribute(XMLConsts.TaskTypesName);
            string taskTypeName = reader.Value;
            reader.MoveToAttribute(XMLConsts.TaskTypesCode);
            int taskTypeCode = Convert.ToInt32(reader.Value);
            reader.MoveToAttribute(XMLConsts.TaskTypesTaskType);
            int taskType = Convert.ToInt32(reader.Value);
            reader.MoveToAttribute(XMLConsts.TaskTypesDescription);
            string taskTypeDescription = reader.Value;
            ImportTaskTypes(taskTypeID, taskType, taskTypeCode, taskTypeName, taskTypeDescription, ref taskTypesID);
        }

        /// <summary>
        /// получение и сохранение списка типов задач
        /// </summary>
        /// <param name="taskTypeID"></param>
        /// <param name="taskTypeTaskType"></param>
        /// <param name="taskTypeCode"></param>
        /// <param name="taskTypeName"></param>
        /// <param name="taskTypeDescription"></param>
        /// <param name="taskTypesID"></param>
        private void ImportTaskTypes(int taskTypeID, int taskTypeTaskType, int taskTypeCode, string taskTypeName,
            string taskTypeDescription, ref Dictionary<int, int> taskTypesID)
        {
            if (taskTypeName == String.Empty)
                throw new Exception(String.Format("Не найдено название для типа задачи (TasksTypes.ID = {0})", taskTypeID));
            string filter = String.Format("(Name like '{0}') and (TaskType = {1})", taskTypeName, taskTypeTaskType);
            DataRow[] filteredTasksTypes = allTaskTypes.Select(filter);
            if (filteredTasksTypes.Length == 0)
            {
                // если типа нет - получаем новое ID
                int newID = _db.GetGenerator("G_TASKSTYPES");
                // добавляем
                DataRow newRow = allTaskTypes.Rows.Add();
                newRow["ID"] = newID;
                newRow["Code"] = taskTypeCode;
                newRow["Name"] = taskTypeName;
                newRow["TaskType"] = taskTypeTaskType;
                newRow["Description"] = taskTypeDescription;
                // запоминаем ID
                if (!taskTypesID.ContainsKey(taskTypeID))
                    taskTypesID.Add(taskTypeID, newID);
            }
            else
                // если есть - запоминаем ID
                if (!taskTypesID.ContainsKey(taskTypeID))
                    taskTypesID.Add(taskTypeID, Convert.ToInt32(filteredTasksTypes[0]["ID"]));
            
            // обновляем справочник задач (в транзакции)
            DataTable taskTypesChanges = allTaskTypes.GetChanges();
            if ((taskTypesChanges != null) && (taskTypesChanges.Rows.Count > 0))
            {
                Scheme.UsersManager.ApplayTasksTypesChanges(_db, taskTypesChanges);
                allTaskTypes.AcceptChanges();
            }
        }

        #endregion

        #region получение данных констант и параметров

        /// <summary>
        /// чтение основных атрибутов по константам и параметрам
        /// </summary>
        /// <param name="reader"></param>
        private void ReadTaskConstParam(XmlTextReader reader)
        {
            reader.MoveToAttribute(XMLConsts.ConstParamName);
            ConstParamParams.Name = reader.Value;
            reader.MoveToAttribute(XMLConsts.ConstParamDescription);
            ConstParamParams.Description = reader.Value;
            if (reader.MoveToAttribute(XMLConsts.ConstParamAllowMultiSelect))
                ConstParamParams.AllowMultiselect = Convert.ToBoolean(Convert.ToInt32(reader.Value));
            if (reader.MoveToAttribute(XMLConsts.ConstParamDimension))
                ConstParamParams.Dimension = reader.Value;
        }

        /// <summary>
        /// получение данных констант и параметров
        /// </summary>
        /// <param name="reader"></param>
        private void ReadTaskConstParamData(XmlTextReader reader)
        {
            reader.MoveToAttribute(XMLConsts.ConstParamCrc);
            ConstParamParams.Crc = Convert.ToUInt32(reader.Value);
            reader.MoveToAttribute(XMLConsts.ConstParamSize);
            ConstParamParams.Size = Convert.ToInt32(reader.Value);
            // сохраняем уже распакованные данные
            ConstParamParams.Data = GetUncompressedData(ConstParamParams.Size, ConstParamParams.Crc, Convert.FromBase64String(reader.ReadElementString()), true);
        }

        private static byte[] GetUncompressedData(int size, uint crc, byte[] compressedData, bool compareCrc)
        {
            MemoryStream baseStream = new MemoryStream();
            baseStream.Write(compressedData, 0, compressedData.Length);
            baseStream.Position = 0;
            // создаем архивирующий стрим и сжимаем данные документа
            DeflateStream uncompressedStream = new DeflateStream(baseStream, CompressionMode.Decompress, true);
            // преобразуем сжатые данные в строку
            byte[] uncompressedBytes = new byte[size];
            uncompressedStream.Read(uncompressedBytes, 0, uncompressedBytes.Length);
            uncompressedStream.Close();
            baseStream.Close();
            // проверяем СРС
            uint newCrc = CRCHelper.CRC32(uncompressedBytes, 0, uncompressedBytes.Length);
            if (compareCrc && newCrc != crc)
                throw new Exception("Элемент данных поврежден. Контрольные суммы не совпадают");
            return uncompressedBytes;
        }

        private void ImportTaskConstParameter(bool isConst, ref DataTable importedParamsTable, int taskID)
        {
            string prmDimension = String.Empty;
            TaskParameterType tp = TaskParameterType.taskConst;
            if (!isConst)
                tp = TaskParameterType.taskParameter;
            DataRow newPrm = importedParamsTable.NewRow();
            newPrm.BeginEdit();
            newPrm["ID"] = _db.GetGenerator("G_TASKSPARAMETERS");
            newPrm["Name"] = ConstParamParams.Name;
            newPrm["Description"] = ConstParamParams.Description;
            newPrm["ParamType"] = tp;
            newPrm["RefTasks"] = taskID;
            if (ConstParamParams.Data != null)
                newPrm["ParamValues"] = Encoding.GetEncoding(1251).GetString(ConstParamParams.Data);
            newPrm["AllowMultiSelect"] = ConstParamParams.AllowMultiselect;
            if (!isConst)
                newPrm["Dimension"] = ConstParamParams.Dimension;
            newPrm.EndEdit();
            importedParamsTable.Rows.Add(newPrm);
            // сохраняем в базе
        }

        #endregion

        #endregion

        #region экспорт задач

        private const string taskDocumentsQuery =
            "Select ID, Name, SourceFileName, DocumentType, Description, Ownership from {0} where {1} = {2} order by ID";

        private static string taskDocumentsFilter = "Select ID from {0} where {1} = {2} order by ID";
        private static string taskDocumentInfoFilter = "Select Name, SourceFileName, DocumentType, Description, Ownership  from {0} where ID = {1}";
        private static string taskDocumentDataFilter = "Select Document  from {0} where ID = {1}";
        private static string taskDocumentsMainTableName = "Documents";
        private static string taskDocumentsTempTableName = "DocumentsTemp";
        private static string taskDocumentsMainRefName = "RefTasks";
        private static string taskDocumentsTempRefName = "RefTasksTemp";

        private void ExportTaskToXmlStream(Stream stream, int[] tasksID)
        {
            // устанавливаем русскую кодировку
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.GetEncoding(1251);
            settings.CheckCharacters = false;
            settings.Indent = true;
            // начинаем писать xml документ 
            XmlWriter writer = XmlWriter.Create(stream, settings);
            writer.WriteStartDocument(true);
            writer.WriteStartElement(XMLConsts.DocStart);
            CreateXMLAttribute(writer, XMLConsts.Version, Scheme.UsersManager.ServerLibraryVersion());
            // запись задач и всего остального, в том числе типов задач
            WriteTasks(writer, tasksID);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }

        private void WriteTasks(XmlWriter writer, int[] tasksIDList)
        {
            // получаем все задачи доступные пользователю
            DataTable tasksList = BeginExport();
            try
            {
                //получаем собственно задачи, которые хотим экспортировать
                StringBuilder sb = new StringBuilder("ID in (");
                for (int i = 0; i < tasksIDList.Length; i++)
                {
                    sb.Append(tasksIDList[i]);
                    if (i < tasksIDList.Length - 1)
                        sb.Append(", ");
                }
                sb.Append(')');
                string filter = sb.ToString();
                // получаем выбранные задачи
                DataRow[] selectedTasks = tasksList.Select(filter);
                // проверяем соотвествие запрошенного количества задач фактическому
                // если количество задач разное, выходим
                if (selectedTasks.Length != tasksIDList.Length)
                    return;
                List<string> taskTypesID = new List<string>();
                foreach (DataRow row in selectedTasks)
                {
                    string taskType = Convert.ToString(row["RefTasksTypes"]);
                    if (!taskTypesID.Contains(taskType))
                        taskTypesID.Add(taskType);
                }
                // запись типов задач
                WriteTaskTypes(writer, taskTypesID.ToArray());
                // запись задач с документами и константами
                writer.WriteStartElement(XMLConsts.Tasks);
                foreach (int taskId in tasksIDList)
                {
                    foreach (DataRow row in tasksList.Select(string.Format("ID = {0}", taskId)))
                    {
                        writer.WriteStartElement(XMLConsts.Task);
                        CreateXMLAttribute(writer, XMLConsts.ID, Convert.ToString(row[0]));
                        CreateXMLAttribute(writer, XMLConsts.TaskHeadline, Convert.ToString(row["HeadLine"]));
                        CreateXMLAttribute(writer, XMLConsts.TaskJob, Convert.ToString(row["Job"]));
                        CreateXMLAttribute(writer, XMLConsts.TaskDescription, Convert.ToString(row["Description"]));
                        CreateXMLAttribute(writer, XMLConsts.TaskState, Convert.ToString(row["State"]));
                        CreateXMLAttribute(writer, XMLConsts.TaskFromDate, Convert.ToString(row["FromDate"]));
                        CreateXMLAttribute(writer, XMLConsts.TaskToDate, Convert.ToString(row["ToDate"]));
                        CreateXMLAttribute(writer, XMLConsts.TaskOwner, Convert.ToString(row["Owner"]));
                        CreateXMLAttribute(writer, XMLConsts.TaskDoer, Convert.ToString(row["Doer"]));
                        CreateXMLAttribute(writer, XMLConsts.TaskCurator, Convert.ToString(row["Curator"]));
                        CreateXMLAttribute(writer, XMLConsts.TaskType, Convert.ToString(row["RefTasksTypes"]));
                        CreateXMLAttribute(writer, XMLConsts.TaskLockByUser, Convert.ToString(row["LockByUser"]));
                        CreateXMLAttribute(writer, XMLConsts.TaskLockedUserName, Convert.ToString(row["LockedUserName"]));
                        CreateXMLAttribute(writer, XMLConsts.TaskParentID, Convert.ToString(row["RefTasks"]));
                        // запись документов задачи
                        if (row.IsNull("LockByUser"))
                            WriteTaskDocuments(writer, Convert.ToInt32(row[0]), -1);
                        else
                            WriteTaskDocuments(writer, Convert.ToInt32(row[0]), Convert.ToInt32(row["LockByUser"]));
                        // запись параметров и констант задачи
                        WriteTaskConstsParams(writer, Convert.ToInt32(row[0]));
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
            }
            finally
            {
                Scheme.TaskManager.Tasks.EndExport();
            }
        }

        private void WriteTaskTypes(XmlWriter writer, string[] taskTypesID)
        {
            List<string> usedTaskTypes = new List<string>();
            // формируем список с уникальными ID типов
            // получаем все содержимое справочника типов задач
            DataTable taskTypesTable = Scheme.UsersManager.GetTasksTypes();
            // формируем фильтр на ID по созданному ранее списку
            string usedIDs = String.Join(", ", taskTypesID);
            usedIDs = String.Format("ID in ({0})", usedIDs);
            // фильтруем справочник типов задач
            DataRow[] usedTaskTypesRows = taskTypesTable.Select(usedIDs);
            // создаем секцию и добавляем необходмую информацию по всем используемым типам
            writer.WriteStartElement(XMLConsts.TaskTypes);
            foreach (DataRow row in usedTaskTypesRows)
            {
                writer.WriteStartElement(XMLConsts.TaskType);
                CreateXMLAttribute(writer, XMLConsts.ID, Convert.ToString(row["ID"]));
                CreateXMLAttribute(writer, XMLConsts.TaskTypesName, Convert.ToString(row["Name"]));
                CreateXMLAttribute(writer, XMLConsts.TaskTypesCode, Convert.ToString(row["Code"]));
                CreateXMLAttribute(writer, XMLConsts.TaskTypesTaskType, Convert.ToString(row["TaskType"]));
                CreateXMLAttribute(writer, XMLConsts.TaskTypesDescription, Convert.ToString(row["Description"]));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void WriteTaskDocuments(XmlWriter writer, int taskID, int lockByUser)
        {
            // определяем откуда брать документы - из основной таблицы или из кэша
			bool useCash = Scheme.UsersManager.GetCurrentUserID() == lockByUser;
            // получаем документы 
            string queryStr;
            queryStr = useCash ?
                String.Format(taskDocumentsQuery, taskDocumentsTempTableName, taskDocumentsTempRefName, taskID) :
                String.Format(taskDocumentsQuery, taskDocumentsMainTableName, taskDocumentsMainRefName, taskID);
            DataTable documents = (DataTable)_db.ExecQuery(queryStr, QueryResultTypes.DataTable);
            // если документы есть - добавляем их
            if ((documents != null) && (documents.Rows.Count > 0))
            {
                // создаем родительский узел для всех документов
                writer.WriteStartElement(XMLConsts.Documents);
                // добавляем каждый документ
                foreach (DataRow row in documents.Rows)
                {
                    writer.WriteStartElement(XMLConsts.Document);
                    //int docID = Convert.ToInt32(row["ID"]);
                    //queryStr = String.Format(taskDocumentInfoFilter, useCash ? taskDocumentsTempTableName : taskDocumentsMainTableName, docID);
                    //DataTable document = (DataTable)_db.ExecQuery(queryStr, QueryResultTypes.DataTable);
                    //DataRow docRow = document.Rows[0];
                    //string docName = Convert.ToString(docRow["Name"]);
                    // основные параметры документа
                    CreateXMLAttribute(writer, XMLConsts.ID, row["ID"].ToString());
                    CreateXMLAttribute(writer, XMLConsts.DocumentName, Convert.ToString(row["Name"]));
                    CreateXMLAttribute(writer, XMLConsts.DocumentSourceFileName, Convert.ToString(row["SourceFileName"]));
                    CreateXMLAttribute(writer, XMLConsts.DocumentType, Convert.ToString(row["DocumentType"]));
                    CreateXMLAttribute(writer, XMLConsts.DocumentDescription, Convert.ToString(row["Description"]));
                    CreateXMLAttribute(writer, XMLConsts.DocumentOwnership, Convert.ToString(row["Ownership"]));
                    // дописываем данные самого документа
                    writer.WriteStartElement(XMLConsts.DocumentData);
                    queryStr = String.Format(taskDocumentDataFilter, useCash 
                        ? taskDocumentsTempTableName :
                        taskDocumentsMainTableName, row["ID"]);
                    object docData = _db.ExecQuery(queryStr, QueryResultTypes.Scalar);
                    //DataRow docDataRow = docDataTable.Rows[0];
                    // получаем данные документа
                    byte[] documentData = docData == null || docData == DBNull.Value ? new byte[0] : (byte[])docData;
                    // распаковываем данные для получения параметров атрибутов
                    uint crc32 = CRCHelper.CRC32(documentData, 0, documentData.Length);
                    CreateXMLAttribute(writer, XMLConsts.DocumentCrc, Convert.ToString(crc32));
                    CreateXMLAttribute(writer, XMLConsts.DocumentSize, Convert.ToString(documentData.Length));
                    MemoryStream baseStream = new MemoryStream();
                    // создаем архивирующий стрим и сжимаем данные документа
                    DeflateStream compressedStream = new DeflateStream(baseStream, CompressionMode.Compress, true);
                    compressedStream.Write(documentData, 0, documentData.Length);
                    compressedStream.Close();
                    // преобразуем сжатые данные в строку
                    baseStream.Position = 0;
                    // пишем цдату к узлу
                    baseStream.Close();
                    writer.WriteCData(Convert.ToBase64String(baseStream.ToArray(), Base64FormattingOptions.InsertLineBreaks));
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    documentData = null;
                }
                writer.WriteEndElement();
            }
        }

        private void WriteTaskConstsParams(XmlWriter writer, int taskID)
        {
            DataTable dtConsts = new DataTable();
            DataTable dtParams = new DataTable();
            Scheme.TaskManager.Tasks.GetTaskConstsParams(taskID, true, ref dtConsts, ref dtParams);
            // если есть константы - добавляем
            if (dtConsts.Rows.Count > 0)
            {
                writer.WriteStartElement(XMLConsts.Consts);
                foreach (DataRow row in dtConsts.Rows)
                {
                    writer.WriteStartElement(XMLConsts.Const);
                    AppendTaskConstParamToXml(writer, row, TaskParameterType.taskConst);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            // если есть параметры - добавляем
            if (dtParams.Rows.Count > 0)
            {
                writer.WriteStartElement(XMLConsts.Params);
                foreach (DataRow row in dtParams.Rows)
                {
                    writer.WriteStartElement(XMLConsts.Param);
                    AppendTaskConstParamToXml(writer, row, TaskParameterType.taskParameter);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        private void AppendTaskConstParamToXml(XmlWriter writer, DataRow row, TaskParameterType paramType)
        {
            // сначала добавим основные параметры
            if (paramType == TaskParameterType.taskConst)
            {
                CreateXMLAttribute(writer, XMLConsts.ID, Convert.ToString(row["ID"]));
                CreateXMLAttribute(writer, XMLConsts.ConstParamName, Convert.ToString(row["Name"]));
                CreateXMLAttribute(writer, XMLConsts.ConstParamDescription, Convert.ToString(row["Description"]));
            }
            else
            {
                CreateXMLAttribute(writer, XMLConsts.ID, Convert.ToString(row["ID"]));
                CreateXMLAttribute(writer, XMLConsts.ConstParamName, Convert.ToString(row["Name"]));
                CreateXMLAttribute(writer, XMLConsts.ConstParamDescription, Convert.ToString(row["Description"]));
                CreateXMLAttribute(writer, XMLConsts.ConstParamAllowMultiSelect, Convert.ToString(row["AllowMultiSelect"]));
                CreateXMLAttribute(writer, XMLConsts.ConstParamDimension, Convert.ToString(row["Dimension"]));
            }
            #region Значения большие, их надо сжимать
            // если значение пустое - ничего не делаем
            if (row.IsNull("ParamValues"))
                return;
            // получаем данные документа
            string elemDataStr = Convert.ToString(row["ParamValues"]);
            byte[] elemData = Encoding.GetEncoding(1251).GetBytes(elemDataStr);
            // CRC и Size
            // считаем црц и помещаем его в аттрибут
            uint crc32 = CRCHelper.CRC32(elemData, 0, elemData.Length);
            writer.WriteStartElement(XMLConsts.Values);
            CreateXMLAttribute(writer, XMLConsts.ConstParamCrc, Convert.ToString(crc32));
            CreateXMLAttribute(writer, XMLConsts.ConstParamSize, Convert.ToString(elemData.Length.ToString()));
            // создаем стрим для архивации
            MemoryStream baseStream = new MemoryStream();
            // создаем архивирующий стрим и сжимаем данные документа
            DeflateStream compressedStream = new DeflateStream(baseStream, CompressionMode.Compress, true);
            compressedStream.Write(elemData, 0, elemData.Length);
            compressedStream.Close();
            // преобразуем сжатые данные в строку
            baseStream.Position = 0;
            // пишем цдату к узлу
            baseStream.Close();
            writer.WriteCData(Convert.ToBase64String(baseStream.ToArray(), Base64FormattingOptions.InsertLineBreaks));
            writer.WriteEndElement();
            //GC.Collect();
            #endregion
        }

        internal static void CreateXMLAttribute(XmlWriter writer, string attributeName, string attributeValue)
        {
            writer.WriteStartAttribute(attributeName);
            writer.WriteString(attributeValue);
            writer.WriteEndAttribute();
        }

        #endregion
    }
}
