using System;

namespace Krista.FM.Domain
{
	public class D_Report_Row : ClassifierTable
	{
		public static readonly string Key = "04847b4c-6c73-4948-9830-7b1ebad7a5cb";

		public virtual int RowType { get; set; }
		public virtual D_Report_Section RefSection { get; set; }
		public virtual D_Form_TableRow RefFormRow { get; set; }
	}
}
