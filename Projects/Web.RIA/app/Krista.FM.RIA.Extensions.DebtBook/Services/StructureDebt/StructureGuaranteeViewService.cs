namespace Krista.FM.RIA.Extensions.DebtBook
{
    public class StructureGuaranteeViewService : DebtBookViewService
    {
        public StructureGuaranteeViewService(IDebtBookExtension extension)
            : base(extension, 3)
        {
        }

        public override string GetDataFilter()
        {
            return "RefKind = 4";
        }
    }
}
