namespace bus.gov.ru
{
    /// <summary>
    /// Типы событий для протокола закачки данных
    /// </summary>
    public enum DataPumpEventType
    {
        /// <summary>
        /// Тип - Ошибка
        /// </summary>
        Error, 

        /// <summary>
        /// Тип - Предупреждение
        /// </summary>
        Warning,

        /// <summary>
        /// Тип - Информация
        /// </summary>
        Info
    }
}
