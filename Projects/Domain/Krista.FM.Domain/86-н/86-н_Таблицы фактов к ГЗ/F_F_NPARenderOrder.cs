using System;

namespace Krista.FM.Domain
{
	public class F_F_NPARenderOrder : FactTable
	{
		public static readonly string Key = "7515cb72-baa1-41ce-8c5d-d375a5cd1683";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual string RenderEnact { get; set; }
		public virtual F_F_GosZadanie RefFactGZ { get; set; }
		public virtual string TypeNpa { get; set; }
		public virtual DateTime? DateNpa { get; set; }
		public virtual string NumberNpa { get; set; }
	}
}
