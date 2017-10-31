using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using Krista.FM.Expert.PivotData;
using Krista.FM.Client.MDXExpert.Grid.UserInterface;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Общие свойства осей
    /// </summary>
    public class PivotAxisBrowseAdapter : PivotObjectBrowseAdapterBase
    {

        public PivotAxisBrowseAdapter(PivotAxis pivotAxis, CustomReportElement reportElement)
            : base(pivotAxis, pivotAxis.Caption, reportElement)
        {
        }

        #region свойства

        protected PivotAxis CurrentPivotObject
        {
            get { return (PivotAxis)base.PivotObject; }
        }

        [Category("Общие")]
        [DisplayName("Заголовок")]
        [Description("Заголовок оси")]
        public string Caption
        {
            get
            {
                return this.CurrentPivotObject.Caption;
            }
        }

        [Category("Поведение")]
        [DisplayName("Скрывать пустые")]
        [Description("Скрывать элементы, для которых отсутствуют данные показателей")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        [DynamicPropertyFilter("CurrentAxisType", "atRows, atColumns")]
        public bool HideEmptyPositions
        {
            get
            {
                return this.CurrentPivotObject.HideEmptyPositions;
            }
            set
            {
                this.CurrentPivotObject.HideEmptyPositions = value;
            }
        }

        [Category("Поведение")]
        [DisplayName("Показывать общий итог")]
        [Description("Показывать общий итог")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        [DynamicPropertyFilter("CurrentAxisType", "atRows, atColumns")]
        public bool GrandTotalVisible
        {
            get
            {
                return this.CurrentPivotObject.GrandTotalVisible;
            }
            set
            {
                this.CurrentPivotObject.GrandTotalVisible = value;
            }
        }

        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(false)]
        public AxisType CurrentAxisType
        {
            get { return this.CurrentPivotObject.AxisType; }
        }

        [Category("Управление данными")]
        [DisplayName("Количество строк на странице")]
        [Description("Определяет максимальное количество строк, отображаемых на странице в таблице.")]
        [DefaultValue(TablePager.DefaultPageSize)]
        [DynamicPropertyFilter("IsTableAxisRow", "True")]
        public int MaxTablePageSize
        {
            get { return ((TableReportElement)this.ReportElement).TablePager.PageSize; }
            set { ((TableReportElement)this.ReportElement).TablePager.PageSize = value; }
        }

        /// <summary>
        /// Если элемент отчета таблица и ось строк вернет - true
        /// </summary>
        [Browsable(false)]
        public bool IsTableAxisRow
        {
            get
            {
                if ((this.ReportElement != null) &&
                    (this.ReportElement is TableReportElement))
                {
                    return this.CurrentPivotObject.AxisType == AxisType.atRows;
                }
                return false;
            }
        }

        /// <summary>
        /// Отображать ли свойство HideEmptyMode
        /// </summary>
        [Browsable(false)]
        public bool IsVisibleHideEmptyMode
        {
            get
            {
                return !this.IsVisibleLiteHideEmptyMode && (this.CurrentAxisType != AxisType.atFilters);
            }
        }

        /// <summary>
        /// Отображать ли свойство LiteHideEmptyMode
        /// </summary>
        [Browsable(false)]
        public bool IsVisibleLiteHideEmptyMode
        {
            get
            {
                if (this.IsTableAxisRow)
                {
                    return ((TableReportElement)this.ReportElement).PadingModeEnable;
                }
                return false;
            }
        }

        /// <summary>
        /// Существует три способа скрыть пустые данные, но для многостраничной таблице, для оси строк 
        /// доступно только два из них, в таком случае (мнг.стр.реж) вместо этого свойства будем использовать
        /// ниже лежащее MultipageHideEmptyMode.
        /// </summary>
        [Category("Управление данными")]
        [DisplayName("Скрытие пустых")]
        [Description("Если у оси стоит признак \"Скрывать пустые элементы\", определяет способ их удаления")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DefaultValue(HideEmptyMode.NonEmpty)]
        [DynamicPropertyFilter("IsVisibleHideEmptyMode", "True")]
        public HideEmptyMode HideEmptyMode
        {
            get { return this.CurrentPivotObject.HideEmptyMode; }
            set { this.CurrentPivotObject.SetHideEmptyMode(value, true); }
        }

        /// <summary>
        /// Данное свойство будет появляться только при многостраничном режиме у таблицы,
        /// и будет заменять HideEmptyMode. Т.к. не нашел другого способа убрать из 
        /// выпадающего списка лишние элементы, пришлось прибегнуть к такому.
        /// </summary>
        [Category("Управление данными")]
        [DisplayName("Скрытие пустых")]
        [Description("Если у оси стоит признак \"Скрывать пустые элементы\", определяет способ их удаления")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DefaultValue(MultipageHideEmptyMode.UsingFilter)]
        [DynamicPropertyFilter("IsVisibleLiteHideEmptyMode", "True")]
        public MultipageHideEmptyMode HideEmptyLiteMode
        {
            get
            {
                switch (this.CurrentPivotObject.HideEmptyMode)
                {
                    case HideEmptyMode.NonEmpty: return MultipageHideEmptyMode.UsingFilter;
                    case HideEmptyMode.UsingFilter: return MultipageHideEmptyMode.UsingFilter;
                    case HideEmptyMode.NonEmptyCrossJoin: return MultipageHideEmptyMode.NonEmptyCrossJoin;
                }
                return MultipageHideEmptyMode.UsingFilter;
            }
            set
            {
                switch (value)
                {
                    case MultipageHideEmptyMode.UsingFilter:
                        this.CurrentPivotObject.SetHideEmptyMode(HideEmptyMode.UsingFilter, true);
                        break;
                    case MultipageHideEmptyMode.NonEmptyCrossJoin:
                        this.CurrentPivotObject.SetHideEmptyMode(HideEmptyMode.NonEmptyCrossJoin, true);
                        break;
                }
            }
        }
        #endregion свойства

        /// <summary>
        /// Режим скрытия пустых, для многостарничных таблиц
        /// </summary>
        public enum MultipageHideEmptyMode
        {
            [Description("На уровне множества (Filter)")]
            UsingFilter,
            [Description("На уровне таблицы фактов (NonEmptyCrossJoin)")]
            NonEmptyCrossJoin
        }
    }
}