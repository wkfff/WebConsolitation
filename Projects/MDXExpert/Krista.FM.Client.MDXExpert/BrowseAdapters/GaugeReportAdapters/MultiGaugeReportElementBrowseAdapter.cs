using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Infragistics.UltraGauge.Resources;
using Infragistics.Win.UltraWinDock;
using Dundas.Maps.WinControl;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.ComponentModel.Design;
using Infragistics.Win.UltraWinGauge;
using Krista.FM.Client.MDXExpert.Common;
using Microsoft.AnalysisServices.AdomdClient;
using System.Windows.Forms;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(PropertySorter))]
    class MultiGaugeReportElementBrowseAdapter : CustomReportElementBrowseAdapter
    {
        private ExpertGauge gauge;
        private MultipleGaugeReportElement gaugeElement;
        private MultiLinearGaugeLabelsBrowseClass _linearGaugeLabels;
        private MultiRadialGaugeLabelsBrowseClass _radialGaugeLabels;
        private MultiGaugeColorRangeBrowseClass _colorRanges;
        //private MultiRadialGaugeColorRangeBrowseClass _radialColorRanges;
        private GaugesLocationBrowseClass _location;
        private BoxAnnotationBrowseClass _annotationBrowse;
        private GaugeMarginBrowseClass _marginBrowse;

        [Browsable(false)]
        public bool IsGaugeExists
        {
            get
            {
                return (this.gauge != null) && (this.gauge.Gauges.Count > 0);
            }
        }
        
        [Browsable(false)]
        public bool IsRadialGauge
        {
            get { return (this.IsGaugeExists)&&(this.gauge.Gauges[0] is RadialGauge); }
        }

        [Browsable(false)]
        public bool IsLinearGauge
        {
            get { return (this.IsGaugeExists)&&(this.gauge.Gauges[0] is LinearGauge); }
        }

        [Browsable(false)]
        public bool IsSyncronized
        {
            get { return (!String.IsNullOrEmpty(this.gaugeElement.Synchronization.BoundTo)); }
        }


        [PropertyOrder(10)]
        [Category("Данные")]
        [Description("Начальное значение")]
        [DisplayName("Начальное значение")]
        [DynamicPropertyFilter("IsGaugeExists", "True")]
        [Browsable(true)]
        public double StartValue
        {
            get { return this.gaugeElement.StartValue; }
            set
            {
                if (!CheckSync())
                    this.gaugeElement.StartValue = value;
            }
        }

        [PropertyOrder(20)]
        [Category("Данные")]
        [Description("Конечное значение")]
        [DisplayName("Конечное значение")]
        [DynamicPropertyFilter("IsGaugeExists", "True")]
        [Browsable(true)]
        public double EndValue
        {
            get { return this.gaugeElement.EndValue; }
            set
            {
                if (!CheckSync())
                    this.gaugeElement.EndValue = value;
            }
        }
        
        /*
        [PropertyOrder(30)]
        [Category("Данные")]
        [Description("Текущее значение")]
        [DisplayName("Текущее значение")]
        [DynamicPropertyFilter("GaugeType", "Standart")]
        [Browsable(true)]
        public double Value
        {
            get { return this.gaugeElement.Value; }
            set
            {
                if (!CheckSync())
                    this.gaugeElement.Value = value;
            }
        }

        [PropertyOrder(31)]
        [Category("Внешний вид")]
        [Description("Тип индикатора")]
        [DisplayName("Тип индикатора")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(true)]
        public GaugeType GaugeType
        {
            get { return this.gaugeElement.GaugeType; }
            set { this.gaugeElement.GaugeType = value; }
        }
        */
        [PropertyOrder(35)]
        [Category("Внешний вид")]
        [Description("Размещение индикаторов")]
        [DisplayName("Размещение")]
        [Browsable(true)]
        public GaugesLocationBrowseClass Location
        {
            get { return this._location; }
            set { this._location = value; }
        }

        [PropertyOrder(36)]
        [Category("Внешний вид")]
        [Description("Поля индикаторов")]
        [DisplayName("Поля")]
        [Browsable(true)]
        public GaugeMarginBrowseClass Margin
        {
            get { return this._marginBrowse; }
            set { this._marginBrowse = value; }
        }


        [PropertyOrder(40)]
        [Category("Внешний вид")]
        [Description("Подпись")]
        [DisplayName("Подпись")]
        [DynamicPropertyFilter("IsGaugeExists", "True")]
        [Browsable(true)]
        public BoxAnnotationBrowseClass Annotation
        {
            get { return this._annotationBrowse; }
            set { this._annotationBrowse = value; }
        }

        /*
        [PropertyOrder(40)]
        [Category("Внешний вид")]
        [Description("Подпись")]
        [DisplayName("Подпись")]
        [DynamicPropertyFilter("IsGaugeExists", "True")]
        [Browsable(true)]
        public string Text
        {
            get { return this.gaugeElement.Text; }
            set { this.gaugeElement.Text = value; }
        }

        [PropertyOrder(45)]
        [Category("Внешний вид")]
        [Description("Шрифт подписи")]
        [DisplayName("Шрифт подписи")]
        [DynamicPropertyFilter("IsGaugeExists", "True")]
        [TypeConverter(typeof(FontTypeConverter))]
        [Browsable(true)]
        public Font TextFont
        {
            get { return this.gaugeElement.TextFont; }
            set { this.gaugeElement.TextFont = value; }
        }
        */

        [PropertyOrder(1)]
        [Category("Внешний вид")]
        [Description("Тип индикатора")]
        [DisplayName("Тип индикатора")]
        [TypeConverter(typeof(GaugePresetsConverter))]
        [Browsable(true)]
        public string Preset
        {
            get
            {
                return this.gaugeElement.PresetName;
            }
            set
            {
                this.gaugeElement.PresetName = value;
                InitLabels();
                this.gaugeElement.MainForm.RefreshUserInterface(gaugeElement);


            }
        }

        /*
        [Category("Внешний вид")]
        [Description("Test")]
        [DisplayName("Test")]
        [Browsable(true)]
        public UltraGauge Gauge
        {
            get
            {
                return this.gauge;
            }
        }
        */

        [PropertyOrder(50)]
        [Category("Внешний вид")]
        [Description("Метки")]
        [DisplayName("Метки")]
        [DynamicPropertyFilter("IsLinearGauge", "True")]
        [Browsable(true)]
        public MultiLinearGaugeLabelsBrowseClass LinearLabels
        {
            get { return this._linearGaugeLabels; }
        }

        [PropertyOrder(50)]
        [Category("Внешний вид")]
        [Description("Метки")]
        [DisplayName("Метки")]
        [DynamicPropertyFilter("IsRadialGauge", "True")]
        [Browsable(true)]
        public MultiRadialGaugeLabelsBrowseClass RadialLabels
        {
            get { return this._radialGaugeLabels; }
        }

        [PropertyOrder(54)]
        [Category("Внешний вид")]
        [Description("Автоматический расчет интервалов между метками")]
        [DisplayName("Автоматический расчет интервалов")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool AutoTickmarkCalculation
        {
            get { return this.gaugeElement.AutoTickmarkCalculation; }
            set { this.gaugeElement.AutoTickmarkCalculation = value; }
        }

        [PropertyOrder(55)]
        [Category("Внешний вид")]
        [Description("Интервал между метками")]
        [DisplayName("Интервал между метками")]
        [DynamicPropertyFilter("AutoTickmarkCalculation", "False")]
        [Browsable(true)]
        public double TickmarkInterval
        {
            get { return this.gaugeElement.TickmarkInterval; }
            set { this.gaugeElement.TickmarkInterval = value; }
        }
        /*
        [PropertyOrder(56)]
        [Category("Внешний вид")]
        [Description("Цветовая шкала")]
        [DisplayName("Цветовая шкала")]
        [Editor(typeof(LinearGaugeRangeCollectionEditor), typeof(UITypeEditor))]
        [DynamicPropertyFilter("IsLinearGauge", "True")]
        [Browsable(true)]
        public LinearGaugeRangeCollection LinearGaugeRanges
        {
            get
            {
                if (this.IsLinearGauge)
                {
                    return ((LinearGauge)this.gauge.Gauges[0]).Scales[0].Ranges;
                }
                else
                    return null;
            }
        }

        [PropertyOrder(57)]
        [Category("Внешний вид")]
        [Description("Цветовая шкала")]
        [DisplayName("Цветовая шкала")]
        [Editor(typeof(RadialGaugeRangeCollectionEditor), typeof(UITypeEditor))]
        [DynamicPropertyFilter("IsRadialGauge", "True")]
        [Browsable(true)]
        public RadialGaugeRangeCollection RadialGaugeRanges
        {
            get
            {
                if (this.IsRadialGauge)
                {
                    return ((RadialGauge)this.gauge.Gauges[0]).Scales[0].Ranges;
                }
                else
                    return null;
            }
        }
        */

        [PropertyOrder(56)]
        [Category("Внешний вид")]
        [Description("Цветовая шкала")]
        [DisplayName("Цветовая шкала")]
        [Browsable(true)]
        public MultiGaugeColorRangeBrowseClass ColorRanges
        {
            get
            {
                return this._colorRanges;
            }
        }
        /*
        [PropertyOrder(57)]
        [Category("Внешний вид")]
        [Description("Цветовая шкала")]
        [DisplayName("Цветовая шкала")]
        [DynamicPropertyFilter("IsRadialGauge", "True")]
        [Browsable(true)]
        public MultiRadialGaugeColorRangeBrowseClass RadialGaugeRanges
        {
            get { return this._radialColorRanges; }
        }
        */

        [Category("Управление данными")]
        [DisplayName("Синхронизация")]
        [Description("Синхронизация")]
        [Editor(typeof(MultiGaugeSyncEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        public MultipleGaugeSynchronization Synchronization
        {
            get
            {
                return this.gaugeElement.Synchronization;
            }
            set
            {
                this.gaugeElement.Synchronization = value;
            }
        }


        public MultiGaugeReportElementBrowseAdapter(DockableControlPane dcPane)
            : base(dcPane)
        {
            CustomReportElement reportElement = (CustomReportElement)dcPane.Control;
            this.gaugeElement = ((MultipleGaugeReportElement)reportElement);
            if (gaugeElement.MainGauge != null)
                this.gauge = gaugeElement.MainGauge;

            this._colorRanges = new MultiGaugeColorRangeBrowseClass(this.gaugeElement);

            /*
            if (this.IsLinearGauge)
            {
                this._linearColorRanges = new MultiLinearGaugeColorRangeBrowseClass((LinearGauge)this.gauge.Gauges[0], this.gaugeElement);
            }
            if (this.IsRadialGauge)
            {
                this._radialColorRanges = new  MultiRadialGaugeColorRangeBrowseClass((RadialGauge)this.gauge.Gauges[0], this.gaugeElement);
            }
            */

            this._location = new GaugesLocationBrowseClass(this.gaugeElement.GaugesLocation);

            this._annotationBrowse = new BoxAnnotationBrowseClass((BoxAnnotation) this.gauge.Annotations[0],
                                                                  this.gaugeElement);
            this._marginBrowse = new GaugeMarginBrowseClass(this.gaugeElement);

            InitLabels();
        }

        /// <summary>
        /// Проверка, есть ли синхронизация с таблицей
        /// </summary>
        /// <returns></returns>
        private bool CheckSync()
        {
            bool result = false;
            result = !String.IsNullOrEmpty(this.gaugeElement.Synchronization.BoundTo);

            /*
            if (this.GaugeType == GaugeType.Standart)
            {
                result = String.IsNullOrEmpty(this.gaugeElement.Synchronization.BoundTo);
            }
            else
            {
                if (this.GaugeType == GaugeType.Standart)
                {
                    result = String.IsNullOrEmpty(this.gaugeElement.Synchronization.BoundTo);
                }
            }
            */
            if (result)
                MessageBox.Show("Невозможно редактировать значение. Для редактирования значения снимите синхронизацию с таблицей.",
                    "MDX Expert", MessageBoxButtons.OK, MessageBoxIcon.Warning);


            return result;
        }

        private void InitLabels()
        {
            if (this.IsLinearGauge)
                this._linearGaugeLabels =
                    new MultiLinearGaugeLabelsBrowseClass(((LinearGauge)this.gauge.Gauges[0]).Scales[0].Labels, gaugeElement);

            if (this.IsRadialGauge)
                this._radialGaugeLabels =
                    new MultiRadialGaugeLabelsBrowseClass(((RadialGauge)this.gauge.Gauges[0]).Scales[0].Labels, gaugeElement);
        }

    }
}
