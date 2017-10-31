using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infragistics.WebUI.UltraWebGrid;

namespace Krista.FM.Server.Dashboards.Common.GridIndicatorRules
{
    /// <summary>
    /// Настройка хинтов
    /// </summary>
    public class HintRule : IndicatorRule
    {
        #region поля
        //-----------------------------------------------
        
        // номер колонки показателя
        private readonly int columnIndex = -1;

        // название колонки с хинтом
        private readonly string columnDescrName = String.Empty;
        private int descrIndex = -1;

        //-----------------------------------------------
        #endregion

        #region свойства
        //-----------------------------------------------
        //-----------------------------------------------
        #endregion

        /// <summary>
        /// Правило для колонки устанавливает хинт из другой колонки 
        /// </summary>
        /// <param name="columnIndex">номер колонки</param>
        /// <param name="columnDescrName">название колонки с текстом хинта</param>
        public HintRule(int columnIndex, string columnDescrName)
        {
            this.columnIndex = columnIndex;
            this.columnDescrName = columnDescrName;
        }

        public override void SetRowStyle(UltraGridRow row)
        {
            // если не все параметры найдены - выходим
            if (columnIndex == -1 || descrIndex == -2)
                return;

            // находим номер колонки с хинтом
            int i = 0;
            while ((i < row.Cells.Count) && (descrIndex == -1))
            {
                string columnCaption = row.Band.Grid.Columns[i].Header.Caption;
                if (columnCaption.Contains(columnDescrName))
                {
                    descrIndex = i;
                    break;
                }
                i++;
            }

            // если не нашли чего-то нужного - выход
            if (descrIndex == -1)
            {
                descrIndex = -2;
                return;
            }

            // установим хинт
            UltraGridCell cell = row.Cells[columnIndex];
            string descr = 
                (row.Cells[descrIndex].Value != null) 
                    ? 
                row.Cells[descrIndex].Value.ToString() 
                    : 
                String.Empty;

            cell.Title = descr;

        }
    }
}
