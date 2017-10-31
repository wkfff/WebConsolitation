using System;
using System.Collections.Generic;

using Krista.FM.Common.Consolidation.Forms.Layout;
using Krista.FM.Domain;

namespace Krista.FM.Common.Consolidation.Forms
{
    /// <summary>
    /// Отвечает за валидацию форм.
    /// </summary>
    [CLSCompliant(false)]
    public class FormValidationService : IFormValidationService
    {
        /// <summary>
        /// Полная проверка формы.
        /// </summary>
        /// <param name="form">Проверяемая форма.</param>
        /// <returns>Результат проверки. Список ошибок.</returns>
        public List<string> Validate(D_CD_Templates form)
        {
            List<string> errors = new List<string>();

            errors.AddRange(ValidateForm(form));
            errors.AddRange(ValidateMapping(form));
            errors.AddRange(ValidateLayout(form));

            return errors;
        }

        /// <summary>
        /// Проверка структуры формы.
        /// </summary>
        /// <param name="form">Проверяемая форма.</param>
        /// <returns>Результат проверки. Список ошибок.</returns>
        public List<string> ValidateForm(D_CD_Templates form)
        {
            return new FormValidator().Validate(form);
        }

        /// <summary>
        /// Проверка соответствия маппинга структуре формы.
        /// </summary>
        /// <param name="form">Проверяемая форма.</param>
        /// <returns>Результат проверки. Список ошибок.</returns>
        public List<string> ValidateMapping(D_CD_Templates form)
        {
            return new FormMappingValidator().Validate(form);
        }

        /// <summary>
        /// Проверка разметки книги Excel и соответствия разметки маппингу.
        /// </summary>
        /// <param name="form">Проверяемая форма.</param>
        /// <returns>Результат проверки. Список ошибок.</returns>
        public List<string> ValidateLayout(D_CD_Templates form)
        {
            return new FormLayoutMarkupValidator().Validate(form);
        }
    }
}
