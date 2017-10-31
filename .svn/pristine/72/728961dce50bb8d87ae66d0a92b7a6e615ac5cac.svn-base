using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Поля координат
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AxisMarginsBrowseClass
    {
        #region Поля

        private AxisMargin axisMargin;

        #endregion

        #region Свойства

        /// <summary>
        /// Единица измерения поля
        /// </summary>
        [Category("Поля")]
        [Description("Единица измерения поля")]
        [DisplayName("Единица измерения")]
        [TypeConverter(typeof(MarginLocationTypeConvterter))]
        [DefaultValue(LocationType.Percentage)]
        [Browsable(true)]
        public LocationType MarginType
        {
            get { return axisMargin.MarginType; }
            set { axisMargin.MarginType = value; }
        }

        /// <summary>
        /// Значение поля
        /// </summary>
        [Category("Поля")]
        [Description("Значение поля")]
        [DisplayName("Значение")]
        [DefaultValue(typeof(double), "0")]
        [Browsable(true)]
        public double Value
        {
            get { return axisMargin.Value; }
            set { axisMargin.Value = value; }
        }

        #endregion

        public AxisMarginsBrowseClass(AxisMargin axisMargin)
        {
            this.axisMargin = axisMargin;
        }

        public override string ToString()
        {
            return Value + "; " + MarginLocationTypeConvterter.ToString(MarginType);
        }
    }
}