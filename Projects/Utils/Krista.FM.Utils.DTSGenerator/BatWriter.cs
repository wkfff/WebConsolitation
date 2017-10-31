using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Utils.DTSGenerator
{
    public class BatWriter
    {
        private static readonly string sourceConnectionVariable = "Source";

        private static readonly string destConnectionVariable = "Dest";

        /// <summary>
        /// Заголовочная информация
        /// </summary>
        /// <returns></returns>
        public static string HeaderBat()
        {
            String batString = string.Empty;

            Encoding cyrillic = Encoding.GetEncoding(1251);

            batString += "echo off" + Environment.NewLine;
            batString += "chcp 1251" + Environment.NewLine;

            byte[] bytes = cyrillic.GetBytes("Старт переноса...");
            char[] cyrillicChars = new char[cyrillic.GetCharCount(bytes, 0, bytes.Length)];
            cyrillic.GetChars(bytes, 0, bytes.Length, cyrillicChars, 0);

            string convert = new string(cyrillicChars);

            batString += "echo " + convert + Environment.NewLine + Environment.NewLine;

            return batString;
        }

        /// <summary>
        /// Переменные
        /// </summary>
        /// <param name="batString"></param>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public static string SetVariable(string source, string dest)
        {
            String batString = string.Empty;

            batString += "Set " + sourceConnectionVariable + "=" + "\"" +  source + "\"" + Environment.NewLine;
            batString += "Set " + destConnectionVariable + "=" + "\"" + dest + "\"" + Environment.NewLine + Environment.NewLine;

            return batString;
        }

        public static string PackageTask(string fileName)
        {
            String batString = string.Empty;

            fileName = RenameFileName(fileName);

            batString += "dtexec ^" + Environment.NewLine;
            batString += " /FILE" + " \"" + fileName + "\"" + " ^" + Environment.NewLine;
            batString += " /CONNECTION OLEDBConnectionSource;" + "\"\\\"" + "%" + sourceConnectionVariable + "%" + "\\\"\"" + " ^" + Environment.NewLine;
            batString += " /CONNECTION OLEDBConnectionDest;" + "\"\\\"" + "%" + destConnectionVariable + "%" + "\\\"\"" + " ^" + Environment.NewLine;
            batString += " /MAXCONCURRENT " + "\" -1 \"" + " ^" + Environment.NewLine;
            batString += " /CHECKPOINTING OFF  ^" + Environment.NewLine;
            batString += " /REPORTING EWCDI  ^" + Environment.NewLine;
            batString += " /LOGGER \"{0A039101-ACC1-4E06-943F-279948323883}\";FileLogProviderConnection" + Environment.NewLine + Environment.NewLine;

            return batString;
        }

        private static string RenameFileName(string fileName)
        {
            if (fileName == "Системные объекты.dtsx")
                fileName = "SystemTables.dtsx";
            return fileName;
        }

        public static string EndBat()
        {
            return "pause";
        }
    }
}
