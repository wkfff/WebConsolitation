using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Services
{
    public class OmsuEstimateService : IOmsuEstimateService
    {
        public D_OMSU_CompEstim GetMark()
        {
            throw new NotImplementedException();
        }

        public F_OMSU_Reg17 GetFact(int markId, int regionId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<F_OMSU_Reg17> GetFacts(int markId)
        {
            throw new NotImplementedException();
        }

        public DataTable GetTargetFactsView(int markId)
        {
            throw new NotImplementedException();
        }
    }
}
