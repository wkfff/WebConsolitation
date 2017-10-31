namespace Krista.FM.Domain.Services.FinSourceDebtorBook
{
    public interface IFinSourceDebtorBookFacade
    {
        CopyRegionVariantService CopyRegionVariantService { get; }
        DataSourceService DataSourceService { get; }
        RegionsAccordanceService RegionsAccordanceService { get; }
        RegionsService RegionsService { get; }
        TransfertDataService TransfertDataService { get; }
        VariantService VariantService { get; }
    }
}
