namespace Krista.Diagnostics
{
    /// <summary>
    /// Интерфес содписчика к системе сервера обмена сообщениями
    /// </summary>
    public interface IChatServerSubscriber
    {
        /// <summary>
        /// Сообщения сервера
        /// </summary>
        /// <param name="data">Массив сообщений</param>
        void ServerMessage(ChatData[] data);
    }
}