using System.IO;

namespace Krista.FM.RIA.Extensions.OrgGKH.Services
{
    public interface IExportService
    {
        /// <summary>
        /// Экспорт данных в Excel еженедельной формы сбора
        ///  </summary>
        /// <param name="orgId">Идентификатор организации</param>
        /// <param name="periodId">Идентификатор периода</param>
        /// <param name="sourceId">Идентификатор источника</param>
        /// <param name="terr">Наименование территории</param>
        Stream ExportWeek(int orgId, int periodId, int sourceId, string terr);

        /// <summary>
        /// Экспорт данных в Excel ежемесячной формы сбора
        ///  </summary>
        /// <param name="orgId">Идентификатор организации</param>
        /// <param name="periodId">Идентификатор периода</param>
        /// <param name="sourceId">Идентификатор источника</param>
        /// <param name="terr">Наименование территории</param>
        Stream ExportMonth(int orgId, int periodId, int sourceId, string terr);
    }
}
