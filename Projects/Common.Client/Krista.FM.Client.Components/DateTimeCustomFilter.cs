using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinGrid;

namespace Krista.FM.Client.Components
{
    public partial class DateTimeCustomFilter : Form
    {
        private static DateTimeCustomFilter filterForm = null;

        public static bool ShowDateTimeCustomFilter(string filterFormCaption, ref object grid, string columnName)
        {
            if (filterForm == null)
                filterForm = new DateTimeCustomFilter();
            filterForm.Text = String.Format("¬ведите параметры фильтра дл€ пол€ '{0}'", filterFormCaption);
            if (filterForm.ShowDialog() == DialogResult.OK)
            {
                SetServerFilterValue((UltraGrid)grid, columnName, StartDate, EndDate);
                return true;
            }
            else
                return false;
        }

        public DateTimeCustomFilter()
        {
            InitializeComponent();
            StartDate = this.udteStartDate.DateTime;
            EndDate = this.udteEndDate.DateTime;
        }

        private static DateTime StartDate;
        private static DateTime EndDate;

        private void udteStartDate_ValueChanged(object sender, EventArgs e)
        {
            StartDate = this.udteStartDate.DateTime;
        }

        private void udteEndDate_ValueChanged(object sender, EventArgs e)
        {
            EndDate = this.udteEndDate.DateTime;
        }

        private static void SetServerFilterValue(UltraGrid grid, string columnName, object value1, object value2)
        {
            UltraGridBand band = grid.DisplayLayout.Bands[0];
            if (band.Columns.Exists(columnName))
            {
                UltraGridColumn column = band.Columns[columnName];
                column.FilterOperatorDefaultValue = FilterOperatorDefaultValue.Equals;
                band.ColumnFilters[columnName].ClearFilterConditions();
                band.ColumnFilters[columnName].FilterConditions.Add(FilterComparisionOperator.GreaterThanOrEqualTo, value1);
                band.ColumnFilters[columnName].FilterConditions.Add(FilterComparisionOperator.LessThanOrEqualTo, value2);
                band.ColumnFilters[columnName].LogicalOperator = FilterLogicalOperator.And;
            }
        }
    }
}