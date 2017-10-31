using System;

namespace Krista.FM.Domain
{
	public class B_Organizations_BridgePlan : ClassifierTable
	{
		public static readonly string Key = "12c09c82-0275-47ef-b36d-38865a305c01";

		public virtual int RowType { get; set; }
		public virtual int? SourceID { get; set; }
		public virtual int? Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string ShortName { get; set; }
		public virtual string City { get; set; }
		public virtual string Country { get; set; }
		public virtual string Phone { get; set; }
		public virtual string Address { get; set; }
		public virtual string INN20 { get; set; }
		public virtual int? SubINN { get; set; }
		public virtual string EMail { get; set; }
		public virtual string WebSite { get; set; }
		public virtual string DirName { get; set; }
		public virtual string BugName { get; set; }
		public virtual string DirPhone { get; set; }
		public virtual string BugPhone { get; set; }
		public virtual int? OKATOCode { get; set; }
		public virtual string OKATOName { get; set; }
		public virtual int? RegionClsCode { get; set; }
		public virtual string RegionClsName { get; set; }
		public virtual int? OrgClsCode { get; set; }
		public virtual string OrgClsName { get; set; }
		public virtual int? ParentID { get; set; }
	}
}
