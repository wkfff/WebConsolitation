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
        /// Формирует список колонок по годам, начиная с года начала и заканчивая годом окончания реализации проекта
        /// </summary>
        /// <param name="projId">id проекта</param>
        IList<int> GetYearsColumns(int projId);
    }
}