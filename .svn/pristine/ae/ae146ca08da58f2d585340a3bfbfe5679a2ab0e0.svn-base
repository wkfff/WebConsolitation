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
 *  Информация по состоянию учреждений культуры, искусства, физической культуры и спорта.
 */
namespace Krista.FM.Server.Dashboards.reports.PMO_0001_0008
{
    public partial class Default : CustomReportPage
    {
        private static String chart_error_message = "в настоящий момент данные отсутствуют";
        // заголовок страницы
        private static String pageTitleCaption;//= RegionSettingsHelper.Instance.GetPropertyValue("TitlePage"); 
        // заголовок для Grid
        private static String grid1TitleCaption = "Показатели учреждений культуры в {0} году";
        // заголовок для Grid2
        private static String grid2TitleCaption = "Показатели учреждений физической культуры и спорта";
        // заголовок для Chart1
        private static String chart1TitleCaption = "Число спортивных сооружений и их удельный вес в общем объеме в {0} году, единица";
        // заголовок для Chart2
        private static String chart2TitleCaption = "Мощность спортивных сооружений (посещений) и их удельный вес в общем объеме в {0} году, единица";          
        // параметр для выбранной/текущей территории
        private CustomParam current_region { get { return (UserParams.CustomParam("current_region")); } }
        // параметр для последней актуальной даты
        private CustomParam last_year { get { return (UserParams.CustomParam("last_year")); } }
        // параметр для выбранного/текущего показателя
        private CustomParam branch { get { return (UserParams.CustomParam("current_branch")); } }
        // параметр для выбранного года
        private CustomParam selected_year { get { return (UserParams.CustomParam("selected_year")); } }
        // ширина экрана в пикселях
        private Int32 screen_width { get { return (int)Session["width_size"]; } }
        // заголовки столбцов для UltraWebGrid
        private static String[] grid1Сolumns = { "Показатели", "Ед. изм.", "Значение" };
        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }

        private CustomParam marks;// { get { return (new CustomParam("2")); } }
        string BN = "IE";
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                base.Page_PreLoad(sender, e);
                System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
                BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
                marks = UserParams.CustomParam("marks");
                // установка размера диаграмм
                web_grid.Width = (int)((screen_width - 55) * 0.4);
                grid2.Width = (int)((screen_width - 40));
                chart_dinamic.Width = (int)((screen_width - 55) * 0.6);

                web_grid.Height = 215;
                

                chart_pie1.Width = (int)((screen_width - 55) * 0.49);
                chart_pie2.Width = (int)((screen_width - 55) * 0.49);

                pageTitleCaption = RegionSettingsHelper.Instance.GetPropertyValue("TitlePage");
                grid1TitleCaption = RegionSettingsHelper.Instance.GetPropertyValue("grid1TitleCaption");//"Показатели учреждений культуры в {0} году";
        // заголовок для Grid2

                grid2TitleCaption = RegionSettingsHelper.Instance.GetPropertyValue("grid2TitleCaption"); //"Показатели учреждений физической культуры и спорта";
        // заголовок для Chart1
                chart1TitleCaption = RegionSettingsHelper.Instance.GetPropertyValue("chart1TitleCaption");///"Число спортивных сооружений и их удельный вес в общем объеме в {0} году, единица";
        // заголовок для Chart2
                chart2TitleCaption = RegionSettingsHelper.Instance.GetPropertyValue("chart2TitleCaption"); //"Мощность спортивных сооружений (посещений) и их удельный вес в общем объеме в {0} году, единица"; 
                //WebAsyncRefreshPanel4.AddLinkedRequestTrigger(Ref);
                WebAsyncRefreshPanel4.AddRefreshTarget(chart_pie1);
                WebAsyncRefreshPanel4.AddRefreshTarget(chart1title);
                WebAsyncRefreshPanel4.AddRefreshTarget(chart2title);
                WebAsyncRefreshPanel4.AddRefreshTarget(chart_pie2);

                Ref.AddLinkedRequestTrigger(grid2);
                Ref.AddRefreshTarget(grid2);
                
                PopupInformer1.HelpPageUrl = RegionSettingsHelper.Instance.GetPropertyValue("nameHelpPage");

                if (BN == "IE")
                {
                    TitleChart.Height = CRHelper.GetChartHeight(54);
                    chart_dinamic.Height = CRHelper.GetChartHeight(215);
                }
                else
                {
                    if (BN == "FIREFOX")
                    {
                        TitleChart.Height = CRHelper.GetChartHeight(54);
                        chart_dinamic.Height = CRHelper.GetChartHeight(196);
                    }
                    else
                    {
                        TitleChart.Height = CRHelper.GetChartHeight(54);
                        chart_dinamic.Height = CRHelper.GetChartHeight(213);
                    }
                }
            }
            catch (Exception)
            {
                // установка размеров не удалась ...
            }
        }
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            string oldId = RegionSettings.Instance.Id;
         //   RegionSettingsHelper.Instance.SetWorkingRegion("Novoorsk");
            baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            
                base.Page_Load(sender, e);

                if (!Page.IsPostBack)
                {   // опрерации которые должны выполняться при только первой загрузке страницы




                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(web_grid);

                    setChartSettings(chart_dinamic);
                     setChartSettings(chart_pie1);
                     setChartSettings(chart_pie2);
                     //Label2.Text = "0"; 
                     marks = SetMarks(marks, Getmarks("grid1_mark_"), true);

                    
                    // установка параметра территории
                    current_region.Value = baseRegion.Value;

                    last_year.Value = getLastDate();
                    
                    String year = UserComboBox.getLastBlock(last_year.Value);

                    //Label2.Text = "1";
                    // установка заголовка для страницы
                    page_title.Text = String.Format(pageTitleCaption,UserComboBox.getLastBlock(baseRegion.Value));
                    Page.Title = page_title.Text;
                    Label1.Text = RegionSettingsHelper.Instance.GetPropertyValue("PageSubTitle");
                    grid_caption.Text = String.Format(grid1TitleCaption, year);

                    last_year.Value = getLastDate_();
                    year = UserComboBox.getLastBlock(last_year.Value);

                    TitleGrid2.Text = grid2TitleCaption;

                    marks = SetMarks(marks, Getmarks("grid1_mark_"),true);
                    // заполнение UltraWebGrid данными
                    web_grid.DataBind();

                    marks = SetMarks(marks, Getmarks("grid2_mark_"),true);
                    grid2.DataBind();
                    // заполнение UltraChart данными

                    grid2.Rows[0].Cells[grid2.Rows[0].Cells.Count - 1].Selected = 1 == 1;
                    grid2.Rows[0].Cells[grid2.Rows[0].Cells.Count - 1].Activate();
                    grid2.Rows[0].Cells[grid2.Rows[0].Cells.Count - 1].Activated = 1 == 1;

                    grid2.Columns[grid2.Columns.Count - 1].Selected = 1 == 1;

                    marks = SetMarks(marks, Getmarks("chart2_mark_"),true);
                    chart_pie1.DataBind();

                    marks = SetMarks(marks, Getmarks("chart3_mark_"),true);
                    chart_pie2.DataBind();
                    // установка активной строки в UltraWebGrid
                    webGridActiveRowChange(0, true);
                    // установка активной ячейки в UltraWebGrid
                    grid2Manual_ActiveCellChange(grid2.Columns.Count-1);
                    grid2.DisplayLayout.CellClickActionDefault = CellClickAction.CellSelect;
                }
                
          
            RegionSettingsHelper.Instance.SetWorkingRegion(oldId);
        }

        /// <summary>
        /// Метод получения последней актуальной даты
        /// </summary>
        /// <returns>возвращает строку - год</returns>
        private String getLastDate()
        {
            try
            {
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("last_date"));
                //return "[Период].[Год Квартал Месяц].[Год].["+cs.Axes[1].Positions[cs.Axes[1].Positions.Count -1].Members[0].ToString()+"]";
                return cs.Axes[1].Positions[cs.Axes[1].Positions.Count - 1].Members[0].ToString();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private String getLastDate_()
        {
            try
            {
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("last_date_"));
                return cs.Axes[1].Positions[0].Members[0].ToString();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        protected void HeaderPR1_Load(object sender, EventArgs e)
        {

        }

        protected void web_grid_DataBinding(object sender, EventArgs e)
        {
            DataTable grid_table = new DataTable();
            // Установка параметра последней актуальной даты
            UserParams.CustomParam("period_last_date").Value = getLastDate();
            
                CellSet grid_set = null;
                // Загрузка данных в CellSet
                grid_set = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid"));
                // Добавление столбцов в таблицу и заполнение данных в DataTable
                for (int i = 0; i < grid1Сolumns.Length; ++i)
                {   // установка заголовков для столбцов UltraWebGrid
                    grid_table.Columns.Add(grid1Сolumns[i]);
                }
                foreach (Position pos in grid_set.Axes[1].Positions)
                {   // создание списка значений для строки UltraWebGrid
                    object[] values = {
                        //pos.Ordinal + 1,
                        grid_set.Axes[1].Positions[pos.Ordinal].Members[0].Caption,
                        pos.Members[0].MemberProperties[0].Value.ToString(),
                        float.Parse(grid_set.Cells[0, pos.Ordinal].FormattedValue)
                    };
                    // заполнение строки данными
                    grid_table.Rows.Add(values);
                }
            
          
            // установка источника данных для UltraWebGrid
            web_grid.DataSource = grid_table.DefaultView;
        }

        protected void web_grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            // настройка столбцов UltraWebGrid
            double tempWidth = web_grid.Width.Value - 14;
            web_grid.DisplayLayout.RowSelectorStyleDefault.Width = 20 - 2;
            e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth - 10) * 0.55) - 5;
            e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 10) * 0.25) - 5;
            e.Layout.Bands[0].Columns[2].Width = (int)((tempWidth - 10) * 0.15) - 5;

            //web_grid.Bands[0].Columns[0].Width = 280;
            //web_grid.Bands[0].Columns[1].Width = 70;
            //web_grid.Bands[0].Columns[2].Width = 60;
            web_grid.Bands[0].Columns[0].CellStyle.Wrap = true;
            web_grid.Bands[0].Columns[1].CellStyle.Wrap = true;
            //web_grid.Bands[0].Columns[1].Width = 370; // TODO: переделать: надо бы сделать вычислиямые размеры
            // установка формата отображения ячеек
            //CRHelper.FormatNumberColumn(web_grid.Bands[0].Columns[0], "N2");
            CRHelper.FormatNumberColumn(web_grid.Bands[0].Columns[2], "#.##");
        }

        /// <summary>
        /// Метод выполняет запрос и возвращает DataView, в случае неудачи возвращает 'null' (nothrow)
        /// </summary>
        /// <param name="query_name">имя запроса MDX</param>
        /// <returns>DataView</returns>
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

        /// <summary>
        /// Установка активной сторки элемента UltraWebGrid
        /// </summary>
        /// <param name="index">выбранная строка</param>
        /// <param name="active">установить или нет свойство 'Active' у выбранной строки</param>
        private void webGridActiveRowChange(Int32 index, bool active)
        {
           
                //marks = SetMarks(marks, Getmarks("grid1_mark_"), true);
                // получаем выбранную строку
                UltraGridRow row = web_grid.Rows[index];
                // устанавливаем ее активной, если необходимо
                if (active) { row.Activate(); row.Selected = 1 == 1; row.Activated = 1 == 1; }
                // получение заголовка выбранного показателя
                String branch_name = row.Cells[0].Value.ToString();
                // установка параметра выбранного показателя
                branch.Value = branch_name;
                // загрузка данных для диаграммы

                ArrayList itemGrid = Getmarks("grid1_mark_");

                int i = 0;

                for ( i = 0; itemGrid.Count > i; i++)
                {
                    if (row.Cells[0].Text == UserComboBox.getLastBlock(itemGrid[i].ToString()))
                    {
                        break;
                    }
 
                }
                

                marks.Value = itemGrid[i].ToString();


                chart_dinamic.DataBind();
                chart_dinamic.Tooltips.FormatString = "<b><DATA_VALUE:### ### ##0.##></b>, " + web_grid.Rows[index].Cells[1].Text.ToLower();
                TitleChart.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart"), web_grid.Rows[index].Cells[0].Text, web_grid.Rows[index].Cells[1].Text.ToLower());
                // установка заголовка для диаграммы
            //    TitleChart.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart"),web_grid.Rows[0].Cells[0].Text,web_grid.Rows[0].Cells[1].Text.ToLower());
                //TitleChart.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart"), branch_name, row.Cells[1].Value.ToString().ToLower(), UserComboBox.getLastBlock(last_year.Value),
                 //       UserComboBox.getLastBlock(current_region.Value)); //= "«" + branch_name + ", " + row.Cells[1].Value.ToString().ToLower() + "»";
          
        }

        // --------------------------------------------------------------------

        protected void web_grid_ActiveRowChange(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            webGridActiveRowChange(e.Row.Index, false);
            //TitleChart.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("titleDynamicchart"), e.Row.Cells[0].Text, e.Row.Cells[1].Text.ToLower());
        }

        protected void chart_dinamic_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart_dinamic"), "sfn", dt);
            


            float min, max;
            min = max = float.Parse(dt.Rows[0].ItemArray[1].ToString());
            for (int i = 1; i < dt.Rows[0].ItemArray.Length; i++)
            {
                if (float.Parse(dt.Rows[0].ItemArray[i].ToString()) < min)
                {
                    min = float.Parse(dt.Rows[0].ItemArray[i].ToString());
                }
                if (float.Parse(dt.Rows[0].ItemArray[i].ToString()) > max)
                {
                    max = float.Parse(dt.Rows[0].ItemArray[i].ToString());
                }
            }

            if (max - min < 10 & max - min >= 1 & min < 10)
            {
                chart_dinamic.Axis.Y.RangeMin = 0;
                chart_dinamic.Axis.Y.RangeMax = max*1.1;
                chart_dinamic.Axis.Y.TickmarkPercentage = (int)(100 / max);
                chart_dinamic.Axis.Y.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Percentage;
            }
            else
            {
                if (max == min)
                {
                    chart_dinamic.Axis.Y.RangeMin = 0;
                    chart_dinamic.Axis.Y.RangeMax = max * 2;
                    chart_dinamic.Axis.Y.TickmarkPercentage = 100;
                    chart_dinamic.Axis.Y.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Percentage;
                }
                else
                {
                    chart_dinamic.Axis.Y.TickmarkPercentage = 10;
                    chart_dinamic.Axis.Y.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
                    if ((min / max) > 0.98)
                    {
                        chart_dinamic.Axis.Y.RangeMin = min * 0.999;
                        chart_dinamic.Axis.Y.RangeMax = max * 1.1;
                    }
                    else
                    {
                        chart_dinamic.Axis.Y.RangeMin = min * 0.98;
                        chart_dinamic.Axis.Y.RangeMax = max * 1.1;
                    }
                }

            }


            chart_dinamic.DataSource = dt.DefaultView;
        }

        protected void chart_pie1_DataBinding(object sender, EventArgs e)
        {
            chart_pie1.DataSource = dataBind("chart");
        }

        protected void chart_pie2_DataBinding(object sender, EventArgs e)
        {
            chart_pie2.DataSource = dataBind("chart");
        }

        protected void chart_dinamic_Init(object sender, EventArgs e)
        {
            // добавляем метки со значениями на диаграмму 
            // из дизайнера почему-то они не добавляются
            Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance text =
                new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance();
            text.Row = -2;
            text.Column = -2;
            text.ItemFormatString = "<DATA_VALUE:0.##>";
            text.PositionFromRadius = 50;
            text.VerticalAlign = System.Drawing.StringAlignment.Far;
            text.Visible = true;
            chart_dinamic.SplineAreaChart.ChartText.Add(text);
        }

        protected void chart_dinamic_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
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

        protected void grid2_DataBinding(object sender, EventArgs e)
        {
            // Установка параметра последней актуальной даты
            UserParams.CustomParam("period_last_date").Value = getLastDate_();
            DataTable grid_table = new DataTable();
            DataTable dt = new DataTable();
           
                CellSet grid_set = null;
                // Загрузка данных в CellSet
                grid_set = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid2"));
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid2"), " ", dt);
                // Добавление столбцов в таблицу и заполнение данных в DataTable
                grid_table.Columns.Add("Показатели");
                for (int i = 1; i < dt.Columns.Count; i++)
                {   // установка заголовков для столбцов UltraWebGrid
                    grid_table.Columns.Add(dt.Columns[i].ColumnName);
                }
                object[] o=new object[grid_table.Columns.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    o[0] = dt.Rows[i].ItemArray[0].ToString() + ", " + grid_set.Axes[1].Positions[i].Members[0].MemberProperties[0].Value.ToString().ToLower(); 
                    for (int j = 1; j < dt.Columns.Count; j++)
                    {
                        o[j] =String.Format("{0:### ### ##0.00}",double.Parse(dt.Rows[i].ItemArray[j].ToString()));
                    }
                    grid_table.Rows.Add(o);
                }
            grid2.DataSource=grid_table;
            // установка источника данных для UltraWebGrid

        }

        protected void grid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            // настройка столбцов
            grid2.Bands[0].HeaderStyle.Height = 40;
            grid2.Bands[0].Columns[0].Width = 500;
            grid2.Bands[0].Columns[0].CellStyle.Wrap = true;
            grid2.Bands[0].Columns[0].CellStyle.Height = 20;
            for (int i = 1; i < grid2.Bands[0].Columns.Count; ++i)
            {
                // настройка столбцов
                grid2.Bands[0].Columns[i].Width = 60;
                // установка формата отображения ячеек
                CRHelper.FormatNumberColumn(grid2.Bands[0].Columns[i], "### ### ##0.00");
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            }
            // Установка цвета первой ячейки
            e.Layout.Bands[0].Columns[0].AllowUpdate = AllowUpdate.No;
            e.Layout.RowSelectorsDefault = RowSelectors.No;
        }
        int oldCellIndex = 1;
        protected void grid2_ActiveCellChange(object sender, CellEventArgs e)
        {
            
            try
            {
                UltraGridCell cell = e.Cell;
                int CellIndex = cell.Column.Index;
                if (e.Cell.Column.Index != 0)
                {
                    grid2Manual_ActiveCellChange(CellIndex);
                    grid2.Columns[e.Cell.Column.Index].Selected = true;
                }
                else
                {
                   grid2Manual_ActiveCellChange(CellIndex);
                   e.Cell.Selected = 1 == 2;
                   grid2.Columns[1].Selected = true;
                }
            }
            catch
            {
                grid2Manual_ActiveCellChange(1);
                grid2.Columns[1].Selected = true;
                // grid2Manual_ActiveCellChange(1); 
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
                if (CellIndex != 0)
                {
                    selected_year.Value = grid2.Columns[CellIndex].Header.Key.ToString();
                    oldCellIndex = CellIndex;
                }
                else
                {
                    selected_year.Value = grid2.Columns[oldCellIndex].Header.Key.ToString();
                    grid2.Rows[0].Cells[1].Selected = 1 == 1;
                }
                chart1title.Text = String.Format(chart1TitleCaption, selected_year.Value, UserComboBox.getLastBlock(current_region.Value));
                chart2title.Text = String.Format(chart2TitleCaption, selected_year.Value, UserComboBox.getLastBlock(current_region.Value));
                marks = SetMarks(marks, Getmarks("chart2_mark_"), true);
                chart_pie1.DataBind();
                marks = SetMarks(marks, Getmarks("chart3_mark_"), true);
                chart_pie2.DataBind();
                grid2.Columns[CellIndex].Selected = true;
            }
            catch { }
        }

        protected void grid2_Click(object sender, ClickEventArgs e)
        {
           
                int CellIndex = e.Column.Index;
                try
                {
                    e.Column.Selected = 1 == 1;
                    if (e.Column.Index != 0)
                    {

                        grid2Manual_ActiveCellChange(CellIndex);
                        
                    }
                    else
                    {
                        grid2Manual_ActiveCellChange(CellIndex);
                      //  e.Column.Selected = 1 == 1;
                    }
                }
                catch 
                {
                    e.Column.Selected = 1 == 1;
                }
                
            

        }

        protected void chart_dinamic_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
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
        public static void setChartErrorFont(Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = chart_error_message;
            e.LabelStyle.Font = new Font("Verdana", 20);
            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
        }

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
            try
            {
                if (clearParam.Length > 0 && clearParam[0]) { param.Value = ""; }
                int i;
                for (i = 0; i < AL.Count - 1; i++)
                {
                    param.Value += AL[i].ToString() + ",";
                }
                param.Value += AL[i].ToString();

            }
            catch { }
            return param;
        }


        public static void setChartSettings(UltraChart chart)
        {
            chart.Legend.BackgroundColor = Color.Empty;
            chart.Legend.BorderColor = Color.Empty;
        }

        protected void chart_pie2_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }

        protected void chart_pie1_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }

        protected void grid2_ClickCellButton(object sender, CellEventArgs e)
        {

        }


    }

}

