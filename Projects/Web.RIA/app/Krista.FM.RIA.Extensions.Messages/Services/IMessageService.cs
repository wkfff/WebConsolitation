using System.Collections.Generic;
using System.Data;
using Krista.FM.Domain;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Messages.Services
{
    public interface IMessageService
    {
        DataTable GroupsTable { get; }

        DataTable UsersTable { get; }

        SendMessageStatus SendMessage(MessageWrapper messageWrapper, List<int> groupList, List<int> userList);

        IList<MessageDTO> ReciveMessages(int currentUserId, int page, int itemPerPage);

        int GetNewMessagesCount(int currentUserId);

        int GetMessagesCount(int currentUserId);

        void DeleteMessage(int id);

        void UpdateMessage(int id, int newStatus);

        bool UploadAttachment(MessageAttachment messageAttachment);
    }
}