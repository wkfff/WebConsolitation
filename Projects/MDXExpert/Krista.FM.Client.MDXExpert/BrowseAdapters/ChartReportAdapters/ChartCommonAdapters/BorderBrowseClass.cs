using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Граница
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BorderBrowseClass
    {
        #region Поля

        private BorderAppearance borderAppearance;

        #endregion

        #region Свойства

        /// <summary>
        /// Цвет границы
        /// </summary>
        [Category("Граница")]
        [Description("Цвет границы")]
        [DisplayName("Цвет")]
        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        public Color Color
        {
            get { return borderAppearance.Color; }
            set { borderAppearance.Color = value; }
        }

        /// <summary>
        /// Толщина границы
        /// </summary>
        [Category("Граница")]
        [Description("Толщина границы")]
        [DisplayName("Толщина")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(0)]
        [Browsable(true)]
        public int Thickness
        {
            get { return borderAppearance.Thickness; }
            set { borderAppearance.Thickness = value; }
        }

        /// <summary>
        /// Стиль границы
        /// </summary>
        [Category("Граница")]
        [Description("Стиль границы")]
        [DisplayName("Стиль")]
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
        /// Радиус углов границы
        /// </summary>
        [Category("Граница")]
        [Description("Радиус углов границы")]
        [DisplayName("Радиус углов")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int CornerRadius
        {
            get { return borderAppearance.CornerRadius; }
            set { borderAppearance.CornerRadius = value; }
        }

        /// <summary>
        /// Выпуклость границы
        /// </summary>
        [Category("Граница")]
        [Description("Выпуклость границы")]
        [DisplayName("Выпуклость")]
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