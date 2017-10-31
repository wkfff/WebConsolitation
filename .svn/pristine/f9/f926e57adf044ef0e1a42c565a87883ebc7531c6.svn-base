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
 *  Основные показатели капитального строительства.
 */
namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._00080
{
    public partial class Default : CustomReportPage
    {
        // --------------------------------------------------------------------

        // заголовок страницы
        private static String page_title_caption = "Основные показатели капитального строительства";
        // заголовок для Grid1
        private static String grid1_title_caption = "Основные показатели";
        // заголовок для Grid2
        private static String grid2_title_caption = "Инвестиции в основной капитал";

        // заголовок для Chatr1
        private static String chart1_title_caption = "«{0}»";
        // заголовок для Chatr2
        private static String chart2_title_caption = "Структура инвестиций в объекты производственного назначения за&nbsp;{0}&nbsp;год";
        // заголовок для Chatr3
        private static String chart3_title_caption = "Структура инвестиций в объекты непроизводственного назначения за&nbsp;{0}&nbsp;год";
        // заголовок для Chatr4
        private static String chart4_title_caption = "Источники финансирования инвестиций в основной капитал за&nbsp;{0}&nbsp;год";

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
        // --------------------------------------------------------------------
        private CustomParam grid1Marks;
        private CustomParam grid2Marks;
        private CustomParam chart2_mark;
        private CustomParam chart3_mark;
        private CustomParam chart4_mark;
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                base.Page_PreLoad(sender, e);
                // установка размера диаграммы
                web_grid1.Width = (int)((screen_width - 55));
                web_grid2.Width = (int)((screen_width - 55));
                Chart1.Width = (int)((screen_width - 55));
                Chart2.Width = (int)((screen_width - 55) * 0.33);
                Chart3.Width = (int)((screen_width - 55) * 0.33);
                Chart4.Width = (int)((screen_width - 55) * 0.33);
                //Label1.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart1_title1");
                grid1Marks = UserParams.CustomParam("grid1Marks");
                grid2Marks = UserParams.CustomParam("grid2Marks");
                chart2_mark = UserParams.CustomParam("chart2_mark");
                chart3_mark = UserParams.CustomParam("chart3_mark");
                chart4_mark = UserParams.CustomParam("chart4_mark");
            }
            catch (Exception)
            {
                // установка размеров не удалась ...
            }
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
        // --------------------------------------------------------------------

        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                base.Page_Load(sender, e);
                if (!Page.IsPostBack)
                {   // опрерации которые должны выполняться при только первой загрузке страницы
                    grid1Marks = ForMarks.SetMarks(grid1Marks, ForMarks.Getmarks("grid1_mark_"), true);
                    grid2Marks = ForMarks.SetMarks(grid2Marks, ForMarks.Getmarks("grid2_mark_"), true);
                    chart2_mark.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart2_mark");
                    chart3_mark.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart3_mark");
                    chart4_mark.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart4_mark");
                    RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;

                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(web_grid1);
                    //ORG_0003_0001._default.setChartSettings(Chart1);
                    //ORG_0003_0001._default.setChartSettings(Chart2);
                    //ORG_0003_0001._default.setChartSettings(Chart3);
                    //ORG_0003_0001._default.setChartSettings(Chart4);
                    last_year.Value = getLastDate("ОСНОВНЫЕ ПОКАЗАТЕЛИ КАПИТАЛЬНОГО СТРОИТЕЛЬСТВА");
                    String year = UserComboBox.getLastBlock(last_year.Value);
                    page_title.Text = RegionSettingsHelper.Instance.GetPropertyValue("page_title");

                    Grid1Label.Text = RegionSettingsHelper.Instance.GetPropertyValue("grid1_title");
                    web_grid1.DataBind();
                    webGrid1ActiveRowChange(0, true);

                    Grid2Label.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("grid2_title"), year);
                    web_grid2.DataBind();
                    web_grid2.Columns[web_grid2.Columns.Count - 1].Selected = 1 == 1;

                    //String year = UserComboBox.getLastBlock(last_year.Value);
                    Chart2Lab.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart2_title"), year); 
                    Chart2.DataBind();
                    Chart3Lab.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart3_title"), year); 
                    Chart3.DataBind();
                    Chart4Lab.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart4_title"), year); 
                    Chart4.DataBind();
                    //Label2.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart4_title1");
                    //Label3.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart4_title1");
                    //Label4.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart4_title1");
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
                Chart1.DataBind();

                Chart1Lab.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart1_title"), GetString_(row.Cells[1].Value.ToString()), _GetString_(row.Cells[1].Value.ToString()));
            }
            catch (Exception)
            {
                // выбор сторки пошел с ошибкой ...
            }
        }

        protected void Chart1_DataBinding(object sender, EventArgs e)
        {
            Chart1.DataSource = dataBind("chart1");
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
            try
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
            catch { return s; }


        }

        protected void InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
           // ORG_0003_0001._default.setChartErrorFont(e);
        }

        protected void web_grid1_DataBinding(object sender, EventArgs e)
        {
            try
            {
                CellSet grid_set = null;
                DataTable grid_table = new DataTable();
                // Загрузка таблицы цен и товаров в CellSet
                grid_set = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid1"));
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
                        try
                        {

                            values[i + 2] = string.Format("{0:### ### ##0.##}", grid_set.Cells[i + 1, pos.Ordinal].Value);//grid_set.Cells[i + 1, pos.Ordinal].FormattedValue.ToString();

                        } catch
                        {
                            values[i + 2] = grid_set.Cells[i + 1, pos.Ordinal].FormattedValue.ToString();
                        }
                    }
                    // заполнение строки данными
                    grid_table.Rows.Add(values);
                }
                web_grid1.DataSource = grid_table.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                throw new Exception(exception.Message, exception);
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

                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 20) * 0.3) - 5;
                for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = (int)((tempWidth - 20) * 0.7 / (e.Layout.Bands[0].Columns.Count - 2)) - 5;
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

        protected void web_grid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            try
            {
                // настройка столбцов
                double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
                e.Layout.RowSelectorStyleDefault.Width = 20 - 3;
                e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth - 20) * 0.3) - 5;
                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 20) * 0.7) - 5;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "### ### ### ##0.##");
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            }
            catch
            {
                // ошибка инициализации
            }
        }

        protected void web_grid1_ActiveRowChange(object sender, RowEventArgs e)
        {
            webGrid1ActiveRowChange(e.Row.Index, false);
        }
        DataTable Chart2grid_master = new DataTable();
        protected void Chart2_DataBinding(object sender, EventArgs e)
        {
            try
            {
               
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart2"), "_", Chart2grid_master);
                Chart2.DataSource = Chart2grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }
        DataTable Chart3grid_master = new DataTable();
        protected void Chart3_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart3"), "_", Chart3grid_master);
                Chart3.DataSource = Chart3grid_master.DefaultView;
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
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart4"), "_", Chart4grid_master);
                Chart4.DataSource = Chart4grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }

        }

        protected void web_grid2_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid2"), "Показатели", grid_master);
                object[] gridRow = grid_master.Rows[0].ItemArray;
                for (int i = 1; i != gridRow.Length; i++)
                    gridRow[i] = Convert.ToDouble(gridRow[i]) / 1000000;
                grid_master.Rows[0].ItemArray = gridRow;
                object[] gridRow2 = new object[gridRow.Length];
                gridRow2[0] = "Динамика инвестиций в основной капитал, процент";
                gridRow2[1] = "0";
                for (int i = 2; i != gridRow.Length; i++)
                {
                    gridRow2[i] = (Convert.ToDouble(gridRow[i]) / Convert.ToDouble(gridRow[i - 1]) * 100).ToString();
                }
                grid_master.Rows.Add(gridRow2);
                grid_master.Rows[0][0] = grid_master.Rows[0][0].ToString() + ", милион рублей";
                //grid_master.Rows[1].ItemArray = gridRow;

                web_grid2.DataSource = grid_master.DefaultView;
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

        protected void web_grid2_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 1)
                for (int i = 2; i < e.Row.Cells.Count; i++)
                {
                    double cellValue = Convert.ToDouble(e.Row.Cells[i].Value);
                    if ((cellValue > 0) & (cellValue < 1000))
                    {
                        if (cellValue != 100)
                        {
                            if (Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                                e.Row.Cells[i].Style.CssClass = "ArrowDownRed";
                            else
                                e.Row.Cells[i].Style.CssClass = "ArrowUpGreen";
                        }
                        //else
                            //e.Row.Cells[i].Style.CssClass = "RightYellow";
                        e.Row.Cells[i].Value = String.Format("{0:N2}", e.Row.Cells[i].Value) + "%";
                    }
                    else
                        e.Row.Cells[i].Value = "";
                }
        }

        protected void Chart2_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            //for (int i = 0; i < Chart2grid_master.Rows.Count; i++)
            //{
            //    Infragistics.UltraChart.Core.Primitives.Text textLabel = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(28, 256 + i * 20 - i, 250, 10), Chart2grid_master.Rows[i].ItemArray[0].ToString(), new Infragistics.UltraChart.Shared.Styles.LabelStyle(new Font("Microsoft Sans Serif", 8), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Far, Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal));
            //    e.SceneGraph.Add(textLabel);
            //}
        }

        protected void Chart3_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
         /*   for (int i = 0; i < Chart3grid_master.Rows.Count; i++)
            {
                Infragistics.UltraChart.Core.Primitives.Text textLabel = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(28, 274 + i * 20 - i, 250, 10), Chart3grid_master.Rows[i].ItemArray[0].ToString(), new Infragistics.UltraChart.Shared.Styles.LabelStyle(new Font("Microsoft Sans Serif", 8), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Far, Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal));
                e.SceneGraph.Add(textLabel);
            }*/
            //Infragistics.UltraChart.Core.Primitives.Text textLabel1 = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(28, 256, 250, 10), Chart3grid_master.Rows[0].ItemArray[0].ToString(), new Infragistics.UltraChart.Shared.Styles.LabelStyle(new Font("Microsoft Sans Serif", 8), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Far, Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal));
            //e.SceneGraph.Add(textLabel1);
            //Infragistics.UltraChart.Core.Primitives.Text textLabel2 = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(28, 283, 250, 10), Chart3grid_master.Rows[1].ItemArray[0].ToString(), new Infragistics.UltraChart.Shared.Styles.LabelStyle(new Font("Microsoft Sans Serif", 8), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Far, Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal));
            //e.SceneGraph.Add(textLabel2);
            //Infragistics.UltraChart.Core.Primitives.Text textLabel3 = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(28, 295, 250, 10), Chart3grid_master.Rows[2].ItemArray[0].ToString(), new Infragistics.UltraChart.Shared.Styles.LabelStyle(new Font("Microsoft Sans Serif", 8), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Far, Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal));
            //e.SceneGraph.Add(textLabel3);
            //Infragistics.UltraChart.Core.Primitives.Text textLabel4 = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(28, 313, 250, 10), Chart3grid_master.Rows[3].ItemArray[0].ToString(), new Infragistics.UltraChart.Shared.Styles.LabelStyle(new Font("Microsoft Sans Serif", 8), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Far, Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal));
            //e.SceneGraph.Add(textLabel4);
        }

        protected void Chart4_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            //for (int i = 0; i < Chart4grid_master.Rows.Count; i++)
            //{
            //    Infragistics.UltraChart.Core.Primitives.Text textLabel = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(28, 256 + i * 20 - i, 250, 10), Chart4grid_master.Rows[i].ItemArray[0].ToString(), new Infragistics.UltraChart.Shared.Styles.LabelStyle(new Font("Microsoft Sans Serif", 8), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Far, Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal));
            //    e.SceneGraph.Add(textLabel);
            //}
        }

        protected void web_grid2_ActiveRowChange(object sender, RowEventArgs e)
        {

        }

        protected void web_grid2_Click(object sender, ClickEventArgs e)
        {
            int Cellindex = web_grid2.Columns.Count-1;
            //[Период].[Год Квартал Месяц].[Год]
            
            if (e.Column != null)
            {
                Cellindex = e.Column.Index;
            }
            else
            {
                Cellindex = e.Cell.Column.Index;
            }
            if (Cellindex == 0)
            {
                Cellindex++;
            }
            String year = web_grid2.Columns[Cellindex].Header.Caption;
            last_year.Value = string.Format("[Период].[Год Квартал Месяц].[Год].[{0}]",year);
            Chart2Lab.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart2_title"), year);
            Chart2.DataBind();
            Chart3Lab.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart3_title"), year);
            Chart3.DataBind();
            Chart4Lab.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart4_title"), year);
            Chart4.DataBind();
            web_grid2.Columns[Cellindex].Selected = 1 == 1;
        }

        protected void Chart2_ChartDrawItem(object sender, Infragistics.UltraChart.Shared.Events.ChartDrawItemEventArgs e)
        {
            {
                addY = 0;
                UltraChart C = Chart2;
                //UltraChart UltraChart_ = (UltraChart)sender;
                //устанавливаем ширину текста легенды 
                Infragistics.UltraChart.Core.Primitives.Text text = e.Primitive as Infragistics.UltraChart.Core.Primitives.Text;
                if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
                {
                    int widthLegendLabel;
                    //UltraChart_.Legend.Location =
                    if ((C.Legend.Location == Infragistics.UltraChart.Shared.Styles.LegendLocation.Top) || (C.Legend.Location == Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom))
                    {
                        widthLegendLabel = (int)C.Width.Value - 20;

                    }
                    else
                    {
                        widthLegendLabel = ((int)C.Legend.SpanPercentage * (int)C.Width.Value / 100) - 20;
                    }

                    widthLegendLabel -= C.Legend.Margins.Left + Chart2.Legend.Margins.Right;
                    if (text.labelStyle.Trimming != StringTrimming.None)
                    {
                        text.bounds.Width = widthLegendLabel;
                    }
                    text.labelStyle.Dy += addY;
                    string Capt = text.GetTextString();
                    if (Capt.Length > 60)
                    {
                        int i = 60;
                        for (; Capt[i] != ' '; i--) { }

                        text.SetTextString(Capt.Insert(i, "\n"));
                        //Боланз)
                        text.labelStyle.Dy -= 6;
                        addY += 6;
                    }

                }

            }
        }

        protected void Chart3_ChartDrawItem(object sender, Infragistics.UltraChart.Shared.Events.ChartDrawItemEventArgs e)
        {
            {
                addY = 0;
                UltraChart C = Chart3;
                //UltraChart UltraChart_ = (UltraChart)sender;
                //устанавливаем ширину текста легенды 
                Infragistics.UltraChart.Core.Primitives.Text text = e.Primitive as Infragistics.UltraChart.Core.Primitives.Text;
                if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
                {
                    int widthLegendLabel;
                    //UltraChart_.Legend.Location =
                    if ((C.Legend.Location == Infragistics.UltraChart.Shared.Styles.LegendLocation.Top) || (C.Legend.Location == Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom))
                    {
                        widthLegendLabel = (int)C.Width.Value - 20;

                    }
                    else
                    {
                        widthLegendLabel = ((int)C.Legend.SpanPercentage * (int)C.Width.Value / 100) - 20;
                    }

                    widthLegendLabel -= C.Legend.Margins.Left + Chart3.Legend.Margins.Right;
                    if (text.labelStyle.Trimming != StringTrimming.None)
                    {
                        text.bounds.Width = widthLegendLabel;
                    }
                    text.labelStyle.Dy += addY;
                    string Capt = text.GetTextString();
                    if (Capt.Length > 60)
                    {
                        int i = 60;
                        for (; Capt[i] != ' '; i--) { }

                        text.SetTextString(Capt.Insert(i, "\n"));
                        //Боланз)
                        text.labelStyle.Dy -= 6;
                        addY += 6;
                    }

                }

            }
        }
        int addY = 0;
        protected void Chart4_ChartDrawItem(object sender, Infragistics.UltraChart.Shared.Events.ChartDrawItemEventArgs e)
        {
            {
                addY = 0;
                UltraChart C = Chart4;
                //UltraChart UltraChart_ = (UltraChart)sender;
                //устанавливаем ширину текста легенды 
                Infragistics.UltraChart.Core.Primitives.Text text = e.Primitive as Infragistics.UltraChart.Core.Primitives.Text;
                if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
                {
                    int widthLegendLabel;
                    //UltraChart_.Legend.Location =
                    if ((C.Legend.Location == Infragistics.UltraChart.Shared.Styles.LegendLocation.Top) || (C.Legend.Location == Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom))
                    {
                        widthLegendLabel = (int)C.Width.Value - 20;

                    }
                    else
                    {
                        widthLegendLabel = ((int)C.Legend.SpanPercentage * (int)C.Width.Value / 100) - 20;
                    }

                    widthLegendLabel -= C.Legend.Margins.Left + Chart4.Legend.Margins.Right;
                    if (text.labelStyle.Trimming != StringTrimming.None)
                    {
                        text.bounds.Width = widthLegendLabel;
                    }
                    text.labelStyle.Dy += addY;
                    string Capt = text.GetTextString();
                    if (Capt.Length > 60)
                    {
                        int i = 60;
                        for (; Capt[i] != ' '; i--) { }

                        text.SetTextString(Capt.Insert(i, "\n"));
                        //Боланз)
                        text.labelStyle.Dy -= 6;
                        addY += 6;
                    }

                }

            }
        }

    }

}

