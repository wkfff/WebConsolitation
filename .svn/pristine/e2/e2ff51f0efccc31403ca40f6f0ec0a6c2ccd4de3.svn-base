using System;
using System.Globalization;
using System.Windows.Forms;

namespace Krista.FM.Update.Framework.Controls
{
    public class DataGridViewColumnFactory
    {
        #region [TextBox]

        public static DataGridViewColumn BuildTextColumnStyle(String pDataPropertyName, String pHeaderText, int pWidth, int pMinWidth, int pFillWeight, bool isSort)
        {
            return BuildTextColumnStyle(pDataPropertyName, pHeaderText, pWidth, "", DataGridViewContentAlignment.MiddleLeft, "", true, pMinWidth, pFillWeight, isSort);
        }

        public static DataGridViewColumn BuildTextColumnStyle(String pDataPropertyName, String pHeaderText, int pWidth, string ToolTipText, int pMinWidth, int pFillWeight, bool isSort)
        {
            DataGridViewColumn col = BuildTextColumnStyle(pDataPropertyName, pHeaderText, pWidth, "", DataGridViewContentAlignment.MiddleLeft, "", true, pMinWidth, pFillWeight, isSort);
            col.ToolTipText = ToolTipText;
            return col;
        }

        public static DataGridViewColumn BuildTextColumnStyle(String pDataPropertyName, String pHeaderText, int pWidth, String pNullText, DataGridViewContentAlignment pAlignment, String pFormat, bool bVisible, int pMinWidth, int pFillWeight, bool isSort)
        {
            DataGridViewTextBoxColumn oTextColumnStyle = new DataGridViewTextBoxColumn();
            oTextColumnStyle.HeaderText = pHeaderText;
            oTextColumnStyle.Name = pDataPropertyName;
            oTextColumnStyle.DataPropertyName = pDataPropertyName;
            oTextColumnStyle.Width = pWidth;
            oTextColumnStyle.FillWeight = pFillWeight;
            oTextColumnStyle.MinimumWidth = pMinWidth;
            oTextColumnStyle.CellTemplate.Style.Format = pFormat;
            oTextColumnStyle.CellTemplate.Style.NullValue = pNullText;
            oTextColumnStyle.CellTemplate.Style.Alignment = pAlignment;
            oTextColumnStyle.Visible = bVisible;
            oTextColumnStyle.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (isSort) oTextColumnStyle.SortMode = DataGridViewColumnSortMode.Automatic;

            return oTextColumnStyle;
        }

        #endregion [TextBox]

        #region [DateTime]
        public static DataGridViewTextBoxColumn BuildDateTimeColumnStyle(String pDataPropertyName, String pHeaderText)
        {
            return BuildDateTimeColumnStyle(pDataPropertyName, pHeaderText, pHeaderText);
        }
        public static DataGridViewTextBoxColumn BuildDateTimeColumnStyle(String pDataPropertyName, string pHeaderText, string ToolTipText)
        {
            DataGridViewTextBoxColumn oTextBoxColumnStyle = new DataGridViewTextBoxColumn
                                                                {
                                                                    HeaderText = pHeaderText,
                                                                    ToolTipText = ToolTipText,
                                                                    DataPropertyName = pDataPropertyName,
                                                                    Name = pDataPropertyName,
                                                                    Width = 85
                                                                };
            oTextBoxColumnStyle.CellTemplate.Style.Format = "yyyy/MM/dd";
            oTextBoxColumnStyle.CellTemplate.Style.NullValue = string.Empty;
            oTextBoxColumnStyle.CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            oTextBoxColumnStyle.ReadOnly = true;
            return oTextBoxColumnStyle;
        }

        #endregion [DateTime]

        #region [Numeric]

        public static DataGridViewTextBoxColumn BuildNumericColumnStyle(String pDataPropertyName, String pHeaderText, int pWidth, bool visible)
        {
            DataGridViewTextBoxColumn oTextBoxColumnStyle = BuildNumericColumnStyle(pDataPropertyName, pHeaderText, pWidth);
            oTextBoxColumnStyle.Visible = visible;
            return oTextBoxColumnStyle;
        }

        public static DataGridViewTextBoxColumn BuildNumericColumnStyle(String pDataPropertyName, String pHeaderText, int pWidth, string tooltip)
        {
            DataGridViewTextBoxColumn oTextBoxColumnStyle = BuildNumericColumnStyle(pDataPropertyName, pHeaderText, pWidth);
            oTextBoxColumnStyle.ToolTipText = tooltip;
            return oTextBoxColumnStyle;
        }

        public static DataGridViewTextBoxColumn BuildNumericColumnStyle(String pDataPropertyName, String pHeaderText, int pWidth)
        {
            DataGridViewTextBoxColumn oTextBoxColumnStyle = new DataGridViewTextBoxColumn();
            oTextBoxColumnStyle.HeaderText = pHeaderText;
            oTextBoxColumnStyle.DataPropertyName = pDataPropertyName;
            oTextBoxColumnStyle.Name = pDataPropertyName;
            oTextBoxColumnStyle.Width = pWidth;
            oTextBoxColumnStyle.CellTemplate.Style.Format = "N";
            oTextBoxColumnStyle.CellTemplate.Style.FormatProvider = AppNumberFormat(NUMFORMAT.STDNUMBER);
            oTextBoxColumnStyle.CellTemplate.Style.NullValue = string.Empty;
            oTextBoxColumnStyle.CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            oTextBoxColumnStyle.ReadOnly = true;
            return oTextBoxColumnStyle;
        }

        public enum NUMFORMAT { STDNUMBER, TCNUMBER, INTEGER };

        public static NumberFormatInfo AppNumberFormat(NUMFORMAT nf)
        {
            NumberFormatInfo nfi = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            if (nf == NUMFORMAT.STDNUMBER)
            {
                nfi.CurrencyDecimalDigits = 2;
                nfi.CurrencyDecimalSeparator = ".";
                nfi.NumberDecimalSeparator = ".";
                nfi.NumberDecimalDigits = 2;
            }
            else if (nf == NUMFORMAT.TCNUMBER)
            {
                nfi.CurrencyDecimalDigits = 3;
                nfi.CurrencyDecimalSeparator = ".";
                nfi.NumberDecimalSeparator = ".";
                nfi.NumberDecimalDigits = 3;
            }
            else if (nf == NUMFORMAT.INTEGER)
            {
                nfi.CurrencyDecimalDigits = 0;
                nfi.CurrencyDecimalSeparator = ".";
                nfi.NumberDecimalSeparator = ".";
                nfi.NumberDecimalDigits = 0;
            }
            return nfi;
        }

        #endregion [Numeric]
    }
}
