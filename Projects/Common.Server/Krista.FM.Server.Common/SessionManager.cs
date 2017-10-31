using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

using Krista.FM.Common;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Common
{
    /// <summary>
    /// Менеджер сессий
    /// </summary>
    public sealed class SessionManager : DisposableObject, ISessionManager
    {
        private static SessionManager instance;
        private IScheme scheme;
        private SessionIDManager sessionIDManager;
        private Dictionary<string, ISession> sessions = new Dictionary<string, ISession>();

        /// <summary>
        /// Основной поток сервера, обслуживающий очередь запросов
        /// </summary>
        private static Thread idleThread;

        public static SessionManager Instance
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                if (instance == null)
                {
                    throw new ServerException("Невозможно получить экземпляр SessionManager, т.к. он не проинициализирован.");
                }
                return instance;
            }
        }

		/// <summary>
		/// Этот метод должен вызываться только закачкой данных 
		/// для установки статического экземпляра серверного менеджера сессий.
		/// </summary>
		/// <param name="sessionManager">Ссылка на серверный объект менеджера сессий.</param>
		public static void SetInstanceSessionManagerForDataPump(ISessionManager sessionManager)
		{
			if (instance != null)
				throw new ServerException("Экземпляр серверного менеджера сессий уже проинициализирован.");
			
			instance = (SessionManager)sessionManager;
		}

        [System.Diagnostics.DebuggerStepThrough]
        public static SessionManager GetInstance(IScheme scheme)
        {
            if (instance == null)
            {
                lock (typeof(SessionManager))
                {
                    instance = new SessionManager(scheme);
                }
            }
            return instance;
        }

        /// <summary>
        /// Менеджер сессий
        /// </summary>
        public SessionManager(IScheme scheme)
        {
            this.scheme = scheme;
            sessionIDManager = new SessionIDManager();

            //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Idle), this);
            idleThread = new Thread(new ParameterizedThreadStart(Idle));
            idleThread.Priority = ThreadPriority.Lowest;
            idleThread.Start(this);
        }

        /// <summary>
        /// Функция выполняющая в фоновом режиме удаление сессий по истечению их времени жизни
        /// </summary>
        /// <param name="stateInfo">Параметр запуска</param>
        internal void Idle(Object stateInfo)
        {
            Thread.CurrentThread.Name = "SessionManager.Idle";
            while (true)
            {
                Thread.Sleep(new TimeSpan(0, 1, 0));

                lock (sessions)
                {
                    List<Session> forKill = new List<Session>();
                    foreach (Session item in sessions.Values)
                    {
                        if (!item.Alive() && !forKill.Contains(item))
                        {
                            /*if (item.ClientType == SessionClientType.WebService)
                            {
                                if ((DateTime.Now - item.LogonTime) < item.SessionTimeout)
                                    continue;
                            }*/
                            forKill.Add(item);
                            Trace.TraceVerbose("Сессия ID = {0} помечена на удаление.", item.SessionId);
                        }
                    }

                    foreach (Session item in forKill)
                    {
						LogicalCallContextData lccd = LogicalCallContextData.GetContext();
						try
						{
							lccd["SessionId"] = item.SessionId;
							item.Dispose();
							sessions.Remove(item.SessionId);
						}
						finally
						{
							lccd["SessionId"] = null;
						}
                    }
                    forKill.Clear();
                }
            }
        }

        public ISession this[string sessionId]
        {
            get
            {
                if (!sessions.ContainsKey(sessionId))
                    throw new Exception(String.Format("ID сессии не найден {0}.", sessionId));
                return sessions[sessionId];
            }
        }

        // TODO СУБД зависимый код
        private void SetDatabaseContextParameter(string parameterName, string parameterValue, string sessionId)
        {
            if (string.IsNullOrEmpty(scheme.SchemeDWH.FactoryName))
                return;
            if (scheme.SchemeDWH.FactoryName == ProviderFactoryConstants.SqlClient)
                return;

			IDbConnection connection = ((IConnectionProvider)scheme.SchemeDWH).Connection;
            IDbCommand cmd = null;
            try
            {
                connection.Open();
                cmd = connection.CreateCommand();
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
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                connection.Close();
            }
        }

        /// <summary>
        /// Устанавливает значения переменных контекста сессии в базе данных
        /// </summary>
        public void SetDatabaseContext(ISession session)
        {
            if (scheme.SchemeDWH == null)
                return;

            SetDatabaseContextParameter("SESSIONID", session.SessionId, session.SessionId);
            SetDatabaseContextParameter("USERNAME", session.Principal.Identity.Name, session.SessionId);
        }

        public string Create(LogicalCallContextData context)
        {
            sessionIDManager.InitializeRequest(context, scheme.Name);

            if (context["ClientType"] == null)
                throw new Exception("В контексте клиента не указано свойство ClientType");

            context["SchemeName"] = scheme.Name;

            SessionClientType ct = (SessionClientType)context["ClientType"];
            Session session = new Session(scheme, sessionIDManager.GetSessionID(context), ct);

            session.Principal = context.Principal;

            session.LogonTime = DateTime.Now;

            //Trace.TraceVerbose("lock (sessions)");
            lock (sessions)
            {
                sessions.Add(session.SessionId, session);
            }
            //Trace.TraceVerbose("unlock (sessions)");

            //context["Session"] = session;

            if (context["ClientSession"] != null)
            {
                session.ClientSession = (ClientSession)context["ClientSession"];
                session.Application = Convert.ToString(context["Application"]);
                session.Host = Convert.ToString(context["Host"]);
            }

            SetDatabaseContext(session);

            return session.SessionId;
        }

        public void Close(LogicalCallContextData context)
        {
            if (context == null)
                throw new Exception("Контекст пользователя не установлен или пользователь не аутентифицирован.");

            string sessionId = Convert.ToString(context["SessionID"]);

            if (!sessions.ContainsKey(sessionId))
                throw new KeyNotFoundException(String.Format("ID сессии не найден {0}.", sessionId));

            //((Session)sessions[sessionId]).Close(context);
            sessions[sessionId].Dispose();
            sessions[sessionId] = null;

            RemoveSession(sessionId);

            sessionIDManager.ClearRequest(context);
        }

        /// <summary>
        /// Удаление сессии из списка сессий
        /// </summary>
        /// <param name="sessionId">ID сессии</param>
        internal void RemoveSession(string sessionId)
        {
            lock (sessions)
            {
                sessions.Remove(sessionId);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (sessions)
                {
                    idleThread.Abort();
                    idleThread = null;
                    foreach (ISession session in sessions.Values)
                    {
                        try
                        {
                            session.Dispose();
                        }
                        catch (Exception e)
                        {
                            Trace.TraceError(e.ToString());
                        }
                    }
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Коллекция сессий
        /// </summary>
        public IDictionary<string, ISession> Sessions
        {
            get
            {
                return sessions;
            }
        }

        public void ClientSessionIsAlive(string sessionID)
        {
            Session session = (Session)this[sessionID];
            session.LastClientResponseTime = DateTime.Now;
            //Trace.TraceVerbose(String.Format(" {0} получен отклик от пользовательской сессии для {1}", DateTime.Now, sessionID));
        }

        public TimeSpan MaxClientResponseDelay
        {
            get { return Session.MaxClientResponseDelay; }
            set { Session.MaxClientResponseDelay = value; }
        }
    }
}
