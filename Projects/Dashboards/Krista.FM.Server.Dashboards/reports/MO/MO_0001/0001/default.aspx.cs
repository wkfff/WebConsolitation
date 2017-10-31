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
 *  Характеристика территории МО РФ.
 */
namespace Krista.FM.Server.Dashboards.reports.BC_0001_0001
{
    public partial class Default : CustomReportPage
    {
        // --------------------------------------------------------------------

        // заголовок страницы
        private static String page_title_caption = "Характеристика территории МО РФ";
        // текст отчёта
        private static String report_text = "{0}1. Общая площадь муниципального образования – <b>{1}</b> гектар.<br>2. Площадь муниципального образования, предназначенная для строительства – <b>{2}</b> гектар, в том числе<br>&nbsp;&nbsp;&nbsp;&nbsp;жилые здания – <b>{3}</b> гектар;<br>&nbsp;&nbsp;&nbsp;&nbsp;объекты социально-культурного назначения – <b>{4}</b> гектар.<br>3. Площадь дорог (улиц, проездов, набережных) – <b>{5}</b> гектар.<br>4. Площадь незастроенных территорий – <b>{6}</b> гектар, в том числе:<br>&nbsp;&nbsp;&nbsp;&nbsp;свободные земли, пригодные для застройки – <b>{7}</b> гектар;<br>&nbsp;&nbsp;&nbsp;&nbsp;лесные угодья – <b>{8}</b> гектар;<br>&nbsp;&nbsp;&nbsp;&nbsp;площади под древесно-кустарниковой растительностью, не входящие в лесной фонд – <b>{9}</b> гектар;<br>&nbsp;&nbsp;&nbsp;&nbsp;болота и площади под водой – <b>{10}</b> гектар;<br>&nbsp;&nbsp;&nbsp;&nbsp;зеленые насаждения общего пользования – <b>{11}</b> гектар.<br>5. Общая площадь проезжей части улиц, проездов, набережных – <b>{12}</b> квадратных метров.<br>6. Площадь свободных земель, пригодных для застройки – <b>{13}</b> гектар.";
        // заголовок для Grid
        private static String grid_title_caption = "Основные показатели, характеризующие территорию МО";
        // заголовок для Grid1
        private static String grid1_title_caption = "Муниципальный земельный контроль и регулирование застройки территорий в&nbsp;{0}&nbsp;году";
        // заголовок для Grid2
        private static String grid2_title_caption = "Площадь застроенных земель, гектар";
        // заголовок для Chatr1
        private static String chart1_title_caption = "Площадь земель в черте поселений, входящих в&nbsp;состав муниципального образования, на&nbsp;{0}&nbsp;год, гектар";
        // заголовок для Chatr2
        private static String chart2_title_caption = "Протяженность автомобильных дорог на&nbsp;{0}&nbsp;год, километры";
        // заголовок для Chatr3
        private static String chart3_title_caption = "Количество инвентарного жилья на&nbsp;{0}&nbsp;год, тысячи&nbsp;м<sup>2</sup>";

        // заголовок для Chart_1
        private static String chart_1_title_caption = "«{0}»";
        // заголовок для Chart_1
        private static String chart_2_title_caption = "Площадь застроенных земель на&nbsp;{0}&nbsp;год, гектар";
        // заголовок для Chart_1
        private static String chart_3_title_caption = "Площадь застроенных земель под непроизводственные объекты на&nbsp;{0}&nbsp;год, гектар";

        // параметр для выбранного текущего способа
        private CustomParam current_way1 { get { return (UserParams.CustomParam("current_way1" )); } }
        // параметр для выбранного текущего способа
        private CustomParam current_way2 { get { return (UserParams.CustomParam("current_way2")); } }
        // параметр для последней актуальной даты
        private CustomParam last_year { get { return (UserParams.CustomParam("last_year")); } }
        // параметр запроса для последней актуальной даты
        private CustomParam way_last_year { get { return (UserParams.CustomParam("way_last_year")); } }
        // параметр для выбранного/текущего года
        private CustomParam selected_year { get { return (UserParams.CustomParam("selected_year")); } }
        // параметр для выбранного столбца
        private CustomParam selected_cell_index { get { return (UserParams.CustomParam("selected_cell_index")); } }
        // ширина экрана в пикселях
        private Int32 screen_width { get { return (int)Session["width_size"]; } }

        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }

        // --------------------------------------------------------------------

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                base.Page_PreLoad(sender, e);
                // установка размера диаграммы
                web_grid.Width = (int)((screen_width - 55) );

                UltraChart1.Width = (int)((screen_width - 55) * 0.33);
                UltraChart2.Width = (int)((screen_width - 55) * 0.33);
                UltraChart3.Width = (int)((screen_width - 55) * 0.33);
                Label1.Width = (int)((screen_width - 55) * 0.32);
                Label2.Width = (int)((screen_width - 55) * 0.32);
                Label3.Width = (int)((screen_width - 55) * 0.32);

                web_grid1.Width = (int)((screen_width - 70
                    ) * 0.33);
                web_grid2.Width = (int)((screen_width - 50) * 0.33);

                //Grid1Label.Width = (int)((screen_width - 55) * 0.32);
                Grid2Label.Width = (int)((screen_width - 55) * 0.32);

                Chart1.Width = (int)((screen_width - 55) * 0.66);
                //Chart1.Height = Ga
                Chart2.Width = (int)((screen_width - 55) * 0.33);
                Chart3.Width = (int)((screen_width - 55) * 0.33);


                
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

                    WebAsyncRefreshPanel2.AddLinkedRequestTrigger(web_grid);
                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(web_grid1);
                    WebAsyncRefreshPanel3.AddLinkedRequestTrigger(web_grid2);
                    //ORG_0003_0001._default.setChartSettings(Chart1);
                    //ORG_0003_0001._default.setChartSettings(Chart2);
                    //ORG_0003_0001._default.setChartSettings(Chart3);
                    //ORG_0003_0001._default.setChartSettings(Chart4);
                    last_year.Value = getLastDate("ТЕРРИТОРИЯ  МУНИЦИПАЛЬНОГО ОБРАЗОВАНИЯ");
                    String year = UserComboBox.getLastBlock(last_year.Value);
                    //page_title.Text = page_title_caption;

                    DataTable table = new DataTable();
                    try
                    {
                        DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("text"), "sfn", table);
                    }
                    catch (Exception)
                    {
                        table = null;
                    }
                    if (table != null)
                    {
                        object[] currentArray = new object[2];
                        object[] tempArray = new object[table.Rows.Count];
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            currentArray = table.Rows[i].ItemArray;
                            tempArray[i] = currentArray[1];
                        }
                        WebPanel1.Header.Text = String.Format("По данным на {0} год:", year);
                        ReportText.Text = String.Format(report_text, "",
                                                        tempArray[0].ToString(),
                                                        tempArray[1].ToString(),
                                                        tempArray[2].ToString(),
                                                        tempArray[3].ToString(),
                                                        tempArray[4].ToString(),
                                                        tempArray[5].ToString(),
                                                        tempArray[6].ToString(),
                                                        tempArray[7].ToString(),
                                                        tempArray[8].ToString(),
                                                        tempArray[9].ToString(),
                                                        tempArray[10].ToString(),
                                                        tempArray[11].ToString(),
                                                        tempArray[12].ToString());
                            
                    }
                    else ReportText.Text = "в настоящий момент данные отсутствуют!";
                    //DropDownList1.Items.Add(year - 2);
                    // установка заголовка для таблицы
                    grid_caption.Text = String.Format(grid_title_caption, year);

                    // заполнение UltraWebGrid данными
                    web_grid.DataBind();
                    web_grid.Columns[web_grid.Columns.Count - 1].Selected = true;
                    selected_cell_index.Value = (web_grid.Columns.Count - 1).ToString();
                    gridManual_ActiveCellChange(web_grid.Columns.Count - 1);

                    last_year.Value = getLastDate("МУНИЦИПАЛЬНЫЙ ЗЕМЕЛЬНЫЙ КОНТРОЛЬ");
                    String year_ = UserComboBox.getLastBlock(last_year.Value);
                    Grid1Label.Text = String.Format(grid1_title_caption, year_);
                    web_grid1.DataBind();
                    webGrid1ActiveRowChange(0, true);

                    last_year.Value = getLastDate("РЕГУЛИРОВАНИЕ ПЛАНИРОВКИ И ЗАСТРОЙКИ ТЕРРИТОРИЙ");
                    Grid2Label.Text = grid2_title_caption;
                    web_grid2.DataBind();
                    webGrid2ActiveRowChange(web_grid2.Rows.Count - 1, true);                  

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

        protected void web_grid_DataBinding(object sender, EventArgs e)
        {
            try
            {
                CellSet grid_set = null;
                DataTable grid_table = new DataTable();
                // Загрузка таблицы цен и товаров в CellSet
                grid_set = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid"));
                // Добавление столбцов в таблицу и заполнение данных в DataTable
                grid_table.Columns.Add("Показатели");
                int columnsCount = grid_set.Cells.Count / grid_set.Axes[1].Positions.Count;
                for (int i = 0; i < columnsCount; ++i)
                {   // установка заголовков для столбцов UltraWebGrid
                    grid_table.Columns.Add((Convert.ToInt32(UserComboBox.getLastBlock(last_year.Value)) - columnsCount + i + 1).ToString());
                }
                foreach (Position pos in grid_set.Axes[1].Positions)
                {   // создание списка значений для строки UltraWebGrid
                    object[] values = new object[columnsCount+1];
                    values[0] = grid_set.Axes[1].Positions[pos.Ordinal].Members[0].Caption + ", " + pos.Members[0].MemberProperties[0].Value.ToString().ToLower();
                    for (int i = 0; i < columnsCount; ++i)
                    {
                        values[i+1] = grid_set.Cells[i, pos.Ordinal].FormattedValue.ToString();
                    }
                    // заполнение строки данными
                    grid_table.Rows.Add(values);
                }
                web_grid.DataSource = grid_table.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                throw new Exception(exception.Message, exception);
            }
        }

        protected void web_grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            try
            {
                // настройка столбцов
                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 0;
//                e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth - 20) * 0.6) - 5;
//                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 20) * 0.4) - 5;
                e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth) * 0.4) - 5;
                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = (int)((tempWidth) * 0.6 / (e.Layout.Bands[0].Columns.Count - 1)) - 5;
                    e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                    // установка формата отображения данных в UltraWebGrid
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ### ###.##");
                }

//                e.Layout.Bands[0].Columns[1].Header.Caption = "Доход, р.";
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
//                e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
                // установка формата отображения ячеек
//                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "### ### ### ###.00");
            }
            catch
            {
                // ошибка инициализации
            }
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
         *  Установка активной сторки элемента UltraWebGrid
         *  params: 
         *      index - выбранная строка;
         *      active - установить или нет свойство 'Active' у выбранной строки.
         *  </summary>         
         */
        
        private void webGrid1ActiveRowChange(int index, bool active)
        {
            try
            {
                // получаем выбранную строку
                UltraGridRow row = web_grid1.Rows[index];
                // устанавливаем ее активной, если необходимо
                if (active)
                {
                    row.Activate();
                    row.Activated = true;
                    row.Selected = true;
                }
                // получение заголовка выбранной отрасли
                current_way1.Value = row.Cells[0].Value.ToString();
                Chart1.DataBind();
                Chart1Lab.Text = String.Format(chart_1_title_caption, row.Cells[1].Value.ToString());
            }
            catch (Exception)
            {
                // выбор сторки пошел с ошибкой ...
            }
        }

        private void webGrid2ActiveRowChange(int index, bool active)
        {
            try
            {
                // получаем выбранную строку
                UltraGridRow row = web_grid2.Rows[index];
                // устанавливаем ее активной, если необходимо
                if (active)
                {
                    row.Activate();
                    row.Activated = true;
                    row.Selected = true;
                }
                // получение заголовка выбранной отрасли
                current_way2.Value = row.Cells[0].Value.ToString();
                Chart2.DataBind();
                Chart3.DataBind();
                Chart2Lab.Text = String.Format(chart_2_title_caption, row.Cells[0].Value.ToString());
                Chart3Lab.Text = String.Format(chart_3_title_caption, row.Cells[0].Value.ToString());
            }
            catch (Exception)
            {
                // выбор сторки пошел с ошибкой ...
            }
        }

        protected void Chart1_DataBinding1(object sender, EventArgs e)
        {
            Chart1.DataSource = dataBind("grid1");
        }

        protected void InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
//            ORG_0003_0001._default.setChartErrorFont(e);
        }

        protected void web_grid2_ActiveRowChange(object sender, RowEventArgs e)
        {
            webGrid2ActiveRowChange(e.Row.Index, false);
        }

        protected void web_grid2_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid2"), "Год", grid_master);
                web_grid2.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }

        protected void Chart2_DataBinding1(object sender, EventArgs e)
        {
            Chart2.DataSource = dataBind("chart2");
        }

        protected void Chart2_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }

        /// <summary>
        /// Обновление данных для UltraChart при изменении активной ячейки в UltraWebGrid
        /// </summary>
        /// <param name="CellIndex">индекс активной ячейки</param>
        protected void gridManual_ActiveCellChange(int CellIndex)
        {
            web_grid.Columns[CellIndex].Selected = true;
            selected_year.Value = web_grid.Columns[CellIndex].Header.Key.ToString();

            UltraChart1.DataBind();
            UltraChart2.DataBind();
            UltraChart3.DataBind();
            Label1.Text = String.Format(chart1_title_caption, selected_year.Value);
            Label2.Text = String.Format(chart2_title_caption, selected_year.Value);
            Label3.Text = String.Format(chart3_title_caption, selected_year.Value);
        }

        protected void web_grid1_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                //DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid1"), "Показатель", grid_master);
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid1"));

                grid_master.Columns.Add();
                grid_master.Columns.Add("Показатель");
                grid_master.Columns.Add("Значение");

                foreach (Position pos in cs.Axes[1].Positions)
                {   // создание списка значений для строки UltraWebGrid
                    object[] values = {
                        pos.Members[0].MemberProperties[0].Value.ToString(),
                        cs.Axes[1].Positions[pos.Ordinal].Members[0].Caption + ", " + cs.Cells[1, pos.Ordinal].Value.ToString().ToLower(),
                        cs.Cells[0, pos.Ordinal].Value
                    };
                    // заполнение строки данными
                    grid_master.Rows.Add(values);
                }
/*
                for (int i = 0; i < cs.Axes[1].Positions.Count; i++)
                {
                    object[] tempArray = new object[3];
                    tempArray[0] = cs.Axes[1].Positions[i].Members[0].ToString();
                    tempArray[1] = cs.Axes[1].Positions[i].Members[1].ToString();
                    tempArray[2] = cs.Axes[1].Positions[i].Members[2].ToString();
                    grid_master.Rows.Add(tempArray);
                }
*/               

                web_grid1.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }

        protected void web_grid_Click(object sender, ClickEventArgs e)
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
                selected_cell_index.Value = CellIndex.ToString();
                gridManual_ActiveCellChange(CellIndex);
            }
            else
                gridManual_ActiveCellChange(int.Parse(selected_cell_index.Value));
            
        }

        protected void web_grid_ActiveCellChange(object sender, CellEventArgs e)
        {
            UltraGridCell cell = e.Cell;
            int CellIndex = cell.Column.Index;
            if (CellIndex > 0)
            {
                selected_cell_index.Value = CellIndex.ToString();
                gridManual_ActiveCellChange(CellIndex);
            }
            else
                gridManual_ActiveCellChange(int.Parse(selected_cell_index.Value));
        }

        protected void web_grid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            try
            {
                // настройка столбцов
                e.Layout.Bands[0].Columns[0].Hidden = true;
                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 20 - 3;
                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 20) * 0.78) - 5;
                e.Layout.Bands[0].Columns[2].Width = (int)((tempWidth - 20) * 0.22) - 5;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "### ### ### ##0.##");
                e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            }
            catch
            {
                // ошибка инициализации
            }
        }

        protected void web_grid2_DataBinding1(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid2"), "Отрасли", grid_master);
                web_grid2.DataSource = grid_master.DefaultView;
                Chart2.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }




        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart1"), "_", grid_master);
                UltraChart1.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart2"), "_", grid_master);
                UltraChart2.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart3"), "_", grid_master);
                UltraChart3.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }

        protected void web_grid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            try
            {
                // настройка столбцов
                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 20 - 3;
                e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth - 20) * 0.3) - 5;
                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 20) * 0.7) - 5;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "### ### ### ##0.##");
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            }
            catch
            {
                // ошибка инициализации
            }
        }

        protected void Chart1_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart_1"), "_", grid_master);
                Chart1.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }

        protected void web_grid1_ActiveRowChange(object sender, RowEventArgs e)
        {
            if (e.Row != null)
                webGrid1ActiveRowChange(e.Row.Index, false);
        }

        protected void web_grid2_ActiveRowChange1(object sender, RowEventArgs e)
        {
            webGrid2ActiveRowChange(e.Row.Index, false);
        }

        protected void Chart2_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart_2"), "_", grid_master);
                Chart2.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }

        protected void Chart3_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart_3"), "_", grid_master);
                Chart3.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }

        protected void web_grid_Click1(object sender, ClickEventArgs e)
        {
            int CellIndex = e.Cell.Column.Index;
        }








    }

}

