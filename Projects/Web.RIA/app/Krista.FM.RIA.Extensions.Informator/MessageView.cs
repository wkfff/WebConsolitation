using System;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Informator
{
    public class MessageView
    {
        public int? ID { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string MessageStatus { get; set; }

        public string MessageImportance { get; set; }

        public MessageType MessageType { get; set; }

        public string RefUserSender { get; set; }

        public string RefUserRecipient { get; set; }

        public int RefMessageAttachment { get; set; }

        public bool MessageAttachment { get; set; }

        public byte[] Document { get; set; }

        public string DocumentFileName { get; set; }

        public DateTime ReceivedDate { get; set; }
    }
}
