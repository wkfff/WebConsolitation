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
using Krista.FM.Server.Dashboards.Core;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;

/**
 *  Основные показатели социально-экономического развития.
 */
namespace Krista.FM.Server.Dashboards.reports.PMO_0001_0003
{
    public partial class _default : Dashboards.CustomReportPage
    {
        // --------------------------------------------------------------------

        // заголовок страницы
        private static String page_title_caption = "Основные показатели социально-экономического развития по данным на {0} год <nobr>({1})</nobr>";
        // таблица склонения имен месяцев
        private static Hashtable month_names = monthNames();
        // параметр для выбранной.текущей территории
        private CustomParam current_region { get { return (new CustomParam("current_region", Session)); } }
        // параметр для последней актуальной даты
        private CustomParam last_year { get { return (new CustomParam("last_year", Session)); } }
        // параметр для выбранного/текущего показателя
        private CustomParam branch { get { return (new CustomParam("current_branch", Session)); } }
        // ширина экрана в пикселях
        private Int32 screen_width { get { return (int)Session["width_size"]; } }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                base.Page_PreLoad(sender, e);
                // установка размера диаграмм
                chart_avg_count.Width = (screen_width + 150) / 2;
                chart_pie1.Width = (screen_width - 100) / 3;
                chart_pie2.Width = (screen_width - 100) / 3;
                chart_pie3.Width = (screen_width - 100) / 3;
            }
            catch (Exception)
            {
                // установка размеров не удалась ...
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                base.Page_Load(sender, e);
                if (!Page.IsPostBack)
                {   // опрерации которые должны выполняться при только первой загрузке страницы
                    ORG_0003_0001._default.setChartSettings(chart_avg_count);
                    ORG_0003_0001._default.setChartSettings(chart_pie1);
                    ORG_0003_0001._default.setChartSettings(chart_pie2);
                    ORG_0003_0001._default.setChartSettings(chart_pie3);
                }
                // установка параметра территории
                current_region.Value = user_combo.SelectedItem.Value;

                last_year.Value = getLastDate();
                String year = core.UserComboBox.getLastBlock(last_year.Value);

                // установка заголовка для страницы
                page_title.Text = String.Format(
                    page_title_caption, year,
                    core.UserComboBox.getLastBlock(current_region.Value));
                // заполнение UltraWebGrid данными
                web_grid.DataBind();
                // заполнение Carts данными
                chart_pie1.DataBind();
                chart_pie2.DataBind();
                chart_pie3.DataBind();
                // установка активной строки в UltraWebGrid
                webGridActiveRowChange(0, true);
            }
            catch (Exception)
            {
                // неудачная загрузка ...
            }
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
                CellSet cs = PrimaryMASDataProvider.GetCellset(GetQueryText("last_date"));
                return cs.Axes[1].Positions[0].Members[0].ToString();
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


        protected void HeaderPR1_Load(object sender, EventArgs e)
        {

        }

        protected void web_grid_DataBinding(object sender, EventArgs e)
        {
            // Установка параметра последней актуальной даты
            new CustomParam("period_last_date", Session).Value = getLastDate();
            DataTable chart_table = new DataTable();
            string query = GetQueryText("grid");
            this.SecondaryMASDataProvider.GetDataTableForChart(query, "Предприятия, учреждения и организации", chart_table);
            chart_table.Columns[1].Caption = "Число";
            web_grid.DataSource = chart_table.DefaultView;
        }

        protected void web_grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            // настройка столбцов
            web_grid.Bands[0].Columns[0].Width = 350;
            web_grid.Bands[0].Columns[1].Width = 50;
            web_grid.Bands[0].Columns[0].CellStyle.Wrap = true;
            // установка формата отображения ячеек
            CRHelper.FormatNumberColumn(web_grid.Bands[0].Columns[3], "N2");
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
                PrimaryMASDataProvider.GetDataTableForChart(GetQueryText(query_name), "sfn", table);
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
                String branch_name = row.Cells[0].Value.ToString();
                // установка параметра выбранного показателя
                branch.Value = branch_name;
                // загрузка данных для диаграммы
                chart_avg_count.DataBind();
                //chart_avg_wage.DataBind();
                // установка заголовка для диаграммы
                Label1.Text = branch_name;
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
            chart_avg_count.DataSource = dataBind("chart_avg_count");
        }

        protected void chart_avg_count_Init(object sender, EventArgs e)
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
            chart_avg_count.SplineAreaChart.ChartText.Add(text);
        }

        protected void chart_pie_DataBinding(object sender, EventArgs e)
        {
            chart_pie1.DataSource = dataBind("chart_pie1");
        }

        protected void chart_pie2_DataBinding(object sender, EventArgs e)
        {
            chart_pie2.DataSource = dataBind("chart_pie2");
        }

        protected void chart_pie3_DataBinding(object sender, EventArgs e)
        {
            chart_pie3.DataSource = dataBind("chart_pie3");
        }

        protected void InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            ORG_0003_0001._default.setChartErrorFont(e);
        }


    }

}

