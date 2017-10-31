using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using log4net.Appender;
using log4net.Core;

namespace Krista.Diagnostics.Log4Net
{
    /// <summary>
    /// Клиент чат-сервера, отсылает все сообщения чат-серверу.
    /// Для лучшей производительности сообщения буферизуются, после переполнения буфера
    /// сообщения отправляются чат-серверу в отдельном потоке.
    /// </summary>
    public class ChatAppender : AppenderSkeleton
    {
        /// <summary>
        /// Строка URL с указанем местоположения сервера.
        /// </summary>
        public string Url
        {
            get { return serverUrl; }
            set { serverUrl = value; }
        }

        /// <summary>
        /// Строка URL с указанем местоположения сервера.
        /// </summary>
        private string serverUrl = String.Empty;
        
        /// <summary>
        /// Поток отправки сообщений серверу.
        /// </summary>
        private Thread chatSenderThread;

        /// <summary>
        /// Очередь отладочных сообщений.
        /// </summary>
        private readonly List<ChatData> messageList = new List<ChatData>();

        /// <summary>
        /// Событие, определяющее наличие нового сообщения в очереди.
        /// </summary>
        private readonly EventWaitHandle newMessageEvent = new EventWaitHandle(false, EventResetMode.ManualReset);

        public ChatAppender()
        {
            Initialize();
        }

        /// <summary>
        /// Инициализация прослушивателя.
        /// </summary>
        private void Initialize()
        {
            try
            {
                chatSenderThread = new Thread(ChatServerSernder_ThreadEntryPoint);
                chatSenderThread.Priority = ThreadPriority.Lowest;
                chatSenderThread.Start();
            }
            catch (Exception e)
            {
                Trace.TraceEvent(e);
            }
        }

        private void ChatServerSernder_ThreadEntryPoint()
        {
            // Сервер обмена сообщениями, которому будет отправляться трассировочные сообщения
            ICharServerSender serverInstance = null;
            
            // Клиентский канал
            TcpClientChannel clientChannel;
            lock (typeof(ChannelServices))
            {
                if (ChannelServices.GetChannel("krista.diagnostics") == null)
                {
                    clientChannel = new TcpClientChannel("krista.diagnostics", null);
                    //TODO #warning Аутентификацию соединения надо требовать согласно настройке в конфигурации
                    ChannelServices.RegisterChannel(clientChannel, false);
                }
                else
                {
                    clientChannel = null;
                }
            }
            
            while (true)
            {
                ChatData[] data;
                
                newMessageEvent.WaitOne(Timeout.Infinite, false);
                
                lock (messageList)
                {
                    data = messageList.ToArray();
                    messageList.Clear();
                }
                
                newMessageEvent.Reset();
                
                try
                {
                    if (data != null)
                    {
                        if (serverInstance == null)
                        {
                            try
                            {
                                serverInstance = Activator.GetObject(typeof(ChatServerSenderStub), serverUrl) as ICharServerSender;
                            }
                            catch (Exception e)
                            {
                                Trace.TraceEvent(e);
                            }
                        }
                        
                        object authorizationData = CallContext.GetData("Authorization");
                        try
                        {
                            CallContext.SetData("Authorization", null);
                            serverInstance.BrodcastMessages(data);
                        }
                        catch (Exception e)
                        {
                            serverInstance = null;
                            Trace.TraceEvent(e);
                        }
                        finally
                        {
                            if (authorizationData != null)
                                CallContext.SetData("Authorization", authorizationData);
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    if (clientChannel != null)
                    {
                        ChannelServices.UnregisterChannel(clientChannel);
                    }
                    return;
                }
            }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (serverUrl.Length != 0)
            {
                // Рендерим сообщение. Если не вызвать св-во RenderedMessage, 
                // то чат-сервер не сможет получить отрендеренное сообщение.
                string dummy = loggingEvent.RenderedMessage;

                ChatData data = new ChatData();
                data.ChannelName = loggingEvent.LoggerName;
                data.ChannelData = loggingEvent;

                lock (messageList)
                {
                    messageList.Add(data);
                }
                newMessageEvent.Set();
            }
        }

        override protected bool RequiresLayout
        {
            get { return false; }
        }
    }
}
