using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CandleChartBrowseClass
    {
        #region Поля

        private CandleChartAppearance candleChartAppearance;

        #endregion

        #region Свойства

        /// <summary>
        /// Видимость фитиля
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Показывать фитиль")]
        [DisplayName("Показывать фитиль")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool HighLowVisible
        {
            get { return candleChartAppearance.HighLowVisible; }
            set { candleChartAppearance.HighLowVisible = value; }
        }

        /// <summary>
        /// Цвет фитиля
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Цвет фитиля")]
        [DisplayName("Цвет фитиля")]
        [DefaultValue(typeof(Color), "Blue")]
        [Browsable(true)]
        public Color WickColor
        {
            get { return candleChartAppearance.WickColor; }
            set { candleChartAppearance.WickColor = value; }
        }

        /// <summary>
        /// Толщина фитиля
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Толщина фитиля")]
        [DisplayName("Толщина фитиля")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(1)]
        [Browsable(true)]
        public int WickThickness
        {
            get { return candleChartAppearance.WickThickness; }
            set { candleChartAppearance.WickThickness = value; }
        }

        /// <summary>
        /// Фитиль на переднем плане
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Фитиль на переднем плане")]
        [DisplayName("Фитиль на переднем плане")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool WicksInForeground
        {
            get { return candleChartAppearance.WicksInForeground; }
            set { candleChartAppearance.WicksInForeground = value; }
        }

        /// <summary>
        /// Видимость томов
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Показывать тома")]
        [DisplayName("Показывать тома")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool VolumeVisible
        {
            get { return candleChartAppearance.VolumeVisible; }
            set { candleChartAppearance.HighLowVisible = value; }
        }

        /// <summary>
        /// Цвет томов
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Цвет томов")]
        [DisplayName("Цвет томов")]
        [DefaultValue(typeof(Color), "Beige")]
        [Browsable(true)]
        public Color VolumeColor
        {
            get { return candleChartAppearance.VolumeColor; }
            set { candleChartAppearance.VolumeColor = value; }
        }

        /// <summary>
        /// Цвет положительной области
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Цвет положительной области")]
        [DisplayName("Цвет положительной области")]
        [DefaultValue(typeof(Color), "White")]
        [Browsable(true)]
        public Color PositiveRangeColor
        {
            get { return candleChartAppearance.PositiveRangeColor; }
            set { candleChartAppearance.PositiveRangeColor = value; }
        }

        /// <summary>
        /// Цвет отрицательной области
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Цвет отрицательной области")]
        [DisplayName("Цвет отрицательной области")]
        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        public Color NegativeRangeColor
        {
            get { return candleChartAppearance.NegativeRangeColor; }
            set { candleChartAppearance.NegativeRangeColor = value; }
        }

        /// <summary>
        /// Видимость открытых и закрытых частей
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Видимость открытых и закрытых частей диаграммы")]
        [DisplayName("Видимость открытых и закрытых частей")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool OpenCloseVisible
        {
            get { return candleChartAppearance.OpenCloseVisible; }
            set { candleChartAppearance.OpenCloseVisible = value; }
        }

        /// <summary>
        /// Число пропусков между двумя датами
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Число пропусков между двумя датами")]
        [DisplayName("Число пропусков")]
        [DefaultValue(1)]
        [Browsable(true)]
        public int SkipN
        {
            get { return candleChartAppearance.SkipN; }
            set { candleChartAppearance.SkipN = value; }
        }

        #endregion

        public CandleChartBrowseClass(CandleChartAppearance candleChartAppearance)
        {
            this.candleChartAppearance = candleChartAppearance;
        }

        public override string ToString()
        {
            return WickColor.Name + "; " + PositiveRangeColor.Name + "; " + NegativeRangeColor.Name;
        }
    }
}