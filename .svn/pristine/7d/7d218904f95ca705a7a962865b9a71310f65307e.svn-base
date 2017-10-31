using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

using Microsoft.AnalysisServices;

using Krista.FM.Common;

using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.OLAP.Processor.Deprecated
{
    /// <summary>
    /// Создает пакеты (BatchProcessor) и уничтожает их по мере завершения расчета.
    /// </summary>
    public sealed class ProcessManager2005 : DisposableObject, IProcessManager
    {
        private static volatile ProcessManager2005 instance;
        private static object syncRoot = new object();

        private string serverName;

        private IScheme scheme;
        private OlapDBWrapper olapDBWrapper;

        private Microsoft.AnalysisServices.Server server = new Microsoft.AnalysisServices.Server();
        private Microsoft.AnalysisServices.Trace trace;
        private TraceEventHandler traceEventHandler;
        private const string traceId = "FM_ProcessManager";

        private List<BatchProcessor> batchProcessorList = new List<BatchProcessor>();
        private BatchComplitedDelegate batchComplitedDelegate;

        public static ProcessManager2005 GetInstance(IScheme scheme, string serverName, bool replaceTrace)
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new ProcessManager2005(scheme, serverName, replaceTrace);
                    }
                }
            }
            return instance;
        }

        private ProcessManager2005(IScheme scheme, string serverName, bool replaceTrace)
        {
            this.scheme = scheme;
            this.olapDBWrapper = ((ProcessorClass)scheme.Processor).OlapDBWrapper;
            this.serverName = serverName;
            batchComplitedDelegate = new BatchComplitedDelegate(batchProcessor_BatchComplitedEvent);
            server.Connect(serverName);
            InitTrace(replaceTrace);
            StartTrace();
        }

        /// <summary>
        /// Обработчик сообщений SSAS2005. Перехватывет сообщения об окончании расчета и
        /// модифицирует соответствующим образом данные пакета.
        /// </summary>
        private static void OnTraceEventHandler(object sender, Microsoft.AnalysisServices.TraceEventArgs e)
        {
            try
            {
                string cubeId;
                string measureGroupId;


                switch (e.EventClass)
                {
                    case TraceEventClass.ProgressReportEnd:
                        //Trace.TraceVerbose("{0} - {1} - {2} - {3}", e.EventClass, e.EventSubclass, e.TextData, e.ObjectName);
                        if (e.EventSubclass == TraceEventSubclass.Process && !string.IsNullOrEmpty(e.ObjectID))
                        {
                            OlapObjectType objectType = TraceUtils.GetPartitionParents(e.ObjectPath, out cubeId, out measureGroupId);
                            //accumulatorManager.SetRecordStatus(objectType, cubeId, measureGroupId, e.ObjectID, RecordStatus.inProcess, RecordStatus.complitedSuccessful);
                        }
                        break;
                    case TraceEventClass.ProgressReportError:
                        //Trace.TraceVerbose("{0} - {1} - {2}", e.EventClass, e.EventSubclass, e.TextData);
                        if (e.EventSubclass == TraceEventSubclass.Process && !string.IsNullOrEmpty(e.ObjectID))
                        {
                            OlapObjectType objectType = TraceUtils.GetPartitionParents(e.ObjectPath, out cubeId, out measureGroupId);
                            //accumulatorManager.SetRecordStatus(objectType, cubeId, measureGroupId, e.ObjectID, RecordStatus.inProcess, RecordStatus.complitedWithErrors, e.Error);
                        }
                        break;
                    case TraceEventClass.CommandEnd:
                        //Trace.TraceVerbose("{0} - {1} - {2}", e.EventClass, e.EventSubclass, e.TextData);
                        if (e.EventSubclass == TraceEventSubclass.Batch)
                        {
                            //int batchId = accumulatorManager.ComplitBatch(e.SessionID);
                            //if (batchId > 0)
                            //{
                            //    accumulatorManager.DeleteBatch(batchId);
                            //}
                        }
                        break;
                    case TraceEventClass.ProgressReportCurrent:
                        //Trace.TraceVerbose("{0} - {1} - {2}", e.EventClass, e.EventSubclass, e.TextData);
                        if (e.EventSubclass == TraceEventSubclass.Batch)
                        {
                            //int batchId = accumulatorManager.ComplitBatch(e.SessionID);
                            //if (batchId > 0)
                            //{
                            //    accumulatorManager.DeleteBatch(batchId);
                            //}
                        }
                        break;
                    default:
                        //Trace.TraceVerbose("{0} - {1} - {2}", e.EventClass, e.EventSubclass, e.TextData);
                        break;
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Trace.WriteLine(exception.Message);
            }
        }

        /// <summary>
        /// Добавляет события и их колонки к трассировщику.
        /// </summary>
        /// <param name="eventClass"></param>
        private void AddTraceEvent(TraceEventClass eventClass)
        {
            if (!trace.Events.Contains(eventClass))
            {
                TraceEvent traceEvent = trace.Events.Add(eventClass);
                switch (eventClass)
                {
                    case TraceEventClass.CommandBegin:
                    case TraceEventClass.CommandEnd:
                        traceEvent.Columns.Add(TraceColumn.SessionID);
                        break;
                    case TraceEventClass.ProgressReportBegin:
                    case TraceEventClass.ProgressReportCurrent:
                    case TraceEventClass.ProgressReportEnd:
                    case TraceEventClass.ProgressReportError:
                        traceEvent.Columns.Add(TraceColumn.ObjectID);
                        traceEvent.Columns.Add(TraceColumn.ObjectName);
                        traceEvent.Columns.Add(TraceColumn.ObjectPath);
                        break;
                    default:
                        break;
                }
                traceEvent.Columns.Add(TraceColumn.EventClass);
                if (eventClass != TraceEventClass.Error)
                {
                    traceEvent.Columns.Add(TraceColumn.EventSubclass);
                }
                traceEvent.Columns.Add(TraceColumn.TextData);
            }
        }

        /// <summary>
        /// Создает трассировщик событий cервера SSAS2005.
        /// </summary>
        private void CreateTrace()
        {
            trace = new Microsoft.AnalysisServices.Trace("FM_ProcessManager", "FM_ProcessManager");
            server.Traces.Add(trace);
            //AddTraceEvent(TraceEventClass.CommandBegin);
            AddTraceEvent(TraceEventClass.CommandEnd);
            AddTraceEvent(TraceEventClass.ProgressReportBegin);
            AddTraceEvent(TraceEventClass.ProgressReportCurrent);
            AddTraceEvent(TraceEventClass.ProgressReportEnd);
            AddTraceEvent(TraceEventClass.ProgressReportError);
        }

        /// <summary>
        /// Подключаемся к существующему трассировщику или создем новый.
        /// </summary>
        /// <param name="replaceIfExist">Заменять или нет существующий трассировщик.</param>
        private void InitTrace(bool replaceIfExist)
        {
            if (server != null && server.Connected)
            {
                trace = server.Traces.FindByName(traceId);
                if (trace == null)
                {
                    CreateTrace();
                }
                else
                {
                    if (replaceIfExist)
                    {
                        server.Traces.Remove(trace);
                        CreateTrace();
                    }
                }
                traceEventHandler = new TraceEventHandler(OnTraceEventHandler);
                trace.OnEvent += traceEventHandler;
                trace.Update();
            }
        }

        /// <summary>
        /// Начинает трассировку сообщений сервера.
        /// </summary>
        private void StartTrace()
        {
            if (server != null && server.Connected && trace != null && !trace.IsStarted)
            {
                trace.Start();
            }
        }

        /// <summary>
        /// Завершает трассировку сообщений сервера.
        /// </summary>
        private void StopTrace()
        {
            if (server != null && server.Connected && trace != null)
            {
                //trace.Stop();
                trace.OnEvent -= traceEventHandler;
                //trace.Drop(DropOptions.IgnoreFailures);
                trace.Update();
            }
        }

        /// <summary>
        /// Запускает пакет на выполнение. Присваивает пакету идентификатор сессии.
        /// Идентификатор сессии нам необходим, для того, чтобы остановить пакет.
        /// </summary>
        public void StartBatch(Guid batchGuid)
        {
            BatchProcessor batchProcessor = new BatchProcessor(serverName, olapDBWrapper.DatabaseId);
            batchProcessorList.Add(batchProcessor);

            batchProcessor.BatchComplitedEvent += batchComplitedDelegate;
            batchProcessor.StartAsync(GetScript(batchGuid.ToString()));


            //accumulatorManager.StartBatch(batchGuid.ToString(), batchProcessor.SessionId);
        }

        /// <summary>
        /// Останавливает пакет.
        /// </summary>
        /// <param name="batchGuid"></param>
        public void StopBatch(Guid batchGuid)
        {
            //string sessionId = accumulatorManager.CancelBatch(batchId);
            //if (!string.IsNullOrEmpty(sessionId))
            //{
            //    server.CancelSession(sessionId, true);
            //}
        }

        public bool Paused
        {
            get { return ProcessService.Paused; }
            set { ProcessService.Paused = value; }
        }

        // В данной процедуре необходимо отслеживать закончившие свою работу BatchProcessor'ы и
        // ставить их в очередь на удаление. НЕ РЕАЛИЗОВАНО!!!
        private void batchProcessor_BatchComplitedEvent(object sender, EventArgs e)
        {
            BatchProcessor batchProcessor = sender as BatchProcessor;
            batchProcessorList.Remove(batchProcessor);

            ////Насколько я понимаю, вызывать Dispose объекта в обработчике события даннаго объекта - некорректно!!!
            ////Но когда вызывать Dispose?
            batchProcessor.Dispose();
        }

        /// <summary>
        /// Генерирует XMLA скрипт SSAS2005 для расчета объектов.
        /// </summary>
        /// <param name="batchGuid"></param>
        /// <returns></returns>
        private string GetScript(string batchGuid)
        {
            return XmlaGenerator.GetBatchScriptSequential(
                olapDBWrapper.GetProcessableObjectFromDataRows(olapDBWrapper.GetOlapObjectsRowsByBatchGuid(batchGuid)),
                olapDBWrapper.DatabaseId, false, server);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                for (int i = 0; i < batchProcessorList.Count; i++)
                {
                    batchProcessorList[i].BatchComplitedEvent -= batchComplitedDelegate;
                    batchProcessorList[i].Dispose();
                }
                StopTrace();
                trace = null;

                if (server != null && server.Connected)
                {
                    server.Disconnect(true);
                    server.Dispose();
                    server = null;
                }
            }
            base.Dispose(disposing);
        }

        #region IProcessManager Members


        public ProcessMode ProcessMode
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public TransactionMode TransactionMode
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion
    }

    public delegate void BatchComplitedDelegate(object sender, EventArgs e);

    /// <summary>
    /// Класс подключается к серверу SSAS2005, генерирует и отправляет скрипт для расчета.
    /// </summary>
    public class BatchProcessor : DisposableObject
    {
        private Microsoft.AnalysisServices.Server server = new Microsoft.AnalysisServices.Server();
        private string serverName;
        private string databaseId;

        private BackgroundWorker backgroundWorker;

        public event BatchComplitedDelegate BatchComplitedEvent;

        public BatchProcessor(string serverName, string databaseId)
        {
            this.serverName = serverName;
            this.databaseId = databaseId;
            server.Connect(serverName);
        }

        public string StartAsync(string script)
        {
            InitWorker();
            backgroundWorker.RunWorkerAsync(script);
            return server.SessionID;
        }

        public string SessionId
        {
            get
            {
                if (server.Connected)
                {
                    return server.SessionID;
                }
                return string.Empty;
            }
        }

        private void InitWorker()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += new DoWorkEventHandler(processWorker_DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(processWorker_RunWorkerCompleted);
        }

        private void processWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (BatchComplitedEvent != null)
            {
                BatchComplitedEvent(this, EventArgs.Empty);
            }
            if (e.Cancelled)
            {
                //MessageBox.Show("Отменили!!!");
            }
            else
            {
                //MessageBox.Show("Рассчитал!!!");
            }
        }

        private void processWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Process((string)e.Argument);
        }

        /// <summary>
        /// Собственно выполнение скрипта на сервере SSAS2005.
        /// </summary>
        /// <param name="script">Скрипт, который необходимо выполнить.</param>
        private void Process(string script)
        {
            try
            {
                server.Execute(script);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        // Implement  the override Dispose method that will contain common cleanup functionality
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                backgroundWorker.Dispose();
                backgroundWorker = null;
            }
            base.Dispose(disposing);
        }
    }
}
