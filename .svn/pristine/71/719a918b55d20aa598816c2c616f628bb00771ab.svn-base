using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Krista.Diagnostics
{
    /// <summary>
    /// Серверная информация о подписчике
    /// </summary>
    internal class SubscriberData : IDisposable
    {
        /// <summary>
        /// Список сообщений, отправляемых клиенту
        /// </summary>
        private readonly List<ChatData> dataList = new List<ChatData>();
        
        /// <summary>
        /// Поток, отправляющий клиенту данные
        /// </summary>
        private readonly Thread dataSenderThread;
        
        /// <summary>
        /// Событие, сигналижирующее о поступлении новых данных
        /// </summary>
        private readonly EventWaitHandle newMessageEvent = new EventWaitHandle(false, EventResetMode.ManualReset);

        /// <summary>
        /// Отправка сообщений клиенту
        /// </summary>
        /// <param name="data"></param>
        public void SendData(ChatData[] data)
        {
            lock (dataList)
            {
                dataList.AddRange(data);
                newMessageEvent.Set();
            }
        }
        /// <summary>
        /// Поток отправки сообщений
        /// </summary>
        private void DataSendThread_EnrtyPoint()
        {
            while (true)
            {
                try
                {
                    ChatData[] data;
                    newMessageEvent.WaitOne(Timeout.Infinite, false);
                    lock (dataList)
                    {
                        if (dataList.Count > 0)
                        {
                            data = dataList.ToArray();
                            dataList.Clear();
                        }
                        else
                        {
                            data = null;
                        }
                        newMessageEvent.Reset();
                    }
                    if (data != null)
                    {
                        try
                        {
                            if (!allChannels)
                            {
                                List<ChatData> dataForSend = new List<ChatData>();

                                foreach (ChatData chatData in data)
                                {
                                    if (channels.Contains(chatData.ChannelName))
                                    {
                                        dataForSend.Add(chatData);
                                    }
                                }
                                data = dataForSend.ToArray();

                            }
                            if (data.Length > 0)
                            {
                                Subscriber.ServerMessage(data);
                            }

                            Bans = 0;
                        }
                        catch(Exception e)
                        {
                            Bans++;
                            //Выводим сообщение о причинах штрафа подписчика
                            CharServerTrace.TraceEvent(e);
                            //Штрафуем подписчика
                            CharServerTrace.TraceEvent(TraceEventType.Information, "Подписчик [{0}] будет оштрафован.", Cookie);
                        }
                    }

                }
                catch (ThreadAbortException)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Идентификатор подключения
        /// </summary>
        public int Cookie = Guid.NewGuid().GetHashCode();
        
        /// <summary>
        /// Число штрафов
        /// </summary>
        public int Bans;
        
        /// <summary>
        /// Собственно подписчик
        /// </summary>
        public IChatServerSubscriber Subscriber;
        
        /// <summary>
        /// Констркутор
        /// </summary>
        /// <param name="subscriber">Инрерфейс подписчика</param>
        public SubscriberData(IChatServerSubscriber subscriber)
        {
            Subscriber = subscriber;
            dataSenderThread = new Thread(DataSendThread_EnrtyPoint);
            dataSenderThread.Start();
        }

        private readonly List<string> channels = new List<string>();
        private bool allChannels;

        /// <summary>
        /// Регистрация подписчика в канале
        /// </summary>
        /// <param name="channelName">Имя канала</param>
        public void RegisterChannel(string channelName)
        {
            if (channelName == "*")
            {
                allChannels = true;
            }
            else
            {
                if ((channelName.Length!=0)&&(!channels.Contains(channelName)))
                    channels.Add(channelName);
            }
        }
        /// <summary>
        /// Исключение канала из списка
        /// </summary>
        /// <param name="channelName">Имя канала</param>
        public void UnregisterChannel(string channelName)
        {
            if (channelName == "*")
            {
                allChannels = false;
            }
            else
            {
                channels.Remove(channelName);
            }
        }


        public void Dispose()
        {
            if (dataSenderThread != null)
            {
                dataSenderThread.Abort();
                dataSenderThread.Join();
            }
        }
    }
}