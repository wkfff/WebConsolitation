using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Drawing;
using System.Xml;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Shared;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Common.Xml;
using Microsoft.AnalysisServices.AdomdClient;
using Microsoft.Win32;
using System.Windows.Forms;
using ADODB;

namespace Krista.FM.Client.MDXExpert.Common
{
    public struct CommonUtils
    {
        //Separator для методов ArrayToString and ArrayFromString
        public const char separator = ';';

        /// <summary>
        /// Ассоциация разрешения отчетов с экспертом
        /// </summary>
        /// <param name="executablePath"></param>
        static public void AssociationReportExtension(string executablePath)
        {
            try
            {
                RegistryKey rootKey = Registry.ClassesRoot;
                //Удалим старые значения из реестра
                rootKey.DeleteSubKey(Consts.reportExt, false);
                RegistryKey keyForDelete = null;
                keyForDelete = rootKey.OpenSubKey(Consts.applicationNameRegKey);
                if (keyForDelete != null)
                {
                    rootKey.DeleteSubKeyTree(Consts.applicationNameRegKey);
                    keyForDelete = null;
                }
                keyForDelete = rootKey.OpenSubKey(Consts.autoExtensionKey);
                if (keyForDelete != null)
                {
                    rootKey.DeleteSubKeyTree(Consts.autoExtensionKey);
                    keyForDelete = null;
                }

                RegistryKey extensionKey = rootKey.CreateSubKey(Consts.reportExt);
                extensionKey.SetValue(string.Empty, Consts.applicationNameRegKey);
                extensionKey.SetValue("Content Type", Consts.mimeType);



                RegistryKey applNameKey = rootKey.CreateSubKey(Consts.applicationNameRegKey);
                applNameKey.SetValue(string.Empty, Consts.reportInform);
                RegistryKey tempKey = applNameKey.CreateSubKey("shell");
                tempKey = tempKey.CreateSubKey("open");
                tempKey = tempKey.CreateSubKey("command");
                //Приложение которое будет запускаться при двойнем клике по отчету
                tempKey.SetValue(string.Empty, string.Format("\"{0}\" \"{1}\"", executablePath, "%1"));

                tempKey.Close();
                applNameKey.Close();
                extensionKey.Close();
                rootKey.Close();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Показывает дружественную форму с исключением, далее действует по ситуации,
        /// либо продолжаем работу, либо закрываем приложение
        /// </summary>
        /// <param name="e"></param>
        public static void ProcessException(Exception e)
        {
            ProcessException(e, false);
        }

        /// <summary>
        /// Показывает дружественную форму с исключением, далее действует по ситуации,
        /// либо продолжаем работу, либо закрываем приложение
        /// </summary>
        /// <param name="e"></param>
        /// <param name="needKillProcess">true - для насильственного убийства приложения :). 
        /// Нужен когда Application.Exit() не работает (например во время создания главной формы)
        /// </param>
        public static void ProcessException(Exception e, bool needKillProcess)
        {
            ErrorFormResult res = FormException.ShowErrorForm(e);
            switch (res)
            {
                case ErrorFormResult.efrClose:
                    if (needKillProcess)
                    {
                        Process.GetCurrentProcess().Kill();
                    }
                    else
                    {
                        Application.Exit();
                    }
                    break;
                case ErrorFormResult.efrRestart:
                    Application.Restart();
                    break;
            }
        }


        #region Работа со строками
        /// <summary>
        /// Зная ширину строки, вычисляем ее высоту, с учетом шрифта...
        /// </summary>
        /// <param name="g"></param>
        /// <param name="measuredString"></param>
        /// <param name="font"></param>
        /// <param name="rectangleWidth"></param>
        /// <returns>int(Высота строки)</returns>
        public static int GetStringHeight(Graphics g, string measuredString, Font font, int rectangleWidth)
        {
            SizeF sizeF = g.MeasureString(measuredString, font, rectangleWidth);
            Size rect = Size.Round(sizeF);
            return rect.Height;
        }

        /// <summary>
        /// Узнаем ширину строки
        /// </summary>
        /// <param name="g"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <returns>int(Ширина строки)</returns>
        public static float GetStringWidth(Graphics g, string text, Font font)
        {
            return g.MeasureString(text, font).Width;
        }

        //получить максимальную ширину из представленных строк
        public static int GetStringMaxWidth(Graphics graphics, string[] strings, Font font)
        {
            int result = 0;
            foreach (string str in strings)
            {
                int width = (int)graphics.MeasureString(str, font).Width + 1;
                result = Math.Max(result, width);
            }
            return result;
        }

        //получить ширину строк
        public static int[] GetStringWidths(Graphics graphics, string[] strings, Font font)
        {
            int[] result = new int[strings.Length];
            for (int i = 0; i < strings.Length; i++)
            {
                result[i] = (int)graphics.MeasureString(strings[i], font).Width + 1;
            }
            return result;
        }

        /// <summary>
        /// Конвертирует массив чисел в строку
        /// </summary>
        /// <param name="array">Массив целых чисел</param>
        /// <param name="separator">Символ разделителя</param>
        /// <returns></returns>
        public static string ArrayToString(int[] array, char separator)
        {
            StringBuilder result = new StringBuilder();
            foreach (int item in array)
            {
                result.Append(item.ToString() + separator);
            }
            if (result.Length > 0)
                result[result.Length - 1] = ' ';
            return result.ToString().Trim();
        }

        /// <summary>
        /// Парсит строку (по separator) и возвращает массив целых чисел содержащихся в ней
        /// </summary>
        /// <param name="source">строка с числами</param>
        /// <param name="separator">разделитель</param>
        /// <returns></returns>
        public static int[] ArrayFromString(string source, char separator)
        {

            List<int> intArray = new List<int>();
            string[] strArray = source.Split(separator);
            int i;
            foreach (string item in strArray)
            {
                if (item == string.Empty)
                    continue;

                if (Int32.TryParse(item, out i))
                {
                    intArray.Add(i);
                }
                else
                {
                    intArray.Add(0);
                }
            }
            return intArray.ToArray();
        }

        /// <summary>
        /// Из полного пути, получить имя файла
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        static public string SeparateFileName(string fullFileName, bool isIncludeExtension)
        {
            if (fullFileName == string.Empty)
                return string.Empty;
            string[] strArray = fullFileName.Split('\\');
            string fileName = strArray[strArray.Length - 1];
            if (!isIncludeExtension && (fileName.LastIndexOf('.') != -1))
                fileName = fileName.Remove(fileName.LastIndexOf('.'));
            return fileName;
        }

        static public string SetExtension(string fileName, string extension)
        {
            if (extension == string.Empty)
                return fileName;

            //удаляем старое расширение
            if (fileName.LastIndexOf('.') != -1)
                fileName = fileName.Remove(fileName.LastIndexOf('.'));
            //возвращаем новое
            return fileName + extension;
        }

        static private string CutPart(ref string tail, string separator)
        {
            string result = "0";
            if (!string.IsNullOrEmpty(tail))
            {
                int p = tail.IndexOf(separator);
                if (p == -1)
                {
                    result = tail;
                    tail = String.Empty;
                }
                else
                {
                    result = tail.Substring(0, p);
                    tail = tail.Remove(0, p + separator.Length);
                }
            }
            return result;
        }

        #endregion

        /// <summary>
        /// Сохраняет позиции селсета в указаный в строке файл
        /// </summary>
        /// <param name="poses"></param>
        /// <param name="path"></param>
        public static void WritePoses(PositionCollection poses, string path)
        {
            try
            {
                List<string> str = new List<string>();
                int posNum = 0;
                foreach (Position pos in poses)
                {
                    //str.Add(posNum.ToString().PadRight(40, '-'));
                    //str.Add(string.Empty.PadRight(40, '-'));
                    //int memNum = 0;
                    string s = string.Empty; ;
                    foreach (Member mem in pos.Members)
                    {
                        s += string.Format("{0} | {1}    ||   ", mem.LevelDepth, mem.UniqueName);
                        //str.Add("    " + memNum + "  " + mem.UniqueName);
                        //str.Add(string.Format("    {0} | {1}", mem.LevelDepth, mem.UniqueName));
                        // memNum++;
                    }
                    str.Add(s);
                    //str.Add(string.Empty.PadRight(40, '-'));
                    //str.Add(string.Empty);
                    posNum++;
                }

                System.IO.File.WriteAllLines(path, str.ToArray());
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show(@"Если вы выдите это сообщение, обратитесь к разработчику,
                    т.к. он забыл удалить код, используемый для отладки");
            }
        }

        /// <summary>
        /// Заточено под корректное выставление значения полю, в котором должно содержаться целочисленное
        /// значения в указанном диапазоне, если newValue не удовлетворяет хотя бы одному требованию, будет 
        /// возбуждено исключение c текстом указанным в параметре "exceptionText"
        /// </summary>
        /// <param name="newValue">новое значение</param>
        /// <param name="oldValidValue">последнее правильное значение</param>
        /// <param name="lowerBound">нижний предел значения</param>
        /// <param name="upperBound">верхний предел значения</param>
        /// <param name="isExitEdiValue">если выходим из режима редактирования</param>
        /// <param name="sException">текст исключения</param>
        /// <param name="outValue">значение контрола</param>
        /// <returns></returns>
        static public bool IsValidIntValue(string newValue, int oldValidValue, int lowerBound, int upperBound,
            bool isExitEditMode, string exceptionText, out string outValue)
        {
            bool result = true;
            outValue = newValue;

            int i;
            if (!int.TryParse(newValue, out i) || !(i >= lowerBound) || !(i <= upperBound))
            {
                result = false;
                //если выходим из режима редактирования с ошибкой, выставим значению контрола, 
                //последнее правильное... Иначе не будем этого делать, тем самым дадим пользователю возможность
                //исправить введенное значени
                if (isExitEditMode)
                    outValue = oldValidValue.ToString();

                FormException.ShowErrorForm(new Exception(exceptionText),
                    ErrorFormButtons.WithoutTerminate);
            }

            return result;
        }

        #region Работа с изображением
        /// <summary>
        /// Получить изображение указанной области экрана
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static Bitmap GetBitMap(Rectangle bounds)
        {

            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);

            using (Graphics bmpGraphics = Graphics.FromImage(bitmap))
            {
                bmpGraphics.CopyFromScreen(bounds.X, bounds.Y, 0, 0, new Size(bounds.Width, bounds.Height));
            }

            return bitmap;
        }

        /// <summary>
        /// Создает новый Bitmap и копирует в него содержимое Graphics
        /// </summary>
        /// <param name="graphicsSource"></param>
        /// <param name="x">X координата левого верхнего угла</param>
        /// <param name="y">Y координата левого верхнего угла</param>
        /// <param name="width">Ширина копируемой области</param>
        /// <param name="height">Высота копируемой области</param>
        static public Bitmap GetBitmapFromGraphics(Graphics graphicsSource, int x, int y, int width, int height)
        {
            IntPtr hDCSource = graphicsSource.GetHdc();
            IntPtr hDCDestination = Win32.CreateCompatibleDC(hDCSource);
            IntPtr hBitmap = Win32.CreateCompatibleBitmap(hDCSource, width, height);
            IntPtr hPreviousBitmap = Win32.SelectObject(hDCDestination, hBitmap);
            Win32.BitBlt(hDCDestination, 0, 0, width, height, hDCSource, x, y, Win32.SRCCOPY);
            Bitmap bitmap = Bitmap.FromHbitmap(hBitmap);
            Win32.DeleteObject(hDCDestination);
            Win32.DeleteObject(hBitmap);
            graphicsSource.ReleaseHdc(hDCSource);
            return bitmap;
        }
        #endregion

        static public VersionRelation GetVersionRelation(string reportVersionStr, string comparedVersionStr)
        {
            int partCount = Consts.versionPartNumber;
            int[] reportVersion = new int[partCount];
            int[] comparedVersion = new int[partCount];

            int i;
            for (i = 0; i < partCount; i++)
            {
                Int32.TryParse(CutPart(ref reportVersionStr, "."), out reportVersion[i]);
                Int32.TryParse(CutPart(ref comparedVersionStr, "."), out comparedVersion[i]);
            }

            i = 0;

            while (i < partCount)
            {
                if (reportVersion[i] < comparedVersion[i])
                {
                    return VersionRelation.Ancient;
                }
                else
                {
                    if (reportVersion[i] > comparedVersion[i])
                    {
                        return VersionRelation.Future;
                    }
                }
                i++;
            }
            return VersionRelation.Modern;
        }

        //получение списка баз с сервера
        static public List<string> GetCatalogList(string connStr)
        {

            ADODB.Connection con = new ADODB.Connection();
            ADODB.Recordset catalogRecordset = null;
            List<string> result = new List<string>();

            try
            {
                con.Open(connStr, "", "", -1);

                if (con.State == (int)ADODB.ObjectStateEnum.adStateOpen)
                {
                    catalogRecordset = con.OpenSchema(SchemaEnum.adSchemaCatalogs, Missing.Value, Missing.Value);

                    while (!catalogRecordset.EOF)
                    {
                        result.Add(catalogRecordset.Fields["CATALOG_NAME"].Value.ToString());
                        catalogRecordset.MoveNext();
                    }

                }
            }
            finally
            {
                if (catalogRecordset != null)
                {
                    if (catalogRecordset.State == (int)ADODB.ObjectStateEnum.adStateOpen)
                    {
                        catalogRecordset.Close();
                    }
                    catalogRecordset = null;
                }

                if (con != null)
                {
                    if (con.State == (int)ADODB.ObjectStateEnum.adStateOpen)
                    {
                        con.Close();
                    }
                    con = null;
                }

            }
            return result;
        }

        /// <summary>
        /// Принадлежит ли объект сету
        /// </summary>
        /// <param name="set">сет</param>
        /// <param name="value">объект</param>
        /// <returns></returns>
        static public bool IsInSet(object[] set, object value)
        {
            if ((set != null) && (value != null))
            {
                foreach (object setItem in set)
                {
                    if (setItem.ToString() == value.ToString())
                        return true;
                }
            }
            return false;
        }


        /// <summary>
        /// установить точность числа
        /// </summary>
        /// <param name="number">число</param>
        /// <param name="digitCount">кол-во знаков после запятой</param>
        /// <returns>число с заданной точностью</returns>
        public static double SetNumberPrecision(double number, int digitCount)
        {
            try
            {
                number = Math.Round(number, digitCount);
                /*
                number *= Math.Pow(10, digitCount);
                number = Math.Truncate(number);
                number /= Math.Pow(10, digitCount);*/
                return number;
            }
            catch
            {
                return number;
            }
        }

        /// <summary>
        /// Скопировать папку со всем содержимым
        /// </summary>
        /// <param name="src">папка, которую копируем</param>
        /// <param name="dst">папка, в которую копируем</param>
        public static void CopyDirectory(string src, string dst)
        {
            try
            {
                String[] Files;
                if (dst[dst.Length - 1] != Path.DirectorySeparatorChar)
                    dst += Path.DirectorySeparatorChar;
                if (!Directory.Exists(dst)) Directory.CreateDirectory(dst);
                Files = Directory.GetFileSystemEntries(src);
                foreach (string element in Files)
                {
                    if (Directory.Exists(element))
                        CopyDirectory(element, dst + Path.GetFileName(element));
                    else
                        File.Copy(element, dst + Path.GetFileName(element), true);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Получить описание значения перечислимого типа (строковое значение аттрибута [Description])
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumValueDescription(Type enumType, object value)
        {
            FieldInfo fi = value.GetType().GetField(Enum.GetName(enumType, value));
            DescriptionAttribute dna =
              (DescriptionAttribute)Attribute.GetCustomAttribute(
                fi, typeof(DescriptionAttribute));

            if (dna != null)
                return dna.Description;
            else
                return value.ToString();

        }

        /// <summary>
        /// Получение заголовка элемента без ID
        /// </summary>
        /// <param name="mbr">элемент</param>
        /// <returns></returns>
        static public string GetMemberCaptionWithoutID(Member mbr)
        {
            string memCaption = mbr.Caption;
            //Если перед заголовком элемента прописан ID элемента - удаляем этот ID
            string memID = GetMemberIDFromCaption(mbr.Caption);
            if (!String.IsNullOrEmpty(memID))
            {
                mbr.FetchAllProperties();
                if ((mbr.Properties.Find("PKID") != null) && (mbr.Properties["PKID"].Value != null))
                {
                    //Если в заголовке кроме ID ничего нет,  то оставляем заголовок как есть
                    if (memCaption == mbr.Properties["PKID"].Value.ToString())
                    {
                        return memCaption;
                    }

                    if (mbr.Properties["PKID"].Value.ToString() == memID)
                    {
                        bool isDataMember = false;
                        if ((mbr.Properties.Find("IS_DATAMEMBER") != null) && (mbr.Properties["IS_DATAMEMBER"].Value != null))
                        {
                            isDataMember = (mbr.Properties["IS_DATAMEMBER"].Value.ToString().ToUpper() == "TRUE");
                        }
                        memCaption = isDataMember
                                         ? memCaption.Remove(1, memID.Length)
                                         : memCaption.Remove(0, memID.Length);
                    }
                }
            }
            return memCaption;
        }

        /// <summary>
        /// Получение ID элемента из заголовка
        /// </summary>
        /// <param name="caption">заголовок элемента</param>
        /// <returns>ID</returns>
        static public string GetMemberIDFromCaption(string caption)
        {
            string memberID = String.Empty;
            //Если кэпшн начинается с символа "(" - возможно это датамембер (через свойство IS_DATAMEMBER 
            //проверять каждый мембер ресурсоемко)
            int startMemberID = 0;
            if ((caption.Length > 0) && (caption[0] == '('))
                startMemberID = 1;

            for (int i = startMemberID; i < caption.Length; i++)
            {
                if (Char.IsNumber(caption[i]))
                {
                    memberID += caption[i];
                }
                else
                {
                    break;
                }
            }
            return memberID;
        }
    }

    //отношение между разными версиями
    public enum VersionRelation
    {
        //старее
        Ancient,
        //совпадает
        Modern,
        //новее
        Future
    }

    public enum VersionType
    {
        Release,
        Test
    }

    public interface ICompositeLegendParams
    {
        LegendLocation CompositeLegendLocation
        {
            get; set;
        }

        int CompositeLegendExtent
        {
            get; set;
        }
    }


}