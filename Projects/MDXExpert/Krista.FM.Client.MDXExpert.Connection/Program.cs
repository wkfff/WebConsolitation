using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Common.RegistryUtils;
using Microsoft.Win32;

namespace Krista.FM.Client.MDXExpert.Connection
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Consts.connectionName = Settings.GetConnectionName();
            Application.Run(new ConnectionForm(Consts.connectionName, true));
        }
    }
}
