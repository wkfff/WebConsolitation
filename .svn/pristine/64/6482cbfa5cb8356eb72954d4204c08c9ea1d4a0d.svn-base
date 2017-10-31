using System.Collections.Generic;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.MarksOMSU
{
    public interface IMarksOmsuExtension
    {
        int CurrentYear { get; set; }
        
        FX_Date_YearDayUNV CurrentYearUNV { get; }

        FX_Date_YearDayUNV PreviousYearUNV { get; }

        Users User { get; }

        DataSources DataSourceOmsu { get; }
        
        DataSources DataSourceOmsuPrevious { get; }

        DataSources DataSourceRegion { get; }
        
        D_Regions_Analysis UserRegionCurrent { get; }
        
        D_OMSU_ResponsOIV ResponsOIV { get; }

        /// <summary>
        /// Возвращает список лет, на которые зарегистрированы источники
        /// </summary>
        List<int> Years { get; }
    }
}