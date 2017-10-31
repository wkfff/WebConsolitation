using System;
using System.Collections.Generic;

using Krista.FM.Server.Common;

using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.OLAP.BatchOperations;


namespace Krista.FM.Server.OLAP.Processor.BatchOperations
{
    /// <summary>
    /// �������� ��������� ������������ �������.
    /// </summary>
    internal class ProcessOlapObjectOperation : BatchOperationAbstract
    {
        private readonly IProcessableObjectInfo objectInfo;
        private readonly IProcessor processor;

        /// <summary>
        /// ������������ ��������, ���������� ������� ������� �� ���� ��������.
        /// </summary>
        readonly Dictionary<string, BatchOperationAbstract> parentOperations = new Dictionary<string, BatchOperationAbstract>();

        /// <summary>
        /// ������������� ���������� �������.
        /// </summary>
        /// <param name="info">���������� �� �������.</param>
        /// <param name="processType">��� ������� �������.</param>
        /// <param name="processor">�������� ��������.</param>
        /// <param name="batchId">������������� ������.</param>
        /// <param name="protocol">������ � ���������.</param>
        public ProcessOlapObjectOperation(
            IProcessableObjectInfo info, 
            Microsoft.AnalysisServices.ProcessType processType,
            IProcessor processor,
            Guid batchId, 
            IMDProcessingProtocol protocol)
            : base(batchId, protocol)
        {
            this.objectInfo = info;
            this.processor = processor;

            ObjectInfo.ProcessType = processType;
        }

        public override string Name
        {
            get
            {
                return String.Format(
                    "��������� ������������ ������� \"{0}\", ��� ���������: {1}",
                    ObjectInfo.ObjectName, ObjectInfo.ProcessType);
            }
        }

        internal IProcessableObjectInfo ObjectInfo
        {
            get { return objectInfo; }
        }

        /// <summary>
        /// ������������ ��������, ���������� ������� ������� �� ���� ��������.
        /// </summary>
        internal Dictionary<string, BatchOperationAbstract> ParentOperations
        {
            get { return parentOperations; }
        }

        public override string Execute()
        {
            startTime = DateTime.Now;
            try
            {
                Trace.TraceVerbose("{0} ����� ������ ������� \"{1}\"", Authentication.UserDate, ObjectInfo.ObjectName);

                string objectName = ObjectInfo.ObjectName;
                if (ObjectInfo.ObjectType == OlapObjectType.Partition)
                {
                    ProcessableObjectInfo cube = ((OlapDBWrapper)processor.OlapDBWrapper).GetCubeByPartitionId(ObjectInfo.ObjectID);
                    if (cube.ObjectName != objectName)
                    {
                        objectName = String.Format("{0}\\{1}", cube.ObjectName, objectName);
                    }
                }

                protocol.WriteEventIntoMDProcessingProtocol(
                    "Krista.FM.Server.OLAP.Processor",
                    MDProcessingEventKind.mdpeStart,
                    String.Format("����� ������ �������. ����� ������� {0}", objectInfo.ProcessType),
                    objectName, ObjectInfo.ObjectID, ObjectInfo.ObjectType, batchId.ToString());

                ObjectInfo.ProcessResult = ((ProcessManager)processor.ProcessManager).ProcessObject(ObjectInfo, batchId);
                
                // ��� ������� �������, ������� ���� ������������� ������� �������
                if (String.IsNullOrEmpty(ObjectInfo.ProcessResult))
                {
                    ((OlapDBWrapper)processor.OlapDBWrapper).RefreshState(
                        ObjectInfo.ObjectType,
                        ObjectInfo.CubeName,
                        ObjectInfo.ObjectName, 
                        ObjectInfo.ObjectID);
                    
                    ((OlapDBWrapper)processor.OlapDBWrapper).SetNeedProcess(ObjectInfo, SetNeedProcessOptions.FixedValue, false);
                    
                    ObjectInfo.RecordStatus = RecordStatus.ComplitedSuccessful;

                    // ���� ����� ������� ��������� ��� Rebuild, 
                    // �� ���������� ���� ������� � ���� ������, � ������� ������������ ���������
                    if (ObjectInfo.ObjectType == OlapObjectType.Dimension &&
                        ObjectInfo.ProcessType == Microsoft.AnalysisServices.ProcessType.ProcessFull)
                    {
                        ((OlapDBWrapper)processor.OlapDBWrapper).ResetUsedPartitionsInDimension(ObjectInfo.ObjectID);
                    }

                    Trace.TraceVerbose("{0} ������� �������� ������ ������� \"{1}\". ����� ������� {2}",
                        Authentication.UserDate, objectName, DateTime.Now - startTime);

                    protocol.WriteEventIntoMDProcessingProtocol("Krista.FM.Server.OLAP.Processor",
                        MDProcessingEventKind.mdpeSuccefullFinished,
                        String.Format("������� �������� ������ �������. ����� ������� {0}", DateTime.Now - startTime),
                        objectName, ObjectInfo.ObjectID, ObjectInfo.ObjectType, batchId.ToString());
                }
                else
                {
                    // ��� ��������� �������� ������������� ��������� �������
                    foreach (ProcessOlapObjectOperation item in ParentOperations.Values)
                    {
                        item.ObjectInfo.RecordStatus =  RecordStatus.ComplitedWithErrors;
                        item.ObjectInfo.ProcessResult = String.Format("��������� \"{0}\" ������� ������ \"{1}\"",
                            ObjectInfo.ObjectName, ObjectInfo.ProcessResult);
                    }

                    // ��� �������� ������� ������������� ��������� �������
                    ObjectInfo.RecordStatus = RecordStatus.ComplitedWithErrors;

                    Trace.TraceError("{0} ������� ������� ������� ����������� ��������: {1}", Authentication.UserDate, ObjectInfo.ProcessResult);

                    protocol.WriteEventIntoMDProcessingProtocol("Krista.FM.Server.OLAP.Processor",
                        MDProcessingEventKind.mdpeError,
                        string.Format("������� ������� ������� ����������� ��������: {0}", ObjectInfo.ProcessResult),
                        objectName, ObjectInfo.ObjectID, ObjectInfo.ObjectType, batchId.ToString());
                }
                return ObjectInfo.ProcessResult;
            }
            catch (Exception e)
            {
                string error = Krista.Diagnostics.KristaDiagnostics.ExpandException(e);
                Trace.TraceCritical("{0} ������� ������� ������� ����������� ��������: {1}", Authentication.UserDate, error);
                return error;
            }
            finally
            {
                finishTime = DateTime.Now;
            }
        }
    }
}
