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
    public class GaugeReportElement : CustomReportElement
    {
        private UltraGauge gauge;
        private string _presetName;
        private GaugeSynchronization _synchronization;
        private bool _autoTickmarkCalculation;
        private bool _isNeedRefresh;
        private ExpertLegend _legend;
        private GaugeColorRangeCollection _colorRanges;

        private EventHandler _rangeCollectionChanged = null;

        /// <summary>
        /// Имя текущей настройки для индикатора
        /// </summary>
        public string PresetName
        {
            get { return this._presetName; }
            set
            {
                try
                {
                    this.ElementPlace.SuspendLayout();

                    this._presetName = value;
                    //при смене шаблона индикатора сохраняем текущие значения и формат
                    double startValue = this.StartValue;
                    double endValue = this.EndValue;
                    double currValue = this.Value;
                    double tickMarkInterval = this.TickmarkInterval;

                    string labelsFormatString = GetLabelsFormaString();
                    string text = this.Text;
                    Font textFont = this.TextFont;

                    GaugeColorRangeCollection colorRanges = this.ColorRanges;

                    LoadPreset(String.Format(Application.StartupPath + "\\GaugePresets\\{0}.xml", value));

                    this.IsNeedRefresh = false;
                    SetValuesWithoutCalculation(startValue, endValue, currValue, tickMarkInterval);
                    this.IsNeedRefresh = true;

                    //this.TickmarkInterval = tickMarkInterval;

                    SetLabelsFormatString(labelsFormatString);
                    this.Text = text;
                    this.TextFont = textFont;
                    this.ColorRanges = colorRanges;
                    this.ElementPlace.ResumeLayout();

                    this.gauge.Refresh();
                }
                catch
                {
                    this.ElementPlace.ResumeLayout();
                }
            }
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
        /// Подпись
        /// </summary>
        public string Text
        {
            get { return GetText(); }
            set { SetText(value); }
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
                    if (!String.IsNullOrEmpty(this.Synchronization.BoundTo))
                    {
                        TableReportElement tableElement = this.MainForm.FindTableReportElement(this.Synchronization.BoundTo);
                        if (tableElement != null)
                        {
                            tableElement.UpdateAnchoredGauge();
                        }
                    }
                    else
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


        public GaugeSynchronization Synchronization
        {
            get { return _synchronization; }
            set { _synchronization = value; }
        }


        public UltraGauge Gauge
        {
            get { return gauge; }
        }

        /// <summary>
        /// Нужно ли перерисовывать индикатор
        /// </summary>
        public bool IsNeedRefresh
        {
            get { return _isNeedRefresh; }
            set { _isNeedRefresh = value; }
        }

        /*
        /// <summary>
        /// Цветовые интервалы
        /// </summary>
        public List<GaugeRange> ColorRanges
        {
            get { return GetColorRanges(); }
            set { SetColorRanges(value); }
        }
        */

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




        /// <summary>
        /// Поля индикатора
        /// </summary>
        public Margin Margin
        {
            get
            {
                if (this.Gauge.Gauges.Count > 0)
                {
                    return this.Gauge.Gauges[0].Margin;
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


        public GaugeReportElement(MainForm mainForm)
            : base(mainForm, ReportElementType.eGauge)
        {
            this.gauge = new UltraGauge();
            this.ColorRanges = new GaugeColorRangeCollection();

            this.ElementPlace.SuspendLayout();
            this.ElementPlace.AutoScroll = true;
            //this.ElementPlace.Controls.Add(this.chart);

            this.gauge.Parent = this.ElementPlace;
            this.gauge.Dock = DockStyle.Fill;

            this._legend = new ExpertLegend();
            this._legend.Parent = this.ElementPlace;
            this._legend.Location = LegendLocation.Right;
            this._legend.LegendSize = 200;
            this._legend.Visible = false;

            this.gauge.MouseClick += new MouseEventHandler(gauge_MouseClick);
            this.gauge.SizeChanged += new EventHandler(gauge_SizeChanged);

            //Восстановим отрисовку
            this.ElementPlace.ResumeLayout();

            this.ElementType = ReportElementType.eGauge;

            this.Synchronization = new GaugeSynchronization(this);
            this.IsNeedRefresh = true;

            string firstPreset = GetFirstPresetName();
            if (!String.IsNullOrEmpty(firstPreset))
            {
                this.PresetName = firstPreset;
                this.StartValue = 0;
                this.EndValue = 100;
                this.Value = 50;
            }

            //this.Text = "строка 1 \n строка2 \n строка 3";

            this.AutoTickmarkCalculation = true;
        }

        void gauge_SizeChanged(object sender, EventArgs e)
        {
            RefreshMargin();
        }

        /// <summary>
        /// Расчет полей индикатора, если тип измерения - пикселы
        /// </summary>
        private void RefreshMargin()
        {
            if (this.Gauge.Gauges.Count > 0)
            {
                if (this.Gauge.Gauges[0].Margin.Measure == Measure.Pixels)
                {
                    SetMargin(this.Margin);
                }
            }
        }

        public void InitLegend()
        {
            this.Legend.Items.Clear();
            string rangeText = String.Empty;
            Color rangeColor = Color.Empty;
            foreach(GaugeColorRange range in this.ColorRanges)
            {
                if (range.Color != null)
                {
                    rangeColor = range.Color;
                    rangeText = range.Text;
                    this.Legend.Items.Add(rangeColor, rangeText, range.StartValue, range.EndValue);
                }
            }
        }



        private void SetMargin(Margin value)
        {
            if (this.Gauge.Gauges.Count > 0)
            {
                this.SuspendLayout();
                this.Gauge.Gauges[0].Margin = value;

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
                    newBounds = new Rectangle(-left + 2, -top + 2, this.Gauge.Width - 4, this.Gauge.Height - 4);
                }

                if (this.Gauge.Gauges[0].BrushElement != null)
                {
                    if (this.Gauge.Gauges[0].BrushElement is BrushElementGroup)
                    {
                        BrushElementGroup brElements = (BrushElementGroup)this.Gauge.Gauges[0].BrushElement;
                        foreach (BrushElement brElem in brElements.BrushElements)
                        {
                            brElem.RelativeBounds = newBounds;
                            brElem.RelativeBoundsMeasure = value.Measure;
                        }
                    }
                    else
                    {
                        this.Gauge.Gauges[0].BrushElement.RelativeBounds = newBounds;
                        this.Gauge.Gauges[0].BrushElement.RelativeBoundsMeasure = value.Measure;
                    }
                }

                if ((this.Gauge.Gauges[0].StrokeElement != null) && (this.Gauge.Gauges[0].StrokeElement.BrushElement != null))
                {
                    this.Gauge.Gauges[0].StrokeElement.BrushElement.RelativeBounds = newBounds;
                    this.Gauge.Gauges[0].StrokeElement.BrushElement.RelativeBoundsMeasure = value.Measure;
                }
            }


            this.ResumeLayout();
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
                    MessageBox.Show("Текущее значение размера интервалов некорректно. Расчет интервалов переведен в автоматический режим.",
                        "MDX Эксперт", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }


        /// <summary>
        /// Получение первой настрой индикатора из списка
        /// </summary>
        /// <returns></returns>
        private string GetFirstPresetName()
        {
            if (Directory.Exists(Application.StartupPath + "\\GaugePresets"))
            {
                string[] files = Directory.GetFiles(Application.StartupPath + "\\GaugePresets\\", "*.xml");
                if (files.Length > 0)
                    return System.IO.Path.GetFileNameWithoutExtension(files[0]);
            }
            return String.Empty;
        }

        void gauge_MouseClick(object sender, MouseEventArgs e)
        {
            this.MainForm.RefreshUserInterface(this);
        }

        /// <summary>
        /// Получение начального значения шкалы индикатора
        /// </summary>
        /// <returns></returns>
        private double GetStartValue()
        {
            if (this.gauge.Gauges[0] is RadialGauge)
            {
                return (double)((RadialGauge)this.gauge.Gauges[0]).Scales[0].Axis.GetStartValue();
            }
            else
                if (this.gauge.Gauges[0] is LinearGauge)
                {
                    return (double)((LinearGauge)this.gauge.Gauges[0]).Scales[0].Axis.GetStartValue();
                }

            return 0;
        }

        /// <summary>
        /// Получение конечного значения шкалы индикатора
        /// </summary>
        /// <returns></returns>
        private double GetEndValue()
        {
            if (this.gauge.Gauges[0] is RadialGauge)
            {
                return (double)((RadialGauge)this.gauge.Gauges[0]).Scales[0].Axis.GetEndValue();
            }
            else
                if (this.gauge.Gauges[0] is LinearGauge)
                {
                    return (double)((LinearGauge)this.gauge.Gauges[0]).Scales[0].Axis.GetEndValue();
                }
            return 0;
        }

        /// <summary>
        /// Получение текущего значения идикатора
        /// </summary>
        /// <returns></returns>
        private double GetValue()
        {
            if (this.gauge.Gauges[0] is RadialGauge)
            {
                foreach(RadialGaugeScale scale in ((RadialGauge)this.gauge.Gauges[0]).Scales)
                {
                    if (scale.Markers.Count > 0)
                    {
                        return (double)scale.Markers[0].Value;
                    }
                }
                return 0;
            }
            else
                if (this.gauge.Gauges[0] is LinearGauge)
                {
                    foreach (LinearGaugeScale scale in ((LinearGauge)this.gauge.Gauges[0]).Scales)
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

        if (this.gauge.Gauges[0] is RadialGauge)
            {
                return ((RadialGauge) this.gauge.Gauges[0]).Scales[0].Labels.FormatString;
            }
            else
                if (this.gauge.Gauges[0] is LinearGauge)
                {
                    return ((LinearGauge)this.gauge.Gauges[0]).Scales[0].Labels.FormatString;
                }

            return result;
        }

        /// <summary>
        /// Установка формата меток
        /// </summary>
        /// <param name="value"></param>
        private void SetLabelsFormatString(string value)
        {
            if (this.gauge.Gauges[0] is RadialGauge)
            {
                ((RadialGauge)this.gauge.Gauges[0]).Scales[0].Labels.FormatString = value;
            }
            else
                if (this.gauge.Gauges[0] is LinearGauge)
                {
                    ((LinearGauge)this.gauge.Gauges[0]).Scales[0].Labels.FormatString = value;
                }
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
                if (this.gauge.Gauges[0] is RadialGauge)
                {
                    foreach (RadialGaugeScale scale in ((RadialGauge)this.gauge.Gauges[0]).Scales)
                    {
                        if (scale.Axis != null)
                        {
                            scale.Axis.SetTickmarkInterval(tickMarkInterval);
                        }
                    }
                }
                else
                    if (this.gauge.Gauges[0] is LinearGauge)
                    {
                        foreach (LinearGaugeScale scale in ((LinearGauge) this.gauge.Gauges[0]).Scales)
                        {
                            if (scale.Axis != null)
                            {
                                scale.Axis.SetTickmarkInterval(tickMarkInterval);
                            }
                        }
                    }
            }

            if (this.gauge.Gauges[0] is RadialGauge)
            {
                foreach (RadialGaugeScale scale in ((RadialGauge)this.gauge.Gauges[0]).Scales)
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
                if (this.gauge.Gauges[0] is LinearGauge)
                {
                    foreach (LinearGaugeScale scale in ((LinearGauge)this.gauge.Gauges[0]).Scales)
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
                this.gauge.Refresh();

            this.InitLegend();
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
                if (this.gauge.Gauges[0] is RadialGauge)
                {
                    foreach (RadialGaugeScale scale in ((RadialGauge)this.gauge.Gauges[0]).Scales)
                    {
                        if (scale.Axis != null)
                        {
                            scale.Axis.SetTickmarkInterval(tickMarkInterval);
                        }
                    }
                }
                else
                    if (this.gauge.Gauges[0] is LinearGauge)
                    {
                        foreach (LinearGaugeScale scale in ((LinearGauge) this.gauge.Gauges[0]).Scales)
                        {
                            if (scale.Axis != null)
                            {
                                scale.Axis.SetTickmarkInterval(tickMarkInterval);
                            }
                        }
                    }
            }

            if (this.gauge.Gauges[0] is RadialGauge)
            {
                foreach (RadialGaugeScale scale in ((RadialGauge)this.gauge.Gauges[0]).Scales)
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
                if (this.gauge.Gauges[0] is LinearGauge)
                {
                    foreach (LinearGaugeScale scale in ((LinearGauge)this.gauge.Gauges[0]).Scales)
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
                this.gauge.Refresh();

            this.InitLegend();

        }

        /// <summary>
        /// Установка значений без расчета
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="valu"></param>
        private void SetValuesWithoutCalculation(double startValue, double endValue, double value, double tickMarkInterval)
        {
            try
            {
                if (this.gauge.Gauges[0] is RadialGauge)
                {
                    foreach (RadialGaugeScale scale in ((RadialGauge)this.gauge.Gauges[0]).Scales)
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
                else if (this.gauge.Gauges[0] is LinearGauge)
                {
                    foreach (LinearGaugeScale scale in ((LinearGauge)this.gauge.Gauges[0]).Scales)
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
                if (this.IsNeedRefresh)
                    this.Gauge.Refresh();

                this.InitLegend();
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


            if (this.AutoTickmarkCalculation)
            {
                double tickMarkInterval = CalcTickMarkInterval(ref startValue, ref value, 10);
                if (this.gauge.Gauges[0] is RadialGauge)
                {
                    foreach (RadialGaugeScale scale in ((RadialGauge)this.gauge.Gauges[0]).Scales)
                    {
                        if (scale.Axis != null)
                        {
                            scale.Axis.SetTickmarkInterval(tickMarkInterval);
                        }
                    }
                }
                else
                    if (this.gauge.Gauges[0] is LinearGauge)
                    {
                        foreach (LinearGaugeScale scale in ((LinearGauge) this.gauge.Gauges[0]).Scales)
                        {
                            if (scale.Axis != null)
                            {
                                scale.Axis.SetTickmarkInterval(tickMarkInterval);
                            }
                        }
                    }
            }

            if (this.gauge.Gauges[0] is RadialGauge)
            {
                foreach (RadialGaugeScale scale in ((RadialGauge)this.gauge.Gauges[0]).Scales)
                {
                    if (scale.Axis != null)
                    {
                        scale.Axis.SetStartValue(startValue);
                        scale.Axis.SetEndValue(value);
                        if (scale.Ranges.Count > 0)
                        {
                            scale.Ranges[0].StartValue = startValue;
                            scale.Ranges[scale.Ranges.Count - 1].EndValue = value;
                        }

                    }
                }
            }
            else
                if (this.gauge.Gauges[0] is LinearGauge)
                {
                    foreach (LinearGaugeScale scale in ((LinearGauge)this.gauge.Gauges[0]).Scales)
                    {
                        if (scale.Axis != null)
                        {
                            scale.Axis.SetStartValue(startValue);
                            scale.Axis.SetEndValue(value);
                            if (scale.Ranges.Count > 0)
                            {
                                scale.Ranges[0].StartValue = startValue;
                                scale.Ranges[scale.Ranges.Count - 1].EndValue = value;
                            }
                        }
                    }
                }

            CheckTickMarkInterval();
            RefreshColorRanges();
            RefreshAppearance();
            
            if (this.IsNeedRefresh)
                this.gauge.Refresh();

            this.InitLegend();

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

            if (this.gauge.Gauges[0] is RadialGauge)
            {
                foreach (RadialGaugeScale scale in ((RadialGauge)this.gauge.Gauges[0]).Scales)
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
                if (this.gauge.Gauges[0] is LinearGauge)
                {
                    foreach (LinearGaugeScale scale in ((LinearGauge)this.gauge.Gauges[0]).Scales)
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
                this.gauge.Refresh();
            this.InitLegend();

        }


        /// <summary>
        /// Получить интервал между отметками
        /// </summary>
        /// <param name="value"></param>
        private double GetTickmarkInterval()
        {
            if (this.gauge.Gauges[0] is RadialGauge)
            {
                return (double)((RadialGauge) this.gauge.Gauges[0]).Scales[0].Axis.GetTickmarkInterval();
            }
            else
                if (this.gauge.Gauges[0] is LinearGauge)
                {
                    return (double)((LinearGauge)this.gauge.Gauges[0]).Scales[0].Axis.GetTickmarkInterval();
                }
            return 10;
        }



        /// <summary>
        /// Установка текущего значения индикатора
        /// </summary>
        /// <param name="value"></param>
        private void SetValue(double value)
        {
            for (int i = 0; i < this.gauge.Gauges.Count; i++)
            {
                if (this.gauge.Gauges[i] is RadialGauge)
                {
                    foreach (RadialGaugeScale scale in ((RadialGauge)this.gauge.Gauges[0]).Scales)
                    {
                        if (scale.Markers.Count > 0)
                        {
                            scale.Markers[0].Value = value;
                        }
                    }
                }
                else if (this.gauge.Gauges[i] is LinearGauge)
                {
                    foreach (LinearGaugeScale scale in ((LinearGauge)this.gauge.Gauges[0]).Scales)
                    {
                        if (scale.Markers.Count > 0)
                        {
                            scale.Markers[0].Value = value;
                        }
                    }
                }
                else if (this.gauge.Gauges[i] is SegmentedDigitalGauge)
                {
                    string formatStr = "0";

                    if (this.gauge.Gauges[0] is RadialGauge)
                    {
                        formatStr = ((RadialGauge)this.gauge.Gauges[0]).Scales[0].Labels.FormatString;
                    }
                    else if (this.gauge.Gauges[0] is LinearGauge)
                    {
                        formatStr = ((LinearGauge) this.gauge.Gauges[0]).Scales[0].Labels.FormatString;
                    }

                    GaugeFormatBrowseClass format = new GaugeFormatBrowseClass(formatStr);
                    format.DisplayUnits = false;
                    formatStr = format.FormatString;

                    formatStr = String.Format("{0:" + formatStr + "}", value);

                    formatStr = formatStr.Replace("%", "");
                    double newValue = Double.Parse(formatStr);

                    ((SegmentedDigitalGauge)this.gauge.Gauges[i]).Digits = newValue.ToString().Length > 5 ? value.ToString().Length : 6;
                    ((SegmentedDigitalGauge)this.gauge.Gauges[i]).Text = newValue.ToString().Replace(',', '.');
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
            foreach (Gauge g in this.gauge.Gauges)
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
            if (this.gauge.Annotations.Count > 0)
            {
                return ((BoxAnnotation)this.gauge.Annotations[0]).Label.FormatString;
            }

            return String.Empty;
        }

        private void SetText(string value)
        {
            if (this.gauge.Annotations.Count > 0)
            {
                ((BoxAnnotation)this.gauge.Annotations[0]).Label.FormatString = value;
            }
        }


        private Font GetTextFont()
        {
            if (this.gauge.Annotations.Count > 0)
            {
                return ((BoxAnnotation)this.gauge.Annotations[0]).Label.Font;
            }

            return null;
        }

        private void SetTextFont(Font value)
        {
            if (value != null)
            {
                if (this.gauge.Annotations.Count > 0)
                {
                    ((BoxAnnotation) this.gauge.Annotations[0]).Label.Font = value;
                }
            }
        }



        public override System.Xml.XmlNode Save()
        {
            XmlNode result = base.Save();

            XmlHelper.SetAttribute(result, Consts.templateName, this.PresetName);

            this.SavePreset(XmlHelper.AddChildNode(result, Common.Consts.presets));
            
            XmlHelper.AddChildNode(result, Consts.synchronization,
                new string[2] { Consts.boundTo, this.Synchronization.BoundTo },
                new string[2] { Consts.isCurrentColumnValues, this.Synchronization.IsCurrentColumnValues.ToString() }
                );

            XmlHelper.SetAttribute(result, Consts.autoTickmarkCalculation, this.AutoTickmarkCalculation.ToString());

            this.ColorRanges.Save(XmlHelper.AddChildNode(result, Consts.colorRanges));
            this.Legend.Save(XmlHelper.AddChildNode(result, Common.Consts.legend));

            return result;
        }

        public override void Load(System.Xml.XmlNode reportElement, bool isForceDataUpdate)
        {
            base.Load(reportElement, isForceDataUpdate);

            if (reportElement == null)
                return;

            //Для старых отчетов, в которых настройки цветовых интервалов хранились в настройках самих индикаторов нужно сделать восстановление
            //интервалов в новое свойство ColorRanges (если нет отдельно сохраненного нового узла \\ColorRanges)

            this.LoadPreset(reportElement.SelectSingleNode(Common.Consts.presets));
            Margin margin = this.Margin;
            Rectangle annotationBounds = new Rectangle();
            if (this.Gauge.Annotations.Count > 0)
            {
                annotationBounds = ((BoxAnnotation)this.Gauge.Annotations[0]).Bounds;
            }

            XmlNode colorRangesNode = reportElement.SelectSingleNode(Common.Consts.colorRanges);
            if (colorRangesNode != null)
            {
                this.ColorRanges.Load(colorRangesNode);
            }
            else
            {
                this.ColorRanges.InitByVisibleRanges(this.VisibleColorRanges);
            }

            this.PresetName = XmlHelper.GetStringAttrValue(reportElement, Consts.templateName, string.Empty);
            this.Margin = margin;
            if (this.Gauge.Annotations.Count > 0)
            {
                ((BoxAnnotation)this.Gauge.Annotations[0]).Bounds = annotationBounds;
            }


            XmlNode syncNode = reportElement.SelectSingleNode(Consts.synchronization);
            if (syncNode != null)
            {
                this.Synchronization.BoundTo = XmlHelper.GetStringAttrValue(syncNode, Consts.boundTo, "");
                this.Synchronization.IsCurrentColumnValues = XmlHelper.GetBoolAttrValue(syncNode, Consts.isCurrentColumnValues, false);
            }

            this.AutoTickmarkCalculation = XmlHelper.GetBoolAttrValue(reportElement, Consts.autoTickmarkCalculation, true);

            this.Legend.Load(reportElement.SelectSingleNode(Common.Consts.legend));
            this.RefreshColorRanges();
            this.InitLegend();
        }


        private void SavePreset(XmlNode presetNode)
        {
            using (StringWriter strWriter = new StringWriter())
            {
                this.Gauge.SavePreset(strWriter, "UltraGaugePreset", String.Empty, PresetType.All);
                XmlHelper.AppendCDataSection(presetNode, strWriter.ToString());
            }
        }

        private void LoadPreset(XmlNode presetNode)
        {
            if (presetNode == null)
                return;

            string reportElementPreset = presetNode.FirstChild.Value;

            if (reportElementPreset != string.Empty)
            {
                using (StringReader stringReader = new StringReader(reportElementPreset))
                {
                    this.Gauge.LoadPreset(stringReader, true);
                }
            }
        }



        protected override void RefreshData()
        {
            base.RefreshData();
        }

        /// <summary>
        /// Построение диаграммы по MDX-запросу
        /// </summary>
        protected override CellSet SetMDXQuery(string mdxQuery)
        {
            CellSet cls = null;

            try
            {
                cls = base.SetMDXQuery(mdxQuery);
                this.InitialByCellSet(cls);
            }
            catch (Exception e)
            {
                this.InitialByCellSet();
                Common.CommonUtils.ProcessException(e);
            }
            return cls;
        }

        /// <summary>
        /// Именно в этом методе происходит инициализация элемента отчета по CellSet-у
        /// </summary>
        /// <param name="cls"></param>
        public override void InitialByCellSet(CellSet cls)
        {
            base.InitialByCellSet(cls);
        }

        public override void SetElementVisible(bool value)
        {
            if (this.Gauge.Visible != value)
            {
                this.Gauge.Visible = value;
                Application.DoEvents();
            }
        }

        /// <summary>
        /// Получить изображение для печати, если у диаграммы стоит признак растягивать, 
        /// значит уместим изображение в указаные границы, если его не стоит, значит получаем полное 
        /// изображение элемента (вероятно оно будет напечатано на нескольиких страницах)
        /// </summary>
        /// <param name="imageBounds"></param>
        /// <returns></returns>
        public override Bitmap GetPrintableImage(Rectangle pageBounds)
        {
            return (this.Gauge.Dock == DockStyle.Fill) ? this.GetBitmap(pageBounds) : this.GetBitmap();
        }

        /// <summary>
        /// Получить полное изображение элемента
        /// </summary>
        /// <returns></returns>
        public override Bitmap GetBitmap()
        {
            Rectangle fullElementBounds = this.ClientRectangle;
            /*fullElementBounds.Width -= this.ElementPlace.Width;
            fullElementBounds.Height -= this.ElementPlace.Height;

            fullElementBounds.Width += this.Gauge.Size.Width;
            fullElementBounds.Height += this.Gauge.Size.Height;*/

            fullElementBounds.Width = Math.Max(this.ClientRectangle.Width, fullElementBounds.Width);
            fullElementBounds.Height = Math.Max(this.ClientRectangle.Height, fullElementBounds.Height);
            return base.GetBitmap(fullElementBounds);
        }

        public override string ToString()
        {
            return string.Empty;
        }

        /// <summary>
        /// Т.к. этот элемент отчета не содержит данного интерфейса, возвращаем null
        /// </summary>
        public override IGridUserInterface GridUserInterface
        {
            get { return null; }
        }



        public override bool IsShowErrorMessage
        {
            get { return false; }
        }

        public ExpertLegend Legend
        {
            get { return _legend; }
            set { _legend = value; }
        }

        public EventHandler RangeCollectionChanged
        {
            get { return _rangeCollectionChanged; }
            set { _rangeCollectionChanged = value; }
        }


        private void LoadPreset(string fileName)
        {
            this.gauge.LoadPreset(fileName, true);
        }

        public void Synchronize()
        {
            PivotData pivotData = null;
            TableReportElement tableElement = null;

            if (!String.IsNullOrEmpty(this.Synchronization.BoundTo))
            {
                tableElement = this.MainForm.FindTableReportElement(this.Synchronization.BoundTo);
                if (tableElement != null)
                {
                    pivotData = tableElement.PivotData;
                }
            }
            if (pivotData == null)
            {
                this.MainForm.UndoRedoManager.AddEvent(this, UndoRedoEventType.DataChange);
                return;
            }

            if (tableElement != null)
            {
                tableElement.UpdateAnchoredGauge();
            }

            if (tableElement != null)
                this.MainForm.UndoRedoManager.AddEvent(tableElement, UndoRedoEventType.DataChange);

        }


        private List<GaugeRange> GetVisibleColorRanges()
        {
            List<GaugeRange> result = new List<GaugeRange>();
            if (this.Gauge.Gauges.Count > 0)
            {
                if (this.Gauge.Gauges[0] is LinearGauge)
                {
                    if (((LinearGauge)this.Gauge.Gauges[0]).Scales.Count > 0)
                    {
                        foreach (GaugeRange range in ((LinearGauge)this.Gauge.Gauges[0]).Scales[0].Ranges)
                            result.Add(range);
                    }
                }
                else
                    if (this.Gauge.Gauges[0] is RadialGauge)
                    {
                        if (((RadialGauge)this.Gauge.Gauges[0]).Scales.Count > 0)
                        {
                            foreach (GaugeRange range in ((RadialGauge)this.Gauge.Gauges[0]).Scales[0].Ranges)
                                result.Add(range);
                        }
                    }
            }
            return result;
        }

        private void SetVisibleColorRanges(List<GaugeRange> value)
        {
            if (this.Gauge.Gauges.Count > 0)
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

                if (this.Gauge.Gauges[0] is LinearGauge)
                {
                    if (((LinearGauge)this.Gauge.Gauges[0]).Scales.Count > 0)
                    {
                        ((LinearGauge)this.Gauge.Gauges[0]).Scales[0].Ranges.Clear();

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
                            ((LinearGauge)this.Gauge.Gauges[0]).Scales[0].Ranges.Add(linearRange);
                        }
                    }
                }
                else
                    if (this.Gauge.Gauges[0] is RadialGauge)
                    {
                        if (((RadialGauge)this.Gauge.Gauges[0]).Scales.Count > 0)
                        {
                            ((RadialGauge)this.Gauge.Gauges[0]).Scales[0].Ranges.Clear();

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
                                ((RadialGauge)this.Gauge.Gauges[0]).Scales[0].Ranges.Add(radialRange);
                            }

                        }
                    }
            }

        }


        private void SetColorRanges(GaugeColorRangeCollection value)
        {
            if (this.Gauge.Gauges.Count > 0)
            {
                if ((GaugeConsts.gaugeCollectionSettings == null) || (!GaugeConsts.gaugeCollectionSettings.ContainsKey(this.PresetName)))
                    return;

                GaugeSettings settings = GaugeConsts.gaugeCollectionSettings[this.PresetName];

                double startValue = 0;
                double endVAlue = 0;

                if (this.Gauge.Gauges[0] is LinearGauge)
                {
                    if (((LinearGauge)this.Gauge.Gauges[0]).Scales.Count > 0)
                    {
                        LinearGaugeRangeCollection ranges = ((LinearGauge)this.Gauge.Gauges[0]).Scales[0].Ranges;
                        ranges.Clear();

                        foreach (GaugeColorRange range in value)
                        {
                            //Если пользовательский интервал выходит за границы шкалы индикатора, то его рисовать не будем
                            if ((range.EndValue <= this.StartValue) || (range.StartValue > this.EndValue))
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
                    if (this.Gauge.Gauges[0] is RadialGauge)
                    {
                        if (((RadialGauge)this.Gauge.Gauges[0]).Scales.Count > 0)
                        {
                            RadialGaugeRangeCollection ranges = ((RadialGauge)this.Gauge.Gauges[0]).Scales[0].Ranges;
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
        public void RefreshColorRanges()
        {
            SetColorRanges(this.ColorRanges);
        }



    }


    /// <summary>
    /// Синхронизация индикатора с таблицей
    /// </summary>
    public class GaugeSynchronization
    {
        private string _boundTo;
        private GaugeReportElement _gaugeElement;
        private string _measure;
        private bool _isCurrentColumnValues;

        /// <summary>
        /// Здесь прописывается уникальный ключ таблицы, с которой синхронизируется элемент
        /// </summary>
        public string BoundTo
        {
            get { return _boundTo; }
            set
            {
                SetBoundTo(value);
            }
        }

        /// <summary>
        /// Уникальное имя меры, значения для которой отображаются на индикаторе
        /// </summary>
        public string Measure
        {
            get { return _measure; }
            set { _measure = value; }
        }

        /// <summary>
        /// При получении минимального и максимального значений показателя учитывать значения только текущего столбца таблицы
        /// </summary>
        public bool IsCurrentColumnValues
        {
            get { return this._isCurrentColumnValues; }
            set { this._isCurrentColumnValues = value; }
        }

        public GaugeReportElement GaugeElement
        {
            get { return _gaugeElement; }
            set { _gaugeElement = value; }
        }


        public GaugeSynchronization(GaugeReportElement gaugeElement)
        {
            this._gaugeElement = gaugeElement;
            this._isCurrentColumnValues = false;
        }

        private void SetBoundTo(string key)
        {
            if (key == this.BoundTo)
                return;

            //удаляем у таблицы ссылку на индикатор
            if (!String.IsNullOrEmpty(this.BoundTo))
            {
                TableReportElement tableElement = this.GaugeElement.MainForm.FindTableReportElement(this.BoundTo);
                if (tableElement != null)
                    tableElement.AnchoredElements.Remove(this.GaugeElement.UniqueName);
            }

            //добавляем для таблицы ссылку на индикатор
            if (!String.IsNullOrEmpty(key))
            {
                TableReportElement tableElement = this.GaugeElement.MainForm.FindTableReportElement(key);
                if (tableElement != null)
                {
                    tableElement.AnchoredElements.Add(this.GaugeElement.UniqueName);
                    tableElement.UpdateAnchoredGauge();
                }
            }

            this._boundTo = key;
        }

        public override string ToString()
        {
            if (!String.IsNullOrEmpty(this.BoundTo))
            {
                return this.GaugeElement.MainForm.GetReportElementText(this.BoundTo);
            }
            else
            {
                return "";
            }
        }
    }



}
