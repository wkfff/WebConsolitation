using System.IO;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Mrot
{
    public interface IExportService
    {
        /// <summary>
        /// ������� ������ � Excel � ������������� ������ ������.
        /// </summary>
        /// <param name="taskId">Id ������.</param>
        Stream ExportUnactedRegions(int taskId);

        /// <summary>
        /// ������� ������ � Excel �� ����� �����.
        /// </summary>
        /// <param name="taskId">Id ������.</param>
        Stream ExportFormCollection(int taskId);

        /// <summary>
        /// ������� ������ � Excel ������ �� ��.
        /// </summary>
        /// <param name="taskId">Id ������.</param>
        Stream ExportMOReport(int taskId);

        /// <summary>
        /// ������� ������ � Excel ������ �� ������������.
        /// </summary>
        /// <param name="taskId">Id ������.</param>
        Stream ExportExecuters(int taskId);

        /// <summary>
        /// ������� ������ � Excel ������ �� ������������.
        /// </summary>
        /// <param name="taskId">Id ������.</param>
        /// <param name="reportName">������������ ����� ������.</param>
        /// <param name="subjectReport">������� ��� ����� ����������������� .</param>
        string GetDocumentName(int taskId, string reportName, bool subjectReport);
    }
}