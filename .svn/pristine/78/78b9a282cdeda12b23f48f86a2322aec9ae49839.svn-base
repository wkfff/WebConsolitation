using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ColumnChart3DBrowseClass
    {
        #region Поля

        private ColumnChart3DAppearance columnChart3DAppearance;

        #endregion

        #region Свойства

        /// <summary>
        /// Рассторяние между категориями
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Рассторяние между категориями")]
        [DisplayName("Рассторяние между категориями")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int ColumnSpacing
        {
            get { return columnChart3DAppearance.ColumnSpacing; }
            set { columnChart3DAppearance.ColumnSpacing = value; }
        }

        /// <summary>
        /// Отображение пустых значений
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Отображение пустых значений")]
        [DisplayName("Пустые значения")]
        [TypeConverter(typeof(NullHandlingTypeConverter))]
        [DefaultValue(NullHandling.Zero)]
        [Browsable(true)]
        public NullHandling NullHandling
        {
            get { return columnChart3DAppearance.NullHandling; }
            set { columnChart3DAppearance.NullHandling = value; }
        }

        #endregion

        public ColumnChart3DBrowseClass(ColumnChart3DAppearance columnChart3DAppearance)
        {
            this.columnChart3DAppearance = columnChart3DAppearance;
        }

        public override string ToString()
        {
            return ColumnSpacing.ToString();
        }
    }
}