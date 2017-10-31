using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PolarChartBrowseClass
    {
        #region ����

        private PolarChartAppearance polarChartAppearance;
        private SplineApperanceBrowseClass polarLineBrowse;

        #endregion

        #region ��������

        /// <summary>
        /// ������� ��������� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("������� ��������� �����")]
        [DisplayName("������� ��������� �����")]
        [TypeConverter(typeof(AngleUnitTypeConverter))]
        [Browsable(true)]
        public AngleUnit AngleUnit
        {
            get { return polarChartAppearance.AngleUnit; }
            set { polarChartAppearance.AngleUnit = value; }
        }

        /// <summary>
        /// ������ �����
        /// </summary>
        [Category("������� ���������")]
        [Description("������ �����")]
        [DisplayName("������ �����")]
        [Browsable(true)]
        public Char Character
        {
            get { return polarChartAppearance.Character; }
            set { polarChartAppearance.Character = value; }
        }

        /// <summary>
        /// ����� �������
        /// </summary>
        [Category("������� ���������")]
        [Description("����� �������")]
        [DisplayName("����� �������")]
        [TypeConverter(typeof(FontTypeConverter))]
        [Browsable(true)]
        public Font CharacterFont
        {
            get { return polarChartAppearance.CharacterFont; }
            set { polarChartAppearance.CharacterFont = value; }
        }

        /// <summary>
        /// ������ ��� X
        /// </summary>
        [Category("������� ���������")]
        [Description("������ ��� X")]
        [DisplayName("������ ��� X")]
        [Browsable(true)]
        public int ColumnX
        {
            get { return polarChartAppearance.ColumnX; }
            set { polarChartAppearance.ColumnX = value; }
        }

        /// <summary>
        /// ������ ��� Y
        /// </summary>
        [Category("������� ���������")]
        [Description("������ ��� Y")]
        [DisplayName("������ ��� Y")]
        [Browsable(true)]
        public int ColumnY
        {
            get { return polarChartAppearance.ColumnY; }
            set { polarChartAppearance.ColumnY = value; }
        }

        /// <summary>
        /// ���������� ����� ������
        /// </summary>
        [Category("������� ���������")]
        [Description("���������� ����� ������")]
        [DisplayName("���������� �����")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool ConnectWithLines
        {
            get { return polarChartAppearance.ConnectWithLines; }
            set { polarChartAppearance.ConnectWithLines = value; }
        }

        /// <summary>
        /// ������� �������
        /// </summary>
        [Category("������� ���������")]
        [Description("������� �������")]
        [DisplayName("������� �������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool FillArea
        {
            get { return polarChartAppearance.FillArea; }
            set { polarChartAppearance.FillArea = value; }
        }

        /// <summary>
        /// ��������� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("��������� �����")]
        [DisplayName("��������� �����")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool EnableLabelFlipping
        {
            get { return polarChartAppearance.EnableLabelFlipping; }
            set { polarChartAppearance.EnableLabelFlipping = value; }
        }

        /// <summary>
        /// ��������� ���� ��������� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("��������� ���� ��������� �����")]
        [DisplayName("��������� ���� ��������� �����")]
        [Browsable(true)]
        public int LabelFlippingStartAngle
        {
            get { return polarChartAppearance.LabelFlippingStartAngle; }
            set { polarChartAppearance.LabelFlippingStartAngle = value; }
        }

        /// <summary>
        /// �������� ���� ��������� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("�������� ���� ��������� �����")]
        [DisplayName("�������� ���� ��������� �����")]
        [Browsable(true)]
        public int LabelFlippingEndAngle
        {
            get { return polarChartAppearance.LabelFlippingEndAngle; }
            set { polarChartAppearance.LabelFlippingEndAngle = value; }
        }

        /// <summary>
        /// ��� ������
        /// </summary>
        [Category("������� ���������")]
        [Description("��� ������")]
        [DisplayName("������")]
        [TypeConverter(typeof(SymbolIconTypeConverter))]
        [Browsable(true)]
        public SymbolIcon Icon
        {
            get { return polarChartAppearance.Icon; }
            set { polarChartAppearance.Icon = value; }
        }

        /// <summary>
        /// ������ ������
        /// </summary>
        [Category("������� ���������")]
        [Description("������ ������")]
        [DisplayName("������ ������")]
        [TypeConverter(typeof(SymbolIconSizeTypeConverter))]
        [Browsable(true)]
        public SymbolIconSize IconSize
        {
            get { return polarChartAppearance.IconSize; }
            set { polarChartAppearance.IconSize = value; }
        }

        /// <summary>
        /// ������ ����������� �� ��������
        /// </summary>
        [Category("������� ���������")]
        [Description("������ ����������� �� ��������")]
        [DisplayName("������ ����������� �� ��������")]
        [Browsable(true)]
        public int GroupByColumn
        {
            get { return polarChartAppearance.GroupByColumn; }
            set { polarChartAppearance.GroupByColumn = value; }
        }

        /// <summary>
        /// ����������� �� ��������
        /// </summary>
        [Category("������� ���������")]
        [Description("����������� �� ��������")]
        [DisplayName("����������� �� ��������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool UseGroupByColumn
        {
            get { return polarChartAppearance.UseGroupByColumn; }
            set { polarChartAppearance.UseGroupByColumn = value; }
        }

        /// <summary>
        /// ��������� ������������ ����
        /// </summary>
        [Category("������� ���������")]
        [Description("��������� ������������ ����")]
        [DisplayName("��������� ������������ ����")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool FreeStandingAxis
        {
            get { return polarChartAppearance.FreeStandingAxis; }
            set { polarChartAppearance.FreeStandingAxis = value; }
        }

        /// <summary>
        /// ����� ����� � ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("����� ����� � ���������")]
        [DisplayName("�����")]
        [Browsable(true)]
        public SplineApperanceBrowseClass PolarLineBrowse
        {
            get { return polarLineBrowse; }
            set { polarLineBrowse = value; }
        }

        /// <summary>
        /// ����������� ������ ��������
        /// </summary>
        [Category("������� ���������")]
        [Description("����������� ������ ��������")]
        [DisplayName("������ ��������")]
        [TypeConverter(typeof(NullHandlingTypeConverter))]
        [Browsable(true)]
        public NullHandling NullHandling
        {
            get { return polarChartAppearance.NullHandling; }
            set { polarChartAppearance.NullHandling = value; }
        }

        /// <summary>
        /// ������� ������
        /// </summary>
        [Category("������� ���������")]
        [Description("������� ������")]
        [DisplayName("������� ������")]
        [Editor(typeof(CustomChartTextCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public ChartTextCollection ChartText
        {
            get { return polarChartAppearance.ChartText; }
        }

        #endregion

        public PolarChartBrowseClass(PolarChartAppearance polarChartAppearance)
        {
            this.polarChartAppearance = polarChartAppearance;
            polarLineBrowse = new SplineApperanceBrowseClass(polarChartAppearance.LineAppearance);
        }

        public override string ToString()
        {
            return SymbolIconTypeConverter.ToString(Icon) + "; " + AngleUnitTypeConverter.ToString(AngleUnit) + "; " + LineDrawStyleTypeConverter.ToString(polarLineBrowse.DrawStyle);
        }
    }
}