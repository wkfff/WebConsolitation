namespace Krista.FM.RIA.Extensions.DebtBook
{
    public class StructureBudgetCreditViewService : DebtBookViewService
    {
        public StructureBudgetCreditViewService(IDebtBookExtension extension)
            : base(extension, 3)
        {
        }

        public override string GetDataFilter()
        {
            return "RefKind = 2";
        }
    }
}
