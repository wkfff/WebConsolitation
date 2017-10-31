using System;

namespace Krista.FM.WriteBackLibrary
{
    public interface IWriteBackServer : IDisposable
    {
        string ProcessQuery(string queryString);

        /// <summary>
        /// Производит закрытие и уничтожение объекта
        /// </summary>
        void Close();

        /// <summary>
        /// Активация сервера
        /// </summary>
        void Activate();
    }
}