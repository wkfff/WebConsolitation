using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services
{
    public interface IBaseDocService : INewRestService
    {
        /// <summary>
        /// �������� ���������
        /// </summary>
        /// <param name="docId">ID ���������</param>
        void DeleteDoc(int docId);

        /// <summary>
        /// �������� �� ������ ��������
        /// </summary>
        /// <param name="docId">ID ���������</param>
        bool CheckDocEmpty(int docId);

        /// <summary>
        /// ����������� �������� ��������� � ����� ��������
        /// </summary>
        /// <param name="docId">ID ���������</param>
        void CopyContent(int docId);
    }
}