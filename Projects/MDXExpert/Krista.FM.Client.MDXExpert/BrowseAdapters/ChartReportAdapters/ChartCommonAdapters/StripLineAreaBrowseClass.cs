using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;
using Krista.FM.Client.MDXExpert.Controls;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// ����� ������� ����� �����
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StripLineAreaBrowseClass : FilterablePropertyBase
    {
        #region ����

        private UltraChart chart;
        private StripLineAppearance stripLineAppearance;

        private Color solidColor = Color.Transparent;
        
        #endregion

        #region ��������

        /// <summary>
        /// ��� �������
        /// </summary>
        [Category("������� �����������")]
        [Description("��� �������")]
        [DisplayName("���")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DefaultValue(CustomPaintElementType.cpSolidFill)]
        [Browsable(true)]
        public CustomPaintElementType ElementType
        {
            get { return PaintElementTypeConverter.ConvertToCustom(stripLineAppearance.PE.ElementType); }
            set
            {
                // ��������������� ���� �������
                if (stripLineAppearance.PE.ElementType == PaintElementType.None)
                {
                    stripLineAppearance.PE.Fill = solidColor;
                }

                stripLineAppearance.PE.ElementType = PaintElementTypeConverter.ConvertFromCustom(value);

                // ��������� ���� �������
                if (stripLineAppearance.PE.ElementType == PaintElementType.None)
                {
                    solidColor = stripLineAppearance.PE.Fill;
                    stripLineAppearance.PE.Fill = Color.Transparent;
                }
                chart.InvalidateLayers();
            }
        }

        /// <summary>
        /// ���� �������
        /// </summary>
        [Category("������� �����������")]
        [Description("���� �������")]
        [DisplayName("���� �������")]
        [DynamicPropertyFilter("ElementType", "cpGradient, cpHatch, cpSolidFill, cpTexture")]
        [DefaultValue(typeof(Color), "Transparent")]
        [Browsable(true)]
        public Color Fill
        {
            get { return stripLineAppearance.PE.Fill; }
            set
            {
                stripLineAppearance.PE.Fill = value;
                chart.InvalidateLayers();
            }
        }

        /// <summary>
        /// ������������ �������
        /// </summary>
        [Category("������� �����������")]
        [Description("������������ �������")]
        [DisplayName("������������ �������")]
        [Editor(typeof(OpacityEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DynamicPropertyFilter("ElementType", "cpGradient, cpHatch, cpSolidFill, cpTexture")]
        [DefaultValue(typeof(byte), "255")]
        [Browsable(true)]
        public byte FillOpacity
        {
            get { return stripLineAppearance.PE.FillOpacity; }
            set
            {
                stripLineAppearance.PE.FillOpacity = value;
                chart.InvalidateLayers();
            }
        }

        /// <summary>
        /// ���� ���� �������
        /// </summary>
        [Category("������� �����������")]
        [Description("���� ���� �������")]
        [DisplayName("���� ����")]
        [DynamicPropertyFilter("ElementType", "cpGradient, cpHatch")]
        [DefaultValue(typeof(Color), "Transparent")]
        [Browsable(true)]
        public Color FillStopColor
        {
            get { return stripLineAppearance.PE.FillStopColor; }
            set
            {
                stripLineAppearance.PE.FillStopColor = value;
                chart.InvalidateLayers();
            }
        }

        /// <summary>
        /// ������������ ���� �������
        /// </summary>
        [Category("������� �����������")]
        [Description("������������ ���� �������")]
        [DisplayName("������������ ����")]
        [Editor(typeof(OpacityEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DynamicPropertyFilter("ElementType", "cpGradient, cpHatch")]
        [DefaultValue(typeof(byte), "255")]
        [Browsable(true)]
        public byte FillStopOpacity
        {
            get { return stripLineAppearance.PE.FillStopOpacity; }
            set
            {
                stripLineAppearance.PE.FillStopOpacity = value;
                chart.InvalidateLayers();
            }
        }

        /// <summary>
        /// �����������
        /// </summary>
        [Category("������� �����������")]
        [Description("�����������")]
        [DisplayName("�����������")]
        [Editor(typeof(ImageSelectEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DynamicPropertyFilter("ElementType", "cpImage")]
        [TypeConverter(typeof(TypeConverter))]
        [Browsable(true)]
        public Image FillImage
        {
            get { return stripLineAppearance.PE.FillImage; }
            set
            {
                stripLineAppearance.PE.FillImage = value;
                chart.InvalidateLayers();
            }
        }

        /// <summary>
        /// ����� �����������
        /// </summary>
        [Category("������� �����������")]
        [Description("����� �����������")]
        [DisplayName("����� �����������")]
        [TypeConverter(typeof(ImageFitStyleTypeConverter))]
        [DynamicPropertyFilter("ElementType", "cpImage")]
        [DefaultValue(ImageFitStyle.StretchedFit)]
        [Browsable(true)]
        public ImageFitStyle ImageFitStyle
        {
            get { return stripLineAppearance.PE.ImageFitStyle; }
            set
            {
                stripLineAppearance.PE.ImageFitStyle = value;
                chart.InvalidateLayers();
            }
        }

        /// <summary>
        /// ��������� ������������� �����������
        /// </summary>
        [Category("������� �����������")]
        [Description("��������� ������������� �����������")]
        [DisplayName("��������� ������������� �����������")]
        [DynamicPropertyFilter("ElementType", "cpImage")]
        [DefaultValue(WrapMode.Tile)]
        [TypeConverter(typeof(WrapModeTypeConverter))]
        [Browsable(true)]
        public WrapMode ImageWrapMode
        {
            get { return stripLineAppearance.PE.ImageWrapMode; }
            set
            {
                stripLineAppearance.PE.ImageWrapMode = value;
                chart.InvalidateLayers();
            }
        }

        /// <summary>
        /// ����� ���������
        /// </summary>
        [Category("������� �����������")]
        [Description("����� ���������")]
        [DisplayName("����� ���������")]
        [Editor(typeof(GradientEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DynamicPropertyFilter("ElementType", "cpGradient")]
        [DefaultValue(GradientStyle.None)]
        [Browsable(true)]
        public GradientStyle FillGradientStyle
        {
            get { return stripLineAppearance.PE.FillGradientStyle; }
            set
            {
                stripLineAppearance.PE.FillGradientStyle = value;
                chart.InvalidateLayers();
            }
        }

        /// <summary>
        /// ����� ���������
        /// </summary>
        [Category("������� �����������")]
        [Description("����� ���������")]
        [DisplayName("����� ���������")]
        [Editor(typeof(HatchEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DynamicPropertyFilter("ElementType", "cpHatch")]
        [DefaultValue(FillHatchStyle.None)]
        [Browsable(true)]
        public FillHatchStyle Hatch
        {
            get { return stripLineAppearance.PE.Hatch; }
            set
            {
                stripLineAppearance.PE.Hatch = value;
                chart.InvalidateLayers();
            }
        }

        /// <summary>
        /// ����� ��������
        /// </summary>
        [Category("������� �����������")]
        [Description("����� ��������")]
        [DisplayName("����� ��������")]
        [Editor(typeof(TextureEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DynamicPropertyFilter("ElementType", "cpTexture")]
        [DefaultValue(TexturePresets.LightGrain)]
        [Browsable(true)]
        public TexturePresets Texture
        {
            get { return stripLineAppearance.PE.Texture; }
            set
            {
                stripLineAppearance.PE.Texture = value;
                chart.InvalidateLayers();
            }
        }

        /// <summary>
        /// ����� ��������
        /// </summary>
        [Category("������� �����������")]
        [Description("��� ��������")]
        [DisplayName("��� ��������")]
        [DefaultValue(TextureApplicationStyle.Normal)]
        [TypeConverter(typeof(TextureApplicationStyleTypeConverter))]
        [DynamicPropertyFilter("ElementType", "cpTexture")]
        [Browsable(true)]
        public TextureApplicationStyle TextureApplication
        {
            get { return stripLineAppearance.PE.TextureApplication; }
            set
            {
                stripLineAppearance.PE.TextureApplication = value;
                chart.InvalidateLayers();
            }
        }

        #endregion

        public StripLineAreaBrowseClass(StripLineAppearance stripLineAppearance, UltraChart chart)
        {
            this.chart = chart;

            this.stripLineAppearance = stripLineAppearance;
        }

        public override string ToString()
        {
            return PaintElementTypeConverter.CustomToString(ElementType);
        }
    }
}
