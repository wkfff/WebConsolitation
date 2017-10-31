namespace Krista.FM.Domain
{
    public class DataSources : DomainObject
    {
        public virtual string SupplierCode { get; set; }
        public virtual int DataCode { get; set; }
        public virtual string DataName { get; set; }
        public virtual string KindsOfParams { get; set; }
        public virtual string Name { get; set; }
        public virtual string Year { get; set; }
        public virtual string Month { get; set; }
        public virtual string Variant { get; set; }
        public virtual string Quarter { get; set; }
        public virtual string Territory { get; set; }
        public virtual string Locked { get; set; }
        public virtual string Deleted { get; set; }
        public virtual string DataSourceName { get; set; }
    }
}
