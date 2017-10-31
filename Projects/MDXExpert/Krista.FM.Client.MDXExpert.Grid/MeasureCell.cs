using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Data;
using Microsoft.AnalysisServices.AdomdClient;
using Krista.FM.Client.MDXExpert.Grid.Style;


namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// ������ ����������
    /// </summary>
    public class MeasureCell : GridControl
    {
        private MeasureData _measureData;
        private DimensionCell _rowCell;
        private Cell _value;
        private bool _isTopCell;
        private bool _isBottomCell;
        /// <summary>
        /// ������ ������ � ������ � �������
        /// </summary>
        public int Index
        {
            get { return this.MeasureData.IndexOf(this); }
        }

        public MeasureCell(MeasureData measureData)
            : this(measureData, null, null, false, false)
        {
        }

        /// <summary>
        /// ���������� ������
        /// </summary>
        /// <param name="measureData">������ � �������</param>
        /// <param name="value">������ ���� ����</param>
        /// <param name="rowCell">������ ������, � ������� ������������� ������ ��������� �������� 
        /// �������� � ������������ � ������ ����������</param>
        public MeasureCell(MeasureData measureData, Cell value, DimensionCell rowCell, bool isTopCell, bool isBottomCell)
            : base(measureData.Grid, GridObject.MeasureCell)
        {
            this.MeasureData = measureData;
            this.Value = value;
            this.RowCell = rowCell;
            this.IsTopCell = isTopCell;
            this.IsBottomCell = isBottomCell;

            if (this.Grid.SelectedCells != null)
            {
                if (this.Grid.SelectedCells.HashCodes.Contains(this.GetHashCode()))
                {
                    this.State = ControlState.Selected;
                    if (!this.Grid.SelectedCells.CurrentCells.Contains(this))
                    {
                        this.Grid.SelectedCells.CurrentCells.Add(this);
                    }
                }
                else
                {
                    this.State = ControlState.Normal;
                }
            }
        }

        public override void OnClick(System.Drawing.Point mousePosition)
        {
        }

        /// <summary>
        /// ���� ����� ����������� ���������� ����������� ������, ������ True
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override bool GetHitTest(Point point)
        {
            return this.GetBounds().Contains(point);
        }

        /// <summary>
        /// ���� ������ ������ � �������, ������ True
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override bool GetHitTest(Rectangle searchBounds)
        {
            return this.GetBounds().IntersectsWith(searchBounds);
        }


        /// <summary>
        /// ���� ����� ����������� ������, ������ True
        /// </summary>
        /// <param name="point"></param>
        /// <param name="isFindByOffsetBounds"></param>
        /// <returns></returns>
        public override bool GetHitTest(Point point, bool isFindByOffsetBounds)
        {
            if (isFindByOffsetBounds)
                return this.GetOffsetBounds().Contains(point);
            else
                return this.GetHitTest(point);
        }

        /// <summary>
        /// ���� ������ ������ � �������, ������ True
        /// </summary>
        /// <param name="point"></param>
        /// <param name="isFindByOffsetBounds"></param>
        /// <returns></returns>
        public bool GetHitTest(Rectangle searchBounds, bool isFindByOffsetBounds)
        {
            if (isFindByOffsetBounds)
                return this.GetOffsetBounds().IntersectsWith(searchBounds);
            else
                return this.GetHitTest(searchBounds);
        }


        /// <summary>
        /// �������� ������� ������� ��������
        /// </summary>
        /// <returns></returns>
        public override Rectangle GetParentVisibleBounds()
        {
            return this.MeasureData.MeasuresData.GetVisibleBounds();
        }

        /// <summary>
        /// �������� ��� ��� ������
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.GetAllUniqueName().GetHashCode();
        }

        /// <summary>
        /// �������� ���� � ���� ����� ��������� UN 
        /// </summary>
        /// <returns></returns>
        public string GetAllUniqueName()
        {
            return this.GetAllUniqueName(string.Empty);
        }

        /// <summary>
        /// �������� ���� � ���� ����� ��������� UN 
        /// </summary>
        /// <param name="splitter">����������� ����� UN</param>
        /// <returns></returns>
        public string GetAllUniqueName(string splitter)
        {
            string result = string.Empty;

            if (this.MeasureData.MeasureCaption != null)
                result = this.MeasureData.MeasureCaption.UniqueName;

            DimensionCell dimCell = this.RowCell;

            string un = dimCell != null ? dimCell.GetAllUniqueName(splitter) : String.Empty;
            result += (result == string.Empty) ? un : splitter + un;

            dimCell = this.CaptionsSection.ColumnCell;
            un = dimCell != null ? dimCell.GetAllUniqueName(splitter) : String.Empty;
            result += (result == string.Empty) ? un : splitter + un;

            return result;
        }


        /// <summary>
        /// �������� �������� ������������ � ������
        /// </summary>
        /// <returns></returns>
        public override string GetValue()
        {
            return this.GetFormatedValue(this.Value, this.MeasureData.MeasureCaption.MeasureValueFormat);
        }

        /// <summary>
        /// �������� �������� ��� �������� � Excel
        /// </summary>
        /// <returns></returns>
        public string GetValueForExcel()
        {
            ValueFormat measureFormat = this.MeasureData.MeasureCaption.MeasureValueFormat;
            string result = string.Empty;

            if (measureFormat.FormatType == FormatType.Auto)
            {
                //� �������������� �������, ��� ��� �� ����� �������, ��� ������ ����������� ��������,
                //��� ����������� ����������� ������� ����� Excel
                result = this.GetValue().Replace(this.MeasureData.MeasuresData.NumberFormat.CurrencyGroupSeparator, "");
            }
            else
            {
                UnitDisplayType customType = measureFormat.UnitDisplayType;
                bool customThousandDelimiter = measureFormat.ThousandDelimiter;

                measureFormat.UnitDisplayType = UnitDisplayType.None;
                measureFormat.ThousandDelimiter = false;
                measureFormat.IsMayHook = true;

                result = this.GetFormatedValue(this.Value, this.MeasureData.MeasureCaption.MeasureValueFormat);

                measureFormat.UnitDisplayType = customType;
                measureFormat.ThousandDelimiter = customThousandDelimiter;
                measureFormat.IsMayHook = false;
            }
            return result;
        }

        /// <summary>
        /// ������� ������� ������
        /// </summary>
        /// <returns></returns>
        public override Rectangle GetVisibleBounds()
        {
            return Rectangle.Intersect(this.MeasureData.GetVisibleBounds(), this.GetOffsetBounds());
        }

        /// <summary>
        /// ������ ������
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="painter"></param>
        public override void OnPaint(System.Drawing.Graphics graphics, Painter painter)
        {
            this.OnPaint(graphics, painter, false);
        }

        /// <summary>
        /// ������ ������
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="painter"></param>
        public void OnPaint(System.Drawing.Graphics graphics, Painter painter, bool isDummyCell)
        {
            //!!! ������ ���������� ������ ����� ������������� (� �� ���� � ������� ������, ���� �� ����), 
            //������� ���������� ����������� ���� ����, ������������ � �.�. ������� ����� ���� ������� 
            //� PivotData, ���� ��������� ���� ������������ ��������� � ������ ������� �����������, ������ 
            //��������� � ����� �������� ���������� �� ������������
            ValueFormat valueFormat = this.MeasureData.MeasureCaption.MeasureValueFormat;
            if (valueFormat.ValueAlignment != this.MeasureData.MeasuresData.Style.StringFormat.Alignment)
            {
                this.MeasureData.MeasuresData.Style.StringFormat.Alignment = valueFormat.ValueAlignment;
                this.MeasureData.MeasuresData.StyleForTotals.StringFormat.Alignment = valueFormat.ValueAlignment;
                this.MeasureData.MeasuresData.StyleForHigherAverageCells.StringFormat.Alignment = valueFormat.ValueAlignment;
                this.MeasureData.MeasuresData.StyleForLowerAverageCells.StringFormat.Alignment = valueFormat.ValueAlignment;
                this.MeasureData.MeasuresData.StyleForHigherDeviationCells.StringFormat.Alignment = valueFormat.ValueAlignment;
                this.MeasureData.MeasuresData.StyleForLowerDeviationCells.StringFormat.Alignment = valueFormat.ValueAlignment;
                this.MeasureData.MeasuresData.StyleForHigherMedianCells.StringFormat.Alignment = valueFormat.ValueAlignment;
                this.MeasureData.MeasuresData.StyleForLowerMedianCells.StringFormat.Alignment = valueFormat.ValueAlignment;
                this.MeasureData.MeasuresData.StyleForTopCountCells.StringFormat.Alignment = valueFormat.ValueAlignment;
                this.MeasureData.MeasuresData.StyleForBottomCountCells.StringFormat.Alignment = valueFormat.ValueAlignment;
            }

            //�������� ���������� ������ � ������ ��������, � ������ ��
            painter.DrawMeasureCell(graphics, this.GetOffsetBounds(), this.State,
                //���������� ����� ����������� � ������
                this.GetCellStyle(isDummyCell),
                //����������� ������������ ��������
                this.GetFormatedValue(this.Value, valueFormat));
        }

        public override bool IsClickChildElement(Point point)
        {
            return false;
        }

        /// <summary>
        /// ���������� ����� ��� ��������� ������
        /// </summary>
        /// <param name="isDummyCell">��������� �� ������</param>
        /// <returns></returns>
        public CellStyle GetCellStyle(bool isDummyCell)
        {
            CellStyle style = this.MeasureData.MeasuresData.Style;

            if (isDummyCell || (this.Value == null))
                style = this.MeasureData.MeasuresData.StyleForDummyCells;
            else
                if (this.IsTotal)
                    style = this.MeasureData.MeasuresData.StyleForTotals;


            CellStyle newStyle = (this.IsTopCell) ? this.MeasureData.MeasuresData.StyleForTopCountCells : style;

            if (newStyle == style)
            {
                newStyle = (this.IsBottomCell) ? this.MeasureData.MeasuresData.StyleForBottomCountCells : style;
            }

            if (newStyle == style)
            {
                newStyle = this.ApplyMedianColors(style);
            }

            if (newStyle == style)
            {
                newStyle = this.ApplyAverageColors(style);
            }

            if (newStyle == style)
            {
                return this.Grid.ColorRules.ApplyColorRuleForCell(this, style);
            }
            return newStyle;
        }


        /// <summary>
        /// ��������� � ������ �����, ��������� ��� ������� �������� �������� (����/���� ��������, ����/���� ������ ������������ ����������)
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        private CellStyle ApplyAverageColors(CellStyle style)
        {
            CellStyle result = style;
            try
            {
                if ((this.Grid.PivotData.AverageSettings.AverageType == AverageType.None) ||
                    (this.MeasureData.MeasureCaption.IsAverageDev) ||
                    (this.Index < 0) ||
                    (this.Grid.PivotData.RowAxis.FieldSets.Count == 0) ||
                    (this.Value == null) ||
                    (this.Value.Value == null) ||
                    (this.RowCell == null) ||
                    (this.RowCell.IsTotal) ||
                    (this.RowCell.IsAverage) ||
                    (this.RowCell.IsMedian) ||
                    (this.RowCell.IsStandartDeviation) ||
                    (this.IsTotal)
                    )
                    return result;

                //������ ������ �� ������� ���������
                DimensionCell averageRowCell = this.RowCell.Parent.GetAverage();
                if (averageRowCell == null)
                    return result;

                //������ ������, � ������� ���-�� ������� �������� ������� ������ ���������
                int avgMeasureCellIndex = averageRowCell.LeafIndex;
                decimal avgValue = 0;

                if ((this.MeasureData[avgMeasureCellIndex].Value != null) &&
                    (this.MeasureData[avgMeasureCellIndex].Value.Value != null))
                {
                    if (!Decimal.TryParse(this.MeasureData[avgMeasureCellIndex].Value.Value.ToString(), NumberStyles.Any, null, out avgValue))
                    {
                        return result;
                    }
                }


                if (this.Grid.PivotData.AverageSettings.IsLowerHigherAverageSeparate)
                {
                    decimal curValue;
                    if(Decimal.TryParse(this.Value.Value.ToString(), NumberStyles.Any, null, out curValue))
                    {
                        result = (curValue < avgValue)
                                     ? this.MeasureData.MeasuresData.StyleForLowerAverageCells
                                     : this.MeasureData.MeasuresData.StyleForHigherAverageCells;

                    }
                }


                if (this.Grid.PivotData.AverageSettings.IsStandartDeviationCalculate)
                {
                    if (this.RowCell.IsTotal)
                        return result;

                    DimensionCell stDevRowCell = this.RowCell.Parent.GetStandartDeviation();

                    if (stDevRowCell == null)
                        return result;

                    //������ ������, � ������� ���-�� �������� ������������ ���������� ��� ������� ������ ���������
                    int stDevRowCellIndex = stDevRowCell.LeafIndex;


                    if ((this.MeasureData[stDevRowCellIndex].Value != null) &&
                        (this.MeasureData[stDevRowCellIndex].Value.Value != null))
                    {
                        decimal stDevValue = 0;
                        if (Decimal.TryParse(this.MeasureData[stDevRowCellIndex].Value.Value.ToString(), NumberStyles.Any, null, out stDevValue))
                        {
                            //������ ������� ������������ ����������
                            decimal currMinAverageDeviation = avgValue - stDevValue;
                            //������� ������� ������������ ����������
                            decimal currMaxAverageDeviation = avgValue + stDevValue;

                            decimal curValue = 0;
                            if (Decimal.TryParse(this.Value.Value.ToString(), out curValue))
                            {
                                result = (curValue < currMinAverageDeviation)
                                             ? this.MeasureData.MeasuresData.StyleForLowerDeviationCells
                                             : (curValue > currMaxAverageDeviation)
                                                   ? this.MeasureData.MeasuresData.StyleForHigherDeviationCells
                                                   : result;

                            }

                        }
                    }


                }
            }
            catch (Exception exc)
            {
                //MessageBox.Show(exc.Message);
            }
            return result;
        }


        /// <summary>
        /// ��������� � ������ �����, ��������� ��� ������� �������
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        private CellStyle ApplyMedianColors(CellStyle style)
        {
            CellStyle result = style;
            try
            {
                if ((!this.Grid.PivotData.MedianSettings.IsMedianCalculate) ||
                    (!this.Grid.PivotData.MedianSettings.IsLowerHigherSeparate) ||
                    (this.MeasureData.MeasureCaption.IsAverageDev) ||
                    (this.Index < 0) ||
                    (this.Grid.PivotData.RowAxis.FieldSets.Count == 0) ||
                    (this.Value == null) ||
                    (this.Value.Value == null) ||
                    (this.RowCell == null) ||
                    (this.RowCell.IsTotal) ||
                    (this.RowCell.IsMedian) ||
                    (this.RowCell.IsAverage) ||
                    (this.RowCell.IsStandartDeviation) ||
                    (this.IsTotal)
                    )
                    return result;

                //������ ������ � ��������
                DimensionCell medianRowCell = this.RowCell.Parent.GetMedian();
                if (medianRowCell == null)
                    return result;

                //������ ������, � ������� ���-�� ������� ������� ������ ���������
                int medMeasureCellIndex = medianRowCell.LeafIndex;
                decimal medianValue = 0;

                if ((this.MeasureData[medMeasureCellIndex].Value != null) &&
                    (this.MeasureData[medMeasureCellIndex].Value.Value != null))
                {
                    if (!Decimal.TryParse(this.MeasureData[medMeasureCellIndex].Value.Value.ToString(), NumberStyles.Any, null, out medianValue))
                    {
                        return result;
                    }
                }

                if (this.Grid.PivotData.MedianSettings.IsLowerHigherSeparate)
                {
                    decimal curValue;
                    if (Decimal.TryParse(this.Value.Value.ToString(), NumberStyles.Any, null, out curValue))
                    {
                        result = (curValue < medianValue)
                                     ? this.MeasureData.MeasuresData.StyleForLowerMedianCells
                                     : this.MeasureData.MeasuresData.StyleForHigherMedianCells;

                    }
                }

            }
            catch (Exception exc)
            {
                //MessageBox.Show(exc.Message);
            }
            return result;
        }


        /// <summary>
        /// �������� ��� �������� ���������, ���������� ��������������� ��������� ��������, Dicimal ����
        /// </summary>
        /// <param name="sValue">��������� ��������</param>
        /// <param name="sDigitCount">���������� ���� ����� �������</param>
        /// <param name="unit">������� ���������</param>
        /// <param name="divisor">��������</param>
        /// <returns></returns>
        private string GetCurrencyValue(string sValue, string sDigitCount, string unit, int divisor)
        {
            string result = sValue;
            decimal cValue;
            if (decimal.TryParse(sValue, NumberStyles.Any, null, out cValue))
            {
                if (divisor > 1)
                    cValue /= divisor;
                result = cValue.ToString("N" + sDigitCount);
                if (unit != string.Empty)
                    result += unit;
            }
            else
            {
                result = GridConsts.errorValue;
            }
            return result;
        }

        /// <summary>
        /// �������� ��� �������� ���������, ���������� ��������������� ��������� ��������, Double ����
        /// </summary>
        /// <param name="sValue">��������� ��������</param>
        /// <param name="sDigitCount">���������� ���� ����� �������</param>
        /// <param name="unit">������� ���������</param>
        /// <param name="divisor">��������</param>
        /// <returns></returns>
        private string GetNumericValue(string sValue, string sDigitCount, string unit, int divisor)
        {
            string result = sValue;
            double nValue;
            if (double.TryParse(sValue, NumberStyles.Any, null, out nValue))
            {
                if (divisor > 1)
                    nValue /= divisor;
                result = nValue.ToString("N" + sDigitCount);
                if (unit != string.Empty)
                    result += unit;
            }
            else
            {
                result = GridConsts.errorValue;
            }
            return result;
        }

        /// <summary>
        /// ����������� value � ��������� �������
        /// </summary>
        /// <param name="value">�������� ��� ��������������</param>
        /// <param name="format">������</param>
        /// <returns></returns>
        private string GetFormatedValue(Cell cell, ValueFormat format)
        {
            try
            {
                if ((cell == null) || (cell.Value == null))
                    return string.Empty;
            }
            catch
            {
                return GridConsts.errorValue;
            }

            if ((format == null) || (format.FormatType == FormatType.Auto))
            {
                //���� ������ ��������������, ����� �������� ��� ������...
                if (cell.FormattedValue != "-1.#IND")
                {
                    return cell.FormattedValue;
                }
                else
                {
                    return GridConsts.errorDivZero;
                }
            }

            string result = cell.Value.ToString();
            //string result = CellProperties.GetMemberPropertyValue(cell.Value);

            //���������� ���� ����� �������
            string sDigitCount = format.DigitCount.ToString();
            //���������� ������� ��������� � ������
            bool isDisplayUnit = format.UnitDisplayType == UnitDisplayType.DisplayAtValue;

            switch (format.FormatType)
            {
                //��� ��������������
                case FormatType.None:
                    {
                        break;
                    }
                //��������
                case FormatType.Currency:
                    {
                        Decimal cValue;
                        
                        if (decimal.TryParse(result, NumberStyles.Any, null, out cValue))
                        {
                            //���� �� ���������� ������� ���������, �� �������� �������� � ��������� �������
                            result = cValue.ToString((isDisplayUnit ? "C" : "N") + sDigitCount);
                        }
                        else
                        {
                            result = GridConsts.errorValue;
                        }
                        break;
                    }
                //��������, ���.�.
                case FormatType.ThousandsCurrency:
                    {
                        result = this.GetCurrencyValue(result, sDigitCount, isDisplayUnit ? "���.�." : "", 1000);
                        break;
                    }
                //��������, ���.�. ��� �������
                case FormatType.ThousandsCurrencyWitoutDivision:
                    {
                        result = this.GetCurrencyValue(result, sDigitCount, isDisplayUnit ? "���.�." : "", 0);
                        break;
                    }
                //��������, ���.�.
                case FormatType.MillionsCurrency:
                    {
                        result = this.GetCurrencyValue(result, sDigitCount, isDisplayUnit ? "���.�." : "", 1000000);
                        break;
                    }
                //��������, ���.�. ��� �������
                case FormatType.MillionsCurrencyWitoutDivision:
                    {
                        result = this.GetCurrencyValue(result, sDigitCount, isDisplayUnit ? "���.�." : "", 0);
                        break;
                    }
                //��������, ����.�.
                case FormatType.MilliardsCurrency:
                    {
                        result = this.GetCurrencyValue(result, sDigitCount, isDisplayUnit ? "����.�." : "", 1000000000);
                        break;
                    }
                //��������, ����.�. ��� �������
                case FormatType.MilliardsCurrencyWitoutDivision:
                    {
                        result = this.GetCurrencyValue(result, sDigitCount, isDisplayUnit ? "����.�." : "", 0);
                        break;
                    }
                //����������
                case FormatType.Percent:
                    {
                        double pValue;
                        if (double.TryParse(result, NumberStyles.Any, null, out pValue))
                        {
                            if (isDisplayUnit)
                            {
                                result = pValue.ToString("P" + sDigitCount);
                            }
                            else
                            {
                                pValue *= 100;
                                result = pValue.ToString("N" + sDigitCount);
                            }
                        }
                        else
                        {
                            result = GridConsts.errorValue;
                        }
                        break;
                    }
                //��������
                case FormatType.Numeric:
                    {
                        double nValue;
                        if (double.TryParse(result, NumberStyles.Any, null, out nValue))
                        {
                            result = nValue.ToString("N" + sDigitCount);
                        }
                        else
                        {
                            result = GridConsts.errorValue;
                        }
                        break;
                    }
                //��������, ���.
                case FormatType.ThousandsNumeric:
                    {
                        result = this.GetNumericValue(result, sDigitCount, isDisplayUnit ? "���." : "", 1000);
                        break;
                    }
                //��������, ���.
                case FormatType.MillionsNumeric:
                    {
                        result = this.GetNumericValue(result, sDigitCount, isDisplayUnit ? "���." : "", 1000000);
                        break;
                    }
                //��������, ����.
                case FormatType.MilliardsNumeric:
                    {
                        result = this.GetNumericValue(result, sDigitCount, isDisplayUnit ? "����." : "", 1000000000);
                        break;
                    }
                //����������������
                case FormatType.Exponential:
                    {
                        double eValue;
                        if (double.TryParse(result, NumberStyles.Any, null, out eValue))
                        {
                            result = eValue.ToString("E" + sDigitCount);
                        }
                        else
                        {
                            result = GridConsts.errorValue;
                        }
                        break;
                    }
                //���� �����
                case FormatType.ShortDate:
                case FormatType.ShortTime:
                case FormatType.LongDate:
                case FormatType.LongTime:
                case FormatType.DateTime:
                    {
                        DateTime dateTime;
                        if (!DateTime.TryParse(result, out dateTime))
                        {
                            //���� ��������� ����������(�� ������� DateTime), ������� ������ ������� (�� long)...
                            long uValue;
                            if (long.TryParse(result, NumberStyles.Any, null, out uValue))
                            {
                                dateTime = DateTime.FromBinary(uValue);
                            }
                        }

                        if (dateTime != null)
                        {
                            switch (format.FormatType)
                            {
                                case FormatType.ShortDate: { result = dateTime.ToShortDateString(); break; }
                                case FormatType.ShortTime: { result = dateTime.ToShortTimeString(); break; }
                                case FormatType.LongDate: { result = dateTime.ToLongDateString(); break; }
                                case FormatType.LongTime: { result = dateTime.ToLongTimeString(); break; }
                                case FormatType.DateTime: { result = dateTime.ToString(); break; }
                            }
                        }
                        break;
                    }
                //��/���
                //������/����
                case FormatType.YesNo:
                case FormatType.TrueFalse:
                    {
                        double d;
                        //���� �������� �����, ����� ��� ��� ����� 0 - false, ��������� true
                        if (double.TryParse(result, NumberStyles.Any, null, out d))
                        {
                            if (format.FormatType == FormatType.YesNo)
                                result = (d == 0) ? "���" : "��";
                            else
                                result = (d == 0) ? "����" : "������";
                        }
                        else
                        {
                            bool eValue;
                            //��� �� ��������� ���������� ������, ����� ��� ���������� ��������� 
                            //��������(true/false)
                            if (bool.TryParse(result, out eValue))
                            {
                                if (format.FormatType == FormatType.YesNo)
                                    result = eValue ? "��" : "���";
                                else
                                    result = eValue ? "������" : "����";
                            }
                            else
                            {
                                result = GridConsts.errorValue;
                            }
                        }
                        break;
                    }
            }

            //������� ����������� �������� � ������ ���� ����������� �������������� �����
            if ((!format.ThousandDelimiter) && (format.FormatType != FormatType.DateTime))
            {
                result = result.Replace(this.MeasureData.MeasuresData.NumberFormat.CurrencyGroupSeparator, "");
            }

            return result;
        }

        /// <summary>
        /// ������ ���� ������
        /// </summary>
        public new void Clear()
        {
            this.MeasureData = null;
            this.RowCell = null;
            this.Value = null;
            base.Clear();
        }

        /// <summary>
        /// ���� � ������ ������ ������������ �������� �����, ������ true
        /// </summary>
        public bool IsTotal
        {
            get
            {
                if ((this.RowCell != null) && (this.RowCell.IsTotal))
                    return true;
                DimensionCell dimCell = this.CaptionsSection.ColumnCell;
                if ((dimCell != null) && (dimCell.IsTotal))
                    return true;
                return false;
            }
        }


        /// <summary>
        /// ���� � ������ ������ ������������ ������� ��������, ������ true
        /// </summary>
        public bool IsAverage
        {
            get { return ((this.RowCell != null) && (this.RowCell.IsAverage)); }
        }

        /// <summary>
        /// ���� � ������ ������ ������������ ����������� ����������, ������ true
        /// </summary>
        public bool IsStandartDeviation
        {
            get { return ((this.RowCell != null) && (this.RowCell.IsStandartDeviation)); }
        }

        /// <summary>
        /// ���� � ������ ������ ������������ �������, ������ true
        /// </summary>
        public bool IsMedian
        {
            get { return ((this.RowCell != null) && (this.RowCell.IsMedian)); }
        }



        /// <summary>
        /// ������������� ���������� ������
        /// </summary>
        /// <returns>Rectangle</returns>
        public override Rectangle GetOffsetBounds()
        {
            Rectangle result = this.GetBounds();
            result.X -= this.Grid.HScrollBarState.Offset;
            result.Y -= this.Grid.VScrollBarState.Offset;

            return result;
        }

        /// <summary>
        /// �.�. ������ ���������� ��������� �������� �� ���������� 
        /// ����������� ������ �� ��������� ������ ������ � ������ � �������
        /// </summary>
        /// <returns></returns>
        public override Rectangle GetBounds()
        {
            Rectangle result = new Rectangle();
            result.X = this.MeasureData.Location.X;
            result.Y = (this.RowCell != null) ? this.RowCell.Location.Y 
                : this.MeasureData.MeasureCaption.Bounds.Bottom;
            result.Width = this.MeasureData.Width;
            result.Height = (this.RowCell != null) ? this.RowCell.Height
                : this.MeasureData.MeasuresData.Style.TextHeight;
            return result;
        }

        /// <summary>
        /// �������� �����������
        /// </summary>
        /// <returns></returns>
        public override string GetComment()
        {
            string result = string.Empty;
            try
            {
                if ((this.Value != null) && (this.Value.Value != null))
                {
                    result = string.Format("�������� �� ����: {0}\n\n", this.Value.FormattedValue);
                }
            }
            catch
            {
                result = string.Format("�������� �� ����: {0}\n\n", GridConsts.errorValue);
            }

            if (this.RowCell != null)
            {
                result += string.Format("��������� ������: {0}\n\n", this.GetHierarchyCell(this.RowCell));
            }
            if (this.CaptionsSection.ColumnCell != null)
            {
                result += string.Format("��������� �������: {0}\n\n", 
                    this.GetHierarchyCell(this.CaptionsSection.ColumnCell));
            }
            result += string.Format("����: {0}", this.MeasureData.MeasureCaption.Text);
            return result;
        }

        //�������� ���� �������� ���� ������� ���������
        private string GetHierarchyCell(DimensionCell cell)
        {
            string result = string.Empty;
            while (cell != null)
            {
                //� ����������� ����������� ����� ���� ������� ���������, "�����"(���� �� �� "����� ����") - � 
                //�� ��������
                result = (((cell.Text != string.Empty)
                    && (!cell.IsTotal || cell.IsGrandTotal))
                    ? " - " + cell.Text
                    : string.Empty) + result;
                cell = cell.GetVisibleParent();
            }
            if (result != string.Empty)
            {
                result = result.Substring(3, result.Length - 3);
            }
            return result;
        }

        /// <summary>
        /// �������������� ������
        /// </summary>
        public void DrillTrought(string actionName)
        {
            //���������� ��� ����
            string measureUN = this.MeasureData.MeasureCaption.UniqueName;
            //���������� ��� ������ �����
            string rowCellUN = this.RowCell == null ? string.Empty :
                this.RowCell.ClsMember.UniqueName;
            //�������� �� ������� ����� ������
            bool rowCellIsTotal = this.RowCell == null ? false : this.RowCell.IsGrandTotal; // IsTotal;
            //���������� ��� ������ �������
            string columnCellUN = this.CaptionsSection.ColumnCell == null ? string.Empty :
                this.CaptionsSection.ColumnCell.ClsMember.UniqueName;
            //�������� �� ������ ������� ������
            bool columnCellIsTotal = this.CaptionsSection.ColumnCell == null
                                         ? false
                                         : this.CaptionsSection.ColumnCell.IsGrandTotal; // IsTotal;

            this.Grid.OnDrillThrough(measureUN, rowCellUN, rowCellIsTotal, columnCellUN, columnCellIsTotal, actionName);
        }

        /// <summary>
        /// ������ �� ������ � ���� ����, ����� ��� �� ����������� ������� ����������� ��������� ����������,
        /// ���� ��������� ���������, ���������� ����
        /// </summary>
        public Cell Value
        {
            get
            {
                if (this.CaptionsSection.IsDummy)
                {
                    return null;
                }
                else
                {
                    return this._value;
                }
            }
            set { this._value = value; }
        }

        /// <summary>
        /// ������ �� ������ � �������, ������� ����������� ������ ������
        /// </summary>
        public MeasureData MeasureData
        {
            get { return this._measureData; }
            set { this._measureData = value; }
        }

        /// <summary>
        /// ������ ������, � ������� ������������� ������ ��������� �������� 
        /// �������� � ������������ � ������ ����������
        /// </summary>
        public DimensionCell RowCell
        {
            get { return this._rowCell; }
            set { this._rowCell = value; }
        }

        /// <summary>
        /// ������ ������
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return (this.RowCell == null) ? true : this.RowCell.IsVisibleLeaf;
            }
        }

        /// <summary>
        /// ������ �� ������ ����������, � ������� ����������� ������ ������
        /// </summary>
        public MeasuresCaptionsSection CaptionsSection
        {
            get 
            {
                return this.MeasureData.MeasureCaption.Captions as MeasuresCaptionsSection; 
            }
        }

        /// <summary>
        /// ������ �� ������ � k-������
        /// </summary>
        public bool IsTopCell
        {
            get { return _isTopCell; }
            set { _isTopCell = value; }
        }

        /// <summary>
        /// ������ �� ������ � k-���������
        /// </summary>
        public bool IsBottomCell
        {
            get { return _isBottomCell; }
            set { _isBottomCell = value; }
        }


    }
}
