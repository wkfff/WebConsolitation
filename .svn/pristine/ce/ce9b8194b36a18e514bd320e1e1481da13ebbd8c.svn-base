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
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;

/**
 *  Показатели сферы образования.
 */
namespace Krista.FM.Server.Dashboards.reports.PMO_0001_0007
{
    public partial class _default : Dashboards.CustumReportPage
    {
        // --------------------------------------------------------------------

        // заголовок страницы
        private static String page_title_caption = "Показатели сферы образования {0} год <nobr>({1})</nobr>";
        // заголовок для Grid
        private static String grid_title_caption = "Основные показатели в {0} году.";
        // параметр для выбранной.текущей территории
        private CustomParam current_region { get { return (new CustomParam("current_region", Session)); } }
        // параметр для последней актуальной даты
        private CustomParam last_year { get { return (new CustomParam("last_year", Session)); } }
        // ширина экрана в пикселях
        private Int32 screen_width { get { return (int)Session["width_size"]; } }
        // заголовки столбцов для UltraWebGrid
        private static String[] grid_columns = { "№ п/п", "Показатели", "Ед. изм.", "Значение" };

        // --------------------------------------------------------------------

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                base.Page_PreLoad(sender, e);
                // установка размера диаграммы
                //chart.Width = screen_width - 50;
                //PMO_0001_0003._default.setChartSettings(chart_avg_count);
                chart_avg_count.Width = (screen_width + 100) / 2;
                chart_pie1.Width = (screen_width - 100) / 3;
                chart_pie2.Width = (screen_width - 100) / 3;
                chart_pie3.Width = (screen_width - 100) / 3;
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
                    ORG_0003_0001._default.setChartSettings(chart_avg_count);
                    ORG_0003_0001._default.setChartSettings(chart_pie1);
                    ORG_0003_0001._default.setChartSettings(chart_pie2);
                    ORG_0003_0001._default.setChartSettings(chart_pie3);
                }
                // установка параметра территории
                current_region.Value = user_combo.SelectedItem.Value;

                last_year.Value = getLastDate();
                String year = core.UserComboBox.getLastBlock(last_year.Value);

                // установка заголовка для страницы
                page_title.Text = String.Format(
                    page_title_caption, year,
                    core.UserComboBox.getLastBlock(current_region.Value));
                grid_caption.Text = String.Format(grid_title_caption, year);
                // заполнение UltraWebGrid данными
                web_grid.DataBind();
                chart_avg_count.DataBind();
                chart_pie1.DataBind();
                chart_pie2.DataBind();
                chart_pie3.DataBind();
            }
            catch (Exception)
            {
                // неудачная загрузка ...
            }
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

        protected void HeaderPR1_Load(object sender, EventArgs e)
        {

        }

        // --------------------------------------------------------------------

        protected void web_grid_DataBinding(object sender, EventArgs e)
        {
            // Установка параметра последней актуальной даты
            //new CustomParam("period_last_date", Session).Value = getLastDate();
            //DataTable chart_table = new DataTable();
            //string query = DataProvider.GetQueryText("grid");
            //DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатели", chart_table);
            //chart_table.Columns[1].Caption = "Значение";
            //web_grid.DataSource = chart_table.DefaultView;

            DataTable grid_table = new DataTable();
            // Установка параметра последней актуальной даты
            new CustomParam("period_last_date", Session).Value = getLastDate();

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
                        grid_set.Axes[1].Positions[pos.Ordinal].Members[0].Caption,
//                      grid_names[grid_set.Axes[1].Positions[pos.Ordinal].Members[0].Caption],
                        pos.Members[0].MemberProperties[0].Value.ToString(),
                        grid_set.Cells[0, pos.Ordinal].Value
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
            web_grid.Bands[0].Columns[1].Width = 350;
            web_grid.Bands[0].Columns[2].Width = 50;
            web_grid.Bands[0].Columns[3].Width = 60;
            web_grid.Bands[0].Columns[1].CellStyle.Wrap = true;
            //web_grid.Bands[0].Columns[1].Width = 370; // TODO: переделать: надо бы сделать вычислиямые размеры
            //web_grid.Bands[0].Hidden = true;
            // установка формата отображения ячеек
            CRHelper.FormatNumberColumn(web_grid.Bands[0].Columns[0], "N2");
            CRHelper.FormatNumberColumn(web_grid.Bands[0].Columns[3], "N2");
        }

        // --------------------------------------------------------------------

       /// <summary>
       /// 
       /// </summary>
       /// <param name="query_name"></param>
       /// <returns></returns>
        protected DataView dataBind(String query_name)
        {
            DataTable table = new DataTable();
            try
            {
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(query_name), "sfn", table);
            }
            catch (Exception)
            {
                table = null;
            }
            return table == null ? null : table.DefaultView;
        }

        // --------------------------------------------------------------------

        protected void chart_avg_count_DataBinding(object sender, EventArgs e)
        {
            chart_avg_count.DataSource = dataBind("chart_avg_count");
        }

        protected void chart_pie_DataBinding(object sender, EventArgs e)
        {
            chart_pie1.DataSource = dataBind("chart_pie1");
        }

        protected void chart_pie2_DataBinding(object sender, EventArgs e)
        {
            chart_pie2.DataSource = dataBind("chart_pie2");
        }

        protected void chart_pie3_DataBinding(object sender, EventArgs e)
        {
            chart_pie3.DataSource = dataBind("chart_pie3");
        }

        // --------------------------------------------------------------------

        protected void InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            ORG_0003_0001._default.setChartErrorFont(e);
        }

    }

}

