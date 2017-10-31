namespace Krista.FM.Domain
{
    public class F_F_AveragePrice : FactTable
    {
        public static readonly string Key = "30ec8924-4e8c-4486-b59d-a4be820f06ea";

        public virtual int SourceID { get; set; }

        public virtual int TaskID { get; set; }
        
        public virtual F_F_PNRZnach2016 RefVolumeIndex { get; set; }
        
        public virtual decimal? ReportYearDec { get; set; }

        public virtual decimal? CurrentYearDec { get; set; }

        public virtual decimal? NextYearDec { get; set; }

        public virtual decimal? PlanFirstYearDec { get; set; }

        public virtual decimal? PlanLastYearDec { get; set; }
    }
}
