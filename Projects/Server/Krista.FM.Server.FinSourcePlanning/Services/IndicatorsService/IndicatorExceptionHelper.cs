using System;
using System.Text.RegularExpressions;
using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Server.FinSourcePlanning.Services.IndicatorsService
{
    /// <summary>
    /// Класс, определяющий причину исключения при расчете.
    /// </summary>
    internal class IndicatorExceptionHelper
    {
        /// <summary>
        /// Определяет причину исключения для индикаторов.
        /// </summary>
        /// <param name="e">Исключение.</param>
        /// <returns>Причина, если удалось определить. Иначе исключение отправляется дальше.</returns>
        public static string ExceptionReason(Exception e)
        {
            if (e == null)
            {
                return string.Empty;
            }
            // Если нет обработчика индикатора.
            if (e is InvalidCastException)
            {
                return "Отсутствует обработчик индикатора.";
            }
            // Если есть, но некорректный.
            if (e is InvalidOperationException)
            {
                return "Ошибка в формате обработчика индикатора.";
            }
            if (e is FormatException)
            {
                return "Граничные значения оценки не указаны или некорректны.";
            }
            if (e.Message.Equals(RuntimeCompiledHandler.handlerErrorExceptionText))
            {
                return string.Format("{0}.", e.Message);
            }
            // Если не нашли причину
            throw e;
        }

        /// <summary>
        /// Определяет причину исключения для показателей.
        /// </summary>
        /// <param name="e">Исключение.</param>
        /// <param name="markName">Имя показателя.</param>
        /// <returns>Причина, если удалось определить. Иначе исключение отправляется дальше.</returns>
        public static string ExceptionReason(Exception e, string markName)
        {
            if (e == null)
            {
                return string.Empty;
            }
            // Если нет обработчика показателя.
            if (e is InvalidCastException)
            {
                return string.Format("Отсутствует обработчик показателя {0}.", markName);
            }
            // Если есть, но некорректный.
            if (e is InvalidOperationException)
            {
                return string.Format("Ошибка в формате обработчика показателя {0}.", markName);
            }
            if (e is AdomdErrorResponseException)
            {
                IndicatorExceptionHelper helper = new IndicatorExceptionHelper();
                return helper.AdomdErrorResponseExceptionReason(e, markName);
            }
            if (e.Message.Equals(RuntimeCompiledHandler.handlerErrorExceptionText))
            {
                return string.Format("{0} показателя {1}", e.Message, markName);
            }
            // Если не нашли причину
            throw e;
        }

        private string AdomdErrorResponseExceptionReason(Exception e, string markName)
        {
            if (e.Message.Contains("Formula error - cannot find dimension member"))
            {
                Regex regerx = new Regex("\".*\"");
                Match math = regerx.Match(e.Message);
                if (math.Success)
                {
                    return string.Format("Не удалось найти член измерения {0} при расчете показателя {1}", math.Captures[0].Value, markName);
                }
            }
            // The cube 'ФО_Результат ИФ' does not exist, or it is not processed
            if (e.Message.Contains("does not exist, or it is not processed"))
            {
                Regex regerx = new Regex("'.*'");
                Match math = regerx.Match(e.Message);
                if (math.Success)
                {
                    return string.Format("Куб '{0}' не существует или не рассчитан.", math.Captures[0].Value);
                }
            }
            throw e;
        }
    }
}
