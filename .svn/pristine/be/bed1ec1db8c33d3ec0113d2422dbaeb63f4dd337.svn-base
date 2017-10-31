using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.FO41.Presentation.Models;

namespace Krista.FM.RIA.Extensions.FO41.Services
{
    public interface IAppPrivilegeService : ILinqRepository<D_Application_Privilege>
    {
        IQueryable GetQueryAll();

        object GetQueryOne(int id);

        D_Application_Privilege Get(int id);

        int GetYear(int id);

        IQueryable GetQueryByCategory(int categoryId);

        IQueryable GetRequestToInputData(int categoryId, int periodId);

        IQueryable GetReqPrevPeriods(int orgId, int year);

        DetailsViewModel GetDetailsViewModel(int id);

        /// <summary>
        /// Удаление заявки
        /// </summary>
        /// <param name="requestId">Идентификатор заявки</param>
        string RemoveRequest(int requestId);

        // для ХМАО

        /// <summary>
        /// Получение списка заявок по налогоплательщику
        /// </summary>
        /// <param name="taxPayerId">Идентификатор налогоплательщика</param>
        /// <param name="periodId">Идентификатор периода</param>
        /// <returns>Список заявок</returns>
        IQueryable GetAppForTaxPayer(int taxPayerId, int periodId);

        /// <summary>
        /// Получение списка заявок по категории и периоду
        /// </summary>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <param name="periodId">Идентификатор периода</param>
        /// <returns>Список заявок</returns>
        IQueryable GetAppForOGV(int categoryId, int periodId);

        /// <summary>
        /// Получение списка заявок по категории и периоду
        /// </summary>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <param name="periodId">Идентификатор периода</param>
        /// <returns>Список заявок</returns>
        IList<D_Application_Privilege> GetAppForOGVList(int categoryId, int periodId);

        HMAODetailViewModel GetHMAODetailsViewModel(int id, int? typeTaxId);

        IQueryable GetQueryByCategory(int? categoryId, int? ogvId, int? periodId, List<int> filters);

        IQueryable GetQueryByTaxPayer(int taxPayerId, int? periodId, List<int> filters);
    }
}