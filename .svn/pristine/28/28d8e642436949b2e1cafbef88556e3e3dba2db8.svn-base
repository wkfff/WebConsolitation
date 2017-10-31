using System.ComponentModel;
using System.Drawing.Design;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Grid;
using Krista.FM.Client.MDXExpert.Grid.UserInterface;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert

{
    /// <summary>
    /// Общие свойства мер
    /// </summary>
    public class TableTotalAxisBrowseAdapter : TotalAxisBrowseAdapter
    {
        private TotalCaptionsBrowseClass totalCaptionsBrowse;
        private DataAreaBrowseClass dataAreaBrowse;
        private ColorRuleCollection colorRules;

        public TableTotalAxisBrowseAdapter(TotalAxis totalAxis, CustomReportElement reportElement)
            : base(totalAxis, reportElement)
        {
            this.totalCaptionsBrowse = new TotalCaptionsBrowseClass(reportElement.GridUserInterface);
            this.dataAreaBrowse = new DataAreaBrowseClass(reportElement.GridUserInterface);
            this.colorRules = reportElement.GridUserInterface.ColorRules;
        }

        #region свойства

        //заголовки мер
        [Category("Вид")]
        [Description("Заголовков мер")]
        [DisplayName("Заголовков")]
        [Browsable(true)]
        public TotalCaptionsBrowseClass TotalCaptionsBrowse
        {
            get { return totalCaptionsBrowse; }
            set { totalCaptionsBrowse = value; }
        }

        //Ячейки мер
        [Category("Вид")]
        [Description("Ячеек мер")]
        [DisplayName("Ячеек")]
        [Browsable(true)]
        public DataAreaBrowseClass DataAreaBrowse
        {
            get { return dataAreaBrowse; }
            set { dataAreaBrowse = value; }
        }

        [Category("Вид")]
        [Description("Условная раскраска")]
        [DisplayName("Условная раскраска")]
        [Editor(typeof(ColorRuleCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public ColorRuleCollection ColorRules
        {
            get { return this.colorRules; }
            set { this.colorRules = value; }
        }

        #endregion

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class TotalCaptionsBrowseClass
        {
            private IGridUserInterface gridUserInterface;

            public TotalCaptionsBrowseClass(IGridUserInterface gridInterface)
            {
                this.gridUserInterface = gridInterface;
            }

            public override string ToString()
            {
                return TotalCaptionsFont.Name;
            }

            #region Свойства

            //шрифт
            [Category("Заголовков")]
            [DisplayName("Шрифт текста")]
            [Description("Шрифт текста у заголовков мер")]
            [TypeConverter(typeof(FontTypeConverter))]
            [Browsable(true)]
            public Font TotalCaptionsFont
            {
                get { return gridUserInterface.MeasureCaptionsFont; }
                set { gridUserInterface.MeasureCaptionsFont = value; }
            }

            //цвет фона
            [Category("Заголовков")]
            [DisplayName("Начальный цвет фона")]
            [Description("Начальный цвет фона у заголовков мер")]
            [Browsable(true)]
            public Color TotalCaptionsStartBackColor
            {
                get { return gridUserInterface.MeasureCaptionsStartBackColor; }
                set { gridUserInterface.MeasureCaptionsStartBackColor = value; }
            }

            //цвет фона
            [Category("Заголовков")]
            [DisplayName("Завершающий цвет фона")]
            [Description("Завершающий цвет фона у заголовков мер")]
            [Browsable(true)]
            public Color TotalCaptionsEndBackColor
            {
                get { return gridUserInterface.MeasureCaptionsEndBackColor; }
                set { gridUserInterface.MeasureCaptionsEndBackColor = value; }
            }

            //цвет текста
            [Category("Заголовков")]
            [DisplayName("Цвет текста")]
            [Description("Цвет текста у заголовков мер")]
            [Browsable(true)]
            public Color TotalCaptionForeColor
            {
                get { return gridUserInterface.MeasureCaptionsForeColor; }
                set { gridUserInterface.MeasureCaptionsForeColor = value; }
            }

            //цвет бордюра
            [Category("Заголовков")]
            [DisplayName("Цвет границы")]
            [Description("Цвет границы у заголовков мер")]
            [Browsable(true)]
            public Color TotalCaptionBorderColor
            {
                get { return gridUserInterface.MeasureCaptionsBorderColor; }
                set { gridUserInterface.MeasureCaptionsBorderColor = value; }
            }
            #endregion
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class DataAreaBrowseClass
        {
            private IGridUserInterface gridUserInterface;

            public DataAreaBrowseClass(IGridUserInterface gridInterface)
            {
                this.gridUserInterface = gridInterface;
            }

            public override string ToString()
            {
                return DataAreaFont.Name + "; " + DataAreaTotalsFont.Name;
            }

            #region Свойства

            //шрифт
            [Category("Ячеек")]
            [DisplayName("Шрифт текста")]
            [Description("Шрифт текста у ячеек мер")]
            [TypeConverter(typeof(FontTypeConverter))]
            [Browsable(true)]
            public Font DataAreaFont
            {
                get { return gridUserInterface.DataAreaFont; }
                set { gridUserInterface.DataAreaFont = value; }
            }

            //цвет фона
            [Category("Ячеек")]
            [DisplayName("Цвет фона")]
            [Description("Цвет фона у ячеек мер")]
            [Browsable(true)]
            public Color DataAreaBackColor
            {
                get { return gridUserInterface.DataAreaBackColor; }
                set { gridUserInterface.DataAreaBackColor = value; }
            }

            //цвет текста
            [Category("Ячеек")]
            [DisplayName("Цвет текста")]
            [Description("Цвет текста у ячеек мер")]
            [Browsable(true)]
            public Color DataAreaForeColor
            {
                get { return gridUserInterface.DataAreaForeColor; }
                set { gridUserInterface.DataAreaForeColor = value; }
            }

            //шрифт итогов
            [Category("Ячеек")]
            [DisplayName("Шрифт текста итогов")]
            [Description("Шрифт текста итогов у ячеек мер")]
            [TypeConverter(typeof(FontTypeConverter))]
            [Browsable(true)]
            public Font DataAreaTotalsFont
            {
                get { return gridUserInterface.DataAreaTotalsFont; }
                set { gridUserInterface.DataAreaTotalsFont = value; }
            }

            //цвет фона итогов
            [Category("Ячеек")]
            [DisplayName("Цвет фона итогов")]
            [Description("Цвет фона итогов у ячеек мер")]
            [Browsable(true)]
            public Color DataAreaTotalsBackColor
            {
                get { return gridUserInterface.DataAreaTotalsBackColor; }
                set { gridUserInterface.DataAreaTotalsBackColor = value; }
            }

            //цвет текста итогов
            [Category("Ячеек")]
            [DisplayName("Цвет текста итогов")]
            [Description("Цвет текста итогов у ячеек мер")]
            [Browsable(true)]
            public Color DataAreaTotalsForeColor
            {
                get { return gridUserInterface.DataAreaTotalsForeColor; }
                set { gridUserInterface.DataAreaTotalsForeColor = value; }
            }

            //цвет бордюра
            [Category("Ячеек")]
            [DisplayName("Цвет границы")]
            [Description("Цвет границы у ячеек мер")]
            [Browsable(true)]
            public Color DataAreaBorderColor
            {
                get { return gridUserInterface.DataAreaBorderColor; }
                set { gridUserInterface.DataAreaBorderColor = value; }
            }

            //цвет фона фиктивный ячеек
            [Category("Ячеек")]
            [DisplayName("Цвет фона фиктивных ячеек")]
            [Description("Цвет фона фиктивных ячеек у мер")]
            [Browsable(true)]
            public Color DataAreaDummyBackColor
            {
                get { return gridUserInterface.DataAreaDummyBackColor; }
                set { gridUserInterface.DataAreaDummyBackColor = value; }
            }
            #endregion
        }
    }
}
