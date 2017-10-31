using System;
using System.Linq;
using System.Text.RegularExpressions;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Informator
{
    public class NewsService : INewsService
    {
        private readonly IRepository<Users> userRepository;
        private readonly ILinqRepository<D_Org_UserProfile> users;
        private readonly ILinqRepository<Message> messageRepository;
        private readonly ILinqRepository<MessageAttachment> repository;

        public NewsService(
            IRepository<Users> userRepository,
            ILinqRepository<D_Org_UserProfile> users,
            ILinqRepository<Message> messageRepository,
            ILinqRepository<MessageAttachment> repository)
        {
            this.users = users;
            this.userRepository = userRepository;
            this.messageRepository = messageRepository;
            this.repository = repository;
        }

        public int GetNewMessagesCount(int currentUserId)
        {
            using (new ServerContext())
            {
                try
                {
                    return
                        messageRepository.FindAll().Count(x => x.RefUserRecipient == currentUserId && x.MessageStatus == 1 && x.MessageType == 1);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }

        public MessageView RecieveOneMessage(int id)
        {
            var message = messageRepository.FindOne(id);
            return new MessageView
            {
                ID = message.ID,
                Subject = GetSubject(message.Subject),
                Body = GetBody(message.Subject),
                MessageStatus = GetStringMessageStatus((MessageStatus)message.MessageStatus),
                MessageType = (MessageType)message.MessageType,
                MessageImportance =
                    GetStringMessageImportance((MessageImportance)message.MessageImportance),
                RefUserSender = GetOrgName(message.RefUserSender),
                RefUserRecipient = GetOrgName(message.RefUserRecipient),
                ////RefMessageAttachment = message.RefMessageAttachment != null ? message.RefMessageAttachment.ID : -1,
                ////MessageAttachment = (message.RefMessageAttachment != null),
                ////Document = (message.RefMessageAttachment == null) ? null : message.RefMessageAttachment.Document,
                ////DocumentFileName = (message.RefMessageAttachment == null) ? null : message.RefMessageAttachment.DocumentFileName,
                ReceivedDate = message.DateTimeOfCreation
            };
        }

        public bool UploadAttachment(MessageAttachment messageAttachment)
        {
            try
            {
                repository.Save(messageAttachment);
                repository.DbContext.CommitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetSubject(string subject)
        {
            return subject.IndexOf("/separator/") > 0
                       ? Regex.Unescape(subject.Substring(0, Regex.Unescape(subject).IndexOf("/separator/")))
                       : subject;
        }

        public string GetBody(string body)
        {
            return body.IndexOf("/separator/") > 0
                ? Regex.Unescape(body.Substring(Regex.Unescape(body).IndexOf("/separator/") + 11, Regex.Unescape(body).Length - Regex.Unescape(body).IndexOf("/separator/") - 11))
                : string.Empty;
        }

        private static string GetStringMessageImportance(MessageImportance messageImportance)
        {
            var status = string.Empty;
            switch (messageImportance)
            {
                case MessageImportance.HighImportance:
                    status = "Высокая важность";
                    break;
                case MessageImportance.Importance:
                    status = "Важная";
                    break;
                case MessageImportance.Regular:
                    status = "Обычная";
                    break;
                case MessageImportance.Unimportant:
                    status = "Неважная";
                    break;
            }

            return status;
        }

        private static string GetStringMessageStatus(MessageStatus messageStatus)
        {
            var status = string.Empty;
            switch (messageStatus)
            {
                case MessageStatus.Read:
                    status = "Прочитана";
                    break;
                case MessageStatus.New:
                    status = "Новая";
                    break;
                case MessageStatus.Deleted:
                    status = "Удалена";
                    break;
            }

            return status;
        }

        private string GetOrgName(int? refUserId)
        {
            if (refUserId.HasValue)
            {
                var name = userRepository.Get((int)refUserId).Name;
                try
                {
                    return users.FindAll().Where(x => x.UserLogin == name).Select(x => x.RefUchr.Name).First();
                }
                catch (Exception)
                {
                    return name;
                }
            }

            return "Администратор";
        }
    }
}
