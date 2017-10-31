using System;

namespace Krista.FM.Domain
{
	public class D_Fin_VidRash : ClassifierTable
	{
		public static readonly string Key = "9a4cf14a-0219-4256-af4c-99fdb26316c5";

		public virtual int RowType { get; set; }
		public virtual string Code { get; set; }
		public virtual string Name { get; set; }
        public virtual DateTime? EffectiveFrom { get; set; }
        public virtual DateTime? EffectiveBefore { get; set; }
	}
}
