namespace Krista.FM.Domain
{
	public class D_ExcCosts_Document : ClassifierTable
	{
		public static readonly string Key = "520b6b73-dfbd-4439-86a0-ed6a1d45eb51";

		public virtual int RowType { get; set; }
        public virtual byte[] Doc { get; set; }
		public virtual string DocName { get; set; }
		public virtual int? DocType { get; set; }
		public virtual string DocFileName { get; set; }
		public virtual int Code { get; set; }
		public virtual D_ExcCosts_CObject RefDoc { get; set; }
	}
}
