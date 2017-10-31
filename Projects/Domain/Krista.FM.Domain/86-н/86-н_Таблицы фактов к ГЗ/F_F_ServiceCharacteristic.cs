namespace Krista.FM.Domain
{
    public class F_F_ServiceCharacteristic : FactTable
    {
        public static readonly string Key = "8399b96c-289e-4413-92bd-9787b1b139e0";

        public virtual int SourceID { get; set; }

        public virtual int TaskID { get; set; }

        public virtual D_Services_ServiceRegister RefService { get; set; }

        public virtual D_Services_Indicators RefIndicators { get; set; }
    }
}
