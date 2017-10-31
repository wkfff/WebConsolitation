using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.WebUI.UltraWebGrid;

namespace Krista.FM.Server.Dashboards.Common.GridIndicatorRules
{
    /// <summary>
    /// Правило индикации
    /// </summary>
    public class IndicatorRule
    {
        public virtual void SetRowStyle(UltraGridRow row)
        {
        }

        protected static int GetIntDtValue(UltraGridRow row, int cellIndex)
        {
            if (row.Cells.Count > cellIndex)
            {
                int value;
                if (row.Cells[cellIndex].Value != null && Int32.TryParse(row.Cells[cellIndex].Value.ToString(), out value))
                {
                    return value;
                }
            }
            return Int32.MinValue;
        }

        protected static int GetIntDtValue(UltraGridRow row, string columnName)
        {
            if (row.Band != null)
            {
                for (int i = 0; i < row.Band.Columns.Count; i++)
                {
                    UltraGridColumn column = row.Band.Columns[i];
                    if (column.Header.Caption == columnName)
                    {
                        return GetIntDtValue(row, i);
                    }
                }

            }
            return Int32.MinValue;
        }

        protected static double GetDoubleDtValue(UltraGridRow row, int cellIndex)
        {
            if (row.Cells.Count > cellIndex)
            {
                double value;
                if (row.Cells[cellIndex].Value != null && Double.TryParse(row.Cells[cellIndex].Value.ToString(), out value))
                {
                    return value;
                }
            }
            return Double.MinValue;
        }

        protected static double GetDoubleDtValue(UltraGridRow row, string columnName)
        {
            if (row.Band != null)
            {
                for (int i = 0; i < row.Band.Columns.Count; i++)
                {
                    UltraGridColumn column = row.Band.Columns[i];
                    if (column.Header.Caption == columnName)
                    {
                        return GetDoubleDtValue(row, i);
                    }
                }

            }
            return Double.MinValue;
        }
    }

}
