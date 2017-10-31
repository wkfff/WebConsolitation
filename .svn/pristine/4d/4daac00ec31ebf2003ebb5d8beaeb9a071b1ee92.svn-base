using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Легенда
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LegendBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private LegendAppearance legendAppearance;
        private MarginsBrowseClass marginsBrowse;
        private ChartFormatBrowseClass legendFormat;

        #endregion

        #region Свойства

        /// <summary>
        /// Цвет границы легенды
        /// </summary>
        [Category("Легенда")]
        [Description("Цвет границы легенды")]
        [DisplayName("Цвет границы")]
        [DefaultValue(typeof(Color), "Navy")]
        [Browsable(true)]
        public Color BorderColor
        {
            get { return legendAppearance.BorderColor; }
            set { legendAppearance.BorderColor = value; }
        }

        /// <summary>
        /// Толщина границы легенды
        /// </summary>
        [Category("Легенда")]
        [Description("Толщина границы легенды")]
        [DisplayName("Толщина границы")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(1)]
        [Browsable(true)]
        public int BorderThickness
        {
            get { return legendAppearance.BorderThickness; }
            set { legendAppearance.BorderThickness = value; }
        }

        /// <summary>
        /// Стиль границы легенды
        /// </summary>
        [Category("Легенда")]
        [Description("Стиль границы легенды")]
        [DisplayName("Стиль границы")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [DefaultValue(LineDrawStyle.Solid)]
        [Browsable(true)]
        public LineDrawStyle BorderStyle
        {
            get { return legendAppearance.BorderStyle; }
            set { legendAppearance.BorderStyle = value; }
        }

        /// <summary>
        /// Цвет фона легенды
        /// </summary>
        [Category("Легенда")]
        [Description("Цвет фона легенды")]
        [DisplayName("Цвет фона")]
        [DefaultValue(typeof(Color), "FloralWhite")]
        [Browsable(true)]
        public Color BackColor
        {
            get { return legendAppearance.BackgroundColor; }
            set { legendAppearance.BackgroundColor = value; }
        }

        /// <summary>
        /// Уровень прозрачности фона
        /// </summary>
        [Category("Легенда")]
        [Description("Уровень прозрачности фона легенды")]
        [DisplayName("Уровень прозрачности фона")]
        [Editor(typeof(OpacityEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(typeof(byte), "150")]
        [Browsable(true)]
        public byte AlphaLevel
        {
            get { return legendAppearance.AlphaLevel; }
            set { legendAppearance.AlphaLevel = value; }
        }

        /// <summary>
        /// Шрифт текста легенды
        /// </summary>
        [Category("Легенда")]
        [Description("Шрифт текста легенды")]
        [DisplayName("Шрифт текста")]
        [TypeConverter(typeof(FontTypeConverter))]
        [DefaultValue(typeof(Font), "Microsoft Sans Serif, 7.8pt")]
        [Browsable(true)]
        public Font Font
        {
            get { return legendAppearance.Font; }
            set { legendAppearance.Font = value; }
        }

        /// <summary>
        /// Цвет шрифта текста легенды
        /// </summary>
        [Category("Легенда")]
        [Description("Цвет шрифта текста легенды")]
        [DisplayName("Цвет шрифта")]
        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        public Color FontСolor
        {
            get { return legendAppearance.FontColor; }
            set { legendAppearance.FontColor = value; }
        }

        /// <summary>
        /// Расположение легенды
        /// </summary>
        [Category("Легенда")]
        [Description("Расположение легенды")]
        [DisplayName("Расположение")]
        [TypeConverter(typeof(LocationTypeConverter))]
        [DefaultValue(LegendLocation.Right)]
        [Browsable(true)]
        public LegendLocation Location
        {
            get { return legendAppearance.Location; }
            set { legendAppearance.Location = value; }
        }
        
        /// <summary>
        /// нужно показывать формат или нет
        /// </summary>
        [Browsable(false)]
        public bool DisplayFormat
        {
            get 
            {
                switch (this.legendAppearance.ChartComponent.ChartType)
                {
                    case ChartType.BubbleChart:
                    case ChartType.HeatMapChart:
                    case ChartType.HeatMapChart3D:
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Формат легенды
        /// </summary>
        [Category("Легенда")]
        [Description("Строка формата легенды")]
        [DisplayName("Формат (строка)")]
        [DefaultValue("<ITEM_LABEL>")]
        [DynamicPropertyFilter("DisplayFormat", "True")]
        [Browsable(true)]
        public string FormatString
        {
            get 
            { 
                return legendAppearance.FormatString; 
            }
            set 
            {
                LegendFormat.FormatString = value;
                legendAppearance.FormatString = value;
            }
        }

        [Category("Легенда")]
        [DisplayName("Формат (шаблон)")]
        [Description("Шаблон формата легенды")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DynamicPropertyFilter("DisplayFormat", "True")]
        [Browsable(true)]
        public LegendFormatPattern LegendPattern
        {
            get
            {
                return this.legendFormat.LegendPattern;
            }
            set
            {
                this.legendFormat.LegendPattern = value;
            }
        }

        [Category("Легенда")]
        [Description("Формат легенды")]
        [DisplayName("Формат")]
        [DynamicPropertyFilter("DisplayFormat", "True")]
        [Browsable(true)]
        public ChartFormatBrowseClass LegendFormat
        {
            get
            {
                return this.legendFormat;

            }
            set
            {
                this.legendFormat = value;
            }
        }



        /// <summary>
        /// Видимость легенды
        /// </summary>
        [Category("Легенда")]
        [Description("Показывать легенду")]
        [DisplayName("Показывать")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool Visible
        {
            get { return legendAppearance.Visible; }
            set { legendAppearance.Visible = value; }
        }

        /// <summary>
        /// Размер
        /// </summary>
        [Category("Легенда")]
        [Description("Размер в процентах от ширины/высоты области диаграммы")]
        [DisplayName("Размер")]
        [DefaultValue(25)]
        [Browsable(true)]
        public int SpanPercentage
        {
            get { return legendAppearance.SpanPercentage; }
            set { legendAppearance.SpanPercentage = value; }
        }

        /// <summary>
        /// Поля легенды
        /// </summary>
        [Category("Легенда")]
        [Description("Поля легенды")]
        [DisplayName("Поля")]
        [Browsable(true)]
        public MarginsBrowseClass MarginsBrowse
        {
            get { return marginsBrowse; }
            set { marginsBrowse = value; }
        }

        /// <summary>
        /// Ассоциация данных
        /// </summary>
        [Category("Легенда")]
        [Description("Ассоциация данных")]
        [DisplayName("Ассоциация данных")]
        [DefaultValue(ChartTypeData.DefaultData)]
        [Browsable(true)]
        public ChartTypeData DataAssociation
        {
            get { return legendAppearance.DataAssociation; }
            set { legendAppearance.DataAssociation = value; }
        }

        #endregion

        public LegendBrowseClass(LegendAppearance legendAppearance)
        {
            this.legendAppearance = legendAppearance;

            marginsBrowse = new MarginsBrowseClass(legendAppearance.Margins);

            legendFormat = new ChartFormatBrowseClass(legendAppearance.FormatString, 
                                                        ChartFormatBrowseClass.LabelType.Legend, 
                                                        legendAppearance.ChartComponent);

            legendFormat.FormatChanged += new ValueFormatEventHandler(legendFormat_FormatChanged);
        }

        private void legendFormat_FormatChanged()
        {
            /*if (legendFormat.FormatType == FormatType.Auto)
            {
                // legendAppearance.FormatString = "<ITEM_LABEL>";
                PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(LegendAppearance));
                pdc["FormatString"].ResetValue(legendAppearance);

            }
            else*/
            {
                legendAppearance.FormatString = legendFormat.FormatString;
            }
        }

        public override string ToString()
        {
            return LocationTypeConverter.ToString(Location) + "; " + BackColor.Name + "; " + FormatString;
        }
    }
}