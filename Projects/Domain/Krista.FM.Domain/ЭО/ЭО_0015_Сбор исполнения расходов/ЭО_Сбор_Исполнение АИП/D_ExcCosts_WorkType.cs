namespace Krista.FM.Domain
{
	public class D_ExcCosts_WorkType : ClassifierTable
	{
		public static readonly string Key = "247aea04-5069-44b7-8270-8512ce18ae02";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }
	}
}
