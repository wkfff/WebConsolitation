using System;

namespace Krista.FM.Domain
{
	public class FX_FX_LevelPPO : ClassifierTable
	{
		public static readonly string Key = "b26f3d0a-fb59-4a4d-9704-23b5f01f0137";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
	}
}
