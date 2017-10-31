using System.Data;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Common
{
    public interface IMessageFactory
    {
        DataTable ReceiveMessages();
        MessageAttachmentDTO GetMessageAttachment(int messageId);
        void UpdateMessageStatus(int messageId, MessageStatus status);
        void DeleteMessage(int messageId);
    }
}
