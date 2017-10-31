using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using Infragistics.UltraGauge.Resources;
using Infragistics.Win.UltraWinGauge;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Controls;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Grid.UserInterface;
using Krista.FM.Common.Xml;
using Microsoft.AnalysisServices.AdomdClient;
using System.Drawing;
using System.Xml;
using System.IO;
using Measure = Infragistics.UltraGauge.Resources.Measure;

namespace Krista.FM.Client.MDXExpert
{
    public class ExpertGauge : UltraGauge
    {
        private string _presetName;
        private bool _autoTickmarkCalculation;
        private bool _isNeedRefresh;
        private ExpertLegend _legend;
        private GaugeColorRangeCollection _colorRanges;
        private bool _isLoading;

        private EventHandler _rangeCollectionChanged = null;

        /// <summary>
        /// Имя текущей настройки для индикатора
        /// </summary>
        public string PresetName
        {
            get { return this._presetName; }
            set
            {
                this._presetName = value;
                try
                {
                    this.SuspendLayout();
                    //при смене шаблона индикатора сохраняем текущие значения и формат
                    double startValue = this.StartValue;
                    double endValue = this.EndValue;
                    double currValue = this.Value;
                    double tickMarkInterval = this.TickmarkInterval;

                    string labelsFormatString = GetLabelsFormaString();
                    string text = this.Text;
                    object tag = this.Tag;
                    Font textFont = this.TextFont;
                    Color textColor = this.TextColor;

                    Font labelsFont = this.GetLabelsFont();

                    GaugeColorRangeCollection colorRanges = this.ColorRanges;

                    LoadPreset(String.Format(Application.StartupPath + "\\GaugePresets\\{0}.xml", value));

                    SetValuesWithoutCalculation(startValue, endValue, currValue, tickMarkInterval);

                    //this.TickmarkInterval = tickMarkInterval;
                    SetLabelsFormatString(labelsFormatString);
                    if (labelsFont != null)
                        SetLabelsFont(labelsFont);

                    this.Tag = tag;
                    this.Text = text;
                    this.TextFont = textFont;
                    if (textColor != Color.Empty)
                        this.TextColor = textColor;

                    this.ColorRanges = colorRanges;
                }
                catch
                {}
                finally
                {
                    this.ResumeLayout();
                }
                //this.Refresh();
            }
        }

        public string LabelsFormatString
        {
            get { return GetLabelsFormaString(); }
            set { SetLabelsFormatString(value);}
        }

        public Font LabelsFont
        {
            get { return GetLabelsFont(); }
            set { SetLabelsFont(value); }
        }

        /// <summary>
        /// Начальное значение
        /// </summary>
        public double StartValue
        {
            get { return GetStartValue(); }
            set { SetStartValue(value); }
        }

        /// <summary>
        /// Конечное значение
        /// </summary>
        public double EndValue
        {
            get { return GetEndValue(); }
            set { SetEndValue(value); }
        }

        /// <summary>
        /// Значение
        /// </summary>
        public double Value
        {
            get { return GetValue(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Признак что отчет загружается
        /// </summary>
        public bool IsLoading
        {
            get { return this._isLoading; }
            set { this._isLoading = value; }
        }

        /// <summary>
        /// Подпись
        /// </summary>
        public string Text
        {
            get { return GetText(); }
            set { SetText(value); }
        }

        /// <summary>
        /// Цвет подписи
        /// </summary>
        public Color TextColor
        {
            get
            {
                return GetTextColor();
            }
            set
            {
                SetTextColor(value);
            }
        }

        /// <summary>
        /// Шрифт подписи
        /// </summary>
        public Font TextFont
        {
            get { return GetTextFont(); }
            set { SetTextFont(value); }
        }

        /// <summary>
        /// Интервал между метками
        /// </summary>
        public double TickmarkInterval
        {
            get { return GetTickmarkInterval(); }
            set { SetTickmarkInterval(value); }
        }

        /// <summary>
        /// Автоматический расчет интервала между метками
        /// </summary>
        public bool AutoTickmarkCalculation
        {
            get { return _autoTickmarkCalculation; }
            set
            {
                _autoTickmarkCalculation = value;
                if (value)
                {
                   /* if (!String.IsNullOrEmpty(this.Synchronization.BoundTo))
                    {
                        TableReportElement tableElement = this.MainForm.FindTableReportElement(this.Synchronization.BoundTo);
                        if (tableElement != null)
                        {
                            tableElement.UpdateAnchoredGauge();
                        }
                    }
                    else*/
                    {
                        double startValue = this.StartValue;
                        double endValue = this.EndValue;
                        TickmarkInterval = CalcTickMarkInterval(ref startValue, ref endValue, 10);
                        this.StartValue = startValue;
                        this.EndValue = endValue;
                    }
                }
            }
        }

        public Margin Margin
        {
            get 
            { 
                if (this.Gauges.Count > 0)
                {
                    return this.Gauges[0].Margin;
                }
                else
                {
                    return new Margin();
                }
            }
            set
            {
                SetMargin(value);
            }
        }

        /// <summary>
        /// Нужно ли перерисовывать индикатор
        /// </summary>
        public bool IsNeedRefresh
        {
            get { return _isNeedRefresh; }
            set { _isNeedRefresh = value; }
        }

        /// <summary>
        /// Цветовые интервалы с указанными пользовательскими границами, цветами и названиями
        /// </summary>
        public GaugeColorRangeCollection ColorRanges
        {
            get { return this._colorRanges; }
            set
            {
                this._colorRanges = value;
                SetColorRanges(value);
            }
        }
        
        
        /// <summary>
        /// Цветовые интервалы, которые реально отображаются на индикаторе
        /// </summary>
        public List<GaugeRange> VisibleColorRanges
        {
            get { return GetVisibleColorRanges(); }
            set { SetVisibleColorRanges(value); }
        }





        public ExpertGauge(string presetName)
        {
            //this.MouseClick += new MouseEventHandler(gauge_MouseClick);
            //Восстановим отрисовку
            this.IsNeedRefresh = true;
            this.IsLoading = false;

            this.SizeChanged += new EventHandler(ExpertGauge_SizeChanged);

            this.ColorRanges = new GaugeColorRangeCollection();

            string firstPreset = presetName;
            if (!String.IsNullOrEmpty(firstPreset))
            {
                this.PresetName = firstPreset;
                this.StartValue = 0;
                this.EndValue = 100;
                this.Value = 50;
            }

            this.AutoTickmarkCalculation = true;
        }

        void ExpertGauge_SizeChanged(object sender, EventArgs e)
        {
            RefreshMargin();
        }

        /// <summary>
        /// Расчет полей индикатора, если тип измерения - пикселы
        /// </summary>
        private void RefreshMargin()
        {
            if (this.Gauges.Count > 0)
            {
                if (this.Gauges[0].Margin.Measure == Measure.Pixels)
                {
                    SetMargin(this.Margin);
                }
            }
        }

        /// <summary>
        /// Проверка корректности размера интервала на шкале индикатора
        /// </summary>
        private void CheckTickMarkInterval()
        {
            if(!this.AutoTickmarkCalculation)
            {
                if (((this.EndValue - this.StartValue)/this.TickmarkInterval) > 100)
                {
                    this.AutoTickmarkCalculation = true;

                    if (!this.IsLoading)
                    {
                        MessageBox.Show(
                            "Текущее значение размера интервалов некорректно. Расчет интервалов переведен в автоматический режим.",
                            "MDX Эксперт", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        void gauge_MouseClick(object sender, MouseEventArgs e)
        {

        }

        /// <summary>
        /// Получение начального значения шкалы индикатора
        /// </summary>
        /// <returns></returns>
        private double GetStartValue()
        {
            if (this.Gauges[0] is RadialGauge)
            {
                return (double)((RadialGauge)this.Gauges[0]).Scales[0].Axis.GetStartValue();
            }
            else
                if (this.Gauges[0] is LinearGauge)
                {
                    return (double)((LinearGauge)this.Gauges[0]).Scales[0].Axis.GetStartValue();
                }

            return 0;
        }

        /// <summary>
        /// Получение конечного значения шкалы индикатора
        /// </summary>
        /// <returns></returns>
        private double GetEndValue()
        {
            if (this.Gauges[0] is RadialGauge)
            {
                return (double)((RadialGauge)this.Gauges[0]).Scales[0].Axis.GetEndValue();
            }
            else
                if (this.Gauges[0] is LinearGauge)
                {
                    return (double)((LinearGauge)this.Gauges[0]).Scales[0].Axis.GetEndValue();
                }
            return 0;
        }

        /// <summary>
        /// Получение текущего значения идикатора
        /// </summary>
        /// <returns></returns>
        private double GetValue()
        {
            if (this.Gauges[0] is RadialGauge)
            {
                foreach(RadialGaugeScale scale in ((RadialGauge)this.Gauges[0]).Scales)
                {
                    if (scale.Markers.Count > 0)
                    {
                        return (double)scale.Markers[0].Value;
                    }
                }
                return 0;
            }
            else
                if (this.Gauges[0] is LinearGauge)
                {
                    foreach (LinearGaugeScale scale in ((LinearGauge)this.Gauges[0]).Scales)
                    {
                        if (scale.Markers.Count > 0)
                        {
                            return (double)scale.Markers[0].Value;
                        }
                    }
                    return 0;
                }
            return 0;
        }

        /// <summary>
        /// Получение текущего формата меток
        /// </summary>
        /// <returns></returns>
        private string GetLabelsFormaString()
        {
            string result = "<DATA_VALUE:0>";

        if (this.Gauges[0] is RadialGauge)
            {
                return ((RadialGauge) this.Gauges[0]).Scales[0].Labels.FormatString;
            }
            else
                if (this.Gauges[0] is LinearGauge)
                {
                    return ((LinearGauge)this.Gauges[0]).Scales[0].Labels.FormatString;
                }

            return result;
        }

        /// <summary>
        /// Установка формата меток
        /// </summary>
        /// <param name="value"></param>
        private void SetLabelsFormatString(string value)
        {
            if (this.Gauges[0] is RadialGauge)
            {
                ((RadialGauge)this.Gauges[0]).Scales[0].Labels.FormatString = value;
            }
            else
                if (this.Gauges[0] is LinearGauge)
                {
                    ((LinearGauge)this.Gauges[0]).Scales[0].Labels.FormatString = value;
                }
        }


        /// <summary>
        /// Получение текущего формата меток
        /// </summary>
        /// <returns></returns>
        private Font GetLabelsFont()
        {
            Font result = null;

            if (this.Gauges[0] is RadialGauge)
            {
                return ((RadialGauge)this.Gauges[0]).Scales[0].Labels.Font;
            }
            else
                if (this.Gauges[0] is LinearGauge)
                {
                    return ((LinearGauge)this.Gauges[0]).Scales[0].Labels.Font;
                }

            return result;
        }

        /// <summary>
        /// Установка формата меток
        /// </summary>
        /// <param name="value"></param>
        private void SetLabelsFont(Font value)
        {
            if (this.Gauges[0] is RadialGauge)
            {
                ((RadialGauge)this.Gauges[0]).Scales[0].Labels.Font = value;
            }
            else
                if (this.Gauges[0] is LinearGauge)
                {
                    ((LinearGauge)this.Gauges[0]).Scales[0].Labels.Font = value;
                }
        }


        private void SetMargin(Margin value)
        {
            if (this.Gauges.Count > 0)
            {
                this.SuspendLayout();
                this.Gauges[0].Margin = value;

                int top = 0;
                int left = 0;
                int height = 0;
                int width = 0;

                Rectangle newBounds = new Rectangle();

                
                if (value.Measure == Measure.Percent)
                {
                    height = (int)((100 / (100 - value.Top - value.Bottom)) * (value.Top + value.Bottom));
                    top = (int)((value.Top / (value.Top + value.Bottom)) * height);
                    width = (int)((100 / (100 - value.Left - value.Right)) * (value.Left + value.Right));
                    left = (int)((value.Left / (value.Left + value.Right)) * width);
                    newBounds = new Rectangle(-left + 1, -top + 1, width + 100 - 2, height + 100 - 2);
                }
                else
                {
                    top = ((int)value.Top);
                    left = ((int)value.Left);
                    newBounds = new Rectangle(-left + 2, -top + 2, this.Width - 4, this.Height - 4);
                }

                if (this.Gauges[0].BrushElement != null)
                {
                    if (this.Gauges[0].BrushElement is BrushElementGroup)
                    {
                        BrushElementGroup brElements = (BrushElementGroup) this.Gauges[0].BrushElement;
                        foreach(BrushElement brElem in brElements.BrushElements)
                        {
                            brElem.RelativeBounds = newBounds;
                            brElem.RelativeBoundsMeasure = value.Measure;
                        }
                    }
                    else
                    {
                        this.Gauges[0].BrushElement.RelativeBounds = newBounds;
                        this.Gauges[0].BrushElement.RelativeBoundsMeasure = value.Measure;
                    }
                }

                if ((this.Gauges[0].StrokeElement != null) && (this.Gauges[0].StrokeElement.BrushElement != null))
                {
                    this.Gauges[0].StrokeElement.BrushElement.RelativeBounds = newBounds;
                    this.Gauges[0].StrokeElement.BrushElement.RelativeBoundsMeasure = value.Measure;
                }
            }


            this.ResumeLayout();
        }

        /// <summary>
        /// Установка начального значения для шкалы индикатора
        /// </summary>
        /// <param name="value"></param>
        private void SetStartValue(double value)
        {
            double endValue = this.EndValue;
            
            if (value >= this.EndValue)
                endValue = value + 1;

            if (value > this.Value)
                this.Value = value;

            if (this.AutoTickmarkCalculation)
            {
                double tickMarkInterval = CalcTickMarkInterval(ref value, ref endValue, 10);
                if (this.Gauges[0] is RadialGauge)
                {
                    foreach (RadialGaugeScale scale in ((RadialGauge)this.Gauges[0]).Scales)
                    {
                        if (scale.Axis != null)
                        {
                            scale.Axis.SetTickmarkInterval(tickMarkInterval);
                        }
                    }
                }
                else
                    if (this.Gauges[0] is LinearGauge)
                    {
                        foreach (LinearGaugeScale scale in ((LinearGauge) this.Gauges[0]).Scales)
                        {
                            if (scale.Axis != null)
                            {
                                scale.Axis.SetTickmarkInterval(tickMarkInterval);
                            }
                        }
                    }
            }

            if (this.Gauges[0] is RadialGauge)
            {
                foreach (RadialGaugeScale scale in ((RadialGauge)this.Gauges[0]).Scales)
                {
                    if (scale.Axis != null)
                    {
                        scale.Axis.SetEndValue(endValue);
                        scale.Axis.SetStartValue(value);
                        if (scale.Ranges.Count > 0)
                        {
                            scale.Ranges[0].StartValue = value;
                            scale.Ranges[scale.Ranges.Count - 1].EndValue = endValue;
                        }
                    }
                }
            }
            else
                if (this.Gauges[0] is LinearGauge)
                {
                    foreach (LinearGaugeScale scale in ((LinearGauge)this.Gauges[0]).Scales)
                    {
                        if (scale.Axis != null)
                        {

                            scale.Axis.SetEndValue(endValue);
                            scale.Axis.SetStartValue(value);
                            if (scale.Ranges.Count > 0)
                            {
                                scale.Ranges[0].StartValue = value;
                                scale.Ranges[scale.Ranges.Count - 1].EndValue = endValue;
                            }
                        }
                    }
                }

            CheckTickMarkInterval();

            RefreshColorRanges();
            RefreshAppearance();

            if (this.IsNeedRefresh)
                this.Refresh();
        }

        /// <summary>
        /// Установка начального, конечного и текущего значений
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="value"></param>
        public void SetValues(double startValue, double endValue, double value)
        {
            if (startValue >= endValue)
                endValue = startValue + 1;

            if (startValue > value)
                value = startValue;


            if (this.AutoTickmarkCalculation)
            {
                double tickMarkInterval = CalcTickMarkInterval(ref startValue, ref endValue, 10);
                if (this.Gauges[0] is RadialGauge)
                {
                    foreach (RadialGaugeScale scale in ((RadialGauge)this.Gauges[0]).Scales)
                    {
                        if (scale.Axis != null)
                        {
                            scale.Axis.SetTickmarkInterval(tickMarkInterval);
                        }
                    }
                }
                else
                    if (this.Gauges[0] is LinearGauge)
                    {
                        foreach (LinearGaugeScale scale in ((LinearGauge) this.Gauges[0]).Scales)
                        {
                            if (scale.Axis != null)
                            {
                                scale.Axis.SetTickmarkInterval(tickMarkInterval);
                            }
                        }
                    }
            }

            if (this.Gauges[0] is RadialGauge)
            {
                foreach (RadialGaugeScale scale in ((RadialGauge)this.Gauges[0]).Scales)
                {
                    if (scale.Axis != null)
                    {
                        scale.Axis.SetEndValue(endValue);
                        scale.Axis.SetStartValue(startValue);
                        if (scale.Ranges.Count > 0)
                        {
                            scale.Ranges[0].StartValue = startValue;
                            scale.Ranges[scale.Ranges.Count - 1].EndValue = endValue;
                        }
                    }
                }
            }
            else
                if (this.Gauges[0] is LinearGauge)
                {
                    foreach (LinearGaugeScale scale in ((LinearGauge)this.Gauges[0]).Scales)
                    {
                        if (scale.Axis != null)
                        {
                            scale.Axis.SetEndValue(endValue);
                            scale.Axis.SetStartValue(startValue);
                            if (scale.Ranges.Count > 0)
                            {
                                scale.Ranges[0].StartValue = startValue;
                                scale.Ranges[scale.Ranges.Count - 1].EndValue = endValue;
                            }
                        }
                    }
                }

            CheckTickMarkInterval();

            this.SetValue(value);
            RefreshColorRanges();
            RefreshAppearance();

            if (this.IsNeedRefresh)
                this.Refresh();

        }

        /// <summary>
        /// Установка значений без расчета
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="valu"></param>
        public void SetValuesWithoutCalculation(double startValue, double endValue, double value, double tickMarkInterval)
        {
            try
            {
                this.IsNeedRefresh = false;
                if (this.Gauges[0] is RadialGauge)
                {
                    foreach (RadialGaugeScale scale in ((RadialGauge)this.Gauges[0]).Scales)
                    {
                        if (scale.Axis != null)
                        {
                            scale.Axis.SetEndValue(endValue);
                            scale.Axis.SetStartValue(startValue);
                            scale.Axis.SetTickmarkInterval(tickMarkInterval);
                            if (scale.Ranges.Count > 0)
                            {
                                scale.Ranges[0].StartValue = startValue;
                                scale.Ranges[scale.Ranges.Count - 1].EndValue = endValue;
                            }
                        }
                    }
                }
                else if (this.Gauges[0] is LinearGauge)
                {
                    foreach (LinearGaugeScale scale in ((LinearGauge)this.Gauges[0]).Scales)
                    {
                        if (scale.Axis != null)
                        {
                            scale.Axis.SetEndValue(endValue);
                            scale.Axis.SetStartValue(startValue);
                            scale.Axis.SetTickmarkInterval(tickMarkInterval);
                            if (scale.Ranges.Count > 0)
                            {
                                scale.Ranges[0].StartValue = startValue;
                                scale.Ranges[scale.Ranges.Count - 1].EndValue = endValue;
                            }
                        }
                    }
                }

                this.SetValue(value);
                RefreshAppearance();
                RefreshDigitalGauge();
            }
            finally
            {
                this.IsNeedRefresh = true;
            }
        }

        private double[] sqr = new double[] { 1.414214, 3.162278, 7.071068 };
        private double[] vint = new double[] { 1.0, 2.0, 5.0, 10.0 };
        /// <summary>
        /// Вычисление значения для интервала
        /// </summary>
        /// <param name="xmin"></param>
        /// <param name="xmax"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public double CalcTickMarkInterval(ref double xmin, ref double xmax, int n)
        {
            if (n <= 0)
            {
                return 10;
            }

            if (xmin == xmax)
                return 10;

            double intervalCnt = n;
            double precision = 2E-05;
            double d = Math.Abs((double)(xmax - xmin)) / intervalCnt;
            int digitsCnt = (int)Math.Log10(d);
            if (d < 1.0)
            {
                digitsCnt--;
            }
            double num6 = d / Math.Pow(10.0, (double)digitsCnt);
            int index = 0;
            while ((index < 3) && (num6 >= sqr[index]))
            {
                index++;
            }
            double dataInterval = vint[index] * Math.Pow(10.0, (double)digitsCnt);
            double num9 = xmin / dataInterval;
            long num10 = (long)num9;
            if (num9 < 0.0)
            {
                num10 -= 1L;
            }
            if (Math.Abs((double)((num10 + 1.0) - num9)) < precision)
            {
                num10 += 1L;
            }
            double min = dataInterval * num10;
            double num13 = xmax / dataInterval;
            long num14 = (long)(num13 + 1.0);
            if (num13 < -1.0)
            {
                num14 -= 1L;
            }
            
            if (Math.Abs((double)((num13 + 1.0) - num14)) < precision)
            {
                num14 -= 1L;
            }
            double max = dataInterval * num14;
            if (xmin > min)
            {
                xmin = min;
            }
            if (xmax < max)
            {
                xmax = max;
            }
            while ((xmin + (dataInterval * n)) < xmax)
            {
                dataInterval *= 2.0;
                n = n/2 + 1;
            }

            xmax = xmin + n*dataInterval;
            
            /*
            max = xmin + dataInterval;
            while (xmax - max) > precision)
                max += dataInterval;

            xmax = max;
            */

            return dataInterval;
        }



        /// <summary>
        /// Установка конечного значения для шкалы индикатора
        /// </summary>
        /// <param name="value"></param>
        private void SetEndValue(double value)
        {
            //double tickMarkInterval = (Math.Abs((value - this.StartValue)/10));

            double startValue = this.StartValue;

            if (value <= this.StartValue)
                startValue = value - 1;

            if (value < this.Value)
                this.Value = value;

            double endValue = value;


            if (this.AutoTickmarkCalculation)
            {
                double tickMarkInterval = CalcTickMarkInterval(ref startValue, ref endValue, 10);
                if (this.Gauges[0] is RadialGauge)
                {
                    foreach (RadialGaugeScale scale in ((RadialGauge)this.Gauges[0]).Scales)
                    {
                        if (scale.Axis != null)
                        {
                            scale.Axis.SetTickmarkInterval(tickMarkInterval);
                        }
                    }
                }
                else
                    if (this.Gauges[0] is LinearGauge)
                    {
                        foreach (LinearGaugeScale scale in ((LinearGauge)this.Gauges[0]).Scales)
                        {
                            if (scale.Axis != null)
                            {
                                scale.Axis.SetTickmarkInterval(tickMarkInterval);
                            }
                        }
                    }
            }
            else
            {
                if (((endValue - startValue) / this.TickmarkInterval) <= 100)
                {
                    endValue = startValue;
                    while (endValue < value)
                    {
                        endValue += this.TickmarkInterval;
                    }
                }
            }

            if (this.Gauges[0] is RadialGauge)
            {
                foreach (RadialGaugeScale scale in ((RadialGauge)this.Gauges[0]).Scales)
                {
                    if (scale.Axis != null)
                    {
                        scale.Axis.SetStartValue(startValue);
                        scale.Axis.SetEndValue(endValue);
                        if (scale.Ranges.Count > 0)
                        {
                            scale.Ranges[0].StartValue = startValue;
                            scale.Ranges[scale.Ranges.Count - 1].EndValue = endValue;
                        }

                    }
                }
            }
            else
                if (this.Gauges[0] is LinearGauge)
                {
                    foreach (LinearGaugeScale scale in ((LinearGauge)this.Gauges[0]).Scales)
                    {
                        if (scale.Axis != null)
                        {
                            scale.Axis.SetStartValue(startValue);
                            scale.Axis.SetEndValue(endValue);
                            if (scale.Ranges.Count > 0)
                            {
                                scale.Ranges[0].StartValue = startValue;
                                scale.Ranges[scale.Ranges.Count - 1].EndValue = endValue;
                            }
                        }
                    }
                }

            CheckTickMarkInterval();
            RefreshColorRanges();
            RefreshAppearance();
            
            if (this.IsNeedRefresh)
                this.Refresh();
        }


        /// <summary>
        /// Установка интервала между отметками
        /// </summary>
        /// <param name="value"></param>
        private void SetTickmarkInterval(double value)
        {
            if (value == 0)
                return;

            double minValue = this.StartValue;
            double maxValue = this.EndValue;

            double endValue = minValue;
            while (endValue < maxValue)
                endValue += value;

            if (this.Gauges[0] is RadialGauge)
            {
                foreach (RadialGaugeScale scale in ((RadialGauge)this.Gauges[0]).Scales)
                {
                    if (scale.Axis != null)
                    {
                        scale.Axis.SetEndValue(endValue);
                        scale.Axis.SetTickmarkInterval(value);
                        if (scale.Ranges.Count > 0)
                        {
                            scale.Ranges[scale.Ranges.Count - 1].EndValue = endValue;
                        }
                    }
                }
            }
            else
                if (this.Gauges[0] is LinearGauge)
                {
                    foreach (LinearGaugeScale scale in ((LinearGauge)this.Gauges[0]).Scales)
                    {
                        if (scale.Axis != null)
                        {
                            scale.Axis.SetEndValue(endValue);
                            scale.Axis.SetTickmarkInterval(value);
                            if (scale.Ranges.Count > 0)
                            {
                                scale.Ranges[scale.Ranges.Count - 1].EndValue = endValue;
                            }
                        }
                    }
                }
            CheckTickMarkInterval();

            if (this.IsNeedRefresh)
                this.Refresh();
        }


        /// <summary>
        /// Получить интервал между отметками
        /// </summary>
        /// <param name="value"></param>
        private double GetTickmarkInterval()
        {
            if (this.Gauges[0] is RadialGauge)
            {
                return (double)((RadialGauge) this.Gauges[0]).Scales[0].Axis.GetTickmarkInterval();
            }
            else
                if (this.Gauges[0] is LinearGauge)
                {
                    return (double)((LinearGauge)this.Gauges[0]).Scales[0].Axis.GetTickmarkInterval();
                }
            return 10;
        }



        /// <summary>
        /// Установка текущего значения индикатора
        /// </summary>
        /// <param name="value"></param>
        private void SetValue(double value)
        {
            for (int i = 0; i < this.Gauges.Count; i++)
            {
                if (this.Gauges[i] is RadialGauge)
                {
                    foreach (RadialGaugeScale scale in ((RadialGauge)this.Gauges[0]).Scales)
                    {
                        if (scale.Markers.Count > 0)
                        {
                            scale.Markers[0].Value = value;
                        }
                    }
                }
                else if (this.Gauges[i] is LinearGauge)
                {
                    foreach (LinearGaugeScale scale in ((LinearGauge)this.Gauges[0]).Scales)
                    {
                        if (scale.Markers.Count > 0)
                        {
                            scale.Markers[0].Value = value;
                        }
                    }
                }
                else if (this.Gauges[i] is SegmentedDigitalGauge)
                {
                    string formatStr = "0";

                    if (this.Gauges[0] is RadialGauge)
                    {
                        formatStr = ((RadialGauge)this.Gauges[0]).Scales[0].Labels.FormatString;
                    }
                    else if (this.Gauges[0] is LinearGauge)
                    {
                        formatStr = ((LinearGauge) this.Gauges[0]).Scales[0].Labels.FormatString;
                    }

                    GaugeFormatBrowseClass format = new GaugeFormatBrowseClass(formatStr);
                    format.DisplayUnits = false;
                    formatStr = format.FormatString;

                    formatStr = String.Format("{0:" + formatStr + "}", value);

                    formatStr = formatStr.Replace("%", "");
                    double newValue = Double.Parse(formatStr);

                    ((SegmentedDigitalGauge)this.Gauges[i]).Digits = newValue.ToString().Length > 5 ? value.ToString().Length : 6;
                    ((SegmentedDigitalGauge)this.Gauges[i]).Text = newValue.ToString().Replace(',', '.');
                }
            }
        }

        private void RefreshAppearance()
        {
         /*
            if (this.gauge.Gauges.Count > 0)
            {
                if (this.gauge.Gauges[0] is RadialGauge)
                {
                    RadialGauge rGauge = (RadialGauge)this.gauge.Gauges[0];
                    if (rGauge.Scales.Count > 0)
                    {
                        if (rGauge.Scales[0].Ranges.Count > 0)
                        {
                            rGauge.Scales[0].Ranges[0].StartValue = this.StartValue;
                            rGauge.Scales[0].Ranges[0].EndValue = this.EndValue;
                        }
                    }
                }
            }
            */
        }

        /// <summary>
        /// Обновим значение в семисегментном индикаторе
        /// </summary>
        public void RefreshDigitalGauge()
        {
            foreach (Gauge g in this.Gauges)
            {
                if (g is SegmentedDigitalGauge)
                {
                    this.Value = this.Value;
                    return;
                }
            }
        }



        private string GetText()
        {
            if (this.Annotations.Count > 0)
            {
                return ((BoxAnnotation)this.Annotations[0]).Label.FormatString;
            }

            return String.Empty;
        }

        private void SetText(string value)
        {
            if (this.Annotations.Count > 0)
            {
                ((BoxAnnotation)this.Annotations[0]).Label.FormatString = value;
            }
        }

        private Color GetTextColor()
        {
            if (this.Annotations.Count == 0)
            {
                return Color.Empty;
            }

            BoxAnnotation annotation = ((BoxAnnotation)this.Annotations[0]);
            if ((annotation.Label.BrushElement != null) && (annotation.Label.BrushElement is SolidFillBrushElement))
            {
                return ((SolidFillBrushElement)annotation.Label.BrushElement).Color;
            }
            else
                return Color.Empty;
        }


        private void SetTextColor(Color value)
        {
            if (this.Annotations.Count == 0)
            {
                return;
            }

            BoxAnnotation annotation = ((BoxAnnotation)this.Annotations[0]);

            if ((annotation.Label.BrushElement != null) && (annotation.Label.BrushElement is SolidFillBrushElement))
            {
                ((SolidFillBrushElement)annotation.Label.BrushElement).Color = value;
            }

        }

        private Font GetTextFont()
        {
            if (this.Annotations.Count > 0)
            {
                return ((BoxAnnotation)this.Annotations[0]).Label.Font;
            }

            return null;
        }

        private void SetTextFont(Font value)
        {
            if (value != null)
            {
                if (this.Annotations.Count > 0)
                {
                    ((BoxAnnotation) this.Annotations[0]).Label.Font = value;
                }
            }
        }

        public void LoadPreset(string fileName)
        {
            this.LoadPreset(fileName, true);
        }

        private List<GaugeRange> GetVisibleColorRanges()
        {
            List<GaugeRange> result = new List<GaugeRange>();
            if (this.Gauges.Count > 0)
            {
                if (this.Gauges[0] is LinearGauge)
                {
                    if (((LinearGauge)this.Gauges[0]).Scales.Count > 0)
                    {
                        foreach (GaugeRange range in ((LinearGauge)this.Gauges[0]).Scales[0].Ranges)
                            result.Add(range);
                    }
                }
                else
                    if (this.Gauges[0] is RadialGauge)
                    {
                        if (((RadialGauge)this.Gauges[0]).Scales.Count > 0)
                        {
                            foreach (GaugeRange range in ((RadialGauge)this.Gauges[0]).Scales[0].Ranges)
                                result.Add(range);
                        }
                    }
            }
            return result;
        }

        private void SetVisibleColorRanges(List<GaugeRange> value)
        {
            if (this.Gauges.Count > 0)
            {
                if ((GaugeConsts.gaugeCollectionSettings == null) || (!GaugeConsts.gaugeCollectionSettings.ContainsKey(this.PresetName)))
                    return;

                GaugeSettings settings = GaugeConsts.gaugeCollectionSettings[this.PresetName];

                //зададим начальному и конечному интервалу минимальное и максимальное значения шкалы
                if (value.Count > 0)
                {
                    value[0].StartValue = this.StartValue;

                    if ((value[0].EndValue == null) || ((double)value[0].EndValue > this.EndValue))
                        value[0].EndValue = this.EndValue;

                    for (int i = 1; i < value.Count - 1; i++)
                    {
                        if ((value[i].StartValue == null) || ((double)value[i].StartValue > this.EndValue))
                            value[i].StartValue = this.EndValue;

                        if ((value[i].EndValue == null) || ((double)value[i].EndValue > this.EndValue))
                            value[i].EndValue = this.EndValue;
                    }

                    if ((value[value.Count - 1].StartValue == null) || ((double)value[value.Count - 1].StartValue > (double)this.EndValue))
                        value[value.Count - 1].StartValue = this.EndValue;
                    value[value.Count - 1].EndValue = this.EndValue;
                }

                if (this.Gauges[0] is LinearGauge)
                {
                    if (((LinearGauge)this.Gauges[0]).Scales.Count > 0)
                    {
                        ((LinearGauge)this.Gauges[0]).Scales[0].Ranges.Clear();

                        foreach (GaugeRange range in value)
                        {
                            LinearGaugeRange linearRange = new LinearGaugeRange();
                            linearRange.Key = range.Key;
                            linearRange.StartValue = range.StartValue;
                            linearRange.EndValue = range.EndValue;

                            if ((range.BrushElement != null) && (range.BrushElement is SolidFillBrushElement))
                            {
                                linearRange.BrushElement =
                                    new SolidFillBrushElement(((SolidFillBrushElement)range.BrushElement).Color);
                            }
                            else
                            {
                                linearRange.BrushElement =
                                    new SolidFillBrushElement(Color.Empty);
                            }

                            linearRange.InnerExtent = settings.InnerExtent;
                            linearRange.OuterExtent = settings.OuterExtent;
                            ((LinearGauge)this.Gauges[0]).Scales[0].Ranges.Add(linearRange);
                        }
                    }
                }
                else
                    if (this.Gauges[0] is RadialGauge)
                    {
                        if (((RadialGauge)this.Gauges[0]).Scales.Count > 0)
                        {
                            ((RadialGauge)this.Gauges[0]).Scales[0].Ranges.Clear();

                            foreach (GaugeRange range in value)
                            {
                                RadialGaugeRange radialRange = new RadialGaugeRange();
                                radialRange.Key = range.Key;
                                radialRange.StartValue = range.StartValue;
                                radialRange.EndValue = range.EndValue;
                                if ((range.BrushElement != null) && (range.BrushElement is SolidFillBrushElement))
                                {
                                    radialRange.BrushElement =
                                        new SolidFillBrushElement(((SolidFillBrushElement)range.BrushElement).Color);
                                }
                                else
                                {
                                    radialRange.BrushElement =
                                        new SolidFillBrushElement(Color.Empty);
                                }
                                radialRange.InnerExtentStart = settings.InnerExtent;
                                radialRange.InnerExtentEnd = settings.InnerExtent;
                                radialRange.OuterExtent = settings.OuterExtent;
                                ((RadialGauge)this.Gauges[0]).Scales[0].Ranges.Add(radialRange);
                            }

                        }
                    }
            }

        }


        private void SetColorRanges(GaugeColorRangeCollection value)
        {
            if (this.Gauges.Count > 0)
            {
                if ((GaugeConsts.gaugeCollectionSettings == null) || (!GaugeConsts.gaugeCollectionSettings.ContainsKey(this.PresetName)))
                    return;

                GaugeSettings settings = GaugeConsts.gaugeCollectionSettings[this.PresetName];

                double startValue = 0;
                double endVAlue = 0;

                if (this.Gauges[0] is LinearGauge)
                {
                    if (((LinearGauge)this.Gauges[0]).Scales.Count > 0)
                    {
                        LinearGaugeRangeCollection ranges = ((LinearGauge)this.Gauges[0]).Scales[0].Ranges;
                        ranges.Clear();

                        foreach (GaugeColorRange range in value)
                        {
                            //Если пользовательский интервал выходит за границы шкалы индикатора, то его рисовать не будем
                            if ((range.EndValue <= this.StartValue)||(range.StartValue > this.EndValue))
                                continue;
                            
                            LinearGaugeRange linearRange = new LinearGaugeRange();
                            linearRange.Key = range.Text;

                            linearRange.StartValue = range.StartValue;
                            linearRange.EndValue = range.EndValue;

                            if (range.Color != null)
                            {
                                linearRange.BrushElement =
                                    new SolidFillBrushElement(range.Color);
                            }
                            else
                            {
                                linearRange.BrushElement =
                                    new SolidFillBrushElement(Color.Empty);
                            }

                            linearRange.InnerExtent = settings.InnerExtent;
                            linearRange.OuterExtent = settings.OuterExtent;
                            ranges.Add(linearRange);
                        }

                        //поравняем границы начального и конечного интервалов с границами шкалы индикатора
                        if (ranges.Count > 0)
                        {
                            ranges[0].StartValue = this.StartValue;
                            ranges[ranges.Count - 1].EndValue = this.EndValue;
                        }

                    }
                }
                else
                    if (this.Gauges[0] is RadialGauge)
                    {
                        if (((RadialGauge)this.Gauges[0]).Scales.Count > 0)
                        {
                            RadialGaugeRangeCollection ranges = ((RadialGauge)this.Gauges[0]).Scales[0].Ranges;
                            ranges.Clear();

                            foreach (GaugeColorRange range in value)
                            {
                                //Если пользовательский интервал выходит за границы шкалы индикатора, то его рисовать не будем
                                if ((range.EndValue <= this.StartValue) || (range.StartValue > this.EndValue))
                                    continue;

                                RadialGaugeRange radialRange = new RadialGaugeRange();
                                radialRange.Key = range.Text;
                                radialRange.StartValue = range.StartValue;
                                radialRange.EndValue = range.EndValue;
                                if (range.Color != null)
                                {
                                    radialRange.BrushElement =
                                        new SolidFillBrushElement(range.Color);
                                }
                                else
                                {
                                    radialRange.BrushElement =
                                        new SolidFillBrushElement(Color.Empty);
                                }
                                radialRange.InnerExtentStart = settings.InnerExtent;
                                radialRange.InnerExtentEnd = settings.InnerExtent;
                                radialRange.OuterExtent = settings.OuterExtent;
                                ranges.Add(radialRange);
                            }

                            //поравняем границы начального и конечного интервалов с границами шкалы индикатора
                            if (ranges.Count > 0)
                            {
                                ranges[0].StartValue = this.StartValue;
                                ranges[ranges.Count - 1].EndValue = this.EndValue;
                            }


                        }
                    }
            }

        }


        /// <summary>
        /// Обновление границ цветовых интервалов
        /// </summary>
        private void RefreshColorRanges()
        {
            SetColorRanges(this.ColorRanges);
        }

        public override string ToString()
        {
            return string.Empty;
        }






    }

}
