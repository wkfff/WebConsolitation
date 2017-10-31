using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Install;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.DirectoryServices;

namespace InstallCustomActions
{
    [RunInstaller(true)]
    public class ASPValidateCustomAction : Installer
    {
        public override void Install(System.Collections.IDictionary stateSaver)
        {                    
            base.Install(stateSaver);
            

            // Retrieve configuration settings
            string targetSite = Context.Parameters["targetsite"];
            string targetVDir = Context.Parameters["targetvdir"];
            string targetDirectory = Context.Parameters["targetdir"];

            if (targetSite == null)
                throw new InstallException("Не указано имя сайта!");

            if (targetSite.StartsWith("/LM/"))
                targetSite = targetSite.Substring(4);

            RegisterScriptMaps(targetSite, targetVDir);
        }
                        

        void RegisterScriptMaps(string targetSite, string targetVDir)
        {
            // Calculate Windows path
            string sysRoot = System.Environment.GetEnvironmentVariable("SystemRoot");

            // Launch aspnet_regiis.exe utility to configure mappings
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = Path.Combine(sysRoot, @"Microsoft.NET\Framework\v2.0.50727\aspnet_regiis.exe");
            info.Arguments = string.Format("-s {0}/ROOT/{1}", targetSite, targetVDir);
            info.CreateNoWindow = true;
            info.UseShellExecute = false;

            Process.Start(info);
        }
    }
} 
