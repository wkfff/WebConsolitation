using System.Collections.Generic;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.FO41.Presentation.Models;

namespace Krista.FM.RIA.Extensions.FO41.Services
{
    public interface IFactsService : ILinqRepository<F_Marks_DataPrivilege>
    {
        object GetQueryOne(int id);

        IList<F_Marks_DataPrivilege> GetFactForIndicatorApplic(int indicatorId, int applicationId);

        /// <summary>
        /// Возвращает данные детализирующий показателей для заданной заявки от налогоплательщика
        /// </summary>
        /// <param name="requestId">Идентификатор заявки от налогоплательщика</param>
        IList<T_Marks_Detail> GetDetaiFactsForOrg(int requestId);

        /// <summary>
        /// Возвращает данные показателей для заданного Id или пустые значения для новой записи
        /// </summary>
        /// <param name="requestId">Id заявки, данные по которой нужно вернуть.</param>
        IList<F_Marks_DataPrivilege> GetFactsForOrganization(int requestId);

        /// <summary>
        /// Возвращает данные показателей для заданной категории из источника sourceId за период periodId
        /// </summary>
        /// <param name="sourceId">Идентификатор источника</param>
        /// <param name="periodId">Идентификатор периода</param>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <returns>Список фактов</returns>
        IList<F_Marks_DataPrivilege> GetFacts(int sourceId, int periodId, int categoryId);

        /// <summary>
        /// Возвращает данные показателей для заданного Id или пустые значения для новой записи
        /// </summary>
        /// <param name="requestId">Id заявки, данные по которой нужно вернуть.</param>
        IList<F_Marks_DataPrivilege> GetFactsForOGV(int requestId);

        IList<F_Marks_Privilege> GetFactForIndicatorCategory(int indicatorId, int categoryId);

        IList<F_Marks_DataPrivilege> GetFactsForOrganizationHMAO(int requestId, int taxTypeId);
        
        IList<F_Marks_DataPrivilege> GetFactsForOgvHMAO(int categoryId, int taxTypeId, int periodId);

        IList<Dictionary<string, object>> GetResultFormByTax(int taxTypeId, int periodId);

        /// <summary>
        /// Возвращает данные оценки по категории в периоде
        /// </summary>
        /// <param name="periodId">Идентификатор периода</param>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <param name="taxTypeId">Идентификатор типа налога</param>
        IEnumerable<IndicatorsModel> GetEstimateDataHMAO(int periodId, int categoryId, int taxTypeId);
    }
}