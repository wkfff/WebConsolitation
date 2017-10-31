using System;

namespace Krista.FM.Domain
{
	public class F_F_VedPerProvider : FactTable
	{
		public static readonly string Key = "e9a6422d-1aae-4e2f-bbdc-ba8db8a37a65";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual D_Services_VedPer RefService { get; set; }
		public virtual D_Org_Structure RefProvider { get; set; }
	}
}
