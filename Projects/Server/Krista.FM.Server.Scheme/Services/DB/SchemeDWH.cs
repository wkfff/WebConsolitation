using System;
using System.Diagnostics;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

using Krista.FM.Common;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme
{
	/// <summary>
	/// Объект реализующий доступ к хранилищу данных.
	/// </summary>
	internal class SchemeDWH : DisposableObject, ISchemeDWH, IConnectionProvider
	{
		// Строка подключения
        private ConnectionString connectionString;

        /// <summary>
        /// Имя фабрики классов источников данных
        /// </summary>
		private string providerFactoryName;

        // Версия базы данных
        private string databaseVersion;

        // версия сервера
        private string serverVersion;

        /// <summary>
        /// Режим работы сервера.
        /// </summary>
        private bool multiServerMode;

		/// Конструктор объекта доступа к хранилищу данных.
		/// </summary>
		private SchemeDWH()
		{
		}

        /// <summary>
        /// Экземпляр класса.
        /// </summary>
        public static SchemeDWH Instance
        {
            [DebuggerStepThrough]
            get { return Singleton<SchemeDWH>.Instance; }
        }

        /// <summary>
        /// По строке подключения определяет тип провайдера и возвращает имя фабрики классов объектов доступа к данных.
        /// </summary>
        /// <returns>Имя фабрики классов объектов доступа к данных.</returns>
        private string GetProviderFactoryNameFromConfiguration()
        {
            switch (connectionString.Provider)
            {
                case "OraOLEDB.Oracle":
                case "OraOLEDB.Oracle.1":
                case "OraOLEDB.Oracle.2":
                    //return "System.Data.OracleClient";
                    //return "Krista.FM.Providers.OracleDataAccess";
					//return "Krista.FM.Providers.MSOracleDataAccess";
                    return ProviderFactoryConstants.MSOracleDataAccess;
				case "SQLOLEDB":
                case "SQLOLEDB.1":
                case "SQLOLEDB.2":
                case "SQLNCLI":
                case "SQLNCLI.1":
                case "SQLNCLI10.1":
                    //return "System.Data.SqlClient";
                    return ProviderFactoryConstants.SqlClient;
                default:
                    throw new Exception(String.Format("Поддержка провайдера {0} не реализована.", connectionString.Provider));
            }
        }

        /// <summary>
        /// Иниализация подключения
        /// </summary>
        /// <param name="connectionString">Строка подключения</param>
        /// <returns>true - удачная инициализация; false - ошибка</returns>
        private bool InitializeConnection(ConnectionString connectionString)
        {
            providerFactoryName = GetProviderFactoryNameFromConfiguration();
            DbConnection connection = GetFactory.CreateConnection();
            connection.ConnectionString = connectionString.ToString();
            try
            {
                Trace.Indent();
                IDbCommand cmd = null;
                try
                {
                    connection.Open();

                    ServerVersion = connection.ServerVersion;

                    Trace.TraceInformation(String.Format("Тип провайдера: {0}", providerFactoryName));
                    Trace.TraceInformation(String.Format("Сервер: {0}", serverVersion));

                    // Определяем версию базы данных
                    cmd = connection.CreateCommand();
                    cmd.CommandText = "select Name from DatabaseVersions where ID = (select max(ID) from DatabaseVersions)";
                    databaseVersion = Convert.ToString(cmd.ExecuteScalar());

                    Trace.TraceInformation(String.Format("Версия базы данных: {0}", databaseVersion));

                    // Проверка версии БД
                    if (!CheckDatabaseVersion(databaseVersion))
                    {
                        throw new Exception(String.Format("Версия базы данных {0} не соответствует базовой версии сервера {1}", databaseVersion, AppVersionControl.GetAssemblyBaseVersion(AppVersionControl.GetServerLibraryVersion())));
                    }

                    multiServerMode = DetectMultiServerMode();
                    if (multiServerMode)
                    {
                        Trace.TraceError("Служба запущена в режиме \"MultiServerMode\", т.к. с базой данных уже работают другие службы. Некоторые возможности будут недоступны.");
                    }

                    SchemeClass.InitializeNHibernate(connectionString.ToString(), providerFactoryName, serverVersion, new PersistenceSessionStorage());

                    return true;
                }
                catch (Exception e)
                {
                    Trace.TraceError("Невозможно подключиться к базе данных: {0}", e.Message);
                    throw new Exception(e.Message, e);
                }
                finally
                {
                    if (cmd != null)
                        cmd.Dispose();

                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            finally
            {
                Trace.Unindent();
            }
        }

	    private static bool CheckDatabaseVersion(string version)
	    {
            string serverLibraryVersion = AppVersionControl.GetServerLibraryVersion();

            string baseVesion = AppVersionControl.GetAssemblyBaseVersion(serverLibraryVersion);

            if (String.Compare(AppVersionControl.GetAssemblyBaseVersion(version), baseVesion) != 0)
            {
                return false;
            }
	        return true;
	    }

	    /// <summary>
        /// Процедура инициализации объекта
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            connectionString = new ConnectionString();
            connectionString.ReadConnectionString(SchemeClass.Instance.BaseDirectory + "\\" + "DWH.UDL");
            return InitializeConnection(connectionString);
        }

        /// <summary>
        /// Имя фабрики объектов доступа к данным
        /// </summary>
        public string FactoryName
        {
            [DebuggerStepThrough]
            get { return providerFactoryName; }
        }

        /// <summary>
        /// Создает фабрику классов объектов доступа к данным
        /// </summary>
        internal DbProviderFactory GetFactory
        {
            get 
            {
                return new DbProviderFactoryWrapper(DbProviderFactories.GetFactory(FactoryName)); 
            }
        }

        /// <summary>
        /// Режим работы сервера.
        /// </summary>
        public bool MultiServerMode
        {
            get { return multiServerMode; }
        }

        #region ISchemeDWH Members

        /// <summary>
        /// 
        /// </summary>
		public IDbConnection Connection
		{
            get 
            { 
                DbConnection conn = GetFactory.CreateConnection();
                conn.ConnectionString = connectionString.ToString();
                return conn; 
            }
		}

        /// <summary>
        /// Строка подключения к базе данных
        /// </summary>
		public string ConnectionString
		{
			get 
            { 
                // TODO Ограничить доступ
                return connectionString.ToString(); 
            }
		}

        /// <summary>
        /// Возвращает новый объект для доступа к базе данных.
        /// После использования необходимо вызвать Dispose
        /// </summary>
        public IDatabase DB
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                Database db = new Database(SessionContext.Connection, GetFactory);
                // добавляем таймаут на выполнение запросов если он был указан в строке подключения
                if (connectionString.QueryTimeout != null && connectionString.QueryTimeout > 0)
                    db.CommandTimeout = (int)connectionString.QueryTimeout;
                return db;//new Database(SessionContext.Connection, GetFactory);
            }
        }

        /// <summary>
        /// Создает экземпляр объекта UnitOfWork.
        /// После использования необходимо вызвать Dispose.
        /// </summary>
	    public IUnitOfWork CreateUnitOfWork()
	    {
	        var uow = new NHibernateUnitOfWork(NHibernateSession.Current, SessionContext.Session);
            SessionContext.Session.UnitOfWork = uow;
            return uow;
	    }

	    /// <summary>
        /// Версия базы данных в формате XX.XX.XX.XX
        /// </summary>
        public string DatabaseVersion
        {
            get { return databaseVersion; }
        }

        public string ServerVersion
        {
            get { return serverVersion; }
            set { serverVersion = value; }
        }

        /// <summary>
        /// Имя реляционной базы данных
        /// </summary>
        public string DataBaseName
        {
            get { return (connectionString.InitialCatalog == String.Empty) ? connectionString.DataSource : connectionString.InitialCatalog; }
        }

        /// <summary>
        /// Строка подключения к хранилищу из UDL 
        /// </summary>
	    public string OriginalConnectionString
	    {
            get { return connectionString.OriginalString; }
	    }

        /// <summary>
        /// Имя сервера
        /// </summary>
	    public string ServerName
	    {
            get { return connectionString.DataSource; }
	    }

	    /// <summary>
        /// Определяет режим работы сервера с базой данных.
        /// Если с базой данных уже работает какой либо сервер приложений, то режим будет MultiServerMode.
        /// </summary>
        public bool DetectMultiServerMode()
        {
            return SchemeClass.ScriptingEngineFactory.NullScriptingEngine.DetectMultiServerMode();
        }

        #endregion
    }
}
