namespace Krista.FM.Domain
{
    public class F_F_PNRZnach : FactTable
    {
        public static readonly string Key = "2f578037-98dd-4da3-91dd-8b7a3093cfdc";

        public virtual int SourceID { get; set; }

        public virtual int TaskID { get; set; }

        public virtual string CurrentYear { get; set; }

        public virtual string ReportingYear { get; set; }

        public virtual string Protklp { get; set; }

        public virtual string ComingYear { get; set; }

        public virtual string FirstPlanYear { get; set; }

        public virtual string SecondPlanYear { get; set; }

        public virtual string ActualValue { get; set; }

        public virtual string Info { get; set; }

        public virtual string Source { get; set; }

        public virtual string SourceInfFact { get; set; }

        public virtual F_F_GosZadanie RefFactGZ { get; set; }

        public virtual D_Services_Indicators RefIndicators { get; set; }
    }
}
