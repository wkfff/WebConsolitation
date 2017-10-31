using System.IO;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Mrot
{
    public interface IExportService
    {
        /// <summary>
        /// Ёкспорт данных в Excel с незавершенным вводом данных.
        /// </summary>
        /// <param name="taskId">Id задачи.</param>
        Stream ExportUnactedRegions(int taskId);

        /// <summary>
        /// Ёкспорт данных в Excel по форме сбора.
        /// </summary>
        /// <param name="taskId">Id задачи.</param>
        Stream ExportFormCollection(int taskId);

        /// <summary>
        /// Ёкспорт данных в Excel отчета по ћќ.
        /// </summary>
        /// <param name="taskId">Id задачи.</param>
        Stream ExportMOReport(int taskId);

        /// <summary>
        /// Ёкспорт данных в Excel отчета по исполнител€м.
        /// </summary>
        /// <param name="taskId">Id задачи.</param>
        Stream ExportExecuters(int taskId);

        /// <summary>
        /// Ёкспорт данных в Excel отчета по исполнител€м.
        /// </summary>
        /// <param name="taskId">Id задачи.</param>
        /// <param name="reportName">Ќаименование формы отчета.</param>
        /// <param name="subjectReport">ѕризнак что отчет консолидированный .</param>
        string GetDocumentName(int taskId, string reportName, bool subjectReport);
    }
}