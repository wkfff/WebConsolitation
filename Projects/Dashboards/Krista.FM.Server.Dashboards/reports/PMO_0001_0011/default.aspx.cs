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
 *  Информация о состоянии жилищного фонда.
 */
namespace Krista.FM.Server.Dashboards.reports.PMO_0001_0011
{
    public partial class _default : Dashboards.CustumReportPage
    {
        // --------------------------------------------------------------------

        // заголовок страницы
        private static String page_title_caption = "Информация о состоянии жилищного фонда на {0} год <nobr>({1})</nobr>";
        // заголовок для Grid
        private static String grid_title_caption = "Основные показатели жилищного фонда в {0} году.";
        // параметр для выбранной.текущей территории
        private CustomParam current_region { get { return (new CustomParam("current_region", Session)); } }
        // параметр для последней актуальной даты
        private CustomParam last_year { get { return (new CustomParam("last_year", Session)); } }
        // параметр для выбранной/текущей отрасли
        private CustomParam branch { get { return (new CustomParam("current_branch", Session)); } }
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
                chart_avg_count.Width = (screen_width + 150) / 2;
                //main_table.Width = screen_width - 100;
                chart_pie1.Width = (screen_width - 100) / 2;
                chart_pie2.Width = (screen_width - 100) / 2;
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
                chart_pie1.DataBind();
                chart_pie2.DataBind();
                // установка активной строки в UltraWebGrid
                //webGridActiveRowChange(0);
               // web_grid.Rows[0].Activate();
                webGridActiveRowChange(0, true);
                //UltraWebGridXXX.DataBind();


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
                CellSet cs = PrimaryMASDataProvider.GetCellset(GetQueryText("last_date"));
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

        protected void web_grid_DataBinding(object sender, EventArgs e)
        {
            // Установка параметра последней актуальной даты
            //new CustomParam("period_last_date", Session).Value = getLastDate();
            //DataTable chart_table = new DataTable();
            //string query = GetQueryText("grid");
            //this.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатели", chart_table);
            //chart_table.Columns[1].Caption = "Значение";
            //web_grid.DataSource = chart_table.DefaultView;

            DataTable grid_table = new DataTable();
            // Установка параметра последней актуальной даты
            new CustomParam("period_last_date", Session).Value = getLastDate();

            try
            {
                CellSet grid_set = null;
                // Загрузка таблицы цен и товаров в CellSet
                grid_set = PrimaryMASDataProvider.GetCellset(GetQueryText("grid"));
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

        protected void web_grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            // настройка столбцов
            web_grid.Bands[0].Columns[0].Width = 40;
            web_grid.Bands[0].Columns[1].Width = 260;
            web_grid.Bands[0].Columns[3].Width = 60;
            web_grid.Bands[0].Columns[1].CellStyle.Wrap = true;
            web_grid.Bands[0].Columns[2].CellStyle.Wrap = true;
            //web_grid.Bands[0].Columns[1].Width = 370; // TODO: переделать: надо бы сделать вычислиямые размеры
            //web_grid.Bands[0].Hidden = true;
            // установка формата отображения ячеек
            CRHelper.FormatNumberColumn(web_grid.Bands[0].Columns[0], "N2");
            CRHelper.FormatNumberColumn(web_grid.Bands[0].Columns[3], "N2");
        }

        // --------------------------------------------------------------------

        /** <summary>
         *  Метод выполняет запрос и возвращает DataView, в случае неудачи 
         *  возвращает 'null' (nothrow)
         *  </summary>        
         */
        protected DataView dataBind(String query_name)
        {
            DataTable table = new DataTable();
            try
            {
                PrimaryMASDataProvider.GetDataTableForChart(GetQueryText(query_name), "sfn", table);
            }
            catch (Exception)
            {
                table = null;
            }
            return table == null ? null : table.DefaultView;
        }

        // --------------------------------------------------------------------
        /** <summary>
         *  Установка активной сторки элемента UltraWebGrid
         *  params: 
         *      index - выбранная строка;
         *      active - установить или нет свойство 'Active' у выбранной строки.
         *  </summary>         
         */
        
        private void webGridActiveRowChange(Int32 index, bool active)
        {
            try
            {
                // получаем выбранную строку
                UltraGridRow row = web_grid.Rows[index];
                // устанавливаем ее активной, если необходимо
                if (active) row.Activate();
                // получение заголовка выбранной отрасли
                String branch_name = row.Cells[1].Value.ToString();
                // установка параметра выбранной отрасли
                branch.Value = branch_name;
                // загрузка данных для диаграмм
                chart_avg_count.DataBind();
                //chart_avg_wage.DataBind();
                // установка заголовков для диаграмм
                Label1.Text = branch_name;
                //chart_avg_count_title.Text = String.Format(chart_avg_count_caption, branch_name);
                //chart_avg_wage_title.Text = String.Format(chart_avg_wage_caption, branch_name);
            }
            catch (Exception)
            {
                // выбор сторки пошел с ошибкой ...
            }
        }

        // --------------------------------------------------------------------

        protected void web_grid_ActiveRowChange(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            webGridActiveRowChange(e.Row.Index, false);
        }

        protected void chart_avg_count_DataBinding(object sender, EventArgs e)
        {
            chart_avg_count.DataSource = dataBind("chart_avg_count");
            
            
        }

        protected void chart_pie1_Init(object sender, EventArgs e)
        {
            // добавляем метки со значениями на диаграмму 
            // из дизайнера почему-то они не добавляются
            Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance text =
                new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance();
            text.Row = -2;
            text.Column = -2;
            text.ItemFormatString = "<DATA_VALUE:0,0>";
            text.VerticalAlign = System.Drawing.StringAlignment.Far;
            text.Visible = true;
            chart_pie1.SplineAreaChart.ChartText.Add(text);
        }

        // --------------------------------------------------------------------


        protected void chart_avg_count_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }

        protected void chart_pie_DataBinding(object sender, EventArgs e)
        {
            chart_pie1.DataSource = dataBind("chart_pie1");
        }

        protected void chart_pie2_DataBinding(object sender, EventArgs e)
        {
            chart_pie2.DataSource = dataBind("chart_pie2");
        }

        protected void _Init(object sender, EventArgs e)
        {


        }

        protected void chart_avg_count_Init(object sender, EventArgs e)
        {
            chart_avg_count.Data.ZeroAligned = true;
        }

        protected void chart_avg_count_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            ORG_0003_0001._default.setChartErrorFont(e);
        }

        protected void chart_pie1_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            ORG_0003_0001._default.setChartErrorFont(e);
        }

        protected void chart_pie2_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            ORG_0003_0001._default.setChartErrorFont(e);
        }

    }

}

