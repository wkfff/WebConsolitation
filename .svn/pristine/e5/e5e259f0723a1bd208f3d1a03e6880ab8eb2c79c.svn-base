using System.ComponentModel;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Grid.UserInterface;
using System.Drawing;
using DisplayMemberCount=Krista.FM.Client.MDXExpert.Grid.DisplayMemberCount;


namespace Krista.FM.Client.MDXExpert
{
    class TablePivotAxisBrowseAdapter : PivotAxisBrowseAdapter
    {
        private RowAxisBrowseClass rowAxisBrowse;
        private RowCaptionsBrowseClass rowCaptionBrowse;
        private ColumnAxisBrowseClass columnAxisBrowse;
        private ColumnCaptionsBrowseClass columnCaptionsBrowse;
        private FilterCaptionsBrowseClass filterCaptionsBrowse;
        private FilterValuesBrowseClass filterValuesBrowse;

        public TablePivotAxisBrowseAdapter(Data.PivotAxis pivotAxis, CustomReportElement reportElement)
            : base(pivotAxis, reportElement)
        {
            rowAxisBrowse = new RowAxisBrowseClass(reportElement.GridUserInterface);
            rowCaptionBrowse = new RowCaptionsBrowseClass(reportElement.GridUserInterface);
            columnAxisBrowse = new ColumnAxisBrowseClass(reportElement.GridUserInterface);
            columnCaptionsBrowse = new ColumnCaptionsBrowseClass(reportElement.GridUserInterface);
            filterCaptionsBrowse = new FilterCaptionsBrowseClass(reportElement.GridUserInterface);
            filterValuesBrowse = new FilterValuesBrowseClass(reportElement.GridUserInterface);
        }

        #region свойства

        [Category("Поведение")]
        [DisplayName("Показывать общий итог")]
        [Description("Показывать общий итог")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        [DynamicPropertyFilter("IsRowsOrColumnAxisNotCustomMDX", "True")]
        public bool GrandTotalVisible
        {
            get { return this.CurrentPivotObject.GrandTotalVisible; }
            set { this.CurrentPivotObject.GrandTotalVisible = value; }
        }

        [Category("Поведение")]
        [DisplayName("Показывать свойства элемента")]
        [Description("Выбор типа показа для свойств элементов")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(true)]
        [DynamicPropertyFilter("IsRowsOrColumnAxisNotCustomMDX", "True")]
        public Data.MemberPropertiesDisplayType DisplayType
        {
            get { return this.CurrentPivotObject.PropertiesDisplayType; }
            set { this.CurrentPivotObject.PropertiesDisplayType = value; }
        }

        [Category("Поведение")]
        [DisplayName("Автоматический расчет высоты у ячеек")]
        [Description("Автоматический расчет высоты у ячеек строк")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        [DynamicPropertyFilter("IsTableRowAxisNotCustomMDX", "True")]
        public bool AutoSizeRows
        {
            get { return base.GridInterface.AutoSizeRows; }
            set { base.GridInterface.AutoSizeRows = value; }
        }

        /// <summary>
        /// Отображать ли свойство HideEmptyMode
        /// </summary>
        [Browsable(false)]
        public bool IsVisibleHideEmptyMode
        {
            get
            {
                return !this.IsVisibleLiteHideEmptyMode && (this.CurrentAxisType != Data.AxisType.atFilters)
                    && !this.IsCustomMDX;
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
                if (this.IsTableRowAxisNotCustomMDX)
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
        [DefaultValue(Data.HideEmptyMode.Automate)]
        [DynamicPropertyFilter("IsVisibleHideEmptyMode", "True")]
        public HideEmptyMode HideEmptyMode
        {
            get { return this.CurrentPivotObject.NonDeterminatedHideEmptyMode; }
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
        [DefaultValue(MultipageHideEmptyMode.Automate)]
        [DynamicPropertyFilter("IsVisibleLiteHideEmptyMode", "True")]
        public MultipageHideEmptyMode HideEmptyLiteMode
        {
            get
            {
                switch (this.CurrentPivotObject.NonDeterminatedHideEmptyMode)
                {
                    case HideEmptyMode.Automate: return MultipageHideEmptyMode.Automate;
                    case HideEmptyMode.NonEmpty: return PivotData.AnalysisServicesVersion == AnalysisServicesVersion.v2000 ?
                        MultipageHideEmptyMode.UsingFilter : MultipageHideEmptyMode.NonEmpty2005;
                    case HideEmptyMode.UsingFilter: return MultipageHideEmptyMode.UsingFilter;
                    case HideEmptyMode.NonEmptyCrossJoin: return MultipageHideEmptyMode.NonEmptyCrossJoin;
                    case HideEmptyMode.NonEmpty2005: return MultipageHideEmptyMode.NonEmpty2005;
                }
                return MultipageHideEmptyMode.UsingFilter;
            }
            set
            {
                switch (value)
                {
                    case MultipageHideEmptyMode.Automate:
                        this.CurrentPivotObject.SetHideEmptyMode(HideEmptyMode.Automate, true);
                        break;
                    case MultipageHideEmptyMode.UsingFilter:
                        this.CurrentPivotObject.SetHideEmptyMode(HideEmptyMode.UsingFilter, true);
                        break;
                    case MultipageHideEmptyMode.NonEmptyCrossJoin:
                        this.CurrentPivotObject.SetHideEmptyMode(HideEmptyMode.NonEmptyCrossJoin, true);
                        break;
                    case MultipageHideEmptyMode.NonEmpty2005:
                        this.CurrentPivotObject.SetHideEmptyMode(HideEmptyMode.NonEmpty2005, true);
                        break;
                }
            }
        }

        //заголовки фильтров
        [Category("Вид")]
        [DisplayName("Заголовков")]
        [Description("Вид заголовков фильтров")]
        [Browsable(true)]
        [DynamicPropertyFilter("CurrentAxisType", "atFilters")]
        public FilterCaptionsBrowseClass FilterCaptionsBrowse
        {
            get { return filterCaptionsBrowse; }
            set { filterCaptionsBrowse = value; }
        }

        //значения фильтров
        [Category("Вид")]
        [DisplayName("Значений")]
        [Description("Вид значений фильтров")]
        [Browsable(true)]
        [DynamicPropertyFilter("CurrentAxisType", "atFilters")]
        public FilterValuesBrowseClass FilterValuesBrowse
        {
            get { return filterValuesBrowse; }
            set { filterValuesBrowse = value; }
        }

        //заголовки фильтров
        [Category("Вид")]
        [DisplayName("Показывать")]
        [Description("Показывать заголовки фильтров")]
        [Browsable(true)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [DynamicPropertyFilter("CurrentAxisType", "atFilters")]
        public bool FilterCaptionsVisible
        {
            get { return this.GridInterface.FilterCaptionsVisible; }
            set { this.GridInterface.FilterCaptionsVisible = value; }
        }

        //заголовки фильтров
        [Category("Вид")]
        [DisplayName("Кол-во элементов в комментарии")]
        [Description("Максимально количество элементов, отображаемых в комментарии к фильтру")]
        [Browsable(true)]
        [DefaultValue(10)]
        [DynamicPropertyFilter("CurrentAxisType", "atFilters")]
        public int TipDisplayMaxMemberCount
        {
            get { return this.GridInterface.TipDisplayMaxMemberCount; }
            set { this.GridInterface.TipDisplayMaxMemberCount = value; }
        }

        //заголовки фильтров
        [Category("Вид")]
        [DisplayName("Кол-во элементов в значении")]
        [Description("Максимально количество элементов, отображаемых в значении фильтра")]
        [Browsable(true)]
        [DefaultValue(Grid.DisplayMemberCount.One)]
        [DynamicPropertyFilter("CurrentAxisType", "atFilters")]
        [TypeConverter(typeof(EnumTypeConverter))]
        public DisplayMemberCount DisplayMemberCount
        {
            get { return this.GridInterface.DisplayMemberCount; }
            set { this.GridInterface.DisplayMemberCount = value; }
        }

        [Category("Вид")]
        [DisplayName("Отображать родительские элементы")]
        [Description("Отображать родительские элементы")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DynamicPropertyFilter("CurrentAxisType", "atFilters")]
        [DefaultValue(false)]
        public bool IsCaptionIncludeParents
        {
            get { return this.GridInterface.IsCaptionIncludeParents; }
            set { this.GridInterface.IsCaptionIncludeParents = value; }
        }


        //заголовки строк
        [Category("Вид")]
        [DisplayName("Заголовков")]
        [Description("Вид заголовков строк")]
        [Browsable(true)]
        [DynamicPropertyFilter("CurrentAxisType", "atRows")]
        public RowCaptionsBrowseClass RowCaptionsBrowse
        {
            get { return rowCaptionBrowse; }
            set { rowCaptionBrowse = value; }
        }

        //ось строк
        [Category("Вид")]
        [DisplayName("Ячеек")]
        [Description("Вид ячеек строк")]
        [Browsable(true)]
        [DynamicPropertyFilter("CurrentAxisType", "atRows")]
        public RowAxisBrowseClass RowAxisBrowse
        {
            get { return rowAxisBrowse; }
            set { rowAxisBrowse = value; }
        }

        //заголовки столбцов
        [Category("Вид")]
        [DisplayName("Заголовков")]
        [Description("Вид заголовов столбцов")]
        [Browsable(true)]
        [DynamicPropertyFilter("CurrentAxisType", "atColumns")]
        public ColumnCaptionsBrowseClass ColumnCaptionsBrowse
        {
            get { return columnCaptionsBrowse; }
            set { columnCaptionsBrowse = value; }
        }

        //ось столбцов
        [Category("Вид")]
        [DisplayName("Ячеек")]
        [Description("Вид ячеек столбцов")]
        [Browsable(true)]
        [DynamicPropertyFilter("CurrentAxisType", "atColumns")]
        public ColumnAxisBrowseClass ColumnAxisBrowse
        {
            get { return columnAxisBrowse; }
            set { columnAxisBrowse = value; }
        }
        #endregion

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class RowCaptionsBrowseClass
        {
            private IGridUserInterface gridUserInterface;

            #region Свойства

            //шрифт
            [Category("Заголовков")]
            [DisplayName("Шрифт текста")]
            [Description("Шрифт текста у заголовков строк")]
            [TypeConverter(typeof(FontTypeConverter))]
            [Browsable(true)]
            public Font RowCaptionsFont
            {
                get { return gridUserInterface.RowCaptionsFont; }
                set { gridUserInterface.RowCaptionsFont = value; }
            }

            //цвет фона
            [Category("Заголовков")]
            [DisplayName("Начальный цвет фона")]
            [Description("Начальный цвет фона у заголовков строк")]
            [Browsable(true)]
            public Color RowCaptionsStartBackColor
            {
                get { return gridUserInterface.RowCaptionsStartBackColor; }
                set { gridUserInterface.RowCaptionsStartBackColor = value; }
            }

            //цвет фона
            [Category("Заголовков")]
            [DisplayName("Завершающий цвет фона")]
            [Description("Завершающий цвет фона у заголовков строк")]
            [Browsable(true)]
            public Color RowCaptionsEndBackColor
            {
                get { return gridUserInterface.RowCaptionsEndBackColor; }
                set { gridUserInterface.RowCaptionsEndBackColor = value; }
            }

            //цвет текста
            [Category("Заголовков")]
            [DisplayName("Цвет текста")]
            [Description("Цвет текста у заголовков строк")]
            [Browsable(true)]
            public Color RowCaptionForeColor
            {
                get { return gridUserInterface.RowCaptionsForeColor; }
                set { gridUserInterface.RowCaptionsForeColor = value; }
            }

            //цвет бордюра
            [Category("Заголовков")]
            [DisplayName("Цвет границы")]
            [Description("Цвет границы у заголовков строк")]
            [Browsable(true)]
            public Color RowCaptionBorderColor
            {
                get { return gridUserInterface.RowCaptionsBorderColor; }
                set { gridUserInterface.RowCaptionsBorderColor = value; }
            }
            #endregion

            public RowCaptionsBrowseClass(IGridUserInterface gridUserInterface)
            {
                this.gridUserInterface = gridUserInterface;
            }

            public override string ToString()
            {
                return RowCaptionsFont.Name;
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class RowAxisBrowseClass
        {
            private IGridUserInterface gridUserInterface;

            #region Свойства

            //шрифт
            [Category("Ячеек")]
            [DisplayName("Шрифт текста")]
            [Description("Шрифт текста у ячеек строк")]
            [TypeConverter(typeof(FontTypeConverter))]
            [Browsable(true)]
            public Font RowAxisFont
            {
                get { return gridUserInterface.RowAxisFont; }
                set { gridUserInterface.RowAxisFont = value; }
            }

            //шрифт имени свойств элемента
            [Category("Ячеек")]
            [DisplayName("Шрифт имен у свойств")]
            [Description("Шрифт имен у свойств, отображающихся в ячейках измерения")]
            [TypeConverter(typeof(FontTypeConverter))]
            [Browsable(true)]
            public Font RowMemberPropertiesNameFont
            {
                get { return gridUserInterface.RowMemberPropertiesNameFont; }
                set { gridUserInterface.RowMemberPropertiesNameFont = value; }
            }

            //шрифт значениясвойств элемента
            [Category("Ячеек")]
            [DisplayName("Шрифт значений у свойств")]
            [Description("Шрифт значений у свойств, отображающихся в ячейках измерения")]
            [TypeConverter(typeof(FontTypeConverter))]
            [Browsable(true)]
            public Font RowMemberPropertiesValueFont
            {
                get { return gridUserInterface.RowMemberPropertiesValueFont; }
                set { gridUserInterface.RowMemberPropertiesValueFont = value; }
            }

            //цвет имени свойства
            [Category("Ячеек")]
            [DisplayName("Цвет имен у свойств")]
            [Description("Цвет имен у свойств, отображающихся в ячейках измерения")]
            [Browsable(true)]
            public Color RowMemberPropertiesNameForeColor
            {
                get { return gridUserInterface.RowMemberPropertiesNameForeColor; }
                set { gridUserInterface.RowMemberPropertiesNameForeColor = value; }
            }

            //цвет значения свойства
            [Category("Ячеек")]
            [DisplayName("Цвет значений у свойств")]
            [Description("Цвет значения у свойств, отображающихся в ячейка измерения")]
            [Browsable(true)]
            public Color RowMemberPropertiesValueForeColor
            {
                get { return gridUserInterface.RowMemberPropertiesValueForeColor; }
                set { gridUserInterface.RowMemberPropertiesValueForeColor = value; }
            }

            //цвет фона
            [Category("Ячеек")]
            [DisplayName("Начальный цвет фона")]
            [Description("Начальный цвет фона у ячеек строк")]
            [Browsable(true)]
            public Color RowAxisStartBackColor
            {
                get { return gridUserInterface.RowAxisStartBackColor; }
                set { gridUserInterface.RowAxisStartBackColor = value; }
            }

            //цвет фона
            [Category("Ячеек")]
            [DisplayName("Завершающий цвет фона")]
            [Description("Завершающий цвет фона у ячеек строк")]
            [Browsable(true)]
            public Color RowAxisEndBackColor
            {
                get { return gridUserInterface.RowAxisEndBackColor; }
                set { gridUserInterface.RowAxisEndBackColor = value; }
            }

            //цвет текста
            [Category("Ячеек")]
            [DisplayName("Цвет текста")]
            [Description("Цвет текста у ячеек строк")]
            [Browsable(true)]
            public Color RowAxisForeColor
            {
                get { return gridUserInterface.RowAxisForeColor; }
                set { gridUserInterface.RowAxisForeColor = value; }
            }

            //цвет бордюра
            [Category("Ячеек")]
            [DisplayName("Цвет границы")]
            [Description("Цвет границы у ячеек строк")]
            [Browsable(true)]
            public Color RowAxisBorderColor
            {
                get { return gridUserInterface.RowAxisBorderColor; }
                set { gridUserInterface.RowAxisBorderColor = value; }
            }
            #endregion

            public RowAxisBrowseClass(IGridUserInterface gridUserInterface)
            {
                this.gridUserInterface = gridUserInterface;
            }

            public override string ToString()
            {
                return RowAxisFont.Name;
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class ColumnCaptionsBrowseClass
        {
            private IGridUserInterface gridUserInterface;

            #region Свойства

            //шрифт
            [Category("Заголовков")]
            [DisplayName("Шрифт текста")]
            [Description("Шрифт текста у заголовков столбцов")]
            [TypeConverter(typeof(FontTypeConverter))]
            [Browsable(true)]
            public Font ColumnCaptionsFont
            {
                get { return gridUserInterface.ColumnCaptionsFont; }
                set { gridUserInterface.ColumnCaptionsFont = value; }
            }

            //цвет фона
            [Category("Заголовков")]
            [DisplayName("Начальный цвет фона")]
            [Description("Начальный цвет фона у заголовков столбцов")]
            [Browsable(true)]
            public Color ColumnCaptionsStartBackColor
            {
                get { return gridUserInterface.ColumnCaptionsStartBackColor; }
                set { gridUserInterface.ColumnCaptionsStartBackColor = value; }
            }

            //цвет фона
            [Category("Заголовков")]
            [DisplayName("Завершающий цвет фона")]
            [Description("Завершающий цвет фона у заголовков столбцов")]
            [Browsable(true)]
            public Color ColumnCaptionsEndBackColor
            {
                get { return gridUserInterface.ColumnCaptionsEndBackColor; }
                set { gridUserInterface.ColumnCaptionsEndBackColor = value; }
            }

            //цвет текста
            [Category("Заголовков")]
            [DisplayName("Цвет текста")]
            [Description("Цвет текста у заголовков столбцов")]
            [Browsable(true)]
            public Color ColumnCaptionForeColor
            {
                get { return gridUserInterface.ColumnCaptionsForeColor; }
                set { gridUserInterface.ColumnCaptionsForeColor = value; }
            }

            //цвет бордюра
            [Category("Заголовков")]
            [DisplayName("Цвет границы")]
            [Description("Цвет границы у заголовков столбцов")]
            [Browsable(true)]
            public Color ColumnCaptionBorderColor
            {
                get { return gridUserInterface.ColumnCaptionsBorderColor; }
                set { gridUserInterface.ColumnCaptionsBorderColor = value; }
            }
            #endregion

            public ColumnCaptionsBrowseClass(IGridUserInterface gridUserInterface)
            {
                this.gridUserInterface = gridUserInterface;
            }

            public override string ToString()
            {
                return ColumnCaptionsFont.Name;
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class ColumnAxisBrowseClass
        {
            private IGridUserInterface gridUserInterface;

            #region Свойства

            //шрифт
            [Category("Ячеек")]
            [DisplayName("Шрифт текста")]
            [Description("Шрифт текста у ячеек столбцов")]
            [TypeConverter(typeof(FontTypeConverter))]
            [Browsable(true)]
            public Font ColumnAxisFont
            {
                get { return gridUserInterface.ColumnAxisFont; }
                set { gridUserInterface.ColumnAxisFont = value; }
            }

            //шрифт имени свойств элемента
            [Category("Ячеек")]
            [DisplayName("Шрифт имен у свойств")]
            [Description("Шрифт имен у свойств, отображающихся в ячейках измерения")]
            [TypeConverter(typeof(FontTypeConverter))]
            [Browsable(true)]
            public Font ColumnMemberPropertiesNameFont
            {
                get { return gridUserInterface.ColumnMemberPropertiesNameFont; }
                set { gridUserInterface.ColumnMemberPropertiesNameFont = value; }
            }

            //шрифт значениясвойств элемента
            [Category("Ячеек")]
            [DisplayName("Шрифт значений у свойств")]
            [Description("Шрифт значений у свойств, отображающихся в ячейках измерения")]
            [TypeConverter(typeof(FontTypeConverter))]
            [Browsable(true)]
            public Font ColumnMemberPropertiesValueFont
            {
                get { return gridUserInterface.ColumnMemberPropertiesValueFont; }
                set { gridUserInterface.ColumnMemberPropertiesValueFont = value; }
            }

            //цвет имени свойства
            [Category("Ячеек")]
            [DisplayName("Цвет имен у свойств")]
            [Description("Цвет имен у свойств, отображающихся в ячейках измерения")]
            [Browsable(true)]
            public Color ColumnMemberPropertiesNameForeColor
            {
                get { return gridUserInterface.ColumnMemberPropertiesNameForeColor; }
                set { gridUserInterface.ColumnMemberPropertiesNameForeColor = value; }
            }

            //цвет значения свойства
            [Category("Ячеек")]
            [DisplayName("Цвет значений у свойств")]
            [Description("Цвет значений у свойств, отображающихся в ячейках измерения")]
            [Browsable(true)]
            public Color ColumnMemberPropertiesValueForeColor
            {
                get { return gridUserInterface.ColumnMemberPropertiesValueForeColor; }
                set { gridUserInterface.ColumnMemberPropertiesValueForeColor = value; }
            }

            //цвет фона
            [Category("Ячеек")]
            [DisplayName("Начальный цвет фона")]
            [Description("Начальный цвет фона у ячеек столбцов")]
            [Browsable(true)]
            public Color ColumnAxisStartBackColor
            {
                get { return gridUserInterface.ColumnAxisStartBackColor; }
                set { gridUserInterface.ColumnAxisStartBackColor = value; }
            }

            //цвет фона
            [Category("Ячеек")]
            [DisplayName("Завершающий цвет фона")]
            [Description("Завершающий цвет фона у ячеек столбцов")]
            [Browsable(true)]
            public Color ColumnAxisEndBackColor
            {
                get { return gridUserInterface.ColumnAxisEndBackColor; }
                set { gridUserInterface.ColumnAxisEndBackColor = value; }
            }

            //цвет текста
            [Category("Ячеек")]
            [DisplayName("Цвет текста")]
            [Description("Цвет текста у ячеек столбцов")]
            [Browsable(true)]
            public Color ColumnAxisForeColor
            {
                get { return gridUserInterface.ColumnAxisForeColor; }
                set { gridUserInterface.ColumnAxisForeColor = value; }
            }

            //цвет бордюра
            [Category("Ячеек")]
            [DisplayName("Цвет границы")]
            [Description("Цвет границы у ячеек столбцов")]
            [Browsable(true)]
            public Color ColumnAxisBorderColor
            {
                get { return gridUserInterface.ColumnAxisBorderColor; }
                set { gridUserInterface.ColumnAxisBorderColor = value; }
            }
            #endregion

            public ColumnAxisBrowseClass(IGridUserInterface gridUserInterface)
            {
                this.gridUserInterface = gridUserInterface;
            }

            public override string ToString()
            {
                return ColumnAxisFont.Name;
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class FilterCaptionsBrowseClass
        {
            private IGridUserInterface gridUserInterface;

            #region Свойства

            //шрифт
            [Category("Заголовков")]
            [DisplayName("Шрифт текста")]
            [Description("Шрифт текста у заголовков фильтров")]
            [TypeConverter(typeof(FontTypeConverter))]
            [Browsable(true)]
            public Font FilterCaptionsFont
            {
                get { return gridUserInterface.FilterCaptionsFont; }
                set { gridUserInterface.FilterCaptionsFont = value; }
            }

            //цвет фона
            [Category("Заголовков")]
            [DisplayName("Начальный цвет фона")]
            [Description("Начальный цвет фона у заголовков фильтров")]
            [Browsable(true)]
            public Color FilterCaptionsStartBackColor
            {
                get { return gridUserInterface.FilterCaptionsStartBackColor; }
                set { gridUserInterface.FilterCaptionsStartBackColor = value; }
            }

            //цвет фона
            [Category("Заголовков")]
            [DisplayName("Завершающий цвет фона")]
            [Description("Завершающий цвет фона у заголовков фильтров")]
            [Browsable(true)]
            public Color FilterCaptionsEndBackColor
            {
                get { return gridUserInterface.FilterCaptionsEndBackColor; }
                set { gridUserInterface.FilterCaptionsEndBackColor = value; }
            }

            //цвет текста
            [Category("Заголовков")]
            [DisplayName("Цвет текста")]
            [Description("Цвет текста у заголовков фильтров")]
            [Browsable(true)]
            public Color FilterCaptionForeColor
            {
                get { return gridUserInterface.FilterCaptionsForeColor; }
                set { gridUserInterface.FilterCaptionsForeColor = value; }
            }

            //цвет бордюра
            [Category("Заголовков")]
            [DisplayName("Цвет границы")]
            [Description("Цвет границы у заголовков фильтров")]
            [Browsable(true)]
            public Color FilterCaptionBorderColor
            {
                get { return gridUserInterface.FilterCaptionsBorderColor; }
                set { gridUserInterface.FilterCaptionsBorderColor = value; }
            }
            #endregion

            public FilterCaptionsBrowseClass(IGridUserInterface gridUserInterface)
            {
                this.gridUserInterface = gridUserInterface;
            }

            public override string ToString()
            {
                return FilterCaptionsFont.Name;
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class FilterValuesBrowseClass
        {
            private IGridUserInterface gridUserInterface;

            #region Свойства

            //шрифт
            [Category("Значений")]
            [DisplayName("Шрифт текста")]
            [Description("Шрифт текста у значений фильтров")]
            [TypeConverter(typeof(FontTypeConverter))]
            [Browsable(true)]
            public Font FilterValuesFont
            {
                get { return gridUserInterface.FilterValuesFont; }
                set { gridUserInterface.FilterValuesFont = value; }
            }

            //цвет фона
            [Category("Значений")]
            [DisplayName("Начальный цвет фона")]
            [Description("Начальный цвет фона у значений фильтров")]
            [Browsable(true)]
            public Color FilterValuesStartBackColor
            {
                get { return gridUserInterface.FilterValuesStartBackColor; }
                set { gridUserInterface.FilterValuesStartBackColor = value; }
            }

            //цвет фона
            [Category("Значений")]
            [DisplayName("Завершающий цвет фона")]
            [Description("Завершающий цвет фона у значений фильтров")]
            [Browsable(true)]
            public Color FilterValuesEndBackColor
            {
                get { return gridUserInterface.FilterValuesEndBackColor; }
                set { gridUserInterface.FilterValuesEndBackColor = value; }
            }

            //цвет текста
            [Category("Значений")]
            [DisplayName("Цвет текста")]
            [Description("Цвет текста у значений фильтров")]
            [Browsable(true)]
            public Color FilterCaptionForeColor
            {
                get { return gridUserInterface.FilterValuesForeColor; }
                set { gridUserInterface.FilterValuesForeColor = value; }
            }

            //цвет бордюра
            [Category("Значений")]
            [DisplayName("Цвет границы")]
            [Description("Цвет границы у значений фильтров")]
            [Browsable(true)]
            public Color FilterCaptionBorderColor
            {
                get { return gridUserInterface.FilterValuesBorderColor; }
                set { gridUserInterface.FilterValuesBorderColor = value; }
            }
            #endregion

            public FilterValuesBrowseClass(IGridUserInterface gridUserInterface)
            {
                this.gridUserInterface = gridUserInterface;
            }

            public override string ToString()
            {
                return FilterValuesFont.Name;
            }
        }
    }

    /// <summary>
    /// Режим скрытия пустых, для многостарничных таблиц
    /// </summary>
    public enum MultipageHideEmptyMode
    {
        [Description("Автоматическое")]
        Automate,
        [Description("На уровне множества (Filter)")]
        UsingFilter,
        [Description("На уровне таблицы фактов (NonEmptyCrossJoin)")]
        NonEmptyCrossJoin,
        [Description("На уровне множества (MASS2005)")]
        NonEmpty2005
    }

}
