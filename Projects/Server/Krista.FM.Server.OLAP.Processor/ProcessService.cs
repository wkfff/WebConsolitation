using System;
using System.Data;
using System.Threading;

using Krista.FM.Common;
using Krista.FM.Server.Common;

using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.OLAP.Processor
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class ProcessService : DisposableObject
    {
        private static volatile ProcessService instance;
        private static readonly object syncRoot = new object();

        /// <summary>
        /// Основной поток сервера, обслуживающий очередь запросов
        /// </summary>
        private static Thread serverThread;

        private ProcessorClass processor;
        private static bool paused;

        internal static AutoResetEvent newRequestEvent = new AutoResetEvent(true);
        internal static ManualResetEvent stopServiceEvent = new ManualResetEvent(false);
        internal static AutoResetEvent pauseEvent = new AutoResetEvent(true);

        public ProcessService(ProcessorClass _processor)
        {
            processor = _processor;
            serverThread = new Thread(new ParameterizedThreadStart(ProcessServiceThreadProcedure));
            serverThread.Priority = ThreadPriority.Lowest;

            // Если в режиме мульти-сервер, то ничего не делаем
            if (!processor.MultiServerMode)
            {
                serverThread.Start(LogicalCallContextData.GetContext());
            }
        }

        //[System.Diagnostics.DebuggerStepThrough()]
        public static ProcessService GetInstance(ProcessorClass _processor)
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new ProcessService(_processor);
                    }
                }
            }
            return instance;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                stopServiceEvent.Set();
            }
            base.Dispose(disposing);
        }

        internal static bool Paused
        {
            get { return paused; }
            set
            {
                lock (syncRoot)
                {
                    if (paused != value)
                    {
                        paused = value;
                        newRequestEvent.Set();

                        Trace.TraceVerbose(
                            "{0} Диспетчер обработки запросов {1}",
                            Authentication.UserDate,
                            paused ? "остановлен" : "запущен");
                    }
                }
            }
        }

        private static void CreateSession()
        {
            LogicalCallContextData.SetAuthorization("ProcessService");
            ClientSession.CreateSession(SessionClientType.Server);
            SessionManager.Instance.Create(LogicalCallContextData.GetContext());
            LogicalCallContextData.GetContext()["UserID"] = (int)FixedUsers.FixedUsersIds.System;
            LogicalCallContextData.SetContext(LogicalCallContextData.GetContext());
        }

        private void ProcessServiceThreadProcedure(object args)
        {
            try
            {
                Thread.CurrentThread.Name = "ProcessService";

                CreateSession();

                //Debug.WriteLine("Writeback Server Started at " + DateTime.Now.ToString());
                WaitHandle[] wait_objects = new WaitHandle[2] {newRequestEvent, stopServiceEvent};

                while (WaitHandle.WaitAny(wait_objects) == 0)
                //Просигналил объект события обработки нового запроса или остановки сервера 
                {
                    if (!paused)
                    {
                        // Получаем пакет для обработки
                        DataRow[] rows = processor.OlapDBWrapper.BatchesView.Select(
                            String.Format("BatchState = {0}", (int) BatchState.Waiting),
                            "ID");

                        if (rows.GetLength(0) > 1)
                        {
                            newRequestEvent.Set();
                        }
                        if (rows.GetLength(0) > 0)
                        {
                            Guid firstBatch = new Guid(Convert.ToString(rows[0]["BATCHID"]));
                            BatchStartPriority priority = BatchStartPriority.Auto;

                            // Если есть пакеты с более высоким приоритетом, 
                            // то расчитываем их в первую очередь
                            foreach (DataRow row in rows)
                            {
                                if ((BatchStartPriority) row["Priority"] == BatchStartPriority.Immediately)
                                {
                                    firstBatch = new Guid(Convert.ToString(row["BATCHID"]));
                                    priority = BatchStartPriority.Immediately;
                                }
                            }

                            Trace.TraceVerbose(
                                "Запуск диспетчером обработки пакета \"{0}\". Всего пакетов в очереди: {1}", 
                                firstBatch, rows.GetLength(0) - 1);                        

                            // Запускаем пакет на расчет
                            try
                            {
                                ((ProcessManager) processor.ProcessManager).StartProcessBatch(firstBatch, priority);
                            }
                            catch (Exception e)
                            {
                                Trace.TraceCritical("Диспетчер обработки пакета: {0}", Krista.Diagnostics.KristaDiagnostics.ExpandException(e));
                            }
                        }
                        else
                        {
                            Trace.TraceVerbose("Нет пакетов в очереди на расчет.");
                        }
                    }
                    else
                    {
                        Trace.TraceVerbose("Диспетчер обработки запросов на расчет многомерных объектов не активен.");                        
                    }
                }
            }
            catch(Exception e)
            {
                Trace.TraceCritical("Процесс диспетчера обработки пакетов прерван из-за ошибки: {0}", Krista.Diagnostics.KristaDiagnostics.ExpandException(e));
            }
        }
    }
}
