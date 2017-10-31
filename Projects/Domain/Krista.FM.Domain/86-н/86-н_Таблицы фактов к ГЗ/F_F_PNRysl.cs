namespace Krista.FM.Domain
{
    public class F_F_PNRysl : FactTable
    {
        public static readonly string Key = "a111b160-0e5b-4d06-a20d-d97d2091eb3c";

        public virtual int SourceID { get; set; }
        
        public virtual int TaskID { get; set; }

        public virtual D_Services_VedPer RefPerV { get; set; }

        public virtual D_Services_Indicators RefIndicators { get; set; }
    }
}
