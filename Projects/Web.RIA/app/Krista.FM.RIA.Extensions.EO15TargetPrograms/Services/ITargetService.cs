using System.Collections;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Services
{
    public interface ITargetService
    {
        IList GetTargetsTable(int programId);

        IList GetTargetsTableForLookup(int programId);
        
        void Create(D_ExcCosts_ListPrg program, string name, string note);

        void Update(D_ExcCosts_ListPrg program, int targetId, string name, string note);
        
        void Delete(int targetId, int programId);

        D_ExcCosts_Goals GetTarget(int id);

        void DeleteAllTarget(int programId);
    }
}