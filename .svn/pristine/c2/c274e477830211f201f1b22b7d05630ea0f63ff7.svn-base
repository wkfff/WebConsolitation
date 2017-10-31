using System;
using System.Collections.Generic;

using Krista.FM.Domain;

namespace Krista.FM.Common.Consolidation.Forms
{
    [CLSCompliant(false)]
    public interface IFormValidationService
    {
        /// <summary>
        /// Полная проверка формы.
        /// </summary>
        /// <param name="form">Проверяемая форма.</param>
        /// <returns>Результат проверки. Список ошибок.</returns>
        List<string> Validate(D_CD_Templates form);

        /// <summary>
        /// Проверка структуры формы.
        /// </summary>
        /// <param name="form">Проверяемая форма.</param>
        /// <returns>Результат проверки. Список ошибок.</returns>
        List<string> ValidateForm(D_CD_Templates form);

        /// <summary>
        /// Проверка соответствия маппинга структуре формы.
        /// </summary>
        /// <param name="form">Проверяемая форма.</param>
        /// <returns>Результат проверки. Список ошибок.</returns>
        List<string> ValidateMapping(D_CD_Templates form);

        /// <summary>
        /// Проверка разметки книги Excel и соответствия разметки маппингу.
        /// </summary>
        /// <param name="form">Проверяемая форма.</param>
        /// <returns>Результат проверки. Список ошибок.</returns>
        List<string> ValidateLayout(D_CD_Templates form);
    }
}