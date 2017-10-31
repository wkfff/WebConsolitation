using System;
using System.Collections;
using System.Data;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web.Services;
using System.Text;
using System.IO;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

using Krista.Diagnostics;
using Krista.FM.Common;

using Krista.FM.ServerLibrary;
using Krista.FM.WriteBackLibrary;


namespace Krista.FM.Server.WebServices
{
    [WebService(Namespace = "http://tempuri.org/")]
    //[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class PlaningService : System.Web.Services.WebService
    {
        private string _serverName = "localhost:8008";
        private string _URL = "tcp://{0}/FMServer/Server.rem";

        public PlaningService()
        {
            string serverAddress = System.Web.Configuration.WebConfigurationManager.AppSettings["Server"];
            if (serverAddress != null)
                _serverName = serverAddress;
        }

        #region Вспомогательные методы

        public class PrincipalInfo
        {
            public string strType;
            public string Name = "";
            public string AuthenticationType = "";
            public bool IsAdministrator = false;
            public bool IsAuthenticated = false;

            public PrincipalInfo()
            { }
        }

        [WebMethod]
        public PrincipalInfo GetPrincipalInfo()
        {
            PrincipalInfo myPI = new PrincipalInfo();
            myPI.strType = this.User.GetType().ToString();
            if (this.User.GetType() == typeof(System.Security.Principal.WindowsPrincipal))
            {
                WindowsPrincipal winUser = (WindowsPrincipal)this.User;
                bool IsAdmin = winUser.IsInRole(WindowsBuiltInRole.Administrator);
                myPI.IsAdministrator = IsAdmin;
                myPI.AuthenticationType = winUser.Identity.AuthenticationType.ToString();
                myPI.Name = winUser.Identity.Name.ToString();
                myPI.IsAuthenticated = winUser.Identity.IsAuthenticated;
            }
            return myPI;
        }

        private static LogicalCallContextData GetContextData()
        {
            LogicalCallContextData data = (LogicalCallContextData)CallContext.GetData("Authorization");
            if (data == null)
            {
                LogicalCallContextData.SetAuthorization("SYSTEM Destroyer");
                data = (LogicalCallContextData)CallContext.GetData("Authorization");
            }
            return data;
        }

        [WebMethod(EnableSession = true)]
        public object GetUserID()
        {
            try
            {
                IScheme scheme = CheckConnection();
                return GetContextData()["UserID"];
            }
            catch (Exception e)
            {
                return KristaDiagnostics.ExpandException(e);
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        private const string SERVER_KEY_NAME = "Server";
        private const string CLIENT_SESSION_KEY_NAME = "ClientSession";
        private const string SCHEME_KEY_NAME = "Scheme";
        private const string LOGICAL_CALL_CONTEXT_DATA_KEY_NAME = "LogicalCallContextData";

        /// <summary>
        /// Отладочный метод подключения к схеме.
        /// </summary>
        /// <param name="withinTaskContext">Режим работы в контексте задач </param>
        /// <param name="authType">Тип аутентификации.</param>
        /// <param name="login">Имя пользователя или доменное имя (в зависимости от режима).</param>
        /// <param name="pwd">Пароль (не хэш!) для режима логин/пароль.</param>
        /// <param name="errStr">Строка с расшифровкой ошибки.</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string ConnectDebug(bool withinTaskContext, int authType, string login, string pwd, string errStr)
        {
            bool isConnected = Connect(withinTaskContext, (AuthenticationType)authType, login, pwd, ref errStr);
            if (string.IsNullOrEmpty(errStr))
                return isConnected.ToString();
            else
                return errStr;
        }

        /// <summary>
        /// Метод подключения к схеме.
        /// </summary>
        /// <param name="withinTaskContext">Режим работы в контексте задач </param>
        /// <param name="authType">Тип аутентификации.</param>
        /// <param name="login">Имя пользователя или доменное имя (в зависимости от режима).</param>
        /// <param name="pwd">Пароль (не хэш!) для режима логин/пароль.</param>
        /// <param name="errStr">Строка с расшифровкой ошибки.</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public bool Connect(bool withinTaskContext, AuthenticationType authType, string login, string pwd, ref string errStr)
        {
            try
            {
                if (Session == null)
                {
                    throw new Exception("Ошибка доступа к объекту Http-сессии");
                }

                // есть ли в сессии интерфейс сервера?
                IServer server = Session[SERVER_KEY_NAME] as IServer;
                if (server == null)
                {
                    server = (IServer)Activator.GetObject(typeof(IServer), String.Format(_URL, _serverName));
                    Session[SERVER_KEY_NAME] = server;
                }
                else
                {
                    // проверим действительна ли закэшированная ссылка на сервер
                    try
                    {
                        int portNum = Convert.ToInt32(server.GetConfigurationParameter("ServerPort"));
                    }
                    catch
                    {
                        // если нет - убиваем сессию, просим пользователя повторить операцию
                        Session.Abandon();
                        throw new Exception(
                            "Ссылка на экземпляр сервера более не действительна. Выполнен перезапуск пользовательской сессии. Попробуйте повторить запрос еще раз");
                    }
                }

                // был ли запомнен контекст вызова?
                LogicalCallContextData cnt = Session[LOGICAL_CALL_CONTEXT_DATA_KEY_NAME] as LogicalCallContextData;
                if (cnt != null)
                    LogicalCallContextData.SetContext(cnt);
                else
                    LogicalCallContextData.SetAuthorization();

                // есть ли в сессии интерфейс схемы? если запрошенная схема есть в сессии - возвращаем ее
                IScheme scheme = Session[SCHEME_KEY_NAME] as IScheme;
                if (scheme != null)
                {
                    // TODO: Отключиться от схемы
                    return true;
                }

                // иначе - выполняем подключение
                ClientSession clientSession = ClientSession.CreateSession(SessionClientType.WebService);
                // запоминаем ее в переменной сессии
                Session[CLIENT_SESSION_KEY_NAME] = clientSession;

                if (User.Identity.IsAuthenticated && authType == AuthenticationType.atWindows)
                {
                    login = User.Identity.Name;
                }
                else
                {
                    if (!withinTaskContext)
                        pwd = PwdHelper.GetPasswordHash(pwd);
                }

                if (!server.Connect(out scheme, authType, login, pwd, ref errStr))
                    throw new Exception(String.Format("Ошибка при подключении к схеме: {0}", errStr));

                // запоминаем контекст вызова (для установки в последующих вызовах методов в сессии)
                Session[LOGICAL_CALL_CONTEXT_DATA_KEY_NAME] = LogicalCallContextData.GetContext();
                // инициируем запуск процесса нотификации сервера о живости сессии
                clientSession.SessionManager = scheme.SessionManager;
                // устанавливаем сессии такой же таймаут как максимальная задрежка в отклике клиентской сессии
                Session.Timeout = scheme.SessionManager.MaxClientResponseDelay.Minutes;

                // запоминаем схему
                Session[SCHEME_KEY_NAME] = scheme;

                return true;
            }
            catch (Exception e)
            {
                errStr = e.Message;
                return false;
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }


        private IScheme CheckConnection()
        {
            if (Session == null)
            {
                throw new Exception("Ошибка доступа к объекту Http-сессии.");
            }

            IScheme scheme = Session[SCHEME_KEY_NAME] as IScheme; 
            if (scheme == null)
            {
                throw new Exception("Клиент не авторизован.");
            }

            LogicalCallContextData cnt = Session[LOGICAL_CALL_CONTEXT_DATA_KEY_NAME] as LogicalCallContextData;
            if (cnt != null)
                LogicalCallContextData.SetContext(cnt);
            else
                LogicalCallContextData.SetAuthorization();

            return scheme;
        }

        [WebMethod(EnableSession = true)]
        public void CloseSession()
        {
            // если ASP-сессии по каким-то причинам нет - ничего не делаем
            if (Session == null)
                return;
            // если есть клиентская сессия - уничтожаем
            ClientSession clientSession = Session[CLIENT_SESSION_KEY_NAME] as ClientSession;
            if (clientSession != null)
            {
                try
                {
                    clientSession.Dispose();
                }
                catch (Exception)
                {}
                Session[CLIENT_SESSION_KEY_NAME] = null;
            }
            // если есть контекст вызова - пытаемся достать из него серверную сессию и тоже уничтожить
            LogicalCallContextData contextData = Session[LOGICAL_CALL_CONTEXT_DATA_KEY_NAME] as LogicalCallContextData;
            if (contextData != null)
            {
                LogicalCallContextData.SetContext(contextData);
                
                IServer server = Session[SERVER_KEY_NAME] as IServer;
                if (server != null)
                    try
                    {
                        server.Disconnect();
                    }
                    catch {}
                                    
                Session[LOGICAL_CALL_CONTEXT_DATA_KEY_NAME] = null;
                CallContext.SetData("Authorization", null);
            }
            // уничтожаем ASP-сессию
            Session.Abandon();
        }

        #endregion Вспомогательные методы

        #region IServer Members

        [WebMethod(EnableSession = true)]
        public ArrayList GetSchemes()
        {
            try
            {
                CheckConnection();
                IServer server = Session[SERVER_KEY_NAME] as IServer;
                return new ArrayList(server.SchemeList);
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        [WebMethod(EnableSession = true)]
        public string GetSchemeName()
        {
            try
            {
                IServer server = Session[SERVER_KEY_NAME] as IServer;
                if (server == null)
                {
                    server = (IServer)Activator.GetObject(typeof(IServer), String.Format(_URL, _serverName));
                }

                object name = new ArrayList(server.SchemeList)[0];
                return Convert.ToString(name);
            }
            catch (Exception e)
            {
                return KristaDiagnostics.ExpandException(e);
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        #endregion IServer Members

        #region Для расходников - получение данных объектов схемы
        [WebMethod(EnableSession = true)]
        public string GetSchemeObjectsNames()
        {
            try
            {
                IScheme scheme = CheckConnection();
                List<string> names = new List<string>();
                foreach (IClassifier cls in scheme.Classifiers.Values)
                {
                    names.Add(cls.ObjectOldKeyName);
                }
                foreach (IFactTable fact in scheme.FactTables.Values)
                {
                    names.Add(fact.ObjectOldKeyName);
                }
                // создаем массив строк под имена
                //string[] names = new string[scheme.Classifiers.Count + scheme.FactTables.Count];
                // переносим в массив имена классификаторов и таблиц фактов
                //scheme.Classifiers.Keys.CopyTo(names, 0);
                //scheme.FactTables.Keys.CopyTo(names, scheme.Classifiers.Count);
                // возвращаем имена объектов, перечисленные через запятую
                return String.Join(",", names.ToArray());
            }
            catch (Exception e)
            {
                return KristaDiagnostics.ExpandException(e);
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        private static XmlSerializer dataTableSerializer;
        private static object syncObj = new object();

        // поскольку веб сервис работает в многопоточном режиме,
        // сделаем потокобезопасный синглетон с двойной проверкой
        private static XmlSerializer DataTableSerializer
        {
            get
            {
                if (dataTableSerializer == null)
                {
                    lock (syncObj)
                    {
                        if (dataTableSerializer == null)
                        {
                            dataTableSerializer = new XmlSerializer(typeof(DataTable));
                        }
                    }
                }
                return dataTableSerializer;
            }
        }

        private string GetObjectDataStr(string objectName, string filter)
        {
            try
            {
                IScheme scheme = CheckConnection();
                bool isGuid = objectName.Contains("-");
                #region Поиск целевого объекта
                // ищем объект среди классификатором
                IEntity obj = null;
                if (isGuid)
                {
                    if (scheme.Classifiers.Keys.Contains(objectName))
                    {
                        obj = scheme.Classifiers[objectName];
                    }
                    // ищем объект среди таблиц фактов
                    if (obj == null)
                    {
                        if (scheme.FactTables.Keys.Contains(objectName))
                        {
                            obj = scheme.FactTables[objectName];
                        }
                    }
                }
                else
                {
                    foreach (IClassifier classifier in scheme.Classifiers.Values)
                    {
                        if (classifier.ObjectOldKeyName.Equals(objectName, StringComparison.OrdinalIgnoreCase))
                        {
                            obj = classifier;
                            break;
                        }
                    }
                    if (obj == null)
                    {
                        foreach (IFactTable fackTable in scheme.FactTables.Values)
                        {
                            if (fackTable.ObjectOldKeyName.Equals(objectName, StringComparison.OrdinalIgnoreCase))
                            {
                                obj = fackTable;
                                break;
                            }
                        }
                    }
                }
                // если объект до сих пор не найден - исключение
                if (obj == null)
                {
                    throw new Exception(String.Format("Объект '{0}' не найден среди объектов схемы", objectName));
                }
                #endregion

                #region Получение данных объекта
                using (IDataUpdater upd = obj.GetDataUpdater(filter, null))
                {
                    DataTable dt = new DataTable(objectName);
                    upd.Fill(ref dt);
                    StringBuilder sb = new StringBuilder(1024);
                    using (StringWriter sw = new StringWriter(sb))
                    {
                        DataTableSerializer.Serialize(sw, dt);
                        sw.Flush();
                    }
                    return sb.ToString();
                }
                #endregion
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        [WebMethod(EnableSession = true)]
        public string GetObjectData(string objectName, string filter)
        {
            try
            {
                return GetObjectDataStr(objectName, filter);
            }
            catch (Exception e)
            {
                return KristaDiagnostics.ExpandException(e);
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        /// <summary>
        /// передаем содержимое рекордсета в виде строки
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string GetObjectRecordSet(string objectName, string filter)
        {
            ADODB.Recordset rs = null;
            string tmpFileName = string.Empty;
            try
            {
                string data = GetObjectDataStr(objectName, filter);
                //string localPath = ConfigurationManager.AppSettings["LocalPath"];
                // получаем уникальное имя для временного хранения данных рекордсета
                tmpFileName = Path.GetTempFileName();//localPath + Path.DirectorySeparatorChar + Path.GetRandomFileName();
                using (StringReader sr = new StringReader(data))
                {
                    // так как файл создается, а нужно только имя, удалим временный файл
                    DeleteFile(tmpFileName);
                    DataTable dt = (DataTable)DataTableSerializer.Deserialize(sr);
                    rs = DataTableConverter.ConvertToRecordset(ref dt);
                    // сохраняем данные во временный файл
                    rs.Save(tmpFileName, ADODB.PersistFormatEnum.adPersistXML);
                    // читаем данные из файла, передаем данные строкой
                    FileStream fs = new FileStream(tmpFileName, FileMode.Open, FileAccess.Read);
                    StreamReader reader = new StreamReader(fs, Encoding.GetEncoding(1251));
                    string xmlText = reader.ReadToEnd();
                    reader.Close();
                    fs.Close();
                    return xmlText;
                }
            }
            catch (Exception e)
            {
                return KristaDiagnostics.ExpandException(e);
            }
            finally
            {
                if (rs != null)
                {
                    rs.Close();
                    DeleteFile(tmpFileName);
                }
                CallContext.SetData("Authorization", null);
            }
        }

        /// <summary>
        /// удаляем файл по указанному пути
        /// </summary>
        /// <param name="fileName"></param>
        private void DeleteFile(string fileName)
        {
            FileInfo f = new FileInfo(fileName);
            if (f != null)
                f.Delete();
        }

        #endregion

        #region IPlaningProvider Members

        [WebMethod(EnableSession = true)]
        public string GetMetaDataDate2()
        {
            try
            {
                IScheme scheme = CheckConnection();
                return scheme.PlaningProvider.GetMetadataDate();
            }
            catch (Exception e)
            {
                return KristaDiagnostics.ExpandException(e);
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        [WebMethod(EnableSession = true)]
        public string GetMetaData2()
        {
            try
            {
                IScheme scheme = CheckConnection();
                return scheme.PlaningProvider.GetMetaData();
            }
            catch (Exception e)
            {
                return KristaDiagnostics.ExpandException(e);
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        [WebMethod(EnableSession = true)]
        public string GetMembers2(string providerId, string cubeName, string dimensionName, 
            string hierarchyName, string levelNames, string memberPropertiesNames)
        {
            try
            {
                IScheme scheme = CheckConnection();
                return scheme.PlaningProvider.GetMembers(providerId, cubeName, dimensionName, 
                    hierarchyName, levelNames, memberPropertiesNames);
            }
            catch (Exception e)
            {
                return KristaDiagnostics.ExpandException(e);
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        
        [WebMethod(EnableSession = true)]
        public string GetRecordsetData2(string providerId, string queryText)
        {
            try
            {
                IScheme scheme = CheckConnection();
                return scheme.PlaningProvider.GetRecordsetData(providerId, queryText);
            }
            catch (Exception e)
            {
                return KristaDiagnostics.ExpandException(e);
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        [WebMethod(EnableSession = true)]
        public string GetCellsetData2(string providerId, string queryText)
        {
            // ************************************************************************************
            // В рамках FMQ00006236 (OutOfMemory на очень больших выборках в сотни мегабайт)
            // Для таких случаев будем формировать не SOAP-ответ, а просто RAW-данные, помещенные в Response
            // Вызывающая сторона должна трактовать их соответсвенно.
            // ************************************************************************************
            try
            {
                // полчаем указатель на объект схемы
                IScheme scheme = CheckConnection();
                // просим сервер сформировать файл с данными
                string data = scheme.PlaningProvider.GetCellsetData(providerId, queryText);
                // создаем Reader
                StringReader sr = new StringReader(data);
                try
                {
                    // Для верности установим кодировку UTF8 (клиент понимает только ее)
                    this.Context.Response.ContentEncoding = Encoding.UTF8;

                    while (true)
                    {
                        // читаем данные построчно и пишем в Response
                        string s = sr.ReadLine();
                        if (s != null)
                        {
                            this.Context.Response.Output.WriteLine(s);
                        }
                        else
                        {
                            break;
                        }
                    }
                    // Закрываем Response (после этого стандартная реализация SOAP-ответа ничего туда записать не сможет)
                    this.Context.Response.End();
                }
                finally
                {
                    // закрываем Reader
                    sr.Close();
                }
                return String.Empty;
            }
            catch (Exception e)
            {
                // Если это не мы явно закрыли Response, а произошло другое исключение
                if (!(e is System.Threading.ThreadAbortException))
                    // возвращаем подробную информацию
                    return KristaDiagnostics.ExpandException(e);
                else
                    // иначе - ничего не возвращаем
                    return String.Empty;
            }
            finally
            {
                // сбрасываем контекст авторизации
                CallContext.SetData("Authorization", null);
            }
        }


        [WebMethod(EnableSession = true)]
        public string ClientSessionIsAlive()
        {
            try
            {
                IScheme scheme = CheckConnection();
                return (scheme != null).ToString();
            }
            catch (Exception e)
            {
                return KristaDiagnostics.ExpandException(e);
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        [WebMethod(EnableSession = true)]
        public string UpdateTaskConsts(int taskId, string constsText, string sectionDivider, string valuesDivider)
        {
            try
            {
                IScheme scheme = CheckConnection();
                return scheme.PlaningProvider.UpdateTaskConsts(taskId, constsText, sectionDivider, valuesDivider);
            }
            catch (Exception e)
            {
                return KristaDiagnostics.ExpandException(e);
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        [WebMethod(EnableSession = true)]
        public string UpdateTaskParams(int taskId, string paramsText, string sectionDivider, string valuesDivider)
        {
            try
            {
                IScheme scheme = CheckConnection();
                return scheme.PlaningProvider.UpdateTaskParams(taskId, paramsText, sectionDivider, valuesDivider);                
            }
            catch (Exception e)
            {
                return KristaDiagnostics.ExpandException(e);
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        [WebMethod(EnableSession = true)]
        public string GetTaskContext(int taskId)
        {
            try
            {
                IScheme scheme = CheckConnection();
                return scheme.PlaningProvider.GetTaskContext(taskId);
            }
            catch (Exception e)
            {
                return KristaDiagnostics.ExpandException(e);
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }
        #endregion

        #region IWriteBackServer Members

        [WebMethod(EnableSession = true)]
        public string WriteBack2(string data)
        {
            try
            {
                IScheme scheme = CheckConnection();
                IWriteBackServer writeBackServer = scheme.WriteBackServer;
                return writeBackServer.ProcessQuery(data);
            }
            catch (Exception e)
            {
                return KristaDiagnostics.ExpandException(e);
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        [WebMethod(EnableSession = true)]
        public string GetBatchState(string batchGuid)
        {
            try
            {
                IScheme scheme = CheckConnection();
                Guid guid = new Guid(batchGuid);
                return Convert.ToString(scheme.Processor.GetBatchState(guid));
            }
            catch (Exception e)
            {
                return KristaDiagnostics.ExpandException(e);
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        #endregion IWriteBackServer Members

        #region Test

        [WebMethod(EnableSession = true)]
        public System.Data.DataTable Entities(ClassTypes classType)
        {
            try
            {
                IScheme scheme = CheckConnection();
                switch (classType)
                {
                    case ClassTypes.clsDataClassifier:
                        return scheme.Classifiers.GetDataTable();
                    case ClassTypes.clsFactData:
                        return scheme.FactTables.GetDataTable();
                }
                return null;
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        [WebMethod(EnableSession = true)]
        public System.Data.DataTable BridgeAssociations()
        {
            try
            {
                IScheme scheme = CheckConnection();
                return scheme.Associations.GetDataTable();
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        [WebMethod(EnableSession = true)]
        public System.Data.DataTable ConversionTables()
        {
            try
            {
                IScheme scheme = CheckConnection();
                return scheme.ConversionTables.GetDataTable();
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        [WebMethod(EnableSession = true)]
        public System.Data.DataTable EntityAttibutes(string entityFullName)
        {
            try
            {
                IScheme scheme = CheckConnection();
                return scheme.Classifiers[entityFullName].Attributes.GetDataTable();
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        #endregion

        #region методы для ХМАО
        
        /// <summary>
        /// Метод подключения к схеме.
        /// </summary>
        /// <param name="withinTaskContext">Режим работы в контексте задач </param>
        /// <param name="authType">Тип аутентификации.</param>
        /// <param name="login">Имя пользователя или доменное имя (в зависимости от режима).</param>
        /// <param name="pwd">Пароль (не хэш!) для режима логин/пароль.</param>
        /// <param name="errStr">Строка с расшифровкой ошибки.</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public bool ConnectToScheme(string login, string pwd, ref string errStr)
        {
            try
            {
                if (Session == null)
                {
                    throw new Exception("Ошибка доступа к объекту Http-сессии");
                }
                // есть ли в сессии интерфейс сервера?
                IServer server = Session[SERVER_KEY_NAME] as IServer;
                if (server == null)
                {
                    server = (IServer)Activator.GetObject(typeof(IServer), String.Format(_URL, _serverName));
                    Session[SERVER_KEY_NAME] = server;
                }
                else
                {
                    // проверим действительна ли закэшированная ссылка на сервер
                    try
                    {
                        int portNum = Convert.ToInt32(server.GetConfigurationParameter("ServerPort"));
                    }
                    catch
                    {
                        // если нет - убиваем сессию, просим пользователя повторить операцию
                        Session.Abandon();
                        throw new Exception(
                            "Ссылка на экземпляр сервера более не действительна. Выполнен перезапуск пользовательской сессии. Попробуйте повторить запрос еще раз");
                    }
                }

                // был ли запомнен контекст вызова?
                LogicalCallContextData cnt = Session[LOGICAL_CALL_CONTEXT_DATA_KEY_NAME] as LogicalCallContextData;
                if (cnt != null)
                    LogicalCallContextData.SetContext(cnt);
                else
                    LogicalCallContextData.SetAuthorization();

                // есть ли в сессии интерфейс схемы? если запрошенная схема есть в сессии - возвращаем ее
                IScheme scheme = Session[SCHEME_KEY_NAME] as IScheme;
                if (scheme != null)
                {
                    // TODO: Отключиться от схемы
                    return true;
                }

                // иначе - выполняем подключение
                ClientSession clientSession = ClientSession.CreateSession(SessionClientType.WebService);
                // запоминаем ее в переменной сессии
                Session[CLIENT_SESSION_KEY_NAME] = clientSession;

                pwd = PwdHelper.GetPasswordHash(pwd);

                if (!server.Connect(out scheme, AuthenticationType.adPwdSHA512, login, pwd, ref errStr))
                    throw new Exception(String.Format("Ошибка при подключении к схеме: {0}", errStr));

                // запоминаем контекст вызова (для установки в последующих вызовах методов в сессии)
                Session[LOGICAL_CALL_CONTEXT_DATA_KEY_NAME] = LogicalCallContextData.GetContext();
                // инициируем запуск процесса нотификации сервера о живости сессии
                clientSession.SessionManager = scheme.SessionManager;
                // устанавливаем сессии такой же таймаут как максимальная задрежка в отклике клиентской сессии
                Session.Timeout = scheme.SessionManager.MaxClientResponseDelay.Minutes;

                // запоминаем схему
                Session[SCHEME_KEY_NAME] = scheme;

                return true;
            }
            catch (Exception e)
            {
                errStr = e.Message;
                return false;
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        [WebMethod(EnableSession = true)]
        [return: XmlArray()]
        public ReportByMonth[] GetReportByMonth(QueryByMonth queryByMonth)
        {
            try
            {
                QueryByMonth queryMonth = queryByMonth;
                if (queryByMonth == null)
                    return null;
                IScheme scheme = CheckConnection();
                ReportsDataService reportsService = new ReportsDataService();
                return reportsService.GetMonthReportsData(scheme, queryMonth);
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        [WebMethod(EnableSession = true)]
        public ReportByYear GetReportByYear(QueryByYear queryByYear)
        {
            try
            {
                IScheme scheme = CheckConnection();
                ReportsDataService reportsService = new ReportsDataService();
                return reportsService.GetYearReportData(scheme, queryByYear);
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        [WebMethod(EnableSession = true)]
        public ComparableClassifier[] GetComparableClassifiers()
        {
            object[] results = null;
            return ((ComparableClassifier[])(results[0]));
        }

        [WebMethod(EnableSession = true)]
        public DataClassifier[] GetDataClassifiers()
        {
            object[] results = null;
            return ((DataClassifier[])(results[0]));
        }

        [WebMethod(EnableSession = true)]
        public FinancialReport[] GetReports(int year)
        {
            object[] results = null;
            return ((FinancialReport[])(results[0]));
        }

        #endregion
    }
}