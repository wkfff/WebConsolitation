using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Поля оси
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AxisGroupMarginsBrowseClass
    {
        #region Поля 

        private AxisMarginsBrowseClass axisNearMarginsBrowse;
        private AxisMarginsBrowseClass axisFarMarginsBrowse;

        #endregion

        #region Свойства

        /// <summary>
        /// Ближнее поле оси
        /// </summary>
        [Category("Оси")]
        [Description("Ближнее поле оси")]
        [DisplayName("Ближнее поле")]
        [Browsable(true)]
        public AxisMarginsBrowseClass AxisNearMarginsBrowse
        {
            get { return axisNearMarginsBrowse; }
            set { axisNearMarginsBrowse = value; }
        }

        /// <summary>
        /// Дальнее поле оси
        /// </summary>
        [Category("Оси")]
        [Description("Дальнее поле оси")]
        [DisplayName("Дальнее поле")]
        [Browsable(true)]
        public AxisMarginsBrowseClass AxisFarMarginsBrowse
        {
            get { return axisFarMarginsBrowse; }
            set { axisFarMarginsBrowse = value; }
        }

        #endregion

        public AxisGroupMarginsBrowseClass(AxisAppearance axisAppearance)
        {
            axisNearMarginsBrowse = new AxisMarginsBrowseClass(axisAppearance.Margin.Near);
            axisFarMarginsBrowse = new AxisMarginsBrowseClass(axisAppearance.Margin.Far);
        }

        public override string ToString()
        {
            return axisNearMarginsBrowse.Value + "; " + axisFarMarginsBrowse.Value;
        }
    }
}
