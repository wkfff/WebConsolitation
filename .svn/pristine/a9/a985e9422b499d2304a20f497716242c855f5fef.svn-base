using System.ComponentModel;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SMO
{
    [ReadOnly(true)]
    public class SmoAttributeReadOnly : SmoAttribute
    {
        public SmoAttributeReadOnly(IDataAttribute serverObject)
            : this(serverObject, false)
        {
        }

        public SmoAttributeReadOnly(IDataAttribute serverObject, bool cached)
            : base(serverObject, cached)
        {
        }


        public SmoAttributeReadOnly(SMOSerializationInfo cache) 
            : base(cache)
        {
        }
    }
}
