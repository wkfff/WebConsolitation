using System.Collections.Generic;
using System.Data;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Services
{
    public interface IOmsuEstimateService
    {
        D_OMSU_CompEstim GetMark();

        F_OMSU_Reg17 GetFact(int markId, int regionId);

        IEnumerable<F_OMSU_Reg17> GetFacts(int markId);
        
        DataTable GetTargetFactsView(int markId);
    }
}
