using System;
using System.IO;
using System.Windows.Forms;

namespace Krista.FM.Update.Framework.UpdateObjects.Tasks
{
    /// <summary>
    /// Общий класс для работы с файлами
    /// </summary>
    [Serializable]
    public class FileTask : UpdateTask 
    {
        public FileTask(IUpdatePatch owner) : base(owner)
        {
        }

        /// <summary>
        /// Абсолютный путь к целевому файлу
        /// </summary>
        protected string DestinationFile { get; set; }

        protected void CopyToBackupfolderForRollback()
        {
            if (File.Exists(DestinationFile) && !UpdateManager.Instance.IsServerMode)
            {
                string backupPath =
                    Path.Combine(
                        Path.Combine(UpdateManager.Instance.BackupFolder,
                                     UpdateManager.Instance.GetApplicationVersion()), Owner.Name);
          
                // сохраняем предыдуще версии файлов для возможности отката);)
                if (!Directory.Exists(backupPath))
                {
                    Directory.CreateDirectory(backupPath);
                }

                if (
                    !Directory.Exists(
                        Path.GetDirectoryName(Path.Combine(backupPath, Path.GetFileName(Attributes["localPath"])))))
                {
                    Directory.CreateDirectory(
                        Path.GetDirectoryName(Path.Combine(backupPath, Path.GetFileName(Attributes["localPath"]))));
                }

                // копируем с перезаписью существующих файлов!
                string backupFileName = Path.Combine(backupPath, Path.GetFileName(Attributes["localPath"]));
                DateTime time = File.GetLastWriteTime(DestinationFile);
                File.Copy(DestinationFile, backupFileName, true);
                File.SetLastWriteTime(backupFileName, time);

                Trace.TraceInformation(
                    String.Format(
                        "Файл {0} скопирован в папку {1} для возможность rollback",
                        DestinationFile,
                        backupPath));
            }
        }

        public override bool Rollback()
        {
            try
            {
                GetDestinationFile();

                Trace.TraceVerbose(DestinationFile);
                if (string.IsNullOrEmpty(DestinationFile))
                {
                    return true;
                }

                // холодная замена
                if (!Attributes.ContainsKey("apply") ||
                    (Attributes.ContainsKey("apply") && "app-restart".Equals(Attributes["apply"])))
                {
                    UpdateManager.Instance.ExecuteOnAppRestart[Attributes["localPath"]] = Path.Combine(
                        Path.Combine(Path.Combine(UpdateManager.Instance.BackupFolder, UpdateManager.Instance.GetApplicationVersion()), Owner.Name),
                                     Attributes["localPath"]);
                }
                else
                {
                    // горячая замена
                    // Копирует из Backup Folder в локальную папку в случае неудачного обновления
                    if (File.Exists(DestinationFile))
                    {
                        File.Delete(DestinationFile);
                    }

                    File.Copy(
                        Path.Combine(
                            Path.Combine(Path.Combine(UpdateManager.Instance.BackupFolder,
                                             UpdateManager.Instance.GetApplicationVersion()), Owner.Name),
                            Path.GetFileName(Attributes["localPath"])),
                        DestinationFile);
                }
            }
            catch (ArgumentNullException e)
            {
                throw new RollbackException(e.Message);
            }
            catch (ArgumentException e)
            {
                throw new RollbackException(e.Message);
            }
            catch (IOException e)
            {
                throw new RollbackException(e.Message);
            }

            return true;
        }

        #region Helper Methods

        /// <summary>
        /// 
        /// </summary>
        protected void GetDestinationFile()
        {
            try
            {
                if (UpdateManager.Instance.IsServerMode && UpdateManager.InstallerFeed.Equals(Owner.Feed.Name))
                {
                    DestinationFile =
                        Path.Combine(
                            Path.GetDirectoryName(UpdateManager.GetProcessModule().FileName),
                            Attributes["localPath"]);
                }
                else
                {
                    DestinationFile = Path.Combine(
                        UpdateManager.Instance.DestBaseUrl,
                        Attributes["localPath"]);
                }
            }
            catch (ArgumentException e)
            {
                Trace.TraceError(e.Message);
            }
        }

        #endregion
    }
}
