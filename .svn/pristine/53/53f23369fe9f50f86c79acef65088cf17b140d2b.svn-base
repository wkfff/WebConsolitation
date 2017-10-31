using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.FO41.Presentation.Models;

namespace Krista.FM.RIA.Extensions.FO41.Services
{
    public interface IAppFromOGVService : ILinqRepository<D_Application_FromOGV>
    {
        IQueryable GetQueryAll();

        IQueryable GetQueryByOGV(int? ogvId, List<int> filters, int periodId);

        /// <summary>
        /// Получает номер заявки от ОГВ по категории
        /// </summary>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <param name="periodId">Идентификатор периода</param>
        /// <returns>Идентификатор заявки</returns>
        int GetAppIdByCategory(int categoryId, int periodId);

        object GetQueryOne(int id);

        DetailsEstimateModel GetDetailsViewModel(int id);

        DetailsEstimateModel GetDetailsViewModelByCategory(int categoryId, int periodId);

        DetailsEstimateModel GetDetailsViewModelByID(int appFromOGVId);
    }
}
