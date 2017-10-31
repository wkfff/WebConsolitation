using System;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.OLAP.BatchOperations
{
    /// <summary>
    /// Операция для выполнения дополнительных действий над данными таблицы реляционной базы данных.
    /// </summary>
    public class ProcessDatabaseTableOperation : BatchOperationAbstract
    {
        private IProcessableObjectInfo objectInfo;
        /// <summary>
        /// Объект схемы
        /// </summary>
        private IScheme scheme;
        /// <summary>
        /// Имя объекта для протоколов
        /// </summary>
        private string objectName;

        public ProcessDatabaseTableOperation(
            IProcessableObjectInfo info,
            IScheme scheme,
            Guid batchId, 
            IMDProcessingProtocol protocol,
            string objectName)
            : base(batchId, protocol)
        {
            objectInfo = info;
            this.scheme = scheme;
            this.objectName = objectName;
        }

        public override string Name
        {
            get
            {
                return String.Format(
                    "Дополнительные операции для объекта \"{0}\"",
                    objectInfo.ObjectName);
            }
        }

        public override string Execute()
        {
            startTime = DateTime.Now;

            try
            {
                IEntity entity;
                if (scheme.Classifiers.ContainsKey(objectInfo.ObjectKey))
                {
                    entity = scheme.Classifiers[objectInfo.ObjectKey];
                }
                else if (scheme.FactTables.ContainsKey(objectInfo.ObjectKey))
                {
                    entity = scheme.FactTables[objectInfo.ObjectKey];
                }
                else if (objectInfo.FullName == "fx.System.DataSources")
                {
                    // Для системных объектов ни чего не делаем
                    return String.Empty;
                }
                else if (String.IsNullOrEmpty(objectInfo.ObjectKey))
                {
                    // Для объектов не привязанных к схеме тоже ни чего не делаем
                    return String.Empty;
                }
                else
                {
                    throw new ServerException(String.Format(
                        "Невозможно выполнить предварительную обработку таблицы базы данных, " +
                        "т.к. для многомерного объекта \"{0}\" не найден соответствующий объект \"{1}\" в схеме.",
                        objectInfo.ObjectName, objectInfo.FullName));
                }

                if (entity.ProcessObjectData())
                {
                    Trace.TraceVerbose(
                        "{0} Время выполнения дополнительных операций для объекта \"{1}\" {2}",
                        Authentication.UserDate, objectName, DateTime.Now - startTime);

                    protocol.WriteEventIntoMDProcessingProtocol("Krista.FM.Server.OLAP.BatchOperations",
                        MDProcessingEventKind.mdpeInformation,
                        String.Format("Время выполнения дополнительных операций {0}", DateTime.Now - startTime),
                        objectName, objectInfo.ObjectID, objectInfo.ObjectType, batchId.ToString());
                }

                return String.Empty;
            }
            catch(Exception e)
            {
                string errorMessage = String.Format("Ошибка при выполнении доп.операций для объекта \"{0}\": {1}",
                    objectInfo.ObjectName,
                    Krista.Diagnostics.KristaDiagnostics.ExpandException(e));
                
                Trace.TraceError(errorMessage);

                protocol.WriteEventIntoMDProcessingProtocol(
                    "Krista.FM.Server.OLAP.BatchOperations",
                    MDProcessingEventKind.mdpeError,
                    Krista.Diagnostics.KristaDiagnostics.ExpandException(e),
                    objectInfo.ObjectName, objectInfo.ObjectID, objectInfo.ObjectType, batchId.ToString());

                return errorMessage;
            }
            finally
            {
                finishTime = DateTime.Now;
            }
        }
    }
}
