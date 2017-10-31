using System;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.OLAP.BatchOperations
{
    /// <summary>
    /// �������� ��� ���������� �������������� �������� ��� ������� ������� ����������� ���� ������.
    /// </summary>
    public class ProcessDatabaseTableOperation : BatchOperationAbstract
    {
        private IProcessableObjectInfo objectInfo;
        /// <summary>
        /// ������ �����
        /// </summary>
        private IScheme scheme;
        /// <summary>
        /// ��� ������� ��� ����������
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
                    "�������������� �������� ��� ������� \"{0}\"",
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
                    // ��� ��������� �������� �� ���� �� ������
                    return String.Empty;
                }
                else if (String.IsNullOrEmpty(objectInfo.ObjectKey))
                {
                    // ��� �������� �� ����������� � ����� ���� �� ���� �� ������
                    return String.Empty;
                }
                else
                {
                    throw new ServerException(String.Format(
                        "���������� ��������� ��������������� ��������� ������� ���� ������, " +
                        "�.�. ��� ������������ ������� \"{0}\" �� ������ ��������������� ������ \"{1}\" � �����.",
                        objectInfo.ObjectName, objectInfo.FullName));
                }

                if (entity.ProcessObjectData())
                {
                    Trace.TraceVerbose(
                        "{0} ����� ���������� �������������� �������� ��� ������� \"{1}\" {2}",
                        Authentication.UserDate, objectName, DateTime.Now - startTime);

                    protocol.WriteEventIntoMDProcessingProtocol("Krista.FM.Server.OLAP.BatchOperations",
                        MDProcessingEventKind.mdpeInformation,
                        String.Format("����� ���������� �������������� �������� {0}", DateTime.Now - startTime),
                        objectName, objectInfo.ObjectID, objectInfo.ObjectType, batchId.ToString());
                }

                return String.Empty;
            }
            catch(Exception e)
            {
                string errorMessage = String.Format("������ ��� ���������� ���.�������� ��� ������� \"{0}\": {1}",
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
