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
		/// Признак того что объект только что создан (еще не существует в базе)
		/// в этом случае при вызове метода EndUpdate будет выполняться команда 
        /// вставки, а не обновления
		/// </summary>
		bool IsNew { get; }

		/// <summary>
		/// Признак того, что объект находится в режиме обновления (после BeginEdit 
        /// но до  EndEdit или CancelEdit)
		/// </summary>
		bool InEdit { get; }

		/// <summary>
		/// Начать обновление объекта 
		/// </summary>
		void BeginUpdate(string action);

		/// <summary>
		/// Завершить обновление объекта, внести изменения в базу
		/// </summary>
		void EndUpdate();

		/// <summary>
		/// Отменить измения (восстановить предыдущее состояние)
		/// </summary>
		void CancelUpdate();
	}

	/// <summary>
	/// Интерфейс абстарактного объекта-предка коллекции задач
	/// </summary>
    public interface IRealTimeDBObjectsEnumerableCollection : IDisposable, 
        ICollection, IEnumerable
	{
	}

	/// <summary>
	/// Возможные состояния задачи
	/// </summary>
    public enum TaskStates : int { 
        tsUndefined = 0,        // начальное состояние
        tsCreated = 1,          // создана
        tsAssigned = 2,         // назначена
        tsExecuted = 3,         // выполняется
        tsOnCheck = 4,          // на проверку
        tsCheckInProgress = 5,  // проверяется
        tsFinisned = 6,         // завершена
        tsClosed = 7            // закрыта
    };

	/// <summary>
	/// Возможные действия над задачей
	/// </summary>
    public enum TaskActions : int { 
        taUndefined = 0,             // неизвестное действие (для обработки ошибок)
        taCreate = 1,                // создать
        taEdit = 2,                  // редактировать
        taDelete = 3,                // удалить
        taAssign = 4,                // назначить
        taExecute = 5,               // выполнять
        taContinueExecute = 6,       // продолжить выполнение
        taOnCheck = 7,               // на проверку
        taCheck = 8,                 // проверять
        taContinueCheck = 9,         // продолжить проверку
        taCheckWithErrors = 10,      // не прошло проверку     
        taCheckWithoutErrors = 11,   // успешно проверено
        taBackToRework = 12,         // вернуть на доработку
        taBackToCheck = 13,          // вернуть на проверку
        taClose = 14                 // закрыть
        //taCancelEdit = 15            // отменить редактирование
    };

	/// <summary>
	/// Возможные типы документов
	/// </summary>
    public enum TaskDocumentType : int 
    { 
        dtDummyValue = -1,          // когда мы знаем что это документ планирования, но тип нам не важен
        dtCalcSheet = 0,            // расчетный лист
        dtInputForm = 1,            // форма ввода
        dtReport = 2,               // отчет
        dtDataCaptureList = 3,      // форма сбора данных
        dtArbitraryDocument = 5,    // произвольный документ
        dtWordDocument = 6,         // документ MS Word
        dtMDXExpertDocument = 7,    // документ MDX Expert
        dtPlanningSheet = 8,        // лист планирования
        dtExcelDocument = 9         // документ MS Excel
    };

    /// <summary>
    /// возможные типы документов в классификаторах
    /// </summary>
    public enum ClassifierDocumentType : int
    {
        dtDummyValue = -1,
        dtCalcList = 0,             // Надстройка MS Excel – расчетный лист
        dtInputForm = 1,            // Надстройка MS Excel – форма ввода
        dtReport = 2,               // Надстройка MS Excel – отчет
        dtDataCaptureList = 3,      // Надстройка MS Excel – форма сбора
        dtWordReport = 4,           // Надстройка MS Word – отчет
        dtArbitraryDocument = 5,    // Произвольный документ
        dtArbitraryWordDocument = 6,// Произвольный документ MS Word
        dtMDXExpertDocument = 7,    // Документ MDX Expert
        dtPlaningSheet = 8,         // Лист планирования
        dtArbitraryExcelDocument = 9,// Произвольный документ MS Excel
        dtTaskXMLDocument = 10,     // Задача(и) XML файл (в смысле XML с задачей/задачами)
        dtSchemeObjectXMLDocument = 11// Объект системы XML файл (XML классификатора и т.п.)
    };

    // принадлежность документа
    public enum TaskDocumentOwnership : int 
    {
        doGeneral = 0, // Общий документ
        doOwner = 1,   // Документ владельца            
        doDoer = 2,    // Документ исполнителя
        doCurator = 3  // Документ куратора
    };

	// вспомогательное перечисление для добавления документов в задачи
	public enum AddedTaskDocumentType 
    { 
        ndtExisting,            // существующий документ
        ndtNewExcel,            // новый документ MS Excel
        ndtNewWord,             // новый документ MS Word
        ndtNewCalcSheetExcel,   // новый расчетный лист (документ листа планирования Excel)
        ndtNewCalcSheetWord,    // новый расчетный лист (документ листа планирования Word)
        ndtNewMDXExpert         // новый документ MDX Expert
    };

    #region Названия колонок в навигационном DataTаble
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
		// ID объекта
        int ID { get; }

		// Состояние задачи
		string State { get; set; }

        string CashedAction { get; }

		// Срок исполнения - "с" 
		DateTime FromDate { get; set; }

		// Срок исполнения - "по"
		DateTime ToDate { get; set; }

		// Исполнитель (при создании новой задачи автоматически заполняется из поля "Владелец")
		int Doer { get; set; }

		// Владелец
		int Owner {get; set; }

        // Куратор
        int Curator { get; set; }

		// Суть задачи
		string Headline { get; set; }

        // Задание
        string Job { get; set; }

		// Описание (комментарий)
		string Description { get; set; }

		// Родительская задача
        int RefTasks { get; }

        // заблокировано пользователем
        int LockByUser { get; set; }

        // ссылка на справочник видов задач
        int RefTasksTypes { get; set; }

        // получить таблицу с историей изменения задачи
        DataTable GetTaskHistory();

        // сохранить состояние полей задачи
        void SaveStateIntoDatabase();

        /// <summary>
        /// изменение родительской задачи
        /// </summary>
        /// <param name="parentId"></param>
        void SetParentTask(int? parentId);

        // признак только что созданной задачи (находится только с таблице-кэше)
        bool PlacedInCacheOnly { get; }

        bool LockByCurrentUser();

		#region Методы для работы с документами
		/// <summary>
		/// получить ID для нового документа
		/// </summary>
		/// <returns></returns>
		int GetNewDocumentID();

		/// <summary>
		/// Получить IDataUpdater для операций со списком документов задачи
		/// </summary>
        /// <returns>IDataUpdater</returns>
		IDataUpdater GetTaskDocumentsAdapter();

		/// <summary>
		/// Получить контрольную сумму (CRC32) для документа
		/// </summary>
		/// <param name="documentID">ID документа</param>
		/// <returns>контрольная сумма</returns>
		ulong GetDocumentCRC32(int documentID);

		/// <summary>
		/// Получить документ (данные)
		/// </summary>
		/// <param name="documentID">ID документа</param>
		/// <returns>массив с данными документа</returns>
		//Stream GetDocumentData(int documentID);

        /// <summary>
        /// Получить документ (данные)
        /// </summary>
        /// <param name="documentID">ID документа</param>
        /// <returns>массив с данными документа</returns>
        byte[] GetDocumentData(int documentID);

		/// <summary>
		/// Изменить данные документа
		/// </summary>
		/// <param name="documentID">ID документа</param>
		/// <param name="documentData">массив с данными документа</param>
        [Obsolete("Желательно не использовать, сильно жрет память")]
		void SetDocumentData(int documentID, Stream documentData);

        /// <summary>
        /// Изменить данные документа
        /// </summary>
        /// <param name="documentID">ID документа</param>
        /// <param name="documentData">массив с данными документа</param>
        void SetDocumentData(int documentID, byte[] documentData);

        /// <summary>
        /// Проверить возможность выполнения действия для текущего пользователя
        /// </summary>
        /// <param name="operation">код действия</param>
        /// <param name="raiseException">генерировать ли исключение в случае отсутствия прав</param>
        /// <returns>true/false</returns>
        bool CheckPermission(int operation, bool raiseException);

        /// <summary>
        /// Получить список возможных действия для текущего состояния
        /// </summary>
        /// <param name="stateCaption">Заголовок состояния</param>
        /// <returns>Массив со списком возможных действий</returns>
        string[] GetActionsForState(string stateCaption);
		#endregion

	}

    /// <summary>
    /// Интерфейс коллекции задач
    /// </summary>
    public interface ITaskCollection : IRealTimeDBObjectsEnumerableCollection
    {
		/// <summary>
		/// Добавить новую задачу
		/// </summary>
		/// <returns>интерфейс созданной задачи</returns>
        ITask AddNew();
        
        /// <summary>
        /// Добавить новую дочернюю задачу
        /// </summary>
        /// <param name="parentTaskID">ID родительской задачи</param>
        /// <returns>интерфейс созданной задачи</returns>
		ITask AddNew(int parentTaskID);
        
        /// <summary>
        ///  Удалить задачу
        /// </summary>
        /// <param name="task">интерфейс задачи</param>
		void DeleteTask(ITask task);

        /// <summary>
        /// Удалить задачу
        /// </summary>
        /// <param name="taskID">ID задачи</param>
        void DeleteTask(int taskID);

        /// <summary>
        /// Удалить документ
        /// </summary>
        /// <param name="documentID">ID документа</param>
        void DeleteDocument(int documentID);

        /// <summary>
        /// Получить состояние объекта после выполнения действия
        /// </summary>
        /// <param name="actionCaption">Заголовок действия</param>
        /// <returns>Заголовок состояния</returns>
		string GetStateAfterAction(string actionCaption);

        /// <summary>
        /// Идентифицировать действие по заголовку
        /// </summary>
        /// <param name="actionCaption">заголовок действия</param>
        /// <returns>действие</returns>
		TaskActions FindActionsFromCaption(string actionCaption);

        /// <summary>
        /// Идентифицировать состояние по заголовку
        /// </summary>
        /// <param name="stateCaption">заголовок состояния</param>
        /// <returns>состояние</returns>
		TaskStates FindStateFromCaption(string stateCaption);

        /// <summary>
        /// Получить список возможных состояний
        /// </summary>
        /// <returns>массив строк</returns>
        string[] GetAllStatesCaptions();

        /// <summary>
        /// Список задач
        /// </summary>
		ICollection Keys { get; }
        
        /// <summary>
        /// Список ссылок на родительские задачи
        /// </summary>
		ICollection KeyRefs { get; }

        /// <summary>
        /// Индексатор коллекции
        /// </summary>
        /// <param name="key">ID задачи</param>
        /// <returns>интерфейс задачи</returns>
		ITask this[int key] { get; }

        /// <summary>
        /// Получить доступные пользователю задачи
        /// </summary>
        /// <returns>DataTable c информацией о задачах</returns>
		DataTable GetTasksInfo();

        /// <summary>
        /// Получить заблокированные текущим пользователем задачи
        /// </summary>
        /// <returns>DataTable c информацией о задачах</returns>
        DataTable GetCurrentUserLockedTasks();

        /// <summary>
        /// Найти документы задач, удовлетворяющие критериям поиска
        /// </summary>
        /// <param name="filter">критерии поиска</param>
        /// <returns>DataTable c информацией о задачах</returns>
        DataTable FindDocuments(string filter);
        
        /// <summary>
        /// Получить заготовку таблицы для импорта задач
        /// </summary>
        /// <returns>DataTable</returns>
        DataTable GetTasksImportTable();

        #region Импорт/Экспорт
        /// <summary>
        /// Начать экспорт. Кэшируются вспомогательные данные для получения параметров и констант
        /// </summary>
        /// <returns>DataTable c задачами доступными пользователю</returns>
        DataTable BeginExport();

        /// <summary>
        /// Завершить экспорт. Освободить кэш вспомогательных структур
        /// </summary>
        void EndExport();

        /// <summary>
        /// Получить заготовку таблицы для импорта документов
        /// </summary>
        /// <returns>DataTable</returns>
        DataTable GetDocumentsImportTable();

        /// <summary>
        /// Получить заготовку таблицы для импорта параметров
        /// </summary>
        /// <returns></returns>
        DataTable GetParamsImportTable();

        /// <summary>
        /// Импорт
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
        /// Испортировать задачи
        /// </summary>
        /// <param name="externalDb">IDatabase</param>
        /// <param name="newTasks">DataTable с импортируемыми задачами</param>
        void ImportTasks(IDatabase externalDb, DataTable newTasks);
        
        /// <summary>
        /// Импортировать документы задач
        /// </summary>
        /// <param name="externalDb">IDatabase</param>
        /// <param name="newDocuments">DataTable c импортируемыми документами</param>
        void ImportDocuments(IDatabase externalDb, DataTable newDocuments);

        /// <summary>
        /// Импортировать параметры задач
        /// </summary>
        void ImportParams(IDatabase externalDb, DataTable newParams, object parentTaskID);

        /// <summary>
        /// Получить константы и параметры задачи
        /// </summary>
        /// <param name="taskID">ID задачи</param>
        /// <param name="includeParent">включать ли в набор элементы ародительских задач</param>
        /// <param name="consts">таблица с константами</param>
        /// <param name="parameters">таблица с параметрами</param>
        void GetTaskConstsParams(int taskID, bool includeParent, ref DataTable consts, ref DataTable parameters);

        void SetDocumentData(int documentID, byte[] documentData, bool updateTemp, IDatabase db);

        /// <summary>
        /// импорт одной задачи
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
        /// импорт параметров и констант задачи
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
        /// импорт параметров документов
        /// </summary>
        void ImportTaskDocuments(DataTable importedDocuments, Dictionary<int, byte[]> documents);

        /// <summary>
        /// объект для работы с базой данных
        /// </summary>
        IDatabase GetTaskDB();

        void BeginDbTransaction();

        void CommitDbTransaction();

        void RollbackDbTransaction();

        /// <summary>
        /// освобождение внутреннего объекта IDatabase
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
    /// Интерфейс для управления коллекцией задач
    /// </summary>
    public interface ITaskManager : IDisposable
    {
        /// <summary>
        /// Коллекция задач
        /// </summary>
        ITaskCollection Tasks { get; }
    }

    #region Общедоступные интерфейсы
    [ComVisible(false)]
    public enum TaskParameterType : int
    {
        taskParameter = 0, // параметр
        taskConst = 1      // константа
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
    /// Базовый интерфейс, предоставляет информацию о возможности редактирования объекта
    /// </summary>
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("6895D551-59DF-3C1C-A69A-9D73B562DF91")]
    public interface ITaskParamBase : IDisposable
    {
        /// <summary>
        /// Признак унаследованности параметра
        /// </summary>
        [DispId(1)]
        bool Inherited { get; }

        /// <summary>       
        /// ID объекта
        /// </summary>
        [DispId(2)]
        int ID { get; }
    }

    /// <summary>
    /// Интерфейс константы, базовый для параметра
    /// </summary>
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("4CE11103-9014-3547-86BF-1DFC791F9187")]
    public interface ITaskConst : ITaskParamBase
    {
        #region from ITaskParamBase
        /// <summary>
        /// Признак унаследованности параметра
        /// </summary>
        [DispId(1)]
        new bool Inherited { get; }

        /// <summary>       
        /// ID объекта
        /// </summary>
        [DispId(2)]
        new int ID { get; }
        #endregion

        /// <summary>
        /// название
        /// </summary>
        [DispId(3)]       
        string Name { get; set; }

        /// <summary>
        /// комментарий
        /// </summary>
        [DispId(4)]
        string Comment { get; set; }

        /// <summary>
        /// XML документ со значениями
        /// </summary>
        [DispId(5)]
        object Values { get; set; }
    }

    /// <summary>
    /// интерфейс параметра, унаследован от константы
    /// </summary>
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("8514700B-CDDA-32E8-A067-B31CF58326CC")]
    public interface ITaskParam : ITaskConst
    {
        #region from ITaskParamBase
        /// <summary>
        /// Признак унаследованности параметра
        /// </summary>
        [DispId(1)]
        new bool Inherited { get; }

        /// <summary>       
        /// ID объекта
        /// </summary>
        [DispId(2)]
        new int ID { get; }
        #endregion

        #region from ITaskConst
        /// <summary>
        /// название
        /// </summary>
        [DispId(3)]
        new string Name { get; set; }

        /// <summary>
        /// комментарий
        /// </summary>
        [DispId(4)]
        new string Comment { get; set; }

        /// <summary>
        /// XML документ со значениями
        /// </summary>
        [DispId(5)]
        new object Values { get; set; }
        #endregion

        /// <summary>
        /// Измерение
        /// </summary>
        [DispId(6)]
        string Dimension { get; set; }

        /// <summary>
        /// возможность множественного выбора
        /// </summary>
        [DispId(7)]
        bool AllowMultiSelect { get; set; }
    }

    /// <summary>
    /// базовая коллекция для констант и параметров
    /// </summary>
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("C572ABF7-EB6C-330C-BE92-9FD2BD14BCD7")]
    public interface ITaskItemsCollection : IDisposable
    {
        /// <summary>
        /// Данные коллекции
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
        /// Элементы коллекции находятся в режиме "Только для чтения"
        /// т.к. родительская задача не заблокирована
        /// </summary>
        [DispId(1)]
        bool IsReadOnly { get; }

        /// <summary>
        /// Количество параметров
        /// </summary>
        [DispId(2)]
        int Count { get; }

        /// <summary>
        /// Cинхронизировать с другой коллекцией
        /// </summary>
        /// <param name="destination">Коллекция с которой будет производится синхронизация</param>
        /// <param name="errorStr">расшифровка причин неудачи</param>
        /// <returns>true - если все параметры синхронизировались, false - если возникла ошибка</returns>
        [DispId(3)]
        bool SyncWith(ITaskItemsCollection destination, out string errorStr);
    }

    /// <summary>
    /// коллекция констант, унаследована от базовой
    /// </summary>
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("3965D873-F982-3728-BCD8-0DF1AAEA627F")]
    public interface ITaskConstsCollection : ITaskItemsCollection//, IBindingList
    {
        #region from ITaskItemsCollection
        /// <summary>
        /// Элементы коллекции находятся в режиме "Только для чтения"
        /// т.к. родительская задача не заблокирована
        /// </summary>
        [DispId(1)]
        new bool IsReadOnly { get; }

        /// <summary>
        /// Количество параметров
        /// </summary>
        [DispId(2)]
        new int Count { get; }

        /// <summary>
        /// Cинхронизировать с другой коллекцией
        /// </summary>
        /// <param name="destination">Коллекция с которой будет производится синхронизация</param>
        /// <param name="errorStr">расшифровка причин неудачи</param>
        /// <returns>true - если все параметры синхронизировались, false - если возникла ошибка</returns>
        [DispId(3)]
        new bool SyncWith(ITaskItemsCollection destination, out string errorStr);
        #endregion

        /// <summary>
        /// Константа по индексу
        /// </summary>
        /// <param name="index">индекс константы</param>
        /// <returns>интерфейс константы</returns>
        [DispId(4)]
        ITaskConst ConstByIndex(int index);

        /// <summary>
        /// Константа по ID
        /// </summary>
        /// <param name="name">ID константы</param>
        /// <returns>интерфейс константы</returns>
        [DispId(5)]
        ITaskConst ConstByID(int id);
        
        /// <summary>
        /// Константа по имени
        /// </summary>
        /// <param name="name">имя константы</param>
        /// <returns>интерфейс константы</returns>
        [DispId(6)]
        ITaskConst ConstByName(string name);

        /// <summary>
        /// Добавить константу
        /// </summary>
        /// <returns></returns>
        [DispId(7)]
        ITaskConst AddNew();

        /// <summary>
        /// Удалить константу
        /// </summary>
        /// <param name="item">интерфейс константы</param>
        [DispId(8)]
        void Remove(ITaskConst item);

    }

    /// <summary>
    /// Коллекция параметров, унаследована от базовой
    /// </summary>
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("081D9009-90BA-35E4-9B2F-429E72AB0D37")]
    public interface ITaskParamsCollection : ITaskItemsCollection//, IBindingList
    {
        #region from ITaskItemsCollection
        /// <summary>
        /// Элементы коллекции находятся в режиме "Только для чтения"
        /// т.к. родительская задача не заблокирована
        /// </summary>
        [DispId(1)]
        new bool IsReadOnly { get; }

        /// <summary>
        /// Количество параметров
        /// </summary>
        [DispId(2)]
        new int Count { get; }

        /// <summary>
        /// Cинхронизировать с другой коллекцией
        /// </summary>
        /// <param name="destination">Коллекция с которой будет производится синхронизация</param>
        /// <param name="errorStr">расшифровка причин неудачи</param>
        /// <returns>true - если все параметры синхронизировались, false - если возникла ошибка</returns>
        [DispId(3)]
        new bool SyncWith(ITaskItemsCollection destination, out string errorStr);
        #endregion

        /// <summary>
        /// Параметр по индексу
        /// </summary>
        /// <param name="index">индекс параметра</param>
        /// <returns>интерфейс параметра</returns>
        [DispId(4)]
        ITaskParam ParamByIndex(int index);

        /// <summary>
        /// Параметр по ID
        /// </summary>
        /// <param name="name">ID параметра</param>
        /// <returns>интерфейс параметра</returns>
        [DispId(5)]
        ITaskParam ParamByID(int id);

        /// <summary>
        /// Параметр по имени
        /// </summary>
        /// <param name="name">имя параметра</param>
        /// <returns>интерфейс параметра</returns>
        [DispId(6)]
        ITaskParam ParamByName(string name);

        /// <summary>
        /// Добавить новый параметр
        /// </summary>
        /// <returns>интерфейс параметра</returns>
        [DispId(7)]
        ITaskParam AddNew();

        /// <summary>
        /// Удалить параметр
        /// </summary>
        /// <param name="item">интерфейс параметра</param>
        [DispId(8)]
        void Remove(ITaskParam item);

    }
    #endregion

}