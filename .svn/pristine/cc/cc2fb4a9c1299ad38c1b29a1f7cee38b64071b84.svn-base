using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    class SmoAttributeDocument : SmoAttribute
    {
        public SmoAttributeDocument(IDataAttribute serverObject)
            : this(serverObject, false)
        {
        }

        public SmoAttributeDocument(IDataAttribute serverObject, bool cached)
            : base(serverObject, cached)
        {
        }


        public SmoAttributeDocument(SMOSerializationInfo cache) 
            : base(cache)
        {
        }

        new public bool IsDocument
        {
            get { return serverControl is IDocumentAttribute; }
        }
    }
}
