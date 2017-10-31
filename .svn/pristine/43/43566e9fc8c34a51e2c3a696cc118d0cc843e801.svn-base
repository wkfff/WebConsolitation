using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.MessagesManager
{
    public class MessageRepository : NHibernateLinqRepository<Message>, IMessageRepository
    {
        private readonly IPermissionRepository permissionRepository;
        private readonly IObjectRepository objectRepository;
        private readonly IMembershipsRepository membershipsRepository;

        public MessageRepository(IPermissionRepository permissionRepository, IObjectRepository objectRepository, IMembershipsRepository membershipsRepository)
        {
            this.permissionRepository = permissionRepository;
            this.objectRepository = objectRepository;
            this.membershipsRepository = membershipsRepository;
        }

        public IQueryable<Message> GetObsoleteMessages()
        {
            return FindAll().Where(x => (x.DateTimeOfActual < DateTime.Now 
                || x.MessageStatus == (int)MessageStatus.Deleted));
        }

        public IQueryable<Message> GetUserMessages(int userId)
        {
            IList<int> memberships = GetGroupListForUser(userId);

            IList<Message> list = new List<Message>();
            foreach (Message message in FindAll().Where(m =>
                (m.RefUserRecipient == userId)

                   // сообщение должно быть актуальным
                   && m.DateTimeOfActual > DateTime.Now

                   // не удаленным
                   && m.MessageStatus != (int)MessageStatus.Deleted

                   // у пользователя должны быть настроены права
                   /*&& CheckPermission(m, objects, permissions)*/))
            {
                list.Add(message);
            }

            return list.AsQueryable();
        }

        private bool CheckPermission(Message message, IList<Objects> objects, IList<Permissions> permissions)
        {
            if (permissions.Count(p => p.RefObjects.Name == "AllMessages") > 0)
            {
                return true;
            }

            string messagetypeObjectKey =
                ((MessageTypeObjectKey)((MessageType)message.MessageType).GetType().GetField(((MessageType)message.MessageType).ToString()).
                    GetCustomAttributes(typeof(MessageTypeObjectKey), false)[0]).ObjectKey;

            if (!objects.Any(o => o.ObjectKey == messagetypeObjectKey))
            {
                return false;
            }

            return permissions.Count(p => p.RefObjects.ID == objects.First(o => o.ObjectKey == messagetypeObjectKey).ID) != 0;
        }

        private IList<Objects> GetObjectsId()
        {
            IList<string> list = new List<string>();

            foreach (var value in Enum.GetValues(typeof(MessageType)))
            {
                FieldInfo fi = value.GetType().GetField(value.ToString());
                MessageTypeObjectKey[] messageTypeObjectKeys =
                    (MessageTypeObjectKey[])fi.GetCustomAttributes(typeof(MessageTypeObjectKey), false);
                if (messageTypeObjectKeys.Any())
                {
                    if (!list.Contains(messageTypeObjectKeys[0].ObjectKey))
                    {
                        list.Add(messageTypeObjectKeys[0].ObjectKey);
                    }
                }
            }

            return objectRepository.FindAll().Where(o => list.Contains(o.ObjectKey))
                .ToList();
        }

        private IList<Permissions> GetPermissionsForUser(int userId, IList<int> memberships)
        {
            return permissionRepository.GetPermissionsForUser(userId, memberships)
                .ToList();
        }

        private IList<int> GetGroupListForUser(int userId)
        {
            return membershipsRepository.GetGroupsForUser(userId)
                .Select(membershipse => membershipse.RefGroups.ID).ToList();
        }
    }
}
