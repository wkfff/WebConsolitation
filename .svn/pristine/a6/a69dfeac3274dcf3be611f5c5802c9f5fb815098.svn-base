using System.Collections.Generic;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Services
{
    public interface IMarksCalculator
    {
        void Calc(IList<F_OMSU_Reg16> facts, int regionId, bool calculatePrognoz);

        void CalcForceProtected(IList<F_OMSU_Reg16> facts, int regionId, bool calculatePrognoz);

        void CalcForceProtectedDoNotSave(IList<F_OMSU_Reg16> facts, int regionId, bool calculatePrognoz);

        IDictionary<D_OMSU_MarksOMSU, int> GetMarkCalculationPlan(int markId);
    }
}
