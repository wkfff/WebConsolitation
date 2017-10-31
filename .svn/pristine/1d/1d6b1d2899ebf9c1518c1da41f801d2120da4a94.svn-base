namespace Krista.Diagnostics
{
    /// <summary>
    /// Интерфес сервера для подписчиков.
    /// </summary>
    public interface ICharServerPort
    {
        /// <summary>
        /// Подключение к серверу обмена сообщениями
        /// </summary>
        /// <param name="channelName">Имя канала или *</param>
        /// <param name="subscriber">Интерфейс подписчика</param>
        /// <returns>Иделтификатор подключения</returns>
        int Attach(string channelName, IChatServerSubscriber subscriber);
        /// <summary>
        /// Отключение от сервера обмена сообщениями
        /// </summary>
        /// <param name="cookie">Идентификатор подключения</param>
        void Detach(int cookie);
    }
}