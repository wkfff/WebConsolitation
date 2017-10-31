using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infragistics.WebUI.UltraWebGrid;

namespace Krista.FM.Server.Dashboards.Common.GridIndicatorRules
{
    /// <summary>
    /// Индикация темпа роста с масштабом, абсолютные значения
    /// один из показателей 0 - в единицах
    /// результат от 0 до 1 - в процентах
    /// результат больше 100% - в разах
    /// </summary>
    public class GrowRateScaleRule : IndicatorRule
    {
        #region поля
        //-----------------------------------------------

        // номер колонки показателя
        private readonly int columnIndex = -1;

        // колонка со знаком показателя
        private readonly string columnSignName = String.Empty;
        private int columnSignIndex = -1;
        
        // колонка с единицами измерения показателя
        private readonly string columnSIName = String.Empty;
        private int columnSIIndex = -1;
		private string defaultSI = "ед";

        // колонка с форматом вывода показателя
        private readonly string columnFormatName = String.Empty;
        private int columnFormatIndex = -1;
        private string defaultFormat = "N0";

        // тексты хинтов
        private string hintUp = String.Empty;
        private string hintDown = String.Empty;

		// индикаторы-стрелки
		private string imageUp = "~/images/arrowGreenUpBB.png";
		private string imageDown = "~/images/arrowRedDownBB.png";

        //-----------------------------------------------
        #endregion

        #region свойства
        //-----------------------------------------------
        public string HintUp
        {
            set { hintUp = value; }
        }

        public string HintDown
        {
            set { hintDown = value; }
        }

		public string SI
		{
			set { defaultSI = value; }
		}

		public string Format
		{
			set { defaultFormat = value; }
		}

		public string ImageUp
		{
			set { imageUp = value; }
		}

		public string ImageDown
		{
			set { imageDown = value; }
		}
        //-----------------------------------------------
        #endregion

        /// <summary>
        /// Индикация темпа роста с масштабом (единицы, %, разы), абсолютные значения
        /// </summary>
        /// <param name="columnIndex">номер колонки показателя</param>
        /// <param name="columnSignName">название колонки со знаком показателя</param>
        /// <param name="columnSIName">название колонки с единицами измерения показателя</param>
        /// <param name="columnFormatName">название колонки с форматом вывода показателя</param>
        public GrowRateScaleRule(int columnIndex, string columnSignName, string columnSIName, string columnFormatName)
        {
            this.columnIndex = columnIndex;
            this.columnSignName = columnSignName;
            this.columnSIName = columnSIName;
            this.columnFormatName = columnFormatName;
        }


        public override void SetRowStyle(UltraGridRow row)
        {
            // если не все параметры найдены - выходим
            if (columnIndex == -1 || columnSignIndex == -2 || columnSIIndex == -2)
                return;

            // находим номер колонки-флага
            int i = 0;
            while ((i < row.Cells.Count) && (columnSignIndex == -1))
            {
                string columnCaption = row.Band.Grid.Columns[i].Header.Caption;
                if (columnCaption.Contains(columnSignName))
                {
                    columnSignIndex = i;
                    break;
                }
                i++;
            }

            // находим номер колонки с единицами измерения
            i = 0;
            while ((i < row.Cells.Count) && (columnSIIndex == -1))
            {
                string columnCaption = row.Band.Grid.Columns[i].Header.Caption;
                if (columnCaption.Contains(columnSIName))
                {
                    columnSIIndex = i;
                    break;
                }
                i++;
            }

            // находим номер колонки с форматом показателя
            i = 0;
            while ((i < row.Cells.Count) && (columnFormatIndex == -1))
            {
                string columnCaption = row.Band.Grid.Columns[i].Header.Caption;
                if (columnCaption.Contains(columnFormatName))
                {
                    columnFormatIndex = i;
                    break;
                }
                i++;
            }

            // если не нашли чего-то нужного - выход
            if (columnSignIndex == -1 || columnSIIndex == -1)
            {
                columnSignIndex = -2;
                columnSIIndex = -2;
                return;
            }

            // если не нашли чего-то НЕнужного - просто больше не будем это искать
            if (columnFormatIndex == -1)
            {
                columnFormatIndex = -2;
            }

            UltraGridCell cell = row.Cells[columnIndex];
            double value = GetDoubleDtValue(row, columnIndex);
            if (value != Double.MinValue)
            {
                // единицы измерения
            	string si = String.Empty;
				if (row.Cells[columnSIIndex].Value != null)
				{
					if (row.Cells[columnSIIndex].Value.ToString() == "#")
					{
						si = " " + defaultSI;
					}
					else
					{
						si = " " + row.Cells[columnSIIndex].Value;
					}
				}

				// формат вывода
            	string format = String.Empty;
				if ((columnFormatIndex >= 0) && (row.Cells[columnFormatIndex].Value != null))
            	{
					if (row.Cells[columnFormatIndex].Value.ToString() == "#")
					{
						format = defaultFormat;
					}
					else
					{
						format = row.Cells[columnFormatIndex].Value.ToString();
					}
            	}

                // значение показателя
                cell.Value = String.Format("{0:" + format + "}" + si, cell.Value);

                // стрелки
                double growRate = GetDoubleDtValue(row, columnSignIndex);
                if (growRate != Double.MinValue)
                {
                    if (growRate > 0)
                    {
                        cell.Style.BackgroundImage = imageUp;
                        cell.Title = hintUp;
                    }
                    else if (growRate < 0)
                    {
                        cell.Style.BackgroundImage = imageDown;
                        cell.Title = hintDown;
                    }
                    cell.Style.CustomRules =
                        "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
            }

        }
    }
}
