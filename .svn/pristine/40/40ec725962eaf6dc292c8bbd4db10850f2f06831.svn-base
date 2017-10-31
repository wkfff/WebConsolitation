using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;

namespace Krista.FM.Server.Dashboards.Common.GridIndicatorRules
{
    /// <summary>
    /// Стили шрифта для уровней строк
    /// </summary>
    public class FontRowLevelRule : IndicatorRule
    {
        private Dictionary<string, Font> fontLevels;
        private Dictionary<string, Color> fontColorLevels;
        private int levelIndex;

        public FontRowLevelRule(int levelCellIndex)
        {
            levelIndex = levelCellIndex;
            fontLevels = new Dictionary<string, Font>();
            fontColorLevels = new Dictionary<string, Color>();
        }

        public void AddFontLevel(string levelName, Font font)
        {
            if (!fontLevels.ContainsKey(levelName))
            {
                fontLevels.Add(levelName, font);
            }
        }

        public void AddFontLevel(string levelName, Font font, Color color)
        {
            if (!fontLevels.ContainsKey(levelName))
            {
                fontLevels.Add(levelName, font);
            }

            if (!fontColorLevels.ContainsKey(levelName))
            {
                fontColorLevels.Add(levelName, color);
            }
        }

        public override void SetRowStyle(UltraGridRow row)
        {
            string level = String.Empty;
            if (row.Cells[levelIndex].Value != null)
            {
                level = row.Cells[levelIndex].Value.ToString();
            }

            bool applyRowStyle = fontLevels.ContainsKey(level);

            for (int i = 0; i < row.Cells.Count; i++)
            {
                UltraGridCell cell = row.Cells[i];

                if (applyRowStyle)
                {
                    if (fontLevels.ContainsKey(level))
                    {
                        CopyFontStyle(cell.Style.Font, fontLevels[level]);
                    }

                    if (fontColorLevels.ContainsKey(level))
                    {
                        cell.Style.ForeColor = fontColorLevels[level];
                    }
                }
            }
        }

        private static void CopyFontStyle(FontInfo fontInfo, Font font)
        {
            fontInfo.Name = font.Name;
            fontInfo.Size = FontUnit.Point((int)font.SizeInPoints);
            fontInfo.Bold = font.Bold;
            fontInfo.Italic = font.Italic;
        }
    }
}
