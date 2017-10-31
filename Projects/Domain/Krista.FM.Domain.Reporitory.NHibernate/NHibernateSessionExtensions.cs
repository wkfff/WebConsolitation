using System;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using NHibernate;

namespace Krista.FM.Domain.Reporitory.NHibernate
{
    public static class NHibernateSessionExtensions
    {
        /// <summary>
        /// Устанавливает контекст сессии
        /// </summary>
        public static void SetDatabaseContext(this ISession session, string sessionId,  string userName)
        {
            var connection = session.Connection;

            // TODO СУБД зависимый код
            if (connection is OracleConnection ||
                connection.GetType().Name.StartsWith("Krista.FM.Providers.MSOracleDataAccess.OracleDbConnection") ||
                connection.GetType().Name.StartsWith("Krista.FM.Providers.OracleDataAccess.OracleDbConnection"))
            {
                IDbCommand cmd = connection.CreateCommand();
                cmd.CommandText = "DBMS_SESSION.SET_IDENTIFIER";
                cmd.CommandType = CommandType.StoredProcedure;
                IDbDataParameter prm = cmd.CreateParameter();
                prm.ParameterName = "CLIENT_ID";
                prm.DbType = DbType.String;
                prm.Value = sessionId;
                cmd.Parameters.Add(prm);
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                SetDatabaseContextParameter(connection, "SESSIONID", sessionId, sessionId);
                SetDatabaseContextParameter(connection, "USERNAME", userName, sessionId);
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
                    /*SessionContext.IsDeveloper*/false ? "1" : "0",
                    sessionId,
                    userName);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            else
                throw new NotImplementedException();
        }

        private static void SetDatabaseContextParameter(IDbConnection connection, string parameterName, string parameterValue, string sessionId)
        {
            using (IDbCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "DVContext.SetValue";
                cmd.CommandType = CommandType.StoredProcedure;

                IDbDataParameter prm = cmd.CreateParameter();
                prm.ParameterName = "attribute";
                prm.DbType = DbType.String;
                prm.Value = parameterName;
                cmd.Parameters.Add(prm);

                prm = cmd.CreateParameter();
                prm.ParameterName = "value";
                prm.DbType = DbType.String;
                prm.Value = parameterValue;
                cmd.Parameters.Add(prm);

                prm = cmd.CreateParameter();
                prm.ParameterName = "username";
                prm.DbType = DbType.String;
                prm.Value = "DV";
                cmd.Parameters.Add(prm);

                prm = cmd.CreateParameter();
                prm.ParameterName = "client_id";
                prm.DbType = DbType.String;
                prm.Value = sessionId;
                cmd.Parameters.Add(prm);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
