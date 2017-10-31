using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Deployment.WindowsInstaller;

namespace Krista.FM.Update.CustomAction
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult RunBatAsAdmin(Session session)
        {
            try
            {
                ProcessStartInfo procInfo;
                if (Convert.ToInt32(session["VersionNT"]) >= 600)
                {
                    procInfo = new ProcessStartInfo
                                   {
                                       UseShellExecute = true,
                                       FileName = "bat.cmd",
                                       WorkingDirectory =
                                           session["INSTALLDIR"],
                                       Verb = "runas",
                                       WindowStyle = ProcessWindowStyle.Hidden
                                   };
                }
                else
                {
                    procInfo = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        FileName = "bat.cmd",
                        WorkingDirectory =
                            session["INSTALLDIR"],
                        WindowStyle = ProcessWindowStyle.Hidden
                    };
                }

                Process process = Process.Start(procInfo);  //Start that process.
                process.WaitForExit();

                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.Log(string.Format("Ошибка в Custom Action: {0}", ex.Message));
                return ActionResult.Failure;
            }
        }

        [CustomAction]
        public static ActionResult RunRegisterSvrLibAsAdmin(Session session)
        {
            try
            {
                var procInfo = new ProcessStartInfo
                                   {
                                       UseShellExecute = true,
                                       FileName = "RegisterSvrLib.exe",
                                       WorkingDirectory =
                                           session["INSTALLDIR"],
                                       Verb = "runas",
                                       WindowStyle = ProcessWindowStyle.Hidden
                                   };
                Process process = Process.Start(procInfo); //Start that process.
                process.WaitForExit();

                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.Log(string.Format("Ошибка в Custom Action: {0}", ex.Message));
                return ActionResult.Failure;
            }
        }

        public static bool SetPermission(string sid, string folder)
        {
            try
            {
                if (Directory.Exists(folder))
                {
                    var sidID = new SecurityIdentifier(sid);


                    DirectorySecurity ds = Directory.GetAccessControl(folder);
                    ds.AddAccessRule(new FileSystemAccessRule(sidID,
                                                              FileSystemRights.FullControl,
                             InheritanceFlags.ObjectInherit |
                             InheritanceFlags.ContainerInherit,
                             PropagationFlags.None,
                             AccessControlType.Allow));
                    // set folder with correct user rights
                    DirectoryInfo di = new DirectoryInfo(folder);

                    di.SetAccessControl(ds);
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        [CustomAction]
        public static ActionResult SetFolderPermissionSD(Session session)
        {
            if (!Directory.Exists(session["INSTALLLOCATIONSD"]))
                return ActionResult.Success;

            try
            {
                var procInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = "Krista.FM.Updater.CustomActionExe.exe",
                    WorkingDirectory =
                        session["INSTALLLOCATIONSD"],
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = String.Format("{0} \"{1}\"", new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null).Value, session["INSTALLLOCATIONSD"])

                };
                Process process = Process.Start(procInfo);  //Start that process.
                process.WaitForExit();
                var procInfo1 = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = "Krista.FM.Updater.CustomActionExe.exe",
                    WorkingDirectory =
                        session["INSTALLLOCATIONSD"],
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = String.Format("{0} \"{1}\"", new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).Value, session["INSTALLLOCATIONSD"])

                };
                Process process1 = Process.Start(procInfo1);  //Start that process.
                process1.WaitForExit();
            }
            catch (Exception ex)
            {
                session.Log(string.Format("Ошибка в Custom Action: {0}", ex.Message));
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RemoveInstallDirFolder(Session session)
        {
            if (Directory.Exists(session["INSTALLDIR"]))
            {
                try
                {
                    session.Log(Path.Combine(session["SystemFolder"], "cmd.exe"));

                    ProcessStartInfo procInfo;
                    if (session["ALLUSERS"] == "1")
                    {
                        procInfo = new ProcessStartInfo
                                           {
                                               UseShellExecute = true,
                                               FileName = Path.Combine(session["SystemFolder"], "cmd.exe"),
                                               WorkingDirectory =
                                                   session["INSTALLDIR"],
                                               Verb = "runas",
                                               WindowStyle = ProcessWindowStyle.Hidden,
                                               Arguments =
                                                   String.Format("cmd /c rmdir /S /Q \"{0}\"", session["INSTALLDIR"])

                                           };
                    }
                    else
                    {
                        procInfo = new ProcessStartInfo
                                            {
                                                UseShellExecute = true,
                                                FileName = Path.Combine(session["SystemFolder"], "cmd.exe"),
                                                WorkingDirectory =
                                                    session["INSTALLDIR"],
                                                WindowStyle = ProcessWindowStyle.Hidden,
                                                Arguments =
                                                    String.Format("cmd /c rmdir /S /Q \"{0}\"", session["INSTALLDIR"])

                                            };
                    }

                    Process process = Process.Start(procInfo); //Start that process.
                    process.WaitForExit();
                }
                catch (Exception ex)
                {
                    session.Log(string.Format("Ошибка в Custom Action: {0}", ex.Message));
                    return ActionResult.Failure;
                }
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RemoveAfterInstall(Session session)
        {
            try
            {
                string path = Path.Combine(session["INSTALLDIR"], "Installer");
                session.Log(String.Format("Path = {0}", path));
                if (Directory.Exists(path))
                {
                    session.Log(String.Format("Удаляем {0}", path));
                    Directory.Delete(path, true);
                }

                path = Path.Combine(session["INSTALLDIR"], Path.Combine("App", "Client"));
                session.Log(String.Format("Path = {0}", path));
                if (Directory.Exists(path))
                {
                    session.Log(String.Format("Удаляем {0}", path));
                    Directory.Delete(path, true);
                }

                path = Path.Combine(session["INSTALLDIR"], Path.Combine("App", "MDXExpert"));
                session.Log(String.Format("Path = {0}", path));
                if (Directory.Exists(path))
                {
                    session.Log(String.Format("Удаляем {0}", path));
                    Directory.Delete(path, true);
                }

                path = Path.Combine(session["INSTALLDIR"], Path.Combine("App", "Office Add-in"));
                session.Log(String.Format("Path = {0}", path));
                if (Directory.Exists(path))
                {
                    session.Log(String.Format("Удаляем {0}", path));
                    Directory.Delete(path, true);
                }

                path = Path.Combine(session["INSTALLDIR"], "bat.cmd");
                session.Log(String.Format("Path = {0}", path));
                if (File.Exists(path))
                {
                    session.Log(String.Format("Удаляем {0}", path));
                    File.Delete(path);
                }

                /*path = Path.Combine(session["INSTALLDIR"], "Krista.FM.Update.CustomAction.CA.dll");
                session.Log(String.Format("Path = {0}", path));
                if (File.Exists(path))
                {
                    session.Log(String.Format("Удаляем {0}", path));
                    File.Delete(path);
                }

                path = Path.Combine(session["INSTALLDIR"], "Krista.FM.Updater.CustomActionExe.exe");
                session.Log(String.Format("Path = {0}", path));
                if (File.Exists(path))
                {
                    session.Log(String.Format("Удаляем {0}", path));
                    File.Delete(path);
                }*/
            }
            catch (IOException e)
            {
                session.Log(String.Format("Remove error: {0}", e.Message));
                return ActionResult.Failure;
            }
            catch (ArgumentNullException e)
            {
                session.Log(String.Format("Remove error: {0}", e.Message));
                return ActionResult.Failure;
            }
            catch (ArgumentException e)
            {
                session.Log(String.Format("Remove error: {0}", e.Message));
                return ActionResult.Failure;
            }
            catch (NotSupportedException e)
            {
                session.Log(String.Format("Remove error: {0}", e.Message));
                return ActionResult.Failure;
            }
            catch (UnauthorizedAccessException e)
            {
                session.Log(String.Format("Remove error: {0}", e.Message));
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult KillNotifyProcess(Session session)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName("Krista.FM.Update.NotifyIconApp");
                foreach (Process process in processes)
                {
                    session.Log(String.Format("Удаляем процесс: {0}", process));
                    process.Kill();
                }
            }
            catch (InvalidOperationException e)
            {
                session.Log(e.Message);
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        
        [CustomAction]
        public static ActionResult SetFolderPermissionWP(Session session)
        {
            if (!Directory.Exists(session["INSTALLLOCATIONWP"]))
                return ActionResult.Success;

            try
            {
                var procInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = "Krista.FM.Updater.CustomActionExe.exe",
                    WorkingDirectory =
                        session["INSTALLLOCATIONWP"],
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = String.Format("{0} \"{1}\"", new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null).Value, session["INSTALLLOCATIONWP"])

                };
                Process process = Process.Start(procInfo);  //Start that process.
                process.WaitForExit();
                var procInfo1 = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = "Krista.FM.Updater.CustomActionExe.exe",
                    WorkingDirectory =
                        session["INSTALLLOCATIONWP"],
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = String.Format("{0} \"{1}\"", new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).Value, session["INSTALLLOCATIONWP"])

                };
                Process process1 = Process.Start(procInfo1);  //Start that process.
                process1.WaitForExit();
            }
            catch (Exception ex)
            {
                session.Log(string.Format("Ошибка в Custom Action: {0}", ex.Message));
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult SetFolderPermissionOA(Session session)
        {
            if (!Directory.Exists(session["INSTALLLOCATIONOA"]))
                return ActionResult.Success;

            try
            {
                var procInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = "Krista.FM.Updater.CustomActionExe.exe",
                    WorkingDirectory =
                        session["INSTALLLOCATIONOA"],
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = String.Format("{0} \"{1}\"", new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null).Value, session["INSTALLLOCATIONOA"])

                };
                Process process = Process.Start(procInfo);  //Start that process.
                process.WaitForExit();
                var procInfo1 = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = "Krista.FM.Updater.CustomActionExe.exe",
                    WorkingDirectory =
                        session["INSTALLLOCATIONOA"],
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = String.Format("{0} \"{1}\"", new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).Value, session["INSTALLLOCATIONOA"])

                };
                Process process1 = Process.Start(procInfo1);  //Start that process.
                process1.WaitForExit();
            }
            catch (Exception ex)
            {
                session.Log(string.Format("Ошибка в Custom Action: {0}", ex.Message));
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult SetFolderPermissionMDXExpert(Session session)
        {
            if (!Directory.Exists(session["INSTALLDIR"]))
            {
                session.Log(String.Format("Не найдена директория {0}", session["INSTALLDIR"]));
                return ActionResult.Success;
            }

            try
            {
                var procInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = "Krista.FM.Updater.CustomActionExe.exe",
                    WorkingDirectory =
                        session["INSTALLDIR"],
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = String.Format("{0} \"{1}\"", new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null).Value, session["INSTALLDIR"])

                };
                session.Log(String.Format("На директорию {0} для группы {1} будут выданы права на чтение и запись",
                                          session["INSTALLDIR"],
                                          new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null).Value));
                Process process = Process.Start(procInfo);  //Start that process.
                process.WaitForExit();
                session.Log("Права выданы");
                var procInfo1 = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = "Krista.FM.Updater.CustomActionExe.exe",
                    WorkingDirectory =
                        session["INSTALLDIR"],
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = String.Format("{0} \"{1}\"", new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).Value, session["INSTALLDIR"])

                };
                
                session.Log(String.Format("На директорию {0} для группы {1} будут выданы права на чтение и запись",
                          session["INSTALLDIR"],
                          new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).Value));
                Process process1 = Process.Start(procInfo1);  //Start that process.
                process1.WaitForExit();
                session.Log("Права выданы");
            }
            catch (Exception ex)
            {
                session.Log(string.Format("Ошибка в Custom Action: {0}", ex.Message));
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }
        
        /// <summary>
        /// Преобразование из SID в имя пользователя/группы
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult TranslateSidToName(Session session)
        {
            var property = session["PROPERTY_TO_BE_TRANSLATED"];
            if (String.IsNullOrEmpty(property))
            {
                session.Log("The {0} property that should say what property to translate is empty", property);
                return ActionResult.Failure;
            }

            var sid = session[property];
            if (String.IsNullOrEmpty(sid))
            {
                session.Log("The {0} property that should contain the SID to translate is empty", property);
                return ActionResult.Failure;
            }

            try
            {
                // convert the user sid to a domain\name
                var account = new SecurityIdentifier(sid).Translate(typeof(NTAccount)).ToString();
                session[property] = account;
                session.Log("The {0} property translated from {1} SID to {2}", property, sid, account);
            }
            catch (Exception e)
            {
                session.Log("Exception getting the name for the {0} sid. Message: {1}", sid, e.Message);
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }
    }
}
