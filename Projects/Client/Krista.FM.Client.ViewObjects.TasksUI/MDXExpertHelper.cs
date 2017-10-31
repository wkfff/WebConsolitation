using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;


namespace Krista.FM.Client.ViewObjects.TasksUI
{
    public sealed class MDXExpertHelper
    {
        private static string MDX_EXPERT_REG_PATH = @"SOFTWARE\krista\FM\OLAPClient";

        private static string GetMdxExpertPath()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(MDX_EXPERT_REG_PATH, false);
            if (key == null) return String.Empty;
            string appPath = (string)key.GetValue("ApplicationExeName");
            key.Close();
            return appPath;
        }

        private static void OverrideConnectionInfo(string serverName, string catalogName)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(MDX_EXPERT_REG_PATH, true);
            if (key == null) return;
            try
            {
                key.SetValue("Server", serverName, RegistryValueKind.String);
                key.SetValue("Catalog", catalogName, RegistryValueKind.String);
            }
            finally
            {
                key.Close();
            }
        }

        public static bool MdxExpertInstalled()
        {
            return File.Exists(GetMdxExpertPath());
        }

        private static bool RunMdxExpert(string serverName, string catalogName, params string[] runParameters)
        {
            OverrideConnectionInfo(serverName, catalogName);
            string mdxExpertFileName = GetMdxExpertPath();
            StringBuilder sb = new StringBuilder();
            foreach(string prm in runParameters)
                sb.Append(prm);
            string prms = sb.ToString();
            ProcessStartInfo psi = new ProcessStartInfo(mdxExpertFileName, prms);
            psi.WorkingDirectory = Path.GetDirectoryName(mdxExpertFileName);
            psi.UseShellExecute = false;
            Process.Start(psi);
            return true;
        }

        public static bool OpenDocument(string filePath, string serverName, string catalogName)
        {
            if (!MdxExpertInstalled())
                return false;
            return RunMdxExpert(serverName, catalogName, filePath);
        }

        public static bool CreateDocument(string filePath, string serverName, string catalogName)
        {
            if (!MdxExpertInstalled())
                return false;
            FileStream fs = File.Open(filePath, FileMode.OpenOrCreate);
            fs.Close();
            return RunMdxExpert(serverName, catalogName, filePath);

        }

    }
}