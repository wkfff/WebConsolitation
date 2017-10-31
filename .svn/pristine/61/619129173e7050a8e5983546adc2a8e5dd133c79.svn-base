using System.ComponentModel;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    class ChartPivotAxisBrowseAdapter : PivotAxisBrowseAdapter
    {

        public ChartPivotAxisBrowseAdapter(PivotAxis pivotAxis, CustomReportElement reportElement)
            : base(pivotAxis, reportElement)
        {
        }

        /// <summary>
        /// Отображать ли свойство "Итоги по видимым"
        /// </summary>
        [Browsable(false)]
        public bool IsShowGrandTotalVisibleProperties
        {
            get
            {
                return (this.CurrentAxisType == AxisType.atRows || this.CurrentAxisType == AxisType.atColumns)
                    && !this.IsCustomMDX;
            }
        }

        [Browsable(false)]
        public bool IsShowSortByName
        {
            get { return((this.IsRowsOrColumnAxisNotCustomMDX)&&(this.ReportElement is ChartReportElement));}
        }

        /// <summary>
        /// Существует три способа скрыть пустые данные
        /// </summary>
        [Category("Управление данными")]
        [DisplayName("Скрытие пустых")]
        [Description("Если у оси стоит признак \"Скрывать пустые элементы\", определяет способ их удаления")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DefaultValue(Data.HideEmptyMode.Automate)]
        [DynamicPropertyFilter("IsCustomMDX", "False")]
        public HideEmptyMode HideEmptyMode
        {
            get { return this.CurrentPivotObject.NonDeterminatedHideEmptyMode; }
            set { this.CurrentPivotObject.SetHideEmptyMode(value, true); }
        }

        [Category("Поведение")]
        [DisplayName("Показывать общий итог")]
        [Description("Показывать общий итог")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        [DynamicPropertyFilter("IsShowGrandTotalVisibleProperties", "True")]
        public bool GrandTotalVisible
        {
            get { return this.CurrentPivotObject.GrandTotalVisible; }
            set { this.CurrentPivotObject.GrandTotalVisible = value; }
        }

        [Category("Управление данными")]
        [DisplayName("Сортировать по имени")]
        [Description("Сортировать по имени")]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DynamicPropertyFilter("IsShowSortByName", "True")]
        public bool SortByName
        {
            get { return this.CurrentPivotObject.SortByName; }
            set
            {
                this.CurrentPivotObject.SortByName = value;
            }
        }

        [Category("Управление данными")]
        [DisplayName("Обратный порядок элементов")]
        [Description("Отображать элементы в обратном порядке")]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DynamicPropertyFilter("IsShowSortByName", "True")]
        public bool ReverseOrder
        {
            get { return this.CurrentPivotObject.ReverseOrder; }
            set
            {
                this.CurrentPivotObject.ReverseOrder = value;
            }
        }


    }
}
