using System;

namespace Krista.FM.Domain
{
	public class D_Services_ServiceRegister : ClassifierTable
	{
		public static readonly string Key = "bf039d90-2b41-460f-86f5-dd62d5355f30";

		public virtual int RowType { get; set; }
		public virtual string Code { get; set; }
		public virtual string UniqalCode { get; set; }
		public virtual string Name { get; set; }
		public virtual DateTime OpenData { get; set; }
		public virtual DateTime? CloseData { get; set; }
		public virtual int? ParentID { get; set; }
		public virtual D_OKVED_OKVED RefOKVED { get; set; }
		public virtual FX_FX_Subject RefSubject { get; set; }
		public virtual FX_FX_LevelPPO RefPPO { get; set; }
		public virtual FX_FX_ServiceType RefType { get; set; }
		public virtual FX_FX_ServicePayType RefPay { get; set; }
	}
}
