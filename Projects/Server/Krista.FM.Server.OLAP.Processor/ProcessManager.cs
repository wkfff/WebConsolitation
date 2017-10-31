using System;
using System.Collections.Generic;

using Krista.FM.Common;
using Krista.FM.Server.OLAP.BatchOperations;
using Krista.FM.Server.OLAP.Processor.BatchOperations;

using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.Common;
using Microsoft.AnalysisServices;


namespace Krista.FM.Server.OLAP.Processor
{
    /// <summary>
    /// ������� �����, ����������� ��������� ������� �������.
    /// </summary>
    public abstract class ProcessManager : DisposableObject, IProcessManager
    {
        protected IScheme scheme;
        protected IMDProcessingProtocol protocol;
        protected OlapDBWrapper olapDBWrapper;

        protected ProcessMode processMode;
        protected TransactionMode transactionMode;

        /// <summary>
        /// ������� �����, ����������� ��������� ������� �������.
        /// </summary>
        protected ProcessManager(IScheme scheme)
        {               
            this.scheme = scheme;
            this.olapDBWrapper = ((ProcessorClass)scheme.Processor).OlapDBWrapper;
            this.protocol = (IMDProcessingProtocol)this.scheme.GetProtocol(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.Name);            
        }

        /// <summary>
        /// ����������� ������ � �������� ���������������. ���������� ��������� ������� � ���� ������.
        /// ���� ������ ������� - ������ ������.
        /// </summary>
        /// <returns></returns>
        internal abstract string ProcessObject(IProcessableObjectInfo item, Guid batchID);

        /// <summary>
        /// ��������� ������ �������� ��� ������.
        /// </summary>
        /// <param name="batchGuid">������������� ������.</param>
        /// <param name="items">������ ����������� ��������.</param>
        /// <returns>������ �������� ������.</returns>
        private Dictionary<string, BatchOperationAbstract> PrepareBatchOperations(Guid batchGuid, IEnumerable<IProcessableObjectInfo> items)
        {
            Dictionary<string, BatchOperationAbstract> batchOperations = new Dictionary<string, BatchOperationAbstract>();

            //
            // �������� �������������� ��������� ������ � ���� ��� �������������� ��������
            //
            List<string> schemeObjects = new List<string>();
            foreach (IProcessableObjectInfo item in items)
            {
                if (!schemeObjects.Contains(item.ObjectKey))
                {
                    schemeObjects.Add(item.ObjectKey);

                    IProcessableObjectInfo objectInfo = item.ObjectType == OlapObjectType.Partition
                                     ? olapDBWrapper.GetCubeByPartitionId(item.ObjectID)
                                     : item;

                    batchOperations.Add("!" + objectInfo.ObjectID,
                                        new ProcessDatabaseTableOperation(item, scheme, batchGuid, protocol,
                                                                          GetObjectName(objectInfo)));
                }

                if (!String.IsNullOrEmpty(item.BatchOperations))
                {
                    PrepareCustomOperation(item, batchOperations, batchGuid);
                }
            }

            //
            // ��������� �������� ��������� ���������
            //
            foreach (IProcessableObjectInfo item in items)
            {
                if (item.ObjectType == OlapObjectType.Dimension)
                {
                    Microsoft.AnalysisServices.ProcessType processaType = GetProcessTypeForDimension(item); ;
                    
                    batchOperations.Add(
                        item.ObjectID,
                        new ProcessOlapObjectOperation(
                            item,
                            processaType,
                            scheme.Processor,
                            batchGuid,
                            protocol));
                }
            }

            //
            // ��������� ��������� ������������ � ������� �����
            //
            Dictionary<string, BatchOperationAbstract> partitionsOperations = new Dictionary<string, BatchOperationAbstract>();
            List<IProcessableObjectInfo> additionalItems = new List<IProcessableObjectInfo>();
            foreach (IProcessableObjectInfo item in items)
            {
                if (item.ObjectType == OlapObjectType.Partition)
                {
                    // ������� �������� ��� ������� ������ ����
                    ProcessOlapObjectOperation partitionOperation = new ProcessOlapObjectOperation(
                            item,
                            Microsoft.AnalysisServices.ProcessType.ProcessFull,
                            scheme.Processor,
                            batchGuid,
                            protocol);

                    // ��� ������ ������ ������� �������� ������������ ���������. 
                    // ��� �������� �������
                    Dictionary<string, IProcessableObjectInfo> dimensions = olapDBWrapper.DS_GetCubeDimensions(item.CubeId);
                    foreach (KeyValuePair<string, IProcessableObjectInfo> dimension in dimensions)
                    {
                        if (!batchOperations.ContainsKey(dimension.Key))
                        {
                            if (dimension.Value.State == Microsoft.AnalysisServices.AnalysisState.Unprocessed 
                                || ((ProcessableObjectInfo)dimension.Value).NeedProcess)
                            {
                                // ������� �������� �������������� ��������� ������ � ���� ��� ���������
                                if (!schemeObjects.Contains(dimension.Value.ObjectKey))
                                {
                                    schemeObjects.Add(dimension.Value.ObjectKey);

                                    batchOperations.Add("!" + dimension.Value.ObjectID,
                                                        new ProcessDatabaseTableOperation(dimension.Value, scheme,
                                                                                          batchGuid, protocol, GetObjectName(dimension.Value)));
                                }

                                // ������� �������� ������� ���������
                                ProcessOlapObjectOperation dimensionOperation = new ProcessOlapObjectOperation(
                                    dimension.Value,
                                    dimension.Value.State == Microsoft.AnalysisServices.AnalysisState.Processed
                                        ? Microsoft.AnalysisServices.ProcessType.ProcessUpdate
                                        : Microsoft.AnalysisServices.ProcessType.ProcessDefault,
                                    scheme.Processor,
                                    batchGuid,
                                    protocol);

                                dimensionOperation.ParentOperations.Add(item.ObjectID, partitionOperation);

                                batchOperations.Add(dimension.Value.ObjectID, dimensionOperation);
                                additionalItems.Add(dimension.Value);
                            }
                        }
                        else
                        {
                            ((ProcessOlapObjectOperation)batchOperations[dimension.Key])
                                .ParentOperations.Add(item.ObjectID, partitionOperation);
                        }
                    }

                    partitionsOperations.Add(item.ObjectID, partitionOperation);
                }
            }

            foreach (IProcessableObjectInfo additionalItem in additionalItems)
            {
                if (!((List<IProcessableObjectInfo>)items).Contains(additionalItem))
                {
                    ((List<IProcessableObjectInfo>)items).Add(additionalItem);
                }
            }

            //
            // ��������� ������ �����
            //
            foreach (KeyValuePair<string, BatchOperationAbstract> item in partitionsOperations)
            {
                batchOperations.Add(item.Key, item.Value);
            }

            return batchOperations;
        }

        private ProcessType GetProcessTypeForDimension(IProcessableObjectInfo item)
        {
            if (item.State == Microsoft.AnalysisServices.AnalysisState.Unprocessed)
            {
                return ProcessType.ProcessFull;
            }

            if (scheme.SchemeMDStore.IsAS2005())
            {
                return ProcessType.ProcessUpdate;
            }
            
            return item.ProcessType;
        }

        /// <summary>
        /// ���������������� �������������� ���������
        /// </summary>
        /// <param name="item"></param>
        /// <param name="batchOperations"></param>
        /// <param name="batchGuid"></param>
        private void PrepareCustomOperation(IProcessableObjectInfo item, Dictionary<string, BatchOperationAbstract> batchOperations, Guid batchGuid)
        {
            IProcessableObjectInfo objectInfo = item.ObjectType == OlapObjectType.Partition
                                                    ? olapDBWrapper.GetCubeByPartitionId(item.ObjectID)
                                                    : item;

            string[] operations = item.BatchOperations.Split(';');
            foreach (var operation in operations)
            {
                try
                {
                    // ������������ ��������� ��� �������������� ��������
                    object[] parameters = { batchGuid, protocol, scheme };
                    batchOperations.Add ("!!" + objectInfo.ObjectID,
                                            (BatchOperationAbstract)Activator.CreateInstance(Type.GetType(operation), parameters));
                }
                catch (Exception e)
                {
                    Trace.TraceError(
                        string.Format(
                            "��� ������� {0} �� ������� ���������������� ������ � ����� {1}. ���������� : {2}",
                            objectInfo.ObjectKey, operation, e.Message));

                    protocol.WriteEventIntoMDProcessingProtocol(
                        "Krista.FM.Server.OLAP.Processor",
                        MDProcessingEventKind.mdpeWarning,
                        string.Format("��� ������� {0} �� ������� ���������������� ������ � ����� {1}. ���������� : {2}",
                                        objectInfo.ObjectKey, operation, e.Message),
                        "������������ ����� ������",
                        "������������ ����� ������",
                        OlapObjectType.Database,
                        batchGuid.ToString());
                }
            }
        }

        private string GetObjectName(IProcessableObjectInfo objectInfo)
        {
            string objectName = objectInfo.ObjectName;
            if (objectInfo.ObjectType == OlapObjectType.Partition)
            {
                IProcessableObjectInfo cube = ((OlapDBWrapper)scheme.Processor.OlapDBWrapper).GetCubeByPartitionId(objectInfo.ObjectID);
                if (cube.ObjectName != objectName)
                {
                    objectName = String.Format("{0}\\{1}", cube.ObjectName, objectName);
                }
            }
            return objectName;
        }

        /// <summary>
        /// ���������� �������� ������ �� ����������.
        /// </summary>
        /// <param name="batchOperations">������ �������� ������.</param>
        /// <returns>��������� ����������: ���� ������ ������, �� ��� �������� ��������� �������, ����� ����� ������.</returns>
        /// <param name="batchGuid">������������� ������</param>
        /// <param name="priority"> ��������� ���������� ������</param>
        protected virtual string ExecuteBatchOperations(IEnumerable<KeyValuePair<string, BatchOperationAbstract>> batchOperations, Guid batchGuid, BatchStartPriority priority)
        {
            string processResult = String.Empty;
            foreach (KeyValuePair<string, BatchOperationAbstract> operation in batchOperations)
            {
                processResult = operation.Value.Execute();
                if (!String.IsNullOrEmpty(processResult))
                {
                    break;
                }
            }

            return processResult;
        }

        /// <summary>
        /// ��������� ��� ����� ������������.
        /// </summary>
        /// <param name="batchOperations">������ �������� ���������� � ������.</param>
        /// <param name="batchGuid">������������� ������.</param>
        private void RefreshPlaningHash(IDictionary<string, BatchOperationAbstract> batchOperations, Guid batchGuid)
        {
            List<string> dimensionsNames = new List<string>();
            List<string> cubesNames = new List<string>();

            foreach (BatchOperationAbstract item in batchOperations.Values)
            {
                if (item is ProcessOlapObjectOperation)
                {
                    ProcessOlapObjectOperation operation = (ProcessOlapObjectOperation)item;

                    // ������������ ������ ������� ����������� ��������
                    //if (operation.ObjectInfo.RecordStatus == RecordStatus.ComplitedSuccessful)
                    {
                        if (operation.ObjectInfo.ObjectType == OlapObjectType.Dimension)
                        {
                            if (!dimensionsNames.Contains(operation.ObjectInfo.ObjectName))
                            {
                                dimensionsNames.Add(operation.ObjectInfo.ObjectName);
                            }

                            // ��������� ����, � ������� �������������� ���������
                            if (operation.ObjectInfo.ProcessType == Microsoft.AnalysisServices.ProcessType.ProcessFull)
                            {
                                Dictionary<string, IProcessableObjectInfo> dependedCubes = olapDBWrapper.DS_GetDimensionCubes(operation.ObjectInfo.ObjectID);
                                foreach (IProcessableObjectInfo cube in dependedCubes.Values)
                                {
                                    if (!cubesNames.Contains(cube.ObjectName))
                                    {
                                        cubesNames.Add(cube.ObjectName);
                                    }
                                }
                            }
                        }

                        if (operation.ObjectInfo.ObjectType == OlapObjectType.Partition)
                        {
                            ProcessableObjectInfo info = olapDBWrapper.GetCubeByPartitionId(operation.ObjectInfo.ObjectID);
                            if (!cubesNames.Contains(info.ObjectName))
                            {
                                cubesNames.Add(info.ObjectName);
                            }
                        }
                    }
                }
            }

            try
            {
                if (dimensionsNames.Count > 0)
                {
                    Trace.TraceVerbose("{0} ���������� ���� ��������� ����� ������������ ({1}).", Authentication.UserDate, dimensionsNames.Count);

                    protocol.WriteEventIntoMDProcessingProtocol("Krista.FM.Server.OLAP.Processor",
                        MDProcessingEventKind.mdpeInformation,
                        String.Format("���������� ���� ��������� ����� ������������ ({0}).", dimensionsNames.Count),
                        "��� ����� ������������",
                        "��� ����� ������������",
                        OlapObjectType.Database,
                        batchGuid.ToString());
                    scheme.PlaningProvider.RefreshDimension("0", dimensionsNames.ToArray());
                }
                if (cubesNames.Count > 0)
                {
                    Trace.TraceVerbose("{0} ���������� ���� ����� ����� ������������ ({1}).", Authentication.UserDate, cubesNames.Count);

                    protocol.WriteEventIntoMDProcessingProtocol("Krista.FM.Server.OLAP.Processor",
                        MDProcessingEventKind.mdpeInformation,
                        String.Format("���������� ���� ����� ����� ������������ ({0}).", cubesNames.Count),
                        "��� ����� ������������",
                        "��� ����� ������������",
                        OlapObjectType.Database,
                        batchGuid.ToString());
                    scheme.PlaningProvider.RefreshCube("0", cubesNames.ToArray());
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("��� ���������� ���� ����� ������������ ��������� ������: {0}", Krista.Diagnostics.KristaDiagnostics.ExpandException(e));

                protocol.WriteEventIntoMDProcessingProtocol(
                    "Krista.FM.Server.OLAP.Processor",
                    MDProcessingEventKind.mdpeWarning,
                    String.Format("��� ���������� ���� ����� ������������ ��������� ������: {0}", Krista.Diagnostics.KristaDiagnostics.ExpandException(e)),
                    "��� ����� ������������",
                    "��� ����� ������������",
                    OlapObjectType.Database,
                    batchGuid.ToString());
            }
        }

        /// <summary>
        /// ������ ������ �� ������.
        /// </summary>
        /// <param name="batchGuid">������������� ������.</param>
        /// <param name="priority"> ��������� ������</param>
        internal void StartProcessBatch(Guid batchGuid, BatchStartPriority priority)
        {
            Trace.TraceVerbose("{0} ����� ������ ������ \"{1}\"", Authentication.UserDate, batchGuid);

            protocol.WriteEventIntoMDProcessingProtocol("Krista.FM.Server.OLAP.Processor",
                MDProcessingEventKind.mdpeStart, "����� ������ ������", "�����",
                batchGuid.ToString(), OlapObjectType.Database, batchGuid.ToString());

            //����������� ������� � ������ RecordStatus = inProcess.
            IEnumerable<IProcessableObjectInfo> items = olapDBWrapper.StartProcessBatch(batchGuid, SessionContext.SessionId);

            // ������������ �������� ������
            Dictionary<string, BatchOperationAbstract> batchOperations = PrepareBatchOperations(batchGuid, items);

            // ������ �������� �� ����������
            string processResult = ExecuteBatchOperations(batchOperations, batchGuid, priority);

            // ��������� ���������� ����� ������������
            try
            {
                RefreshPlaningHash(batchOperations, batchGuid);
            }
            catch (Exception e)
            {
                processResult = String.Format("{0}\n{1}", processResult, Krista.Diagnostics.KristaDiagnostics.ExpandException(e));
            }

            //
            // ���������� ��������� ������
            //
            Trace.TraceVerbose("{0} ������ ������ �������� {1}", Authentication.UserDate, String.IsNullOrEmpty(processResult) ? String.Empty : ". ", processResult);

            protocol.WriteEventIntoMDProcessingProtocol(
                "Krista.FM.Server.OLAP.Processor",
                String.IsNullOrEmpty(processResult) ? MDProcessingEventKind.mdpeSuccefullFinished : MDProcessingEventKind.mdpeFinishedWithError,
                String.Format("������ ������ ��������{0}{1}",
                    String.IsNullOrEmpty(processResult) ? String.Empty : ". ", processResult),
                "�����",
                batchGuid.ToString(),
                OlapObjectType.Database, batchGuid.ToString());

            olapDBWrapper.ComplitBatch(batchGuid,
                String.IsNullOrEmpty(processResult) ? BatchState.Complited : BatchState.ComplitedWithError,
                items);
        }

        #region IProcessManager Members

        public void StartBatch(Guid batchGuid)
        {
            //����������� ������� � ������ RecordStatus = inProcess.
            olapDBWrapper.StartBatch(batchGuid, SessionContext.SessionId);
            ProcessService.newRequestEvent.Set();
        }

        public virtual void StopBatch(Guid batchGuid)
        {
            Trace.WriteLine("����� \"ProcessManager2000.StopBatch()\" �� ����������.");
        }

        public bool Paused
        {
            get { return ProcessService.Paused; }
            set { ProcessService.Paused = value; }
        }

        public ProcessMode ProcessMode
        {
            get
            {
                return processMode;
            }
            set
            {
                processMode = value;
            }
        }

        public TransactionMode TransactionMode
        {
            get
            {
                return transactionMode;
            }
            set
            {
                transactionMode = value;
            }
        }

        #endregion
    }
}
