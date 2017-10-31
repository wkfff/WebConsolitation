using System.Collections.Generic;
using System.Data;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Models;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Services
{
    public interface IInvestPlanService
    {
        DataTable GetInvestsTable(int refProjId, InvProjInvestType projInvestType);
        
        void CreateFactData(int refProjId, int refIndicatorId, List<int> years, Dictionary<string, string> rowData);
        
        void UpdateFactData(int refProjId, int refIndicatorId, List<int> years, Dictionary<string, string> rowData);
        
        void DeleteFactData(int refProjId, int refIndicatorId);

        /// <summary>
        /// ��������� ������ ������� �� �����, ������� � ���� ������ � ���������� ����� ��������� ���������� �������
        /// </summary>
        /// <param name="projId">id �������</param>
        IList<int> GetYearsColumns(int projId);
    }
}