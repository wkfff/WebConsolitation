using System;

namespace Krista.FM.Domain
{
    public class F_Fact_InspectionEvent : FactTable
    {
        public static readonly string Key = "2d798863-ae7e-42f0-bc14-a1de5d2a95c5";

        public virtual int SourceID { get; set; }

        public virtual int TaskID { get; set; }

        public virtual string Topic { get; set; }

        public virtual DateTime EventBegin { get; set; }

        public virtual DateTime EventEnd { get; set; }

        public virtual string Violation { get; set; }

        public virtual string ResultActivity { get; set; }

        public virtual string Supervisor { get; set; }

        public virtual F_F_ParameterDoc RefParametr { get; set; }
    }
}
