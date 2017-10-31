using System.Collections.Generic;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Services
{
    public interface IFactRepository : IRepository<F_OIV_REG10Qual> 
    {
        /// <summary>
        /// ���������� �������� ���������� ��� ��������� ������
        /// ��� ���������� � ������ ���������� ��������������������� ������
        /// </summary>
        /// <param name="markId">Id ����������.</param>
        /// <param name="territoryId">Id ����������</param>
        F_OIV_REG10Qual GetFactForMarkTerritory(int markId, int territoryId);

        /// <summary>
        /// ���������� ������ ����������� ��� �������������� ���.
        /// </summary>
        IList<F_OIV_REG10Qual> GetMarksForOiv();

        IList<F_OIV_REG10Qual> GetMarksForIMA();

        /// <summary>
        /// ���������� ������ ����������� ��� ����.
        /// </summary>
        IList<F_OIV_REG10Qual> GetMarks(D_Territory_RF territory);

        /// <summary>
        /// ���������� ������ �� ����������� �� ���������� ����������
        /// </summary>
        IList<F_OIV_REG10Qual> GetTerritories(int markId);
    }
}