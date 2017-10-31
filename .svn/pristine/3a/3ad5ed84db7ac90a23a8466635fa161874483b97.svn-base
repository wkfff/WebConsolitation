using System;
using System.Collections.Generic;
using NHibernate.Criterion;

namespace Krista.FM.RIA.Extensions.DebtBook.Services.DAL
{
    public interface IObjectRepository
    {
        /// <summary>
        /// Находит все записи в таблице recType, удовлетворяющие условию criterion
        /// </summary>
        IList<object> FindAll(Type recType, ICriterion criterion);

        /// <summary>
        /// Находит все записи в таблице с именем entityFullDbName, удовлетворяющие условиям фильтра
        /// </summary>
        IList<object> GetRows(string entityFullDbName, string serverFilter, int? variantId, int? sourceId);

        /// <summary>
        /// Находит запись по id в таблице с именем entityFullDbName
        /// </summary>
        /// <returns>DomainObject row</returns>
        object GetRow(string entityFullDbName, int id);

        /// <summary>
        /// Находит "родительскую" запись, полученную при копировании варианта, через SoureKey=ID
        /// </summary>
        object GetPrevious(object record);
    }
}
