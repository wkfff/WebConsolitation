using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// �������
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BorderBrowseClass
    {
        #region ����

        private BorderAppearance borderAppearance;

        #endregion

        #region ��������

        /// <summary>
        /// ���� �������
        /// </summary>
        [Category("�������")]
        [Description("���� �������")]
        [DisplayName("����")]
        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        public Color Color
        {
            get { return borderAppearance.Color; }
            set { borderAppearance.Color = value; }
        }

        /// <summary>
        /// ������� �������
        /// </summary>
        [Category("�������")]
        [Description("������� �������")]
        [DisplayName("�������")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(0)]
        [Browsable(true)]
        public int Thickness
        {
            get { return borderAppearance.Thickness; }
            set { borderAppearance.Thickness = value; }
        }

        /// <summary>
        /// ����� �������
        /// </summary>
        [Category("�������")]
        [Description("����� �������")]
        [DisplayName("�����")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [DefaultValue(LineDrawStyle.Solid)]
        [Browsable(true)]
        public LineDrawStyle Style
        {
            get { return borderAppearance.DrawStyle; }
            set { borderAppearance.DrawStyle = value; }
        }

        /// <summary>
        /// ������ ����� �������
        /// </summary>
        [Category("�������")]
        [Description("������ ����� �������")]
        [DisplayName("������ �����")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int CornerRadius
        {
            get { return borderAppearance.CornerRadius; }
            set { borderAppearance.CornerRadius = value; }
        }

        /// <summary>
        /// ���������� �������
        /// </summary>
        [Category("�������")]
        [Description("���������� �������")]
        [DisplayName("����������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool RaisedBrowse
        {
            get { return borderAppearance.Raised; }
            set { borderAppearance.Raised = value; }
        }

        #endregion

        public BorderBrowseClass(BorderAppearance borderAppearance)
        {
            this.borderAppearance = borderAppearance;
        }

        public override string ToString()
        {
            return Color.Name + "; " + LineDrawStyleTypeConverter.ToString(Style) + "; " + Thickness;
        }
    }
}