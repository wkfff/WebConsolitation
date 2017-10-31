using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.Win.UltraWinChart;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Стиль границы линии полос
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StripLineBorderBrowseClass
    {
        #region Поля

        private StripLineAppearance stripLineAppearance;

        #endregion 

        #region Свойства

        /// <summary>
        /// Цвет границы
        /// </summary>
        [Category("Элемент отображения")]
        [Description("Цвет границы")]
        [DisplayName("Цвет")]
        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        public Color Stroke
        {
            get { return stripLineAppearance.PE.Stroke; }
            set { stripLineAppearance.PE.Stroke = value; }
        }

        /// <summary>
        /// Прозрачность границы
        /// </summary>
        [Category("Элемент отображения")]
        [Description("Прозрачность границы")]
        [DisplayName("Прозрачность")]
        [Editor(typeof(OpacityEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(typeof(byte), "255")]
        [Browsable(true)]
        public byte StrokeOpacity
        {
            get { return stripLineAppearance.PE.StrokeOpacity; }
            set { stripLineAppearance.PE.StrokeOpacity = value; }
        }

        /// <summary>
        /// Ширина границы
        /// </summary>
        [Category("Элемент отображения")]
        [Description("Ширина границы")]
        [DisplayName("Ширина")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(1)]
        [Browsable(true)]
        public int StrokeWidth
        {
            get { return stripLineAppearance.PE.StrokeWidth; }
            set { stripLineAppearance.PE.StrokeWidth = value; }
        }

        #endregion

        public StripLineBorderBrowseClass(StripLineAppearance stripLineAppearance)
        {
            this.stripLineAppearance = stripLineAppearance;
        }

        public override string ToString()
        {
            return Stroke.Name + "; " + StrokeWidth + "; " + StrokeOpacity;
        }
    }
}
