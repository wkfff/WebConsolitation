using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Messages.Services
{
    public class MessageService : IMessageService
    {
        #region field

        private readonly IScheme scheme;
        private readonly ILinqRepository<MessageAttachment> attachmentRepository;
        private readonly object lockObj = new object();

        #endregion

        #region ctor

        public MessageService(IScheme scheme, ILinqRepository<MessageAttachment> attachmentRepository)
        {
            this.scheme = scheme;
            this.attachmentRepository = attachmentRepository;
        }

        #endregion

        public DataTable GroupsTable
        {
            get { return scheme.UsersManager.GetGroups(); }
        }

        public DataTable UsersTable
        {
            get { return scheme.UsersManager.GetUsers(); }
        }

        #region send message

        public SendMessageStatus SendMessage(MessageWrapper messageWrapper, List<int> groupList, List<int> userList)
        {
            var status = SendMessageStatus.Success;

            using (new ServerContext())
            {
                var msgManager = scheme.MessageManager;

                foreach (var user in userList)
                {
                    var wrapper = new MessageWrapper
                        {
                            MessageType = messageWrapper.MessageType,
                            MessageImportance = MessageImportance.Importance,
                            Subject = messageWrapper.Subject,
                            RefUserSender = messageWrapper.RefUserSender,
                            RefUserRecipient = user,
                            DateTimeOfCreation = DateTime.Now,
                            DateTimeOfActual = DateTime.MaxValue,
                            RefMessageAttachment = messageWrapper.RefMessageAttachment
                        };

                    try
                    {
                        msgManager.SendMessage(wrapper);
                    }
                    catch (Exception e)
                    {
                    }
                }

                foreach (var group in groupList)
                {
                    var wrapper = new MessageWrapper
                        {
                            MessageType = messageWrapper.MessageType,
                            MessageImportance = MessageImportance.Importance,
                            Subject = messageWrapper.Subject,
                            RefUserSender = messageWrapper.RefUserSender,
                            RefGroupRecipient = group,
                            DateTimeOfCreation = DateTime.Now,
                            DateTimeOfActual = DateTime.MaxValue,
                            RefMessageAttachment = messageWrapper.RefMessageAttachment
                        };

                    try
                    {
                        msgManager.SendMessage(wrapper);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }

            return status;
        }

        #endregion

        #region recive mesages

        public IList<MessageDTO> ReciveMessages(int currentUserId, int page, int itemPerPage)
        {
            lock (lockObj)
            {
                using (new ServerContext())
                {
                    var msgManager = scheme.MessageManager;

                    try
                    {
                        return msgManager.ReceiveMessages(currentUserId, page, itemPerPage);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }
            }
        }

        public int GetNewMessagesCount(int currentUserId)
        {
            using (new ServerContext())
            {
                var msgManager = scheme.MessageManager;

                try
                {
                    return msgManager.ReceiveMessages(currentUserId).Count(x => x.MessageStatus == MessageStatus.New);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }

        public int GetMessagesCount(int currentUserId)
        {
            using (new ServerContext())
            {
                var msgManager = scheme.MessageManager;

                try
                {
                    return msgManager.GetMessageCount(currentUserId);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }

        #endregion

        #region delete message

        public void DeleteMessage(int id)
        {
            using (new ServerContext())
            {
                var msgManager = scheme.MessageManager;
                msgManager.DeleteMessage(id);
            }
        }

        #endregion

        #region updateMessage

        public void UpdateMessage(int id, int newStatus)
        {
            using (new ServerContext())
            {
                var msgManager = scheme.MessageManager;
                msgManager.UpdateMessage(id, (MessageStatus)newStatus);
            }
        }

        public bool UploadAttachment(MessageAttachment messageAttachment)
        {
            attachmentRepository.Save(messageAttachment);
            attachmentRepository.DbContext.CommitChanges();
            return true;
        }

        #endregion
    }
}
