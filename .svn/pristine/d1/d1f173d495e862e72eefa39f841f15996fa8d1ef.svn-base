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
namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_0010.LifeLevel
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

        private static String[] grid4_columns = { "Показатели", "Ед. изм.", "Значение" };
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
        private CustomParam LastReportUrl { get { return (UserParams.CustomParam("LastReportUrl")); } }
        private static String Descendant = "[Мониторинг__Дата подписания].[Мониторинг__Дата подписания].[Данные всех периодов].[{0}]";

        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam grid1Marks;
        private CustomParam grid2Marks;
        private CustomParam grid3Marks;
        private CustomParam chart1Mark;
        private CustomParam chart1Marks;
        private CustomParam chart2Mark;
        private CustomParam chart2Marks;
        private CustomParam chart3Mark;
        private CustomParam chart3Marks;
        private CustomParam chart4Marks;
        private CustomParam chart5Marks;
        private CustomParam chart6Marks;
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
                grid1Marks = UserParams.CustomParam("grid1Marks");
                grid2Marks = UserParams.CustomParam("grid2Marks");
                grid3Marks = UserParams.CustomParam("grid3Marks");
                chart1Mark = UserParams.CustomParam("chart1Mark");
                chart1Marks = UserParams.CustomParam("chart1Marks");
                chart2Mark = UserParams.CustomParam("chart2Mark");
                chart2Marks = UserParams.CustomParam("chart2Marks");
                chart3Mark = UserParams.CustomParam("chart3Mark");
                chart3Marks = UserParams.CustomParam("chart3Marks");
                chart4Marks = UserParams.CustomParam("chart4Marks");
                chart5Marks = UserParams.CustomParam("chart5Marks");
                chart6Marks = UserParams.CustomParam("chart6Marks");

            }
            catch (Exception)
            {
                // установка размеров не удалась ...
            }
        }

        // --------------------------------------------------------------------
        static public class ForMarks
        {

            public static ArrayList Getmarks(string prefix)
            {
                ArrayList AL = new ArrayList();
                string CurMarks = RegionSettingsHelper.Instance.GetPropertyValue(prefix + "1");
                int i = 2;
                while (!string.IsNullOrEmpty(CurMarks))
                {
                    AL.Add(CurMarks.ToString());

                    CurMarks = RegionSettingsHelper.Instance.GetPropertyValue(prefix + i.ToString());

                    i++;
                }

                return AL;
            }

            public static CustomParam SetMarks(CustomParam param, ArrayList AL, params bool[] clearParam)
            {
                if (clearParam.Length > 0 && clearParam[0]) { param.Value = ""; }
                int i;
                for (i = 0; i < AL.Count - 1; i++)
                {
                    param.Value += AL[i].ToString() + ",";
                }
                param.Value += AL[i].ToString();

                return param;
            }


        }
        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {   
                base.Page_Load(sender, e);               

                RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                if (!Page.IsPostBack)
                {   // опрерации которые должны выполняться при только первой загрузке страницы
                    
                    grid1Marks = ForMarks.SetMarks(grid1Marks, ForMarks.Getmarks("grid1_mark_"), true);
                    grid2Marks = ForMarks.SetMarks(grid2Marks, ForMarks.Getmarks("grid2_mark_"), true);
                    grid3Marks = ForMarks.SetMarks(grid3Marks, ForMarks.Getmarks("grid3_mark_"), true);
                    chart1Mark.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart1_mark_1");
                    chart1Marks=ForMarks.SetMarks(chart1Marks, ForMarks.Getmarks("chart1_marks_"), true);
                    chart2Mark.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart2_mark_1");
                    chart2Marks = ForMarks.SetMarks(chart2Marks, ForMarks.Getmarks("chart2_marks_"), true);
                    chart3Mark.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart3_mark_1");
                    chart3Marks = ForMarks.SetMarks(chart3Marks, ForMarks.Getmarks("chart3_marks_"), true);
                    chart4Marks = ForMarks.SetMarks(chart4Marks, ForMarks.Getmarks("chart4_mark_"), true);
                    chart5Marks = ForMarks.SetMarks(chart5Marks, ForMarks.Getmarks("chart5_mark_"), true);
                    chart6Marks = ForMarks.SetMarks(chart6Marks, ForMarks.Getmarks("chart6_mark_"), true);

                    last_year.Value = getLastDate();
                    String year = UserComboBox.getLastBlock(last_year.Value);
                    page_title.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("page_title"), year);
                    Page.Title = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("page_title"), year);
                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(web_grid);
                    WebAsyncRefreshPanel2.AddLinkedRequestTrigger(web_grid2);
                    WebAsyncRefreshPanel3.AddLinkedRequestTrigger(web_grid3);
                    WebAsyncRefreshPanel4.AddLinkedRequestTrigger(web_grid4);                    

                    // установка заголовка для таблицы
                    grid_caption.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("grid1_title"), year);
                    grid2_caption.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("grid2_title"), year);
                    grid3_caption.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("grid3_title"), year);
                    Label1.Text = RegionSettingsHelper.Instance.GetPropertyValue("grid4_title");
                    Label2.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart4_title");
                    Label3.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart5_title");
                    Label4.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart6_title");
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
                webGrid4ActiveRowChange(0, true);

                if (BN == "FIREFOX")
                {
                    web_grid.Height = 305;
                    web_grid2.Height = 276;
                    web_grid3.Height = 256;
                    web_grid4.Height = 358;
                }else
                    if (BN == "IE")
                    {
                        //web_grid.Height = 305;
                        //web_grid2.Height = 276;
                        //web_grid3.Height = 256;
                        web_grid4.Height = 349 - 14;
                    }
                    else
                    {
                        web_grid4.Height = 349 - 14;
                    }
                Page.Title = page_title.Text;
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
        DataTable Pdt = null;
        protected void web_grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            try
            {
                Pdt = Pdt!=null?Pdt:GetDSForChart("Pprop_");
                int StupidIndex = 0;
                if ((UltraWebGrid)(sender) == web_grid2) StupidIndex = 1;
                if ((UltraWebGrid)(sender) == web_grid3) StupidIndex = 2;

                e.Layout.Bands[0].Columns[1].Header.Caption = "Доход, " + Pdt.Rows[StupidIndex][1].ToString().ToLower();
                // настройка столбцов
                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 20 - 3;
                e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth - 20) * 0.54) - 5;
                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 20) * 0.4) - 5;
                 
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

        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "sadad");
            return dt;
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
            DataView dt = dataBind("chart1");
            dt.Table.Rows[0][0] = "Среднемесячный доход на 1 работающего в отрасли " + "''" + dt.Table.Rows[0][0].ToString() + "''";
            Chart1.DataSource = dt;
            Chart1.Tooltips.FormatString = "<SERIES_LABEL> в <ITEM_LABEL> году, "+Pdt.Rows[0][1].ToString().ToLower()+" <b><DATA_VALUE:###,##0.##></b>";
        }

        protected void InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e); 
           // ORG_0003_0001._default.setChartErrorFont(e);
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
                
            }
        }

        protected void Chart2_DataBinding1(object sender, EventArgs e)
        {
            DataView dt = dataBind("chart2");
            dt.Table.Rows[0][0] = "Среднемесячный доход на 1 работающего на муниципальных предприятиях в отрасли " + "''" + dt.Table.Rows[0][0].ToString() + "''";
            Chart2.DataSource = dt;
            Chart2.Tooltips.FormatString = "<SERIES_LABEL> в <ITEM_LABEL> году, " + Pdt.Rows[1][1].ToString().ToLower() + " <b><DATA_VALUE:###,##0.##></b>";
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
            DataView dt = dataBind("chart3");
            dt.Table.Rows[0][0] = "Среднемесячный доход на 1 работающего в бюджетной сфере в отрасли " + "''" + dt.Table.Rows[0][0].ToString() + "''";
            Chart3.DataSource = dt;
            Chart3.Tooltips.FormatString = "<SERIES_LABEL> в <ITEM_LABEL> году, " + Pdt.Rows[2][1].ToString().ToLower() + " <b><DATA_VALUE:###,##0.##></b>";
        }

        protected void web_grid4_InitializeLayout(object sender, LayoutEventArgs e)
        {
            try
            {
                // настройка столбцов
                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 20 - 3;
                e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth - 20)) - 10;
                if (BN == "FIREFOX")
                {
                    e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth - 20)) - 30;
                }                
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

            for (int i = 0; i < grid4_columns_.Length; i++)
                table.Rows.Add(grid4_columns_[i]);
            web_grid4.DataSource = table.DefaultView;

        }

        protected void web_grid4_ActiveRowChange(object sender, RowEventArgs e)
        {
            webGrid4ActiveRowChange(e.Row.Index, false);
        }
        string SetBR(string res)
        {
            for (int j = 1; j < res.Length / 30; j++)
            {
                int k;
                for (k = j * 30; k < res.Length; k++)
                {
                    if (res[k] == ' ')
                    {
                        break;
                    }
                }

                res = !(k == res.Length) ? res.Insert(k, "<br>") : res;
            }
            return res; 
        }

        protected void Grid4Chart1_DataBinding(object sender, EventArgs e)
        {
            DataTable table = new DataTable();
            try
            {
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid4Chart1"), "sfn", table);
            }
            catch (Exception)
            {
                table = null;
            }
            Label2.Text = Label2.Text.Split(',')[0] + ", " + table.Rows[0].ItemArray[table.Columns.Count - 1].ToString().ToLower();
            

            
            //table.Rows[0][0] = SetBR(table.Rows[0][0].ToString());

            table.Columns.Remove(table.Columns[table.Columns.Count - 1]);
            
            Grid4Chart1.DataSource = table.DefaultView;
               
            Grid4Chart1.Tooltips.FormatString = SetBR(current_way4.Value);
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
            Label3.Text = Label3.Text.Split(',')[0] + ", " + table.Rows[0].ItemArray[table.Columns.Count - 1].ToString().ToLower();
            table.Rows[0][0] = SetBR(table.Rows[0][0].ToString());

            table.Columns.Remove(table.Columns[table.Columns.Count - 1]); 
            Grid4Chart2.DataSource = table.DefaultView;
            Grid4Chart2.Tooltips.FormatString = SetBR(current_way4.Value);
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
            Label4.Text = Label4.Text.Split(',')[0] + ", " + table.Rows[0].ItemArray[table.Columns.Count - 1].ToString().ToLower();
            table.Rows[0][0] = SetBR(table.Rows[0][0].ToString());
            table.Columns.Remove(table.Columns[table.Columns.Count - 1]); 
            Grid4Chart3.DataSource = table.DefaultView;
            Grid4Chart3.Tooltips.FormatString = SetBR(current_way4.Value);
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
            //int xOct = 0;
            //int xNov = 0;
            //Text decText = null;
            //int year = int.Parse(UserComboBox.getLastBlock(last_year.Value));
            //String year1 = (year - 1).ToString();
            //String year2 = (year - 2).ToString();


            //foreach (Primitive primitive in e.SceneGraph)
            //{
            //    if (primitive is Text)
            //    {
            //        Text text = primitive as Text;

            //        if (year2 == text.GetTextString()){
            //            xOct = text.bounds.X;
            //            continue;
            //        }
            //        if (year1 == text.GetTextString())
            //        {
            //            xNov = text.bounds.X;
            //            decText = new Text();
            //            decText.bounds = text.bounds;
            //            decText.labelStyle = text.labelStyle;
            //            continue;
            //        }
            //    }
            //    if (decText != null)
            //    {
            //        decText.bounds.X = xNov + (xNov - xOct);
            //        decText.SetTextString(year.ToString());
            //        e.SceneGraph.Add(decText);
            //        break;
            //    }
            //}
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Session.Add("LastOpenReport", "LifeLevel");
            Response.Redirect("../OverallTable/default.aspx");
        }

        protected void LinkButton1_Command(object sender, CommandEventArgs e)
        {
            Session.Add("LastOpenReport", "LifeLevel");
            Response.Redirect("../OverallTable/default.aspx");
        }

        protected void LinkButton1_Click1(object sender, EventArgs e)
        {
            //Response.Redirect("../OverallTable/default.aspx");
            Response.Redirect(((Button)sender).PostBackUrl);
        }


    }

}

