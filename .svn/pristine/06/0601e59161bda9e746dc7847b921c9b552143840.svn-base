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
 *  Информация по состоянию учреждений культуры, искусства, физической культуры и спорта.
 */
namespace Krista.FM.Server.Dashboards.reports.PMO_0001_0008
{
    public partial class _default : Dashboards.CustumReportPage
    {
        // заголовок страницы
        private static String pageTitleCaption = "Информация по состоянию учреждений культуры, искусства, физической культуры и спорта на {0} год <nobr>({1})</nobr>";
        // заголовок для Grid
        private static String grid1TitleCaption = "Показатели учреждений культуры в {0} году.";
        // заголовок для Grid2
        private static String grid2TitleCaption = "Показатели учреждений физической культуры и спорта в {0} году.";
        // заголовок для Chart1
        private static String chart1TitleCaption = "Число спортивных сооружений и их удельный вес в общем объеме в {0} году.";
        // заголовок для Chart2
        private static String chart2TitleCaption = "Мощность спортивных сооружений (посещений) и их удельный вес в общем объеме в {0} году.";          
        // параметр для выбранной/текущей территории
        private CustomParam current_region { get { return (new CustomParam("current_region", Session)); } }
        // параметр для последней актуальной даты
        private CustomParam last_year { get { return (new CustomParam("last_year", Session)); } }
        // параметр для выбранного/текущего показателя
        private CustomParam branch { get { return (new CustomParam("current_branch", Session)); } }
        // параметр для выбранного года
        private CustomParam selected_year { get { return (new CustomParam("selected_year", Session)); } }
        // ширина экрана в пикселях
        private Int32 screen_width { get { return (int)Session["width_size"]; } }
        // заголовки столбцов для UltraWebGrid
        private static String[] grid1Сolumns = { "№ п/п", "Показатели", "Ед. изм.", "Значение" };


        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                base.Page_PreLoad(sender, e);
                // установка размера диаграмм
                chart_dinamic.Width = (screen_width + 150) / 2;
                chart_pie1.Width = (screen_width - 100) / 2;
                chart_pie2.Width = (screen_width - 100) / 2;
            }
            catch (Exception)
            {
                // установка размеров не удалась ...
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                base.Page_Load(sender, e);
                if (!Page.IsPostBack)
                {   // опрерации которые должны выполняться при только первой загрузке страницы
                    ORG_0003_0001._default.setChartSettings(chart_dinamic);
                    ORG_0003_0001._default.setChartSettings(chart_pie1);
                    ORG_0003_0001._default.setChartSettings(chart_pie2);
                }
                // установка параметра территории
                current_region.Value = user_combo.SelectedItem.Value;

                last_year.Value = getLastDate();
                String year = core.UserComboBox.getLastBlock(last_year.Value);

                // установка заголовка для страницы
                page_title.Text = String.Format(
                    pageTitleCaption, year,
                    core.UserComboBox.getLastBlock(current_region.Value));
                grid_caption.Text = String.Format(grid1TitleCaption, year);
                TitleGrid2.Text = String.Format(grid2TitleCaption, year);
                // заполнение UltraWebGrid данными
                web_grid.DataBind();
                grid2.DataBind();
                // заполнение UltraChart данными
                chart_pie1.DataBind();
                chart_pie2.DataBind();
                // установка активной строки в UltraWebGrid
                webGridActiveRowChange(0, true);
                // установка активной ячейки в UltraWebGrid
                grid2Manual_ActiveCellChange(10);
            }
            catch (Exception)
            {
                // неудачная загрузка ...
            }
        }

        /// <summary>
        /// Метод получения последней актуальной даты
        /// </summary>
        /// <returns>возвращает строку - год</returns>
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

        protected void HeaderPR1_Load(object sender, EventArgs e)
        {

        }

        protected void web_grid_DataBinding(object sender, EventArgs e)
        {
            DataTable grid_table = new DataTable();
            // Установка параметра последней актуальной даты
            new CustomParam("period_last_date", Session).Value = getLastDate();
            try
            {
                CellSet grid_set = null;
                // Загрузка данных в CellSet
                grid_set = PrimaryMASDataProvider.GetCellset(GetQueryText("grid"));
                // Добавление столбцов в таблицу и заполнение данных в DataTable
                for (int i = 0; i < grid1Сolumns.Length; ++i)
                {   // установка заголовков для столбцов UltraWebGrid
                    grid_table.Columns.Add(grid1Сolumns[i]);
                }
                foreach (Position pos in grid_set.Axes[1].Positions)
                {   // создание списка значений для строки UltraWebGrid
                    object[] values = {
                        pos.Ordinal + 1,
                        grid_set.Axes[1].Positions[pos.Ordinal].Members[0].Caption,
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
            // настройка столбцов UltraWebGrid
            web_grid.Bands[0].Columns[0].Width = 40;
            web_grid.Bands[0].Columns[1].Width = 280;
            web_grid.Bands[0].Columns[2].Width = 70;
            web_grid.Bands[0].Columns[3].Width = 60;
            web_grid.Bands[0].Columns[1].CellStyle.Wrap = true;
            web_grid.Bands[0].Columns[2].CellStyle.Wrap = true;
            //web_grid.Bands[0].Columns[1].Width = 370; // TODO: переделать: надо бы сделать вычислиямые размеры
            // установка формата отображения ячеек
            CRHelper.FormatNumberColumn(web_grid.Bands[0].Columns[0], "N2");
            CRHelper.FormatNumberColumn(web_grid.Bands[0].Columns[3], "N2");
        }

        /// <summary>
        /// Метод выполняет запрос и возвращает DataView, в случае неудачи возвращает 'null' (nothrow)
        /// </summary>
        /// <param name="query_name">имя запроса MDX</param>
        /// <returns>DataView</returns>
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

        /// <summary>
        /// Установка активной сторки элемента UltraWebGrid
        /// </summary>
        /// <param name="index">выбранная строка</param>
        /// <param name="active">установить или нет свойство 'Active' у выбранной строки</param>
        private void webGridActiveRowChange(Int32 index, bool active)
        {
            try
            {
                // получаем выбранную строку
                UltraGridRow row = web_grid.Rows[index];
                // устанавливаем ее активной, если необходимо
                if (active) row.Activate();
                // получение заголовка выбранного показателя
                String branch_name = row.Cells[1].Value.ToString();
                // установка параметра выбранного показателя
                branch.Value = branch_name;
                // загрузка данных для диаграммы
                chart_dinamic.DataBind();
                // установка заголовка для диаграммы
                TitleChart.Text = branch_name;
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

        protected void chart_dinamic_DataBinding(object sender, EventArgs e)
        {
            chart_dinamic.DataSource = dataBind("chart_dinamic");
        }

        protected void chart_pie1_DataBinding(object sender, EventArgs e)
        {
            chart_pie1.DataSource = dataBind("chart_pie1");
        }

        protected void chart_pie2_DataBinding(object sender, EventArgs e)
        {
            chart_pie2.DataSource = dataBind("chart_pie2");
        }

        protected void chart_dinamic_Init(object sender, EventArgs e)
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
            chart_dinamic.SplineAreaChart.ChartText.Add(text);
        }

        protected void chart_dinamic_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
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

        protected void grid2_DataBinding(object sender, EventArgs e)
        {
            // Установка параметра последней актуальной даты
            new CustomParam("period_last_date", Session).Value = getLastDate();
            DataTable chart_table = new DataTable();
            // Загрузка данных в DataTable
            string query = GetQueryText("grid2");
            this.SecondaryMASDataProvider.GetDataTableForChart(query, "Год", chart_table);
            // Добаление единицы измерения
            object[] ItemsForChange = {};
            ItemsForChange = chart_table.Rows[0].ItemArray;
            ItemsForChange[0] = ItemsForChange[0] + " (Тысяча человек)";
            chart_table.Rows[0].ItemArray = ItemsForChange;
            // установка источника данных для UltraWebGrid
            grid2.DataSource = chart_table.DefaultView;
        }

        protected void grid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            // настройка столбцов
            grid2.Bands[0].Columns[0].Width = 500;
            grid2.Bands[0].Columns[0].CellStyle.Wrap = true;
            grid2.Bands[0].Columns[0].CellStyle.Height = 50;
            for (int i = 1; i < grid2.Bands[0].Columns.Count; ++i)
            {
                // настройка столбцов
                grid2.Bands[0].Columns[i].Width = 40;
                // установка формата отображения ячеек
                CRHelper.FormatNumberColumn(grid2.Bands[0].Columns[i], "N2");
            }
            // Установка цвета первой ячейки
            grid2.Bands[0].Columns[0].CellStyle.BackColor = Color.White;
        }

        protected void grid2_ActiveCellChange(object sender, CellEventArgs e)
        {
            UltraGridCell cell = e.Cell;
            int CellIndex = cell.Column.Index;
            if (CellIndex > 0)
            {
                grid2Manual_ActiveCellChange(CellIndex);
            }
        }
        /// <summary>
        /// Обновление данных для UltraChart при изменении активной ячейки в UltraWebGrid
        /// </summary>
        /// <param name="CellIndex">индекс активной ячейки</param>
        protected void grid2Manual_ActiveCellChange(int CellIndex)
        {
            selected_year.Value = grid2.Columns[CellIndex].Header.Key.ToString();
            chart1title.Text = String.Format(chart1TitleCaption, selected_year.Value);
            chart2title.Text = String.Format(chart2TitleCaption, selected_year.Value);
            chart_pie1.DataBind();
            chart_pie2.DataBind();
        }
    }

}

