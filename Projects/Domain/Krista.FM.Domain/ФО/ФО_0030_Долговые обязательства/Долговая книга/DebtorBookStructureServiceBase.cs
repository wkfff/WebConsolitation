namespace Krista.FM.Domain
{
    public abstract class DebtorBookStructureServiceBase : DebtorBookFactBase
    {
        public virtual int TaskID { get; set; }
        public virtual FX_FX_KindMunDebt RefKind { get; set; }
    }
}
