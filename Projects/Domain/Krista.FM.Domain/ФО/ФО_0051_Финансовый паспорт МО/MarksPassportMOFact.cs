namespace Krista.FM.Domain
{
    public class MarksPassportMOFact : DomainObject
    {
        public virtual int ID { get; set; }

        public virtual string MarkName { get; set; }

        public virtual string MarkCode { get; set; }

        public virtual decimal? OrigPlan { get; set; }

        public virtual decimal? ScoreMO1 { get; set; }

        public virtual decimal? ScoreMO2 { get; set; }

        public virtual decimal? ScoreMO3 { get; set; }

        public virtual decimal? ScoreMO4 { get; set; }

        public virtual decimal? ScoreMO5 { get; set; }

        public virtual decimal? ScoreMO6 { get; set; }

        public virtual decimal? ScoreMO7 { get; set; }

        public virtual decimal? ScoreMO8 { get; set; }

        public virtual decimal? ScoreMO9 { get; set; }

        public virtual decimal? ScoreMO10 { get; set; }

        public virtual decimal? ScoreMO11 { get; set; }

        public virtual decimal? ScoreMO12 { get; set; }

        public virtual decimal? ScoreMOQuarter1 { get; set; }

        public virtual decimal? ScoreMOQuarter2 { get; set; }

        public virtual decimal? ScoreMOQuarter3 { get; set; }

        public virtual decimal? ScoreMOQuarter4 { get; set; }

        public virtual decimal? FactPeriod { get; set; }

        public virtual decimal? FactLastYear { get; set; }

        public virtual decimal? PlanPeriod { get; set; }

        public virtual decimal? RefinPlan { get; set; }

        public virtual int Level { get; set; }

        public virtual bool IsLeaf { get; set; }

        public virtual int State { get; set; }

        public virtual bool Editable3456 { get; set; }
    }
}