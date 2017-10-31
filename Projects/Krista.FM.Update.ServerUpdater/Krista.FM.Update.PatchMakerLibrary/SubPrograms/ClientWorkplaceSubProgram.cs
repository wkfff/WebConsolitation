using System;

namespace Krista.FM.Update.PatchMakerLibrary.SubPrograms
{
    public sealed class ClientWorkplaceSubProgram : ClientSubProgram
    {
        public ClientWorkplaceSubProgram(string path) : base(path)
        {
        }

        protected override string GetBaseUrl(string patchName, string installerVersion)
        {
            return String.Format(@"Client\Workplace\{0}\", patchName);
        }

        public override string GetFeedName()
        {
            return "Client.Workplace.Feed.xml";
        }
    }
}
