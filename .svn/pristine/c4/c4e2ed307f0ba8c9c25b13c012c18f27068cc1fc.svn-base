using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Ограничения по минимальному и максимальному значениям
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DataMinMaxBrowseClass
    {
        #region Поля

        private DataAppearance dataAppearance;

        #endregion

        #region Свойства

        /// <summary>
        /// Максимальное значение
        /// </summary>
        [Category("Вид")]
        [Description("Максимальное значение. Учитывается, когда включено свойство \"Ограничения\"")]
        [DisplayName("Максимальное значение")]
        [DefaultValue(1.7976931348623157E+308)]
        [Browsable(true)]
        public double MaxValue
        {
            get { return dataAppearance.MaxValue; }
            set { dataAppearance.MaxValue = value; }
        }

        /// <summary>
        /// Минимальное значение
        /// </summary>
        [Category("Вид")]
        [Description("Минимальное значение. Учитывается, когда включено свойство \"Ограничения\"")]
        [DisplayName("Минимальное значение")]
        [DefaultValue(-1.7976931348623157E+308)]
        [Browsable(true)]
        public double MinValue
        {
            get { return dataAppearance.MinValue; }
            set { dataAppearance.MinValue = value; }
        }

        /// <summary>
        /// Учет ограничений
        /// </summary>
        [Category("Вид")]
        [Description("Ограничения по минимальному и максимальному значениям")]
        [DisplayName("Учет ограничений")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool UseMinMax
        {
            get { return dataAppearance.UseMinMax; }
            set { dataAppearance.UseMinMax = value; }
        }

        #endregion

        public DataMinMaxBrowseClass(DataAppearance dataAppearance)
        {
            this.dataAppearance = dataAppearance;
        }

        public override string ToString()
        {
            return BooleanTypeConverter.ToString(UseMinMax) + "; " + MinValue + "; " + MaxValue;
        }
    }
}
