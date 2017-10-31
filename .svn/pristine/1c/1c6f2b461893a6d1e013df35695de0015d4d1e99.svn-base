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
 *  Финансовое стояние предприятий.
 */
namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._0090
{
    public partial class Default : CustomReportPage
    {
        // --------------------------------------------------------------------

        // заголовок страницы
        private static String page_title_caption;// = "Финансовое состояние предприятий";
        // заголовок для Grid
        private static String grid_title_caption ;//= "Кредиторская и дебиторская задолженность, рублей";
        // заголовок для Grid_
        private static String grid_title_caption_;// = "Просроченная дебиторская и кредиторская задолженность, рублей";
        // заголовок для Grid1
        private static String grid1_title_caption;// = "Кредиторская и дебиторская задолженности МУП за&nbsp;{0}&nbsp;год, рублей";

        // заголовок для Grid2
        private static String grid2_title_caption ;//= "Балансовая прибыль (убыток) предприятий, рублей";
        // заголовок для Chatr1
        private static String chart1_title_caption ;//= "Структура кредиторской задолженности в&nbsp;{0}&nbsp;году";
        // заголовок для Chatr2
        private static String chart2_title_caption ;//= "Структура дебиторской задолженности в&nbsp;{0}&nbsp;году";
        // заголовок для Chatr3
        private static String chart3_title_caption ;//= "Динамика просроченной кредиторской задолженности, рубли";
        // заголовок для Chatr4
        private static String chart4_title_caption ;//= "Динамика просроченной дебиторской задолженности, рубли";
        // заголовок для Chatr5
        private static String chart5_title_caption;// = "Прибыльные и убыточные предприятия";

        // заголовок для Chart_1
        private static String chart_1_title_caption ;//= "«{0}, рубли»";
        // заголовок для Chart_1
        private static String chart_2_title_caption;// = "Распределение балансовой прибыли, %";
        // заголовок для Chart_1
        private static String chart_3_title_caption;// = "Распределение балансового убытка, %";

        private object[] perses = new object[11];

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
        private CustomParam grid_Marks;
        private CustomParam gridMarks;
        private CustomParam grid2Marks;
        private CustomParam chart1Marks;
        private CustomParam chart2Marks;
        private CustomParam chart3Marks;
        private CustomParam chart4Marks;
        private CustomParam chart5Marks;
        private CustomParam chart_2Marks;
        private CustomParam chart_3Marks;
        // --------------------------------------------------------------------
        string BN = "IE";
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            base.Page_PreLoad(sender, e);
            // установка размера диаграммы
            web_grid.Width = (int)((screen_width - 55) );
            web_grid_.Width = (int)((screen_width - 55));

            UltraChart1.Width = (int)((screen_width - 55) * 0.5);
            UltraChart2.Width = (int)((screen_width - 55) * 0.5);
            UltraChart3.Width = (int)((screen_width - 55) * 0.5);
            UltraChart4.Width = (int)((screen_width - 55) * 0.5);
            UltraChart5.Width = (int)((screen_width - 55) * 0.33);
            
            Label1.Width = (int)((screen_width - 55) * 0.49);
            Label2.Width = (int)((screen_width - 55) * 0.49);
           

            web_grid1.Width = (int)((screen_width - 55) * 0.33);
            web_grid2.Width = (int)((screen_width - 55) );

            Grid1Label.Width = (int)((screen_width - 55) * 0.32);
            Grid2Label.Width = (int)((screen_width - 55));

            Chart1.Width = (int)((screen_width - 55) * 0.64);
            Chart2.Width = (int)((screen_width - 55) * 0.33);
            Chart3.Width = (int)((screen_width - 55) * 0.33);
            if (BN == "IE")
            {
                web_grid1.Height = CRHelper.GetChartHeight(281);
            }
            else 
            {
                if (BN == "FIREFOX")
                {
                    web_grid1.Height = CRHelper.GetChartHeight(225);
                }
                else 
                {
                    web_grid1.Height = CRHelper.GetChartHeight(264);
                }
            }
            WebAsyncRefreshPanel1.AddLinkedRequestTrigger(web_grid1);
            RefreshPanel1.AddLinkedRequestTrigger(web_grid);
            RefreshPanel1.AddRefreshTarget(web_grid);
            WebAsyncRefreshPanel1.AddLinkedRequestTrigger(web_grid1);
            WebAsyncRefreshPanel1.AddRefreshTarget(Chart1);
            WebAsyncRefreshPanel1.AddRefreshTarget(Chart1Lab);

            Label4.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart_1_title1");
            grid1Marks = UserParams.CustomParam("grid1Marks");
            grid2Marks = UserParams.CustomParam("grid2Marks");
            grid_Marks = UserParams.CustomParam("grid_Marks");
            gridMarks = UserParams.CustomParam("gridMarks");
            chart5Marks = UserParams.CustomParam("chart5Marks");
            chart1Marks = UserParams.CustomParam("chart1Marks");
            chart2Marks = UserParams.CustomParam("chart2Marks");
            chart3Marks = UserParams.CustomParam("chart3Marks");
            chart4Marks = UserParams.CustomParam("chart4Marks");
            chart_2Marks = UserParams.CustomParam("chart_2Marks");
            chart_3Marks = UserParams.CustomParam("chart_3Marks");
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
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {   // опрерации которые должны выполняться при только первой загрузке страницы
                grid1Marks = ForMarks.SetMarks(grid1Marks, ForMarks.Getmarks("grid1_mark_"), true);
                grid2Marks = ForMarks.SetMarks(grid2Marks, ForMarks.Getmarks("grid2_mark_"), true);
                grid_Marks = ForMarks.SetMarks(grid_Marks, ForMarks.Getmarks("grid__mark_"), true);
                gridMarks = ForMarks.SetMarks(gridMarks, ForMarks.Getmarks("grid_mark_"), true);
                chart5Marks = ForMarks.SetMarks(chart5Marks, ForMarks.Getmarks("chart5_mark_"), true);

                chart1Marks = ForMarks.SetMarks(chart1Marks, ForMarks.Getmarks("chart1_mark_"), true);

                chart2Marks = ForMarks.SetMarks(chart2Marks, ForMarks.Getmarks("chart2_mark_"), true);

                chart3Marks = ForMarks.SetMarks(chart3Marks, ForMarks.Getmarks("chart3_mark_"), true);

                chart4Marks = ForMarks.SetMarks(chart4Marks, ForMarks.Getmarks("chart4_mark_"), true);

                chart_2Marks = ForMarks.SetMarks(chart_2Marks, ForMarks.Getmarks("chart_2_mark_"), true);

                chart_3Marks = ForMarks.SetMarks(chart_3Marks, ForMarks.Getmarks("chart_3_mark_"), true);
                RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;

                //WebAsyncRefreshPanel2.AddLinkedRequestTrigger(web_grid);

                last_year.Value = getLastDate(RegionSettingsHelper.Instance.GetPropertyValue("way_last_year_mark"));
                String year = UserComboBox.getLastBlock(last_year.Value);
                page_title.Text =String.Format(RegionSettingsHelper.Instance.GetPropertyValue("page_title"),RegionSettingsHelper.Instance.Name);
                Label4.Text = RegionSettingsHelper.Instance.GetPropertyValue("pageSubTitle");

                // установка заголовка для таблицы
                grid_caption.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("grid_title"), year);
                // заполнение UltraWebGrid данными
                web_grid.DataBind();
                web_grid.Columns[web_grid.Columns.Count - 1].Selected = true;
                gridManual_ActiveCellChange(web_grid.Columns.Count - 1);

                grid_caption_.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("grid_title_"), year);
                web_grid_.DataBind();

                Chart3Lab.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart3_title");
                Chart4Lab.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart4_title");
                UltraChart3.DataBind();
                UltraChart4.DataBind();

                Grid1Label.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("grid1_title"), year);
                web_grid1.DataBind();
                webGrid1ActiveRowChange(0, true);

                Grid2Label.Text = RegionSettingsHelper.Instance.GetPropertyValue("grid2_title");
                web_grid2.DataBind();
                grid2Manual_ActiveCellChange(web_grid2.Columns.Count - 1);

                Label3.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart5_title");
                UltraChart5.DataBind();
                if (BN == "FIREFOX")
                {
                    web_grid1.Height = 311;
                }
                web_grid.DisplayLayout.CellClickActionDefault = CellClickAction.CellSelect;
                
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
            DataTable grid_master = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid"), "Показатели", grid_master);
            web_grid.DataSource = grid_master.DefaultView;
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
            Chart1Lab.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart_1_title"), row.Cells[1].Value.ToString());
        }

        private void webGrid2ActiveRowChange(int index, bool active)
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
        }

        protected void Chart1_DataBinding1(object sender, EventArgs e)
        {
            Chart1.DataSource = dataBind("grid1");
        }

        protected void InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
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
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid2"), "Показатель", grid_master);
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



        protected void web_grid_ActiveCellChange(object sender, CellEventArgs e)
        {
            UltraGridCell cell = e.Cell;
            int CellIndex = cell.Column.Index;
            if (CellIndex > 0)
            {
                gridManual_ActiveCellChange(CellIndex);
            }
            else
            {
                gridManual_ActiveCellChange(1);
            }
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
           
            Label1.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart1_title"), selected_year.Value);
            Label2.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart2_title"), selected_year.Value);
           
        }

        protected void grid2Manual_ActiveCellChange(int CellIndex)
        {
            web_grid2.Columns[CellIndex].Selected = true;
            selected_year2.Value = web_grid2.Columns[CellIndex].Header.Key.ToString();

            Chart2.DataBind();
            Chart3.DataBind();

            Chart2Lab.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart_2_title"), selected_year2.Value);
            Chart3Lab_.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart_3_title"), selected_year2.Value);

        }


        protected void web_grid1_DataBinding(object sender, EventArgs e)
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
                        cs.Axes[1].Positions[pos.Ordinal].Members[0].Caption,
                        cs.Cells[0, pos.Ordinal].FormattedValue
                    };
                    // заполнение строки данными
                    grid_master.Rows.Add(values);
                }          

                web_grid1.DataSource = grid_master.DefaultView;
        }

        protected void web_grid_Click(object sender, ClickEventArgs e)
        {
            int CellIndex = 0;
            try
            {
                CellIndex = e.Cell.Column.Index;
                web_grid.Columns[e.Cell.Column.Index].Selected = 1 == 1;
            }
            catch
            {
                CellIndex = e.Column.Index;
                web_grid.Columns[e.Cell.Column.Index].Selected = 1 == 1;
            }

            if (CellIndex > 0)
            {
                gridManual_ActiveCellChange(CellIndex);
                web_grid.Columns[e.Cell.Column.Index].Selected = 1 == 1;
            }
            else
            {
                gridManual_ActiveCellChange(1);
                web_grid.Columns[1].Selected=1==1;
            }
            
        }

        protected void web_grid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            try
            {
                // настройка столбцов
                e.Layout.Bands[0].Columns[0].Hidden = true;
                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 20 - 3;
                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 20) * 0.7) - 5;
                e.Layout.Bands[0].Columns[2].Width = (int)((tempWidth - 20) * 0.3) - 5;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "### ### ###");
                e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            }
            catch
            {
                // ошибка инициализации
            }
        }

        protected void web_grid2_DataBinding1(object sender, EventArgs e)
        {
           
            DataTable grid_master = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid2"), "Отрасли", grid_master);
            web_grid2.DataSource = grid_master.DefaultView;
            Chart2.DataSource = grid_master.DefaultView;
            
        }




        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            DataTable grid_master = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart1"), "_", grid_master);
            UltraChart1.DataSource = grid_master.DefaultView;
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            DataTable grid_master = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart2"), "_", grid_master);
            UltraChart2.DataSource = grid_master.DefaultView;
        }

        protected void web_grid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            // настройка столбцов
            double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
            e.Layout.RowSelectorStyleDefault.Width = 20 - 3;
            e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth - 20) * 0.3) - 5;
            e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 20) * 0.7) - 5;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "### ### ### ##0.##");
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
        }

        protected void Chart1_DataBinding(object sender, EventArgs e)
        {
            DataTable grid_master = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart_1"), "_", grid_master);
            Chart1.DataSource = grid_master.DefaultView;
        }

        protected void web_grid1_ActiveRowChange(object sender, RowEventArgs e)
        {
            webGrid1ActiveRowChange(e.Row.Index, false);
        }

        protected void web_grid2_ActiveRowChange1(object sender, RowEventArgs e)
        {
            webGrid2ActiveRowChange(e.Row.Index, false);
        }

        protected void Chart2_DataBinding(object sender, EventArgs e)
        {
            Chart2Lab.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart_2_title"), selected_year2.Value);
            DataTable grid_master = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart_2"), "_", grid_master);
            Chart2.DataSource = grid_master.DefaultView;
        }

        protected void Chart3_DataBinding(object sender, EventArgs e)
        {
            Chart3Lab_.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart_3_title"), selected_year2.Value);
            DataTable grid_master = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart_3"), "_", grid_master);
            Chart3.DataSource = grid_master.DefaultView;
        }

        protected void web_grid__DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid_"), "Показатели", grid_master);
                web_grid_.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                throw new Exception(exception.Message, exception);
            }
        }

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {
            DataTable grid_master = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart3"), "Показатели", grid_master);
            UltraChart3.DataSource = grid_master.DefaultView;
        }

        protected void UltraChart4_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart4"), "Показатели", grid_master);
                UltraChart4.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                throw new Exception(exception.Message, exception);
            }
        }

        protected void UltraChart3_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
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

        protected void UltraChart5_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart5"), "_", grid_master);
                perses = grid_master.Rows[1].ItemArray;
                UltraChart5.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
            
        }

        protected void UltraChart5_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            String pers = "#<#>#";
            int i = 1;
            foreach (Primitive primitive in e.SceneGraph)
            {
                if (primitive is Text)
                {
                    Text text = primitive as Text;
                    if (pers == text.GetTextString())
                    {
                        text.SetTextString( perses[i].ToString() + " %" );
                        i++;
                    }
                }
            }
        }







    }

}

