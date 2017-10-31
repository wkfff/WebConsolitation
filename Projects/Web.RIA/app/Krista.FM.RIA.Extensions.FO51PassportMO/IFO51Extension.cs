using System.Collections.Generic;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.FO51PassportMO
{
    public interface IFO51Extension 
    {
        Users User { get; }

        /// <summary>
        /// Группа, к которой принадлежит пользователь - МО или ОГВ
        /// </summary>
        int UserGroup { get; }

        /// <summary>
        /// Только для ОГВ
        /// Ответственный ОГВ
        /// </summary>
        D_OMSU_ResponsOIV ResponsOIV { get; }

        /// <summary>
        /// Только для ОГВ
        /// Показатель, за который ответственен ОГВ
        /// </summary>
        D_Marks_PassportMO MarkOIV { get; }

        List<DataSources> DataSourcesFO51 { get; }
        
        D_Regions_Analysis GetActualRegion(int periodId, int regionId);

        List<D_Regions_Analysis> GetRegions(int periodId);

        D_Regions_Analysis GetRegionByBridge(int periodId, int regionBridgeId);

        string GetSvodReportUrl();

        string GetReportForRegionsUrl();
    }
}
