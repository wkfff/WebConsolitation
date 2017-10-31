using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.FO41.Services
{
    public interface ICategoryTaxpayerService
    {
        IQueryable GetQueryAll();

        /// <summary>
        /// Возвращает список категорий,  по которым указанный ОГВ отвественный
        /// </summary>
        /// <param name="ogv">Идентификатор ОГВ</param>
        /// <param name="exceptIDs">Список идентификаторов категорий, которые не нужно показывать или null</param>
        /// <returns>Список категорий</returns>
        IQueryable GetByOGV(int ogv, string exceptIDs);

        /// <summary>
        /// Возвращает категорию по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <returns>Объект категории</returns>
        D_Org_CategoryTaxpayer GetQueryOne(int id);

        IList<D_Org_CategoryTaxpayer> GetByTax(int taxId);
    }
}
