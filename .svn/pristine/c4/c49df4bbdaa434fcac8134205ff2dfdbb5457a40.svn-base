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
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

/**
 *  Информация о ценах на нефтепродукты, реализуемые через 
 *  АЗС и другие хозяйствующие субъекты на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.ORG_0003_0002
{
    public partial class _default : CustomReportPage
    {
        // --------------------------------------------------------------------

        // заголовок страницы
        private static String page_title_caption = "Информация о ценах по нефтепродуктам на {0} ({1})";
        // заголовок для UltraChart по динамике розничной цены
        private static String chart_title_caption = "Динамика розничной цены по \"{0}\"";
        private static String chart_title_error = "Для отображения динамики цен необходимо выбрать производителя";
        // заголовок для UltraChart по уровню цен
        private static String grid_chart_caption = "Уровень цен на различные виды топлива в разрезе поставщиков на {0}";
        // заголовки для первого уровня таблицы
        private static String[] grid_columns_b0 = { "Наименование товаров, услуг", "Средняя цена, рублей" };
        // заголовки для второго уровня таблицы
        private static String[] grid_columns_b1 = { "Наименование товаров, услуг", "Продавец", "Цена, рублей" };
        // ширина экрана в пикселях
        private Int32 screen_width { get { return (int)Session["width_size"]; } }
        // высота экрана в пикселях
        private Int32 screen_height { get { return (int)Session["height_size"]; } }
        // параметр для последней актуальной даты
        private CustomParam last_date { get { return UserParams.CustomParam("last_date"); } }
        // параметр для выбранной.текущей территории
        private CustomParam current_region { get { return (UserParams.CustomParam("current_region")); } }

        // --------------------------------------------------------------------

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                base.Page_PreLoad(sender, e);
                // насройка размеров для элементов странички
                chart.Width = (screen_width - 50) * 6/10;
                chart.Height = (screen_height - 200) * 1/2;
                grid_chart.Width = screen_width - 50;
                grid_chart.Height = (screen_height - 200) * 4/10;                
            }
            catch (Exception)
            {
                // установка размеров прошла не удачно
            }
        }

        // --------------------------------------------------------------------

        protected override void Page_Load(object sender, EventArgs e)
        {            
            try
            {
                base.Page_Load(sender, e);
                if (!Page.IsPostBack)
                {
                    // Установка настроек для DimensionTree
                    //tree_area.HierarchyName = "[Территории].[РФ]";
                    //tree_area.DefaultMember = "[Территории].[РФ].[Все территории]";
                    //tree_area.InitTree();
                }
                // установка параметра территории
                current_region.Value = user_combo.SelectedItem.Value;
                // установка параметра последней актуальной даты
                last_date.Value = getLastDate();
                // получаем иныормацию о последней актуальной дате
                String date = ORG_0003_0001._default.mdxTime2String(last_date.Value);
                // установка заголовка для UltraChart по уровню цен
                grid_chart_lable.Text = String.Format(grid_chart_caption, date);
                // установка заголовка страницы
                page_title.Text = String.Format(
                    page_title_caption, date,
                    UserComboBox.getLastBlock(current_region.Value));
                // заполнение UltraWebGrid данными
                grid.DataBind();
                // раскраска UltraWebGrid в различные цвета
                setColorsOfGrid(grid);
                // заполнение диаграммы UltraChart
                grid_chart.DataBind();
                // установка заголовка к диаграмме по Динамике розничной цены
                chart_lable.Text = chart_title_error;
                if (!Page.IsPostBack)
                {
                    // установка строки по умолчанию
                    webGridActiveRowChange(grid.Rows[0].Rows[0], true);
                    grid.Rows[0].Expand(true);

                    // настройка диаграмм
                    ORG_0003_0001._default.setChartSettings(chart);
                    ORG_0003_0001._default.setChartSettings(grid_chart);
                }                
            }
            catch (Exception)
            {
                // не удачная загрузка страницы
            }
        }

        // --------------------------------------------------------------------

        protected void grid_DataBinding(object sender, EventArgs e)
        {            
            DataSet grid_ds = new DataSet();
            DataTable grid_master = new DataTable();
            DataTable grid_detail = new DataTable();
            try
            {
                // получаем данные для таблицы первого уровня
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid_master"), "Product", grid_master);
                // получаем данные для таблицы второго уровня
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(DataProvider.GetQueryText("grid_detail"), grid_detail);
                // удаляем лишние столбцы в таблице второго уровня
                grid_detail.Columns.RemoveAt(0);
                grid_detail.Columns.RemoveAt(0);
                // установка заголовков столбцов для первого и второго уровней таблицы
                for (int i = 0; i < grid_master.Columns.Count; ++i)
                    grid_master.Columns[i].Caption = grid_columns_b0[i];
                for (int i = 0; i < grid_detail.Columns.Count; ++i)
                    grid_detail.Columns[i].Caption = grid_columns_b1[i];
                // выполняем связывание таблиц первого и второго уровня в DataSet
                grid_ds.Tables.Add(grid_master);
                grid_ds.Tables.Add(grid_detail);
                grid_ds.Relations.Add(grid_master.Columns[0], grid_detail.Columns[0]);
            }
            catch (Exception)
            {
                grid_ds = null;
            }
            // устанавливаем источник данных для UltraWebGrid
            grid.DataSource = grid_ds == null ? null : grid_ds.Tables[0].DefaultView;
        }

        // --------------------------------------------------------------------

        protected void grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            try
            {                                
                // насройка видимости и размеров столбцов в UltraWebGrid 
                grid.Bands[0].Columns[0].Width = 200;
                grid.Bands[0].Columns[1].Width = 150;
                grid.Bands[1].Columns[0].Hidden = true;
                grid.Bands[1].Columns[1].Width = 300;
                grid.Bands[1].Columns[2].Width = 80;
                // установка формата отображения данных в UltraWebGrid
                CRHelper.FormatNumberColumn(grid.Bands[0].Columns[1], "N2");
                CRHelper.FormatNumberColumn(grid.Bands[1].Columns[2], "N2");
            }
            catch (Exception)
            {
                // неуспешно отформатировали таблицу
            }
        }

        // --------------------------------------------------------------------

        protected void chart_DataBinding(object sender, EventArgs e)
        {
            DataTable chart_table = new DataTable();
            try
            {
                // формирование DataTable для диаграммы цен за три месяца по выбранному продавцу
                ORG_0003_0001._default.cellSet2DataTable(
                    DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("chart")), chart_table, true);
                // форматирование столбцовых заголовков
                foreach (DataColumn col in chart_table.Columns)
                {
                    if (col.Ordinal > 0)
                        col.ColumnName = ORG_0003_0001._default.mdxTime2String(col.ColumnName);
                }
                // настройка отображения UltraChart
                setUltraChartRanges(chart, chart_table);
            }
            catch (Exception)
            {
                chart_table = null;
            }            
            // установка источника данных для диаграммы цен по выбранному производителю
            chart.DataSource = chart_table == null ? null : chart_table.DefaultView;            
        }

        // --------------------------------------------------------------------

        protected void grid_chart_DataBinding(object sender, EventArgs e)
        {
            DataTable chart_table = new DataTable();
            try
            {
                // заполнение таблицы для диаграммы уровня цен на топливо
                ORG_0003_0001._default.cellSet2DataTable(
                    DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid_chart")),
                    chart_table, false
                );
                // настройка отображения UltraChart
                setUltraChartRanges(grid_chart, chart_table);
            }
            catch (Exception)
            {
                chart_table = null;
            }
            grid_chart.DataSource = chart_table == null ? null : chart_table.DefaultView;            
        }
            
        // --------------------------------------------------------------------

        protected void grid_ActiveRowChange(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            webGridActiveRowChange(e.Row, false);
        }

        // --------------------------------------------------------------------

        protected void chart_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            ORG_0003_0001._default.setChartErrorFont(e);
        }

        // --------------------------------------------------------------------

        protected void grid_chart_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            ORG_0003_0001._default.setChartErrorFont(e);
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
         *  Установка параметров для UltraChart (избавляемся от пустых ячеек)
         *  </summary>
         */
        public static void setUltraChartRanges(UltraChart chart, DataTable chart_table)
        {
            if (chart_table == null) return;

            bool init = false;
            double min = 0;
            double max = 0;
            // 1. вычисление максимального и минимального элементов в таблице
            // 2. заполнение пустых ячеек очень большими числами
            foreach (DataRow row in chart_table.Rows)
            {
                Object[] values = row.ItemArray;
                for (int i = 1; i < values.Length; ++i)
                {
                    if (values[i].ToString().Length == 0)
                    {
                        values[i] = -1.0E+10;
                    }
                    else
                    {
                        double value = Convert.ToDouble(values[i]);
                        if (!init)
                        {
                            min = max = value;
                            init = true;
                        }
                        else
                        {
                            if (min > value) min = value;
                            if (max < value) max = value;
                        }
                    }
                }
                row.ItemArray = values;
            }
            // установка настроек для UltraChart
            chart.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
            chart.Axis.Y.RangeMin = min;
            chart.Axis.Y.RangeMax = max;

            chart.Data.UseMinMax = true;
            chart.Data.MinValue = min;
            chart.Data.MaxValue = max;
        }

        // --------------------------------------------------------------------

        /** <summary>
         *  Настройка цветов отображения для UltraWebGrid
         *  </summary>
         */
        public static void setColorsOfGrid(UltraWebGrid grid)
        {            
            foreach (UltraGridRow row in grid.Rows)
            {
                Boolean init = false;
                Decimal min = 0, max = 0;
                Int32 i_min = -1, i_max = -1;
                foreach (UltraGridRow sub_row in row.Rows)
                {
                    if (sub_row.Cells.Count < 3) break;
                    Decimal value = Convert.ToDecimal(sub_row.Cells[2].Value);
                    if (min > value || !init)
                    {
                        min = value;
                        i_min = sub_row.Index;
                    }
                    if (max < value || !init)
                    {
                        max = value;
                        i_max = sub_row.Index;
                    }
                    init = true;
                }
                if (init)
                {
                    Color[] colors = { Color.LightGreen, Color.LightCoral };
                    row.Rows[i_min].Cells[2].Style.BackColor = colors[0];
                    row.Rows[i_max].Cells[2].Style.BackColor = colors[1];
                }
            }
        }

        // --------------------------------------------------------------------

        /** <summary>
         *  Устанавливает текущую строку в таблице
         *  параметры:
         *      row - настраиваемая строка;
         *      active - устанавливать или нет свойство 'Activate' у строки.
         *  </summary>
         */
        private void webGridActiveRowChange(UltraGridRow row, bool active)
        {
            try
            {
                if (row.Band.Index == 1) // выбрана строка из второго уровня таблицы
                {
                    if (active) row.Activate();
                    // извлекаем производителя/продавца в строку
                    String seller = row.Cells[1].Value.ToString().Replace("\\\"", "\"");
                    // устанавливаем параметр текущего производителя/продавца
                    UserParams.CustomParam("seller_name").Value =
                        row.Cells[1].Column.Header.Key.Replace("MEMBER_CAPTION", seller);
                    chart.DataBind();
                    // установка заголовка для диаграммы
                    chart_lable.Text = String.Format(chart_title_caption, seller);
                }
                else // выбрана строка из первого уровня таблицы
                {
                    // установка заголовка для диаграммы
                    chart_lable.Text = chart_title_error;
                }
            }
            catch (Exception)
            {
                // что-то пошло не так :(
            }
        }

        // --------------------------------------------------------------------

        protected void grid_DblClick(object sender, ClickEventArgs e)
        {
        }
       
        // --------------------------------------------------------------------
    }
}
