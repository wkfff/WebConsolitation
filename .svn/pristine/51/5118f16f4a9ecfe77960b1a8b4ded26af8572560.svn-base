using System;

namespace Krista.FM.Update.PatchMakerLibrary.SubPrograms
{
    public sealed class ClientSchemeDesignerSubProgram : ClientSubProgram
    {
        public ClientSchemeDesignerSubProgram(string path) : base(path)
        {
        }

        protected override string GetBaseUrl(string patchName, string installerVersion)
        {
            return String.Format(@"Client\SchemeDesigner\{0}\", patchName);
        }

        public override string GetFeedName()
        {
            return "Client.SchemeDesigner.Feed.xml";
        }
    }
}
