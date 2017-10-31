using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.MessagesManager
{
    public class MessageManager : DisposableObject, IMessageManager
    {
        private readonly IMessageExchangeProtocol log;
        private readonly IMessageRepository messageRepository;
        private readonly IRepository<Users> userRepository;
        private readonly IMembershipsRepository membershipsRepository;
        private readonly ILinqRepository<MessageAttachment> messageAttachmentRepository;
        private readonly IPermissionRepository permissionRepository;
        private readonly IObjectRepository objectsRepository;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private Dictionary<string, int> objectsCashedDictionary;

        public MessageManager(
            IMessageExchangeProtocol log,
            IUnitOfWorkFactory unitOfWorkFactory,
            IMessageRepository messageRepository,
            IRepository<Users> userRepository,
            IMembershipsRepository membershipsRepository,
            ILinqRepository<MessageAttachment> messageAttachmentRepository,
            IPermissionRepository permissionRepository,
            IObjectRepository objectsRepository)
        {
            this.log = log;
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.messageRepository = messageRepository;
            this.userRepository = userRepository;
            this.membershipsRepository = membershipsRepository;
            this.messageAttachmentRepository = messageAttachmentRepository;
            this.permissionRepository = permissionRepository;
            this.objectsRepository = objectsRepository;

            objectsCashedDictionary = GetObjectsDictionary();
        }

        #region IMessageManager

        public bool SendMessage(MessageWrapper messageWrapper)
        {
            if (String.IsNullOrEmpty(messageWrapper.Subject))
            {
                throw new ArgumentNullException("subject");
            }

            if (messageWrapper.RefUserRecipient == null && messageWrapper.RefGroupRecipient == null && messageWrapper.SendAll == false)
            {
                throw new ArgumentNullException("userReceiver and groupReceiver and not sendAllOnLine");
            }

            HashSet<int> recipientUsers = GetRecipientUsers(
                messageWrapper.RefUserRecipient,
                messageWrapper.RefGroupRecipient,
                messageWrapper.SendAll,
                messageWrapper.MessageType);

            try
            {
                MessageAttachment messageAttachment = null;
                if (messageWrapper.RefMessageAttachment != null)
                {
                    messageAttachment = CreateMessageAttach(
                        messageWrapper.RefMessageAttachment.DocumentName,
                        messageWrapper.RefMessageAttachment.DocumentFileName,
                        messageWrapper.RefMessageAttachment.Document);
                }

                using (IUnitOfWork unitOfWork = unitOfWorkFactory.Create())
                {
                    foreach (int recipientUser in recipientUsers)
                    {
                        try
                        {
                            Message message = new Message
                            {
                                Subject = messageWrapper.Subject,
                                DateTimeOfCreation = messageWrapper.DateTimeOfCreation,
                                DateTimeOfActual = messageWrapper.DateTimeOfActual,
                                MessageType = (int)messageWrapper.MessageType,
                                MessageStatus = (int)messageWrapper.MessageStatus,
                                MessageImportance = (int)messageWrapper.MessageImportance,
                                RefUserSender = messageWrapper.RefUserSender,
                                RefUserRecipient = recipientUser,
                                RefMessageAttachment = messageAttachment,
                                TransferLink = messageWrapper.TransferLink
                            };

                            messageRepository.Save(message);
                            messageRepository.DbContext.CommitChanges();

                            log.WriteEventIntoMessageExchangeProtocol(
                                GetMessageEventKind(messageWrapper.MessageType),
                                String.Format("Создано новое сообщение от пользователя {0} пользователю {1}. Тип сообщения - {2}", GetUserName(messageWrapper.RefUserSender), GetUserName(recipientUser), messageWrapper.MessageType));
                        }
                        catch (Exception e)
                        {
                            log.WriteEventIntoMessageExchangeProtocol(
                                MessagesEventKind.mekSendError,
                                String.Format("При отправке сообщения пользователю {0} возникло исключение : {1}", GetUserName(recipientUser), e.Message));
                        }
                    }

                    unitOfWork.Commit();
                }

                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError(String.Format("При отправке сообщения возникло исключение : {0}", e.Message));
                log.WriteEventIntoMessageExchangeProtocol(
                                MessagesEventKind.mekSendError,
                                String.Format("При отправке сообщения возникло исключение : {0}", e.Message));
                return false;
            }
        }

        public IList<MessageDTO> ReceiveMessages(int userId)
        {
            using (new PersistenceContext())
            {
                IList<Message> messages = messageRepository.GetUserMessages(userId).ToList();

                var filteredMessages = from message in messages
                                       select new MessageDTO
                                       {
                                           ID = message.ID,
                                           Subject = message.Subject,
                                           MessageStatus = (MessageStatus)message.MessageStatus,
                                           MessageType = (MessageType)message.MessageType,
                                           MessageImportance = (MessageImportance)message.MessageImportance,
                                           RefUserSender = GetUserName(message.RefUserSender),
                                           RefMessageAttachment = (message.RefMessageAttachment == null) ? 0 : message.RefMessageAttachment.ID,
                                           ReceivedDate = message.DateTimeOfCreation,
                                           TransferLink = message.TransferLink
                                       };

                return filteredMessages.ToList();
            }
        }

        public void UpdateMessage(int messageId, MessageStatus status)
        {
            using (IUnitOfWork unitOfWork = unitOfWorkFactory.Create())
            {
                var message = messageRepository.FindOne(messageId);
                message.MessageStatus = (int)status;

                messageRepository.Save(message);

                unitOfWork.Commit();
            }
        }

        /// <summary>
        /// Удаление сообщения (не приводит к полному удалению
        /// сообщения из БД, а только помечает его как "удаленное".
        /// Полное удаление осуществляется сборщиком неактуальных сообщений по расписанию.
        /// </summary>
        /// <param name="messageId"> Идентификатор сообщения</param>
        public void DeleteMessage(int messageId)
        {
            using (IUnitOfWork unitOfWork = unitOfWorkFactory.Create())
            {
                var message = messageRepository.FindOne(messageId);
                message.MessageStatus = (int)MessageStatus.Deleted;

                messageRepository.Save(message);

                unitOfWork.Commit();
            }
        }

        public MessageAttachmentDTO GetMessageAttachment(int messageId)
        {
            using (new PersistenceContext())
            {
                var message = messageRepository.FindOne(messageId);
                if (message == null)
                {
                    return null;
                }

                if (message.RefMessageAttachment == null)
                {
                    return null;
                }

                MessageAttachment messageAttachment = messageAttachmentRepository.FindOne(message.RefMessageAttachment.ID);

                MessageAttachmentDTO messageAttachmentDto = new MessageAttachmentDTO
                {
                    Document = messageAttachment.Document,
                    DocumentFileName = messageAttachment.DocumentFileName
                };

                return messageAttachmentDto;
            }
        }

        #endregion

        /// <summary>
        /// Очищает неактуальные сообщения.
        /// </summary>
        internal void RemoveObsoleteMessage()
        {
            log.WriteEventIntoMessageExchangeProtocol(
                    MessagesEventKind.mekRemoveObsoleteMessages,
                    "Запущена процедура очистки неактуальных сообщений.");

            using (IUnitOfWork unitOfWork = unitOfWorkFactory.Create())
            {
                foreach (var message in messageRepository.GetObsoleteMessages())
                {
                    messageRepository.Delete(message);
                }

                unitOfWork.Commit();
            }
        }

        private MessageAttachment CreateMessageAttach(string documentName, string documentFileName, byte[] document)
        {
            if (String.IsNullOrEmpty(documentName))
            {
                return null;
            }

            MessageAttachment messageAttachment = new MessageAttachment
            {
                DocumentName = documentName,
                DocumentFileName = documentFileName,
                Document = document
            };

            using (IUnitOfWork unitOfWork = unitOfWorkFactory.Create())
            {
                messageAttachmentRepository.Save(messageAttachment);
                unitOfWork.Commit();
            }

            return messageAttachment;
        }

        #region Mappers

        private string GetUserName(int? refUserId)
        {
            if (refUserId == null)
            {
                return "Система";
            }

            Users user = userRepository.Get((int)refUserId);
            if (String.IsNullOrEmpty(user.FirstName) && String.IsNullOrEmpty(user.LastName) && String.IsNullOrEmpty(user.Patronymic))
            {
                return user.Name;
            }

            return String.Format(
                "{0} {1} {2}",
                user.LastName,
                user.FirstName,
                user.Patronymic);
        }

        #endregion

        #region Helper

        private HashSet<int> GetRecipientUsers(int? refUserRecipient, int? refGroupRecipient, bool sendAll, MessageType messageType)
        {
            HashSet<int> users = new HashSet<int>();

            try
            {
                if (refUserRecipient != null)
                {
                    if (!users.Contains((int)refUserRecipient))
                    {
                        users.Add((int)refUserRecipient);
                    }
                }

                if (refGroupRecipient != null || sendAll)
                {
                    int objectId;
                    try
                    {
                        objectId = GetObjectId(messageType);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(e.Message);
                        return users;
                    }

                    using (new PersistenceContext())
                    {
                        foreach (Permissions permission in permissionRepository.GetPemissionsForObject(objectId))
                        {
                            if (!sendAll)
                            {
                                if (permission.RefGroups != null && permission.RefGroups.ID == refGroupRecipient)
                                {
                                    // есть права на группу
                                    foreach (Memberships membershipse in membershipsRepository.GetUsersForGroup(permission.RefGroups.ID))
                                    {
                                        users.Add(membershipse.RefUsers.ID);
                                    }
                                }
                                else if (permission.RefUsers != null)
                                {
                                    IQueryable<Memberships> memberships =
                                        membershipsRepository.GetUsersForGroup((int)refGroupRecipient);
                                    foreach (var mem in memberships)
                                    {
                                        if (mem != null)
                                        {
                                            if (permission.RefUsers.ID == mem.RefUsers.ID)
                                            {
                                                if (!users.Contains(permission.RefUsers.ID))
                                                {
                                                    users.Add(permission.RefUsers.ID);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (sendAll)
                            {
                                if (permission.RefUsers != null)
                                {
                                    if (!users.Contains(permission.RefUsers.ID))
                                    {
                                        users.Add(permission.RefUsers.ID);
                                    }
                                }

                                if (permission.RefGroups != null)
                                {
                                    foreach (Memberships membershipse in membershipsRepository.GetUsersForGroup(permission.RefGroups.ID))
                                    {
                                        users.Add(membershipse.RefUsers.ID);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("При получение списка получателей сообщения возникло исключение: {0}", e.Message));
            }

            return users;
        }

        /// <summary>
        /// Получает коллекцию объектов для системы прав
        /// </summary>
        private Dictionary<string, int> GetObjectsDictionary()
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();

            foreach (var value in Enum.GetValues(typeof(MessageType)))
            {
                FieldInfo fi = value.GetType().GetField(value.ToString());
                MessageTypeObjectKey[] messageTypeObjectKeys =
                    (MessageTypeObjectKey[])fi.GetCustomAttributes(typeof(MessageTypeObjectKey), false);
                string enumObjectKey = messageTypeObjectKeys.Length > 0
                                           ? messageTypeObjectKeys[0].ObjectKey
                                           : value.ToString();
                Objects objects = objectsRepository.GetObjectsByObjectKey(enumObjectKey);

                if (objects != null)
                {
                    dict.Add(enumObjectKey, objects.ID);
                }
            }

            return dict;
        }

        private int GetObjectId(MessageType messageType)
        {
            FieldInfo fi = messageType.GetType().GetField(messageType.ToString());
            MessageTypeObjectKey[] messageTypeObjectKeys =
                (MessageTypeObjectKey[])fi.GetCustomAttributes(typeof(MessageTypeObjectKey), false);
            if (messageTypeObjectKeys.Any())
            {
                string objectKey = messageTypeObjectKeys[0].ObjectKey;

                if (objectsCashedDictionary.ContainsKey(objectKey))
                {
                    return objectsCashedDictionary[objectKey];
                }
            }

            throw new Exception(String.Format("Объект {0} не найден в таблице объектов системы", messageType.ToString()));
        }

        private MessagesEventKind GetMessageEventKind(MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.AdministratorMessage:
                    return MessagesEventKind.mekCreateAdmMessage;
                case MessageType.CubesManagerMessage:
                    return MessagesEventKind.mekCreateCubeMessage;
                default:
                    return MessagesEventKind.mekCreateOther;
            }
        }

        #endregion
    }
}