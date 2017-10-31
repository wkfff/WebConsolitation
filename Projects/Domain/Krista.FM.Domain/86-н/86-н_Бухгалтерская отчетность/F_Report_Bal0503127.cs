namespace Krista.FM.Domain
{
    public class F_Report_Bal0503127 : FactTable
    {
        public static readonly string Key = "913012bd-781e-46df-a5cd-74e93b9c4a97";

        public virtual int SourceID { get; set; }

        public virtual int TaskID { get; set; }

        public virtual decimal ApproveBudgAssign { get; set; }

        public virtual decimal budgObligatLimits { get; set; }

        public virtual decimal execFinAuthorities { get; set; }

        public virtual decimal execBankAccounts { get; set; }

        public virtual decimal execNonCashOperation { get; set; }

        public virtual decimal execTotal { get; set; }

        public virtual decimal unexecAssignments { get; set; }

        public virtual decimal unexecBudgObligatLimit { get; set; }

        public virtual string budgClassifCode { get; set; }

        public virtual string Name { get; set; }

        public virtual string lineCode { get; set; }

        public virtual int Section { get; set; }

        public virtual F_F_ParameterDoc RefParametr { get; set; }
    }
}
