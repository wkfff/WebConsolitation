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
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;

/**
 *  Производственная деятельность организаций.
 */
namespace Krista.FM.Server.Dashboards.reports.BC_0001_0007
{
    public partial class Default : CustomReportPage
    {
        // --------------------------------------------------------------------

        // заголовок страницы
        private static String page_title_caption = "Производственная деятельность организаций";
        // заголовок для Grid
        private static String grid_title_caption = "Основные показатели производственной деятельности";
        // заголовок для Grid1
        private static String grid1_title_caption = "Число предприятий по отраслям в&nbsp;{0}&nbsp;году, единиц";
        // заголовок для Grid2
        private static String grid2_title_caption = "Отгружено товаров и услуг в фактических ценах в&nbsp;{0}&nbsp;году, рубль";
        // заголовок для Grid3
        private static String grid3_title_caption = "Отгружено товаров и услуг в фактических ценах МУП в&nbsp;{0}&nbsp;году, рубль";
        // заголовок для Grid4
        private static String grid4_title_caption = "Среднесписочная численность работающих на предприятиях в&nbsp;{0}&nbsp;году, человек";
        // заголовок для Grid5
        private static String grid5_title_caption = "Среднесписочная численность работающих на МУП в&nbsp;{0}&nbsp;году, человек";
        // заголовок для Chatr1
        private static String chart1_title_caption = "Доля отраслей в общем числе предприятий в {0} году";
        // заголовок для Chatr2
        private static String chart2_title_caption = "Доля отраслей в объеме отгруженных товаров в {0} году";
        // заголовок для Chatr3
        private static String chart3_title_caption = "Доля отраслей в объеме отгруженных товаров МУП в {0} году";
        // заголовок для Chatr4
        private static String chart4_title_caption = "Распределение численности работающих на предприятиях по отраслям в {0} году";
        // заголовок для Chatr5
        private static String chart5_title_caption = "Распределение численности работающих на МУП  по отраслям в {0} году";
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
        // параметр для выбранного столбца
        private CustomParam selected_cell_index { get { return (UserParams.CustomParam("selected_cell_index")); } }
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
                                                    "Возмещение расходов по оплате проезда детям-сиротам и детям, оставшимся без попечения родителей , учащимся и студентам, многодетным и одиноким матерям, лицам, получающим пенсию по СПК",
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
                web_grid.Width = (int)((screen_width - 55) );
                web_grid1.Width = (int)((screen_width - 55) * 0.4);
                web_grid2.Width = (int)((screen_width - 55) * 0.4);
                web_grid3.Width = (int)((screen_width - 55) * 0.4);
                web_grid4.Width = (int)((screen_width - 55) * 0.4);
                web_grid5.Width = (int)((screen_width - 55) * 0.4);
                
                Grid1Lab.Width = (int)((screen_width - 55) * 0.4) - 20;
                Grid2Lab.Width = (int)((screen_width - 55) * 0.4) - 20;
                Grid3Lab.Width = (int)((screen_width - 55) * 0.4) - 20;
                Grid4Lab.Width = (int)((screen_width - 55) * 0.4) - 20;
                Grid5Lab.Width = (int)((screen_width - 55) * 0.4) - 20;
                 
                Chart1.Width = (int)((screen_width - 55) * 0.6);
                Chart2.Width = (int)((screen_width - 55) * 0.6);
                Chart3.Width = (int)((screen_width - 55) * 0.6);
                Chart4.Width = (int)((screen_width - 55) * 0.6);
                Chart5.Width = (int)((screen_width - 55) * 0.6);

                WebAsyncRefreshPanel1.AddLinkedRequestTrigger(web_grid);

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
                    
                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(web_grid);

                    //ORG_0003_0001._default.setChartSettings(Chart1);
                    //ORG_0003_0001._default.setChartSettings(Chart2);
                    //ORG_0003_0001._default.setChartSettings(Chart3);
                    //ORG_0003_0001._default.setChartSettings(Chart4);
                    last_year.Value = getLastDate();
                    String year = UserComboBox.getLastBlock(last_year.Value);
                    page_title.Text = String.Format(page_title_caption, year);
                    
                    //DropDownList1.Items.Add(year - 2);

                    // установка заголовка для таблицы
                    grid_caption.Text = String.Format(grid_title_caption, year);
                }
                
                // заполнение UltraWebGrid данными
                web_grid.DataBind();
                web_grid.Columns[web_grid.Columns.Count-1].Selected = true;
                gridManual_ActiveCellChange(web_grid.Columns.Count - 1);
                //web_grid2.DataBind();
                //web_grid3.DataBind();
                //web_grid4.DataBind();

                // установка активной строки в UltraWebGrid
                //webGridActiveRowChange(0, true);
                //webGrid2ActiveRowChange(0, true);
                //webGrid3ActiveRowChange(0, true);
                //webGrid4ActiveRowChange(1, true);
                
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
                CellSet grid_set = null;
                DataTable grid_table = new DataTable();
                // Загрузка таблицы цен и товаров в CellSet
                grid_set = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid"));
                // Добавление столбцов в таблицу и заполнение данных в DataTable
                grid_table.Columns.Add("Отрасли");
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
                web_grid.DataSource = grid_table.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                throw new Exception(exception.Message, exception);
            }


/*
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid"), "Отрасли", grid_master);
                web_grid.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
*/
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
        
        private void webGridActiveRowChange(int index, bool active)
        {
            try
            {
                // получаем выбранную строку
                UltraGridRow row = web_grid.Rows[index];
                // устанавливаем ее активной, если необходимо
                if (active) row.Activate();
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
                if (active) row.Activate();
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
                if (active) row.Activate();
                // получение заголовка выбранной отрасли
                current_way3.Value = row.Cells[0].Value.ToString();
                Chart3.DataBind();
            }
            catch (Exception)
            {
                // выбор сторки пошел с ошибкой ...
            }
        }



        protected void chart1_Init(object sender, EventArgs e)
        {

        }


        protected void web_grid_ActiveRowChange(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            webGridActiveRowChange(e.Row.Index, false);
        }

        protected void Chart1_DataBinding1(object sender, EventArgs e)
        {
            Chart1.DataSource = dataBind("grid1");
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
                e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth - 20)) - 5;

                e.Layout.ColHeadersVisibleDefault = ShowMarginInfo.No;
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            }
            catch
            {
                // ошибка инициализации
            }
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

        /// <summary>
        /// Обновление данных для UltraChart при изменении активной ячейки в UltraWebGrid
        /// </summary>
        /// <param name="CellIndex">индекс активной ячейки</param>
        protected void gridManual_ActiveCellChange(int CellIndex)
        {
            web_grid.Columns[CellIndex].Selected = true;
            selected_year.Value = web_grid.Columns[CellIndex].Header.Key.ToString();

            Grid1Lab.Text = String.Format(grid1_title_caption, selected_year.Value);
            Grid2Lab.Text = String.Format(grid2_title_caption, selected_year.Value);
            Grid3Lab.Text = String.Format(grid3_title_caption, selected_year.Value);
            Grid4Lab.Text = String.Format(grid4_title_caption, selected_year.Value);
            Grid5Lab.Text = String.Format(grid5_title_caption, selected_year.Value);
            Chart1Lab.Text = String.Format(chart1_title_caption, selected_year.Value);
            Chart2Lab.Text = String.Format(chart2_title_caption, selected_year.Value);
            Chart3Lab.Text = String.Format(chart3_title_caption, selected_year.Value);
            Chart4Lab.Text = String.Format(chart4_title_caption, selected_year.Value);
            Chart5Lab.Text = String.Format(chart5_title_caption, selected_year.Value);
            web_grid1.DataBind();
            web_grid2.DataBind();
            web_grid3.DataBind();
            web_grid4.DataBind();
            web_grid5.DataBind();
            Chart1.DataBind();
            Chart2.DataBind();
            Chart3.DataBind();
            Chart4.DataBind();
            Chart5.DataBind();
        }

        protected void web_grid1_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid1"), "Отрасли", grid_master);
                web_grid1.DataSource = grid_master.DefaultView;
                Chart1.DataSource = grid_master.DefaultView;
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
                gridManual_ActiveCellChange(CellIndex);
            }
            else
                gridManual_ActiveCellChange(1);
        }

        protected void web_grid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            try
            {
                // настройка столбцов
                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 0;
                e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth ) * 0.75) - 5;
                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth ) * 0.25) - 5;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "### ### ### ###.##");
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
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

        protected void web_grid3_DataBinding1(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid3"), "Отрасли", grid_master);
                web_grid3.DataSource = grid_master.DefaultView;
                Chart3.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }

        protected void web_grid4_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid4"), "Отрасли", grid_master);
                web_grid4.DataSource = grid_master.DefaultView;
                Chart4.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }

        protected void web_grid5_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid5"), "Отрасли", grid_master);
                web_grid5.DataSource = grid_master.DefaultView;
                Chart5.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }





    }

}

