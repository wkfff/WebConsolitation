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
 *  Малые предприятия.
 */
namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_0001._0270
{

    public partial class Default : CustomReportPage
    {
        // --------------------------------------------------------------------

        // заголовок страницы
        private static String page_title_caption = "Малые предприятия ";

        // заголовок для Grid1
        private static String grid1_title_caption = "Основные показатели по малым предприятиям за&nbsp;{0}&nbsp;год";
        // заголовок для Grid2
        private static String grid2_title_caption = "Результаты торговли для малых предприятий, рубли";
        // заголовок для Grid3
        private static String grid3_title_caption = "Численность работников малых предприятий, человек";
        // заголовок для Grid4
        private static String grid4_title_caption = "Объём налоговых поступлений, рубли";

        // заголовок для Chatr1
        private static String chart1_title_caption = "Динамика показателя «{0}»";
        // заголовок для Chatr2
        private static String chart2_title_caption = "Платные улуги населению в&nbsp;{0}&nbsp;году, рубли";
        // заголовок для Chatr3
        private static String chart3_title_caption = "Розничная торговля в&nbsp;{0}&nbsp;году, рубли";
        // заголовок для Chatr4
        private static String chart4_title_caption = "Оптовая торговля в&nbsp;{0}&nbsp;году, рубли";
        // заголовок для Chatr5
        private static String chart5_title_caption = "Среднесписочная численность работников по малым предприятиям в&nbsp;{0}&nbsp;году, человек";
        // заголовок для Chatr6
        private static String chart6_title_caption = "Численности работающих по отраслям в&nbsp;{0}&nbsp;году, человек";
        // заголовок для Chatr7
        private static String chart7_title_caption = "Налоговые поступления в&nbsp;{0}&nbsp;году, рубли";

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
                web_grid1.Width = (int)((screen_width - 55) * 0.5);
                web_grid2.Width = (int)((screen_width - 55));
                web_grid3.Width = (int)((screen_width - 55) * 0.33);
                web_grid4.Width = (int)((screen_width - 55) * 0.5);
                if (BN == "IE")
                {
                    UltraChart1.Width = (int)((screen_width - 70) * 0.5);
                    UltraChart7.Width = (int)((screen_width - 70) * 0.5);
                }
                if (BN == "FIREFOX")
                {
                    UltraChart1.Width = (int)((screen_width - 65) * 0.5);
                    UltraChart7.Width = (int)((screen_width - 65) * 0.5);
                }
                if (BN == "APPLEMAC-SAFARI")
                {
                    UltraChart1.Width = (int)((screen_width - 55) * 0.5);
                    UltraChart7.Width = (int)((screen_width - 55) * 0.5);
                }

                UltraChart2.Width = (int)((screen_width - 55) * 0.33);
                UltraChart3.Width = (int)((screen_width - 55) * 0.33);
                UltraChart4.Width = (int)((screen_width - 55) * 0.33);
                UltraChart5.Width = (int)((screen_width - 55) * 0.33);
                UltraChart6.Width = (int)((screen_width - 55) * 0.33);
                

                Grid1Label.Width = (int)((screen_width - 55) * 0.49);
                Grid2Label.Width = (int)((screen_width - 55));

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

                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(web_grid3);
                    WebAsyncRefreshPanel3.AddLinkedRequestTrigger(web_grid2);
                    WebAsyncRefreshPanel4.AddLinkedRequestTrigger(web_grid4);
                    WebAsyncRefreshPanel2.AddLinkedRequestTrigger(web_grid1);


                    last_year.Value = getLastDate("МАЛЫЕ ПРЕДПРИЯТИЯ");
                    String year = UserComboBox.getLastBlock(last_year.Value);
                    page_title.Text = String.Format(page_title_caption, year);

    
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

                    // установка заголовков
                    Grid1Label.Text = String.Format(grid1_title_caption, year);
                    Grid2Label.Text = String.Format(grid2_title_caption, year);
                    Grid3Label.Text = String.Format(grid3_title_caption, year);
                    Grid4Label.Text = String.Format(grid4_title_caption, year);

                    
                    
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
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid2"), "Показатели", grid_master);
                web_grid2.DataSource = grid_master.DefaultView;
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
                double tempWidth = e.Layout.FrameStyle.Width.Value - 16;
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

                UltraChart7.DataBind();
                Label7.Text = String.Format(chart7_title_caption, current_way2.Value);
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
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart4")," ",dt);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dt1.Columns.Add(dt.Columns[i].ColumnName,dt.Columns[i].DataType);
            }
            object[] o=new object[dt.Columns.Count];
            o[0] = "Оборот оптовой торговли";
            o[1] = dt.Rows[0].ItemArray[1];
            dt1.Rows.Add(o);
                UltraChart4.DataSource = dt1;
        }

        protected void UltraChart5_DataBinding(object sender, EventArgs e)
        {
            UltraChart5.DataSource = dataBind("chart5");
        }

        protected void UltraChart6_DataBinding(object sender, EventArgs e)
        {
            UltraChart6.DataSource = dataBind("chart6");
        }
        DataTable Chart7grid_master = new DataTable();
        protected void UltraChart7_DataBinding(object sender, EventArgs e)
        {
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart7"), " ", Chart7grid_master);

            UltraChart7.DataSource = Chart7grid_master;//dataBind("chart7");
        }

        protected void UltraChart7_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            
            Infragistics.UltraChart.Core.Primitives.Text textLabel = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(28, 398, 250, 10), Chart7grid_master.Columns[1].ColumnName, new Infragistics.UltraChart.Shared.Styles.LabelStyle(new Font("Microsoft Sans Serif", 8), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Far, Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal));
            e.SceneGraph.Add(textLabel);
            Infragistics.UltraChart.Core.Primitives.Text textLabel1 = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(28, 398 + 20 - 1, 250, 10), Chart7grid_master.Columns[2].ColumnName, new Infragistics.UltraChart.Shared.Styles.LabelStyle(new Font("Microsoft Sans Serif", 8), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Far, Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal));
            e.SceneGraph.Add(textLabel1);
            Infragistics.UltraChart.Core.Primitives.Text textLabel2 = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(28, 398 + 20*2+5, 250, 10), Chart7grid_master.Columns[3].ColumnName, new Infragistics.UltraChart.Shared.Styles.LabelStyle(new Font("Microsoft Sans Serif", 8), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Far, Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal));
            e.SceneGraph.Add(textLabel2);
            Infragistics.UltraChart.Core.Primitives.Text textLabel3 = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(28, 398 + 20*3 - 3, 250, 10), Chart7grid_master.Columns[4].ColumnName, new Infragistics.UltraChart.Shared.Styles.LabelStyle(new Font("Microsoft Sans Serif", 8), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Far, Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal));
            e.SceneGraph.Add(textLabel3);
        }

    }
}

