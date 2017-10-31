using System;
using System.ComponentModel;
using System.Data;

namespace Krista.FM.ServerLibrary
{
	/// <summary>
	/// Возможные типы протоколов
	/// </summary>
	public enum ModulesTypes
	{
        // Закачка данных
        [Description("Закачка данных")]
        DataPumpModule = 1,
        // Операции сопоставления
        [Description("Операции сопоставления")]
        BridgeOperationsModule = 2,
        // Операции обработки многомерных хранилищ
        [Description("Операции обработки многомерных хранилищ")]
        MDProcessingModule = 3,
        // Действия пользователей
        [Description("Действия пользователей")]
        UsersOperationsModule = 4,
        // Системные события
        [Description("Системные события")]
        SystemEventsModule = 5,
        // Операции проверки данных
        [Description("Операции проверки данных")]
        ReviseDataModule = 6,
        // Операции обработки данных
        [Description("Операции обработки данных")]
        ProcessDataModule = 7,
        // Операции обработки данных
        [Description("Операции удаления данных")]
        DeleteDataModule = 8,
        // Операции предпросмотра данных
        [Description("Операции предпросмотра данных")]
        PreviewDataModule = 9,
        // Операции с классификаторами
        [Description("Операции с классификаторами")]
        ClassifiersModule = 10,
        // Аудит
        [Description("Аудит")]
        AuditModule = 11,
        // Обновление схемы
        [Description("Обновление схемы")]
        UpdateModule = 12,
        // Источники данных.
        [Description("Источники данных")]
        DataSourceModule = 13,
        // Операция перехода на новый год
        [Description("Функция перевода базы на новый год")]
        TransferDBToNewYearModule = 14,
        // Собщения
        [Description("Обмен сообщениями")]
        MessagesExchangeModule = 15
    }

    public interface IDataOperations : IDisposable
    {
        /// <summary>
        /// получение данных аудита
        /// </summary>
        /// <param name="auditData"></param>
        //void GetAuditData(ref DataTable auditData);
        void GetAuditData(ref DataTable auditData, string filter, params IDbDataParameter[] parameters);
    }

	/// <summary>
	/// Общий интерфейс для протоколов
	/// </summary>
	public interface IBaseProtocol : IDisposable
	{
		/// <summary>
		/// Получение данных протокола
		/// </summary>
		/// <param name="mt">Тип протокола</param>
		/// <param name="Filter">Фильтр</param>
		/// <param name="ProtocolData">Сами данные</param>
		void GetProtocolData(ModulesTypes mt, ref DataTable ProtocolData);
		void GetProtocolData(ModulesTypes mt, ref DataTable ProtocolData, string Filter, params IDbDataParameter[] parameters);
        /// <summary>
        /// Получение граничных дат в протоколах
        /// </summary>
        void GetProtocolsDate(ref DateTime MinDate, ref DateTime MaxDate);
        /// <summary>
        /// Получение минималный даты протокола
        /// </summary>
        DateTime MinProtocolsDate { get;}
        /// <summary>
		/// Удаление данных лога
		/// </summary>
		/// <param name="mt">Тип лога</param>
		/// <param name="sourceID">ID источника данных</param>
		/// <returns>Количество удаленных записей</returns>
		int DeleteProtocolData(ModulesTypes mt, int sourceID);
		/// <summary>
		/// Удаление данных лога
		/// </summary>
		/// <param name="mt">Тип лога</param>
		/// <param name="sourceID">ID источника данных</param>
		/// <param name="pumpHistoryID">ID закачки</param>
		/// <returns>Количество удаленных записей</returns>
		int DeleteProtocolData(ModulesTypes mt, int sourceID, int pumpHistoryID);
		/// <summary>
		/// Удаление данных лога
		/// </summary>
		/// <param name="mt">Тип лога</param>
		/// <param name="filterStr">Ограничение на удаляемые данные</param>
		/// <returns>Количество удаленных записей</returns>
		int DeleteProtocolData(ModulesTypes mt, string filterStr);
        ///<summary>
        /// Удаление заархивированных данных
        /// </summary>
        /// <param name="filterStr">Ограничение на удаляемые данные</param>
        bool DeleteProtocolArchive(string filterStr, params IDbDataParameter[] parameters);
        /// <summary>
        /// ИД операции пользователя, инициализируется при первой операции записи события.
        /// </summary>
        int UserOperationID { get; }
	}

	/// <summary>
	/// Типы событий для протокола закачки данных
	/// </summary>
	public enum DataPumpEventKind
	{
		// Начало операции закачки
		dpeStart = 101,
		// Информационное сообщение
		dpeInformation = 102,
		// Предупреждение
		dpeWarning = 103,
		// Успешное окончание операции закачки
		dpeSuccefullFinished = 104,
		// Завершение операции закачки с ошибками
		dpeFinishedWithErrors = 105,
		// Ошибка в процессе закачки
		dpeError = 106,
		// Критическая ошибка в процесе закачки (ведет к завершению процесса закачки)
		dpeCriticalError = 107,
		// Начало закачки файла 
		dpeStartFilePumping = 108,
		// Завершение закачки файла с ошибкой
		dpeFinishFilePumpWithError = 109,
		// Успешное завершение закачки файла
		dpeSuccessfullFinishFilePump = 110,
        // начало обработки источника данных
        dpeStartDataSourceProcessing = 111,
        // Завершение обработки источника данных c ошибкой
        dpeFinishDataSourceProcessingWithError = 112,
        // Успешное завершение обработки источника данных
        dpeSuccessfullFinishDataSourceProcess = 113
	}

	/// <summary>
	/// Интерфейс объекта для протоколирования операций закачек
	/// </summary>
	public interface IDataPumpProtocol : IBaseProtocol
	{
		/// <summary>
		/// Записать событие в протокол закачки данных
		/// </summary>
		/// <param name="EventKind">Тип события</param>
		/// <param name="PumpHistoryID">ИД операции закачки</param>
		/// <param name="DataSourceID">ИД источника данных</param>
		/// <param name="InfoMsg">Информационное сообщение</param>
		void WriteEventIntoDataPumpProtocol(DataPumpEventKind EventKind, int PumpHistoryID, int DataSourceID, string InfoMsg);
	}

	/// <summary>
	/// Типы событий для протоколов сопоставления
	/// </summary>
	public enum BridgeOperationsEventKind
	{
		// Начало процесса сопоставления
		boeStart = 201,
		// Информациооное сообщение
		boeInformation = 202,
		// Предупреждение
		boeWarning = 203,
		// Успешное завершение процесса сопоставления
		boeSuccefullFinished = 204,
		// Завершение процесса сопоставления с ошибками
		boeFinishedWithError = 205,
		// Ошибка при сопоставлении
		boeError = 206,
		// Критическая ошибка при сопоставлении (ведет к прекращению процесса сопоставления)
		boeCriticalError = 207,
        // начало обработки источника данных
        dpeStartDataSourceProcessing = 211,
        // Завершение обработки источника данных c ошибкой
        dpeFinishDataSourceProcessingWithError = 212,
        // Успешное завершение обработки источника данных
        dpeSuccessfullFinishDataSourceProcess = 213
	}

	/// <summary>
	/// Интерфейс объекта для протоколирования операций сопоставления классификаторов
	/// </summary>
	public interface IBridgeOperationsProtocol : IBaseProtocol
	{
		/// <summary>
		/// Записать событие в протокол операций сопоставления
		/// </summary>
		/// <param name="EventKind">Тип события</param>
		/// <param name="BridgeRoleA">Сопоставляемый классификатор</param>
		/// <param name="BridgeRoleB">Сопоставимый классификатор</param>
		/// <param name="InfoMsg">Информационное сообщение</param>
		void WriteEventIntoBridgeOperationsProtocol(BridgeOperationsEventKind EventKind, string BridgeRoleA, string BridgeRoleB, string InfoMsg, int PumpHistoryID, int DataSourceID);
	}

	/// <summary>
	/// Типы событий для протоколов обработки многомерных хранилищ
	/// </summary>
	public enum MDProcessingEventKind
	{
		// Начало процесса расчета
        [Description("Начало операции обработки многомерных хранилищ")]
        mdpeStart = 301,
		// Информациооное сообщение
        [Description("Информация в процессе обработки многомерных хранилищ")]
        mdpeInformation = 302,
		// Предупреждение
        [Description("Предупреждение")]
        mdpeWarning = 303,
        // Успешное завершение процесса расчета
        [Description("Успешное окончание операции обработки многомерных хранилищ")]
        mdpeSuccefullFinished = 304,
        // Завершение процесса расчета с ошибками
        [Description("Окончание операции обработки многомерных хранилищ с ошибкой")]
        mdpeFinishedWithError = 305,
        // Ошибка при расчете
        [Description("Ошибка в процессе обработки многомерных хранилищ")]
        mdpeError = 306,
        // Критическая ошибка при расчете (ведет к прекращению процесса расчета)
        [Description("Критическая ошибка в процессе обработки многомерных хранилищ")]
        mdpeCriticalError = 307,

        [Description("Требование на расчет")]
        InvalidateObject = 308,
	}

	/// <summary>
	/// Интерфейс объекта для протоколирования обработки многомерных хранилищ
	/// </summary>
	public interface IMDProcessingProtocol : IBaseProtocol
	{
		/// <summary>
		/// Записать событие в протокол обработки многомерных хранилищ
		/// </summary>
		/// <param name="EventKind">Тип события</param>
		/// <param name="InfoMsg">Информационное сообщение</param>
		/// <param name="MDObjectName">Имя объекта многомерной базы</param>
		void WriteEventIntoMDProcessingProtocol(MDProcessingEventKind EventKind, string InfoMsg, string MDObjectName);

        void WriteEventIntoMDProcessingProtocol(string moduleName, MDProcessingEventKind EventKind, string InfoMsg, string MDObjectName,
            string ObjectIdentifier, Krista.FM.Server.ProcessorLibrary.OlapObjectType OlapObjectType, string batchId);
	}

	/// <summary>
	/// Типы событий для протоколов действий пользователей
	/// </summary>
	public enum UsersOperationEventKind
	{
		uoeUserConnectToScheme = 401,
		uoeUserDisconnectFromScheme = 402,
        // для связи действий в закачке и протоколом пользователей
        uoeStartWorking_RefUserName = 40100,
        // Действия по изменению назначеных прав, членства в группах и т.п.
        uoeChangeUsersTable = 40301,
        uoeChangeGroupsTable = 40302, 
        uoeChangeDepartmentsTable = 40303, 
        uoeChangeOrganizationsTable = 40304,
        uoeChangeTasksTypes = 40305,
        uoeChangeMembershipsTable = 40306,
        uoeChangePermissionsTable = 40307,
        // Работа с вариантами
        uoeVariantCopy = 40401,
        uoeVariantLock = 40402,
        uoeVariantUnlok = 40403,
        uoeVariantDelete = 40404,
        // Обновление схемы
        uoeSchemeUpdate = 40501,
        // необработанные ошибки
        uoeUntilledExceptionsEvent = 40666,
        // архивирование протоколов
        uoeProtocolsToArchive = 40701
	}

	/// <summary>
	/// Интерйес объекта для протоколирования действий пользователя
	/// </summary>
	public interface IUsersOperationProtocol : IBaseProtocol
	{
        void WriteEventIntoUsersOperationProtocol(UsersOperationEventKind eventKind, string infoMsg, params string[] UserHost);
		/// <summary>
		/// Записать событие в протокол действий пользователей
		/// </summary>
		/// <param name="EvenKind">Тип события</param>
		/// <param name="UserName">Имя пользователя</param>
		/// <param name="ObjectName">Название объекта системы</param>
		/// <param name="ActionName">Действие</param>
		/// <param name="InfoMsg">Информационное сообщение</param>
        [Obsolete]
		void WriteEventIntoUsersOperationProtocol(string Module, UsersOperationEventKind EvenKind, string UserName, string ObjectName,
			string ActionName, string InfoMsg);
	}

	/// <summary>
	/// Типы системных событий
	/// </summary>
	public enum SystemEventKind
	{
		// 500
		// Информациооное сообщение
		seInformation = 502,
		// Предупреждение
		seWarning = 503,
		// Ошибка при сопоставлении
		seError = 506,
		// Критическая ошибка при сопоставлении (ведет к прекращению процесса сопоставления)
		seCriticalError = 507

	}

	/// <summary>
	/// Интерфейс объекта для протоколирования системных событий
	/// </summary>
	public interface ISystemEventsProtocol : IBaseProtocol
	{
		void WriteEventIntoSystemEventsProtocol(SystemEventKind EventKind, string InfoMsg, string ObjectName);
	}

	/// <summary>
	/// Типы событий по сверке данных
	/// </summary>
	public enum ReviseDataEventKind
	{
		// Начало операции сверки данных
		pdeStart = 601,
		// Информационное сообщение
		pdeInformation = 602,
		// Предупреждение
		pdeWarning = 603,
		// Успешное окончание операции сверки данных
		pdeSuccefullFinished = 604,
		// Завершение операции сверки данных с ошибками
		pdeFinishedWithErrors = 605,
		// Ошибка в процессе сверки данных
		pdeError = 606,
		// Критическая ошибка в процесе сверки данных (ведет к завершению процесса закачки)
		pdeCriticalError = 607,
        // начало обработки источника данных
        dpeStartDataSourceProcessing = 611,
        // Завершение обработки источника данных c ошибкой
        dpeFinishDataSourceProcessingWithError = 612,
        // Успешное завершение обработки источника данных
        dpeSuccessfullFinishDataSourceProcess = 613
	}

	/// <summary>
	/// Интерфейс объекта для протоколирования сверки данных
	/// </summary>
	public interface IReviseDataProtocol : IBaseProtocol
	{
		/// <summary>
		/// Записать событие в протокол сверки данных
		/// </summary>
		/// <param name="EventKind">Вид события</param>
		/// <param name="InfoMsg">Информационное сообщение</param>
		void WriteEventIntoReviseDataProtocol(ReviseDataEventKind EventKind, string InfoMsg, int PumpHistoryID, int DataSourceID);
	}

	/// <summary>
	/// Типы событий обработки данных
	/// </summary>
	public enum ProcessDataEventKind
	{
		// Начало операции обработки данных
		pdeStart = 701,
		// Информационное сообщение
		pdeInformation = 702,
		// Предупреждение
		pdeWarning = 703,
		// Успешное окончание операции обработки данных
		pdeSuccefullFinished = 704,
		// Завершение операции обработки данных с ошибками
		pdeFinishedWithErrors = 705,
		// Ошибка в процессе обработки данных
		pdeError = 706,
		// Критическая ошибка в процесе обработки данных (ведет к завершению процесса закачки)
		pdeCriticalError = 707,
        // начало обработки источника данных
        dpeStartDataSourceProcessing = 711,
        // Завершение обработки источника данных c ошибкой
        dpeFinishDataSourceProcessingWithError = 712,
        // Успешное завершение обработки источника данных
        dpeSuccessfullFinishDataSourceProcess = 713
	}

	/// <summary>
	/// Интерфейс объекта для протоколирования событий обработки данных
	/// </summary>
	public interface IProcessDataProtocol : IBaseProtocol
	{
		/// <summary>
		/// Записать событие в протокол обработки данных
		/// </summary>
		/// <param name="EventKind">Вид событий</param>
		/// <param name="InfoMsg">Иформационное сообщение</param>
		/// <param name="PumpHistoryID">ИД закачки</param>
		/// <param name="DataSourceID">ИД источника данных</param>
		void WriteEventIntoProcessDataProtocol(ProcessDataEventKind EventKind, string InfoMsg, int PumpHistoryID, int DataSourceID);
	}

	/// <summary>
	/// Типы событий удаления данных
	/// </summary>
	public enum DeleteDataEventKind
	{
		// Начало операции удаления данных
		ddeStart = 801,
		// Информационное сообщение
		ddeInformation = 802,
		// Предупреждение
		ddeWarning = 803,
		// Успешное окончание операции удаления данных
		ddeSuccefullFinished = 804,
		// Завершение операции удаления данных с ошибками
		ddeFinishedWithErrors = 805,
		// Ошибка в процессе удаления данных
		ddeError = 806,
		// Критическая ошибка в процесе удаления данных (ведет к завершению процесса закачки)
		ddeCriticalError = 807,
        // начало обработки источника данных
        dpeStartDataSourceProcessing = 811,
        // Завершение обработки источника данных c ошибкой
        dpeFinishDataSourceProcessingWithError = 812,
        // Успешное завершение обработки источника данных
        dpeSuccessfullFinishDataSourceProcess = 813
	}

	/// <summary>
	/// Интерфейс объекта для протоколирования операций удаления данных
	/// </summary>
	public interface IDeleteDataProtocol : IBaseProtocol
	{
		/// <summary>
		/// Записать событие в протокол удаления данных
		/// </summary>
		/// <param name="EventKind">Тип события</param>
		/// <param name="InfoMsg">Информационное сообщение</param>
		/// <param name="PumpHistoryID">ИД закачки</param>
		/// <param name="DataSourceID">ИД источника данных</param>
		void WriteEventIntoDeleteDataProtocol(DeleteDataEventKind EventKind, string InfoMsg, int PumpHistoryID, int DataSourceID);
	}

    /// <summary>
    /// Типы событий предпросмотра данных
    /// </summary>
    public enum PreviewDataEventKind
    {
        // Начало операции закачки
        dpeStart = 901,
        // Информационное сообщение
        dpeInformation = 902,
        // Предупреждение
        dpeWarning = 903,
        // Успешное окончание операции
        dpeSuccefullFinished = 904,
        // Завершение операции с ошибками
        dpeFinishedWithErrors = 905,
        // Ошибка в процессе
        dpeError = 906,
        // Критическая ошибка в процесе
        dpeCriticalError = 907,
        // Начало закачки файла 
        dpeStartFilePumping = 908,
        // Завершение закачки файла с ошибкой
        dpeFinishFilePumpWithError = 909,
        // Успешное завершение закачки файла
        dpeSuccessfullFinishFilePump = 910,
        // начало обработки источника данных
        dpeStartDataSourceProcessing = 911,
        // Завершение обработки источника данных c ошибкой
        dpeFinishDataSourceProcessingWithError = 912,
        // Успешное завершение обработки источника данных
        dpeSuccessfullFinishDataSourceProcess = 913
    }

    /// <summary>
    /// интерфейс протоколирования операций предпросмотра данных
    /// </summary>
    public interface IPreviewDataProtocol : IBaseProtocol
    {
        /// <summary>
        /// Записать событие в протокол предварительного просмотра
        /// </summary>
        /// <param name="EventKind">Тип события</param>
        /// <param name="InfoMsg">Информационное сообщение</param>
        /// <param name="PumpHistoryID">ИД закачки</param>
        /// <param name="DataSourceID">ИД источника данных</param>
        void WriteEventIntoPreviewDataProtocol(PreviewDataEventKind EventKind, int PumpHistoryID, int DataSourceID, string InfoMsg);
    }

    /// <summary>
    /// типы событий классификаторов
    /// </summary>
    public enum ClassifiersEventKind
    {
        // Информационное сообщение
        ceInformation = 1001,
        // Предупреждение
        ceWarning = 1002,
        // Ошибка в процессе
        ceError = 1003,
        // Критическая ошибка в процесе
        ceCriticalError = 1004,
        // начало установки иерархии классификатора
        ceStartHierarchySet = 1005,
        // Успешное завершение установки иерархии
        ceSuccessfullFinishHierarchySet = 1006,
        // Завершение установки иерархии с ошибкой
        ceFinishHierarchySetWithError = 1007,
        // Очищение классификатора 
        ceClearClassifierData = 1008,
        // Импортирование данных в классификатор
        ceImportClassifierData = 1009,
        // Формирование сопоставимого классификатора по классификатору данных
        ceCreateBridge = 1010,
        // Успешное окончание операции
        ceSuccefullFinished = 1011,
        // Работа с вариантами
        ceVariantCopy = 1012,
        ceVariantLock = 1013,
        ceVariantUnlok = 1014,
        ceVariantDelete = 1015
    }
    
    /// <summary>
    /// интерфейс протоколирования операций над классификаторами
    /// </summary>
    public interface IClassifiersProtocol : IBaseProtocol
    {
        /// <summary>
        /// Записать событие в протокол предварительного просмотра
        /// </summary>
        /// <param name="EventKind">Тип события</param>
        /// <param name="InfoMsg">Информационное сообщение</param>
        /// <param name="PumpHistoryID">Наименование классификатора</param>
        /// <param name="DataSourceID">ИД источника данных</param>
        void WriteEventIntoClassifierProtocol(ClassifiersEventKind eventKind, string classifier, int pumpHistoryID, int dataSourceID, int dataOperationsObjectType, string infoMsg);
    }

    /// <summary>
    /// Типы событий протокола "Обновление схемы".
    /// </summary>
    public enum UpdateSchemeEventKind
    {
        /// <summary>
        /// Информационное сообщение.
        /// </summary>
        Information = 50000,
        
        /// <summary>
        /// Начало обновления схемы.
        /// </summary>
        BeginUpdate = 50001,
        
        /// <summary>
        /// Обновление успешно закончено.
        /// </summary>
        EndUpdateSuccess = 50002,

        /// <summary>
        /// Обновление закончено с ошибкой.
        /// </summary>
        EndUpdateWithError = 50003
    }

    /// <summary>
    /// Интерфейс протоколирования операций обновления схемы.
    /// </summary>
    public interface IUpdateSchemeProtocol : IBaseProtocol
    {
        /// <summary>
        /// Записать событие в протокол.
        /// </summary>
        /// <param name="eventKind">Тип события.</param>
        /// <param name="objectFullName">Английское наименование объекта.</param>
        /// <param name="objectFullCaption">Русское наименование объекта.</param>
        /// <param name="modificationType">Тип модификации.</param>
        /// <param name="infoMsg">Информационное сообщение.</param>
        void WriteEventIntoUpdateSchemeProtocol(UpdateSchemeEventKind eventKind, string objectFullName, string objectFullCaption, ModificationTypes modificationType, string infoMsg);
    }

    /// <summary>
    /// Типы событий источника данных.
    /// </summary>
    public enum DataSourceEventKind
    {
        // Блокировка источника.
        ceSourceLock = 1016,
        // Открытие источника.
        ceSourceUnlock = 1017,
        // Удаление источника.
        ceSourceDelete = 1018
    }

    /// <summary>
    /// Типы событий при переводе базы на новый год (11xx)
    /// </summary>
    public enum TransferDBToNewYearEventKind
    {
        // Создание источника
        tnyeCreateSource = 1101,
        // Экспорт классификатора
        tnyeExportClassifierData = 1102,
        // Импорт классификатора
        tnyeImportClassifierData = 1103,
        // Требование на расчет измерения
        tnyeInvalidateDimension = 1104,
        // Требование на расчет куба
        tnyeInvalidateCube = 1105,
        // Предупреждение
        tnyeWarning = 1106,
        // Ошибка
        tnyeError = 1107,
        // Начало работы функции перевода базы на новый год
        tnyeBegin = 1108,
        // Окончание работы функции перевода базы на новый год
        tnyeEnd = 1109
    }

    /// <summary>
    /// Типы событий работы с сообщениями
    /// </summary>
    public enum MessagesEventKind
    {
        /// <summary>
        /// Создание сообщения от администратора
        /// </summary>
        mekCreateAdmMessage = 1201,

        /// <summary>
        /// Удаление сообщения
        /// </summary>
        mekDeleteMessage = 1202,

        /// <summary>
        /// Очистка неактуальных сообщений
        /// </summary>
        mekRemoveObsoleteMessages = 1203,

        /// <summary>
        /// Создание сообщения от интерфейса расчета кубов
        /// </summary>
        mekCreateCubeMessage = 1204,

        /// <summary>
        /// Создание сообщения (прочее)
        /// </summary>
        mekCreateOther = 1210,

        /// <summary>
        /// Ошибка при отправке сообщения
        /// </summary>
        mekSendError = 1211
    }

    /// <summary>
    /// Интерфейс протоколирования событий источников данных.
    /// </summary>
    public interface IDataSourceProtocol : IBaseProtocol
    {
        /// <summary>
        /// Записать событие в протокол.
        /// </summary>
        /// <param name="eventKind">Тип события.</param>
        /// <param name="dataSourceID">ID источника.</param>
        /// <param name="infoMsg">Информационное сообщение.</param>
        void WriteEventIntoDataSourceProtocol(DataSourceEventKind eventKind, int dataSourceID, string infoMsg);
    }

    /// <summary>
    /// Интерфейс протоколирования операции перехода на новый год
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