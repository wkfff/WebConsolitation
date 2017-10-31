namespace Krista.FM.Domain
{
    public abstract class DebtorBookFactBase : FactTable
    {
        public virtual int SourceID { get; set; }
        public virtual int? ParentID { get; set; }
        public virtual int? SourceKey { get; set; }
        public virtual bool IsBlocked { get; set; }
        public virtual D_Variant_Schuldbuch RefVariant { get; set; }
        public virtual D_Regions_Analysis RefRegion { get; set; }
    }
}
