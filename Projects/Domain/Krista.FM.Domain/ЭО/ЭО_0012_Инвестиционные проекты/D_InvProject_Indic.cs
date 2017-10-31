using System;

namespace Krista.FM.Domain
{
	public class D_InvProject_Indic : ClassifierTable
	{
		public static readonly string Key = "2c4e0edc-324d-469e-a005-1bf9ced15e29";

		public virtual int RowType { get; set; }
		public virtual int SourceID { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
		public virtual string Note { get; set; }
		public virtual string ShortName { get; set; }
		public virtual FX_InvProject_TypeI RefTypeI { get; set; }
		public virtual D_Units_OKEI RefOKEI { get; set; }
	}
}
