using System;
using System.Collections.Generic;
using System.Drawing;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Krista.FM.Common.Consolidation.Forms.Layout
{
    [CLSCompliant(false)]
    public static class NPOIHelper
    {
        private static Dictionary<short, string> styleAlign = new Dictionary<short, string>
        {
            { HSSFCellStyle.ALIGN_CENTER, "center" },
            { HSSFCellStyle.ALIGN_CENTER_SELECTION, "center" },
            { HSSFCellStyle.ALIGN_FILL, "justify" },
            { HSSFCellStyle.ALIGN_GENERAL, "left" },
            { HSSFCellStyle.ALIGN_JUSTIFY, "justify" },
            { HSSFCellStyle.ALIGN_LEFT, "left" },
            { HSSFCellStyle.ALIGN_RIGHT, "right" }
        };

        private static Dictionary<short, string> styleAlignVertical = new Dictionary<short, string>
        {
            { HSSFCellStyle.VERTICAL_BOTTOM, "bottom" },
            { HSSFCellStyle.VERTICAL_CENTER, "middle" },
            { HSSFCellStyle.VERTICAL_JUSTIFY, "baseline" },
            { HSSFCellStyle.VERTICAL_TOP, "top" }
        };

        private static Dictionary<short, string> styleBorder = new Dictionary<short, string>
        {
            { HSSFCellStyle.BORDER_DASHED, "dotted" },
            { HSSFCellStyle.BORDER_DASH_DOT, "dotted" },
            { HSSFCellStyle.BORDER_DASH_DOT_DOT, "dotted" },
            { HSSFCellStyle.BORDER_DOTTED, "dotted" },
            { HSSFCellStyle.BORDER_DOUBLE, "double" },
            { HSSFCellStyle.BORDER_HAIR, "solid" },
            { HSSFCellStyle.BORDER_MEDIUM, "solid" },
            { HSSFCellStyle.BORDER_MEDIUM_DASHED, "dotted" },
            { HSSFCellStyle.BORDER_MEDIUM_DASH_DOT, "dotted" },
            { HSSFCellStyle.BORDER_MEDIUM_DASH_DOT_DOT, "dotted" },
            { HSSFCellStyle.BORDER_NONE, "none" },
            { HSSFCellStyle.BORDER_SLANTED_DASH_DOT, "dotted" },
            { HSSFCellStyle.BORDER_THICK, "solid" },
            { HSSFCellStyle.BORDER_THIN, "solid" },
        };

        private static Dictionary<FontStyle, string> styleFontStyle = new Dictionary<FontStyle, string>
        {
            { FontStyle.Bold, "bold" },
            { FontStyle.Italic, "italic" },
            { FontStyle.Regular, "regular" },
            { FontStyle.Strikeout, "strikeout" },
            { FontStyle.Underline, "underline" },
        };

        private static Dictionary<short, string> styleColor = new Dictionary<short, string>
        {
            { 65, "white" },
            { HSSFColor.AQUA.index, "aqua" },
            { HSSFColor.AUTOMATIC.index, "white" },
            { HSSFColor.BLACK.index, "black" },
            { HSSFColor.BLUE.index, "blue" },
            { HSSFColor.BLUE_GREY.index, String.Empty },
            { HSSFColor.BRIGHT_GREEN.index, String.Empty },
            { HSSFColor.BROWN.index, "brown" },
            { HSSFColor.CORAL.index, "coral" },
            { HSSFColor.CORNFLOWER_BLUE.index, String.Empty },
            { HSSFColor.DARK_BLUE.index, "darkblue" },
            { HSSFColor.DARK_GREEN.index, "darkgreen" },
            { HSSFColor.DARK_RED.index, "darkred" },
            { HSSFColor.DARK_TEAL.index, "#006464" },
            { HSSFColor.DARK_YELLOW.index, String.Empty },
            { HSSFColor.GOLD.index, "gold" },
            { HSSFColor.GREEN.index, "green" },
            { HSSFColor.GREY_25_PERCENT.index, "#dcdcdc" },
            { HSSFColor.GREY_40_PERCENT.index, "grey" },
            { HSSFColor.GREY_50_PERCENT.index, "grey" },
            { HSSFColor.GREY_80_PERCENT.index, "#696969" },
            { HSSFColor.INDIGO.index, "indigo" },
            { HSSFColor.LAVENDER.index, "lavender" },
            { HSSFColor.LEMON_CHIFFON.index, "lemonchiffon" },
            { HSSFColor.LIGHT_BLUE.index, "lightblue" },
            { HSSFColor.LIGHT_CORNFLOWER_BLUE.index, String.Empty },
            { HSSFColor.LIGHT_GREEN.index, "lightgreen" },
            { HSSFColor.LIGHT_ORANGE.index, String.Empty },
            { HSSFColor.LIGHT_TURQUOISE.index, String.Empty },
            { HSSFColor.LIGHT_YELLOW.index, "#FFFF99" },
            { HSSFColor.LIME.index, "lime" },
            { HSSFColor.MAROON.index, "maroon" },
            { HSSFColor.OLIVE_GREEN.index, String.Empty },
            { HSSFColor.ORANGE.index, "orange" },
            { HSSFColor.ORCHID.index, "orchid" },
            { HSSFColor.PALE_BLUE.index, String.Empty },
            { HSSFColor.PINK.index, "pink" },
            { HSSFColor.PLUM.index, "plum" },
            { HSSFColor.RED.index, "red" },
            { HSSFColor.ROSE.index, String.Empty },
            { HSSFColor.ROYAL_BLUE.index, "royalblue" },
            { HSSFColor.SEA_GREEN.index, "seagreen" },
            { HSSFColor.SKY_BLUE.index, "skyblue" },
            { HSSFColor.TAN.index, "tan" },
            { HSSFColor.TEAL.index, "tean" },
            { HSSFColor.TURQUOISE.index, "turquoise" },
            { HSSFColor.VIOLET.index, "violet" },
            { HSSFColor.WHITE.index, "white" },
            { HSSFColor.YELLOW.index, "yellow" }
        };

        public static Dictionary<short, string> StyleAlign
        {
            get { return styleAlign; }
        }

        public static Dictionary<short, string> StyleAlignVertical
        {
            get { return styleAlignVertical; }
        }

        public static Dictionary<short, string> StyleBorder
        {
            get { return styleBorder; }
        }

        public static Dictionary<FontStyle, string> StyleFontStyle
        {
            get { return styleFontStyle; }
        }

        public static Dictionary<short, string> StyleColor
        {
            get { return styleColor; }
        }

        public static HSSFName GetRegion(this HSSFWorkbook workbook, string regionName)
        {
            var regionIndex = workbook.GetNameIndex(regionName);
            return workbook.GetNameAt(regionIndex);
        }

        public static bool RegionExists(this HSSFWorkbook workbook, string regionName)
        {
            return workbook.GetNameIndex(regionName) != -1;
        }

        public static AreaReference GetRegionArea(this HSSFWorkbook workbook, string regionName)
        {
            HSSFName region;
            try
            {
                region = workbook.GetRegion(regionName);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentOutOfRangeException(String.Format("Не удалось найти область \"{0}\" в шаблоне формы.", regionName));
            }

            return new AreaReference(region.Reference);
        }

        /// <summary>
        /// Выполняет проверку является ли область area подмножеством.
        /// </summary>
        public static bool IsSubArea(this AreaReference parent, AreaReference area)
        {
            return area.FirstCell.Row >= parent.FirstCell.Row
                && area.LastCell.Row <= parent.LastCell.Row
                && area.FirstCell.Col >= parent.FirstCell.Col
                && area.LastCell.Col <= parent.LastCell.Col;
        }
    }
}
