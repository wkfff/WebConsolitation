using System;
using System.IO;
using System.Threading;
using Krista.FM.Update.Framework.Utils;

namespace Krista.FM.Update.Framework.UpdateObjects.Tasks
{
    /// <summary>
    /// Задача обновления файла
    /// </summary>
    [Serializable]
    public sealed class FileUpdateTask : FileTask
    {
        #region Fields

        private string tempFile;
        
        #endregion

        #region Constructor

        public FileUpdateTask(IUpdatePatch owner)
            : base(owner)
        {
        }

        #endregion

        #region IUpdateTask Methods

        public override bool Prepare(IUpdateSource source)
        {
            if (!Attributes.ContainsKey("localPath"))
            {
                IsPrepared = PrepareState.PrepareSuccess;
                return true;
            }

            string fileName = Attributes.ContainsKey("updateTo") ? Attributes["updateTo"] : Attributes["localPath"];
            tempFile = null;

            try
            {
                string tempFileLocal = Path.Combine(UpdateManager.Instance.TempFolder, Guid.NewGuid().ToString());
                if (!source.GetData(
                    fileName,
                    string.Format("{0}\\", Path.Combine(Path.Combine(UpdateManager.Instance.SourceBaseUri, Owner.BaseUrl), Owner.Name)),
                    ref tempFileLocal, true))
                {
                    return false;
                }

                Trace.TraceVerbose(string.Format("Файл {0} скопирован во временную папку {1}", fileName, tempFileLocal));

                tempFile = tempFileLocal;
                IsPrepared = PrepareState.PrepareSuccess;

                // добавляем, чтобы до обновления знать будет ли перезапущено приложение
                if (!Attributes.ContainsKey("apply") ||
                    (Attributes.ContainsKey("apply") && "app-restart".Equals(Attributes["apply"])))
                {
                    UpdateManager.Instance.ExecuteOnAppRestart[string.Format("{0}:{1}", Owner.Name, Attributes["localPath"])] = String.Empty;
                }
            }
            catch (PreparetaskException ex)
            {
                IsPrepared = PrepareState.PrepareWithError;
                Trace.TraceError(String.Format("Ошибка при подготовке задачи к выполнению : {0}", ex.Message));
                return false;
            }

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

            if (!Attributes.ContainsKey("localPath"))
            {
                return ExecuteState.ExecuteSuccess;
            }

            GetDestinationFile();

            // В случае "горячей" замены удаляем целевой файл и переносим из временной папки
            // При обновлении на сервере "холодная" замена не нужна, но может потребоваться обновить саму 
            // прогу-обновлялку с библиотекой обновления
            if (Attributes.ContainsKey("apply") && "hot-swap".Equals(Attributes["apply"]))
            {
                try
                {
                    // Копируем заменяемый файл в Backup Folder, для возможности отката в случае неудачного обновления
                    CopyToBackupfolderForRollback();

                    // для замены должны быть права на удаление
                    bool canWriteToFolder =
                        PermissionsCheck.HaveWritePermissionsForFolder(DestinationFile);

                    if (canWriteToFolder)
                    {
                        if (File.Exists(DestinationFile))
                        {
                            int retries = 5;
                            bool access = false;

                            while (!access && retries > 0)
                            {
                                try
                                {
                                    if (File.Exists(DestinationFile))
                                    {
                                        File.Delete(DestinationFile);
                                    }

                                    access = true;
                                }
                                catch (UnauthorizedAccessException)
                                {
                                    // Если проиизошло исключение, возникающее в случае запрета доступа из-за ввода вывода или особого типа ошибки безопасности,
                                    // то усыпляем выполнение потока и повторяем ошибку
                                    retries--;
                                    Thread.Sleep(1000);
                                }
                            }

                            if (!access)
                            {
                                Trace.TraceError(
                                    string.Format(
                                        "Не доступа к файлу {0} для его удаления. Возможно он занят другим приложением",
                                        DestinationFile));
                            }
                        }

                        if (!Directory.Exists(Path.GetDirectoryName(DestinationFile)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(DestinationFile));
                        }

                        DateTime time = File.GetLastWriteTime(tempFile);
                        File.Move(tempFile, DestinationFile);
                        File.SetLastWriteTime(DestinationFile, time);

                        Trace.TraceInformation(String.Format(
                            "Задача {0} по копированию файла из {1} в {2} из патча {3} выполнена!",
                            GetType().Name,
                            tempFile,
                            DestinationFile,
                            Owner.Name));
                    }
                    else
                    {
                        throw new UnauthorizedAccessException(String.Format(
                            "Нет прав для удаления файла: {0}",
                            DestinationFile));
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(
                        String.Format("Не удалось перенести файл из временной директории : {0}", ex.Message));
                    return ExecuteState.ExecuteWithError;
                }
            }
            else if (!Attributes.ContainsKey("apply") ||
                     (Attributes.ContainsKey("apply") && "app-restart".Equals(Attributes["apply"])))
            {
                // Для "холодного" обновления в backup копирует сам Updater, для возможности корректного отката 
                // патчей с одними и теми же модулями.
                // файловые задачи кэшируем для обновления 
                FileUpdateTask fut = this;
                UpdateManager.Instance.ExecuteOnAppRestart[string.Format("{0}:{1}", Owner.Name, Attributes["localPath"])] = fut.tempFile;
            }

            return ExecuteState.ExecuteSuccess;
        }

        #endregion

    }
}
