using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Krista.FM.Expert.PivotData;
using Krista.FM.Client.MDXExpert.Grid.UserInterface;
using System.Drawing;


namespace Krista.FM.Client.MDXExpert
{
    class TablePivotAxisBrowseAdapter : PivotAxisBrowseAdapter
    {
        private RowAxisBrowseClass rowAxisBrowse;
        private RowCaptionsBrowseClass rowCaptionBrowse;
        private ColumnAxisBrowseClass columnAxisBrowse;
        private ColumnCaptionsBrowseClass columnCaptionsBrowse;
        /*private FilterCaptionsBrowseClass filterCaptionsBrowse;*/

        public TablePivotAxisBrowseAdapter(PivotAxis pivotAxis, CustomReportElement reportElement)
            : base(pivotAxis, reportElement)
        {
            rowAxisBrowse = new RowAxisBrowseClass(reportElement.GridUserInterface);
            rowCaptionBrowse = new RowCaptionsBrowseClass(reportElement.GridUserInterface);
            columnAxisBrowse = new ColumnAxisBrowseClass(reportElement.GridUserInterface);
            columnCaptionsBrowse = new ColumnCaptionsBrowseClass(reportElement.GridUserInterface);
            /*filterCaptionsBrowse = new FilterCaptionsBrowseClass(gridInterface);*/
        }

        #region свойства

        [Category("Поведение")]
        [DisplayName("Показывать свойства элемента")]
        [Description("Выбор типа показа для свойств элементов")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(true)]
        [DynamicPropertyFilter("CurrentAxisType", "atRows, atColumns")]
        public MemberPropertiesDisplayType DisplayType
        {
            get
            {
                return this.CurrentPivotObject.PropertiesDisplayType;
            }
            set
            {
                this.CurrentPivotObject.PropertiesDisplayType = value;
            }
        }


        [Category("Поведение")]
        [DisplayName("Автоматический расчет высоты у ячеек")]
        [Description("Автоматический расчет высоты у ячеек строк")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        [DynamicPropertyFilter("CurrentAxisType", "atRows")]
        public bool AutoSizeRows
        {
            get { return base.GridInterface.AutoSizeRows; }
            set { base.GridInterface.AutoSizeRows = value; }
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

        
        /*//заголовки фильтров
        [Category("Вид")]
        [Description("Заголовков")]
        [DisplayName("Вид заголовков фильтров")]
        [Browsable(true)]
        [DynamicPropertyFilter("CurrentAxisType", "atFilters")]
        public FilterCaptionsBrowseClass FilterCaptionsBrowse
        {
            get { return filterCaptionsBrowse; }
            set { filterCaptionsBrowse = value; }
        }*/


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
 
    }
}
