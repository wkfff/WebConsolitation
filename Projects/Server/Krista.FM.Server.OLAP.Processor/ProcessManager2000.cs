using System;
using System.Threading;
using DSO;

using Krista.FM.Common;
using Krista.FM.Server.Common;

using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.OLAP.Processor
{
    public class ProcessManager2000 : ProcessManager
    {        
        private static volatile ProcessManager2000 instance;
        private static readonly object syncRoot = new object();

        private string errorMessage = string.Empty;
        private AutoResetEvent autoEvent;
                
        [System.Diagnostics.DebuggerStepThrough]
        public static ProcessManager2000 GetInstance(IScheme scheme)
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new ProcessManager2000(scheme);
                    }
                }
            }
            return instance;
        }

        protected ProcessManager2000(IScheme scheme)
            : base(scheme)
        {               
        }

        /// <summary>
        /// ����������� ������ � �������� ���������������. ���������� ��������� ������� � ���� ������.
        /// ���� ������ ������� - ������ ������.
        /// </summary>
        /// <returns></returns>
        internal override string ProcessObject(IProcessableObjectInfo item, Guid bathcID)
        {
            errorMessage = string.Empty;

            autoEvent = new AutoResetEvent(false);

            DSO.Database database = (DSO.Database)olapDBWrapper.OlapDatabase.DatabaseObject;

            // �������� �� �������� ��������� �� ������
            database.ReportError += Database_ReportError;

            database.ReportAfter += database_ReportAfter;

            if (null != item)
            {
                try
                {
                    Trace.TraceVerbose("{0} ����� ������ ������� {1} \"{2}\". ����� ������� {3}",
                        Authentication.UserDate, item.ObjectType, item.ObjectName, item.ProcessType);

                    DSO.ProcessTypes _processType = ProcessorUtils.ConvertToAS2000ProcessType(item.ProcessType);
                    switch (item.ObjectType)
                    {                        
                        case OlapObjectType.Partition:
                            Cube cube = (Cube)database.Cubes.Item(item.CubeName);
                            if (null != cube)
                            {
                                Partition partition = (Partition)cube.Partitions.Item(item.ObjectName);
                                if (partition != null)
                                {
                                    try
                                    {
                                        partition.LockObject(OlapLockTypes.olapLockProcess,
                                                             "���������� ��� ������� ��������");

                                        partition.Process(_processType);

                                        autoEvent.WaitOne();

                                        item.State = ProcessorUtils.ConvertToAS2005State(partition.State);
                                        item.LastProcessed = partition.LastProcessed;
                                        item.RecordStatus = RecordStatus.ComplitedSuccessful;
                                    }
                                    finally
                                    {
                                        partition.UnlockObject();
                                    }
                                }
                                else
                                {
                                    throw new ServerException(String.Format("������ \"{0}\" �� ������� � ����������� ���� ������.", item.ObjectName));
                                }
                            }
                            break;
                        case OlapObjectType.Dimension:
                            Dimension dimension = (Dimension)database.Dimensions.Item(item.ObjectName);
                            if (dimension != null)
                            {
                                try
                                {
                                    dimension.LockObject(OlapLockTypes.olapLockProcess,
                                                         "���������� ��� ������� ���������");

                                    dimension.Process(_processType);
                                    autoEvent.WaitOne();

                                    item.State = ProcessorUtils.ConvertToAS2005State(dimension.State);
                                    item.LastProcessed = dimension.LastProcessed;
                                    item.RecordStatus = RecordStatus.ComplitedSuccessful;
                                }
                                finally
                                {
                                    dimension.UnlockObject();
                                }
                            }
                            else
                            {
                                throw new ServerException(String.Format("��������� \"{0}\" �� ������� � ����������� ���� ������.", item.ObjectName));
                            }
                            break;
                        default:
                            break;
                    }

                    Trace.TraceVerbose("{0} ������ ������� �������� {1} \"{2}\" {3}", Authentication.UserDate, item.ObjectType, item.ObjectName, item.RecordStatus);

                    string messageSubject = String.Format("������� �������� ������ ������� {0}",
                                                            item.ObjectName);
                    MessageWrapper messageWrapper = new MessageWrapper();
                    messageWrapper.Subject = messageSubject;
                    messageWrapper.MessageType = MessageType.CubesManagerMessage;
                    messageWrapper.MessageImportance = MessageImportance.Importance;
                    messageWrapper.SendAll = true;
                    messageWrapper.TransferLink = bathcID.ToString();

                    scheme.MessageManager.SendMessage(messageWrapper);
                }
                catch (Exception e)
                {
                    Trace.TraceError("{0} ������ ������� �������� � �������: {1} \"{2}\" {3}", Authentication.UserDate, item.ObjectType, item.ObjectName, e.Message);

                    // ��������� ������� ������ ���������� �� ������ � ��������, ���� ��������� �� ����������� ��������� ������
                    if (String.IsNullOrEmpty(e.Message) || String.Equals(e.Message, "Process Canceled") || String.Equals(e.Message, "Process operation failed"))
                        return String.Format("������ ������� �������� � ������� {0} \"{1}\" {2}. ��� ��������� ���������� ��������� �� ������ ������������ ������ {3} � Analysis Manager. \n ������������� ������: /*!!{4}!!*/"
                            , Authentication.UserDate, item.ObjectType, errorMessage, item.ObjectName, bathcID);

                    string message = String.Format("{0} \n������������� ������: /*!!{1}!!*/", e.Message, bathcID);

                    string messageSubject = String.Format("������ ��� ������� ������������ ������� {0}",
                                                           item.ObjectName);
                    MessageWrapper messageWrapper = new MessageWrapper();
                    messageWrapper.Subject = messageSubject;
                    messageWrapper.MessageType = MessageType.CubesManagerMessage;
                    messageWrapper.MessageImportance = MessageImportance.Importance;
                    messageWrapper.SendAll = true;
                    messageWrapper.TransferLink = bathcID.ToString();

                    scheme.MessageManager.SendMessage(messageWrapper);

                    return message;
                }
            }
            return string.Empty;
        }

        void database_ReportAfter(ref object obj, short Action, bool success)
        {
            autoEvent.Set();
        }

        void Database_ReportError(ref object obj, short Action, int ErrorCode, string Message, ref bool Cancel)
        {
            if (!string.IsNullOrEmpty(Message) && String.IsNullOrEmpty(errorMessage))
            {
                Cancel = true;

                if (ErrorHandler.Instance.Errors.ContainsKey(ErrorCode))
                {
                    BaseError error =
                        (BaseError)ErrorHandler.Instance.Errors[ErrorCode].Assembly.CreateInstance(
                            ErrorHandler.Instance.Errors[ErrorCode].FullName, false, System.Reflection.BindingFlags.CreateInstance,
                                null, new object[] { ErrorCode }, null, null);

                    errorMessage = error.Execute(Message);
                }
                else
                    errorMessage = String.Format("{0}. ��� ������: {1}", Message, ErrorCode);
            }
        }

    }
}