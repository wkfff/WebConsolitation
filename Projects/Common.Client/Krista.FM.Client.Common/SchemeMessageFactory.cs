using System;
using System.Data;
using System.Diagnostics;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Common
{
    public class SchemeMessageFactory : IMessageFactory
    {
        private readonly IScheme scheme;
        private readonly int userId;

        public SchemeMessageFactory(IScheme scheme, int userId)
        {
            this.scheme = scheme;
            this.userId = userId;
        }

        public DataTable ReceiveMessages()
        {
            try
            {
                DataTable dataTable = new DataTable();

                dataTable.Columns.Add("ID", typeof(int));
                dataTable.Columns.Add("Subject", typeof(string));
                dataTable.Columns.Add("RefUserSender", typeof(string));
                dataTable.Columns.Add("MessageImportance", typeof(int));
                dataTable.Columns.Add("MessageStatus", typeof(int));
                dataTable.Columns.Add("MessageType", typeof(int));
                dataTable.Columns.Add("RefMessageAttachment", typeof(int));
                dataTable.Columns.Add("ReceivedDate", typeof(DateTime));
                dataTable.Columns.Add("TransferLink", typeof(string));
                dataTable.Columns.Add("SelectBoxColumn", typeof(bool));

                dataTable.PrimaryKey = new[] {dataTable.Columns["ID"]};

                foreach (var receiveMessage in scheme.MessageManager.ReceiveMessages(userId))
                {
                    DataRow row = dataTable.NewRow();

                    row["ID"] = receiveMessage.ID;
                    row["Subject"] = receiveMessage.Subject;
                    row["RefUserSender"] = receiveMessage.RefUserSender;
                    row["MessageImportance"] = receiveMessage.MessageImportance;
                    row["MessageStatus"] = receiveMessage.MessageStatus;
                    row["MessageType"] = receiveMessage.MessageType;
                    row["RefMessageAttachment"] = receiveMessage.RefMessageAttachment;
                    row["ReceivedDate"] = receiveMessage.ReceivedDate;
                    row["TransferLink"] = receiveMessage.TransferLink;
                    row["SelectBoxColumn"] = false;

                    dataTable.Rows.Add(row);
                }

                return dataTable;
            }
            catch (Exception e)
            {
                Trace.TraceError(String.Format("При получении сообщений возникло исключение: {0}", e.Message));
                return null;
            }
        }

        public MessageAttachmentDTO GetMessageAttachment(int messageId)
        {
            return scheme.MessageManager.GetMessageAttachment(messageId);
        }

        public void UpdateMessageStatus(int messageId, MessageStatus status)
        {
            scheme.MessageManager.UpdateMessage(messageId, status);
        }

        public void DeleteMessage(int messageId)
        {
            scheme.MessageManager.DeleteMessage(messageId);
        }
    }
}
