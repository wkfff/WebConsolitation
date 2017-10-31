using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

using Microsoft.Win32;
using System.Drawing;
using Microsoft.WindowsCE.Forms;
using OpenNETCF.Windows.Forms;

namespace Krista.FM.Client.iMonitoringWM.Common
{
    static public class Utils
    {

        /// <summary>
        /// Пустая ли группа
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        static public bool IsGroupEmpty(Group group)
        {
            return ((group == null) || ((group != null) && (group.Value == string.Empty)));
        }

        #region Работа с uri
        /// <summary>
        /// Комбинируем Uri
        /// </summary>
        /// <param name="uri1"></param>
        /// <param name="uri2"></param>
        /// <returns></returns>
        public static string CombineUri(string uri1, string uri2)
        {
            return Path.Combine(uri1, uri2).Replace('\\', '/');
        }

        public static string GetUriWithoutQuery(string uriStr)
        {
            uriStr = GetUriWithoutParams(uriStr);
            //если последняя точка находится дальше слэша, значит обрежим имя страницы, 
            //оставим только путь
            int pointIndex = uriStr.LastIndexOf('.');
            int slashIndex = uriStr.LastIndexOf('/');
            if (pointIndex > slashIndex)
            {
                uriStr = uriStr.Remove(slashIndex + 1, uriStr.Length - slashIndex - 1);
            }
            return uriStr;
        }

        public static string GetUriWithoutParams(string uriStr)
        {
            int queryMarkIndex = uriStr.LastIndexOf('?');
            if (queryMarkIndex > 0)
            {
                uriStr = uriStr.Remove(queryMarkIndex, uriStr.Length - queryMarkIndex);
            }
            return uriStr;
        }
        #endregion

        #region Работа с файлами
        /// <summary>
        /// Перемещает директорию, отличительная черта от Directory.Move(), что может
        /// копировать саму в себя
        /// </summary>
        /// <param name="sourceDirName">что копируем</param>
        /// <param name="destDirName">куда коприруем</param>
        /// <param name="isRemoveSourceDir">удалить ли исходную директорию</param>
        static public void MoveDirectory(string sourceDirName, string destDirName, bool isRemoveSourceDir)
        {
            if (Directory.Exists(destDirName))
                Directory.Delete(destDirName, true);
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
        /// Записываем в файл текст
        /// </summary>
        /// <param name="path">путm</param>
        /// <param name="text">текст</param>
        static public void WriteAllText(string path, string text)
        {
            try
            {
                using (FileStream fs = File.Open(path, FileMode.Create, FileAccess.ReadWrite))
                {
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.Write(text);
                        sw.Close();
                        sw.Dispose();
                    }
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch (Exception e)
            {
                Messages.ShowError(e.Message, "Ошибка");
            }
        }

        /// <summary>
        /// Путь к кэшу
        /// </summary>
        /// <param name="startupPath">стартовый путь приложения</param>
        /// <param name="place">расположение кэша</param>
        /// <returns></returns>
        static public string GetCachePath(string startupPath, CachePlace place)
        {
            string cachePath = Path.Combine(startupPath, Consts.cacheFolderName);
            string storeCardPath = "\\" + Utils.GetStorageCardName();

            if (Utils.IsExistStorageCard())
            {
                switch (place)
                {
                    case CachePlace.storageCard:
                        {
                            if (!cachePath.StartsWith(storeCardPath))
                                cachePath = storeCardPath + cachePath;
                            break;
                        }
                    case CachePlace.storagePhone:
                        {
                            if (cachePath.StartsWith(storeCardPath))
                                cachePath = cachePath.Replace(storeCardPath, string.Empty);
                            break;
                        }
                }
            }

            return cachePath;
        }

        static public string GetStorageCardName()
        {
            if (Directory.Exists(Consts.storageCardName))
                return Consts.storageCardName;
            else
                if (Directory.Exists(Consts.storageCardRusName))
                    return Consts.storageCardRusName;
                else
                    if (Directory.Exists(Consts.storageSDCardName))
                        return Consts.storageSDCardName;
            return string.Empty;
        }

        static public bool IsExistStorageCard()
        {
            return GetStorageCardName() != string.Empty;
        }
        #endregion

        #region Работа со строками
        /// <summary>
        /// Обрезает строку с конца, начиная с указаного символа
        /// </summary>
        /// <param name="sourceString">исходная строка</param>
        /// <param name="cuttingChar">символ начиная с которого режем</param>
        /// <param name="isCutWithChar">образать вместе с cuttingChar</param>
        /// <returns></returns>
        static public string CutEndString(string sourceString, char cuttingChar, bool isCutWithChar)
        {
            if (!string.IsNullOrEmpty(sourceString))
            {
                int cuttingCharIndex = sourceString.LastIndexOf(cuttingChar);
                if (cuttingCharIndex > 0)
                {
                    if (!isCutWithChar)
                        cuttingCharIndex++;
                    sourceString = sourceString.Remove(cuttingCharIndex, sourceString.Length - cuttingCharIndex);
                }
            }
            return sourceString;
        }

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
        /// Вернет размеры строки
        /// </summary>
        /// <param name="str"></param>
        /// <param name="gr"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public static SizeF GetStringSize(string str, Graphics gr, Font font)
        {
            return gr.MeasureString(str, font);
        }

        /// <summary>
        /// Обрезает строку до заданной ширины
        /// </summary>
        /// <param name="sourceStr">строку которую надо обрезать</param>
        /// <param name="sourceStrWidth">ширина исходной строки</param>
        /// <param name="needStrWidth">ширина обрезанной строки</param>
        /// <param name="endCutString">окончание обрезанной строки</param>
        /// <returns></returns>
        public static string CutString(string sourceStr, float sourceStrWidth, float cutStrWidth,
            string cutStringEnd)
        {
            string cutString = sourceStr;

            //если требуется обрезать
            if (sourceStrWidth > cutStrWidth)
            {
                //количество пикселей занимаемое одним символом
                float charWidth = sourceStrWidth / sourceStr.Length;
                //количество пикселей требующие обрезание
                float widthDifference = sourceStrWidth - cutStrWidth;
                //количество символов требующих обрезание
                int redundantCharCount = (int)(widthDifference / charWidth + cutStringEnd.Length);
                cutString = sourceStr.Substring(0, sourceStr.Length - redundantCharCount) + cutStringEnd;
            }
            return cutString;
        }
        #endregion

        #region Работа с контролами
        /// <summary>
        /// Выравнивает контрол по середине родительского
        /// </summary>
        /// <param name="source">контрол</param>
        /// <param name="parent">контрол по которому надо выравнять</param>
        static public void AlignByCentr(Control source, Control parent)
        {
            if ((source != null) && (parent != null))
            {
                int x = (int)(((float)parent.Width / 2f) - ((float)source.Width / 2f));
                int y = (int)(((float)parent.Height / 2f) - ((float)source.Height / 2f));
                source.Location = new Point(x, y);
            }
        }

        /// <summary>
        /// Устанавливает всем элементам коллекции признак отрисовки контролов
        /// </summary>
        /// <param name="value">если true - отрисовываем, иначе нет</param>
        /// <param name="isRecurse">устнавливать рекурсивно</param>
        static public void SetControlsLayout(System.Windows.Forms.Control.ControlCollection controls, bool value,
            bool isRecurse)
        {
            if (controls != null)
            {
                foreach (Control item in controls)
                {
                    if (value)
                        item.ResumeLayout(false);
                    else
                        item.SuspendLayout();
                    if (isRecurse)
                        SetControlsLayout(item.Controls, value, isRecurse);
                }
            }
        }

        /// <summary>
        /// Устанавливает всей коллекции контролов указанный стиль
        /// </summary>
        /// <param name="controls">коллекция контролов</param>
        /// <param name="style">устанавливаемый стиль</param>
        /// <param name="isRecurse">устанавливать рекурсивно</param>
        static public void SetControlsAnchor(System.Windows.Forms.Control.ControlCollection controls,
            AnchorStyles style, bool isRecurse)
        {
            if (controls != null)
            {
                foreach (Control item in controls)
                {
                    item.Anchor = style;
                    if (isRecurse)
                        SetControlsAnchor(item.Controls, style, isRecurse);
                }
            }
        }
        #endregion

        #region Работа с экраном
        /// <summary>
        /// Размера экрана
        /// </summary>
        static public ScreenSizeMode ScreenSize
        {
            get
            {
                return GetScreenSizeMode(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            }
        }

        /// <summary>
        /// Получаем стандарт размера экрана
        /// </summary>
        /// <param name="width">цирина</param>
        /// <param name="height">высота</param>
        /// <returns></returns>
        static public ScreenSizeMode GetScreenSizeMode(int width, int height)
        {
            //если ориентация горизонтальная, поменяем размеры местами
            if (width > height)
            {
                int temp = height;
                height = width;
                width = temp;
            }

            switch (width)
            {
                case 240:
                    {
                        switch (height)
                        {
                            case 320: return ScreenSizeMode.s240x320;
                        }
                        break;
                    }
                case 480:
                    {
                        switch (height)
                        {
                            case 640: return ScreenSizeMode.s480x640;
                            case 800: return ScreenSizeMode.s480x800;
                        }
                        break;
                    }
            }
            return ScreenSizeMode.unsupportedSize;
        }

        static public Rectangle GetScreenBounds()
        {
            Rectangle result = Screen.PrimaryScreen.Bounds;
            switch (ScreenSize)
            {
                case ScreenSizeMode.s480x800:
                    {
                        if ((SystemSettings.ScreenOrientation == ScreenOrientation.Angle0) ||
                            (SystemSettings.ScreenOrientation == ScreenOrientation.Angle180))
                        {
                            result = new Rectangle(Screen.PrimaryScreen.WorkingArea.X, Screen.PrimaryScreen.WorkingArea.Y,
                                Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                        }
                        break;
                    }
            }
            return result;
        }

        static public Bitmap ScreenShot(Graphics gx, Rectangle rect)
        {
            Bitmap result = new Bitmap(rect.Width, rect.Height);
            using (Graphics g = Graphics.FromImage(result))
            {
                Win32Helper.BitBlt(g.GetHdc(), rect.Left, rect.Top, rect.Width, rect.Height, gx.GetHdc(), rect.Left, rect.Top, Win32Helper.SRCCOPY);
            }
            return result;
        }
        #endregion

        #region Работа с реестром
        public static string GetRegValue(RegistryKey ownKey, string subKey, string value, 
            string defaultValue)
        {
            string result = defaultValue;
            try
            {
                RegistryKey reg = ownKey.OpenSubKey(subKey);
                if (reg != null)
                {
                    result = reg.GetValue(value).ToString();
                    reg.Close();
                }
            }
            catch
            {
            }
            return result;
        }

        public static void SetRegValue(RegistryKey ownKey, string subKey, string propertiesName, 
            object value)
        {
            try
            {
                RegistryKey reg = ownKey.OpenSubKey(subKey, true);
                if (reg != null)
                {
                    reg.SetValue(propertiesName, value);
                    reg.Close();
                }
            }
            catch
            {
            }
        }
        #endregion

        #region Работа с клавиатурой
        /// <summary>
        /// Вычисляем нажатую кнопку, учитывая ориентацию устройства.
        /// </summary>
        /// <param name="keyData">код нажатой кнопки</param>
        /// <returns></returns>
        static public HardwareButtons GetHardwareButton(KeyData keyData)
        {
            HardwareButtons result = HardwareButtons.None;
            switch (keyData.KeyCode)
            {
                case 13: { result = HardwareButtons.Center; break; }
                case 37: { result = HardwareButtons.Left; break; }
                case 38: { result = HardwareButtons.Up; break; }
                case 39: { result = HardwareButtons.Right; break; }
                case 40: { result = HardwareButtons.Bottom; break; }
            }
            return result;
        }
        #endregion
    }
}
