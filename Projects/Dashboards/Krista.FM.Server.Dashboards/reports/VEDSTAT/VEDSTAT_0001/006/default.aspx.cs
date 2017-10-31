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
 *  Социальная поддержка и обеспечение занятости.
 */
namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._006
{
    public partial class Default : CustomReportPage
    {
        // --------------------------------------------------------------------

        // заголовок страницы
        private static String page_title_caption = "Социальная поддержка и содействие занятости населения";
        // заголовок для Grid1
        private static String grid1_title_caption = "Численность населения, нуждающегося в социальной поддержке, человек";
        // заголовок для Grid2
        private static String grid2_title_caption = "Малообеспеченные семьи";
        // заголовок для Grid3
        private static String grid3_title_caption = "Занятость населения в&nbsp;{0}&nbsp;году";
        // заголовок для Grid4
        private static String grid4_title_caption = "Субсидии на оплату ЖКХ в&nbsp;{0}&nbsp;году";
        // заголовок для Chatr1
        private static String chart1_title_caption = "Численность пенсионеров в&nbsp;{0}&nbsp;году, человек";
        // заголовок для Chatr2
        private static String chart2_title_caption = "Численность не работающих пенсионеров в&nbsp;{0}&nbsp;году, человек";
        // заголовок для Chatr3
        private static String chart3_title_caption = "Численность инвалидов в&nbsp;{0}&nbsp;году, человек";
        // заголовок для Chatr4
        private static String chart4_title_caption = "Малообеспеченные семьи <br>(по видам) в&nbsp;{0}&nbsp;году";
        // заголовок для Chatr5
        private static String chart5_title_caption = "Численность детей в малообеспеченных<br>семьях (по видам) в&nbsp;{0}&nbsp;году";
        // заголовок для Chatr6
        private static String chart6_title_caption = "{0}";
        // заголовок для Chatr7
        private static String chart7_title_caption = "{0}";

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

        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam grid1Marks;
        private CustomParam grid2Marks;
        private CustomParam grid3Marks;
        private CustomParam grid4Marks;
        private CustomParam chart1Marks;
        private CustomParam chart2Marks;
        private CustomParam chart3Marks;
        private CustomParam chart4Marks;
        private CustomParam chart5Marks;
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
                web_grid2.Width = (int)((screen_width - 55) * 0.33);
                web_grid3.Width = (int)((screen_width - 55) * 0.33);
                web_grid4.Width = (int)((screen_width - 55) * 0.33);

                Chart1.Width = (int)((screen_width - 55));
                Chart2.Width = (int)((screen_width - 55));
                Chart3.Width = (int)((screen_width - 55));
                Chart4.Width = (int)((screen_width - 55) * 0.332);
                Chart5.Width = (int)((screen_width - 55) * 0.332);
                Chart6.Width = (int)((screen_width - 55) * 0.67);
                Chart7.Width = (int)((screen_width - 55) * 0.67);
                chart4_caption.Width = Chart4.Width;
                chart5_caption.Width = Chart5.Width;
                if (BN == "FIREFOX")
                {
                    web_grid2.Height = 420;
                    web_grid3.Height = 323;
                    web_grid4.Height = 323;
                    Chart4.Width = (int)((screen_width - 55) * 0.33);
                    Chart5.Width = (int)((screen_width - 55) * 0.33);
                }
                
                WebAsyncRefreshPanel6.AddLinkedRequestTrigger(web_grid2);
                WebAsyncRefreshPanel8.AddLinkedRequestTrigger(web_grid3);
                WebAsyncRefreshPanel2.AddLinkedRequestTrigger(web_grid1);
                WebAsyncRefreshPanel9.AddLinkedRequestTrigger(web_grid4);
                //WebAsyncRefreshPanel4.AddLinkedRequestTrigger(web_grid4);
                grid1Marks = UserParams.CustomParam("grid1Marks");
                grid2Marks = UserParams.CustomParam("grid2Marks");
                grid3Marks = UserParams.CustomParam("grid3Marks");
                grid4Marks = UserParams.CustomParam("grid4Marks");
                chart1Marks = UserParams.CustomParam("chart1Marks");
                chart2Marks = UserParams.CustomParam("chart2Marks");
                chart3Marks = UserParams.CustomParam("chart3Marks");
                chart4Marks = UserParams.CustomParam("chart4Marks");
                chart5Marks = UserParams.CustomParam("chart5Marks");
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
                if (!Page.IsPostBack)
                {   // опрерации которые должны выполняться при только первой загрузке страницы
                    grid1Marks = ForMarks.SetMarks(grid1Marks, ForMarks.Getmarks("grid1_mark_"), true);
                    grid2Marks = ForMarks.SetMarks(grid2Marks, ForMarks.Getmarks("grid2_mark_"), true);
                    grid3Marks = ForMarks.SetMarks(grid3Marks, ForMarks.Getmarks("grid3_mark_"), true);
                    grid4Marks = ForMarks.SetMarks(grid4Marks, ForMarks.Getmarks("grid4_mark_"), true);
                    chart1Marks = ForMarks.SetMarks(chart1Marks, ForMarks.Getmarks("chart1_mark_"), true);
                    chart2Marks = ForMarks.SetMarks(chart2Marks, ForMarks.Getmarks("chart2_mark_"), true);
                    chart3Marks = ForMarks.SetMarks(chart3Marks, ForMarks.Getmarks("chart3_mark_"), true);
                    chart4Marks = ForMarks.SetMarks(chart4Marks, ForMarks.Getmarks("chart4_mark_"), true);
                    chart5Marks = ForMarks.SetMarks(chart5Marks, ForMarks.Getmarks("chart5_mark_"), true);
                    RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                    WebAsyncRefreshPanel2.AddLinkedRequestTrigger(web_grid1);

                    WebAsyncRefreshPanel6.AddLinkedRequestTrigger(web_grid2);
                    WebAsyncRefreshPanel6.AddRefreshTarget(Chart4);
                    WebAsyncRefreshPanel6.AddRefreshTarget(chart4_caption);
                    WebAsyncRefreshPanel7.AddRefreshTarget(Chart5);
                    WebAsyncRefreshPanel7.AddRefreshTarget(chart5_caption);
                    WebAsyncRefreshPanel8.AddLinkedRequestTrigger(web_grid3);
                    WebAsyncRefreshPanel8.AddRefreshTarget(Chart6);
                    WebAsyncRefreshPanel8.AddRefreshTarget(chart6_caption);
                    WebAsyncRefreshPanel9.AddLinkedRequestTrigger(web_grid4);
                    WebAsyncRefreshPanel9.AddRefreshTarget(Chart7);
                    WebAsyncRefreshPanel9.AddRefreshTarget(chart7_caption);
                    //WebAsyncRefreshPanel4.AddLinkedRequestTrigger(web_grid4);
                    //ORG_0003_0001._default.setChartSettings(Chart1);
                    //ORG_0003_0001._default.setChartSettings(Chart2);
                    //ORG_0003_0001._default.setChartSettings(Chart3);
                    //ORG_0003_0001._default.setChartSettings(Chart4);
                    last_year.Value = getLastDate(RegionSettingsHelper.Instance.GetPropertyValue("way_last_year_mark"));
                    String year = UserComboBox.getLastBlock(last_year.Value);
                    page_title.Text = RegionSettingsHelper.Instance.GetPropertyValue("page_title");
                    Page.Title = page_title.Text;
                    // установка заголовка для таблицы
                    grid1_caption.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("grid1_title"), year);
                    // заполнение UltraWebGrid данными
                    web_grid1.DataBind();
                    web_grid1.Columns[web_grid1.Columns.Count - 1].Selected = true;
                    grid1Manual_ActiveCellChange(web_grid1.Columns.Count - 1);
                    grid2_caption.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("grid2_title"), year);
                    web_grid2.DataBind();
                    webGrid2ActiveRowChange(web_grid2.Rows.Count-1, true);

                    grid3_caption.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("grid3_title"), year);
                    web_grid3.DataBind();
                    webGrid3ActiveRowChange(0, true);

                    grid4_caption.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("grid4_title"), year);
                    web_grid4.DataBind();
                    webGrid4ActiveRowChange(0, true);
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
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid1"), "Показатели", grid_master);
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
            web_grid1.Columns[CellIndex].Selected = true;
            selected_year.Value = web_grid1.Columns[CellIndex].Header.Key.ToString();

            Chart1.DataBind();
            Chart2.DataBind();
            Chart3.DataBind();

            chart1_caption.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart1_title"), selected_year.Value);
            chart2_caption.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart2_title"), selected_year.Value);
            chart3_caption.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart3_title"), selected_year.Value);

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
                selected_year2.Value = row.Cells[0].Value.ToString();
                Chart4.DataBind();
                chart4_caption.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart4_title"), row.Cells[0].Value.ToString());
                Chart5.DataBind();
                chart5_caption.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart5_title"), row.Cells[0].Value.ToString());
            }
            catch (Exception)
            {
                // выбор сторки пошел с ошибкой ...
            }
        }

        protected void web_grid2_ActiveRowChange(object sender, RowEventArgs e)
        {
            webGrid2ActiveRowChange(e.Row.Index, false);
        }

        protected void web_grid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            try
            {
                // настройка столбцов
                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 20 - 3;
                e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth - 20) * 0.1) - 5;
                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 20) * 0.4) - 5;
                e.Layout.Bands[0].Columns[2].Width = (int)((tempWidth - 20) * 0.4) - 5;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "##");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "### ### ### ##0.##");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "### ### ### ##0.##");
                e.Layout.Bands[0].Columns[1].Header.Style.Wrap = true;
                e.Layout.Bands[0].Columns[2].Header.Style.Wrap = true;
            }
            catch
            {
                // ошибка инициализации
            }
        }

        // --------------------------------------------------------------------

        protected void web_grid3_DataBinding(object sender, EventArgs e)
        {
            try
            {

                DataTable grid_master = new DataTable();
                //DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid1"), "Показатель", grid_master);
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid3"));

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
                web_grid3.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
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
                current_way1.Value = row.Cells[0].Value.ToString();
                Chart6.DataBind();
                chart6_caption.Text = String.Format(chart6_title_caption, row.Cells[1].Value.ToString());
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

        // --------------------------------------------------------------------

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
                Chart7.DataBind();
                chart7_caption.Text = String.Format(chart7_title_caption, row.Cells[1].Value.ToString());
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


        protected void web_grid4_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                //DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid1"), "Показатель", grid_master);
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid4"));

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
                web_grid4.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }
        // --------------------------------------------------------------------

        protected void InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
           // ORG_0003_0001._default.setChartErrorFont(e);
        }

        protected void Chart1_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart1"), "_", grid_master);

                int max = 0;
                //object[] tableRow = grid_master.Rows[0].ItemArray;
                for (int i = 0; i < grid_master.Rows.Count; i++)
                {
                    max += Convert.ToInt32(grid_master.Rows[i].ItemArray[1]);
                }
                Chart1.Axis.X.RangeMax = max;

                Chart1.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }

        protected void Chart2_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart2"), "_", grid_master);

                int max = 0;
                //object[] tableRow = grid_master.Rows[0].ItemArray;
                for (int i = 0; i < grid_master.Rows.Count; i++)
                {
                    max += Convert.ToInt32(grid_master.Rows[i].ItemArray[1]);
                }
                Chart2.Axis.X.RangeMax = max;

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
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart3"), "_", grid_master);

                int max = 0;
                //object[] tableRow = grid_master.Rows[0].ItemArray;
                for (int i = 0; i < grid_master.Rows.Count; i++)
                {
                    max += Convert.ToInt32(grid_master.Rows[i].ItemArray[1]);
                }
                Chart3.Axis.X.RangeMax = max;

                Chart3.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }
        DataTable Chart4grid_master = new DataTable();
        protected void Chart4_DataBinding(object sender, EventArgs e)
        {
            try
            {
                //DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart4"), "_", Chart4grid_master);
                Chart4.DataSource = Chart4grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }
        DataTable Chart5grid_master = new DataTable();
        protected void Chart5_DataBinding(object sender, EventArgs e)
        {
            try
            {
                
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart5"), "_", Chart5grid_master);
                Chart5.DataSource = Chart5grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }

        protected void Chart6_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart6"), "_", grid_master);
                Chart6.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }

        protected void Chart7_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart7"), "_", grid_master);
                Chart7.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }


        protected void Chart6_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
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

                    if (year2 == text.GetTextString())
                    {
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

        protected void Chart4_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i=0; i<Chart4grid_master.Rows.Count;i++)
            {
                Infragistics.UltraChart.Core.Primitives.Text textLabel = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(28, 274 + i * 20-i, 250, 10), Chart4grid_master.Rows[i].ItemArray[0].ToString(), new Infragistics.UltraChart.Shared.Styles.LabelStyle(new Font("Microsoft Sans Serif", 8), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Far, Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal));
            e.SceneGraph.Add(textLabel);
            }
        }

        protected void Chart5_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < Chart5grid_master.Rows.Count; i++)
            {
                Infragistics.UltraChart.Core.Primitives.Text textLabel = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(28, 274 + i * 20 - i, 250, 10), Chart5grid_master.Rows[i].ItemArray[0].ToString(), new Infragistics.UltraChart.Shared.Styles.LabelStyle(new Font("Microsoft Sans Serif", 8), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Far, Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal));
                e.SceneGraph.Add(textLabel);
            }
        }

        protected void Chart2_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }


        // --------------------------------------------------------------------

        

    }

}

