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
 *  Организация ритуальных услуг и содержание мест захоронения.
 */
namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._018
{
 
    public partial class Default : CustomReportPage
    {
        // --------------------------------------------------------------------

        // заголовок страницы
        private static String page_title_caption = "Организация ритуальных услуг и содержание мест захоронения";

        // заголовок для Grid1
        private static String grid1_title_caption = "Основные показатели, характеризующие качество организации ритуальных услуг";

        // заголовок для Chatr1
        private static String chart1_title_caption = "«{0}»";
        // заголовок для Chatr2
        private static String chart2_title_caption = "«{0}»";

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
        private CustomParam grid1Marks;
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                base.Page_PreLoad(sender, e);
                // установка размера диаграммы
                web_grid1.Width = (int)((screen_width - 55) );


                UltraChart1.Width = (int)((screen_width - 55) * 0.5);
                UltraChart2.Width = (int)((screen_width - 55) * 0.5);

                Label1.Width = (int)((screen_width - 55) * 0.49);
                Label2.Width = (int)((screen_width - 55) * 0.49);
                Label3.Text =RegionSettingsHelper.Instance.GetPropertyValue("chart1_title1");
                Label4.Text =RegionSettingsHelper.Instance.GetPropertyValue("chart2_title1");


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
            { }
            catch (Exception ex)
            {
                // неудачная загрузка ...
                throw new Exception(ex.Message, ex);
            }
                base.Page_Load(sender, e);
                if (!Page.IsPostBack)
                {   // опрерации которые должны выполняться при только первой загрузке страницы
                    grid1Marks = UserParams.CustomParam("grid1Marks");
                    grid1Marks = ForMarks.SetMarks(grid1Marks, ForMarks.Getmarks("grid1_mark_"), true);
                    RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                    WebAsyncRefreshPanel2.AddLinkedRequestTrigger(web_grid1);

                    last_year.Value = getLastDate(RegionSettingsHelper.Instance.GetPropertyValue("way_last_year_mark"));
                    String year = UserComboBox.getLastBlock(last_year.Value);
                    page_title.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("page_title"), year);
                    Grid1Label.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("grid1_title"), year);
                    Page.Title = page_title.Text;
                }
    
                    // заполнение UltraWebGrid данными
                    web_grid1.DataBind();
                    webGrid1ActiveRowChange(0, true);

             
                    // установка заголовков
                    
                    
                
            
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
        string[] EdIzm;
        protected void web_grid1_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid1"));
                EdIzm = new string[cs.Axes[1].Positions.Count];
                grid_master.Columns.Add();
                grid_master.Columns.Add("Показатель");
                grid_master.Columns.Add("_");
                grid_master.Columns.Add(UserComboBox.getLastBlock(last_year.Value));
                grid_master.Columns.Add("Темп роста в сравнении с предыдущим периодом");
                int index = 0;
                foreach (Position pos in cs.Axes[1].Positions)
                {   // создание списка значений для строки UltraWebGrid
                    object[] values = {
                        pos.Members[0].MemberProperties[0].Value.ToString(),
                        cs.Axes[1].Positions[pos.Ordinal].Members[0].Caption  + ", " + cs.Cells[0, pos.Ordinal].FormattedValue.ToString().ToLower(),
                        string.Format("{0:### ##0.##}", cs.Cells[1, pos.Ordinal].Value),
                        string.Format("{0:### ##0.##}", cs.Cells[2, pos.Ordinal].Value),
                        string.Format("{0:### ##0.##}", (
                                                Convert.ToDouble(cs.Cells[2, pos.Ordinal].Value)/
                                                Convert.ToDouble(cs.Cells[1, pos.Ordinal].Value)
                                                    *100)
                                                )
                    };
                    EdIzm[index] =cs.Axes[1].Positions[pos.Ordinal].Members[0].Caption  + ";" + cs.Cells[0, pos.Ordinal].FormattedValue.ToString().ToLower();
                    index++;
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
                e.Layout.Bands[0].Columns[2].Hidden = true;

                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 20 - 3;
                e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 20) * 0.4   ) - 5;

                e.Layout.Bands[0].Columns[3].Width = (int)((tempWidth - 15) * 0.2) - 5;
                //CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "### ### ### ##0.00");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "### ### ### ##0.##");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "### ### ### ##0.##");
                e.Layout.Bands[0].Columns[3].Header.Style.HorizontalAlign = HorizontalAlign.Center;

                e.Layout.Bands[0].Columns[4].Width = (int)((tempWidth - 15) * 0.2) - 5;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "### ### ### ##0.##");
                e.Layout.Bands[0].Columns[4].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[4].Header.Style.Wrap = true;
                e.Layout.Bands[0].Columns[4].Hidden = 1 == 1;
                
            }
            catch
            {
                // ошибка инициализации
            }
            //foreach(UltraGridColumn col in e.Layout.Bands[0].Columns)
            //{
            //    CRHelper.FormatNumberColumn(col, "### ### ### ##0.00");
            //}
        }

        private void webGrid1ActiveRowChange(int index, bool active)
        {
            try
            { }
            catch (Exception)
            {
                // выбор сторки пошел с ошибкой ...
            } 
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
                index = row.Index;
                string Name = "";
                //for (int i = 0; i < row.Cells[1].Value.ToString().Split(',').Length-1; i++)
                //{
                //    Name +=row.Cells[1].Value.ToString().Split(',')[i]
                //}
                //string Name = ;
                //Label1.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart1_title"), Value, EdIzm_);

                //Label2.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart2_title"), Value, EdIzm_);

                //String.Format(index.ToString());
                //String.Format(EdIzm[index]);

                string Value = EdIzm[index];
                string EdIzm_ = Value.Split(';')[1];
                String.Format(Value+"-----"+index.ToString());
                Value = Value.Split(';')[0];

                Label1.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart1_title"), Value, EdIzm_);

                Label2.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart2_title"), Value, EdIzm_);
                //UltraChart1.Tooltips.FormatString = "<SERIES_LABEL> в <ITEM_LABEL> году, "+EdIzm_+" <b><DATA_VALUE:###,##0.##></b>";
                UltraChart1.Tooltips.FormatString =  "<b><DATA_VALUE:###,##0.##></b>, "+EdIzm_;
                
 
         
        }

        protected void web_grid1_ActiveRowChange(object sender, RowEventArgs e)
        {
            webGrid1ActiveRowChange(e.Row.Index, false);
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
            
            DataTable dt = dataBind("chart1").ToTable();

            double max = double.Parse(dt.Rows[0].ItemArray[1].ToString()),
min = double.Parse(dt.Rows[0].ItemArray[1].ToString());
            for (int i = 2; i < dt.Rows[0].ItemArray.Length; i++)
            {
                if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) > max) { max = double.Parse(dt.Rows[0].ItemArray[i].ToString()); }
                if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) < min) { min = double.Parse(dt.Rows[0].ItemArray[i].ToString()); }
            }
            UltraChart1.Axis.Y.RangeMax = max * 1.1;
            UltraChart1.Axis.Y.RangeMin = min * 0.9;
            UltraChart1.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
            UltraChart1.DataSource = dt;

        }

        protected void web_grid1_InitializeRow(object sender, RowEventArgs e)
        {
            try
            {
                double rowValue = Convert.ToDouble(e.Row.Cells[4].Value);
                if ((rowValue > 0) & (rowValue < 1000))
                {
                    if (rowValue <= 100)
                        e.Row.Cells[4].Style.CssClass = "ArrowDownRed";
                    else
                        e.Row.Cells[4].Style.CssClass = "ArrowUpGreen";
                    e.Row.Cells[4].Value = e.Row.Cells[4].Value + " %";
                }
                else
                    e.Row.Cells[4].Value = "";
            }
            catch
            {
                // ошибка инициализации
            }
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            
            try
            {
                DataTable table = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart2"), "sfn", table);
                object[] values = table.Rows[0].ItemArray;
                for (int i = values.Length; i > 2; i--)
                {
                    if (i > 2)
                        values[i-1] = Convert.ToDouble(values[i-1]) / Convert.ToDouble(values[i - 2]) * 100;
                }
                //values[1] = null;

                
                table.Rows[0].ItemArray = values;
                table.Columns.Remove(table.Columns[1]);

                UltraChart2.DataSource = table.DefaultView;
                DataTable dt = table;

                double max = double.Parse(dt.Rows[0].ItemArray[1].ToString()),
                min = double.Parse(dt.Rows[0].ItemArray[1].ToString());
                for (int i = 2; i < dt.Rows[0].ItemArray.Length; i++)
                {
                    if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) > max) { max = double.Parse(dt.Rows[0].ItemArray[i].ToString()); }
                    if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) < min) { min = double.Parse(dt.Rows[0].ItemArray[i].ToString()); }
                }
                UltraChart2.Axis.Y.RangeMax = max * 1.1;
                UltraChart2.Axis.Y.RangeMin = min * 0.9;
                UltraChart2.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
                UltraChart2.AreaChart.NullHandling = Infragistics.UltraChart.Shared.Styles.NullHandling.DontPlot;

            }
            catch (Exception exception) // блок для обработки исключений
            {
                throw new Exception(exception.Message, exception);
            }
        }


        protected void UltraChart2_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
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

        protected void UltraChart2_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }
    }
}

