using System.Collections.Generic;
using System.Data;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Services
{
    public interface IIneffExpencesService
    {
        D_OMSU_MarksOMSU GetMarkIneffGkh();

        D_OMSU_MarksOMSU GetMarkIneffOmu();
        
        D_OMSU_MarksOMSU GetMarkIneffEducation();
        
        D_OMSU_MarksOMSU GetMarkIneffMedicine();

        void CalculateFactToRam(F_OMSU_Reg16 fact);

        IEnumerable<F_OMSU_Reg16> GetFacts(int markId);

        F_OMSU_Reg16 GetFact(int markId, int regionId);

        IDictionary<D_OMSU_MarksOMSU, int> GetMarkCalculationPlan(int markId);

        DataTable GetTargetFactsViewModel(int targetMarkId, bool withSourceFacts);

        IDictionary<F_OMSU_Reg16, int> GetSourceFactsWithHierarchy(IDictionary<D_OMSU_MarksOMSU, int> markCalculationPlan, int regionId);
    }
}
