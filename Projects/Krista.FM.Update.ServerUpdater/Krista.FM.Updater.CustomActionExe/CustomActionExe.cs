using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Forms;

namespace Krista.FM.Updater.CustomActionExe
{
    public class CustomActionExe
    {
        static void Main(string[] args)
        {
            string sid = args[0];
            string folder = args[1].Trim('\"');

            SetPermission(sid, folder);
        }

        public static bool SetPermission(string sid, string folder)
        {
            try
            {
                if (Directory.Exists(folder))
                {
                    DirectoryInfo dirInfo = Directory.CreateDirectory(folder);
                    DirectorySecurity dirSecurityOld = dirInfo.GetAccessControl();
                    AuthorizationRuleCollection cold = dirSecurityOld.GetAccessRules(true, false,
                                                                                     typeof (SecurityIdentifier));
                    DirectorySecurity dirSecurityNew = new DirectorySecurity();
                    foreach (AuthorizationRule rule in cold)
                    {
                        dirSecurityNew.AddAccessRule((FileSystemAccessRule) rule);
                    }

                    // Add access for everyone to the folder.
                    SecurityIdentifier sidID = new SecurityIdentifier(sid);

                    FileSystemAccessRule newAccessRule = new FileSystemAccessRule(sidID,
                                                                                  FileSystemRights.FullControl,
                                                                                  InheritanceFlags.ContainerInherit |
                                                                                  InheritanceFlags.ObjectInherit,
                                                                                  PropagationFlags.None,
                                                                                  AccessControlType.Allow);
                    dirSecurityNew.AddAccessRule(newAccessRule);
                    dirInfo.SetAccessControl(dirSecurityNew);
                }

                else
                    MessageBox.Show(String.Format("Назначение прав.Не найдена директория {0}", folder), "Ошибка");

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("Назначение прав. {0}. Свойства: {1}, {2}", e.Message, folder, sid), "Ошибка");
                return false;
            }
        }
    }
}
