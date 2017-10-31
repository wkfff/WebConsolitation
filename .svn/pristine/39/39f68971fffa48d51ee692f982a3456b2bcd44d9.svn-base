using System;
using Infragistics.WebUI.UltraWebGrid;

namespace Krista.FM.Server.Dashboards.Common.GridIndicatorRules
{
    public class ImageRowValueRule : IndicatorRule
    {
        string columnName = String.Empty;
        int columnIndex = -1;
        private string pattern;
        private string img;

        private bool UseColumnName
        {
            get { return columnName != String.Empty; }
        }

        public ImageRowValueRule(int columnIndex, string pattern, string img)
        {
            this.columnIndex = columnIndex;
            this.pattern = pattern;
            this.img = img;
        }

        public ImageRowValueRule(string columnName, string pattern, string img)
        {
            this.columnName = columnName;
            this.pattern = pattern;
            this.img = img;
        }

        public override void SetRowStyle(UltraGridRow row)
        {
            for (int i = 0; i < row.Cells.Count; i++)
            {
                UltraGridCell cell = row.Cells[i];

                string columnCaption = row.Band.Grid.Columns[i].Header.Caption;

                bool isIndicatorColumn = UseColumnName && columnCaption.Contains(columnName) ||
                                         !UseColumnName && i == columnIndex;

                if (isIndicatorColumn && cell.Value != null)
                {
                    string value = cell.Value.ToString();
                    if (value == pattern)
                    {
                        cell.Style.BackgroundImage = String.Format("~/images/{0}", img);
                        cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }
            }
        }
    }
}


