using System;
using System.IO;
using System.Threading;
using System.Xml;

using Krista.FM.Client.MobileReports.Common;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.MobileReports.Core
{
    public class SnapshotServer : DisposableObject, ISnapshotService
    {
        private MobileReportsCore helper;
        private Thread th;
        private IScheme scheme;

        private BootloaderState bootloaderState;
        
        #region IDisposable Members

        public override void Close()
        {
            AbortWorking();
            base.Close();
        }

        #endregion

        #region ISnapshotService Members

        public BootloaderState GetState()
        {
            return bootloaderState;
        }
        
        public void DoSnapshot(SnapshotStartParams startParams)
        {
            Trace.TraceVerbose("Запрос на генерацию отчетов");
            AbortWorking();
            th = new Thread(new ParameterizedThreadStart(WorkingProccess));
            th.Start(startParams);
        }

        void helper_ChangeUpdateState(object sender, UpdateState state)
        {
            bootloaderState.UpdateState = state;
            bootloaderState.PercentDone = 0;
        }

        private void helper_ErrorEvent(object sender, ErrorEventArgs e)
        {
            if (!(e.GetException() is ThreadAbortException))
            {
                bootloaderState.LastError = e.GetException().Message + Environment.NewLine + e.GetException().StackTrace;
                AbortWorking();
            }
        }

        private void helper_CurrentStateProgress(object sender, int percentDone)
        {
            bootloaderState.PercentDone = percentDone;
        }
        
        public void Activate()
        {}

        #endregion

        private void AbortWorking()
        {
            bootloaderState = new BootloaderState();
            if (th != null)
            {
                th.Abort();
                th = null;
            }
        }

        private void WorkingProccess(object startParams)
        {
            try
            {
                SnapshotStartParams snapshotStartParams = (SnapshotStartParams)startParams;
                String serverURL = snapshotStartParams.ServerURL;
                Trace.TraceVerbose("Подключаемся к {0}", serverURL);
                Connect(serverURL);
                Trace.TraceVerbose("Подключились");
                //загружаем xml со стартовыми параметрами
                XmlDocument document = new XmlDocument();
                document.LoadXml(snapshotStartParams.StartParams);
                //создаем помошника обновления отчетов
                helper = new MobileReportsCore(scheme, snapshotStartParams.ReportsHostAddress);

                helper.ChangeUpdateState += new MobileReportsCore.ChangeUpdateStateHandler(helper_ChangeUpdateState);
                helper.CurrentStateProgress += new MobileReportsCore.CurrentStateProgressHandler(helper_CurrentStateProgress);
                helper.ErrorEvent += new ErrorEventHandler(helper_ErrorEvent);
                //начинаем процесс обновления
                Trace.TraceVerbose("Начинаем процесс обновления");
                helper.DeployData(document.SelectSingleNode(@"//deployingReports"));
            }
            catch (Exception e)
            {
                // Если не обработали в хелпере.
                if (bootloaderState.LastError == String.Empty)
                {
                    bootloaderState.LastError = e.Message + Environment.NewLine + e.StackTrace;
                    Trace.TraceVerbose(e.Message + Environment.NewLine + e.StackTrace);
                }
            }
            finally
            {
                //отключаемся от схемы
                Trace.TraceVerbose("Пытаемся отключиться");
                Disconnect();
            }
        }

        #region Подключение к схеме
        
        /// <summary>
        /// Подключение к серверу и схеме, создание клиентской сессии
        /// </summary>
        private void Connect(string serverURL)
        {
            LogicalCallContextData.SetAuthorization();
            
            IServer server = (IServer)Activator.GetObject(typeof(IServer), serverURL);
            ClientSession.CreateSession(SessionClientType.WindowsNetClient);
            string errStr = String.Empty;
            if (!server.Connect(out scheme, AuthenticationType.atUndefined, String.Empty, String.Empty, ref errStr))
            {
                throw new ServerException(String.Format("Ошибка при подключении к схеме: {0}", errStr));
            }
            LogicalCallContextData lccd = LogicalCallContextData.GetContext();
            // Прикинемся закачкой.
            lccd["UserID"] = 3;
        }

        /// <summary>
        /// Уничтожение сессии, освобождение ресурсов
        /// </summary>
        private void Disconnect()
        {
            // явно уничтожаем сессию
            if (scheme != null)
            {
                LogicalCallContextData lccd = LogicalCallContextData.GetContext();
                if ((lccd != null) && (lccd["SessionID"] != null))
                {
                    string sessionID = (string) lccd["SessionID"];
                    if (scheme.SessionManager.Sessions.ContainsKey(sessionID))
                    {
                        scheme.SessionManager.Sessions[sessionID].Dispose();
                        Trace.TraceVerbose("Отключаемся от схемы");
                    }
                }
            }
        }
        #endregion
    }
}
