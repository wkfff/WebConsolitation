using System.Collections.Generic;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.FO51PassportMO.Services
{
    public interface IFactsPassportMOSaveService
    {
        void SaveUpdatedRecordsAndCalc(
            List<Dictionary<string, object>> updatedRecords, 
            D_Regions_Analysis region, 
            DataSources source, 
            int periodId, 
            int year, 
            int month, 
            int periodForYear, 
            int periodLastYear, 
            D_Marks_PassportMO parentMark);

        void SaveMesOtchReCalc(
            D_Regions_Analysis region,
            DataSources source,
            int periodId,
            int year,
            int month,
            int periodForYear,
            int periodLastYear,
            D_Marks_PassportMO parentMark);
    }
}
