using System;

namespace Krista.FM.Domain
{
	public class D_Variant_Schuldbuch : ClassifierTable
	{
		public static readonly string Key = "f37827df-c22a-4569-9512-c0c48791d46c";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual bool CurrentVariant { get; set; }
		public virtual int ActualYear { get; set; }
		public virtual DateTime ReportDate { get; set; }
		public virtual int VariantCompleted { get; set; }
		public virtual string VariantComment { get; set; }
	}
}
