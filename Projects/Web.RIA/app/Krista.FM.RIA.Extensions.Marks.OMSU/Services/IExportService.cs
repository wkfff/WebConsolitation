using System.IO;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Services
{
    public interface IExportService
    {
        /// <summary>
        /// ������� ������ � Excel ��� ����.
        /// </summary>
        /// <param name="sourceId">Id ��������� ������.</param>
        /// <param name="regionId">Id ������.</param>
        Stream ExportForOmsu(int sourceId, int regionId);

        /// <summary>
        /// ������� ������ � Excel ��� ���.
        /// </summary>
        /// <param name="sourceId">Id ��������� ������.</param>
        /// <param name="markId">Id ����������.</param>
        Stream ExportForOiv(int sourceId, int markId);

        /// <summary>
        /// ������� ������ �/� "����� ����� ����������� ��� ������ ������������� ������������ ������� �������� �������������� ��������� ������� � ������������� ������� ����������� ������."
        /// </summary>
        /// <param name="sourceId">Id ��������� ������.</param>
        /// <param name="markId">Id ����������.</param>
        Stream ExportOivInputData(int sourceId, int markId);

        /// <summary>
        /// ������� ������ ����������� ������������� �����������.
        /// </summary>
        /// <param name="markId">Id �������� ���������� � ����������.</param>
        /// <param name="itfHeader">��������� ����������, ����� �������� � �����</param>
        Stream ExportIneffExpenceFacts(int markId, string itfHeader);
    }
}