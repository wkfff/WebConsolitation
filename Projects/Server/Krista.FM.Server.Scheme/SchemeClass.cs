using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Services;
using System.Threading;
using System.Xml;
using System.Xml.XPath;

using Krista.Diagnostics;
using Krista.FM.Common;
using Krista.FM.Common.Exceptions;
using Krista.FM.Common.Xml;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumpManagement;
using Krista.FM.Server.DataSourcesManager;
using Krista.FM.Server.GlobalConsts;
using Krista.FM.Server.Logger;
using Krista.FM.Server.MessagesManager;
using Krista.FM.Server.OLAP.Processor;
using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.Server.Providers.Planing;
using Krista.FM.Server.ReportsService;
using Krista.FM.Server.Scheme.Classes;
using Krista.FM.Server.Scheme.Modifications;
using Krista.FM.Server.Scheme.ScriptingEngine;
using Krista.FM.Server.Scheme.ScriptingEngine.Oralce;
using Krista.FM.Server.Scheme.ScriptingEngine.Sql;
using Krista.FM.Server.Tasks;
using Krista.FM.Server.Users;
using Krista.FM.Server.WriteBack;
using Krista.FM.Server.XmlExportImporter;

using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;
using Krista.FM.ServerLibrary.Forecast;
using Krista.FM.ServerLibrary.TemplatesService;
using Krista.FM.WriteBackLibrary;
using Krista.FM.Server.DataVersionsManager;

namespace Krista.FM.Server.Scheme
{
    internal partial class SchemeClass : SMOSerializable, IScheme
    {
    	public const string UseNullScriptingEngineImplParamName = "UseNullScriptingEngineImpl";

        #region Поля

        /// <summary>
        /// Наименование схемы.
        /// </summary>
        private string name;

        /// <summary>
        /// Ссылка на объект сервера.
        /// </summary>
        private IServer server;

        /// <summary>
        /// Путь к настройкам схемы.
        /// </summary>
        private string configFile;
        
        /// <summary>
        /// Список семантических принадлежностей.
        /// </summary>
        private SemanticsCollection semantics;
        
        /// <summary>
        /// Рассчитывалка многомерной базы.
        /// </summary>
		private IProcessor processor;

        /// <summary>
        /// Объект упраления источниками данных.
        /// </summary>
        private IDataSourceManager dataSourceManager;

        /// <summary>
        /// Объект управления версиями классификаторов
        /// </summary>
        private IDataVersionManager dataVersionManager;
        
        /// <summary>
        /// Объект упраления программами закачки.
        /// </summary>
        private IDataPumpManager dataPumpManager;
        
        /// <summary>
        /// Объект управления задачами.
        /// </summary>
        private TaskManager taskManager;
        
        /// <summary>
        /// Объект управления пользователями.
        /// </summary>
        private UsersManager usersManager;
        
        /// <summary>
        /// Сервер обратной записи
        /// </summary>
        private IWriteBackServer writeBackServer;

        /// <summary>
        /// Менеджер сессий.
        /// </summary>
        private SessionManager sessionManager;

        /// <summary>
        /// Объект доступа к константам.
        /// </summary>
        private GlobalConstsManager globalConstsManager;

        /// <summary>
        /// Менеджер сообщений.
        /// </summary>
        private IMessageManager messageManager;

        /// <summary>
        /// Менеджер очистки неактуальных сообщений
        /// </summary>
        private IMessageCleanerManager messageCleanerManager;

        /// <summary>
        /// Главный корневой пакет схемы.
        /// </summary>
        private Package rootPackage;

        /// <summary>
        /// Информация о сервере приложений.
        /// </summary>
        private readonly ServerSystemInfo serverInfo;

        /// <summary>
        /// Интерфейс правил расщепления.
        /// </summary>
        private DisintRules.DisintRules disintRules;

        /// <summary>
        /// Сборка провайдера SourceSafe.
        /// </summary>
        private Assembly vssProviderAssembly;

        /// <summary>
        /// Объект для синхронизации операций обновления схемы.
        /// </summary>
        private static readonly Mutex mutexSchemeAutoUpdate = new Mutex();

        /// <summary>
        /// Объект для синхронизации операций обновления схемы. Класс Semaphore не обеспечивает потоковой идентификации.
        /// </summary>
        private static readonly Semaphore semaphoreSchemeAutoUpdate = new Semaphore(1, 1);

        /// <summary>
        /// Признак указывающий серверу на необходимость произвести обновление схемы.
        /// </summary>
        private bool needUpdateScheme = false;

        /// <summary>
        /// Признак указывающий серверу на необходимость произвести обновление схемы.
        /// </summary>
        private bool needPostUpdateScheme = false;

        /// <summary>
        /// Фабрика создания сущностей
        /// </summary>
        private static EntityFactoryAbstract _entityFactory;

        /// <summary>
        /// Фабрика создания ассоциаций
        /// </summary>
        private static EntityAssociationFactoryAbstract _entityAssociationFactory;

        /// <summary>
        /// сервис получения списка отчетов системы в виде дерева отчетов
        /// </summary>
        private ReportsTreeService reportsTreeService;

       /// <summary>
        /// Признак указывающий серверу на необходимость произвести обновление схемы.
        /// </summary>
        internal bool NeedUpdateScheme
        {
            [DebuggerStepThrough]
            get { return needUpdateScheme; }
        }

        /// <summary>
        /// Признак указывающий серверу на необходимость произвести обновление схемы.
        /// </summary>
        internal bool NeedPostUpdateScheme
        {
            [DebuggerStepThrough]
            get { return needPostUpdateScheme; }
        }

        // Индикаторы производительности
        //private PerformanceCounters performanceCounters;

        // Глобальный объект доступа к базе данных используемый для обновления метаданных
        private Database schemeDatabase;
        private Database createDDLDatabase;

        /// <summary>
        /// Используется для доступа к схеме (метаданным) базы
        /// </summary>
        private Database SchemeDatabase
        {
            [DebuggerStepThrough]
            get
            {
                if (schemeDatabase == null)
                    schemeDatabase = new Database(((IConnectionProvider)SchemeDWH).Connection, DbProviderFactories.GetFactory(SchemeDWH.FactoryName));
                else if (schemeDatabase != null && schemeDatabase.Disposed)
                    schemeDatabase = new Database(((IConnectionProvider)SchemeDWH).Connection, DbProviderFactories.GetFactory(SchemeDWH.FactoryName));
                return schemeDatabase;
            }
        }

        internal Database DDLDatabase
        {
            [DebuggerStepThrough]
            get
            {
                if (createDDLDatabase == null)
                {
                    bool isDeveloper = false;
                    LogicalCallContextData lccd = LogicalCallContextData.GetContext();
                    try
                    {
                        // Устанавливаем свойства контекста
                        if (lccd != null)
                        {
                            isDeveloper = Convert.ToBoolean(lccd["IsDeveloper"]);
                            lccd["IsDeveloper"] = true;
                        }

                        // Создаем экземпляр объекта
                        createDDLDatabase =
                            new Database(((IConnectionProvider)SchemeDWH).Connection, DbProviderFactories.GetFactory(SchemeDWH.FactoryName));
                    }
                    finally
                    {
                        // Восстанавливаем свойства контекста
                        if (lccd != null)
                        {
                            lccd["IsDeveloper"] = isDeveloper;
                        }
                    }
                }
                return createDDLDatabase;
            }
        }

        #endregion Поля
    
        #region Базовые методы

        /// <summary>
        /// Конструктор объекта схемы
        /// </summary>
        public SchemeClass()
            : base(null)
        {
            try
            {
                serverInfo = new ServerSystemInfo();
            }
            catch (Exception e)
            {
                Trace.TraceError("Ошибка инициализации класса ServerSystemInfo: ", KristaDiagnostics.ExpandException(e));
            }
        }

        /// <summary>
        /// Экземпляр схемы.
        /// </summary>
        public static SchemeClass Instance
        {
            [DebuggerStepThrough]
            get
            {
                return (SchemeClass)Resolver.Get<IScheme>();
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (dataPumpManager != null)
                    {
                        dataPumpManager.Dispose();
                    }

                    if (sessionManager != null)
                    {
                        sessionManager.Close();
                    }

                    if (writeBackServer != null)
                    {
                        writeBackServer.Dispose();
                    }

                    if (processor != null)
                    {
                        processor.Dispose();
                    }

                    if (messageCleanerManager != null)
                    {
                        messageCleanerManager.Stop();
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
                Trace.WriteLine("Scheme disposed.");
            }
        }

        #endregion Базовые методы

        #region Вспомогательные методы

        internal static ScriptingEngineFactory ScriptingEngineFactory
        {
            [DebuggerStepThrough]
            get
            {
				if (Instance.Server.GetConfigurationParameter(UseNullScriptingEngineImplParamName) != null)
				{
					// Режим без генерации скриптов базы данных.
					return new ScriptingEngineFactory(new NullScriptingEngineImpl(Instance.SchemeDatabase));
				}

                if (Instance.SchemeDWH.FactoryName == ProviderFactoryConstants.OracleClient || 
					Instance.SchemeDWH.FactoryName == ProviderFactoryConstants.OracleDataAccess ||
					Instance.SchemeDWH.FactoryName == ProviderFactoryConstants.MSOracleDataAccess)
                    return new ScriptingEngineFactory(new OracleScriptingEngineImpl(Instance.SchemeDatabase));
                else if (Instance.SchemeDWH.FactoryName == ProviderFactoryConstants.SqlClient)
                    return new ScriptingEngineFactory(new SqlScriptingEngineImpl(Instance.SchemeDatabase));
                else
                    throw new Exception(String.Format("Поддержка провайдера {0} не реализована.", Instance.SchemeDWH.FactoryName));
            }
        }

        /// <summary>
        /// Определяет режим работы сервера с базой данных.
        /// Если с базой данных уже работает какой либо сервер приложений, то режим будет MultiServerMode.
        /// </summary>
        public bool MultiServerMode
        {
            get { return ((SchemeDWH)SchemeDWH).MultiServerMode; }
        }

        /// <summary>
        /// Фабрика создания сущностей
        /// </summary>
        internal static EntityFactoryAbstract EntityFactory
        {
            get
            {
                if (_entityFactory == null)
                    _entityFactory = new EntityFactoryBase();
                return _entityFactory;
            }
        }

        /// <summary>
        /// Фабрика создания ассоциаций
        /// </summary>
        internal static EntityAssociationFactoryAbstract EntityAssociationFactory
        {
            get
            {
                if (_entityAssociationFactory == null)
                    _entityAssociationFactory = new EntityAssociationFactoryBase();
                return _entityAssociationFactory;
            }
        }

        #endregion Вспомогательные методы

        #region Инициализация системных объектов

        /// <summary>
        /// Инициализация объекта для доступа к хранилищу данных
        /// </summary>
        /// <returns>
        /// Возвращаен истину или ложь в зависимости от результатов инициализации объекта.
        /// Подробное описание ошибок должно находися в логе инициализации схемы
        /// </returns>
        private bool InitializeSchemeDWH()
        {
            // Инициализируем объект
            return Scheme.SchemeDWH.Instance.Initialize();
        }

        internal static void InitializeNHibernate(
            string connectionString,
            string factoryName,
            string serverVersion,
            ISessionStorage sessionStorage)
        {
            NHibernateInitializer.Instance().InitializeNHibernateOnce(
                () => NHibernateSession.InitializeNHibernateSession(
                    sessionStorage,
                    connectionString,
                    factoryName,
                    serverVersion));
        }


        /// <summary>
        /// Инициализация объекта для доступа к многомерному хранилищу данных
        /// </summary>
        /// <returns>
        /// Возвращаен истину или ложь в зависимости от результатов инициализации объекта.
        /// Подробное описание ошибок должно находися в логе инициализации схемы
        /// </returns>
        private bool InitializeSchemeMDStore()
        {
            bool initializeMDStore = false;
            try
            {
                initializeMDStore = Services.OLAP.SchemeMDStore.Instance.Initialize();
            }
            catch (Exception e)
            {
                Trace.TraceWarning(e.Message);
            }

            return initializeMDStore;
        }

		private bool InitializeProcessor()
		{
			try
			{
				processor = ProcessorClass.GetInstance(this);
				return true;				
			}
			catch (Exception e)
			{
				Trace.TraceError("Ошибка инициализации менеджера расчетов многомерной базы: {0}", e.Message);
				return false;
			}
		}

        private bool InitializeMessageManager()
        {
            try
            {
                ObjectRepository objectRepository = new ObjectRepository();
                MembershipsRepository membershipsRepository = new MembershipsRepository();
                PermissionRepository permissionRepository = new PermissionRepository();
                MessageRepository messageRepository = new MessageRepository(permissionRepository, objectRepository, membershipsRepository);

                messageManager = new MessageManager(
                    (IMessageExchangeProtocol)GetProtocol("Krista.FM.Server.Scheme.dll"),
                    new NHibernateUnitOfWorkFactory(),
                    messageRepository, 
                    new NHibernateLinqRepository<Domain.Users>(), 
                    membershipsRepository, 
                    new NHibernateLinqRepository<MessageAttachment>(), 
                    permissionRepository, 
                    objectRepository);

                messageCleanerManager = new MessageCleanerManager(messageManager);

                return true;
            }
            catch(Exception e)
            {
                Trace.TraceError("Ошибка инициализации менеджера сообщений: {0}", e.Message);
                return false;
            }
        }

        /// <summary>
        /// Инициализация списка семантической принадлежности
        /// </summary>
        private void InitializeSemantics()
        {
            semantics = new SemanticsCollection(BaseDirectory + "\\" + "Semantics.xml");
        }
               
        /// <summary>
        /// Инициализация сервера обратной записи
        /// </summary>
        /// <returns></returns>
        private bool InitializeWriteBackServer()
        {
            try
            {
                this.writeBackServer = new WriteBackServerClass(this);
                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError("Ошибка инициализации сервера обратной записи: {0}", e.Message);
                return false;
            }
        }


        /// <summary>
        /// Инициализация провайдера доступа к VSS
        /// </summary>
        private void InitializeVSSProvider()
        {
            Trace.TraceInformation(String.Format("Инициализация провайдера доступа к SourceSafe"));
            Trace.Indent();
            try
            {
                string sourceSafeIniFile = Server.GetConfigurationParameter("SourceSafeIniFile");
                if (!String.IsNullOrEmpty(sourceSafeIniFile))
                {
                    Trace.WriteLine(String.Format("База SourceSafe {0}", sourceSafeIniFile));

                    string sourceSafeWorkingProject = Server.GetConfigurationParameter("SourceSafeWorkingProject");
                    if (!String.IsNullOrEmpty(sourceSafeWorkingProject))
                    {
                        Trace.WriteLine(String.Format("Рабочий каталог {0}", sourceSafeWorkingProject));
                        Trace.WriteLine(String.Format("Пользователь {0}", Server.GetConfigurationParameter("SourceSafeUser")));

                        try
                        {
                            vssProviderAssembly = AppDomain.CurrentDomain.Load("Krista.FM.Providers.VSS", AppDomain.CurrentDomain.Evidence);
                            Trace.WriteLine(VSSFacade.ToString());
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(String.Format("Ошибка инициализации провайдера: {0}", e.Message));
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                Trace.Unindent();
            }
        }

        /// <summary>
        /// Создает и возвращает фасад доступа к SourceSafe
        /// </summary>
        /// <returns>Фасад доступа к SourceSafe</returns>
        public IVSSFacade VSSFacade
        {
            get
            {
                try
                {
                    if (vssProviderAssembly != null)
                        try
                        {
                            IVSSFacade vssFacade = (IVSSFacade)vssProviderAssembly.CreateInstance("Krista.FM.Providers.VSS.VSSFacade");
                            string p1 = Server.GetConfigurationParameter("SourceSafeIniFile");
                            string p2 = Server.GetConfigurationParameter("SourceSafeUser");
                            string p3 = Server.GetConfigurationParameter("SourceSafePassword");
                            string p4 = Server.GetConfigurationParameter("SourceSafeWorkingProject");
                            vssFacade.Open(p1, p2, p3, p4, this.BaseDirectory);
                            return vssFacade;
                        }
                        catch (Exception e)
                        {
                            Trace.TraceError("Ошибка инициализации провайдера Krista.FM.Providers.VSS.VSSFacade: {0}", e.ToString());
                            return null;
                        }
                    else
                        return null;
                }
                catch (Exception e)
                {
                    Trace.TraceError("Ошибка при получении фасада доступа к SourceSafe: {0}", e.ToString());
                    return null;
                }
            }
        }

        /// <summary>
        /// Инициализация объекта схемы из XML узла описания схемы
        /// </summary>
        /// <param name="schemeFileName">XML узел "UMLScheme" описывающий схему</param>
        /// <returns>
        /// Возвращаен истину или ложь в зависимости от результатов инициализации схемы.
        /// Подробное описание ошибок должно находися в логе
        /// </returns>
        public bool Initialize(string schemeFileName)
        {
            Trace.WriteLine(String.Format("Инициализация схемы \"{0}\", конфигурационный файл {1}", this.Name, schemeFileName));
            Trace.Indent();
            try
            {
                #region Создаем сессию для системного пользователя текущей схемы

                sessionManager = Krista.FM.Server.Common.SessionManager.GetInstance(this);
                LogicalCallContextData.SetAuthorization("SYSTEM");
                ClientSession.CreateSession(SessionClientType.Server);
                Krista.FM.Server.Common.SessionManager.Instance.Create(LogicalCallContextData.GetContext());
                LogicalCallContextData.GetContext()["UserID"] = (int)FixedUsers.FixedUsersIds.System;
                SessionContext.SetSystemContext(LogicalCallContextData.GetContext());

                #endregion Создаем сессию для системного пользователя текущей схемы

                //performanceCounters = new PerformanceCounters(String.Format("{0}:{1}", portNumber, this.Name));

                // сохраняем путь к настроечному файлу
                configFile = schemeFileName;

                string errMsg;

                // для начала проверим на валидность
                XmlDocument xmlDoc = Validator.LoadValidated(schemeFileName, "ServerConfiguration.xsd", "xmluml", out errMsg);
                if (xmlDoc == null)
                {
                    Trace.TraceError("Ошибка при загрузке настроек схемы{0}{1}", Environment.NewLine, errMsg);
                    return false;
                }

                // Инициализация параметров схемы
                Trace.TraceInformation("Инициализация параметров схемы");

                InitializeVSSProvider();

                // Инициализируем хранилище данных
                Trace.TraceInformation("Инициализация хранилища данных");
                if (!InitializeSchemeDWH())
                {
                    Trace.TraceError("Can't initialize Schema SchemeDWH");
                    return false;
                }

                // Определяем необходимость выполнения обновления схемы
                using (IDatabase db = SchemeDWH.DB)
                {
                    int needUpdateValue = Convert.ToInt32(db.ExecQuery(
                        "select NeedUpdate from DatabaseVersions where ID = (select max(ID) from DatabaseVersions)",
                        QueryResultTypes.Scalar));
                    needUpdateScheme = needUpdateValue == 1;
                    needPostUpdateScheme = needUpdateScheme;
                    if (needUpdateScheme)
                        Trace.TraceWarning("Сервер запущен в режиме обновления версии.");
                }

                // Устанавливаем контекст системной сессии
                sessionManager.SetDatabaseContext(sessionManager.Sessions[SessionContext.SessionId]);

                // Проверка базы данных
                RunTestDatabase();

                // Инициализируем многомерное хранилище
                Trace.TraceInformation("Инициализация многомерного хранилища");
                if (!InitializeSchemeMDStore())
                {
                    Trace.TraceError("Can't initialize Schema SchemeMDStore");
                    return false;
                }

                // Инициализируем рассчитывалки многомерной базы
                if (SchemeClass.Instance.Server.GetConfigurationParameter("WithOutOLAPProcessror") == null)
                {
                    if (!InitializeProcessor())
                    {
                        Trace.TraceError("Can't initialize Schema Processor");
                        return false;
                    }
                }

                if (SchemeClass.Instance.Server.GetConfigurationParameter("UseReplaceIPAddressTrackingHandler") != null)
                {
                    Trace.TraceInformation("Подключение добавления в контекст вызова имени или адреса ремоутинг-сервера, видимого с клиента.");
                    ReplaceIPAddressTrackingHandler replaceIpAddressTrackingHandler = new ReplaceIPAddressTrackingHandler();
                    TrackingServices.RegisterTrackingHandler(replaceIpAddressTrackingHandler);
                }

                // Инициализация пользователей
                Trace.TraceInformation("Инициализация пользователей/прав");
                Trace.Indent();
                try
                {
                    usersManager = new UsersManager(this);
                }
                catch (Exception exp)
                {
                    Trace.TraceError("BUG: {0}", Krista.Diagnostics.KristaDiagnostics.ExpandException(exp));
                    return false;
                }
                Trace.Unindent();

                // ТУДУ: Инициализация архивных папок

                // Инициализация источников данных
                Trace.TraceInformation("Инициализация источников данных и реестра закачек");
                dataSourceManager = new DataSourceManager(this);

                // Инициализация источников данных
                Trace.TraceInformation("Инициализация версий классификаторов");
                dataVersionManager = new DataVersionManager(this);

                // Инициализация менеджера закачек
                Trace.TraceInformation("Инициализация менеджера закачек");
                dataPumpManager = new DataPumpManager(this);
                dataPumpManager.Initialize();

                // Инициализация списка семантической принадлежности
                Trace.TraceInformation("Инициализация списка\"Семантическая принадлежность\"");
                InitializeSemantics();

                // Инициализация задач
                Trace.TraceInformation("Инициализация задач");
                Trace.Indent();
                try
                {
                    taskManager = new TaskManager(this);
                }
                catch (Exception exp)
                {
                    Trace.TraceError("BUG: {0}", exp.Message);
                    return false;
                }
                Trace.Unindent();

                #region Инициализация шаблонов

#if TEMPLATES

                // Инициализация задач
                Trace.TraceInformation("Инициализация шаблонов");
                Trace.Indent();
                try
                {
                    Krista.FM.Server.TemplatesService.TemplatesService.Instance.Initialize(this);
                }
                catch (Exception exp)
                {
                    Trace.TraceError("BUG: {0}", exp.Message);
                    return false;
                }
                Trace.Unindent();

#endif

                #endregion

                Trace.TraceInformation("Инициализация констант");
                Trace.Indent();
                try
                {
                    globalConstsManager = new GlobalConstsManager(this);
                }
                catch (Exception exp)
                {
                    Trace.TraceError("BUG: {0}", exp.Message);
                    return false;
                }
                Trace.Unindent();

                Trace.TraceInformation("Инициализация сервиса отчетов системы");
                Trace.Indent();
                try
                {
                    reportsTreeService = new ReportsTreeService(this);
                }
                catch (Exception exp)
                {
                    Trace.TraceError("BUG: {0}", exp.Message);
                    return false;
                }
                Trace.Unindent();

                // Инициализация объектов схемы по метаданным из базы данных
                Trace.TraceInformation("Инициализация объектов схемы");
                Trace.Indent();
                if (!InitializeObjects())
                {
                    Trace.TraceError("Scheme's objects initializing failed. Detailed infofmation see in the log file.");
                }
                Trace.Unindent();

                Trace.TraceInformation("Формирование коллекций");
                InitializeObjectsCollections();

                //SchemeClass.Instance.ConvertDiagrams(SchemeClass.Instance.RootPackage);

                PostInitialize();

                //CheckIncexes();

                InitializeWriteBackServer();

                Trace.TraceInformation("Инициализация менеджера сообщений");
                if (!InitializeMessageManager())
                {
                    Trace.TraceError("Can't initialize Schema MessageManager");
                    return false;
                }

                //StartPlaningProvider();

                // Запуск шедулера.
				// Если с базой данных уже работает какой либо сервер приложений, 
				// то диспетчер выполнения закачек по расписанию не запускаем.
				if (!MultiServerMode)
					dataPumpManager.StartScheduler();
            }
            catch (Exception e)
            {
                Trace.TraceError("Необработанная ошибка во время инициализации схемы: {0}", e.ToString());
                return false;
            }
            finally
            {
                Trace.Unindent();
            }
            Trace.TraceInformation("Инициализация схемы \"{0}\" завершена.", this.Name);
            return true;
        }

        internal void ConvertDiagrams(IPackage package)
        {
            IDatabase db = Scheme.SchemeDWH.Instance.DB;
            try
            {
                foreach (IPackage item in package.Packages.Values)
                {
                    ConvertDiagrams(item);

                    foreach (IDocument document in item.Documents.Values)
                    {
                        ((Document) document).ConvertDiagramToGuidKeys(db);
                    }
                }
            }
            finally
            {
                db.Dispose();
            }
        }

        private void PostInitialize()
        {
            #region Утилитариум

            //Classifiers["d.KD.Analysis"].ReverseRowsRange(new int[] { 4189 });

            /*
            foreach (IPackage item in Packages.Values)
                ((Package)item).StoreInVSS();
            */
            #endregion Утилитариум

            //BackgroundInitializeTasks.PostInitializeSchemeObjects();
            BackgroundInitializeTasks.PostInitializeSchemeObjectsEntryPoint();
        }

        internal void RunTestDatabase()
        {
            // Проверка базы данных
            Trace.TraceInformation("Проверка базы данных");
            Trace.Indent();
			try
			{
				if (SchemeDWH.FactoryName != ProviderFactoryConstants.OracleClient)
					return;

				if (SchemeDWH.FactoryName != ProviderFactoryConstants.OracleDataAccess)
					return;

				if (SchemeDWH.FactoryName != ProviderFactoryConstants.MSOracleDataAccess)
					return;

				using (IDatabase db = SchemeDWH.DB)
				{
					OracleScriptingEngineImpl.TestDatabase(db);
				}
			}
			catch (Exception exp)
			{
				Trace.TraceError("BUG: {0}", KristaDiagnostics.ExpandException(exp));
			}
			finally
			{
				Trace.Unindent();
			}
        }

        #endregion Инициализация системных объектов

        #region Реализация интерфейса IScheme

        /// <summary>
        /// Наименование схемы
        /// </summary>
        public string Name
        {
            [DebuggerStepThrough]
            get { return name; }
            set
            {
                if (!String.IsNullOrEmpty(name))
                    throw new Exception("Значение свойства Name уже установлено.");
                name = value;
            }
        }

        /// <summary>
        /// Главный корневой пакет схемы.
        /// </summary>
        public IPackage RootPackage
        {
            [DebuggerStepThrough]
            get { return rootPackage; }
        }

        /// <summary>
        /// Возвращает базовый каталог схемы.
        /// </summary>
        public string BaseDirectory
        {
            get
            {
                Uri uri = new Uri(configFile);
                string dir = Path.GetDirectoryName(uri.LocalPath);
                return dir.Substring(0, dir.IndexOf("Packages") - 1);
            }
        }

        /// <summary>
        /// Возвращает ссылку на объект хранилища данных схемы.
        /// </summary>
        public ISchemeDWH SchemeDWH
        {
            [DebuggerStepThrough]
            get { return Scheme.SchemeDWH.Instance; }
        }

        /// <summary>
        /// Возвращает ссылку на объект многомерной базы данных схемы
        /// </summary>
        public ISchemeMDStore SchemeMDStore
        {
            [DebuggerStepThrough]
            get { return Services.OLAP.SchemeMDStore.Instance; }
        }

        public ITemplatesService TemplatesService
        {
            [DebuggerStepThrough]
            get
            {
                return Resolver.Get<ITemplatesService>();
            }
        }

		/// <summary>
		/// Интерфейс на рассчитывалку многомерной базы
		/// </summary>
		public IProcessor Processor
		{
            [DebuggerStepThrough]
            get { return processor; }
		}

        /// <summary>
        /// Возвращает ссылку на объект, управляющий источниками данных схемы
        /// </summary>
        public IDataSourceManager DataSourceManager
        {
            [DebuggerStepThrough]
            get { return dataSourceManager; }
        }

        /// <summary>
        /// Возвращает ссылку на объект, управляющий версиями классифмкаторов
        /// </summary>
        public IDataVersionManager DataVersionsManager
        {
            [DebuggerStepThrough]
            get { return dataVersionManager; }
        }

        /// <summary>
        /// Возвращает ссылку на объект, управляющий программами закачки
        /// </summary>
        public IDataPumpManager DataPumpManager
        {
            [DebuggerStepThrough]
            get { return dataPumpManager; }
        }

        /// <summary>
        /// Коллекция семантических принадлежностей
        /// </summary>
        public ISemanticsCollection Semantics
        {
            [DebuggerStepThrough]
            get { return semantics; }
            set { semantics = (SemanticsCollection)value; }
        }
                
        public IGlobalConstsManager GlobalConstsManager
        {
            [DebuggerStepThrough]
            get { return globalConstsManager; }
        }

        /// <summary>
        /// Менеджер сообщений.
        /// </summary>
        public IMessageManager MessageManager
        {
            get { return messageManager; }
        }

        public string Connect()
        {
            LogicalCallContextData userContext = LogicalCallContextData.GetContext();
            try
            {
                SessionContext.SetSystemContext();
                Trace.TraceVerbose("sessionManager.Create(userContext)");
                return sessionManager.Create(userContext);
            }
            finally
            {
                LogicalCallContextData.SetContext(userContext);
                Trace.TraceInformation(String.Format("Клиент {0} подключился к схеме {1} SessionID = {2}", Authentication.UserDate, Name, userContext["SessionID"]));
            }
        }

        public void Disconnect()
        {
            LogicalCallContextData userContext = LogicalCallContextData.GetContext();

            try
            {
                SessionContext.SetSystemContext();
                sessionManager.Close(userContext);
                Trace.TraceInformation(String.Format("Клиент {0} отключился от схемы {1}", userContext.Principal.Identity.Name + " " + DateTime.Now, Name));
            }
            finally
            {
                LogicalCallContextData.SetContext(userContext);
            }
        }

        /// <summary>
        /// Путь к расшаренному архивному каталогу  
        /// </summary>
        public string ArchiveDirectory
        {
            [DebuggerStepThrough]
            get { return this.Server.GetConfigurationParameter("ArchiveShare"); }
        }

        /// <summary>
        /// Путь к расшаренному каталогу источников данных 
        /// </summary>
        public string DataSourceDirectory
        {
            [DebuggerStepThrough]
            get { return this.Server.GetConfigurationParameter("DataSourcesShare"); }
        }

        public IBaseProtocol GetProtocol(string CallerAssemblyName)
        {
            return new Logger.Logger(this, CallerAssemblyName);
        }

        public IDataOperations GetAudit()
        {
            return new DataOperations(this);
        }

        public IExportImportManager GetXmlExportImportManager()
        {
            return new ExportImportManager(this);
        }

        public IConversionTableCollection ConversionTables
        {
            [DebuggerStepThrough]
            get { return ConversionTableCollection.Instance; }
        }

        /// <summary>
        /// Интерфейс на объект обратной записи
        /// </summary>
        public IWriteBackServer WriteBackServer
        {
            [DebuggerStepThrough]
            get { return writeBackServer; }
        }

        /// <summary>
        /// Возвращает ссылку на объект управляющий архивными папками схемы
        /// </summary>
        public IArchive Archive
        {
            get { return null; }
        }

        /// <summary>
        /// Возвращает ссылку на объект управляющий источниками данных схемы
        /// </summary>
        public ITaskManager TaskManager
        {
            [DebuggerStepThrough]
            get { return taskManager; }
        }

        /// <summary>
        /// Возвращает ссылку на объект управления пользователями/правами
        /// </summary>
        public IUsersManager UsersManager
        {
            [DebuggerStepThrough]
            get { return usersManager; }
        }

        public IPlaningProvider PlaningProvider
        {
            [DebuggerStepThrough]
            get
            {
                foreach (IDisposable resource in SessionContext.SessionResources)
                {
                    if (resource is PlaningProviderWrapper)
                        return (PlaningProviderWrapper)resource;
                }

                PlaningProviderWrapper ppw = new PlaningProviderWrapper(this);
                SessionContext.RegisterObject(ppw);
                return ppw;

            }
        }

        public IReportsTreeService ReportsTreeService
        {
            get { return reportsTreeService; }
        }

        /// <summary>
        /// Объект сервера приложения
        /// </summary>
        public IServer Server
        {
            [DebuggerStepThrough]
            get { return server; }
            set
            {
                if (!Authentication.IsSystemRole())
                    throw new Exception("Недостаточно привелегий для выполнения операции.");

                server = value;
            }
        }

        /// <summary>
        /// Интерфейс правил расщепления
        /// </summary>
        public IDisintRules DisintRules
        {
            get 
            {
                if (disintRules == null)
                {
                    disintRules = new DisintRules.DisintRules(this);
                }
                return disintRules; 
            }
        }

        /// <summary>
        /// Фасад системы планирования источников финансирования.
        /// </summary>
        public IFinSourcePlanningFace FinSourcePlanningFace
        {
            get
            {
                if (!Krista.FM.Server.FinSourcePlanning.FinSourcePlanningFace.Instance.Initialized)
                {
                    Krista.FM.Server.FinSourcePlanning.FinSourcePlanningFace.Instance.Initialize(this);
                }
                return Krista.FM.Server.FinSourcePlanning.FinSourcePlanningFace.Instance;
            }
        }

		public IForecastService ForecastService
		{
			get
			{
				if (!Krista.FM.Server.Forecast.ForecastService.Instance.Initialized)
				{
					Krista.FM.Server.Forecast.ForecastService.Instance.Initialize(this);
				}
				return Krista.FM.Server.Forecast.ForecastService.Instance;
			}
		}

		public IForm2pService Form2pService
		{
			get
			{
				if (!Krista.FM.Server.Forecast.Form2pService.Instance.Initialized)
				{
					Krista.FM.Server.Forecast.Form2pService.Instance.Initialize(this);
				}
				return Krista.FM.Server.Forecast.Form2pService.Instance;
			}
		}

        /// <summary>
        /// Менеджер сессий
        /// </summary>
        public ISessionManager SessionManager 
        {
            get { return sessionManager; } 
        }

        public DataTable ServerSystemInfo
        {
            get { return serverInfo.GetInfo(); }
        }

        /// <summary>
        /// Объект для синхронизации операций обновления схемы.
        /// </summary>
        internal static Mutex MutexSchemeAutoUpdate
        {
            get { return mutexSchemeAutoUpdate; }
        }

        /// <summary>
        /// Объект для синхронизации операций обновления схемы. Класс Semaphore не обеспечивает потоковой идентификации.
        /// </summary>
        internal static Semaphore SemaphoreSchemeAutoUpdate
        {
            get { return semaphoreSchemeAutoUpdate; }
        }

        /// <summary>
        /// Создает контекст для применения изменений к схеме.
        /// </summary>
        /// <returns>Контекст в котором выполняются изменения структуры схемы.</returns>
        public IModificationContext CreateModificationContext()
        {
            return new ModificationContext(new Database(((IConnectionProvider)SchemeDWH).Connection, DbProviderFactories.GetFactory(SchemeDWH.FactoryName)));
        }

        #endregion Реализация интерфейса IScheme

        #region IServiceProvider Members

        /// <summary>
        /// Возвращает запрашиваемый сервис
        /// </summary>
        /// <param name="serviceType">Тип сервиса</param>
        /// <returns>Запрашиваемый сервис</returns>
        public object GetService(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            if (serviceType == typeof(IScheme))
                return this;

            if (serviceType == typeof(ICommentsCheckService))
                return this.CommentsCheckServiceInstance;

            throw new ArgumentException(String.Format("Сервис типа {0} не поддерживается.", serviceType));
        }

        #endregion

        #region IScheme Members
        
        /// <summary>
        /// Для документации
        /// </summary>
        /// <exception cref="Krista.FM.Common.Exceptions.HelpException"></exception>
        public string ConfigurationXMLDocumentation
        {
            get
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.InnerXml = "<?xml version=\"1.0\" encoding=\"windows-1251\"?><ServerConfiguration/>";

                    XmlNode element = doc.CreateElement("Package");
                    doc.DocumentElement.AppendChild(element);

                    this.rootPackage.Save2XmlDocumentation(element);
                    // собираем сюда источники и виды поступающей информации
                    XmlNode dataSuppliers = doc.CreateElement("Suppliers");

                    XmlDocument docSupplier = new XmlDocument();
                    docSupplier.Load(Instance.BaseDirectory + "//DataSources.xml");

                    XmlNode suppiersNode = docSupplier.SelectSingleNode("//Suppliers");
                    dataSuppliers.InnerXml = suppiersNode.InnerXml;

                    doc.DocumentElement.AppendChild(dataSuppliers);

                    // собираем сюда семантики
                    XmlNode semanticsData = doc.CreateElement("Semantics");

                    XmlDocument docSemantic = new XmlDocument();
                    docSemantic.Load(Instance.BaseDirectory + "//Semantics.xml");

                    XmlNode semanticNode = docSemantic.SelectSingleNode("//Semantics");
                    semanticsData.InnerXml = semanticNode.InnerXml;

                    doc.DocumentElement.AppendChild(semanticsData);

                    // Проверка на валидность
                    Validator.LoadDocument(doc.InnerXml);

                    return doc.InnerXml; 
                }
                catch (XmlException ex)
                {
                    throw new HelpException(ex.ToString());
                }
                catch (ArgumentException ex)
                {
                    throw new HelpException(ex.ToString());
                }
                catch(InvalidOperationException ex)
                {
                    throw new HelpException(ex.ToString());
                }
                catch(XPathException ex)
                {
                    throw new HelpException(ex.ToString());
                }
            }
        }

        #endregion
    }
}
