using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Runtime.CompilerServices;

using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.Users;

namespace Krista.FM.Server.Tasks
{
    public partial class TaskCollection : RealTimeDBObjectsEnumerableCollection, ITaskCollection//, ITasksExport, ITasksImport
    {
        #region Импорт задачи

        private IDatabase _db;
        public IDatabase GetTaskDB()
        {
            ClearDB();
            _db = Scheme.SchemeDWH.DB;
            return _db;
        }

        public void BeginDbTransaction()
        {
            if (_db != null)
                _db.BeginTransaction();
        }

        public void CommitDbTransaction()
        {
            if (_db != null)
                _db.Commit();
        }

        public void RollbackDbTransaction()
        {
            if (_db != null)
                _db.Rollback();
        }

        public void ClearDB()
        {
            if (_db != null)
                _db.Dispose();
            _db = null;
        }

        public void ImportTaskTypes(DataTable taskTypes)
        {
            Scheme.UsersManager.ApplayTasksTypesChanges(_db, taskTypes);
        }

        public DataTable ImportTaskTypes(int taskTypeID, int taskTypeTaskType, int taskTypeCode, string taskTypeName,
                                  string taskTypeDescription)
        {
            // создаем маленькую таблицу со старыми и новыми ID импортируемых типов задач
            DataTable importedTasksTypesIds = new DataTable("ExportedTasksTypes");
            importedTasksTypesIds.Columns.Add("ID", typeof(int));
            importedTasksTypesIds.Columns.Add("newID", typeof(int));
            // получаем все типы задач
            DataTable allTaskTypes = Scheme.UsersManager.GetTasksTypes();

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
                importedTasksTypesIds.Rows.Add(taskTypeID, newID);
            }
            else
            {
                // если есть - запоминаем ID 
                importedTasksTypesIds.Rows.Add(taskTypeID, filteredTasksTypes[0]["ID"]);
            }

            // обновляем справочник задач (в транзакции)
            DataTable taskTypesChanges = allTaskTypes.GetChanges();
            if ((taskTypesChanges != null) && (taskTypesChanges.Rows.Count > 0))
                Scheme.UsersManager.ApplayTasksTypesChanges(_db, taskTypesChanges);
            return importedTasksTypesIds;
        }

        public int ImportTask(string headline, string job, string description, DateTime usedDate, int taskTypeID, DataTable tasks, object parentTaskID)
        {
            int newID = _db.GetGenerator("G_TASKS");
            
            tasks.BeginLoadData();
            DataRow newTaskRow = tasks.Rows.Add();
            newTaskRow.BeginEdit();
            newTaskRow["ID"] = newID;
            newTaskRow["State"] = "Создана";
            newTaskRow["FromDate"] = usedDate;
            newTaskRow["ToDate"] = usedDate;
            newTaskRow["Doer"] = Scheme.UsersManager.GetCurrentUserID();
            newTaskRow["Owner"] = Scheme.UsersManager.GetCurrentUserID();
            newTaskRow["Curator"] = Scheme.UsersManager.GetCurrentUserID();
            newTaskRow["Headline"] = headline;
            newTaskRow["Job"] = job;
            newTaskRow["Description"] = description;
            newTaskRow["Reftasks"] = parentTaskID;
            newTaskRow["RefTasksTypes"] = taskTypeID;
            newTaskRow.EndEdit();
            tasks.EndLoadData();
            Scheme.TaskManager.Tasks.ImportTasks(_db, tasks.GetChanges(DataRowState.Added));
            return newID;
        }

        public void ImportTaskConstsParameters(object parentTaskID, DataTable paramsTable)
        {
            Scheme.TaskManager.Tasks.ImportParams(_db, paramsTable, parentTaskID);
        }

        public void ImportTaskDocuments(DataTable importedDocuments, Dictionary<int, byte[]> documents)
        {
            ImportDocuments(_db, importedDocuments);

            foreach (KeyValuePair<int, byte[]> kvp in documents)
            {
                Scheme.TaskManager.Tasks.SetDocumentData(kvp.Key, kvp.Value, false, _db);
            }
        }

        public void ImportTaskConstParameter(string xmlNode,
            bool isConst, ref DataTable importedParamsTable, int taskID)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlNode);
            XmlNode paramNode = document.ChildNodes[0];

            // ID
            // вставит генератор
            string prmName = XmlHelper.GetStringAttrValue(paramNode, TasksExportXmlConsts.TaskConstsParamsNameTagName, String.Empty);
            string prmDescription = XmlHelper.GetStringAttrValue(paramNode, TasksExportXmlConsts.TaskConstsParamsDescriptionTagName, String.Empty);
            bool prmAllowMultiSelect = false;
            string prmDimension = String.Empty;
            TaskParameterType tp = TaskParameterType.taskConst;
            if (!isConst)
            {
                prmAllowMultiSelect = Convert.ToBoolean(XmlHelper.GetIntAttrValue(paramNode, TasksExportXmlConsts.TaskConstsParamsAllowMultiSelectTagName, 0));
                prmDimension = XmlHelper.GetStringAttrValue(paramNode, TasksExportXmlConsts.TaskConstsParamsDimensionTagName, String.Empty);
                tp = TaskParameterType.taskParameter;
            }
            // ParamValues
            byte[] values = GetUncompressData(paramNode, TasksExportXmlConsts.TaskConstsParamsValuesTagName);

            DataRow newPrm = importedParamsTable.NewRow();
            newPrm.BeginEdit();
            newPrm["ID"] = _db.GetGenerator("G_TASKSPARAMETERS");
            newPrm["Name"] = prmName;
            newPrm["Description"] = prmDescription;
            newPrm["ParamType"] = tp;
            newPrm["RefTasks"] = taskID;
            if (values != null)
                newPrm["ParamValues"] = Encoding.GetEncoding(1251).GetString(values);
            newPrm["AllowMultiSelect"] = prmAllowMultiSelect;
            if (!isConst)
                newPrm["Dimension"] = prmDimension;
            newPrm.EndEdit();
            importedParamsTable.Rows.Add(newPrm);
        }

        private static byte[] GetUncompressData(XmlNode mainNode, string dataTagName)
        {
            XmlNode cDataNode = mainNode.SelectSingleNode(dataTagName);
            if (cDataNode == null)
                return null;
            cDataNode = cDataNode.FirstChild;
            if (cDataNode == null)
                return null;

            byte[] compressedBytes = Convert.FromBase64String(cDataNode.InnerText);

            MemoryStream baseStream = new MemoryStream();
            baseStream.Write(compressedBytes, 0, compressedBytes.Length);
            baseStream.Position = 0;
            // создаем архивирующий стрим и сжимаем данные документа
            DeflateStream uncompressedStream = new DeflateStream(baseStream, CompressionMode.Decompress, true);
            int size = XmlHelper.GetIntAttrValue(mainNode.FirstChild, TasksExportXmlConsts.SizeTagName, 0);
            // преобразуем сжатые данные в строку
            byte[] uncompressedBytes = new byte[size];
            uncompressedStream.Read(uncompressedBytes, 0, uncompressedBytes.Length);
            uncompressedStream.Close();
            baseStream.Close();
            // проверяем СРС
            uint newCRC = CRCHelper.CRC32(uncompressedBytes, 0, uncompressedBytes.Length);
            string oldCRCStr = XmlHelper.GetStringAttrValue(mainNode.FirstChild, TasksExportXmlConsts.CRCTagName, String.Empty);
            uint oldCRC = Convert.ToUInt32(oldCRCStr);
            if (newCRC != oldCRC)
                throw new Exception("Элемент данных поврежден. Контрольные суммы не совпадают");
            return uncompressedBytes;
        }

        #endregion

        private DataTable GetImportTable(string queryStr)
        {
            bool dbNull = false;
            if (_db == null)
            {
                _db = Scheme.SchemeDWH.DB;
                dbNull = true;
            }
            try
            {
                DataTable res = (DataTable)_db.ExecQuery(queryStr, QueryResultTypes.DataTable);
                return res;
            }
            finally
            {
                if (dbNull)
                    _db.Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public DataTable GetTasksImportTable()
        {
            // формируем условие выборки нужных полей с заведомо пустым результатом
            string queryStr = "Select ID, State, FromDate, ToDate, Doer, Owner, Curator, " +
                " Headline, Job, Description, Reftasks, RefTasksTypes from Tasks where (ID is NULL)";
            return GetImportTable(queryStr);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public DataTable GetDocumentsImportTable()
        {
            string queryStr = "select ID, DocumentType, Name, SourceFileName, Version, " +
                "RefTasks, Description, Ownership from Documents where (ID is null)";
            return GetImportTable(queryStr);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public DataTable GetParamsImportTable()
        {
            string queryStr = TaskItemsCollection._selectParamsSQLFields.Substring(0, TaskItemsCollection._selectParamsSQLFields.Length - 2) + " from TasksParameters where (ID is Null)";
            return GetImportTable(queryStr);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private IDataUpdater GetTasksImportUpdater(IDatabase externalDb)
        {
            IDbDataParameter prm = null;
            Database db = (Database)externalDb;
            IDbDataAdapter adapter = db.GetDataAdapter();
            // команда вставки данных
            adapter.InsertCommand = db.Connection.CreateCommand();
            string queryText = "insert into TASKS" +
                " (ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasksTypes, RefTasks)" +
                " values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
            // ID
            prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ID";
            adapter.InsertCommand.Parameters.Add(prm);
            // State
            prm = db.CreateParameter("State", DataAttributeTypes.dtString, 50);
            prm.SourceColumn = "State";
            adapter.InsertCommand.Parameters.Add(prm);
            // FromDate
            prm = db.CreateParameter("FromDate", DataAttributeTypes.dtDateTime, 0);
            prm.SourceColumn = "FromDate";
            adapter.InsertCommand.Parameters.Add(prm);
            // ToDate
            prm = db.CreateParameter("ToDate", DataAttributeTypes.dtDateTime, 0);
            prm.SourceColumn = "ToDate";
            adapter.InsertCommand.Parameters.Add(prm);
            // Doer
            prm = db.CreateParameter("Doer", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "Doer";
            adapter.InsertCommand.Parameters.Add(prm);
            // Owner
            prm = db.CreateParameter("Owner", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "Owner";
            adapter.InsertCommand.Parameters.Add(prm);
            // Curator
            prm = db.CreateParameter("Curator", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "Curator";
            adapter.InsertCommand.Parameters.Add(prm);
            // Headline
            prm = db.CreateParameter("Headline", DataAttributeTypes.dtString, 255);
            prm.SourceColumn = "Headline";
            adapter.InsertCommand.Parameters.Add(prm);
            // Job
            prm = db.CreateParameter("Job", DataAttributeTypes.dtString, 4000);
            prm.SourceColumn = "Job";
            adapter.InsertCommand.Parameters.Add(prm);
            // Description
            prm = db.CreateParameter("Description", DataAttributeTypes.dtString, 4000);
            prm.SourceColumn = "Description";
            adapter.InsertCommand.Parameters.Add(prm);
            // RefTasksTypes
            prm = db.CreateParameter("RefTasksTypes", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "RefTasksTypes";
            adapter.InsertCommand.Parameters.Add(prm);
            // LockByUser
            //prm = db.CreateParameter("LockByUser", DataAttributeTypes.dtInteger, 10);
            //prm.SourceColumn = "LockByUser";
            //adapter.InsertCommand.Parameters.Add(prm);
            // RefTasks
            prm = db.CreateParameter("RefTasks", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "RefTasks";
            ((System.Data.Common.DbParameter)prm).IsNullable = true;
            adapter.InsertCommand.Parameters.Add(prm);

            adapter.InsertCommand.CommandText = db.GetQuery(queryText, adapter.InsertCommand.Parameters);

            DataUpdater du = new DataUpdater(adapter, null, db);
            du.Transaction = db.Transaction;
            return (IDataUpdater)du;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private IDataUpdater GetDocumentsImportUpdater(IDatabase externalDb)
        {
            IDbDataParameter prm = null;
            Database db = (Database)externalDb;
            IDbDataAdapter adapter = db.GetDataAdapter();
            // команда вставки данных
            adapter.InsertCommand = db.Connection.CreateCommand();

            string queryText = "insert into Documents (ID, DocumentType, Name, SourceFileName, Version, " +
                "RefTasks, Description, Ownership) values (?, ?, ?, ?, ?, ?, ?, ?)";
            // ID
            prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ID";
            adapter.InsertCommand.Parameters.Add(prm);
            // DocumentType
            prm = db.CreateParameter("DocumentType", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "DocumentType";
            adapter.InsertCommand.Parameters.Add(prm);
            // Name
            prm = db.CreateParameter("Name", DataAttributeTypes.dtString, 255);
            prm.SourceColumn = "Name";
            adapter.InsertCommand.Parameters.Add(prm);
            // SourceFileName
            prm = db.CreateParameter("SourceFileName", DataAttributeTypes.dtString, 255);
            prm.SourceColumn = "SourceFileName";
            adapter.InsertCommand.Parameters.Add(prm);
            // Version
            prm = db.CreateParameter("Version", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "Version";
            adapter.InsertCommand.Parameters.Add(prm);
            // RefTasks
            prm = db.CreateParameter("RefTasks", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "RefTasks";
            adapter.InsertCommand.Parameters.Add(prm);
            // Description
            prm = db.CreateParameter("Description", DataAttributeTypes.dtString, 4000);
            prm.SourceColumn = "Description";
            adapter.InsertCommand.Parameters.Add(prm);
            // Ownership
            prm = db.CreateParameter("Ownership", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "Ownership";
            adapter.InsertCommand.Parameters.Add(prm);
            // Document
            //prm = db.CreateBlobParameter("Document", ParameterDirection.Input);
            //prm.SourceColumn = "Document";
            //adapter.InsertCommand.Parameters.Add(prm);
            //
            adapter.InsertCommand.CommandText = db.GetQuery(queryText, adapter.InsertCommand.Parameters);

            DataUpdater du = new DataUpdater(adapter, null, db);
            du.Transaction = db.Transaction;
            return (IDataUpdater)du;
        }

        private IDataUpdater GetParamsDataUpdater(IDatabase externalDb)
        {
            Database db = (Database)externalDb;
            IDbDataAdapter adapter = db.GetDataAdapter();
            // команда вставки данных
            adapter.InsertCommand = db.Connection.CreateCommand();
            TaskItemsCollection.AppendParamsCommandParameters(db, adapter.InsertCommand, false);
            string query = TaskItemsCollection._insertParamsSQL.Replace("TasksParametersTemp", "TasksParameters");
            adapter.InsertCommand.CommandText = db.GetQuery(query, adapter.InsertCommand.Parameters);
            DataUpdater upd = new DataUpdater(adapter, null, db);
            upd.Transaction = db.Transaction;
            return (IDataUpdater)upd;
        }

        private void ImportTable(DataTable changes, IDataUpdater upd)
        {
            try
            {
                if ((changes == null) || (changes.Rows.Count == 0))
                    return;
                upd.Update(ref changes);
            }
            finally
            {
                if (upd != null)
                    upd.Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ImportTasks(IDatabase externalDb, DataTable newTasks)
        {
            ImportTable(newTasks, GetTasksImportUpdater(externalDb));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ImportDocuments(IDatabase externalDb, DataTable newDocuments)
        {
            ImportTable(newDocuments, GetDocumentsImportUpdater(externalDb));
        }

        private static string DuplicateNameFilter = "(Name LIKE '{0}') and (ParamType = {1})";

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ImportParams(IDatabase externalDb, DataTable newParams, object parentTaskID)
        {
            // если нет ни параметров ни констант - выходим
            if ((newParams == null) || (newParams.Rows.Count == 0))
                return;
            #region Проверка уникальности параметров и констант
            if ((parentTaskID != null) && (parentTaskID != DBNull.Value))
            {
                // получаем сущесвующие параметры и константы
                DataTable existingParameters = null;
                DataTable existingConsts = null;
                GetTaskConstsParams(Convert.ToInt32(parentTaskID), true, ref existingConsts, ref existingParameters, externalDb);
                // проверяем каждый из новых элементов на допустимость
                DataRow[] newParamsArr = newParams.Select();
                //int maxNumInTransaction
                foreach (DataRow row in newParamsArr)
                {
                    string name = Convert.ToString(row["Name"]);
                    TaskParameterType paramType = (TaskParameterType)Convert.ToInt32(row["ParamType"]);
                    string query = String.Format(DuplicateNameFilter, name, (int)paramType);
                    DataRow[] existing = null;
                    // в зависимости от типа получаем сущесвующие константы или параметры
                    if (paramType == TaskParameterType.taskParameter)
                        existing = existingParameters.Select(query);
                    else
                        existing = existingConsts.Select(query);
                    // если они есть
                    if ((existing != null) && (existing.Length > 0))
                    {
                        bool needDelete = false;
                        switch (paramType)
                        {
                            case TaskParameterType.taskParameter:
                                // для параметров не пишем дубли по измерению
                                needDelete = (String.Compare(
                                    Convert.ToString(row["Dimension"]),
                                    Convert.ToString(existing[0]["Dimension"]),
                                    true) == 0);
                                break;
                            case TaskParameterType.taskConst:
                                needDelete = true;
                                break;
                        }
                        // если надо - удаляем новый параметр
                        if (needDelete)
                        {
                            newParams.Rows.Remove(row);
                        }
                        else
                        {
                            // меняем название нового параметра
                            string baseName = TaskItemsCollection.GetItemBaseName(name);
                            // ищем максимальное значение номера для родительских параметров с таким же именен 
                            int parentItemNum = TaskItemsCollection.GetNewItemNum(existing, baseName);
                            // теперь для вновь добавляемых
                            int selfItemNum = 1;
                            query = String.Format(DuplicateNameFilter, baseName, (int)paramType);
                            DataRow[] newLikely = newParams.Select(query);
                            if ((newLikely != null) && (newLikely.Length > 0))
                                selfItemNum = TaskItemsCollection.GetNewItemNum(newLikely, baseName);
                            name = String.Format(TaskItemsCollection.DuplicateParameterFormat, baseName, 
                                parentItemNum > selfItemNum ? parentItemNum : selfItemNum);
                            row["Name"] = name;
                        }
                    }
                }
            }
            #endregion
            ImportTable(newParams, GetParamsDataUpdater(externalDb));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ImportParams(IDatabase externalDb, DataTable newParams, object parentTaskID, IDataUpdater paramsUpdater)
        {
            // если нет ни параметров ни констант - выходим
            if ((newParams == null) || (newParams.Rows.Count == 0))
                return;
            #region Проверка уникальности параметров и констант
            if ((parentTaskID != null) && (parentTaskID != DBNull.Value))
            {
                // получаем сущесвующие параметры и константы
                DataTable existingParameters = null;
                DataTable existingConsts = null;
                GetTaskConstsParams(Convert.ToInt32(parentTaskID), true, ref existingConsts, ref existingParameters, externalDb);
                // проверяем каждый из новых элементов на допустимость
                DataRow[] newParamsArr = newParams.Select();
                //int maxNumInTransaction
                foreach (DataRow row in newParamsArr)
                {
                    string name = Convert.ToString(row["Name"]);
                    TaskParameterType paramType = (TaskParameterType)Convert.ToInt32(row["ParamType"]);
                    string query = String.Format(DuplicateNameFilter, name, (int)paramType);
                    DataRow[] existing = null;
                    // в зависимости от типа получаем сущесвующие константы или параметры
                    if (paramType == TaskParameterType.taskParameter)
                        existing = existingParameters.Select(query);
                    else
                        existing = existingConsts.Select(query);
                    // если они есть
                    if ((existing != null) && (existing.Length > 0))
                    {
                        bool needDelete = false;
                        switch (paramType)
                        {
                            case TaskParameterType.taskParameter:
                                // для параметров не пишем дубли по измерению
                                needDelete = (String.Compare(
                                    Convert.ToString(row["Dimension"]),
                                    Convert.ToString(existing[0]["Dimension"]),
                                    true) == 0);
                                break;
                            case TaskParameterType.taskConst:
                                needDelete = true;
                                break;
                        }
                        // если надо - удаляем новый параметр
                        if (needDelete)
                        {
                            newParams.Rows.Remove(row);
                        }
                        else
                        {
                            // меняем название нового параметра
                            string baseName = TaskItemsCollection.GetItemBaseName(name);
                            // ищем максимальное значение номера для родительских параметров с таким же именен 
                            int parentItemNum = TaskItemsCollection.GetNewItemNum(existing, baseName);
                            // теперь для вновь добавляемых
                            int selfItemNum = 1;
                            query = String.Format(DuplicateNameFilter, baseName, (int)paramType);
                            DataRow[] newLikely = newParams.Select(query);
                            if ((newLikely != null) && (newLikely.Length > 0))
                                selfItemNum = TaskItemsCollection.GetNewItemNum(newLikely, baseName);
                            name = String.Format(TaskItemsCollection.DuplicateParameterFormat, baseName,
                                parentItemNum > selfItemNum ? parentItemNum : selfItemNum);
                            row["Name"] = name;
                        }
                    }
                }
            }
            #endregion
            UpdateTable(newParams.GetChanges(), paramsUpdater);
            newParams.AcceptChanges();
            //ImportTable(newParams, paramsUpdater);
        }

        #region Для импорта констант
        private Dictionary<int, DataTable> _cashedTasksInfo = new Dictionary<int, DataTable>();

        public DataTable BeginExport()
        {
            int userID = (int)Authentication.UserID;
            DataTable userTasks = this.GetTasksInfo();
            if (_cashedTasksInfo.ContainsKey(userID))
                _cashedTasksInfo.Remove(userID);
            _cashedTasksInfo.Add(userID,userTasks);
            return userTasks;
        }

        /// <summary>
        /// Завершить экспорт. Освободить кэш вспомогательных структур
        /// </summary>
        public void EndExport()
        {
            int userID = (int)Authentication.UserID;
            if (_cashedTasksInfo.ContainsKey(userID))
            {
                _cashedTasksInfo[userID].Clear();
                _cashedTasksInfo[userID] = null;
                _cashedTasksInfo.Remove(userID);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void GetTaskConstsParams(int taskID, bool includeParent, ref DataTable consts, ref DataTable parameters)
        {
            GetTaskConstsParams(taskID, includeParent, ref consts, ref parameters, null);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void GetTaskConstsParams(int taskID, bool includeParent, ref DataTable consts, ref DataTable parameters, IDatabase externalDb)
        {
            Database db = null;
            try
            {
                if (externalDb != null)
                    db = (Database)externalDb;
                else
                    db = (Database)this.Scheme.SchemeDWH.DB;

                // если список задач был закэширован, берем DataTable из кэша
                int curUser = (int)Authentication.UserID;
                DataTable tasksInfo = null;
                if (_cashedTasksInfo.ContainsKey(curUser))
                    tasksInfo = _cashedTasksInfo[curUser];
                else
                    tasksInfo = this.GetTasksInfo(db);

                DataRow current = tasksInfo.Rows.Find(taskID);
                if (current == null)
                    throw new Exception(String.Format("Не найдена задача с ID={0}", taskID));

                DataTable tmpConsts = new DataTable("Consts");
                List<int> tasksProcessed = new List<int>();
                TaskItemsCollection.AppendTaskParams(db, this.Scheme, TaskParameterType.taskConst, 
                    tasksInfo, current, true, ref tmpConsts, includeParent, ref tasksProcessed);
                tmpConsts.EndLoadData();
                tmpConsts.DefaultView.Sort = "Inherited DESC, ID ASC";
                consts = tmpConsts.DefaultView.ToTable();
                consts.RemotingFormat = SerializationFormat.Binary;
                //tmpConsts.Clear();

                DataTable tmpParams = new DataTable("Params");
                tasksProcessed.Clear();
                TaskItemsCollection.AppendTaskParams(db, this.Scheme, TaskParameterType.taskParameter, 
                    tasksInfo, current, true, ref tmpParams, includeParent, ref tasksProcessed);
                tmpParams.EndLoadData();
                tmpParams.DefaultView.Sort = "Inherited DESC, ID ASC";
                parameters = tmpParams.DefaultView.ToTable();
                parameters.RemotingFormat = SerializationFormat.Binary;
                //tmpParams.Clear();

            }
            finally
            {
                if ((db != null) && (externalDb == null))
                    db.Dispose();
            }
        }

        #endregion

    }
}