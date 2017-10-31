using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LabelStyleBrowseClass
    {
        #region ����

        private LabelStyle labelStyle;

        #endregion

        #region ��������

        /// <summary>
        /// ��������� ������
        /// </summary>
        [Category("������� ���������")]
        [Description("��������� ������")]
        [DisplayName("��������� ������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool ClipText
        {
            get { return labelStyle.ClipText; }
            set { labelStyle.ClipText = value; }
        }

        /// <summary>
        /// ��������� ������
        /// </summary>
        [Category("������� ���������")]
        [Description("��������� ������")]
        [DisplayName("��������� ������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool Flip
        {
            get { return labelStyle.Flip; }
            set { labelStyle.Flip = value; }
        }

        /// <summary>
        /// �������� ������ �� ��� X
        /// </summary>
        [Category("������� ���������")]
        [Description("�������� ������ �� ��� X")]
        [DisplayName("�������� �� ��� X")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int Dx
        {
            get { return labelStyle.Dx; }
            set { labelStyle.Dx = value; }
        }

        /// <summary>
        /// �������� ������ �� ��� Y
        /// </summary>
        [Category("������� ���������")]
        [Description("�������� ������ �� ��� Y")]
        [DisplayName("�������� �� ��� Y")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int Dy
        {
            get { return labelStyle.Dy; }
            set { labelStyle.Dy = value; }
        }

        /// <summary>
        /// ����� ������ �������
        /// </summary>
        [Category("�������")]
        [Description("����� ������ �������")]
        [DisplayName("����� ������")]
        [TypeConverter(typeof(FontTypeConverter))]
        [DefaultValue(typeof(Font), "Microsoft Sans Serif, 7.8pt")]
        [Browsable(true)]
        public Font Font
        {
            get { return labelStyle.Font; }
            set { labelStyle.Font = value; }
        }

        /// <summary>
        /// ���� ������ ����� �������
        /// </summary>
        [Category("�������")]
        [Description("���� ������ ����� �������")]
        [DisplayName("���� ������")]
        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        public Color FontColor
        {
            get { return labelStyle.FontColor; }
            set { labelStyle.FontColor = value; }
        }

        /// <summary>
        /// �������������� ������������
        /// </summary>
        [Category("�������")]
        [Description("�������������� ������������")]
        [DisplayName("�������������� ������������")]
        [TypeConverter(typeof(StringAlignmentBarHorizontalConverter))]
        [DefaultValue(StringAlignment.Near)]
        [Browsable(true)]
        public StringAlignment HorizontalAlign
        {
            get { return labelStyle.HorizontalAlign; }
            set { labelStyle.HorizontalAlign = value; }
        }

        /// <summary>
        /// ������������ ������������
        /// </summary>
        [Category("�������")]
        [Description("������������ ������������")]
        [DisplayName("������������ ������������")]
        [TypeConverter(typeof(StringAlignmentBarVerticalConverter))]
        [DefaultValue(StringAlignment.Center)]
        [Browsable(true)]
        public StringAlignment VerticalAlign
        {
            get { return labelStyle.VerticalAlign; }
            set { labelStyle.VerticalAlign = value; }
        }

        /// <summary>
        /// ����������
        /// </summary>
        [Category("�������")]
        [Description("����������")]
        [DisplayName("����������")]
        [TypeConverter(typeof(TextOrientationTypeConverter))]
        [DefaultValue(TextOrientation.Horizontal)]
        [Browsable(true)]
        public TextOrientation Orientation
        {
            get { return labelStyle.Orientation; }
            set { labelStyle.Orientation = value; }
        }

        /// <summary>
        /// ���������� �����
        /// </summary>
        [Category("�������")]
        [Description("���������� �����")]
        [DisplayName("���������� �����")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool ReverseText
        {
            get { return labelStyle.ReverseText; }
            set { labelStyle.ReverseText = value; }
        }

        /// <summary>
        /// ���� ��������
        /// </summary>
        [Category("�������")]
        [Description("���� ��������")]
        [DisplayName("���� ��������")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int RotationAngle
        {
            get { return labelStyle.RotationAngle; }
            set { labelStyle.RotationAngle = value; }
        }

        /// <summary>
        /// �������
        /// </summary>
        [Category("�������")]
        [Description("�������")]
        [DisplayName("�������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool WrapText
        {
            get { return labelStyle.WrapText; }
            set { labelStyle.WrapText = value; }
        }

        /// <summary>
        /// ����������������
        /// </summary>
        [Category("�������")]
        [Description("����������������")]
        [DisplayName("����������������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool FontSizeBestFit
        {
            get { return labelStyle.FontSizeBestFit; }
            set { labelStyle.FontSizeBestFit = value; }
        }

        /// <summary>
        /// ��������
        /// </summary>
        [Category("�������")]
        [Description("��������")]
        [DisplayName("��������")]
        [DefaultValue(StringTrimming.None)]
        [Browsable(true)]
        public StringTrimming Trimming
        {
            get { return labelStyle.Trimming; }
            set { labelStyle.Trimming = value; }
        }

        #endregion

        public LabelStyleBrowseClass(LabelStyle labelStyle)
        {
            this.labelStyle = labelStyle;
        }

        public override string ToString()
        {
            return Font.Name + "; " + TextOrientationTypeConverter.ToString(Orientation) + "; " + Dx + "; " + Dy;
        }
    }
}