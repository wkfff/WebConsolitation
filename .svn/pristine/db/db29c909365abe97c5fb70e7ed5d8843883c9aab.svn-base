using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Подсказки
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TooltipsBrowseClass
    {
        #region Поля

        private WinTooltipAppearance tooptipApperance;
        private PaintElementBrowseClass paintElementBrowse;
        private ChartFormatBrowseClass tooltipFormat;

        #endregion

        #region Свойства

        /// <summary>
        /// Формат подсказки
        /// </summary>
        [Category("Подсказки")]
        [Description("Тип формата подсказки")]
        [DisplayName("Тип формата")]
        [DefaultValue(TooltipStyle.DataValue)]
        [Browsable(false)]
        public TooltipStyle Format
        {
            get 
            { 
                return tooptipApperance.Format; 
            }
            set 
            { 
                tooptipApperance.Format = value;
                this.FormatString = tooptipApperance.FormatString;
            }
        }

        /// <summary>
        /// Строка формата подсказки
        /// </summary>
        [Category("Подсказки")]
        [Description("Строка формата подсказки")]
        [DisplayName("Формат (строка)")]
        [DefaultValue("<DATA_VALUE:00.##>")]
        [Browsable(true)]
        public string FormatString
        {
            get 
            { 
                return tooptipApperance.FormatString; 
            }
            set 
            {
                TooltipFormat.FormatString = value;
                tooptipApperance.FormatString = value;
            }
        }

        [Category("Подсказки")]
        [DisplayName("Формат (шаблон)")]
        [Description("Шаблон формата подсказки")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(true)]
        public TooltipFormatPattern TooltipPattern
        {
            get
            {
                return this.tooltipFormat.TooltipPattern;
            }
            set
            {
                this.tooltipFormat.TooltipPattern = value;
            }
        }
        

        [Category("Подсказки")]
        [Description("Формат подсказки")]
        [DisplayName("Формат")]
        [Browsable(true)]
        public ChartFormatBrowseClass TooltipFormat
        {
            get
            {
                return this.tooltipFormat;

            }
            set
            {
                this.tooltipFormat = value;
            }
        }


        /// <summary>
        /// Цвет выделения
        /// </summary>
        [Category("Подсказки")]
        [Description("Цвет выделения")]
        [DisplayName("Цвет выделения")]
        [DefaultValue(typeof(Color), "Yellow")]
        [Browsable(true)]
        public Color FillColor
        {
            get { return tooptipApperance.HighlightFillColor; }
            set { tooptipApperance.HighlightFillColor = value; }
        }

        /// <summary>
        /// Цвет границы выделения
        /// </summary>
        [Category("Подсказки")]
        [Description("Цвет границы выделения")]
        [DisplayName("Цвет границы выделения")]
        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        public Color OutlineColor
        {
            get { return tooptipApperance.HighlightOutlineColor; }
            set { tooptipApperance.HighlightOutlineColor = value; }
        }

        /// <summary>
        /// Цвет фона подсказки
        /// </summary>
        [Category("Подсказки")]
        [Description("Цвет фона подсказки")]
        [DisplayName("Цвет фона")]
        [Browsable(true)]
        public Color BackColor
        {
            get { return tooptipApperance.BackColor; }
            set { tooptipApperance.BackColor = value; }
        }

        /// <summary>
        /// Шрифт подсказки
        /// </summary>
        [Category("Подсказки")]
        [Description("Шрифт текста подсказки")]
        [DisplayName("Шрифт")]
        [TypeConverter(typeof(FontTypeConverter))]
        [DefaultValue(typeof(Font), "Microsoft Sans Serif, 7.8pt")]
        [Browsable(true)]
        public Font Font
        {
            get { return tooptipApperance.Font; }
            set { tooptipApperance.Font = value; }
        }

        /// <summary>
        /// Цвет шрифта подсказки
        /// </summary>
        [Category("Подсказки")]
        [Description("Цвет шрифта текста подсказки")]
        [DisplayName("Цвет шрифта")]
        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        public Color FontColor
        {
            get { return tooptipApperance.FontColor; }
            set { tooptipApperance.FontColor = value; }
        }

        /// <summary>
        /// Событие выделения
        /// </summary>
        [Category("Подсказки")]
        [Description("Отображение подсказки")]
        [DisplayName("Отображение")]
        [TypeConverter(typeof(TooltipDisplayTypeConverter))]
        [DefaultValue(TooltipDisplay.MouseMove)]
        [Browsable(true)]
        public TooltipDisplay Display
        {
            get { return tooptipApperance.Display; }
            set { tooptipApperance.Display = value; }
        }

        /// <summary>
        /// Элемент отображения
        /// </summary>
        [Category("Подсказки")]
        [Description("Элемент отображения")]
        [DisplayName("Элемент отображения")]
        [Browsable(true)]
        public PaintElementBrowseClass PaintElementBrowse
        {
            get { return paintElementBrowse; }
            set { paintElementBrowse = value; }
        }

        /// <summary>
        /// Видимость выделения
        /// </summary>
        [Category("Подсказки")]
        [Description("Показывать выделения")]
        [DisplayName("Показывать выделения")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool HighlightDataPoint
        {
            get { return tooptipApperance.HighlightDataPoint; }
            set { tooptipApperance.HighlightDataPoint = value; }
        }

        #endregion

        public TooltipsBrowseClass(WinTooltipAppearance tooptipApperance)
        {
            this.tooptipApperance = tooptipApperance;
            paintElementBrowse = new PaintElementBrowseClass(tooptipApperance.PE);

            this.tooltipFormat = new ChartFormatBrowseClass(tooptipApperance.FormatString, 
                                                            ChartFormatBrowseClass.LabelType.Tooltip,
                                                            tooptipApperance.ChartComponent);

            this.tooltipFormat.FormatChanged += new ValueFormatEventHandler(tooltipFormat_FormatChanged);
        }

        private void tooltipFormat_FormatChanged()
        {
            /*if (tooltipFormat.FormatType == FormatType.Auto)
            {
                //tooptipApperance.FormatString = "<DATA_VALUE:00.##>";
                PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(WinTooltipAppearance));
                pdc["FormatString"].ResetValue(tooptipApperance);

            }
            else*/
            {
                tooptipApperance.FormatString = tooltipFormat.FormatString;
            }
        }

        public override string ToString()
        {
            return TooltipDisplayTypeConverter.ToString(Display) + "; " + FillColor.Name + "; " + Font.Name + "; " + BackColor.Name;
        }
    }
}