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
using Infragistics.UltraChart.Core.Primitives;

/**
 *  Численность и структура населения.
 */
//namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._0040
namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00001._NumberAndStructurePopulation
{
    public partial class Default : CustomReportPage
    {
        // --------------------------------------------------------------------

        // заголовок страницы
        private static String page_title_caption = "Численность и структура населения";
        // заголовок для Grid1
        private static String grid1_title_caption = "Численность постоянного населения, человек";
        // заголовок для Grid2
        private static String grid2_title_caption = "Коэффициенты движения населения";
        //первый заголовой для Chart
        private static String chart_title_caption = "Структура населения";
        // заголовок для Chatr1
        private static String chart1_title_caption = "(по полу) в&nbsp;{0}&nbsp;году";
        // заголовок для Chatr2
        private static String chart2_title_caption = "в возрасте до 18 лет в&nbsp;{0}&nbsp;году";
        // заголовок для Chatr3
        private static String chart3_title_caption = "в трудоспособном возрасте в&nbsp;{0}&nbsp;году";
        // заголовок для Chatr4
        private static String chart4_title_caption = "старше трудоспособного возраста в&nbsp;{0}&nbsp;году";
        // заголовок для Chatr5
        private static String chart5_title_caption1 = "Динамика показателя";
        private static String chart5_title_caption = "Динамика показателя «{0}», {1}";

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

        private CustomParam chart1Marks;
        private CustomParam chart2Marks;
        private CustomParam chart3Marks;
        private CustomParam chart4Marks;
        private CustomParam chart5Marks;
        private CustomParam grid1Marks;
        private CustomParam grid2Marks;
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {   
                base.Page_PreLoad(sender, e);
                // установка размера диаграммы
                web_grid1.Width = (int)((screen_width - 55));
                web_grid2.Width = (int)((screen_width - 55));

                UltraChart1.Width = (int)((screen_width - 56) * 0.24);
                UltraChart2.Width = (int)((screen_width - 56) * 0.24);
                UltraChart3.Width = (int)((screen_width - 56) * 0.24);
                UltraChart4.Width = (int)((screen_width - 56) * 0.24);
                UltraChart5.Width = (int)((screen_width - 55));
                //Label6.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart_title");
                //Label7.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart_title");
                //Label8.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart_title");
                //Label9.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart_title");
                //Label10.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart5_title1");
                grid1Marks = UserParams.CustomParam("grid1Marks");
                grid2Marks = UserParams.CustomParam("grid2Marks");
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
                {
                    Session.Add("LastOpenReport", "004");
                    
                    // опрерации которые должны выполняться при только первой загрузке страницы

                    RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                    grid1Marks = ForMarks.SetMarks(grid1Marks, ForMarks.Getmarks("grid1_mark_"), true);
                    grid2Marks = ForMarks.SetMarks(grid2Marks, ForMarks.Getmarks("grid2_mark_"), true);
                    chart1Marks = ForMarks.SetMarks(chart1Marks, ForMarks.Getmarks("chart1_mark_"), true);
                    chart2Marks = ForMarks.SetMarks(chart2Marks, ForMarks.Getmarks("chart2_mark_"), true);
                    chart3Marks = ForMarks.SetMarks(chart3Marks, ForMarks.Getmarks("chart3_mark_"), true);
                    chart4Marks = ForMarks.SetMarks(chart4Marks, ForMarks.Getmarks("chart4_mark_"), true);
                    chart5Marks = ForMarks.SetMarks(chart5Marks, ForMarks.Getmarks("chart5_mark_"), true);
                    //chart1_mark.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart1_mark");
                    //chart2_mark.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart2_mark");
                   // chart3_mark.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart3_mark");
                   // chart4_mark.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart4_mark");
                   // chart5_mark.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart5_mark");
                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;

                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(web_grid2);
                    WebAsyncRefreshPanel2.AddLinkedRequestTrigger(web_grid1);

                    last_year.Value = getLastDate(RegionSettingsHelper.Instance.GetPropertyValue("way_last_year_mark"));
                    String year = UserComboBox.getLastBlock(last_year.Value);
                    page_title.Text = RegionSettingsHelper.Instance.GetPropertyValue("page_title");

                    // установка заголовка для таблиц
                    Grid1Label.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("grid1_title"), year);
                    Grid2Label.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("grid2_title"), year);

                    // заполнение UltraWebGrid данными
                    web_grid1.DataBind();
                    web_grid1.Columns[web_grid1.Columns.Count - 1].Selected = true;
                    grid1Manual_ActiveCellChange(web_grid1.Columns.Count - 1);

                    web_grid2.DataBind();
                    webGrid2ActiveRowChange(0, true);
                    

                    //EnclaveMO
                }
            }
            catch (Exception ex)
            {
                // неудачная загрузка ...
                throw new Exception(ex.Message, ex);
            }
            web_grid2.Columns[0].Width = 350;
            web_grid1.Columns[0].Width = 350;
            Page.Title = page_title.Text;
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
                e.Layout.RowSelectorsDefault = RowSelectors.No;
                e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth) * 0.4) - 60;
                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = (int)((tempWidth) * 0.6 / (e.Layout.Bands[0].Columns.Count - 1)) - 5;
                    e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                    // установка формата отображения данных в UltraWebGrid
                    if ((UltraWebGrid)(sender) == web_grid1)
                    {
                        CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ### ##0.##");
                    }
                    else
                    {
                        CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ### ##0.00");
                    }
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

        /// <summary>
        /// Обновление данных для UltraChart при изменении активной ячейки в UltraWebGrid
        /// </summary>
        /// <param name="CellIndex">индекс активной ячейки</param>
        protected void grid1Manual_ActiveCellChange(int CellIndex)
        {
            web_grid1.Columns[CellIndex].Selected = true;
            
            //web_grid1.Columns[CellIndex].se

            selected_year.Value = web_grid1.Columns[CellIndex].Header.Key.ToString();

            UltraChart1.DataBind();
            UltraChart2.DataBind();
            UltraChart3.DataBind();
            UltraChart4.DataBind();

            Label1.Text = "Структура населения " + String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart1_title"), selected_year.Value);
            Label2.Text = "Структура населения " + String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart2_title"), selected_year.Value);
            Label3.Text = "Структура населения " + String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart3_title"), selected_year.Value);
            Label4.Text = "Структура населения " + String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart4_title"), selected_year.Value);
        }

        // --------------------------------------------------------------------



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
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid2"), "Показатели", grid_master);
                web_grid2.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                throw new Exception(exception.Message, exception);
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
                current_way1.Value = row.Cells[0].Value.ToString().Split(',')[0];
                UltraChart5.DataBind();
                Label5.Text = String.Format(chart5_title_caption, row.Cells[0].Value.ToString().Split(',')[0], row.Cells[0].Value.ToString().Split(',')[1]);
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
            UltraChart4.DataSource = dataBind("chart4");
        }

        protected void UltraChart5_DataBinding(object sender, EventArgs e)
        {
            UltraChart5.DataSource = dataBind("chart5");
        }

        protected void UltraChart5_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
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

        protected void web_grid2_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[0].Value += ", человек";
        }

        protected void UltraChart3_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }

        protected void web_grid1_ActiveCellChange1(object sender, CellEventArgs e)
        {
            int CellIndex = e.Cell.Column.Index;
            

            if (CellIndex > 0)
            {
                grid1Manual_ActiveCellChange(CellIndex);
            }
            else
                grid1Manual_ActiveCellChange(1);
        }

     

    }

}

