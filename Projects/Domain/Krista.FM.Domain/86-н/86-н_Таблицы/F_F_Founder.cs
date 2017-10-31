namespace Krista.FM.Domain
{
    public class F_F_Founder : FactTable
    {
        public static readonly string Key = "ac8c376c-0668-4a6b-a857-90e693554362";

        public virtual int SourceID { get; set; }

        public virtual int TaskID { get; set; }

        public virtual bool formative { get; set; }

        public virtual bool stateTask { get; set; }

        public virtual bool? supervisoryBoard { get; set; }

        public virtual bool manageProperty { get; set; }

        public virtual bool financeSupply { get; set; }

        public virtual F_Org_Passport RefPassport { get; set; }

        public virtual D_Org_OrgYchr RefYchred { get; set; }
    }
}
