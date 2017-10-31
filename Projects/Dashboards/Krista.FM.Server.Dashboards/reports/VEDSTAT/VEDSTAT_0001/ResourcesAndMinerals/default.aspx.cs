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
 *  Водные ресурсы и полезные ископаемые.
 */
namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._015
{
    public partial class Default : CustomReportPage
    {
        // --------------------------------------------------------------------

        // заголовок страницы
        private static String page_title_caption = "Водные ресурсы и полезные ископаемые";
        // заголовок для Grid1
        private static String grid1_title_caption = "Использование водных ресурсов, тыс. м<sup>3</sup>";
        // заголовок для Grid2
        private static String grid2_title_caption = "Добыча нефти и газа";

        // заголовок для Chatr1
        private static String chart1_title_caption = "Структура использования водных ресурсов в&nbsp;{0}&nbsp;году";
        // заголовок для Chatr2
        private static String chart2_title_caption = "Динамика показателя «{0}» <nobr>в {1} - {2} годах</nobr>";

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
        private CustomParam grid1Marks;
        private CustomParam grid2Marks;
        private CustomParam chart1Marks;
        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "sadad");
            return dt;
        }

        private string _GetString_(string s)
        {
            string res = "";
            int i = 0;
            for (i = s.Length - 1; s[i] != ','; i--) { res = s[i] + res; };

            return res;


        }

        private string GetString_(string s)
        {
            string res = "";
            int i = 0;
            for (i = s.Length - 1; s[i] != ','; i--) ;
            for (int j = 0; j < i; j++)
            {
                res += s[j];
            }
            return res;


        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
                BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
                base.Page_PreLoad(sender, e);
                // установка размера диаграммы
                web_grid1.Width = (int)((screen_width - 45) * 0.3333333333333333);
                web_grid2.Width = (int)((screen_width - 55) * 0.6666666666666666);

                UltraChart1.Width = (int)((screen_width - 55) * 0.33333333333333);
                CC.Width = (int)((screen_width - 55) * 0.66666666666666666666666);
                
                chart1_caption.Width = (int)((screen_width - 55) * 0.33333333333);
                chart2_caption.Width = (int)((screen_width - 55) * 0.66666666666);
                if (BN == "FOREFOX")
                {
                    UltraChart1.Height = 267;
                }
                grid1Marks = UserParams.CustomParam("grid1Marks");
                grid2Marks = UserParams.CustomParam("grid2Marks");
                chart1Marks = UserParams.CustomParam("chart1Marks");
                //UltraGauge1.Width = (int)((screen_width - 55) * 0.33);
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
                    chart1Marks=ForMarks.SetMarks(chart1Marks, ForMarks.Getmarks("chart1_mark_"), true);
                   
                    RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;

                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(web_grid1);
                    WebAsyncRefreshPanel3.AddLinkedRequestTrigger(web_grid2);
                    

                    last_year.Value = getLastDate(RegionSettingsHelper.Instance.GetPropertyValue("way_last_year_mark"));
                    String year = UserComboBox.getLastBlock(last_year.Value);
                    page_title.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("page_title"), year, UserComboBox.getLastBlock(RegionSettingsHelper.Instance.RegionBaseDimension));

                    // установка заголовка для таблиц
                    Grid1Label.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("grid1_title"), year);
                    Grid2Label.Text = RegionSettingsHelper.Instance.GetPropertyValue("grid2_title");

                    // заполнение UltraWebGrid данными
                    web_grid1.DataBind();
                    webGrid1ActiveRowChange(web_grid1.Rows.Count - 1, true);

                    web_grid2.DataBind();
                    webGrid2ActiveRowChange(0, true);
                    if (BN == "FIREFOX")
                    {
                        web_grid2.Height = 174;
                    }
                    if (BN == "APPLEMAC-SAFARI")
                    {
                        web_grid2.Height = 167;
                    }
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
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid1"), "Год", grid_master);
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
                Double coef = 1.0;
                if (BN == "IE")
                {
                    coef = 0.90;
                }
                if (BN == "FIREFOX")
                {
                    coef = 0.90;
                }
                // настройка столбцов
                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 20 - 3;
                e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth) * 0.1 * coef)-2;
                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth) * 0.3 * coef) - 2;
                e.Layout.Bands[0].Columns[2].Width = (int)((tempWidth) * 0.3 * coef) - 2;
                e.Layout.Bands[0].Columns[3].Width = (int)((tempWidth) * 0.3 * coef) - 2;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "### ### ### ##0.00");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "### ### ### ##0.00");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "### ### ### ##0.00");
                e.Layout.Bands[0].Columns[1].Header.Style.Wrap = true;
                e.Layout.Bands[0].Columns[2].Header.Style.Wrap = true;
                e.Layout.Bands[0].Columns[3].Header.Style.Wrap = true;
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
                selected_year.Value = row.Cells[0].Value.ToString();
                UltraChart1.DataBind();
                chart1_caption.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart1_title"), row.Cells[0].Value.ToString());
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

        protected void web_grid1_InitializeRow(object sender, RowEventArgs e)
        {
            try
            {
                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Index == 0) return;
                    if (e.Row.Cells[i].Value.ToString() == "") continue;
                    if (Convert.ToDouble(e.Row.PrevRow.Cells[i].Value) >= Convert.ToDouble(e.Row.Cells[i].Value))
                    {
                        if (Convert.ToDouble(e.Row.PrevRow.Cells[i].Value) == Convert.ToDouble(e.Row.Cells[i].Value)) return;
                        e.Row.Cells[i].Style.CssClass = "ArrowDownGreen";
                    }
                    else
                        e.Row.Cells[i].Style.CssClass = "ArrowUpRed";
                }
            }
            catch
            {
                // ошибка инициализации
            }
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
                grid_table.Columns.Add();
                grid_table.Columns.Add("Показатели");
                int columnsCount = grid_set.Cells.Count / grid_set.Axes[1].Positions.Count;
                for (int i = 1; i < columnsCount; ++i)
                {   // установка заголовков для столбцов UltraWebGrid
                    grid_table.Columns.Add((Convert.ToInt32(UserComboBox.getLastBlock(last_year.Value)) - columnsCount + i + 1).ToString());
                }
                foreach (Position pos in grid_set.Axes[1].Positions)
                {   // создание списка значений для строки UltraWebGrid
                    object[] values = new object[columnsCount + 1];
                    values[0] = pos.Members[0].MemberProperties[0].Value.ToString();
                    values[1] = grid_set.Axes[1].Positions[pos.Ordinal].Members[0].Caption + ", " + grid_set.Cells[0, pos.Ordinal].FormattedValue.ToString().ToLower();
                    for (int i = 0; i < columnsCount - 1; ++i)
                    {
                        values[i + 2] = grid_set.Cells[i + 1, pos.Ordinal].FormattedValue.ToString();
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
            /*
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
             */
        }

        protected void web_grid2_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            try
            {
                // настройка столбцов
                Double coef = 1.0;
                if (BN == "IE")
                {
                    coef = 0.95;
                }
                if (BN == "FIREFOX")
                {
                    coef = 0.95;
                }
                e.Layout.Bands[0].Columns[0].Hidden = true;
                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 20 - 3;

                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth*coef - 20) * 0.4) - 5;
                for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = (int)((tempWidth) * 0.85*coef / (e.Layout.Bands[0].Columns.Count));
                    e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                    // установка формата отображения данных в UltraWebGrid
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ### ###.##");
                }
                e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            }
            catch
            {
                // ошибка инициализации
            }
        }

        private void GaugeSet(LinearGauge gauge, int maxValue, int minValue, int value)
        {
            
            gauge.Scales[0].Axis.SetEndValue(maxValue);
            gauge.Scales[0].Axis.SetStartValue(minValue);
            gauge.Scales[0].Markers[0].Value = value;
            gauge.Scales[0].Labels.Frequency = (maxValue - minValue) / 4;
            gauge.Scales[0].Labels.FormatString = "<DATA_VALUE:###,##0>";

            

            gauge.Scales[0].MajorTickmarks.Frequency = (maxValue - minValue) / 4;
            gauge.Scales[0].MinorTickmarks.Frequency = (maxValue - minValue) / 16;
            //gauge.Scales[0].MajorTickmarks.EndWidth = 10;
            //gauge.Scales[0].MajorTickmarks.EndExtent = 40;

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
                
                current_way1.Value = row.Cells[0].Value.ToString();
                chart2_caption.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("gauge_title"), row.Cells[1].Value.ToString(), Convert.ToInt16(UserComboBox.getLastBlock(last_year.Value)) - 4, UserComboBox.getLastBlock(last_year.Value));

                //CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("gauge"));
                CC.DataBind();
                chart2_caption.Text = string.Format("Динамика показателя «{0}», {1}",GetString_(row.Cells[1].Value.ToString()),_GetString_(row.Cells[1].Value.ToString()));
                CC.Tooltips.FormatString = "<b><DATA_VALUE:### ### ##0.##></b>, " + _GetString_(row.Cells[1].Value.ToString());
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

        protected void CC_DataBinding(object sender, EventArgs e)
        {   
            DataTable dt = GetDSForChart("gauge");
            double max = 0, min = 0;
            //



            for (int j = 0; j < dt.Rows.Count; j++)
                for (int i = 1; i < dt.Rows[j].ItemArray.Length; i++)
                {
                    try
                    {
                        if (double.Parse(dt.Rows[j].ItemArray[i].ToString()) > max) { max = double.Parse(dt.Rows[j].ItemArray[i].ToString()); }
                        if (double.Parse(dt.Rows[j].ItemArray[i].ToString()) < min) { min = double.Parse(dt.Rows[j].ItemArray[i].ToString()); }
                    }
                    catch { }
                }

            CC.Axis.Y.RangeMax = max * 1.20;
            CC.Axis.Y.RangeMin = 0;
            CC.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom; 
            CC.DataSource = dt;
        }


    }
}

