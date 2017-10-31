using System;
using Infragistics.WebUI.UltraWebGrid;

namespace Krista.FM.Server.Dashboards.Common.GridIndicatorRules
{
    /// <summary>
    /// Индикация темпа роста
    /// </summary>
    public class GrowRateRule : IndicatorRule
    {
        private string columnName = String.Empty;
        private int columnIndex = -1;
		private string inverseColumnName = String.Empty;
        private int inverseColumnIndex = -1;
        private string increaseText = "Рост к прошлому году";
        private string decreaseText = "Снижение к прошлому году";
        private string increaseImg = "~/images/arrowGreenUpBB.png";
        private string decreaseImg = "~/images/arrowRedDownBB.png";
		private string increaseInvImg = "~/images/arrowRedUpBB.png";
		private string decreaseInvImg = "~/images/arrowGreenDownBB.png";
        private double limit = 1;
        private int leftPadding = 2;

        private bool UseColumnName
        {
            get { return columnName != String.Empty; }
        }

        public string IncreaseText
        {
            get { return increaseText; }
            set { increaseText = value; }
        }

        public string DecreaseText
        {
            get { return decreaseText; }
            set { decreaseText = value; }
        }

        public string IncreaseImg
        {
            get { return increaseImg; }
            set { increaseImg = value; }
        }

        public string DecreaseImg
        {
            get { return decreaseImg; }
            set { decreaseImg = value; }
        }

		public string IncreaseInvImg
		{
			get { return increaseInvImg; }
			set { increaseInvImg = value; }
		}

		public string DecreaseInvImg
		{
			get { return decreaseInvImg; }
			set { decreaseInvImg = value; }
		}

        public double Limit
        {
            get { return limit; }
            set { limit = value; }
        }

        public int LeftPadding
        {
            get { return leftPadding; }
            set { leftPadding = value; }
        }

		public string InverseColumnName
		{
			get { return inverseColumnName; }
			set { inverseColumnName = value; }
		}

		public int InverseColumnIndex
		{
			get { return inverseColumnIndex; }
			set { inverseColumnIndex = value; }
		}

        public GrowRateRule(int columnIndex)
        {
            this.columnIndex = columnIndex;
        }

        public GrowRateRule(string columnName)
        {
            this.columnName = columnName;
        }

        public GrowRateRule(int columnIndex, string increaseText, string decreaseText)
        {
            this.columnIndex = columnIndex;
            this.increaseText = increaseText;
            this.decreaseText = decreaseText;
        }

        public GrowRateRule(string columnName, string increaseText, string decreaseText)
        {
            this.columnName = columnName;
            this.increaseText = increaseText;
            this.decreaseText = decreaseText;
        }

        public override void SetRowStyle(UltraGridRow row)
        {
			// если установлено имя столбеца-флаг для инверсии
			if (inverseColumnIndex == -1 && !inverseColumnName.Equals(String.Empty))
			{
				inverseColumnIndex = -2;

				// находим его номер
				int i = 0;
				while ((i < row.Cells.Count) && (inverseColumnIndex == -2))
				{
					string columnCaption = row.Band.Grid.Columns[i].Header.Caption;
					if (columnCaption.Contains(inverseColumnName))
					{
						inverseColumnIndex = i;
						break;
					}
					i++;
				}
			}

        	for (int i = 0; i < row.Cells.Count; i++)
            {
                UltraGridCell cell = row.Cells[i];
                
                string columnCaption = row.Band.Grid.Columns[i].Header.Caption;

                bool isIndicatorColumn = UseColumnName && columnCaption.Contains(columnName) ||
                                       !UseColumnName && i == columnIndex;

				if (isIndicatorColumn)
                {
					// обработка инверсии
                	bool invert = false;
					if (inverseColumnIndex > -1)
					{
						invert = GetIntDtValue(row, inverseColumnIndex) == 1;
					}

                    double growRate = GetDoubleDtValue(row, i); 
                    if (growRate != Double.MinValue)
                    {
						if (growRate > limit)
                        {
							cell.Style.BackgroundImage = !invert ? increaseImg : increaseInvImg;
                            cell.Title = increaseText;
                        }
						else if (growRate < limit)
                        {
							cell.Style.BackgroundImage = !invert ? decreaseImg : decreaseInvImg;
							cell.Title = decreaseText;
                        }
                        cell.Style.CustomRules = String.Format("background-repeat: no-repeat; background-position: {0}% center; margin: 2px", leftPadding);
                    }
                }
            }
        }
    }
}