using System;

namespace Krista.FM.Domain
{
	public class D_ExcCosts_Events : ClassifierTable
	{
		public static readonly string Key = "98e43b5b-be62-45a1-9772-7a65d6bb7325";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string Results { get; set; }
		public virtual string Note { get; set; }
		public virtual string ResultEv { get; set; }
		public virtual string ReasonFail { get; set; }
		public virtual D_ExcCosts_Tasks RefTask { get; set; }
		public virtual D_ExcCosts_Creators RefCreators { get; set; }
	}
}
