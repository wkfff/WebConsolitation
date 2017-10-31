using System.Collections;
using System.Collections.Generic;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Services
{
    public interface IAdditionalDataService
    {
        FX_Date_YearDayUNV GetRefYearQuarter(int year, int quarter);
        
        FX_Date_YearDayUNV GetRefYear(int year);

        FX_Date_YearDayUNV GetRefYearDayUnvUndefined();

        D_Territory_RF GetRefTerritory(int id);
        
        FX_InvProject_Status GetRefStatus(int id);
        
        FX_InvProject_Part GetRefPart(int id);
        
        D_OK_OKVED GetRefOKVED(int id);

        IList GetIndicatorList(int refTypeI);

        D_InvProject_Indic GetIndicator(int id);
    }
}