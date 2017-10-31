using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Krista.FM.Common.OfficeHelpers
{
    /// <summary>
    /// ������� ����������� ����� ��� ������ � ��������� MS Office.
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
                "��� ����������� �������� ���������� ��� ��������� ����� \"{0}\".", fileName));
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
        /// ������� �� ������� ProgId � ���������� ����� ������.
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
        /// ������� �� ������� ProgId � ���������� ����� ������.
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
        /// ������� ������ MS Office.
        /// </summary>
        /// <param name="progId">Prog ID �������.</param>
        /// <param name="connectToExisting"></param>
        /// <returns>������ MS Office.</returns>
        private static object GetOfficeObject(string progId, bool connectToExisting)
        {
            object obj;
            // ���� ����� ����������� � ������������� ������� - ���� ��� � ROT
            if (connectToExisting)
            {
                try
                {
                    // ����� ������ ������, �������� �� ����������
                    obj = Marshal.GetActiveObject(progId);
                    return obj;
                }
                catch
                {
                }
            }

            // ���� �� ���������� - ������� ����� ���������
            // �������� ��� �������
            Type objectType = Type.GetTypeFromProgID(progId);

            // ���� �� ������� - ���������� ���������� �� ������
            if (objectType == null)
                throw new Exception("���������� ������� ������ " + progId);
            // ������� ������ MS Office
            obj = Activator.CreateInstance(objectType);
            // ���������� ��������� ��� ���������� ������
            return obj;
        }
    }
}
