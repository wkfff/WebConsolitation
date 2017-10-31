using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using Krista.FM.Common;
using Krista.FM.Server.OLAP.BatchOperations;
using Krista.FM.Server.OLAP.Processor.BatchOperations;
using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.ServerLibrary;
using Microsoft.AnalysisServices;

namespace Krista.FM.Server.OLAP.Processor
{
    /// <summary>
    /// ������� ������ (BatchProcessor) � ���������� �� �� ���� ���������� �������.
    /// </summary>
    public class ProcessManager2005 : ProcessManager
    {
        private static volatile ProcessManager2005 instance;
        private static readonly object syncRoot = new object();

        private Microsoft.AnalysisServices.Server server;
        private Microsoft.AnalysisServices.Trace trace;

        /// <summary>
        /// ID ������
        /// </summary>
        private string sessionID;

        private TraceEventHandler traceEventHandler;
        private string traceId = String.Empty;

        private readonly List<BatchProcessor> batchProcessorList = new List<BatchProcessor>();
        private readonly BatchComplitedDelegate batchComplitedDelegate;

        private readonly BatchProcessor batchProcessor;

        /// <summary>
        /// ��������� ������������ ManualResetEvent ��� �������� �� ��������� � ������������ ���������
        /// </summary>
        private ManualResetEvent manualEvent;
        private DateTime startTime;

        /// <summary>
        /// ID ������
        /// </summary>
        private Guid batchID = Guid.Empty;

        /// <summary>
        /// ��������� � ������ ��������� ��������� �� ������ �� ��������
        /// </summary>
        private string errorMessage = string.Empty;
        /// <summary>
        /// GUID ������� � ������, ���������� ������
        /// </summary>
        private string errorObjectID = string.Empty;

        private const string ProcessTimeOut = "ProcessTimeOut";

        /// <summary>
        /// ������� ������� �� ���������, ���� ��� ������� ��� ��� - ������ ������
        /// </summary>
        private bool isGetMessageFromTrace;

        /// <summary>
        /// ������� ������ �������
        /// </summary>
        private bool canceled;

        public static ProcessManager2005 GetInstance(IScheme scheme, Microsoft.AnalysisServices.Server server, bool replaceTrace)
        {
            if (instance == null)
            {
                lock (syncRoot)                
                {
                    if (instance == null)
                    {
                        instance = new ProcessManager2005(scheme, server, replaceTrace);
                    }
                }
            }
            return instance;
        }

        protected ProcessManager2005(IScheme scheme, Microsoft.AnalysisServices.Server server, bool replaceTrace)
            : base (scheme)
        {
            this.server = server;

            manualEvent = new ManualResetEvent(false);

            InitTrace(replaceTrace);
            StartTrace();

            batchProcessor = new BatchProcessor(server);
            batchProcessor.BatchComplitedEvent +=batchProcessor_BatchComplitedEvent;

            processMode = ProcessMode.ParallelMode;
            transactionMode = TransactionMode.OneTransaction;
        }

        /// <summary>
        /// ������� �����, ����������� ��������� ������� �������.
        /// </summary>
        protected ProcessManager2005(IScheme scheme)
            : base(scheme)
        {
            
        }

        /// <summary>
        /// ��������� ��������� ������
        /// </summary>
        /// <param name="batchGuid"></param>
        public override void StopBatch(Guid batchGuid)
        {
            canceled = true;
            batchProcessor.CancelProcess();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchOperations"></param>
        /// <param name="batchGuid"></param>
        /// <returns></returns>
        protected override string ExecuteBatchOperations(IEnumerable<KeyValuePair<string, BatchOperationAbstract>> batchOperations, Guid batchGuid, BatchStartPriority priority)
        {
            canceled = false;

            IEnumerable<IProcessableObjectInfo> items = GetProccessCollection(batchOperations);

            try
            {
                manualEvent.Reset();

                ExecuteInitialize(batchGuid);
                
                protocol.WriteEventIntoMDProcessingProtocol("Krista.FM.Server.OLAP.Processor",
                            MDProcessingEventKind.mdpeSuccefullFinished,
                            "��������� �������� ��� ������� ��������",
                            "�����", string.Empty, 0, batchID.ToString());

                // �������� ������
                string script = GetScript(items);

                protocol.WriteEventIntoMDProcessingProtocol("Krista.FM.Server.OLAP.Processor",
                            MDProcessingEventKind.mdpeSuccefullFinished,
                            String.Format("������ ��� {0} ������� ������� �������", (processMode == ProcessorLibrary.ProcessMode.ParallelMode) ? "�������������" : "�����������������"),
                            "�����", string.Empty, 0, batchID.ToString());

                startTime = DateTime.Now;

                // ��� ��������
                if (canceled)
                {
                   return FailureFinishProcess(items, "�������� ��������");
                }

                batchProcessor.StartAsync(script);

                if (scheme.Server.GetConfigurationParameter(ProcessTimeOut) != null)
                {
                    int timeout = Convert.ToInt32(scheme.Server.GetConfigurationParameter(ProcessTimeOut));

                    protocol.WriteEventIntoMDProcessingProtocol("Krista.FM.Server.OLAP.Processor",
                                                                MDProcessingEventKind.mdpeSuccefullFinished,
                                                                string.Format(
                                                                    "����� ������. ������������ ����������������� ��������� ������ {0} ��",
                                                                    timeout),
                                                                "�����", string.Empty, 0, batchID.ToString());


                    if (manualEvent.WaitOne(timeout, false))
                    {
                        manualEvent.Reset();

                        if (String.IsNullOrEmpty(errorMessage) && !canceled)
                            return SuccessFinishProcess(items, priority);
                        return FailureFinishProcess(items, errorMessage);
                    }

                    return FailureFinishProcess(items, "����������� ����� �������� ������� ������");
                }
                else
                {
                    protocol.WriteEventIntoMDProcessingProtocol("Krista.FM.Server.OLAP.Processor",
                                                                MDProcessingEventKind.mdpeSuccefullFinished,
                                                                "����� ������. ������������ ����������������� ������� �� ����������",
                                                                "�����", string.Empty, 0, batchID.ToString());
                    if (manualEvent.WaitOne())
                    {
                        manualEvent.Reset();

                        Thread.Sleep(5000);

                        if (String.IsNullOrEmpty(errorMessage) && !canceled)
                            return SuccessFinishProcess(items, priority);
                        return FailureFinishProcess(items, errorMessage);
                    }

                    return FailureFinishProcess(items, "����������� ����� �������� ������� ������");
                }
            }
            catch (Exception e)
            {
                return FailureFinishProcess(items, e.Message);
            }
        }

        private string FailureFinishProcess(IEnumerable<IProcessableObjectInfo> items, string errorMessage)
        {
            if (canceled)
            {
                errorMessage = "�������� ��������.";
            }
            Trace.TraceError(errorMessage);
            isGetMessageFromTrace = false;


            protocol.WriteEventIntoMDProcessingProtocol("Krista.FM.Server.OLAP.Processor",
                                                        MDProcessingEventKind.mdpeError,
                                                        String.Format(
                                                            "������ ������� �������� � �������. {0}",
                                                            errorMessage),
                                                        "�����", string.Empty, 0, batchID.ToString());
            errorMessage = String.Format("{0} \n������������� ������: /*!!{1}!!*/", errorMessage, batchID);

            int result = 0;
            IProcessableObjectInfo first = null;
            if (!String.IsNullOrEmpty(errorMessage))
            {
                foreach (IProcessableObjectInfo op in items)
                {
                    if (first == null)
                    {
                        first = op;
                    }

                    if (op.ObjectID.Equals(errorObjectID) || errorObjectID == batchID.ToString())
                    {
                        op.RecordStatus = RecordStatus.ComplitedWithErrors;
                        op.ProcessResult = string.Format("������ ������� �������� � �������. {0}",
                                                         errorMessage);
                    }

                    result++;
                }
            }

            #region Send Message

            try
            {
                string messageSubject = (result > 1) ?
                    "������ ��� ������� ������������ �������" :
                    String.Format("������ ��� ������� ������������ ������� {0}",
                                                            first.ObjectName);
                MessageWrapper messageWrapper = new MessageWrapper();
                messageWrapper.Subject = messageSubject;
                messageWrapper.MessageType = MessageType.CubesManagerMessage;
                messageWrapper.MessageImportance = MessageImportance.Importance;
                messageWrapper.SendAll = true;
                messageWrapper.TransferLink = batchID.ToString();

                scheme.MessageManager.SendMessage(messageWrapper);
            }
            catch (Exception e)
            {
                Trace.TraceError(String.Format("��� �������� ��������� �� ������ ������� ������������ ������� �������� ����������: {0}", e.Message));
            }

            #endregion

            return errorMessage;
        }

        private string SuccessFinishProcess(IEnumerable<IProcessableObjectInfo> items, BatchStartPriority priority)
        {
            if (canceled)
            {
                return FailureFinishProcess(items, "�������� ��������");
            }

            if (!isGetMessageFromTrace)
            {
                return FailureFinishProcess(items, "������������� ������ ����������, ������, ������� � ������������ � ��������� ������.");
            }

            isGetMessageFromTrace = false;

            protocol.WriteEventIntoMDProcessingProtocol("Krista.FM.Server.OLAP.Processor",
                                                        MDProcessingEventKind.mdpeSuccefullFinished,
                                                        String.Format(
                                                            "������� �������� ������ �������. ����� ������� {0}",
                                                            DateTime.Now - startTime),
                                                        "�����", string.Empty, 0, batchID.ToString());


            int result = 0;
            IProcessableObjectInfo first = null;
            if (String.IsNullOrEmpty(errorMessage))
            {
                foreach (IProcessableObjectInfo op in items)
                {
                    if (first == null)
                    {
                        first = op;
                    }

                    op.LastProcessed = DateTime.Now;
                    op.State = AnalysisState.Processed;
                    op.RecordStatus = RecordStatus.ComplitedSuccessful;
                    op.ProcessResult = string.Empty;
                    result++;
                }
            }

            AdditionalOperation(items);

            #region Send Message

            // ���������� ��������� ������ ��� ������� � ������ �����������. 
            // � ������� (�� �������� ������) �� ����������. ������� �� �����.
            try
            {
                if (priority == BatchStartPriority.Auto)
                {
                    string messageSubject = (result > 1)
                                                ? "������� �������� ������ ������"
                                                : String.Format("������� �������� ������ ������� {0}",
                                                                first.ObjectName);
                    MessageWrapper messageWrapper = new MessageWrapper();
                    messageWrapper.Subject = messageSubject;
                    messageWrapper.MessageType = MessageType.CubesManagerMessage;
                    messageWrapper.MessageImportance = MessageImportance.Regular;
                    messageWrapper.SendAll = true;
                    messageWrapper.TransferLink = batchID.ToString();

                    scheme.MessageManager.SendMessage(messageWrapper);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(String.Format("��� �������� ��������� �� ������ ������� ������������ ������� �������� ����������: {0}", e.Message));
            }

            #endregion

            return string.Empty;
        }

        /// <summary>
        /// ������������� ���������� �������
        /// </summary>
        /// <param name="batchGuid"></param>
        private void ExecuteInitialize(Guid batchGuid)
        {
            errorMessage = String.Empty;
            batchID = batchGuid;
            errorObjectID = string.Empty;
        }

        private void AdditionalOperation(IEnumerable<IProcessableObjectInfo> items)
        {
            foreach (IProcessableObjectInfo item in items)
            {
                // ��� ������� �������, ������� ���� ������������� ������� �������
                if (String.IsNullOrEmpty(item.ProcessResult))
                {
                    ((OlapDBWrapper)scheme.Processor.OlapDBWrapper).RefreshState(
                        item.ObjectType,
                        item.CubeId,
                        item.MeasureGroupId,
                        item.ObjectID);

                    ((OlapDBWrapper)scheme.Processor.OlapDBWrapper).SetNeedProcess(item, SetNeedProcessOptions.FixedValue, false);

                    item.RecordStatus = RecordStatus.ComplitedSuccessful;

                    // ���� ����� ������� ��������� ��� Rebuild, 
                    // �� ���������� ���� ������� � ���� ������, � ������� ������������ ���������
                    if (item.ObjectType == OlapObjectType.Dimension &&
                        item.ProcessType == ProcessType.ProcessFull)
                    {
                        ((OlapDBWrapper)scheme.Processor.OlapDBWrapper).ResetUsedPartitionsInDimension(item.ObjectID);
                    }
                }
            }
        }


        private static IEnumerable<IProcessableObjectInfo> GetProccessCollection(IEnumerable<KeyValuePair<string, BatchOperationAbstract>> operations)
        {
            List<IProcessableObjectInfo> list = new List<IProcessableObjectInfo>();

            foreach (KeyValuePair<string, BatchOperationAbstract> operation in operations)
            {
                if (operation.Value is ProcessOlapObjectOperation)
                {
                    list.Add(((ProcessOlapObjectOperation)operation.Value).ObjectInfo);
                }
                else
                {
                    operation.Value.Execute();
                }
            }

            return list;
        }

        /// <summary>
        /// ����������� ������ � �������� ���������������. ���������� ��������� ������� � ���� ������.
        /// ���� ������ ������� - ������ ������.
        /// </summary>
        /// <returns></returns>
        internal override string ProcessObject(IProcessableObjectInfo item, Guid batchID)
        {
            Database database = (Database)olapDBWrapper.OlapDatabase.DatabaseObject;

            if (null != item)
            {
                try
                {
                    Trace.TraceVerbose("{0} ����� ������ ������� {1} \"{2}\". ����� ������� {3}",
                        Krista.FM.Server.Common.Authentication.UserDate, item.ObjectType, item.ObjectName, item.ProcessType);

                    switch (item.ObjectType)
                    {
                        case OlapObjectType.Partition:
                            Cube cube;
                            try
                            {
                                cube = database.Cubes[item.CubeId];
                            }
                            catch (Exception e)
                            {
                                throw new ServerException(String.Format("��� \"{0}\" �� ������ � ����������� ���� ������.", item.CubeName), e);
                            }

                            MeasureGroup mg;
                            try
                            {
                                mg = cube.MeasureGroups[item.MeasureGroupId];
                            }
                            catch (Exception e)
                            {
                                throw new ServerException(String.Format("������ ��� \"{0}\" �� ������� � ����������� ���� ������.", item.MeasureGroupName), e);
                            }

                            Partition partition;
                            try
                            {
                                partition = mg.Partitions[item.ObjectID];
                            }
                            catch (Exception e)
                            {
                                throw new ServerException(String.Format("������ \"{0}\" �� ������� � ����������� ���� ������.", item.ObjectName), e);
                            }

                            partition.Process(item.ProcessType);
                            item.State = partition.State;
                            item.LastProcessed = partition.LastProcessed;
                            item.RecordStatus = RecordStatus.ComplitedSuccessful;
                            break;
                        case OlapObjectType.Dimension:
                            Dimension dimension;
                            try
                            {
                                dimension = database.Dimensions[item.ObjectID];
                            }
                            catch (Exception e)
                            {
                                throw new ServerException(
                                    String.Format("��������� \"{0}\" �� ������� � ����������� ���� ������.", item.ObjectName), e);
                            }

                            dimension.Process(item.ProcessType);
                            item.State = dimension.State;
                            item.LastProcessed = dimension.LastProcessed;
                            item.RecordStatus = RecordStatus.ComplitedSuccessful;
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError("{0} ������ ������� �������� � �������: {1} \"{2}\" {3}", 
                        Krista.FM.Server.Common.Authentication.UserDate, item.ObjectType, item.ObjectName, e.Message);
                    return e.Message;
                }
            }

            return String.Empty;

        }

        /// <summary>
        /// ���������� ��������� SSAS2005. ������������ ��������� �� ��������� ������� �
        /// ������������ ��������������� ������� ������ ������.
        /// </summary>        
        private void OnTraceEventHandler(object sender, TraceEventArgs e)
        {
            if (e.SessionID != null && e.SessionID != server.SessionID)
            {
                return;
            }

            if (e.SessionID == server.SessionID
                && String.IsNullOrEmpty(sessionID))
            {
                sessionID = e.SessionID;
            }

            try
            {
                if (server.SessionID == sessionID)
                {
                    switch (e.EventClass)
                    {
                        case TraceEventClass.ProgressReportBegin:
                            {
                                isGetMessageFromTrace = true;

                                if (e.EventSubclass == TraceEventSubclass.Process && !string.IsNullOrEmpty(e.ObjectID) &&
                                    (((Database) olapDBWrapper.OlapDatabase.DatabaseObject).Dimensions.Contains(
                                         e.ObjectID) ||
                                     ((Database) olapDBWrapper.OlapDatabase.DatabaseObject).Cubes.Contains(e.ObjectID)))
                                {
                                    protocol.WriteEventIntoMDProcessingProtocol(
                                        "Krista.FM.Server.OLAP.Processor",
                                        MDProcessingEventKind.mdpeStart,
                                        e.TextData, e.ObjectName, e.ObjectName, 0, batchID.ToString());
                                }
                                break;
                            }
                        case TraceEventClass.ProgressReportEnd:
                            {
                                if (e.EventSubclass == TraceEventSubclass.Process && !string.IsNullOrEmpty(e.ObjectID) &&
                                    (((Database) olapDBWrapper.OlapDatabase.DatabaseObject).Dimensions.Contains(
                                         e.ObjectID) ||
                                     ((Database) olapDBWrapper.OlapDatabase.DatabaseObject).Cubes.Contains(e.ObjectID)))
                                {
                                    protocol.WriteEventIntoMDProcessingProtocol(
                                        "Krista.FM.Server.OLAP.Processor",
                                        MDProcessingEventKind.mdpeSuccefullFinished,
                                        e.TextData, e.ObjectName, e.ObjectName, 0, batchID.ToString());

                                    if (e.TextData == "���������� ������� ��������� � �������.")
                                    {
                                        errorMessage = "���������� ������� ��������� � �������";
                                        errorObjectID = e.ObjectID;
                                    }
                                }
                                break;
                            }
                        case TraceEventClass.Error:
                        case TraceEventClass.ProgressReportError:
                            protocol.WriteEventIntoMDProcessingProtocol(
                                "Krista.FM.Server.OLAP.Processor",
                                MDProcessingEventKind.mdpeError,
                                e.TextData, batchID.ToString(), batchID.ToString(), 0, batchID.ToString());

                            // ������ � ���� ������ �������� ��������� �� ������ �� �������
                            errorMessage = e.TextData;
                            errorObjectID = batchID.ToString();

                            manualEvent.Set();

                            break;
                        case TraceEventClass.CommandEnd:
                            if (e.EventSubclass == TraceEventSubclass.Batch)
                            {
                                sessionID = string.Empty;
                                manualEvent.Set();
                            }
                            break;
                       case TraceEventClass.ProgressReportCurrent:
                            if (e.EventSubclass == TraceEventSubclass.Batch)
                            {
                            }
                            break;
                        default:
                            break;
                    }
                }

                // ������� sessionID ����� ������ �������������� ������
                if (e.EventClass == TraceEventClass.CommandEnd &&
                    sessionID == server.SessionID)
                        sessionID = String.Empty;
            }
            catch (Exception exception)
            {
                Trace.TraceError(Krista.Diagnostics.KristaDiagnostics.ExpandException(exception));
            }
        }

      
        /// <summary>
        /// ��������� ������� � �� ������� � �������������.
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
        /// ������� ������������ ������� c������ SSAS2005.
        /// </summary>
        private void CreateTrace()
        {
            traceId = Guid.NewGuid().ToString();
            trace = new Microsoft.AnalysisServices.Trace(traceId, Guid.NewGuid().ToString());
            server.Traces.Add(trace);

            AddTraceEvent(TraceEventClass.CommandBegin);
            AddTraceEvent(TraceEventClass.CommandEnd);
            AddTraceEvent(TraceEventClass.ProgressReportBegin);
            AddTraceEvent(TraceEventClass.ProgressReportCurrent);
            AddTraceEvent(TraceEventClass.ProgressReportEnd);
            AddTraceEvent(TraceEventClass.ProgressReportError);
            AddTraceEvent(TraceEventClass.Error);
        }        

        /// <summary>
        /// ������������ � ������������� ������������� ��� ������ �����.
        /// </summary>
        /// <param name="replaceIfExist">�������� ��� ��� ������������ ������������.</param>
        private void InitTrace(bool replaceIfExist)
        {
            if (server != null && server.Connected)
            {
                if (String.IsNullOrEmpty(traceId))
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

                traceEventHandler = OnTraceEventHandler;
                trace.OnEvent += traceEventHandler;                
                trace.Update();
            }
        }

        /// <summary>
        /// �������� ����������� ��������� �������.
        /// </summary>
        private void StartTrace()
        {
            if (server != null && server.Connected && trace != null && !trace.IsStarted)
            {
                trace.Start();
            }
        }

        /// <summary>
        /// ��������� ����������� ��������� �������.
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

        // � ������ ��������� ���������� ����������� ����������� ���� ������ BatchProcessor'� �
        // ������� �� � ������� �� ��������. �� �����������!!!
        private void batchProcessor_BatchComplitedEvent(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                errorMessage = e.Error.Message;
                if (errorMessage == "The connection either timed out or was lost." ||
                    errorMessage == "The Server object is not connected.")
                {
                    errorMessage += " ������������� ������ ����������, ������, ������� � ������������ � ��������� ������.";
                }

                errorObjectID = batchID.ToString();
                manualEvent.Set();
            }
        }

        /// <summary>
        /// ���������� XMLA ������ SSAS2005 ��� ������� ��������.
        /// </summary>
        /// <param name="batchGuid"></param>
        /// <returns></returns>
        private string GetScript(IEnumerable<IProcessableObjectInfo> ops)
        {
            switch (processMode)
            {
                case ProcessMode.ParallelMode:
                    return XmlaGenerator.GetBatchScriptParallel(
                            ops,
                            olapDBWrapper.DatabaseId, server);
                case ProcessMode.SequentialMode:
                    return XmlaGenerator.GetBatchScriptSequential(
                            ops,
                            olapDBWrapper.DatabaseId, (transactionMode == TransactionMode.OneTransaction) ? true : false, server);
            }

            return string.Empty;
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
    }
}
