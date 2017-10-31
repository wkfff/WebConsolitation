using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.PlaningProviderCOMWrapper
{
    [ClassInterface(ClassInterfaceType.None), ComVisible(true), Guid("0D590FFF-4333-4781-8896-1B023F05F5CE")]
    public class PlaningProviderComWrapper : IPlaningProviderCOMWrapper
    {
        #region приватные поля и методы
        private IScheme scheme;
        
        private readonly string startLocation;

        // сериализатор для объектов DataTable
        private static XmlSerializer dataTableSerializer;

        // объект-синхронизатор для потокобезопасного доступа к сериализатору
        private static object syncObj = new object();

        private void CheckConnection()
        {
            if (!Connected)
                throw new Exception("Подключение не установлено");
        }

        private static string NullStringToEmpty(string s)
        {
            if (s == null)
                return string.Empty;
            return s;
        }

        private static string MakeServerName(string inStr)
        {
            inStr = inStr.Trim();
            try
            {
                // Отрезаем имя сервера.
                string[] serverName = inStr.Split(':');
                // Отрезаем номер порта.
                string[] portNumber = serverName[1].Split();
                // Проверям, является ли порт числом
                bool digit = true;
                foreach (char item in portNumber[0])
                {
                    if (!char.IsDigit(item))
                    {
                        digit = false;
                    }
                }
                // Если порт число, то возвращаем сервер с портом.
                if (digit && portNumber[0].Length <= 4 && !String.IsNullOrEmpty(portNumber[0]) && !serverName[0].Contains(" "))
                {
                    return serverName[0] + ":" + portNumber[0];
                }
                else // Если порт некорректный, меняем лейбл и возвращаем пустую строку.
                {
                    return String.Empty;
                }
            }
            catch (IndexOutOfRangeException) // Если попали сюда, то номер порта не введен.
            {
                string[] serverName = inStr.Split();
                // Возвращаем только имя сервера.
                return serverName[0];
            }
        }

        private Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("Krista.FM.Common"))
            {
                Assembly ass = Assembly.LoadFrom(startLocation + "\\Krista.FM.Common.dll");
                return ass;
            }
            if (args.Name.StartsWith("Krista.FM.ServerLibrary"))
            {
                Assembly ass = Assembly.LoadFrom(startLocation + "\\Krista.FM.ServerLibrary.dll");
                return ass;
            }
            if (args.Name.StartsWith("Krista.Diagnostics"))
            {
                Assembly ass = Assembly.LoadFrom(startLocation + "\\Krista.Diagnostics.dll");
                return ass;
            }
            return null;
        }

        /// <summary>
        /// Cериализатор для объектов DataTable
        /// </summary>
        private static XmlSerializer DataTableSerializer
        {
            get
            {
                // синглетон с двойной проверкой
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

        private string GetObjectData(string objectName, string filter)
        {
            CheckConnection();
            try
            {
                bool isGuid = objectName.Contains("-");
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

                // Получение данных объекта
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
            }
            catch (Exception e)
            {
                return Krista.Diagnostics.KristaDiagnostics.ExpandException(e);
            }
        }

        #endregion

        public PlaningProviderComWrapper()
        {
            startLocation = Path.GetDirectoryName(GetType().Assembly.Location);
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;         
        }

        #region IPlaningProviderCOMWrapper members

        /// <summary>
        /// подключение к серверу
        /// </summary>
        /// <param name="serverNameNPort"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="authType"></param>
        /// <param name="withinTaskContext"></param>
        /// <param name="errStr"></param>
        /// <returns></returns>
        public bool Connect(string serverNameNPort, string userName, string password,
                            int authType, bool withinTaskContext, ref string errStr)
        {
            serverNameNPort = NullStringToEmpty(serverNameNPort);
            userName = NullStringToEmpty(userName);
            password = NullStringToEmpty(password);

            if (!Connected)
            {
                string url = string.Format("tcp://{0}/FMServer/Server.rem", MakeServerName(serverNameNPort));
                IServer server = (IServer)Activator.GetObject(typeof(IServer), url);
                try
                {
                    // Делаем первое обращение к удаленному серверу для проверки соединения (доступности сервера)
                    server.Activate();
                }
                catch (Exception e)
                {
                    errStr = Diagnostics.KristaDiagnostics.ExpandException(e);
                    errStr = string.Format("Ошибка при обращении к объекту сервера: {0}", errStr);
                    return false;
                }

                // авторизуемся и запомним контекст вызова
                AuthenticationType _authType = (AuthenticationType)authType;
                LogicalCallContextData.SetAuthorization();
                LogicalCallContextData context = LogicalCallContextData.GetContext();

                if (_authType == AuthenticationType.atWindows &&
                    context.Principal.Identity.IsAuthenticated)
                {
                    userName = context.Principal.Identity.Name;
                }
                else
                {
                    if (!withinTaskContext)
                        password = PwdHelper.GetPasswordHash(password);
                }


                ClientSession clientSession = ClientSession.CreateSession(SessionClientType.WindowsNetClient);
                // подключаемся
                try
                {
                    if (!server.Connect(out scheme, _authType, userName, password, ref errStr))
                    {
                        errStr = String.Format("Ошибка при подключении к схеме: {0}", errStr);
                        context = null;
                        scheme = null;
                        return false;
                    }

                    try
                    {
                        UpdateFrameworkLibraryFactory.SetPropertyValue("Scheme", scheme);
                        UpdateFrameworkLibraryFactory.InvokeMethod("InitializeNotifyIconForm");
                    }
                    catch { }
                }
                catch (Exception err)
                {
                    errStr = Diagnostics.KristaDiagnostics.ExpandException(err);
                }

                clientSession.SessionManager = scheme.SessionManager;
                context = LogicalCallContextData.GetContext();
            }

            return Connected;
        }

        /// <summary>
        /// отключение от сервера
        /// </summary>
        public void Disconnect()
        {
            if (scheme != null)
                if (scheme.Server != null)
                    scheme.Server.Disconnect();
            try
            {
                ClientSession clientSession = ClientSession.Instance;
                if (clientSession != null)
                    clientSession.Dispose();
            }
            catch (Exception) { }

            scheme = null;
        }

        /// <summary>
        /// признак действительного подключения
        /// </summary>
        public bool Connected
        {
            get
            {
                if (scheme == null)
                    return false;
                string schemeName;
                try
                {
                    schemeName = scheme.Name;
                }
                catch (Exception)
                {
                    return false;
                }
                return schemeName != string.Empty;
            }
        }

        public string GetSchemeName(string providerId)
        {
            CheckConnection();
            return scheme.Name;
        }


        public string Writeback(string data)
        {
            try
            {
                return scheme.WriteBackServer.ProcessQuery(data);
            }
            catch (Exception e)
            {
                return Diagnostics.KristaDiagnostics.ExpandException(e);
            }

        }

        public object GetObjectRecordset(string objectName, string filter)
        {
            string data = GetObjectData(objectName, filter);
            object rs = null;
            using (StringReader sr = new StringReader(data))
            {
                DataTable dt = (DataTable)DataTableSerializer.Deserialize(sr);
                rs = DataTableConverter.ConvertToRecordset(ref dt);
            }
            return rs;
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            Disconnect();
            UpdateFrameworkLibraryFactory.InvokeMethod("Dispose");
        }

        #endregion

        #region IPlaningProvider Members

        public string GetMetadataDate()
        {
            CheckConnection();
            try
            {
                return scheme.PlaningProvider.GetMetadataDate();
            }
            catch (Exception e)
            {
                return Diagnostics.KristaDiagnostics.ExpandException(e);
            }
        }

        public string GetMetaData()
        {
            CheckConnection();
            try
            {
                return scheme.PlaningProvider.GetMetaData();
            }
            catch (Exception e)
            {
                return Diagnostics.KristaDiagnostics.ExpandException(e);
            }
        }

        public string GetMembers(string providerId, string cubeName, string dimensionName, string hierarchyName, string levelNames, string memberPropertiesNames)
        {
            cubeName = NullStringToEmpty(cubeName);
            dimensionName = NullStringToEmpty(dimensionName);
            hierarchyName = NullStringToEmpty(hierarchyName);
            levelNames = NullStringToEmpty(levelNames);
            memberPropertiesNames = NullStringToEmpty(memberPropertiesNames);

            CheckConnection();
            try
            {
                return scheme.PlaningProvider.GetMembers(providerId, cubeName, dimensionName, hierarchyName,
                                                         levelNames, memberPropertiesNames);
            }
            catch (Exception e)
            {
                return Diagnostics.KristaDiagnostics.ExpandException(e);
            }
        }

        public string GetRecordsetData(string providerId, string queryText)
        {
            queryText = NullStringToEmpty(queryText);

            CheckConnection();
            try
            {
                return scheme.PlaningProvider.GetRecordsetData(providerId, queryText);
            }
            catch(Exception e)
            {
                return Diagnostics.KristaDiagnostics.ExpandException(e);
            }
        }

        public string GetCellsetData(string providerId, string queryText)
        {
            queryText = NullStringToEmpty(queryText);

            CheckConnection();
            try
            {
                return scheme.PlaningProvider.GetCellsetData(providerId, queryText);
            }
            catch(Exception e)
            {
                return Diagnostics.KristaDiagnostics.ExpandException(e);
            }
        }

        public void RefreshDimension(string providerId, string[] names)
        {
            CheckConnection();
            scheme.PlaningProvider.RefreshDimension(providerId, names);
        }

        public void RefreshCube(string providerId, string[] names)
        {
            CheckConnection();
            scheme.PlaningProvider.RefreshCube(providerId, names);
        }

        public string RefreshMetaData()
        {
            CheckConnection();
            try
            {
                return scheme.PlaningProvider.RefreshMetaData();
            }
            catch (Exception e)
            {
                return Diagnostics.KristaDiagnostics.ExpandException(e);
            }
        }
        
        public string UpdateTaskParams(int taskId, string paramsText, string sectionDivider, string valuesDivider)
        {
            try
            {
                //CheckConnection();
                return scheme.PlaningProvider.UpdateTaskParams(taskId, paramsText, sectionDivider, valuesDivider);
            }
            catch (Exception e)
            {

                return Diagnostics.KristaDiagnostics.ExpandException(e);
            }
        }

        public string UpdateTaskConsts(int taskId, string constsText, string sectionDivider, string valuesDivider)
        {
            try
            {
                return scheme.PlaningProvider.UpdateTaskConsts(taskId, constsText, sectionDivider, valuesDivider);
            }
            catch (Exception e)
            {
                return Diagnostics.KristaDiagnostics.ExpandException(e);
            }
        }

        public string GetTaskContext(int taskId)
        {
            try
            {
                CheckConnection();
                return scheme.PlaningProvider.GetTaskContext(taskId);
            }
            catch (Exception e)
            {

                return Diagnostics.KristaDiagnostics.ExpandException(e);
            }
        }

        #endregion
        
    }

}
