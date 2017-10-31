using System.Collections.Generic;
using Krista.FM.Domain.MappingAttributes;

namespace Krista.FM.Domain
{
    public class D_CD_Report : D_CD_ReportBase
    {
        public static readonly string Key = "7934160c-154d-424d-9305-f0689fa0f2e9";

        private IList<D_Report_Section> sections;

        public D_CD_Report()
        {
            sections = new List<D_Report_Section>();
        }

        public virtual int RowType { get; set; }
        public virtual D_CD_Task RefTask { get; set; }
        public virtual D_CD_Templates RefForm { get; set; }

        [ReferenceField("RefReport")]
        public virtual IList<D_Report_Section> Sections
        {
            get { return sections; }
            set { sections = value; }
        }
    }
}
