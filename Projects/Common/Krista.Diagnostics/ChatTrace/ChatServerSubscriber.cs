using System;

namespace Krista.Diagnostics
{
    /// <summary>
    /// Базовый, и для стандартной реализации достаточный, класс
    /// реализующий подписчика к серверу обмена сообщениями
    /// </summary>
    public class ChatServerSubscriber : MarshalByRefObject, IChatServerSubscriber
    {
        /// <summary>
        /// Событие, позникающее при наличии сообщений от сервера
        /// </summary>
        public event ServerMessageDelegate OnServerMessage;
        /// <summary>
        /// Метод, вызываемый сервером обмена сообщениями
        /// </summary>
        /// <param name="data"></param>
        public virtual void ServerMessage(ChatData[] data)
        {
            if (OnServerMessage != null) OnServerMessage(data);
        }
        /// <summary>
        /// Инициализация сервиса времени жизни объекта.
        /// Реализация определяет вечноживущий объкт
        /// </summary>
        /// <returns>null. Определяет что объект живет вечно</returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}