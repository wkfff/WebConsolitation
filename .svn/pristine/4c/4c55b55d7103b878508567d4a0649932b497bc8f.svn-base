using System;
using System.IO;

namespace Krista.FM.RIA.Core.Extensions
{
    public interface IParametersService
    {
        /// <summary>
        /// Регистрирует конфигурационные переменные расширения.
        /// </summary>
        /// <param name="stream">Конфигурация расширения.</param>
        void RegisterExtensionConfigParameters(Stream stream);

        /// <summary>
        /// Регистрирует конфигурационные переменные расширения.
        /// </summary>
        /// <param name="name">Имя регистрируемого параметра.</param>
        /// <param name="type">Тип параметра.</param>
        void RegisterExtensionConfigParameter(string name, Type type);

        /// <summary>
        /// Возвращает вычесленное значение параметра.
        /// </summary>
        /// <param name="parameterName">Имя параметра.</param>
        /// <returns>Значение параметра.</returns>
        string GetParameterValue(string parameterName);

        void Clear();
    }
}