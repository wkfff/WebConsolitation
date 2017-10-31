using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Infragistics.WebUI.UltraWebGrid;

namespace Krista.FM.Server.Dashboards.Common.GridIndicatorRules
{
    /// <summary>
    /// Заменяет значение, соответствуюющее паттерну, значением из другой колонки
    /// </summary>
    public class ReplaceValueColumnRule : IndicatorRule
    {
        #region поля
        // номер колонки показателя
        private readonly int columnIndex = -1;

		// паттерн, значение соответствующее паттерну будет заменено
    	private readonly string pattern = String.Empty;

        // название колонки с новыми значениями
        private readonly string columnFromName = String.Empty;
        private int fromIndex = -1;
        #endregion

        #region свойства
        #endregion

    	/// <summary>
    	/// Правило заменяет значение, соответствуюющее паттерну, значением из другой колонки
    	/// </summary>
    	/// <param name="columnIndex">номер колонки показателя</param>
    	/// <param name="pattern">regexp-совместимый паттерн</param>
    	/// <param name="columnFromName">название колонки с текстом для замены</param>
    	public ReplaceValueColumnRule(int columnIndex, string pattern, string columnFromName)
        {
            this.columnIndex = columnIndex;
    		this.pattern = pattern;
            this.columnFromName = columnFromName;
        }

        public override void SetRowStyle(UltraGridRow row)
        {
            // если не все параметры найдены - выходим
            if (columnIndex == -1 || fromIndex == -2)
                return;

            // находим номер колонки с хинтом
            int i = 0;
			while ((i < row.Cells.Count) && (fromIndex == -1))
            {
                string columnCaption = row.Band.Grid.Columns[i].Header.Caption;
                if (columnCaption.Contains(columnFromName))
                {
					fromIndex = i;
                    break;
                }
                i++;
            }

            // если не нашли чего-то нужного - выход
			if (fromIndex == -1)
            {
				fromIndex = -2;
                return;
            }

            // правило
            UltraGridCell cell = row.Cells[columnIndex];

			string value = 
				(cell.Value != null) 
					? 
				cell.Value.ToString() 
					: 
				String.Empty;

            string replaceTo =
				(row.Cells[fromIndex].Value != null) 
                    ?
				row.Cells[fromIndex].Value.ToString() 
                    : 
                String.Empty;

			cell.Value = Regex.Replace(value, pattern, replaceTo);

        }
    }
}
