using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

using Krista.FM.Common;
using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.ServerLibrary;
using Microsoft.AnalysisServices;

//test

namespace Krista.FM.Server.OLAP.Processor
{
    /// <summary>
    /// ����� ��������� ������� �����.
    /// </summary>
    public sealed class ProcessorClass : DisposableObject, IProcessor
	{
        private static volatile ProcessorClass instance;
		private static readonly object syncRoot = new object();

		private readonly IScheme scheme;
		private ProcessService processService;
		
		/// <summary>
		/// ������������ ������������� ������ ��� ������������������ ��������.
		/// </summary>
		private IProcessManager processManagerForDispose;

        private ProcessorClass(IScheme scheme)
		{
			this.scheme = scheme;
            processService = ProcessService.GetInstance(this);
		}

        [System.Diagnostics.DebuggerStepThrough]
        public static ProcessorClass GetInstance(IScheme _scheme)
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new ProcessorClass(_scheme);
                    }
                }
            }
            return instance;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                processService.Dispose();
				if (processManagerForDispose != null)
					processManagerForDispose.Dispose();

				if (OlapDBWrapper != null)
					OlapDBWrapper.Dispose();

				if (ProcessManager != null)
					ProcessManager.Dispose();

            }
            base.Dispose(disposing);
        }

        internal bool MultiServerMode
        {
            get { return scheme.MultiServerMode; }
        }

		#region IProcessor Members
		
		/// <summary>
		/// ������� ����. � ����������� �� ������ OLAP-������� (������������ �������������) ������������ ���� ��������� DSO (AS2000), ���� ��������� AMO (SSAS2005).
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="processTypes">��� �����������.</param>
        /// <param name="name">��� ������������.</param>
		public void ProcessCube(IEntity entity, ProcessTypes processTypes, string name)
		{
			try
			{
                InvalidatePartition(entity, "DataPump", InvalidateReason.DataPump, name);
                //if (scheme.SchemeMDStore.IsAS2005())
                //{
                //    ProcessCube(GetProcessingMode(processTypes), name);
                //}
                //else
                //{
                //    ProcessDSOCube(processTypes, name);
                //}
			}
			catch (Exception e)
			{
				string message = String.Format("��� \"{0}\", ������ �������: {1}", name, e.Message);
				Trace.TraceError(message);				
				throw new ServerException(e.Message, e);				
			}
		}        

        /// <summary>
        /// � ����������� �� ������� ������ ���������� ������ �� ���������� ����� ����������.
        /// </summary>
        /// <param name="invalidateReason">������� ������.</param>
        /// <returns></returns>
        private static BatchStartPriority GetBatchStartOptions(InvalidateReason invalidateReason)
        {
            switch (invalidateReason)
            {
                case InvalidateReason.WriteBack:
                    return BatchStartPriority.Immediately;
                
                case InvalidateReason.ClassifierChanged:
                case InvalidateReason.AssociationChanged:
                case InvalidateReason.UserPleasure:
                case InvalidateReason.DataPump:
                    return BatchStartPriority.Auto;                    
                default:
                    return BatchStartPriority.Auto;
            }
        }

        /// <summary>
        /// ������� ����� ����� ������� ��������.
        /// </summary>
        /// <returns>ID ������.</returns>
        public Guid CreateBatch()
        {
            return OlapDBWrapper.CreateBatch();
        }

        /// <summary>
        /// ���������� ��������� ������.
        /// ����� ��������� ��� ����������� ���������� ����������� �������� ��������� ������.
        /// </summary>
        /// <param name="batchId">ID ������.</param>
        /// <returns>��������� ������.</returns>
        public BatchState GetBatchState(Guid batchId)
        {
            return OlapDBWrapper.GetBatchState(batchId.ToString());
        }

        /// <summary>
        /// ������� ����� ����������� � ��������� ��������.
        /// </summary>
        /// <remarks>
        /// ����� ������ ���������� ���� � �������� ������������ ������ ��������� ������.
        /// ���� ����� ����� ��������� �������� �� "������", �� ����� ������ ����������.
        /// </remarks>
        /// <param name="batchId">ID ������.</param>
        public void RevertBatch(Guid batchId)
        {
            OlapDBWrapper.DeleteBatch(batchId);
        }

        private const string invalidateMessage = "���������� �� ������. {0}{1}";
        private const string invalidateRefuseMessage = "���������� �� ������ ���������, �.�. ������ ��� ��������� � ������ ������ ��������� ��� ��� ��������������. {0}";

        private static string InvalidateReasonToString(InvalidateReason invalidateReason)
        {
            string text;
            switch (invalidateReason)
            {
                case InvalidateReason.AssociationChanged:
                    text = "��������� �������������";
                    break;
                case InvalidateReason.ClassifierChanged:
                    text = "��������� ��������������/�������";
                    break;
                case InvalidateReason.DataPump:
                    text = "������� ������";
                    break;
                case InvalidateReason.UserPleasure:
                    text = "������ ������� �������������";
                    break;
                case InvalidateReason.WriteBack:
                    text = "�������� ������";
                    break;
                default:
                    text = "�� ����������";
                    break;
            }
            return String.Format("{0} ({1})", text, (int)invalidateReason);
        }

        private IEntity GetEntityByFullName(string objectKey)
        {
            if (scheme.Classifiers.ContainsKey(objectKey))
            {
                return scheme.Classifiers[objectKey];
            }
            else if (scheme.FactTables.ContainsKey(objectKey))
            {
                return scheme.FactTables[objectKey];
            }
            else
            {
                throw new Exception(String.Format("������ \"{0}\" � ����� �� ������.", objectKey));
            }
        }

        public void InvalidatePartition(string objectKey, string moduleName, InvalidateReason invalidateReason, string cubeName)
        {
			try
			{
				Guid batchGuid = CreateBatch();
				InvalidatePartition(GetEntityByFullName(objectKey), moduleName, invalidateReason, cubeName, batchGuid);
				ProcessManager.StartBatch(batchGuid);
			}
			catch (Exception e)
			{
				throw new InvalidateOlapObjectException(e.Message, e);
			}
		}

        public void InvalidatePartition(string objectKey, string moduleName, InvalidateReason invalidateReason, string cubeName, string partitionName)
        {
			try
			{
				Guid batchGuid = CreateBatch();
				InvalidatePartition(GetEntityByFullName(objectKey), moduleName, invalidateReason, cubeName, partitionName, batchGuid);
				ProcessManager.StartBatch(batchGuid);
			}
			catch (Exception e)
			{
				throw new InvalidateOlapObjectException(e.Message, e);
			}
		}

        public void InvalidateDimension(string objectKey, string moduleName, InvalidateReason invalidateReason, string dimensionName)
        {
			try
			{
				Guid batchGuid = CreateBatch();
				InvalidateDimension(GetEntityByFullName(objectKey), moduleName, invalidateReason, dimensionName, batchGuid);
				ProcessManager.StartBatch(batchGuid);
			}
			catch (Exception e)
			{
				throw new InvalidateOlapObjectException(e.Message, e);
			}
		}


        public void InvalidatePartition(string objectKey, string moduleName, InvalidateReason invalidateReason, string cubeName, Guid batchGuid)
        {
            InvalidatePartition(GetEntityByFullName(objectKey), moduleName, invalidateReason, cubeName, batchGuid);
        }

        public void InvalidatePartition(string objectKey, string moduleName, InvalidateReason invalidateReason, string cubeName, string partitionName, Guid batchGuid)
        {
            InvalidatePartition(GetEntityByFullName(objectKey), moduleName, invalidateReason, cubeName, partitionName, batchGuid);
        }

        public void InvalidateDimension(string objectKey, string moduleName, InvalidateReason invalidateReason, string dimensionName, Guid batchGuid)
        {
            InvalidateDimension(GetEntityByFullName(objectKey), moduleName, invalidateReason, dimensionName, batchGuid);
        }


        public void InvalidatePartition(IEntity entity, string moduleName, InvalidateReason invalidateReason, string cubeName)
        {
			try
			{
				Guid batchGuid = CreateBatch();
				InvalidatePartition(entity, moduleName, invalidateReason, cubeName, batchGuid);
				ProcessManager.StartBatch(batchGuid);
			}
			catch (Exception e)
			{
				throw new InvalidateOlapObjectException(e.Message, e);
			}
		}

        public void InvalidatePartition(IEntity entity, string moduleName, InvalidateReason invalidateReason, string cubeName, string partitionName)
        {
			try
			{
				Guid batchGuid = CreateBatch();
				InvalidatePartition(entity, moduleName, invalidateReason, cubeName, partitionName, batchGuid);
				ProcessManager.StartBatch(batchGuid);
			}
			catch (Exception e)
			{
				throw new InvalidateOlapObjectException(e.Message, e);
			}
		}

        public void InvalidateDimension(IEntity entity, string moduleName, InvalidateReason invalidateReason, string dimensionName)
        {
			try
			{
				Guid batchGuid = CreateBatch();
				InvalidateDimension(entity, moduleName, invalidateReason, dimensionName, batchGuid);
				ProcessManager.StartBatch(batchGuid);
			}
			catch (Exception e)
			{
				throw new InvalidateOlapObjectException(e.Message, e);
			}
        }

        public void InvalidatePartition(IEntity entity, string moduleName, InvalidateReason invalidateReason, string cubeName, Guid batchGuid)
        {
            InvalidatePartition(entity, moduleName, invalidateReason, cubeName, -1, -1, batchGuid);
        }

        /// <summary>
        /// ������� ���� � ���� ��� �������� � ������������� �������.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="moduleName"></param>
        /// <param name="invalidateReason"></param>
        /// <param name="cubeName"></param>
        /// <param name="pumpID"></param>
        /// <param name="sourceID"></param>
        /// <param name="batchGuid"></param>
        public void InvalidatePartition(IEntity entity, string moduleName, InvalidateReason invalidateReason, string cubeName, int pumpID, int sourceID, Guid batchGuid)
        {
            IMDProcessingProtocol protocol = (IMDProcessingProtocol)this.scheme.GetProtocol(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.Name);
            try
            {
                // ��� ������� �� ������������ ������������ ��������� ���
                if (invalidateReason == InvalidateReason.UserPleasure)
                {
                    string cubeId = OlapDBWrapper.GetCubeIdByName(cubeName);
                    InvalidateCube(cubeId, moduleName, invalidateReason, cubeName, pumpID, sourceID, batchGuid, protocol);
                }
                // ��� ������� �� ������� ��� ���� ���������� � ���� ������ ������� �� FullName
                else
                {
                    foreach (string cubeId in OlapDBWrapper.GetOlapObjectsIdByObjectKey(OlapObjectType.Cube, entity.ObjectKey.ToString()))
                    {
                        InvalidateCube(cubeId, moduleName, invalidateReason, String.Empty, pumpID, sourceID, batchGuid, protocol);
                    }
                }
            }
            finally
            {
                protocol.Dispose();
            }
        }

        /// <summary>
        /// ������� ���� � ���� ��� �������� � ������������� �������.
        /// </summary>
        /// <param name="cubeId"></param>
        /// <param name="moduleName"></param>
        /// <param name="invalidateReason"></param>
        /// <param name="proposedCubeName"></param>
        /// <param name="pumpID"></param>
        /// <param name="sourceID"></param>
        /// <param name="batchGuid"></param>
        /// <param name="protocol"></param>
        public void InvalidateCube(string cubeId, string moduleName, InvalidateReason invalidateReason, string proposedCubeName, int pumpID, int sourceID, Guid batchGuid, IMDProcessingProtocol protocol)
        {
            string cubeName = String.IsNullOrEmpty(proposedCubeName) ? OlapDBWrapper.GetCubeNameById(cubeId) : proposedCubeName;

            if (!String.IsNullOrEmpty(cubeId))
            {
                if (!OlapDBWrapper.Partitions.Rows.Contains(cubeId))
                {
                    throw new ServerException(String.Format("�� ������ ��� � ��������������� \"{0}\", �������� � ���� �� ������ GUID.", cubeId));
                }

                // ���� ������ ��� ��������� � ������, �� ��������� ������ �� ������
                if (OlapDBWrapper.ObjectBlocked(cubeId))
                {
                    protocol.WriteEventIntoMDProcessingProtocol(moduleName,
                        MDProcessingEventKind.mdpeWarning,
                        String.Format(invalidateRefuseMessage, InvalidateReasonToString(invalidateReason)),
                        cubeName, cubeId, OlapObjectType.Cube, batchGuid.ToString());
                    return;
                }

                protocol.WriteEventIntoMDProcessingProtocol(moduleName, MDProcessingEventKind.InvalidateObject,
                    String.Format(invalidateMessage, InvalidateReasonToString(invalidateReason), GetDataPumpInfo(pumpID, sourceID)),
                    cubeName, cubeId, OlapObjectType.Cube, batchGuid.ToString());

                //�������� ��� ������� ����.
                Dictionary<string, IProcessableObjectInfo> partitions = OlapDBWrapper.DS_GetCubePartitions(cubeId);

                //����������, ������� �� ���������� ��������� ������.
                BatchStartPriority batchStartOptions = GetBatchStartOptions(invalidateReason);

                List<IProcessableObjectInfo> usedObjectList = new List<IProcessableObjectInfo>();
                //������������� ������� ������������� �������.
                foreach (IProcessableObjectInfo objectInfo in partitions.Values)
                {
                    if (objectInfo.LastProcessed > SqlDateTime.MinValue.Value ||
                        invalidateReason == InvalidateReason.UserPleasure ||
                        invalidateReason == InvalidateReason.DataPump ||
                        batchStartOptions == BatchStartPriority.Immediately)
                    {
                        usedObjectList.AddRange(OlapDBWrapper.SetNeedProcess(objectInfo, SetNeedProcessOptions.Auto, true));
                    }
                }

                //��� ������������ ������� ��������� � �������.
                foreach (IProcessableObjectInfo usedObject in usedObjectList)
                {
                    if (batchStartOptions == BatchStartPriority.Immediately ||
                                   invalidateReason == InvalidateReason.UserPleasure ||
                                       invalidateReason == InvalidateReason.DataPump)
                                        OlapDBWrapper.AddToBatch(usedObject.ObjectID, batchStartOptions, batchGuid, batchStartOptions);
                }
            }
            else
            {
                protocol.WriteEventIntoMDProcessingProtocol(moduleName,
                    MDProcessingEventKind.mdpeWarning,
                    String.Format("���������� �� ������ ���������, �.�. ������ \"{0}\" �� ������. {1}",
                        cubeName,
                        InvalidateReasonToString(invalidateReason)),
                    cubeName, String.Empty/*partitionId*/, OlapObjectType.Cube, batchGuid.ToString());
            }
        }

        /// <summary>
        /// ���������� ������ ������������ ������ ����. 
        /// ���� ���� ����� ���� � ������ ���� ���������, �� ������������ ������ ��� ������ ����.
        /// </summary>
        /// <param name="cubeName">��� ����.</param>
        /// <param name="partitionId">������������� ������ ����.</param>
        /// <returns>������ ������������.</returns>
        private string GetPartitionFullCaption(string cubeName, string partitionId)
        {
            string objectName = cubeName;
            string subObjectName = (OlapDBWrapper.GetPartitionNameById(partitionId) == String.Empty ? partitionId : OlapDBWrapper.GetPartitionNameById(partitionId));
            if (objectName != subObjectName)
            {
                objectName = String.Format("{0}\\{1}", objectName, subObjectName);
            }
            return objectName;
        }

        /// <summary>
        /// ���������� ������ ������������ ��������� �������.
        /// </summary>
        /// <param name="pumpID"></param>
        /// <param name="sourceID"></param>
        /// <returns></returns>
        private static string GetDataPumpInfo(int pumpID, int sourceID)
        {
            string dataPumpSourceInfo = String.Empty;
            if (sourceID != -1 || pumpID != -1)
            {
                dataPumpSourceInfo = String.Format(". ID ��������� {0}, ID ������� {1}", sourceID, pumpID);
            }
            return dataPumpSourceInfo;
        }

        public void InvalidatePartition(IEntity entity, string moduleName, InvalidateReason invalidateReason, string cubeName, string partitionId, Guid batchGuid)
        {
            InvalidatePartition(entity, moduleName, invalidateReason, cubeName, partitionId, -1, -1, batchGuid);
        }

        public void InvalidateDimension(IEntity entity, string moduleName, InvalidateReason invalidateReason, string dimensionName, Guid batchGuid)
        {
            InvalidateDimension(entity, moduleName, invalidateReason, dimensionName, -1, -1, batchGuid);
        }

        /// <summary>
        /// ������� ������� ���� � ������������� �������.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="moduleName"></param>
        /// <param name="invalidateReason"></param>
        /// <param name="cubeName"></param>
        /// <param name="partitionId"></param>
        /// <param name="pumpID"></param>
        /// <param name="sourceID"></param>
        /// <param name="batchGuid"></param>
        public void InvalidatePartition(IEntity entity, string moduleName, InvalidateReason invalidateReason, string cubeName, string partitionId, int pumpID, int sourceID, Guid batchGuid)
        {
            IMDProcessingProtocol protocol = (IMDProcessingProtocol)this.scheme.GetProtocol(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.Name);
            try
            {
                ProcessableObjectInfo cube = OlapDBWrapper.GetCubeByPartitionId(partitionId);
                string cubeId = cube.ObjectID;
                if (!string.IsNullOrEmpty(cubeId))
                {
                    if (!OlapDBWrapper.Partitions.Rows.Contains(cubeId))
                    {
                        throw new ServerException(String.Format("�� ������ ��� � ��������������� \"{0}\", �������� � ���� �� ������ GUID.", cubeId));
                    }

                    // ���� ������ ��� ��������� � ������, �� ��������� ������ �� ������
                    if (OlapDBWrapper.ObjectBlocked(partitionId))
                    {
                        protocol.WriteEventIntoMDProcessingProtocol(moduleName,
                            MDProcessingEventKind.mdpeWarning,
                            String.Format(invalidateRefuseMessage, InvalidateReasonToString(invalidateReason)),
                            GetPartitionFullCaption(cube.ObjectName, partitionId),
                            cubeId, OlapObjectType.Partition, batchGuid.ToString());
                        return;
                    }

                    protocol.WriteEventIntoMDProcessingProtocol(moduleName,
                        MDProcessingEventKind.InvalidateObject, String.Format(invalidateMessage, InvalidateReasonToString(invalidateReason), GetDataPumpInfo(pumpID, sourceID)),
                        GetPartitionFullCaption(cube.ObjectName, partitionId),
                        cubeId, OlapObjectType.Partition, batchGuid.ToString());

                    //�������� ��� ������� ����.
                    Dictionary<string, IProcessableObjectInfo> partitions = OlapDBWrapper.DS_GetCubePartitions(cubeId);

                    //����������, ������� �� ���������� ��������� ������.
                    BatchStartPriority batchStartOptions = GetBatchStartOptions(invalidateReason);

                    foreach (KeyValuePair<string, IProcessableObjectInfo> item in partitions)
                    {
                        if (item.Value.ObjectID.Equals(partitionId, StringComparison.OrdinalIgnoreCase))
                        {
                            //������������� ������� ������������� �������.
                            IEnumerable<IProcessableObjectInfo> usedObjectList = OlapDBWrapper.SetNeedProcess(item.Value, SetNeedProcessOptions.Auto, true);

                            //��� ������������ ������� ��������� � �������.
                            foreach (IProcessableObjectInfo usedObject in usedObjectList)
                            {
                                if (batchStartOptions == BatchStartPriority.Immediately ||
                                    invalidateReason == InvalidateReason.UserPleasure ||
                                        invalidateReason == InvalidateReason.DataPump)
                                            OlapDBWrapper.AddToBatch(usedObject.ObjectID, batchStartOptions, batchGuid, batchStartOptions);
                            }
                        }
                    }
                }
                else
                {
                    protocol.WriteEventIntoMDProcessingProtocol(moduleName,
                        MDProcessingEventKind.mdpeWarning,
                        String.Format("���������� �� ������ ���������, �.�. ������ \"{0}\" �� ������. {1}",
                            cubeName,
                            InvalidateReasonToString(invalidateReason)),
                        cubeName, String.Empty/*partitionId*/, OlapObjectType.Cube, batchGuid.ToString());
                }
            }
            finally
            {
                protocol.Dispose();
            }
        }

        /// <summary>
        /// ������� ��������� � ������������� �������.
        /// ��� �� ���������� ��� ����, � ������� ������������ ��� ���������.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="moduleName"></param>
        /// <param name="invalidateReason"></param>
        /// <param name="dimensionName"></param>
        /// <param name="pumpID"></param>
        /// <param name="sourceID"></param>
        /// <param name="batchGuid"></param>
        public void InvalidateDimension(IEntity entity, string moduleName, InvalidateReason invalidateReason, string dimensionName, int pumpID, int sourceID, Guid batchGuid)
        {
           InvalidateDimension(entity, moduleName, invalidateReason, dimensionName, pumpID, sourceID, batchGuid, ProcessType.ProcessFull);
        }

        public void InvalidateDimension(IEntity entity, string moduleName, InvalidateReason invalidateReason, string dimensionName, int pumpID, int sourceID, Guid batchGuid, ProcessType processType)
        {
            IMDProcessingProtocol protocol = (IMDProcessingProtocol)this.scheme.GetProtocol(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.Name);
            try
            {
                // ������ ��������������� ����������� �������� ��� ������� ���������� ���������� ������� �������
                List<string> objectsId = new List<string>();

                // ���� ������ ������ �� ������������, �� ���� ������ �� dimensionName (����� ���������� �� dimensionId)
                if (invalidateReason == InvalidateReason.UserPleasure)
                {
                    string dimensionId = OlapDBWrapper.GetDimensionIdByName(dimensionName);
                    objectsId.Add(dimensionId);
                }
                // ���� ������ ������ �� �������, �� ���� ������� �� FullName � ��������� �� ��� � ������ 
                else
                {
                    objectsId.AddRange(OlapDBWrapper.GetOlapObjectsIdByObjectKey(
                        OlapObjectType.Dimension, entity.ObjectKey.ToString()));
                }

                // ���� ������� �� ����� �� ������ � �������, 
                // �� �������� ��� ����� �� FullName.
                // �� ������� �������������� � ������ �� ���� �� ������
                //if (String.IsNullOrEmpty(dimensionId))
                //{
                //    protocol.WriteEventIntoMDProcessingProtocol(moduleName,
                //        MDProcessingEventKind.mdpeWarning,
                //        String.Format(
                //            "���������� �� ������ ���������, �.�. ������ \"{0}\" �� ������ � ������� ��� �� ����� GUID. {1}", 
                //            dimensionName, InvalidateReasonToString(invalidateReason)),
                //        dimensionName, dimensionId, OlapObjectType.Dimension, batchGuid.ToString());
                //    return;
                //}

                //������ ��� �������� ������ ���� ��������, ������� ���������� ����������.
                List<string> objectToProcessKeyList = new List<string>();

                //����������, ������� �� ���������� ��������� ������.
                BatchStartPriority batchStartOptions = GetBatchStartOptions(invalidateReason);

                // ������������ ��� �������
                foreach (string objectId in objectsId)
                {
                    System.Data.DataRow objectRow = OlapDBWrapper.GetOlapObjectsRow(objectId);
                    string objectName = Convert.ToString(objectRow["ObjectName"]);

                    // ���� ������ ��� ��������� � ������, �� ��������� ������ �� ������
                    if (OlapDBWrapper.ObjectBlocked(objectId))
                    {
                        protocol.WriteEventIntoMDProcessingProtocol(moduleName,
                            MDProcessingEventKind.mdpeWarning,
                            String.Format(invalidateRefuseMessage, InvalidateReasonToString(invalidateReason)),
                            objectName, objectId, OlapObjectType.Dimension, batchGuid.ToString());
                        return;
                    }

                    protocol.WriteEventIntoMDProcessingProtocol(moduleName,
                        MDProcessingEventKind.InvalidateObject,
                        String.Format(invalidateMessage, InvalidateReasonToString(invalidateReason), GetDataPumpInfo(pumpID, sourceID)),
                        dimensionName, objectId, OlapObjectType.Dimension, batchGuid.ToString());

                    //������������� ������� ������������� �������.
                    IProcessableObjectInfo poi = OlapDBWrapper.GetProcessableObjectFromDataRow(OlapDBWrapper.Partitions.Rows.Find(objectId));
                    if (poi.LastProcessed > SqlDateTime.MinValue.Value
                        || invalidateReason == InvalidateReason.UserPleasure
                        || batchStartOptions == BatchStartPriority.Immediately)
                    {
                        OlapDBWrapper.SetNeedProcess(poi, SetNeedProcessOptions.Auto, true);
                        OlapDBWrapper.SetProcessType(poi, (int)processType);

                        //�������� � ���� ������ ���� ���������.
                        objectToProcessKeyList.Add(objectId);
                    }

                    //�������� ��� ����, ������������ ������ ���������.
                    Dictionary<string, IProcessableObjectInfo> cubes = OlapDBWrapper.DS_GetDimensionCubes(objectId);

                    foreach (KeyValuePair<string, IProcessableObjectInfo> cube in cubes)
                    {
                        //�������� ��� ������� ������� ����.
                        Dictionary<string, IProcessableObjectInfo> partitions = OlapDBWrapper.DS_GetCubePartitions(cube.Value.ObjectID);
                        Dictionary<string, IProcessableObjectInfo> invalidatePartitions = new Dictionary<string, IProcessableObjectInfo>();

                        //������������� ������� ������������� ������� ������ ��� ������������ ��������
                        foreach (IProcessableObjectInfo partition in partitions.Values)
                        {
                            if (partition.LastProcessed > SqlDateTime.MinValue.Value
                                && partition.State == Microsoft.AnalysisServices.AnalysisState.Processed)
                            {
                                invalidatePartitions.Add(partition.ObjectID, partition);
                            }
                        }
                        if (invalidatePartitions.Count > 0)
                        {
                            OlapDBWrapper.SetNeedProcess(invalidatePartitions.Values, SetNeedProcessOptions.FixedValue, true);
                        }
                    }
                }

                if (batchStartOptions == BatchStartPriority.Immediately ||
                    invalidateReason == InvalidateReason.UserPleasure ||
                    invalidateReason == InvalidateReason.DataPump)
                {
                    OlapDBWrapper.AddToBatch(objectToProcessKeyList, batchStartOptions, batchGuid, batchStartOptions);
                }
            }
            finally
            {
                protocol.Dispose();
            }
        }
        
        public OlapDBWrapper OlapDBWrapper
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                if (scheme.SchemeMDStore.IsAS2005())
                    return OlapDBWrapper2005.GetInstance(scheme.SchemeMDStore.OlapDatabase, scheme);
                else
                    return OlapDBWrapper2000.GetInstance(scheme.SchemeMDStore.OlapDatabase, scheme);
            }
        }

        IOlapDBWrapper IProcessor.OlapDBWrapper
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return this.OlapDBWrapper;
            }
        }

        public IProcessManager ProcessManager
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {                
                if (scheme.SchemeMDStore.IsAS2005())
                    processManagerForDispose = ProcessManager2005.GetInstance(scheme, (Microsoft.AnalysisServices.Server)scheme.SchemeMDStore.OlapDatabase.ServerObject, false);
                else
                    processManagerForDispose = ProcessManager2000.GetInstance(scheme);
            	return processManagerForDispose;
            }
        }

        public IOlapDatabaseGenerator OlapDatabaseGenerator
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {   
                return new OlapDatabaseGenerator2000(scheme);
            }
        }

		#endregion
    }
}
