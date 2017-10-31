using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    public abstract class CreditViewService : DebtBookViewService
    {
        protected CreditViewService(IDebtBookExtension extension, int tabType) 
            : base(extension, tabType)
        {
        }

        public abstract int CreditTypeId { get; }

        protected override System.Text.StringBuilder GetInitNewRecordScript()
        {
            var sb = base.GetInitNewRecordScript();
            sb.AppendLine("record.data.REFTYPECREDIT = {0};".FormatWith(CreditTypeId));
            return sb;
        }
    }
}
