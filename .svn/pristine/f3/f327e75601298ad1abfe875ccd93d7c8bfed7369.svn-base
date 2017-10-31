using System.Collections.Generic;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.FO51PassportMO.Services
{
    public interface IFactsByMesOtchService
    {
        /// <summary>
        /// Сохранение данных из месячного отчета
        /// </summary>
        void SaveMonthReportData(
            D_Regions_Analysis region,
            FX_Date_YearDayUNV period,
            IList<F_Marks_PassportMO> facts, 
            IEnumerable<D_Marks_PassportMO> marksAll,
            int sourceId, 
            FX_MeansType_SKIF defaultMeansType,
            FX_BdgtLevels_SKIF defaultBdgt);
    }
}
