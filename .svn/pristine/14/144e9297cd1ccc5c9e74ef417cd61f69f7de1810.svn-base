using System.ComponentModel;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;
using System.Windows.Forms;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Свойства измерения оси
    /// </summary>
    public class FieldSetBrowseAdapter : PivotObjectBrowseAdapterBase
    {
        public FieldSetBrowseAdapter(Data.FieldSet fieldSet, CustomReportElement reportElement)
            : base(fieldSet, fieldSet.Caption, reportElement)
        {
        }

        #region Свойства

        private Data.FieldSet CurrentPivotObject
        {
            get { return (Data.FieldSet)base.PivotObject; }
        }

        /// <summary>
        /// Отображать ли свойство IsTableVisibleTotals
        /// </summary>
        [Browsable(false)]
        public bool IsDisplayTableTotalProperty
        {
            get 
            { 
                return (this.ElementType == ReportElementType.eTable) && 
                    (this.CurrentPivotObject.AxisType != Data.AxisType.atFilters); 
            }
        }

        /// <summary>
        /// Отображать ли свойство IsChartVisibleTotals
        /// </summary>
        [Browsable(false)]
        public bool IsDisplayChartTotalProperty
        {
            get
            {
                return ((this.ElementType == ReportElementType.eChart)||(this.ElementType == ReportElementType.eMultiGauge)) &&
                    (this.CurrentPivotObject.AxisType != Data.AxisType.atFilters);
            }
        }

        /// <summary>
        /// Отображать ли свойство IsHideDataMembers
        /// </summary>
        [Browsable(false)]
        public bool IsDisplayDataMembersProperty
        {
            get
            {
                return (this.ElementType != ReportElementType.eMap) &&
                    (this.CurrentPivotObject.AxisType != Data.AxisType.atFilters);
            }
        }


        /// <summary>
        /// Отображать ли свойство IsChartVisibleTotals
        /// </summary>
        [Browsable(false)]
        public bool IsDisplayMemberTypeProperty
        {
            get
            {
                return (this.ElementType == ReportElementType.eTable) &&
                    (this.CurrentPivotObject.AxisType == Data.AxisType.atFilters);
            }
        }

        /// <summary>
        /// Отображать лм свойство сортировки, у измерение с мерами делать это не будем
        /// т.к. мер сортироваться сама по себе не умеет
        /// </summary>
        [Browsable(false)]
        public bool IsDisplaySortType
        {
            get
            {
                return (this.CurrentPivotObject.UniqueName != "[Measures]") && 
                    (this.CurrentPivotObject.AxisType != AxisType.atFilters) && !this.IsCustomMDX;
            }
        }

        [Category("Общие")]
        [DisplayName("Уникальное имя")]
        [Description("Уникальное имя измерения")]
        public string UniqueName
        {
            get { return this.CurrentPivotObject.UniqueName; }
        }

        [Category("Общие")]
        [DisplayName("Заголовок")]
        [Description("Заголовок измерения")]
        public string Caption
        {
            get { return this.CurrentPivotObject.Caption; }
            set 
            {
                base.Header = value;
                this.CurrentPivotObject.Caption = value;
            }
        }

        [Category("Управление данными")]
        [DisplayName("Сортировка")]
        [Description("Вид сортировки измерения")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DefaultValue(Data.SortType.None)]
        [DynamicPropertyFilter("IsDisplaySortType", "True")]
        public Data.SortType SortType
        {
            get { return this.CurrentPivotObject.SortType; }
            set { this.CurrentPivotObject.SortType = value; }
        }

        [Category("Управление данными")]
        [DisplayName("Показывать итоги")]
        [Description("Показывать итоги, принадлежащие элементам данного измерения")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DynamicPropertyFilter("IsDisplayTableTotalProperty", "True")]
        [DefaultValue(true)]
        public bool IsTableVisibleTotals
        {
            get { return this.CurrentPivotObject.IsVisibleTotals; }
            set { this.CurrentPivotObject.IsVisibleTotals = value; }
        }

        [Category("Управление данными")]
        [DisplayName("Скрывать элементы (ДАННЫЕ)")]
        [Description("Скрывать элементы (ДАННЫЕ)")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DynamicPropertyFilter("IsDisplayDataMembersProperty", "True")]
        [DefaultValue(true)]
        public bool IsHideDataMembers
        {
            get { return !this.CurrentPivotObject.IsVisibleDataMembers; }
            set { this.CurrentPivotObject.IsVisibleDataMembers = !value; }
        }

        [Category("Управление данными")]
        [DisplayName("Отображать элементы")]
        [Description("Определяет какие элементы будут выводиться в значении фильтра")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DynamicPropertyFilter("IsDisplayMemberTypeProperty", "True")]
        [DefaultValue(Data.DisplayMemberType.Auto)]
        public DisplayMemberType DisplayMemberType
        {
            get { return this.CurrentPivotObject.DisplayMemberType; }
            set { this.CurrentPivotObject.DisplayMemberType = value; }
        }

        [Category("Управление данными")]
        [DisplayName("Показывать нелистовые элементы")]
        [Description("Показывать нелистовые элементы")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DynamicPropertyFilter("IsDisplayChartTotalProperty", "True")]
        [DefaultValue(false)]
        public bool IsChartVisibleTotals
        {
            get { return this.CurrentPivotObject.IsVisibleTotals; }
            set { this.CurrentPivotObject.IsVisibleTotals = value; }
        }


        #endregion
    }
}
