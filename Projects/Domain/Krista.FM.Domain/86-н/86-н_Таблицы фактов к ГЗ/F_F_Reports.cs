using System;

namespace Krista.FM.Domain
{
    public class F_F_Reports : FactTable
    {
        public static readonly string Key = "035cb7b8-f084-4b51-adaa-7b5804e5c0d2";

        public virtual int SourceID { get; set; }
        public virtual int TaskID { get; set; }
        public virtual string ReportGuid { get; set; }
        public virtual string NameReport { get; set; }
        public virtual string HeadName { get; set; }
        public virtual string HeadPosition { get; set; }
        public virtual DateTime DateReport { get; set; }
        public virtual F_F_ParameterDoc RefFactGZ { get; set; }
    }
}
