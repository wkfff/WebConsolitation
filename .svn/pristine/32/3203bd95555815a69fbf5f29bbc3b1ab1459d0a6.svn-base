using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Krista.Diagnostics
{
    /// <summary>
    /// Прослушиватель трассировочных сообщений
    /// </summary>
    public class ChatServerTraceListener : TraceListener
    {
        /// <summary>
        /// Строка URL с указанем местоположения сервера
        /// </summary>
        private readonly string serverUrl = string.Empty;

        /// <summary>
        /// Поток отправки сообщений серверу
        /// </summary>
        private Thread chatSenderThread;

        /// <summary>
        /// Очередь отладочных сообщений
        /// </summary>
        private readonly List<ChatData> messageList = new List<ChatData>();

        /// <summary>
        /// Событие, определяющее наличие нового сообщения в очереди
        /// </summary>
        private readonly EventWaitHandle newMessageEvent = new EventWaitHandle(false, EventResetMode.ManualReset);

        /// <summary>
        /// Инициализация прослушивателя
        /// </summary>
        private void Initialize()
        {
            try
            {
            	Trace.Listeners.Add(this);
                chatSenderThread = new Thread(ChatServerSernder_ThreadEntryPoint);
                chatSenderThread.Start();
            }
            catch (Exception e)
            {
                CharServerTrace.TraceEvent(e);
            }
        }

        /// <summary>
        /// Базовый конструктор
        /// </summary>
        public ChatServerTraceListener()
        {
            Initialize();
        }
        
		/// <summary>
        /// Конструктор с параметром из  конфигурационного файла
        /// </summary>
        /// <param name="initializeData">Строка инициализации прослушивателя</param>
        public ChatServerTraceListener(string initializeData)
			: base(initializeData)
        {
            serverUrl = initializeData;
            Initialize();
        }

        private void ChatServerSernder_ThreadEntryPoint()
        {
            // Сервер обмена сообщениями, которому будет отправляться трассировочные сообщения
            ICharServerSender serverInstance = null;
            // Клиентский канал
            TcpClientChannel clientChannel;
            lock(typeof(ChannelServices))
            {
                if (ChannelServices.GetChannel("krista.diagnostics")==null)
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
                                CharServerTrace.TraceEvent(e);
                            }
                        }
						object authorizationData = System.Runtime.Remoting.Messaging.CallContext.GetData("Authorization");
						try
                        {
							System.Runtime.Remoting.Messaging.CallContext.SetData("Authorization", null);
							serverInstance.BrodcastMessages(data);
                        }
                        catch (Exception e)
                        {
                            serverInstance = null;
                            CharServerTrace.TraceEvent(e);
                        }
						finally
                        {
							if (authorizationData != null)
								System.Runtime.Remoting.Messaging.CallContext.SetData("Authorization", authorizationData);
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


        /// <summary>
        /// Трассировочное сообщение
        /// </summary>
        /// <param name="eventCache">Кэш сообщений</param>
        /// <param name="source">Источник сообщения</param>
        /// <param name="eventType">Тип сообщения</param>
        /// <param name="id">Идентификатор сообщения</param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            TraceEvent(eventCache, source, eventType, id, null, null);
        }
        /// <summary>
        /// Трассировочное сообщение
        /// </summary>
        /// <param name="eventCache">Кэш сообщений</param>
        /// <param name="source">Источник сообщения</param>
        /// <param name="eventType">Тип сообщения</param>
        /// <param name="id">Идентификатор сообщения</param>
        /// <param name="message">Трассировочное сообщение</param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            TraceEvent(eventCache, source, eventType, id, message, null);
        }

        //Эта функция должна быть асинхронной.
        /// <summary>
        /// Трассировочное сообщение
        /// </summary>
        /// <param name="eventCache">Кэш сообщений</param>
        /// <param name="source">Источник сообщения</param>
        /// <param name="eventType">Тип сообщения</param>
        /// <param name="id">Идентификатор сообщения</param>
        /// <param name="format">Строка с форматом сообщения</param>
        /// <param name="args">Параметы для строки с форматом сообщения</param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            if (serverUrl.Length != 0)
            {
                TracePacket packet = new TracePacket();
                packet.Source = source;
                packet.EventType = eventType;
                packet.Id = id;
                packet.Format = format;
                packet.Args = args;
                ChatData data = new ChatData();
                data.ChannelName = source;
                data.ChannelData = packet;
                lock (messageList)
                {
                    messageList.Add(data);
                }
                newMessageEvent.Set();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        public override void Write(string message)
        {
            WriteLine(message);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        public override void WriteLine(string message)
        {
            TraceEvent(null, "noname", TraceEventType.Information, 0, message);
        }

        /// <summary>
        ///
        /// </summary>
        ~ChatServerTraceListener()
        {
            Dispose(false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (chatSenderThread != null)
            {
                chatSenderThread.Abort();
                chatSenderThread.Join();
            }
            base.Dispose(disposing);
        }

		public override void Close()
		{
			if (chatSenderThread != null)
			{
				chatSenderThread.Abort();
				chatSenderThread.Join();
			}
			base.Close();
		}
    }
}