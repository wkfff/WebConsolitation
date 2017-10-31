using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Services
{
    public interface IMarksDataInitializer
    {
        void CreateMarksForTerritory(int year, D_Territory_RF territory, bool onlyMO);
    }
}