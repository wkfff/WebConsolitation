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
    /// Фасад менеджера расчета кубов.
    /// </summary>
    public sealed class ProcessorClass : DisposableObject, IProcessor
	{
        private static volatile ProcessorClass instance;
		private static readonly object syncRoot = new object();

		private readonly IScheme scheme;
		private ProcessService processService;
		
		/// <summary>
		/// Запоминается исключительно только для детерменированного удаления.
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
		/// Рассчет куба. В зависимости от версии OLAP-сервера (определяется автоматически) используется либо интерфейс DSO (AS2000), либо интерфейс AMO (SSAS2005).
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="processTypes">Как расчитывать.</param>
        /// <param name="name">Что рассчитывать.</param>
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
				string message = String.Format("Куб \"{0}\", ошибка расчета: {1}", name, e.Message);
				Trace.TraceError(message);				
				throw new ServerException(e.Message, e);				
			}
		}        

        /// <summary>
        /// В зависимости от причины вызова определяет должен ли стартовать пакет немедленно.
        /// </summary>
        /// <param name="invalidateReason">Причина вызова.</param>
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
        /// Создает новый пакет расчета объектов.
        /// </summary>
        /// <returns>ID пакета.</returns>
        public Guid CreateBatch()
        {
            return OlapDBWrapper.CreateBatch();
        }

        /// <summary>
        /// Возвращает состояние пакета.
        /// Метод необходим для определения результата асинхронной операции обработки пакета.
        /// </summary>
        /// <param name="batchId">ID пакета.</param>
        /// <returns>Состояние пакета.</returns>
        public BatchState GetBatchState(Guid batchId)
        {
            return OlapDBWrapper.GetBatchState(batchId.ToString());
        }

        /// <summary>
        /// Удаляет пакет находящийся в состоянии создания.
        /// </summary>
        /// <remarks>
        /// Метод должен вызываться если в процессе формирования пакета произошла ошибка.
        /// Если пакет имеет состояние отличное от "Создан", то метод вернет исключение.
        /// </remarks>
        /// <param name="batchId">ID пакета.</param>
        public void RevertBatch(Guid batchId)
        {
            OlapDBWrapper.DeleteBatch(batchId);
        }

        private const string invalidateMessage = "Требование на расчет. {0}{1}";
        private const string invalidateRefuseMessage = "Требование на расчет отклонено, т.к. объект уже находится в пакете ожидая обработки или уже обрабатывается. {0}";

        private static string InvalidateReasonToString(InvalidateReason invalidateReason)
        {
            string text;
            switch (invalidateReason)
            {
                case InvalidateReason.AssociationChanged:
                    text = "Изменение сопоставления";
                    break;
                case InvalidateReason.ClassifierChanged:
                    text = "Изменение классификатора/таблицы";
                    break;
                case InvalidateReason.DataPump:
                    text = "Закачка данных";
                    break;
                case InvalidateReason.UserPleasure:
                    text = "Запуск расчета пользователем";
                    break;
                case InvalidateReason.WriteBack:
                    text = "Обратная запись";
                    break;
                default:
                    text = "Не определено";
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
                throw new Exception(String.Format("Объект \"{0}\" в схеме не найден.", objectKey));
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
        /// Пометка куба и всех его разделов о необходимости расчета.
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
                // При запросе от пользователя рассчитываем указанный куб
                if (invalidateReason == InvalidateReason.UserPleasure)
                {
                    string cubeId = OlapDBWrapper.GetCubeIdByName(cubeName);
                    InvalidateCube(cubeId, moduleName, invalidateReason, cubeName, pumpID, sourceID, batchGuid, protocol);
                }
                // При запросе от системы имя куба игнорируем и ишем нужные объекты по FullName
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
        /// Пометка куба и всех его разделов о необходимости расчета.
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
                    throw new ServerException(String.Format("Не найден куб с идентификатором \"{0}\", возможно у куба не указан GUID.", cubeId));
                }

                // Если объект уже находится в пакете, то откланяем запрос на расчет
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

                //Получаем все разделы куба.
                Dictionary<string, IProcessableObjectInfo> partitions = OlapDBWrapper.DS_GetCubePartitions(cubeId);

                //Определяем, следует ли немедленно запускать расчет.
                BatchStartPriority batchStartOptions = GetBatchStartOptions(invalidateReason);

                List<IProcessableObjectInfo> usedObjectList = new List<IProcessableObjectInfo>();
                //Устанавливаем признак необходимости расчета.
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

                //Все используемые объекты нуждаются в расчете.
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
                    String.Format("Требование на расчет отклонено, т.к. объект \"{0}\" не найден. {1}",
                        cubeName,
                        InvalidateReasonToString(invalidateReason)),
                    cubeName, String.Empty/*partitionId*/, OlapObjectType.Cube, batchGuid.ToString());
            }
        }

        /// <summary>
        /// Возвращает полное наименование секции куба. 
        /// Если если имена куба и секции куба совпадают, то возвращается только имя секции куба.
        /// </summary>
        /// <param name="cubeName">Имя куба.</param>
        /// <param name="partitionId">Идентификатор секции куба.</param>
        /// <returns>Полное наименование.</returns>
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
        /// Возвращает строку определяющую параметры закачки.
        /// </summary>
        /// <param name="pumpID"></param>
        /// <param name="sourceID"></param>
        /// <returns></returns>
        private static string GetDataPumpInfo(int pumpID, int sourceID)
        {
            string dataPumpSourceInfo = String.Empty;
            if (sourceID != -1 || pumpID != -1)
            {
                dataPumpSourceInfo = String.Format(". ID источника {0}, ID закачки {1}", sourceID, pumpID);
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
        /// Пометка раздела куба о необходимости расчета.
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
                        throw new ServerException(String.Format("Не найден куб с идентификатором \"{0}\", возможно у куба не указан GUID.", cubeId));
                    }

                    // Если объект уже находится в пакете, то отклоняем запрос на расчет
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

                    //Получаем все разделы куба.
                    Dictionary<string, IProcessableObjectInfo> partitions = OlapDBWrapper.DS_GetCubePartitions(cubeId);

                    //Определяем, следует ли немедленно запускать расчет.
                    BatchStartPriority batchStartOptions = GetBatchStartOptions(invalidateReason);

                    foreach (KeyValuePair<string, IProcessableObjectInfo> item in partitions)
                    {
                        if (item.Value.ObjectID.Equals(partitionId, StringComparison.OrdinalIgnoreCase))
                        {
                            //Устанавливаем признак необходимости расчета.
                            IEnumerable<IProcessableObjectInfo> usedObjectList = OlapDBWrapper.SetNeedProcess(item.Value, SetNeedProcessOptions.Auto, true);

                            //Все используемые объекты нуждаются в расчете.
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
                        String.Format("Требование на расчет отклонено, т.к. объект \"{0}\" не найден. {1}",
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
        /// Пометка измерения о необходимости расчета.
        /// Так же помечаются все кубы, в которых используется это измерение.
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
                // Список идентификаторов многомерных объектов для которых необходимо установить признак расчета
                List<string> objectsId = new List<string>();

                // Если запрос пришел от пользователя, то ищем объект по dimensionName (нужно переделать на dimensionId)
                if (invalidateReason == InvalidateReason.UserPleasure)
                {
                    string dimensionId = OlapDBWrapper.GetDimensionIdByName(dimensionName);
                    objectsId.Add(dimensionId);
                }
                // Если запрос пришел от системы, то ищем объекты по FullName и добавляем их все в список 
                else
                {
                    objectsId.AddRange(OlapDBWrapper.GetOlapObjectsIdByObjectKey(
                        OlapObjectType.Dimension, entity.ObjectKey.ToString()));
                }

                // Если объекта по имени не найден в реестре, 
                // то пытаемся его найти по FullName.
                // то выводим предупреждение и больше ни чего не делаем
                //if (String.IsNullOrEmpty(dimensionId))
                //{
                //    protocol.WriteEventIntoMDProcessingProtocol(moduleName,
                //        MDProcessingEventKind.mdpeWarning,
                //        String.Format(
                //            "Требование на расчет отклонено, т.к. объект \"{0}\" не найден в системе или не имеет GUID. {1}", 
                //            dimensionName, InvalidateReasonToString(invalidateReason)),
                //        dimensionName, dimensionId, OlapObjectType.Dimension, batchGuid.ToString());
                //    return;
                //}

                //Список для хранения ключей всех объектов, которые необходимо рассчитать.
                List<string> objectToProcessKeyList = new List<string>();

                //Определяем, следует ли немедленно запускать расчет.
                BatchStartPriority batchStartOptions = GetBatchStartOptions(invalidateReason);

                // Обрабатываем все объекты
                foreach (string objectId in objectsId)
                {
                    System.Data.DataRow objectRow = OlapDBWrapper.GetOlapObjectsRow(objectId);
                    string objectName = Convert.ToString(objectRow["ObjectName"]);

                    // Если объект уже находится в пакете, то отклоняем запрос на расчет
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

                    //Устанавливаем признак необходимости расчета.
                    IProcessableObjectInfo poi = OlapDBWrapper.GetProcessableObjectFromDataRow(OlapDBWrapper.Partitions.Rows.Find(objectId));
                    if (poi.LastProcessed > SqlDateTime.MinValue.Value
                        || invalidateReason == InvalidateReason.UserPleasure
                        || batchStartOptions == BatchStartPriority.Immediately)
                    {
                        OlapDBWrapper.SetNeedProcess(poi, SetNeedProcessOptions.Auto, true);
                        OlapDBWrapper.SetProcessType(poi, (int)processType);

                        //Включаем в этот список само измерение.
                        objectToProcessKeyList.Add(objectId);
                    }

                    //Получаем все кубы, использующие данное измерение.
                    Dictionary<string, IProcessableObjectInfo> cubes = OlapDBWrapper.DS_GetDimensionCubes(objectId);

                    foreach (KeyValuePair<string, IProcessableObjectInfo> cube in cubes)
                    {
                        //Получаем все разделы данного куба.
                        Dictionary<string, IProcessableObjectInfo> partitions = OlapDBWrapper.DS_GetCubePartitions(cube.Value.ObjectID);
                        Dictionary<string, IProcessableObjectInfo> invalidatePartitions = new Dictionary<string, IProcessableObjectInfo>();

                        //Устанавливаем признак необходимости расчета только для рассчитанных объектов
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
