using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SmoBridgeClassifier : SmoClassifier, IBridgeClassifier
    {
        public SmoBridgeClassifier(IClassifier serverObject) : base(serverObject)
        {
        }


        public SmoBridgeClassifier(SMOSerializationInfo cache) 
            : base(cache)
        {
        }
    }
}
