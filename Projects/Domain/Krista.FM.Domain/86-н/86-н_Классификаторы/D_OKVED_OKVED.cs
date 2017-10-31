using System;

namespace Krista.FM.Domain
{
    public class D_OKVED_OKVED : ClassifierTable
    {
        public static readonly string Key = "e70474e6-ffdd-44c4-b8af-4cb45a7fb6a5";

        public virtual int RowType { get; set; }

        public virtual string Name { get; set; }

        public virtual string Section { get; set; }

        public virtual string SubSection { get; set; }

        public virtual string Code { get; set; }

        public virtual int? ParentID { get; set; }

        public virtual DateTime? OpenDate { get; set; }

        public virtual DateTime? CloseDate { get; set; }
    }
}