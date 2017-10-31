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
    /// Базовый класс, управляющий процессом расчета пакетов.
    /// </summary>
    public abstract class ProcessManager : DisposableObject, IProcessManager
    {
        protected IScheme scheme;
        protected IMDProcessingProtocol protocol;
        protected OlapDBWrapper olapDBWrapper;

        protected ProcessMode processMode;
        protected TransactionMode transactionMode;

        /// <summary>
        /// Базовый класс, управляющий процессом расчета пакетов.
        /// </summary>
        protected ProcessManager(IScheme scheme)
        {               
            this.scheme = scheme;
            this.olapDBWrapper = ((ProcessorClass)scheme.Processor).OlapDBWrapper;
            this.protocol = (IMDProcessingProtocol)this.scheme.GetProtocol(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.Name);            
        }

        /// <summary>
        /// Расчитывает объект с заданным идентификатором. Возвращает результат расчета в виде строки.
        /// Если расчет успешен - строка пустая.
        /// </summary>
        /// <returns></returns>
        internal abstract string ProcessObject(IProcessableObjectInfo item, Guid batchID);

        /// <summary>
        /// Формирует список операций для пакета.
        /// </summary>
        /// <param name="batchGuid">Идентификатор пакета.</param>
        /// <param name="items">Список многомерных объектов.</param>
        /// <returns>Список операций пакета.</returns>
        private Dictionary<string, BatchOperationAbstract> PrepareBatchOperations(Guid batchGuid, IEnumerable<IProcessableObjectInfo> items)
        {
            Dictionary<string, BatchOperationAbstract> batchOperations = new Dictionary<string, BatchOperationAbstract>();

            //
            // Операции дополнительной обработки данных в базе для рассчитываемых объектов
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
            // Добавляем операции обработки измерений
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
            // Добавляем измерения используемые в секциях кубов
            //
            Dictionary<string, BatchOperationAbstract> partitionsOperations = new Dictionary<string, BatchOperationAbstract>();
            List<IProcessableObjectInfo> additionalItems = new List<IProcessableObjectInfo>();
            foreach (IProcessableObjectInfo item in items)
            {
                if (item.ObjectType == OlapObjectType.Partition)
                {
                    // Создаем операцию для расчета секции куба
                    ProcessOlapObjectOperation partitionOperation = new ProcessOlapObjectOperation(
                            item,
                            Microsoft.AnalysisServices.ProcessType.ProcessFull,
                            scheme.Processor,
                            batchGuid,
                            protocol);

                    // Для каждой записи раздела получаем используемые измерения. 
                    // для неявного расчета
                    Dictionary<string, IProcessableObjectInfo> dimensions = olapDBWrapper.DS_GetCubeDimensions(item.CubeId);
                    foreach (KeyValuePair<string, IProcessableObjectInfo> dimension in dimensions)
                    {
                        if (!batchOperations.ContainsKey(dimension.Key))
                        {
                            if (dimension.Value.State == Microsoft.AnalysisServices.AnalysisState.Unprocessed 
                                || ((ProcessableObjectInfo)dimension.Value).NeedProcess)
                            {
                                // Создаем операцию дополнительной обработки данных в базе для измерения
                                if (!schemeObjects.Contains(dimension.Value.ObjectKey))
                                {
                                    schemeObjects.Add(dimension.Value.ObjectKey);

                                    batchOperations.Add("!" + dimension.Value.ObjectID,
                                                        new ProcessDatabaseTableOperation(dimension.Value, scheme,
                                                                                          batchGuid, protocol, GetObjectName(dimension.Value)));
                                }

                                // Создаем операцию расчета измерения
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
            // Добавляем секции кубов
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
        /// Пользовательские дополнительные обработки
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
                    // обязательные параметры для дополнительной операции
                    object[] parameters = { batchGuid, protocol, scheme };
                    batchOperations.Add ("!!" + objectInfo.ObjectID,
                                            (BatchOperationAbstract)Activator.CreateInstance(Type.GetType(operation), parameters));
                }
                catch (Exception e)
                {
                    Trace.TraceError(
                        string.Format(
                            "Для объекта {0} не создана пользовательская задача с типом {1}. Исключение : {2}",
                            objectInfo.ObjectKey, operation, e.Message));

                    protocol.WriteEventIntoMDProcessingProtocol(
                        "Krista.FM.Server.OLAP.Processor",
                        MDProcessingEventKind.mdpeWarning,
                        string.Format("Для объекта {0} не создана пользовательская задача с типом {1}. Исключение : {2}",
                                        objectInfo.ObjectKey, operation, e.Message),
                        "Формирование задач пакета",
                        "Формирование задач пакета",
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
        /// Отправляет операции пакета на выполнение.
        /// </summary>
        /// <param name="batchOperations">Список операций пакета.</param>
        /// <returns>Результат выполнения: если пустая строка, то все операции выполнены успешно, иначе текст ошибки.</returns>
        /// <param name="batchGuid">Идентификатор пакета</param>
        /// <param name="priority"> Приоритет выполнения пакета</param>
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
        /// Обновляет хеш листа плпнирования.
        /// </summary>
        /// <param name="batchOperations">Список операций выполненых в пакете.</param>
        /// <param name="batchGuid">Идентификатор пакета.</param>
        private void RefreshPlaningHash(IDictionary<string, BatchOperationAbstract> batchOperations, Guid batchGuid)
        {
            List<string> dimensionsNames = new List<string>();
            List<string> cubesNames = new List<string>();

            foreach (BatchOperationAbstract item in batchOperations.Values)
            {
                if (item is ProcessOlapObjectOperation)
                {
                    ProcessOlapObjectOperation operation = (ProcessOlapObjectOperation)item;

                    // Обрабатываем только успешно выполненные операции
                    //if (operation.ObjectInfo.RecordStatus == RecordStatus.ComplitedSuccessful)
                    {
                        if (operation.ObjectInfo.ObjectType == OlapObjectType.Dimension)
                        {
                            if (!dimensionsNames.Contains(operation.ObjectInfo.ObjectName))
                            {
                                dimensionsNames.Add(operation.ObjectInfo.ObjectName);
                            }

                            // обновляем кубы, в которых использовалось измерение
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
                    Trace.TraceVerbose("{0} Обновление кэша измерений листа планирования ({1}).", Authentication.UserDate, dimensionsNames.Count);

                    protocol.WriteEventIntoMDProcessingProtocol("Krista.FM.Server.OLAP.Processor",
                        MDProcessingEventKind.mdpeInformation,
                        String.Format("Обновление кэша измерений листа планирования ({0}).", dimensionsNames.Count),
                        "Кэш листа планирования",
                        "Кэш листа планирования",
                        OlapObjectType.Database,
                        batchGuid.ToString());
                    scheme.PlaningProvider.RefreshDimension("0", dimensionsNames.ToArray());
                }
                if (cubesNames.Count > 0)
                {
                    Trace.TraceVerbose("{0} Обновление кэша кубов листа планирования ({1}).", Authentication.UserDate, cubesNames.Count);

                    protocol.WriteEventIntoMDProcessingProtocol("Krista.FM.Server.OLAP.Processor",
                        MDProcessingEventKind.mdpeInformation,
                        String.Format("Обновление кэша кубов листа планирования ({0}).", cubesNames.Count),
                        "Кэш листа планирования",
                        "Кэш листа планирования",
                        OlapObjectType.Database,
                        batchGuid.ToString());
                    scheme.PlaningProvider.RefreshCube("0", cubesNames.ToArray());
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("При обновлении кэша листа планирования произошла ошибка: {0}", Krista.Diagnostics.KristaDiagnostics.ExpandException(e));

                protocol.WriteEventIntoMDProcessingProtocol(
                    "Krista.FM.Server.OLAP.Processor",
                    MDProcessingEventKind.mdpeWarning,
                    String.Format("При обновлении кэша листа планирования произошла ошибка: {0}", Krista.Diagnostics.KristaDiagnostics.ExpandException(e)),
                    "Кэш листа планирования",
                    "Кэш листа планирования",
                    OlapObjectType.Database,
                    batchGuid.ToString());
            }
        }

        /// <summary>
        /// Запуск пакета на расчет.
        /// </summary>
        /// <param name="batchGuid">Идентификатор пакета.</param>
        /// <param name="priority"> Приоритет пакета</param>
        internal void StartProcessBatch(Guid batchGuid, BatchStartPriority priority)
        {
            Trace.TraceVerbose("{0} Начат расчет пакета \"{1}\"", Authentication.UserDate, batchGuid);

            protocol.WriteEventIntoMDProcessingProtocol("Krista.FM.Server.OLAP.Processor",
                MDProcessingEventKind.mdpeStart, "Начат расчет пакета", "Пакет",
                batchGuid.ToString(), OlapObjectType.Database, batchGuid.ToString());

            //Присваиваем записям в пакете RecordStatus = inProcess.
            IEnumerable<IProcessableObjectInfo> items = olapDBWrapper.StartProcessBatch(batchGuid, SessionContext.SessionId);

            // Формирование операций пакета
            Dictionary<string, BatchOperationAbstract> batchOperations = PrepareBatchOperations(batchGuid, items);

            // Запуск операций на выполнение
            string processResult = ExecuteBatchOperations(batchOperations, batchGuid, priority);

            // Обновляем метаданные листа планирования
            try
            {
                RefreshPlaningHash(batchOperations, batchGuid);
            }
            catch (Exception e)
            {
                processResult = String.Format("{0}\n{1}", processResult, Krista.Diagnostics.KristaDiagnostics.ExpandException(e));
            }

            //
            // Завершение обработки пакета
            //
            Trace.TraceVerbose("{0} Расчет пакета завершен {1}", Authentication.UserDate, String.IsNullOrEmpty(processResult) ? String.Empty : ". ", processResult);

            protocol.WriteEventIntoMDProcessingProtocol(
                "Krista.FM.Server.OLAP.Processor",
                String.IsNullOrEmpty(processResult) ? MDProcessingEventKind.mdpeSuccefullFinished : MDProcessingEventKind.mdpeFinishedWithError,
                String.Format("Расчет пакета завершен{0}{1}",
                    String.IsNullOrEmpty(processResult) ? String.Empty : ". ", processResult),
                "Пакет",
                batchGuid.ToString(),
                OlapObjectType.Database, batchGuid.ToString());

            olapDBWrapper.ComplitBatch(batchGuid,
                String.IsNullOrEmpty(processResult) ? BatchState.Complited : BatchState.ComplitedWithError,
                items);
        }

        #region IProcessManager Members

        public void StartBatch(Guid batchGuid)
        {
            //Присваиваем записям в пакете RecordStatus = inProcess.
            olapDBWrapper.StartBatch(batchGuid, SessionContext.SessionId);
            ProcessService.newRequestEvent.Set();
        }

        public virtual void StopBatch(Guid batchGuid)
        {
            Trace.WriteLine("Метод \"ProcessManager2000.StopBatch()\" не реализован.");
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
