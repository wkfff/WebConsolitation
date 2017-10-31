using System;
using System.Collections.Generic;
using Krista.FM.Common;


namespace Krista.FM.Server.Scheme.Services.OLAP
{
    internal class OlapDatabase2005 : OlapDatabase
    {
        // Сервер SSAS2005.
        private Microsoft.AnalysisServices.Server server;

        // Многомерная база SSAS2005.
        private Microsoft.AnalysisServices.Database database;

        
        /// <summary>
        /// Инициализация экземпляра объекта.
        /// </summary>
        /// <param name="connectionString">Строка подключения.</param>
        public OlapDatabase2005(OlapConnectionString connectionString)
            : base (connectionString)
        {
            server = new Microsoft.AnalysisServices.Server();
            server.Connect(connectionString.DataSource);
            WriteServerInfo(server.Name, server.Version, server.Edition.ToString());
            database = server.Databases[connectionString.InitialCatalog];
            if (database == null)
            {
                throw new Exception(String.Format("Не удалось подключиться к многомерной базе. База {0} не найдена.", connectionString.InitialCatalog));
            }
            if (!InitializeDatabaseVersion())
            {
                Trace.TraceError("Версия многомерной базы {0} не соответствует базовой версии сервера",
                                  databaseVersion);
                throw new Exception(
                    String.Format("Версия многомерной базы {0} не соответствует базовой версии сервера {1}",
                                  databaseVersion,
                                  AppVersionControl.GetAssemblyBaseVersion(
                                      AppVersionControl.GetServerLibraryVersion())));
            }
        }

        public override object ServerObject
        {
            get { return server; }
        }

        public override object DatabaseObject
        {
            get
            {
                CheckConnection();
                return database;
            }
        }

        public override void Connect()
        {
            if (!server.Connected)
            {
                server.Connect(connectionString.DataSource);
            }
            database = server.Databases[connectionString.InitialCatalog];
        }

        public override void CloseConnection()
        {
            if (database != null)
            {
                database.Dispose();
                database = null;
            }
            if (server != null && server.Connected)
            {
                server.Disconnect(true);
            }
        }

        public override void CheckConnection()
        {
            bool needReconnect = false;
            try
            {
                if (database == null)
                {
                    needReconnect = true;
                }
                else
                {
                    Microsoft.AnalysisServices.AnalysisState state = database.State;
                }
            }
            catch (Exception)
            {
                needReconnect = true;
            }

            if (needReconnect)
            {
                CloseConnection();
                Connect();
            }
        }

        public override void Dispose()
        {
            CloseConnection();

            if (server != null)
            {
                server.Dispose();
            }
        }

        public override string OlapDataSourceName
        {
            get
            {
                ConnectionString connectionToDWH = new ConnectionString();
                connectionToDWH.Parse(database.DataSources["DV"].ConnectionString);
                return String.IsNullOrEmpty(connectionToDWH.InitialCatalog) ? connectionToDWH.DataSource : connectionToDWH.InitialCatalog;
            }
        }

        public override string OlapDataSourceServer
        {
            get
            {
                ConnectionString connectionToDWH = new ConnectionString();
                connectionToDWH.Parse(database.DataSources["DV"].ConnectionString);
                return String.IsNullOrEmpty(connectionToDWH.InitialCatalog) ? String.Empty : connectionToDWH.DataSource;
            }
        }

        public override string ServerVersion
        {
            get { return server.Version; }
        }

        protected override bool InitializeDatabaseVersion()
        {
            if (database.Annotations["Версия"] == null)
                return false;

            if (string.IsNullOrEmpty(database.Annotations["Версия"].Value.InnerText))
                return false;

            string serverLibraryVersion = AppVersionControl.GetServerLibraryVersion();

            string baseVesion = AppVersionControl.GetAssemblyBaseVersion(serverLibraryVersion);
            string olapDatabaseVersion = database.Annotations["Версия"].Value.InnerText;
            databaseVersion = olapDatabaseVersion;

            if (String.Compare(AppVersionControl.GetAssemblyBaseVersion(olapDatabaseVersion), baseVesion) == 0)
            {
                Trace.Write(String.Format("Версия многомерной базы: {0}", databaseVersion));
                return true;
            }

            return NeedCheckDatabasVersion();
        }
    }
}
