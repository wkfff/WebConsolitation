using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Common.Services;

namespace Krista.FM.Client.Common.Resources
{
    public class Loader
    {
        public static void Initialize()
        {
            ResourceService.RegisterImages("Krista.FM.Client.Common.Resources", typeof(Loader).Assembly);
        }
    }
}
