using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Krista.FM.Client.MobileReports.Common
{
    static public class Utils
    {
        /// <summary>
        /// Ищет в исходном тексте, вхождение любой строки из массива
        /// </summary>
        /// <param name="source">исходный текст</param>
        /// <param name="text">массив с искомыми значениями</param>
        /// <returns></returns>
        static public bool IsExistText(string source, string[] text)
        {
            bool result = false;
            if (string.IsNullOrEmpty(source))
                return result;

            foreach (string item in text)
            {
                if (source.Contains(item))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// В тексте заменяет все старые комбинации строк на новые
        /// </summary>
        /// <param name="text"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        static public string ReplaceAll(string text, string oldValue, string newValue)
        {
            string result = text;
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            while (result.Contains(oldValue))
            {
                result = result.Replace(oldValue, newValue);
            }
            return result;
        }

        /// <summary>
        /// Обрезает у Path последний слеш, если он есть
        /// </summary>
        /// <returns></returns>
        static public string PathWithoutLastSlash(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;
            return path.TrimEnd('\\');
        }

        /// <summary>
        /// Пустая ли группа
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        static public bool IsGroupEmpty(Group group)
        {
            return ((group == null) || ((group != null) && (group.Value == string.Empty)));
        }

        /// <summary>
        /// Перемещает директорию, отличительная черта от Directory.Move(), что может
        /// копировать саму в себя
        /// </summary>
        /// <param name="sourceDirName">что копируем</param>
        /// <param name="destDirName">куда коприруем</param>
        /// <param name="isRemoveSourceDir">удалить ли исходную директорию</param>
        static public void MoveDirectory(string sourceDirName, string destDirName, bool isRemoveSourceDir)
        {
            Directory.CreateDirectory(destDirName);
            DirectoryInfo sourceDir = new DirectoryInfo(sourceDirName);

            //скопируем все директории из исходной директории
            foreach (DirectoryInfo movingDir in sourceDir.GetDirectories())
            {
                if (movingDir.FullName != destDirName)
                    Directory.Move(movingDir.FullName, Path.Combine(destDirName, movingDir.Name));
            }

            //скопируем все файлы из исходной директории
            foreach (FileInfo movingFile in sourceDir.GetFiles())
            {
                File.Move(movingFile.FullName, Path.Combine(destDirName, movingFile.Name));
            }

            if (isRemoveSourceDir)
                Directory.Delete(sourceDirName, true);
        }

        /// <summary>
        /// Получить полный путь к папке с ресурсами (если запущено из IIS, он меняется)
        /// </summary>
        /// <param name="startupPath"></param>
        /// <param name="isWorkFromAsp"></param>
        /// <returns></returns>
        static public string GetResorcePath(string startupPath, bool isWorkFromAsp)
        {
            string result = startupPath;
            if (isWorkFromAsp)
                result = Path.Combine(result, Consts.aspWorkDir);
            return Path.Combine(result, Consts.resourceFolderName);
        }

        /// <summary>
        /// Путь сохранения отчетов
        /// </summary>
        static public string GetReportsSavePath(MobileReportsSnapshotMode snapshotMode, string dataBurstSavePath)
        {
            //путь сохранениея отчетов
            string reportsSavePath = Path.Combine(dataBurstSavePath, Consts.reportsBurstName);
            //исходя из режима в котором генерим отчеты, будем раскладывать их в разные папки
            if (snapshotMode == MobileReportsSnapshotMode.New)
                reportsSavePath = Path.Combine(reportsSavePath, Consts.newSnapshotModeName);
            else
                reportsSavePath = Path.Combine(reportsSavePath, Consts.oldSnapshotModeName);
            return Utils.PathWithoutLastSlash(reportsSavePath);
        }

        /// <summary>
        /// Возвращаем хэш код коллекции 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        static public int GetHashCode(List<int> list)
        {
            int result = 0;
            foreach (int itemList in list)
            {
                result += itemList.GetHashCode();
            }
            return result.GetHashCode();
        }
    }
}
