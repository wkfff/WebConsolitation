using System.IO;

namespace Krista.FM.RIA.Extensions.FO41.Services
{
    public interface IExportService
    {
        /// <summary>
        /// Экспорт данных в Excel заявки от налогоплательщика. (Ярославль)
        /// </summary>
        /// <param name="applicationId">Идентификатор заявки от налогоплательщика</param>
        Stream ExportForTaxpayer(int applicationId);

        /// <summary>
        /// Экспорт данных в Excel заявки от налогоплательщика. (ХМАО)
        /// </summary>
        /// <param name="applicationId">Идентификатор заявки от налогоплательщика</param>
        Stream ExportForTaxpayerHMAO(int applicationId);

        /// <summary>
        /// Экспорт данных в Excel заявки от ОГВ.
        /// </summary>
        /// <param name="appFromOGVId">Идентификатор заявки от ОГВ</param>
        Stream ExportForOGV(int appFromOGVId);

        /// <summary>
        /// Экспорт данных в Excel заявок по категории и периоду
        /// </summary>
        /// <param name="taxTypeId">Идентификатор типа налога</param>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <param name="categoryName">Наименование категории</param>
        /// <param name="periodId">Идентификатор периода</param>
        Stream ExportResultCategoryHMAO(int taxTypeId, int categoryId, string categoryName, int periodId);
        
        /// <summary>
        /// Экспорт данных в Excel таблицы с итоговыми показателями
        /// </summary>
        /// <param name="appFromOGVId">Идентификатор заявки от ОГВ</param>
        /// <param name="sourceId">Идентификатор источника данных</param>
        Stream ExportResult(int appFromOGVId, int sourceId);

        /// <summary>
        /// Экспорт данных в Excel таблицы с итоговыми показателями по типу налога и периоду
        /// </summary>
        /// <param name="taxTypeId">Идентификатор типа налога</param>
        /// <param name="periodId">Идентификатор периода</param>
        Stream ExportResultTaxType(int taxTypeId, int periodId);

        /// <summary>
        /// Экспорт данных в Excel таблицы с итоговыми показателями ОЦЕНКИ по определенной категории
        /// </summary>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <param name="categoryName">Наименование категории</param>
        /// <param name="taxTypeId">Идентификатор типа налога</param>
        /// <param name="periodId">Идентификатор периода</param>
        Stream ExportEstimateCategoryHMAO(int categoryId, string categoryName, int taxTypeId, int periodId);
    }
}
