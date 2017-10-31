namespace Krista.FM.Domain
{
    public class F_F_PlanPaymentIndex : FactTable
    {
        public static readonly string Key = "f5b123bb-f37f-42b6-9ea2-4cd9f65218f4";

        public virtual int SourceID { get; set; }

        public virtual int TaskID { get; set; }

        public virtual string Name { get; set; }

        public virtual string LineCode { get; set; }

        public virtual string Kbk { get; set; }

        public virtual decimal Total { get; set; }

        public virtual decimal? FinancialProvision { get; set; }

        public virtual decimal? SubsidyFinSupportFfoms { get; set; }

        public virtual decimal? SubsidyOtherPurposes { get; set; }

        public virtual decimal? CapitalInvestment { get; set; }

        public virtual decimal? HealthInsurance { get; set; }

        public virtual decimal? ServiceTotal { get; set; }

        public virtual decimal? ServiceGrant { get; set; }

        public virtual int Period { get; set; }

        public virtual F_F_ParameterDoc RefParametr { get; set; }
        
        public virtual decimal? Summ5678()
        {
            return FinancialProvision + SubsidyOtherPurposes + CapitalInvestment + HealthInsurance;
        }
    }
}
