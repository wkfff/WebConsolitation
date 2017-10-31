using Ext.Net.MVC;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance
{
    public interface IAnnualBalanceService : INewRestService
    {
        RestResult F0503730Read(int recId, int section);

        RestResult F0503730Save(string data, int recId, int section);

        RestResult F0503121Read(int recId, int section);

        RestResult F0503121Save(string data, int recId, int section);

        RestResult F0503127Read(int recId, int section);

        RestResult F0503127Save(string data, int recId, int section);

        RestResult F0503130Read(int recId, int section);

        RestResult F0503130Save(string data, int recId, int section);

        RestResult F0503137Read(int recId, int section);

        RestResult F0503137Save(string data, int recId, int section);
        
        RestResult F0503737Read(int recId, int section);

        RestResult F0503737Save(string data, int recId, int section);

        /// <summary>
        /// �������� ���������
        /// </summary>
        /// <param name="recId">ID ���������</param>
        void DeleteDoc(int recId);

        /// <summary>
        /// ������ ���� ��� �������� �����������
        /// </summary>
        /// <param name="docId">������������� ���������</param>
        /// <param name="section">����������� � ���������</param>
        void CalculateSumm(int docId, int section);

        /// <summary>
        /// ���������� ��������� �� ��������� 
        /// </summary>
        void �heckDocContent(int recId);
    }
}