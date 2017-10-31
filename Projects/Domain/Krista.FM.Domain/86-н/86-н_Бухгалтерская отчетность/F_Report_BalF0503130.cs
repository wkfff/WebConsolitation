using System;

namespace Krista.FM.Domain
{
    public class F_Report_BalF0503130 : FactTable
    {
        public static readonly string Key = "f3aab193-f603-4665-b69e-5736f3ba108a";

        public virtual int SourceID { get; set; }

        public virtual int TaskID { get; set; }

        public virtual string Name { get; set; }

        public virtual string lineCode { get; set; }

        public virtual Decimal budgetActivityBegin { get; set; }

        public virtual Decimal budgetActivityEnd { get; set; }

        public virtual Decimal incomeActivityBegin { get; set; }

        public virtual Decimal incomeActivityEnd { get; set; }

        public virtual Decimal availableMeansBegin { get; set; }

        public virtual Decimal availableMeansEnd { get; set; }

        public virtual Decimal totalBegin { get; set; }

        public virtual Decimal totalEnd { get; set; }

        public virtual int Section { get; set; }

        public virtual F_F_ParameterDoc RefParametr { get; set; }
    }
}
