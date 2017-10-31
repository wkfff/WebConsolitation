using System;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using System.Data.SqlClient;
using Krista.FM.Providers.OracleDataAccess;
using Krista.FM.Providers.MSOracleDataAccess;

namespace Krista.FM.Server.Common
{
    /// <summary>
    /// Оболочка для коннекта
    /// </summary>
    public class DbConnectionWrapper : DbConnection
    {
        /// <summary>
        /// Внутренний коннект
        /// </summary>
        private DbConnection connection;
		private bool available;


        public DbConnectionWrapper(DbConnection connection)
        {
            if (connection is DbConnectionWrapper)
				throw new ArgumentException("Объект подключения к базе не может иметь тип DbConnectionWrapper", "connection");

            this.connection = connection;
        }

		/// <summary>
		/// Определяет доступно ли подключение для повторного извлечения из пула.
		/// </summary>
		internal bool Available
		{
			get { return available; }
			set { available = value; }
		}

        /// <summary>
        /// Устанавливает контекст сессии
        /// </summary>
        private void SetDatabaseContext()
        {
            // TODO СУБД зависимый код
			if (connection is OracleConnection || 
				connection is Krista.FM.Providers.MSOracleDataAccess.OracleDbConnection ||
				connection is Krista.FM.Providers.OracleDataAccess.OracleDbConnection)
            {
                IDbCommand cmd = connection.CreateCommand();
                cmd.CommandText = "DBMS_SESSION.SET_IDENTIFIER";
                cmd.CommandType = CommandType.StoredProcedure;
                IDbDataParameter prm = cmd.CreateParameter();
                prm.ParameterName = "CLIENT_ID";
                prm.DbType = DbType.String;
                prm.Value = SessionContext.Session.SessionId;
                cmd.Parameters.Add(prm);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            else if (connection is SqlConnection)
            {
                IDbCommand cmd = connection.CreateCommand();
                cmd.CommandText = String.Format(
                    "DECLARE @BinVar varbinary(128)\n" +
                    "SET @BinVar = " +
                    "CAST('{0}' as varbinary(1)) + " +
                    "CAST((CAST('{1}' AS char(24))) AS varbinary(24) ) + " +
                    "CAST((CAST('{2}' AS char(64))) AS varbinary(64))\n" +
                    "SET CONTEXT_INFO @BinVar",
                    SessionContext.IsDeveloper ? "1" : "0",
                    SessionContext.Session.SessionId,
                    SessionContext.UserName);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                /*
                cmd = connection.CreateCommand();
                cmd.CommandText = String.Format(
                    "select 1, " +
                    "cast(left(cast(CONTEXT_INFO() as varbinary(1)), 1) as int) as IsDevelop, " +
                    "rtrim(cast(substring(cast(CONTEXT_INFO() as varbinary(128)), 2, 24) as varchar(24))) as SessionID, " +
                    "rtrim(cast(substring(cast(CONTEXT_INFO() as varbinary(128)), 26, 64) as varchar(64))) as UserName");
                cmd.CommandType = CommandType.Text;
                IDataReader dr = cmd.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.BeginLoadData();
                dataTable.Load(dr, LoadOption.OverwriteChanges);
                dataTable.EndLoadData();
                dataTable.RemotingFormat = SerializationFormat.Binary;
                dr.Close();*/
            }
            else
                throw new NotImplementedException();
        }

        #region DbConnection Members

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return connection.BeginTransaction(isolationLevel);
        }

        public override void ChangeDatabase(string databaseName)
        {
            connection.ChangeDatabase(databaseName);
        }

        public override void Close()
        {
            connection.Close();
        	available = true;
        }

        public override string ConnectionString
        {
            get
            {
                return connection.ConnectionString;
            }
            set
            {
                connection.ConnectionString = value;
            }
        }

        public override int ConnectionTimeout
        {
            get { return connection.ConnectionTimeout; }
        }

        protected override DbCommand CreateDbCommand()
        {
            return connection.CreateCommand();
        }

        public override string DataSource
        {
            get { return connection.DataSource; }
        }

        public override string Database
        {
            get { return connection.Database; }
        }

        public override void Open()
        {
			available = false;
			connection.Open();
            SetDatabaseContext();
        }

        public override string ServerVersion
        {
            get { return connection.ServerVersion; }
        }

        public override ConnectionState State
        {
            get { return connection.State; }
        }

    	#endregion
    }
}
