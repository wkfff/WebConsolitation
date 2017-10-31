using System;
using System.Collections.Generic;
using System.Text;

using Infragistics.Win;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert.Common.Converts
{
    public struct AlignConvertor
    {
        /// <summary>
        /// Конвертор значений горизонтального выравнивания инфрагистика в екселевкий формат
        /// </summary>
        /// <param name="infragisticsHAlign"></param>
        /// <returns></returns>
        static public Excel.XlHAlign ToExcelHAlign(HAlign infragisticsHAlign)
        {
            switch (infragisticsHAlign)
            {
                case HAlign.Center: return Excel.XlHAlign.xlHAlignCenter;
                case HAlign.Left: return Excel.XlHAlign.xlHAlignLeft;
                case HAlign.Right: return Excel.XlHAlign.xlHAlignRight;
            }
            return Excel.XlHAlign.xlHAlignLeft;
        }

        /// <summary>
        /// Конвертор значений вертикального выравнивания инфрагистика в екселевкий формат
        /// </summary>
        /// <param name="infragisticsVAlign"></param>
        /// <returns></returns>
        static public Excel.XlVAlign ToExcelVAlign(VAlign infragisticsVAlign)
        {
            switch (infragisticsVAlign)
            {
                case VAlign.Bottom: return Excel.XlVAlign.xlVAlignBottom;
                case VAlign.Middle: return Excel.XlVAlign.xlVAlignCenter;
                case VAlign.Top: return Excel.XlVAlign.xlVAlignTop;
            }
            return Excel.XlVAlign.xlVAlignTop;
        }

        /// <summary>
        /// Конвертор значений выравнивания из инфрагистиковского в строковое
        /// </summary>
        /// <param name="ingragisticsVAlign"></param>
        /// <returns></returns>
        static public StringAlignment ToStringAlignment(VAlign infragisticsVAlign)
        {
            switch (infragisticsVAlign)
            {
                case VAlign.Bottom: return StringAlignment.Far;
                case VAlign.Middle: return StringAlignment.Center;
                case VAlign.Top: return StringAlignment.Near;
            }
            return StringAlignment.Near;
        }

        /// <summary>
        /// Конвертор значений выравнивания из инфрагистиковского в строковое
        /// </summary>
        /// <param name="ingragisticsHAlign"></param>
        /// <returns></returns>
        static public StringAlignment ToStringAlignment(HAlign infragisticsHAlign)
        {
            switch (infragisticsHAlign)
            {
                case HAlign.Right: return StringAlignment.Far;
                case HAlign.Center: return StringAlignment.Center;
                case HAlign.Left: return StringAlignment.Near;
            }
            return StringAlignment.Near;
        }

        /// <summary>
        /// Конвертор значений выравнивания из строкового в инфрагистиковский
        /// </summary>
        /// <param name="stringAlignment"></param>
        /// <returns></returns>
        static public HAlign ToHAlign(StringAlignment stringAlignment)
        {
            switch (stringAlignment)
            {
                case StringAlignment.Center: return HAlign.Center;
                case StringAlignment.Far: return HAlign.Right;
                case StringAlignment.Near: return HAlign.Left;
            }
            return HAlign.Default;
        }

        /// <summary>
        /// Конвертор значений выравнивания из строкового в инфрагистиковский
        /// </summary>
        /// <param name="stringAlignment"></param>
        /// <returns></returns>
        static public VAlign ToVAlign(StringAlignment stringAlignment)
        {
            switch (stringAlignment)
            {
                case StringAlignment.Center: return VAlign.Middle;
                case StringAlignment.Far: return VAlign.Bottom;
                case StringAlignment.Near: return VAlign.Top;
            }
            return VAlign.Default;
        }
    }
}
