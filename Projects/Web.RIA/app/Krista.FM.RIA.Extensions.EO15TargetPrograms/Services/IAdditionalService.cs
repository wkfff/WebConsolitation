using System.Collections;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Services
{
    public interface IAdditionalService
    {
        D_ExcCosts_Creators GetCreator(string login);

        D_ExcCosts_Creators GetCreator(int id);

        FX_Date_YearDayUNV GetRefYear(int year);

        FX_Date_YearDayUNV GetRefYearMonth(int year, int month); 
        
        FX_ExcCosts_TpPrg GetRefTypeProg(int id);

        IList GetAllOwnersList();

        IList GetAllUnitListForLookup();

        D_Units_OKEI GetUnit(int id);

        FX_Date_YearDayUNV GetRefYearMonthDay(int year, int month, int day);

        D_Territory_RF GetRefTerritory(int id);
    }
}
