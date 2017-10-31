using System;
using System.Threading;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.MessagesManager
{
    /// <summary>
    /// Периодически будем очищать сообщения у которых истек срок действия
    /// </summary>
    public class MessageCleanerManager : IMessageCleanerManager
    {
        private readonly IMessageManager messageManager;
        private readonly int mainThreadSleepMilliSeconds;
        private readonly object lockObject = new object();
        private readonly Thread secondThread;
        private bool stop;

        public MessageCleanerManager(IMessageManager messageManager)
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
                        Trace.TraceError("При очистке сообщений возникло исключение: {0}", ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("При очистке сообщений возникло исключение: {0}", ex.Message);
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
