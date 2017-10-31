using Microsoft.Win32;
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace CustomInstall
{
    [RunInstaller(true)]
    public class CustomInstallApp : Installer
    {
        private static ArrayList __ENCList = new ArrayList();
        private const string INI_FILE = @"\setup.ini";

        [DebuggerNonUserCode]
        public CustomInstallApp()
        {
            base.AfterUninstall += new InstallEventHandler(this.Installer_AfterUninstall);
            base.AfterInstall += new InstallEventHandler(this.Installer_AfterInstall);
            ArrayList list = __ENCList;
            lock (list)
            {
                __ENCList.Add(new WeakReference(this));
            }
        }

        public static string GetIniPath()
        {
            return ("\"" + Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Setup.ini") + "\"");
        }

        public static string GetWindowsCeApplicationManager()
        {
            string ceAppPath = KeyExists();
            if (ceAppPath == string.Empty)
            {
                MessageBox.Show("Windows CE App Manager not installed", "Setup", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return string.Empty;
            }
            return ceAppPath;
        }

        private void Installer_AfterInstall(object sender, InstallEventArgs e)
        {
            string ceAppPath = GetWindowsCeApplicationManager();
            if (ceAppPath != string.Empty)
            {
                string iniPath = GetIniPath();
                Process.Start(ceAppPath, iniPath);
            }
        }

        private void Installer_AfterUninstall(object sender, InstallEventArgs e)
        {
            string ceAppPath = GetWindowsCeApplicationManager();
            if (ceAppPath != string.Empty)
            {
                string iniPath = GetIniPath();
                Process.Start(ceAppPath, string.Empty);
            }
        }

        private static string KeyExists()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\CEAPPMGR.EXE");
            if (key == null)
            {
                return string.Empty;
            }
            return key.GetValue(string.Empty, string.Empty).ToString();
        }
    }
}