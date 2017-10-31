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
using System.IO;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.Misc;
using Infragistics.Documents.Excel;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;
using Infragistics.UltraChart.Core;
using Color = System.Drawing.Color;
using Graphics = System.Drawing.Graphics;
using Image = System.Drawing.Image;
using TextAlignment = Infragistics.Documents.Reports.Report.TextAlignment;
using Dundas.Maps.WebControl;
using System.Drawing.Imaging;

/**
 *  Организация, содержание и развитие учреждений здравоохранения.
 */
namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._0130
{
    public partial class Default : CustomReportPage
    {
        // --------------------------------------------------------------------

        // заголовок страницы
        private static String page_title_caption = "Организация, содержание и развитие учреждений здравоохранения {0}";
        // текст отчёта
        private static String report_text = "Число больничных учреждений составляет&nbsp;-&nbsp;<b>{0}</b>, больничных коек&nbsp;-&nbsp;<b>{1}</b> (из них реанимационные&nbsp;-&nbsp;<b>{2}</b>).<br>Число родильных домов&nbsp;-&nbsp;<b>{3}</b>.<br>Станции скорой <nobr>помощи - <b>{4}</b></nobr>.<br>Амбулаторно-поликлинические <nobr>учреждения - <b>{5}</b></nobr>.<br>Мощность ЛПУ составляет&nbsp;-&nbsp;<b>{6}</b>.<br>Численность врачей составляет <b>{7}</b>&nbsp;,человек, численность среднего медицинского персонала <b>{8}</b>&nbsp;,человек.";

        // заголовок для Grid1
        private static String grid1_title_caption = "Число случаев заболеваний";
        // заголовок для Grid2
        private static String grid2_title_caption = "Заболеваемость и болезненность населения";

        // заголовок для Chatr1
        private static String chart1_title_caption = "«{0}» до&nbsp;{1}&nbsp;года, человек";
        // заголовок для Chatr2
        private static String chart2_title_caption = "«{0}» в&nbsp;{1}&nbsp;году, %";
        // заголовок для Chatr3
        private static String chart3_title_caption = "«{0}» в&nbsp;{1}&nbsp;году, %";

        // заголовок для Gauge1
        private static String gauge1_title_caption = "Заболеваемость населения, острые заболевания в&nbsp;{0}&nbsp;году (на 1000 человек), человек";
        // заголовок для Gauge2
        private static String gauge2_title_caption = "Болезненность населения, острые и хронические заболевания в&nbsp;{0}&nbsp;году (на 1000 человек), человек";

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
        private CustomParam textMarks;
        private CustomParam gridMarks;
        private CustomParam grid2Marks;
        private CustomParam gaugeMarks;
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
                BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
                base.Page_PreLoad(sender, e);
                // установка размера диаграммы
                web_grid1.Width = (int)((screen_width - 50) * 0.33);
                web_grid2.Width = (int)((screen_width - 50) * 0.33);

                UltraChart1.Width = (int)((screen_width - 54) * 0.6667);
                UltraChart2.Width = (int)((screen_width - 60) * 0.33);
                UltraChart3.Width = (int)((screen_width - 60) * 0.33);

                Grid1Label.Width = (int)((screen_width - 55) * 0.32);
                Grid2Label.Width = (int)((screen_width - 55) * 0.32);
                //Label2.Width = (int)((screen_width - 55) * 0.33);
                //Label3.Width = (int)((screen_width - 55) * 0.33);
                Label2.Height = 54;
                Label3.Height = 54;
                GaugeLabel1.Width = (int)((screen_width - 55) * 0.33);
                GaugeLabel2.Width = (int)((screen_width - 55) * 0.33);
                Label4.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart1_title1");
                Label5.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart2_title1");
                Label6.Text = RegionSettingsHelper.Instance.GetPropertyValue("grid1_title2");
                Label7.Text = RegionSettingsHelper.Instance.GetPropertyValue("grid2_title2");
                Label1.Text = "Динамика количества случаев заболевания";
                if (BN == "FIREFOX")
                {
                    web_grid1.Height = 749;
                    web_grid2.Height = 285;
                }
                if (BN == "APPLEMAC-SAFARI")
                {
                    web_grid2.Height = 212;
                    web_grid1.Height=733;
                }
                textMarks = UserParams.CustomParam("textMarks");
                gridMarks = UserParams.CustomParam("gridMarks");
                grid2Marks = UserParams.CustomParam("grid2Marks");
                gaugeMarks = UserParams.CustomParam("gaugeMarks");
            }
            catch (Exception)
            {
                // установка размеров не удалась ...
            }
        }

        // --------------------------------------------------------------------
        Dictionary<string, int> YearsLoad(string lastyear)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            String year = UserComboBox.getLastBlock(lastyear);
            for (int i = int.Parse(year); i >= (int.Parse(year) - 4); i--)
            {
                d.Add(i.ToString(), 0);
            }
            return d;
        }
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
                    textMarks=ForMarks.SetMarks(textMarks,ForMarks.Getmarks("text_mark_"),true);
                    gridMarks = ForMarks.SetMarks(gridMarks, ForMarks.Getmarks("grid_mark_"), true);
                    grid2Marks = ForMarks.SetMarks(grid2Marks, ForMarks.Getmarks("grid2_mark_"), true);
                    gaugeMarks = ForMarks.SetMarks(gaugeMarks, ForMarks.Getmarks("gauge_mark_"), true);
                    RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                    WebAsyncRefreshPanel2.AddLinkedRequestTrigger(web_grid1);
                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(web_grid2);
                    Year.Title = "Год";
                    last_year.Value = getLastDate(RegionSettingsHelper.Instance.GetPropertyValue("way_last_year_mark"));
                    Year.FillDictionaryValues(YearsLoad(last_year.Value));
                    String year = UserComboBox.getLastBlock(last_year.Value);

                }
                selected_year.Value = Year.SelectedValue;
                //selected_year.Value = DropDownListYear.SelectedValue;
                page_title.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("page_title"), selected_year.Value);
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
                    ReportHeader.Text = String.Format("По данным на {0} год:", selected_year.Value);
                    //WebPanel1.Header.Text = String.Format("По данным на {0} год:", selected_year.Value);
                    ReportText.Text = String.Format(report_text,
                                                    tempArray[0].ToString(),
                                                    tempArray[1].ToString(),
                                                    tempArray[2].ToString(),
                                                    tempArray[3].ToString(),
                                                    tempArray[4].ToString(),
                                                    tempArray[5].ToString(),
                                                    tempArray[6].ToString(),
                                                    tempArray[7].ToString(),
                                                    tempArray[8].ToString());

                }
                else ReportText.Text = "в настоящий момент данные отсутствуют!";


                // установка заголовка для таблиц
                Grid1Label.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("grid1_title"), selected_year.Value);
                Grid2Label.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("grid2_title"), selected_year.Value);

                // заполнение UltraWebGrid данными
                web_grid1.DataBind();
                webGrid1ActiveRowChange(0, true);

                web_grid2.DataBind();
                webGrid2ActiveRowChange(web_grid2.Rows.Count-1, true);
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
                grid_master.Columns.Add("Значение1");
                grid_master.Columns.Add(selected_year.Value.ToString());

                int cellCol = 1;
                if (cs.Cells.Count == cs.Axes[1].Positions.Count)
                    cellCol = 0;

                foreach (Position pos in cs.Axes[1].Positions)
                {   // создание списка значений для строки UltraWebGrid
                    object[] values = {
                        pos.Members[0].MemberProperties[0].Value.ToString(),
                        cs.Axes[1].Positions[pos.Ordinal].Members[0].Caption,
                        string.Format("{0:N0}", cs.Cells[0, pos.Ordinal].Value),
                        string.Format("{0:N0}", cs.Cells[cellCol, pos.Ordinal].Value)
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
                if (e.Layout.Bands[0].Columns.Count > 3)
                    e.Layout.Bands[0].Columns[2].Hidden = true;
                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 20 - 3;
                e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 20) * 0.75) - 5;

                e.Layout.Bands[0].Columns[2].Width = (int)((tempWidth - 20) * 0.25) - 5;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "### ### ### ##0");
                e.Layout.Bands[0].Columns[2].Header.Style.HorizontalAlign = HorizontalAlign.Center;

                e.Layout.Bands[0].Columns[3].Width = (int)((tempWidth - 20) * 0.25) - 5;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "### ### ### ##0");
                e.Layout.Bands[0].Columns[3].Header.Style.HorizontalAlign = HorizontalAlign.Center;
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
                UltraChart2.DataBind();
                UltraChart3.DataBind();
                Label8.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart1_title"), row.Cells[1].Value.ToString(), selected_year.Value);
                Label2.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart2_title"), row.Cells[1].Value.ToString(), int.Parse(selected_year.Value) - 1);
                Label3.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart3_title"), row.Cells[1].Value.ToString(), selected_year.Value);
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

        private void GaugeSet(LinearGauge gauge, int maxValue, int value, int avgValue)
        {
            if (value < avgValue)
            {
                gauge.Scales[0].Markers[1].Value = value;
                gauge.Scales[0].Markers[0].Visible = false;
                gauge.Scales[0].Markers[1].Visible = true;
            }
            else
            {
                gauge.Scales[0].Markers[0].Value = value;
                gauge.Scales[0].Markers[0].Visible = true;
                gauge.Scales[0].Markers[1].Visible = false;
            }

                
            gauge.Scales[0].Axis.SetEndValue(maxValue);
            gauge.Scales[0].Ranges[0].EndValue = maxValue;
            gauge.Scales[0].Labels.PostInitial = value;
            gauge.Scales[0].Labels.Frequency = 10000;
            gauge.Scales[0].MajorTickmarks.PostInitial = avgValue;
            gauge.Scales[0].MajorTickmarks.Frequency = 10000;
            gauge.Scales[0].MinorTickmarks.Frequency = 100;
            gauge.Scales[0].MajorTickmarks.EndWidth = 10;
            gauge.Scales[0].MajorTickmarks.EndExtent = 40;
            
        }

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
                selected_year2.Value = row.Cells[0].Value.ToString();
                //UltraGauge1.DataBind();
                GaugeLabel1.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("gauge1_title"), selected_year2.Value);
                GaugeLabel2.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("gauge2_title"), selected_year2.Value);
                

                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("gauge"));

                Caption1.Text = cs.Axes[1].Positions[0].Members[0].Caption.ToString();
                Caption2.Text = cs.Axes[1].Positions[1].Members[0].Caption.ToString();
                Caption3.Text = cs.Axes[1].Positions[2].Members[0].Caption.ToString();
                Caption4.Text = cs.Axes[1].Positions[3].Members[0].Caption.ToString();
                Caption5.Text = cs.Axes[1].Positions[4].Members[0].Caption.ToString();
                Caption6.Text = cs.Axes[1].Positions[5].Members[0].Caption.ToString(); 

                int maxValue = 0;
                if (Convert.ToInt32(cs.Cells[0, 0].Value) > maxValue)
                    maxValue = Convert.ToInt32(cs.Cells[0, 0].Value);
                if (Convert.ToInt32(cs.Cells[0, 1].Value) > maxValue)
                    maxValue = Convert.ToInt32(cs.Cells[0, 1].Value);
                if (Convert.ToInt32(cs.Cells[0, 2].Value) > maxValue)
                    maxValue = Convert.ToInt32(cs.Cells[0, 2].Value);
                maxValue = Convert.ToInt32(maxValue * 1.1);

                
                GaugeSet(((LinearGauge)UltraGauge1.Gauges[0]), maxValue, Convert.ToInt32(cs.Cells[0, 0].Value), Convert.ToInt32(row.Cells[1].Value));
                GaugeSet(((LinearGauge)UltraGauge2.Gauges[0]), maxValue, Convert.ToInt32(cs.Cells[0, 1].Value), Convert.ToInt32(row.Cells[1].Value));
                GaugeSet(((LinearGauge)UltraGauge3.Gauges[0]), maxValue, Convert.ToInt32(cs.Cells[0, 2].Value), Convert.ToInt32(row.Cells[1].Value));

                maxValue = 0;
                if (Convert.ToInt32(cs.Cells[0, 3].Value) > maxValue)
                    maxValue = Convert.ToInt32(cs.Cells[0, 3].Value);
                if (Convert.ToInt32(cs.Cells[0, 4].Value) > maxValue)
                    maxValue = Convert.ToInt32(cs.Cells[0, 4].Value);
                if (Convert.ToInt32(cs.Cells[0, 5].Value) > maxValue)
                    maxValue = Convert.ToInt32(cs.Cells[0, 5].Value);
                maxValue = Convert.ToInt32(maxValue * 1.1);

                GaugeSet(((LinearGauge)UltraGauge4.Gauges[0]), maxValue, Convert.ToInt32(cs.Cells[0, 3].Value), Convert.ToInt32(row.Cells[2].Value));
                GaugeSet(((LinearGauge)UltraGauge5.Gauges[0]), maxValue, Convert.ToInt32(cs.Cells[0, 4].Value), Convert.ToInt32(row.Cells[2].Value));
                GaugeSet(((LinearGauge)UltraGauge6.Gauges[0]), maxValue, Convert.ToInt32(cs.Cells[0, 5].Value), Convert.ToInt32(row.Cells[2].Value));


                //UltraChart5.DataBind();
                //Label5.Text = String.Format(chart5_title_caption, row.Cells[0].Value.ToString());
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

        protected void UltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            
            int xOct = 0;
            int xNov = 0;
            Infragistics.UltraChart.Core.Primitives.Text decText = null;
            int year = int.Parse(selected_year.Value);
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

        protected void UltraGauge1_DataBinding(object sender, EventArgs e)
        {
            
        }






    }

}

