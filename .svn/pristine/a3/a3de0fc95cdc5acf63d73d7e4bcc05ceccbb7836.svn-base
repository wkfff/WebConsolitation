using System;
using System.Diagnostics;

namespace Krista.Diagnostics
{
    /// <summary>
    /// Пакет трассировки
    /// </summary>
    [Serializable]
    public class TracePacket
    {
        /// <summary>
        /// Источник сообщения
        /// </summary>
        public string Source;
        /// <summary>
        /// Тип сообщения
        /// </summary>
        public TraceEventType EventType;
        /// <summary>
        /// Идентификатор сообщения
        /// </summary>
        public int Id;
        /// <summary>
        /// Строка сообщения
        /// </summary>
        public string Format;
        /// <summary>
        /// Аргументы, если строка сообщения представлена как формат
        /// </summary>
        public object[] Args;
    }
}