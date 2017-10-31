using System;

namespace Krista.FM.Domain
{
	public class F_F_PotrYs : FactTable
	{
		public static readonly string Key = "5db43b31-6a9d-461c-840c-69434401084a";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual D_Services_VedPer RefVedPP { get; set; }
		public virtual D_Services_CPotr RefCPotr { get; set; }
	}
}
