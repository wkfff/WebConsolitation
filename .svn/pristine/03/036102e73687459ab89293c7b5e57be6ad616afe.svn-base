using System;
using System.Diagnostics;
using System.IO;

namespace Krista.FM.Update.Framework
{
    /// <summary>
    /// Вспомогательные статические методы для работы с архивами
    /// </summary>
    public class ArchiveHelper
    {
        /// <summary>
        /// Добавить каталог а архив
        /// </summary>
        /// <param name="arcPath">Полный путь к используемому архиватору(7za.exe)</param>
        /// <param name="dirName">Директория для упаковки в архив</param>
        /// <param name="arcName">Имя архива</param>
        /// <param name="outputDir">Директория куда выложить архив</param>
        public static void CompressDir(string arcPath, string dirName, string arcName, string outputDir)
        {
            string path = Path.Combine(outputDir, arcName);
            string args = String.Format("a -tzip \"{0}\" \"{1}\\*\"", path, dirName);

            Process arc = new Process
                              {
                                  StartInfo =
                                      {
                                          FileName = arcPath,
                                          Arguments = args,
                                          WindowStyle = ProcessWindowStyle.Hidden,
                                          UseShellExecute = false,
                                          CreateNoWindow = true,
                                          RedirectStandardOutput = true,
                                          RedirectStandardError = true
                                      }
                              };
            try
            {
                arc.Start();

                //
                // Read in all the text from the process with the StreamReader.
                //
                using (StreamReader reader = arc.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
                arc.WaitForExit();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Ошибка при создании архива {0}: {1}", arcName, ex.Message);
            }
            finally
            {
                arc.Close();
            }
        }

        /// <summary>
        /// Извлечь в каталог
        /// </summary>
        /// <param name="arcPath">Полный путь к используемому архиватору(7za.exe)</param>
        /// <param name="arcName">Имя архива </param>
        /// <param name="outputDir">Директория для извлечения</param>
        public static void ExtractToDir(string arcPath, string arcName, string outputDir)
        {
            string args = String.Format("x -aoa -o\"{0}\" \"{1}\"", outputDir, arcName);
            Trace.TraceVerbose("Аргументы для архиватора: {0}", args);

            Process arc = new Process
                              {
                                  StartInfo =
                                      {
                                          FileName = arcPath,
                                          Arguments = args,
                                          WindowStyle = ProcessWindowStyle.Hidden,
                                          UseShellExecute = false,
                                          CreateNoWindow = true,
                                          RedirectStandardOutput = true,
                                          RedirectStandardError = true
                                      }
                              };
            try
            {
                arc.Start();

                //
                // Read in all the text from the process with the StreamReader.
                //
                using (StreamReader reader = arc.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Trace.TraceInformation(result);
                }
                arc.WaitForExit();
            }
            catch(Exception ex)
            {
                Trace.TraceError("Ошибка при извлечении архива {0}: {1}", arcName, ex.Message);
            }
            finally
            {
                arc.Close();
            }
        }
    }
}
