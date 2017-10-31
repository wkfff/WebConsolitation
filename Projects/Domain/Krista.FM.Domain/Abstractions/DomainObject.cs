using System.Runtime.Serialization;

namespace Krista.FM.Domain
{
    [DataContract]
    public abstract class DomainObject
    {
        protected DomainObject()
        {
            isNew = true;
        }

        [IgnoreDataMember]
        public virtual int ID { get; set; }

        private bool isNew;
        public virtual bool IsNew()
        {
            return isNew;
        }
    }
}
