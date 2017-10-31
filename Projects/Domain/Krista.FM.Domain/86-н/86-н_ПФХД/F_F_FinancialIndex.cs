namespace Krista.FM.Domain
{
    public class F_F_FinancialIndex : FactTable
    {
        public static readonly string Key = "f6d4c343-2f13-4d5e-9ebd-8f530567cc34";

        public virtual int SourceID { get; set; }

        public virtual int TaskID { get; set; }

        public virtual decimal? NonFinancialAssets { get; set; }

        public virtual decimal? RealAssets { get; set; }

        public virtual decimal? RealAssetsDepreciatedCost { get; set; }

        public virtual decimal? HighValuePersonalAssets { get; set; }

        public virtual decimal? HighValuePADepreciatedCost { get; set; }

        public virtual decimal? FinancialAssets { get; set; }

        public virtual decimal? MoneyInstitutions { get; set; }

        public virtual decimal? FundsAccountsInstitution { get; set; }

        public virtual decimal? FundsPlacedOnDeposits { get; set; }

        public virtual decimal? OtherFinancialInstruments { get; set; }

        public virtual decimal? DebitIncome { get; set; }

        public virtual decimal? DebitExpense { get; set; }

        public virtual decimal? FinancialCircumstanc { get; set; }

        public virtual decimal? Debentures { get; set; }

        public virtual decimal? AccountsPayable { get; set; }

        public virtual decimal? KreditExpired { get; set; }

        public virtual F_F_ParameterDoc RefParametr { get; set; }
    }
}
