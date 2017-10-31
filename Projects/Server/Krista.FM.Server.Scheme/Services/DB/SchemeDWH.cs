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
	/// ������ ����������� ������ � ��������� ������.
	/// </summary>
	internal class SchemeDWH : DisposableObject, ISchemeDWH, IConnectionProvider
	{
		// ������ �����������
        private ConnectionString connectionString;

        /// <summary>
        /// ��� ������� ������� ���������� ������
        /// </summary>
		private string providerFactoryName;

        // ������ ���� ������
        private string databaseVersion;

        // ������ �������
        private string serverVersion;

        /// <summary>
        /// ����� ������ �������.
        /// </summary>
        private bool multiServerMode;

		/// ����������� ������� ������� � ��������� ������.
		/// </summary>
		private SchemeDWH()
		{
		}

        /// <summary>
        /// ��������� ������.
        /// </summary>
        public static SchemeDWH Instance
        {
            [DebuggerStepThrough]
            get { return Singleton<SchemeDWH>.Instance; }
        }

        /// <summary>
        /// �� ������ ����������� ���������� ��� ���������� � ���������� ��� ������� ������� �������� ������� � ������.
        /// </summary>
        /// <returns>��� ������� ������� �������� ������� � ������.</returns>
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
                    throw new Exception(String.Format("��������� ���������� {0} �� �����������.", connectionString.Provider));
            }
        }

        /// <summary>
        /// ����������� �����������
        /// </summary>
        /// <param name="connectionString">������ �����������</param>
        /// <returns>true - ������� �������������; false - ������</returns>
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

                    Trace.TraceInformation(String.Format("��� ����������: {0}", providerFactoryName));
                    Trace.TraceInformation(String.Format("������: {0}", serverVersion));

                    // ���������� ������ ���� ������
                    cmd = connection.CreateCommand();
                    cmd.CommandText = "select Name from DatabaseVersions where ID = (select max(ID) from DatabaseVersions)";
                    databaseVersion = Convert.ToString(cmd.ExecuteScalar());

                    Trace.TraceInformation(String.Format("������ ���� ������: {0}", databaseVersion));

                    // �������� ������ ��
                    if (!CheckDatabaseVersion(databaseVersion))
                    {
                        throw new Exception(String.Format("������ ���� ������ {0} �� ������������� ������� ������ ������� {1}", databaseVersion, AppVersionControl.GetAssemblyBaseVersion(AppVersionControl.GetServerLibraryVersion())));
                    }

                    multiServerMode = DetectMultiServerMode();
                    if (multiServerMode)
                    {
                        Trace.TraceError("������ �������� � ������ \"MultiServerMode\", �.�. � ����� ������ ��� �������� ������ ������. ��������� ����������� ����� ����������.");
                    }

                    SchemeClass.InitializeNHibernate(connectionString.ToString(), providerFactoryName, serverVersion, new PersistenceSessionStorage());

                    return true;
                }
                catch (Exception e)
                {
                    Trace.TraceError("���������� ������������ � ���� ������: {0}", e.Message);
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
        /// ��������� ������������� �������
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            connectionString = new ConnectionString();
            connectionString.ReadConnectionString(SchemeClass.Instance.BaseDirectory + "\\" + "DWH.UDL");
            return InitializeConnection(connectionString);
        }

        /// <summary>
        /// ��� ������� �������� ������� � ������
        /// </summary>
        public string FactoryName
        {
            [DebuggerStepThrough]
            get { return providerFactoryName; }
        }

        /// <summary>
        /// ������� ������� ������� �������� ������� � ������
        /// </summary>
        internal DbProviderFactory GetFactory
        {
            get 
            {
                return new DbProviderFactoryWrapper(DbProviderFactories.GetFactory(FactoryName)); 
            }
        }

        /// <summary>
        /// ����� ������ �������.
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
        /// ������ ����������� � ���� ������
        /// </summary>
		public string ConnectionString
		{
			get 
            { 
                // TODO ���������� ������
                return connectionString.ToString(); 
            }
		}

        /// <summary>
        /// ���������� ����� ������ ��� ������� � ���� ������.
        /// ����� ������������� ���������� ������� Dispose
        /// </summary>
        public IDatabase DB
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                Database db = new Database(SessionContext.Connection, GetFactory);
                // ��������� ������� �� ���������� �������� ���� �� ��� ������ � ������ �����������
                if (connectionString.QueryTimeout != null && connectionString.QueryTimeout > 0)
                    db.CommandTimeout = (int)connectionString.QueryTimeout;
                return db;//new Database(SessionContext.Connection, GetFactory);
            }
        }

        /// <summary>
        /// ������� ��������� ������� UnitOfWork.
        /// ����� ������������� ���������� ������� Dispose.
        /// </summary>
	    public IUnitOfWork CreateUnitOfWork()
	    {
	        var uow = new NHibernateUnitOfWork(NHibernateSession.Current, SessionContext.Session);
            SessionContext.Session.UnitOfWork = uow;
            return uow;
	    }

	    /// <summary>
        /// ������ ���� ������ � ������� XX.XX.XX.XX
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
        /// ��� ����������� ���� ������
        /// </summary>
        public string DataBaseName
        {
            get { return (connectionString.InitialCatalog == String.Empty) ? connectionString.DataSource : connectionString.InitialCatalog; }
        }

        /// <summary>
        /// ������ ����������� � ��������� �� UDL 
        /// </summary>
	    public string OriginalConnectionString
	    {
            get { return connectionString.OriginalString; }
	    }

        /// <summary>
        /// ��� �������
        /// </summary>
	    public string ServerName
	    {
            get { return connectionString.DataSource; }
	    }

	    /// <summary>
        /// ���������� ����� ������ ������� � ����� ������.
        /// ���� � ����� ������ ��� �������� ����� ���� ������ ����������, �� ����� ����� MultiServerMode.
        /// </summary>
        public bool DetectMultiServerMode()
        {
            return SchemeClass.ScriptingEngineFactory.NullScriptingEngine.DetectMultiServerMode();
        }

        #endregion
    }
}
