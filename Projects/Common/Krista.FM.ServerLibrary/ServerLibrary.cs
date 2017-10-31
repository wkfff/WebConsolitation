using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

using Krista.FM.ServerLibrary.FinSourcePlanning;
using Krista.FM.ServerLibrary.Forecast;
using Krista.FM.ServerLibrary.TemplatesService;
using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.WriteBackLibrary;


namespace Krista.FM.ServerLibrary
{
    #region Описание перечислений
    /// <summary>
    /// Классы объектов
    /// </summary>
    public enum ClassTypes
    {
        /// <summary>
        /// Сопоставимый классификатор данных
        /// </summary>
		[Description("Сопоставимый классификатор")]
        clsBridgeClassifier = 0,

        /// <summary>
        /// Классификатор данных
        /// </summary>
        [Description("Классификатор данных")]
        clsDataClassifier = 1,

        /// <summary>
        /// Фиксированный классификатор
        /// </summary>
		[Description("Фиксированный классификатор")]
        clsFixedClassifier = 2,

        /// <summary>
        /// Таблица фактов
        /// </summary>
        [Description("Таблица фактов")]
        clsFactData = 3,

        /// <summary>
        /// Ассоциация (связь)
        /// </summary>
        [Description("Ассоциация")]
        clsAssociation = 8,

        /// <summary>
        /// Пакет
        /// </summary>
        [Description("Пакет")]
        clsPackage = 9,

        /// <summary>
		/// Таблица данных.
        /// </summary>
        [Description("Таблица данных")]
        Table = 10,

        /// <summary>
		/// Табличный документ.
        /// </summary>
		[Description("Табличный документ")]
		DocumentEntity = 11
	}

    /// <summary>
    /// Типы ассоциаций данных.
    /// </summary>
    public enum AssociationClassTypes
    {
        /// <summary>
        /// Ассоциация фактов с классификаторами данных
        /// </summary>
        [Description("Ассоциация данных")]
        Link = 0,

        /// <summary>
        /// Ассоциация между классификаторами данных
        /// </summary>
        //Data = 1,

        /// <summary>
        /// Ассоциация между классификаторами данных и сопоставимыми
        /// </summary>
        [Description("Ассоциация сопоставления")]
        Bridge = 2,

        /// <summary>
        /// Ассоциация между сопоставимыми классификаторами
        /// </summary>
        [Description("Ассоциация сопоставления версий")]
        BridgeBridge = 3,

        /// <summary>
        /// Ассоциация Мастер-деталь.
        /// </summary>
        [Description("Мастер-деталь")]
        MasterDetail = 5
    }

    /// <summary>
    /// Подклассы объектов
    /// </summary>
    public enum SubClassTypes
    {
        /// <summary>
        /// Используется только для сопоставимых и фиксированных классификаторов (SubClassTypes для них не используется)
        /// </summary>
        [Description("Обычный")]
        Regular = 0,

        /// <summary>
        /// Классы (таблицы) предназначенные для закачки данных
        /// </summary>
        [Description("Закачка данных")]
        Pump = 1,

        // СБОР
        //Task = 2,

        /// <summary>
        /// Системные классы (fx.Date.Year и т.п.)
        /// </summary>
        [Description("Системный")]
        System = 4,

        /// <summary>
        /// Классы (таблицы) предназначенные для ввода
        /// </summary>
        [Description("Ввод данных")]
        Input = 5,

        /// <summary>
        /// Закачка и ввод данных
        /// </summary>
        [Description("Закачка и ввод данных")]
        PumpInput = 6
    }

    /// <summary>
    /// Типы атрибутов данных
    /// </summary>
    public enum DataAttributeTypes
    {
        [Description("Неизвестный")]
        dtUnknown = -1,

        [Description("Целый")]
        dtInteger = 0,

        [Description("Вещественный")]
        dtDouble = 1,

        [Description("Символьный")]
        dtChar = 2,

        [Description("Строковый")]
        dtString = 3,

        [Description("Логический")]
        dtBoolean = 4,

        [Description("Дата")]
        dtDate = 5,

        [Description("Дата и время")]
        dtDateTime = 6,

        [Description("BLOB")]
        dtBLOB = 7
    }

    /// <summary>
    /// Класс атрибута
    /// </summary>
    public enum DataAttributeClassTypes
    {
        [Description("Системный")]
        System = 0,

        [Description("Фиксированный")]
        Fixed = 1,

        [Description("Пользовательский")]
        Typed = 2,

        [Description("Ссылка")]
        Reference = 3
    }

    /// <summary>
    /// Вид атрибута
    /// </summary>
    public enum DataAttributeKindTypes
    {
        /// <summary>
        /// Системные (ID исходной записи, ParentID, CubeParentID, поля расщепленных кодов, поля родителей для фиксированной иерархии)
        /// </summary>
        [Description("Системный")]
        Sys = 0,

        /// <summary>
        /// Служебные (ID, источник, закачка, задача)
        /// </summary>
        [Description("Служебный")]
        Serviced = 1,

        /// <summary>
        /// Обычные
        /// </summary>
        [Description("Обычный")]
        Regular = 2
    }

    /// <summary>
    /// Состояние сервера
    /// </summary>
    public enum ServerStates
    {
        stateUnknown,
        stateConnected,
        stateFailed
    }

    /// <summary>
    /// Состояние схемы
    /// </summary>
    public enum SchemeStates
    {
        Shutdown,
        Start,
        Open
    }

    /// <summary>
    /// Определяет состояние объектов базы данных
    /// </summary>
    public enum DBObjectStateTypes
    {
        /// <summary>
        /// Неопределенное состояние. Объект еще не проинициализирован.
        /// </summary>
        [Description("Неопределенное")]
        Unknown = -1,

        /// <summary>
        /// Объект создан в базе данных.
        /// </summary>
        [Description("В базе данных")]
        InDatabase = 0,

        /// <summary>
        /// Новый объект, в базе данных еще не создан.
        /// </summary>
        [Description("Новый")]
        New = 1,

        /// <summary>
        /// Объект создан в базе данных, но его текущее структура отличается от структуры в базе данных.
        /// </summary>
        [Description("Изменен")]
        Changed = 2,

        /// <summary>
        /// Объект создан в базе данных и его необходимо удалить из базы данных.
        /// </summary>
        [Description("Удален")]
        ForDelete = 3,
    }

    /// <summary>
    /// Типы документов привязываемые к пакету
    /// </summary>
    public enum DocumentTypes
    {
        /// <summary>
        /// Диаграмма классов
        /// </summary>
        [Description("Диаграмма классов")]
        Diagram = 0,

        /// <summary>
        /// URL-ссылка на внешний документ
        /// </summary>
        [Description("URL-ссылка")]
        URL = 1,

		/// <summary>
        /// Диаграмма табличных документов.
        /// </summary>
		[Description("Диаграмма табличных документов")]
        DocumentEntityDiagram = 2,

    }

    /// <summary>
    /// Определяет тип лукап атрибута.
    /// </summary>
    public enum LookupAttributeTypes
    {
        /// <summary>
        /// Не участвует в лукапе
        /// </summary>
        [Description("Не участвует")]
        None = 0,

        /// <summary>
        /// Основное поле лукапа 
        /// </summary>
        [Description("Основное")]
        Primary = 1,

        /// <summary>
        /// Дополнительное поле лукапа 
        /// </summary>
        [Description("Дополнительное")]
        Secondary = 2
    }
    #endregion

    #region Server.Providers.Planing

    /// <summary>
    /// Васин провайдер
    /// </summary>
    public interface IPlaningProvider : IDisposable
    {
        /// <summary>
        /// Получение даты обновления метаданных
        /// </summary>
        /// <returns>Дата обновления метаданных</returns>
        string GetMetadataDate();

        /// <summary>
        /// Извлечение метаданных схемы. По умолчанию данные берутся из тайника.
        /// </summary>
        /// <returns>Метаданные схемы</returns>
        string GetMetaData();

        
        /// <summary>
        /// Возвращает список членов измерения
        /// </summary>
        /// <param name="cubeName"></param>
        /// <param name="dimensionName"></param>
        /// <param name="hierarchyName"></param>
        /// <param name="levelNames"></param>
        /// <param name="memberPropertiesNames"></param>
        /// <returns></returns>
        string GetMembers(string providerId, string cubeName, string dimensionName, string hierarchyName, string levelNames, string memberPropertiesNames);

        /// <summary>
        /// Выполнить MDX-запрос через рекордсет
        /// </summary>
        /// <param name="queryText">MDX-запрос</param>
        /// <returns>результат выполнения запроса</returns>
        string GetRecordsetData(string providerId, string queryText);

        /// <summary>
        /// Выполнить MDX-запрос через Cellset
        /// </summary>
        /// <param name="queryText">MDX-запрос</param>
        /// <returns>результат выполнения запроса</returns>
        string GetCellsetData(string providerId, string queryText);

        /// <summary>
        /// Обновляет данные кэша для указанных измерений.
        /// </summary>
        /// <remarks>Вызываестся диспетчером расчета многомерных объектов после их обработки.</remarks>
        /// <param name="names">Список имен измерений.</param>
        void RefreshDimension(string providerId, string[] names);

        /// <summary>
        /// Обновляет данные кэша для указанных кубов.
        /// </summary>
        /// <remarks>Вызываестся диспетчером расчета многомерных объектов после их обработки.</remarks>
        /// <param name="names">Список имен кубов.</param>
        void RefreshCube(string providerId, string[] names);

        /// <summary>
        /// Обновление метаданных с принудительной очисткой кэша
        /// </summary>
        /// <returns></returns>
        string RefreshMetaData();

        /// <summary>
        /// Обеспечивает механизм синхронизации параметров в документе с задачей
        /// </summary>
        /// <param name="taskId">айди задачи</param>
        /// <param name="paramsText">строка с последовательно прописанными наборами свойств параметров</param>
        /// <param name="sectionDivider">разделитель параметров</param>
        /// <param name="valuesDivider">разделитель свойств параметра внутри блока</param>
        /// <returns>возвращает строку пар вида OldParamId=NewParamId, разделенных sectionDivider-ом</returns>
        string UpdateTaskParams(int taskId, string paramsText, string sectionDivider, string valuesDivider);

        /// <summary>
        /// Обеспечивает механизм синхронизации констант в документе с задачей
        /// </summary>
        /// <param name="taskId">айди задачи</param>
        /// <param name="constsText">строка с последовательно прописанными наборами свойств констант</param>
        /// <param name="sectionDivider">разделитель констант</param>
        /// <param name="valuesDivider">разделитель свойств константы внутри блока</param>
        /// <returns>возвращает строку пар вида OldParamId=NewParamId, разделенных sectionDivider-ом</returns>
        string UpdateTaskConsts(int taskId, string constsText, string sectionDivider, string valuesDivider);

        /// <summary>
        /// Возвращает хмл с параметрами и константами задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        string GetTaskContext(int taskId);
    }

    #endregion Server.Providers.Planing

    #region Server.Providers.VSS

    public enum VSSFileStatus
    {
        VSSFILE_NOTCHECKEDOUT = 0,
        VSSFILE_CHECKEDOUT = 1,
        VSSFILE_CHECKEDOUT_ME = 2,
    }

    /// <summary>
    /// Предоставляет доступ к файлам в Visual Source Safe.
    /// </summary>
    public interface IVSSFacade
    {
        /// <summary>
        /// Open the Visual SourceSafe database. 
        /// </summary>
        /// <param name="srcSafeIni">A fully qualified path, including a file name, to the srcsafe.ini file of the current SourceSafe database.</param>
        /// <param name="username">A name of the Visual SourceSafe user attempting to log in to the SourceSafe database. The default is the name of the user logged in to the Windows session</param>
        /// <param name="password">A password of the Visual SourceSafe user attempting to log in to the SourceSafe database. The default is an empty string.</param>
        /// <param name="baseWorkingProject"></param>
        /// <param name="localPath"></param>
        void Open(string srcSafeIni, string username, string password, string baseWorkingProject, string localPath);

        /// <summary>
        /// Closes the Visual SourceSafe database. 
        /// </summary>
        void Close();

        /// <summary>
        /// Checks in a checked-out file or project to the SourceSafe database
        /// </summary>
        /// <param name="local">A string representing a fully qualified local path from which a file or a project is checked in. The default value, null, represents a folder to which the file or the project was checked out</param>
        /// <param name="comments">A string containing a comment. The default is an empty string.</param>
        void Checkin(string local, string comments);

        /// <summary>
        /// Checks out a file or project from the SourceSafe database
        /// </summary>
        /// <param name="local">A string representing a fully qualified local path to which a file or a project is checked out. If you check out a file, the path includes the file name. The default value, null or an empty string, represents the working folder (LocalSpec ) of a file or a project. </param>
        /// <param name="comments">A string containing a comment. The default is an empty string.</param>
        void Checkout(string local, string comments);

        /// <summary>
        /// Implements undo checkout of a checked-out file or a checked-out project
        /// </summary>
        /// <param name="local">A string representing a fully qualified local path from which you want to undo the checkout. The default value, null, represents a folder to which the file or the project was checked out</param>
        void UndoCheckout(string local);

        /// <summary>
        /// Gets a value indicating whether a particular file is checked out
        /// </summary>
        /// <param name="local"></param>
        /// <returns>A value indicating whether a particular file is checked out</returns>
        VSSFileStatus IsCheckedOut(string local);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="local"></param>
        /// <returns></returns>
        bool Find(string local);

        /// <summary>
        /// Имя пользователя, извлекшего файл
        /// </summary>
        /// <param name="local"></param>
        /// <returns></returns>
        string GetCheckOutUser(string local);

        /// <summary>
        /// Имя пользователя
        /// </summary>
        /// <returns></returns>
        string GetUserName();

        /// <summary>
        /// Имя базы данных
        /// </summary>
        /// <returns></returns>
        string GetDatabaseName();

        /// <summary>
        /// Имя ini-файла
        /// </summary>
        /// <returns></returns>
        string GetINI();

        /// <summary>
        /// Метод берет версия файла из VSS на диск без блокирования файла
        /// </summary>
        /// <param name="local">локальный путь файла</param>
        /// <param name="path"> Полный путь к выкачаному файлу</param>
        void Get(string local, string path);

        /// <summary>
        /// 
        /// </summary>
        void Refresh();
    }

    #endregion Server.Providers.VSS

    #region Common

    /// <summary>
    /// Представляет базавый интерфейс коллекций элементов ключ/значение
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IDictionaryBase<TKey, TValue> : IDictionary<TKey, TValue>, ISMOSerializable
    {
        /// <summary>
        /// Добавляет в коллекцию новый элемент с указанным ключом
        /// </summary>
        /// <param name="key">Ключ элемента</param>
        /// <returns>Эначение элемента</returns>
        TValue New(TKey key);

        /// <summary>
        /// Сохраняет изменения сделанные в коллекции
        /// </summary>
        void Update();
    }

    /// <summary>
    /// Представляет базавый интерфейс коллекций элементов,  значение которых может быть получено по индивидуальному индексу
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IListBase<TValue> : IList<TValue>, ISMOSerializable
    {
        
    }

    #endregion Common

    #region Server

    /// <summary>
    /// Интерфейс для доступа с серверу приложений
    /// </summary>
    public interface IServer : IDisposable
    {
        /// <summary>
        /// Имя машины на которой находится сервер приложений
        /// </summary>
        string Machine { get; }

        /// <summary>
        /// Возвращает список имен схем запущеных на сервере
        /// </summary>
        ICollection SchemeList { get; }

        /// <summary>
        /// Подключение к схеме с именем schemeName.
        /// </summary>
        /// <param name="schemeName">Имя схемы к которой нужно подключиться.</param>
        /// <param name="scheme">Если подключение возможно, то scheme содержит указатель на объект схемы.</param>
        /// <param name="authType">Тип аутентификации.</param>
        /// <param name="login">Имя пользователя.</param>
        /// <param name="pwdHash">Кэш пароля.</param>
        /// <param name="errStr">Строка содержащая сведения о ошибке.</param>
        /// <returns>
        /// Возвращаен истину или ложь в зависимости от результата подключения к схеме.
        /// Подробное описание ошибок должно находися в логе сервера приложений.
        /// </returns>
        bool Connect(string schemeName, out IScheme scheme, AuthenticationType authType, string login, string pwdHash, ref string errStr);
        bool Connect(string schemeName, out IScheme scheme, AuthenticationType authType, string login, string pwdHash, ref string errStr, string clientServerLibraryVersion);

        bool Connect(out IScheme scheme, AuthenticationType authType, string login, string pwdHash, ref string errStr);
        bool Connect(out IScheme scheme, AuthenticationType authType, string login, string pwdHash, ref string errStr, string clientServerLibraryVersion);

        ISchemeStub Connect();

        /// <summary>
        /// Адрес веб-сервиса
        /// </summary>
        [Obsolete("Для доступа к свойству необходимо использовать метод GetConfigurationParameter(string key)")]
        string WebServiceUrl { get; }

        /// <summary>
        /// Путь к репозиторию сервера приложений
        /// </summary>
        [Obsolete("Для доступа к свойству необходимо использовать метод GetConfigurationParameter(string key)")]
        string RepositoryPath { get; }

        /// <summary>
        /// Активация сервера
        /// </summary>
        void Activate();

        /// <summary>
        /// Отключение от сервера
        /// </summary>
        void Disconnect();

        [Obsolete("Для доступа к свойству необходимо использовать метод GetConfigurationParameter(string key)")]
        int ServerPort { get; set; }

        /// <summary>
        /// Возвращает конфигурационный параметр сервера приложений.
        /// </summary>
        /// <param name="key">Уникальный ключ параметра.</param>
        /// <returns>Значение параметра.</returns>
        string GetConfigurationParameter(string key);

        /// <summary>
        /// Возвращает раздел конфигурации сервера приложений
        /// </summary>
        /// <param name="key">Уникальный ключ раздела</param>
        /// <returns>Значение раздела</returns>
        object GetConfigurationSection(string key);
    }

    /// <summary>
    /// Константы для коллекции метаданных объекта
    /// </summary>
    public class SQLMetadataConstants
    {
        /// <summary>
        /// наименование констрейнта на родительский ключ
        /// </summary>
        public const string ParentIDForeignKeyConstraint = "ParentIDForeignKeyConstraint";

        /// <summary>
        /// тригер блокировки по источнику
        /// </summary>
        public const string DataSourceLockTrigger = "DataSourceLockTrigger";

        /// <summary>
        /// тригер блокировки варианта
        /// </summary>
        public const string VariantLockTrigger = "VariantLockTrigger";

        /// <summary>
        /// тригер блокировки записей разработчика
        /// </summary>
        public const string DeveloperLockTrigger = "DeveloperLockTrigger";

        /// <summary>
        /// тригер аудит
        /// </summary>
        public const string AuditTrigger = "AuditTrigger";
    }

    #endregion Server

    #region SMOSerializable

    /// <summary>
    /// Базовый интерфейс для всех сериализуемых объктов
    /// </summary>
    public interface ISMOSerializable : IServerSideObject
    {
        /// <summary>
        /// Сериализации свойств объекта
        /// </summary>
        /// <returns></returns>
        SMOSerializationInfo GetSMOObjectData();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        SMOSerializationInfo GetSMOObjectData(LevelSerialization level);
    }

    #endregion

    #region Server.Scheme

    public enum ProcessTypes
    {
        Default = 0,
        Full = 1,
        RefreshData = 2,
        BuildStructure = 3,
        Resume = 4,
        Suspend = 5,
        RefreshDataAndIndex = 6,
        Reaggregate = 7,
        FullReaggregate = 8
    }

    /// <summary>
    /// Состояние серверного объекта во времени его существования
    /// </summary>
    public enum ServerSideObjectStates
    {
        /// <summary>
        /// Постоянный
        /// </summary>
        Consistent = 0,

        /// <summary>
        /// Новый
        /// </summary>
        New = 1,

        /// <summary>
        /// Удаленный
        /// </summary>
        Deleted = 2,

        /// <summary>
        /// Bpvtytyysq
        /// </summary>
        Changed = 3
    }

    /// <summary>
    /// Базовый интерфейс для всех серверных объектов. 
    /// Определяет повение серверных объектов на основе паттерна "Компоновщик"
    /// </summary>
    public interface IServerSideObject : ICloneable, IDisposable
    {
        /// <summary>
        /// Блокировка объекта
        /// </summary>
        /// <returns>Копия заблокированного объекта</returns>
        IServerSideObject Lock();

        /// <summary>
        /// Снятие блокировки
        /// </summary>
        void Unlock();

        /// <summary>
        /// Возвращает true, если объект заблокирован
        /// </summary>
        bool IsLocked { get; }

        /// <summary>
        /// ID пользователя заблокировавшего объект
        /// </summary>
        int LockedByUserID { get; }

        /// <summary>
        /// Имя пользователя заблокировавшего объект
        /// </summary>
        string LockedByUserName { get; }

        /// <summary>
        /// true - Объект является целой частью, false - является частью составного объекта 
        /// </summary>
        bool IsEndPoint { get; }

        /// <summary>
        /// Состояние серверного объекта во времени его существования
        /// </summary>
        ServerSideObjectStates State { get; }

        /// <summary>
        /// Родительский объект
        /// </summary>
        IServerSideObject OwnerObject { get; }

        #region Для тестовых целей

        /// <summary>
        /// Идентификатор текущего объекта
        /// </summary>
        int Identifier { get; }

        /// <summary>
        /// true - объект является клоном
        /// </summary>
        bool IsClone { get; }

        /// <summary>
        /// Копия объекта с которым работаем польхователь заблокировавший его
        /// </summary>
        IServerSideObject ICloneObject { get; }

        #endregion Для тестовых целей

    }

    /// <summary>
    /// Объекты идентифицируемые по уникальному ключу при помощи GUID.
    /// </summary>
    public interface IKeyIdentifiedObject : ISMOSerializable
    {
        /// <summary>
        /// Уникальный ключ объекта.
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string ObjectKey { get; set; }


        
        /// <summary>
        /// Уникальное наименование объекта (старый ключ).
        /// Это свойство после выпуска версии 2.4.1 необходимо удалить.
        /// </summary>
        string ObjectOldKeyName { get; }
    }

    /// <summary>
    /// Реализует поведение зависимых (агрегатов) объектов
    /// </summary>
    public interface IMinorObject : IKeyIdentifiedObject
    {
    }

    /// <summary>
    /// Интерфейс для доступа к семантикам как к массиву 
    /// </summary>
    [SMOInterface]
    public interface ISemanticsCollection : IDictionary<string, string>
    {
        bool ReadWrite { get; set; }
    }

    /// <summary>
    /// Интерфейс для доступа к хранилищу данных
    /// </summary>
    public interface ISchemeDWH
    {
        /// <summary>
        /// Возвращает новый объект для доступа к базе данных.
        /// После использования необходимо вызвать Dispose
        /// </summary>
        IDatabase DB { get; }

        /// <summary>
        /// Создает экземпляр объекта UnitOfWork.
        /// После использования необходимо вызвать Dispose.
        /// </summary>
        IUnitOfWork CreateUnitOfWork();

        /// <summary>
        /// Версия базы данных в формате XX.XX.XX
        /// </summary>
        string DatabaseVersion { get; }

        /// <summary>
        /// Строка подключения к хранилищу
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Имя фабрики объектов доступа к данным
        /// </summary>
        string FactoryName { get; }

        /// <summary>
        /// Версия сервера
        /// </summary>
        string ServerVersion { get; set; }

        /// <summary>
        /// Имя реляционной базы, к которой подключились
        /// </summary>
        string DataBaseName { get; }

        /// <summary>
        /// Строка подключения к хранилищу из UDL 
        /// </summary>
        string OriginalConnectionString { get; }

        /// <summary>
        /// Имя сервера, на котором запущена реляционная БД
        /// </summary>
        string ServerName { get; }
    }

    /// <summary>
    /// Интерфейс доступа к многомерной базе данных.
    /// </summary>
    public interface IOlapDatabase
    {
        /// <summary>
        /// Возвращает объект доступа к серверу Analisys Service.
        /// </summary>
        object ServerObject { get; }

        /// <summary>
        /// Возвращает объект доступа к базе данных.
        /// </summary>
        object DatabaseObject { get; }

        /// <summary>
        /// Проверяет наличие продключения к базе данных, 
        /// в случаи потери соединения восстанавливает его.
        /// </summary>
        void CheckConnection();
    }

    /// <summary>
    /// Интерфейс для доступа к многомерной модели данных
    /// </summary>
    public interface ISchemeMDStore
    {
        /// <summary>
        /// Объект для доступа к многомерной базе данных.
        /// </summary>
        IOlapDatabase OlapDatabase { get; }

        string CatalogName { get; }

        string ServerName { get; }

        /// <summary>
        /// Версия многомерной базы
        /// </summary>
        string DatabaseVersion { get; }

        /// <summary>
        /// Имя реляционной базы, на которую ссылается многомерная база
        /// </summary>
        string OlapDataSourceName { get; }

        /// <summary>
        /// Разбирает строку с версией сервера. Если разобрать не удалось - генерирует исключение.
        /// </summary>
        /// <returns>true, если подключены к SSAS2005</returns>
        bool IsAS2005();

        /// <summary>
        /// Имя сервера реляционной базы, на которую настроена многомерка
        /// </summary>
        string OlapDataSourceServer { get; }
    }

    /// <summary>
    /// Интерфейс для работы с "Архивными папками"
    /// </summary>
    public interface IArchive
    {
    }

    /// <summary>
    /// Определяет методы для преобразования коллекции в DataTable
    /// </summary>
    public interface ICollection2DataTable
    {
        /// <summary>
        /// Возвращает DataTable сформированный по коллекции
        /// </summary>
        DataTable GetDataTable();
    }

    /// <summary>
    /// Классы атрибутов.
    /// </summary>
    public enum AttributeClass
    {
        /// <summary>
        /// Обычные поля таблицы.
        /// </summary>
        Regular,
        
		/// <summary>
        /// Поля-ссылки, принадлежащие ассоциациям.
        /// </summary>
        Reference,
        
		/// <summary>
        /// Поле-документ с дополнительными полями.
        /// </summary>
        Document,

		/// <summary>
		/// Атрибуты ссылки на атрибуты другой сущности.
		/// </summary>
		RefAttribute
    }

    /// <summary>
    /// Атрибут данных объектов схемы
    /// </summary>
    [SMOInterface]
    public interface IDataAttribute : IMinorObject
    {
        /// <summary>
        /// Имя атрибута в БД
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string Name { get; set; }

        /// <summary>
        /// Наименование атрибута выводимое в интерфейсе
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string Caption { get; set; }

        /// <summary>
        /// Описание атрибута
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string Description { get; set; }

        /// <summary>
        /// Тип данных атрибута
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        DataAttributeTypes Type { get; set; }

        /// <summary>
        /// Размер данных атрибута
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        int Size { get; set; }

        /// <summary>
        /// Точность данных атрибута
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        int Scale { get; set; }

        /// <summary>
        /// Значение атрибута по умолчанию
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        object DefaultValue { get; set; }

        /// <summary>
        /// Маска для ввода значения
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string Mask { get; set; }

        /// <summary>
        /// Маска расщепления значения кода
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string Divide { get; set; }

        /// <summary>
        /// Определяет видимость атрибута
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        bool Visible { get; set; }

        /// <summary>
        /// Определяет обязательность значения
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        bool IsNullable { get; set; }

        /// <summary>
        /// Определяет фиксированный атрибут или нет
        /// </summary>
        [Obsolete]
        bool Fixed { get; }

        /// <summary>
        /// Определяет тип лукап атрибута.
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        LookupAttributeTypes LookupType { get; set; }

        /// <summary>
        /// Определяет является ли атрибут атрибутом типа Lookup
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        bool IsLookup { get; }

        /// <summary>
        /// Полное наименование объекта в котором находится ключ (ID), 
        /// на который "ссылается" значение хранимое в атрибуте
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string LookupObjectName { get; }

        /// <summary>
        /// Наименование аттрибута лукап-объекта значение которого отображается
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string LookupAttributeName { get; set; }

        /// <summary>
        /// Системный класс
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        DataAttributeClassTypes Class { get; }

        /// <summary>
        /// Вид атрибута (для пользовательского интерфейса)
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        DataAttributeKindTypes Kind { get; }

        /// <summary>
        /// Определяет возможность редактирования атрибута пользователем
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        bool IsReadOnly { get; set; }

        /// <summary>
        /// SQL-определение атрибута
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string SQLDefinition { get; }

        /// <summary>
        /// Определяет является ли атрибут строковым идентификатором или нет.
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        bool StringIdentifier { get; set; }

        /// <summary>
        /// Описание разработчика объекта
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string DeveloperDescription { get; set; }

        /// <summary>
        /// Позиция атрибута в коллекции (для контроля последовательности атрибутов) 
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        int Position { get; set; }

        /// <summary>
        /// Родительский атрибут при группировке
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string GroupParentAttribute { get; set; }

        /// <summary>
        /// Метод получает вычисленную позицию атрибута в коллекции
        /// </summary>
        /// <returns></returns>
        string GetCalculationPosition();

        /// <summary>
        /// Тэги группировки атрибута
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string GroupTags { get; set; }
    }

    public interface IGroupAttribute : IDataAttribute
    {
        Dictionary<string, IDataAttribute> Attributes { get; set; }
    }

    /// <summary>
    /// Атрибут-документ.
    /// </summary>
    public interface IDocumentAttribute : IDataAttribute
    {
        /// <summary>
        /// Дополнительный атрибут "Наименование".
        /// </summary>
        IDataAttribute DocumentName { get; }

        /// <summary>
        /// Дополнительный атрибут "Тип документа".
        /// </summary>
        IDataAttribute DocumentType { get; }

        /// <summary>
        /// Дополнительный атрибут "Имя файла".
        /// </summary>
        IDataAttribute DocumentFileName { get; }

        /// <summary>
        /// Сохраняет данные документа в БД.
        /// </summary>
        /// <param name="documentData">Документа.</param>
        /// <param name="tableName">Имя таблицы.</param>
        /// <param name="rowID">ID записи.</param>
        void SaveDocumentDataToDataBase(byte[] documentData, string tableName, int rowID);

        /// <summary>
        /// Выбирает данные документа из БД.
        /// </summary>
        /// <param name="tableName">Имя таблицы.</param>
        /// <param name="rowID">ID записи.</param>
        /// <returns>Документ.</returns>
        byte[] GetDocumentDataFromDataBase(string tableName, int rowID);
    }

	/// <summary>
	/// Атрибут документа, взятый из внешней сущности.
	/// </summary>
	public interface IDocumentEntityAttribute : IDataAttribute
	{
		/// <summary>
		/// Ключ сущности в которой находится исходный атрибут.
		/// </summary>
		string SourceEntityKey { get; }

		/// <summary>
		/// Ключ атрибута в исходной сущности.
		/// </summary>
		string SourceEntityAttributeKey { get; }

		/// <summary>
		/// Устанавливает исходный атрибут.
		/// </summary>
		/// <param name="sourceAttribute">Исходный атрибут.</param>
		void SetSourceAttribute(IDataAttribute sourceAttribute);
	}

    /// <summary>
    /// Коллекция атрибутов данных объектов схемы
    /// </summary>
    [SMOInterface]
    public interface IDataAttributeCollection : IDictionaryBase<string, IDataAttribute>, ICollection2DataTable
    {
        void Add(IDataAttribute dataAttribute);
        IDataAttribute CreateItem(AttributeClass attributeClass);
    }

    /// <summary>
    /// Базовый интерфейс реализуемый всеми объектами схемы
    /// </summary>
    public interface ICommonObject : IKeyIdentifiedObject
    {
        /// <summary>
        /// Имя объекта
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string Name { get; set; }

        /// <summary>
        /// Полное имя объекта
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string FullName { get; }

        /// <summary>
        /// Полное имя объекта
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string FullDBName { get; }

        /// <summary>
        /// Определяет состояние корректности объекта
        /// </summary>
        [Obsolete]
        bool IsValid { get; }

        /// <summary>
        /// Описание объекат
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string Description { get; set; }

        /// <summary>
        /// Наименование объекта выводимое в интерфейсе
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string Caption { get; set; }

        /// <summary>
        /// XML конфигурация объекта
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string ConfigurationXml { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ICommonDBObject : ICommonObject, IMajorModifiable
    {
        /// <summary>
        /// ID объекта в БД
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        int ID { get; }

        /// <summary>
        /// Уникальный ключ объекта
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string Key { get; }

        /// <summary>
        /// Родительский пакет
        /// </summary>
        IPackage ParentPackage { get; }

        /// <summary>
        /// Текущее состояние структуры объекта
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        DBObjectStateTypes DbObjectState { get; }

        /// <summary>
        /// Описание разработчика объекта
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string DeveloperDescription { get; set; }

        /// <summary>
        /// Завершение редактирования с применением всех изменений
        /// </summary>
        void EndEdit();

        /// <summary>
        /// Завершение редактирования с применением всех изменений
        /// </summary>
        /// <param name="comments">Комментарии к сделанным изменениям</param>
        void EndEdit(string comments);

        /// <summary>
        /// Отмена редактирования объекта
        /// </summary>
        void CancelEdit();

        /// <summary>
        /// Коллекция с метаданными (имена констрейнов, тригеров) объекта 
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetSQLMetadataDictionary();

        /// <summary>
        /// Возвращает SQL-определение объекта.
        /// </summary>
        /// <returns>SQL-определение объекта.</returns>
        List<string> GetSQLDefinitionScript();

		/// <summary>
		/// Семантичаская принадлежность объекта
		/// </summary>
		[SMOSerializable(ReturnType.Value)]
		string Semantic { get; set; }

		/// <summary>
		/// Русской наменование семантичаской принадлежности объекта
		/// </summary>
		[SMOSerializable(ReturnType.Value)]
		string SemanticCaption { get; }
	}

    /// <summary>
    /// Тип иерархии классификатора
    /// </summary>
    public enum HierarchyType
    {
        [Description("Фиксированная")]
        Regular,

        [Description("Иерархическая")]
        ParentChild
    }

    /// <summary>
    /// Тип уровня измерения
    /// </summary>
    public enum LevelTypes
    {
        [Description("Все")]
        All,

        [Description("Обычный")]
        Regular
    }

    /// <summary>
    /// Уровень измерения
    /// </summary>
    [SMOInterface]
    public interface IDimensionLevel : IKeyIdentifiedObject
    {
        /// <summary>
        /// Классификатор в котором находится уровень
        /// </summary>
        IClassifier Parent { get; }

        /// <summary>
        /// Наименование
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string Description { get; set; }

        /// <summary>
        /// Тип уровня
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        LevelTypes LevelType { get; }

        /// <summary>
        /// Наименования уровней для деревянной иерархии
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string LevelNamingTemplate { get; set; }

        IDataAttribute MemberKey { get; set; }

        IDataAttribute MemberName { get; set; }

        [SMOSerializable(ReturnType.Object)]
        IDataAttribute ParentKey { get; set; }
    }

    /// <summary>
    /// Коллекция уровней измерения
    /// </summary>
    [SMOInterface]
    public interface IDimensionLevelCollection : IDictionaryBase<string, IDimensionLevel>, IMinorModifiable
    {
        /// <summary>
        /// Тип иерархии классификатора
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        HierarchyType HierarchyType { get; set; }

        /// <summary>
        /// ALL - фиктивный, по умолчанию falsе, в xml конфигурацию сохраняем, если только true
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        bool AllHidden { get; set; }

        /// <summary>
        /// Создает новый элемент в коллекции
        /// </summary>
        /// <param name="name">Ключ (Имя элемента)</param>
        /// <param name="levelType">Тип элемента</param>
        /// <returns>Созданный элемент</returns>
        IDimensionLevel CreateItem(string name, LevelTypes levelType);
    }


    /// <summary>
    /// Уникальный ключ
    /// </summary>
    [SMOInterface]
    public interface IUniqueKey : ICommonObject
    {
        /// <summary>
        /// Таблица, в которой находится ключ (BridgeCls, DataCls, TableEntity)
        /// </summary>
        [SMOSerializable(ReturnType.Object)]
        IEntity Parent { get; }
        
        /// <summary>
        /// Список полей в ключе
        /// </summary>
        [SMOSerializable(ReturnType.Dictionary)]
        List<string> Fields { get; set; }

        /// <summary>
        /// Признак наличия системного поля "hash_uk" с хэшем по на набору полей из данного уникального ключа
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        bool Hashable { get; set; }
    }


    /// <summary>
    /// Коллекция уникальных ключей
    /// </summary>
    [SMOInterface]
    public interface IUniqueKeyCollection : IDictionaryBase<string, IUniqueKey>, IMinorModifiable
    {
        /// <summary>
        /// Создает новый элемент в коллекции
        /// </summary>
        /// <param name="caption">Наименование объекта выводимое в интерфейсе</param>
        /// <param name="fields">Список полей, образующих уникальный ключ</param>
        /// <param name="hashable">Признак формирования поля с хэшем по этим полям</param>
        /// <returns>Созданный уникальный ключ</returns>
        IUniqueKey CreateItem(string caption, List<string> fields, bool hashable);
    }


    /// <summary>
    /// Базовый интерфейс объектов делящихся по источникам
    /// </summary>
    public interface IDataSourceDividedClass : IEntity
    {
        /// <summary>
        /// Метод определяет для текущего источника параметр деления
        /// </summary>
        /// <returns> Вид параметров источника</returns>
        ParamKindTypes DataSourceParameter(int sourceID);

        /// <summary>
        /// Список видов источника
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string DataSourceKinds { get; set; }

        /// <summary>
        /// Определяет делится ли объект по источникам или нет
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        bool IsDivided { get; }
    }      

    /// <summary>
    /// Классификатор данных
    /// </summary>
    [SMOInterface]
    public interface IClassifier : IDataSourceDividedClass
    {
        /// <summary>
        /// Коллекция уровней
        /// </summary>
        [SMOSerializable(ReturnType.Dictionary, LevelSerialization.DeepSerialize)]
        IDimensionLevelCollection Levels { get; }

        /// <summary>
        /// Возвращает список источников по которым сформирован классификатор
        /// </summary>
        /// <returns>Key - ID источника данных; Value - описание источника</returns>
        Dictionary<int, string> GetDataSourcesNames();

        /// <summary>
        /// Фопмирует иерархию для построения по ней измерений
        /// </summary>
        /// <returns>количество обработанных записей</returns>
        int FormCubesHierarchy();

        /// <summary>
        /// Разбивает код классификатора на Code1, Code2 и т.д.
        /// Устанавливает древовидную иерархию
        /// </summary>
        int DivideAndFormHierarchy(int SourceID, bool setFullHierarchy);
        int DivideAndFormHierarchy(int SourceID, int dataPumpID, ref DataSet clsDataSet);
        int DivideAndFormHierarchy(int SourceID, IDatabase database);
        /// <summary>
        /// Разбивает код классификатора на Code1, Code2 и т.д.
        /// </summary>
        /// <param name="SourceID"></param>
        /// <returns></returns>
        int DivideClassifierCode(int SourceID);

        /// <summary>
        /// Добавляет в классификатор фиксированную запись для неизвестных данных
        /// </summary>
        /// <param name="sourceID">ID источника к которому добавлять запись</param>
        /// <returns>ID добавленной записи</returns>
        int UpdateFixedRows(int sourceID);
        int UpdateFixedRows(IDatabase db, int sourceID);

        /// <summary>
        /// Создает многомерныобъект в базе DV на текущем аналитическом сервере
        /// </summary>
        void CreateOlapObject();

        /// <summary>
        /// Возвращает значение лукап-атрибута (работает быстрее и рекомендуется для использования)
        /// </summary>
        /// <param name="db">Объект для боступа к базе данных</param>
        /// <param name="rowID">ID записи</param>
        /// <param name="attributeName">Наименование атрибута значение которого нужно получить</param>
        /// <returns>Значение лукап-атрибута</returns>
        object GetLookupAttribute(IDatabase db, int rowID, string attributeName);

        /// <summary>
        /// Возвращает значение лукап-атрибута
        /// </summary>
        /// <param name="rowID">ID записи</param>
        /// <param name="attributeName">Наименование атрибута значение которого нужно получить</param>
        /// <returns>Значение лукап-атрибута</returns>
        object GetLookupAttribute(int rowID, string attributeName);

        /// <summary>
        /// Возвращает таблицу фиксированных записей
        /// </summary>
        /// <returns>Таблица фиксированных записей</returns>
        DataTable GetFixedRowsTable();

        /// <summary>
        /// Устанавливает таблицу фиксированных записей
        /// </summary>
        /// <param name="dt">Таблица фиксированных записей</param>
        void SetFixedRowsTable(DataTable dt);

        /// <summary>
        /// Изменяет диапазон записей в котором они находятся 
        /// с пользовательского диапазона на диапазон разработчика и наоборот.
        /// </summary>
        /// <param name="rowsID">Массив исходных ID записей.</param>
        /// <returns>Массив новых ID записей.</returns>
        /// <remarks>
        /// Если массив исходных ID записей rowsID передается пустой, 
        /// то диапазон меняется для всех записей классификатора.
        /// </remarks>
        int[] ReverseRowsRange(int[] rowsID);
    }

    /// <summary>
    /// Прототип функции обратного вызова информирования клиента
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    public delegate void VariantListenerDelegate(string message);

    /// <summary>
    /// Классификатор вариантов
    /// </summary>
    public interface IVariantDataClassifier
    {
        /// <summary>
        /// Копирование варианта
        /// </summary>
        /// <param name="variantID">ID варианта</param>
        /// <returns>ID копии варианта</returns>
        int CopyVariant(int variantID);

        /// <summary>
        /// Копирование варианта
        /// </summary>
        /// <param name="variantID">ID варианта</param>
        /// <param name="listener">Функция обратного вызова информирования клиента</param>
        /// <returns>ID копии варианта</returns>
        int CopyVariant(int variantID, VariantListenerDelegate listener);

        /// <summary>
        /// Удаление варианта
        /// </summary>
        /// <param name="variantID">ID варианта</param>
        void DeleteVariant(int variantID);

        /// <summary>
        /// Удаление варианта
        /// </summary>
        /// <param name="variantID">ID варианта</param>
        /// <param name="listener">Функция обратного вызова информирования клиента</param>
        void DeleteVariant(int variantID, VariantListenerDelegate listener);

        /// <summary>
        /// Блокировка(закрытие) варианта
        /// </summary>
        /// <param name="variantID">ID варианта</param>
        void LockVariant(int variantID);

        /// <summary>
        /// Открытие варианта
        /// </summary>
        /// <param name="variantID">ID варианта</param>
        void UnlockVariant(int variantID);
    }

    /// <summary>
    /// Коллекция классификаторов данных
    /// </summary>
    public interface IClassifierCollection : IEntityCollection<IClassifier>
    {
    }

    /// <summary>
    /// Сопоставимый классификатор
    /// </summary>
    public interface IBridgeClassifier : IClassifier
    {
    }

    /// <summary>
    /// Таблица перекодировок
    /// </summary>
    public interface IConversionTable : IDisposable
    {
        /// <summary>
        /// Наименование ассоциации к которой относится таблица перекодировок
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Полное наименовение ( = Name + "." + RuleName)
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Наименование правила сопоставления в котором используется таблицаперекодировок
        /// </summary>
        string RuleName { get; }

        /// <summary>
        /// Семантика
        /// </summary>
        string SemanticName { get; set; }

        /// <summary>
        /// Добавляет новую перекодировку
        /// </summary>
        /// <param name="fromRow">входная строка</param>
        /// <param name="toRow">выходная строка</param>
        void AddConversion(object[] fromRow, object[] toRow);
        //void AddConversion(DataRow fromRow, DataRow toRow);

        /// <summary>
        /// DataUpdater для просмотра и редактирования таблицы перекодировки
        /// </summary>
        /// <returns></returns>
        IDataUpdater GetDataUpdater();

        /// <summary>
        /// Удаляет все данные из таблицы перекодировки
        /// </summary>
        void Clear();

        /// <summary>
        /// Удаляет строку из таблицы перекодировок
        /// </summary>
        /// <param name="rowID">ID записи которую необходимо удалить</param>
        void DeleteRow(int rowID);
    }

    /// <summary>
    /// Коллекция таблиц перекодировок
    /// </summary>
    public interface IConversionTableCollection : ICollection2DataTable
    {
        /// <summary>
        /// Определяет содержит ли коллекция таблицу с указанным именем.
        /// </summary>
        /// <param name="association">Ассоциация для которой нужно найти таблицу перекодировки.</param>
        /// <param name="ruleName">Имя правила сопоставления, которому соответствует таблица перекодировки.</param>
        /// <returns>true если таблица в базе данных с таким именем существует, иначе false.</returns>
        bool ContainsKey(IAssociation association, string ruleName);

        /// <summary>
        /// Количество таблиц перекодировок в базе данных
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Коллекция имен таблиц перекодировок
        /// </summary>
        ICollection Keys { get; }

        /// <summary>
        /// Индексатор
        /// </summary>
        /// <param name="key">Имя таблицы перекодировки</param>
        /// <returns>Интерфейс таблицы перекодировки</returns>
        IConversionTable this[string key] { get; }

        /// <summary>
        /// Индексатор возвращает таблицу перекодировки для указанной ассоциации и заданным правилом сопоставления
        /// </summary>
        /// <param name="association">Ассоциация для которой необходимо получить таблицу перекодировки</param>
        /// <param name="ruleName">Наименование правила сопоставления</param>
        IConversionTable this[IAssociation association, string ruleName] { get; }
    }

    /// <summary>
    /// Значение используемое в правиле сопоставления
    /// </summary>
    [SMOInterface]
    public interface IMappingValue : ISMOSerializable
    {
        /// <summary>
        /// Наименование поля
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string Name { get; }

        /// <summary>
        /// Определяет простое значение или сложное составное
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        bool IsSample { get; }

        /// <summary>
        /// Описание атрибута содержащего значение
        /// </summary>
        IDataAttribute Attribute { get; set; }

        /// <summary>
        /// Наименования хранимых в базе аттрибутов на основе которых получается значение
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string[] SourceAttributes { get; }
    }

    /// <summary>
    /// Маппинг для формирования и сопоставления классификаторов
    /// </summary>
    [SMOInterface]
    public interface IAssociateMapping : IKeyIdentifiedObject
    {
        /// <summary>
        /// Значение со стороны классификатора данных
        /// </summary>
        [SMOSerializable(ReturnType.Object)]
        IMappingValue DataValue { get; }

        /// <summary>
        /// Значение со стороны сопоставимого классификатора данных
        /// </summary>
        [SMOSerializable(ReturnType.Object)]
        IMappingValue BridgeValue { get; }
    }


    /// <summary>
    /// Коллекция правил формирования
    /// </summary>
    [SMOInterface]
    public interface IAssociateMappingCollection : IListBase<IAssociateMapping>
    {
        /// <summary>
        /// Создание нового объекта
        /// </summary>
        /// <returns>Новый созданный объект</returns>
        IAssociateMapping CreateItem();
    }

    /// <summary>
    /// Правило сопоставления
    /// </summary>
    [SMOInterface]
    public interface IAssociateRule : IMinorModifiable, IKeyIdentifiedObject
    {
        /// <summary>
        /// Наименование правила
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string Name { get; set; }

        /// <summary>
        /// true - сопоставление с использованием таблицы перекодировок; false - без таблицы перекодировок
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        bool UseConversionTable { get; set; }

        /// <summary>
        /// true - сопоставление по совпадению полей; false - не учитывать совпадение.
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        bool UseFieldCoincidence { get; set; }

        /// <summary>
        /// true - Автоматически добавлять новые записи в сопоставимый; false - не добавлять новые записи
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        bool AddNewRecords { get; set; }

        /// <summary>
        /// true - автоматически сопоставлять все записи; false - сопоставлять только несопоставленные
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        bool ReAssociate { get; set; }

        /// <summary>
        /// Определяет можно ли изменять свойства правила сопоставления
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        bool ReadOnly { get; }

        /// <summary>
        /// Настройки сопоставления для строковых полей, если null, то используются значения по умолчанию
        /// </summary>
        StringElephanterSettings Settings { get; set; }

        /// <summary>
        /// Маппинг полей
        /// </summary>
        [SMOSerializable(ReturnType.Dictionary, LevelSerialization.DeepSerialize)]
        IAssociateMappingCollection Mappings { get; }

        /// <summary>
        /// Создает копию правила доступного для редактирования
        /// </summary>
        /// <returns>Копия правила сопоставления</returns>
        /// <remarks>По окончанию работы с копией правила необходимо вызвать метод Dispose!</remarks>
        IAssociateRule CloneRule();
    }

    /// <summary>
    /// Коллекция правил сопоставления
    /// </summary>
    [SMOInterface]
    public interface IAssociateRuleCollection : IDictionaryBase<string, IAssociateRule>
    {
        /// <summary>
        /// Имя правила, используемого по умолчанию.
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string AssociateRuleDefault { get; set; }
    }

    /// <summary>
    /// UML пакет
    /// </summary>
	[SMOInterface]
	public interface IPackage : ICommonDBObject
    {
        /// <summary>
        /// Локальный путь к файлу пакета, если не указан, то пакет встроенный
        /// </summary>
        string PrivatePath { get; set; }

        /// <summary>
        /// Коллекция классов принадлежащих пакету
        /// </summary>
        IPackageCollection Packages { get; }

        /// <summary>
        /// Коллекция классов принадлежащих пакету
        /// </summary>
        IEntityCollection<IEntity> Classes { get; }

        /// <summary>
        /// Коллекция ассоциаций принадлежащих пакету
        /// </summary>
        IEntityAssociationCollection Associations { get; }

        /// <summary>
        /// Коллекция документов принадлежащих пакету
        /// </summary>
        IDocumentCollection Documents { get; }

        /// <summary>
        /// Поиск таблицы по имени в пакете и подпакетах
        /// </summary>
        /// <param name="name">Полное имя объекта</param>
        /// <returns>Найденый объект</returns>
        IEntity FindEntityByName(string name);

        /// <summary>
        /// Поиск ассоциации по имени в пакете и подпакетах
        /// </summary>
        /// <param name="name">Полное имя объекта</param>
        /// <returns>Найденый объект</returns>
        IEntityAssociation FindAssociationByName(string name);

        /// <summary>
        /// Возвращает состояние файла в SourceSafe
        /// </summary>
        VSSFileStatus IsCheckedOut { get; }

        /// <summary>
        /// Поиск объектов в схеме.
        /// </summary>
        /// <param name="searchParam">Условия поиска</param>
        /// <param name="searchTable">Таблица результатов</param>
        bool Search(SearchParam searchParam, ref DataTable searchTable);

        /// <summary>
        /// Проверяет пакет на корректность состояния.
        /// </summary> 
        /// <returns>Таблица с результатами проверки. Если null, то ошибок при проверке не обнаружено.</returns>
        DataTable Validate();

        /// <summary>
        /// Ищет зависимые данные по источникам.
        /// </summary>
        /// <param name="sID">ID источника.</param>
        /// <returns>DataTable с зависимыми данными.</returns>
        DataTable GetSourcesDependedData(int sID);

        /// <summary>
        /// Получаем таблицу конфликтных зависимостей между пакетами
        /// </summary>
        /// <returns></returns>
        DataTable GetConflictPackageDependents();

        /// <summary>
        /// Для пакета определяем от каких пакетов в схеме он зависит, включая зависимые через промежуточный пакет
        /// </summary>
        /// <returns> Коллекция зависимых пакетов</returns>
        List<string> GetDependentsByPackage();

        /// <summary>
        /// Сортировка объектов внутри пакета с учетом внутренних зависимостей
        /// </summary>
        /// <returns>Отсортированная коллекция объектов</returns>
        List<IEntity> GetSortEntitiesByPackage();

        /// <summary>
        /// Сохранение пакета в репозиторий
        /// </summary>
        void SaveToDisk();
    }

    /// <summary>
    /// Коллекция классификаторов данных
    /// </summary>
    public interface IPackageCollection : IModifiableCollection<string, IPackage>
    {
    }

    #region Работа с представлениями для реалиционной таблицы

    /// <summary>
    /// Интерфейс, реализующий представление для реляционной таблицы
    /// </summary>
    [SMOInterface]
    public interface IPresentation : IMinorObject
    {
        /// <summary>
        /// Имя представления
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string Name { get; set; }

        /// <summary>
        /// Коллекция атрибутов для прдставления
        /// </summary>
        [SMOSerializable(ReturnType.Dictionary, LevelSerialization.DeepSerialize)]
        IDataAttributeCollection Attributes { get; }

        /// <summary>
        /// Наименование уровней иерархии
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string LevelNamingTemplate { get; set; }

        /// <summary>
        /// Xml-клнфигурация представления
        /// </summary>
        string Configuration
        {
            get;
        }

        /// <summary>
        /// Коллекция сгруппированных атрибутов
        /// </summary>
        IDataAttributeCollection GroupedAttributes { get; }
    }

    /// <summary>
    /// Коллекция представлений для реляционной таблицы
    /// </summary>
    [SMOInterface]
    public interface IPresentationCollection : IDictionaryBase<string, IPresentation> 
    {
        /// <summary>
        /// Иненификатор представления по умолчанию
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string DefaultPresentation { get; set; }

        /// <summary>
        /// Создание нового представления.
        /// </summary>
        /// <returns>Новый созданный объект</returns>
        IPresentation CreateItem(string key, string name, List<IDataAttribute> attributes,
            string levelNamingTemplate);
    }

    #endregion Работа с представлениями для реалиционной таблицы

    /// <summary>
    /// Интерфейс реализующий реляционную таблицу
    /// </summary>
    [SMOInterface]
    public interface IEntity : ICommonDBObject
    {
        /// <summary>
        /// Коллекция атрибутов объекта
        /// </summary>
        [SMOSerializable(ReturnType.Dictionary, LevelSerialization.DeepSerialize)]
        IDataAttributeCollection Attributes { get; }

        /// <summary>
        /// Коллекция представлений объекта
        /// </summary>
        [SMOSerializable(ReturnType.Dictionary, LevelSerialization.DeepSerialize)]
        IPresentationCollection Presentations { get; }

        /// <summary>
        /// Коллекция уникальных ключей
        /// </summary>
        [SMOSerializable(ReturnType.Dictionary, LevelSerialization.DeepSerialize)]
        IUniqueKeyCollection UniqueKeys { get; }

        /// <summary>
        /// Возвращает признак допустимости создания уникальных ключей у данного вида объектов
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        bool UniqueKeyAvailable { get; }

        /// <summary>
        /// Класс объекта
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        ClassTypes ClassType { get; }

        /// <summary>
        /// Подкласс объекта
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        SubClassTypes SubClassType { get; set; }

        /// <summary>
        /// Короткое название объекта, испоьзуется для построения кубов
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string ShortCaption { get; set; }

        /// <summary>
        /// Обработчики событий.
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string MacroSet { get; set; }

        /// <summary>
        /// Полное русское наименование объекта, состоящее из семантического имени и названия объекта разделенных точкой.
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string FullCaption { get; }

        /// <summary>
        /// Возвращает имя соответствующего OLAP-объекта
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string OlapName { get; }

        /// <summary>
        /// Возвращает наименование генератора (FullDBName обрезается до 28 символов).
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string GeneratorName { get; }

        /// <summary>
        /// Возвращает наименование генератора для режима "Разработчика" (FullDBName обрезается до 28 символов).
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string DeveloperGeneratorName { get; }

        /// <summary>
        /// Возвращает следующее значение генератора
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int GetGeneratorNextValue { get; }

        /// <summary>
        /// Ассоциации сущъности (внение ключи).
        /// </summary>
        [SMOSerializable(ReturnType.Dictionary, LevelSerialization.Serialize)]
        IEntityAssociationCollection Associations { get; }

        /// <summary>
        /// Ассоциации, которые ссылаются на сущъность.
        /// </summary>
        [SMOSerializable(ReturnType.Dictionary, LevelSerialization.Serialize)]
        IEntityAssociationCollection Associated { get; }

        /// <summary>
        /// Возвращает количество записей по указанному источнику.
        /// </summary>
        /// <param name="sourceID">ID источника данных. Если равен -1, то возвращает по всем источникам.</param>
        /// <returns>количество записей по источнику</returns>
        int RecordsCount(int sourceID);

        /// <summary>
        /// Возвращает объект для заполнения и обновления данных
        /// </summary>
        IDataUpdater GetDataUpdater();

        /// <summary>
        /// Возвращает объект для заполнения и обновления данных
        /// </summary>
        IDataUpdater GetDataUpdater(string selectFilter, int? maxRecordCountInSelect, params IDbDataParameter[] selectFilterParameters);

        /// <summary>
        /// Удаляет данные объекта
        /// </summary>
        /// <param name="whereClause">
        /// Выражение "where Param1 = ? and Param2 = ? or ...", 
        /// если условия нет, то нужно передавать пустую строку: String.Empty</param>
        /// <param name="parameters">Значения параметров перечисленных в whereClause</param>
        /// <returns>Количество удаленных записей</returns>
        int DeleteData(string whereClause, params object[] parameters);

        /// <summary>
        /// Удаляет данные объекта с отключением аудита
        /// </summary>
        /// <param name="whereClause">
        /// Выражение "where Param1 = ? and Param2 = ? or ...", 
        /// если условия нет, то нужно передавать пустую строку: String.Empty</param>
        /// <param name="parameters">Значения параметров перечисленных в whereClause</param>
        /// <param name="disableTriggerAudit">true - отключиние аудина
        /// false - эмуляция работы триггера аудита с отключенным триггером</param>
        /// <returns></returns>
        int DeleteData(string whereClause, bool disableTriggerAudit, params object[] parameters);

        /// <summary>
        /// Ищет зависимые данные.
        /// </summary>
        /// <param name="rowID">Идентификатор строки в таблице</param>
        /// <param name="recursive">Следует ли искать зависимые для зависимых.</param>
        DataSet GetDependedData(int rowID, bool recursive);

        /// <summary>
		/// Возвращает описание типа объекта.
		/// </summary>
        string GetObjectType();

        /// <summary>
        /// Выполняет дополнительную обработку данных в таблице.
        /// </summary>
        bool ProcessObjectData();

        /// <summary>
        /// Производит слияние дубликатов.
        /// </summary>
        /// <param name="mainRecordID">ID основной записи.</param>
        /// <param name="duplicateRecordID">ID дубликата.</param>
        /// <param name="listener">Функция информирования клиента.</param>
        void MergingDuplicates(int mainRecordID, List<int> duplicateRecordID, MergeDuplicatesListener listener);

        /// <summary>
        /// Коллекция сгруппированных атрибутов
        /// </summary>
        IDataAttributeCollection GroupedAttributes { get; }
    }

	/// <summary>
	/// Табличный документ.
	/// </summary>
	public interface IDocumentEntity : IEntity
	{
	}

    /// <summary>
    /// Прототип функции информирования клиента.
    /// </summary>
    /// <param name="message"></param>
    public delegate void MergeDuplicatesListener(string message);

    /// <summary>
    /// Интерфейс объекта, который поддерживает расчет соответствующих ему объектов многомерной базы.
    /// </summary>
    public interface IProcessableObject
    {
        /// <summary>
        /// Расчитывает многомерный объект.
        /// </summary>
        void Process();

        /// <summary>
        /// Расчитывает многомерный объект.
        /// </summary>
        /// <param name="sourceID">ID источника партицию которого необходимо расчитать.</param>
        void Process(int sourceID);

        /// <summary>
        /// Расчитывает многомерный объект.
        /// </summary>
        /// <param name="sourceID">ID источника партицию которого необходимо расчитать.</param>
        /// <param name="pumpID">ID операции закачки.</param>
        void Process(int sourceID, int pumpID);
    }

    /// <summary>
    /// Коллекция классификаторов данных
    /// </summary>
    [SMOInterface]
    public interface IEntityCollection<TItemIntf> : IDictionaryBase<string, TItemIntf>, ICollection2DataTable
    {
        /// <summary>
        /// Создание нового объекта
        /// </summary>
        /// <param name="classType">Класс объекта</param>
        /// <param name="subClassType">Подкласс объекта</param>
        /// <returns>Новый созданный объект</returns>
        TItemIntf CreateItem(ClassTypes classType, SubClassTypes subClassType);
    }

    /// <summary>
    /// Интерфейс реализующий ассоциацию между двумя реляционными таблицами
    /// </summary>
    [SMOInterface]
    public interface IEntityAssociation : ICommonDBObject
    {
        /// <summary>
        /// Класс ассоциации
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        AssociationClassTypes AssociationClassType { get; }

        /// <summary>
        /// Составное наименование ассоциации включающее наименование обоих ролей
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string FullCaption { get; }

        /// <summary>
        /// Поле роли А
        /// </summary>
        IDataAttribute RoleDataAttribute { get; }

        /// <summary>
        /// Ссылка на роль данных (таблица фактов, классификатор данных, сопоставимый)
        /// </summary>
        IEntity RoleData { get; }

        /// <summary>
        /// Ссылка на роль справочника данных (классификатор данных, сопоставимый)
        /// </summary>
        IEntity RoleBridge { get; }
    }

    /// <summary>
    /// Ассоциация
    /// </summary>
    public interface IAssociation : IEntityAssociation
    {
    }

    /// <summary>
    /// Ассоциация сопоставления.
    /// </summary>
    [SMOInterface]
    public interface IBridgeAssociation : IAssociation
    {
        /// <summary>
        /// Сопоставляет объект RoleData с сопоставимым классификатором RoleBridge
        /// </summary>
        /// <returns>Количество обработанных записей(сопоставленных)</returns>
        int Associate();

        /// <summary>
        /// Сопоставляет объект RoleData с сопоставимым классификатором RoleBridge.
        /// </summary>
        /// <param name="sourceID">ID источника, данные которого будут сопоставляться</param>
        /// <returns>Количество обработанных записей(сопоставленных)</returns>
        int Associate(int dataClsSourceId, int bridgeClsSourceID);

        /// <summary>
        /// Сопоставляет объект RoleData с сопоставимым классификатором RoleBridge.
        /// </summary>
        /// <param name="sourceID">ID источника, данные которого будут сопоставляться</param>
        /// <param name="pumpID">ID закачки, данные которой будут сопоставляться</param>
        /// <returns>Количество обработанных записей(сопоставленных)</returns>
        int Associate(int dataClsSourceId, int bridgeClsSourceID, int pumpID);

        /// <summary>
        /// Сопоставляет объект RoleData с сопоставимым классификатором RoleBridge.
        /// </summary>
        /// <param name="sourceID">ID источника, данные которого будут сопоставляться</param>
        /// <param name="pumpID">ID закачки, данные которой будут сопоставляться</param>
        /// <param name="allowDigits">Учитывать цифровые символы</param>
        /// <param name="reAssociate">true - автоматически сопоставлять все записи; false - сопоставлять только несопоставленные</param>
        /// <returns>Количество обработанных записей(сопоставленных)</returns>
        int Associate(int dataClsSourceId, int bridgeClsSourceID, int pumpID, bool allowDigits, bool reAssociate);

        /// <summary>
        /// Сопоставление по равенству атрибутов или с использования таблицы перекодировок
        /// </summary>
        /// <param name="sourceID">ID источника по данные которого будет сопоставляться</param>
        /// <param name="associateRule">Правило сопоставления</param>
        /// <returns>Количество обработанных(сопоставленных) записей</returns>
        int Associate(int dataClsSourceId, int bridgeClsSourceID, IAssociateRule associateRule);

        /// <summary>
        /// Сопоставление по равенству атрибутов или с использования таблицы перекодировок
        /// </summary>
        /// <param name="sourceID">ID источника по данные которого будет сопоставляться</param>
        /// <param name="associateRuleObjectKey">Уникальный ключ правила сопоставления</param>
        /// <param name="stringSettings">Параметры работы со строками при сопоставлении</param>
        /// <param name="ruleParams">параметры сопоставления</param>
        /// <returns></returns>
        int Associate(int dataClsSourceId, int bridgeClsSourceID, string associateRuleObjectKey, StringElephanterSettings stringSettings, AssociationRuleParams ruleParams);

        /// <summary>
        /// Очистка сопоставления
        /// </summary>
        /// <param name="DataSourceID">ID источника, если -1, то очищать по всем источникам</param>
        void ClearAssociationReference(int DataSourceID);

        /// <summary>
        /// Формирует сопоставимый классификатор по классификатору данных
        /// </summary>
        /// <param name="dataSourceID">ID источника по данным которого будет формироваться сопоставимый классификатор</param>
        /// <returns>Количество сформированных записей</returns>
        int FormBridgeClassifier(int dataSourceID, int bridgeSourceID);

        /// <summary>
        /// Копирует запись из классификатора данных в сопоставимый и устанавливает соответствие сопоставления
        /// </summary>
        /// <param name="rowID">ID записи в классификаторе данных</param>
        /// <returns>ID добавленной записи</returns>
        int CopyAndAssociateRow(int rowID, int bridgeSourceID);

        /// <summary>
        /// Копирует запись из классификатора данных в сопоставимый с установкой иерархии и устанавливает соответствие сопоставления
        /// </summary>
        /// <param name="rowID">ID записи в классификаторе данных</param>
        /// <returns>ID добавленной записи</returns>
        int CopyAndAssociateRow(int rowID, int parentId, int bridgeSourceID);

        /// <summary>
        /// Возвращает количество записей, сопоставленных с данной версией сопоставимого
        /// </summary>
        /// <param name="sourceID">Версия сопоставимого</param>
        /// <returns>Количество сопоставленных записей по всем классификаторам</returns>
        int GetCountBridgeRow(int sourceID);

        /// <summary>
        /// Перенос ссылок на новую версию классификатора
        /// </summary>
        /// <param name="sourceID">ID источника новой версии</param>
        /// <param name="pumpID"></param>
        /// <param name="oldSourceID">ID источника предыдущей версии</param>
        /// <returns>Количество перенесенных данных</returns>
        int ReplaceLinkToNewVersionCls(int sourceID, int pumpID, int oldSourceID);

        /// <summary>
        /// Возвращает true, если есть праволо формирования сопставимого
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        bool MappingRuleExist { get; }

        /// <summary>
        /// Привила формирования
        /// </summary>
        [SMOSerializable(ReturnType.Dictionary, LevelSerialization.DeepSerialize)]
        IAssociateMappingCollection Mappings { get; }

        /// <summary>
        /// Правила сопоставления
        /// </summary>
        [SMOSerializable(ReturnType.Dictionary, LevelSerialization.DeepSerialize)]
        IAssociateRuleCollection AssociateRules { get; }

		/// <summary>
		/// Возвращает правило сопоставления используемое по умолчанию.
		/// </summary>
		/// <returns>Имя правила сопоставления.</returns>
		string GetDefaultAssociateRule();
		
		/// <summary>
		/// Устанавливает правило сопоставления используемое по умолчанию.
		/// </summary>
		/// <param name="key">Имя правила сопоставления.</param>
		void SetDefaultAssociateRule(string key);

        /// <summary>
        /// Количество всех записей в сопоставляемом классификаторе
        /// </summary>
        int GetAllRecordsCount();

        /// <summary>
        /// Количество всех несопоставленных записей в сопоставляемом классификаторе
        /// </summary>
        int GetAllUnassociateRecordsCount();

        /// <summary>
        /// Количество записей в сопоставляемом классификаторе по текущему источнику
        /// </summary>
        int GetRecordsCountByCurrentDataSource(int sourceID);

        /// <summary>
        /// Количество несопоставленных записей в сопоставляемом классификаторе по текущему источнику
        /// </summary>
        int GetUnassociateRecordsCountByCurrentDataSource(int sourceID);

        /// <summary>
        /// устанавливает все количества записей в -1 для обновления этих значений
        /// </summary>
        void RefreshRecordsCount();

        /// <summary>
        /// Возвращаяет SourceID версии сопоставимого, с которым сопоставлен версия сопоставляемого, 
        /// -1 - если нет сопоставления
        /// </summary>
        /// <returns></returns>
        int GetAssociateBridgeSourceID(IDatabase db, int dataClsSourceID);
    }

    /// <summary>
    /// Интерфейс сопоставления ассоциации сопоставления с Расходы.Базовый
    /// </summary>
    public interface IBridgeAssociationReport
    {
        /// <summary>
        /// Добавление в Расходы.Базовый всех несопоставленных записей
        /// </summary>
        void CopyAndAssociateAllRow(int sourceID, int bridgeSourceID);
    }

    /// <summary>
    /// Параметры сопоставления классификаторов
    /// </summary>
    [Serializable]
    public struct AssociationRuleParams
    {
        /// <summary>
        /// true - автоматически сопоставлять все записи; false - сопоставлять только несопоставленные
        /// </summary>
        public bool ReAssiciate;
        /// <summary>
        /// true - Автоматически добавлять новые записи в сопоставимый; false - не добавлять новые записи
        /// </summary>
        public bool AddNewRecords;
        /// <summary>
        /// true - сопоставление с использованием таблицы перекодировок; false - без таблицы перекодировок
        /// </summary>
        public bool UseConversionTable;
        /// <summary>
        /// true - сопоставление по совпадению полей; false - не учитывать совпадение.
        /// </summary>
        public bool UseFieldCoincidence;
    }

    /// <summary>
    /// Коллекция ассоциация
    /// </summary>
    [SMOInterface]
    public interface IEntityAssociationCollection : IDictionaryBase<string, IEntityAssociation>, ICollection2DataTable
    {
        /// <summary>
        /// Создание нового объекта
        /// </summary>
        /// <param name="roleA">Откуда</param>
        /// <param name="roleB">Куда</param>
        /// <returns>Новый созданный объект</returns>
        [Obsolete("Использовать метод CreateItem(IEntity roleA, IEntity roleB, AssociationClassTypes associationClassType).")]
        IEntityAssociation CreateItem(IEntity roleA, IEntity roleB);

        /// <summary>
        /// Создание нового объекта.
        /// </summary>
        /// <param name="roleA">Откуда</param>
        /// <param name="roleB">Куда</param>
        /// <param name="associationClassType">Класс объекта</param>
        /// <returns>Новый созданный объект</returns>
        IEntityAssociation CreateItem(IEntity roleA, IEntity roleB, AssociationClassTypes associationClassType);
    }

    /// <summary>
    /// Коллекция ассоциация
    /// </summary>
    public interface IAssociationCollection : IEntityAssociationCollection
    {
    }

    /// <summary>
    /// Таблица фактов
    /// </summary>
    [SMOInterface]
    public interface IFactTable : IDataSourceDividedClass
    {
        /// <summary>
        /// Возвращает список источников по которым сформирован куб
        /// </summary>
        /// <returns>Key - ID источника данных; Value - описание источника</returns>
        Dictionary<int, string> GetDataSourcesNames();

        /// <summary>
        /// Возвращает имя партиции куба по идентификатору источника данных.
        /// </summary>
        /// <param name="sourceID">Идентификатор источника данных.</param>
        /// <returns>Имя партиции куба.</returns>
        List<string> GetPartitionsNameBySourceID(int sourceID);
    }

    /// <summary>
    /// Коллекция таблиц фактов
    /// </summary>
    [SMOInterface]
    public interface IFactTableCollection : IEntityCollection<IFactTable>
    {
    }

    /// <summary>
    /// Документ
    /// </summary>
    public interface IDocument : ICommonDBObject
    {
        /// <summary>
        /// Тип документа
        /// </summary>
        DocumentTypes DocumentType { get; }

        /// <summary>
        /// Представление документа в виде XML
        /// </summary>
        string Configuration { get; set; }
    }

    public interface IDocumentCollection : IModifiableCollection<string, IDocument>
    {
    	IDocument CreateItem(DocumentTypes documentType);
    }

    /// <summary>
    /// Инерфейс серверного объекта схемы
    /// </summary>
    [SMOInterface]
    public interface IScheme : IMajorModifiable, ISMOSerializable, IServiceProvider
    {
        /// <summary>
        /// Наименование схемы
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string Name { get; set; }

        /// <summary>
        /// Возвращает ссылку на главный объект сервера приложений
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IServer Server { get; set; }

        /// <summary>
        /// Возвращает ссылку на объект хранилища данных схемы
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ISchemeDWH SchemeDWH { get; }

        /// <summary>
        /// Возвращает ссылку на объект многомерной базы данных схемы
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ISchemeMDStore SchemeMDStore { get; }

        /// <summary>
        /// Возвращает ссылку на объект, управляющий источниками данных схемы
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IDataSourceManager DataSourceManager { get; }

        /// <summary>
        /// Возвращает ссылку на объект, управляющий версиями классифмкаторов
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IDataVersionManager DataVersionsManager { get; }

        /// <summary>
        /// Возвращает ссылку на объект, управляющий программами закачки
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IDataPumpManager DataPumpManager { get; }

        /// <summary>
        /// Возвращает интерфейс на объект управляющий задачами схемы
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ITaskManager TaskManager { get; }

        /// <summary>
        /// Сервис шаблонов.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ITemplatesService TemplatesService { get; }

        /// <summary>
        /// Возвращает интерфейс обекта для работы с пользователями и правами
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IUsersManager UsersManager { get; }

        /// <summary>
        /// Возвращает ссылку на объект управляющий архивными папками схемы
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IArchive Archive { get; }

        /// <summary>
        /// Возвращает базовый каталог схемы
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string BaseDirectory { get; }

        /// <summary>
        /// Путь к расшаренному архивному каталогу  
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string ArchiveDirectory { get; }

        /// <summary>
        /// Путь к расшаренному каталогу источников данных 
        /// </summary>
        [SMOSerializable(ReturnType.Value)]
        string DataSourceDirectory { get; }

        /// <summary>
        /// Инициализирует экземпляр объекта для ведения логов и возвращает ссылку
        /// </summary>
        IBaseProtocol GetProtocol(string CallerAssemblyName);

        /// <summary>
        /// Инициализирует экземпляр объекта для получения данных аудита
        /// </summary>
        /// <returns></returns>
        IDataOperations GetAudit();

        /// <summary>
        /// Инициализирует экземпляр объекта для серверного экспорта и импорта из XML документов
        /// </summary>
        /// <returns></returns>
        IExportImportManager GetXmlExportImportManager();

        IReportsTreeService ReportsTreeService
        { 
            get;
        }

        /// <summary>
        /// Возвращает ссылку на объект, управляющий константами системы
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IGlobalConstsManager GlobalConstsManager { get; }

        /// <summary>
        /// Главный корневой пакет схемы
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IPackage RootPackage { get; }

        /// <summary>
        /// Коллекция пакетов
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IPackageCollection Packages { get; }

        /// <summary>
        /// Коллекцию классификаторов
        /// </summary>
        [SMOSerializable(ReturnType.Dictionary, LevelSerialization.Serialize)]
        IEntityCollection<IClassifier> Classifiers { get; }

        /// <summary>
        /// Возвращает коллекцию таблиц фактов
        /// </summary>
        [SMOSerializable(ReturnType.Dictionary, LevelSerialization.Serialize)]
        IEntityCollection<IFactTable> FactTables { get; }

        /// <summary>
        /// Коллекция ассоциаций
        /// </summary>
        [SMOSerializable(ReturnType.Dictionary, LevelSerialization.Serialize)]
        IEntityAssociationCollection Associations { get; }

        /// <summary>
        /// Коллекция ассоциаций
        /// </summary>
        [SMOSerializable(ReturnType.Dictionary, LevelSerialization.Serialize)]
        IEntityAssociationCollection AllAssociations { get; }

        /// <summary>
        /// Коллекция таблиц перекодировок
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IConversionTableCollection ConversionTables { get; }

        /// <summary>
        /// Список семантицеской принадлежности
        /// </summary>
        [SMOSerializable(ReturnType.Dictionary, LevelSerialization.Serialize)]
        ISemanticsCollection Semantics { get; }

        /// <summary>
        /// Провайдер для получения метаданных схемы
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IPlaningProvider PlaningProvider { get; }

        /// <summary>
        /// Интерфейс на объект обратной записи
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IWriteBackServer WriteBackServer { get; }

        /// <summary>
        /// Интерфейс правил расщепления
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IDisintRules DisintRules { get; }

        /// <summary>
        /// Фасад системы планирования источников финансирования.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IFinSourcePlanningFace FinSourcePlanningFace { get; }

		/// <summary>
		/// Интерфейс системы планирования развития региона
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		IForecastService ForecastService { get; }

		/// <summary>
		/// Интерфейсa формы2п системы планирования развития региона
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		IForm2pService Form2pService { get; }


        /// <summary>
        /// Менеджер сессий
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ISessionManager SessionManager { get; }

        /// <summary>
        /// Возвращает объект по его уникальному ключу
        /// </summary>
        /// <param name="key">Уникальный ключ объекта</param>
        /// <returns>Объект схемы</returns>
        ICommonDBObject GetObjectByKey(string key);

        /// <summary>
        /// Рассчитывалка многомерной базы.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IProcessor Processor { get; }

        /// <summary>
        /// Возвращает объект менеджера сообщений.
        /// </summary>
        IMessageManager MessageManager { get; }

        /// <summary>
        /// Собранная информация о сервере
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DataTable ServerSystemInfo { get; }

        /// <summary>s
        /// Создает контекст для применения изменений к схеме.
        /// </summary>
        /// <returns>Контекст в котором выполняются изменения структуры схемы.</returns>
        IModificationContext CreateModificationContext();

        /// <summary>
        /// Фасад доступа к SourceSafe.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IVSSFacade VSSFacade { get; }

        /// <summary>
        /// XML-конфигурация для генерации справки по семантической структуре
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string ConfigurationXMLDocumentation { get; }

        /// <summary>
        /// Определяет режим работы сервера с базой данных.
        /// Если с базой данных уже работает какой либо сервер приложений, то режим будет MultiServerMode.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool MultiServerMode { get; }

        /// <summary>
        /// Функция перехода на новый год
        /// </summary>
        /// <param name="currentYear"> Текущий год</param>
        /// <returns> Конечное состояние работы функции</returns>
        TransferDBToNewYearState TransferDBToNewYear(int currentYear);
    }

    /// <summary>
    /// Конечные состояния операции перехода на новый год
    /// </summary>
    public enum TransferDBToNewYearState
    {
        /// <summary>
        /// Завершено успешно
        /// </summary>
        Successfully = 0,
        /// <summary>
        /// Завершено успешно с предупреждениями
        /// </summary>
        SuccessfullyWithWarning = 1,
        /// <summary>
        /// Завершено с ошибками
        /// </summary>
        Error = 2
    }

    public class SystemSchemeObjects
    {
        public static readonly string ROOT_PACKAGE_KEY = "fc75a7a2-d46a-4938-a43e-f376772b9d92";
        public static readonly string SYSTEM_PACKAGE_KEY = "c7a4196e-568e-482f-8219-921423b8a77a";
        public static readonly string YearDayUNV_ENTITY_KEY = "b4612528-0e51-4e6b-8891-64c22611816b";
        public static readonly string YearDay_ENTITY_KEY = "675ede52-a0b4-423a-b0f6-365ad02d0f6f";
        public static readonly string YearMonth_ENTITY_KEY = "30fd54c6-de78-4664-afb2-d5fcadc10e9c";
        public static readonly string Year_ENTITY_KEY = "c66d6056-7282-4ab0-ab0b-ea43ad68cb4c";
        public static readonly string SystemDataSources_ENTITY_KEY = "88fe2e16-cb45-4e55-9df6-26f07d28a582";
    }

    /// <summary>
    /// Параметры поиска.
    /// </summary>
    [Serializable]
    public struct SearchParam
    {
        public bool UseRegExp;
        public bool UseCaseSense;
        public bool UseWholeWord;
        public bool FindPackages;
        public bool FindClassifiers;
        public bool FindAssocitions;
        public bool FindDocuments;
        public bool FindAttributes;
        public bool FindHierarchyLevel;
        public bool FindAssociateMapping;
        public bool FindAssociateRule;
        public string pattern;
    }

    #endregion Server.Scheme

    #region Прочие интерфейсы

    /// <summary>
    /// Заглушка для управления временем жизни схемы.
    /// </summary>
    public interface ISchemeStub
    {
        /// <summary>
        /// Запускает схему.
        /// </summary>
        void Startup();

        /// <summary>
        /// Закрывает и выгружает схему.
        /// </summary>
        void Shutdown();
    }

    /*
    public enum NormativesKind { AllNormatives, NormativesBK, NormativesRegionRF, NormativesMR, VarNormativesRegionRF, VarNormativesMR, Unknown };

    /// <summary>
    /// Интерфейс правил расщепления
    /// </summary>
    public interface IDisintRules
    {
        /// <summary>
        /// Таблица правил расщепления
        /// </summary>
        IDataUpdater GetDisintRules_KD();

        /// <summary>
        /// Таблица альтернативных кодов для расщепления
        /// </summary>
        IDataUpdater GetDisintRules_ALTKD();

        /// <summary>
        /// Таблица исключений
        /// </summary>
        IDataUpdater GetDisintRules_EX();

        /// <summary>
        /// Исключения по району
        /// </summary>
        IDataUpdater GetDisintRules_ExPeriod();

        /// <summary>
        /// Исключения по периоду
        /// </summary>
        IDataUpdater GetDisintRules_ExRegion();

        /// <summary>
        /// Исключения района по периоду
        /// </summary>
        IDataUpdater GetDisintRules_ExBoth();

        /// <summary>
        /// возвращает норматив отчислений
        /// </summary>
        /// <param name="normatives"></param>
        /// <returns></returns>
        DataTable GetNormatives(NormativesKind normatives);

        /// <summary>
        /// применение изменений к нормативу
        /// </summary>
        /// <param name="normatives"></param>
        /// <param name="changes"></param>
        /// <returns></returns>
        bool ApplyChanges(NormativesKind normatives, DataTable changes);

        /// <summary>
        /// получение значения - ссылки на другой норматив
        /// </summary>
        /// <param name="normative"></param>
        /// <param name="refKD"></param>
        /// <param name="refYear"></param>
        /// <param name="refBudLevel"></param>
        /// <returns></returns>
        double GetConsRegionBudget(NormativesKind normative, int refKD, int refYear, int refBudLevel);
    }
    */
    /// <summary>
    /// Интерфейс проверки осмысленности комментариев
    /// </summary>
    public interface ICommentsCheckService
    {
        /// <summary>
        /// Проверяет текст комментария на осмысленность.
        /// </summary>
        /// <param name="comments">Текст комментария.</param>
        /// <returns>true - комментарий осмысленный.</returns>
        bool CheckComments(string comments);
    }

    #endregion Прочие интерфейсы'

    /// <summary>
    /// Фабрики классов объектов доступа к данных
    /// </summary>
    public abstract class ProviderFactoryConstants
    {
        public const string OracleDataAccess = "Krista.FM.Providers.OracleDataAccess";
		public const string MSOracleDataAccess = "Krista.FM.Providers.MSOracleDataAccess";
		public const string OracleClient = "System.Data.OracleClient";
        public const string SqlClient = "System.Data.SqlClient";
    }
}