using System;
using System.IO;
using System.Threading;

namespace Krista.FM.Update.ColdUpdater.Actions
{
    internal sealed class FileCopyAction : IUpdateAction 
    {
        private readonly string source;
        private readonly string dest;

        public FileCopyAction(string _source, string _destination)
        {
            source = _source;
            dest = _destination;
        }

        #region IUpdateAction Members

        public bool Do()
        {
            // для замены должны быть права на удаление
            //bool canWriteToFolder = Framework.Utils.PermissionsCheck.HaveWritePermissionsForFolder(dest);

            //if (canWriteToFolder)
            //{
            try
            {
                if (File.Exists(dest))
                {
                    int retries = 60;
                    bool access = false;
                    while (!access && retries > 0)
                    {
                        try
                        {
                            Trace.TraceError(String.Format("Удаляем файл {0}", dest));
                            File.Delete(dest);
                            access = true;
                        }
                        catch (UnauthorizedAccessException)
                        {
                            // Если проиизошло исключение, возникающее в случае запрета доступа из-за ввода вывода или особого типа ошибки безопасности,
                            // то усыпляем выполнение потока и повторяем ошибку
                            retries--;
                            Trace.TraceInformation(String.Format("Ждем - {0}", --retries));
                            Thread.Sleep(1000);
                        }
                    }

                    if (!access)
                        throw new Exception("Не доступа к файлу " + dest + " для его удаления");
                }

                if (!Directory.Exists(Path.GetDirectoryName(dest)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(dest));
                }

                DateTime time = File.GetLastWriteTime(source);
                Trace.TraceInformation(String.Format("Переносим из {0} в {1}", source, dest));
                File.Move(source, dest);
                File.SetLastWriteTime(dest, time);
            }
            catch(FileNotFoundException e)
            {
                Trace.TraceError(String.Format("Файл не найден: {0}", e.Message));
                // если не нашли может его и не должно там быть
                return true;
            }
            catch (IOException e)
            {
                Trace.TraceError(String.Format("Ошибка ввода-вывода: {0}. Параметры: Source - {1}, Dest - {2}",
                                               e.Message, source, dest));
                throw new UnauthorizedAccessException(e.Message);
            }
            //}
           // else
            //{
                //throw new UnauthorizedAccessException("No write permission available on the file");
            //}
            return true;
        }

        public void Rollback(string backupPath)
        {
            try
            {
                if (File.Exists(dest))
                {
                    int retries = 5;
                    bool access = false;

                    while (!access && retries > 0)
                    {
                        try
                        {
                            File.Delete(dest);
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
                        throw new Exception("Не доступа к файлу " + dest + " для его удаления");
                }

                if (File.Exists(backupPath))
                {
                    try
                    {
                        Trace.Indent();
                        Trace.TraceInformation(String.Format("Отказываем {0}", backupPath));

                        DateTime time = File.GetLastWriteTime(backupPath);
                        File.Move(backupPath, dest);
                        File.SetLastWriteTime(dest, time);
                    }
                    finally
                    {
                        Trace.Unindent();
                    }
                }

                if (Directory.GetFiles(Path.GetDirectoryName(backupPath)).Length == 0)
                {
                    Directory.Delete(Path.GetDirectoryName(backupPath));
                }
            }
            catch (FileNotFoundException e)
            {
                Trace.TraceError(String.Format("Файл не найден: {0}", e.Message));
                // если не нашли может его и не должно там быть
                return;
            }
            catch (IOException e)
            {
                Trace.TraceError(String.Format("Ошибка при откате: {0}", e.Message));
                throw new UnauthorizedAccessException(e.Message);
            }
        }

        #endregion
    }
}

