using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Drawing;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

/**
 *  Розничные цены на продовольственные товары первой необходимости по данным на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.ORG_0003_0001
{
    public partial class _default : CustomReportPage
    {
        // --------------------------------------------------------------------

        // заголовок страницы
        private static String page_title_caption = "Розничные цены на продовольственные товары первой необходимости по данным на {0} <nobr>({1})</nobr>";
        // заголовок для UltraChart
        private static String chart_title_caption = "Динамика розничной цены на товар \"{0}\"";
        // заголовки столбцов для UltraWebGrid
        private static String[] grid_columns = { "№ п/п", "Наименование продукта", "Ед. изм.", "Цена, рублей", "unique_name" };
        // таблица преобразования имен товаров
        private static Hashtable grid_names = foodstuffNames();
        // таблица склонения имен месяцев
        private static Hashtable month_names = monthNames();
        // имя столбца заголовков для UltraChart (нигде не отображается)
        private static String chart_table_name = "series_name";
        // сообщения об ошибке при некорректной загрузке данных для UltraChart
        private static String chart_error_message = "данных не найдено";
        // ширина экрана в пикселях
        private int screen_width { get { return (int)Session["width_size"]; } }
        // высота экрана в пикселях
        private int screen_height { get { return (int)Session["height_size"]; } }
        // параметр для выбранной.текущей территории
        private CustomParam current_region { get { return (UserParams.CustomParam("current_region")); } }

        // --------------------------------------------------------------------

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                base.Page_PreLoad(sender, e);
                // установка размера диаграммы
                chart.Width = screen_width - 50;
            }
            catch (Exception)
            {
                // установка размеров не удалась ...
            }
        }

        // --------------------------------------------------------------------

        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                base.Page_Load(sender, e);
                if (!Page.IsPostBack)
                {   // опрерации которые должны выполняться при только первой загрузке страницы

                    // Установка настроек для DimensionTree
                    //dim_tree.HierarchyName = "[Территории].[РФ]";
                    //dim_tree.DefaultMember = "[Территории].[РФ].[Все территории]";
                    //dim_tree.InitTree();                    
                }
                // установка параметра территории
                current_region.Value = user_combo.SelectedItem.Value;
                // установка заголовка для страницы
                page_title.Text = String.Format(
                    page_title_caption, mdxTime2String(getLastDate()),
                    UserComboBox.getLastBlock(current_region.Value));
                // заполнение UltraWebGrid данными
                web_grid.DataBind();
                // установка активной строки в UltraWebGrid
                webGridActiveRowChange(0);
                web_grid.Rows[0].Activate();
            }
            catch (Exception)
            {
                // неудачная загрузка ...
            }
        }

        // --------------------------------------------------------------------

        protected void web_grid_DataBinding(object sender, EventArgs e)
        {
            DataTable grid_table = new DataTable();
            // Установка параметра последней актуальной даты
            UserParams.CustomParam("period_last_date").Value = getLastDate();

            try
            {
                CellSet grid_set = null;
                // Загрузка таблицы цен и товаров в CellSet
                grid_set = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid"));
                // Добавление столбцов в таблицу и заполнение данных в DataTable
                for (int i = 0; i < grid_columns.Length; ++i)
                {   // установка заголовков для столбцов UltraWebGrid
                    grid_table.Columns.Add(grid_columns[i]);
                }
                foreach (Position pos in grid_set.Axes[1].Positions)
                {   // создание списка значений для строки UltraWebGrid
                    object[] values = {
                        pos.Ordinal + 1,
                        grid_names[grid_set.Axes[1].Positions[pos.Ordinal].Members[0].Caption],
                        pos.Members[0].MemberProperties[0].Value.ToString(),
                        grid_set.Cells[0, pos.Ordinal].Value,
                        pos.Members[0].ToString()
                    };
                    // заполнение строки данными
                    grid_table.Rows.Add(values);
                }
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
            // установка источника данных для UltraWebGrid
            web_grid.DataSource = grid_table.DefaultView;
        }

        // --------------------------------------------------------------------

        protected void web_grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            // настройка столбцов
            web_grid.Bands[0].Columns[0].Width = 40;
            web_grid.Bands[0].Columns[1].CellStyle.Wrap = true;
            web_grid.Bands[0].Columns[1].Width = 370; // TODO: переделать: надо бы сделать вычислиямые размеры
            web_grid.Bands[0].Columns[4].Hidden = true;
            // установка формата отображения ячеек
            CRHelper.FormatNumberColumn(web_grid.Bands[0].Columns[0], "N2");
            CRHelper.FormatNumberColumn(web_grid.Bands[0].Columns[3], "N2");
        }

        // --------------------------------------------------------------------

        protected void chart_DataBinding(object sender, EventArgs e)
        {
            DataTable chart_table = new DataTable();
            try
            {                
                // загрузка таблицы цен за период "3 месяца хотя можно и другой установить :)" на текущий товар
                CellSet chart_set = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("chart"));
                // заполнение таблицы DataTable данными из CellSet
                cellSet2DataTable(chart_set, chart_table, true);
                // корректировка заголовков столбцов в DataTable
                foreach (DataColumn col in chart_table.Columns)
                {
                    if (col.Ordinal > 0)
                    {
                        col.ColumnName = mdxTime2String(col.ColumnName);
                    }
                }
            }
            catch (Exception exception)
            {
                chart_table = null;
            }
            // установка источника данных для UltraChart
            chart.DataSource = (chart_table == null) ? null : chart_table.DefaultView;
            try
            {
                // определение максимального и минимального значений по оси ординат для UltraChart
                ArrayList list = new ArrayList(chart_table.Rows[0].ItemArray);
                list.RemoveAt(0);
                list.Sort();
                double min = double.Parse(list[0].ToString());
                double max = double.Parse(list[list.Count - 1].ToString());
                // настройка параметров оси ординат
                chart.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
                chart.Axis.Y.RangeMin = min - (max - min) / 5;
                chart.Axis.Y.RangeMax = max;
            }
            catch (Exception)
            { 
                // настройка параметров оси прошла не удачно ... бывает ... ничего страшного
            }
        }

        // --------------------------------------------------------------------

        protected void web_grid_ActiveRowChange(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {          
            webGridActiveRowChange(e.Row.Index);
        }

        // --------------------------------------------------------------------

        protected void chart_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {            
            setChartErrorFont(e);
        }

        // --------------------------------------------------------------------

        protected void web_grid_DblClick(object sender, Infragistics.WebUI.UltraWebGrid.ClickEventArgs e)
        {
        }

        // --------------------------------------------------------------------

        /** <summary>
         *  Устанавливает текущую строку в таблице 
         *  </summary>
         */
        private void webGridActiveRowChange(int index)
        {   // установка заголовка для UltraChart
            chart_title.Text = String.Format(chart_title_caption, web_grid.Rows[index].Cells[1].Text);         
            // установка параметра выбранного продукта
            UserParams.CustomParam("food_stuff").Value =
                String.Format("{0}", web_grid.Rows[index].Cells[4].Text);
            // заполнение UltraChart данными
            chart.DataBind();
        }

        // --------------------------------------------------------------------
        
        /** <summary>
         *  Метод заполнения DataTable из CellSet
         *  примечание: надо бы проверить может и не только для этой страницы пригодится
         *  </summary>
         */
        public static void cellSet2DataTable(CellSet chart_set, DataTable chart_table, bool columns_by_name)
        {
            // проверка на наличие данных в таблице CellSet
            if (chart_set.Cells.Count == 0)
            {
                throw new Exception("cell set is empty");
            }
            // заполнение заголовков столбцов
            chart_table.Columns.Add(chart_table_name);            
            foreach (Position p in chart_set.Axes[0].Positions)
            {
                DataColumn dc = chart_table.Columns.Add(
                    columns_by_name ? p.Members[0].ToString() : p.Members[0].Caption);
                dc.DataType = typeof(Decimal);
            }
            // заполнение заголовков для строк
            foreach (Position p in chart_set.Axes[1].Positions)
            {
                chart_table.Rows.Add(p.Members[0].Caption);
            }
            // заполнение таблицы данными
            Int32 row_index = 0;
            Object[] values = null;
            foreach (DataRow row in chart_table.Rows)
            {
                values = row.ItemArray;
                foreach (DataColumn col in chart_table.Columns)
                {
                    if (col.Ordinal != 0)
                    {
                        values[col.Ordinal] = chart_set.Cells[col.Ordinal - 1, row_index].Value;
                    }
                }
                row.ItemArray = values;
                ++row_index;
            }
        }

        // --------------------------------------------------------------------

        /** <summary>
         *  Создание таблицы имен для продуктов питания 
         *  </summary>
         */
        private static Hashtable foodstuffNames()
        {
            Hashtable table = new Hashtable();
            String[] data = 
            {
                "В отчете", "В МДХ запросе",
                "Говядина (на кости)",  "Говядина на кости",
                "Куры (тушка 1 кат. отечественного производства)",  "Кура-тушка 1 категории",
                "Колбаса варёная (русская, любительская, докторская, молочная)", "Колбаса вареная типа в/c (Русская, Любительская)",
                "Масло сливочное (отечественного производства, без наполнителей)", "Масло сливочное коровье (жир не менее 72%)",
                "Масло подсолнечное (отечественного производства)", "Масло подсолнечное, рафинированное отечественное",
                "Молоко (3,2%, 2,5% жирности)", "Молоко цельное разл. (3,2 % жирности.)",
                "Творог (без наполнителей)", "Творог 5% жирности",
                "Кефир (без наполнителей)", "Кефир",
                "Сыр твёрдых сортов (отечественного производства)", "Сыр \"Российский\", твердый",
                "Яйца", "Яйцо куриное столовое диетическое",
                "Хлеб ржано-пшеничный", "Хлеб ржано-пшеничный",
                "Картофель", "Картофель свежий продовольственный 1кл",
                "Мука", "Мука пшеничная в/с",
                "Сахар", "Сахарный песок весовой",
                "Соль", "Соль поваренная, пищевая",
            };
            for (int i = 0; i < data.Length; i += 2)
            {
                table.Add(data[i + 1], data[i]);
            }
            return table;
        }

        // --------------------------------------------------------------------

        /** <summary>
         *  Создание таблицы имен для преобразования названий месяцев 
         *  </summary>
         */
        private static Hashtable monthNames()
        {
            Hashtable table = new Hashtable();
            String[] data = 
            {
                "Январь", "января",
                "Февраль", "февраля",
                "Март", "марта",
                "Апрель", "апреля",
                "Май", "мая",
                "Июнь", "июня",
                "Июль", "июля",
                "Август", "августа",
                "Сентябрь", "сентября",
                "Октябрь", "октября",
                "Ноябрь", "ноября",
                "Декабрь", "декабря",
            };
            for (int i = 0; i < data.Length; i += 2)
            {
                table.Add(data[i], data[i + 1]);
            }
            return table;
        }

        // --------------------------------------------------------------------

        /** <summary>
         *  Преобразование члена измерения времени в строку формата ЧЧ.ММ.ГГГГ
         * 
         *  до преобразования - [Период].[День].[Данные всех периодов].[2008].[Полугодие 2].[Квартал 4].[Декабрь].[1]
         *  после преобразования - 1 декабря 2008
         *  </summary>
         */
        public static String mdxTime2String(String str)
        {
            if (str == null) return null;
            String[] list = str.Split('.');
            for (int i = 0; i < list.Length; ++i)
            {
                list[i] = list[i].Replace("[", "");
                list[i] = list[i].Replace("]", "");
            }
            return (list.Length > 7) ?
                list[7] + " " + month_names[list[6]] + " " + list[3] :
                null;
        }

        // --------------------------------------------------------------------

        /** <summary>
         *  Метод получения последней актуальной даты 
         *  </summary>
         */
        private String getLastDate()
        {            
            try
            {
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("last_date"));
                return cs.Axes[1].Positions[0].Members[0].ToString();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        // --------------------------------------------------------------------

        /** <summary>
         *  Настройка формата сообщения об ошибке для UltraChart
         *  </summary>
         */
        public static void setChartErrorFont(Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = chart_error_message;
            e.LabelStyle.Font = new Font("Verdana", 20);           
            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
        }

        // --------------------------------------------------------------------

        public static void setChartSettings(UltraChart chart)
        {
            chart.Legend.BackgroundColor = Color.Empty;
            chart.Legend.BorderColor = Color.Empty;
        }

        // --------------------------------------------------------------------
    }
}
