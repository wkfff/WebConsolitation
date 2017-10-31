using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Common.RegistryUtils;
using System.Windows.Forms;

namespace Krista.FM.Client.MDXExpert.Connection
{
    public static class Settings
    {
        public static string GetConnectionName()
        {
            string regValue = string.Empty;
            Utils regUtils = new Utils(typeof(Settings), true);

            regValue = regUtils.GetKeyValue(Consts.connectionNameKey);
            return regValue;
        }

        public static void SaveConnectionName()
        {
            Utils regUtils = new Utils(typeof(Settings), true);
            regUtils.SetKeyValue(Consts.connectionNameKey, Consts.connectionName);
        }

    }
}
