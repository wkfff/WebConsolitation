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
using Infragistics.UltraGauge.Resources;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.UltraChart.Core.Primitives;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;
/**
 *  Торговля, общественное питание и бытовое обслуживание.
 */
namespace Krista.FM.Server.Dashboards.reports.BC_0001_0021
{
 
    public partial class Default : CustomReportPage
    {
        // --------------------------------------------------------------------

        // заголовок страницы
        private static String page_title_caption = "Торговля, общественное питание и бытовое обслуживание";

        // заголовок для Grid1
        private static String grid1_title_caption = "Розничная торговля";
        // заголовок для Grid2
        private static String grid2_title_caption = "Бытовые услуги населению";

        // заголовок для Chatr1
        private static String chart1_title_caption = "Объекты розничной торговли в&nbsp;{0}&nbsp;году, ед.";
        // заголовок для Chatr2
        private static String chart2_title_caption = "Распределение площади объектов розничной торговли в&nbsp;{0}&nbsp;году, м<sup>2</sup>";
        // заголовок для Chatr3
        private static String chart3_title_caption = "Структура розничной торговли в&nbsp;{0}&nbsp;году, рубль";
        // заголовок для Chatr4
        private static String chart4_title_caption = "Распределение предприятий по видам оказываемых услуг в&nbsp;{0}&nbsp;году, %";
        // заголовок для Chatr5
        private static String chart5_title_caption = "Распределение ПБОЮЛ по видам оказываемых услуг в&nbsp;{0}&nbsp;году, %";
        // заголовок для Chatr6
        private static String chart6_title_caption = "Распределение объема оказанных услуг между МУП и прочими организациями в&nbsp;{0}&nbsp;году, %";

        // параметр для последней актуальной даты
        private CustomParam last_year { get { return (UserParams.CustomParam("last_year")); } }
        // параметр запроса для последней актуальной даты
        private CustomParam way_last_year { get { return (UserParams.CustomParam("way_last_year")); } }
        // параметр для выбранного/текущего года
        private CustomParam selected_year { get { return (UserParams.CustomParam("selected_year")); } }
        // параметр для выбранного/текущего года
        private CustomParam selected_year2 { get { return (UserParams.CustomParam("selected_year2")); } }
        // ширина экрана в пикселях
        private Int32 screen_width { get { return (int)Session["width_size"]; } }
        // сообщения об ошибке при некорректной загрузке данных для UltraChart
        private static String chart_error_message = "в настоящий момент данные отсутствуют";
        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        // --------------------------------------------------------------------
        string BN = "IE";
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
                BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
                base.Page_PreLoad(sender, e);
                // установка размера диаграммы
                web_grid1.Width = (int)((screen_width - 55));
                web_grid2.Width = (int)((screen_width - 55));

                UltraChart1.Width = (int)((screen_width - 55) * 0.67);
                UltraChart2.Width = (int)((screen_width - 55) * 0.67);
                UltraChart3.Width = (int)((screen_width - 55) * 0.33);
                UltraChart4.Width = (int)((screen_width - 55) * 0.33);
                UltraChart5.Width = (int)((screen_width - 55) * 0.33);
                UltraChart6.Width = (int)((screen_width - 55) * 0.33);

                Label1.Width = (int)((screen_width - 55) * 0.66);
                Label2.Width = (int)((screen_width - 55) * 0.66);
                Label3.Width = (int)((screen_width - 55) * 0.32);
                Label4.Width = (int)((screen_width - 55) * 0.32);
                Label5.Width = (int)((screen_width - 55) * 0.32);
                Label6.Width = (int)((screen_width - 55) * 0.32);

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
                    RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(web_grid1);
                    WebAsyncRefreshPanel3.AddLinkedRequestTrigger(web_grid2);

                    last_year.Value = getLastDate("СОЗДАНИЕ УСЛОВИЙ ДЛЯ ОБЕСПЕЧЕНИЯ НАСЕЛЕНИЯ УСЛУГАМИ ТОРГОВЛИ, ОБЩЕСТВЕННОГО ПИТАНИЯ И БЫТОВОГО ОБСЛУЖИВАНИЯ");
                    String year = UserComboBox.getLastBlock(last_year.Value);
                    page_title.Text = String.Format(page_title_caption, year);

                    // заполнение UltraWebGrid данными
                    web_grid1.DataBind();
                    web_grid1.Columns[web_grid1.Columns.Count - 1].Selected = true;
                    grid1Manual_ActiveCellChange(web_grid1.Columns.Count - 1);

                    web_grid2.DataBind();
                    web_grid2.Columns[web_grid2.Columns.Count - 1].Selected = true;
                    grid2Manual_ActiveCellChange(web_grid2.Columns.Count - 1);

                    // установка заголовков
                    Grid1Label.Text = String.Format(grid1_title_caption, year);
                    Grid2Label.Text = String.Format(grid2_title_caption, year);
                }
                if (BN == "FIREFOX")
                {
                    UltraChart3.Height = 429;
                    UltraChart6.Height = 389;
                }
                if (BN == "APPLEMAC-SAFARI")
                {
                    UltraChart3.Height = 434;
                    UltraChart6.Height = 389;
                }
            }
            catch (Exception ex)
            {
                // неудачная загрузка ...
                throw new Exception(ex.Message, ex);
            }
        }

        // --------------------------------------------------------------------

        /** <summary>
         *  Метод получения последней актуальной даты 
         *  </summary>
         */
        private String getLastDate(String way_ly)
        {
            try
            {
                way_last_year.Value = way_ly;
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("last_date"));
                return cs.Axes[1].Positions[0].Members[0].ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
                //return null;
            }
        }

        // --------------------------------------------------------------------

        protected void web_grid1_DataBinding(object sender, EventArgs e)
        {
            try
            {
                CellSet grid_set = null;
                DataTable grid_table = new DataTable();
                // Загрузка таблицы цен и товаров в CellSet
                grid_set = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid1"));
                // Добавление столбцов в таблицу и заполнение данных в DataTable
                grid_table.Columns.Add("Показатели");
                int columnsCount = grid_set.Cells.Count / grid_set.Axes[1].Positions.Count;
                for (int i = 0; i < columnsCount; ++i)
                {   // установка заголовков для столбцов UltraWebGrid
                    grid_table.Columns.Add((Convert.ToInt32(UserComboBox.getLastBlock(last_year.Value)) - columnsCount + i + 1).ToString());
                }
                foreach (Position pos in grid_set.Axes[1].Positions)
                {   // создание списка значений для строки UltraWebGrid
                    object[] values = new object[columnsCount + 1];
                    values[0] = grid_set.Axes[1].Positions[pos.Ordinal].Members[0].Caption + ", " + pos.Members[0].MemberProperties[0].Value.ToString().ToLower();
                    for (int i = 0; i < columnsCount; ++i)
                    {
                        values[i + 1] = grid_set.Cells[i, pos.Ordinal].FormattedValue.ToString();
                    }
                    // заполнение строки данными
                    grid_table.Rows.Add(values);
                }
                web_grid1.DataSource = grid_table.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                throw new Exception(exception.Message, exception);
            }
        }

        protected void web_grid1_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            try
            {
                // настройка столбцов
                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 0;
                e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth) * 0.35) - 5;
                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = (int)((tempWidth) * 0.65 / (e.Layout.Bands[0].Columns.Count - 1)) - 5;
                    e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                    // установка формата отображения данных в UltraWebGrid
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ### ###.##");
                }
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            }
            catch
            {
                // ошибка инициализации
            }
        }

        protected void web_grid1_ActiveCellChange(object sender, CellEventArgs e)
        {
            UltraGridCell cell = e.Cell;
            int CellIndex = cell.Column.Index;
            if (CellIndex > 0)
            {
                grid1Manual_ActiveCellChange(CellIndex);
            }
            else
            {
                grid1Manual_ActiveCellChange(1);
            }
        }

        /// <summary>
        /// Обновление данных для UltraChart при изменении активной ячейки в UltraWebGrid
        /// </summary>
        /// <param name="CellIndex">индекс активной ячейки</param>
        protected void grid1Manual_ActiveCellChange(int CellIndex)
        {
            try
            {
                web_grid1.Columns[CellIndex].Selected = true;
                selected_year.Value = web_grid1.Columns[CellIndex].Header.Key.ToString();


                UltraChart1.DataBind();
                UltraChart2.DataBind();
                UltraChart3.DataBind();
                Label1.Text = String.Format(chart1_title_caption, selected_year.Value);
                Label2.Text = String.Format(chart2_title_caption, selected_year.Value);
                Label3.Text = String.Format(chart3_title_caption, selected_year.Value);
            }
            catch
            {
            }
        }

        protected void web_grid1_Click(object sender, ClickEventArgs e)
        {
            int CellIndex = 0;
            try
            {
                CellIndex = e.Cell.Column.Index;
            }
            catch
            {
                CellIndex = e.Column.Index;
            }

            if (CellIndex > 0)
            {
                grid1Manual_ActiveCellChange(CellIndex);
            }
            else
                grid1Manual_ActiveCellChange(1);
        }


        // --------------------------------------------------------------------

        protected void web_grid2_DataBinding(object sender, EventArgs e)
        {
            try
            {
                CellSet grid_set = null;
                DataTable grid_table = new DataTable();
                // Загрузка таблицы цен и товаров в CellSet
                grid_set = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid2"));
                // Добавление столбцов в таблицу и заполнение данных в DataTable
                grid_table.Columns.Add("Показатели");
                int columnsCount = grid_set.Cells.Count / grid_set.Axes[1].Positions.Count;
                for (int i = 0; i < columnsCount; ++i)
                {   // установка заголовков для столбцов UltraWebGrid
                    grid_table.Columns.Add((Convert.ToInt32(UserComboBox.getLastBlock(last_year.Value)) - columnsCount + i + 1).ToString());
                }
                foreach (Position pos in grid_set.Axes[1].Positions)
                {   // создание списка значений для строки UltraWebGrid
                    object[] values = new object[columnsCount + 1];
                    values[0] = grid_set.Axes[1].Positions[pos.Ordinal].Members[0].Caption + ", " + pos.Members[0].MemberProperties[0].Value.ToString().ToLower();
                    for (int i = 0; i < columnsCount; ++i)
                    {
                        values[i + 1] = grid_set.Cells[i, pos.Ordinal].FormattedValue.ToString();
                    }
                    // заполнение строки данными
                    grid_table.Rows.Add(values);
                }
                web_grid2.DataSource = grid_table.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                throw new Exception(exception.Message, exception);
            }
        }

        protected void web_grid2_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            try
            {
                // настройка столбцов
                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 0;
                e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth) * 0.4) - 5;
                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = (int)((tempWidth) * 0.6 / (e.Layout.Bands[0].Columns.Count - 1)) - 5;
                    e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                    // установка формата отображения данных в UltraWebGrid
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ### ###.##");
                }
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            }
            catch
            {
                // ошибка инициализации
            }
        }

        protected void web_grid2_ActiveCellChange(object sender, CellEventArgs e)
        {
            UltraGridCell cell = e.Cell;
            int CellIndex = cell.Column.Index;
            if (CellIndex > 0)
            {
                grid2Manual_ActiveCellChange(CellIndex);
            }
            else
            {
                grid2Manual_ActiveCellChange(1);
            }
        }

        /// <summary>
        /// Обновление данных для UltraChart при изменении активной ячейки в UltraWebGrid
        /// </summary>
        /// <param name="CellIndex">индекс активной ячейки</param>
        protected void grid2Manual_ActiveCellChange(int CellIndex)
        {
            try
            {
                web_grid2.Columns[CellIndex].Selected = true;
                selected_year2.Value = web_grid2.Columns[CellIndex].Header.Key.ToString();
                

                UltraChart4.DataBind();
                UltraChart5.DataBind();
                UltraChart6.DataBind();
                Label4.Text = String.Format(chart4_title_caption, selected_year2.Value);
                Label5.Text = String.Format(chart5_title_caption, selected_year2.Value);
                Label6.Text = String.Format(chart6_title_caption, selected_year2.Value);
            }
            catch
            {
            }
        }

        protected void web_grid2_Click(object sender, ClickEventArgs e)
        {
            int CellIndex = 0;
            try
            {
                CellIndex = e.Cell.Column.Index;
            }
            catch
            {
                CellIndex = e.Column.Index;
            }

            if (CellIndex > 0)
            {
                grid2Manual_ActiveCellChange(CellIndex);
            }
            else
                grid2Manual_ActiveCellChange(1);

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


        protected void InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = chart_error_message;
            e.LabelStyle.FontSizeBestFit = false;
            e.LabelStyle.Font = new Font("Verdana", 30);
            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            UltraChart1.DataSource = dataBind("chart1");
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            UltraChart2.DataSource = dataBind("chart2");
        }

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {
            UltraChart3.DataSource = dataBind("chart3");
        }

        protected void UltraChart4_DataBinding(object sender, EventArgs e)
        {
            UltraChart4.DataSource = dataBind("chart4");
        }

        protected void UltraChart5_DataBinding(object sender, EventArgs e)
        {
            UltraChart5.DataSource = dataBind("chart5");
        }

        protected void UltraChart6_DataBinding(object sender, EventArgs e)
        {
            UltraChart6.DataSource = dataBind("chart6");
        }

    }
}

