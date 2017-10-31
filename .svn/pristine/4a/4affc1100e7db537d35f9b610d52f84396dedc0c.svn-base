using System;
using System.Data;
using System.Threading;
using System.Timers;
using Krista.FM.Client.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.MessagesUI
{
    public class TimedMessageManager : ITimedMessageManager
    {
        private readonly IMessageFactory messageFactory;
        private readonly System.Timers.Timer timer;
        private readonly object stateLock = new object();
        private Thread thread;
        private DataTable messagesTable;
        private DateTime lastUpdate;

        public TimedMessageManager(IMessageFactory messageFactory)
        {
            this.messageFactory = messageFactory;

            // timer
            timer = new System.Timers.Timer(1000 * 60);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        public event EventHandler OnReciveMessages;
        public event EventHandler OnStartReciveMessages;

        public DataTable MessagesTable
        {
            get { return messagesTable; }
        }

        public DateTime LastUpdate
        {
            get { return lastUpdate; }
        }

        /// <summary>
        /// Количство непрочитанных сообщений
        /// </summary>
        public int NewMessagesCount { get; set; }
        /// <summary>
        /// Количество важных непрочитанных сообщений
        /// </summary>
        public int NewImportanceMessages { get; set; }

        public void Activate()
        {
            lock (stateLock)
            {
                ReceiveMessages();
            }
        }

        public MessageAttachmentDTO GetMessageAttachment(int messageId)
        {
            lock (stateLock)
            {
                return messageFactory.GetMessageAttachment(messageId);
            }
        }

        public void UpdateMessage(int messageId, MessageStatus status)
        {
            lock (stateLock)
            {
                messageFactory.UpdateMessageStatus(messageId, status);
            }
        }

        public void DeleteMessage(int messageId)
        {
            lock (stateLock)
            {
                messageFactory.DeleteMessage(messageId);
            }
        }

        private void ReciveMessagesEvent(EventArgs e)
        {
            EventHandler handler = OnReciveMessages;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void StartReciveMessagesEvent(EventArgs e)
        {
            EventHandler handler = OnStartReciveMessages;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            lock (stateLock)
            {
                ReceiveMessages();
            }
        }

        public void ReceiveMessages()
        {
            StartReciveMessagesEvent(new EventArgs());

            if (thread != null && thread.IsAlive)
            {
                thread.Abort();
                thread = null;
            }

            if (messageFactory != null)
            {
                var threadStart = new ThreadStart(ResiveMessagesFromFactory);
                thread = new Thread(threadStart)
                             {
                                 IsBackground = true,
                                 Priority = ThreadPriority.BelowNormal,
                                 Name = "ReceiveMessagesThread"
                             };

                thread.Start();
            }
        }

        private void ResiveMessagesFromFactory()
        {
            messagesTable = messageFactory.ReceiveMessages();

            NewMessagesCount = GetNewMessagesCount();
            NewImportanceMessages = GetNewImportanceMessagesCount();
            lastUpdate = DateTime.Now;
            
            ReciveMessagesEvent(new EventArgs());
        }

        /// <summary>
        /// Возвращает количество непрочитанных сообщений
        /// </summary>
        /// <returns> Количество непрочитанных сообщений</returns>
        private int GetNewMessagesCount()
        {
            if (messagesTable == null)
                return 0;

            return messagesTable.Select(String.Format("MessageStatus = {0}", (int)MessageStatus.New)).Length;
        }


        private int GetNewImportanceMessagesCount()
        {
            if (messagesTable == null)
            {
                return 0;
            }

            return
                messagesTable.Select(String.Format("MessageImportance in ({0}, {1})", (int) MessageImportance.Importance,
                                                   (int) MessageImportance.HighImportance)).Length;
        }
    }
}
