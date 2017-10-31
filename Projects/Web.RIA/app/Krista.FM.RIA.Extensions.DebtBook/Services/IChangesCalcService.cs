using System.Collections.Generic;
using Krista.FM.RIA.Core.ViewModel;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    public interface IChangesCalcService
    {
        /// <summary>
        /// Пересчет всех платежей договора.
        /// </summary>
        /// <param name="entity">Сущность договора.</param>
        /// <param name="documentId">Id договора.</param>
        /// <param name="formules">Список формул.</param>
        void Recalc(IEntity entity, int documentId, Dictionary<string, ColumnState> formules);
    }
}