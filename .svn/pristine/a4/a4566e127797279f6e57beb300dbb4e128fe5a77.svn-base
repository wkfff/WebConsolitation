using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Krista.FM.Server.DataPumps.DataAccess
{

    /// <summary>
    /// Формат файла, в который можно сохранить документ Word
    /// </summary>
    public enum WdSaveFormat
    {
        wdFormatDocument = 0,
        wdFormatDOSText = 4,
        wdFormatDOSTextLineBreaks = 5,
        wdFormatUnicodeText = 7,
        wdFormatFilteredHTML = 10,
        wdFormatHTML = 8,
        wdFormatRTF = 6,
        wdFormatTemplate = 1,
        wdFormatText = 2,
        wdFormatTextLineBreaks = 3,
        wdFormatWebArchive = 9,
        wdFormatXML = 11
    }

    /// <summary>
    /// Класс для работы с MS Word
    /// </summary>
    public class WordHelper
    {

        #region Поля

        private object wordObj = null;
        private object documents = null;
        private object document = null;

        #endregion Поля

        #region Константы

        private const string PROG_ID = "Word.Application";
        // Формат сохраняемого файла Rtf
        private const int WD_SAVE_FORMAT_RTF = 6;

        #endregion Константы

        #region Конструктор

        public WordHelper()
        {
            try
            {
                Type objectType = Type.GetTypeFromProgID(PROG_ID);
                wordObj = Activator.CreateInstance(objectType);
            }
            catch
            {
                // если не удалось - генерируем исключение об ошибке
                throw new Exception("Невозможно создать объект " + PROG_ID);
            }
        }

        #endregion Конструктор

        #region Свойства

        /// <summary>
        /// Отображать окно Word
        /// </summary>
        public bool Visible
        {
            set
            {
                wordObj.GetType().InvokeMember(
                    "Visible", BindingFlags.SetProperty, null, wordObj, new object[] { value });
            }
        }

        #endregion Свойства

        #region Работа с документом

        /// <summary>
        /// Открыть Word-документ
        /// </summary>
        /// <param name="filename">Имя файла</param>
        public void OpenDocument(string filename)
        {
            documents = wordObj.GetType().InvokeMember(
                "Documents", BindingFlags.GetProperty, null, wordObj, null);
            document = documents.GetType().InvokeMember(
                "Open", BindingFlags.InvokeMethod, null, documents, new object[] { filename, false });
        }

        /// <summary>
        /// Сохранить документ как...
        /// </summary>
        /// <param name="filename">Имя файла</param>
        /// <param name="saveFormat">Формат файла</param>
        public void SaveAs(string filename, WdSaveFormat saveFormat)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            document.GetType().InvokeMember(
                "SaveAs", BindingFlags.InvokeMethod, null, document, new object[] { filename, saveFormat });
        }

        /// <summary>
        /// Закрыть документ
        /// </summary>
        public void CloseDocument()
        {
            try
            {
                documents.GetType().InvokeMember(
                    "Close", BindingFlags.InvokeMethod, null, documents, null);
                Marshal.ReleaseComObject(document);
                Marshal.ReleaseComObject(documents);
                wordObj.GetType().InvokeMember(
                    "Quit", BindingFlags.InvokeMethod, null, wordObj, null);
                Marshal.ReleaseComObject(wordObj);
                GC.GetTotalMemory(true);
            }
            catch
            {
            }
        }

        #endregion Работа с документом

    }

}
