using System;

namespace Krista.FM.Domain
{
	public class FX_FX_ServiceType : ClassifierTable
	{
		public static readonly string Key = "becca482-01f2-4db2-aa5c-94dda75afd98";

	    public static readonly string CodeOfService = "0";
        public static readonly int IdOfService = 1;
                               
        public static readonly string CodeOfWork = "1";
	    public static readonly int IdOfWork = 2;

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
		public virtual string Code { get; set; }
	}
}
