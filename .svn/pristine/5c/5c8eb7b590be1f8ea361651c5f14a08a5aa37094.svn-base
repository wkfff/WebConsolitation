using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Xml;
using System.Runtime.CompilerServices;

using Krista.FM.Common;
using Krista.FM.Common.Xml;
//using Krista.FM.Common.TaskParameters;
using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.Users;


namespace Krista.FM.Server.Tasks
{
	/// <summary>
	/// Класс реализующий объект "Задача"
	/// </summary>
    public class Task : UpdatedDBObject, ITask, ITaskContext
    {
        private LogicalCallContextData ownerContext = null;

        private void RestoreContext()
        {
            if (LogicalCallContextData.GetContext() == null)
                LogicalCallContextData.SetContext(ownerContext);
        }

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="parentCollection">Родительская коллекция</param>
        public Task(RealTimeDBObjectsEnumerableCollection parentCollection) : base(parentCollection)
		{
            //TaskParamsCollection prms = GetTaskParams();
            ownerContext = LogicalCallContextData.GetContext();
        }

        #region Параметры (пока создается новая копия по запросу)
        public bool LockByCurrentUser()
        {
            return InEdit;
        }

        private ITaskParamsCollection _taskParams = null;

        public ITaskParamsCollection GetTaskParams()
        {
            RestoreContext();
            if (_taskParams == null)
                _taskParams = new TaskParamsCollection(this.ParentCollection.Scheme, this.ID, !LockByCurrentUser());
            return _taskParams;           
        }

        private ITaskConstsCollection _taskConsts = null;

        public ITaskConstsCollection GetTaskConsts()
        {
            RestoreContext();
            if (_taskConsts == null)
                _taskConsts = new TaskConstsCollection(this.ParentCollection.Scheme, this.ID, !LockByCurrentUser());
            return _taskConsts;
        }
        #endregion


        #region реализация свойств
        // ID объекта
		private int _ID = -1;
		public int ID
		{
			get { return _ID; }
			set { _ID = value; }
		}
		
		// Состояние задачи
		private string _state = String.Empty;
		public string State
		{
			get { return _state; }
			set { _state = value; }
		}

		// предыдущее состояние (до записи в Actions)
		private string _cashedState = string.Empty;
		// выполненное действие
		private string _cashedAction = string.Empty;
        public string CashedAction
        {
            get { return _cashedAction; }
        }

        public override bool InEdit
        {
            get { return LockByUser == (int)Authentication.UserID; }
        }

		// Срок исполнения - "с" 
		private  DateTime _fromDate;
		public DateTime FromDate
		{
			get { return _fromDate; }
			set 
            {
                // если старое значение равно новому - ничего не делаем
                if (_fromDate == value) return;
                if (!IsNew)
                {
                    // Изменять может только владелец задачи и пользователи обладающие 
                    // правами на редактирование
                    int curUser = (int)Authentication.UserID;

                    if ((curUser != this.Owner) && (!this.CheckPermission((int)TaskTypeOperations.EditTaskAction, false)))
                    {
                        throw new PermissionException(Authentication.UserName, "Задача", "Изменение срока выполнения",
                            "Только владелец задачи может изменить срок выполнения");
                    }
                }

                _fromDate = value; 
            }
		}

		// Срок исполнения - "по"
		private DateTime _toDate;
		public DateTime ToDate
		{
			get { return _toDate; }
			set 
            {
                // если старое значение равно новому - ничего не делаем
                if (_toDate == value) return;
                if (!IsNew)
                {
                    // Изменять может только владелец задачи и пользователи обладающие 
                    // правами на редактирование
                    int curUser = (int)Authentication.UserID;

                    if ((curUser != this.Owner) && (!this.CheckPermission((int)TaskTypeOperations.EditTaskAction, false)))
                    {
                        throw new PermissionException(Authentication.UserName, "Задача", "Изменение срока выполнения",
                            "Только владелец задачи может изменить срок выполнения");
                    }
                }
                _toDate = value; 
            }
		}

		// Исполнитель (при создании новой задачи автоматически заполняется из поля "Владелец")
		private int _doer;
		public int Doer
		{
			get { return _doer; }
			set { _doer = value; }
		}

		// Владелец
		private int _owner;
		public int Owner
		{
			get { return _owner; }
			set { _owner = value; }
		}

        // Куратор
        private int _curator;
        public int Curator
        {
            get { return _curator; }
            set { _curator = value; }
        }

		// Наименование
		private string _headline = String.Empty;
		public string Headline
		{
			get { return _headline; }
			set 
            {
                // если старое значение равно новому - ничего не делаем
                if (_headline == value) return;
                if (!IsNew)
                {
                    // Изменять может только владелец задачи и пользователи обладающие 
                    // правами на редактирование
                    int curUser = (int)Authentication.UserID;
                    
                    if ((curUser != this.Owner) && (!this.CheckPermission((int)TaskTypeOperations.EditTaskAction, false)))
                    {
                        throw new PermissionException(Authentication.UserName, "Задача", "Изменение наименования",
                            "Только владелец задачи может изменить наименование");
                    }
                }
                _headline = value.Replace(Environment.NewLine, " "); 
            }
		}

        // Задание
        private string _job = String.Empty;
        public string Job
        {
            get { return _job; }
            set 
            {
                // если старое значение равно новому - ничего не делаем
                if (_job == value) return;
                if (!IsNew)
                {
                    // Изменять может только владелец задачи
                    int curUser = (int)Authentication.UserID;
                    if ((curUser != this.Owner)&& (!this.CheckPermission((int)TaskTypeOperations.EditTaskAction, false)))
                    {
                        throw new PermissionException(Authentication.UserName, "Задача", "Изменение задания",
                            "Только владелец задачи может изменить задание");
                    }
                }
                _job = value; 
           }
        }

		// Описание (комментарий)
		private string _description = String.Empty;
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		// Родительская задача
		private int _refTasks = -1;
		public int RefTasks
		{
			get
			{
                // необходимо получать всегда свежую информацию о том, заблокирована задача или нет
			    return _refTasks;
			}
			set { _refTasks = value; }
		}

        // признак заблокированности
        private int _lockByUser = -1;
        public int LockByUser
        {
            get
            {
                return GetActualLockedUser();
            }
            set { _lockByUser = value; }
        }

        // вид задачи
        private int _refTasksTypes = -1;
        public int RefTasksTypes
        {
            get { return _refTasksTypes; }
            set { _refTasksTypes = value; }
        }

        private bool _placedInCacheOnly = false;
        public bool PlacedInCacheOnly 
        { 
            get {return _placedInCacheOnly; } 
        }

		#endregion

        /// <summary>
        /// Внутренний методы выполнения хранимых процедур для помещения/извлечения задачи из кэша
        /// </summary>
        /// <param name="locked">Заблокирована</param>
        /// <param name="canceled">Действие отменено</param>
        /// <param name="chAction">Текущее действие</param>
        /// <param name="chState">Текущее состояние</param>
        private void ExecTasksStoredProc(bool locked, int canceled, string chAction, string chState)
        {
            Database db = null;
            IDbDataParameter prm = null;
            try
            {
                // инициализируем объекты и параметры для выполнения ХП
                db = (Database)this.ParentCollection.GetDB();
                IDbCommand cmd = db.Connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                prm = db.CreateParameter("taskID", (object)this.ID, DbType.Int32);
                cmd.Parameters.Add(prm);
                if (locked)
                {
                    object lockByUser = db.ExecQuery("select LockByUser from Tasks where ID = ?", QueryResultTypes.Scalar,
                                 new DbParameterDescriptor("p0", ID));
                    if (lockByUser != null && lockByUser != DBNull.Value)
                        throw new Exception("Задача уже находится в режиме редактирования");

                    int curUser = (int)Authentication.UserID;
                    prm = db.CreateParameter("userID", (object)curUser, DbType.Int32);
                    cmd.Parameters.Add(prm);
                    prm = db.CreateParameter("CAct", (object)chAction, DbType.String);
                    cmd.Parameters.Add(prm);
                    prm = db.CreateParameter("CSt", (object)chState, DbType.String);
                    cmd.Parameters.Add(prm);
                    cmd.CommandText = "SP_BEGINTASKUPDATE";
                    this.LockByUser = curUser;
                }
                else
                {
                    prm = db.CreateParameter("canceled", (object)canceled, DbType.Int32);
                    cmd.Parameters.Add(prm);
                    cmd.CommandText = "SP_ENDTASKUPDATE";
                    this.LockByUser = -1;
                }
                // осуществляем вызов ХП
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }

        }

        /// <summary>
        /// Заблокировать задачу (поместить в таблицу-кэш)
        /// </summary>
        /// <param name="chAction">Текущее действие</param>
        /// <param name="chState">Текущее сосотояние</param>
        private void LockTask(string chAction, string chState)
        {
            // заблокировать можно только свободную задачу
            if (LockByUser != -1)
                throw new Exception("Задача уже заблокирована");

            ExecTasksStoredProc(true, 0, chAction, chState);
        }

        /// <summary>
        /// Разблокировать задачу
        /// </summary>
        /// <param name="canceled">Действие отменено</param>
        private void UnlockTask(int canceled)
        {
            // разблокировать можно только занятую задачу
            if (LockByUser == -1)
                throw new Exception("Задача уже разблокирована");

            ExecTasksStoredProc(false, canceled, String.Empty, String.Empty);
        }

        /// <summary>
        /// Обновить информацию о задаче в БД
        /// </summary>
        public void SaveStateIntoDatabase()
        {
            // допустимо только для заблокированной задачи
            if (LockByUser == -1)
                throw new Exception("Невозможно обновить кэш состояния свободной задачи");
            SaveTaskState();
        }

        /// <summary>
        /// изменение родительской задачи для текущей
        /// </summary>
        /// <param name="parentId"></param>
        public void SetParentTask(int? parentId)
        {
            // допустимо только для незаблокированной задачи
            if (LockByUser != -1)
                throw new Exception("Невозможно изменить родительскую задачу у заблокированной задачи");
            RefTasks = parentId ?? -1;
            using (IDatabase db = ParentCollection.GetDB())
            {
                db.ExecQuery("update tasks set RefTasks = ? where ID = ?", QueryResultTypes.NonQuery,
                             new DbParameterDescriptor("p0", (object) parentId ?? DBNull.Value),
                             new DbParameterDescriptor("p1", ID));
            }
        }

        private void SaveTaskState()
        {
            // метод очень похож на метод базового класса, неплохо было бы их объединить 
            string commandText = String.Empty;
            IDbDataParameter[] parameters = null;
            // если это только что созданный объект
            if (IsNew)
                // ..запрашиваем параметры для команды вставки
                InitializeInsertCommand(ref commandText, ref parameters);
            else
                // иначе запрашиваем параметры для команды обновления
                InitializeUpdateCommand(ref commandText, ref parameters);
            using (IDatabase db = ParentCollection.GetDB())
            {
                // пытаемся внести изменения в базу
                db.ExecQuery(commandText, QueryResultTypes.NonQuery, parameters);
                IsNew = false;
                // попробуем сохранить параметры и константы здесь
                if (_taskParams != null)
                    _taskParams.SaveChanges();
                if (_taskConsts != null)
                    _taskConsts.SaveChanges();
            }
        }

        #region реализация обстрактных и виртуальных методов UpdatedDBObject
        /// <summary>
        /// Перейти в режим редактирования (выполнения действия)
        /// </summary>
        /// <param name="action">Заголовок действия</param>
        public override void BeginUpdate(string action)
		{
            // запоминаем текущее действие и состояние
            _cashedState = _state;
            _cashedAction = action;
            // если это сущесвующая задача - помещаем задачу в кэш
            if (!this.IsNew)
                LockTask(action, _cashedState);
            // вставляем в таблицу истории отметку о начале действия
            InsertIntoActions("Начато");
            
            // константы и параметры
            if (_taskParams != null)
                ((TaskItemsCollection)_taskParams).IsReadOnly = false;
            if (_taskConsts != null)
                ((TaskItemsCollection)_taskConsts).IsReadOnly = false;

            // вызываем метод родительского класса
			base.BeginUpdate(action);
		}

		/// <summary>
		/// Отменить изменения
		/// </summary>
        public override void CancelUpdate()
		{
            // вызываем метод родительского класса
			base.CancelUpdate();
            // вставляем в таблицу истории отметку об отмене действия
            InsertIntoActions("Отменено");
            // восстанавлиаваем предыдущие значения действия и состояния
            _cashedState = String.Empty;
            _cashedAction = String.Empty;
            // Разблокируем задачу (удаляем из кэша)
            UnlockTask(1);
            // константы и параметры
            if (_taskParams != null)
                ((TaskItemsCollection)_taskParams).IsReadOnly = true;
            if (_taskConsts != null)
                ((TaskItemsCollection)_taskConsts).IsReadOnly = true;

		}

        /// <summary>
        /// Вставка записи в таблицу истории задачи
        /// </summary>
        /// <param name="additionalText">Дополнительная информация</param>
        private void InsertIntoActions(string additionalText)
        {
            // если задачи еще нет в основной таблице - выходим
            if ((this.IsNew) || (this.PlacedInCacheOnly))
                return;

            IDatabase db = ParentCollection.GetDB();
            try
            {
                /* поля таблицы Actions
                ID					number (10) not null,		 PK 
                ActionDate			date not null,				 Дата и время выполнения действия 
                Action				varchar2 (50) not null,		 Выполненное действие 
                RefUsers			number (10) not null,		 Пользователь выполнивший действие 
                OldState			varchar2 (50) not null,		 Прежнее состояние 
                NewState			varchar2 (50) not null,		 Новое состояние 
                RefTasks			number (10) not null,		 Ссылка на задачу */

                int userID = (int)Authentication.UserID;
                // ***
                // когда производится отмена действия другого пользователя _cahedAction и _cahedState 
                // не инициализированы - в этом случае пытаемся вычитать их их временной таблицы
                if ((String.IsNullOrEmpty(_cashedState)) || (String.IsNullOrEmpty(_cashedAction)))
                {
                    DataTable cashedRow = (DataTable)db.ExecQuery(String.Format("Select CState, CAction from TasksTemp where ID = {0}", ID), QueryResultTypes.DataTable);
                    if ((cashedRow != null) && (cashedRow.Rows.Count > 0))
                    {
                        try
                        {
                            _cashedState = Convert.ToString(cashedRow.Rows[0][0]);
                            _cashedAction = Convert.ToString(cashedRow.Rows[0][1]);
                        }
                        catch { }
                    }
                }
                if (String.IsNullOrEmpty(_cashedState))
                    throw new Exception("Внутренняя ошибка: не удалось получить _cashedState");
                if (String.IsNullOrEmpty(_cashedAction))
                    throw new Exception("Внутренняя ошибка: не удалось получить _cashedAction");
                // ***
                string queryText = "insert into Actions (ID, ActionDate, Action, RefUsers, OldState, NewState, RefTasks)" +
                    " values (?, ?, ?, ?, ?, ?, ?)";
                IDbDataParameter[] parameters = new IDbDataParameter[] {
					db.CreateParameter("ID",			(object)db.GetGenerator("g_Actions")),
					db.CreateParameter("ActionDate",	(object)System.DateTime.Now),
					db.CreateParameter("Action",		(object)String.Format("{0}: {1}", additionalText, _cashedAction)),
					db.CreateParameter("RefUsers",		(object)userID),
					db.CreateParameter("OldState",		(object)_cashedState),
					db.CreateParameter("NewState",		(object)State), 
					db.CreateParameter("RefTasks",		(object)ID)
				};
                db.ExecQuery(queryText, QueryResultTypes.NonQuery, parameters);
            }
            finally
            {
                db.Dispose();
            }

        }

        /// <summary>
        /// Зафиксировать изменения и выйти из режима редактирвания
        /// </summary>
		public override void EndUpdate()
		{
			bool oldInEdit = InEdit;
			base.EndUpdate();
            UnlockTask(0);
			if (oldInEdit)
			{
                InsertIntoActions("Завершено");
			}
            _cashedAction = String.Empty;
            // константы и параметры
            if (_taskParams != null)
                ((TaskItemsCollection)_taskParams).IsReadOnly = true;
            if (_taskConsts != null)
                ((TaskItemsCollection)_taskConsts).IsReadOnly = true;
		}

		/// <summary>
		/// Метод инициализации команды вставки записи в БД
		/// </summary>
		/// <param name="insertCommandText">текст команды INSERT</param>
		/// <param name="insertCommandParameters">параметры команды INSERT</param>
		public override void InitializeInsertCommand(ref string insertCommandText, ref IDbDataParameter[] insertCommandParameters)
		{
			// вставлять всегда будем в таблицу кэша
            IDatabase db = ParentCollection.GetDB();
			try
			{
				if (_refTasks != -1)
				{
					// текст команды вставки
					insertCommandText = "insert into TASKSTEMP" +
						" (ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes, LockByUser, CAction, CState)" +
						" values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
					// параметры команды вставки
					insertCommandParameters = new IDbDataParameter[] {
					db.CreateParameter("ID",		(object)_ID),
					db.CreateParameter("State",		(object)_state),
					db.CreateParameter("FromDate",	(object)_fromDate),
					db.CreateParameter("ToDate",	(object)_toDate),
					db.CreateParameter("Doer",		(object)_doer),
					db.CreateParameter("Owner",		(object)_owner),
					db.CreateParameter("Curator",	(object)_curator),
					db.CreateParameter("Headline",	(object)_headline),
					db.CreateParameter("Job",	    (object)_job),
					db.CreateParameter("Description",(object)_description),
					db.CreateParameter("RefTasks",	(object)_refTasks),
					db.CreateParameter("RefTasksTypes",	(object)_refTasksTypes),
                    db.CreateParameter("LockByUser",	(object)_lockByUser),
                    db.CreateParameter("CAction", (object)_cashedAction),
                    db.CreateParameter("CState", (object)_cashedState)
				};
				}
				else
				{
					// текст команды вставки
					insertCommandText = "insert into TASKSTEMP" +
						" (ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasksTypes, LockByUser, CAction, CState)" +
						" values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
					// параметры команды вставки
					insertCommandParameters = new IDbDataParameter[] {
					db.CreateParameter("ID",		(object)_ID),
					db.CreateParameter("State",		(object)_state),
					db.CreateParameter("FromDate",	(object)_fromDate),
					db.CreateParameter("ToDate",	(object)_toDate),
					db.CreateParameter("Doer",		(object)_doer),
					db.CreateParameter("Owner",		(object)_owner),
					db.CreateParameter("Curator",	(object)_curator),
					db.CreateParameter("Headline",	(object)_headline),
					db.CreateParameter("Job",	    (object)_job),
					db.CreateParameter("Description",(object)_description),
					db.CreateParameter("RefTasksTypes",	(object)_refTasksTypes),
                    db.CreateParameter("LockByUser",	(object)_lockByUser),
                    db.CreateParameter("CAction", (object)_cashedAction),
                    db.CreateParameter("CState", (object)_cashedState)
    				};
				}
			}
			finally
			{
				db.Dispose();
			}
		}

        /// <summary>
        /// Получить ID пользователя заблокировавшего задачу
        /// </summary>
        /// <returns></returns>
        public int GetActualLockedUser()
        {
            IDatabase db = ParentCollection.GetDB();
            try
            {
                // определяем наличие и заблокированность задачи в основной таблице
                string queryText = "select ID, LockByUser from Tasks where ID = ?";
                DataTable dt = (DataTable)db.ExecQuery(queryText, QueryResultTypes.DataTable,
                    db.CreateParameter("ID", (object)this.ID));

                int lockedUser = -1;
                _placedInCacheOnly = dt.Rows.Count == 0;
                if (!_placedInCacheOnly)
                {
                    // если в основной таблице есть такая задача - пытаемся определить заблокирована ли она и кем заблокирована
                    object lockedUserObj = dt.Rows[0]["LockByUser"];
                    if (lockedUserObj.ToString() != String.Empty)
                        lockedUser = Convert.ToInt32(lockedUserObj);
                }
                else
                    // если в основной таблице задачи нет - значит это новая задача текущего пользователя
                    lockedUser = (int)Authentication.UserID;
                return lockedUser;
            }
            finally
            {
                if (db!= null)
                    db.Dispose();
            }
        }

        /// <summary>
		/// Метод инициализации команды выборки данных
		/// </summary>
		/// <param name="selectCommandText">текст команды SELECT</param>
		/// <param name="selectCommandParameters">параметры команды SELECT</param>
		public override void InitializeSelectCommand(ref string selectCommandText, ref IDbDataParameter[] selectCommandParameters)
		{
			// а вот выбирать - для заблокировавшего пользователя из таблицы кэша, для остальных - из основной таблицы
            IDatabase db = ParentCollection.GetDB();
			try
			{
                int curUser = (int)Authentication.UserID;
                int lockedUser = this.GetActualLockedUser();

                // в зависимости от заблокированности определяем имя таблицы для выборки
                if (lockedUser == curUser)
                {
                    selectCommandText = 
                        "select ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, " +
                        " Description, RefTasks, LockByUser, RefTasksTypes, CAction, CState" +
                        " from TasksTemp where (ID = ?)";
                }
                else
                {
                    // текст команды выборки
                    selectCommandText = 
                        "select ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, " +
                        " Description, RefTasks, LockByUser, RefTasksTypes" +
                        " from Tasks where (ID = ?)";
                }

				// параметры команды выборки
				selectCommandParameters = new IDbDataParameter[] {
					db.CreateParameter("ID", (object)_ID)
				};
			}
			finally
			{
				db.Dispose();
			}
		}

		/// <summary>
		/// Метод инициализации  команды вставки данных
		/// </summary>
		/// <param name="updateCommandText">текст команды UPDATE</param>
		/// <param name="updateCommandParameters">параметры команды UPDATE</param>
		public override void InitializeUpdateCommand(ref string updateCommandText, ref IDbDataParameter[] updateCommandParameters)
		{
            // судя по всему, обновлтять всегда будем в таблице кэша
			IDatabase db = ParentCollection.GetDB();
			try
			{
				if (_refTasks != -1)
				{
					// текст команды вставки
					updateCommandText = "update TASKSTEMP set " +
						" State = ?, FromDate = ?, ToDate = ?, Doer = ?, Owner = ?, Curator = ?, " +
                        " Headline = ?, Job = ?, Description = ?, " +
						" RefTasks = ?, RefTasksTypes = ? where ID = ?";
					// параметры команды вставки
					updateCommandParameters = new IDbDataParameter[] {
						db.CreateParameter("State",		(object)_state),
						db.CreateParameter("FromDate",	(object)_fromDate),
						db.CreateParameter("ToDate",	(object)_toDate),
						db.CreateParameter("Doer",		(object)_doer),
						db.CreateParameter("Owner",		(object)_owner),
						db.CreateParameter("Curator",	(object)_curator),
						db.CreateParameter("Headline",	(object)_headline),
						db.CreateParameter("Job",	    (object)_job),
						db.CreateParameter("Description",(object)_description),
						db.CreateParameter("RefTasks",	(object)_refTasks),
						db.CreateParameter("RefTasksTypes",	(object)_refTasksTypes),
						db.CreateParameter("ID",		(object)_ID)
					};
				}
				else
				{
					// текст команды вставки
					updateCommandText = "update TASKSTEMP set " +
						" State = ?, FromDate = ?, ToDate = ?, Doer = ?, Owner = ?, Curator = ?, " +
                        " Headline = ?, Job = ?, Description = ?, " +
						" RefTasksTypes = ? where ID = ?";
					// параметры команды вставки
					updateCommandParameters = new IDbDataParameter[] {
						db.CreateParameter("State",		(object)_state),
						db.CreateParameter("FromDate",	(object)_fromDate),
						db.CreateParameter("ToDate",	(object)_toDate),
						db.CreateParameter("Doer",		(object)_doer),
						db.CreateParameter("Owner",		(object)_owner),
						db.CreateParameter("Curator",	(object)_curator),
						db.CreateParameter("Headline",	(object)_headline),
						db.CreateParameter("Job",	    (object)_job),
						db.CreateParameter("Description",(object)_description),
						db.CreateParameter("RefTasksTypes",	(object)_refTasksTypes),
						db.CreateParameter("ID",		(object)_ID)
					};
				}
			}
			finally
			{
				db.Dispose();
			}
		}

		/// <summary>
		/// Метод обновления полей объекта
		/// </summary>
		/// <param name="dataRow">данные объекта</param>
		public override void RefreshObjectFields(DataRow dataRow)
		{
			_ID				= Convert.ToInt32(		dataRow["ID"]);
			_state			= Convert.ToString(		dataRow["STATE"]);
			try
			{
				_fromDate = Convert.ToDateTime(dataRow["FROMDATE"]);
			}
			catch
			{
			}
			try
			{
				_toDate = Convert.ToDateTime(dataRow["TODATE"]);
			}
			catch
			{
			}
			_doer = Convert.ToInt32(dataRow["DOER"]);
            _owner = Convert.ToInt32(dataRow["OWNER"]);
            _curator = Convert.ToInt32(dataRow["CURATOR"]);
            _headline = Convert.ToString(dataRow["HEADLINE"]);
            _job = Convert.ToString(dataRow["JOB"]);
            _description = Convert.ToString(dataRow["DESCRIPTION"]);
            try
            {
                _refTasks = Convert.ToInt32(dataRow["REFTASKS"]);
            }
            catch
            {
                _refTasks = -1;
            }
            try
            {
                _lockByUser = Convert.ToInt32(dataRow["LOCKBYUSER"]);
            }
            catch
            {
                _lockByUser = -1;
            }
            _refTasksTypes = Convert.ToInt32(dataRow["REFTASKSTYPES"]);
            // если есть закэшированное действие и состояние - обновляем и их
            try
            {
                _cashedAction = Convert.ToString(dataRow["CAction"]);
            }
            catch { }
            try
            {
                _cashedState = Convert.ToString(dataRow["CState"]);
            }
            catch { }
        }
		#endregion

		#region Методы для работы с документами

		/// <summary>
		/// Получить ID для нового документа
		/// </summary>
		/// <returns>значение ID</returns>
		public int GetNewDocumentID()
		{
			int docID = 0;
			IDatabase db = ParentCollection.GetDB();
			try
			{
				docID = Convert.ToInt32(db.GetGenerator("g_Documents"));
			}
			finally
			{
				db.Dispose();
			}
			return docID;
		}

		/// <summary>
        /// Возвращает IDataUpdater для работы со списком документов, относящихся к задаче
		/// </summary>
        /// <returns>IDataUpdater</returns>
		public IDataUpdater GetTaskDocumentsAdapter()
		{
			string queryText;
			IDbDataParameter prm;
			DataUpdater du = null;

            // получаем ID текущего пользователя
            int curUser = (int)Authentication.UserID;
            bool lockByCurrentUser = (this.LockByUser == curUser);

			Database db = (Database)ParentCollection.GetDB();
			try
			{
				IDbDataAdapter adapter = db.GetDataAdapter();

				// команда выборки данных
                // текущему пользователю доступны - 
                string filter = String.Empty;
                // ... обшие документы
                filter = String.Format("(Ownership = {0})", (int)TaskDocumentOwnership.doGeneral);
                // ... если пользователь владелец - то документы владельца
                if (curUser == this.Owner)
                {
                    filter = String.Format("{0} or (Ownership = {1})", filter, (int)TaskDocumentOwnership.doOwner);
                }
                // ... если пользователь исполнитель - то документы исполнителя
                if (curUser == this.Doer)
                {
                    filter = String.Format("{0} or (Ownership = {1})", filter, (int)TaskDocumentOwnership.doDoer);
                }
                // ... если пользователь куратор - то документы куратора
                if (curUser == this.Curator)
                {
                    filter = String.Format("{0} or (Ownership = {1})", filter, (int)TaskDocumentOwnership.doCurator);
                }
                //
				adapter.SelectCommand = db.Connection.CreateCommand();
                // в зависимости от заблокированности выбираем документы из основной таблицы или из кэша
                if (!lockByCurrentUser)
                {
                    queryText = String.Format(
                        "select ID, DocumentType, Name, SourceFileName, Version, RefTasks," +
                        "Description, Ownership from Documents where RefTasks = {0} and ({1}) order by ID",
                        this._ID, filter
                    );
                }
                else
                {
                    queryText = String.Format(
                        "select ID, DocumentType, Name, SourceFileName, Version, RefTasksTemp," +
                        "Description, Ownership from DocumentsTemp where RefTasksTemp = {0} and ({1}) order by ID",
                        this._ID, filter
                    );
                }
				adapter.SelectCommand.CommandText = queryText;

				// команда удаления данных
				adapter.DeleteCommand = db.Connection.CreateCommand();
				prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
				prm.SourceColumn = "ID";
				adapter.DeleteCommand.Parameters.Add(prm);
                // удаляем всегда из кэша
				queryText = "delete from DocumentsTemp where ID = ?";
				adapter.DeleteCommand.CommandText = db.GetQuery(queryText, adapter.DeleteCommand.Parameters);

				// команда обновления данных
				adapter.UpdateCommand = db.Connection.CreateCommand();
				// Name
				prm = db.CreateParameter("Name", DataAttributeTypes.dtString, 255);
				prm.SourceColumn = "Name";
				adapter.UpdateCommand.Parameters.Add(prm);
				// Description
				prm = db.CreateParameter("Description", DataAttributeTypes.dtString, 4000);
				prm.SourceColumn = "Description";
				adapter.UpdateCommand.Parameters.Add(prm);
				// DocumentType
                prm = db.CreateParameter("DocumentType", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "DocumentType";
                adapter.UpdateCommand.Parameters.Add(prm);
                // Ownership
                prm = db.CreateParameter("Ownership", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "Ownership";
                adapter.UpdateCommand.Parameters.Add(prm);
				// ID
				prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
				prm.SourceColumn = "ID";
				adapter.UpdateCommand.Parameters.Add(prm);

				// обновляем всегда в кэше
                queryText = "update DocumentsTemp set Name = ?, Description = ?, DocumentType = ?, Ownership = ? where ID = ?";
				adapter.UpdateCommand.CommandText = db.GetQuery(queryText, adapter.UpdateCommand.Parameters);


				// команда вставки данных
                // (вставляем всегда в кэш)
				adapter.InsertCommand = db.Connection.CreateCommand();

				queryText = "insert into DocumentsTemp (ID, DocumentType, Name, SourceFileName, Version, " +
                    "RefTasksTemp, Description, Ownership) values (?, ?, ?, ?, ?, ?, ?, ?)";
				// ID
				IDbDataParameter prmID = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
				prmID.SourceColumn = "ID";
				adapter.InsertCommand.Parameters.Add(prmID);
				// DocumentType
				IDbDataParameter prmDocumentType = db.CreateParameter("DocumentType", DataAttributeTypes.dtInteger, 10);
				prmDocumentType.SourceColumn = "DocumentType";
				adapter.InsertCommand.Parameters.Add(prmDocumentType);
				// Name
				IDbDataParameter prmName = db.CreateParameter("Name", DataAttributeTypes.dtString, 255);
				prmName.SourceColumn = "Name";
				adapter.InsertCommand.Parameters.Add(prmName);
				// SourceFileName
				IDbDataParameter prmSourceFileName = db.CreateParameter("SourceFileName", DataAttributeTypes.dtString, 255);
				prmSourceFileName.SourceColumn = "SourceFileName";
				adapter.InsertCommand.Parameters.Add(prmSourceFileName);
				// Version
				IDbDataParameter prmVersion = db.CreateParameter("Version", DataAttributeTypes.dtInteger, 10);
				prmVersion.SourceColumn = "Version";
				adapter.InsertCommand.Parameters.Add(prmVersion);
				// RefTasks
				IDbDataParameter prmRefTasks = db.CreateParameter("RefTasksTemp", DataAttributeTypes.dtInteger, 10);
				prmRefTasks.SourceColumn = "RefTasksTemp";
				adapter.InsertCommand.Parameters.Add(prmRefTasks);
                // Description
                IDbDataParameter prmDescription = db.CreateParameter("Description", DataAttributeTypes.dtString, 4000);
                prmDescription.SourceColumn = "Description";
                adapter.InsertCommand.Parameters.Add(prmDescription);
                // Ownership
                IDbDataParameter prmOwnership = db.CreateParameter("Ownership", DataAttributeTypes.dtInteger, 10);
                prmOwnership.SourceColumn = "Ownership";
                adapter.InsertCommand.Parameters.Add(prmOwnership);

				adapter.InsertCommand.CommandText = db.GetQuery(queryText, adapter.InsertCommand.Parameters);

				du = new DataUpdater(adapter, null, db);
			}
			finally
			{
				db.Dispose();
			}
			return (IDataUpdater)du;
		}

		/// <summary>
		/// Получить контрольную сумму (CRC32) для документа
		/// </summary>
		/// <param name="documentID">ID документа</param>
		/// <returns>контрольная сумма</returns>
		public ulong GetDocumentCRC32(int documentID)
		{
			byte[] documentData = GetDocumentDataBuff(documentID);
            if (documentData == null)
                return 0;
            else
            {
                ulong crc = CRCHelper.CRC32(documentData, 0, documentData.Length);
                GC.GetTotalMemory(true);
                return crc;
            }
		}

		/// <summary>
		/// Получить данные документа
		/// </summary>
		/// <param name="documentID">ID документа</param>
		/// <returns>Массив данных</returns>
        private byte[] GetDocumentDataBuff(int documentID)
		{

			byte[] documentData = new byte[0];
			IDatabase db = ParentCollection.GetDB();
			try
			{
                // в зависимости от заблокированности выбираем документ из кэша либо из основной таблицы
                int curUser = (int)Authentication.UserID;
                string queryText = String.Empty;
                if (curUser == this.LockByUser)
                {
                    queryText = "Select Document from DocumentsTemp where ID = " + documentID.ToString();
                }
                else
                {
                    queryText = "Select Document from Documents where ID = " + documentID.ToString();
                }
				DataTable dt = (DataTable)db.ExecQuery(queryText, QueryResultTypes.DataTable);
				// исключение возникнет если документ пуст (еще не загружен)
				try
				{
					documentData = (byte[])dt.Rows[0]["Document"];
				}
				catch { };
			}
			finally
			{
				db.Dispose();
			}
			return documentData;
		}

		/// <summary>
		/// Получить документ (данные)
		/// </summary>
		/// <param name="documentID">ID документа</param>
		/// <returns>массив с данными документа</returns>
		public byte[] GetDocumentData(int documentID)
		{
            return GetDocumentDataBuff(documentID);
			/*byte[] documentData = GetDocumentDataBuff(documentID);
			if (documentData != null)
				return new MemoryStream(documentData);
			else
				return null;*/
		}

		/// <summary>
		/// Изменить данные документа
		/// </summary>
		/// <param name="documentID">ID документа</param>
		/// <param name="documentData">массив с данными документа</param>
		public void SetDocumentData(int documentID, Stream documentData)
		{
			byte[] buff = null;
			if (documentData != null)
			{
				buff = new byte[documentData.Length];
				documentData.Position = 0;
				documentData.Read(buff, 0, (int)documentData.Length);
			}
            SetDocumentData(documentID, buff);
		}

        public void SetDocumentData(int documentID, byte[] documentData)
        {
            IDatabase db = ParentCollection.GetDB();
            try
            {
                ((TaskCollection)ParentCollection).SetDocumentData(documentID, documentData, true, db);
            }
            finally
            {
                db.Dispose();
                //GC.GetTotalMemory(true);
            }
        }

		#endregion

        /// <summary>
        /// Получить историю изменения задачи
        /// </summary>
        /// <returns>DataTable с записями таблицы истории</returns>
        public DataTable GetTaskHistory()
        {
            string queryText = String.Format("select ID, ActionDate, Action, RefUsers, OldState, NewState " +
                "from Actions where RefTasks = {0} order by id", this.ID);
            DataTable dt = null;
            IDatabase db = ParentCollection.GetDB();
            try
            {
                dt = (DataTable)db.ExecQuery(queryText, QueryResultTypes.DataTable);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
            // разыменовываем пользователя
            ParentCollection.GetUsersManager().AppendUserNameColumn(dt, "RefUsers");
            return dt;
        }

        /// <summary>
        /// Проверить возможность выполнения действия текущим пользователем
        /// </summary>
        /// <param name="operation">код действия</param>
        /// <param name="raiseException">Генерировать исключение в случае отсутствия прав доступа</param>
        /// <returns></returns>
        public bool CheckPermission(int operation, bool raiseException)
        {
            UsersManager um = (UsersManager)ParentCollection.GetUsersManager();
            int curUser = (int)Authentication.UserID;
            TaskState curState = ((TaskCollection)ParentCollection)._actionsManager.FindStateFromCaption(this.State);

            bool allowed = false;
            // в зависимости от типа операции проверяем всякие дополнительные условия, о которых UsersManager не знает
            switch (operation)
            {
                case (int)TaskOperations.View:
                    #region Временно отключено
                    // владелец всегда может видеть задачу
                    /*if (curUser == this.Owner)
                    {
                        allowed = true;
                        break;
                    }
                    // исполнитель может видеть задачу только с состояния "назначена"
                    if ((curUser == this.Doer) && ((int)curStates >= (int)TaskStates.tsAssigned))
                    {
                        allowed = true;
                        break;
                    }
                    // куратор может видеть задачу только с состояния "на проверку"
                    if ((curUser == this.Curator) && ((int)curStates >= (int)TaskStates.tsOnCheck))
                    {
                        allowed = true;
                        break;
                    }*/
                    #endregion
                    // видеть задачу может владелец, исполнитель и куратор
                    if ((curUser == this.Owner) || (curUser == this.Doer) || (curUser == this.Curator))
                    {
                        allowed = true;
                        break;
                    }
                    break;
                case (int)AllTasksOperations.AssignTaskViewPermission:
                    // владелец может назначать права просмотра задач всех типов
                    allowed = (curUser == this.Owner);
                    break;
                case (int)TaskTypeOperations.AssignTaskViewPermission:
                    // аналогично для конкретного типа
                    allowed = (curUser == this.Owner);
                    break;
            }
            if (allowed)
                return true;

            // если нет удовлетворяющих дополнительных условий - ищем разрешения в UsersManager
            return um.CheckPermissionForTask(this.ID, this.RefTasksTypes, operation, raiseException);
        }

        /// <summary>
        /// Получить  список доступных действий в зависимости от состояния и прав пользователя
        /// </summary>
        /// <param name="stateCaption">Заголовок состояния</param>
        /// <returns>Доступные действия</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string[] GetActionsForState(string stateCaption)
        {
            // получаем необходимые объекты и параметры
            TaskActionManager aMngr = ((TaskCollection)ParentCollection)._actionsManager;
            UsersManager um = ((TaskCollection)ParentCollection).GetUsersManager();
            int curUser = (int)Authentication.UserID;
            bool isAdmin = UserInAdminGroup(curUser, um);
            // получаем все возможные действия для состояния
            TaskActions[] allActions = aMngr.GetActionsForState(stateCaption, isAdmin);
            ArrayList allowedActionsCaptions = new ArrayList();
            // фильтруем действия в соответствии с правами пользователя и дополнительными условиями
            for (int i = 0; i < allActions.Length; i++)
            {
                TaskActions action= allActions[i];
                bool allow = false;
                switch (action)
                {
                    case TaskActions.taCreate:
                        allow = true;
                        break;
                    case TaskActions.taAssign:
                        allow = (curUser == this.Owner);
                        break;
                    case TaskActions.taExecute:
                        allow = (curUser == this.Doer) ||
                            um.CheckPermissionForTask(this.ID, this.RefTasksTypes, (int)TaskOperations.Perform, false);
                        break;
                    case TaskActions.taContinueExecute:
                        allow = (curUser == this.Doer) ||
                            um.CheckPermissionForTask(this.ID, this.RefTasksTypes, (int)TaskOperations.Perform, false);
                        break;
                    case TaskActions.taOnCheck:
                        allow = (curUser == this.Doer);
                        break;
                    case TaskActions.taCheck:
                        allow = (curUser == this.Curator);
                        break;
                    case TaskActions.taContinueCheck:
                        allow = (curUser == this.Curator);
                        break;
                    case TaskActions.taCheckWithErrors:
                        allow = (curUser == this.Curator);
                        break;
                    case TaskActions.taCheckWithoutErrors:
                        allow = (curUser == this.Curator);
                        break;
                    case TaskActions.taBackToRework:
                        allow = (curUser == this.Owner) || isAdmin;
                        break;
                    case TaskActions.taBackToCheck:
                        allow = (curUser == this.Owner);
                        break;
                    case TaskActions.taClose:
                        allow = (curUser == this.Owner);
                        break;
                    case TaskActions.taEdit:
                        allow = um.CheckPermissionForTask(this.ID, this.RefTasksTypes, (int)TaskTypeOperations.EditTaskAction, false);
                        break;
                    case TaskActions.taDelete:
                        allow = um.CheckPermissionForTask(this.ID, this.RefTasksTypes, (int)TaskTypeOperations.DelTaskAction, false);
                        break;
                }
                if (allow)
                    allowedActionsCaptions.Add(aMngr.Actions[action].Caption);
            }
            // возвращаем названия разрешенных действий
            return (string[])allowedActionsCaptions.ToArray(typeof(string));
        }

        private static bool UserInAdminGroup(int curUser, IUsersManager um)
        {
            DataTable dtGrops = um.GetGroupsForUser(curUser);
            DataRow[] adminGroup = dtGrops.Select("ID = 1");
            return Convert.ToBoolean(adminGroup[0][2]);
        }
	}
}
