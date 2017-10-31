using System;

namespace Krista.FM.Domain
{
	public class D_Services_TipY : ClassifierTable
	{
		public static readonly string Key = "0cb0dd1b-f7c8-4033-945f-e63aad874b1b";

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }

        public const int FX_FX_SERVICE = 1;
        public const int FX_FX_WORK = 2;
	}
}
