namespace Krista.FM.RIA.Extensions.DebtBook
{
    public class StructureOrganizationCreditViewService : DebtBookViewService
    {
        public StructureOrganizationCreditViewService(IDebtBookExtension extension)
            : base(extension, 3)
        {
        }

        public override string GetDataFilter()
        {
            return "RefKind = 3";
        }
    }
}
