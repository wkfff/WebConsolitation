using System;
using System.Collections.ObjectModel;
using Infragistics.WebUI.UltraWebGrid;

namespace Krista.FM.Server.Dashboards.Common.GridIndicatorRules
{
	/// <summary>
	/// Индикатор-звезда (лучший/худший показатель)
	/// </summary>
    public class StarIndicatorRule : IndicatorRule
    {
        #region Поля

		// колонка показателя
        private int columnIndex = -1;
		private string columnName = String.Empty;

		// колонка индикатора
        private int flagIndex = -1;
        private string flagName = String.Empty;

		private string bestRankHint = "Лучший показатель";
		private string worseRankHint = "Худший показатель";

        private string bestRankImg = "~/images/starYellowBB.png";
        private string worseRankImg = "~/images/starGrayBB.png";
		
		private Collection<int> disableOnRows = new Collection<int>();

        #endregion

        #region Свойства

        public string BestRankHint
        {
            get { return bestRankHint; }
            set { bestRankHint = value; }
        }

        public string WorseRankHint
        {
            get { return worseRankHint; }
            set { worseRankHint = value; }
        }

        public string BestRankImg
        {
            get { return bestRankImg; }
            set { bestRankImg = value; }
        }

        public string WorseRankImg
        {
            get { return worseRankImg; }
            set { worseRankImg = value; }
        }

		/// <summary>
		/// Отключить обработку строк с указанными номерами
		/// </summary>
		public Collection<int> DisableOnRows
		{
			get { return disableOnRows; }
			set { disableOnRows = value; }
		}

        #endregion

		/// <summary>
		/// Добавляет индикатор-звезду (лучший/худший показатель)
		/// </summary>
		/// <param name="columnIndex">номер колонки показателя</param>
		/// <param name="flagIndex">номер колонки флага индикатора (1 - лучший, -1 - худший)</param>
        public StarIndicatorRule(int columnIndex, int flagIndex)
        {
            this.columnIndex = columnIndex;
            this.flagIndex = flagIndex;
        }

		/// <summary>
		/// Добавляет индикатор-звезду (лучший/худший показатель)
		/// </summary>
		/// <param name="columnIndex">номер колонки показателя</param>
		/// <param name="flagName">название колонки флага индикатора (значение флага соответствует: "1"-лучшему, "-1"-худшему показателю)</param>
		public StarIndicatorRule(int columnIndex, string flagName)
		{
			this.columnIndex = columnIndex;
			this.flagName = flagName;
		}

		/// <summary>
		/// Добавляет индикатор-звезду (лучший/худший показатель)
		/// </summary>
		/// <param name="columnName">название колонки показателя</param>
		/// <param name="flagName">название колонки флага индикатора (значение флага соответствует: "1"-лучшему, "-1"-худшему показателю)</param>
        public StarIndicatorRule(string columnName, string flagName)
        {
            this.columnName = columnName;
            this.flagName = flagName;
        }


		public override void SetRowStyle(UltraGridRow row)
		{
			// если не все параметры найдены - выходим
			if (columnIndex == -2 || flagIndex == -2)
				return;

			// если данную строку не нужно обрабатывать - выходим
			if (DisableOnRows.Contains(row.Index))
				return;

			// находим номер рабочей колонки
			int i = 0;
			while ((i < row.Cells.Count) && (columnIndex == -1))
			{
				string columnCaption = row.Band.Grid.Columns[i].Header.Caption;
				if (columnCaption.Contains(columnName))
				{
					columnIndex = i;
					break;
				}
				i++;
			}

			// находим номер колонки с флагом
			i = 0;
			while ((i < row.Cells.Count) && (flagIndex == -1))
			{
				string columnCaption = row.Band.Grid.Columns[i].Header.Caption;
				if (columnCaption.Contains(flagName))
				{
					flagIndex = i;
					break;
				}
				i++;
			}

			// если не нашли чего-то нужного - выход
			if (columnIndex == -1 || flagIndex == -1)
			{
				columnIndex = -2;
				flagIndex = -2;
				return;
			}

			// установим хинт
			UltraGridCell cell = row.Cells[columnIndex];
			int flag = GetIntDtValue(row, flagIndex);

			if (flag == 1)
			{
				cell.Style.BackgroundImage = bestRankImg;
				cell.Title = bestRankHint;
			}
			else if (flag == -1)
			{
				cell.Style.BackgroundImage = worseRankImg;
				cell.Title = worseRankHint;
			}

			cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";

		}
    }
}
