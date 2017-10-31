using Krista.FM.Domain.MappingAttributes;

namespace Krista.FM.Domain
{
    public class MessageAttachment : DomainObject
    {
        public virtual string DocumentName { get; set; }
        public virtual string DocumentFileName { get; set; }
        [BlobLenght(1073741824)]
        public virtual byte[] Document { get; set; }
    }
}
