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
 *  Транспорт и связь.
 */
namespace Krista.FM.Server.Dashboards.reports.BC_0001_0020
{
 
    public partial class Default : CustomReportPage
    {
        // --------------------------------------------------------------------

        // заголовок страницы
        private static String page_title_caption = "Транспорт и связь";
        // текст отчёта
        private static String report_text = "расположение МО город Губкинский характеризуется следующими значениями показателей:<br>1. Удаленность от ближайшей ж/д станции&nbsp;-&nbsp;<b>{0}</b>&nbsp;км.<br>2.	Удаленность от ближайшего аэропорта&nbsp;-&nbsp;<b>{1}</b>&nbsp;км.";

        // заголовок для Grid1
        private static String grid1_title_caption = "Основные показатели по транспорту и связи";
        // заголовок для Grid2
        private static String grid2_title_caption = "Наличие транспортных средств и стоянок для них";
        // заголовок для Grid3
        private static String grid3_title_caption = "Объём оказанных услуг предприятиями связи, штуки";
        // заголовок для Grid4
        private static String grid4_title_caption = "Число основных телефонных аппаратов телефонной сети общего пользования или имеющих на неё выход, штук";

        // заголовок для Chatr1
        private static String chart1_title_caption = "Динамика показателя «{0}»";
        // заголовок для Chatr2
        private static String chart2_title_caption = "Наличие транспортных средств по видам транспорта (предприятия) на&nbsp;{0}&nbsp;год, %";
        // заголовок для Chatr3
        private static String chart3_title_caption = "Наличие транспортных средств, находящихся в собственности индивидуальных владельцев, на&nbsp;{0}&nbsp;год, %";
        // заголовок для Chatr4
        private static String chart4_title_caption = "Площадь стоянок для транспортных средств на&nbsp;{0}&nbsp;год, %";
        // заголовок для Chatr5
        private static String chart5_title_caption = "Входящий обмен при оказании основных услуг предприятиями связи в&nbsp;{0}&nbsp;году, %";
        // заголовок для Chatr6
        private static String chart6_title_caption = "Исходящий обмен при оказании основных услуг предприятиями связи в&nbsp;{0}&nbsp;году, %";
        // заголовок для Chatr7
        private static String chart7_title_caption = "«Оказание основных услуг предприятиями связи», штуки";

        // заголовок для Gauge1
        private static String gauge1_title_caption = "«{0}» (темп прироста)";


        // параметр для выбранного текущего способа
        private CustomParam current_way1 { get { return (UserParams.CustomParam("current_way1")); } }
        // параметр для выбранного текущего способа
        private CustomParam current_way2 { get { return (UserParams.CustomParam("current_way2")); } }
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
                web_grid1.Width = (int)((screen_width - 55) * 0.33);
                web_grid2.Width = (int)((screen_width - 55));
                web_grid3.Width = (int)((screen_width - 55) * 0.33);
                web_grid4.Width = (int)((screen_width - 55) * 0.67);

                UltraChart1.Width = (int)((screen_width - 55) * 0.67);
                UltraChart2.Width = (int)((screen_width - 55) * 0.33);
                UltraChart3.Width = (int)((screen_width - 55) * 0.33);
                UltraChart4.Width = (int)((screen_width - 55) * 0.33);
                UltraChart5.Width = (int)((screen_width - 55) * 0.33);
                UltraChart6.Width = (int)((screen_width - 55) * 0.33);

                Grid1Label.Width = (int)((screen_width - 55) * 0.32);
                Grid2Label.Width = (int)((screen_width - 55) * 0.32);

                Label2.Width = (int)((screen_width - 55) * 0.32);
                Label3.Width = (int)((screen_width - 55) * 0.32);
                Label4.Width = (int)((screen_width - 55) * 0.32);
                Label5.Width = (int)((screen_width - 55) * 0.32);
                Label6.Width = (int)((screen_width - 55) * 0.32);
                Label7.Width = (int)((screen_width - 55) * 0.32);

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
                    WebAsyncRefreshPanel2.AddLinkedRequestTrigger(web_grid1);
                    WebAsyncRefreshPanel3.AddLinkedRequestTrigger(web_grid2);
                    WebAsyncRefreshPanel6.AddLinkedRequestTrigger(web_grid3);
                    WebAsyncRefreshPanel6.AddRefreshTarget(UltraChart5);
                    WebAsyncRefreshPanel6.AddRefreshTarget(Label5);
                    //WebAsyncRefreshPanel1.AddLinkedRequestTrigger(web_grid3);
                    WebAsyncRefreshPanel4.AddLinkedRequestTrigger(web_grid4);
                    Label9.Text = "Структурная динамика показателя";
                    last_year.Value = getLastDate("ТРАНСПОРТНОЕ ОБСЛУЖИВАНИЕ И ОБЕСПЕЧЕНИЕ УСЛУГАМИ СВЯЗИ НАСЕЛЕНИЯ, ПРЕДПРИЯТИЙ, ОРГАНИЗАЦИЙ И УЧРЕЖДЕНИЙ");
                    String year = UserComboBox.getLastBlock(last_year.Value);
                    page_title.Text = String.Format(page_title_caption, year);

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
                        int[] tempArray = new int[table.Rows.Count];
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            currentArray = table.Rows[i].ItemArray;
                            if (currentArray[1].ToString() == "")
                                tempArray[i] = 0;
                            else
                                tempArray[i] = Convert.ToInt32(currentArray[1]);
                        }
                        Label8.Text = String.Format("По данным на {0} год:", year);
                       
                        ReportText.Text = String.Format(report_text,
                                                        tempArray[0].ToString(),
                                                        tempArray[1].ToString());

                    }
                    else ReportText.Text = "в настоящий момент данные отсутствуют!";


                    // заполнение UltraWebGrid данными
                    web_grid1.DataBind();
                    webGrid1ActiveRowChange(0, true);

                    web_grid2.DataBind();
                    web_grid2.Columns[web_grid2.Columns.Count - 1].Selected = true;
                    grid2Manual_ActiveCellChange(web_grid2.Columns.Count - 1);

                    web_grid3.DataBind();
                    webGrid3ActiveRowChange(web_grid3.Rows.Count - 1, true);

                    web_grid4.DataBind();
                    webGrid4ActiveRowChange(0, true);

                    UltraChart7.DataBind();

                    // установка заголовков
                    Grid1Label.Text = String.Format(grid1_title_caption, year);
                    Grid2Label.Text = String.Format(grid2_title_caption, year);
                    Grid3Label.Text = String.Format(grid3_title_caption, year);
                    Grid4Label.Text = String.Format(grid4_title_caption, year);

                    Label7.Text = String.Format(chart7_title_caption, year);
                    
                }
                if (BN=="APPLEMAC-SAFARI")
                {   
                    UltraChart2.Height=367;
                    UltraChart4.Height=367;
                    UltraChart7.Height=380;
                }
                if (BN == "FIREFOX")
                {
                    UltraChart2.Height = 367;
                    UltraChart4.Height = 367;
                    UltraChart7.Height = 374;
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
                DataTable grid_master = new DataTable();
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid1"));

                grid_master.Columns.Add();
                grid_master.Columns.Add("Показатель");
                grid_master.Columns.Add("Значение");

                foreach (Position pos in cs.Axes[1].Positions)
                {   // создание списка значений для строки UltraWebGrid
                    object[] values = {
                        pos.Members[0].MemberProperties[0].Value.ToString(),
                        cs.Axes[1].Positions[pos.Ordinal].Members[0].Caption + ", " + cs.Cells[1, pos.Ordinal].Value.ToString().ToLower(),
                        string.Format("{0:N0}", cs.Cells[0, pos.Ordinal].Value)
                    };
                    // заполнение строки данными
                    grid_master.Rows.Add(values);
                }
                web_grid1.DataSource = grid_master.DefaultView;
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
                e.Layout.Bands[0].Columns[0].Hidden = true;

                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 20 - 3;
                e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 20) * 0.78) - 5;

                e.Layout.Bands[0].Columns[2].Width = (int)((tempWidth - 20) * 0.22) - 5;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "### ### ### ##0");
                e.Layout.Bands[0].Columns[2].Header.Style.HorizontalAlign = HorizontalAlign.Center;

            }
            catch
            {
                // ошибка инициализации
            }
        }

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
                
                UltraChart1.DataBind();
                Label1.Text = String.Format(chart1_title_caption, row.Cells[1].Value.ToString());
            }
            catch (Exception)
            {
                // выбор сторки пошел с ошибкой ...
            }
        }

        protected void web_grid1_ActiveRowChange(object sender, RowEventArgs e)
        {
            webGrid1ActiveRowChange(e.Row.Index, false);
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
                selected_year.Value = web_grid2.Columns[CellIndex].Header.Key.ToString();
                

                UltraChart2.DataBind();
                UltraChart3.DataBind();
                UltraChart4.DataBind();
                Label2.Text = String.Format(chart2_title_caption, selected_year.Value);
                Label3.Text = String.Format(chart3_title_caption, selected_year.Value);
                Label4.Text = String.Format(chart4_title_caption, selected_year.Value);
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

/*
            int CellIndex = e.Column.Index;
            if (CellIndex > 0)
            {
                grid2Manual_ActiveCellChange(CellIndex);
            }
            else
            {
                grid2Manual_ActiveCellChange(1);
            }
 */
        }

        // --------------------------------------------------------------------


        protected void web_grid4_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid4"), "Показатель", grid_master);
                web_grid4.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                throw new Exception(exception.Message, exception);
            }
        }

        protected void web_grid4_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            try
            {
                // настройка столбцов
                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 20 - 3;
                e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth - 20) * 0.4) - 5;
                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = (int)((tempWidth - 20) * 0.6 / (e.Layout.Bands[0].Columns.Count - 1)) - 5;
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

        private void GaugeSet(LinearGauge gauge, int maxValue, int minValue, Double value)
        {

            gauge.Scales[0].Axis.SetEndValue(maxValue);
            gauge.Scales[0].Axis.SetStartValue(minValue);
            gauge.Scales[0].Markers[0].Value = value;
            int result = 0;
            if ((maxValue < 0) & (minValue < 0))
                result = -minValue + maxValue;
            else
                if (minValue < 0)
                    result = -minValue + maxValue;
                else
                    result = maxValue - minValue;


            gauge.Scales[0].Labels.Frequency = 5;
            gauge.Scales[0].Labels.FormatString = "<DATA_VALUE:##0> %";
            gauge.Scales[0].MajorTickmarks.Frequency = 5;
            gauge.Scales[0].MinorTickmarks.Frequency = 1;
            //gauge.Scales[0].MajorTickmarks.EndWidth = 10;
            //gauge.Scales[0].MajorTickmarks.EndExtent = 40;

        }

        private void webGrid4ActiveRowChange(int index, bool active)
        {
            try
            {
                // получаем выбранную строку
                UltraGridRow row = web_grid4.Rows[index];
                // устанавливаем ее активной, если необходимо
                if (active)
                {
                    row.Activate();
                    row.Activated = true;
                    row.Selected = true;
                }
                // получение заголовка выбранной отрасли
                current_way2.Value = row.Cells[0].Value.ToString();

                Double[] percentsArr = new Double [row.Cells.Count - 2];
                Double maxPercent = -100;
                Double minPercent = 100;
                int min = 0;
                int max = 0;

                for (int i = 0; i < row.Cells.Count - 2; i++)
                {
                    percentsArr[i] = Convert.ToDouble(row.Cells[i+2].Value) / Convert.ToDouble(row.Cells[i + 1].Value) * 100 - 100;
                    if (percentsArr[i] > maxPercent)
                        maxPercent = percentsArr[i];
                    if (percentsArr[i] < minPercent)
                        minPercent = percentsArr[i];
                }
/*
                if (maxPercent < 0)
                    max = Convert.ToInt32((maxPercent * 0.9) / 10 + 1);
                else
                    max = Convert.ToInt32((maxPercent * 1.1) / 10 + 1);
                if (minPercent < 0)
                    min = Convert.ToInt32((minPercent * 1.1) / 10 - 1);
                else
                    min = Convert.ToInt32((minPercent * 0.9) / 10 - 1);
 */
                if (maxPercent < 0)
                    max = Convert.ToInt32((maxPercent) / 10 + 1);
                else
                    max = Convert.ToInt32((maxPercent) / 10 + 1);
                if (minPercent < 0)
                    min = Convert.ToInt32((minPercent) / 10 - 1);
                else
                    min = Convert.ToInt32((minPercent) / 10 - 1);
                max = max * 10;
                min = min * 10;

                gauge1_caption.Text = String.Format(gauge1_title_caption, current_way2.Value);

                GaugeSet(((LinearGauge)UltraGauge1.Gauges[0]), max, min, percentsArr[0]);
                GaugeSet(((LinearGauge)UltraGauge2.Gauges[0]), max, min, percentsArr[1]);
                GaugeSet(((LinearGauge)UltraGauge3.Gauges[0]), max, min, percentsArr[2]);
                GaugeSet(((LinearGauge)UltraGauge4.Gauges[0]), max, min, percentsArr[3]);
                GaugeSet(((LinearGauge)UltraGauge5.Gauges[0]), max, min, percentsArr[4]);

                String year = UserComboBox.getLastBlock(last_year.Value);
                Gauge1Label.Text = (Convert.ToInt16(year) - 4) + " год<br>(" + String.Format("{0:N2}", percentsArr[0]) + " %)";
                Gauge2Label.Text = (Convert.ToInt16(year) - 3) + " год<br>(" + String.Format("{0:N2}", percentsArr[1]) + " %)";
                Gauge3Label.Text = (Convert.ToInt16(year) - 2) + " год<br>(" + String.Format("{0:N2}", percentsArr[2]) + " %)";
                Gauge4Label.Text = (Convert.ToInt16(year) - 1) + " год<br>(" + String.Format("{0:N2}", percentsArr[3]) + " %)";
                Gauge5Label.Text = (Convert.ToInt16(year) - 0) + " год<br>(" + String.Format("{0:N2}", percentsArr[4]) + " %)";

                //UltraChart1.DataBind();
                //Label1.Text = String.Format(chart1_title_caption, row.Cells[1].Value.ToString());
            }
            catch (Exception)
            {
                // выбор сторки пошел с ошибкой ...
            }
        }



        protected void web_grid4_ActiveRowChange(object sender, RowEventArgs e)
        {
            webGrid4ActiveRowChange(e.Row.Index, false);
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

        // --------------------------------------------------------------------

        protected void web_grid3_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid3"), "Год", grid_master);
                web_grid3.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                throw new Exception(exception.Message, exception);
            }
        }

        private void webGrid3ActiveRowChange(int index, bool active)
        {
            try
            {
                // получаем выбранную строку
                UltraGridRow row = web_grid3.Rows[index];
                // устанавливаем ее активной, если необходимо
                if (active)
                {
                    row.Activate();
                    row.Activated = true;
                    row.Selected = true;
                }
                // получение заголовка выбранной отрасли
                selected_year2.Value = row.Cells[0].Value.ToString();
                UltraChart5.DataBind();
                UltraChart6.DataBind();
                Label5.Text = String.Format(chart5_title_caption, selected_year2.Value);
                Label6.Text = String.Format(chart6_title_caption, selected_year2.Value);
            }
            catch (Exception)
            {
                // выбор сторки пошел с ошибкой ...
            }
        }

        protected void web_grid3_ActiveRowChange(object sender, RowEventArgs e)
        {
            webGrid3ActiveRowChange(e.Row.Index, false);
        }


        protected void web_grid3_InitializeLayout(object sender, LayoutEventArgs e)
        {
            try
            {
                // настройка столбцов
                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 20 - 3;
                e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth - 20) * 0.14) - 5;
                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 20) * 0.43) - 5;
                e.Layout.Bands[0].Columns[2].Width = (int)((tempWidth - 20) * 0.43) - 5;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "### ### ### ##0");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "### ### ### ##0");
                e.Layout.Bands[0].Columns[1].Header.Style.Wrap = true;
                e.Layout.Bands[0].Columns[2].Header.Style.Wrap = true;
            }
            catch
            {
                // ошибка инициализации
            }
        }
        // --------------------------------------------------------------------


        protected void InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = chart_error_message;
            e.LabelStyle.Font = new Font("Verdana", 16);           
            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart1"), "sfn", dt);

            float min, max;
            min = max = float.Parse(dt.Rows[0].ItemArray[1].ToString());
            for (int i = 1; i < dt.Rows[0].ItemArray.Length; i++)
            {
                if (float.Parse(dt.Rows[0].ItemArray[i].ToString()) < min)
                {
                    min = float.Parse(dt.Rows[0].ItemArray[i].ToString());
                }
                if (float.Parse(dt.Rows[0].ItemArray[i].ToString()) > max)
                {
                    max = float.Parse(dt.Rows[0].ItemArray[i].ToString());
                }
            }

            UltraChart1.Axis.Y.RangeMax = max * 1.1;
            UltraChart1.Axis.Y.RangeMin = min * 0.8;

            UltraChart1.DataSource = dt.DefaultView;

            //UltraChart1.DataSource = dataBind("chart1");
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

        protected void UltraChart7_DataBinding(object sender, EventArgs e)
        {
            UltraChart7.DataSource = dataBind("chart7");
        }

        protected void UltraChart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            int xOct = 0;
            int xNov = 0;
            Infragistics.UltraChart.Core.Primitives.Text decText = null;
            int year = int.Parse(UserComboBox.getLastBlock(last_year.Value));
            String year1 = (year - 1).ToString();
            String year2 = (year - 2).ToString();


            foreach (Infragistics.UltraChart.Core.Primitives.Primitive primitive in e.SceneGraph)
            {
                if (primitive is Infragistics.UltraChart.Core.Primitives.Text)
                {
                    Infragistics.UltraChart.Core.Primitives.Text text = primitive as Infragistics.UltraChart.Core.Primitives.Text;

                    if (year2 == text.GetTextString())
                    {
                        xOct = text.bounds.X;
                        continue;
                    }
                    if (year1 == text.GetTextString())
                    {
                        xNov = text.bounds.X;
                        decText = new Infragistics.UltraChart.Core.Primitives.Text();
                        decText.bounds = text.bounds;
                        decText.labelStyle = text.labelStyle;
                        continue;
                    }
                }
                if (decText != null)
                {
                    decText.bounds.X = e.ChartCore.GridLayerBounds.Width + e.ChartCore.GridLayerBounds.X - decText.bounds.Width;
                    //decText.bounds.X = xNov + (xNov - xOct);
                    decText.SetTextString(year.ToString());
                    e.SceneGraph.Add(decText);
                    break;
                }
            }
        }

    }
}

