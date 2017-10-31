using System;

namespace Krista.FM.Domain
{
    public class F_Report_BalF0503730 : FactTable
    {
        public static readonly string Key = "0b19b1bd-824b-49a4-bb90-14857686a541";

        public virtual int SourceID { get; set; }

        public virtual int TaskID { get; set; }

        public virtual string Name { get; set; }

        public virtual string lineCode { get; set; }

        public virtual Decimal targetFundsBegin { get; set; }

        public virtual Decimal targetFundsEnd { get; set; }

        public virtual Decimal? servicesBegin { get; set; }

        public virtual Decimal? servicesEnd { get; set; }

        public virtual Decimal? temporaryFundsBegin { get; set; }

        public virtual Decimal? temporaryFundsEnd { get; set; }

        public virtual Decimal totalStartYear { get; set; }

        public virtual Decimal totalEndYear { get; set; }

        public virtual int Section { get; set; }

        public virtual F_F_ParameterDoc RefParametr { get; set; }

        public virtual Decimal? StateTaskFundStartYear { get; set; }

        public virtual Decimal? StateTaskFundEndYear { get; set; }

        public virtual Decimal? RevenueFundsStartYear { get; set; }

        public virtual Decimal? RevenueFundsEndYear { get; set; }

        public F_Report_BalF0503730()
        {
            servicesBegin = 0;
            servicesEnd = 0;
            temporaryFundsBegin = 0;
            temporaryFundsEnd = 0;
            StateTaskFundStartYear = 0;
            StateTaskFundEndYear = 0;
            RevenueFundsStartYear = 0;
            RevenueFundsEndYear = 0;
        }
    }
}
