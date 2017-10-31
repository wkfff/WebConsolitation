using System.Collections.Generic;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.FO51PassportMO.Services
{
    public interface IFactsPassportMOService
    {
        List<MarksPassportMOFact> GetPassportMOFacts(int markId, int periodId, int regionId, int sourceId, bool isFictRegion);

        int GetStateId(int periodId, int regionId, int sourceId);

        void SaveStateForFacts(int periodId, int stateId, int regionId, int sourceId);

        IEnumerable<F_Marks_PassportMO> FindFacts(
            int regionId,
            int sourceId,
            int markId,
            int periodId,
            int variantCode);

        F_Marks_PassportMO FindFirstOrDefault(
            int regionId,
            int sourceId,
            int markId,
            int periodId,
            int variantCode);
    }
}
