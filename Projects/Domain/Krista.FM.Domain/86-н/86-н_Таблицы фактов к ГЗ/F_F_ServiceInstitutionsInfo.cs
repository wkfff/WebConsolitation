using System;

namespace Krista.FM.Domain
{
	public class F_F_ServiceInstitutionsInfo : FactTable
	{
		public static readonly string Key = "58bd4a70-6172-4d40-a2a9-46a83ec5b296";

        public virtual int SourceID { get; set; }
        public virtual int TaskID { get; set; }
        public virtual string Code { get; set; }
        public virtual string OKOPF { get; set; }
        public virtual DateTime? DateReg { get; set; }
        public virtual string InstKindCode { get; set; }
        public virtual string InstKindName { get; set; }
        public virtual D_Services_Service RefService { get; set; }
        public virtual D_Org_Structure RefStructure { get; set; }
        public virtual string OKVED { get; set; }
    }
}
