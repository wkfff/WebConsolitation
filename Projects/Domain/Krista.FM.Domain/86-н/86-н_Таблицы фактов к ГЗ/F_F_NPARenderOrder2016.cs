using System;

namespace Krista.FM.Domain
{
	public class F_F_NPARenderOrder2016 : FactTable
	{
		public static readonly string Key = "fc1d5f3c-bbb0-4431-9ee2-0aa2ee208eff";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual string TypeNpa { get; set; }
		public virtual DateTime? DateNpa { get; set; }
		public virtual string NumberNpa { get; set; }
		public virtual string RenderEnact { get; set; }
		public virtual F_F_GosZadanie2016 RefFactGZ { get; set; }
        public virtual string Author { get; set; }
    }
}
