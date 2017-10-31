using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Krista.Diagnostics
{
    /// <summary>
    /// Собственно реализация класса сервера обмена сообщениями
    /// </summary>
    internal class ChatServer : IDisposable
    {
        /// <summary>
        /// Максимальное количество штрафов у подпичика
        /// </summary>
        const int CriticalBansCount = 2;
        
        /// <summary>
        /// Список сообщений
        /// </summary>
        private readonly List<ChatData> chatMessageList = new List<ChatData>();
        
        /// <summary>
        /// Элемент синхронизации вызовов коллекции подписчиков
        /// </summary>
        private readonly ReaderWriterLock listenersRwLock = new ReaderWriterLock();

        /// <summary>
        /// Общий список подписчиков
        /// </summary>
        private readonly Dictionary<int, SubscriberData> subscribers = new Dictionary<int, SubscriberData>();

        /// <summary>
        /// Событие, сигнализирующее о поступлении нового сообщения
        /// </summary>
        private readonly EventWaitHandle newChatMessageEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
        
        /// <summary>
        /// Поток рассылки сообщений
        /// </summary>
        private readonly Thread chatSenderThread;
        
        /// <summary>
        /// Конструктор
        /// </summary>
        public ChatServer()
        {
            chatSenderThread = new Thread(ChatSenderThread_EntryPoint);
            chatSenderThread.Start();
        }

        /// <summary>
        /// Рассылка сообщения
        /// </summary>
        /// <param name="data">Пакет сообщеня</param>
        public void BrodcastMessage(ChatData data)
        {
            lock(chatMessageList)
            {
                chatMessageList.Add(data);
                newChatMessageEvent.Set();
            }
        }
        /// <summary>
        /// Рассылка сообщений
        /// </summary>
        /// <param name="data">Пакет сообщений</param>
        public void BrodcastMessages(ChatData[] data)
        {
            lock (chatMessageList)
            {
                chatMessageList.AddRange(data);
                newChatMessageEvent.Set();
            }
        }

        /// <summary>
        /// Поток обработки сообщений
        /// </summary>
        private void ChatSenderThread_EntryPoint()
        {
            while (true)
            {
                try
                {
                    //Ожидаем прихода уведомления о новых сообщениях
                    newChatMessageEvent.WaitOne(Timeout.Infinite, false);
                    ChatData[] data;
                    //Блокируем очередь, и преобразуем ее к массиву элементов
                    //очищая саму очередь
                    lock (chatMessageList)
                    {
                        if (chatMessageList.Count > 0)
                        {
                            data = chatMessageList.ToArray();
                            chatMessageList.Clear();
                        }
                        else
                        {
                            data = null;
                        }
                        //Сбрасываем событие прихода новых сообщений
                        newChatMessageEvent.Reset();
                    }

                    if (data != null)
                    {
                        #region Анализ штафов
                        //Удалим оштрафованых подписчиков

                        //Одновременно итерировать и изменять коллекцию нельзя,
                        //поэтому ее пакетное изменение разбиваем на два этапа.

                        //Сначала наберем их в коллекцию
                        List<int> bannedForRemove = new List<int>();
                        listenersRwLock.AcquireReaderLock(Timeout.Infinite);
                        try
                        {
                            foreach (KeyValuePair<int, SubscriberData> subscriber in subscribers)
                            {
                                if (subscriber.Value.Bans >= CriticalBansCount)
                                {
                                    bannedForRemove.Add(subscriber.Value.Cookie);
                                }
                            }
                        }
                        finally
                        {
                            listenersRwLock.ReleaseReaderLock();
                        }

                        //Потом отключим отштрафованые объекты
                        foreach (int banned in bannedForRemove)
                        {
                            Detach(banned);
                        }
                        #endregion

                        //Подписчики к каналу * должны получать все сообщения.
                        listenersRwLock.AcquireReaderLock(Timeout.Infinite);
                        try
                        {
                            //Проход по всем подписчикам и рассылка сообщения.
                            foreach (KeyValuePair<int,SubscriberData> subscriber in subscribers)
                            {
                                subscriber.Value.SendData(data);
                            }
                        }
                        finally
                        {
                            listenersRwLock.ReleaseReaderLock();
                        }
                    }
                }
                catch (Exception e)
                {
                    CharServerTrace.TraceEvent(e);
                }
            }
        }

        /// <summary>
        /// Отключение от сервера обмена сообщениями
        /// </summary>
        /// <param name="cookie">Идентификатор подключения</param>
        public void Detach(int cookie)
        {
            CharServerTrace.TraceEvent(TraceEventType.Information, "Подписчик [{0}] будет отключен.", cookie);
            //Удалим подпсичика из общего списка подписчиков

            //блокикуем полностью доступ к списку подписчиков
            listenersRwLock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                subscribers.Remove(cookie);
            }
            finally
            {
                //разблокируем доступ к списку подписчиков
                listenersRwLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Подключение к серверу обмена сообщениями
        /// </summary>
        /// <param name="channelName">Имя канала или *</param>
        /// <param name="subscriber">Интерфейс подписчика</param>
        /// <returns>Иделтификатор подключения</returns>
        public int Attach(string channelName, IChatServerSubscriber subscriber)
        {

            //Создаем класс с данными о подписчике
            SubscriberData subscriberData = new SubscriberData(subscriber);
            subscriberData.RegisterChannel(channelName);

            CharServerTrace.TraceEvent(TraceEventType.Information, "Новый подписчик [{0}] к каналу [{1}].", subscriberData.Cookie, channelName);

            //Регистриуем подписчика в списке каналов
            //блокикуем полностью доступ к списку подписчиков
            listenersRwLock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                //Регистрируем подписчика в общем списке
                subscribers.Add(subscriberData.Cookie, subscriberData);
            }
            finally
            {
                //разблокируем доступ к списку подписчиков
                listenersRwLock.ReleaseWriterLock();
            }
            //Возвращаем идентификатор подписки
            return subscriberData.Cookie;
        }

        #region IDisposable Members

        /// <summary>
        /// Завершение работы объекта. Удаление потоков
        /// </summary>
        public void Dispose()
        {
            chatSenderThread.Abort();
            chatSenderThread.Join();
        }

        #endregion

    }
}