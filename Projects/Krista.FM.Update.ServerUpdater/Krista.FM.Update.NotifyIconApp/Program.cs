using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Krista.FM.Update.Framework;

namespace Krista.FM.Update.NotifyIconApp
{
    static class Program
    {
        private static IUpdateManager Connect()
        {
            IUpdateManager manager = null;
            try
            {
                manager = UpdateManagerFactory.CreateUpdateManager(true);
            }
            catch (FrameworkRemotingException e)
            {
                using (var writer =
                    new StreamWriter(string.Format("{0}\\CrashNotify.txt",
                                                   Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName))))
                {
                    writer.Write(e.InnerException);
                }
            }
            return manager;
        }

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            IUpdateManager manager = Connect();
            NotifyIconForm form = new NotifyIconForm(manager, true) {Visible = false};
            form.Activate();
            Application.Run();
        }
    }
}
