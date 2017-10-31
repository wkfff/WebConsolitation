using System;
using System.Linq;
using Microsoft.Win32;

namespace Krista.FM.Update.Framework.UpdateObjects.Tasks
{
    /// <summary>
    /// Задача обновления на версию, обновляет информацию о версии в реестре
    /// </summary>
    [Serializable]
    public class VersionUpdateTask : UpdateTask
    {
        const string uninstallProgramsKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        const string installedProgramsKey = @"Software\Microsoft\Installer\Products";

        public VersionUpdateTask(IUpdatePatch owner) : base(owner)
        {
        }

        public override bool Prepare(IUpdateSource source)
        {
            // пользователь
            string user = string.Format("{0}\\{1}", Environment.UserDomainName, Environment.UserName);

            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(uninstallProgramsKey))
            {
                string programKey = String.Empty;
                try
                {
                    programKey =
                        rk.GetSubKeyNames().Where(
                            r =>
                            rk.OpenSubKey(r).GetValue("DisplayName") != null &&
                            rk.OpenSubKey(r).GetValue("DisplayName").ToString().ToLower().Contains(
                                Attributes["DisplayName"].ToLower())).First();
                }
                catch (Exception e)
                {
                    Trace.TraceWarning(String.Format("В кусте реестра {0} не найдено приложение с атрибутом {1}", programKey, Attributes["DisplayName"]));
                    IsPrepared = PrepareState.PrepareWithWarning;
                    return false;
                }

                if (String.IsNullOrEmpty(programKey))
                {
                    Trace.TraceWarning(String.Format("Не найдена ветка реестра {0}", programKey));
                    IsPrepared = PrepareState.PrepareWithWarning;
                    return false;
                }

                try
                {
                    rk.OpenSubKey(programKey, true);
                }
                catch (System.Security.SecurityException e)
                {
                    Trace.TraceWarning(String.Format("У пользователя {0} нет прав на изменение ветки реестра. {1}",
                                                     user, e));
                    IsPrepared = PrepareState.PrepareWithWarning;
                    return false;
                }
            }

            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(installedProgramsKey))
            {
                string programKey = String.Empty;
                try
                {
                    programKey =
                        rk.GetSubKeyNames().Where(
                            r =>
                            rk.OpenSubKey(r).GetValue("ProductName") != null &&
                            rk.OpenSubKey(r).GetValue("ProductName").ToString().ToLower().Contains(
                                Attributes["DisplayName"].ToLower())).First();
                }
                catch (Exception e)
                {
                    Trace.TraceWarning(String.Format("В кусте реестра {0} не найдено приложение с атрибутом {1}", programKey, Attributes["DisplayName"]));
                    IsPrepared = PrepareState.PrepareWithWarning;
                    return false;
                }

                if (String.IsNullOrEmpty(programKey))
                {
                    Trace.TraceWarning(String.Format("Не найдена ветка реестра {0}", programKey));
                    IsPrepared = PrepareState.PrepareWithWarning;
                    return false;
                }

                try
                {
                    rk.OpenSubKey(programKey, true);
                }
                catch (System.Security.SecurityException e)
                {
                    Trace.TraceWarning(String.Format("У пользователя {0} нет прав на изменение ветки реестра. {1}",
                                                     user, e));
                    IsPrepared = PrepareState.PrepareWithWarning;
                    return false;
                }
            }

            IsPrepared = PrepareState.PrepareSuccess;
            return true;
        }

        public override ExecuteState Execute()
        {
            if (IsPrepared == PrepareState.PrepareWithError)
            {
                Trace.TraceError(
                    String.Format(
                        "Задача {0} из патча {1} не выполнена, потому что для неё операция подготовки завершилась неудачно",
                        this.GetType().Name,
                        Owner.Name));
                return ExecuteState.ExecuteWithError;
            }
           
            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(uninstallProgramsKey))
            {
                try
                {
                    foreach (string programKey in rk.GetSubKeyNames().Where(
                            r => rk.OpenSubKey(r).GetValue("DisplayName") != null &&
                                 rk.OpenSubKey(r).GetValue("DisplayName").ToString().ToLower().Contains(
                                     Attributes["DisplayName"].ToLower())))
                    {

                        RegistryKey registryKey = rk.OpenSubKey(programKey, true);
                        registryKey.SetValue("DisplayVersion", Attributes["DisplayVersion"]);
                        registryKey.SetValue("DisplayName",
                                             string.Format("{0} {1}", Attributes["DisplayName"],
                                                           Attributes["DisplayVersion"]));
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(
                        String.Format("Не удалось изменить номер версии в реестре : {0}", ex.Message));
                }
            }

            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(installedProgramsKey))
            {
                try
                {
                    foreach (string programKey in rk.GetSubKeyNames().Where(
                            r => rk.OpenSubKey(r).GetValue("ProductName") != null &&
                                 rk.OpenSubKey(r).GetValue("ProductName").ToString().ToLower().Contains(
                                     Attributes["DisplayName"].ToLower())))
                    {
                        RegistryKey registryKey = rk.OpenSubKey(programKey, true);
                        registryKey.SetValue("ProductName",
                                             string.Format("{0} {1}", Attributes["DisplayName"],
                                                           Attributes["DisplayVersion"]));
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(
                        String.Format("Не удалось изменить номер версии в реестре : {0}", ex.Message));
                }
            }

            return ExecuteState.ExecuteSuccess;
        }
    }
}
