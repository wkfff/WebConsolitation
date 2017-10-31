namespace Krista.FM.RIA.Core 
{
    /// <summary>
    /// Поведение контроллера при взаимодействии с SessionState.
    /// </summary>
    /// <remarks>
    /// Взято из .NET 4 перечисления SessionStateBehavior.
    /// </remarks>
    public enum ControllerSessionState 
    {
        /// <summary>
        /// Тоже самое что и Required.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Полная блокировка сессии, все обращения к контроллеру выполняются последовательно.
        /// </summary>
        Required = 1,

        /// <summary>
        /// Сессия не блокируется, контроллер может читать значения в сессии, 
        /// но не может модифицировать. Обращения к контроллеру выполняются параллельно.
        /// </summary>
        ReadOnly = 2,

        /// <summary>
        /// Сессия контроллеру не доступна.
        /// Обращения к контроллеру выполняются параллельно.
        /// </summary>
        Disabled = 3
    }
}
