using System;
using System.IO;
using System.Windows.Forms;

namespace Krista.FM.Server.DataPumps.DataAccess
{

    /// <summary>
    /// Класс для работы с RTF-файлами
    /// </summary>
    public class RtfHelper
    {

        /// <summary>
        /// Имя Rtf-файла
        /// </summary>
        private string filename;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="filename">Имя Rtf-файла</param>
        public RtfHelper(string filename)
        {
            this.filename = filename;
        }

        /// <summary>
        /// Получить текстовое представление
        /// </summary>
        /// <param name="rtfText">Содержимое RTF-файла</param>
        /// <returns></returns>
        public string GetPlainText()
        {
            RichTextBox rtBox = new RichTextBox();
            using (StreamReader reader = new StreamReader(filename))
            {
                rtBox.Rtf = reader.ReadToEnd();
            }
            return rtBox.Text;
        }

    }

}
