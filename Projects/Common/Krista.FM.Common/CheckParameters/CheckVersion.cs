using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Common
{
    public class CheckVersion : CheckRule
    {
        public CheckVersion(string parametr, string value, string errorMessage, bool invalid)
            : base(parametr, value, errorMessage, invalid)
        { }

        public override bool Execute(string value)
        {
            if (AppVersionControl.GetAssemblyBaseVersion(AppVersionControl.GetServerLibraryVersion()) != AppVersionControl.GetAssemblyBaseVersion(value))
                return false;

            return true;
        }
    }
}
