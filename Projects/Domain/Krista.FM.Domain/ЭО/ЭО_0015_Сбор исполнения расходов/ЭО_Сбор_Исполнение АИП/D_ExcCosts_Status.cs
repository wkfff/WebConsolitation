namespace Krista.FM.Domain
{
	public class D_ExcCosts_Status : ClassifierTable
	{
		public static readonly string Key = "16eedb4f-cb57-4628-8739-e1ecb62200c0";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
	}
}
