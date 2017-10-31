using System;
using System.Runtime.Remoting.Lifetime;
using System.Threading;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Common
{
    /// <summary>
    /// Клиентская сессия. Класс создается и находится на клиенте 
    /// (по идее должен являться спонсором лицензий для серверных объектов, точнеее для серверной сессии)
    /// </summary>
    public class ClientSession : DisposableObject, ISponsor
    {
		/// <summary>
		/// Возвращает экземпляр созданной клиентской сессии.
		/// </summary>
    	public static ClientSession Instance
    	{
    		get
    		{
				LogicalCallContextData lccd = LogicalCallContextData.GetContext();
				return (ClientSession)lccd["ClientSession"];
			}
    	}

        public static void GetAuthenticationInfo(IScheme Scheme, out int authType, out string login, out string pwdHash)
        {
            // Вытащим из контекста вызова тип аутентификации
            LogicalCallContextData lccd = LogicalCallContextData.GetContext();
            AuthenticationType _authType = (AuthenticationType)lccd["AuthType"];
            authType = (int)_authType;
            login = string.Empty;
            pwdHash = string.Empty;

            switch (_authType)
            {
                case AuthenticationType.atWindows:
                    login = Scheme.UsersManager.GetCurrentUserName();
                    break;
                case AuthenticationType.adPwdSHA512:
                    login = Scheme.UsersManager.GetCurrentUserName();
                    // Хэш пользовательского пароля достанем запросом из базы
                    int id = Scheme.UsersManager.GetCurrentUserID();
                    using (IDatabase db = Scheme.SchemeDWH.DB)
                    {
                        string query = String.Format("select PwdHashSHA from users where id = {0}", id);
                        pwdHash = db.ExecQuery(query, QueryResultTypes.Scalar).ToString();
                    }
                    break;
                default:
                    break;
            }
        }
        
		/// <summary>
        /// Статический конструктор. Создает экземпляр класса и помещает в контекст вызова.
        /// </summary>
        /// <param name="clientType">Тип сессии</param>
        public static ClientSession CreateSession(SessionClientType clientType)
        {
            LogicalCallContextData lccd = LogicalCallContextData.GetContext();
            ClientSession clientSession = new ClientSession();
            clientSession._clientType = clientType;
            lccd["ClientSession"] = clientSession;
            lccd["Application"] = Environment.CommandLine;
            lccd["Host"] = Environment.MachineName;
            lccd["ClientType"] = clientType;
            lccd["IsDeveloper"] = CommandLineUtils.ParameterPresent("LogAsDeveloper");
            lccd["IgnoreVersions"] = CommandLineUtils.ParameterPresent("IgnoreVersions");
            return clientSession;
        }

        private SessionClientType _clientType = SessionClientType.Undefined;
        private ISessionManager _sessionManager = null;
        
        // ID соответствующей серверной сессии (для отсылки нотификаций)
        private string serverSessionID;

        /// <summary>
        /// Интерфейс пула серверных сессий к которому относится клиентская. 
        /// Используется для уведомлений о том что сессия жива
        /// </summary>
        public ISessionManager SessionManager
        {
            set 
            {
                // если схема ранее не была задана - создаем таймер нотификаций
                if ((_sessionManager == null) && (value != null))
                {
                    LogicalCallContextData lccd = LogicalCallContextData.GetContext();
                    // запоминаем ID серверной сессии, т.к. для WEB-сервиса контект вызова будет недоступен 
                    serverSessionID = Convert.ToString(lccd["SessionID"]);
                    // запускаем таймер нотификации
                    // На будущее: надо будет последить будут ли проходить вызовы при сильной загруженности нити.
                    // Если нет - использовать другой таймер или сделать отдельную нить.
                    _aliveTimer = new Timer(new TimerCallback(this.OnAliveTimer), null, TimeSpan.Zero, new TimeSpan(0, 1, 0));
                }
                _sessionManager = value; 
            }
        }

        // таймер для вызова метода уведомления схемы о том что сессия жива
        private Timer _aliveTimer = null;
        /// <summary>
        /// ОБраточик срабатывания таймера
        /// </summary>
        /// <param name="state"></param>
        private void OnAliveTimer(object state)
        {
            // если интерфейс пула сессий задан - уведомляем его о том что клиентская сессия жива
            if (_sessionManager != null)
            {
                try
                {
					_sessionManager.ClientSessionIsAlive(serverSessionID);
                }
                catch
                {
                    // если работаем в контексте ASP.NET - самоликвидируемся, веб сервис не рушим
                    if (_clientType == SessionClientType.WebService)
                    {
                        Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        private ClientSession()
        {
        }

		public override void Close()
		{
            LogicalCallContextData lccd = LogicalCallContextData.GetContext();
			
            base.Close();
            
            if (lccd != null)
            {
                Trace.TraceVerbose("Клиентская сессия {0} уничтожена.", lccd["SessionID"]);
            }
		}

        /// <summary>
        /// Деструктор класса
        /// </summary>
        /// <param name="disposing">Вызван пользователем или сборщиком мусора</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_aliveTimer != null)
                {
                    _aliveTimer.Dispose();
                    _aliveTimer = null;
                }
                _sessionManager = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Объект жив
        /// </summary>
        /// <returns>true/false</returns>
        [Obsolete("В следующих версиях метод будет удален")]
        public bool Alive()
        {
            return true;
        }

        /// <summary>
        /// Запущенное приложение и его параметры
        /// </summary>
        public string Application
        {
            get { return Environment.CommandLine; }
        }

        /// <summary>
        /// Имя машины
        /// </summary>
        public string Host
        {
            get { return Environment.MachineName; }
        }

		/// <summary>
		/// ID клиентской сессии. Извлекается из контекста вызова.
		/// </summary>
    	public static string SessionId
    	{
    		get
    		{
                LogicalCallContextData lccd = LogicalCallContextData.GetContext();
    			return lccd["SessionID"] as String;
    		}
    	}

		/// <summary>
		/// Если True, то работаем в режиме "разработчика", иначе в режиме обычного пользователя.
		/// </summary>
    	public static bool IsDeveloper
    	{
			get
			{
				LogicalCallContextData lccd = LogicalCallContextData.GetContext();
				return Convert.ToBoolean(lccd["IsDeveloper"]);
			}
		}

        #region ISponsor Members

        /// <summary>
        /// Запрос на продление лицензии
        /// </summary>
        /// <param name="lease">лицензия</param>
        /// <returns>на сколько продлить</returns>
        public new TimeSpan Renewal(ILease lease)
        {
            //Debug.Assert(lease.CurrentState == LeaseState.Active);

            // Продлеваем лицензию на 5 секунд
            return TimeSpan.FromSeconds(10);
        }

        #endregion
    }
}
