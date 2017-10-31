using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace Krista.FM.Update.Framework
{
    /// <summary>
    /// Starts the cold update process by extracting the updater app from the library's resources,
    /// passing it all the data it needs and terminating the current application
    /// </summary>
    internal class UpdateStarter
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern SafeFileHandle CreateNamedPipe(
           String pipeName,
           uint dwOpenMode,
           uint dwPipeMode,
           uint nMaxInstances,
           uint nOutBufferSize,
           uint nInBufferSize,
           uint nDefaultTimeOut,
           IntPtr lpSecurityAttributes);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int ConnectNamedPipe(
           SafeFileHandle hNamedPipe,
           IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern SafeFileHandle CreateFile(
           String pipeName,
           uint dwDesiredAccess,
           uint dwShareMode,
           IntPtr lpSecurityAttributes,
           uint dwCreationDisposition,
           uint dwFlagsAndAttributes,
           IntPtr hTemplate);

        //private const uint DUPLEX = (0x00000003);
        private const uint WRITE_ONLY = (0x00000002);
        private const uint FILE_FLAG_OVERLAPPED = (0x40000000);

        internal string PIPE_NAME { get { return string.Format("\\\\.\\pipe\\{0}", _syncProcessName); } }
        internal uint BUFFER_SIZE = 4096;

        private readonly string _updaterPath;
        private readonly Dictionary<string, object> _updateData;
        private readonly string _syncProcessName;

        public UpdateStarter(string pathWhereUpdateExeShouldBeCreated,
            Dictionary<string, object> updateData, string syncProcessName)
        {
            _updaterPath = pathWhereUpdateExeShouldBeCreated;
            _updateData = updateData;
            _syncProcessName = syncProcessName;
        }

        public bool Start()
        {
            // Debugger.Break();

            ExtractUpdaterFromResource(); //take the update executable and extract it to the path where it should be created

            using (SafeFileHandle clientPipeHandle = CreateNamedPipe(
                   PIPE_NAME,
                   WRITE_ONLY | FILE_FLAG_OVERLAPPED,
                   0,
                   1, // 1 max instance (only the updater utility is expected to connect)
                   BUFFER_SIZE,
                   BUFFER_SIZE,
                   0,
                   IntPtr.Zero))
            {
                //failed to create named pipe
                if (clientPipeHandle.IsInvalid)
                    return false;


                ProcessStartInfo info = new ProcessStartInfo(_updaterPath, string.Format(@"""{0},{1}""", _syncProcessName, Path.GetDirectoryName(UpdateManager.Instance.ApplicationPath)))
                                            {
                                                UseShellExecute = false,
                                                Verb = "runas"
                                            };
                try
                {
                    Process.Start(info);
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    return false;
                }

                while (true)
                {
                    int success = 0;
                    try
                    {
                        success = ConnectNamedPipe(
                           clientPipeHandle,
                           IntPtr.Zero);
                    }
                    catch { }

                    if (success != 1)
                    {
                        Trace.TraceInformation("Failed to connect client pipe");
                        break;
                    }

                    Trace.TraceInformation("Client connection successfull");

                    // сериализуем "холодные" замены в файл
                    using (FileStream fStream = new FileStream(clientPipeHandle, FileAccess.Write, (int)BUFFER_SIZE, true))
                    {
                        new BinaryFormatter().Serialize(fStream, _updateData);
                        fStream.Close();
                    }
                }
            }

            return true;
        }

        private void ExtractUpdaterFromResource()
        {
            Trace.TraceVerbose("Проверяем, нет ли ранее запущенных процессов обновления: {0}", _updaterPath);
            Process[] processes = Process.GetProcessesByName("updaterfm");
            foreach (var process in processes)
            {
                Trace.TraceVerbose("_updaterPath {0}", Path.GetDirectoryName(_updaterPath));
                Trace.TraceVerbose("_currentDirectory {0}", Path.GetDirectoryName(GetProcessModule(process).FileName));

                var directoryName = Path.GetDirectoryName(_updaterPath);
                if (directoryName != null && directoryName.Equals(Path.GetDirectoryName(GetProcessModule(process).FileName)))
                {
                    // проверять откуда запущен процесс
                    Trace.TraceVerbose("Убиваем процесс.");
                    process.Kill();
                    Thread.Sleep(2000);
                }
            }
            
            if (!Directory.Exists(Path.GetDirectoryName(_updaterPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(_updaterPath));

            Trace.TraceInformation("Копируем во временную папку Krista.Diagnostics");
            File.Copy(
                Path.Combine(Path.GetDirectoryName(UpdateManager.Instance.ApplicationPath), "Krista.Diagnostics.dll"),
                Path.Combine(Path.GetDirectoryName(_updaterPath), "Krista.Diagnostics.dll"), true);
            if (!File.Exists(Path.Combine(Path.GetDirectoryName(_updaterPath), "Krista.Diagnostics.dll")))
                Trace.TraceError("Krista.Diagnostics не скопирована во времнную папку!");

            using (var writer = new BinaryWriter(File.Open(_updaterPath, FileMode.Create)))
                writer.Write(Resources.UpdaterFM);
        }

        private static ProcessModule GetProcessModule(Process process)
        {
            try
            {
                foreach (ProcessModule module in
                    process.Modules.Cast<ProcessModule>().Where(
                        module => module.ModuleName == "Krista.FM.Update.Framework.dll"))
                {
                    return module;
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }

            return process.MainModule;
        }
    }
}