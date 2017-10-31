using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.Threading;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server
{
    /// <summary>
    /// Основной корневой серверный объект IServer.
    /// Содержит в себе ссылки на объекты схем установленные на сервере
    /// </summary>
    public sealed class ServerClass : DisposableObject, IServer
    {
        #region Поля

        /// <summary>
        /// Заглушка для объекта схемы.
        /// </summary>
        private SchemeStub stub;

        /// <summary>
        /// Порт сервера.
        /// </summary>
        private int serverPort;

        /// <summary>
        /// Адрес веб сервиса
        /// </summary>
        private string webServiceUrl;
        
        /// <summary>
        /// Путь в каталору репозитория схем. 
        /// </summary>
        private string repositoryPath;

        #endregion Поля

        #region Базовые методы

        /// <summary>
        /// Конструктор серверного класса
        /// </summary>
        internal ServerClass()
        {
            LogicalCallContextData.SetAuthorization("SYSTEM");

            try
            {
                Trace.TraceEvent(TraceEventType.Information, "{0}", DateTime.Now);
                Trace.TraceEvent(TraceEventType.Information, "ID процесса: {0}", Process.GetCurrentProcess().Id);
                Trace.TraceEvent(TraceEventType.Information, "Режим работы GC: {0}", GCSettings.IsServerGC ? "серверный" : "клиентский");


                if (!CheckOSVersion())
                    return;

                if (!CheckServerAssemblyVersions())
                    return;

                webServiceUrl = GetConfigurationParameter("WebServiceUrl");
                ServerPort = Convert.ToInt16(GetConfigurationParameter("ServerPort"));

                if (!InitializeSchemes())
                {
                    Trace.TraceEvent(TraceEventType.Error, "Ошибка при инициализации схем", "ServerClass");
                }
            }
            catch (Exception e)
            {
                // В эту секцию исключения не должны попадать
                Trace.TraceEvent(TraceEventType.Critical, "Баг в конструкторе объекта ServerClass: {0}", e);
                //throw (new Exception(e.Message));
            }
        }

        /// <summary>
        /// Выполняет уничтожение серверного класса
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                Trace.TraceEvent(TraceEventType.Verbose, "~{0}({1})", GetType().FullName, disposing);
            }
            catch { /* глушим все исключения */ }

            lock (this)
            {
                if (disposing)
                {
                    try
                    {
                        stub.Shutdown();
                    }
                    catch (Exception e)
                    {
                        Trace.TraceEvent(TraceEventType.Critical, e.ToString());
                    }
                }
            }
            base.Dispose(disposing);
        }

        #endregion Базовые методы

        #region Инициализация

        /// <summary>
        /// Проверка версии операционной системы
        /// </summary>
        /// <returns>true - операционная система поддерживается</returns>
        private static bool CheckOSVersion()
        {
            Trace.TraceEvent(TraceEventType.Information, "Версия операционной системы: {0}", Environment.OSVersion);
            Trace.TraceEvent(TraceEventType.Information, "Количество процессоров: {0}", Environment.ProcessorCount);
            Trace.TraceEvent(TraceEventType.Information, "Версия CLR: {0}", Environment.Version);
            Trace.TraceEvent(TraceEventType.Information, "Имя машины: {0}", Environment.MachineName);
            Trace.TraceEvent(TraceEventType.Information, "Имя домена: {0}", Environment.UserDomainName);
            Trace.TraceEvent(TraceEventType.Information, "Запущен под учетной записью: {0}", Environment.UserName);
            return true;
        }

        /// <summary>
        /// Проверка версий сборок
        /// </summary>
        /// <returns></returns>
        private static bool CheckServerAssemblyVersions()
        {
            string serverLibraryVersion = AppVersionControl.GetServerLibraryVersion();

            string baseVesion = AppVersionControl.GetAssemblyBaseVersion(serverLibraryVersion);
            Trace.TraceEvent(TraceEventType.Information, "Базовая версия сервера {0}", baseVesion);

            Dictionary<string, string> badAssemblies = new Dictionary<string, string>();

            AppVersionControl.CheckAssemblyVersions(baseVesion, "Krista.FM.Common.dll", badAssemblies, true);
            AppVersionControl.CheckAssemblyVersions(baseVesion, "Krista.FM.Server.*.dll", badAssemblies, true);

            if (badAssemblies.Count > 0)
            {
                Trace.TraceEvent(TraceEventType.Critical, "Обнаружено {0} сборок с версией отличающейся от базовой.", badAssemblies.Count);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Процедура инициализации схем, которые он будет обслуживать
		/// </summary>
		/// <returns>
		/// Возвращаен истину или ложь в зависимости от результатов инициализации.
		/// Подробное описание ошибок должно находися в логе
		/// </returns>
        private bool InitializeSchemes()
        {
            // XML настройка репозитория
            string RepositorySettings = "RepositorySettings.xml";

            // Получаем путь к репозиторию из настроек приложения
            repositoryPath = GetConfigurationParameter("RepositoryPath");

            string[] qualifiedPath = repositoryPath.Split('\\');
            bool isRepositoryAbove2_3_0 = !qualifiedPath[qualifiedPath.Length - 1].Contains("Repository");

            Trace.TraceEvent(TraceEventType.Information, "Путь к репозиторию схемы " + repositoryPath, "ServerClass");

            if (isRepositoryAbove2_3_0)
            {
                InitializeScheme(repositoryPath + "\\Packages\\SchemeConfiguration.xml");
            }
            else
            {
                // Открываем и разбираем файл настроек репозитория
                string errMsg;
                XmlDocument xmlDoc = Validator.LoadValidated(repositoryPath + "\\" + RepositorySettings,
                    "ServerConfiguration.xsd", "xmluml", out errMsg);
                if (xmlDoc == null)
                {
                    Trace.TraceEvent(TraceEventType.Error, "Ошибка при загрузке настроек репозитория\n{0}", errMsg);
                    return false;
                }

                // Обрабатываем список схем репозитория
                XmlNodeList xmlSchemesList = xmlDoc.SelectNodes("/ServerConfiguration/Repository/Schemes/Scheme");
                foreach (XmlNode xmlScheme in xmlSchemesList)
                {
                    InitializeScheme(repositoryPath + "\\" + xmlScheme.Attributes["privatePath"].Value);
                    break; // Допустима поддержка только одной схемы
                }
            }

            return true;
        }

        /// <summary>
        /// Инициализация отдельной схемы.
        /// </summary>
        /// <param name="schemeFileName">Полный путь к XML-файлу настроек схемы</param>
        /// <returns>
        /// Возвращаен истину или ложь в зависимости от результатов инициализации схемы.
        /// Подробное описание ошибок должно находися в логе
        /// </returns>
        private bool InitializeScheme(string schemeFileName)
        {
            string schemeName = "UnassignedName";
            try
            {
                string errMsg;

                // для начала проверим на валидность
                XmlDocument xmlDoc = Validator.LoadValidated(schemeFileName, "ServerConfiguration.xsd", "xmluml", out errMsg);
                if (xmlDoc == null)
                {
                    Trace.TraceEvent(TraceEventType.Error, "Ошибка при загрузке настроек схемы{0}{1}", Environment.NewLine, errMsg);
                    return false;
                }

                // Получаем имя схемы
                XmlNode xmlNode = xmlDoc.SelectSingleNode("/ServerConfiguration/Package/@name");
                schemeName = xmlNode.Value;

                // Добавляем в коллекцию установленных схем сервера
                stub = new SchemeStub(this, schemeName, schemeFileName);
                stub.Startup();
                //if (stub.Scheme != null)
                //    stub.Scheme.Server = this;
            }
            catch (Exception e)
            {
                Trace.TraceEvent(TraceEventType.Error, "Ошибка инициализации схемы{0}{1}{2}", schemeName, Environment.NewLine, e.ToString());
                return false;
            }
            return true;
        }

        #endregion Инициализация

        #region Вспомогательные функции

        private bool IsConnected(LogicalCallContextData context)
        {
            if(context["SessionID"] == null)
                return false;

            string schemeName = Convert.ToString(context["SchemeName"]);
            if (!String.IsNullOrEmpty(schemeName))
            {
                if (stub.Scheme.Name == schemeName)
                    return true;

            }
            return false;
        }

        private void Disconnect(LogicalCallContextData context)
        {
            if (context == null)
                return;

            if (!IsConnected(context))
                return;
            
            Trace.TraceEvent(TraceEventType.Verbose, "ServerClass.Disconnect()", "Server");
            if (stub.Scheme != null)
            {
                stub.Disconnect();
            }
            
        }

        #endregion Вспомогательные функции

        #region Реализация интерфейса IServer

        void IServer.Activate()
        {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "Server";
            Trace.TraceEvent(TraceEventType.Verbose, "Объект сервера активирован");
            return;
        }

        public void Disconnect()
        {
            Disconnect(LogicalCallContextData.GetContext());
        }

        public ISchemeStub Connect()
        {
            if (!Authentication.IsSystemRole())
                throw new Exception("Недостаточно привелегий для подключения к неоткрытой схеме.");

            // проверим есть ли схема
            if (stub == null)
                throw new Exception("Схема запущена с ошибкой! См. лог сервера.");

            return stub;
        }

        public void Connect(ISchemeStub schemeStub, out IScheme schemeObject)
        {
            // TODO: Проверить контекст пользователя на предмет подключения к какой-либо схеме
            // и если он подключен, то принудительно отключим это соединение
            Disconnect(LogicalCallContextData.GetContext());

            // TODO: Создать сессию нового соединения и поместить ее атрибуты в контекст пользователя
            Trace.TraceEvent(TraceEventType.Verbose, "Scheme.Connect()");
            ((SchemeStub)schemeStub).Connect();

            schemeObject = ((SchemeStub)schemeStub).Scheme;
        }
        
        /*
        /// <summary>
        /// Подключение к схеме с именем schemeName
        /// </summary>
        /// <param name="schemeName">Имя схемы к которой нужно подключиться</param>
        /// <param name="schemeObject">Если подключение возможно, то scheme содержит указатель на объект схемы</param>
        /// <returns>
        /// Возвращаен истину или ложь в зависимости от результата подключения к схеме.
        /// Подробное описание ошибок должно находися в логе сервера приложений
        /// </returns>
        public bool Connect(string schemeName, out IScheme scheme)
        {
            string errStr = String.Empty;
            return Connect(schemeName, out scheme, AuthenticationType.atUndefined, String.Empty, String.Empty, ref errStr);
        }
        */

        public bool Connect(string schemeName, out IScheme scheme, AuthenticationType authType, string login,
            string pwdHash, ref string errStr)
        {
            return Connect(out scheme, authType, login, pwdHash, ref errStr, String.Empty);
        }
        
        public bool Connect(out IScheme scheme, AuthenticationType authType, string login,
            string pwdHash, ref string errStr)
        {
            return Connect(out scheme, authType, login, pwdHash, ref errStr, String.Empty);
        }

        public bool Connect(string schemeName, out IScheme scheme, AuthenticationType authType, string login,
            string pwdHash, ref string errStr, string clientServerLibraryVersion)
        {
            return Connect(out scheme, authType, login, pwdHash, ref errStr, clientServerLibraryVersion);
        }

        public bool Connect(out IScheme scheme, AuthenticationType authType, string login,
            string pwdHash, ref string errStr, string clientServerLibraryVersion)
        
        {
            Trace.TraceEvent(TraceEventType.Information, "Пользователь {0} запросил подключение к схеме под именем {1}. Способ идентификации {2}", Authentication.UserDate, login, authType);

            if (stub.Scheme == null)
            {
                errStr = "Схема запущена с ошибкой! См. лог сервера.";
                scheme = null;
                return false;
            }

            scheme = null;

            // если нужно - сверим версии сбрки с декларациями интерфейсов
            if (!String.IsNullOrEmpty(clientServerLibraryVersion))
            {
                string clientBaseVersion = AppVersionControl.GetAssemblyBaseVersion(clientServerLibraryVersion);
                string serverLibraryVersion = AppVersionControl.GetServerLibraryVersion();
                string serverBaseVersion = AppVersionControl.GetAssemblyBaseVersion(serverLibraryVersion);
                if (serverBaseVersion != clientBaseVersion)
                {
                    errStr = String.Concat(
                        String.Format("Обнаружено различие версий общей сборки '{0}'", AppVersionControl.ServerLibraryAssemblyName),
                        Environment.NewLine,
                        Environment.NewLine,
                        String.Format("Сервер: {0}", serverLibraryVersion),
                        Environment.NewLine,
                        String.Format("Клиент: {0}", clientServerLibraryVersion),
                        Environment.NewLine,
                        Environment.NewLine,
                        "Подключение невозможно"
                    );
                    return false;
                }
            }


            LogicalCallContextData userContext = LogicalCallContextData.GetContext();
            ISchemeStub ss;
            try
            {
                if (userContext["SchemeName"] == null)
                    userContext["SchemeName"] = stub.Scheme.Name;

                SessionContext.SetSystemContext();
                ss = Connect();
            }
            finally
            {
                LogicalCallContextData.SetContext(userContext);
            }

            if (ss == null)
                return false;

            Connect(ss, out scheme);

            Trace.TraceEvent(TraceEventType.Verbose, "Аутентификация клиента");
            errStr = String.Empty;
            // если нужно - аутентифицируем клиента
            switch (authType)
            {
                case AuthenticationType.atWindows:
                    // заглушка - добавляние вновь подключающихмя пользователей
                    scheme.UsersManager.CheckCurrentUser();
                    scheme.UsersManager.AuthenticateUser(login, ref errStr);
                    break;
                case AuthenticationType.adPwdSHA512:
                    scheme.UsersManager.AuthenticateUser(login, pwdHash, ref errStr);
                    break;
            }
            // если клиент не аутентифицирован - закрываем соединение
            if (!String.IsNullOrEmpty(errStr))
            {
                stub.Disconnect();
                scheme = null;
                Trace.TraceError(errStr);
            }
            else
                Trace.TraceVerbose(String.Format("Клиент аутентифицирован. ID сессии = {0}", SessionContext.SessionId));
            
            return String.IsNullOrEmpty(errStr);
        }

        /// <summary>
        /// Путь к репозиторию сервера приложений
        /// </summary>
        public string RepositoryPath
        {
            get { return repositoryPath; }
        }

        /// <summary>
        /// Адрес веб сервиса
        /// </summary>
        public string WebServiceUrl
        {
            get { return webServiceUrl; }
        }

        /// <summary>
        /// Возвращает список имен открытых схем на сервере
        /// </summary>
        public ICollection SchemeList
        {
            get
            {
                ArrayList sl = new ArrayList();
                sl.Add(stub.Scheme == null ? "Схема запущена с ошибкой! См. логи." : stub.Scheme.Name);
                return sl;
            }
        }

        public string Machine 
        { 
            get 
            { 
                return Environment.MachineName; 
            } 
        }

        public int ServerPort
        {
            get { return serverPort; }
            set
            {
                if (serverPort == 0)
                {
                    serverPort = value;
                }
                else
                    throw new Exception("Свойство ServerPort не редактируемое.");
            }
        }

        /// <summary>
        /// Возвращает конфигурационный параметр сервера приложений.
        /// </summary>
        /// <param name="key">Уникальный ключ параметра.</param>
        /// <returns>Значение параметра.</returns>
        public string GetConfigurationParameter(string key)
        {
            string value = ConfigurationManager.AppSettings[key];

            #region Временная заглушка для обработки отсутствия параметров
            if (value == null)
            {
                if (key == "ProductName")
                {
                    Assembly assembly = AppVersionControl.GetServerLibraryAssembly();
                    object[] attr = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                    if (attr.Length > 0)
                        return ((AssemblyProductAttribute)attr[0]).Product;
                    else
                        return null;
                }
                if (key == "SupportServiceInfo")
                {
                    Assembly assembly = AppVersionControl.GetServerLibraryAssembly();
                    object[] attr = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                    if (attr.Length > 0)
                        return ((AssemblyCompanyAttribute)attr[0]).Company;
                    else
                        return null;
                }
                if (key == "AssemblyBaseVersion")
                {
                   return AppVersionControl.GetAssemblyBaseVersion(AppVersionControl.GetServerLibraryVersion());
                }
            }
            #endregion

            return value;
        }

        #endregion Реализация IServer

        // объект будет жить вечно
        public override object InitializeLifetimeService()
        {          
            return null;
        }
    }
}
