using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// �������� �����
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ColorModelBrowseClass
    {
        #region ����

        private ColorAppearance colorApperance;

        #endregion

        #region ��������

        /// <summary>
        /// ������� ������������
        /// </summary>
        [Category("�������� �����")]
        [Description("������� ������������")]
        [DisplayName("������� ������������")]
        [Editor(typeof(OpacityEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(typeof(byte), "255")]
        [Browsable(true)]
        public byte AlphaLevel
        {
            get { return colorApperance.AlphaLevel; }
            set { colorApperance.AlphaLevel = value; }
        }

        /// <summary>
        /// ��������� ����
        /// </summary>
        [Category("�������� �����")]
        [Description("��������� ����")]
        [DisplayName("��������� ����")]
        [DefaultValue(typeof(Color), "DarkGoldenRod")]
        [Browsable(true)]
        public Color ColorBegin
        {
            get { return colorApperance.ColorBegin; }
            set { colorApperance.ColorBegin = value; }
        }

        /// <summary>
        /// �������� ����
        /// </summary>
        [Category("�������� �����")]
        [Description("�������� ����")]
        [DisplayName("�������� ����")]
        [DefaultValue(typeof(Color), "Navy")]
        [Browsable(true)]
        public Color ColorEnd
        {
            get { return colorApperance.ColorEnd; }
            set { colorApperance.ColorEnd = value; }
        }

        /// <summary>
        /// �������� ������
        /// </summary>
        [Category("�������� �����")]
        [Description("�������� ������")]
        [DisplayName("�������� ������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool GrayScale
        {
            get { return colorApperance.Grayscale; }
            set { colorApperance.Grayscale = value; }
        }

        /// <summary>
        /// ����� �������� �����
        /// </summary>
        [Category("�������� �����")]
        [Description("����� �������� �����")]
        [DisplayName("�����")]
        [DefaultValue(ColorModels.PureRandom)]
        [Browsable(true)]
        public ColorModels ModelStyle
        {
            get { return colorApperance.ModelStyle; }
            set { colorApperance.ModelStyle = value; }
        }

        /// <summary>
        /// ����� ����� ������
        /// </summary>
        [Category("�������� �����")]
        [Description("����� ����� ������")]
        [DisplayName("����� ����� ������")]
        [DefaultValue(ColorScaling.None)]
        [TypeConverter(typeof(ColorScalingTypeConverter))]
        [Browsable(true)]
        public ColorScaling Scaling
        {
            get { return colorApperance.Scaling; }
            set { colorApperance.Scaling = value; }
        }

        #endregion

        public ColorModelBrowseClass(ColorAppearance colorApperance)
        {
            this.colorApperance = colorApperance;
        }

        public override string ToString()
        {
            return ModelStyle + "; " + ColorBegin.Name + "; " + ColorEnd.Name + "; " + AlphaLevel;
        }
    }
}