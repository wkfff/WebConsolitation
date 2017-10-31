using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using Krista.FM.Update.ColdUpdater.Actions;
using Microsoft.Win32.SafeHandles;
using System.Runtime.Serialization.Formatters.Binary;
using Trace = Krista.FM.Update.ColdUpdater;

namespace Krista.FM.Update.ColdUpdater
{
    public static class AppStart
    {
        #region const and field

        private const string InstalledUpdatesFileName = "InstalledUpdates.xml";
        private const string InstalledUpdatesFileNameTemp = "InstalledUpdatesTemp.xml";
        private const string ReceivedUpdatesFileName = "ReceivedUpdates.xml";
        private const string ReceivedUpdatesFileNameTemp = "ReceivedUpdatesTemp.xml";

        static readonly uint GENERIC_READ = (0x80000000);
        static readonly uint OPEN_EXISTING = 3;
        static readonly uint FILE_FLAG_OVERLAPPED = (0x40000000);
        static readonly int BUFFER_SIZE = 4096;
        private static string appFolder;
        private static string syncProcessName;

        static string appPath, appDir, tempFolder, backupFolder = String.Empty, processName;
        static string serviceName;
        static string appVersion;
        static bool relaunchApp = true, updateSuccessful = true, rollback, autoUpdate;
        static Dictionary<string, object> dict;

        #endregion

        #region win32

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeFileHandle CreateFile(
            String pipeName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplate);

        #endregion

        private static void Main()
        {
            // отладка
            // Debugger.Launch();
            try
            {
                // массив аргументов командной строки
                string[] args = Environment.GetCommandLineArgs();
                string[] updateArgs = args[1].Split(',');
                if (updateArgs.Length < 2)
                {
                    MessageBox.Show("Приложение Updaterfm.exe переданы не все параметры!");
                    return;
                }

                syncProcessName = updateArgs[0];
                appFolder = updateArgs[1];

                // Подключение трассировки
                InitializeDiagnostics();

                // Connect to the named pipe and retrieve the updates list
                string PIPE_NAME = string.Format("\\\\.\\pipe\\{0}", syncProcessName);
                object updates = GetUpdates(PIPE_NAME);

                // Чтение параметров обновления
                appPath = ReadParameters(updates);

                if (string.IsNullOrEmpty(syncProcessName))
                {
                    Application.Exit();
                }

                if (!String.IsNullOrEmpty(serviceName))
                {
                    if (!WaitStopService())
                    {
                        throw new InstallUpdateException(String.Format(
                                "Не удалось остановить службу {0}. Для более детальной ошибки см. логи обновления.", serviceName));
                    }
                }
                else
                {
                    WaitProcessExit();
                }

                InstallUpdates();

                Trace.TraceInformation("Обновление завершилось успешно!");
            }
            catch (GetUpdatesException e)
            {
                Trace.TraceError(String.Format("В процессе работы updaterfm.exe возникло исключение: {0}", e.Message));
                MessageBox.Show(String.Format("В процессе работы updaterfm.exe возникло исключение: {0}", e.Message),
                                "Ошибка при установке обновлений.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (InstallUpdateException e)
            {
                Trace.TraceError(String.Format("В процессе работы updaterfm.exe возникло исключение: {0}", e.Message));
                MessageBox.Show(String.Format("В процессе работы updaterfm.exe возникло исключение: {0}", e.Message),
                                "Ошибка при установке обновлений.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ReadParametersException e)
            {
                Trace.TraceError(String.Format("В процессе работы updaterfm.exe возникло исключение: {0}", e.Message));
                MessageBox.Show(String.Format("В процессе работы updaterfm.exe возникло исключение: {0}", e.Message),
                                "Ошибка при установке обновлений.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception e)
            {
                Trace.TraceError(String.Format("В процессе работы updaterfm.exe возникло исключение: {0}", e.Message));
                MessageBox.Show(String.Format("В процессе работы updaterfm.exe возникло исключение: {0}", e.Message),
                                "Ошибка при установке обновлений.", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            finally
            {
                Trace.TraceInformation("Закрываем приложение");
                Application.Exit();
            }
        }

        private static void InitializeDiagnostics()
        {
            try
            {
                if (!Directory.Exists(Path.Combine(appFolder, "UpdateLog")))
                {
                    return;
                }

                TraceSource source = Diagnostics.KristaDiagnostics.GetTraceSource("Krista.FM.Update.ColdUpdater");
                SourceSwitch sourceSwitch = new SourceSwitch("SourceSwitchUpdate", "Verbose");
                source.Switch = sourceSwitch;
                source.Listeners.Clear();
                string _currentLog = Path.Combine(Path.Combine(appFolder, "UpdateLog"),
                                                  string.Format("log_updates_{0:yyyy-MM-dd_HH.mm.ss}.txt", DateTime.Now));
                source.Listeners.Add(
                    new TextWriterTraceListener(_currentLog, "textlog"
                        ));
            }
            catch (Exception e)
            {
                using (var writer =
                   new StreamWriter(string.Format("{0}\\CrashUpdater_{1:yyyy-MM-dd_HH.mm.ss}.txt",
                                                  appFolder, DateTime.Now)))
                {
                    writer.Write(Diagnostics.KristaDiagnostics.ExpandException(e));
                }
            }
        }

        private static void WaitProcessExit()
        {
            Trace.TraceInformation("Старт ожидание закрытия клиенских приложений");
            Trace.Indent();
            try
            {
                List<string> processList = new List<string>();

                if ("OfficeAddInUpdateSync".Equals(syncProcessName))
                {
                    processList.Add("EXCEL");
                    processList.Add("WINWORD");
                    processList.Add("Krista.FM.Client.Workplace");

                    WaitProcessesExit(processList);
                }
                else
                {
                    Trace.TraceInformation(String.Format("Рабочий процесс: {0}", syncProcessName));
                    string _processName = (GetProcessName());
                    if (!String.IsNullOrEmpty(_processName))
                    {
                        processList.Add(_processName);
                    }

                    WaitProcessesExit(processList);    
                }
            }
            finally
            {
                Trace.Unindent();
            }

            Trace.TraceInformation("Окончание ожидание закрытия клиенских приложений");
        }

        private static bool WaitStopService()
        {
            Trace.TraceInformation(String.Format("Ищем приложение {0}", "Krista.FM.Update.NotifyIconApp.exe"));
            var notifyProcess = Process.GetProcessesByName("Krista.FM.Update.NotifyIconApp");
            Trace.TraceInformation(String.Format("Найдено приложений: {0}", notifyProcess.Length));
            foreach (var process in notifyProcess)
            {
                Trace.TraceInformation(String.Format("Останавливаем приложение {0}", "Krista.FM.Update.NotifyIconApp.exe"));
                process.Kill();
            }

            Trace.TraceInformation(String.Format("Останавливаем службу {0}", serviceName));
            ServiceController controller = new ServiceController();
            controller.MachineName = Environment.MachineName;
            controller.ServiceName = serviceName;

            try
            {
                // Stop the service
                controller.Stop();
                Trace.TraceInformation("Остановили.");
                Thread.Sleep(15000);

                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError(String.Format("При завервении работы службы возникло исключение: {0}", e.Message));
                return false;
            }
        }

        /// <summary>
        /// В ручном режиме обновления - ожидаем закрытия обновляемых приложений,
        /// В автоматическом - закрываем принудительно.
        /// </summary>
        /// <param name="processList"></param>
        private static void WaitProcessesExit(List<string> processList)
        {
            foreach (string name in processList)
            {
                List<Process> processes = GetProcessInWorkingDir(Process.GetProcessesByName(name));
                int i = processes.Count;
                Trace.TraceInformation(String.Format("Найдено {0} процессов {1}, требующих завершения.", i, name));

                if (autoUpdate)
                {
                    foreach (Process process in processes)
                    {
                        Trace.TraceInformation(String.Format("Закрытия процесса {0}", process.ProcessName));
                        Trace.Indent();
                        try
                        {
                            process.Kill();
                        }
                        finally
                        {
                            Trace.Unindent();
                        }

                        i--;
                    }
                }

                foreach (Process process in processes)
                {
                    Trace.TraceInformation(String.Format("Ожидание закрытия процесса {0}", process.ProcessName));
                    Trace.Indent();
                    try
                    {
                        while (!process.WaitForExit(5000))
                        {
                            Trace.TraceInformation(String.Format("Ожидание закрытия ..."));
                        }
                    }
                    finally
                    {
                        Trace.Unindent();
                    }

                    i--;
                    Trace.TraceInformation("Процесс {0} завершен. Осталось {1} процессов {2} ожидающих закрытия",
                                           process.ProcessName, i, process.ProcessName);
                }
            }
        }

        private static List<Process> GetProcessInWorkingDir(Process[] getProcessesByName)
        {
            List<Process> processes = new List<Process>();

            foreach (Process process in getProcessesByName)
            {
                if ("OfficeAddInUpdateSync".Equals(syncProcessName))
                {
                    processes.Add(process);
                }
                else if (Path.GetDirectoryName(process.MainModule.FileName) == appFolder)
                {
                    processes.Add(process);
                }
            }

            return processes;
        }

        /// <summary>
        /// Ожидание запуска процесса
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool WaitProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(name))
                {
                    return true;
                }
            }

            Thread.Sleep(1000);

            return WaitProcessOpen(name);
        } 

        private static void InstallUpdates()
        {
            if ("OfficeAddInUpdateSync".Equals(syncProcessName))
            {
                relaunchApp = false;
            }

            // обновление
            Dictionary<string, object>.Enumerator en = dict.GetEnumerator();
            // успешно примененные файлы
            Dictionary<string, IUpdateAction> successAction = new Dictionary<string, IUpdateAction>();
            while (en.MoveNext())
            {
                if (en.Current.Key.StartsWith("ENV:"))
                    continue;

                string patchName;
                string localName;
                GetPachNameAndLocalName(en.Current.Key, out patchName, out localName);

                if (!String.IsNullOrEmpty(patchName))
                {
                    bool isCopy = BacupLocalFile(Path.Combine(backupFolder, Path.Combine(appVersion, patchName)), localName,
                                    appDir);
                    if (!isCopy)
                    {
                        Trace.TraceError(String.Format("Не удалось скопировать файл в каталог для отмены обновлений."));
                    }
                }

                IUpdateAction a = null;
                if (String.IsNullOrEmpty(en.Current.Value.ToString()))
                {
                    a = new FileDeleteActions(Path.Combine(appDir, localName));
                }
                else if (en.Current.Value is string)
                {
                    a = new FileCopyAction(en.Current.Value.ToString(),
                                            Path.Combine(appDir, localName));
                }
                else if (en.Current.Value is byte[])
                {
                    a = new FileDumpAction(Path.Combine(appDir, localName), (byte[]) en.Current.Value);
                }

                if (a != null)
                {
                    try
                    {
                        Trace.TraceInformation(String.Format("Копируем {0}", localName));
                        if (!a.Do())
                        {
                            updateSuccessful = false;
                            break;
                        }

                        if (!rollback)
                        {
                            successAction.Add(
                                Path.Combine(Path.Combine(backupFolder, Path.Combine(appVersion, patchName)), localName),
                                a);
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(String.Format("В процессе обновления возникло исключение: {0}", e.Message));

                        try
                        {
                            Trace.TraceInformation("Откатываем уже примененные обновления.");
                            // в случае ошибки необходимо откатить ранее примененные файлы
                            foreach (var updateAction in successAction)
                            {
                                updateAction.Value.Rollback(updateAction.Key);
                            }
                        }
                        catch (Exception exception)
                        {
                            Trace.TraceError(String.Format("При откате изменений после ошибки возникло исключение: {0}", exception.Message));
                        }
                        
                        updateSuccessful = false;
                        break;
                    }
                }
            }

            // если все успешно заменяем InstalledUpdatesTemp
            ReplaceInstalledUpdatesTemp();
            // после операции Rollback удаляем патч из Backup
            if(rollback && updateSuccessful)
            {
                try
                {
                    foreach (var o in dict)
                    {
                        if (o.Key.StartsWith("ENV:BackupFolder"))
                        {
                            if (Directory.Exists(o.Value.ToString()))
                                Directory.Delete(o.Value.ToString(), true);       
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, e.Message);
                }
            }

            Trace.TraceInformation(String.IsNullOrEmpty(serviceName)
                                       ? String.Format("Запускаем рабочий процесс: {0}", appPath)
                                       : String.Format("Запускаем рабочий процесс: {0}", serviceName));
            if (updateSuccessful)
            {
                RunMainProcess();
            }

            try
            {
                ProcessStartInfo Info = new ProcessStartInfo
                                            {
                                                Arguments =
                                                    string.Format(
                                                        @"/C del ""{0}\*.*"" & rmdir ""{0}"""
                                                        , tempFolder),
                                                WindowStyle = ProcessWindowStyle.Hidden,
                                                CreateNoWindow = true,
                                                FileName = "cmd.exe"
                                            };
                Process.Start(Info);
            }
            catch { /* ignore exceptions thrown while trying to clean up */ }
        }

        private static void RunMainProcess()
        {
            if (relaunchApp)
            {
                if (!String.IsNullOrEmpty(serviceName))
                {
                    Trace.TraceInformation(String.Format("Запускаем службу {0}.", serviceName));
                    try
                    {
                        ServiceController controller = new ServiceController();
                        controller.MachineName = Environment.MachineName;
                        controller.ServiceName = serviceName;

                        // Start the service
                        controller.Start();
                        Thread.Sleep(10000);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceInformation(String.Format("При запуске службы {0} возникло исключение: {1}", serviceName, e.Message));
                        throw new InstallUpdateException(String.Format(
                            "При запуске службы {0} возникло исключение: {1}", serviceName, e.Message));
                    }

                    Trace.TraceInformation(String.Format("Запускаем приложение {0}.", "NotifyIconApp.exe"));

                    ApplicationLoader.PROCESS_INFORMATION procInfo;
                    ApplicationLoader.StartProcessAndBypassUAC(
                        Path.Combine(Path.GetDirectoryName(appPath), "Krista.FM.Update.NotifyIconApp.exe"), out procInfo);
                    //Process.Start(Path.Combine(Path.GetDirectoryName(appPath), "Krista.FM.Update.NotifyIconApp.exe"));
                }
                else
                {
                    Process.Start(appPath);    
                }
            }
        }

        private static void ReplaceInstalledUpdatesTemp()
        {
            try
            {
                appDir = Path.Combine(appDir, String.Format("{0}\\{1}", "InstalledUpdates", appVersion));

                DirectoryInfo directoryInfo = new DirectoryInfo(appDir);
                directoryInfo.CreateDirectory();

                if (File.Exists(Path.Combine(appDir, InstalledUpdatesFileNameTemp)))
                {
                    File.Copy(Path.Combine(appDir, InstalledUpdatesFileNameTemp),
                              Path.Combine(appDir, InstalledUpdatesFileName), true);
                    File.Delete(Path.Combine(appDir, InstalledUpdatesFileNameTemp));
                }

                if (File.Exists(Path.Combine(appDir, ReceivedUpdatesFileNameTemp)))
                {
                    File.Copy(Path.Combine(appDir, ReceivedUpdatesFileNameTemp),
                              Path.Combine(appDir, ReceivedUpdatesFileName), true);
                    File.Delete(Path.Combine(appDir, ReceivedUpdatesFileNameTemp));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.Message);
            }
        }

        public static string ReadParameters(object updates)
        {
            Trace.TraceInformation("Считываем параметры, переданные через именованный канал");

            try
            {
                if (updates is Dictionary<string, object>)
                {
                    dict = updates as Dictionary<string, object>;
                }

                if (dict == null || dict.Count == 0)
                {
                    Application.Exit();
                }

                // Параметры для обновления
                appPath = dict["ENV:AppPath"].ToString();
                appDir = Path.GetDirectoryName(appPath);

                if (dict.ContainsKey("ENV:OfficeAddInPath"))
                {
                    appDir = dict["ENV:OfficeAddInPath"].ToString();
                }

                tempFolder = dict["ENV:TempFolder"].ToString();
                if (dict.ContainsKey("ENV:BackupFolder"))
                {
                    backupFolder = dict["ENV:BackupFolder"].ToString();
                }

                relaunchApp = dict["ENV:RelaunchApplication"] as bool? ?? true;
                if (dict.ContainsKey("ENV:Rollback"))
                {
                    rollback = dict["ENV:Rollback"] as bool? ?? false;
                }

                if (dict.ContainsKey("ENV:ServiceName"))
                {
                    serviceName = dict["ENV:ServiceName"].ToString();
                }

                if (dict.ContainsKey("ENV:ProcessName"))
                {
                    processName = dict["ENV:ProcessName"].ToString();
                }

                if (dict.ContainsKey("ENV:AppVersion"))
                {
                    appVersion = dict["ENV:AppVersion"].ToString();
                }

                if (dict.ContainsKey("ENV:AutoUpdateMode"))
                {
                    autoUpdate = dict["ENV:AutoUpdateMode"] as bool? ?? false;
                }

                Trace.TraceInformation("Параметры, переданные через именованный канал, получены успешно.");

                return appPath;
            }
            catch (Exception e)
            {
                throw new ReadParametersException(String.Format("Ошибка при чтение переданных параметров : {0}", e.Message));
            }
        }

        internal static void CloseCurrentProcess()
        {
            string officeApp = String.Empty;
            if (Process.GetCurrentProcess().ProcessName.ToUpper() == "EXCEL")
                officeApp = "WINWORD";
            if (Process.GetCurrentProcess().ProcessName.ToUpper() == "WINWORD")
                officeApp = "EXCEL";
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.StartsWith(processName)
                    || (!String.IsNullOrEmpty(officeApp) && clsProcess.ProcessName.StartsWith(officeApp)))
                {
                    try
                    {
                        if (clsProcess.MainWindowHandle == IntPtr.Zero)
                        {
                            clsProcess.Kill();
                        }
                        else
                        {
                            Thread.Sleep(1000);
                            clsProcess.CloseMainWindow();
                            // Ждем завершения основного потока
                            clsProcess.WaitForExit();
                            clsProcess.Close();
                        }
                    }
                    catch (Win32Exception e)
                    {
                        Trace.TraceError(
                            String.Format("При закрытии текущего процесса возникло исключение: {0}",
                                          e.Message));
                    }
                    catch (NotSupportedException e)
                    {
                        Trace.TraceError(
                            String.Format("При закрытии текущего процесса возникло исключение: {0}",
                                          e.Message));
                    }
                    catch (InvalidOperationException e)
                    {
                        Trace.TraceError(
                            String.Format("При закрытии текущего процесса возникло исключение: {0}",
                                          e.Message));
                    }
                    catch (SystemException e)
                    {
                        Trace.TraceError(
                            String.Format("При закрытии текущего процесса возникло исключение: {0}",
                                          e.Message));
                    }
                }
            }
        }

        public static bool BacupLocalFile(string backupFolder, string localName, string appFolder)
        {
            if (String.IsNullOrEmpty(backupFolder))
            {
                Trace.TraceInformation("Отсутствует обязательный параметр");
                throw new ArgumentNullException("backupFolder");  
            }

            if (String.IsNullOrEmpty(localName))
            {
                Trace.TraceInformation("Отсутствует обязательный параметр");
                throw new ArgumentNullException("localName");
            }

            if (String.IsNullOrEmpty(appFolder))
            {
                Trace.TraceInformation("Отсутствует обязательный параметр");
                throw new ArgumentNullException("appFolder");
            }

            try
            {
                new DirectoryInfo(backupFolder);
            }
            catch (System.IO.PathTooLongException)
            {
                throw new ArgumentOutOfRangeException(string.Format("Слишком длинное имя{0}", "ARG0"));
            }
            catch (Exception)
            { }

            if (!IsValidFilename(backupFolder))
            {
                return false;
            }

            if (!IsValidFilename(appFolder))
            {
                return false;
            }

            try
            {
                if (!Directory.Exists(backupFolder))
                    Directory.CreateDirectory(backupFolder);

                if (!Directory.Exists(Path.GetDirectoryName(Path.Combine(backupFolder, localName))))
                    Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(backupFolder, localName)));

                if (File.Exists(Path.Combine(appFolder, localName)))
                {
                    // сохраняем предыдуще версии файлов для возможности отката
                    // копируем с перезаписью существующих файлов!
                    File.Copy(Path.Combine(appFolder, localName), Path.Combine(backupFolder, localName),
                              true);
                }
            }
            catch (Exception e)
            {
                Trace.TraceInformation(String.Format("При копировании файла в Backup возникло исключение: {0}", e));
                return false;
            }

            return true;
        }

        public static bool IsValidFilename(string testName)
        {
            string regexString = "[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]";
            Regex containsABadCharacter = new Regex(regexString);

            if (containsABadCharacter.IsMatch(testName))
            {
                return false;
            }

            return true;
        }

        private static void GetPachNameAndLocalName(string key, out string patchName, out string localName)
        {
            string[] parts = key.Split(':');
            if (parts.Length == 1)
            {
                localName = key;
                patchName = null;
            }
            else
            {
                patchName = parts[0];
                localName = parts[1];
            }
        }

        private static object GetUpdates(string PIPE_NAME)
        {
            try
            {
                using (SafeFileHandle pipeHandle = CreateFile(
                    PIPE_NAME,
                    GENERIC_READ,
                    0,
                    IntPtr.Zero,
                    OPEN_EXISTING,
                    FILE_FLAG_OVERLAPPED,
                    IntPtr.Zero))
                {

                    if (pipeHandle.IsInvalid)
                        return null;

                    using (FileStream fStream = new FileStream(pipeHandle, FileAccess.Read, BUFFER_SIZE, true))
                    {
                        return new BinaryFormatter().Deserialize(fStream);
                    }
                }
            }
            catch (Exception e)
            {
                throw new GetUpdatesException(String.Format("Ошибки при получение списка объектов на обновление: {0}", e.Message));
            }
        }

        private static string GetProcessName()
        {
            switch (syncProcessName)
            {
                case "WorkplaceUpdateSync":
                    return "Krista.FM.Client.Workplace";
                case "SchemeDesignerUpdateSync":
                    return "Krista.FM.Client.SchemeDesigner";
                case "OlapAdminUpdateSync":
                    return "Krista.FM.Client.OlapAdmin";
                case "MDXExpertUpdateSync":
                    return "MDXExpert";
                default:
                    return String.Empty;
            }

        }
    }

    public static class Extensions
    {
        /// <summary>
        /// Рекурсивное создание каталога
        /// </summary>
        /// <param name="dirInfo">Каталог, который требуется создать</param>
        public static void CreateDirectory(this DirectoryInfo dirInfo)
        {
            if (dirInfo.Parent != null)
            {
                CreateDirectory(dirInfo.Parent);
            }

            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
        }
    }
}