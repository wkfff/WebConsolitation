using System.Collections.Generic;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine
{
    public interface IFormScriptingEngine
    {
        /// <summary>
        /// Создает структуры в базе данных для хранения данных формы.
        /// </summary>
        /// <param name="form">Мета-описания структуры формы.</param>
        /// <param name="version">Версия формы.</param>
        IList<string> Create(D_CD_Templates form, int version);

        /// <summary>
        /// Уданяет структуры из базы данных для для указанной формы.
        /// АХТУНГ! Этод метод только для внутреннего использования (отладки), 
        /// в реальном мире структуры и данные формы не должны удаляться.
        /// </summary>
        /// <param name="form">Мета-описания структуры формы.</param>
        IList<string> Drop(D_CD_Templates form);
    }
}
