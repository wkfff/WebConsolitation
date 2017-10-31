using System;

namespace Krista.FM.Domain
{
    public class F_Report_BalF0503721 : FactTable
    {
        public static readonly string Key = "9f7512f5-aa02-4396-a4c0-0360236f051c";

        public virtual int SourceID { get; set; }

        public virtual int TaskID { get; set; }

        public virtual string Name { get; set; }

        public virtual string lineCode { get; set; }

        public virtual string analyticCode { get; set; }

        public virtual decimal targetFunds { get; set; }

        public virtual decimal? services { get; set; }

        public virtual decimal? temporaryFunds { get; set; }

        public virtual decimal total { get; set; }

        public virtual int Section { get; set; }

        public virtual F_F_ParameterDoc RefParametr { get; set; }

        public virtual decimal? StateTaskFunds { get; set; }

        public virtual decimal? RevenueFunds { get; set; }
    }
}
