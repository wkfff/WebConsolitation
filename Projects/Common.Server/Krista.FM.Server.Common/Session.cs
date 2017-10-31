using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Security.Principal;

using Krista.FM.Common;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Common
{
    public sealed class Session : DisposableObject, ISession
    {
        private IScheme scheme;
         
        /// <summary>
        /// Уникальный идентификатор сессии
        /// </summary>
        private string sessionId = String.Empty;

        private IPrincipal principal;
        private DateTime logonTime;
        private ClientSession clientSession;
        private List<IDisposable> resources;
        private IDbConnection connection;
        private ConnectionPool connectionPool;
        private bool blocked = false;
        private bool inDisposing = false;
        private SessionClientType clientType;
        //private TimeSpan sessionTimeout = new TimeSpan(0, 10, 0);

        // Время последнего сообщения клиента о том что он жив.
        // Сравнивается с текущим временем и если разница превышает некоторрое значение - 
        // клиент считается мертвым и его сессия подлежит уничтожению
        // Но есть объекты для который такая методика может не подойти, например закачки -
        // у них основная нить всегда загружена и нотификация от таймера может не пройти.
        // Или серверные сессии - они существуют всегда на протяжении жизни схемы, нет смысла
        // их проверять.
        // Поэтому пока, чтобы не усложнять код отдельной нотифицирующей нитью, проинициализируем
        // начальное значение так, чтобы разница между текущим временем и им всегда была отрицательной.
        // Т.е. сессии у которых не задан ISessionManager буду жить пока им явно не вызовешь Dispose.
        private DateTime lastClientResponseTime = DateTime.MaxValue;

        /// <summary>
        /// Прилоложение из которого подключились
        /// </summary>
        private string aplication;

        /// <summary>
        /// Имя машины с которой подключилось клиентское приложение
        /// </summary>
        private string host;

        private List<string> executedActions = new List<string>(20);

        /// <summary>
        /// Сессия
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="sessionId"></param>
        /// <param name="clientType"></param>
        public Session(IScheme scheme, string sessionId, SessionClientType clientType)
        {
            this.scheme = scheme;
            this.sessionId = sessionId;
            this.clientType = clientType;
            resources = new List<IDisposable>();
            connectionPool = new ConnectionPool();
        }

        /// <summary>
        /// Сессия NHibernate привязанная к сесии клиента.
        /// </summary>
        public NHibernate.ISession PersistenceSession { get; set; }

        /// <summary>
        /// UnitOfWork выполняемый в рамках сесии клиента.
        /// </summary>
        public IUnitOfWork UnitOfWork { get; set; }

        public List<IDisposable> Resources
        {
            get
            {
                return resources;
            }
        }

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            try
            {
                Close(LogicalCallContextData.GetContext());
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #endregion

        /// <summary>
        /// Регистрация объекта в сессии
        /// </summary>
        /// <param name="obj">Регистрируемый объект</param>
        public void Register(IDisposable obj)
        {
            if (inDisposing)
                throw new Exception(String.Format("Сессия {0} в состоянии уничтожения.", sessionId));

            if (!resources.Contains(obj))
                resources.Add(obj);
            else
                throw new Exception(String.Format("Объект {0} уже используется сессией.", obj));
        }

        /// <summary>
        /// Удаление объекта из сессии
        /// </summary>
        /// <param name="obj"></param>
        public void Unregister(IDisposable obj)
        {
            if (resources.Contains(obj) && !inDisposing)
                resources.Remove(obj);
        }

        /// <summary>
        /// Запись в историю выполненых действий
        /// </summary>
        /// <param name="actionText">Описание выполненого действия</param>
        public void PostAction(string actionText)
        {
            if (inDisposing)
                throw new Exception(String.Format("Сессия {0} в состоянии уничтожения.", sessionId));

            executedActions.Add(actionText);
        }

        // Интервал времени, отведенный клиентской сессии на самовосстановление, по истечении - сессиия уничтожается
        //private readonly TimeSpan REMOTING_CLIENTCALL_TIMEOUT = new TimeSpan(0, 5, 0);
        // время последней ошибки при опросе состояния клиента
        //private DateTime lastRemotingClientCallErrorTime = DateTime.MaxValue;

        private static TimeSpan _maxClientResponseDelay = new TimeSpan(0, 10, 0);

        public static TimeSpan MaxClientResponseDelay
        {
            get { return _maxClientResponseDelay; }
            set { _maxClientResponseDelay = value; }
        }

        /// <summary>
        /// Проверяет доступность(активность) клиента
        /// </summary>
        /// <returns>Если клиент не доступен, то возвращает false</returns>
        public bool Alive()
        {
            return DateTime.Now - LastClientResponseTime <= MaxClientResponseDelay;
            #region UNUSED 
            // Метод с задержкой уничтожения клиента, если он не ответил серверу вовремя, себя не оправдала,
            // могут быть ситуации когда сервер вообще не может опросить клиента - включенный фаервол, запрещающий обратные вызовы,
            // кривая настройка сети, глючные драйвера и т.п.
            // Попробуем вообще отказать от опроса сервером клиентов, сделаем наоборот - клиент будет сообщать серверу
            // о том что он жив. В этом случае, при плохой связи, пользователь сразу получит четкое сообщение
            // в виде эксепшена о том что удаленный компьютер недоступен.
            /*
            try
            {
                // проверям доступность клиента
                clientSession.Alive();
                // если клиент доступен, но предыдущее обращение к нему закончилось с ошибкой - пишем
                // сообщение о восстановлении подключения для сессии
                if (lastRemotingClientCallErrorTime != DateTime.MaxValue)
                    Trace.TraceWarning(String.Format("Клиентское подключение для сессии {0} восстановлено", this.SessionId));
                // сбрасываем время возникновения последней ошибки
                lastRemotingClientCallErrorTime = DateTime.MaxValue;
                return true;
            }
            catch (Exception e)
            {
                // пишем в трэйс расшифровку ошибки
                Trace.TraceError(String.Format("Ошибка при запросе состояния сессии {0}: {1}", 
                    this.SessionId, ExceptionHelper.DumpException(e)));
                // если это первая ошибка для клиента  - запоминаем время возникновения
                if (lastRemotingClientCallErrorTime == DateTime.MaxValue)
                    lastRemotingClientCallErrorTime = DateTime.Now;
                // если разница между текущим временем и временем возникновения последней ошибки
                // превысила допустимое значение - клиент недоступен, сессию можно уничтожить 
                return DateTime.Now - lastRemotingClientCallErrorTime <= REMOTING_CLIENTCALL_TIMEOUT;
            }
             */
            #endregion
        }

        internal IScheme Scheme
        {
            get { return scheme; }
        }

        /// <summary>
        /// Уникальный идентификатор сессии
        /// </summary>
        public string SessionId
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return sessionId; }
        }

        /// <summary>
        /// Тип клиентской сессий
        /// </summary>
        public SessionClientType ClientType
        {
            get { return clientType; }
        }

        public IPrincipal Principal
        {
            get { return principal; }
            set { principal = value; }
        }

        /// <summary>
        /// Время подключения
        /// </summary>
        public DateTime LogonTime 
        {
            get { return logonTime; }
            set { logonTime = value; }
        }

        public int ResourcesCount
        {
            get { return resources.Count; }
        }

        public bool IsBlocked
        {
            get { return blocked; }
            set { blocked = value; }
        }

        /// <summary>
        /// Клиентская сессия
        /// </summary>
        public ClientSession ClientSession
        {
            get { return clientSession; }
            set { clientSession = value; }
        }

        /// <summary>
        /// Прилоложение из которого подключились
        /// </summary>
        public string Application
        {
            get 
            {
                try
                {
                    string appName;
                    string[] parts = aplication.Split('\"');
                    if (parts.GetLength(0) == 1)
                        appName = Path.GetFileName(parts[0]);
                    else
                        appName = Path.GetFileName(parts[1]);
                    return appName;
                }
                catch
                {
                    return aplication;
                }
            }
            set { aplication = value; }
        }

        /// <summary>
        /// Имя машины с которой подключилось клиентское приложение
        /// </summary>
        public string Host
        {
            get { return host; }
            set { host = value; }
        }

        /*public TimeSpan SessionTimeout
        {
            get { return sessionTimeout; }
            set { sessionTimeout = value; }
        }*/

        public DateTime LastClientResponseTime
        {
            get { return lastClientResponseTime; }
            set { lastClientResponseTime = value; }
        }

        public string ExecutedActions
        {
            get
            {
                return String.Join("\n", executedActions.ToArray());
            }
        }

        internal ConnectionPool ConnectionPool
        {
            get { return connectionPool; }
        }

        [Obsolete()]
        internal IDbConnection Connection
        {
            get { return connection; }
            set { connection = value; }
        }

        public void Close(LogicalCallContextData context)
        {
            if (context == null)
            {
                Trace.TraceError("Session.Close(LogicalCallContextData context): context is null.");
                return;
            }

            LogicalCallContextData callerContext = LogicalCallContextData.GetContext();
            try
            {
                LogicalCallContextData.SetContext(context);

                inDisposing = true;

                Trace.TraceVerbose("Освобождение ресурсов сессии ID = {0} User = {1}", sessionId, principal.Identity.Name);

                try
                {
                    if (UnitOfWork != null)
                    {
                        UnitOfWork.Rollback();
                    }

                    if (PersistenceSession != null)
                    {
                        PersistenceSession.Close();
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.ExpandException());
                }

                foreach (DisposableObject item in resources)
                {
                    try
                    {
                        item.Dispose();
                        Trace.TraceVerbose("Ресурс \"{0}\" уничтожен.", item);
                    }
                    catch { ; }
                }
                resources.Clear();
                ConnectionPool.Dispose();
                Trace.TraceInformation("Сессия уничтожена ID = {0} User = {1}", sessionId, principal.Identity.Name);

                ((SessionManager)Scheme.SessionManager).RemoveSession(this.SessionId);
            }
            finally
            {
                LogicalCallContextData.SetContext(callerContext);
            }
        }
    }
}