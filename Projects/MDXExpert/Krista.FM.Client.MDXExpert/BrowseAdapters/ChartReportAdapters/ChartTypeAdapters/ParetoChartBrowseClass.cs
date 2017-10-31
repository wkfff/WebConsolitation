using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ParetoChartBrowseClass
    {
        #region Поля

        private ParetoChartAppearance paretoChartAppearance;
        private ParetoLineBrowseClass paretoLineBrowse;
        private PaintElementBrowseClass paintElementBrowse;

        #endregion

        #region Свойства

        /// <summary>
        /// Расстояние между колонками диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Расстояние между колонками диаграммы")]
        [DisplayName("Расстояние между колонками")]
        [DefaultValue(typeof(double), "0.5")]
        [Browsable(true)]
        public double ColumnSpacing
        {
            get { return paretoChartAppearance.ColumnSpacing; }
            set { paretoChartAppearance.ColumnSpacing = value; }
        }

        /// <summary>
        /// Метка линии в легенде
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Метка линии в легенде диаграммы")]
        [DisplayName("Метка линии в легенде")]
        [DefaultValue("Running Total")]
        [Browsable(true)]
        public string LineLabel
        {
            get { return paretoChartAppearance.LineLabel; }
            set { paretoChartAppearance.LineLabel = value; }
        }

        /// <summary>
        /// Видимость линий в легенде диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Показывать линии в легенде диаграммы")]
        [DisplayName("Показывать линии в легенде")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool ShowLineInLegend
        {
            get { return paretoChartAppearance.ShowLineInLegend; }
            set { paretoChartAppearance.ShowLineInLegend = value; }
        }

        /// <summary>
        /// Стиль линий в легенде диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Стиль линий диаграммы")]
        [DisplayName("Стиль линий")]
        [Browsable(true)]
        public ParetoLineBrowseClass ParetoLineBrowse
        {
            get { return paretoLineBrowse; }
            set { paretoLineBrowse = value; }
        }

        /// <summary>
        /// Стиль элемента отображения диаграммы
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Стиль элемента отображения диаграммы")]
        [DisplayName("Стиль элемента отображения")]
        [Browsable(true)]
        public PaintElementBrowseClass PaintElementBrowse
        {
            get { return paintElementBrowse; }
            set { paintElementBrowse = value; }
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
            get { return paretoChartAppearance.NullHandling; }
            set { paretoChartAppearance.NullHandling = value; }
        }

        #endregion

        public ParetoChartBrowseClass(ParetoChartAppearance paretoChartAppearance)
        {
            this.paretoChartAppearance = paretoChartAppearance;
            paretoLineBrowse = new ParetoLineBrowseClass(paretoChartAppearance.LineStyle);
            paintElementBrowse = new PaintElementBrowseClass(paretoChartAppearance.LinePE);
        }

        public override string ToString()
        {
            return ColumnSpacing + "; " + LineLabel;
        }
    }
}