using System;
using System.IO;
using System.Threading;
using Krista.FM.Update.Framework.Utils;

namespace Krista.FM.Update.Framework.UpdateObjects.Tasks
{
    /// <summary>
    /// Задача на удаление файла/каталога
    /// </summary>
    internal sealed class FileDeleteTask : FileTask
    {
        public FileDeleteTask(IUpdatePatch owner) : base(owner)
        {
        }

        public override bool Prepare(IUpdateSource source)
        {
            if (!Attributes.ContainsKey("localPath"))
            {
                IsPrepared = PrepareState.PrepareWithError;
                return false;
            }

            GetDestinationFile();

            if (File.Exists(DestinationFile))
            {
                // добавляем, чтобы до обновления знать будет ли перезапущено приложение
                if (!Attributes.ContainsKey("apply") ||
                    (Attributes.ContainsKey("apply") && "app-restart".Equals(Attributes["apply"])))
                {
                    UpdateManager.Instance.ExecuteOnAppRestart[
                        string.Format("{0}:{1}", Owner.Name, Attributes["localPath"])] = String.Empty;
                }

                // Есть файл, который необходимо удалить
                IsPrepared = PrepareState.PrepareSuccess;
                return true;
            }

            return false;
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

                        Trace.TraceInformation(String.Format(
                            "Задача {0} по удалению файла из {1} из патча {2} выполнена!",
                            GetType().Name,
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
                        String.Format("Не удалось удалить файл : {0}", ex.Message));
                    return ExecuteState.ExecuteWithError;
                }

            }

            else if (!Attributes.ContainsKey("apply") ||
                     (Attributes.ContainsKey("apply") && "app-restart".Equals(Attributes["apply"])))
            {
                // Для "холодного" обновления в backup копирует сам Updater, для возможности корректного отката 
                // патчей с одними и теми же модулями.
                // файловые задачи кэшируем для обновления 
                FileDeleteTask fut = this;
                // Для удаляемого файла не нужен tempfile, по этому признаку и будем определять то, что задачу надо создать на удаление
                UpdateManager.Instance.ExecuteOnAppRestart[string.Format("{0}:{1}", Owner.Name, Attributes["localPath"])
                    ] = String.Empty;
            }

            return ExecuteState.ExecuteSuccess;
        }
    }

}
