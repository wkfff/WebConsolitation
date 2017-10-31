using System.ComponentModel;
using Infragistics.UltraChart.Core.Annotations;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AnnotationBrowseClass
    {
        #region ����

        private AnnotationsAppearance annotationsAppearance;

        #endregion

        #region ��������

        /// <summary>
        /// ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("���������� ���������")]
        [DisplayName("����������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool Visible
        {
            get { return annotationsAppearance.Visible; }
            set { annotationsAppearance.Visible = value; }
        }

        /// <summary>
        /// ���������
        /// </summary>
        [Category("������� ���������")]
        [Description("���������")]
        [DisplayName("���������")]
        [Editor(typeof(AnnotationCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        public AnnotationCollection Annotations
        {
            get { return annotationsAppearance.Annotations; }
        }

        #endregion

        public AnnotationBrowseClass(AnnotationsAppearance annotationsAppearance)
        {
            this.annotationsAppearance = annotationsAppearance;
        }

        public override string ToString()
        {
            return BooleanTypeConverter.ToString(Visible);
        }
    }
}