using System;
using System.IO;
using System.Threading;

namespace Krista.FM.Update.ColdUpdater.Actions
{
    /// <summary>
    /// Действие по удалению файла
    /// </summary>
	internal sealed class FileDeleteActions : IUpdateAction
	{
        private readonly string _destination;

        public FileDeleteActions(string _destination)
        {
            this._destination = _destination;
        }

	    public bool Do()
	    {
            try
            {
                if (File.Exists(_destination))
                {
                    int retries = 60;
                    bool access = false;
                    while (!access && retries > 0)
                    {
                        try
                        {
                            File.Delete(_destination);
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
                        throw new Exception("Не доступа к файлу " + _destination + " для его удаления");
                }

            }
            catch (FileNotFoundException)
            {
                // если не нашли может его и не должно там быть
                return true;
            }
            catch (IOException e)
            {
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
                if (File.Exists(_destination))
                {
                    int retries = 5;
                    bool access = false;

                    while (!access && retries > 0)
                    {
                        try
                        {
                            File.Delete(_destination);
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
                        throw new Exception("Не доступа к файлу " + _destination + " для его удаления");
                }

                if (File.Exists(backupPath))
                {
                    DateTime time = File.GetLastWriteTime(backupPath);
                    File.Move(backupPath, _destination);
                    File.SetLastWriteTime(_destination, time);
                }

                if (Directory.GetFiles(Path.GetDirectoryName(backupPath)).Length == 0)
                {
                    Directory.Delete(Path.GetDirectoryName(backupPath));
                }
            }
            catch (FileNotFoundException)
            {
                // если не нашли может его и не должно там быть
                return;
            }
            catch (IOException e)
            {
                throw new UnauthorizedAccessException(e.Message);
            }
	    }
	}
}
