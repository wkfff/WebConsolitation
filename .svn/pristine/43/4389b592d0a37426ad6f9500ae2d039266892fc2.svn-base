using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Infragistics.UltraGauge.Resources;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.MDXExpert.Controls;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class  GaugeLegendBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private ExpertLegend _legend;
        private GaugeFormatBrowseClass _format;

        #endregion

        #region Свойства

        [DisplayName("Размер")]
        [Description("Размер легенды")]
        [Browsable(true)]
        public int Size
        {
            get { return this._legend.LegendSize; }
            set { this._legend.LegendSize = value; }
        }


        [Description("Расположение")]
        [DisplayName("Расположение")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(true)]
        public LegendLocation Location
        {
            get { return this._legend.Location; }
            set { this._legend.Location = value; }
        }

        [Description("Показывать/скрывать легенду")]
        [DisplayName("Показывать")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Visible
        {
            get { return this._legend.Visible; }
            set { this._legend.Visible = value; }
        }

        [Description("Показывать/скрывать значения интервалов легенду")]
        [DisplayName("Показывать значения интервалов")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool RangeLimitsVisible
        {
            get { return this._legend.RangeLimitsVisible; }
            set { this._legend.RangeLimitsVisible = value; }
        }


        [DisplayName("Шрифт")]
        [Description("Шрифт")]
        [Browsable(true)]
        public Font Font
        {
            get { return this._legend.Font; }
            set { this._legend.Font = value; }
        }

        [DisplayName("Цвет")]
        [Description("Цвет")]
        [Browsable(true)]
        public Color Color
        {
            get { return this._legend.Color; }
            set { this._legend.Color = value; }
        }

        [DisplayName("Цвет границы")]
        [Description("Цвет границы")]
        [Browsable(true)]
        public Color BorderColor
        {
            get { return this._legend.BorderColor; }
            set { this._legend.BorderColor = value; }
        }

        [Description("Формат значений")]
        [DisplayName("Формат значений")]
        [Browsable(true)]
        public GaugeFormatBrowseClass Format
        {
            get { return this._format; }
            set { this._format = value; }
        }

        #endregion

        public GaugeLegendBrowseClass(ExpertLegend legend)
        {
            this._legend = legend;
            this._format = new GaugeFormatBrowseClass(legend.FormatString);
            this._format.ShowUnitsDisplayType = false;
            this._format.FormatChanged += new ValueFormatEventHandler(format_FormatChanged);

        }

        void format_FormatChanged()
        {
            this._legend.FormatString = this.Format.FormatString;
        }

        public override string ToString()
        {
            return String.Empty;
        }

    }
       
}