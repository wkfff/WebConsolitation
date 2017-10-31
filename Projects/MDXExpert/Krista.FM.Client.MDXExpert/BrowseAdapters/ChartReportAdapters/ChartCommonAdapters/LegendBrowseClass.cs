using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// �������
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LegendBrowseClass : FilterablePropertyBase
    {
        #region ����

        private LegendAppearance legendAppearance;
        private MarginsBrowseClass marginsBrowse;
        private ChartFormatBrowseClass legendFormat;

        #endregion

        #region ��������

        /// <summary>
        /// ���� ������� �������
        /// </summary>
        [Category("�������")]
        [Description("���� ������� �������")]
        [DisplayName("���� �������")]
        [DefaultValue(typeof(Color), "Navy")]
        [Browsable(true)]
        public Color BorderColor
        {
            get { return legendAppearance.BorderColor; }
            set { legendAppearance.BorderColor = value; }
        }

        /// <summary>
        /// ������� ������� �������
        /// </summary>
        [Category("�������")]
        [Description("������� ������� �������")]
        [DisplayName("������� �������")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(1)]
        [Browsable(true)]
        public int BorderThickness
        {
            get { return legendAppearance.BorderThickness; }
            set { legendAppearance.BorderThickness = value; }
        }

        /// <summary>
        /// ����� ������� �������
        /// </summary>
        [Category("�������")]
        [Description("����� ������� �������")]
        [DisplayName("����� �������")]
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
        /// ���� ���� �������
        /// </summary>
        [Category("�������")]
        [Description("���� ���� �������")]
        [DisplayName("���� ����")]
        [DefaultValue(typeof(Color), "FloralWhite")]
        [Browsable(true)]
        public Color BackColor
        {
            get { return legendAppearance.BackgroundColor; }
            set { legendAppearance.BackgroundColor = value; }
        }

        /// <summary>
        /// ������� ������������ ����
        /// </summary>
        [Category("�������")]
        [Description("������� ������������ ���� �������")]
        [DisplayName("������� ������������ ����")]
        [Editor(typeof(OpacityEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(typeof(byte), "150")]
        [Browsable(true)]
        public byte AlphaLevel
        {
            get { return legendAppearance.AlphaLevel; }
            set { legendAppearance.AlphaLevel = value; }
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
            get { return legendAppearance.Font; }
            set { legendAppearance.Font = value; }
        }

        /// <summary>
        /// ���� ������ ������ �������
        /// </summary>
        [Category("�������")]
        [Description("���� ������ ������ �������")]
        [DisplayName("���� ������")]
        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        public Color Font�olor
        {
            get { return legendAppearance.FontColor; }
            set { legendAppearance.FontColor = value; }
        }

        /// <summary>
        /// ������������ �������
        /// </summary>
        [Category("�������")]
        [Description("������������ �������")]
        [DisplayName("������������")]
        [TypeConverter(typeof(LocationTypeConverter))]
        [DefaultValue(LegendLocation.Right)]
        [Browsable(true)]
        public LegendLocation Location
        {
            get { return legendAppearance.Location; }
            set { legendAppearance.Location = value; }
        }
        
        /// <summary>
        /// ����� ���������� ������ ��� ���
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
        /// ������ �������
        /// </summary>
        [Category("�������")]
        [Description("������ ������� �������")]
        [DisplayName("������ (������)")]
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

        [Category("�������")]
        [DisplayName("������ (������)")]
        [Description("������ ������� �������")]
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

        [Category("�������")]
        [Description("������ �������")]
        [DisplayName("������")]
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
        /// ��������� �������
        /// </summary>
        [Category("�������")]
        [Description("���������� �������")]
        [DisplayName("����������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool Visible
        {
            get { return legendAppearance.Visible; }
            set { legendAppearance.Visible = value; }
        }

        /// <summary>
        /// ������
        /// </summary>
        [Category("�������")]
        [Description("������ � ��������� �� ������/������ ������� ���������")]
        [DisplayName("������")]
        [DefaultValue(25)]
        [Browsable(true)]
        public int SpanPercentage
        {
            get { return legendAppearance.SpanPercentage; }
            set { legendAppearance.SpanPercentage = value; }
        }

        /// <summary>
        /// ���� �������
        /// </summary>
        [Category("�������")]
        [Description("���� �������")]
        [DisplayName("����")]
        [Browsable(true)]
        public MarginsBrowseClass MarginsBrowse
        {
            get { return marginsBrowse; }
            set { marginsBrowse = value; }
        }

        /// <summary>
        /// ���������� ������
        /// </summary>
        [Category("�������")]
        [Description("���������� ������")]
        [DisplayName("���������� ������")]
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