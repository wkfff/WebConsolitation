using System;

namespace Krista.FM.Update.PatchMakerLibrary.SubPrograms
{
    public sealed class ClientCommonSubProgram : ClientSubProgram
    {
        public ClientCommonSubProgram(string path) 
            : base(path)
        {
        }

        protected override string GetBaseUrl(string patchName, string installerVersion)
        {
            return String.Format(@"Client\Common\{0}\", patchName);
        }

        public override string GetFeedName()
        {
            return "Client.Common.Feed.xml";
        }
    }
}
