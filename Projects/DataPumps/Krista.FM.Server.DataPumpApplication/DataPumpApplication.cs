using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting;
using System.Threading;
using System.Windows.Forms;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumpApp
{
    /// <summary>
    /// Хост-класс для запуска процесса закачки данных
    /// </summary>`
    public class DataPumpApplication
    {
        #region Поля

        /// <summary>
        /// УРЛ сервера приложений
        /// </summary>
        private string serverURL;
        /// <summary>
        /// Имя схемы
        /// </summary>
        private string schemeName;
        /// <summary>
        /// Идентификатор программы закачки данных
        /// </summary>
        private string programIdentifier;
        /// <summary>
        /// Ссылка на схему
        /// </summary>
        private IScheme scheme = null;
        /// <summary>
        /// Класс модуля процесса закачки
        /// </summary>
        private DataPumpModuleBase dataPumpModule;
        /// <summary>
        /// Стартовое состояние закачки
        /// </summary>
        private string startState;
        private int pumpID = -1;
        private int sourceID = -1;
        private string userParams = string.Empty;

        #endregion Поля


        /// <summary>
        /// Хост-класс для запуска процесса закачки данных
        /// </summary>
        /// <param name="serverURL">УРЛ сервера приложений</param>
        /// <param name="schemeName">Имя схемы</param>
        /// <param name="programIdentifier">Идентификатор программы звкачки данных</param>
        /// <param name="startState">Стартовое состояние закачки</param>
        public DataPumpApplication(string serverURL, string schemeName, string programIdentifier,
            string startState, string userParams)
        {
            this.serverURL = serverURL;
            this.schemeName = schemeName;
            this.programIdentifier = programIdentifier;
            this.startState = startState;
            this.userParams = userParams;

            //Application.Idle += this.Idle;
            Application.ApplicationExit += this.ApplicationExit;
        }

        /// <summary>
        /// Хост-класс для запуска процесса закачки данных
        /// </summary>
        /// <param name="serverURL">УРЛ сервера приложений</param>
        /// <param name="schemeName">Имя схемы</param>
        /// <param name="programIdentifier">Идентификатор программы звкачки данных</param>
        /// <param name="startState">Стартовое состояние закачки</param>
        /// <param name="pumpID">ИД закачки (для режима удаления)</param>
        /// <param name="sourceID">ИД источника (для режима удаления)</param>
        public DataPumpApplication(string serverURL, string schemeName, string programIdentifier,
            string startState, string pumpID, string sourceID)
        {
            if (!String.IsNullOrEmpty(pumpID))
            {
                this.pumpID = Convert.ToInt32(pumpID);
            }

            if (!String.IsNullOrEmpty(sourceID))
            {
                this.sourceID = Convert.ToInt32(sourceID);
            }
            this.serverURL = serverURL;
            this.schemeName = schemeName;
            this.programIdentifier = programIdentifier;
            this.startState = startState;

            Application.ApplicationExit += this.ApplicationExit;
        }

        /// <summary>
        /// Подключение к серверу
        /// </summary>
        /// <param name="url">Полный адрес объекта сервера</param>
        /// <param name="userName">Полное имя закачки данных</param>
        private static IServer ConnectToServer(string url, string userName)
        {
            if (userName == String.Empty)
            {
                LogicalCallContextData.SetAuthorization();
            }
            else
            {
                LogicalCallContextData.SetAuthorization(userName);
            }

            // Создаем прозрачный прокси объект для взаимодействия с сервером
            IServer proxy = (IServer)Activator.GetObject(typeof(IServer), url);

            try
            {
                // Делаем первое обращение к удаленному серверу для проверки соединения(доступности сервера)
                proxy.Activate();
            }
            catch
            {
                return null;
            }

            return proxy;
        }

        /// <summary>
        /// Подключение к серверу и схеме, создание клиентской сессии
        /// </summary>
        private void Connect()
        {
            //Trace.WriteLine("подключение к серверу" + serverURL, "DataPumpApplication");
            IServer server = ConnectToServer(serverURL, programIdentifier);
            if (server == null)
            {
                throw new Exception("Невозможно подключиться к серверу");
            }

            ClientSession.CreateSession(SessionClientType.DataPump);

            //Trace.WriteLine("подключение к сессии", "DataPumpApplication");
            string errStr = String.Empty;
            server.Connect(schemeName, out scheme, AuthenticationType.atUndefined, String.Empty, String.Empty, ref errStr);

            if (scheme == null)
            {
                throw new Krista.FM.Common.ServerException("Ошибка при подключении к схеме");
            }

        	SessionManager.SetInstanceSessionManagerForDataPump(scheme.SessionManager);
        }

        /// <summary>
        /// Уничтожение сессии, освобождение ресурсов
        /// </summary>
        private void Disconnect()
        {
            // явно уничтожаем сессию
            LogicalCallContextData lccd = LogicalCallContextData.GetContext();
            if ((lccd != null) && (lccd["Session"] != null))
            {
                ISession session = lccd["Session"] as ISession;
                session.Dispose();
            }
        }

        /// <summary>
        /// Проверка соответствия версий сборок базовой
        /// </summary>
        private void CheckAssemblyVersions()
        {
            /*
            Dictionary<string, string> badAssemblies = new Dictionary<string, string>();
            Dictionary<string, string> versions = AppVersionControl.GetAssemblyesVersions("Krista.FM.Server.*.dll");
            string baseVersion = AppVersionControl.GetAssemblyBaseVersion(AppVersionControl.GetServerLibraryVersion());

            int maxLength = 0;
            foreach (string itemKey in versions.Keys)
            {
                if (itemKey.Length > maxLength)
                    maxLength = itemKey.Length;
            }

            string formatString = String.Format("{{0,-{0}}} {{1}} {{2}}", maxLength);
            foreach (KeyValuePair<string, string> item in versions)
            {
                string baseAssemblyVesion = AppVersionControl.GetAssemblyBaseVersion(item.Value);
                string badSign = String.Empty;
                
                if (baseAssemblyVesion != baseVersion)
                {
                    badAssemblies.Add(item.Key, item.Value);
                    badSign = "Версия не соответствует базовой";
                }

                Trace.WriteLine(string.Format(formatString, item.Key, item.Value, badSign));
            }

            if (badAssemblies.Count > 0)
            {
                throw new Exception(string.Format("Обнаружено {0} сборок с версией, отличающейся от базовой.", badAssemblies.Count));
            }
            */
        }

        /// <summary>
        /// Загрузка сборки программы закачки
        /// </summary>
        private void LoadModule()
        {
            // согласно FMQ00005987 формат programIdentifier таков:
            //
            //  <имя модуля>.<идентификатор параметров>
            //
            string programName = programIdentifier;
            int dotIndex = programName.IndexOf('.');
            if (dotIndex == 0)
                throw new Exception("Идентификатор программы закачки не может начинаться с символа '.'");

            bool paramsPresent = dotIndex > 0;
            if (paramsPresent)
            {
                programName = programName.Substring(0, dotIndex);
            }

            string fullTypeName = string.Format("Krista.FM.Server.DataPumps.{0}.{0}Module", programName);
            string assemblyName = string.Format("Krista.FM.Server.DataPumps.{0}", programName);

            Trace.WriteLine(string.Format("Загрузка сборки {0}", assemblyName), "DataPumpApplication");
            // создаем новый домен и загружаем в него модуль закачки данных
            Assembly assembly = AppDomain.CurrentDomain.Load(assemblyName, typeof(DataPumpApplication).Assembly.Evidence);

            Trace.WriteLine(string.Format("Получение экземпляра класса {0}", fullTypeName), "DataPumpApplication");
            dataPumpModule = assembly.CreateInstance(fullTypeName, true) as DataPumpModuleBase;
            IDataPumpModule dpm = dataPumpModule as IDataPumpModule;

            Trace.WriteLine("Проверка версий сборок...", "DataPumpApplication");
            dataPumpModule.SystemVersion = AppVersionControl.GetAssemblyBaseVersion(AppVersionControl.GetServerLibraryVersion());
            dataPumpModule.ProgramVersion = AppVersionControl.GetAssemblyVersion(assembly);
  
            dataPumpModule.Initialize(this.scheme, programIdentifier, this.userParams);
        }

        /// <summary>
        /// Начинает выполнение процесса закачки
        /// </summary>
        public void Run()
        {
            try
            {
                // Проверка соответствия версий сборок
                CheckAssemblyVersions();

                Trace.WriteLine("Настройка среды .NET Remoting...", "DataPumpApplication");
                RemotingConfiguration.Configure(AppDomain.CurrentDomain.BaseDirectory +
                    GetType().Assembly.ManifestModule.Name + ".config", false);

                Trace.WriteLine("Подключение к серверу...", "DataPumpApplication");
                Connect();

                Trace.WriteLine("Создание домена и загрузка в него модуля закачки данных...", "DataPumpApplication");
                LoadModule();

                Mutex mutex = new Mutex(false, dataPumpModule.GetCurrentPumpMutexName());

                Trace.WriteLine("Запуск программы закачки данных...", "DataPumpApplication");

                switch (this.startState.ToUpper())
                {
                    case "DELETEDATA": dataPumpModule.DeleteData(pumpID, sourceID);
                        break;

                    default:
                        dataPumpModule.StartThread();
                        dataPumpModule.State = 
                            this.scheme.DataPumpManager.DataPumpInfo.StringToPumpProcessStates(this.startState);
                        break;
                }

                Trace.WriteLine("Программа закачки данных запущена.", "DataPumpApplication");

                System.Threading.Thread.Sleep(5000);
                mutex.WaitOne();

                Trace.WriteLine("Завершение работы процесса программы закачки данных...", "DataPumpApplication");
                Disconnect();
                Application.Exit();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("CRITICAL ERROR: " + ex.ToString(), "DataPumpApplication");

                try
                {
                    try
                    {
                        // Сообщаем серверу о сбое
                        if (this.scheme != null)
                        {
                            IServerSideDataPumpProgress ssd =
                                this.scheme.DataPumpManager.DataPumpInfo[this.programIdentifier] as IServerSideDataPumpProgress;
                            if (ssd != null)
                            {
                                ssd.OnPumpFailure(ex.ToString());
                            }
                        }
                    }
                    finally
                    {
                        Disconnect();
                        Application.Exit();
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Главный поток процесса закачки
        /// </summary>
        //public void Idle(Object sender, EventArgs e)
        //{
            /*if (dataPumpModule == null)
            {
                Run();
            }
            else
            {
                Trace.WriteLine("Idle");
            }*/
        //}

        /// <summary>
        /// 
        /// </summary>
        public void ApplicationExit(Object sender, EventArgs e)
        {
            //Application.Idle -= this.Idle;
            Application.ApplicationExit -= this.ApplicationExit;

            try
            {
                if (dataPumpModule != null)
                {
                    Trace.WriteLine("Освобождение мьютекса...", "DataPumpApplication");
                    Mutex mutex = Mutex.OpenExisting(dataPumpModule.GetCurrentPumpMutexName());
                    if (mutex != null) mutex.ReleaseMutex();
                }
            }
            catch { }
        }
    }
}
