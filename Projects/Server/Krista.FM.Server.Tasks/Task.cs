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
	/// ����� ����������� ������ "������"
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
		/// ����������� ������
		/// </summary>
		/// <param name="parentCollection">������������ ���������</param>
        public Task(RealTimeDBObjectsEnumerableCollection parentCollection) : base(parentCollection)
		{
            //TaskParamsCollection prms = GetTaskParams();
            ownerContext = LogicalCallContextData.GetContext();
        }

        #region ��������� (���� ��������� ����� ����� �� �������)
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


        #region ���������� �������
        // ID �������
		private int _ID = -1;
		public int ID
		{
			get { return _ID; }
			set { _ID = value; }
		}
		
		// ��������� ������
		private string _state = String.Empty;
		public string State
		{
			get { return _state; }
			set { _state = value; }
		}

		// ���������� ��������� (�� ������ � Actions)
		private string _cashedState = string.Empty;
		// ����������� ��������
		private string _cashedAction = string.Empty;
        public string CashedAction
        {
            get { return _cashedAction; }
        }

        public override bool InEdit
        {
            get { return LockByUser == (int)Authentication.UserID; }
        }

		// ���� ���������� - "�" 
		private  DateTime _fromDate;
		public DateTime FromDate
		{
			get { return _fromDate; }
			set 
            {
                // ���� ������ �������� ����� ������ - ������ �� ������
                if (_fromDate == value) return;
                if (!IsNew)
                {
                    // �������� ����� ������ �������� ������ � ������������ ���������� 
                    // ������� �� ��������������
                    int curUser = (int)Authentication.UserID;

                    if ((curUser != this.Owner) && (!this.CheckPermission((int)TaskTypeOperations.EditTaskAction, false)))
                    {
                        throw new PermissionException(Authentication.UserName, "������", "��������� ����� ����������",
                            "������ �������� ������ ����� �������� ���� ����������");
                    }
                }

                _fromDate = value; 
            }
		}

		// ���� ���������� - "��"
		private DateTime _toDate;
		public DateTime ToDate
		{
			get { return _toDate; }
			set 
            {
                // ���� ������ �������� ����� ������ - ������ �� ������
                if (_toDate == value) return;
                if (!IsNew)
                {
                    // �������� ����� ������ �������� ������ � ������������ ���������� 
                    // ������� �� ��������������
                    int curUser = (int)Authentication.UserID;

                    if ((curUser != this.Owner) && (!this.CheckPermission((int)TaskTypeOperations.EditTaskAction, false)))
                    {
                        throw new PermissionException(Authentication.UserName, "������", "��������� ����� ����������",
                            "������ �������� ������ ����� �������� ���� ����������");
                    }
                }
                _toDate = value; 
            }
		}

		// ����������� (��� �������� ����� ������ ������������� ����������� �� ���� "��������")
		private int _doer;
		public int Doer
		{
			get { return _doer; }
			set { _doer = value; }
		}

		// ��������
		private int _owner;
		public int Owner
		{
			get { return _owner; }
			set { _owner = value; }
		}

        // �������
        private int _curator;
        public int Curator
        {
            get { return _curator; }
            set { _curator = value; }
        }

		// ������������
		private string _headline = String.Empty;
		public string Headline
		{
			get { return _headline; }
			set 
            {
                // ���� ������ �������� ����� ������ - ������ �� ������
                if (_headline == value) return;
                if (!IsNew)
                {
                    // �������� ����� ������ �������� ������ � ������������ ���������� 
                    // ������� �� ��������������
                    int curUser = (int)Authentication.UserID;
                    
                    if ((curUser != this.Owner) && (!this.CheckPermission((int)TaskTypeOperations.EditTaskAction, false)))
                    {
                        throw new PermissionException(Authentication.UserName, "������", "��������� ������������",
                            "������ �������� ������ ����� �������� ������������");
                    }
                }
                _headline = value.Replace(Environment.NewLine, " "); 
            }
		}

        // �������
        private string _job = String.Empty;
        public string Job
        {
            get { return _job; }
            set 
            {
                // ���� ������ �������� ����� ������ - ������ �� ������
                if (_job == value) return;
                if (!IsNew)
                {
                    // �������� ����� ������ �������� ������
                    int curUser = (int)Authentication.UserID;
                    if ((curUser != this.Owner)&& (!this.CheckPermission((int)TaskTypeOperations.EditTaskAction, false)))
                    {
                        throw new PermissionException(Authentication.UserName, "������", "��������� �������",
                            "������ �������� ������ ����� �������� �������");
                    }
                }
                _job = value; 
           }
        }

		// �������� (�����������)
		private string _description = String.Empty;
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		// ������������ ������
		private int _refTasks = -1;
		public int RefTasks
		{
			get
			{
                // ���������� �������� ������ ������ ���������� � ���, ������������� ������ ��� ���
			    return _refTasks;
			}
			set { _refTasks = value; }
		}

        // ������� �����������������
        private int _lockByUser = -1;
        public int LockByUser
        {
            get
            {
                return GetActualLockedUser();
            }
            set { _lockByUser = value; }
        }

        // ��� ������
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
        /// ���������� ������ ���������� �������� �������� ��� ���������/���������� ������ �� ����
        /// </summary>
        /// <param name="locked">�������������</param>
        /// <param name="canceled">�������� ��������</param>
        /// <param name="chAction">������� ��������</param>
        /// <param name="chState">������� ���������</param>
        private void ExecTasksStoredProc(bool locked, int canceled, string chAction, string chState)
        {
            Database db = null;
            IDbDataParameter prm = null;
            try
            {
                // �������������� ������� � ��������� ��� ���������� ��
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
                        throw new Exception("������ ��� ��������� � ������ ��������������");

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
                // ������������ ����� ��
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }

        }

        /// <summary>
        /// ������������� ������ (��������� � �������-���)
        /// </summary>
        /// <param name="chAction">������� ��������</param>
        /// <param name="chState">������� ����������</param>
        private void LockTask(string chAction, string chState)
        {
            // ������������� ����� ������ ��������� ������
            if (LockByUser != -1)
                throw new Exception("������ ��� �������������");

            ExecTasksStoredProc(true, 0, chAction, chState);
        }

        /// <summary>
        /// �������������� ������
        /// </summary>
        /// <param name="canceled">�������� ��������</param>
        private void UnlockTask(int canceled)
        {
            // �������������� ����� ������ ������� ������
            if (LockByUser == -1)
                throw new Exception("������ ��� ��������������");

            ExecTasksStoredProc(false, canceled, String.Empty, String.Empty);
        }

        /// <summary>
        /// �������� ���������� � ������ � ��
        /// </summary>
        public void SaveStateIntoDatabase()
        {
            // ��������� ������ ��� ��������������� ������
            if (LockByUser == -1)
                throw new Exception("���������� �������� ��� ��������� ��������� ������");
            SaveTaskState();
        }

        /// <summary>
        /// ��������� ������������ ������ ��� �������
        /// </summary>
        /// <param name="parentId"></param>
        public void SetParentTask(int? parentId)
        {
            // ��������� ������ ��� ����������������� ������
            if (LockByUser != -1)
                throw new Exception("���������� �������� ������������ ������ � ��������������� ������");
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
            // ����� ����� ����� �� ����� �������� ������, ������� ���� �� �� ���������� 
            string commandText = String.Empty;
            IDbDataParameter[] parameters = null;
            // ���� ��� ������ ��� ��������� ������
            if (IsNew)
                // ..����������� ��������� ��� ������� �������
                InitializeInsertCommand(ref commandText, ref parameters);
            else
                // ����� ����������� ��������� ��� ������� ����������
                InitializeUpdateCommand(ref commandText, ref parameters);
            using (IDatabase db = ParentCollection.GetDB())
            {
                // �������� ������ ��������� � ����
                db.ExecQuery(commandText, QueryResultTypes.NonQuery, parameters);
                IsNew = false;
                // ��������� ��������� ��������� � ��������� �����
                if (_taskParams != null)
                    _taskParams.SaveChanges();
                if (_taskConsts != null)
                    _taskConsts.SaveChanges();
            }
        }

        #region ���������� ����������� � ����������� ������� UpdatedDBObject
        /// <summary>
        /// ������� � ����� �������������� (���������� ��������)
        /// </summary>
        /// <param name="action">��������� ��������</param>
        public override void BeginUpdate(string action)
		{
            // ���������� ������� �������� � ���������
            _cashedState = _state;
            _cashedAction = action;
            // ���� ��� ����������� ������ - �������� ������ � ���
            if (!this.IsNew)
                LockTask(action, _cashedState);
            // ��������� � ������� ������� ������� � ������ ��������
            InsertIntoActions("������");
            
            // ��������� � ���������
            if (_taskParams != null)
                ((TaskItemsCollection)_taskParams).IsReadOnly = false;
            if (_taskConsts != null)
                ((TaskItemsCollection)_taskConsts).IsReadOnly = false;

            // �������� ����� ������������� ������
			base.BeginUpdate(action);
		}

		/// <summary>
		/// �������� ���������
		/// </summary>
        public override void CancelUpdate()
		{
            // �������� ����� ������������� ������
			base.CancelUpdate();
            // ��������� � ������� ������� ������� �� ������ ��������
            InsertIntoActions("��������");
            // ���������������� ���������� �������� �������� � ���������
            _cashedState = String.Empty;
            _cashedAction = String.Empty;
            // ������������ ������ (������� �� ����)
            UnlockTask(1);
            // ��������� � ���������
            if (_taskParams != null)
                ((TaskItemsCollection)_taskParams).IsReadOnly = true;
            if (_taskConsts != null)
                ((TaskItemsCollection)_taskConsts).IsReadOnly = true;

		}

        /// <summary>
        /// ������� ������ � ������� ������� ������
        /// </summary>
        /// <param name="additionalText">�������������� ����������</param>
        private void InsertIntoActions(string additionalText)
        {
            // ���� ������ ��� ��� � �������� ������� - �������
            if ((this.IsNew) || (this.PlacedInCacheOnly))
                return;

            IDatabase db = ParentCollection.GetDB();
            try
            {
                /* ���� ������� Actions
                ID					number (10) not null,		 PK 
                ActionDate			date not null,				 ���� � ����� ���������� �������� 
                Action				varchar2 (50) not null,		 ����������� �������� 
                RefUsers			number (10) not null,		 ������������ ����������� �������� 
                OldState			varchar2 (50) not null,		 ������� ��������� 
                NewState			varchar2 (50) not null,		 ����� ��������� 
                RefTasks			number (10) not null,		 ������ �� ������ */

                int userID = (int)Authentication.UserID;
                // ***
                // ����� ������������ ������ �������� ������� ������������ _cahedAction � _cahedState 
                // �� ���������������� - � ���� ������ �������� �������� �� �� ��������� �������
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
                    throw new Exception("���������� ������: �� ������� �������� _cashedState");
                if (String.IsNullOrEmpty(_cashedAction))
                    throw new Exception("���������� ������: �� ������� �������� _cashedAction");
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
        /// ������������� ��������� � ����� �� ������ �������������
        /// </summary>
		public override void EndUpdate()
		{
			bool oldInEdit = InEdit;
			base.EndUpdate();
            UnlockTask(0);
			if (oldInEdit)
			{
                InsertIntoActions("���������");
			}
            _cashedAction = String.Empty;
            // ��������� � ���������
            if (_taskParams != null)
                ((TaskItemsCollection)_taskParams).IsReadOnly = true;
            if (_taskConsts != null)
                ((TaskItemsCollection)_taskConsts).IsReadOnly = true;
		}

		/// <summary>
		/// ����� ������������� ������� ������� ������ � ��
		/// </summary>
		/// <param name="insertCommandText">����� ������� INSERT</param>
		/// <param name="insertCommandParameters">��������� ������� INSERT</param>
		public override void InitializeInsertCommand(ref string insertCommandText, ref IDbDataParameter[] insertCommandParameters)
		{
			// ��������� ������ ����� � ������� ����
            IDatabase db = ParentCollection.GetDB();
			try
			{
				if (_refTasks != -1)
				{
					// ����� ������� �������
					insertCommandText = "insert into TASKSTEMP" +
						" (ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes, LockByUser, CAction, CState)" +
						" values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
					// ��������� ������� �������
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
					// ����� ������� �������
					insertCommandText = "insert into TASKSTEMP" +
						" (ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasksTypes, LockByUser, CAction, CState)" +
						" values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
					// ��������� ������� �������
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
        /// �������� ID ������������ ���������������� ������
        /// </summary>
        /// <returns></returns>
        public int GetActualLockedUser()
        {
            IDatabase db = ParentCollection.GetDB();
            try
            {
                // ���������� ������� � ����������������� ������ � �������� �������
                string queryText = "select ID, LockByUser from Tasks where ID = ?";
                DataTable dt = (DataTable)db.ExecQuery(queryText, QueryResultTypes.DataTable,
                    db.CreateParameter("ID", (object)this.ID));

                int lockedUser = -1;
                _placedInCacheOnly = dt.Rows.Count == 0;
                if (!_placedInCacheOnly)
                {
                    // ���� � �������� ������� ���� ����� ������ - �������� ���������� ������������� �� ��� � ��� �������������
                    object lockedUserObj = dt.Rows[0]["LockByUser"];
                    if (lockedUserObj.ToString() != String.Empty)
                        lockedUser = Convert.ToInt32(lockedUserObj);
                }
                else
                    // ���� � �������� ������� ������ ��� - ������ ��� ����� ������ �������� ������������
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
		/// ����� ������������� ������� ������� ������
		/// </summary>
		/// <param name="selectCommandText">����� ������� SELECT</param>
		/// <param name="selectCommandParameters">��������� ������� SELECT</param>
		public override void InitializeSelectCommand(ref string selectCommandText, ref IDbDataParameter[] selectCommandParameters)
		{
			// � ��� �������� - ��� ���������������� ������������ �� ������� ����, ��� ��������� - �� �������� �������
            IDatabase db = ParentCollection.GetDB();
			try
			{
                int curUser = (int)Authentication.UserID;
                int lockedUser = this.GetActualLockedUser();

                // � ����������� �� ����������������� ���������� ��� ������� ��� �������
                if (lockedUser == curUser)
                {
                    selectCommandText = 
                        "select ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, " +
                        " Description, RefTasks, LockByUser, RefTasksTypes, CAction, CState" +
                        " from TasksTemp where (ID = ?)";
                }
                else
                {
                    // ����� ������� �������
                    selectCommandText = 
                        "select ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, " +
                        " Description, RefTasks, LockByUser, RefTasksTypes" +
                        " from Tasks where (ID = ?)";
                }

				// ��������� ������� �������
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
		/// ����� �������������  ������� ������� ������
		/// </summary>
		/// <param name="updateCommandText">����� ������� UPDATE</param>
		/// <param name="updateCommandParameters">��������� ������� UPDATE</param>
		public override void InitializeUpdateCommand(ref string updateCommandText, ref IDbDataParameter[] updateCommandParameters)
		{
            // ���� �� �����, ���������� ������ ����� � ������� ����
			IDatabase db = ParentCollection.GetDB();
			try
			{
				if (_refTasks != -1)
				{
					// ����� ������� �������
					updateCommandText = "update TASKSTEMP set " +
						" State = ?, FromDate = ?, ToDate = ?, Doer = ?, Owner = ?, Curator = ?, " +
                        " Headline = ?, Job = ?, Description = ?, " +
						" RefTasks = ?, RefTasksTypes = ? where ID = ?";
					// ��������� ������� �������
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
					// ����� ������� �������
					updateCommandText = "update TASKSTEMP set " +
						" State = ?, FromDate = ?, ToDate = ?, Doer = ?, Owner = ?, Curator = ?, " +
                        " Headline = ?, Job = ?, Description = ?, " +
						" RefTasksTypes = ? where ID = ?";
					// ��������� ������� �������
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
		/// ����� ���������� ����� �������
		/// </summary>
		/// <param name="dataRow">������ �������</param>
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
            // ���� ���� �������������� �������� � ��������� - ��������� � ��
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

		#region ������ ��� ������ � �����������

		/// <summary>
		/// �������� ID ��� ������ ���������
		/// </summary>
		/// <returns>�������� ID</returns>
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
        /// ���������� IDataUpdater ��� ������ �� ������� ����������, ����������� � ������
		/// </summary>
        /// <returns>IDataUpdater</returns>
		public IDataUpdater GetTaskDocumentsAdapter()
		{
			string queryText;
			IDbDataParameter prm;
			DataUpdater du = null;

            // �������� ID �������� ������������
            int curUser = (int)Authentication.UserID;
            bool lockByCurrentUser = (this.LockByUser == curUser);

			Database db = (Database)ParentCollection.GetDB();
			try
			{
				IDbDataAdapter adapter = db.GetDataAdapter();

				// ������� ������� ������
                // �������� ������������ �������� - 
                string filter = String.Empty;
                // ... ����� ���������
                filter = String.Format("(Ownership = {0})", (int)TaskDocumentOwnership.doGeneral);
                // ... ���� ������������ �������� - �� ��������� ���������
                if (curUser == this.Owner)
                {
                    filter = String.Format("{0} or (Ownership = {1})", filter, (int)TaskDocumentOwnership.doOwner);
                }
                // ... ���� ������������ ����������� - �� ��������� �����������
                if (curUser == this.Doer)
                {
                    filter = String.Format("{0} or (Ownership = {1})", filter, (int)TaskDocumentOwnership.doDoer);
                }
                // ... ���� ������������ ������� - �� ��������� ��������
                if (curUser == this.Curator)
                {
                    filter = String.Format("{0} or (Ownership = {1})", filter, (int)TaskDocumentOwnership.doCurator);
                }
                //
				adapter.SelectCommand = db.Connection.CreateCommand();
                // � ����������� �� ����������������� �������� ��������� �� �������� ������� ��� �� ����
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

				// ������� �������� ������
				adapter.DeleteCommand = db.Connection.CreateCommand();
				prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
				prm.SourceColumn = "ID";
				adapter.DeleteCommand.Parameters.Add(prm);
                // ������� ������ �� ����
				queryText = "delete from DocumentsTemp where ID = ?";
				adapter.DeleteCommand.CommandText = db.GetQuery(queryText, adapter.DeleteCommand.Parameters);

				// ������� ���������� ������
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

				// ��������� ������ � ����
                queryText = "update DocumentsTemp set Name = ?, Description = ?, DocumentType = ?, Ownership = ? where ID = ?";
				adapter.UpdateCommand.CommandText = db.GetQuery(queryText, adapter.UpdateCommand.Parameters);


				// ������� ������� ������
                // (��������� ������ � ���)
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
		/// �������� ����������� ����� (CRC32) ��� ���������
		/// </summary>
		/// <param name="documentID">ID ���������</param>
		/// <returns>����������� �����</returns>
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
		/// �������� ������ ���������
		/// </summary>
		/// <param name="documentID">ID ���������</param>
		/// <returns>������ ������</returns>
        private byte[] GetDocumentDataBuff(int documentID)
		{

			byte[] documentData = new byte[0];
			IDatabase db = ParentCollection.GetDB();
			try
			{
                // � ����������� �� ����������������� �������� �������� �� ���� ���� �� �������� �������
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
				// ���������� ��������� ���� �������� ���� (��� �� ��������)
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
		/// �������� �������� (������)
		/// </summary>
		/// <param name="documentID">ID ���������</param>
		/// <returns>������ � ������� ���������</returns>
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
		/// �������� ������ ���������
		/// </summary>
		/// <param name="documentID">ID ���������</param>
		/// <param name="documentData">������ � ������� ���������</param>
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
        /// �������� ������� ��������� ������
        /// </summary>
        /// <returns>DataTable � �������� ������� �������</returns>
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
            // �������������� ������������
            ParentCollection.GetUsersManager().AppendUserNameColumn(dt, "RefUsers");
            return dt;
        }

        /// <summary>
        /// ��������� ����������� ���������� �������� ������� �������������
        /// </summary>
        /// <param name="operation">��� ��������</param>
        /// <param name="raiseException">������������ ���������� � ������ ���������� ���� �������</param>
        /// <returns></returns>
        public bool CheckPermission(int operation, bool raiseException)
        {
            UsersManager um = (UsersManager)ParentCollection.GetUsersManager();
            int curUser = (int)Authentication.UserID;
            TaskState curState = ((TaskCollection)ParentCollection)._actionsManager.FindStateFromCaption(this.State);

            bool allowed = false;
            // � ����������� �� ���� �������� ��������� ������ �������������� �������, � ������� UsersManager �� �����
            switch (operation)
            {
                case (int)TaskOperations.View:
                    #region �������� ���������
                    // �������� ������ ����� ������ ������
                    /*if (curUser == this.Owner)
                    {
                        allowed = true;
                        break;
                    }
                    // ����������� ����� ������ ������ ������ � ��������� "���������"
                    if ((curUser == this.Doer) && ((int)curStates >= (int)TaskStates.tsAssigned))
                    {
                        allowed = true;
                        break;
                    }
                    // ������� ����� ������ ������ ������ � ��������� "�� ��������"
                    if ((curUser == this.Curator) && ((int)curStates >= (int)TaskStates.tsOnCheck))
                    {
                        allowed = true;
                        break;
                    }*/
                    #endregion
                    // ������ ������ ����� ��������, ����������� � �������
                    if ((curUser == this.Owner) || (curUser == this.Doer) || (curUser == this.Curator))
                    {
                        allowed = true;
                        break;
                    }
                    break;
                case (int)AllTasksOperations.AssignTaskViewPermission:
                    // �������� ����� ��������� ����� ��������� ����� ���� �����
                    allowed = (curUser == this.Owner);
                    break;
                case (int)TaskTypeOperations.AssignTaskViewPermission:
                    // ���������� ��� ����������� ����
                    allowed = (curUser == this.Owner);
                    break;
            }
            if (allowed)
                return true;

            // ���� ��� ��������������� �������������� ������� - ���� ���������� � UsersManager
            return um.CheckPermissionForTask(this.ID, this.RefTasksTypes, operation, raiseException);
        }

        /// <summary>
        /// ��������  ������ ��������� �������� � ����������� �� ��������� � ���� ������������
        /// </summary>
        /// <param name="stateCaption">��������� ���������</param>
        /// <returns>��������� ��������</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string[] GetActionsForState(string stateCaption)
        {
            // �������� ����������� ������� � ���������
            TaskActionManager aMngr = ((TaskCollection)ParentCollection)._actionsManager;
            UsersManager um = ((TaskCollection)ParentCollection).GetUsersManager();
            int curUser = (int)Authentication.UserID;
            bool isAdmin = UserInAdminGroup(curUser, um);
            // �������� ��� ��������� �������� ��� ���������
            TaskActions[] allActions = aMngr.GetActionsForState(stateCaption, isAdmin);
            ArrayList allowedActionsCaptions = new ArrayList();
            // ��������� �������� � ������������ � ������� ������������ � ��������������� ���������
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
            // ���������� �������� ����������� ��������
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
