using System.Collections.Generic;

using Krista.FM.Common.Consolidation.Data;
using Krista.FM.Domain;

namespace Krista.FM.Common.Consolidation.Calculations
{
    public interface IDataProvider
    {
        /// <summary>
        /// Устанавливает контекст данных для правой части выражения.
        /// Контекст определяет из какой формы и какого раздела необходимо выбрать данные.
        /// </summary>
        /// <param name="sectionName">Код раздела.</param>
        /// <param name="formName">Код формы.</param>
        /// <param name="slave">Подотчетные субъекты.</param>
        void SetContext(string sectionName, string formName, bool slave);

        IList<IRecord> GetSectionRows();

        IList<IRecord> GetSectionRows(string sqlFilter);

        IList<D_Form_TableColumn> GetMetaColumns();
    }
}
