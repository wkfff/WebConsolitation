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
using Infragistics.UltraChart.Core.Primitives;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;

/**
 *  Уровень жизни населения.
 */
namespace Krista.FM.Server.Dashboards.reports.BC_0001_0005
{
    public partial class Default : CustomReportPage
    {
        // --------------------------------------------------------------------

        // заголовок страницы
        private static String page_title_caption = "Уровень жизни населения на {0} год";
        // заголовок для Grid
        private static String grid_title_caption = "Среднемесячный доход на одного работающего (по отраслям) в {0} году";
        // заголовок для Grid2
        private static String grid2_title_caption = "Среднемесячный доход на одного работающего муниципальных предприятий (по отраслям) в {0} году";
        // заголовок для Grid2
        private static String grid3_title_caption = "Среднемесячный доход на одного работающего в бюджетной сфере (по отраслям) в {0} году";
        // параметр для выбранного текущего способа
        private CustomParam current_way { get { return (UserParams.CustomParam("current_way")); } }
        // параметр для выбранного текущего способа
        private CustomParam current_way2 { get { return (UserParams.CustomParam("current_way2")); } }
        // параметр для выбранного текущего способа
        private CustomParam current_way3 { get { return (UserParams.CustomParam("current_way3")); } }
        // параметр для выбранного текущего способа
        private CustomParam current_way4 { get { return (UserParams.CustomParam("current_way4")); } }
        // параметр для последней актуальной даты
        private CustomParam last_year { get { return (UserParams.CustomParam("last_year")); } }
        // параметр для выбранного/текущего года
        private CustomParam selected_year { get { return (UserParams.CustomParam("selected_year")); } }
        // ширина экрана в пикселях
        private Int32 screen_width { get { return (int)Session["width_size"]; } }

        // заголовки столбцов для UltraWebGrid
        private static String[] grid_columns = { "Показатели", "Ед. изм.", "Значение" };

        private static String[] grid4_columns = {   "Доплаты до прожиточного минимума",
                                                    "Адресная социальная помощь",
                                                    "Возмещение расходов на продукты питания беременным женщинам и кормящим матерям",
                                                    "Частичная компенсация стоимости продуктов питания",
                                                    "Денежные компенсации расходов на продукты питания студентам",
                                                    "Социальная стипендия (детям из малообеспеченных семей)",
                                                    "Компенсация расходов на питание м/о учащимся школ",
                                                    "Возмещение расходов по оплате проезда детям-сиротам и детям, оставшимся без попечения родителей, учащимся и студентам, многодетным и одиноким матерям, лицам, получающим пенсию по СПК",
                                                    "Частичная компенсация родительской платы в детских дошкольных учреждениях",
                                                    "Ежемесячное пособие многодетным семьям",
                                                    "Материальная помощь к памятным датам истории, выплата мат.помощи",
                                                    "Социальное пособие на погребение"};
        private static String grid4_columns1 = "[Показатели].[ВедСтат_Свод].[Все показатели].[УРОВЕНЬ ЖИЗНИ НАСЕЛЕНИЯ].[Социальная поддержка малообеспеченных слоёв населения и льготной категории граждан].[Количество получателей пособий].[{0}]";
        private static String grid4_columns2 = "[Показатели].[ВедСтат_Свод].[Все показатели].[УРОВЕНЬ ЖИЗНИ НАСЕЛЕНИЯ].[Социальная поддержка малообеспеченных слоёв населения и льготной категории граждан].[Средняя сумма доплат на одного получателя].[{0}]";
        private static String grid4_columns3 = "[Показатели].[ВедСтат_Свод].[Все показатели].[УРОВЕНЬ ЖИЗНИ НАСЕЛЕНИЯ].[Социальная поддержка малообеспеченных слоёв населения и льготной категории граждан].[Объем выплаченных пособий].[{0}]";
        

        private static String[] grid4_columns_ = {  "Доплаты до прожиточного минимума",
                                                    "Адресная социальная помощь (адр.соц.помощь, нат.помощь, единовременная соц.помощь)",
                                                    "Возмещение расходов на продукты питания беременным женщинам и кормящим матерям",
                                                    "Частичная компенсация стоимости продуктов питания",
                                                    "Денежные компенсации расходов на продукты питания студентам",
                                                    "Социальная стипендия (детям из малообеспеченных семей)",
                                                    "Компенсация расходов на питание м/о учащимся школ",
                                                    "Возмещение расходов по оплате проезда детям-сиротам и детям, оставшимся без попечения родителей, учащимся и студентам, многодетным и одиноким матерям, лицам, получающим пенсию по СПК",
                                                    "Частичная компенсация родительской платы в детских дошкольных учреждениях",
                                                    "Ежемесячное пособие многодетным семьям",
                                                    "Материальная помощь к памятным датам истории, выплата мат.помощи",
                                                    "Социальное пособие на погребение" };
        /*
                private static String[] grid4_columns2 = { 
                                                            "[Показатели].[ВедСтат_Свод].[Все показатели].[УРОВЕНЬ ЖИЗНИ НАСЕЛЕНИЯ].[Социальная поддержка малообеспеченных слоёв населения и льготной категории граждан].[Средняя сумма доплат на одного получателя].[Доплаты до прожиточного минимума]",
                                                            "[Показатели].[ВедСтат_Свод].[Все показатели].[УРОВЕНЬ ЖИЗНИ НАСЕЛЕНИЯ].[Социальная поддержка малообеспеченных слоёв населения и льготной категории граждан].[Средняя сумма доплат на одного получателя].[Адресная социальная помощь (адр.соц.помощь, нат.помощь, единовременная соц.помощь)]",
                                                            "[Показатели].[ВедСтат_Свод].[Все показатели].[УРОВЕНЬ ЖИЗНИ НАСЕЛЕНИЯ].[Социальная поддержка малообеспеченных слоёв населения и льготной категории граждан].[Средняя сумма доплат на одного получателя].[Возмещение расходов на продукты питания беременным женщинам и кормящим матерям]",
                                                            "[Показатели].[ВедСтат_Свод].[Все показатели].[УРОВЕНЬ ЖИЗНИ НАСЕЛЕНИЯ].[Социальная поддержка малообеспеченных слоёв населения и льготной категории граждан].[Средняя сумма доплат на одного получателя].[Частичная компенсация стоимости продуктов питания]",
                                                            "[Показатели].[ВедСтат_Свод].[Все показатели].[УРОВЕНЬ ЖИЗНИ НАСЕЛЕНИЯ].[Социальная поддержка малообеспеченных слоёв населения и льготной категории граждан].[Средняя сумма доплат на одного получателя].[Денежные компенсации расходов на продукты питания студентам]",
                                                            "[Показатели].[ВедСтат_Свод].[Все показатели].[УРОВЕНЬ ЖИЗНИ НАСЕЛЕНИЯ].[Социальная поддержка малообеспеченных слоёв населения и льготной категории граждан].[Средняя сумма доплат на одного получателя].[Социальная стипендия (детям из малообеспеченных семей)]",
                                                            "[Показатели].[ВедСтат_Свод].[Все показатели].[УРОВЕНЬ ЖИЗНИ НАСЕЛЕНИЯ].[Социальная поддержка малообеспеченных слоёв населения и льготной категории граждан].[Средняя сумма доплат на одного получателя].[Компенсация расходов на питание м/о учащимся школ]",
                                                            "[Показатели].[ВедСтат_Свод].[Все показатели].[УРОВЕНЬ ЖИЗНИ НАСЕЛЕНИЯ].[Социальная поддержка малообеспеченных слоёв населения и льготной категории граждан].[Средняя сумма доплат на одного получателя].[Возмещение расходов по оплате проезда детям-сиротам и детям, оставшимся без попечения родителей , учащимся и студентам, многодетным и одиноким матерям, лицам, получающим пенсию по СПК]",
                                                            "[Показатели].[ВедСтат_Свод].[Все показатели].[УРОВЕНЬ ЖИЗНИ НАСЕЛЕНИЯ].[Социальная поддержка малообеспеченных слоёв населения и льготной категории граждан].[Средняя сумма доплат на одного получателя].[Частичная компенсация родительской платы в детских дошкольных учреждениях]",
                                                            "[Показатели].[ВедСтат_Свод].[Все показатели].[УРОВЕНЬ ЖИЗНИ НАСЕЛЕНИЯ].[Социальная поддержка малообеспеченных слоёв населения и льготной категории граждан].[Средняя сумма доплат на одного получателя].[Ежемесячное пособие многодетным семьям]",
                                                            "[Показатели].[ВедСтат_Свод].[Все показатели].[УРОВЕНЬ ЖИЗНИ НАСЕЛЕНИЯ].[Социальная поддержка малообеспеченных слоёв населения и льготной категории граждан].[Средняя сумма доплат на одного получателя].[Материальная помощь к памятным датам истории, выплата мат.помощи]",
                                                            "[Показатели].[ВедСтат_Свод].[Все показатели].[УРОВЕНЬ ЖИЗНИ НАСЕЛЕНИЯ].[Социальная поддержка малообеспеченных слоёв населения и льготной категории граждан].[Средняя сумма доплат на одного получателя].[Социальное пособие на погребение])" };
        */

        // параметр для последних трёх лет
        private CustomParam Descendants { get { return (UserParams.CustomParam("Descendants")); } }
        private static String Descendant = "[Мониторинг__Дата подписания].[Мониторинг__Дата подписания].[Данные всех периодов].[{0}]";

        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }	        
        // --------------------------------------------------------------------

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                base.Page_PreLoad(sender, e);
                // установка размера диаграммы
                web_grid.Width = (int)((screen_width - 55) * 0.25);
                web_grid2.Width = (int)((screen_width - 55) * 0.25);
                web_grid3.Width = (int)((screen_width - 55) * 0.25);
                web_grid4.Width = (int)((screen_width - 55) * 0.25);
                Chart1.Width = (int)((screen_width - 55) * 0.75);
                Chart2.Width = (int)((screen_width - 55) * 0.75);
                Chart3.Width = (int)((screen_width - 55) * 0.75);
                Grid4Chart1.Width = (int)((screen_width - 55) * 0.24);
                Grid4Chart2.Width = (int)((screen_width - 55) * 0.24);
                Grid4Chart3.Width = (int)((screen_width - 55) * 0.24);

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
                    
                    //ORG_0003_0001._default.setChartSettings(Chart1);
                    //ORG_0003_0001._default.setChartSettings(Chart2);
                    //ORG_0003_0001._default.setChartSettings(Chart3);
                    //ORG_0003_0001._default.setChartSettings(Chart4);

                    last_year.Value = getLastDate();
                    String year = UserComboBox.getLastBlock(last_year.Value);
                    page_title.Text = String.Format(page_title_caption, year);

                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(web_grid);
                    WebAsyncRefreshPanel2.AddLinkedRequestTrigger(web_grid2);
                    WebAsyncRefreshPanel3.AddLinkedRequestTrigger(web_grid3);
                    WebAsyncRefreshPanel4.AddLinkedRequestTrigger(web_grid4);                    

                    //DropDownList1.Items.Add(year - 2);

                    // установка заголовка для таблицы
                    grid_caption.Text = String.Format(grid_title_caption, year);
                    grid2_caption.Text = String.Format(grid2_title_caption, year);
                    grid3_caption.Text = String.Format(grid3_title_caption, year);
                }
                
                // заполнение UltraWebGrid данными
                web_grid.DataBind();
                web_grid2.DataBind();
                web_grid3.DataBind();
                web_grid4.DataBind();

                // установка активной строки в UltraWebGrid
                webGridActiveRowChange(0, true);
                webGrid2ActiveRowChange(0, true);
                webGrid3ActiveRowChange(0, true);
                webGrid4ActiveRowChange(1, true);
                
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

        protected void web_grid_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid1"), "Отрасли", grid_master);
                web_grid.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }

        protected void web_grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            try
            {
                // настройка столбцов
                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 20 - 3;
                e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth - 20) * 0.6) - 5;
                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 20) * 0.4) - 5;
                e.Layout.Bands[0].Columns[1].Header.Caption = "Доход, р.";
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
                // установка формата отображения ячеек
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "### ### ### ###.00");
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
        
        private void webGridActiveRowChange(int index, bool active)
        {
            try
            {
                // получаем выбранную строку
                UltraGridRow row = web_grid.Rows[index];
                // устанавливаем ее активной, если необходимо
                if (active)
                {
                    row.Activate();
                    row.Activated = true;
                    row.Selected = true;
                }
                // получение заголовка выбранной отрасли
                current_way.Value = row.Cells[0].Value.ToString();
                Chart1.DataBind();
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
            }
            catch (Exception)
            {
                // выбор сторки пошел с ошибкой ...
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
                current_way3.Value = row.Cells[0].Value.ToString();
                Chart3.DataBind();
            }
            catch (Exception)
            {
                // выбор сторки пошел с ошибкой ...
            }
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
                //current_way4.Value = row.Cells[0].Value.ToString();
                current_way4.Value = grid4_columns_[index];
                
                Grid4Chart1.DataBind();
                Grid4Chart2.DataBind();
                Grid4Chart3.DataBind();
            }
            catch (Exception)
            {
                // выбор сторки пошел с ошибкой ...
            }
        }
        // --------------------------------------------------------------------


        protected void chart1_Init(object sender, EventArgs e)
        {

        }


        protected void web_grid_ActiveRowChange(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            if ( e != null)
            {
                try
                {
                    webGridActiveRowChange(e.Row.Index, false);
                }
                catch { }
            }
        }

        protected void Chart1_DataBinding1(object sender, EventArgs e)
        {
            Chart1.DataSource = dataBind("chart1");
        }

        protected void InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            ORG_0003_0001._default.setChartErrorFont(e);
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
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid2"), "Отрасли", grid_master);
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

        protected void web_grid3_ActiveRowChange(object sender, RowEventArgs e)
        {
            webGrid3ActiveRowChange(e.Row.Index, false);
        }

        protected void web_grid3_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid3"), "Отрасли", grid_master);
                web_grid3.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }

        protected void Chart3_DataBinding(object sender, EventArgs e)
        {
            Chart3.DataSource = dataBind("chart3");
        }

        protected void web_grid4_InitializeLayout(object sender, LayoutEventArgs e)
        {
            try
            {
                // настройка столбцов
                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 20 - 3;
                e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth - 20)) - 10;

                //e.Layout.ColHeadersVisibleDefault = ShowMarginInfo.No;
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            }
            catch
            {
                // ошибка инициализации
            }
        }

        protected void web_grid4_DataBinding(object sender, EventArgs e)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Показатели");
            //web_grid4.Columns.Add("1");
            //web_grid4.Columns.Add("2");
            //web_grid4.Rows.Add("123");
            //web_grid4.Rows.Add("456");
            for (int i = 0; i < grid4_columns.Length; i++)
                table.Rows.Add(grid4_columns[i]);
            web_grid4.DataSource = table.DefaultView;
        }

        protected void web_grid4_ActiveRowChange(object sender, RowEventArgs e)
        {
            webGrid4ActiveRowChange(e.Row.Index, false);
        }

        protected void Grid4Chart1_DataBinding(object sender, EventArgs e)
        {
            //Grid4Chart1.DataSource = dataBind("Grid4Chart1");
            DataTable table = new DataTable();
            try
            {
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid4Chart1"), "sfn", table);
            }
            catch (Exception)
            {
                table = null;
            }
            Grid4Chart1.DataSource = table.DefaultView;
            /*
            if (table.Columns.Count < 2)
            {
                Grid4Chart1.Visible = false;
                Label2.Visible = false;
            }
            else
            {
                Grid4Chart1.DataSource = table.DefaultView;
                Grid4Chart1.Visible = true;
                Label2.Visible = true;
            }
            */
        }

        protected void Grid4Chart2_DataBinding(object sender, EventArgs e)
        {
            //Grid4Chart2.DataSource = dataBind("Grid4Chart2");
            DataTable table = new DataTable();
            try
            {
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid4Chart2"), "sfn", table);
            }
            catch (Exception)
            {
                table = null;
            }
            Grid4Chart2.DataSource = table.DefaultView;
            /*
            if (table.Columns.Count < 2)
            {
                Grid4Chart2.Visible = false;
                Label3.Visible = false;
            }
            else
            {
                Grid4Chart2.DataSource = table.DefaultView;
                Grid4Chart2.Visible = true;
                Label3.Visible = true;
            }
            */
        }

        protected void Grid4Chart3_DataBinding(object sender, EventArgs e)
        {
            //Grid4Chart3.DataSource = dataBind("Grid4Chart3");
            DataTable table = new DataTable();
            try
            {
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid4Chart3"), "sfn", table);
            }
            catch (Exception)
            {
                table = null;
            }
            Grid4Chart3.DataSource = table.DefaultView;
            /*
            if (table.Columns.Count < 2)
            {
                Grid4Chart3.Visible = false;
                Label4.Visible = false;
            }
            else
            {
                Grid4Chart3.DataSource = table.DefaultView;
                Grid4Chart3.Visible = true;
                Label4.Visible = true;
            }
             */
        }

        protected void Chart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            int xOct = 0;
            int xNov = 0;
            Text decText = null;
            int year = int.Parse(UserComboBox.getLastBlock(last_year.Value));
            String year1 = (year - 1).ToString();
            String year2 = (year - 2).ToString();


            foreach (Primitive primitive in e.SceneGraph)
            {
                if (primitive is Text)
                {
                    Text text = primitive as Text;

                    if (year2 == text.GetTextString()){
                        xOct = text.bounds.X;
                        continue;
                    }
                    if (year1 == text.GetTextString())
                    {
                        xNov = text.bounds.X;
                        decText = new Text();
                        decText.bounds = text.bounds;
                        decText.labelStyle = text.labelStyle;
                        continue;
                    }
                }
                if (decText != null)
                {
                    decText.bounds.X = xNov + (xNov - xOct);
                    decText.SetTextString(year.ToString());
                    e.SceneGraph.Add(decText);
                    break;
                }
            }
        }


    }

}

