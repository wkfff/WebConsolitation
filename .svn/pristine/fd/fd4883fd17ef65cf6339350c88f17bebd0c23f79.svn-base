using System;
using System.Threading;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.MessagesManager
{
    /// <summary>
    /// Периодически будем очищать сообщения у которых истек срок действия
    /// </summary>
    public class MessageDispatcher
    {
        private readonly IMessageManager messageManager;
        private readonly int mainThreadSleepMilliSeconds;
        private readonly object lockObject = new object();
        private readonly Thread secondThread;
        private bool stop;

        public MessageDispatcher(IMessageManager messageManager)
        {
            this.messageManager = messageManager;
            mainThreadSleepMilliSeconds = 60 * 60 * 1000;

            secondThread = new Thread(Start)
                               {
                                   Name = "ClearObsoleteMessagesThread",
                                   IsBackground = true
                               };
            secondThread.Start();
        }

        public void Start()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(mainThreadSleepMilliSeconds);

                    Monitor.Enter(lockObject);
                    if (stop)
                    {
                        Monitor.Exit(lockObject);
                        break;
                    }

                    Monitor.Exit(lockObject);

                    try
                    {
                        DeleteObsoleteMessages();
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        
        public void Stop()
        {
            try
            {
                Monitor.Enter(lockObject);
                stop = true;
                Monitor.Exit(lockObject);

                if (secondThread != null && secondThread.IsAlive)
                {
                    secondThread.Abort();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void DeleteObsoleteMessages()
        {
            Trace.WriteLine(String.Format("{0}. Очистка устаревших сообщений", DateTime.Now));
            ((MessageManager)messageManager).RemoveObsoleteMessage();
        }
    }
}
