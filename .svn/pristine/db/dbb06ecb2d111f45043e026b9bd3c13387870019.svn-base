using System;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

using Microsoft.AnalysisServices.AdomdClient;
/**
 *  Труд и заработная плата  
 */
namespace Krista.FM.Server.Dashboards.reports.PMO_0001_0005
{
    public partial class _default : CustomReportPage
    {
        // заголовок страницы
        private static String page_title_caption = "Труд и заработная плата на {0} год ({1})";
        private static String page_title_empty = "Труд и заработная плата";
        // заголовок таблицы
        private static String grid_caption = "Среднесписочная численность работающих на предприятиях и организациях в {0} году";
        private static String grid_empty = "Среднесписочная численность работающих на предприятиях и организациях";
        // заголовки для столбцов таблицы
        private static String[] grid_columns = { "Отрасль", "Численность" };
        // заголовок для круговой диаграммы
        private static String chart_pie_caption = "Удельный вес отраслей в распределении трудовых ресурсов по состоянию на {0} год";
        private static String chart_pie_empty = "Удельный вес отраслей в распределении трудовых ресурсов";
        // заголовок для диаграммы по "динамике среднесписочной численности работающих" 
        private static String chart_avg_count_caption = "Динамика среднесписочной численности работающих в отрасли \"{0}\"";
        private static String chart_avg_count_empty = "Для отображения динамики среднесписочной численности работающих необходимо выбрать отрасль";
        // заголовок для диаграммы по ""
        private static String chart_avg_wage_caption = "Динамика среднемесячного дохода работающих в отрасли \"{0}\"";
        private static String chart_avg_wage_empty = "Для отображения динамики среднемесячного дохода работающих необходимо выбрать отрасль";

        // --------------------------------------------------------------------

        // параметр для выбранной/текущей отрасли
        private CustomParam branch { get { return (UserParams.CustomParam("current_branch")); } }
        // параметр для выбранной.текущей территории
        private CustomParam region { get { return (UserParams.CustomParam("current_region")); } }
        // параметр для последней актуальной даты
        private CustomParam last_year { get { return (UserParams.CustomParam("last_year")); } }

        // --------------------------------------------------------------------

        // ширина экрана в пикселях
        private Int32 screen_width { get { return (int)Session["width_size"]; } }
        // высота экрана в пикселях
        private Int32 screen_height { get { return (int)Session["height_size"]; } }

        // --------------------------------------------------------------------

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            try
            {
                // настройка размеров диаграмм
                chart_pie.Width = (screen_width - 50) * 1 / 2;
                chart_avg_count.Width = (screen_width - 50) * 1 / 2;
                chart_avg_wage.Width = (screen_width - 50) * 1 / 2;
            }
            catch (Exception)
            {
                // что-то пошло не так ...
            }
        }

        // --------------------------------------------------------------------

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            try
            {
                if (!Page.IsPostBack)
                {
                    // Установка настроек для DimensionTree
                    //tree_area.HierarchyName = "[Территории].[РФ]";
                    //tree_area.DefaultMember = "[Территории].[РФ].[Все территории]";
                    //tree_area.InitTree();

                    // настройка диаграмм
                    ORG_0003_0001._default.setChartSettings(chart_avg_count);
                    ORG_0003_0001._default.setChartSettings(chart_avg_wage);
                    ORG_0003_0001._default.setChartSettings(chart_pie);
                }
                // установка параметра территории
                region.Value = user_combo.SelectedItem.Value;
                // загрузка параметра последней актуальной даты
                last_year.Value = getLastDate();
                String year = UserComboBox.getLastBlock(last_year.Value);
                // установка заголовка страницы
                page_title.Text = String.Format(
                    page_title_caption, year,
                    UserComboBox.getLastBlock(region.Value));
                // установка заголовков                                
                grid_title.Text = String.Format(grid_caption, year);
                chart_pie_title.Text = String.Format(chart_pie_caption, year);
                chart_avg_count_title.Text = chart_avg_count_empty;
                chart_avg_wage_title.Text = chart_avg_wage_empty;
                if (year.Length == 0 || region.Value.Length == 0)
                {   // если с данными что-то не так то устанавливаем альтернативные заголовки
                    page_title.Text = page_title_empty;
                    grid_title.Text = grid_empty;
                    chart_pie_title.Text = chart_pie_empty;
                }
                // загрузка данных
                grid.DataBind();
                chart_pie.DataBind();
                // установка активной ячейки в UltraWebGrid
                webGridActiveRowChange(0, true);
            }
            catch (Exception) 
            {
                // не удачно загрузились ...
            }
        }

        // --------------------------------------------------------------------

        protected void grid_DataBinding(object sender, EventArgs e)
        {
            grid.DataSource = dataBind("grid");
        }

        // --------------------------------------------------------------------

        protected void chart_pie_DataBinding(object sender, EventArgs e)
        {
            chart_pie.DataSource = dataBind("grid");
        }

        // --------------------------------------------------------------------

        protected void chart_avg_count_DataBinding(object sender, EventArgs e)
        {            
            chart_avg_count.DataSource = dataBind("chart_avg_count");
        }

        // --------------------------------------------------------------------

        protected void chart_avg_wage_DataBinding(object sender, EventArgs e)
        {            
            chart_avg_wage.DataSource = dataBind("chart_avg_wage");
        }

        // --------------------------------------------------------------------

        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            try
            {
                // настройка размеров UltraWebGrid
                grid.Bands[0].Columns[0].Width = 300;
                // установка заголовков для столбцов
                for (int i = 0; i < grid_columns.Length; ++i)
                    grid.Bands[0].Columns[i].Header.Caption = grid_columns[i];
                // установка формата ячеек
                CRHelper.FormatNumberColumn(grid.Bands[0].Columns[1], "##,###,###");
            }
            catch (Exception)
            {
                // форматирование UltraWebGrid прошло не удачно ...
            }
        }

        // --------------------------------------------------------------------

        protected void chart_avg_count_Init(object sender, EventArgs e)
        {
            // добавляем метки со значениями на диаграмму 
            // из дизайнера почему-то они не добавляются
            ChartTextAppearance text = 
                new ChartTextAppearance();
                text.Row = -2;
                text.Column = -2;            
                text.ItemFormatString = "<DATA_VALUE:0,0>";
                text.VerticalAlign = StringAlignment.Far;
                text.Visible = true;
            chart_avg_count.SplineAreaChart.ChartText.Add(text);            
        }

        // --------------------------------------------------------------------

        protected void grid_ActiveRowChange(object sender, RowEventArgs e)
        {
            webGridActiveRowChange(e.Row.Index, false);
        }

        // --------------------------------------------------------------------

        protected void chart_pie_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            ORG_0003_0001._default.setChartErrorFont(e);
        }

        // --------------------------------------------------------------------

        protected void chart_avg_wage_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            ORG_0003_0001._default.setChartErrorFont(e);
        }

        // --------------------------------------------------------------------

        protected void chart_avg_count_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            ORG_0003_0001._default.setChartErrorFont(e);
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
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(query_name), "sfn", table);
            }
            catch (Exception)
            {
                table = null;
            }
            return table == null ? null : table.DefaultView;
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
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("last_year"));
                return cs.Axes[1].Positions[0].Members[0].ToString();
            }
            catch (Exception e)
            {
                return null;
            }
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
                UltraGridRow row = grid.Rows[index];
                // устанавливаем ее активной, если необходимо
                if (active) row.Activate();
                // получение заголовка выбранной отрасли
                String branch_name = row.Cells[0].Value.ToString();
                // установка параметра выбранной отрасли
                branch.Value = branch_name;
                // загрузка данных для диаграмм
                chart_avg_count.DataBind();
                chart_avg_wage.DataBind();
                // установка заголовков для диаграмм
                chart_avg_count_title.Text = String.Format(chart_avg_count_caption, branch_name);
                chart_avg_wage_title.Text = String.Format(chart_avg_wage_caption, branch_name);
            }
            catch (Exception)
            {
                // выбор сторки пошел с ошибкой ...
            }
        }

        // --------------------------------------------------------------------

        protected void grid_DblClick(object sender, ClickEventArgs e)
        {
        }
        
        // --------------------------------------------------------------------
    }
}
