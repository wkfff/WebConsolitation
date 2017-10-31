using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Xml;

namespace Krista.FM.ServerLibrary
{
	public interface IUpdatedDBObject : IDisposable
	{
		/// <summary>
		/// ������� ���� ��� ������ ������ ��� ������ (��� �� ���������� � ����)
		/// � ���� ������ ��� ������ ������ EndUpdate ����� ����������� ������� 
        /// �������, � �� ����������
		/// </summary>
		bool IsNew { get; }

		/// <summary>
		/// ������� ����, ��� ������ ��������� � ������ ���������� (����� BeginEdit 
        /// �� ��  EndEdit ��� CancelEdit)
		/// </summary>
		bool InEdit { get; }

		/// <summary>
		/// ������ ���������� ������� 
		/// </summary>
		void BeginUpdate(string action);

		/// <summary>
		/// ��������� ���������� �������, ������ ��������� � ����
		/// </summary>
		void EndUpdate();

		/// <summary>
		/// �������� ������� (������������ ���������� ���������)
		/// </summary>
		void CancelUpdate();
	}

	/// <summary>
	/// ��������� ������������� �������-������ ��������� �����
	/// </summary>
    public interface IRealTimeDBObjectsEnumerableCollection : IDisposable, 
        ICollection, IEnumerable
	{
	}

	/// <summary>
	/// ��������� ��������� ������
	/// </summary>
    public enum TaskStates : int { 
        tsUndefined = 0,        // ��������� ���������
        tsCreated = 1,          // �������
        tsAssigned = 2,         // ���������
        tsExecuted = 3,         // �����������
        tsOnCheck = 4,          // �� ��������
        tsCheckInProgress = 5,  // �����������
        tsFinisned = 6,         // ���������
        tsClosed = 7            // �������
    };

	/// <summary>
	/// ��������� �������� ��� �������
	/// </summary>
    public enum TaskActions : int { 
        taUndefined = 0,             // ����������� �������� (��� ��������� ������)
        taCreate = 1,                // �������
        taEdit = 2,                  // �������������
        taDelete = 3,                // �������
        taAssign = 4,                // ���������
        taExecute = 5,               // ���������
        taContinueExecute = 6,       // ���������� ����������
        taOnCheck = 7,               // �� ��������
        taCheck = 8,                 // ���������
        taContinueCheck = 9,         // ���������� ��������
        taCheckWithErrors = 10,      // �� ������ ��������     
        taCheckWithoutErrors = 11,   // ������� ���������
        taBackToRework = 12,         // ������� �� ���������
        taBackToCheck = 13,          // ������� �� ��������
        taClose = 14                 // �������
        //taCancelEdit = 15            // �������� ��������������
    };

	/// <summary>
	/// ��������� ���� ����������
	/// </summary>
    public enum TaskDocumentType : int 
    { 
        dtDummyValue = -1,          // ����� �� ����� ��� ��� �������� ������������, �� ��� ��� �� �����
        dtCalcSheet = 0,            // ��������� ����
        dtInputForm = 1,            // ����� �����
        dtReport = 2,               // �����
        dtDataCaptureList = 3,      // ����� ����� ������
        dtArbitraryDocument = 5,    // ������������ ��������
        dtWordDocument = 6,         // �������� MS Word
        dtMDXExpertDocument = 7,    // �������� MDX Expert
        dtPlanningSheet = 8,        // ���� ������������
        dtExcelDocument = 9         // �������� MS Excel
    };

    /// <summary>
    /// ��������� ���� ���������� � ���������������
    /// </summary>
    public enum ClassifierDocumentType : int
    {
        dtDummyValue = -1,
        dtCalcList = 0,             // ���������� MS Excel � ��������� ����
        dtInputForm = 1,            // ���������� MS Excel � ����� �����
        dtReport = 2,               // ���������� MS Excel � �����
        dtDataCaptureList = 3,      // ���������� MS Excel � ����� �����
        dtWordReport = 4,           // ���������� MS Word � �����
        dtArbitraryDocument = 5,    // ������������ ��������
        dtArbitraryWordDocument = 6,// ������������ �������� MS Word
        dtMDXExpertDocument = 7,    // �������� MDX Expert
        dtPlaningSheet = 8,         // ���� ������������
        dtArbitraryExcelDocument = 9,// ������������ �������� MS Excel
        dtTaskXMLDocument = 10,     // ������(�) XML ���� (� ������ XML � �������/��������)
        dtSchemeObjectXMLDocument = 11// ������ ������� XML ���� (XML �������������� � �.�.)
    };

    // �������������� ���������
    public enum TaskDocumentOwnership : int 
    {
        doGeneral = 0, // ����� ��������
        doOwner = 1,   // �������� ���������            
        doDoer = 2,    // �������� �����������
        doCurator = 3  // �������� ��������
    };

	// ��������������� ������������ ��� ���������� ���������� � ������
	public enum AddedTaskDocumentType 
    { 
        ndtExisting,            // ������������ ��������
        ndtNewExcel,            // ����� �������� MS Excel
        ndtNewWord,             // ����� �������� MS Word
        ndtNewCalcSheetExcel,   // ����� ��������� ���� (�������� ����� ������������ Excel)
        ndtNewCalcSheetWord,    // ����� ��������� ���� (�������� ����� ������������ Word)
        ndtNewMDXExpert         // ����� �������� MDX Expert
    };

    #region �������� ������� � ������������� DataT�ble
    public struct TasksNavigationTableColumns
    {
        public const string ID_COLUMN = "ID";
        public const string STATE_COLUMN = "State";
        public const string FROMDATE_COLUMN = "FromDate";
        public const string TODATE_COLUMN = "ToDate";
        public const string DOER_COLUMN = "Doer";
        public const string OWNER_COLUMN = "Owner";
        public const string CURATOR_COLUMN = "Curator";
        public const string HEADLINE_COLUMN = "HeadLine";
        public const string REFTASKS_COLUMN = "RefTasks";
    public const string REFTASKSTYPES_COLUMN = "RefTasksTypes";
        public const string LOCKBYUSER_COLUMN = "LockByUser";
        public const string JOB_COLUMN = "Job";
        public const string DESCRIPRION_COLUMN = "Description";
        public const string VISIBLE_COLUMN = "visible";
        public const string DOERNAME_COLUMN = "DoerName";
        public const string OWNERNAME_COLUMN = "OwnerName";
        public const string CURATORNAME_COLUMN = "CuratorName";
        public const string LOCKEDUSERNAME_COLUMN = "LockedUserName";
        public const string STATECODE_COLUMN = "StateCode";
    }

    #endregion

    public interface ITask : IUpdatedDBObject, ITaskContext
    {
		// ID �������
        int ID { get; }

		// ��������� ������
		string State { get; set; }

        string CashedAction { get; }

		// ���� ���������� - "�" 
		DateTime FromDate { get; set; }

		// ���� ���������� - "��"
		DateTime ToDate { get; set; }

		// ����������� (��� �������� ����� ������ ������������� ����������� �� ���� "��������")
		int Doer { get; set; }

		// ��������
		int Owner {get; set; }

        // �������
        int Curator { get; set; }

		// ���� ������
		string Headline { get; set; }

        // �������
        string Job { get; set; }

		// �������� (�����������)
		string Description { get; set; }

		// ������������ ������
        int RefTasks { get; }

        // ������������� �������������
        int LockByUser { get; set; }

        // ������ �� ���������� ����� �����
        int RefTasksTypes { get; set; }

        // �������� ������� � �������� ��������� ������
        DataTable GetTaskHistory();

        // ��������� ��������� ����� ������
        void SaveStateIntoDatabase();

        /// <summary>
        /// ��������� ������������ ������
        /// </summary>
        /// <param name="parentId"></param>
        void SetParentTask(int? parentId);

        // ������� ������ ��� ��������� ������ (��������� ������ � �������-����)
        bool PlacedInCacheOnly { get; }

        bool LockByCurrentUser();

		#region ������ ��� ������ � �����������
		/// <summary>
		/// �������� ID ��� ������ ���������
		/// </summary>
		/// <returns></returns>
		int GetNewDocumentID();

		/// <summary>
		/// �������� IDataUpdater ��� �������� �� ������� ���������� ������
		/// </summary>
        /// <returns>IDataUpdater</returns>
		IDataUpdater GetTaskDocumentsAdapter();

		/// <summary>
		/// �������� ����������� ����� (CRC32) ��� ���������
		/// </summary>
		/// <param name="documentID">ID ���������</param>
		/// <returns>����������� �����</returns>
		ulong GetDocumentCRC32(int documentID);

		/// <summary>
		/// �������� �������� (������)
		/// </summary>
		/// <param name="documentID">ID ���������</param>
		/// <returns>������ � ������� ���������</returns>
		//Stream GetDocumentData(int documentID);

        /// <summary>
        /// �������� �������� (������)
        /// </summary>
        /// <param name="documentID">ID ���������</param>
        /// <returns>������ � ������� ���������</returns>
        byte[] GetDocumentData(int documentID);

		/// <summary>
		/// �������� ������ ���������
		/// </summary>
		/// <param name="documentID">ID ���������</param>
		/// <param name="documentData">������ � ������� ���������</param>
        [Obsolete("���������� �� ������������, ������ ���� ������")]
		void SetDocumentData(int documentID, Stream documentData);

        /// <summary>
        /// �������� ������ ���������
        /// </summary>
        /// <param name="documentID">ID ���������</param>
        /// <param name="documentData">������ � ������� ���������</param>
        void SetDocumentData(int documentID, byte[] documentData);

        /// <summary>
        /// ��������� ����������� ���������� �������� ��� �������� ������������
        /// </summary>
        /// <param name="operation">��� ��������</param>
        /// <param name="raiseException">������������ �� ���������� � ������ ���������� ����</param>
        /// <returns>true/false</returns>
        bool CheckPermission(int operation, bool raiseException);

        /// <summary>
        /// �������� ������ ��������� �������� ��� �������� ���������
        /// </summary>
        /// <param name="stateCaption">��������� ���������</param>
        /// <returns>������ �� ������� ��������� ��������</returns>
        string[] GetActionsForState(string stateCaption);
		#endregion

	}

    /// <summary>
    /// ��������� ��������� �����
    /// </summary>
    public interface ITaskCollection : IRealTimeDBObjectsEnumerableCollection
    {
		/// <summary>
		/// �������� ����� ������
		/// </summary>
		/// <returns>��������� ��������� ������</returns>
        ITask AddNew();
        
        /// <summary>
        /// �������� ����� �������� ������
        /// </summary>
        /// <param name="parentTaskID">ID ������������ ������</param>
        /// <returns>��������� ��������� ������</returns>
		ITask AddNew(int parentTaskID);
        
        /// <summary>
        ///  ������� ������
        /// </summary>
        /// <param name="task">��������� ������</param>
		void DeleteTask(ITask task);

        /// <summary>
        /// ������� ������
        /// </summary>
        /// <param name="taskID">ID ������</param>
        void DeleteTask(int taskID);

        /// <summary>
        /// ������� ��������
        /// </summary>
        /// <param name="documentID">ID ���������</param>
        void DeleteDocument(int documentID);

        /// <summary>
        /// �������� ��������� ������� ����� ���������� ��������
        /// </summary>
        /// <param name="actionCaption">��������� ��������</param>
        /// <returns>��������� ���������</returns>
		string GetStateAfterAction(string actionCaption);

        /// <summary>
        /// ���������������� �������� �� ���������
        /// </summary>
        /// <param name="actionCaption">��������� ��������</param>
        /// <returns>��������</returns>
		TaskActions FindActionsFromCaption(string actionCaption);

        /// <summary>
        /// ���������������� ��������� �� ���������
        /// </summary>
        /// <param name="stateCaption">��������� ���������</param>
        /// <returns>���������</returns>
		TaskStates FindStateFromCaption(string stateCaption);

        /// <summary>
        /// �������� ������ ��������� ���������
        /// </summary>
        /// <returns>������ �����</returns>
        string[] GetAllStatesCaptions();

        /// <summary>
        /// ������ �����
        /// </summary>
		ICollection Keys { get; }
        
        /// <summary>
        /// ������ ������ �� ������������ ������
        /// </summary>
		ICollection KeyRefs { get; }

        /// <summary>
        /// ���������� ���������
        /// </summary>
        /// <param name="key">ID ������</param>
        /// <returns>��������� ������</returns>
		ITask this[int key] { get; }

        /// <summary>
        /// �������� ��������� ������������ ������
        /// </summary>
        /// <returns>DataTable c ����������� � �������</returns>
		DataTable GetTasksInfo();

        /// <summary>
        /// �������� ��������������� ������� ������������� ������
        /// </summary>
        /// <returns>DataTable c ����������� � �������</returns>
        DataTable GetCurrentUserLockedTasks();

        /// <summary>
        /// ����� ��������� �����, ��������������� ��������� ������
        /// </summary>
        /// <param name="filter">�������� ������</param>
        /// <returns>DataTable c ����������� � �������</returns>
        DataTable FindDocuments(string filter);
        
        /// <summary>
        /// �������� ��������� ������� ��� ������� �����
        /// </summary>
        /// <returns>DataTable</returns>
        DataTable GetTasksImportTable();

        #region ������/�������
        /// <summary>
        /// ������ �������. ���������� ��������������� ������ ��� ��������� ���������� � ��������
        /// </summary>
        /// <returns>DataTable c �������� ���������� ������������</returns>
        DataTable BeginExport();

        /// <summary>
        /// ��������� �������. ���������� ��� ��������������� ��������
        /// </summary>
        void EndExport();

        /// <summary>
        /// �������� ��������� ������� ��� ������� ����������
        /// </summary>
        /// <returns>DataTable</returns>
        DataTable GetDocumentsImportTable();

        /// <summary>
        /// �������� ��������� ������� ��� ������� ����������
        /// </summary>
        /// <returns></returns>
        DataTable GetParamsImportTable();

        /// <summary>
        /// ������
        /// </summary>
        /// <param name="taskTypeID"></param>
        /// <param name="taskTypeTaskType"></param>
        /// <param name="taskTypeCode"></param>
        /// <param name="taskTypeName"></param>
        /// <param name="taskTypeDescription"></param>
        /// <returns></returns>
        DataTable ImportTaskTypes(int taskTypeID, int taskTypeTaskType, int taskTypeCode, string taskTypeName,
                                  string taskTypeDescription);

        /// <summary>
        /// ������������� ������
        /// </summary>
        /// <param name="externalDb">IDatabase</param>
        /// <param name="newTasks">DataTable � �������������� ��������</param>
        void ImportTasks(IDatabase externalDb, DataTable newTasks);
        
        /// <summary>
        /// ������������� ��������� �����
        /// </summary>
        /// <param name="externalDb">IDatabase</param>
        /// <param name="newDocuments">DataTable c �������������� �����������</param>
        void ImportDocuments(IDatabase externalDb, DataTable newDocuments);

        /// <summary>
        /// ������������� ��������� �����
        /// </summary>
        void ImportParams(IDatabase externalDb, DataTable newParams, object parentTaskID);

        /// <summary>
        /// �������� ��������� � ��������� ������
        /// </summary>
        /// <param name="taskID">ID ������</param>
        /// <param name="includeParent">�������� �� � ����� �������� ������������� �����</param>
        /// <param name="consts">������� � �����������</param>
        /// <param name="parameters">������� � �����������</param>
        void GetTaskConstsParams(int taskID, bool includeParent, ref DataTable consts, ref DataTable parameters);

        void SetDocumentData(int documentID, byte[] documentData, bool updateTemp, IDatabase db);

        /// <summary>
        /// ������ ����� ������
        /// </summary>
        /// <param name="taskNode"></param>
        /// <param name="usedDate"></param>
        /// <param name="taskTypeID"></param>
        /// <param name="tasks"></param>
        /// <param name="parentTaskID"></param>
        /// <returns></returns>
        int ImportTask(string headline, string job, string description, DateTime usedDate, int taskTypeID,
                       DataTable tasks, object parentTaskID);

        /// <summary>
        /// ������ ���������� � �������� ������
        /// </summary>
        /// <param name="taskNode"></param>
        /// <param name="taskID"></param>
        /// <param name="parentTaskID"></param>
        /// <param name="paramsTable"></param>
        void ImportTaskConstsParameters(object parentTaskID, DataTable paramsTable);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="isConst"></param>
        /// <param name="importedParamsTable"></param>
        /// <param name="taskID"></param>
        void ImportTaskConstParameter(string xmlNode,
                                             bool isConst, ref DataTable importedParamsTable, int taskID);

        /// <summary>
        /// ������ ���������� ����������
        /// </summary>
        void ImportTaskDocuments(DataTable importedDocuments, Dictionary<int, byte[]> documents);

        /// <summary>
        /// ������ ��� ������ � ����� ������
        /// </summary>
        IDatabase GetTaskDB();

        void BeginDbTransaction();

        void CommitDbTransaction();

        void RollbackDbTransaction();

        /// <summary>
        /// ������������ ����������� ������� IDatabase
        /// </summary>
        void ClearDB();

        void ImportTask(Stream xmlStream, object parentTaskID);

        void ExportTask(Stream stream, int[] tasksID);

        void CopyTask(int[] tasksIdList);

        void PasteTask(int? parentID);

        void DeleteTempFile();

        #endregion
    }

    /// <summary>
    /// ��������� ��� ���������� ���������� �����
    /// </summary>
    public interface ITaskManager : IDisposable
    {
        /// <summary>
        /// ��������� �����
        /// </summary>
        ITaskCollection Tasks { get; }
    }

    #region ������������� ����������
    [ComVisible(false)]
    public enum TaskParameterType : int
    {
        taskParameter = 0, // ��������
        taskConst = 1      // ���������
    }

    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("4AF4821C-3396-320C-99CE-E92D6AD43097")]
    public interface ITaskContext
    {
        [DispId(1)]
        ITaskParamsCollection GetTaskParams();
        [DispId(2)]
        ITaskConstsCollection GetTaskConsts();
    }

    /// <summary>
    /// ������� ���������, ������������� ���������� � ����������� �������������� �������
    /// </summary>
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("6895D551-59DF-3C1C-A69A-9D73B562DF91")]
    public interface ITaskParamBase : IDisposable
    {
        /// <summary>
        /// ������� ���������������� ���������
        /// </summary>
        [DispId(1)]
        bool Inherited { get; }

        /// <summary>       
        /// ID �������
        /// </summary>
        [DispId(2)]
        int ID { get; }
    }

    /// <summary>
    /// ��������� ���������, ������� ��� ���������
    /// </summary>
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("4CE11103-9014-3547-86BF-1DFC791F9187")]
    public interface ITaskConst : ITaskParamBase
    {
        #region from ITaskParamBase
        /// <summary>
        /// ������� ���������������� ���������
        /// </summary>
        [DispId(1)]
        new bool Inherited { get; }

        /// <summary>       
        /// ID �������
        /// </summary>
        [DispId(2)]
        new int ID { get; }
        #endregion

        /// <summary>
        /// ��������
        /// </summary>
        [DispId(3)]       
        string Name { get; set; }

        /// <summary>
        /// �����������
        /// </summary>
        [DispId(4)]
        string Comment { get; set; }

        /// <summary>
        /// XML �������� �� ����������
        /// </summary>
        [DispId(5)]
        object Values { get; set; }
    }

    /// <summary>
    /// ��������� ���������, ����������� �� ���������
    /// </summary>
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("8514700B-CDDA-32E8-A067-B31CF58326CC")]
    public interface ITaskParam : ITaskConst
    {
        #region from ITaskParamBase
        /// <summary>
        /// ������� ���������������� ���������
        /// </summary>
        [DispId(1)]
        new bool Inherited { get; }

        /// <summary>       
        /// ID �������
        /// </summary>
        [DispId(2)]
        new int ID { get; }
        #endregion

        #region from ITaskConst
        /// <summary>
        /// ��������
        /// </summary>
        [DispId(3)]
        new string Name { get; set; }

        /// <summary>
        /// �����������
        /// </summary>
        [DispId(4)]
        new string Comment { get; set; }

        /// <summary>
        /// XML �������� �� ����������
        /// </summary>
        [DispId(5)]
        new object Values { get; set; }
        #endregion

        /// <summary>
        /// ���������
        /// </summary>
        [DispId(6)]
        string Dimension { get; set; }

        /// <summary>
        /// ����������� �������������� ������
        /// </summary>
        [DispId(7)]
        bool AllowMultiSelect { get; set; }
    }

    /// <summary>
    /// ������� ��������� ��� �������� � ����������
    /// </summary>
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("C572ABF7-EB6C-330C-BE92-9FD2BD14BCD7")]
    public interface ITaskItemsCollection : IDisposable
    {
        /// <summary>
        /// ������ ���������
        /// </summary>
        [ComVisible(false)]
        DataTable ItemsTable { get; }

        [ComVisible(false)]
        void SaveChanges();
        
        [ComVisible(false)]
        void CancelChanges();

        [ComVisible(false)]
        bool HasChanges();

        [ComVisible(false)]
        void ReloadItemsTable();

        /// <summary>
        /// �������� ��������� ��������� � ������ "������ ��� ������"
        /// �.�. ������������ ������ �� �������������
        /// </summary>
        [DispId(1)]
        bool IsReadOnly { get; }

        /// <summary>
        /// ���������� ����������
        /// </summary>
        [DispId(2)]
        int Count { get; }

        /// <summary>
        /// C��������������� � ������ ����������
        /// </summary>
        /// <param name="destination">��������� � ������� ����� ������������ �������������</param>
        /// <param name="errorStr">����������� ������ �������</param>
        /// <returns>true - ���� ��� ��������� ������������������, false - ���� �������� ������</returns>
        [DispId(3)]
        bool SyncWith(ITaskItemsCollection destination, out string errorStr);
    }

    /// <summary>
    /// ��������� ��������, ������������ �� �������
    /// </summary>
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("3965D873-F982-3728-BCD8-0DF1AAEA627F")]
    public interface ITaskConstsCollection : ITaskItemsCollection//, IBindingList
    {
        #region from ITaskItemsCollection
        /// <summary>
        /// �������� ��������� ��������� � ������ "������ ��� ������"
        /// �.�. ������������ ������ �� �������������
        /// </summary>
        [DispId(1)]
        new bool IsReadOnly { get; }

        /// <summary>
        /// ���������� ����������
        /// </summary>
        [DispId(2)]
        new int Count { get; }

        /// <summary>
        /// C��������������� � ������ ����������
        /// </summary>
        /// <param name="destination">��������� � ������� ����� ������������ �������������</param>
        /// <param name="errorStr">����������� ������ �������</param>
        /// <returns>true - ���� ��� ��������� ������������������, false - ���� �������� ������</returns>
        [DispId(3)]
        new bool SyncWith(ITaskItemsCollection destination, out string errorStr);
        #endregion

        /// <summary>
        /// ��������� �� �������
        /// </summary>
        /// <param name="index">������ ���������</param>
        /// <returns>��������� ���������</returns>
        [DispId(4)]
        ITaskConst ConstByIndex(int index);

        /// <summary>
        /// ��������� �� ID
        /// </summary>
        /// <param name="name">ID ���������</param>
        /// <returns>��������� ���������</returns>
        [DispId(5)]
        ITaskConst ConstByID(int id);
        
        /// <summary>
        /// ��������� �� �����
        /// </summary>
        /// <param name="name">��� ���������</param>
        /// <returns>��������� ���������</returns>
        [DispId(6)]
        ITaskConst ConstByName(string name);

        /// <summary>
        /// �������� ���������
        /// </summary>
        /// <returns></returns>
        [DispId(7)]
        ITaskConst AddNew();

        /// <summary>
        /// ������� ���������
        /// </summary>
        /// <param name="item">��������� ���������</param>
        [DispId(8)]
        void Remove(ITaskConst item);

    }

    /// <summary>
    /// ��������� ����������, ������������ �� �������
    /// </summary>
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("081D9009-90BA-35E4-9B2F-429E72AB0D37")]
    public interface ITaskParamsCollection : ITaskItemsCollection//, IBindingList
    {
        #region from ITaskItemsCollection
        /// <summary>
        /// �������� ��������� ��������� � ������ "������ ��� ������"
        /// �.�. ������������ ������ �� �������������
        /// </summary>
        [DispId(1)]
        new bool IsReadOnly { get; }

        /// <summary>
        /// ���������� ����������
        /// </summary>
        [DispId(2)]
        new int Count { get; }

        /// <summary>
        /// C��������������� � ������ ����������
        /// </summary>
        /// <param name="destination">��������� � ������� ����� ������������ �������������</param>
        /// <param name="errorStr">����������� ������ �������</param>
        /// <returns>true - ���� ��� ��������� ������������������, false - ���� �������� ������</returns>
        [DispId(3)]
        new bool SyncWith(ITaskItemsCollection destination, out string errorStr);
        #endregion

        /// <summary>
        /// �������� �� �������
        /// </summary>
        /// <param name="index">������ ���������</param>
        /// <returns>��������� ���������</returns>
        [DispId(4)]
        ITaskParam ParamByIndex(int index);

        /// <summary>
        /// �������� �� ID
        /// </summary>
        /// <param name="name">ID ���������</param>
        /// <returns>��������� ���������</returns>
        [DispId(5)]
        ITaskParam ParamByID(int id);

        /// <summary>
        /// �������� �� �����
        /// </summary>
        /// <param name="name">��� ���������</param>
        /// <returns>��������� ���������</returns>
        [DispId(6)]
        ITaskParam ParamByName(string name);

        /// <summary>
        /// �������� ����� ��������
        /// </summary>
        /// <returns>��������� ���������</returns>
        [DispId(7)]
        ITaskParam AddNew();

        /// <summary>
        /// ������� ��������
        /// </summary>
        /// <param name="item">��������� ���������</param>
        [DispId(8)]
        void Remove(ITaskParam item);

    }
    #endregion

}