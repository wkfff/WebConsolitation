using System;

namespace Krista.Diagnostics
{
    /// <summary>
    /// Статический класс содержащий тип запускаемого приложения
    /// </summary>
    [Serializable]
    public static class RunMode
    {
        /// <summary>
        /// Тип запускаемого приложения
        /// </summary>
        public static RunModeType RunModeType = RunModeType.Console;
    }
}