using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Core.ColorModel;
using Infragistics.UltraChart.Core.Layers;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Data.Series;
using Infragistics.Win.UltraWinChart;

namespace Krista.FM.Client.MDXExpert
{
    public class EmptyAppearanceBrowseClass : FilterablePropertyBase
    {
        private EmptyAppearance _emptyAppearance;
        private LineStyleBrowseClass _lineStyleBrowse;
        private PaintElementBrowseClass _paintElementBrowse;
        private PaintElementBrowseClass _pointPEBrowse;
        private PointStyleBrowseClass _pointStyleBrowse;

        [Category("Свойства")]
        [Description("Включить стиль линии")]
        [DisplayName("Включить стиль линии")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool EnableLineStyle
        {
            get { return this._emptyAppearance.EnableLineStyle; }
            set { this._emptyAppearance.EnableLineStyle = value; }
        }


        [Category("Свойства")]
        [Description("Включить стиль элемента отображения")]
        [DisplayName("Включить стиль элемента отображения")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool EnablePE
        {
            get { return this._emptyAppearance.EnablePE; }
            set { this._emptyAppearance.EnablePE = value; }
        }

        [Category("Свойства")]
        [Description("Включить точки")]
        [DisplayName("Включить точки")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool EnablePoint
        {
            get { return this._emptyAppearance.EnablePoint; }
            set { this._emptyAppearance.EnablePoint = value; }
        }

        [Category("Свойства")]
        [Description("Тип элемента в легенде")]
        [DisplayName("Тип элемента в легенде")]
        [TypeConverter(typeof(LegendEmptyDisplayTypeConverter))]
        [Browsable(true)]
        public LegendEmptyDisplayType LegendDisplayType
        {
            get { return this._emptyAppearance.LegendDisplayType; }
            set { this._emptyAppearance.LegendDisplayType = value; }
        }


        [Category("Свойства")]
        [Description("Стиль линии")]
        [DisplayName("Стиль линии")]
        [Browsable(true)]
        public LineStyleBrowseClass LineStyle
        {
            get { return this._lineStyleBrowse; }
            set { this._lineStyleBrowse = value; }
        }

        [Category("Свойства")]
        [Description("Стиль элемента отображения")]
        [DisplayName("Стиль элемента отображения")]
        [Browsable(true)]
        public PaintElementBrowseClass PE
        {
            get { return this._paintElementBrowse; }
            set { this._paintElementBrowse = value; }
        }

        [Category("Свойства")]
        [Description("Стиль отображения точки")]
        [DisplayName("Стиль отображения точки")]
        [Browsable(true)]
        public PaintElementBrowseClass PointPE
        {
            get { return this._pointPEBrowse; }
            set { this._pointPEBrowse = value; }
        }

        [Category("Свойства")]
        [Description("Вид точки")]
        [DisplayName("Вид точки")]
        [Browsable(true)]
        public PointStyleBrowseClass PointStyle
        {
            get { return this._pointStyleBrowse; }
            set { this._pointStyleBrowse = value; }
        }

        [Category("Свойства")]
        [Description("Стиль отображения точки")]
        [DisplayName("Стиль отображения точки")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool ShowInLegend
        {
            get { return this._emptyAppearance.ShowInLegend; }
            set { this._emptyAppearance.ShowInLegend = value; }
        }

        [Category("Свойства")]
        [Description("Текст")]
        [DisplayName("Текст")]
        [Browsable(true)]
        public string Text
        {
            get { return this._emptyAppearance.Text; }
            set { this._emptyAppearance.Text = value; }
        }


        public EmptyAppearanceBrowseClass(EmptyAppearance emptyAppearance)
        {
            this._emptyAppearance = emptyAppearance;
            this._lineStyleBrowse = new LineStyleBrowseClass(emptyAppearance.LineStyle);
            this._paintElementBrowse = new PaintElementBrowseClass(emptyAppearance.PE);
            this._pointPEBrowse = new PaintElementBrowseClass(emptyAppearance.PointPE);
            this._pointStyleBrowse = new PointStyleBrowseClass(emptyAppearance.PointStyle);
        }

        public override string ToString()
        {
            return "";
        }
    }
}