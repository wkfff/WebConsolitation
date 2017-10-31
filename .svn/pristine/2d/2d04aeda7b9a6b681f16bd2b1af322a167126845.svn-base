namespace Krista.FM.Domain
{
    public class HashObjectsNames : DomainObject
    {
        public virtual string HashName { get; set; }
        public virtual string LongName { get; set; }
        public virtual int ObjectType { get; set; }
        
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var t = obj as HashObjectsNames;
            if (t == null)
            {
                return false;
            }

            if (HashName == t.HashName && LongName == t.LongName)
            {
                return true;
            }
            
            return false;
        }
        public override int GetHashCode()
        {
            return (HashName + "|" + LongName).GetHashCode();
        }  
    }
}
