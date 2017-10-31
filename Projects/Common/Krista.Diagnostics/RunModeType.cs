using System;

namespace Krista.Diagnostics
{
    /// <summary>
    /// Тип запускаемого приложения
    /// </summary>
    [Serializable]
    public enum RunModeType { 
        /// <summary>
        /// Консольное приложение
        /// </summary>
        Console, 
        /// <summary>
        /// Приложение - сервис
        /// </summary>
        Service, 
        /// <summary>
        /// Windows Forms приложение
        /// </summary>
        WinForm 
    };
}