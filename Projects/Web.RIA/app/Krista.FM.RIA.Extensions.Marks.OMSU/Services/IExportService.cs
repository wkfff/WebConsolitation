using System.IO;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Services
{
    public interface IExportService
    {
        /// <summary>
        /// Ёкспорт данных в Excel дл€ ќћ—”.
        /// </summary>
        /// <param name="sourceId">Id источника данных.</param>
        /// <param name="regionId">Id района.</param>
        Stream ExportForOmsu(int sourceId, int regionId);

        /// <summary>
        /// Ёкспорт данных в Excel дл€ ќ»¬.
        /// </summary>
        /// <param name="sourceId">Id источника данных.</param>
        /// <param name="markId">Id показател€.</param>
        Stream ExportForOiv(int sourceId, int markId);

        /// <summary>
        /// Ёкспорт данных и/ф "‘орма ввода показателей дл€ оценки эффективности де€тельности органов местного самоуправлени€ городских округов и муниципальных районов автономного округа."
        /// </summary>
        /// <param name="sourceId">Id источника данных.</param>
        /// <param name="markId">Id показател€.</param>
        Stream ExportOivInputData(int sourceId, int markId);

        /// <summary>
        /// Ёкспорт данных интерфейсов неэффективных показателей.
        /// </summary>
        /// <param name="markId">Id базового показател€ с интерфейса.</param>
        /// <param name="itfHeader">«аголовок интерфейса, будет размещен в файле</param>
        Stream ExportIneffExpenceFacts(int markId, string itfHeader);
    }
}