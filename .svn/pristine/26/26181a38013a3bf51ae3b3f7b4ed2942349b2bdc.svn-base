using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;


namespace Krista.FM.Client.ViewObjects.DataPumpUI
{
	public static class FileManager
	{
        #region Константы

        internal const int DRIVE_UNKNOWN = 0;
        internal const int DRIVE_NO_ROOT_DIR = 1;
        internal const int DRIVE_REMOVABLE = 2;
        internal const int DRIVE_FIXED = 3;
        internal const int DRIVE_REMOTE = 4;
        internal const int DRIVE_CDROM = 5;
        internal const int DRIVE_RAMDISK = 6;

        internal const int GW_CHILD = 5;

        internal const int WM_COMMAND = 0x0111;

        #endregion Константы


        #region Методы класса

        /// <summary>
        /// Затемняет цвет на указанную величину
        /// </summary>
        /// <param name="color">Цвет</param>
        /// <param name="amount">Величина затемнения</param>
        /// <returns>Затемненный цвет</returns>
        internal static Color DarkenColor(Color color, int amount)
        {
            return Color.FromArgb(
              color.A,
              Math.Max(color.R - amount, 0),
              Math.Max(color.G - amount, 0),
              Math.Max(color.B - amount, 0));
        }

        /// <summary>
        /// Возвращает строку с информацией о файле
        /// </summary>
        internal static string FileSizeFromFileInfo(FileInfo fileInfo)
        {
            decimal fileSize = 0;
            if (fileInfo.Length > 0)
            {
                fileSize = Convert.ToDecimal(fileInfo.Length / 1000.0);
                fileSize = Math.Round(fileSize);
                fileSize = Math.Max(1, fileSize);
            }

            int fileSizeInt = Convert.ToInt32(fileSize);

            return fileSizeInt.ToString("###,###,##0") + " KB";
        }

        /// <summary>
        /// Удаляет путь из полного имени файла
        /// </summary>
        /// <param name="fullyQualifedName"></param>
        /// <returns></returns>
        public static string RemovePath(string fullyQualifedName)
        {
            // Возвращаем все символы после последнего слэша
            int lastSlashIndex = fullyQualifedName.LastIndexOf("\\");

            if (lastSlashIndex == -1)
                return fullyQualifedName;
            else
                return fullyQualifedName.Substring(lastSlashIndex + 1);
        }

        /// <summary>
        /// Удаляет путь из полного имени файла.
        /// Если если это корневой каталог, то возвращаем 'Локальный диск (X:)', где 'X' - имя диска.
        /// </summary>
        /// <param name="fullyQualifedName">Путь</param>
        /// <returns>Преобразованный путь</returns>
        public static string RemovePathEx(string fullyQualifedName)
        {
            if (fullyQualifedName.Length == 3 &&
                fullyQualifedName.Substring(1, 1) == ":" &&
                fullyQualifedName.Substring(2, 1) == "\\")
                return "Локальный диск (" + fullyQualifedName.Substring(0, 2) + ")";

            return RemovePath(fullyQualifedName);
        }

        #endregion Методы класса


        #region Функции WinAPI

        /// <summary>
        /// Возвращает тип диска (локальный, сд, флоп и т.п.)
        /// </summary>
        /// <param name="path">Путь</param>
        /// <returns>Тип диска</returns>
        internal static int GetDriveTypeApi(string path)
        {
            IntPtr lpString = Marshal.StringToCoTaskMemAnsi(path);

            int driveType = GetDriveType(lpString);

            Marshal.FreeCoTaskMem(lpString);

            return driveType;
        }

        [DllImport("Kernel32.dll", EntryPoint = "GetDriveType")]
        private static extern int GetDriveType(IntPtr lpRootPathName);

        /// <summary>
        /// Возвращает имя диска
        /// </summary>
        /// <param name="path">Путь</param>
        /// <returns>Наименование диска</returns>
        internal static string GetDriveNameApi(string path)
        {
            string lpszVolumeName = string.Empty;
            int lpVolumeSerialNumber = -1;
            int lpMaximumComponentLength = -1;
            int lpFileSystemFlags = -1;
            string lpFileSystemNameBuffer = string.Empty;

            /*bool result =*/ GetVolumeInformation(path, lpszVolumeName, 50, lpVolumeSerialNumber,
                lpMaximumComponentLength, lpFileSystemFlags, lpFileSystemNameBuffer, 100);

            return lpszVolumeName;
        }

        [DllImport("Kernel32.dll", EntryPoint = "GetVolumeInformationW")]
        private static extern bool GetVolumeInformation(string lpRootPathName, string lpVolumeNameBuffer,
            int nVolumeNameSize, int lpVolumeSerialNumber, int lpMaximumComponentLength,
            int lpFileSystemFlags, string lpFileSystemNameBuffer, int nFileSystemNameSize);

        /// <summary>
        /// Устанавливает тип представления папки
        /// </summary>
        /// <param name="hwnd">Дескриптор окна браузера</param>
        /// <param name="folderViewMode">Тип предствления</param>
        public static int SetFolderView(IntPtr hwnd, FolderViewMode folderViewMode)
        {
            IntPtr child = GetWindow(hwnd, GW_CHILD);
            IntPtr handle = FindWindowEx(child, IntPtr.Zero, "SHELLDLL_DefView", string.Empty);
            return SendMessage(handle, WM_COMMAND, (int)0x7028 + (int)folderViewMode, 0);
        }

        [DllImport("User32.dll", EntryPoint="SendMessageW")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wparam, int lparam);

        [DllImport("User32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr parent, IntPtr child, string className, string window);

        [DllImport("User32.dll", EntryPoint = "GetWindow")]
        private static extern IntPtr GetWindow(IntPtr hwnd, int cmd);

        #endregion Функции WinAPI
    }


    /// <summary>
    /// Тип представления каталога
    /// </summary>
    public enum FolderViewMode
    {
        FVM_FIRST = 1,
        FVM_ICON = 1,
        FVM_SMALLICON = 2,
        FVM_LIST = 3,
        FVM_DETAILS = 4,
        FVM_THUMBNAIL = 5,
        FVM_TILE = 6,
        FVM_THUMBSTRIP = 7,
        FVM_LAST = 7
    }
}
