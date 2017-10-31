using System;
using System.ComponentModel;
using System.Data;

namespace Krista.FM.ServerLibrary
{
	/// <summary>
	/// ��������� ���� ����������
	/// </summary>
	public enum ModulesTypes
	{
        // ������� ������
        [Description("������� ������")]
        DataPumpModule = 1,
        // �������� �������������
        [Description("�������� �������������")]
        BridgeOperationsModule = 2,
        // �������� ��������� ����������� ��������
        [Description("�������� ��������� ����������� ��������")]
        MDProcessingModule = 3,
        // �������� �������������
        [Description("�������� �������������")]
        UsersOperationsModule = 4,
        // ��������� �������
        [Description("��������� �������")]
        SystemEventsModule = 5,
        // �������� �������� ������
        [Description("�������� �������� ������")]
        ReviseDataModule = 6,
        // �������� ��������� ������
        [Description("�������� ��������� ������")]
        ProcessDataModule = 7,
        // �������� ��������� ������
        [Description("�������� �������� ������")]
        DeleteDataModule = 8,
        // �������� ������������� ������
        [Description("�������� ������������� ������")]
        PreviewDataModule = 9,
        // �������� � ����������������
        [Description("�������� � ����������������")]
        ClassifiersModule = 10,
        // �����
        [Description("�����")]
        AuditModule = 11,
        // ���������� �����
        [Description("���������� �����")]
        UpdateModule = 12,
        // ��������� ������.
        [Description("��������� ������")]
        DataSourceModule = 13,
        // �������� �������� �� ����� ���
        [Description("������� �������� ���� �� ����� ���")]
        TransferDBToNewYearModule = 14,
        // ��������
        [Description("����� �����������")]
        MessagesExchangeModule = 15
    }

    public interface IDataOperations : IDisposable
    {
        /// <summary>
        /// ��������� ������ ������
        /// </summary>
        /// <param name="auditData"></param>
        //void GetAuditData(ref DataTable auditData);
        void GetAuditData(ref DataTable auditData, string filter, params IDbDataParameter[] parameters);
    }

	/// <summary>
	/// ����� ��������� ��� ����������
	/// </summary>
	public interface IBaseProtocol : IDisposable
	{
		/// <summary>
		/// ��������� ������ ���������
		/// </summary>
		/// <param name="mt">��� ���������</param>
		/// <param name="Filter">������</param>
		/// <param name="ProtocolData">���� ������</param>
		void GetProtocolData(ModulesTypes mt, ref DataTable ProtocolData);
		void GetProtocolData(ModulesTypes mt, ref DataTable ProtocolData, string Filter, params IDbDataParameter[] parameters);
        /// <summary>
        /// ��������� ��������� ��� � ����������
        /// </summary>
        void GetProtocolsDate(ref DateTime MinDate, ref DateTime MaxDate);
        /// <summary>
        /// ��������� ���������� ���� ���������
        /// </summary>
        DateTime MinProtocolsDate { get;}
        /// <summary>
		/// �������� ������ ����
		/// </summary>
		/// <param name="mt">��� ����</param>
		/// <param name="sourceID">ID ��������� ������</param>
		/// <returns>���������� ��������� �������</returns>
		int DeleteProtocolData(ModulesTypes mt, int sourceID);
		/// <summary>
		/// �������� ������ ����
		/// </summary>
		/// <param name="mt">��� ����</param>
		/// <param name="sourceID">ID ��������� ������</param>
		/// <param name="pumpHistoryID">ID �������</param>
		/// <returns>���������� ��������� �������</returns>
		int DeleteProtocolData(ModulesTypes mt, int sourceID, int pumpHistoryID);
		/// <summary>
		/// �������� ������ ����
		/// </summary>
		/// <param name="mt">��� ����</param>
		/// <param name="filterStr">����������� �� ��������� ������</param>
		/// <returns>���������� ��������� �������</returns>
		int DeleteProtocolData(ModulesTypes mt, string filterStr);
        ///<summary>
        /// �������� ���������������� ������
        /// </summary>
        /// <param name="filterStr">����������� �� ��������� ������</param>
        bool DeleteProtocolArchive(string filterStr, params IDbDataParameter[] parameters);
        /// <summary>
        /// �� �������� ������������, ���������������� ��� ������ �������� ������ �������.
        /// </summary>
        int UserOperationID { get; }
	}

	/// <summary>
	/// ���� ������� ��� ��������� ������� ������
	/// </summary>
	public enum DataPumpEventKind
	{
		// ������ �������� �������
		dpeStart = 101,
		// �������������� ���������
		dpeInformation = 102,
		// ��������������
		dpeWarning = 103,
		// �������� ��������� �������� �������
		dpeSuccefullFinished = 104,
		// ���������� �������� ������� � ��������
		dpeFinishedWithErrors = 105,
		// ������ � �������� �������
		dpeError = 106,
		// ����������� ������ � ������� ������� (����� � ���������� �������� �������)
		dpeCriticalError = 107,
		// ������ ������� ����� 
		dpeStartFilePumping = 108,
		// ���������� ������� ����� � �������
		dpeFinishFilePumpWithError = 109,
		// �������� ���������� ������� �����
		dpeSuccessfullFinishFilePump = 110,
        // ������ ��������� ��������� ������
        dpeStartDataSourceProcessing = 111,
        // ���������� ��������� ��������� ������ c �������
        dpeFinishDataSourceProcessingWithError = 112,
        // �������� ���������� ��������� ��������� ������
        dpeSuccessfullFinishDataSourceProcess = 113
	}

	/// <summary>
	/// ��������� ������� ��� ���������������� �������� �������
	/// </summary>
	public interface IDataPumpProtocol : IBaseProtocol
	{
		/// <summary>
		/// �������� ������� � �������� ������� ������
		/// </summary>
		/// <param name="EventKind">��� �������</param>
		/// <param name="PumpHistoryID">�� �������� �������</param>
		/// <param name="DataSourceID">�� ��������� ������</param>
		/// <param name="InfoMsg">�������������� ���������</param>
		void WriteEventIntoDataPumpProtocol(DataPumpEventKind EventKind, int PumpHistoryID, int DataSourceID, string InfoMsg);
	}

	/// <summary>
	/// ���� ������� ��� ���������� �������������
	/// </summary>
	public enum BridgeOperationsEventKind
	{
		// ������ �������� �������������
		boeStart = 201,
		// �������������� ���������
		boeInformation = 202,
		// ��������������
		boeWarning = 203,
		// �������� ���������� �������� �������������
		boeSuccefullFinished = 204,
		// ���������� �������� ������������� � ��������
		boeFinishedWithError = 205,
		// ������ ��� �������������
		boeError = 206,
		// ����������� ������ ��� ������������� (����� � ����������� �������� �������������)
		boeCriticalError = 207,
        // ������ ��������� ��������� ������
        dpeStartDataSourceProcessing = 211,
        // ���������� ��������� ��������� ������ c �������
        dpeFinishDataSourceProcessingWithError = 212,
        // �������� ���������� ��������� ��������� ������
        dpeSuccessfullFinishDataSourceProcess = 213
	}

	/// <summary>
	/// ��������� ������� ��� ���������������� �������� ������������� ���������������
	/// </summary>
	public interface IBridgeOperationsProtocol : IBaseProtocol
	{
		/// <summary>
		/// �������� ������� � �������� �������� �������������
		/// </summary>
		/// <param name="EventKind">��� �������</param>
		/// <param name="BridgeRoleA">�������������� �������������</param>
		/// <param name="BridgeRoleB">������������ �������������</param>
		/// <param name="InfoMsg">�������������� ���������</param>
		void WriteEventIntoBridgeOperationsProtocol(BridgeOperationsEventKind EventKind, string BridgeRoleA, string BridgeRoleB, string InfoMsg, int PumpHistoryID, int DataSourceID);
	}

	/// <summary>
	/// ���� ������� ��� ���������� ��������� ����������� ��������
	/// </summary>
	public enum MDProcessingEventKind
	{
		// ������ �������� �������
        [Description("������ �������� ��������� ����������� ��������")]
        mdpeStart = 301,
		// �������������� ���������
        [Description("���������� � �������� ��������� ����������� ��������")]
        mdpeInformation = 302,
		// ��������������
        [Description("��������������")]
        mdpeWarning = 303,
        // �������� ���������� �������� �������
        [Description("�������� ��������� �������� ��������� ����������� ��������")]
        mdpeSuccefullFinished = 304,
        // ���������� �������� ������� � ��������
        [Description("��������� �������� ��������� ����������� �������� � �������")]
        mdpeFinishedWithError = 305,
        // ������ ��� �������
        [Description("������ � �������� ��������� ����������� ��������")]
        mdpeError = 306,
        // ����������� ������ ��� ������� (����� � ����������� �������� �������)
        [Description("����������� ������ � �������� ��������� ����������� ��������")]
        mdpeCriticalError = 307,

        [Description("���������� �� ������")]
        InvalidateObject = 308,
	}

	/// <summary>
	/// ��������� ������� ��� ���������������� ��������� ����������� ��������
	/// </summary>
	public interface IMDProcessingProtocol : IBaseProtocol
	{
		/// <summary>
		/// �������� ������� � �������� ��������� ����������� ��������
		/// </summary>
		/// <param name="EventKind">��� �������</param>
		/// <param name="InfoMsg">�������������� ���������</param>
		/// <param name="MDObjectName">��� ������� ����������� ����</param>
		void WriteEventIntoMDProcessingProtocol(MDProcessingEventKind EventKind, string InfoMsg, string MDObjectName);

        void WriteEventIntoMDProcessingProtocol(string moduleName, MDProcessingEventKind EventKind, string InfoMsg, string MDObjectName,
            string ObjectIdentifier, Krista.FM.Server.ProcessorLibrary.OlapObjectType OlapObjectType, string batchId);
	}

	/// <summary>
	/// ���� ������� ��� ���������� �������� �������������
	/// </summary>
	public enum UsersOperationEventKind
	{
		uoeUserConnectToScheme = 401,
		uoeUserDisconnectFromScheme = 402,
        // ��� ����� �������� � ������� � ���������� �������������
        uoeStartWorking_RefUserName = 40100,
        // �������� �� ��������� ���������� ����, �������� � ������� � �.�.
        uoeChangeUsersTable = 40301,
        uoeChangeGroupsTable = 40302, 
        uoeChangeDepartmentsTable = 40303, 
        uoeChangeOrganizationsTable = 40304,
        uoeChangeTasksTypes = 40305,
        uoeChangeMembershipsTable = 40306,
        uoeChangePermissionsTable = 40307,
        // ������ � ����������
        uoeVariantCopy = 40401,
        uoeVariantLock = 40402,
        uoeVariantUnlok = 40403,
        uoeVariantDelete = 40404,
        // ���������� �����
        uoeSchemeUpdate = 40501,
        // �������������� ������
        uoeUntilledExceptionsEvent = 40666,
        // ������������� ����������
        uoeProtocolsToArchive = 40701
	}

	/// <summary>
	/// �������� ������� ��� ���������������� �������� ������������
	/// </summary>
	public interface IUsersOperationProtocol : IBaseProtocol
	{
        void WriteEventIntoUsersOperationProtocol(UsersOperationEventKind eventKind, string infoMsg, params string[] UserHost);
		/// <summary>
		/// �������� ������� � �������� �������� �������������
		/// </summary>
		/// <param name="EvenKind">��� �������</param>
		/// <param name="UserName">��� ������������</param>
		/// <param name="ObjectName">�������� ������� �������</param>
		/// <param name="ActionName">��������</param>
		/// <param name="InfoMsg">�������������� ���������</param>
        [Obsolete]
		void WriteEventIntoUsersOperationProtocol(string Module, UsersOperationEventKind EvenKind, string UserName, string ObjectName,
			string ActionName, string InfoMsg);
	}

	/// <summary>
	/// ���� ��������� �������
	/// </summary>
	public enum SystemEventKind
	{
		// 500
		// �������������� ���������
		seInformation = 502,
		// ��������������
		seWarning = 503,
		// ������ ��� �������������
		seError = 506,
		// ����������� ������ ��� ������������� (����� � ����������� �������� �������������)
		seCriticalError = 507

	}

	/// <summary>
	/// ��������� ������� ��� ���������������� ��������� �������
	/// </summary>
	public interface ISystemEventsProtocol : IBaseProtocol
	{
		void WriteEventIntoSystemEventsProtocol(SystemEventKind EventKind, string InfoMsg, string ObjectName);
	}

	/// <summary>
	/// ���� ������� �� ������ ������
	/// </summary>
	public enum ReviseDataEventKind
	{
		// ������ �������� ������ ������
		pdeStart = 601,
		// �������������� ���������
		pdeInformation = 602,
		// ��������������
		pdeWarning = 603,
		// �������� ��������� �������� ������ ������
		pdeSuccefullFinished = 604,
		// ���������� �������� ������ ������ � ��������
		pdeFinishedWithErrors = 605,
		// ������ � �������� ������ ������
		pdeError = 606,
		// ����������� ������ � ������� ������ ������ (����� � ���������� �������� �������)
		pdeCriticalError = 607,
        // ������ ��������� ��������� ������
        dpeStartDataSourceProcessing = 611,
        // ���������� ��������� ��������� ������ c �������
        dpeFinishDataSourceProcessingWithError = 612,
        // �������� ���������� ��������� ��������� ������
        dpeSuccessfullFinishDataSourceProcess = 613
	}

	/// <summary>
	/// ��������� ������� ��� ���������������� ������ ������
	/// </summary>
	public interface IReviseDataProtocol : IBaseProtocol
	{
		/// <summary>
		/// �������� ������� � �������� ������ ������
		/// </summary>
		/// <param name="EventKind">��� �������</param>
		/// <param name="InfoMsg">�������������� ���������</param>
		void WriteEventIntoReviseDataProtocol(ReviseDataEventKind EventKind, string InfoMsg, int PumpHistoryID, int DataSourceID);
	}

	/// <summary>
	/// ���� ������� ��������� ������
	/// </summary>
	public enum ProcessDataEventKind
	{
		// ������ �������� ��������� ������
		pdeStart = 701,
		// �������������� ���������
		pdeInformation = 702,
		// ��������������
		pdeWarning = 703,
		// �������� ��������� �������� ��������� ������
		pdeSuccefullFinished = 704,
		// ���������� �������� ��������� ������ � ��������
		pdeFinishedWithErrors = 705,
		// ������ � �������� ��������� ������
		pdeError = 706,
		// ����������� ������ � ������� ��������� ������ (����� � ���������� �������� �������)
		pdeCriticalError = 707,
        // ������ ��������� ��������� ������
        dpeStartDataSourceProcessing = 711,
        // ���������� ��������� ��������� ������ c �������
        dpeFinishDataSourceProcessingWithError = 712,
        // �������� ���������� ��������� ��������� ������
        dpeSuccessfullFinishDataSourceProcess = 713
	}

	/// <summary>
	/// ��������� ������� ��� ���������������� ������� ��������� ������
	/// </summary>
	public interface IProcessDataProtocol : IBaseProtocol
	{
		/// <summary>
		/// �������� ������� � �������� ��������� ������
		/// </summary>
		/// <param name="EventKind">��� �������</param>
		/// <param name="InfoMsg">������������� ���������</param>
		/// <param name="PumpHistoryID">�� �������</param>
		/// <param name="DataSourceID">�� ��������� ������</param>
		void WriteEventIntoProcessDataProtocol(ProcessDataEventKind EventKind, string InfoMsg, int PumpHistoryID, int DataSourceID);
	}

	/// <summary>
	/// ���� ������� �������� ������
	/// </summary>
	public enum DeleteDataEventKind
	{
		// ������ �������� �������� ������
		ddeStart = 801,
		// �������������� ���������
		ddeInformation = 802,
		// ��������������
		ddeWarning = 803,
		// �������� ��������� �������� �������� ������
		ddeSuccefullFinished = 804,
		// ���������� �������� �������� ������ � ��������
		ddeFinishedWithErrors = 805,
		// ������ � �������� �������� ������
		ddeError = 806,
		// ����������� ������ � ������� �������� ������ (����� � ���������� �������� �������)
		ddeCriticalError = 807,
        // ������ ��������� ��������� ������
        dpeStartDataSourceProcessing = 811,
        // ���������� ��������� ��������� ������ c �������
        dpeFinishDataSourceProcessingWithError = 812,
        // �������� ���������� ��������� ��������� ������
        dpeSuccessfullFinishDataSourceProcess = 813
	}

	/// <summary>
	/// ��������� ������� ��� ���������������� �������� �������� ������
	/// </summary>
	public interface IDeleteDataProtocol : IBaseProtocol
	{
		/// <summary>
		/// �������� ������� � �������� �������� ������
		/// </summary>
		/// <param name="EventKind">��� �������</param>
		/// <param name="InfoMsg">�������������� ���������</param>
		/// <param name="PumpHistoryID">�� �������</param>
		/// <param name="DataSourceID">�� ��������� ������</param>
		void WriteEventIntoDeleteDataProtocol(DeleteDataEventKind EventKind, string InfoMsg, int PumpHistoryID, int DataSourceID);
	}

    /// <summary>
    /// ���� ������� ������������� ������
    /// </summary>
    public enum PreviewDataEventKind
    {
        // ������ �������� �������
        dpeStart = 901,
        // �������������� ���������
        dpeInformation = 902,
        // ��������������
        dpeWarning = 903,
        // �������� ��������� ��������
        dpeSuccefullFinished = 904,
        // ���������� �������� � ��������
        dpeFinishedWithErrors = 905,
        // ������ � ��������
        dpeError = 906,
        // ����������� ������ � �������
        dpeCriticalError = 907,
        // ������ ������� ����� 
        dpeStartFilePumping = 908,
        // ���������� ������� ����� � �������
        dpeFinishFilePumpWithError = 909,
        // �������� ���������� ������� �����
        dpeSuccessfullFinishFilePump = 910,
        // ������ ��������� ��������� ������
        dpeStartDataSourceProcessing = 911,
        // ���������� ��������� ��������� ������ c �������
        dpeFinishDataSourceProcessingWithError = 912,
        // �������� ���������� ��������� ��������� ������
        dpeSuccessfullFinishDataSourceProcess = 913
    }

    /// <summary>
    /// ��������� ���������������� �������� ������������� ������
    /// </summary>
    public interface IPreviewDataProtocol : IBaseProtocol
    {
        /// <summary>
        /// �������� ������� � �������� ���������������� ���������
        /// </summary>
        /// <param name="EventKind">��� �������</param>
        /// <param name="InfoMsg">�������������� ���������</param>
        /// <param name="PumpHistoryID">�� �������</param>
        /// <param name="DataSourceID">�� ��������� ������</param>
        void WriteEventIntoPreviewDataProtocol(PreviewDataEventKind EventKind, int PumpHistoryID, int DataSourceID, string InfoMsg);
    }

    /// <summary>
    /// ���� ������� ���������������
    /// </summary>
    public enum ClassifiersEventKind
    {
        // �������������� ���������
        ceInformation = 1001,
        // ��������������
        ceWarning = 1002,
        // ������ � ��������
        ceError = 1003,
        // ����������� ������ � �������
        ceCriticalError = 1004,
        // ������ ��������� �������� ��������������
        ceStartHierarchySet = 1005,
        // �������� ���������� ��������� ��������
        ceSuccessfullFinishHierarchySet = 1006,
        // ���������� ��������� �������� � �������
        ceFinishHierarchySetWithError = 1007,
        // �������� �������������� 
        ceClearClassifierData = 1008,
        // �������������� ������ � �������������
        ceImportClassifierData = 1009,
        // ������������ ������������� �������������� �� �������������� ������
        ceCreateBridge = 1010,
        // �������� ��������� ��������
        ceSuccefullFinished = 1011,
        // ������ � ����������
        ceVariantCopy = 1012,
        ceVariantLock = 1013,
        ceVariantUnlok = 1014,
        ceVariantDelete = 1015
    }
    
    /// <summary>
    /// ��������� ���������������� �������� ��� ����������������
    /// </summary>
    public interface IClassifiersProtocol : IBaseProtocol
    {
        /// <summary>
        /// �������� ������� � �������� ���������������� ���������
        /// </summary>
        /// <param name="EventKind">��� �������</param>
        /// <param name="InfoMsg">�������������� ���������</param>
        /// <param name="PumpHistoryID">������������ ��������������</param>
        /// <param name="DataSourceID">�� ��������� ������</param>
        void WriteEventIntoClassifierProtocol(ClassifiersEventKind eventKind, string classifier, int pumpHistoryID, int dataSourceID, int dataOperationsObjectType, string infoMsg);
    }

    /// <summary>
    /// ���� ������� ��������� "���������� �����".
    /// </summary>
    public enum UpdateSchemeEventKind
    {
        /// <summary>
        /// �������������� ���������.
        /// </summary>
        Information = 50000,
        
        /// <summary>
        /// ������ ���������� �����.
        /// </summary>
        BeginUpdate = 50001,
        
        /// <summary>
        /// ���������� ������� ���������.
        /// </summary>
        EndUpdateSuccess = 50002,

        /// <summary>
        /// ���������� ��������� � �������.
        /// </summary>
        EndUpdateWithError = 50003
    }

    /// <summary>
    /// ��������� ���������������� �������� ���������� �����.
    /// </summary>
    public interface IUpdateSchemeProtocol : IBaseProtocol
    {
        /// <summary>
        /// �������� ������� � ��������.
        /// </summary>
        /// <param name="eventKind">��� �������.</param>
        /// <param name="objectFullName">���������� ������������ �������.</param>
        /// <param name="objectFullCaption">������� ������������ �������.</param>
        /// <param name="modificationType">��� �����������.</param>
        /// <param name="infoMsg">�������������� ���������.</param>
        void WriteEventIntoUpdateSchemeProtocol(UpdateSchemeEventKind eventKind, string objectFullName, string objectFullCaption, ModificationTypes modificationType, string infoMsg);
    }

    /// <summary>
    /// ���� ������� ��������� ������.
    /// </summary>
    public enum DataSourceEventKind
    {
        // ���������� ���������.
        ceSourceLock = 1016,
        // �������� ���������.
        ceSourceUnlock = 1017,
        // �������� ���������.
        ceSourceDelete = 1018
    }

    /// <summary>
    /// ���� ������� ��� �������� ���� �� ����� ��� (11xx)
    /// </summary>
    public enum TransferDBToNewYearEventKind
    {
        // �������� ���������
        tnyeCreateSource = 1101,
        // ������� ��������������
        tnyeExportClassifierData = 1102,
        // ������ ��������������
        tnyeImportClassifierData = 1103,
        // ���������� �� ������ ���������
        tnyeInvalidateDimension = 1104,
        // ���������� �� ������ ����
        tnyeInvalidateCube = 1105,
        // ��������������
        tnyeWarning = 1106,
        // ������
        tnyeError = 1107,
        // ������ ������ ������� �������� ���� �� ����� ���
        tnyeBegin = 1108,
        // ��������� ������ ������� �������� ���� �� ����� ���
        tnyeEnd = 1109
    }

    /// <summary>
    /// ���� ������� ������ � �����������
    /// </summary>
    public enum MessagesEventKind
    {
        /// <summary>
        /// �������� ��������� �� ��������������
        /// </summary>
        mekCreateAdmMessage = 1201,

        /// <summary>
        /// �������� ���������
        /// </summary>
        mekDeleteMessage = 1202,

        /// <summary>
        /// ������� ������������ ���������
        /// </summary>
        mekRemoveObsoleteMessages = 1203,

        /// <summary>
        /// �������� ��������� �� ���������� ������� �����
        /// </summary>
        mekCreateCubeMessage = 1204,

        /// <summary>
        /// �������� ��������� (������)
        /// </summary>
        mekCreateOther = 1210,

        /// <summary>
        /// ������ ��� �������� ���������
        /// </summary>
        mekSendError = 1211
    }

    /// <summary>
    /// ��������� ���������������� ������� ���������� ������.
    /// </summary>
    public interface IDataSourceProtocol : IBaseProtocol
    {
        /// <summary>
        /// �������� ������� � ��������.
        /// </summary>
        /// <param name="eventKind">��� �������.</param>
        /// <param name="dataSourceID">ID ���������.</param>
        /// <param name="infoMsg">�������������� ���������.</param>
        void WriteEventIntoDataSourceProtocol(DataSourceEventKind eventKind, int dataSourceID, string infoMsg);
    }

    /// <summary>
    /// ��������� ���������������� �������� �������� �� ����� ���
    /// </summary>
    public interface ITransferDBToNewYearProtocol : IBaseProtocol
    {
        void WriteEventIntoTransferDBToNewYearProtocol(TransferDBToNewYearEventKind eventKind, int dataSourceID,
                                                       string infoMsg);
    }

    public interface IMessageExchangeProtocol : IBaseProtocol
    {
        void WriteEventIntoMessageExchangeProtocol(MessagesEventKind eventKind, string infoMsg);
    }
}