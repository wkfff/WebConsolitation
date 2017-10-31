using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Client.MDXExpert.Controls;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PaintElementBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private PaintElement paintElement;

        #endregion

        #region Свойства

        /// <summary>
        /// Тип элемента
        /// </summary>
        [Category("Элемент отображения")]
        [Description("Тип элемента")]
        [DisplayName("Тип")]
        [TypeConverter(typeof(PaintElementTypeConverter))]
        [DefaultValue(PaintElementType.SolidFill)]
        [Browsable(true)]
        public PaintElementType ElementType
        {
            get { return paintElement.ElementType; }
            set { paintElement.ElementType = value; }
        }

        /// <summary>
        /// Цвет заливки элемента
        /// </summary>
        [Category("Элемент отображения")]
        [Description("Цвет заливки элемента")]
        [DisplayName("Цвет заливки")]
        [DynamicPropertyFilter("ElementType", "SolidFill")]
        [DefaultValue(typeof(Color), "Transparent")]
        [Browsable(true)]
        public Color Fill
        {
            get { return paintElement.Fill; }
            set { paintElement.Fill = value; }
        }

        /// <summary>
        /// Стиль градиента элемента
        /// </summary>
        [Category("Элемент отображения")]
        [Description("Стиль градиента элемента")]
        [DisplayName("Стиль градиента")]
        [Editor(typeof(GradientEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DynamicPropertyFilter("ElementType", "Gradient")]
        [DefaultValue(GradientStyle.None)]
        [Browsable(true)]
        public GradientStyle FillGradientStyle
        {
            get { return paintElement.FillGradientStyle; }
            set { paintElement.FillGradientStyle = value; }
        }

        /// <summary>
        /// Изображение для элемента
        /// </summary>
        [Category("Элемент отображения")]
        [Description("Изображение для элемента")]
        [DisplayName("Изображение")]
        [Editor(typeof(ImageSelectEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DynamicPropertyFilter("ElementType", "Image")]
        [TypeConverter(typeof(TypeConverter))]
        [Browsable(true)]
        public Image FillImage
        {
            get { return paintElement.FillImage; }
            set { paintElement.FillImage = value; }
        }

        /// <summary>
        /// Прозрачность элемента
        /// </summary>
        [Category("Элемент отображения")]
        [Description("Прозрачность элемента")]
        [DisplayName("Прозрачность")]
        [Editor(typeof(OpacityEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(typeof(byte), "255")]
        [Browsable(true)]
        public byte FillOpacity
        {
            get { return paintElement.FillOpacity; }
            set { paintElement.FillOpacity = value; }
        }

        /// <summary>
        /// Цвет фона заливки элемента
        /// </summary>
        [Category("Элемент отображения")]
        [Description("Цвет фона заливки элемента")]
        [DisplayName("Цвет фона заливки")]
        [DefaultValue(typeof(Color), "Transparent")]
        [Browsable(true)]
        public Color FillStopColor
        {
            get { return paintElement.FillStopColor; }
            set { paintElement.FillStopColor = value; }
        }

        /// <summary>
        /// Прозрачность фона элемента
        /// </summary>
        [Category("Элемент отображения")]
        [Description("Прозрачность фона элемента")]
        [DisplayName("Прозрачность фона")]
        [Editor(typeof(OpacityEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(typeof(byte), "255")]
        [Browsable(true)]
        public byte FillStopOpacity
        {
            get { return paintElement.FillStopOpacity; }
            set { paintElement.FillStopOpacity = value; }
        }

        /// <summary>
        /// Стиль штриховки элемента
        /// </summary>
        [Category("Элемент отображения")]
        [Description("Стиль штриховки элемента")]
        [DisplayName("Штриховка")]
        [Editor(typeof(HatchEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DynamicPropertyFilter("ElementType", "Hatch")] 
        [DefaultValue(FillHatchStyle.None)]
        [Browsable(true)]
        public FillHatchStyle Hatch
        {
            get { return paintElement.Hatch; }
            set { paintElement.Hatch = value; }
        }

        /// <summary>
        /// Стиль изображения
        /// </summary>
        [Category("Элемент отображения")]
        [Description("Стиль изображения")]
        [DisplayName("Стиль изображения")]
        [TypeConverter(typeof(ImageFitStyleTypeConverter))]
        [DynamicPropertyFilter("ElementType", "Image")]
        [DefaultValue(ImageFitStyle.StretchedFit)]
        [Browsable(true)]
        public ImageFitStyle ImageFitStyle
        {
            get { return paintElement.ImageFitStyle; }
            set { paintElement.ImageFitStyle = value; }
        }

        /// <summary>
        /// Клеточное представление изображения
        /// </summary>
        [Category("Элемент отображения")]
        [Description("Клеточное представление изображения")]
        [DisplayName("Клеточное представление изображения")]
        [DynamicPropertyFilter("ElementType", "Image")]
        [DefaultValue(WrapMode.Tile)]
        [TypeConverter(typeof(WrapModeTypeConverter))]
        [Browsable(true)]
        public WrapMode ImageWrapMode
        {
            get { return paintElement.ImageWrapMode; }
            set { paintElement.ImageWrapMode = value; }
        }

        /// <summary>
        /// Цвет границы элемента
        /// </summary>
        [Category("Элемент отображения")]
        [Description("Цвет границы элемента")]
        [DisplayName("Цвет границы")]
        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        public Color Stroke
        {
            get { return paintElement.Stroke; }
            set { paintElement.Stroke = value; }
        }

        /// <summary>
        /// Прозрачность границы элемента
        /// </summary>
        [Category("Элемент отображения")]
        [Description("Прозрачность границы элемента")]
        [DisplayName("Прозрачность границы")]
        [Editor(typeof(OpacityEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(typeof(byte), "255")]
        [Browsable(true)]
        public byte StrokeOpacity
        {
            get { return paintElement.StrokeOpacity; }
            set { paintElement.StrokeOpacity = value; }
        }

        /// <summary>
        /// Ширина границы элемента
        /// </summary>
        [Category("Элемент отображения")]
        [Description("Ширина границы элемента")]
        [DisplayName("Ширина границы")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(1)]
        [Browsable(true)]
        public int StrokeWidth
        {
            get { return paintElement.StrokeWidth; }
            set { paintElement.StrokeWidth = value; }
        }

        /// <summary>
        /// Текстура элемента
        /// </summary>
        [Category("Элемент отображения")]
        [Description("Текстура элемента")]
        [DisplayName("Текстура")]
        [Editor(typeof(TextureEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DynamicPropertyFilter("ElementType", "Texture")]
        [DefaultValue(TexturePresets.LightGrain)]
        [Browsable(true)]
        public TexturePresets Texture
        {
            get { return paintElement.Texture; }
            set { paintElement.Texture = value; }
        }

        /// <summary>
        /// Стиль текстуры элемента
        /// </summary>
        [Category("Элемент отображения")]
        [Description("Стиль текстуры элемента")]
        [DisplayName("Стиль текстуры")]
        [DefaultValue(TextureApplicationStyle.Normal)]
        [TypeConverter(typeof(TextureApplicationStyleTypeConverter))]
        [DynamicPropertyFilter("ElementType", "Texture")]
        [Browsable(true)]
        public TextureApplicationStyle TextureApplication
        {
            get { return paintElement.TextureApplication; }
            set { paintElement.TextureApplication = value; }
        }

        #endregion

        public PaintElementBrowseClass(PaintElement paintElement)
        {
            this.paintElement = paintElement;
        }

        public override string ToString()
        {
            return PaintElementTypeConverter.ToString(ElementType) + "; " + Fill.Name + "; " + FillOpacity;
        }
    }
}