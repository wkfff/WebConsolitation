using System;

namespace Krista.FM.Domain
{
    public class DatabaseVersions : DomainObject
    {
        public virtual string Name { get; set; }
        public virtual DateTime Released { get; set; }
        public virtual DateTime Updated { get; set; }
        public virtual string Comments { get; set; }
        public virtual bool NeedUpdate { get; set; }
    }
}
