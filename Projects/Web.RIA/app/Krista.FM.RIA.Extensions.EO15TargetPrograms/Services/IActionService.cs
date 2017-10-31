using System.Collections;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Services
{
    public interface IActionService
    {
        IList GetActionsTable(int programId);

        IList GetActionsTableForLookup(int programId); 
        
        void Create(int programId, D_ExcCosts_Tasks task, string actionName, string actionNote, string actionResult, D_ExcCosts_Creators owner);

        void Update(int programId, int actionId, D_ExcCosts_Tasks task, string actionName, string actionNote, string actionResult, D_ExcCosts_Creators owner);
        
        void Delete(int actionId, int programId);

        D_ExcCosts_Events GetAction(int id);

        void DeleteAllAction(int taskId);

        bool IsUserInActionsOwners(D_ExcCosts_ListPrg program, string userLogin);
    }
}