using System;

namespace Krista.Diagnostics
{
    /// <summary>
    /// Класс-заглушка реализующая интерфейс отправителя сообщений.
    /// Используется для автоматического создания интерфейса через Remoting
    /// </summary>
    public class ChatServerSenderStub : MarshalByRefObject, ICharServerSender
    {
        /// <summary>
        /// Рассылка сообщения
        /// </summary>
        /// <param name="data">Пакет сообщеня</param>
        public void BrodcastMessage(ChatData data)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Рассылка сообщений
        /// </summary>
        /// <param name="data">Пакет сообщений</param>
        public void BrodcastMessages(ChatData[] data)
        {
            throw new NotImplementedException();
        }
    }
}