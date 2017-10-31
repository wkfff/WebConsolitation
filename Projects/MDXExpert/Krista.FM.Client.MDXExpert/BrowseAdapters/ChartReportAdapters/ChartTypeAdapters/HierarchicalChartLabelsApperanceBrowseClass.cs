using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HierarchicalChartLabelsApperanceBrowseClass
    {
        #region Поля

        private HierarchicalChartLabelsAppearance chartLabelApperance;
        private LabelStyleBrowseClass labelStyleBrowse;
        private LeaderLabelStyleBrowseClass leaderLabelStyleBrowse;
        private ChartFormatBrowseClass chartLabelFormat;

        #endregion

        #region Свойства

        /// <summary>
        /// Формат подписи
        /// </summary>
        [Category("Подписи")]
        [Description("Строка формата подписи")]
        [DisplayName("Формат (строка)")]
        [Browsable(true)]
        public string FormatString
        {
            get 
            { 
                return chartLabelApperance.FormatString; 
            }
            set 
            {
                ChartLabelFormat.FormatString = value;
                chartLabelApperance.FormatString = value;
            }
        }

        [Category("Подписи")]
        [DisplayName("Формат (шаблон)")]
        [Description("Шаблон формата метки")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(true)]
        public AxisLabelFormatPattern AxisLabelPattern
        {
            get
            {
                return this.chartLabelFormat.AxisLabelPattern;
            }
            set
            {
                this.chartLabelFormat.AxisLabelPattern = value;
            }
        }


        [Category("Подписи")]
        [Description("Формат подписи")]
        [DisplayName("Формат")]
        [Browsable(true)]
        public ChartFormatBrowseClass ChartLabelFormat
        {
            get
            {
                return this.chartLabelFormat;

            }
            set
            {
                this.chartLabelFormat = value;
            }
        }


        /// <summary>
        /// Видимость подписи
        /// </summary>
        [Category("Подписи")]
        [Description("Показывать подписи")]
        [DisplayName("Показывать")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Visible
        {
            get { return chartLabelApperance.Visible; }
            set { chartLabelApperance.Visible = value; }
        }

        /// <summary>
        /// Положение
        /// </summary>
        [Category("Подписи")]
        [Description("Положение подписи")]
        [DisplayName("Положение")]
        [TypeConverter(typeof(FunnelLabelLocationTypeConverter))]
        [Browsable(true)]
        public FunnelLabelLocation Location
        {
            get { return chartLabelApperance.Location; }
            set { chartLabelApperance.Location = value; }
        }

        /// <summary>
        /// Стиль подписи
        /// </summary>
        [Category("Подписи")]
        [Description("Стиль подписи")]
        [DisplayName("Стиль подписи")]
        [Browsable(true)]
        public LabelStyleBrowseClass LabelStyleBrowse
        {
            get { return labelStyleBrowse; }
            set { labelStyleBrowse = value; }
        }

        /// <summary>
        /// Стиль линии подписи
        /// </summary>
        [Category("Подписи")]
        [Description("Стиль линии подписи")]
        [DisplayName("Стиль линии")]
        [Browsable(true)]
        public LeaderLabelStyleBrowseClass LeaderLabelStyleBrowse
        {
            get { return leaderLabelStyleBrowse; }
            set { leaderLabelStyleBrowse = value; }
        }

        #endregion

        public HierarchicalChartLabelsApperanceBrowseClass(HierarchicalChartLabelsAppearance chartLabelApperance)
        {
            this.chartLabelApperance = chartLabelApperance;
            labelStyleBrowse = new LabelStyleBrowseClass(chartLabelApperance.LabelStyle);
            leaderLabelStyleBrowse = new LeaderLabelStyleBrowseClass(chartLabelApperance);

            this.chartLabelFormat = new ChartFormatBrowseClass(chartLabelApperance.FormatString, 
                                                                ChartFormatBrowseClass.LabelType.AxisLabel,
                                                                chartLabelApperance.ChartComponent);

            this.chartLabelFormat.FormatChanged += new ValueFormatEventHandler(chartLabelFormat_FormatChanged);
        }

        private void chartLabelFormat_FormatChanged()
        {
           /* if (chartLabelFormat.FormatType == FormatType.Auto)
            {
                //chartLabelApperance.FormatString = "<ITEM_LABEL>";
                PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(HierarchicalChartLabelsAppearance));
                pdc["FormatString"].ResetValue(chartLabelApperance);

            }
            else*/
            {
                chartLabelApperance.FormatString = chartLabelFormat.FormatString;
            }
        }

        public override string ToString()
        {
            return FormatString + "; " + FunnelLabelLocationTypeConverter.ToString(Location);
        }
    }
}