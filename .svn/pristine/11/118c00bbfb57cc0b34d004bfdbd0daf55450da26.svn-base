using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Services
{
    public class ClientServicesFacade : IFinSourceDebtorBookFacade
    {
        private IScheme scheme;

        public ClientServicesFacade(IScheme scheme)
        {
            this.scheme = scheme;
        }

        public CopyRegionVariantService CopyRegionVariantService
        {
            get { return new CopyRegionVariantService(scheme, RegionsAccordanceService); }
        }

        public DataSourceService DataSourceService
        {
            get { return new DataSourceService(scheme); }
        }

        public RegionsAccordanceService RegionsAccordanceService
        {
            get { return new RegionsAccordanceService(scheme, DataSourceService); }
        }

        public RegionsService RegionsService
        {
            get { return new RegionsService(scheme); }
        }

        public TransfertDataService TransfertDataService
        {
            get { return new TransfertDataService(scheme); }
        }

        public VariantService VariantService
        {
            get { return new VariantService(scheme); }
        }
    }
}
