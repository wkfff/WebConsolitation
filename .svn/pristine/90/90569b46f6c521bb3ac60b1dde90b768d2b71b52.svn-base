using System.Collections.Generic;
using Krista.FM.Domain.MappingAttributes;

namespace Krista.FM.Domain
{
	public class D_Report_Section : ClassifierTable
	{
		public static readonly string Key = "beed86f8-a5d8-4183-b27f-9bf9f0ff031f";

        private IList<D_Report_Row> rows;

        public D_Report_Section()
        {
            rows = new List<D_Report_Row>();
        }

		public virtual int RowType { get; set; }
		public virtual D_CD_Report RefReport { get; set; }
		public virtual D_Form_Part RefFormSection { get; set; }

	    [ReferenceField("RefSection")]
        public virtual IList<D_Report_Row> Rows
	    {
	        get { return rows; }
	        set { rows = value; }
	    }
	}
}
