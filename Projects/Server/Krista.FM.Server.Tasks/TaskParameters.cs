using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.Data.Common;
using System.Linq;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;

// После отпуска:
// 1) Перенести работу с CLOB в общие функции
// 2) Полноценное кэширование на клиенте
// 3) Кэш клиента в роли источника данных для грида
//
namespace Krista.FM.Server.Tasks
{
    [Serializable]
    public class TaskParamBase : DisposableObject, ITaskParamBase
    {

        protected TaskParameterType _parameterType;

        protected TaskItemsCollection _parentCollection;

        private int _id;
        public int ID
        {
            get { return _id; }
        }

        internal TaskParamBase()
        {
        }

        /// <summary>
        /// Очистка ресурсов
        /// </summary>
        /// <param name="disposing">вызван пользователем или сборщиком мусора</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _parentCollection = null;
            }
            base.Dispose(disposing);
        }

        internal DataRow ItemData()
        {
            DataRow[] itemData = _parentCollection.itemsTable.Select(String.Format("ID = {0}", _id));
            if ((itemData == null) || (itemData.Length == 0))
                throw new Exception("Не удалось получить доступ к данным элемента");
            return itemData[0];
        }

        internal TaskParamBase(TaskItemsCollection parentCollection, int id)
        {
            _parentCollection = parentCollection;
            _id = id;
        }

        public bool Inherited
        {
            get { return Convert.ToBoolean(ItemData()["Inherited"]); }
        }

        protected void CheckCanChange()
        {
            // проверяем режим реадонли
            _parentCollection.CheckReadOnly();
            // проверяем унаследованность параметра
            if (Inherited)
                throw new Exception("Невозможно изменить унаследованный параметр");
        }
    }

    [Serializable]
    public class TaskConst : TaskParamBase, ITaskConst
    {
        internal TaskConst(TaskItemsCollection parentCollection, int parentTaskID)
            : base(parentCollection, parentTaskID)
        {
            this._parameterType = TaskParameterType.taskConst;
        }

        public string Name
        {
            get 
            {
                return Convert.ToString(ItemData()["Name"]);
            }
            set 
            {
                CheckCanChange();
                if (Convert.ToString(ItemData()["Name"]) == value)
                    return;
                // проверка на уникальность имени
                if (_parentCollection.ItemsTable.Rows.Cast<DataRow>().Where(x => x["Name"].Equals(value)).Count() > 0)
                    throw new Exception(String.Format("Коллекция уже содержит элемент с именем '{0}'", value));
                ItemData()["Name"] = value;
            }
        }

        public string Comment
        {
            get 
            { 
                return Convert.ToString(ItemData()["Description"]); 
            }
            set 
            {
                CheckCanChange();
                ItemData()["Description"] = value;
            }
        }

        public object Values
        {
            get 
            { 
                return ItemData()["ParamValues"]; 
            }
            set 
            {
                CheckCanChange();
                ItemData()["ParamValues"] = value;
            }
        }

    }

    [Serializable]
    public class TaskParam : TaskConst, ITaskParam
    {
        internal TaskParam(TaskItemsCollection parentCollection, int parentTaskID)
            : base(parentCollection, parentTaskID)
        {
            this._parameterType = TaskParameterType.taskParameter;
        }

        public string Dimension 
        { 
            get 
            { 
                return Convert.ToString(ItemData()["Dimension"]); 
            }
            set 
            {
                CheckCanChange();
                ItemData()["Dimension"] = value; 
            } 
        }

        public bool AllowMultiSelect 
        { 
            get 
            {
                return Convert.ToBoolean(ItemData()["AllowMultiSelect"]);
            }
            set 
            {
                CheckCanChange();
                ItemData()["AllowMultiSelect"] = value; 
            } 
        }
    }

    [Serializable]
    public abstract class TaskItemsCollection : DisposableObject, ITaskItemsCollection
    {
        abstract protected TaskParameterType ParametersType { get; }

        private bool _isReadOnly = true;
        public bool IsReadOnly 
        {
            get 
            { 
                return _isReadOnly; 
            }
            set
            {
                _isReadOnly = value;
            }
        }

        //private List<TaskParamBase> gettedParams = new List<TaskParamBase>();

        internal DataTable itemsTable = new DataTable("ItemsTable");
        /// <summary>
        /// Данные коллекции
        /// </summary>
        public DataTable ItemsTable 
        {
            get { return itemsTable; } 
        }

        private IScheme _scheme = null;
        private int _parentTaskID = -1;

        public int ParentTaskID
        {
            get { return _parentTaskID; }
        }

        private LogicalCallContextData ownerContext = null;

        public TaskItemsCollection(IScheme scheme, int parentTaskID, bool isReadOnly)
        {
            if (scheme == null)
                throw new Exception("Не задан интерфейс схемы");

            ownerContext = LogicalCallContextData.GetContext();

            _scheme = scheme;
            _parentTaskID = parentTaskID;
            _isReadOnly = isReadOnly;
            InternalLoadData();
        }

        // вызов из внешних приложений идет в неавторизованном контексте, в таком случае
        // устанавливаем контекст создавшего пользователя
        private void RestoreContext()
        {
            if (LogicalCallContextData.GetContext() == null)
                LogicalCallContextData.SetContext(ownerContext);
        }

        /// <summary>
        /// Очистка ресурсов
        /// </summary>
        /// <param name="disposing">вызван пользователем или сборщиком мусора</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                InternalClearData();
                ownerContext = null;
            }
            base.Dispose(disposing);
        }

        internal static string _selectParamsSQLFields = "select ID, Name, Dimension, AllowMultiSelect, " +
            "Description, ParamValues, ParamType, RefTasks, ";

        private static string _selectParamsSQL = _selectParamsSQLFields +
            "{0} Inherited from {1} where (RefTasks = {2}) and (ParamType = {3})";

        public static void AppendTaskParams(Database db, IScheme scheme, TaskParameterType parameterType, 
            DataTable tasksInfo, DataRow taskRow, bool itsCurrentTask, ref DataTable paramsTable, 
            bool includeParent, ref List<int> alreadyProcessed)
        {
            int taskID = Convert.ToInt32(taskRow[TasksNavigationTableColumns.ID_COLUMN]);
            if (alreadyProcessed.Contains(taskID))
                return;
            // обработаем сперва родительские задачи
            if (!taskRow.IsNull(TasksNavigationTableColumns.REFTASKS_COLUMN))
            {
                int parentTaskId = Convert.ToInt32(taskRow[TasksNavigationTableColumns.REFTASKS_COLUMN]);
                DataRow parentTaskRow = tasksInfo.Rows.Find(parentTaskId);
                AppendTaskParams(db, scheme, parameterType, tasksInfo, parentTaskRow, false, ref paramsTable, includeParent, ref alreadyProcessed);
                alreadyProcessed.Add(taskID);
            }
    
            // видима ли задача?
            TaskVisibleInNavigation vsbl = (TaskVisibleInNavigation)Convert.ToInt32(taskRow[TasksNavigationTableColumns.VISIBLE_COLUMN]);

            // если задача видима - запрашиваем ее параметры
            if ((vsbl == TaskVisibleInNavigation.tvVisible) || (vsbl == TaskVisibleInNavigation.tvFantom))
            {
                // из какой таблицы брать данные?
                bool fromTemp = false;
                if (taskRow[TasksNavigationTableColumns.LOCKBYUSER_COLUMN] != DBNull.Value)
                    fromTemp = Convert.ToInt32(taskRow[TasksNavigationTableColumns.LOCKBYUSER_COLUMN]) == 
                        scheme.UsersManager.GetCurrentUserID();
                // формируем запрос
                int inheritedValue = itsCurrentTask ? 0 : 1;
                string sql;
                if (fromTemp)
                    sql = String.Format(_selectParamsSQL, inheritedValue, "TasksParametersTemp", taskID, (int)parameterType);
                else
                    sql = String.Format(_selectParamsSQL, inheritedValue, "TasksParameters", taskID, (int)parameterType);
                // выполняем запрос
                DataTable taskParams = (DataTable)db.ExecQuery(sql, QueryResultTypes.DataTable);
                // если вся таблица параметров не существует или пуста - формируем ее структуру
                if (paramsTable.Columns.Count == 0)
                {
                    paramsTable = taskParams.Clone();
                    paramsTable.BeginLoadData();
                }
                // если были возвращены значения параметров - копируем в основную таблицу
                if (taskParams.Rows.Count != 0)
                {
                    foreach (DataRow row in taskParams.Rows)
                    {
                        string name = Convert.ToString(row["Name"]);
                        TaskParameterType paramType = (TaskParameterType)Convert.ToInt32(row["ParamType"]);
                       
                        foreach (DataRow dublicate in paramsTable.Rows.Cast<DataRow>().Where(x => x["Name"].Equals(name) && Convert.ToInt32(x["ParamType"]) == (int) paramType))
                        {
                            switch (paramType)
                            {
                                case TaskParameterType.taskConst:
                                    if (String.Compare(
                                    Convert.ToString(row["Dimension"]),
                                    Convert.ToString(dublicate["Dimension"]),
                                    true) == 0)
                                        row.Delete();
                                    break;
                                case TaskParameterType.taskParameter:
                                    if (String.Compare(
                                    Convert.ToString(row["ParamValues"]),
                                    Convert.ToString(dublicate["ParamValues"]),
                                    true) == 0)
                                        row.Delete();
                                    break;
                            }
                            
                        }
                    }
                    taskParams.AcceptChanges();

                    paramsTable.Merge(taskParams);
                }
            }

            // если задача имеет родительскую - вызываем обработку параметров для нее
            if ((!includeParent) || (taskRow[TasksNavigationTableColumns.REFTASKS_COLUMN] == DBNull.Value))
                return;
        }

        public void ReloadItemsTable()
        {
            InternalLoadData();
        }

        protected void InternalLoadData()
        {
            //получаем все задачи доступные пользователю
            DataTable tasksInfo = _scheme.TaskManager.Tasks.GetTasksInfo();

            Database db = null;
            try
            {
                db = (Database)_scheme.SchemeDWH.DB;
                DataTable tmpTable = new DataTable("TaskParameters");
                tmpTable.BeginLoadData();
                try
                {
                    DataRow[] parentTaskRow = tasksInfo.Select(String.Format("ID = {0}", ParentTaskID));
                    if ((parentTaskRow == null) || (parentTaskRow.Length == 0))
                        return;
                    List<int> tasksProcessed = new List<int>();
                    AppendTaskParams(db, this._scheme, this.ParametersType, tasksInfo, parentTaskRow[0], true, ref tmpTable, true, ref tasksProcessed);
                    tmpTable.DefaultView.Sort = "Inherited DESC, ID ASC";
                    
                    //Фильтрация списка. Если параметр с таким именем есть в родительской задаче, 
                    //то из дочерней такие выкидываем
                    for (int i = tmpTable.Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow row = tmpTable.Rows[i];
                        if (row.RowState == DataRowState.Deleted)
                            continue;
                        if (Convert.ToInt32(row["Inherited"]) == 0)
                            continue;
                        
                        string name = row["Name"].ToString();
                        int id = Convert.ToInt32(row["ID"]);
                        DataRow[] rows = tmpTable.Rows.Cast<DataRow>().Where(x => string.Equals(x["Name"].ToString(), name) && (Convert.ToInt32(x["ID"]) != id)).ToArray();

                        foreach (DataRow r in rows)
                        {
                            r.Delete();
                        }
                    }
                }
                finally
                {                    
                    tmpTable.EndLoadData();
                    tmpTable.AcceptChanges();

                }

                itemsTable.Clear();
                itemsTable = tmpTable.DefaultView.ToTable();
                itemsTable.AcceptChanges();

            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        public void SaveChanges()
        {
            DataTable changes = itemsTable.GetChanges();

            if ((changes == null) || (changes.Rows.Count == 0))
                return;

            DataUpdater upd = null;
            Database db = null;
            try
            {
                db = (Database)_scheme.SchemeDWH.DB;
                InitEditParamsDataAdapter(db, out upd);
                upd.Update(ref changes);
                itemsTable.AcceptChanges();
            }
            catch
            {
                itemsTable.RejectChanges();
                throw;
            }
            finally
            {
                if (upd != null)
                    upd.Dispose();
                if (db != null)
                    db.Dispose();
            }
        }

        public void CancelChanges()
        {
            InternalLoadData();
        }

        public bool HasChanges()
        {
            DataTable dt = itemsTable.GetChanges();
            return ((dt != null) && (dt.Rows.Count > 0));
        }

        protected void InternalClearData()
        {
        }

        internal static void AppendParamsCommandParameters(Database db, IDbCommand command, bool includeIDConstraintParameter)
        {
            IDbDataParameter prm = null;
            // ID
            prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ID";
            ((System.Data.Common.DbParameter)prm).IsNullable = true;
            command.Parameters.Add(prm);
            // Name
            prm = db.CreateParameter("Name", DataAttributeTypes.dtString, 1000);
            prm.SourceColumn = "Name";
            command.Parameters.Add(prm);
            // Dimension
            prm = db.CreateParameter("Dimension", DataAttributeTypes.dtString, 1000);
            prm.SourceColumn = "Dimension";
            //prm.IsNullable = true;
            command.Parameters.Add(prm);
            // AllowMultiSelect
            prm = db.CreateParameter("AllowMultiSelect", DataAttributeTypes.dtBoolean, 1);
            prm.SourceColumn = "AllowMultiSelect";
            command.Parameters.Add(prm);
            // Description
            prm = db.CreateParameter("Descriprtion", DataAttributeTypes.dtString, 2000);
            prm.SourceColumn = "Description";
            command.Parameters.Add(prm);
            // ParamValues
            prm = db.CreateBlobParameter("ParamValues", ParameterDirection.Input);
            ((System.Data.Common.DbParameter)prm).IsNullable = true;
            ((System.Data.Common.DbParameter)prm).DbType = DbType.AnsiString;
            prm.SourceColumn = "ParamValues";
            command.Parameters.Add(prm);
            // ParamType
            prm = db.CreateParameter("ParamType", DataAttributeTypes.dtInteger, 1);
            prm.SourceColumn = "ParamType";
            command.Parameters.Add(prm);
            // RefTasks
            prm = db.CreateParameter("RefTasks", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "RefTasks";
            command.Parameters.Add(prm);

            if (includeIDConstraintParameter)
            {
                // ID
                prm = db.CreateParameter("IDConstraint", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                command.Parameters.Add(prm);
            }
        }

        internal static string _insertParamsSQL = "insert into TasksParametersTemp" +
            " (ID, Name, Dimension, AllowMultiSelect, Description, ParamValues, ParamType, RefTasks)" +
            " values (?, ?, ?, ?, ?, ?, ?, ?)";

        private static string _updateParamsSQL = "update TasksParametersTemp set ID = ?, Name = ?, " +
            "Dimension = ?, AllowMultiSelect = ?, Description = ?, ParamValues = ?, ParamType = ?, RefTasks = ? " +
            "where ID = ?";

        private static string _deleteParamsSQL = "delete from TasksParametersTemp where ID = ?";

        /* Может быть это пригожится для хранимой процедуры
        private static string _selectSQLFields = "select ID, Name, Dimension, AllowMultiSelect, " +
            "Description, ParamValues, ParamType, RefTasks, ";

        private static string _selectSQLParentConst = _selectSQLFields + " 1 Inherited from TasksParameters " +
            "where (ParamType = {1}) and " +
            "(RefTasks in (select ID from tasks where (ID <> {0}) start with ID = {0} connect by prior RefTasks = ID)) UNION ALL ";

        private static string _selectSQLSelf = _selectSQLParentConst + _selectSQLFields +
            "0 Inherited from TasksParameters where (RefTasks = {0}) and (ParamType = {1})";

        private static string _selectSQLSelfTemp = _selectSQLParentConst + _selectSQLFields +
            "0 Inherited from TasksParametersTemp where (RefTasks = {0}) and (ParamType = {1})";
        */

        protected void InitEditParamsDataAdapter(Database db, out DataUpdater upd)
        {
            // получение ИД родительских задач
            //private static string dddd = "select ID, LockByUser from Tasks where ID <> {0} start with ID = {0} connect by prior RefTasks = ID ";

            IDbDataAdapter adapter = db.GetDataAdapter();
            // команда вставки данных
            adapter.InsertCommand = db.Connection.CreateCommand();
            AppendParamsCommandParameters(db, adapter.InsertCommand, false);
            adapter.InsertCommand.CommandText = db.GetQuery(_insertParamsSQL, adapter.InsertCommand.Parameters);

            // команда обновления данных
            adapter.UpdateCommand = db.Connection.CreateCommand();
            AppendParamsCommandParameters(db, adapter.UpdateCommand, true);
            adapter.UpdateCommand.CommandText = db.GetQuery(_updateParamsSQL, adapter.UpdateCommand.Parameters);

            // команда удаления данных
            adapter.DeleteCommand = db.Connection.CreateCommand();
            IDbDataParameter prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ID";
            adapter.DeleteCommand.Parameters.Add(prm);
            adapter.DeleteCommand.CommandText = db.GetQuery(_deleteParamsSQL, adapter.DeleteCommand.Parameters);

            upd = new DataUpdater(adapter, null, db);
            upd.Transaction = db.Transaction;
        }

        public int Count 
        {
            get { return itemsTable.Rows.Count; } 
        }

        // синхронизируем себя с другой коллекцией
        public bool SyncWith(ITaskItemsCollection destination, out string errorStr)
        {
            RestoreContext();
            errorStr = String.Empty;
            if (destination == null)
            {
                errorStr = "Не задана коллекция-источник";
                return false;
            }
            ITaskParamBase bprm = null;
            //ITaskParamBase newPrm = null;
            DataRow[] ourPrm = null;
            for (int i = 0; i < destination.Count; i++)
            {
                if (this.ParametersType == TaskParameterType.taskConst)
                    bprm = ((ITaskConstsCollection)destination).ConstByIndex(i);
                else
                    bprm = ((ITaskParamsCollection)destination).ParamByIndex(i);
                
                string prmName = ((ITaskConst)bprm).Name;

                // параметра нет - добавляем
                if (this.ItemsTable.Rows.Cast<DataRow>().Where(x => x["Name"].Equals(prmName)).Count() == 0)
                {
                    if (this.ParametersType == TaskParameterType.taskConst)
                        bprm = ((ITaskConstsCollection)destination).AddNew();
                    else
                        bprm = ((ITaskParamsCollection)destination).AddNew();
                }
                // параметр есть - проверяем
                else
                {
                }

            }
            return true;
        }

        private TaskParamBase GetParamBase(int paramID)
        {
            TaskParamBase prm = null;
            switch (ParametersType)
            {
                case TaskParameterType.taskConst:
                    prm = new TaskConst(this, paramID);
                    break;
                case TaskParameterType.taskParameter:
                    prm = new TaskParam(this, paramID);
                    break;
            }
            return prm;
        }

        internal void CheckReadOnly()
        {
            // Проверяем возможность добавления
            if (this.IsReadOnly)
                throw new Exception("Коллекция находитcя в режиме ReadOnly");
        }

        private static string NewParameterName = "Новый параметр";
        private static string NewConstName = "Новая константа";
        private static string NewNameFilter = "Name LIKE '*{0}*'";
        internal static string DuplicateParameterFormat = "{0} ({1})";

        internal static string GetItemBaseName(string name)
        {
            // если строка не задана или пуста - ничего не делаем
            if (String.IsNullOrEmpty(name))
                return name;
            // последний символ - закрывающая скобка?
            if (name[name.Length - 1] != ')')
                return name;
            // в строке также присутствует открывающая скобка?
            int begNumIndex = name.LastIndexOf('(');
            if (begNumIndex == -1)
                return name;
            // между скобками заключено число?
            string tmpStr = name.Substring(begNumIndex + 1, name.Length - begNumIndex - 2);
            bool isNum = false;
            try
            {
                Convert.ToInt32(tmpStr);
                isNum = true;
            }
            catch {}
            if (!isNum)
                return name;
            // возвращаем содержимое строки до числа в скобках
            tmpStr = name.Substring(0, begNumIndex);
            return tmpStr.Trim();
        }

        internal static int GetNewItemNum(DataRow[] existingItems, string itemBaseName)
        {
            int maxNum = 1;
            foreach (DataRow row in existingItems)
            {
                try
                {
                    string lastName = Convert.ToString(row["Name"]);
                    lastName = lastName.Substring(itemBaseName.Length + 2, lastName.Length - itemBaseName.Length - 3);
                    int curNum = Convert.ToInt32(lastName);
                    if (curNum >= maxNum) maxNum = curNum + 1;
                }
                catch { }
            }
            return maxNum;
        }

        private string GetNewItemName()
        {
            //  в зависимости от типа параметра формируем имя
            string newName = String.Empty;
            if (this.ParametersType == TaskParameterType.taskParameter)
                newName = NewParameterName;
            else
                newName = NewConstName;

            // ищем все параметры с именами содержащими имя по умолчанию
            string filterStr = String.Format(NewNameFilter, newName.Trim());
            DataRow[] existingNames = itemsTable.Select(filterStr, "Name DESC");
            // если такие есть - ищем последнее с номером на конце
            if (existingNames.Length > 0)
            {
                newName = String.Format(DuplicateParameterFormat, newName, GetNewItemNum(existingNames, newName));
            }
            return newName.Trim();
        }

        protected ITaskParamBase InternalAddNew()
        {
            CheckReadOnly();
            RestoreContext();

            // получаем новое ИД
            IDatabase db = null;
            int newID = 0;
            try
            {
                db = _scheme.SchemeDWH.DB;
                newID = db.GetGenerator("g_TasksParameters");
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
            // Добавляем новую запись
            DataRow newParameterRow = itemsTable.NewRow();
            // ставим ей ID и ссылку на задачу
            newParameterRow["ID"] = newID;
            newParameterRow["RefTasks"] = ParentTaskID;
            newParameterRow["Inherited"] = 0;
            newParameterRow["ParamType"] = (int)this.ParametersType;
            newParameterRow["Name"] = GetNewItemName();

            newParameterRow["AllowMultiselect"] = false;
            itemsTable.Rows.Add(newParameterRow);

            // создаем и отдаем пользователю объект для доступа к данным параметра
            return GetParamBase(newID);
        }

        protected void InternalRemove(ITaskParamBase item)
        {
            CheckReadOnly();
            RestoreContext();

            DataRow itemData = ((TaskParamBase)item).ItemData();
            itemData.Delete();
            // сразу сохраняем данные при удалении сразу
            SaveChanges();
        }

        protected ITaskParamBase InternalItemByID(int id)
        {
            DataRow[] rows = itemsTable.Select(String.Format("ID = '{0}'", id));
            if (rows.Length == 0)
                return null;

            return GetParamBase(id);
        }

        protected ITaskParamBase InternalItemByIndex(int index)
        {
            if ((index < 0) || (index > itemsTable.Rows.Count - 1))
                throw new Exception("Индекс задан неверно");

            return GetParamBase(Convert.ToInt32(itemsTable.Rows[index]["ID"]));
        }

        protected ITaskParamBase InternalItemByName(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new Exception("Не задано имя элемента коллекции");

            foreach (DataRow row in itemsTable.Rows.Cast<DataRow>().Where(x => x["Name"].Equals(name)))
            {
                return GetParamBase(Convert.ToInt32(row["ID"]));
            }
            return null;            
        }

        #region Эксперименты
 /* 
        #region IBindingList
        // Events
        private ListChangedEventHandler _onListChanged = null;
        public event ListChangedEventHandler ListChanged
        {
            add { _onListChanged += value; }
            remove { _onListChanged -= value; }
        }

        // Methods
        public void AddIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public object AddNew()
        {
            //throw new NotImplementedException();
            return (object)InternalAddNew();
        }

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            throw new NotImplementedException();
        }

        public int Find(PropertyDescriptor property, object key)
        {
            throw new NotImplementedException();
        }

        public void RemoveIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public void RemoveSort()
        {
            throw new NotImplementedException();
        }


        // Properties
        public bool AllowEdit
        {
            get { return true; }
        }

        public bool AllowNew
        {
            get { return true; }
        }

        public bool AllowRemove
        {
            get { return true; }
        }

        public bool IsSorted
        {
            get { return true; }
        }

        public ListSortDirection SortDirection
        {
            get { return ListSortDirection.Ascending; }
        }

        public PropertyDescriptor SortProperty
        {
            get { return null; }
        }

        public bool SupportsChangeNotification
        {
            get { return true; }
        }

        public bool SupportsSearching
        {
            get { return true; }
        }

        public bool SupportsSorting
        {
            get { return true; }
        }
        #endregion

        #region  IList
        // Methods
        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        // Properties
        public bool IsFixedSize
        {
            get { return false; }
        }


        public object this[int index]
        {
            get
            {
                return InternalItemByIndex(index);
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region ICollection
        // Methods
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        // Properties

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return false; }
        }
        #endregion

        #region IEnumerable
        public IEnumerator GetEnumerator()
        {
            return this as IEnumerator;
        }
        #endregion

        #region IEnumerator

        private int _curIndex = 0;
        public bool MoveNext()
        {
            _curIndex++;
            return (_curIndex >= Count - 1);
        }

        public object Current
        {
            get { return InternalItemByIndex(_curIndex); }
        }

        public void Reset()
        {
            _curIndex = 0;
        }
        #endregion
  */
           #endregion

       }

       [Serializable]
       public class TaskConstsCollection : TaskItemsCollection, ITaskConstsCollection//, IBindingList
       {
           override protected TaskParameterType ParametersType 
           {
               get { return TaskParameterType.taskConst; } 
           }

           public TaskConstsCollection(IScheme scheme, int parentTaskID, bool isReadOnly)
               : base(scheme, parentTaskID, isReadOnly)
           {
           }

           public ITaskConst ConstByIndex(int index)
           {
               return (ITaskConst)InternalItemByIndex(index);
           }

           public ITaskConst ConstByID(int id)
           {
               return (ITaskConst)InternalItemByID(id);
           }

           public ITaskConst ConstByName(string name)
           {
               return (ITaskConst)InternalItemByName(name);
           }

           public ITaskConst AddNew()
           {
               return (ITaskConst)InternalAddNew();
           }

           public void Remove(ITaskConst item)
           {
               InternalRemove(item);
           }
       }

       [Serializable]
       public class TaskParamsCollection : TaskItemsCollection, ITaskParamsCollection//, IBindingList
       {
           override protected TaskParameterType ParametersType
           {
               get { return TaskParameterType.taskParameter; }
           }

           public TaskParamsCollection(IScheme scheme, int parentTaskID, bool isReadOnly)
               : base(scheme, parentTaskID, isReadOnly)
           {           
           }

           public ITaskParam ParamByIndex(int index)
           {
               return (ITaskParam)InternalItemByIndex(index);
           }

           public ITaskParam ParamByID(int id)
           {
               return (ITaskParam)InternalItemByID(id);
           }

           public ITaskParam ParamByName(string name)
           {
               return (ITaskParam)InternalItemByName(name);
           }

           public ITaskParam AddNew()
           {
               return (ITaskParam)InternalAddNew();
           }

           public void Remove(ITaskParam item)
           {
               InternalRemove(item);
           }

       }
   
}