using System;

namespace Krista.FM.Domain
{
	public class FX_FX_Subject : ClassifierTable
	{
		public static readonly string Key = "3e7f2524-886a-41c5-8d95-423148ba8143";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
	}
}
