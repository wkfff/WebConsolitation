using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Common;
using Krista.FM.Common.Services;
using System.Reflection;
using System.IO;
using Krista.FM.Client.Common.Resources;

namespace Krista.FM.Client.MDXExpert
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //если в udl указан юзер, отличный от текущего, запустим под юзером, кот. указан в udl
            //if (RunApplicationForAnotherUser())
            //    return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //ƒл€ всеобщего доступа к ресурсам
            ResourceService.InitializeService(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            Loader.Initialize();

            string fileName = "";
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                fileName = Environment.GetCommandLineArgs()[1];
            }

            try
            {
                Application.Run(new MainForm(fileName));
            }
            catch (Exception e)
            {
                FormException.ShowErrorForm(e);
            }
        }


        /// <summary>
        /// «апустить приложение под другим пользователем (если он указан в udl)
        /// </summary>
        /// <returns>true - если запускаем под другим юзером</returns>
        private static bool RunApplicationForAnotherUser()
        {
            FM.Common.ConnectionString cs = new ConnectionString();

            cs.ReadConnectionString(Application.StartupPath + @"\MAS.udl");
            if (cs.UserID == String.Empty)
                return false;

            string[] userID = cs.UserID.Split('\\');
            string userName = (userID.Length > 1) ? userID[1] : userID[0];
            string domain = (userID.Length > 1)? userID[0] : String.Empty;

            string pass = cs.Password;
            MessageBox.Show(Environment.UserName);

            if (Environment.UserName != userName)
            {
                if (ValidateLogin(userName, pass, domain))
                {
                    SecureString secureStr = new SecureString();
                    for (int i = 0; i < pass.Length; i++)
                    {
                        secureStr.AppendChar(pass[i]);
                    }

                    string fileName = "";
                    if (Environment.GetCommandLineArgs().Length > 1)
                    {
                        fileName = Environment.GetCommandLineArgs()[1];
                    }
                    try
                    {
                        Process.Start(Application.ExecutablePath, fileName, userName, secureStr, domain);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message + "\r\n" + e.InnerException);
                        return false;
                    }
                    return true;
                }
                else
                {
                    MessageBox.Show(String.Format("ѕользователь {0} не найден", userName));
                    return false;
                }
            }
            return false;
        }

        // Declare the logon types as constants
        const long LOGON32_LOGON_INTERACTIVE = 2;
        const long LOGON32_LOGON_NETWORK = 3;

        // Declare the logon providers as constants
        const long LOGON32_PROVIDER_DEFAULT = 0;
        const long LOGON32_PROVIDER_WINNT50 = 3;
        const long LOGON32_PROVIDER_WINNT40 = 2;
        const long LOGON32_PROVIDER_WINNT35 = 1;

        [DllImport("advapi32.dll", EntryPoint = "LogonUser")]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword,
                                                int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        public static bool ValidateLogin(string username, string password, string domain)
        {
            IntPtr token = new IntPtr(0);
            token = IntPtr.Zero;
            return LogonUser(username, domain, password, (int)LOGON32_LOGON_NETWORK, (int)LOGON32_PROVIDER_DEFAULT,
                             ref token);
        }
    }
}