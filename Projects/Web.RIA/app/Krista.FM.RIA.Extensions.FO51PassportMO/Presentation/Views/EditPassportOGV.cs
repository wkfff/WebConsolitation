using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.FO51PassportMO.Presentation.Views
{
    public class EditPassportOGV : EditPassportMOView
    {
        public EditPassportOGV(
            IFO51Extension extension,
            ILinqRepository<D_Regions_Analysis> regionRepository,
            ILinqRepository<D_Marks_PassportMO> marksPassportRepository,
            D_Regions_Analysis regionToUse, 
            int periodId) 
        : base(extension, regionRepository, marksPassportRepository, periodId / 10000)
        {
            Region = regionToUse;
            PeriodId = periodId;
        }
    }
}
