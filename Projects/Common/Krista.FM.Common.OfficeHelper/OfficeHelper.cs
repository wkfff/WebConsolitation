using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Krista.FM.Common.OfficeHelpers
{
    /// <summary>
    /// Базовый абстрактный класс для работы с объектами MS Office.
    /// </summary>
    public static class OfficeHelper
    {
        public static ExcelApplication CreateExcelApplication()
        {
            return new ExcelApplication(GetOfficeObject("Excel.Application", false));
        }

        public static WordApplication CreateWordApplication()
        {
            return new WordApplication(GetOfficeObject("Word.Application", false));
        }

        public static object CreateMsProjectApplication()
        {
            return GetOfficeObject("MSProject.Application", true);
        }

        public static OfficeApplication GetOfficeAppForFile(string fileName)
        {
            if (ExcelApplication.IsApplicableFile(fileName))
            {
                return CreateExcelApplication();
            }
            if (WordApplication.IsApplicableFile(fileName))
            {
                return CreateWordApplication();
            }
            throw new Exception(String.Format(
                "Нет подходящего офисного приложения для обработки файла \"{0}\".", fileName));
        }

        public static string GetExcelExtension()
        {
            if (GetExcelVersionNumber() < 12)
                return OfficeFileExt.ExcelDocument;
            return OfficeFileExt.ExcelDocumentX;
        }

        public static string GetWordExtension()
        {
            if (GetWordVersionNumber() < 12)
                return OfficeFileExt.WordDocument;

            return OfficeFileExt.WordDocumentX;
        }

        /// <summary>
        /// Достает из реестра ProgId и возвращает цифру версии.
        /// </summary>
        public static int GetExcelVersionNumber()
        {
            string regPath = PlatformDetect.Is64BitProcess ? "Wow6432Node\\CLSID\\{00024500-0000-0000-C000-000000000046}\\ProgID" :
                "CLSID\\{00024500-0000-0000-C000-000000000046}\\ProgID";
            RegistryKey rk = Registry.ClassesRoot.OpenSubKey(regPath);
            
            if (rk != null)
            {
                string[] keyValue = rk.GetValue("").ToString().Split('.');
                rk.Close();
                return Convert.ToInt32(keyValue[2]);
            }
            return 0;
        }

        /// <summary>
        /// Достает из реестра ProgId и возвращает цифру версии.
        /// </summary>
        public static int GetWordVersionNumber()
        {
            string regPath = PlatformDetect.Is64BitProcess ? "Wow6432Node\\CLSID\\{000209FF-0000-0000-C000-000000000046}\\ProgID" :
                "CLSID\\{000209FF-0000-0000-C000-000000000046}\\ProgID";
            RegistryKey rk = Registry.ClassesRoot.OpenSubKey(regPath);
            if (rk != null)
            {
                string[] keyValue = rk.GetValue("").ToString().Split('.');
                rk.Close();
                return Convert.ToInt32(keyValue[2]);
            }
            return 0;
        }

        /// <summary>
        /// Создать объект MS Office.
        /// </summary>
        /// <param name="progId">Prog ID объекта.</param>
        /// <param name="connectToExisting"></param>
        /// <returns>Объект MS Office.</returns>
        private static object GetOfficeObject(string progId, bool connectToExisting)
        {
            object obj;
            // если нужно подключится к существующему объекту - ищем его в ROT
            if (connectToExisting)
            {
                try
                {
                    // найти способ узнать, запущено ли приложение
                    obj = Marshal.GetActiveObject(progId);
                    return obj;
                }
                catch
                {
                }
            }

            // если не получилось - создаем новый экземпляр
            // получаем тип объекта
            Type objectType = Type.GetTypeFromProgID(progId);

            // если не удалось - генерируем исключение об ошибке
            if (objectType == null)
                throw new Exception("Невозможно создать объект " + progId);
            // создаем объект MS Office
            obj = Activator.CreateInstance(objectType);
            // возвращаем указатель для дальнейшей работы
            return obj;
        }
    }
}
