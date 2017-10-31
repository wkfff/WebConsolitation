using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using Krista.Diagnostics;

using Krista.FM.Common;
using Krista.FM.Common.Exceptions.DbExceptions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Common
{
    /// <summary>
    /// ���������� ������
    /// </summary>
    internal enum ProviderTypes
    {
        SqlClient,
        OracleClient,
        Odbc,
        OleDb,
        OracleNETProvider,
        FirebirdNETProvider
    }

    internal struct stat
    {
        internal int Created;
        internal int Disposed;
    }

    /// <summary>
    /// ������ ����������� ������������� ������ � ������.
    /// </summary>
    public sealed class Database : DisposableObject, IDatabase
    {
        #region ����

        public static string KeyWord_OnErrorContinue = "##OnErrorContinue##";

        static stat _stat;
        static object g_identifier = 0;

        private DbProviderFactory factory;

        private int identifier = -1;
        private bool disposeResources = true;

        //static PerformanceCounter counter;

        private IDbConnection connection;
        private IDbTransaction activeTransaction;

        private bool sessionContext;

        private int commandTimeout = -1;

        private IDbExceptionService _dbExceptionService;

        private bool useBlob = false;

        #endregion ����


        static Database()
        {
            _stat.Created = 0;
            _stat.Disposed = 0;

            //counter = PerformanceCounters.CreateGlobalPerformanceCounter("IDatabase");
        }

        /// <summary>
        /// ����������� ������� ������� � ������.
        /// </summary>
        /// <param name="factory">������� �������� ������� � ���� ������</param>
        public Database(DbProviderFactory factory)
        {
            this.factory = factory;

            switch (ProviderType)
            {
                case ProviderTypes.OracleClient:
                    _dbExceptionService = new OracleDbExceptionService();
                    break;

                case ProviderTypes.SqlClient:
                    _dbExceptionService = new SQLServerDbExceptionService();
                    break;

                default:
                    _dbExceptionService = new NullDbExceptionService();
                    break;
            }

        }

        /// <summary>
        /// ����������� ������� ������� � ������.
        /// </summary>
        /// <param name="factory">������� �������� ������� � ���� ������</param>
        /// <param name="connection">����������� � ���� ������</param>
        public Database(DbProviderFactory factory, IDbConnection connection)
            : this(factory)
        {
            this.connection = connection;
            if (this.connection.State != ConnectionState.Open)
                this.connection.Open();
        }

        /// <summary>
        /// ����������� ������� ������� � ������.
        /// </summary>
        /// <param name="connection">����������� � ���� ������</param>
        /// <param name="factory">������� �������� ������� � ���� ������</param>
        /// <param name="sessionContext"></param>
        public Database(IDbConnection connection, DbProviderFactory factory, bool sessionContext)
            : this(factory, connection)
        {
            this.sessionContext = sessionContext;

            lock (g_identifier)
            {
                g_identifier = identifier = Convert.ToInt32(g_identifier) + 1;
                //counter.Increment();

                if (this.sessionContext) SessionContext.RegisterObject(this);
            }
            _stat.Created++;

            if (ShowTrace)
            {
                Trace.TraceVerbose("������ Database identifier = {0}, UserName = {1}", identifier, SessionContext.UserName);
            }
        }

        /// <summary>
        /// ����������� ������� ������� � ������.
        /// </summary>
        /// <param name="connection">����������� � ���� ������</param>
        /// <param name="factory">������� �������� ������� � ���� ������</param>
        /// <param name="sessionContext"></param>
        /// <param name="cmdTimeout">������� ������������ ���������</param>
        public Database(IDbConnection connection, DbProviderFactory factory, bool sessionContext, int cmdTimeout)
            : this(connection, factory, sessionContext)
        {
            if (cmdTimeout > 0)
                this.commandTimeout = cmdTimeout;
        }

        /// <summary>
        /// ����������� ������� ������� � ������.
        /// </summary>
        /// <param name="connection">����������� � ���� ������</param>
        /// <param name="factory">��� ���������� ������</param>
        public Database(IDbConnection connection, DbProviderFactory factory)
            : this(connection, factory, true)
        {
        }

        /// <summary>
        /// ����������� ������� ������� � ������.
        /// </summary>
        /// <param name="connection">����������� � ���� ������</param>
        /// <param name="transaction">����������</param>
        /// <param name="factory"></param>
        public Database(IDbConnection connection, IDbTransaction transaction, DbProviderFactory factory)
            : this(connection, factory, true)
        {
            this.activeTransaction = transaction;
            disposeResources = false;
        }

        public override string ToString()
        {
            return String.Format("identifier = {0} : {1}", identifier, base.ToString());
        }

        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            //Debug.WriteLine(String.Format("~~~{0}({1} {2}) Created {3} Disposed {4} Delta {5} {6} {7}", GetType().FullName, disposing, identifier, _stat.Created, _stat.Disposed, _stat.Created - _stat.Disposed, Authentication.User, System.DateTime.Now));
            // �������������� ������ ��� ������� �������������� ������ Dispose/Close
            lock (this)
            {
                if (disposing && disposeResources)
                {
                    // ����� ��� ����� ���������� � �����, ����������� 
                    // �� ������ ������� - ��� ��������� ��� ����, ��� ��� ���
                    // ���� �������� ����� Finalize ��� �� ������
                    if (ShowTrace)
                    {
                        Trace.TraceVerbose("����������� Database.Dispose identifier = {0}, UserName = {1}", identifier, SessionContext.UserName);
                    }
                    
                    if (activeTransaction != null)
                    {
                        activeTransaction.Dispose();
                        activeTransaction = null;
                    }

                    if (this.connection != null)
                    {
                        if (this.connection.State != ConnectionState.Closed)
                            this.connection.Close();
                        this.connection = null;
                    }
                    //counter.Decrement();

                    if (this.sessionContext) SessionContext.UnregisterObject(this);
                }
                else
                {
                    if (ShowTrace)
                    {
                        Trace.TraceVerbose("����������� Database.Finalize identifier = {0}, UserName = {1}", identifier, SessionContext.UserName);
                    }
                    _stat.Disposed++;
                }

                // ����� ������ ����������� �����������/�������� ������������� ��������
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// ������� �������� ������� � ���� ������.
        /// </summary>
        internal DbProviderFactory Factory
        {
            get { return factory; }
            set { factory = value; }
        }

        /// <summary>
        /// ����������� ������������ �������� ��� ������� � ����
        /// </summary>
        public IDbConnection Connection
        {
            get { return connection; }
        }

        internal IDbExceptionService DbExceptionService
        {
            get { return _dbExceptionService; }
        }


        /// <summary>
        /// ��� �����������
        /// </summary>
        internal ProviderTypes ProviderType
        {
            get
            {
                string typeName = factory.GetType().Name;
                switch (typeName)
                {
                    case "OracleClientFactory":
                    case "OracleDataAccessFactory":
                        return ProviderTypes.OracleClient;
                    case "SqlClientFactory":
                        return ProviderTypes.SqlClient;
                    case "OdbcFactory":
                        return ProviderTypes.Odbc;
                    case "OleDbFactory":
                        return ProviderTypes.OleDb;
                    case "DbProviderFactoryWrapper":
                        return ((DbProviderFactoryWrapper)factory).ProviderType;
                    default:
                        throw new Exception(String.Format("��� ���������� {0} �� ��������������.", factory.GetType().Name));
                }
            }
        }

        /// <summary>
        /// �������� ����������
        /// </summary>
        public IDbTransaction Transaction
        {
            get { return activeTransaction; }
        }

        /// <summary>
        /// ������� ���� �����������
        /// </summary>
        public int CommandTimeout
        {
            get
            {
                return commandTimeout;
            }
            set
            {
                commandTimeout = value;
            }
        }

        /// <summary>
        /// ������������ � �������� ���� ���� BLOB (�� ��������� false)
        /// </summary>
        public bool UseBlob
        {
            get
            {
                return useBlob;
            }
            set
            {
                useBlob = value;
            }
        }

        /// <summary>
        /// ������ ����������
        /// </summary>
        public void BeginTransaction()
        {
            BeginTransaction(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// ������ ����������
        /// </summary>
        /// <param name="il">������� ��������</param>
        public void BeginTransaction(IsolationLevel il)
        {
            if (connection.State == ConnectionState.Closed)
                throw new InvalidOperationException("������� BeginTransaction ��������� �������� � ��������� ����������.");
            if (activeTransaction != null)
                throw new InvalidOperationException("���������� ��� �������. ������������ ���������� �� ��������������.");
            activeTransaction = this.connection.BeginTransaction(il);
        }

        /// <summary>
        /// ����� ���������
        /// </summary>
        public void Rollback()
        {
            if (activeTransaction == null)
                throw new InvalidOperationException("���������� ��� ��������� ��� �� ���� �������.");
            activeTransaction.Rollback();
            activeTransaction.Dispose();
            activeTransaction = null;
        }

        /// <summary>
        /// �������� ���������
        /// </summary>
        public void Commit()
        {
            if (activeTransaction == null)
                throw new InvalidOperationException("���������� ��� ��������� ��� �� ���� �������.");
            activeTransaction.Commit();
            activeTransaction.Dispose();
            activeTransaction = null;
        }

        public string GetQuery(string query, IDataParameterCollection parameterCollection)
        {
            // ���� ��� ���������� == OleDb - ���������� ����� ������� � ���������� ����
            if (ProviderType == ProviderTypes.OleDb)
                return query;
			
            // ����� - ��������� ��������������
			string[] splitted = query.Split('?');
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < splitted.Length; i++)
			{
				sb.Append(splitted[i]);
				if (i < splitted.Length - 1)
				{
					IDataParameter prm = ((IDataParameter)parameterCollection[i]);
					// ��� Firebird ����������� ����� ������ ��������� ������ "@"
                    if (ProviderType == ProviderTypes.SqlClient /*|| providerType == ProviderTypes.FirebirdNETProvider*/)
					{
						prm.ParameterName = '@' + prm.ParameterName;
						sb.Append(prm.ParameterName);
					}
					// ��� ��������� - ":"
					else { sb.AppendFormat(":{0}", prm.ParameterName); }
				}
			}
			return sb.ToString();
        }

        private IDbCommand CheckParamsAndCreateCommand(String queryText, out IDbTransaction trans, params IDbDataParameter[] parameters)
        {
            if (queryText == null || queryText.Length == 0)
                throw new InvalidOperationException("�� ������ ����� �������.");

            if (connection == null)
                throw new InvalidOperationException("Database.Connection == null");

            if (connection.State == ConnectionState.Connecting ||
                connection.State == ConnectionState.Executing ||
                connection.State == ConnectionState.Fetching)
                throw new InvalidOperationException("��� ���������� �������� ������������� �������, ������� ������� ������ ���� �������.");

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            if (activeTransaction != null)
                trans = activeTransaction;
            else
                trans = connection.BeginTransaction();

            IDbCommand cmd = InitCommand(trans);

            if (parameters != null)
            {
                foreach (IDbDataParameter prm in parameters)
                    cmd.Parameters.Add(prm);
            }

            if (cmd.Parameters.Count == 0)
                cmd.CommandText = queryText;
            else
                cmd.CommandText = GetQuery(queryText, cmd.Parameters);
            if (cmd.CommandTimeout < 200)
                cmd.CommandTimeout = 200;

            return cmd;
        }

        private const string SELECT_EXPRESSION_CONDITIONAL = "SELECT ";
        private const string ORACLE_SELECT_RECCOUNT_CONSTR = " (RowNum <= {0}) and ";
        private const string MSSQL_SELECT_RECCOUNT_CONSTR = " TOP({0}) ";
        private const string WHERE_EXPRESSION_CONDITIONAL = " WHERE ";

        private const string EXCEPTION_WHERE_SECTION_REQUIRED =
            "��� ���������� ������� � ������������ ����������� ������� ��������� '{0}' ������ ��������� ���� �� ���� ������� WHERE";

        private string AppendMaxRecordCountConstraintIntoSelectQuery(string selectQuery, int maxRecordCountInSelect)
        {
            if (!selectQuery.ToUpper().StartsWith(SELECT_EXPRESSION_CONDITIONAL))
                throw new Exception(String.Format("��������� '{0}' �� �������� �������� ���� SELECT", selectQuery));

            switch (ProviderType)
            {
                // ��� ORACLE ����������� � WHERE ������ �������� ����������� �� RowNum
                case ProviderTypes.OracleClient:
                    int whereInd = selectQuery.ToUpper().IndexOf(WHERE_EXPRESSION_CONDITIONAL);
                    // ������� ������� ���� �� ������ ������� WHERE
                    if (whereInd < 0)
                        throw new Exception(String.Format(EXCEPTION_WHERE_SECTION_REQUIRED, selectQuery));
                    selectQuery = selectQuery.Insert(whereInd + WHERE_EXPRESSION_CONDITIONAL.Length,
                        String.Format(ORACLE_SELECT_RECCOUNT_CONSTR, maxRecordCountInSelect));
                    break;
                // ��� MSSQL - ����������� ����������� TOP ����� SELECT
                case ProviderTypes.SqlClient:
                    selectQuery = selectQuery.Insert(SELECT_EXPRESSION_CONDITIONAL.Length,
                        String.Format(MSSQL_SELECT_RECCOUNT_CONSTR, maxRecordCountInSelect));
                    break;
                default:
                    throw new Exception(String.Format("��� ���������� {0} �� ��������������.", ProviderType));
            }

            return selectQuery;
        }

        private DataTable InternalSelectData(IDbCommand cmd, int? maxRecordCountInResult)
        {
            // ���������� ����������� ���-�� ������� (���� �������)
            if (maxRecordCountInResult != null)
            {
                cmd.CommandText = AppendMaxRecordCountConstraintIntoSelectQuery(
                    cmd.CommandText.Trim(),
                    (int)maxRecordCountInResult);
            }
            using (IDataReader dr = cmd.ExecuteReader())
            {
                DataTable dataTable = new DataTable();
                dataTable.BeginLoadData();
                dataTable.Load(dr, LoadOption.OverwriteChanges);
                dataTable.EndLoadData();
                dataTable.RemotingFormat = SerializationFormat.Binary;
                return dataTable;
            }
        }

        public DataTable SelectData(String queryText, int? maxRecordCountInResult, params IDbDataParameter[] parameters)
        {
            return (DataTable)ExecQuery(queryText, QueryResultTypes.DataTable, parameters);
            /* ����� ������������ ����!!!
            DataTable result = null;
            IDbCommand cmd = null;
            IDbTransaction txn = null;
            try
            {
                cmd = CheckParamsAndCreateCommand(queryText, out txn, parameters);

                result = InternalSelectData(cmd, maxRecordCountInResult);

                if ((activeTransaction == null)&&(txn != null))
                    txn.Commit();

                if (this.sessionContext) 
                    SessionContext.PostAction(cmd.CommandText);

                return result;
            }
            catch (Exception e)
            {
                if ((activeTransaction == null)&&(txn != null))
                    txn.Rollback();
                throw new InvalidOperationException(String.Format("������ \"{0}\" ������ ������ {1}", queryText, e.ToString()));
            }
            finally
            {
                if ((activeTransaction == null)&&(txn != null))
                {
                    cmd.Transaction = null;
                    txn.Dispose();
                }
                if (cmd != null) 
                    cmd.Dispose();
            }*/
        }

        /// <summary>
        /// ����� ��������� ������������� ������ ���������� �������� � ���� ������
        /// </summary>
        /// <param name="queryText">����� ������� � ���� ������</param>
        /// <param name="parameters">������ ���������� �������</param>
        /// <param name="queryResultType">��� ������������� ����������</param>
        /// <returns>��� ������������� ���������� ������� �� ��������� queryResultType</returns>
        /// <remarks>
        /// ������ ������������ ��������� ����� ��� ����������: TYPE, SIZE, COMMENT � ������ ����������������� �������������� ������
        /// </remarks>
        [Obsolete("������ �����, ����� ������ � ��������� �������. ������ ���� ������� ������������ ExecQuery(String queryText, QueryResultTypes queryResultType, params IDbDataParameter[] parameters)")]
        public Object ExecQuery(String queryText, IDbDataParameter[] parameters, QueryResultTypes queryResultType)
        {
            return ExecQuery(queryText, queryResultType, parameters);
        }

        /// <summary>
        /// ����� ��������� ������������� ������ ���������� �������� � ���� ������
        /// </summary>
        /// <param name="queryText">����� ������� � ���� ������</param>
        /// <param name="queryResultType">��� ������������� ����������</param>
        /// <param name="parameters">����� ���������� �������</param>
        /// <returns>��� ������������� ���������� ������� �� ��������� queryResultType</returns>
        /// <remarks>
        /// ������ ������������ ��������� ����� ��� ����������: TYPE, SIZE, COMMENT � ������ ����������������� �������������� ������
        /// </remarks>
        /// <example>
        ///    DataTable dataTable = (DataTable)DB.ExecQuery(
        ///        "select ID, Name from MetaBridgeCls where ID = :ID", 
        ///        QueryResultTypes.DataTable,
        ///        DB.CreateParameter("ID", DbType.Int32, (Object)55));
        ///    foreach (DataRow row in dataTable.Rows)
        ///    {
        ///        foreach (DataColumn col in dataTable.Columns)
        ///        {
        ///            Console.Write(row[col]);
        ///        }
        ///        Console.WriteLine();
        ///    }
        /// </example>
        public Object ExecQuery(String queryText, QueryResultTypes queryResultType, params IDbDataParameter[] parameters)
        {
            return ExecQuery(queryText, queryResultType, null, parameters);
        }

        public Object ExecQuery(String queryText, QueryResultTypes queryResultType, int? maxRecordCountInResult, params IDbDataParameter[] parameters)
        {
            Object resultObject = null;
            IDbCommand cmd = null;
            IDbTransaction txn = null;
            IDbDataParameter[] newparameters = GetServerDbParams(parameters);
            try
            {
                cmd = CheckParamsAndCreateCommand(queryText, out txn, newparameters);
                switch (queryResultType)
                {
                    case QueryResultTypes.NonQuery:
                        resultObject = cmd.ExecuteNonQuery();
                        break;
                    case QueryResultTypes.Scalar:
                        resultObject = cmd.ExecuteScalar();
                        break;
                    case QueryResultTypes.DataSet:
                        IDbDataAdapter da = GetDataAdapter();
                        da.SelectCommand = cmd;
                        DataSet dataSet = new DataSet();
                        da.Fill(dataSet);
                        resultObject = dataSet;
                        break;
                    case QueryResultTypes.DataTable:
                        resultObject = InternalSelectData(cmd, maxRecordCountInResult);
                        break;
                    case QueryResultTypes.StoredProcedure:
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                        // ���������� ��������� ���������� �������� ���������
                        //foreach (IDbDataParameter parameter in newparameters)
                        for (int i = 0; i < newparameters.GetLength(0); i++)
                        {
                            if (newparameters[i].Direction == ParameterDirection.ReturnValue)
                                resultObject = newparameters[i].Value;
                            else if (newparameters[i].Direction == ParameterDirection.Output 
                                      || newparameters[i].Direction == ParameterDirection.InputOutput)
                            {
                                parameters[i].Value = newparameters[i].Value;
                            }
                        }

                        break;
                }
                if (activeTransaction == null)
                    if (txn != null)
                        txn.Commit();

                if (this.sessionContext) SessionContext.PostAction(cmd.CommandText);

                return resultObject;
            }
            catch (Exception e)
            {
                if (activeTransaction == null)
                    if (txn != null && txn.Connection != null)
                        txn.Rollback();

                /*
                  throw new InvalidOperationException(String.Format(
                    "������ \"{0}\" ������ ������: {1}.\n{2}",
                    queryText, e.Message, ConvertParametersValuesToString(parameters)), e);
                 */
                
                InvalidOperationException invalidOperationException = new InvalidOperationException(
                    String.Format("������ \"{0}\" ������ ������: {1}.\n{2}", queryText, e.Message,
                                  ConvertParametersValuesToString(parameters)), e);

                //�������� ���������� ��� ������, ��������� � ��
                var dbException = DbExceptionService.GetDbException(invalidOperationException);
                if (dbException != null)
                {
                    throw dbException;
                }
                else
                {
                    throw invalidOperationException;
                }
            }
            finally
            {
                if (activeTransaction == null)
                {
                    if (txn != null) // ����� ���������
                    {
                        if (cmd != null) cmd.Transaction = null;
                        txn.Dispose();
                    }
                }
                if (cmd != null) // ��� �� �������
                    cmd.Dispose();
            }

        }

        /// <summary>
        /// ����������� �������� ���������� ������� � ������ ��� ������� � ��������� �� ������.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static string ConvertParametersValuesToString(IDbDataParameter[] parameters)
        {
            if (parameters != null && parameters.GetLength(0) > 0)
            {
                try
                {
                    StringBuilder sb = new StringBuilder("�������� ���������� �������:\n");
                    foreach (IDbDataParameter item in parameters)
                    {
                        sb.AppendFormat("{0} [{1}]: ", item.ParameterName, item.DbType);
                        string value = Convert.ToString(item.Value);
                        sb.Append(value.Length <= 50 ? value : value.Substring(0, 50));
                        sb.AppendLine();
                    }
                    return sb.ToString();
                }
                catch
                {
                    return String.Empty;
                }
            }
            else
                return String.Empty;
        }

        /// <summary>
        /// ���������� ������������������ SQL-������ �� ���� ������
        /// </summary>
        /// <param name="script">������ SQL-������</param>
        /// <param name="onErrorContinue">
        /// false - ��� ������������� ������ ��������� ���������� � ���������� ����������
        /// true - ��� ������������� ������ ���������� ����������</param>
        /// <returns>��� ���������� ������� ��� ������ ���������� true, ����� false</returns>
        public bool RunScript(string[] script, bool onErrorContinue)
        {
            bool result = true;

            foreach (string query in script)
            {
                bool onLineErrorContinue = false;
                try
                {
                    if (!String.IsNullOrEmpty(query))
                    {
                        if (query.Length > KeyWord_OnErrorContinue.Length)
                            onLineErrorContinue = query.Substring(0, KeyWord_OnErrorContinue.Length) == KeyWord_OnErrorContinue;
                        string queryText = onLineErrorContinue ? 
                            query.Substring(KeyWord_OnErrorContinue.Length, query.Length - KeyWord_OnErrorContinue.Length)
                            : query;
                        KristaDiagnostics.TraceEvent(TraceEventType.Verbose, "Krista.FM.Server.Common.Database.RunScript", query);
                        ExecQuery(queryText, QueryResultTypes.NonQuery);
                    }
                }
                catch (Exception e)
                {
                    if (onErrorContinue || onLineErrorContinue)
                    {
                        KristaDiagnostics.TraceEvent(TraceEventType.Warning, "Krista.FM.Server.Common.Database.RunScript", e.Message);
                        result = false;
                    }
                    else
                        //� ����� ����� ��������� ����������? �� ������ ���������� - ������ �� �����.
                        //throw new Exception(e.Message, e);
                        throw;
                }
            }
            return result;
        }

        /// <summary>
        /// ���������� ������������������ SQL-������ �� ���� ������
        /// </summary>
        /// <param name="script">������ SQL-������</param>
        public bool RunScript(string[] script)
        {
            return RunScript(script, false);
        }

        /// <summary>
        /// ���������� ��������� �������� ���������� �� ���� ������
        /// </summary>
        /// <param name="generatorName">��� ����������</param>
        /// <returns>��������� �������� ����������</returns>
        public int GetGenerator(string generatorName)
        {
            // TODO ���� ��������� ���
            if (ProviderType == ProviderTypes.SqlClient)
            {
                if (generatorName.StartsWith("g_", StringComparison.OrdinalIgnoreCase))
                    generatorName = generatorName.Substring(2, generatorName.Length - 2);

                IDbDataParameter returnParam = CreateParameter("return", null, DbType.Int32, ParameterDirection.ReturnValue);
                object returnValue = ExecQuery("usp_Generator", QueryResultTypes.StoredProcedure, CreateParameter("generator", generatorName, DbType.String, ParameterDirection.Input), returnParam);
                return Convert.ToInt32(returnValue);
            }

            /*else if (providerType == ProviderTypes.FirebirdNETProvider)
                return Convert.ToInt32(ExecQuery("select gen_id(" + generatorName + ", 1) from rdb$database", QueryResultTypes.Scalar));*/
            if (ProviderType == ProviderTypes.OracleClient)
                return Convert.ToInt32(ExecQuery("select " + generatorName + ".NextVal from Dual", QueryResultTypes.Scalar));

            throw new Exception(String.Format("��� ���������� {0} �� ��������������.", ProviderType));
        }

        /// <summary>
        /// ������������� �������� ����������
        /// </summary>
        /// <param name="generatorName">��� ����������</param>
        /// <param name="value">�������� ����������</param>
        public void SetGenerator(string generatorName, int value)
        {
            if (ProviderType == ProviderTypes.SqlClient)
            {
                // TODO ��������� �������� ����������
                generatorName = "g." + generatorName;
                string setGenQuery = string.Format(@" set identity_insert {0} on
                    insert into {0} (id) values (?)
                    delete from {0} where id = ?
                    set identity_insert {0} off", generatorName);
                ExecQuery(setGenQuery, QueryResultTypes.NonQuery, CreateParameter("p0", value), CreateParameter("p1", value));

            }
            /*else if (providerType == ProviderTypes.FirebirdNETProvider)
                ExecQuery("select gen_id(" + generatorName + ", 0) from rdb$database", QueryResultTypes.Scalar);*/
            else if (ProviderType == ProviderTypes.OracleClient)
            {
                ExecQuery("drop sequence " + generatorName, QueryResultTypes.NonQuery);
                ExecQuery(String.Format("create sequence {0} start with {1}", generatorName, value), QueryResultTypes.NonQuery);
            }
            else
                throw new Exception(String.Format("��� ���������� {0} �� ��������������.", ProviderType));
        }

        /// <summary>
		/// deprecated
        /// </summary>
		public IDataUpdater GetDataUpdater(string queryText)
        {
            return GetDataUpdater(queryText, String.Empty, String.Empty, String.Empty);
        }

        /// <summary>
		/// deprecated
        /// </summary>        
 		public IDataUpdater GetDataUpdater(string queryText, string insertText, string updateText, string deleteText)
        {
            DataUpdater du = new DataUpdater(GetDataAdapter(), GetCommandBuilder(), factory, this.sessionContext);
            du.SelectCommand = this.connection.CreateCommand();
            du.SelectCommand.CommandText = queryText;

            if (String.IsNullOrEmpty(insertText))
                try
                {
                    du.InsertCommand = du.commandBuilder.GetInsertCommand();
                }
                catch { ; }
            else
            {
                du.InsertCommand = this.connection.CreateCommand();
                du.InsertCommand.CommandText = insertText;
            }

            if (String.IsNullOrEmpty(updateText))
                try
                {
                    du.UpdateCommand = du.commandBuilder.GetUpdateCommand();
                }
                catch { ; }
            else
            {
                du.UpdateCommand = this.connection.CreateCommand();
                du.UpdateCommand.CommandText = updateText;
            }

            if (String.IsNullOrEmpty(deleteText))
                try
                {
                    du.DeleteCommand = du.commandBuilder.GetDeleteCommand();
                }
                catch { ; }
            else
            {
                du.DeleteCommand = this.connection.CreateCommand();
                du.DeleteCommand.CommandText = deleteText;
            }

            if (du.SelectCommand != null && CommandTimeout > 0)
                du.SelectCommand.CommandTimeout = CommandTimeout;
            if (du.InsertCommand != null && CommandTimeout > 0)
                du.InsertCommand.CommandTimeout = CommandTimeout;
            if (du.UpdateCommand != null && CommandTimeout > 0)
                du.UpdateCommand.CommandTimeout = CommandTimeout;
            if (du.DeleteCommand != null && CommandTimeout >  0)
                du.DeleteCommand.CommandTimeout = CommandTimeout;

            return du;
        }

		/// <summary>
		/// ��������� ������������������� ���������� IDataUpdater (��� ������������� CommandBuilder)
		/// </summary>
		/// <param name="tableName">��� �������</param>
		/// <param name="attributes">���������� ���������� ������� �����</param>
		/// <returns>������������������ ��������� IDataUpdater</returns>
		/// <param name="selectFilter"></param>
		/// <param name="maxRecordCountInSelect"></param>
		/// <param name="selectFilterParameters"></param>
		public IDataUpdater GetDataUpdater(string tableName, IDataAttributeCollection attributes, 
			string selectFilter, int? maxRecordCountInSelect, IDbDataParameter[] selectFilterParameters)
		{
            selectFilterParameters = GetServerDbParams(selectFilterParameters);
			IDbDataAdapter adapter = GetDataAdapter();
            DataUpdater du = new DataUpdater(adapter, null, factory, sessionContext);
            InitDataAdapter(adapter, tableName, attributes, null, selectFilter, maxRecordCountInSelect, selectFilterParameters);
			du.Transaction = this.Transaction;
			return du;
		}

		/// <summary>
		/// ��������� ������������������� ���������� IDataUpdater (��� ������������� CommandBuilder)
		/// </summary>
		/// <param name="tableName">��� �������</param>
		/// <param name="attributes">���������� ���������� ������� �����</param>
		/// <returns>������������������ ��������� IDataUpdater</returns>
		public IDataUpdater GetDataUpdater(string tableName, IDataAttributeCollection attributes)
		{
			return GetDataUpdater(tableName, attributes, null, null, null);
		}

		/// <summary>
		/// ��������������� ������� ��� ���������� ���������� ������ (IDbCommand)
		/// </summary>
		/// <param name="attributes">��������� ���������� ������� �����</param>
		/// <param name="cmd">�������</param>
		/// <param name="RefIDNeeded">������������� ��������� ��������������� ��������� (�����������) ID</param>
		private void AppendParameters(IDataAttributeCollection attributes, IDbCommand cmd, bool RefIDNeeded)
		{
			// ������� ��� ���������, � ������������ � ����������� ��������� ����������
            foreach (IDataAttribute attr in attributes.Values)
			{
                IDataParameter prm;
                if (attr.Type == DataAttributeTypes.dtBLOB)
                {
                    if (!this.UseBlob)
                        continue;
                    prm = this.CreateBlobParameter(attr.Name, ParameterDirection.Input);
                    prm.SourceColumn = attr.Name;
                    cmd.Parameters.Add(prm);
                }
                else
                {
                    prm = this.CreateParameter(attr.Name, attr.Type, attr.Size);
                    prm.SourceColumn = attr.Name;
                    cmd.Parameters.Add(prm);
                }
			}
			// ���� ����� - ��������� ��������-����������� (ID)
			if (RefIDNeeded)
			{
                IDataParameter RefID = this.CreateParameter("RefID", DataAttributeTypes.dtInteger, 10);
				RefID.SourceColumn = "ID";
				cmd.Parameters.Add(RefID);
			}
        }


        #region ������������� ��������� �������������

        /// <summary>
        /// ������������� �������� ��������
        /// </summary>
        /// <param name="tr">����������</param>
        /// <param name="tableName">�������</param>
        /// <returns>�������</returns>
        public IDbCommand InitDeleteCommand(IDbTransaction tr, string tableName)
        {
            // ���� ������� �� ������� - �������
            IDbCommand deleteCommand = InitCommand(tr);

            // DELETE
            // ��������-����������� - ID
            IDataParameter deleteRefID = this.CreateParameter("RefID", DataAttributeTypes.dtInteger, 10);
            deleteRefID.SourceColumn = "ID";
            deleteCommand.Parameters.Add(deleteRefID);
            // ����� ������� �������� ������
            string queryDelete = this.GetQuery(String.Format(
                "delete from {0} where ID = ?", tableName), deleteCommand.Parameters);
            deleteCommand.CommandText = queryDelete;

            if (CommandTimeout > 0)
                deleteCommand.CommandTimeout = CommandTimeout;

            return deleteCommand;
        }

        /// <summary>
        /// ������������� �������� �������
        /// </summary>
        /// <param name="tr">����������</param>
        /// <param name="tableName">�������</param>
        /// <param name="attributes">��������</param>
        /// <returns>�������</returns>
        public IDbCommand InitInsertCommand(IDbTransaction tr, string tableName, 
            IDataAttributeCollection attributes)
        {
            // ���� ������� �� ������� - �������
            IDbCommand insertCommand = InitCommand(tr);

            List<string> attributesList = new List<string>();

            // ��������� ������ ���� ��������
            foreach (IDataAttribute attr in attributes.Values)
            {
                if ((attr.Type == DataAttributeTypes.dtBLOB) && !this.UseBlob)
                    continue;
                attributesList.Add(attr.Name.ToUpper());
            }

            // INSERT
            // ��������� ��� ���������
            AppendParameters(attributes, insertCommand, false);
            // ��������� ����� ������� ��� ������� ������� ������
            string insertFields = String.Join(", ", attributesList.ToArray());
            StringBuilder insert_sb = new StringBuilder();
            for (int i = 0; i <= attributesList.Count - 1; i++)
            {
                insert_sb.Append("?");
                if (i < attributesList.Count - 1)
                    insert_sb.Append(", ");
            }
            string queryInsert = String.Format(
                "insert into {0} ({1}) values ({2})", tableName, insertFields, insert_sb);
            queryInsert = this.GetQuery(queryInsert, insertCommand.Parameters);
            insertCommand.CommandText = queryInsert;

            if (CommandTimeout > 0)
                insertCommand.CommandTimeout = CommandTimeout;

            return insertCommand;
        }

        /// <summary>
        /// ������������� �������� ����������
        /// </summary>
        /// <param name="tr">����������</param>
        /// <param name="tableName">�������</param>
        /// <param name="attributes">��������</param>
        /// <returns>�������</returns>
        public IDbCommand InitUpdateCommand(IDbTransaction tr, string tableName, 
            IDataAttributeCollection attributes)
        {
            // ���� ������� �� ������� - �������
            IDbCommand updateCommand = InitCommand(tr);

            List<string> attributesList = new List<string>();

            // ��������� ������ ���� ��������
            foreach (IDataAttribute attr in attributes.Values)
            {
                if ((attr.Type == DataAttributeTypes.dtBLOB) && !this.UseBlob)
                    continue;
                attributesList.Add(attr.Name.ToUpper());
            }

            // UPDATE
            // ��������� ��� ��������� + ��������-����������� (ID)
            AppendParameters(attributes, updateCommand, true);
            // ��������� ����� ������� ���������� ������
            string queryUpdate = String.Join("=?, ", attributesList.ToArray()) + "=?";
            queryUpdate = this.GetQuery(String.Format(
                "update {0} set {1} where ID = ?", tableName, queryUpdate), updateCommand.Parameters);
            updateCommand.CommandText = queryUpdate;

            if (CommandTimeout > 0)
                updateCommand.CommandTimeout = CommandTimeout;

            return updateCommand;
        }

        /// <summary>
        /// ������������� �������� �������
        /// </summary>
        /// <param name="tr">����������</param>
        /// <param name="tableName">�������</param>
        /// <param name="attributes">��������</param>
        /// <param name="selectFilter">�����������</param>
        /// <param name="maxRecordCountInSelect">���������� ������� ��� �������</param>
        /// <param name="selectFilterParameters">��������� �������</param>
        /// <returns>�������</returns>
        public IDbCommand InitSelectCommand(IDbTransaction tr, string tableName, 
            IDataAttributeCollection attributes, string selectFilter, int? maxRecordCountInSelect, 
            IDbDataParameter[] selectFilterParameters)
        {
            // ���� ������� �� ������� - �������
            IDbCommand selectCommand = InitCommand(tr);

            List<string> attributesList = new List<string>();

            // ��������� ������ ���� ��������
            foreach (IDataAttribute attr in attributes.Values)
            {
                if ((attr.Type == DataAttributeTypes.dtBLOB) && !this.UseBlob)
                    continue;
                attributesList.Add(attr.Name.ToUpper());
            }

            // SELECT
            // ��������� ������ ��� ������� ������� ������
            string querySelect = String.Format("select {0} from {1}",
                    String.Join(", ", attributesList.ToArray()), tableName);

            // ���� ������� �������������� ���������-�����������, ��������� � ��
            if ((selectFilter != null) && (selectFilter != String.Empty))
            {
                if (selectFilterParameters != null)
                    for (int i = 0; i < selectFilterParameters.Length; i++)
                        selectCommand.Parameters.Add(CreateParameter(selectFilterParameters[i].ParameterName,
                                                                     selectFilterParameters[i].Value));
                querySelect = this.GetQuery(querySelect + " where " + selectFilter, selectCommand.Parameters);
            }

            // ���� ������� ������������ ���-�� ������� � ������� - ���������
            if (maxRecordCountInSelect != null)
                querySelect = AppendMaxRecordCountConstraintIntoSelectQuery(
                    querySelect, (int)maxRecordCountInSelect);

            selectCommand.CommandText = querySelect;

            if (CommandTimeout > 0)
                selectCommand.CommandTimeout = CommandTimeout;

            return selectCommand;
        }

        /// <summary>
        /// ������������� ��������
        /// </summary>
        /// <param name="tr">����������</param>
        /// <returns>�������</returns>
        public IDbCommand InitCommand(IDbTransaction tr)
        {
            IDbCommand command = this.connection.CreateCommand();
            if (tr != null)
                command.Transaction = tr;

            if (this.CommandTimeout > 0)
                command.CommandTimeout = this.CommandTimeout;

            return command;
        }

        #endregion ������������� ��������� �������������


        public void InitDataAdapter(IDbDataAdapter adapter, string tableName, IDataAttributeCollection attributes,
			IDbTransaction tr, string selectFilter, int? maxRecordCountInSelect, IDbDataParameter[] selectFilterParameters)
		{
            adapter.SelectCommand = InitSelectCommand(tr, tableName, attributes, selectFilter, maxRecordCountInSelect,
                selectFilterParameters);

            adapter.UpdateCommand = InitUpdateCommand(tr, tableName, attributes);

            adapter.InsertCommand = InitInsertCommand(tr, tableName, attributes);

            adapter.DeleteCommand = InitDeleteCommand(tr, tableName);
		}

		/// <summary>
		/// ������������� ������ IDbDataAdapter ��� ������������� ������-�������
		/// </summary>
		/// <param name="adapter">��������� IDbDataAdapter</param>
		/// <param name="tableName">��� �������</param>
		/// <param name="attributes">��������� ���������� ������� �����</param>
		/// <param name="tr">���������� � ������� ����� �������� ������� ��������</param>
		public void InitDataAdapter(IDbDataAdapter adapter, string tableName, IDataAttributeCollection attributes, 
			IDbTransaction tr)
		{
			InitDataAdapter(adapter, tableName, attributes, tr, null, null, null);
		}

		/// <summary>
		/// ������������� ������ IDbDataAdapter ��� ������������� ������-�������
		/// </summary>
		/// <param name="adapter">��������� IDbDataAdapter</param>
		/// <param name="tableName">��� �������</param>
		/// <param name="attributes">��������� ���������� ������� �����</param>
		public void InitDataAdapter(IDbDataAdapter adapter, string tableName, IDataAttributeCollection attributes)
		{
			InitDataAdapter(adapter, tableName, attributes, null);
		}


        [DebuggerStepThrough()]
        private DbCommandBuilder GetCommandBuilder()
        {
            return factory.CreateCommandBuilder();
        }

        [DebuggerStepThrough()]
        public IDbDataAdapter GetDataAdapter()
        {
            return factory.CreateDataAdapter();
        }

        [DebuggerStepThrough()]
        private IDbDataParameter GetParameter()
        {
            return factory.CreateParameter();
        }

        public IDbDataParameter[] GetParameters(int count)
        {
            IDbDataParameter[] prm = new DbParameter[count];
            for (int i = 0; i < count; prm[i++] = GetParameter()) {;}
            return prm;
        }

        #region CreateParameter

        [DebuggerStepThrough()]
        public IDbDataParameter CreateParameter(string name, object value, DbType dbType)
        {
            return CreateParameter(name, value, dbType, ParameterDirection.Input);
        }

        [DebuggerStepThrough()]
        public IDbDataParameter CreateParameter(string name, object value, DbType dbType, ParameterDirection direction)
        {
            IDbDataParameter parameter = GetParameter();
            parameter.ParameterName = name;
            parameter.DbType = dbType;
            parameter.Value = value;
            parameter.Direction = direction;
            return parameter;
        }

        [DebuggerStepThrough()]
        public IDbDataParameter CreateParameter(string name, object value)
        {
            IDbDataParameter parameter = GetParameter();
            parameter.ParameterName = name;
            Type tp = value.GetType();
            switch (tp.FullName)
            {
                case "System.Boolean":
                    parameter.DbType = DbType.Boolean;
                    break;
                case "System.Byte":
                    parameter.DbType = DbType.Byte;
                    break;
                case "System.SByte":
                    parameter.DbType = DbType.SByte;
                    break;
                case "System.Char":
                    parameter.DbType = DbType.String;
                    break;
                case "System.Decimal":
                    parameter.DbType = DbType.Decimal;
                    break;
                case "System.Double":
                    // �� �� ������� � �����������, ������ ��� �� Decimal
                    parameter.DbType = DbType.Decimal;//DbType.Double;
                    break;
                case "System.Single":
                    parameter.DbType = DbType.Single;
                    break;
                case "System.Int32":
                    parameter.DbType = DbType.Int32;
                    break;
                case "System.UInt32":
                    parameter.DbType = DbType.UInt32;
                    break;
                case "System.Int64":
                    parameter.DbType = DbType.Int64;
                    break;
                case "System.UInt64":
                    parameter.DbType = DbType.UInt64;
                    break;
                case "System.Int16":
                    parameter.DbType = DbType.Int16;
                    break;
                case "System.UInt16":
                    parameter.DbType = DbType.UInt16;
                    break;
                case "System.String":
                    // ******************************************** 
                    // ��� DbType.String ������ ������ � ���� UNICODE
                    // � ���� ����� ��������� �������� ������ 2000 ��������� 
                    // - ��� ������ ������ � ���� ���������� ���������� � ���������� �������
                    // �������� ��� ������ � ������-�� ���������������� UNICODE -> ANSI
                    // ��������� �� ������ ���������, ������� ��� DbType.String �� DbType.AnsiString
                    // �� ������� �������� � ��� ��� ������������ ����� ������ ��� ���������� ������ ���� 
                    // 8000 ��������
                    // ********************************************
                    //parameter.DbType = DbType.String;
                    parameter.DbType = DbType.AnsiString;
                    break;
                case "System.DateTime":
                    parameter.DbType = DbType.DateTime;
                    break;
                case "System.DBNull":
                    parameter.DbType = DbType.AnsiString;
                    break;
                default:
                    throw new InvalidCastException(String.Format("�������� {0} ����� ����������� ��� �������� {1}", name, tp));
            }
            parameter.Value = value;
            return parameter;
        }

        [DebuggerStepThrough()]
        public IDbDataParameter CreateParameter(string name, DataAttributeTypes dtType, int size)
		{
			IDbDataParameter parameter = GetParameter();
			parameter.ParameterName = name;
			switch (dtType)
			{
				case DataAttributeTypes.dtBoolean:
					parameter.DbType = DbType.Boolean;
					break;
				case DataAttributeTypes.dtChar:
					parameter.DbType = DbType.Byte;
					break;
				case DataAttributeTypes.dtDate:
					parameter.DbType = DbType.Date;
					break;
				case DataAttributeTypes.dtDateTime:
					parameter.DbType = DbType.DateTime;
					break;
				case DataAttributeTypes.dtDouble:
                    // �� �� ������� � �����������, ������ ��� �� Decimal
                    parameter.DbType = DbType.Decimal;//DbType.Double;
					break;
				case DataAttributeTypes.dtInteger:
                    if (size >= 10) parameter.DbType = DbType.Int64;
                    else parameter.DbType = DbType.Int32;
					break;
                case DataAttributeTypes.dtString:
					parameter.DbType = DbType.AnsiString;
					break;
			}
			return parameter;
		}
		
        #endregion CreateParameter

		#region ������ � �������

		private IDbDataParameter GetBlobParameter(string name, ParameterDirection direction, byte[] parameterData)
		{
            IDbDataParameter prm = factory.CreateParameter();
            prm.DbType = DbType.Binary;
			prm.ParameterName = name;
			prm.Direction = direction;
			if ((parameterData != null) && (direction != ParameterDirection.Output) && (direction != ParameterDirection.ReturnValue))
				prm.Value = parameterData;

			return prm;
		}

		/// <summary>
		/// ������� ��������-����
		/// </summary>
		/// <param name="name">��� ���������</param>
		/// <param name="direction">�����������</param>
		/// <returns>��������� ��������</returns>
		public IDbDataParameter CreateBlobParameter(string name, ParameterDirection direction)
		{
			return CreateBlobParameter(name, direction, null);
		}

		/// <summary>
		/// ������� ��������-����
		/// </summary>
		/// <param name="name">��� ���������</param>
		/// <param name="direction">�����������</param>
		/// <param name="parameterData">������</param>
		/// <returns>��������� ��������</returns>
		public IDbDataParameter CreateBlobParameter(string name, ParameterDirection direction, byte[] parameterData)
		{
			return GetBlobParameter(name, direction, parameterData);
		}
		#endregion

        /// <summary>
        /// ����������� ��������� ���������� IDbDataParameter � ���������, ������������ ��� ��������
        /// </summary>
        /// <param name="dbParams"></param>
        /// <returns></returns>
        private IDbDataParameter[] GetServerDbParams(IDbDataParameter[] dbParams)
        {
            if (dbParams == null)
                return null;
            IDbDataParameter[] newDbParams = new IDbDataParameter[dbParams.Length];
            for (int i = 0; i <= dbParams.Length - 1; i++)
            {
                switch (dbParams[i].DbType)
                {
                    // �������� ���� Blob
                    case DbType.Binary:
                        if (dbParams[i].Value != null && dbParams[i].Value != DBNull.Value)
                            newDbParams[i] = CreateBlobParameter(dbParams[i].ParameterName, dbParams[i].Direction,
                                                               (byte[])dbParams[i].Value);
                        else
                            newDbParams[i] = CreateParameter(dbParams[i].ParameterName, dbParams[i].Value,
                                                           dbParams[i].DbType, dbParams[i].Direction);;
                        break;
                    default:
                        newDbParams[i] = CreateParameter(dbParams[i].ParameterName, dbParams[i].Value,
                                                           dbParams[i].DbType, dbParams[i].Direction);
                        newDbParams[i].Size = dbParams[i].Size;
                        break;
                }
                
            }
            return newDbParams;
        }
    }
}
