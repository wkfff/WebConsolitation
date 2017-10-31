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
 *  Охрана окружающей среды.
 */
namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._025
{
    public partial class Default : CustomReportPage
    {
        // --------------------------------------------------------------------

        // заголовок страницы
        private static String page_title_caption = "Охрана окружающей среды";
        // заголовок для Grid1
        private static String grid1_title_caption = "Выбросы загрязняющих веществ в атмосферу, тонны";
        // заголовок для Grid2
        private static String grid2_title_caption = "Основные показатели охраны окружающей среды в&nbsp;{0}&nbsp;году";

        // заголовок для Chatr1
        private static String chart1_title_caption = "Выбросы загрязняющих веществ в атмосферу в&nbsp;{0}&nbsp;году, тонны";
        // заголовок для Chatr2
        private static String chart2_title_caption = "Выбросы жидких и газообразных веществ (по видам веществ) в&nbsp;{0}&nbsp;году, тонны";
        // заголовок для Chatr3
        private static String chart3_title_caption = "«{0}»";

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
        private CustomParam chart2Marks;
        private CustomParam chart3Marks;
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
                BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
                base.Page_PreLoad(sender, e);
                // установка размера диаграммы
                web_grid1.Width = (int)((screen_width - 55) * 0.33);
                web_grid2.Width = (int)((screen_width - 55) * 0.33);

                UltraChart1.Width = (int)((screen_width - 55) * 0.33);
                UltraChart2.Width = (int)((screen_width - 55) * 0.33);
                UltraChart3.Width = (int)((screen_width - 55) * 0.66);

                Grid1Label.Width = (int)((screen_width - 55) * 0.32);
                Grid2Label.Width = (int)((screen_width - 55) * 0.32);
                Label2.Width = (int)((screen_width - 55) * 0.32);
                Label3.Width = (int)((screen_width - 55) * 0.32);
                grid1Marks = UserParams.CustomParam("grid1Marks");
                grid2Marks = UserParams.CustomParam("grid2Marks");
                chart2Marks = UserParams.CustomParam("chart2Marks");
                chart3Marks = UserParams.CustomParam("chart3Marks");
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
                    RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                    grid1Marks = ForMarks.SetMarks(grid1Marks, ForMarks.Getmarks("grid1_mark_"), true);
                    grid2Marks = ForMarks.SetMarks(grid2Marks, ForMarks.Getmarks("grid2_mark_"), true);
                    chart2Marks = ForMarks.SetMarks(chart2Marks, ForMarks.Getmarks("chart2_mark_"), true);
                    chart3Marks = ForMarks.SetMarks(chart3Marks, ForMarks.Getmarks("chart3_mark_"), true);
                    //WebAsyncRefreshPanel2.AddLinkedRequestTrigger(web_grid1);
                    WebAsyncRefreshPanel4.AddLinkedRequestTrigger(web_grid1);
                    //WebAsyncRefreshPanel1.AddLinkedRequestTrigger(web_grid2);
                    WebAsyncRefreshPanel6.AddLinkedRequestTrigger(web_grid2);
                    WebAsyncRefreshPanel6.AddRefreshTarget(Label1);
                    WebAsyncRefreshPanel6.AddRefreshTarget(UltraChart3);

                    last_year.Value = getLastDate(RegionSettingsHelper.Instance.GetPropertyValue("way_last_year_mark"));
                    String year = UserComboBox.getLastBlock(last_year.Value);
                    page_title.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("page_title"), year);

                    // установка заголовка для таблиц
                    Grid1Label.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("grid1_title"), year);
                    Grid2Label.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("grid2_title"), year);

                    // заполнение UltraWebGrid данными
                    web_grid1.DataBind();
                    webGrid1ActiveRowChange(web_grid1.Rows.Count - 1, true);

                    web_grid2.DataBind();
                    webGrid2ActiveRowChange(0, true);
                    //Label4.Text =RegionSettingsHelper.Instance.GetPropertyValue("chart3_title1");
                    if (BN == "FIREFOX")
                    {
                        
                    }
                    web_grid1.Height = Unit.Empty;
                    web_grid2.Height = Unit.Empty;
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
                // настройка столбцов
                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 20 - 3;
                e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth - 20) * 0.14) - 5;
                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 20) * 0.43*2) - 5;
                e.Layout.Bands[0].Columns[2].Width = (int)((tempWidth - 20) * 0.43) - 5;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "### ### ### ##0.00");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "### ### ### ##0.00");
                e.Layout.Bands[0].Columns[2].Hidden = 1 == 1;
                e.Layout.Bands[0].Columns[1].Header.Style.Wrap = true;
                e.Layout.Bands[0].Columns[2].Header.Style.Wrap = true;
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
                UltraChart2.DataBind();
                Label2.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart1_title"), row.Cells[0].Value.ToString());
                Label3.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart2_title"), row.Cells[0].Value.ToString());
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
                if (e.Row.Cells.Count < 4) return;
                if (Convert.ToDouble(e.Row.Cells[2].Value) >= Convert.ToDouble(e.Row.Cells[3].Value))
                {
                    if (Convert.ToDouble(e.Row.Cells[2].Value) == Convert.ToDouble(e.Row.Cells[3].Value)) return;
                    e.Row.Cells[3].Style.CssClass = "ArrowDownGreen";
                }
                else
                    e.Row.Cells[3].Style.CssClass = "ArrowUpRed";
            }
            catch
            {
                // ошибка инициализации
            }
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
        /// <summary>
        /// Получение последнего блока из поля
        /// </summary>
        /// <param name="s">поле</param>
        /// <returns>то что в последних скобачках</returns>
        private String ELV(String s)
        {
            int i = s.Length;
            string res = "";
            while (s[--i] != ']') ;
            while (s[--i] != '[')
            {
                res = s[i] + res;
            }
            return res;

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


        protected void web_grid2_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid2"));

                grid_master.Columns.Add();
                grid_master.Columns.Add("Показатель");
                grid_master.Columns.Add(ELV(last_year.Value));

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
                current_way1.Value = row.Cells[0].Value.ToString();
                //UltraGauge1.DataBind();
                UltraChart3.DataBind();

                Label1.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart3_title"), GetString_(row.Cells[1].Value.ToString()), _GetString_(row.Cells[1].Value.ToString()));
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
                e.Layout.Bands[0].Columns[2].Header.Style.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].Hidden = true;
                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 20 - 3;
                e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 20) * 0.75) - 5;
                e.Layout.Bands[0].Columns[2].Width = (int)((tempWidth - 20) * 0.25) - 5;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "### ### ### ##0.00");
               // e.Layout.Bands[0].Columns[2].Header.Style.HorizontalAlign = HorizontalAlign.Center;
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

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = dataBind("chart1").ToTable();
            double max = 0, min = double.Parse(dt.Rows[0].ItemArray[1].ToString());
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
            UltraChart3.DataSource = dt;
            UltraChart3.Axis.Y.RangeMax = max * 1.20;
            UltraChart3.Axis.Y.RangeMin = min * 0.90;
            UltraChart3.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom; 
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            UltraChart1.DataSource = dataBind("chart2");
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            UltraChart2.DataSource = dataBind("chart3");
        }

        protected void UltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
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


        protected void UltraChart2_ChartDrawItem(object sender, Infragistics.UltraChart.Shared.Events.ChartDrawItemEventArgs e)
        {
            //UltraChart UltraChart_ = (UltraChart)sender;
            //устанавливаем ширину текста легенды 
            Infragistics.UltraChart.Core.Primitives.Text text = e.Primitive as Infragistics.UltraChart.Core.Primitives.Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;
                //UltraChart_.Legend.Location =
                if ((UltraChart2.Legend.Location == Infragistics.UltraChart.Shared.Styles.LegendLocation.Top) || (UltraChart2.Legend.Location == Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom))
                {
                    widthLegendLabel = (int)UltraChart2.Width.Value - 20;

                }
                else
                {
                    widthLegendLabel = ((int)UltraChart2.Legend.SpanPercentage * (int)UltraChart2.Width.Value / 100) - 20;
                }

                widthLegendLabel -= UltraChart2.Legend.Margins.Left + UltraChart2.Legend.Margins.Right;
                if (text.labelStyle.Trimming != StringTrimming.None)
                {
                    text.bounds.Width = widthLegendLabel;



                }

                //text.labelStyle.Dy += addY;
                string Capt = text.GetTextString();
                if (Capt.Length > 30)
                {
                    //int i = 30;
                    //for (; Capt[i] != ' '; i--) { }

                    text.SetTextString(Capt.Remove(30)+"...");//Insert(i, "\n"));
                    //Боланз)
                    //text.labelStyle.Dy -= 5;
                    //addY += 5;
                }





            }

        }
        int addY = 0;
        protected void UltraChart1_ChartDrawItem(object sender, Infragistics.UltraChart.Shared.Events.ChartDrawItemEventArgs e)
        {
            //UltraChart UltraChart_ = (UltraChart)sender;
            //устанавливаем ширину текста легенды 
            Infragistics.UltraChart.Core.Primitives.Text text = e.Primitive as Infragistics.UltraChart.Core.Primitives.Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;
                //UltraChart_.Legend.Location =
                if ((UltraChart1.Legend.Location == Infragistics.UltraChart.Shared.Styles.LegendLocation.Top) || (UltraChart1.Legend.Location == Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom))
                {
                    widthLegendLabel = (int)UltraChart1.Width.Value - 20;

                }
                else
                {
                    widthLegendLabel = ((int)UltraChart1.Legend.SpanPercentage * (int)UltraChart1.Width.Value / 100) - 20;
                }

                widthLegendLabel -= UltraChart1.Legend.Margins.Left + UltraChart1.Legend.Margins.Right;
                if (text.labelStyle.Trimming != StringTrimming.None)
                {
                    text.bounds.Width = widthLegendLabel;



                }

                //text.labelStyle.Dy += addY;
                //string Capt = text.GetTextString();
                //if (Capt.Length > 30)
                //{
                //    int i = 30;
                //    for (; Capt[i] != ' '; i--) { }

                //    text.SetTextString(Capt.Insert(i, "\n"));
                //    //Боланз)
                //    text.labelStyle.Dy -= 5;
                //    addY += 5;
                //}





            }

        }

    }

}

