using System;

namespace Krista.Diagnostics
{
    /// <summary>
    /// Экземпляр сервера обмена сообщениями. Используется внутри системы
    /// для хранения единственного экземпляра. А так же для публикации в Remoting
    /// </summary>
    public class ChatServerInstance : MarshalByRefObject, IChatServer
    {
        /// <summary>
        /// Экземпляр сервера
        /// </summary>
        [NonSerialized]
        private static ChatServer instance;

        /// <summary>
        /// Экземпляр сервера
        /// </summary>
        private static ChatServer Instance
        {
            get { return instance ?? (instance = new ChatServer()); }
        }

        /// <summary>
        /// Рассылка сообщения
        /// </summary>
        /// <param name="data">Пакет сообщеня</param>
        public void BrodcastMessage(ChatData data)
        {
            Instance.BrodcastMessage(data);
        }
        /// <summary>
        /// Рассылка сообщений
        /// </summary>
        /// <param name="data">Пакет сообщений</param>
        public void BrodcastMessages(ChatData[] data)
        {
            Instance.BrodcastMessages(data);
        }

        /// <summary>
        /// Подключение к серверу обмена сообщениями
        /// </summary>
        /// <param name="channelName">Имя канала или *</param>
        /// <param name="subscriber">Интерфейс подписчика</param>
        /// <returns>Иделтификатор подключения</returns>
        public int Attach(string channelName, IChatServerSubscriber subscriber)
        {
            return Instance.Attach(channelName, subscriber);
        }

        /// <summary>
        /// Отключение от сервера обмена сообщениями
        /// </summary>
        /// <param name="cookie">Идентификатор подключения</param>
        public void Detach(int cookie)
        {
            Instance.Detach(cookie);
        }

    }
}