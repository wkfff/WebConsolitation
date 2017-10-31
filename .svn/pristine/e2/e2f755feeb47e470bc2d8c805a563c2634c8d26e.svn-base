using System.IO;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Services
{
    public interface IExportService
    {
        /// <summary>
        /// Ёкспорт данных в Excel дл€ ќ»¬.
        /// </summary>
        Stream ExportForOiv(int sourceId, int territoryId, bool isIMA);

        /// <summary>
        /// Ёкспорт данных в Excel дл€ ќћ—”.
        /// </summary>
        Stream ExportForOmsu(int sourceId, int territoryId);

        /// <summary>
        /// Ёкспорт данных в Excel дл€ ќ»¬ в разрезе ќћ—”.
        /// </summary>
        Stream ExportForOmsuCompare(int sourceId, int? markId);
    }
}