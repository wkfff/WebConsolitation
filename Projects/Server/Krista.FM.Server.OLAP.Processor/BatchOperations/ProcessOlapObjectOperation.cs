using System;
using System.Collections.Generic;

using Krista.FM.Server.Common;

using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.OLAP.BatchOperations;


namespace Krista.FM.Server.OLAP.Processor.BatchOperations
{
    /// <summary>
    /// Операция обработки многомерного объекта.
    /// </summary>
    internal class ProcessOlapObjectOperation : BatchOperationAbstract
    {
        private readonly IProcessableObjectInfo objectInfo;
        private readonly IProcessor processor;

        /// <summary>
        /// Родительские операции, выполнение которых зависит от этой операции.
        /// </summary>
        readonly Dictionary<string, BatchOperationAbstract> parentOperations = new Dictionary<string, BatchOperationAbstract>();

        /// <summary>
        /// Инициализация экземпляра объекта.
        /// </summary>
        /// <param name="info">Информация об объекте.</param>
        /// <param name="processType">Тип расчета объекта.</param>
        /// <param name="processor">Менеджер расчетов.</param>
        /// <param name="batchId">Идентификатор пакета.</param>
        /// <param name="protocol">Доступ к протоколу.</param>
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
                    "Обработка многомерного объекта \"{0}\", тип обработки: {1}",
                    ObjectInfo.ObjectName, ObjectInfo.ProcessType);
            }
        }

        internal IProcessableObjectInfo ObjectInfo
        {
            get { return objectInfo; }
        }

        /// <summary>
        /// Родительские операции, выполнение которых зависит от этой операции.
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
                Trace.TraceVerbose("{0} Начат расчет объекта \"{1}\"", Authentication.UserDate, ObjectInfo.ObjectName);

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
                    String.Format("Начат расчет объекта. Режим расчета {0}", objectInfo.ProcessType),
                    objectName, ObjectInfo.ObjectID, ObjectInfo.ObjectType, batchId.ToString());

                ObjectInfo.ProcessResult = ((ProcessManager)processor.ProcessManager).ProcessObject(ObjectInfo, batchId);
                
                // При удачном расчете, снимаем флаг необходимости расчета объекта
                if (String.IsNullOrEmpty(ObjectInfo.ProcessResult))
                {
                    ((OlapDBWrapper)processor.OlapDBWrapper).RefreshState(
                        ObjectInfo.ObjectType,
                        ObjectInfo.CubeName,
                        ObjectInfo.ObjectName, 
                        ObjectInfo.ObjectID);
                    
                    ((OlapDBWrapper)processor.OlapDBWrapper).SetNeedProcess(ObjectInfo, SetNeedProcessOptions.FixedValue, false);
                    
                    ObjectInfo.RecordStatus = RecordStatus.ComplitedSuccessful;

                    // Если режим расчета измерения был Rebuild, 
                    // то сбрасываем флаг расчета у всех секций, в которых используется измерение
                    if (ObjectInfo.ObjectType == OlapObjectType.Dimension &&
                        ObjectInfo.ProcessType == Microsoft.AnalysisServices.ProcessType.ProcessFull)
                    {
                        ((OlapDBWrapper)processor.OlapDBWrapper).ResetUsedPartitionsInDimension(ObjectInfo.ObjectID);
                    }

                    Trace.TraceVerbose("{0} Успешно закончен расчет объекта \"{1}\". Время расчета {2}",
                        Authentication.UserDate, objectName, DateTime.Now - startTime);

                    protocol.WriteEventIntoMDProcessingProtocol("Krista.FM.Server.OLAP.Processor",
                        MDProcessingEventKind.mdpeSuccefullFinished,
                        String.Format("Успешно закончен расчет объекта. Время расчета {0}", DateTime.Now - startTime),
                        objectName, ObjectInfo.ObjectID, ObjectInfo.ObjectType, batchId.ToString());
                }
                else
                {
                    // Для зависимых партиций устанавливаем результат расчета
                    foreach (ProcessOlapObjectOperation item in ParentOperations.Values)
                    {
                        item.ObjectInfo.RecordStatus =  RecordStatus.ComplitedWithErrors;
                        item.ObjectInfo.ProcessResult = String.Format("Измерение \"{0}\" вызвало ошибку \"{1}\"",
                            ObjectInfo.ObjectName, ObjectInfo.ProcessResult);
                    }

                    // Для текужего объекта устанавливаем результат расчета
                    ObjectInfo.RecordStatus = RecordStatus.ComplitedWithErrors;

                    Trace.TraceError("{0} Попытка расчета объекта завершилась неудачей: {1}", Authentication.UserDate, ObjectInfo.ProcessResult);

                    protocol.WriteEventIntoMDProcessingProtocol("Krista.FM.Server.OLAP.Processor",
                        MDProcessingEventKind.mdpeError,
                        string.Format("Попытка расчета объекта завершилась неудачей: {0}", ObjectInfo.ProcessResult),
                        objectName, ObjectInfo.ObjectID, ObjectInfo.ObjectType, batchId.ToString());
                }
                return ObjectInfo.ProcessResult;
            }
            catch (Exception e)
            {
                string error = Krista.Diagnostics.KristaDiagnostics.ExpandException(e);
                Trace.TraceCritical("{0} Попытка расчета объекта завершилась неудачей: {1}", Authentication.UserDate, error);
                return error;
            }
            finally
            {
                finishTime = DateTime.Now;
            }
        }
    }
}
