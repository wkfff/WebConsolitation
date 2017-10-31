using System;
using Infragistics.WebUI.UltraWebGrid;

namespace Krista.FM.Server.Dashboards.Common.GridIndicatorRules
{
    /// <summary>
    /// Индикация равномерности исполнения
    /// </summary>
    public class PerformanceUniformityRule : IndicatorRule
    {
        private int currentMonthIndex;
        private string columnName = String.Empty;
        private int columnIndex = 0;

        private string nonPerformedText = "Не соблюдается условие равномерности";
        private string perfomedText = "Соблюдается условие равномерности";

        private bool UseColumnName
        {
            get { return columnName != String.Empty; }
        }

        public string NonPerformedText
        {
            get { return nonPerformedText; }
            set { nonPerformedText = value; }
        }

        public string PerfomedText
        {
            get { return perfomedText; }
            set { perfomedText = value; }
        }

        public PerformanceUniformityRule(int columnIndex, int currentMonthIndex)
        {
            this.currentMonthIndex = currentMonthIndex;
            this.columnIndex = columnIndex;
        }

        public PerformanceUniformityRule(string columnName, int currentMonthIndex)
        {
            this.currentMonthIndex = currentMonthIndex;
            this.columnName = columnName;
        }

        public override void SetRowStyle(UltraGridRow row)
        {
            double uniformityLimit = (currentMonthIndex) * 100.0 / 12;
            for (int i = 0; i < row.Cells.Count; i++)
            {
                UltraGridCell cell = row.Cells[i];
                string columnCaption = row.Band.Grid.Columns[i].Header.Caption;

                bool isIndicatorColumn = UseColumnName && columnCaption.Contains(columnName) ||
                                       !UseColumnName && i == columnIndex;

                if (isIndicatorColumn && cell.Value != null && cell.Value.ToString() != String.Empty)
                {
                    if (100 * Convert.ToDouble(cell.Value) < uniformityLimit)
                    {
                        cell.Style.BackgroundImage = "~/images/ballRedBB.png";
                        cell.Title = string.Format("{1} ({0:N2}%)", uniformityLimit, NonPerformedText);
                    }
                    else
                    {
                        cell.Style.BackgroundImage = "~/images/ballGreenBB.png";
                        cell.Title = string.Format("{1} ({0:N2}%)", uniformityLimit, PerfomedText);
                    }
                    cell.Style.Padding.Right = 10;
                    cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
            }
        }
    }
}