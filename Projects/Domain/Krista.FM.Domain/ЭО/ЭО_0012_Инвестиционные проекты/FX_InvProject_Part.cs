using System;

namespace Krista.FM.Domain
{
	public class FX_InvProject_Part : ClassifierTable
	{
		public static readonly string Key = "1e65cde7-c80b-43c4-8364-6c36768a9a8a";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string FullName { get; set; }
	}
}
