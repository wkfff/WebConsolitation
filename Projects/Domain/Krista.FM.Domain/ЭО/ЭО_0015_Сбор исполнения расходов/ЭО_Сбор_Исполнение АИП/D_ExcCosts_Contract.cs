namespace Krista.FM.Domain
{
	public class D_ExcCosts_Contract : ClassifierTable
	{
		public static readonly string Key = "dbeea3ce-141f-428d-bcee-6dc3ae8300e4";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Property { get; set; }
		public virtual D_ExcCosts_Partners RefPartners { get; set; }
		public virtual D_ExcCosts_TypeCont RefTypeCont { get; set; }
	}
}
