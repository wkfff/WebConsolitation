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
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
/**
 *  Основные показатели социально-экономического развития.
 */
namespace Krista.FM.Server.Dashboards.reports.PMO_0001_00030
{
    public partial class Default : CustomReportPage
    {
        // --------------------------------------------------------------------

        // заголовок страницы
        private static String page_title_caption = "Основные показатели социально-экономического развития по данным на {0} год <br>({1})</br>";
        // таблица склонения имен месяцев
        private static Hashtable month_names = monthNames();
        // параметр для выбранной.текущей территории
        private CustomParam current_region { get { return (UserParams.CustomParam("current_region")); } }
        // параметр для последней актуальной даты
        private CustomParam last_year { get { return (UserParams.CustomParam("last_year")); } }
        // параметр для выбранного/текущего показателя
        private CustomParam branch { get { return (UserParams.CustomParam("current_branch")); } }
        // ширина экрана в пикселях
        private Int32 screen_width { get { return (int)Session["width_size"]; } }
        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam marks;
        private CustomParam chart_pie1_par;
        private CustomParam chart_pie2_par;
        private CustomParam chart_pie3_par;

        string BN = "IE";
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
                BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
                base.Page_PreLoad(sender, e);

                double Coef = 1;
                if (BN == "IE") { Coef = 1; }
                if (BN == "FIREFOX") { Coef = 0.999; }
                if (BN == "APPLEMAC-SAFARI") { Coef = 1.027; };

                marks = UserParams.CustomParam("marks");
                chart_pie1_par = UserParams.CustomParam("chart_pie1_par");
                chart_pie2_par = UserParams.CustomParam("chart_pie2_par");
                chart_pie3_par = UserParams.CustomParam("chart_pie3_par");
                
                // установка размера диаграмм
                web_grid.Width = (int)((screen_width -50) * 0.35);
                chart_avg_count.Width = (int)((screen_width -50) * 0.635);


                web_grid.Height = CRHelper.GetGridHeight(253);
                chart_avg_count.Height = CRHelper.GetChartHeight(250);

                chart_pie1.Width = (int)((screen_width - 70) * 0.33);
                chart_pie2.Width = (int)((screen_width - 70) * 0.33);
                chart_pie3.Width = (int)((screen_width - 70) * 0.33);
                Label3.Width = chart_pie1.Width;
                Label4.Width = chart_pie2.Width;
                Label5.Width = chart_pie3.Width;
                
                
                

                PopupInformer1.HelpPageUrl = RegionSettingsHelper.Instance.GetPropertyValue("nameHelpPage");
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
        protected override void Page_Load(object sender, EventArgs e)
        {
          
                base.Page_Load(sender, e);
                string regionS = RegionSettings.Instance.Id;
            //    RegionSettingsHelper.Instance.SetWorkingRegion("Novoorsk");
                baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                if (!Page.IsPostBack)
                {   // опрерации которые должны выполняться при только первой загрузке страницы
                   

                    marks = ForMarks.SetMarks(marks,ForMarks.Getmarks("grid_mark_"),true);
                    chart_pie1_par.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart2_mark_1");
                    chart_pie2_par.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart3_mark_1");
                    chart_pie3_par.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart4_mark_1");
                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(web_grid);

                  /*  ORG_0003_0001._default.setChartSettings(chart_avg_count);
                    ORG_0003_0001._default.setChartSettings(chart_pie1);
                    ORG_0003_0001._default.setChartSettings(chart_pie2);
                    ORG_0003_0001._default.setChartSettings(chart_pie3);*/
                }
                
                // установка параметра территории
                current_region.Value = baseRegion.Value;

                last_year.Value = getLastDate();
                String year = UserComboBox.getLastBlock(last_year.Value);

                page_title_caption = RegionSettingsHelper.Instance.GetPropertyValue("titlePage");
                // установка заголовка для страницы
                page_title.Text = String.Format(
                    page_title_caption, year,
                    UserComboBox.getLastBlock(current_region.Value));
                // заполнение UltraWebGrid данными
                Page.Title = page_title.Text;
                SubTitlePage.Text = RegionSettingsHelper.Instance.GetPropertyValue("SubTitlePage");
                web_grid.DataBind();
                // заполнение Carts данными
                if (!Page.IsPostBack)
                {
                    web_grid.Rows[0].Activated = 1 == 1;
                    web_grid.Rows[0].Activate();
                    web_grid.Rows[0].Selected = 1 == 1;
                }
                chart_avg_count.Tooltips.FormatString = "<b><DATA_VALUE:### ##0.##></b>, " +web_grid.Rows[0].Cells[2].Text.ToLower();
                chart_pie1.DataBind();
                chart_pie2.DataBind();
                chart_pie3.DataBind();
                // установка активной строки в UltraWebGrid
                webGridActiveRowChange(0, true);
                
                Label6.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleGrid"),year,UserComboBox.getLastBlock(current_region.Value));
                string title = web_grid.Rows[0].Cells[0].Text.Remove(web_grid.Rows[0].Cells[0].Text.LastIndexOf(',')); //web_grid.Rows[0].Cells[0].Text.Split(',')[0];
                Label1.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart"), title, web_grid.Rows[0].Cells[2].Text.ToLower());//RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart");
                page_title.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titlePage"), year, UserComboBox.getLastBlock(current_region.Value));
                web_grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
                RegionSettingsHelper.Instance.SetWorkingRegion(regionS);
                chart_pie1.Tooltips.FormatString = "<ITEM_LABEL>";
                chart_pie2.Tooltips.FormatString = "<ITEM_LABEL>";
                chart_pie3.Tooltips.FormatString = "<ITEM_LABEL>";
         
        }

        // --------------------------------------------------------------------

        /** <summary>
         *  Преобразование члена измерения времени в строку формата ЧЧ.ММ.ГГГГ
         * 
         *  до преобразования - [Период].[День].[Данные всех периодов].[2008].[Полугодие 2].[Квартал 4].[Декабрь].[1]
         *  после преобразования - 1 декабря 2008
         *  </summary>
         */
        public static String mdxTime2String(String str)
        {
            if (str == null) return null;
            String[] list = str.Split('.');
            for (int i = 0; i < list.Length; ++i)
            {
                list[i] = list[i].Replace("[", "");
                list[i] = list[i].Replace("]", "");
            }
            return (list.Length > 7) ?
                list[7] + " " + month_names[list[6]] + " " + list[3] :
                null;
        }

        // --------------------------------------------------------------------

        /** <summary>
         *  Метод получения последней актуальной даты 
         *  </summary>
         */
        private String getLastDate()
        {
            try
            {
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("last_date"));
                return cs.Axes[1].Positions[cs.Axes[1].Positions.Count-1].Members[0].ToString();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        // --------------------------------------------------------------------
        /** <summary>
         *  Создание таблицы имен для преобразования названий месяцев 
         *  </summary>
         */
        private static Hashtable monthNames()
        {
            Hashtable table = new Hashtable();
            String[] data = 
            {
                "Январь", "января",
                "Февраль", "февраля",
                "Март", "марта",
                "Апрель", "апреля",
                "Май", "мая",
                "Июнь", "июня",
                "Июль", "июля",
                "Август", "августа",
                "Сентябрь", "сентября",
                "Октябрь", "октября",
                "Ноябрь", "ноября",
                "Декабрь", "декабря",
            };
            for (int i = 0; i < data.Length; i += 2)
            {
                table.Add(data[i], data[i + 1]);
            }
            return table;
        }

        // --------------------------------------------------------------------

        protected void web_grid_DataBinding(object sender, EventArgs e)
        {
            // Установка параметра последней актуальной даты
            UserParams.CustomParam("period_last_date").Value = getLastDate();
            DataTable chart_table = new DataTable();
            string query = DataProvider.GetQueryText("grid");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Предприятия, учреждения и организации", chart_table);
            chart_table.Columns[1].Caption = "Число";
            web_grid.DataSource = chart_table.DefaultView;
        }

        protected void web_grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            // настройка столбцов
            

            double Coef = 1;
            if (BN == "IE") { Coef = 1; }
            if (BN == "FIREFOX") { Coef = 0.98; }
            if (BN == "APPLEMAC-SAFARI") { Coef = 1.02; };
            double tempWidth = web_grid.Width.Value * Coef - 25;
            web_grid.DisplayLayout.RowSelectorStyleDefault.Width = 20 - 2;
            web_grid.Columns[0].Width = (int)((tempWidth) * 0.8) - 5;
            web_grid.Columns[1].Width = (int)((tempWidth) * 0.2) - 5;
            web_grid.Bands[0].Columns[0].CellStyle.Wrap = true;
            web_grid.Bands[0].Columns[0].Header.Style.Wrap = true;
            e.Layout.Bands[0].Columns[0].Header.Caption = "Показатель";
            e.Layout.Bands[0].Columns[1].Header.Caption = "Значение";
            // установка формата отображения ячеек
            CRHelper.FormatNumberColumn(web_grid.Bands[0].Columns[1], "### ##0.00");
            e.Layout.Bands[0].Columns[2].Hidden = true;
            //### ##0.##


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
        
        private void webGridActiveRowChange(Int32 index, bool active)
        {
            try
            {
                // получаем выбранную строку
                UltraGridRow row = web_grid.Rows[index];
                // устанавливаем ее активной, если необходимо
                if (active) row.Activate();
                // получение заголовка выбранного показателя
                string s = row.Cells[0].Value.ToString().Split(',')[0];
                for (int i = 1; i < row.Cells[0].Value.ToString().Split(',').Length-1; i++)
                {
                    s +=","+ row.Cells[0].Value.ToString().Split(',')[i];
                }
                String branch_name = s;
                // установка параметра выбранного показателя
                branch.Value = branch_name;
                // загрузка данных для диаграммы
                chart_avg_count.DataBind();
                chart_avg_count.Tooltips.FormatString = "<b><DATA_VALUE:### ##0.##></b>, " + row.Cells[2].Text.ToLower();
                //chart_avg_wage.DataBind();
                // установка заголовка для диаграммы
                string title = row.Cells[0].Text.Remove(row.Cells[0].Text.LastIndexOf(',')); 
                Label1.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart"),title,row.Cells[2].Text.ToLower());
                
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
        }

        protected void chart_avg_count_DataBinding(object sender, EventArgs e)
        {
            
                DataTable dt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart_avg_count"), "df", dt);
                double max = double.Parse(dt.Rows[0].ItemArray[1].ToString()),
                    min = double.Parse(dt.Rows[0].ItemArray[1].ToString());
                try
                {
                    for (int i = 1; i < dt.Rows[0].ItemArray.Length; i++)
                    {
                        if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) > max) { max = double.Parse(dt.Rows[0].ItemArray[i].ToString()); }
                        if (double.Parse(dt.Rows[0].ItemArray[i].ToString()) < min) { min = double.Parse(dt.Rows[0].ItemArray[i].ToString()); }
                    }
                }
                catch { }

                chart_avg_count.Axis.Y.RangeType = AxisRangeType.Custom;
                chart_avg_count.Axis.Y.RangeMax = max + 10;
                chart_avg_count.Axis.Y.RangeMin = 0;
                chart_avg_count.Axis.Y.TickmarkInterval = (double)((chart_avg_count.Axis.Y.RangeMax) / 10);
                chart_avg_count.DataSource = dt;
           
        }

        DataTable chart_pie1DT;
        protected void chart_pie_DataBinding(object sender, EventArgs e)
        {
            chart_pie1DT = new DataTable();
            DataTable dt = new DataTable();
            CellSet cs;
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart_pie1"), "df", chart_pie1DT);
            cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("chart_pie1"));
            dt.Columns.Add(chart_pie1DT.Columns[0].ColumnName, chart_pie1DT.Columns[0].DataType);
            dt.Columns.Add(chart_pie1DT.Columns[chart_pie1DT.Columns.Count - 1].ColumnName, chart_pie1DT.Columns[chart_pie1DT.Columns.Count - 1].DataType);
            object[] o=new object[dt.Columns.Count];
            int j = 0;
            for (int i = j = 0; j < cs.Axes[1].Positions.Count; i++)
            {
                if (chart_pie1DT.Rows[i].ItemArray[chart_pie1DT.Rows[i].ItemArray.Length - 1].ToString() != "")
                {
                    o[0] = chart_pie1DT.Rows[i].ItemArray[0].ToString() + ", <b>" + String.Format("{0:# ##0.##}", double.Parse(chart_pie1DT.Rows[i].ItemArray[chart_pie1DT.Rows[i].ItemArray.Length - 1].ToString())) + ",</b> " + cs.Axes[1].Positions[j].Members[0].MemberProperties[0].Value.ToString().ToLower();
                    o[1] = chart_pie1DT.Rows[i].ItemArray[chart_pie1DT.Rows[i].ItemArray.Length-1];
                    dt.Rows.Add(o);
                }
                else
                {
                    chart_pie1DT.Rows.Remove(chart_pie1DT.Rows[i]);
                    i -= 1;
                }
                j += 1;
             
            }
            chart_pie1.DataSource = dt;//chart_pie1DT;
            Label3.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("titlechart1"), chart_pie1DT.Columns[chart_pie1DT.Columns.Count - 1].ColumnName);
        }
        DataTable chart_pie2DT;
        protected void chart_pie2_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            CellSet cs;
            chart_pie2DT = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart_pie2"), "df", chart_pie2DT);
            cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("chart_pie2"));
            dt.Columns.Add(chart_pie2DT.Columns[0].ColumnName, chart_pie2DT.Columns[0].DataType);
            dt.Columns.Add(chart_pie2DT.Columns[chart_pie2DT.Columns.Count - 1].ColumnName, chart_pie2DT.Columns[chart_pie2DT.Columns.Count - 1].DataType);
            int j = 0;
            object[] o = new object[dt.Columns.Count];
            for (int i = j = 0; j < cs.Axes[1].Positions.Count; i++)
            {
                if (chart_pie2DT.Rows[i].ItemArray[chart_pie2DT.Rows[i].ItemArray.Length - 1].ToString() != "")
                {
                    o[0] = chart_pie2DT.Rows[i].ItemArray[0].ToString() + ", <b>" + String.Format("{0:# ##0.##}", double.Parse(chart_pie2DT.Rows[i].ItemArray[chart_pie2DT.Rows[i].ItemArray.Length - 1].ToString())) + ",</b> " + cs.Axes[1].Positions[j].Members[0].MemberProperties[0].Value.ToString().ToLower();
                    o[1] = chart_pie2DT.Rows[i].ItemArray[chart_pie2DT.Rows[i].ItemArray.Length - 1];
                    dt.Rows.Add(o);
                }
                else
                {
                    chart_pie2DT.Rows.Remove(chart_pie2DT.Rows[i]);
                    i -= 1;
                }
                j += 1;
            }
            chart_pie2.DataSource = dt;//chart_pie2DT;
            Label4.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("titlechart2"), chart_pie2DT.Columns[chart_pie2DT.Columns.Count - 1].ColumnName);
        }
        DataTable chart_pie3DT;
        protected void chart_pie3_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            CellSet cs;
            chart_pie3DT = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart_pie3"), "df", chart_pie3DT);
            cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("chart_pie3"));
            dt.Columns.Add(chart_pie3DT.Columns[0].ColumnName, chart_pie3DT.Columns[0].DataType);
            dt.Columns.Add(chart_pie3DT.Columns[chart_pie3DT.Columns.Count - 1].ColumnName, chart_pie3DT.Columns[chart_pie3DT.Columns.Count-1].DataType);
            int j = 0;
            object[] o = new object[dt.Columns.Count];
            for (int i=j= 0;  j< cs.Axes[1].Positions.Count; i++)
            {
                if (chart_pie3DT.Rows[i].ItemArray[chart_pie3DT.Rows[i].ItemArray.Length - 1].ToString() != "")
                {
                    o[0] = chart_pie3DT.Rows[i].ItemArray[0].ToString() + ", <b>" + String.Format("{0:# ##0.##}", double.Parse(chart_pie3DT.Rows[i].ItemArray[chart_pie3DT.Rows[i].ItemArray.Length - 1].ToString())) + ",</b> " + cs.Axes[1].Positions[j
                        ].Members[0].MemberProperties[0].Value.ToString().ToLower();
                    o[1] = chart_pie3DT.Rows[i].ItemArray[chart_pie3DT.Rows[i].ItemArray.Length - 1];
                    dt.Rows.Add(o);
                }
                else
                {
                    chart_pie3DT.Rows.Remove(chart_pie3DT.Rows[i]);
                    i -= 1;
                }
                j += 1;
            }
            chart_pie3.DataSource = dt;//chart_pie3DT;
            Label5.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("titlechart3"), chart_pie3DT.Columns[chart_pie3DT.Columns.Count - 1].ColumnName); ;
        }

        protected void InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
            //ORG_0003_0001._default.setChartErrorFont(e);
        }

        protected void chart_avg_count_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
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
        

        protected void web_grid_InitializeRow(object sender, RowEventArgs e)
        {
            try
            {
                e.Row.Cells[0].Text = e.Row.Cells[0].Text + ", " + e.Row.Cells[2].Text.ToLower();
            }
            catch { }
        }

        protected void chart_pie1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            try
            {
                for (int i = 0; i < chart_pie1DT.Rows.Count; i++)
                {
                    Infragistics.UltraChart.Core.Primitives.Text textLabel = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(18, 218 + i * 20 - i, 320, 10), chart_pie1DT.Rows[i].ItemArray[0].ToString(), new LabelStyle(new Font("Verdana", (float)(7.8)), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Near, TextOrientation.Horizontal));
                    e.SceneGraph.Add(textLabel);
                }
            }
            catch { }
        }



        protected void chart_pie3_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            try
            {
                for (int i = 0; i < chart_pie3DT.Rows.Count; i++)
                {
                    Infragistics.UltraChart.Core.Primitives.Text textLabel = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(18, 218 + i * 20 - i, 320, 10), chart_pie3DT.Rows[i].ItemArray[0].ToString(), new LabelStyle(new Font("Verdana", (float)(7.8)), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Near, TextOrientation.Horizontal));
                    e.SceneGraph.Add(textLabel);
                }
            }
            catch { }
        }

        protected void chart_pie2_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            try
            {
                for (int i = 0; i < chart_pie2DT.Rows.Count; i++)
                {
                    Infragistics.UltraChart.Core.Primitives.Text textLabel = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(18, 218 + i * 20 - i, 320, 10), chart_pie2DT.Rows[i].ItemArray[0].ToString(), new LabelStyle(new Font("Verdana", (float)(7.8)), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Near, TextOrientation.Horizontal));
                    e.SceneGraph.Add(textLabel);
                }
            }
            catch { }
        }


    }

}

