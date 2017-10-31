using System;
using System.Collections.Generic;
using Krista.FM.Common;


namespace Krista.FM.Server.Scheme.Services.OLAP
{
    internal class OlapDatabase2000 : OlapDatabase
    {
        // ������ AS2000.
        private DSO.ServerClass server;

        // ����������� ���� AS2000.
        private DSO.Database database;


        /// <summary>
        /// ������������� ���������� �������.
        /// </summary>
        /// <param name="connectionString">������ �����������.</param>
        public OlapDatabase2000(OlapConnectionString connectionString)
            : base(connectionString)
        {
            server = new DSO.ServerClass();
            server.Connect(connectionString.DataSource);
            WriteServerInfo(server.Name, server.Version, server.RepositoryVersion.ToString());
            database = (DSO.Database)server.MDStores.Item(connectionString.InitialCatalog);
            if (database == null)
            {
                throw new Exception(String.Format("�� ������� ������������ � ����������� ����. ���� {0} �� �������.", connectionString.InitialCatalog));
            }
            if(!InitializeDatabaseVersion())
            {
                Trace.Write(String.Format("������ ����������� ���� {0} �� ������������� ������� ������ �������", databaseVersion));
                throw new Exception(String.Format("������ ����������� ���� {0} �� ������������� ������� ������ ������� {1}", databaseVersion, AppVersionControl.GetAssemblyBaseVersion(AppVersionControl.GetServerLibraryVersion())));
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
            if (server.State != DSO.ServerStates.stateConnected)
            {
                server.Connect(connectionString.DataSource);
            }
            if (server.State == DSO.ServerStates.stateFailed)
            {
                throw new Exception("�� ������� ������������ � Analasys Service 2000.");
            }
            database = (DSO.Database)server.MDStores.Item(connectionString.InitialCatalog);
        }

        public override void CloseConnection()
        {
            database = null;
            if (server != null)
            {
                server.CloseServer();
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
                    DSO.OlapStateTypes state = database.State;
                }
            }
            catch (System.Runtime.InteropServices.COMException)
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
            server = null;
        }

        public override string OlapDataSourceName
        {
            get
            {
                ConnectionString connectionToDWH = new ConnectionString();
                connectionToDWH.Parse(((DSO.DataSource)database.DataSources.Item("DV")).ConnectionString);
                return connectionToDWH.DataSource;
            }
        }

        public override string OlapDataSourceServer
        {
            get
            {
                return String.Empty;
            }
        }

        public override string ServerVersion
        {
            get { return server.Version; }
        }

        /// <summary>
        /// ����������� ������ ����������� ����
        /// </summary>
        protected override bool InitializeDatabaseVersion()
        {
            if (database.CustomProperties["������"] == null)
                return false;

            if (string.IsNullOrEmpty(database.CustomProperties["������"].Value.ToString()))
                return false;
            
            string serverLibraryVersion = AppVersionControl.GetServerLibraryVersion();

            string baseVesion = AppVersionControl.GetAssemblyBaseVersion(serverLibraryVersion);
            string olapDatabaseVersion = database.CustomProperties["������"].Value.ToString();
            databaseVersion = olapDatabaseVersion;

            if (String.Compare(AppVersionControl.GetAssemblyBaseVersion(olapDatabaseVersion), baseVesion) == 0)
            {
                Trace.Write(String.Format("������ ����������� ����: {0}", databaseVersion));
                return true;
            }

            return NeedCheckDatabasVersion();                                  
        }
    }
}
