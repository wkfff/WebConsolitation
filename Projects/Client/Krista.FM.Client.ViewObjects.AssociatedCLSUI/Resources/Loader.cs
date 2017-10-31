using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Common.Services;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.ResourcesLoader
{
    public class Loader
    {
		private static bool initialized = false;
        public static void Initialize()
        {
			if (!initialized)
			{
				ResourceService.RegisterImages("Krista.FM.Client.ViewObjects.AssociatedCLSUI.Resources", typeof (Loader).Assembly);
				initialized = true;
			}
        }
    }
}
