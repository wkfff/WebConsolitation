using System.Collections.Generic;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Models;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Services
{
    public interface IProjectService
    {
        IList<ProjectsViewModel> GetProjectsTable(InvProjPart partition, bool[] projStatusFilters);

        void ChangeProjectPart(int projectId); 
        
        void DeleteProject(int projectId);

        D_InvProject_Reestr GetProject(int id);

        ProjectDetailViewModel GetProjectModel(int projectId);

        ProjectDetailViewModel GetInitialProjectModel(InvProjPart projPart);

        void Validate(D_InvProject_Reestr entity);

        void SaveProject(D_InvProject_Reestr entityNew, D_InvProject_Reestr entityOld);
        
        decimal GetSumInvestPlan(int refProjId);

        void DeleteIncorrectFactData(D_InvProject_Reestr entity);

        InvProjStatus GetProjectStatus(int projectId);
    }
}