using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;
using Krista.FM.Client.MDXExpert.Controls;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Стиль границы линии полос для 3D диаграмм
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StripLineArea3DBrowseClass
    {
        #region Поля

        private UltraChart chart;
        private StripLineAppearance stripLineAppearance;

        #endregion

        #region Свойства

        /// <summary>
        /// Цвет заливки
        /// </summary>
        [Category("Элемент отображения")]
        [Description("Цвет заливки")]
        [DisplayName("Цвет заливки")]
        [DefaultValue(typeof(Color), "Transparent")]
        [Browsable(true)]
        public Color Fill
        {
            get { return stripLineAppearance.PE.Fill; }
            set
            {
                stripLineAppearance.PE.Fill = value;
                chart.InvalidateLayers();
            }
        }

        /// <summary>
        /// Прозрачность заливки
        /// </summary>
        [Category("Элемент отображения")]
        [Description("Прозрачность заливки")]
        [DisplayName("Прозрачность заливки")]
        [Editor(typeof(OpacityEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(typeof(byte), "255")]
        [Browsable(true)]
        public byte FillOpacity
        {
            get { return stripLineAppearance.PE.FillOpacity; }
            set
            {
                stripLineAppearance.PE.FillOpacity = value;
                chart.InvalidateLayers();
            }
        }

        #endregion

        public StripLineArea3DBrowseClass(StripLineAppearance stripLineAppearance, UltraChart chart)
        {
            this.chart = chart;

            this.stripLineAppearance = stripLineAppearance;
        }

        public override string ToString()
        {
            return Fill.Name + "; " + FillOpacity;
        }
    }
}
