using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class GanttChartBrowseClass
    {
        #region Поля

        private GanttChartAppearance ganttChartAppearance;
        private PaintElementBrowseClass completePercentagesPE;
        private PaintElementBrowseClass emptyPercentagesPE;
        private LabelStyleBrowseClass labelStyleBrowse;

        #endregion

        #region Свойства

        /// <summary>
        /// Расстояние между сериями
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Расстояние между сериями")]
        [DisplayName("Расстояние между сериями")]
        [DefaultValue(1)]
        [Browsable(true)]
        public int SeriesSpacing
        {
            get { return ganttChartAppearance.SeriesSpacing; }
            set { ganttChartAppearance.SeriesSpacing = value; }
        }

        /// <summary>
        /// Расстояние между элементами
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Расстояние между элементами")]
        [DisplayName("Расстояние между элементами")]
        [DefaultValue(1)]
        [Browsable(true)]
        public int ItemSpacing
        {
            get { return ganttChartAppearance.ItemSpacing; }
            set { ganttChartAppearance.ItemSpacing = value; }
        }

        /// <summary>
        /// Показывать полное процентное соотношение
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Показывать полное процентное отношение")]
        [DisplayName("Показывать полное процентное отношение")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool ShowCompletePercentages
        {
            get { return ganttChartAppearance.ShowCompletePercentages; }
            set { ganttChartAppearance.ShowCompletePercentages = value; }
        }

        /// <summary>
        /// Показывать связи
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Показывать связи")]
        [DisplayName("Показывать связи")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool ShowLinks
        {
            get { return ganttChartAppearance.ShowLinks; }
            set { ganttChartAppearance.ShowLinks = value; }
        }

        /// <summary>
        /// Показывать владельцев
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Показывать владельцев")]
        [DisplayName("Показывать владельцев")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool ShowOwners
        {
            get { return ganttChartAppearance.ShowOwners; }
            set { ganttChartAppearance.ShowOwners = value; }
        }

        /// <summary>
        /// Толщина линии связи
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Толщина линии связи")]
        [DisplayName("Толщина линии связи")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(1)]
        [Browsable(true)]
        public int LinkLineWidth
        {
            get { return ganttChartAppearance.LinkLineWidth; }
            set { ganttChartAppearance.LinkLineWidth = value; }
        }

        /// <summary>
        /// Стиль линии связи
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Стиль линии связи")]
        [DisplayName("Стиль линии связи")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [DefaultValue(LineDrawStyle.Solid)]
        [Browsable(true)]
        public LineDrawStyle DrawStyle
        {
            get { return ganttChartAppearance.LinkLineStyle.DrawStyle; }
            set { ganttChartAppearance.LinkLineStyle.DrawStyle = value; }
        }

        /// <summary>
        /// Начало линии связи
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Начало линии связи")]
        [DisplayName("Начало линии связи")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [DefaultValue(LineCapStyle.NoAnchor)]
        [Browsable(true)]
        public LineCapStyle StartStyle
        {
            get { return ganttChartAppearance.LinkLineStyle.StartStyle; }
            set { ganttChartAppearance.LinkLineStyle.StartStyle = value; }
        }

        /// <summary>
        /// Конец линии связи
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Конец линии связи")]
        [DisplayName("Конец линии связи")]
        [Editor(typeof(LineCapStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineCapStyleTypeConverter))]
        [DefaultValue(LineCapStyle.ArrowAnchor)]
        [Browsable(true)]
        public LineCapStyle EndStyle
        {
            get { return ganttChartAppearance.LinkLineStyle.EndStyle; }
            set { ganttChartAppearance.LinkLineStyle.EndStyle = value; }
        }

        /// <summary>
        /// Отображение промежуточных меток на линии связи
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Отображение промежуточных меток на линии связи")]
        [DisplayName("Отображение промежуточных меток")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool MidPointAnchors
        {
            get { return ganttChartAppearance.LinkLineStyle.MidPointAnchors; }
            set { ganttChartAppearance.LinkLineStyle.MidPointAnchors = value; }
        }

        /// <summary>
        /// Отображиние полного процентного отношения
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Отображиние полного процентного отношения")]
        [DisplayName("Отображиние полного процентного отношения")]
        [Browsable(true)]
        public PaintElementBrowseClass CompletePercentagesPE
        {
            get { return completePercentagesPE; }
            set { completePercentagesPE = value; }
        }

        /// <summary>
        /// Отображиние пустого процентного отношения
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Отображиние пустого процентного отношения")]
        [DisplayName("Отображиние пустого процентного отношения")]
        [Browsable(true)]
        public PaintElementBrowseClass EmptyPercentagesPE
        {
            get { return emptyPercentagesPE; }
            set { emptyPercentagesPE = value; }
        }

        /// <summary>
        /// Стиль метки владельца
        /// </summary>
        [Category("Область диаграммы")]
        [Description("Стиль метки владельца")]
        [DisplayName("Стиль метки владельца")]
        [Browsable(true)]
        public LabelStyleBrowseClass LabelStyleBrowse
        {
            get { return labelStyleBrowse; }
            set { labelStyleBrowse = value; }
        }

        #endregion

        public GanttChartBrowseClass(GanttChartAppearance ganttChartAppearance)
        {
            this.ganttChartAppearance = ganttChartAppearance;

            completePercentagesPE = new PaintElementBrowseClass(ganttChartAppearance.CompletePercentagesPE);
            emptyPercentagesPE = new PaintElementBrowseClass(ganttChartAppearance.EmptyPercentagesPE);
            labelStyleBrowse = new LabelStyleBrowseClass(ganttChartAppearance.OwnersLabelStyle);
        }

        public override string ToString()
        {
            return SeriesSpacing + "; " + ItemSpacing;
        }
    }
}