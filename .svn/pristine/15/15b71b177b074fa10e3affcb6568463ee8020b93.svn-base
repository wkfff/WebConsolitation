using System;

namespace Krista.FM.Domain
{
    public class Message : DomainObject
    {
        public virtual int? Id { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Body { get; set; }
        public virtual DateTime DateTimeOfCreation { get; set; }
        public virtual DateTime DateTimeOfActual { get; set; }
        public virtual int MessageType { get; set; }
        public virtual int MessageStatus { get; set; }
        public virtual int MessageImportance { get; set; }
        public virtual int? RefUserSender { get; set; }
        public virtual int RefUserRecipient { get; set; }
        public virtual MessageAttachment RefMessageAttachment { get; set; }
        public virtual string TransferLink { get; set; }
    }
}
