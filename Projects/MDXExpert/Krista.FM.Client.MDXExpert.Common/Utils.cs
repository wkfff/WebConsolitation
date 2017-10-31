using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml;

using Infragistics.Win.UltraWinToolbars;
using Infragistics.Shared;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Common.Xml;
using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Client.MDXExpert.Common
{
    public struct CommonUtils
    {
        //Separator дл€ методов ArrayToString and ArrayFromString
        public const char separator = ';';

        /// <summary>
        /// «на€ ширину строки, вычисл€ем ее высоту, с учетом шрифта...
        /// </summary>
        /// <param name="g"></param>
        /// <param name="measuredString"></param>
        /// <param name="font"></param>
        /// <param name="rectangleWidth"></param>
        /// <returns>int(¬ысота строки)</returns>
        public static int GetStringHeight(Graphics g, string measuredString, Font font, int rectangleWidth)
        {
            SizeF sizeF = g.MeasureString(measuredString, font, rectangleWidth);
            Size rect = Size.Round(sizeF);
            return rect.Height;
        }

        /// <summary>
        /// ”знаем ширину строки
        /// </summary>
        /// <param name="g"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <returns>int(Ўирина строки)</returns>
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
        /// —охран€ет позиции селсета в указаный в строке файл
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
                System.Windows.Forms.MessageBox.Show(@"≈сли вы выдите это сообщение, обратитесь к разработчику,
                    т.к. он забыл удалить код, используемый дл€ отладки");
            }
        }

        /// <summary>
        ///  онвертирует массив чисел в строку
        /// </summary>
        /// <param name="array">ћассив целых чисел</param>
        /// <param name="separator">—имвол разделител€</param>
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
        /// ѕарсит строку (по separator) и возвращает массив целых чисел содержащихс€ в ней
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
        /// ѕолучить изображение указанной области экрана
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
        /// «аточено под корректное выставление значени€ полю, в котором должно содержатьс€ целочисленное
        /// значени€ в указанном диапазоне, если newValue не удовлетвор€ет хот€ бы одному требованию, будет 
        /// возбуждено исключение c текстом указанным в параметре "exceptionText"
        /// </summary>
        /// <param name="newValue">новое значение</param>
        /// <param name="oldValidValue">последнее правильное значение</param>
        /// <param name="lowerBound">нижний предел значени€</param>
        /// <param name="upperBound">верхний предел значени€</param>
        /// <param name="isExitEdiValue">если выходим из режима редактировани€</param>
        /// <param name="sException">текст исключени€</param>
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
                //если выходим из режима редактировани€ с ошибкой, выставим значению контрола, 
                //последнее правильное... »наче не будем этого делать, тем самым дадим пользователю возможность
                //исправить введенное значени
                if (isExitEditMode)
                    outValue = oldValidValue.ToString();

                FormException.ShowErrorForm(new Exception(exceptionText),
                    ErrorFormButtons.WithoutTerminate);
            }

            return result;
        }

        /// <summary>
        /// —оздает новый Bitmap и копирует в него содержимое Graphics
        /// </summary>
        /// <param name="graphicsSource"></param>
        /// <param name="x">X координата левого верхнего угла</param>
        /// <param name="y">Y координата левого верхнего угла</param>
        /// <param name="width">Ўирина копируемой области</param>
        /// <param name="height">¬ысота копируемой области</param>
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
    }
}