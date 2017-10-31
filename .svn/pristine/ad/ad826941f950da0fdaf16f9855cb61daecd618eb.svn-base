using System;

namespace Krista.Diagnostics
{
    /// <summary>
    /// Класс-заглушка реализующая интерфейс сервера для подписчиков.
    /// Используется для автоматического создания интерфейса через Remoting
    /// </summary>
    public class ChatServerPortStub : MarshalByRefObject, ICharServerPort
    {
        /// <summary>
        /// Подключение к серверу обмена сообщениями
        /// </summary>
        /// <param name="channelName">Имя канала или *</param>
        /// <param name="subscriber">Интерфейс подписчика</param>
        /// <returns>Иделтификатор подключения</returns>
        public int Attach(string channelName, IChatServerSubscriber subscriber)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Отключение от сервера обмена сообщениями
        /// </summary>
        /// <param name="cookie">Идентификатор подключения</param>
        public void Detach(int cookie)
        {
            throw new NotImplementedException();
        }
    }
}