using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using Dundas.Maps.WinControl;
using System.Drawing;
using System.Drawing.Design;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Krista.FM.Client.MDXExpert;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BoxAnnotationBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private GaugeReportElement _gaugeElement;
        private MultipleGaugeReportElement _multiGaugeElement;
        private BoxAnnotation _annotation;
        private AnnotationBoundsBrowseClass _boundsBrowse;
        private bool _isMultiGauge = false;

        #endregion

        #region Свойства
        
        [Browsable(false)]
        public bool IsMultiGauge
        {
            get { return this._isMultiGauge; }
        }

        
        [Description("Цвет")]
        [DisplayName("Цвет")]
        [Browsable(true)]
        public Color Color
        {
            get
            {
                if ((this._annotation.Label.BrushElement != null) && (this._annotation.Label.BrushElement is SolidFillBrushElement))
                {
                    return ((SolidFillBrushElement)this._annotation.Label.BrushElement).Color;
                }
                else
                    return Color.Empty;
            }
            set
            {
                SetColor(value);
            }
        }
        

        [Description("Текст подписи")]
        [DisplayName("Текст")]
        [DynamicPropertyFilter("IsMultiGauge", "False")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public string Text
        {
            get
            {
                return this._annotation.Label.FormatString;
            }
            set { this._annotation.Label.FormatString = value; }
        }

        
        [Description("Шрифт подписи")]
        [DisplayName("Шрифт")]
        [Browsable(true)]
        public Font Font
        {
            get { return this._annotation.Label.Font; }
            set
            {
                SetFont(value);
            }
        }

        [Description("Расположение")]
        [DisplayName("Расположение")]
        [Browsable(true)]
        public AnnotationBoundsBrowseClass Bounds
        {
            get { return this._boundsBrowse; }
            set
            {
                
                this._boundsBrowse = value;
            }
        }

        /*
        [Category("Свойства")]
        [Description("Тип измерения границ")]
        [DisplayName("Тип измерения границ")]
        [TypeConverter(typeof(MeasureConverter))]
        [Browsable(true)]
        public Measure BoundsMeasureType
        {
            get { return this._annotation.BoundsMeasure; }
            set
            {
                SetBoundsMeasure(value);
            }
        }
        */

        #endregion

        public BoxAnnotationBrowseClass(BoxAnnotation annotation, GaugeReportElement gaugeElement)
        {
            this._annotation = annotation;
            this._boundsBrowse = new AnnotationBoundsBrowseClass(this._annotation);
            this._gaugeElement = gaugeElement;
            this._isMultiGauge = false;
        }

        public BoxAnnotationBrowseClass(BoxAnnotation annotation, MultipleGaugeReportElement multiGaugeElement)
        {
            this._annotation = annotation;
            this._boundsBrowse = new AnnotationBoundsBrowseClass(this._annotation);
            this._boundsBrowse.Changed += new EventHandler(_boundsBrowse_Changed);
            this._multiGaugeElement = multiGaugeElement;
            this._isMultiGauge = true;
        }

        void _boundsBrowse_Changed(object sender, EventArgs e)
        {
            if (this.IsMultiGauge)
            {
                foreach(ExpertGauge gauge in this._multiGaugeElement.Gauges)
                {
                    ((BoxAnnotation) gauge.Annotations[0]).Bounds = this._annotation.Bounds;
                }
            }
        }

        private void SetBoundsMeasure(Measure value)
        {
            this._annotation.BoundsMeasure = value;
            if (this.IsMultiGauge)
            {
                foreach (ExpertGauge gauge in this._multiGaugeElement.Gauges)
                {
                    ((BoxAnnotation)gauge.Annotations[0]).BoundsMeasure = value;
                }
            }

        }

        private void SetFont(Font value)
        {
            this._annotation.Label.Font = value;
            if (this.IsMultiGauge)
            {
                foreach (ExpertGauge gauge in this._multiGaugeElement.Gauges)
                {
                    ((BoxAnnotation)gauge.Annotations[0]).Label.Font = value;
                }
                this._multiGaugeElement.RefreshGaugesText();
            }
        }


        private void SetFont(BoxAnnotation annotation, Font value)
        {
            annotation.Label.Font = value;
        }

        private void SetColor(Color value)
        {
            SetColor((BoxAnnotation)this._annotation, value);
            
            if (this.IsMultiGauge)
            {
                foreach (ExpertGauge gauge in this._multiGaugeElement.Gauges)
                {
                    SetColor((BoxAnnotation) gauge.Annotations[0], value);
                    gauge.Refresh();
                }
            }
            else
            {
                if(this._gaugeElement != null)
                {
                    this._gaugeElement.Gauge.Refresh();
                }
            }
        }

        private void SetColor(BoxAnnotation annotation, Color value)
        {
            if ((annotation.Label.BrushElement != null) && (annotation.Label.BrushElement is SolidFillBrushElement))
            {
                ((SolidFillBrushElement)annotation.Label.BrushElement).Color = value;
            }

        }

        public override string ToString()
        {
            return this._annotation.Label.FormatString;
        }

    }


    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AnnotationBoundsBrowseClass
    {
        #region Поля

        private BoxAnnotation _annotation;
        private EventHandler _changed;
        

        #endregion

        #region Свойства
        
        
        [Description("Расположение по горизонтали")]
        [DisplayName("X")]
        [Browsable(true)]
        public int X
        {
            get {  return this._annotation.Bounds.X; }
            set
            {
                SetX(value);
                DoChanged();
            }
        }

        [Description("Расположение по вертикали")]
        [DisplayName("Y")]
        [Browsable(true)]
        public int Y
        {
            get { return this._annotation.Bounds.Y; }
            set
            {
                SetY(value);
                DoChanged();
            }
        }
        
        /*
        [Description("Высота")]
        [DisplayName("Высота")]
        [Browsable(true)]
        public int Height
        {
            get { return this._annotation.Bounds.Height; }
            set
            {
                SetHeight(value);
                DoChanged();
            }
        }

        [Description("Ширина")]
        [DisplayName("Ширина")]
        [Browsable(true)]
        public int Width
        {
            get { return this._annotation.Bounds.Width; }
            set 
            { 
                SetWidth(value);
                DoChanged();
            }
        }
        */


        #endregion

        public event EventHandler Changed
        {
            add { this._changed += value; }
            remove { this._changed -= value; }
        }

        public AnnotationBoundsBrowseClass(BoxAnnotation annotation)
        {
            this._annotation = annotation;
        }

        private void DoChanged()
        {
            if (this._changed != null)
                this._changed(this, EventArgs.Empty);
        }

        private void SetX(int value)
        {
            this._annotation.Bounds = new Rectangle(value, this._annotation.Bounds.Y, this._annotation.Bounds.Width, this._annotation.Bounds.Height);
        }

        private void SetY(int value)
        {
            this._annotation.Bounds = new Rectangle(this._annotation.Bounds.X, value, this._annotation.Bounds.Width, this._annotation.Bounds.Height);
        }

        private void SetWidth(int value)
        {
            this._annotation.Bounds = new Rectangle(this._annotation.Bounds.X, this._annotation.Bounds.Y, value, this._annotation.Bounds.Height);
        }

        private void SetHeight(int value)
        {
            this._annotation.Bounds = new Rectangle(this._annotation.Bounds.X, this._annotation.Bounds.Y, this._annotation.Bounds.Width, value);
        }


        public override string ToString()
        {
            return String.Format("{0}; {1}; {2}; {3}", this.X, this.Y);
        }
    }

}
