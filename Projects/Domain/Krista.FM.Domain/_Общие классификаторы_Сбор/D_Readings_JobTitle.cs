using System;

namespace Krista.FM.Domain
{
	public class D_Readings_JobTitle : ClassifierTable
	{
		public static readonly string Key = "cf60e44c-bc67-4162-b12a-b4615002bd18";

		public virtual int RowType { get; set; }
        public virtual string Title { get; set; }
		public virtual string Executive { get; set; }
		public virtual string Telephone { get; set; }
		public virtual D_Regions_Analysis RefRegions { get; set; }
		public virtual D_Variant_DataCapture RefDataCapture { get; set; }
	}
}
