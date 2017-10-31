using System.ComponentModel;
using Infragistics.UltraChart.Core.Annotations;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AnnotationBrowseClass
    {
        #region Поля

        private AnnotationsAppearance annotationsAppearance;

        #endregion

        #region Свойства

        /// <summary>
        /// Видимость
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Показывать аннотации")]
        [DisplayName("Показывать")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool Visible
        {
            get { return annotationsAppearance.Visible; }
            set { annotationsAppearance.Visible = value; }
        }

        /// <summary>
        /// Аннотация
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Аннотация")]
        [DisplayName("Аннотация")]
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