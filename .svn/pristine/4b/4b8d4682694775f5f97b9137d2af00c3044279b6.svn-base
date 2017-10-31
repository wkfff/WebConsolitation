using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Infragistics.Win;

namespace Krista.FM.Client.MDXExpert.Common.Converts
{
    public struct UltraFontConvertor
    {
        /// <summary>
        /// Конвертирует шрифт инфрагистика в строку
        /// </summary>
        /// <param name="fontData"></param>
        /// <returns></returns>
        public static string FontDataToString(FontData fontData)
        {
            string result = string.Empty;
            if (fontData != null)
            {
                Font font = FontDataToFont(fontData);
                FontConverter fontConvertor = new FontConverter();
                result = fontConvertor.ConvertToString(font);
            }
            return result;
        }

        /// <summary>
        /// Обратное преобразование, из строки в шрифт инфрагистика
        /// </summary>
        /// <param name="sFont"></param>
        /// <returns></returns>
        public static FontData StringToFontData(string sFont)
        {
            FontData result = null;
            if (sFont != string.Empty)
            {
                FontConverter fontConvertor = new FontConverter();
                Font font = (Font)fontConvertor.ConvertFromString(sFont);

                result = FontToFontData(font);
            }
            return result;
        }

        /// <summary>
        /// Конвертирует шрифт инфрагистика в системный
        /// </summary>
        /// <param name="fontData"></param>
        /// <returns></returns>
        public static Font FontDataToFont(FontData fontData)
        {
            Font result = null;
            if (fontData != null)
            {
                FontStyle fontStyle = FontStyle.Regular;
                if (fontData.Bold == DefaultableBoolean.True)
                    fontStyle |= FontStyle.Bold;
                if (fontData.Italic == DefaultableBoolean.True)
                    fontStyle |= FontStyle.Italic;
                if (fontData.Strikeout == DefaultableBoolean.True)
                    fontStyle |= FontStyle.Strikeout;
                if (fontData.Underline == DefaultableBoolean.True)
                    fontStyle |= FontStyle.Underline;

                result = new Font(fontData.Name, fontData.SizeInPoints, fontStyle);
            }
            return result;
        }

        /// <summary>
        /// Конвертирует системый шрифт в инфрагистиковский
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        public static FontData FontToFontData(Font font)
        {
            FontData result = new FontData();
            if (font != null)
            {
                result.Name = font.Name;
                result.Bold = font.Bold ? DefaultableBoolean.True : DefaultableBoolean.Default;
                result.Italic = font.Italic ? DefaultableBoolean.True : DefaultableBoolean.Default;
                result.Strikeout = font.Strikeout ? DefaultableBoolean.True : DefaultableBoolean.Default;
                result.Underline = font.Underline ? DefaultableBoolean.True : DefaultableBoolean.Default;
                result.SizeInPoints = font.SizeInPoints;
            }
            return result;
        }

        /// <summary>
        /// Cинхронизирует значения шрифтоф
        /// </summary>
        /// <param name="clockedFontData">синхронизируемый шрифт</param>
        /// <param name="fontData">шаблон шрифта, по которому идет синхронизация</param>
        public static void SynchronizeFontData(FontData clockedFontData, FontData templateFontData)
        {
            if ((clockedFontData != null) && (templateFontData != null))
            {
                clockedFontData.Name = templateFontData.Name;
                clockedFontData.Bold = templateFontData.Bold;
                clockedFontData.Italic = templateFontData.Italic;
                clockedFontData.Strikeout = templateFontData.Strikeout;
                clockedFontData.Underline = templateFontData.Underline;
                clockedFontData.SizeInPoints = templateFontData.SizeInPoints;
            }
        }

        /// <summary>
        /// Cинхронизирует значения шрифтоф
        /// </summary>
        /// <param name="clockedFont">синхронизируемый шрифт</param>
        /// <param name="templateFont">шаблон шрифта, по которому идет синхронизация</param>
        public static void SynchronizeFont(Excel.Font clockedFont, Font templateFont)
        {
            if ((clockedFont != null) && (templateFont != null))
            {
                clockedFont.Name = templateFont.Name;
                clockedFont.Bold = templateFont.Bold;
                clockedFont.Italic = templateFont.Italic;
                clockedFont.Strikethrough = templateFont.Strikeout;
                clockedFont.Size = templateFont.SizeInPoints;
                if (templateFont.Underline)
                    clockedFont.Underline = Excel.XlUnderlineStyle.xlUnderlineStyleSingle;
            }
        }
    }
}
