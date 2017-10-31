using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Линии оси
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AxisLineStyleBrowseClass
    {
        #region Поля

        private AxisAppearance axisAppearance;

        #endregion

        #region Свойства

        /// <summary>
        /// Цвет линий оси координат
        /// </summary>
        [Category("Оси")]
        [Description("Цвет линии оси координат")]
        [DisplayName("Цвет")]
        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        public Color LineColor
        {
            get { return axisAppearance.LineColor; }
            set { axisAppearance.LineColor = value; }
        }

        /// <summary>
        /// Толщина линий оси координат
        /// </summary>
        [Category("Оси")]
        [Description("Толщина линии оси координат")]
        [DisplayName("Толщина")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(2)]
        [Browsable(true)]
        public int LineThickness
        {
            get { return axisAppearance.LineThickness; }
            set { axisAppearance.LineThickness = value; }
        }

        /// <summary>
        /// Стиль линий оси координат
        /// </summary>
        [Category("Оси")]
        [Description("Стиль линии оси координат")]
        [DisplayName("Стиль")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [DefaultValue(LineDrawStyle.Solid)]
        [Browsable(true)]
        public LineDrawStyle LineStyle
        {
            get { return axisAppearance.LineDrawStyle; }
            set { axisAppearance.LineDrawStyle = value; }
        }

        /// <summary>
        /// Стиль конца линии оси координат
        /// </summary>
        [Category("Оси")]
        [Description("Стиль конца линии оси координат")]
        [DisplayName("Стиль конца")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [DefaultValue(LineCapStyle.NoAnchor)]
        [Browsable(true)]
        public LineCapStyle LineEndCapStyle
        {
            get { return axisAppearance.LineEndCapStyle; }
            set { axisAppearance.LineEndCapStyle = value; }
        }

        #endregion

        public AxisLineStyleBrowseClass(AxisAppearance axisAppearance)
        {
            this.axisAppearance = axisAppearance;
        }

        public override string ToString()
        {
            return LineDrawStyleTypeConverter.ToString(LineStyle) + "; " + LineColor.Name + "; " +
                   LineThickness + "; " + LineCapStyleTypeConverter.ToString(LineEndCapStyle);
        }
    }
}
