using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Client.MDXExpert.Grid.Utils
{
    /// <summary>
    /// Утили грида
    /// </summary>
    public struct GridUtils
    {
        //Separator для методов ArrayToString and ArrayFromString
        public const char separator = ';';

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
            for(int i = 0; i < strings.Length; i++)
            {
                result[i] = (int)graphics.MeasureString(strings[i], font).Width + 1;
            }
            return result;
        }

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
    }
}
