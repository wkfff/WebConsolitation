using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Data.OracleClient;

namespace Krista.FM.Providers.MSOracleDataAccess
{
    public class OracleDbConnection : DbConnection
    {
        private OracleConnection connection = null;

        public OracleDbConnection()
        {
            connection = new OracleConnection();
        }

        public OracleDbConnection(string connectionString)
        {
            connection = new OracleConnection(connectionString);
        }

        public OracleDbConnection(OracleConnection conn)
        {
            this.connection = conn;
        }

        #region основные методы класса DbConnection

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            OracleDbTransaction trans = new OracleDbTransaction(connection.BeginTransaction(IsolationLevel.ReadCommitted));
            return trans;
        }

        public override void ChangeDatabase(string databaseName)
        {
            throw new Exception("The method or operation is not implemented.");
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

        public override string Database
        {
            get { return connection.DataSource; }
        }

        public override string DataSource
        {
            get { return connection.DataSource; }
        }

        public override void Close()
        {
            connection.Close();
        }

        public override string ServerVersion
        {
            get { return connection.ServerVersion; }
        }

        public override System.ComponentModel.ISite Site
        {
            get
            {
                return connection.Site;
            }
            set
            {
                connection.Site = value;
            }
        }

        protected override DbCommand CreateDbCommand()
        {
            OracleCommand command = connection.CreateCommand();
            return new OracleDbCommand(command);
        }

        public override void Open()
        {
            lock (this)
            {
                if (this.ConnectionString == null || this.ConnectionString.Length == 0)
                {
                    throw new InvalidOperationException("Connection String is not initialized.");
                }
                /*
                if (!this.connection.State ==  && this.State != ConnectionState.Connecting)
                {
                    throw new InvalidOperationException("Connection already Open.");
                }
                */
                try
                {
                    this.OnStateChange(new StateChangeEventArgs(this.State, ConnectionState.Connecting));
                    // Update the connection state
                    this.OnStateChange(new StateChangeEventArgs(this.State, ConnectionState.Open));
                }
                catch (Exception)
                {
                    this.OnStateChange(new StateChangeEventArgs(this.State, ConnectionState.Closed));
                    throw;
                }
            }

            connection.Open();
        }

        public override ConnectionState State
        {
            get { return connection.State; }
        }

        public override int ConnectionTimeout
        {
            get
            {
                return connection.ConnectionTimeout;
            }
        }

        public override string ToString()
        {
            return connection.ToString();
        }

        public override bool Equals(object obj)
        {
            return connection.Equals(obj);
        }

        protected override void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing)
                {
                    if (this.connection != null)
                        if (this.connection.State != ConnectionState.Closed)
                            connection.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        public OracleConnection OracleConn
        {
            get { return connection; }
        }

        #endregion
    }
}
