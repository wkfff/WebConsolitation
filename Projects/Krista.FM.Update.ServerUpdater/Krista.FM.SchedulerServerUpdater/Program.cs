using System;
using System.Windows.Forms;
using Krista.FM.Update.SchedulerServerUpdater;

namespace Krista.FM.SchedulerServerUpdater
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
            Application.Run(new ServerUpdaterForm());
        }
    }
}
