using System;

namespace Krista.FM.Domain
{
	public class F_F_NPACena : FactTable
	{
		public static readonly string Key = "ae6e30f6-18f5-4ec4-a76a-0e87e6f60a4c";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual F_F_GosZadanie RefGZPr { get; set; }
		public virtual string Name { get; set; }
		public virtual DateTime? DataNPAGZ { get; set; }
		public virtual string NumNPA { get; set; }
		public virtual string OrgUtvDoc { get; set; }
		public virtual string VidNPAGZ { get; set; }
	}
}
