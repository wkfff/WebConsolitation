using System.ComponentModel;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert

{
    /// <summary>
    /// Свойства уровня оси
    /// </summary>
    public class PivotFieldBrowseAdapter : PivotObjectBrowseAdapterBase
    {

        public PivotFieldBrowseAdapter(PivotField pivotField, CustomReportElement reportElement)
            : base(pivotField, pivotField.Caption, reportElement)
        {
        }

        protected PivotField CurrentPivotObject
        {
            get { return (PivotField)base.PivotObject; }
        }

        /// <summary>
        /// Отображать ли свойство IsTableVisibleTotals
        /// </summary>
        [Browsable(false)]
        public bool IsDisplayTableTotalProperty
        {
            get
            {
                return ((this.ElementType == ReportElementType.eTable) &&
                    (this.CurrentPivotObject.ParentFieldSet.AxisType != AxisType.atFilters));
            }
        }

        /// <summary>
        /// Отображать ли свойство IsHideDataMember
        /// </summary>
        [Browsable(false)]
        public bool IsDisplayDataMemberProperty
        {
            get
            {
                return ((this.ElementType != ReportElementType.eMap) &&
                    (this.CurrentPivotObject.ParentFieldSet.AxisType != AxisType.atFilters));
            }
        }

        [Category("Общие")]
        [DisplayName("Заголовок")]
        [Description("Заголовок уровня")]
        public string Caption
        {
            get 
            {
                return this.CurrentPivotObject.Caption; 
            }
            set
            {
                base.Header = value;
                this.CurrentPivotObject.Caption = value;
            }
        }

        [Category("Общие")]
        [DisplayName("Уникальное имя")]
        [Description("Уникальное имя уровня")]
        public string UniqueName
        {
            get 
            {
                return this.CurrentPivotObject.UniqueName;
            }
        }

        [Category("Управление данными")]
        [DisplayName("Показывать итоги")]
        [Description("Показывать итоги, принадлежащие элементам этого уровня")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DynamicPropertyFilter("IsDisplayTableTotalProperty", "True")]
        [DefaultValue(true)]
        public bool IsVisibleTotal
        {
            get { return this.CurrentPivotObject.IsVisibleTotal; }
            set { this.CurrentPivotObject.IsVisibleTotal = value; }
        }

        [Category("Управление данными")]
        [DisplayName("Скрывать элементы (ДАННЫЕ)")]
        [Description("Скрывать элементы (ДАННЫЕ)")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DynamicPropertyFilter("IsDisplayDataMemberProperty", "True")]
        [DefaultValue(true)]
        public bool IsHideDataMember
        {
            get { return !this.CurrentPivotObject.IsVisibleDataMember; }
            set { this.CurrentPivotObject.IsVisibleDataMember = !value; }
        }
    }
}
