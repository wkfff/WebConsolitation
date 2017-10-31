using System;

namespace Krista.FM.Domain
{
	public class FX_S_StatusSchb : ClassifierTable
	{
		public static readonly string Key = "d705ee06-5244-47da-8ea1-6667df0e97c6";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
	}
}
