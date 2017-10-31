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

using System.Collections.Generic;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;

/**
 *  Информация о состоянии жилищного фонда.
 */
namespace Krista.FM.Server.Dashboards.reports.PMO_0001_0011
{
    public partial class Default : CustomReportPage
    {
        // --------------------------------------------------------------------
        private static String chart_error_message = "в настоящий момент данные отсутствуют";
        // заголовок страницы
        private static String page_title_caption = "Информация о состоянии жилищного фонда на {0} год <nobr>({1})</nobr>";
        // заголовок для Grid
        private static String grid_title_caption = "Основные показатели жилищного фонда в {0} году";
        // параметр для выбранной.текущей территории
        private CustomParam current_region { get { return (UserParams.CustomParam("current_region")); } }
        // параметр для последней актуальной даты
        private CustomParam last_year; //{ get { return (UserParams.CustomParam("last_year")); } }
        // параметр для выбранной/текущей отрасли
        private CustomParam branch { get { return (UserParams.CustomParam("current_branch")); } }
        // ширина экрана в пикселях
        private Int32 screen_width { get { return (int)Session["width_size"]; } }

        // заголовки столбцов для UltraWebGrid
        private static String[] grid_columns;// = { "Показатели", "Ед. изм.", "Значение" };
        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }

        // параметр для последней актуальной даты
        //private CustomParam last_year;//{ get { return (UserParams.CustomParam("last_year")); } }

        private CustomParam marks;// { get { return (new CustomParam("2")); } }
        private CustomParam norefresh;
        private CustomParam norefresh2;

        public static void setChartSettings(UltraChart chart)
        {
            //chart.Legend.BackgroundColor = Color.Empty;
            //chart.Legend.BorderColor = Color.Empty;
        }


        // --------------------------------------------------------------------

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                marks = UserParams.CustomParam("marks");
                last_year = UserParams.CustomParam("last_year");
                base.Page_PreLoad(sender, e);
                System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
                string BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
                // установка размера диаграммы
                norefresh = UserParams.CustomParam("r");
                norefresh2 = UserParams.CustomParam("r2");
                web_grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.5-5);//(int)((screen_width - 55) * 0.38);
                chart_avg_count.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.5-5);
                PopupInformer1.HelpPageUrl = RegionSettingsHelper.Instance.GetPropertyValue("NameHelpPage");
                if (BN == "FIREFOX")
                {
                    web_grid.Height = 321;
                    chart_avg_count.Height = 295;
                    chart_pie1.Height = 285;
                    chart_pie2.Height = 285;
                }
                else
                {
                    web_grid.Height = 286;
                    chart_avg_count.Height = 280;
                    chart_pie1.Height = 280;
                    chart_pie2.Height = 280;
                }
                chart_pie1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.5-5);
                chart_pie2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.5-5);
            }
            catch (Exception)
            {
                // установка размеров не удалась ...
            }
        }

        // --------------------------------------------------------------------
        ArrayList Getmarks(string prefix)
        {
            ArrayList AL = new ArrayList();
            string CurMarks = RegionSettingsHelper.Instance.GetPropertyValue(prefix+"1");
            int i = 2;
            while (!string.IsNullOrEmpty(CurMarks))
            {
                AL.Add(CurMarks.ToString());

                CurMarks = RegionSettingsHelper.Instance.GetPropertyValue(prefix + i.ToString());

                i++;
            }

            return AL;
        }

        Dictionary<string, int> ForParam2(string sql)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            //if (sql != "")
            {
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));


                for (int i = cs.Axes[1].Positions.Count - 1; i >= 0; i--)
                {
                    d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
                }
            }
            return d;
        }

        ArrayList itemGrid,itemChart2,itemChart3;

        protected override void Page_Load(object sender, EventArgs e)
        {
           // try
           // {
                base.Page_Load(sender, e);
                if (!Page.IsPostBack)
                {   // опрерации которые должны выполняться при только первой загрузке страницы
                   // RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;

                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(web_grid);

                    setChartSettings(chart_avg_count);
                    setChartSettings(chart_pie1);
                    setChartSettings(chart_pie2);

                    
                    //Date.SetСheckedState(p1.Value, true);

                }
                // установка параметра территории
                current_region.Value = baseRegion.Value;

                marks.Value = "";
                int i;
                itemGrid = Getmarks("grid_mark_");
                for (i = 0; i < itemGrid.Count - 1; i++)
                {
                    marks.Value += itemGrid[i].ToString() + ",";
                }
                marks.Value += itemGrid[itemGrid.Count - 1].ToString();

                if (!Page.IsPostBack) { Date.FillDictionaryValues(ForParam2("last_date")); };

                last_year.Value = string.Format("[Период].[Год Квартал Месяц].[Год].[{0}]", Date.SelectedValue);
                //last_year.Value = getLastDate();
                //Label1.Text = last_year.Value;
                String year = Date.SelectedValue; //UserComboBox.getLastBlock(last_year.Value);

                page_title_caption = RegionSettingsHelper.Instance.GetPropertyValue("titlePage");
                Label2.Text = RegionSettingsHelper.Instance.GetPropertyValue("PageSubTitle");
                grid_title_caption = RegionSettingsHelper.Instance.GetPropertyValue("titleGrid");

                grid_columns = RegionSettingsHelper.Instance.GetPropertyValue("titleGridColumn").Split(';');

                

                // установка заголовка для страницы
                page_title.Text = String.Format(
                    page_title_caption, year,
                    UserComboBox.getLastBlock(current_region.Value));
                Page.Title = page_title.Text;
                grid_caption.Text = String.Format(grid_title_caption, year);
                // заполнение UltraWebGrid данными
                
                web_grid.DataBind();
                marks.Value = "";
                itemChart2 = Getmarks("chart2_mark_");
                for (i = 0; i < itemChart2.Count - 1; i++)
                {
                    web_grid.Rows[0].Selected = 1 == 1;
                    web_grid.Rows[0].Activate();
                    web_grid.Rows[0].Activated = 1 == 1;

                    
                    marks.Value += itemChart2[i].ToString() + ",";
                }
                chart_avg_count.DataBind();
                marks.Value += itemChart2[itemChart2.Count - 1].ToString();
                
                //marks.Value += itemChart3[itemChart2.Count - 1].ToString();
                //chart_pie1.DataBind();
                chart_pie1.DataBind();
                marks.Value = "";
                itemChart3 = Getmarks("chart3_mark_");
                
                for (i = 0; i < itemChart3.Count - 1; i++)
                {
                    marks.Value += itemChart3[i].ToString() + ",";
                }
                marks.Value += itemChart3[itemChart3.Count - 1].ToString();
                
                chart_pie2.DataBind();
                chart_pie2.Tooltips.FormatString = RegionSettingsHelper.Instance.GetPropertyValue("chart3_tooltips");
                chart_pie1.Tooltips.FormatString = RegionSettingsHelper.Instance.GetPropertyValue("chart2_tooltips");
                // установка активной строки в UltraWebGrid
                

                webGridActiveRowChange(0, true);
                web_grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
                chart_avg_count.Tooltips.FormatString = "<b><DATA_VALUE:### ### ##0.##></b>, "+web_grid.Rows[0].Cells[1].Text.ToLower();
                if (norefresh.Value != Date.SelectedValue)
                {
                    norefresh2.Value = "no";
                }
                else
                {
                    norefresh2.Value = "Yes";
                };

                norefresh.Value = Date.SelectedValue;
         //   }
         //   catch (Exception)
         //   {
                // неудачная загрузка ...
          //  }
        }

        // --------------------------------------------------------------------

        /** <summary>
         *  Метод получения последней актуальной даты 
         *  </summary>
         */
        //private String getLastDate()
        //{
        //    try
        //    {
        //        CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("last_date"));
        //        return cs.Axes[1].Positions[0].Members[0].ToString();
        //    }
        //    catch (Exception e)
        //    {
        //        return null;
        //    }
        //}

        // --------------------------------------------------------------------

        protected void web_grid_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_table = new DataTable();
                // Установка параметра последней актуальной даты
                UserParams.CustomParam("period_last_date").Value = last_year.Value;
                CellSet grid_set = null;
                // Загрузка таблицы цен и товаров в CellSet
                grid_set = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid"));
                // Добавление столбцов в таблицу и заполнение данных в DataTable
                for (int i = 0; i < grid_columns.Length; ++i)
                {   // установка заголовков для столбцов UltraWebGrid
                    grid_table.Columns.Add(grid_columns[i]);
                }

                foreach (Position pos in grid_set.Axes[1].Positions)
                {   // создание списка значений для строки UltraWebGrid

                    object[] values = {
                        //pos.Ordinal + 1,
                        grid_set.Axes[1].Positions[pos.Ordinal].Members[0].Caption,
//                      grid_names[grid_set.Axes[1].Positions[pos.Ordinal].Members[0].Caption],
                        pos.Members[0].MemberProperties[0].Value.ToString(),
                        String.Format("{0:# ##0.00}", float.Parse(grid_set.Cells[0, pos.Ordinal].Value.ToString()))
                        
                    };
                    // заполнение строки данными
                    grid_table.Rows.Add(values);
                }
                // установка источника данных для UltraWebGrid
                web_grid.DataSource = grid_table.DefaultView; 
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }

        protected void web_grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
           
            try
                
            {
                System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
                string BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
                double Coef = 1;

                if (BN == "IE") { Coef = 1; }
                if (BN == "FIREFOX") { Coef = 0.95; }
                if (BN == "APPLEMAC-SAFARI") { Coef = 1; };
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "### ### ##0.00");
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
                // настройка столбцов
                double tempWidth = web_grid.Width.Value*Coef;
                e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth - 22) * 0.8) - 5;
                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 22) * 0.23) - 5;
                e.Layout.Bands[0].Columns[2].Width = (int)((tempWidth - 22) * 0.17) - 5;
                
                //web_grid.Bands[0].Columns[0].Width = 40;
                //web_grid.Bands[0].Columns[0].Width = 260;
                //web_grid.Bands[0].Columns[1].Width = 110;
                //web_grid.Bands[0].Columns[2].Width = 60;
                // установка формата отображения ячеек
                
                e.Layout.Bands[0].Columns[1].Hidden = 1 == 1;
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
                string s = DataProvider.GetQueryText(query_name);
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(s, "sfn", table);
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
                    if (active) row.Activate();
                    // получение заголовка выбранной отрасли
                  //  string branch_name = row.Cells[0].Value.ToString().Split(',')[0];

                    string branch_name = row.Cells[0].Value.ToString().Split(',')[0];
                    int j = 1;
                    for (j = 1; j < row.Cells[0].Value.ToString().Split(',').Length - 1; j++)
                    {
                        branch_name = branch_name + "," + row.Cells[0].Value.ToString().Split(',')[j];
                    }
                    // установка параметра выбранной отрасли
                    //if (branch_name == "Ввод в действие жилых объектов")
                    //{
                    //    branch.Value = "[Ввод в действие жилья, объектов соцкультбыта и здравоохранения].[" + branch_name + "]";
                    //}
                    //else
                    //{
                    //    branch.Value = "[ Жилищный фонд].[" + branch_name + "]";
                    //}
                    // установка заголовков для диаграмм
                    int i;
                    for (i = 0; itemGrid.Count > i; i++)
                    {
                        if (branch_name == UserComboBox.getLastBlock(itemGrid[i].ToString()))
                        {
                            break;
                        }

                    }
                    web_grid.Rows[index].Selected = 1 == 1;
                    web_grid.Rows[index].Activate();
                    web_grid.Rows[index].Activated = 1 == 1;


                    marks.Value = itemGrid[i].ToString();
                    Label1.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleChartDynamic"), branch_name, row.Cells[1].Value.ToString().ToLower());

                    Label3.Text = RegionSettingsHelper.Instance.GetPropertyValue("titleChart1");
                    Label4.Text = RegionSettingsHelper.Instance.GetPropertyValue("titleChart2");
                
                    //"«" + branch_name + ", " + row.Cells[1].Value.ToString().ToLower() + "»";
                    // загрузка данных для диаграмм

                    chart_avg_count.DataBind();
                    
                
            }
            catch (Exception)
            {
                // выбор сторки пошел с ошибкой ...
            }
        }

        // --------------------------------------------------------------------

        protected void web_grid_ActiveRowChange(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
                webGridActiveRowChange(e.Row.Index, false);
                chart_avg_count.Tooltips.FormatString = "<b><DATA_VALUE:### ### ##0.##></b>, " + e.Row.Cells[1].Text.ToLower();
        }

        protected void chart_avg_count_DataBinding(object sender, EventArgs e)
        {
            try
            {
                    //chart_avg_count.DataSource = dataBind("chart");

                    DataTable dt = new DataTable();
                    DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart"), "fdf", dt);
                    double max = double.Parse(dt.Rows[0].ItemArray[1].ToString()),
                        min = double.Parse(dt.Rows[0].ItemArray[1].ToString());
                    try
                    {
                        for (int i = 2; i < dt.Rows[0].ItemArray.Length; i++)
                        {
                            if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) > max) { max = double.Parse(dt.Rows[0].ItemArray[i].ToString()); }
                            if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) < min) { min = double.Parse(dt.Rows[0].ItemArray[i].ToString()); }
                        }
                    }
                    catch { }
                    chart_avg_count.Axis.Y.RangeMax = max * 1.1;
                    chart_avg_count.Axis.Y.RangeMin = min * 0.9;
                    chart_avg_count.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
                    chart_avg_count.DataSource = dt;
            }
            catch { }
        }

        protected void chart_pie1_Init(object sender, EventArgs e)
        {
            try
            {
                // добавляем метки со значениями на диаграмму 
                // из дизайнера почему-то они не добавляются
                Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance text =
                    new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance();
                text.Row = -2;
                text.Column = -2;
                text.ItemFormatString = "<DATA_VALUE:0,0>";
                text.VerticalAlign = System.Drawing.StringAlignment.Far;
                text.Visible = true;
                text.ChartTextFont = new Font("Arial", 8, FontStyle.Bold);
                chart_pie1.SplineAreaChart.ChartText.Add(text);
            }
            catch
            {
                // ошибка инициализации
            }
        }

        protected void chart_pie_DataBinding(object sender, EventArgs e)
        {
            //chart_pie1.DataSource = dataBind("chart");
           
                DataTable dt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart"), "fdf", dt);
                double max = double.Parse(dt.Rows[0].ItemArray[1].ToString()),
                    min = double.Parse(dt.Rows[0].ItemArray[1].ToString());
                
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int j = 1; j < dt.Rows[i].ItemArray.Length; j++)
                        {
                            if (double.Parse(dt.Rows[i].ItemArray[j].ToString()) > max) { max = double.Parse(dt.Rows[i].ItemArray[j].ToString()); }
                            if (double.Parse(dt.Rows[i].ItemArray[j].ToString()) < min) { min = double.Parse(dt.Rows[i].ItemArray[j].ToString()); }
                        }
                    }
                
                chart_pie1.Axis.Y.RangeMax = max * 1.1;
                chart_pie1.Axis.Y.RangeMin = min*0.9;
                chart_pie1.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
                chart_pie1.Axis.Y.TickmarkInterval = (chart_pie1.Axis.Y.RangeMax - chart_pie1.Axis.Y.RangeMin) / 10;
                chart_pie1.DataSource = dt;
         
        }

        protected void chart_pie2_DataBinding(object sender, EventArgs e)
        {
            chart_pie2.DataSource = dataBind("chart");
        }

        public static void setChartErrorFont(Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = chart_error_message;
            e.LabelStyle.Font = new Font("Verdana", 20);
            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
        }

        protected void chart_avg_count_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void chart_pie1_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void chart_pie2_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void chart_avg_count_InvalidDataReceived1(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void chart_pie1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            int xOct = 0;
            int xNov = 0;
            Text decText = null;
            int year = int.Parse(Date.SelectedValue);//UserComboBox.getLastBlock(last_year.Value));
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

        protected void web_grid_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[0].Text = e.Row.Cells[0].Text + ", " + e.Row.Cells[1].Text.ToLower();
        }

        protected void chart_pie2_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }

    }

}

