using System;
using System.Diagnostics;
using System.IO;

namespace Krista.FM.Update.Framework.UpdateObjects.Tasks
{
    /// <summary>
    /// Задача на выполнение файла (.exe, .bat)
    /// </summary>
    [Serializable]
    public sealed class FileExecuteTask : UpdateTask
    {
        public FileExecuteTask(IUpdatePatch owner) : base(owner)
        {
        }

        public override bool Prepare(IUpdateSource source)
        {
            if (!Attributes.ContainsKey("updateTo"))
            {
                Trace.TraceError("В канале обновления не указан файл для выполнения");
                IsPrepared = PrepareState.PrepareWithError;
                return false;
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

            if (IsPrepared == PrepareState.PrepareWithWarning)
            {
                Trace.TraceError(
                    String.Format(
                        "Задача {0} из патча {1} не выполнена, потому что для неё операция подготовки завершилась с предупреждениями",
                        this.GetType().Name,
                        Owner.Name));
                return ExecuteState.ExecuteWithWarning;
            }

            Trace.TraceInformation(String.Format("Начало выполнения задачи {0}", this.GetType().Name));

            string tempFile = Path.Combine(
                            Path.GetDirectoryName(UpdateManager.GetProcessModule().FileName),
                            Attributes["updateTo"]);
            Trace.TraceInformation(String.Format("Файл для выполнения: {0}", tempFile));

            try
            {
                ProcessStartInfo procInfo = new ProcessStartInfo
                                                {
                                                    FileName = tempFile,
                                                    UseShellExecute = false,
                                                    WindowStyle = ProcessWindowStyle.Hidden,
                                                    Verb = "open"
                                                };

                Process process = Process.Start(procInfo);  //Start that process.
                process.WaitForExit();
                Trace.TraceInformation(String.Format("Задача {0} выполнена успешно!", this.GetType().Name));
            }
            catch (Exception e)
            {
                Trace.TraceError(
                        String.Format("В процессе выполнения файла {0} воникло исключение : {1}", tempFile, e.Message));
                return ExecuteState.ExecuteWithError;
            }

            return ExecuteState.ExecuteSuccess;
        }

        public override int OrderByFactor
        {
            get
            {
                return 2;
            }
        }
    }
}
