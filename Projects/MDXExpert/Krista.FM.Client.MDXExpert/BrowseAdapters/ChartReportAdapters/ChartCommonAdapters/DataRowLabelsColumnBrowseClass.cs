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
    public class DataRowLabelsColumnBrowseClass
    {
        #region Поля

        private DataAppearance dataAppearance;

        #endregion

        #region Свойства
        
        /// <summary>
        /// Учет исключаемой категории
        /// </summary>
        [Category("Вид")]
        [Description("Учет исключаемой категории")]
        [DisplayName("Учет исключаемой категории")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool UseRowLabelsColumn
        {
            get { return dataAppearance.UseRowLabelsColumn; }
            set { dataAppearance.UseRowLabelsColumn = value; }
        }

        /// <summary>
        /// Индекс исключаемой категорий
        /// </summary>
        [Category("Вид")]
        [Description("Индекс исключаемой категорий")]
        [DisplayName("Индекс исключаемой категорий")]
        [DefaultValue(-1)]
        [Browsable(true)]
        public int RowLabelsColumn
        {
            get { return dataAppearance.RowLabelsColumn; }
            set { dataAppearance.RowLabelsColumn = value; }
        }

        #endregion

        public DataRowLabelsColumnBrowseClass(DataAppearance dataAppearance)
        {
            this.dataAppearance = dataAppearance;
        }

        public override string ToString()
        {
            return BooleanTypeConverter.ToString(UseRowLabelsColumn) + "; " + RowLabelsColumn;
        }
    }
}
