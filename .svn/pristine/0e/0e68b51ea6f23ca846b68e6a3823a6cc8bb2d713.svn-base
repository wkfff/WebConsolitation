using System;

namespace Krista.Diagnostics
{
    /// <summary>
    /// Опции трассировки
    /// </summary>
    //[Obsolete]
    public class TraceParams : MarshalByRefObject
    {
        /// <summary>
        /// Источник трассировочных сообщений
        /// </summary>
        public string Source = string.Empty;
        /// <summary>
        /// Флаг разрешения трассировки
        /// </summary>
        public bool Enabled;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="source">Источник трассировки</param>
        /// <param name="enabled">Признак разрешения</param>
        public TraceParams(string source, bool enabled)
        {
            Source = source;
            Enabled = enabled;
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        public TraceParams()
        {
        }
        /// <summary>
        /// Копирует значения из другого такого же элемента
        /// </summary>
        /// <param name="options">Источник</param>
        public void Assign(TraceParams options)
        {
            Source = options.Source;
            Enabled = options.Enabled;
        }

    }
}