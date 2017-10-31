using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Krista.FM.Providers.OracleDataAccess
{
    public class OracleDbCommand : DbCommand, IDbCommand
    {
        private OracleCommand command = null;

        public OracleDbCommand()
        {
            this.command = new OracleCommand();
        }

        public OracleDbCommand(OracleCommand command)
        {
            System.Diagnostics.Trace.Assert(command != null, "Parameter command is null");
            this.command = command;
        }

        public OracleDbCommand(string cmdText)
        {
            command = new OracleCommand(cmdText);
        }

        public OracleDbCommand(string cmdText, DbConnection conn)
        {
            command = new OracleCommand(cmdText, ((OracleDbConnection)conn).OracleConn);
        }

        public override void Cancel()
        {
            command.Cancel();
        }

        public override string CommandText
        {
            get
            {
                return command.CommandText;
            }
            set
            {
                command.CommandText = value;
            }
        }

        public override int CommandTimeout
        {
            get
            {
                return command.CommandTimeout;
            }
            set
            {
                command.CommandTimeout = value;
            }
        }

        public override CommandType CommandType
        {
            get
            {
                return command.CommandType;
            }
            set
            {
                command.CommandType = value;
            }
        }

        protected override DbConnection DbConnection
        {
            get
            {
                OracleDbConnection conn = new OracleDbConnection(command.Connection);
                return conn;
            }
            set
            {
                command.Connection = ((OracleDbConnection)value).OracleConn;
            }
        }

        protected override DbParameterCollection DbParameterCollection
        {
            get 
            {
                OracleDbParametersCollection collection = new OracleDbParametersCollection(command.Parameters);
                return collection; 
            }
        }

        protected override DbTransaction DbTransaction
        {
            get
            {
                return new OracleDbTransaction(this.command.Transaction);
            }
            set
            {
                /*OracleDbTransaction trans = value as OracleDbTransaction;
                if (trans != null)
                {
                    //this.command. = trans.OracleTransaction;
                }*/
            }
        }

        public override bool DesignTimeVisible
        {
            get
            {
                return false;
            }
            set
            {
                
            }
        }

        protected override DbParameter CreateDbParameter()
        {
            OracleParameter param = command.CreateParameter();
            return new OracleDbParameter(param);
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get
            {
                return command.UpdatedRowSource;
            }
            set
            {
                command.UpdatedRowSource = value;
            }
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            OracleDbDataReader reader = new OracleDbDataReader(command.ExecuteReader(behavior));
            return reader;
        }

        public override object ExecuteScalar()
        {
            return command.ExecuteScalar();
        }

        public override int ExecuteNonQuery()
        {
            return command.ExecuteNonQuery();
        }

        public override void Prepare()
        {
            command.Prepare();
        }

        protected override void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing)
                {
                    if (command != null)
                        command.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
