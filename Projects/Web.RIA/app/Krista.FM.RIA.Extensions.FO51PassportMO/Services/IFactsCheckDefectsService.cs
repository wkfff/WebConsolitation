using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.FO51PassportMO.Services
{
    public interface IFactsCheckDefectsService
    {
        FactsCheckDefectsService.DefectsListModel GetDefects(int periodId, D_Regions_Analysis region, bool exitIfDefect);

        bool ReportDataExists(int periodId, D_Regions_Analysis region);
    }
}
