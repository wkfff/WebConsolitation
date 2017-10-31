using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Krista.FM.Common
{
    /// <summary>
    /// Наименование архиватора
    /// </summary>
    internal enum ArchivatorName
    {
        Arj,
        Zip,
        Rar
    }

    /// <summary>
    /// Настройки процесса распаковки файлов
    /// </summary>
    internal enum FilesExtractingOption
    {
        /// <summary>
        /// Распаковывать все архивы в один каталог
        /// </summary>
        SingleDirectory,

        /// <summary>
        /// Распаковывать каждый архив в отдельный подкаталог
        /// </summary>
        SeparateSubDirs
    }

    public class Arj32Helper
    {
        #region Функции для работы с архивными файлами

        /// <summary>
        /// Исполняет файл программы с заданными аргументами в сеансе ДОС
        /// </summary>
        /// <param name="settingsFileName">Файл</param>
        /// <param name="arguments">Аргументы</param>
        /// <param name="output">Это то, что сказал ДОС в ответ на команду</param>
        /// <returns>Строка ошибки</returns>
        private static string ExecuteDOSCommand(string fileName, string arguments, out string output)
        {
            Process prc = null;
            output = string.Empty;

            try
            {
                // Устанавливаем параметры запуска процесса
                prc = new Process();
                prc.StartInfo.FileName = fileName;
                prc.StartInfo.Arguments = arguments;
                prc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                prc.StartInfo.UseShellExecute = false;
                prc.StartInfo.CreateNoWindow = true;
                prc.StartInfo.RedirectStandardOutput = false;

                // Старт
                prc.Start();

                // Ждем пока процесс не завершится
                prc.WaitForExit();
                //output = prc.StandardOutput.ReadToEnd();

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (prc != null) prc.Close();
            }
        }

        /// <summary>
        /// Распаковывает файл ARJ
        /// </summary>
        /// <param name="file">Файл архива</param>
        /// <param name="outDir">Каталог куда распаковывать (пустая строка - текущий каталог)</param>
        /// <param name="output">Это то, что сказал ДОС в ответ на команду</param>
        /// <returns>Строка ошибки</returns>
        private static string ExtractARJ(string file, string outDir, out string output)
        {
            output = string.Empty;
            string result = string.Empty;

            // Путь, где сейчас находится архиватор
            string arjCurrentPath = GetCurrentDir().FullName + "\\ARJ32.EXE";

            try
            {
                // Проверяем, на месте ли архиватор
                if (!File.Exists(arjCurrentPath))
                {
                    return "Файл ARJ32.EXE не найден.";
                }

                // Создаем каталог для разорхивации
                if (outDir != string.Empty)
                {
                    Directory.CreateDirectory(outDir);
                }

                // Формируем строку параметров для архиватора
                string arcParams = string.Format("x \"{0}\" \"{1}\" -u -v -y", file, outDir);

                // Вызываем архиватор
                result = ExecuteDOSCommand(arjCurrentPath, arcParams, out output);

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Распаковывает архивный файл
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="destDir">Каталог куда распаковывать (пустая строка - текеущий каталог)</param>
        /// <returns>Временный каталог</returns>
        public static string ExtractArchiveFile(string file, string destDir)
        {
            string str = string.Empty;
            string err = string.Empty;

            err = ExtractARJ(file, destDir, out str);
 
            if (err != string.Empty)
            {
                throw new Exception(
                    string.Format("Ошибка при разорхивировании файла {0}: {1}.", file, err));
            }

            return destDir;
        }

        /// <summary>
        /// упаковываем файл в архив arj, который будет находиться там же, где и сам файл
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string ArchiveFile(string file)
        {
            string output;
            string result = string.Empty;

            // Путь, где сейчас находится архиватор
            string arjCurrentPath = GetCurrentDir().FullName + "\\ARJ32.EXE";

            try
            {
                // Проверяем, на месте ли архиватор
                if (!File.Exists(arjCurrentPath))
                {
                    return "Файл ARJ32.EXE не найден.";
                }

                string archName = file.Replace(Path.GetFileName(file), Path.GetFileNameWithoutExtension(file));
                // Формируем строку параметров для архиватора
                string arcParams = string.Format("A -e \"{0}\" \"{1}\"", archName, file); 
                // Вызываем архиватор
                result = ExecuteDOSCommand(arjCurrentPath, arcParams, out output);

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Текущий каталог
        /// </summary>
        /// <returns>Текущий каталог</returns>
        private static DirectoryInfo GetCurrentDir()
        {
            return new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory;
        }

        #endregion Функции для работы с архивными файлами
    }
}
