using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Krista.FM.Client.ProtocolsViewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ProtocolsViewForm());
        }
    }
}