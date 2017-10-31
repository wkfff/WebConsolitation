using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;
using Infragistics.Win.UltraWinDock;
using System.Xml;
using Krista.FM.Expert.PivotData;
using Infragistics.Win;
using System.Data;
using System.Windows.Forms.Design;

namespace Krista.FM.Client.MDXExpert
{


    class CustomReportElementBrowseAdapter : FilterablePropertyBase
    {
        #region ����

        private CustomReportElement reportElement;
        private DockableControlPane dockableControlPane;

        private CaptionBrowseClass captionBrowse;
        private CommentBrowseClass commentBrowse;
        private ChartLabelsBrowseClass chartColumnsLabelsBrowse;
        private ChartLabelsBrowseClass chartRowsLabelsBrowse;

        #endregion

        /// <summary>
        /// �������� ������ ���������
        /// </summary>
        public class SourceDataDropDownEditor : UITypeEditor
        {

            private ChartReportElement chartElement;

            /// <summary>
            /// ���������� ������ ��������������
            /// </summary>
            public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider, Object value)
            {
                if ((context != null) && (provider != null))
                {
                    IWindowsFormsEditorService svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                    if (svc != null)
                    {
                        this.chartElement = (ChartReportElement)value;
                        RelationalTable tbl = new RelationalTable();
                        tbl.SetDataSource(this.chartElement.SourceDT);
                        tbl.AfterDataSourceUpdate += new RelationalTable.DataSourceEventHandler(tbl_AfterDataSourceUpdate);
                        tbl.Tag = svc;
                        svc.DropDownControl(tbl);
                    }
                }
                return base.EditValue(context, provider, value);
            }

            /// <summary>
            /// ��� �������������� ������������� ������, ����� ���������� ���������� 
            /// DataTable ���������
            /// </summary>
            /// <param name="dt"></param>
            void tbl_AfterDataSourceUpdate(DataTable dt)
            {
                if (this.chartElement != null)
                    this.chartElement.SourceDT = dt;
            }

            /// <summary>
            /// ���������� ����� ��������� - ���������� ����
            /// </summary>
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                if (context != null)
                {
                    return UITypeEditorEditStyle.DropDown;
                }
                else
                {
                    return base.GetEditStyle(context);
                }
            }
        }

        public CustomReportElementBrowseAdapter(DockableControlPane dcPane)
        {
            reportElement = (CustomReportElement)dcPane.Control;
            dockableControlPane = dcPane;

            captionBrowse = new CaptionBrowseClass(reportElement);
            commentBrowse = new CommentBrowseClass(reportElement);
            
            chartColumnsLabelsBrowse = new ChartLabelsBrowseClass(reportElement, reportElement.PivotData.ColumnAxis);
            chartRowsLabelsBrowse = new ChartLabelsBrowseClass(reportElement, reportElement.PivotData.RowAxis);           
        }

        #region ��������

        [Category("���������� �������")]
        [DisplayName("����������")]
        [Description("���� ���� ����������, ��������� �������� �� ������ �� ����")]
        [DynamicPropertyFilter("ElementType", "eChart")]
        [DefaultValue(true)]       
        [TypeConverter(typeof(BooleanTypeConverter))]
        public bool IsUpdatable
        {
            get
            {
                return ((ChartReportElement)reportElement).IsUpdatable;
            }
            set
            {
                ((ChartReportElement)reportElement).IsUpdatable = value;
            }
        }


        [Category("���������� �������")]
        [DisplayName("������")]
        [Description("������")]               
        [DynamicPropertyFilter("ElementType", "eChart")]
        [Editor(typeof(SourceDataDropDownEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(TypeConverter))]
        public ChartReportElement SourceData
        {
            get
            {                
                return ((ChartReportElement)reportElement);                   
            }
            set
            {
            }
        }

        [Category("���������� �������")]
        [Description("����� ���������")]        
        [DisplayName("����� ���������")]
        [DynamicPropertyFilter("ElementType", "eChart")]
        [Browsable(true)]
        public ChartLabelsBrowseClass Chart�olumnsLabelsBrowse
        {
            get { return chartColumnsLabelsBrowse; }
            set { chartColumnsLabelsBrowse = value; }
        }

        [Category("���������� �������")]
        [Description("����� �����")]
        [DisplayName("����� �����")]
        [DynamicPropertyFilter("ElementType", "eChart")]
        [Browsable(true)]
        public ChartLabelsBrowseClass ChartRowsLabelsBrowse
        {
            get { return chartRowsLabelsBrowse; }
            set { chartRowsLabelsBrowse = value; }
        }

        [Category("������� ������")]
        [DisplayName("���")]
        [Description("���, �� �������� ������� ������ ��� �������� ������")]
        [Browsable(true)]
        public string CubeDef
        {
            get { return reportElement.PivotData.CubeName; }
        }

        [Category("MDX-������")]
        [DisplayName("����� �������")]
        [Description("����� MDX-������� � ����")]
        [Browsable(true)]
        public string MDXQuery
        {
            get { return reportElement.MDXQuery; }
        }

        [Category("MDX-������")]
        [DisplayName("����� ������")]
        [Description("����� ������ ����������� MDX-��������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool IsKeepQueryLog
        {
            get { return reportElement.MainForm.MdxCommand.IsKeepLog; }
            set { reportElement.MainForm.MdxCommand.IsKeepLog = value; }
        }
        /*
        [Category("MDX-������")]
        [DisplayName("��� �������")]
        [Description("��� ������ ����������� MDX-��������")]
        [Editor(typeof(DocFileEditor), typeof(UITypeEditor))]
        [DefaultValue(Application.LocalUserAppDataPath + Common.Consts.queryLogName)]
        [Browsable(true)]
        public string QueryLogPath
        {
            get { return reportElement.MainForm.MdxCommand.LogPath; }
            set { reportElement.MainForm.MdxCommand.LogPath = value; }
        }
        */

        [Category("������� ������")]
        [DisplayName("������������")]
        [Description("������������ ��������")]
        [Browsable(true)]
        public string Name
        {
            get { return dockableControlPane.Text; }
            set { dockableControlPane.Text = value; }
        }

        [Category("������� ������")]
        [DisplayName("�����������")]
        [Description("�������������� ������� ���� ��������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool Pinned
        {
            get { return !dockableControlPane.Pinned; }
            set { dockableControlPane.Pinned = !value; }
        }

        [Category("������� ������")]
        [DisplayName("���������")]
        [Description("��������� ������ ��������")]
        [Browsable(true)]
        public string PivotData
        {
            get { return reportElement.PivotData.StrSettings; }
        }

        //��������� ��������
        [Category("������� ������")]
        [Description("���������")]
        [DisplayName("���������")]
        [Browsable(true)]
        public CaptionBrowseClass GridCaptionBrowse
        {
            get { return captionBrowse; }
            set { captionBrowse = value; }
        }

        //����������� � ��������
        [Category("������� ������")]
        [Description("�����������")]
        [DisplayName("�����������")]
        [Browsable(true)]
        public CommentBrowseClass GridCommentBrowse
        {
            get { return commentBrowse; }
            set { commentBrowse = value; }
        }

        /*
        [DisplayName("������ ���������")]
        [Description("������ ��������� ������ �������� ������")]
        [Browsable(true)]
        [Editor(typeof(PivotDataElementEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(PivotObjectConverter))]
        public PivotObject PivotDataElement
        {
            get 
            {
                if (reportElement.PivotData.Selection != null)
                {
                    return reportElement.PivotData.Selection;
                }
                else
                {
                    return reportElement.PivotData;
                }
            }
            set 
            {
                reportElement.PivotData.Selection = value; 
            }
        }*/

        [Category("������� ������")]
        [DisplayName("���")]
        [Description("��� ��������")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(true)]
        public ReportElementType ElementType
        {
            get { return reportElement.ElementType; }
        }

        #endregion
    }

    #region ����������

    #region BooleanTypeConverter

    class BooleanTypeConverter : BooleanConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context,
          CultureInfo culture,
          object value,
          Type destType)
        {
            return ToString(value);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            return (string)value == "��";
        }

        public static string ToString(object value)
        {
            if (value == null)
                return "���";
            else
                return (bool)value ? "��" : "���";
        }
    }

    #endregion

    #region DocFileEditor

    class DocFileEditor : FileNameEditor
    {
        /// <summary>
        /// ��������� ������� ���������� 
        /// </summary>
        protected override void InitializeDialog(OpenFileDialog ofd)
        {
            ofd.CheckFileExists = false;
            ofd.Title = "��� �����";
            ofd.Filter = "��������� �������� (*.txt)|*.txt|All files (*.*)|*.*";
        }
    }

    #endregion

    #region BorderStyleTypeConvertor

    class BorderStyleTypeConvertor : EnumConverter
    {
        const string Dashed = "���������";
        const string Default = "�� ���������";
        const string Dotted = "����������";
        const string Etched = "�������������";
        const string Inset = "����������";
        const string InsetSoft = "���� ����������";
        const string None = "���";
        const string Raised = "�����������";
        const string RaisedSoft = "���� �����������";
        const string Rounded1 = "������������";
        const string Rounded1Etched = "����������� �������������";
        const string Rounded4 = "������ ������������";
        const string Rounded4Thick = "����������� ����������";
        const string Solid = "��������";
        const string TwoColor = "����������";
        const string WindowsVista = "WindowsVista";


        public BorderStyleTypeConvertor(Type type)
            : base(type)
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destType)
        {
            return ToString(value);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            switch ((string)value)
            {
                case Dashed : return UIElementBorderStyle.Dashed;
                case Default : return UIElementBorderStyle.Default;
                case Dotted : return UIElementBorderStyle.Dotted;
                case Etched : return UIElementBorderStyle.Etched;
                case Inset : return UIElementBorderStyle.Inset;
                case InsetSoft : return UIElementBorderStyle.InsetSoft;
                case None : return UIElementBorderStyle.None;
                case Raised : return UIElementBorderStyle.Raised;
                case RaisedSoft : return UIElementBorderStyle.RaisedSoft;
                case Rounded1 : return UIElementBorderStyle.Rounded1;
                case Rounded1Etched : return UIElementBorderStyle.Rounded1Etched;
                case Rounded4 : return UIElementBorderStyle.Rounded4;
                case Rounded4Thick : return UIElementBorderStyle.Rounded4Thick;
                case Solid : return UIElementBorderStyle.Solid;
                case TwoColor : return UIElementBorderStyle.TwoColor;
                case WindowsVista : return UIElementBorderStyle.WindowsVista;
            }
            return UIElementBorderStyle.Solid;
        }

        public static string ToString(object value)
        {
            switch ((UIElementBorderStyle)value)
            {
                case UIElementBorderStyle.Dashed: return Dashed;
                case UIElementBorderStyle.Default: return Default;
                case UIElementBorderStyle.Dotted: return Dotted;
                case UIElementBorderStyle.Etched: return Etched;
                case UIElementBorderStyle.Inset: return Inset;
                case UIElementBorderStyle.InsetSoft: return InsetSoft;
                case UIElementBorderStyle.None: return None;
                case UIElementBorderStyle.Raised: return Raised;
                case UIElementBorderStyle.RaisedSoft: return RaisedSoft;
                case UIElementBorderStyle.Rounded1: return Rounded1;
                case UIElementBorderStyle.Rounded1Etched: return Rounded1Etched;
                case UIElementBorderStyle.Rounded4: return Rounded4;
                case UIElementBorderStyle.Rounded4Thick: return Rounded4Thick;
                case UIElementBorderStyle.Solid: return Solid;
                case UIElementBorderStyle.TwoColor: return TwoColor;
                case UIElementBorderStyle.WindowsVista: return WindowsVista;
            }
            return string.Empty;
        }
    }
    #endregion

    #region CommentPlaceTypeConverter

    class CommentPlaceTypeConverter : EnumConverter
    {
        const string Top = "������";
        const string Right = "������";
        const string Bottom = "�����";
        const string Left = "�����";

        public CommentPlaceTypeConverter(Type type)
            : base(type)
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destType)
        {
            return ToString(value);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            switch ((string)value)
            {
                case Top: return CommentPlace.Top;
                case Right: return CommentPlace.Right;
                case Bottom: return CommentPlace.Bottom;
                case Left: return CommentPlace.Left;
            }
            return CommentPlace.Top;
        }

        public static string ToString(object value)
        {
            switch ((CommentPlace)value)
            {
                case CommentPlace.Top: return Top;
                case CommentPlace.Right: return Right;
                case CommentPlace.Bottom: return Bottom;
                case CommentPlace.Left: return Left;
            }
            return string.Empty;
        }
    }

    #endregion

    #region TextHAlignTypeConverter

    class TextHAlignTypeConverter : EnumConverter
    {
        const string Center = "�� ������";
        const string Default = "�� ���������";
        const string Left = "�� ������ ����";
        const string Right = "�� ������� ����";

        public TextHAlignTypeConverter(Type type)
            : base(type)
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destType)
        {
            return ToString(value);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            switch ((string)value)
            {
                case Center: return HAlign.Center;
                case Default: return HAlign.Default;
                case Left: return HAlign.Left;
                case Right: return HAlign.Right;
            }
            return HAlign.Center;
        }

        public static string ToString(object value)
        {
            switch ((HAlign)value)
            {
                case HAlign.Center: return Center;
                case HAlign.Default: return Default;
                case HAlign.Left: return Left;
                case HAlign.Right: return Right;
            }
            return string.Empty;
        }
    }

    #endregion

    #region TextVAlignTypeConverter

    class TextVAlignTypeConverter : EnumConverter
    {
        const string Bottom = "�� ������� ����";
        const string Default = "�� ���������";
        const string Middle = "�� ��������";
        const string Top = "�� �������� ����";

        public TextVAlignTypeConverter(Type type)
            : base(type)
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destType)
        {
            return ToString(value);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            switch ((string)value)
            {
                case Bottom: return VAlign.Bottom;
                case Default: return VAlign.Default;
                case Middle: return VAlign.Middle;
                case Top: return VAlign.Top;
            }
            return VAlign.Middle;
        }

        public static string ToString(object value)
        {
            switch ((VAlign)value)
            {
                case VAlign.Bottom: return Bottom;
                case VAlign.Default: return Default;
                case VAlign.Middle: return Middle;
                case VAlign.Top: return Top;
            }
            return string.Empty;
        }
    }

    #endregion


    #region EnumTypeConverter

    /// <summary>
    /// TypeConverter ��� Enum, ����������������� Enum � ������ �
    /// ������ �������� Description
    /// </summary>
    class EnumTypeConverter : EnumConverter
    {
        private Type _enumType;
        /// <summary>�������������� ���������</summary>
        /// <param name="type">��� Enum</param>
        public EnumTypeConverter(Type type)
            : base(type)
        {
            _enumType = type;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context,
          Type destType)
        {
            return destType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context,
          CultureInfo culture,
          object value, Type destType)
        {
            if (value == null)
                return string.Empty;
            FieldInfo fi = _enumType.GetField(Enum.GetName(_enumType, value));
            DescriptionAttribute dna =
              (DescriptionAttribute)Attribute.GetCustomAttribute(
                fi, typeof(DescriptionAttribute));

            if (dna != null)
                return dna.Description;
            else
                return value.ToString();
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context,
          Type srcType)
        {
            return srcType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            foreach (FieldInfo fi in _enumType.GetFields())
            {
                DescriptionAttribute dna =
                  (DescriptionAttribute)Attribute.GetCustomAttribute(
                    fi, typeof(DescriptionAttribute));

                if ((dna != null) && ((string)value == dna.Description))
                    return Enum.Parse(_enumType, fi.Name);
            }

            return Enum.Parse(_enumType, (string)value);
        }

        public static string ToString(object value, Type enumType)
        {
            FieldInfo fi = enumType.GetField(Enum.GetName(enumType, value));
            DescriptionAttribute dna =
              (DescriptionAttribute)Attribute.GetCustomAttribute(
                fi, typeof(DescriptionAttribute));

            if (dna != null)
                return dna.Description;
            else
                return value.ToString();
        }
    }

    #endregion

    #region FontTypeConverter
    class FontTypeConverter : TypeConverter
    {
        /// <summary>
        /// ������ � ������
        /// </summary>
        public override bool CanConvertTo(
          ITypeDescriptorContext context, Type destType)
        {
            return true;
        }

        /// <summary>
        /// � ������ ���
        /// </summary>
        public override object ConvertTo(
          ITypeDescriptorContext context, CultureInfo culture,
          object value, Type destType)
        {
            FontConverter convertor = new FontConverter();
            return convertor.ConvertToString(value);
        }
    }
    #endregion

    #endregion

    #region ������ ��� ������������� ������������� ���������� ������� PropertyGrid

    /// <summary>
    /// ������� ��� ��������� ����������� ������������ �������
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    class DynamicPropertyFilterAttribute : Attribute
    {
        string _propertyName;

        /// <summary>
        /// �������� ��������, �� �������� ����� �������� ���������
        /// </summary>
        public string PropertyName
        {
            get { return _propertyName; }
        }

        string _showOn;

        /// <summary>
        /// �������� ��������, �� �������� ������� ��������� 
        /// (����� �������, ���� ���������), ��� ������� ��������, �
        /// �������� �������� �������, ����� ������. 
        /// </summary>
        public string ShowOn
        {
            get { return _showOn; }
        }

        /// <summary>
        /// �����������  
        /// </summary>
        /// <param name="propertyName">�������� ��������, �� �������� ����� 
        /// �������� ���������</param>
        /// <param name="value">�������� �������� (����� �������, ���� ���������), 
        /// ��� ������� ��������, � �������� �������� �������, ����� ������.</param>
        public DynamicPropertyFilterAttribute(string propertyName, string value)
        {
            _propertyName = propertyName;
            _showOn = value;
        }
    }

    /// <summary>
    /// ������� ����� ��� ��������, �������������� ������������ 
    /// ����������� ������� � PropertyGrid
    /// </summary>
    public class FilterablePropertyBase : ICustomTypeDescriptor
    {

        protected PropertyDescriptorCollection
          GetFilteredProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection pdc
              = TypeDescriptor.GetProperties(this, attributes, true);

            PropertyDescriptorCollection finalProps =
              new PropertyDescriptorCollection(new PropertyDescriptor[0]);

            foreach (PropertyDescriptor pd in pdc)
            {
                bool include = false;
                bool dynamic = false;

                foreach (Attribute a in pd.Attributes)
                {
                    if (a is DynamicPropertyFilterAttribute)
                    {
                        dynamic = true;

                        DynamicPropertyFilterAttribute dpf =
                         (DynamicPropertyFilterAttribute)a;

                        string[] strArray = dpf.ShowOn.Split(',');
                        foreach (string strItem in strArray)
                        {
                            if (string.Compare(strItem.Trim(' '), pdc[dpf.PropertyName].GetValue(this).ToString()) == 0)
                            {
                                include = true;
                                break;
                            }
                        }
                    }
                }

                if (!dynamic || include)
                    finalProps.Add(pd);
            }

            return finalProps;
        }

        #region ICustomTypeDescriptor Members

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public PropertyDescriptorCollection GetProperties(
          Attribute[] attributes)
        {
            return GetFilteredProperties(attributes);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return GetFilteredProperties(new Attribute[0]);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        #endregion
    }

    #endregion

    #region �������������� ������ ��� ����������� � PropertyGrid
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CommentBrowseClass
    {
        private CustomReportElement reportElement;

        public CommentBrowseClass(CustomReportElement element)
        {
            this.reportElement = element;
        }

        public override string ToString()
        {
            return string.Empty;
        }

        #region ��������
        //�����
        [Category("�����������")]
        [Description("����� ������������ � ����������� � �������� ������")]
        [DisplayName("�����")]
        [Browsable(true)]
        public string CommentText
        {
            get { return this.reportElement.Comment.Text; }
            set { this.reportElement.Comment.Text = value; }
        }

        //���������
        [Category("�����������")]
        [Description("���������� ������������")]
        [DisplayName("����������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool CommentVisibility
        {
            get { return this.reportElement.Comment.Visible; }
            set { this.reportElement.Comment.Visible = value; }
        }

        //�����
        [Category("�����������")]
        [Description("����� �����������")]
        [DisplayName("�����")]
        [TypeConverter(typeof(FontTypeConverter))]
        [Browsable(true)]
        public Font CommentFont
        {
            get { return this.reportElement.Comment.Font; }
            set { this.reportElement.Comment.Font = value; }
        }

        //����� ������������
        [Category("�����������")]
        [Description("����� ������������ �����������")]
        [DisplayName("������������")]
        [TypeConverter(typeof(CommentPlaceTypeConverter))]
        [Browsable(true)]
        public CommentPlace CommentPlace
        {
            get { return this.reportElement.Comment.Place; }
            set { this.reportElement.Comment.Place = value; }
        }

        //�������������� ������������
        [Category("�����������")]
        [Description("�������������� ������������ ������ � ����������� ������")]
        [DisplayName("�������������� ������������")]
        [TypeConverter(typeof(TextHAlignTypeConverter))]
        [Browsable(true)]
        public HAlign CommentTextHAlign
        {
            get { return this.reportElement.Comment.TextHAlign; }
            set { this.reportElement.Comment.TextHAlign = value; }
        }

        //������������ ������������
        [Category("�����������")]
        [Description("������������ ������������ ������ � ����������� ������")]
        [DisplayName("������������ ������������")]
        [TypeConverter(typeof(TextVAlignTypeConverter))]
        [Browsable(true)]
        public VAlign CommentTextVAlign
        {
            get { return this.reportElement.Comment.TextVAlign; }
            set { this.reportElement.Comment.TextVAlign = value; }
        }

        //����� �������
        [Category("�����������")]
        [Description("����� ������� ����������� ������")]
        [DisplayName("����� �������")]
        [TypeConverter(typeof(BorderStyleTypeConvertor))]
        [Browsable(true)]
        public UIElementBorderStyle BorderStyleType
        {
            get { return this.reportElement.Comment.BorderStyle; }
            set { this.reportElement.Comment.BorderStyle = value; }
        }

        //���� ����
        [Category("�����������")]
        [Description("���� ���� ����������� ������")]
        [DisplayName("���� ����")]
        [Browsable(true)]
        public Color CommentBackColor
        {
            get { return this.reportElement.Comment.BackColor; }
            set { this.reportElement.Comment.BackColor = value; }
        }

        //���� ����
        [Category("�����������")]
        [Description("���� ������ ����������� ������")]
        [DisplayName("���� ������")]
        [Browsable(true)]
        public Color CommentForeColor
        {
            get { return this.reportElement.Comment.Appearance.ForeColor; }
            set { this.reportElement.Comment.Appearance.ForeColor = value; }
        }

        #endregion
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ChartLabelsBrowseClass
    {
        private CustomReportElement reportElement;
        private Axis axis;
         
        public ChartLabelsBrowseClass(CustomReportElement element, Axis ax)
        {
            axis = ax;
            this.reportElement = element;
        }

        public override string ToString()
        {
            return string.Empty;
        }

        [Category("��������� �����")]
        [DisplayName("������")]
        [Description("���������, ������������ � ������������ �����")]
        [DynamicPropertyFilter("ElementType", "eChart")]
        [Editor(typeof(ChartLabelsElementDropDownEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(TypeConverter))]
        public Axis ChartRowsLabels
        {
            get
            {
                return axis;
            }
            set
            {
                ((ChartReportElement)reportElement).InitialByCellSet();
            }
        }

        [Category("��������� �����")]
        [DisplayName("����������� ���������")]
        [Description("������, ����������� �������� �����")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DynamicPropertyFilter("ElementType", "eChart")]
        public ChartSeriesSeparator ColumnsSeparator
        {
            get
            {
                if (axis.AxisType == AxisType.atColumns)
                {
                    return ((ChartReportElement)reportElement).ColumnsSeparator;
                }
                else
                {
                    return ((ChartReportElement)reportElement).RowsSeparator;
                }
            }
            set
            {
                if (axis.AxisType == AxisType.atColumns)
                {
                    ((ChartReportElement)reportElement).ColumnsSeparator = value;
                }
                else
                {
                    ((ChartReportElement)reportElement).RowsSeparator = value;
                }     
            }
        }

        [Category("��������� �����")]
        [DisplayName("��������� ����� ���������")]
        [Description("��������� � ����� ����� ����� ������������ ���������.")]
        [DynamicPropertyFilter("ElementType", "eChart")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        public bool IncludeInLabelParentMember
        {
            get
            {
                if (axis.AxisType == AxisType.atColumns)
                {
                    return ((ChartReportElement)reportElement).PivotData.ColumnAxis.IncludeInChartLabelParentMember;
                }
                else
                {
                    return ((ChartReportElement)reportElement).PivotData.RowAxis.IncludeInChartLabelParentMember;
                }
            }
            set
            {
                if (axis.AxisType == AxisType.atColumns)
                {
                    ((ChartReportElement)reportElement).PivotData.ColumnAxis.IncludeInChartLabelParentMember = value;
                }
                else
                {
                    ((ChartReportElement)reportElement).PivotData.RowAxis.IncludeInChartLabelParentMember = value;
                }
            }
        }

        /// <summary>
        /// �������� ������ ����������� ����� ����, ������� ���������� 
        /// � ������������ ����� ���������
        /// </summary>
        public class ChartLabelsElementDropDownEditor : UITypeEditor
        {
            /// <summary>
            /// ���������� ������ ��������������
            /// </summary>
            public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider, Object value)
            {
                if ((context != null) && (provider != null))
                {
                    IWindowsFormsEditorService svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                    if (svc != null)
                    {
                        AxisComponentListControl clctrl = new AxisComponentListControl((Axis)value);
                        clctrl.Tag = svc;
                        svc.DropDownControl(clctrl);
                        value = null; //��� ��� ����, ��� �� ������ ��������
                    }
                }

                return base.EditValue(context, provider, value); //result 
            }

            /// <summary>
            /// ���������� ����� ��������� - ���������� ����
            /// </summary>
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                if (context != null)
                {
                    return UITypeEditorEditStyle.DropDown;
                }
                else
                {
                    return base.GetEditStyle(context);
                }
            }
        }
     }   

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CaptionBrowseClass
    {
        private CustomReportElement reportElement;

        public CaptionBrowseClass(CustomReportElement element)
        {
            this.reportElement = element;
        }

        #region ��������
        //�����
        [Category("���������")]
        [Description("��������� ������")]
        [DisplayName("�����")]
        [Browsable(true)]
        public string CaptionText
        {
            get { return this.reportElement.Caption.Text; }
            set { this.reportElement.Caption.Text = value; }
        }

        //���������
        [Category("���������")]
        [Description("���������� ��������� ������")]
        [DisplayName("����������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool CaptionVisibility
        {
            get { return this.reportElement.Caption.Visible; }
            set { this.reportElement.Caption.Visible = value; }
        }


        //�����
        [Category("���������")]
        [Description("����� ��������� ������")]
        [DisplayName("�����")]
        [TypeConverter(typeof(FontTypeConverter))]
        [Browsable(true)]
        public Font CaptionFont
        {
            get { return this.reportElement.Caption.Font; }
            set { this.reportElement.Caption.Font = value; }
        }

        //�������������� ������������
        [Category("���������")]
        [Description("�������������� ������������ ������ � ��������� ������")]
        [DisplayName("�������������� ������������")]
        [TypeConverter(typeof(TextHAlignTypeConverter))]
        [Browsable(true)]
        public HAlign CaptionTextHAlign
        {
            get { return this.reportElement.Caption.TextHAlign; }
            set { this.reportElement.Caption.TextHAlign = value; }
        }

        //������������ ������������
        [Category("���������")]
        [Description("������������ ������������ ������ � ��������� ������")]
        [DisplayName("������������ ������������")]
        [TypeConverter(typeof(TextVAlignTypeConverter))]
        [Browsable(true)]
        public VAlign CaptionTextVAlign
        {
            get { return this.reportElement.Caption.TextVAlign; }
            set { this.reportElement.Caption.TextVAlign = value; }
        }

        //����� �������
        [Category("���������")]
        [Description("����� ������� ���������� ������")]
        [DisplayName("����� �������")]
        [TypeConverter(typeof(BorderStyleTypeConvertor))]
        [Browsable(true)]
        public UIElementBorderStyle BorderStyleType
        {
            get { return this.reportElement.Caption.BorderStyle; }
            set { this.reportElement.Caption.BorderStyle = value; }
        }

        //���� ����
        [Category("���������")]
        [Description("���� ���� ��������� ������")]
        [DisplayName("���� ����")]
        [Browsable(true)]
        public Color CaptionBackColor
        {
            get { return this.reportElement.Caption.BackColor; }
            set { this.reportElement.Caption.BackColor = value; }
        }

        //���� ����
        [Category("���������")]
        [Description("���� ������ ��������� ������")]
        [DisplayName("���� ������")]
        [Browsable(true)]
        public Color CaptionForeColor
        {
            get { return this.reportElement.Caption.Appearance.ForeColor; }
            set { this.reportElement.Caption.Appearance.ForeColor = value; }
        }

        #endregion

        public override string ToString()
        {
            return string.Empty;
        }
    }
    #endregion
}
