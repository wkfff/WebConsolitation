using System;

namespace Krista.FM.Update.PatchMakerLibrary.SubPrograms
{
    public sealed class ClientOlapAdminSubProgram : ClientSubProgram
    {
        public ClientOlapAdminSubProgram(string path) : base(path)
        {
        }

        protected override string GetBaseUrl(string patchName, string installerVersion)
        {
            return String.Format(@"Client\OlapAdmin\{0}\", patchName);
        }

        public override string GetFeedName()
        {
            return "Client.OLAPAdmin.Feed.xml";
        }

        public override System.Collections.Generic.List<Type> SubProgramDependentTypes
        {
            get
            {
                return new System.Collections.Generic.List<Type>();
            }
        }
    }
}
