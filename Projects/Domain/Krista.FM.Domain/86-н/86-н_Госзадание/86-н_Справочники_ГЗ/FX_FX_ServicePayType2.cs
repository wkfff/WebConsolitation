using System;

namespace Krista.FM.Domain
{
	public class FX_FX_ServicePayType2 : ClassifierTable
	{
		public static readonly string Key = "2fc68cbc-cdba-40c9-b8aa-a2af58669eb6";

	    public static readonly string CodeOfPayable = "1";
	    public static readonly int IdOfPayable = 1;
                               
	    public static readonly string CodeOfFree = "2";
	    public static readonly int IdOfFree = 2;

		public virtual int RowType { get; set; }
		public virtual string Code { get; set; }
		public virtual string Name { get; set; }
	}
}
