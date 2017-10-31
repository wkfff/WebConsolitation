using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

using Krista.FM.Client.OLAPAdmin.Properties;

namespace Krista.FM.Client.OLAPAdmin
{
	class SaveLoadSettings
	{
		//private static void CheckKey(ref RegistryKey regKey, string keyName, bool createIfNotExist)
		//{
		//    string fullKeyName = Resources.currentUserRegistryPath + keyName;
		//    regKey = Registry.CurrentUser.OpenSubKey(fullKeyName, true);
		//    if ((regKey == null) & createIfNotExist)
		//        regKey = Registry.CurrentUser.CreateSubKey(fullKeyName);
		//}

		//public static string CorrectName(string serverName, bool reverseOnSlash)
		//{
		//    char reverseSlash = '\u005C';
		//    char slash = '\u002F';
		//    if (reverseOnSlash)
		//        return serverName.Replace(reverseSlash, slash);
		//    else
		//        return serverName.Replace(slash, reverseSlash);
		//}

		//public static void SaveServer(string serverName)
		//{			
		//    RegistryKey regKey = Registry.CurrentUser;
		//    regKey = regKey.CreateSubKey(Resources.currentUserRegistryPath +
		//        Resources.regPathServers + "\\" + CorrectName(serverName, true));
		//    regKey.Close();
		//}

		public static void SaveServer(string serverName)
		{
			if (!Settings.Default.Servers.Contains(serverName))
			{
				Settings.Default.Servers.Add(serverName);
			}
		}
		
		//public static void DeleteServer(string serverName)
		//{
		//    RegistryKey regKey = Registry.CurrentUser;
		//    regKey.DeleteSubKey(Resources.currentUserRegistryPath + 
		//        Resources.regPathServers + "\\" + CorrectName(serverName, true), false);
		//    regKey.Close();
		//}

		public static void DeleteServer(string serverName)
		{
			Settings.Default.Servers.Remove(serverName);			
		}

		//public static void ReadServers(out string[] servers)
		//{	
		//    RegistryKey regKey = Registry.CurrentUser;			
		//    regKey =
		//        regKey.CreateSubKey(Resources.currentUserRegistryPath + Resources.regPathServers);
		//    servers = regKey.GetSubKeyNames();
		//    for (int i = 0; i < servers.Length; i++)
		//    {
		//        servers[i] = CorrectName(servers[i], false);
		//    }
		//}

		public static string[] ReadServers()
		{
			string[] servers = new string[Settings.Default.Servers.Count];
			Settings.Default.Servers.CopyTo(servers, 0);
			return servers;
		}
	}
}