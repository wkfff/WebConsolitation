namespace Krista.FM.Domain
{
	public class D_OKOPF_OKOPF : ClassifierTable
	{
		public static readonly string Key = "8e8cb401-7aca-4375-ac02-b30bef9a7630";

        /// <summary>
        /// Бизнес статус 801
        /// </summary>
        public static readonly string Included = "801";

        /// <summary>
        /// Бизнес статус 865
        /// </summary>
        public static readonly string Excluded = "865";

		public virtual int RowType { get; set; }
		public virtual int? Code { get; set; }
        public virtual string Code5Zn { get; set; }
		public virtual string Name { get; set; } 
        public virtual string businessStatus { get; set; }
	}
}
