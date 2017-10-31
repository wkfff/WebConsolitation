using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using Dundas.Maps.WinControl;
using System.Drawing;
using System.Drawing.Design;
using Infragistics.UltraGauge.Resources;
using Krista.FM.Client.MDXExpert;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(PropertySorter))]
    public class GaugeRangeBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private GaugeRange _range;
        private GaugeReportElement _gaugeElement;

        #endregion

        #region Свойства

        [PropertyOrder(1)]
        [Description("Цвет интервала")]
        [DisplayName("Цвет интервала")]
        [Browsable(true)]
        public Color Color
        {
            get
            {
                if ((this._range.BrushElement != null) && (this._range.BrushElement is SolidFillBrushElement))
                {
                    return ((SolidFillBrushElement) this._range.BrushElement).Color;
                }
                else
                    return Color.Empty;
            }
            set
            {
                if ((this._range.BrushElement != null) && (this._range.BrushElement is SolidFillBrushElement))
                {
                    ((SolidFillBrushElement) this._range.BrushElement).Color = value;
                    //this._gaugeElement.InitLegend();
                }
            }
        }


        [PropertyOrder(2)]
        [Description("Текст интервала")]
        [DisplayName("Текст")]
        [Browsable(true)]
        public string Text
        {
            get
            {
                return this._range.Key;
            }
            set
            {
                try
                {
                    this._range.Key = value;
                    //this._gaugeElement.InitLegend();
                }
                catch
                {
                    MessageBox.Show(String.Format("Интервал \"{0}\" уже существует. Задайте другое значение."),
                        "MDX Эксперт", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        
        [PropertyOrder(3)]
        [Description("Начальное значение")]
        [DisplayName("Начальное значение")]
        [Browsable(true)]
        public double StartValue
        {
            get { return GetStartValue(); }
            //set { SetStartValue(value); }
        }

        [PropertyOrder(4)]
        [Description("Конечное значение")]
        [DisplayName("Конечное значение")]
        [Browsable(true)]
        public double EndValue
        {
            get { return GetEndValue(); }
            //set { SetEndValue(value); }
        }
        
        #endregion

        public GaugeRangeBrowseClass(GaugeRange range, GaugeReportElement gaugeElement)
        {
            this._gaugeElement = gaugeElement;
            this._range = range;
        }

        private double GetStartValue()
        {
            if (this._range.StartValue != null)
            {
                return Double.Parse(this._range.StartValue.ToString());
            }
            else
                return 0;
        }

        private void SetStartValue(double value)
        {
            this._range.StartValue = value;
            if (this._range.EndValue == null)
                this._range.EndValue = 0;

        }

        private double GetEndValue()
        {
            if (this._range.EndValue != null)
            {
                return Double.Parse(this._range.EndValue.ToString());
            }
            else
                return 0;
        }

        private void SetEndValue(double value)
        {
            this._range.EndValue = value;
            if (this._range.StartValue == null)
                this._range.StartValue = 0;

        }


        public override string ToString()
        {
            return String.Empty;
        }

    }


}
