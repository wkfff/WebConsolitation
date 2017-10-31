using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Services
{
    /// <summary>
    /// ������������ ������ � �������� ������.������ � ��������� ���������� ����(���������)
    /// </summary>
    public interface IRegionsRepository : ILinqRepository<D_Regions_Analysis>
    {
        int GetDatasourceYear();
    }
}