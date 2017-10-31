using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Infragistics.UltraGauge.Resources;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class  MultiGaugeColorRangeBrowseClass : FilterablePropertyBase
    {
        #region Поля

        //private LinearGauge _gauge;
        private MultipleGaugeReportElement _gaugeElement;
        private GaugeLegendBrowseClass _legend;

        #endregion

        #region Свойства

        [Browsable(false)]
        public MultipleGaugeReportElement GaugeElement
        {
            get { return this._gaugeElement; }
        }

        //Есть ли хотя бы 1 цветовой интервал
        [Browsable(false)]
        public bool IsRangesExists
        {
            get
            {
                return (this.GaugeElement.ColorRanges.Count > 0);
            }
        }

        [DisplayName("Количество интервалов")]
        [Description("Количество интервалов")]
        [Browsable(true)]
        public int RangeCount
        {
            get { return this.GaugeElement.ColorRanges.Count; }
            set { SetRangeCount(value); }
        }

        /*
        [Description("Цвета")]
        [DisplayName("Цвета")]
        [Editor(typeof(LinearGaugeRangeCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public LinearGaugeRangeCollection RangeColors
        {
            get
            {
                return this._gauge.Scales[0].Ranges;
            }
        }*/
        
        [DisplayName("Свойства интервалов")]
        [Description("Свойства интервалов")]
        [Editor(typeof(MultiGaugeRangeEditor), typeof(UITypeEditor))]
        [DynamicPropertyFilter("IsRangesExists", "True")]
        [Browsable(true)]
        public GaugeColorRangeCollection RangeLimits
        {
            get { return this.GaugeElement.ColorRanges; }
            set { }
        }


        [DisplayName("Легенда")]
        [Description("Легенда")]
        [Browsable(true)]
        public GaugeLegendBrowseClass Legend
        {
            get { return this._legend; }
            set { this._legend = value; }
        }



        #endregion

        public MultiGaugeColorRangeBrowseClass(MultipleGaugeReportElement gaugeElement)
        {
            this._gaugeElement = gaugeElement;

            this._legend = new GaugeLegendBrowseClass(gaugeElement.Legend);
        }
        /*
        private List<double> GetRangeLimits()
        {
            List<double> result = new List<double>();

            if (this._gauge. Scales[0].Ranges.Count > 0)
            {
                foreach (LinearGaugeRange range in this._gauge.Scales[0].Ranges)
                {
                    result.Add((double) range.StartValue);
                }
                result.Add((double)this._gauge.Scales[0].Ranges[this._gauge.Scales[0].Ranges.Count - 1].EndValue);
            }
            return result;
        }

        private void SetRangeLimits(List<double> value)
        {
            for(int i = 0; i < value.Count - 1; i++)
            {
                this._gauge.Scales[0].Ranges[i].StartValue = value[i];
                this._gauge.Scales[0].Ranges[i].EndValue = value[i + 1];
            }
        }
        

        private void SetRangeCount(int value)
        {
            SetRangeCount(this._gauge, value);
            foreach(ExpertGauge gauge in this._gaugeElement.Gauges)
            {
                LinearGauge lGauge = (LinearGauge)gauge.Gauges[0];
                SetRangeCount(lGauge, value);
            }
        }
        */

        /// <summary>
        /// Задаем кол-во интервалов
        /// </summary>
        /// <param name="value"></param>
        private void SetRangeCount(int value)
        {
            //удаляем лишние
            for (int i = this.GaugeElement.ColorRanges.Count - 1; i >= value; i--)
            {
                this.GaugeElement.ColorRanges.RemoveAt(i);
            }

            //добавляем новые
            double startValue = this._gaugeElement.StartValue;
            double rangeLength = (this._gaugeElement != null) ? GetRangeLength(this._gaugeElement, value) : 0;

            for (int i = 0; i < value; i++)
            {
                if (i > (this.GaugeElement.ColorRanges.Count-1))
                {
                    this.GaugeElement.ColorRanges.Add(new GaugeColorRange(0, 0, Color.Empty, String.Empty));
                }

                GaugeColorRange range = this.GaugeElement.ColorRanges[i];
                range.StartValue = startValue;
                range.EndValue = startValue + rangeLength;
                startValue += rangeLength;
                range.Color = GetColorForRange(i, value);
            }

            if (this.GaugeElement.ColorRanges.Count > 0)
            {
                this.GaugeElement.ColorRanges[this.GaugeElement.ColorRanges.Count - 1].EndValue = this.GaugeElement.EndValue;
            }

            this.GaugeElement.RefreshColorRanges();
            this.GaugeElement.InitLegend();
        }

        private double GetRangeLength(MultipleGaugeReportElement gaugeElement, int rangeCount)
        {
            return (gaugeElement.EndValue - gaugeElement.StartValue) / (double)(rangeCount);
        }

        private Color GetColorForRange(int index, int rangeCount)
        {
            if (rangeCount < 1)
                return Color.Empty;

            int colorRange = (rangeCount == 1) ? 510 : 510 / (rangeCount - 1);
            int curRange = index * colorRange;
            Color curColor = (curRange < 255) ? Color.FromArgb(curRange, 255, 0) : Color.FromArgb(255, 510 - curRange, 0);

            return curColor;
        }


        public override string ToString()
        {
            return String.Empty;
        }

    }
       
}