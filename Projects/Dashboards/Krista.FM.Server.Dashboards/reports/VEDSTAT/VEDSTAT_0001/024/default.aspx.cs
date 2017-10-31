using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

using Microsoft.AnalysisServices.AdomdClient;

/**
 *  Физкультура и спорт.
 */
namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._0240
{

    public partial class Default : CustomReportPage
    {
        // --------------------------------------------------------------------

        // заголовок страницы
		private static String page_title_caption = "Физическая культура и спорт ({0})";
		private static String page_subtitle_caption = "Анализ динамики и структуры основных показателей, характеризующих физическую культуру и спорт в муниципальном образовании";
		// текст отчёта
		private static String report_text = "Количество спортивных сооружений составляет <b>{0}</b>&nbsp;ед. Из них:<br>Плоскостные спортивные сооружения (площадки, поля, спортивные ядра) – <b>{1}</b>&nbsp;ед.<br>Спортивные залы – <b>{2}</b>&nbsp;ед.<br>Плавательные бассейны – <b>{3}</b>&nbsp;ед.<br>Лыжные базы – <b>{4}</b>&nbsp;ед.<br>Стрелковые тиры – <b>{5}</b>&nbsp;ед.<br>Физкультурно-оздоровительные центры предприятий, учреждений, организаций – <b>{6}</b>&nbsp;ед.";
        
        // заголовок для Grid1
        private static String grid1_title_caption = "Основные показатели развития физкультуры и спорта";
        // заголовок для Grid2
        private static String grid2_title_caption = "Мощность сооружений и количество спортивных коллективов";

        // заголовок для Chatr1
        private static String chart1_title_caption = "Динамика показателя «{0}», {1}";
        // заголовок для Chatr2
		private static String chart2_title_caption = "Распределение мощности спортивных сооружений по видам сооружений в&nbsp;{0}&nbsp;году, %";
        // заголовок для Chatr3
		private static String chart3_title_caption = "Распределение числа коллективов физкультуры, спортивных клубов по типам организаций в&nbsp;{0}&nbsp;году, %";
        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }

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
        // --------------------------------------------------------------------
        string BN = "IE";
        private CustomParam textMarks;
        private CustomParam grid1Marks;
        private CustomParam grid2Marks;
        private CustomParam chart2Marks;
        private CustomParam chart3Marks;
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
                System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
                BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
                base.Page_PreLoad(sender, e);
                // установка размера диаграммы
				web_grid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.33);
				web_grid2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.99);
				if (BN == "IE")
				{
					web_grid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.37 + 1);
					web_grid2.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.075);
				}
				if (BN == "FIREFOX")
				{
					web_grid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.37 - 4);
					web_grid2.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.1);
				}
				if (BN == "APPLEMAC-SAFARI")
				{
					web_grid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.37);
					web_grid2.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.075);
				}


				UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.66);
				UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.49);
				UltraChart3.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.49);
				UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.40);
				UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.50);
				UltraChart3.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.50);

				Label1.Width = (int)((CustomReportConst.minScreenWidth - 55) * 0.66);
				Label1.Height = (int)(CustomReportConst.minScreenHeight * 0.05);
				Grid1Label.Width = (int)((screen_width - 55) * 0.32);

                Label2.Width = (int)((screen_width - 55) * 0.49);
                Label3.Width = (int)((screen_width - 55) * 0.49);
                textMarks = UserParams.CustomParam("textMarks");
                grid1Marks = UserParams.CustomParam("grid1Marks");
                grid2Marks = UserParams.CustomParam("grid2Marks");
                chart2Marks = UserParams.CustomParam("chart2Marks");
                chart3Marks = UserParams.CustomParam("chart3Marks");
				UltraChart3.PyramidChart3D.OthersCategoryPercent = 0;
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
            base.Page_Load(sender, e);
            RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
            baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
			if (!Page.IsPostBack)
			{   // опрерации которые должны выполняться при только первой загрузке страницы


				WebAsyncRefreshPanel2.AddLinkedRequestTrigger(web_grid1);
				WebAsyncRefreshPanel3.AddLinkedRequestTrigger(web_grid2);

				last_year.Value = getLastDate(RegionSettingsHelper.Instance.GetPropertyValue("way_last_year_mark"));
				String year = UserComboBox.getLastBlock(last_year.Value);
				page_title.Text = String.Format(page_title_caption, UserComboBox.getLastBlock(baseRegion.Value));
				Page.Title = page_title.Text;
				page_subtitle.Text = page_subtitle_caption;
				textMarks = ForMarks.SetMarks(textMarks, ForMarks.Getmarks("text_mark_"), true);
				grid1Marks = ForMarks.SetMarks(grid1Marks, ForMarks.Getmarks("grid1_mark_"), true);
				grid2Marks = ForMarks.SetMarks(grid2Marks, ForMarks.Getmarks("grid2_mark_"), true);
				chart2Marks = ForMarks.SetMarks(chart2Marks, ForMarks.Getmarks("chart2_mark_"), true);
				chart3Marks = ForMarks.SetMarks(chart3Marks, ForMarks.Getmarks("chart3_mark_"), true);
				DataTable table = new DataTable();
				DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("text"), "sfn", table);
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
					Label4.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("report_title"), year);
					ReportText.Text = String.Format(report_text,
													tempArray[0].ToString(),
													tempArray[1].ToString(),
													tempArray[2].ToString(),
													tempArray[3].ToString(),
													tempArray[4].ToString(),
													tempArray[5].ToString(),
													tempArray[6].ToString());

				}
				else ReportText.Text = "В настоящий момент данные отсутствуют!";


				// заполнение UltraWebGrid данными
				web_grid1.DataBind();
				webGrid1ActiveRowChange(0, true);

				web_grid2.DataBind();
				web_grid2.Columns[web_grid2.Columns.Count - 1].Selected = true;
				grid2Manual_ActiveCellChange(web_grid2.Columns.Count - 1);

				// установка заголовков
				Grid1Label.Text = String.Format(grid1_title_caption, year);
				Grid2Label.Text = String.Format(grid2_title_caption, year);
			}
        }

        // --------------------------------------------------------------------

        /** <summary>
         *  Метод получения последней актуальной даты 
         *  </summary>
         */
        private String getLastDate(String way_ly)
        {
            way_last_year.Value = way_ly;
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("last_date"));
            return cs.Axes[1].Positions[0].Members[0].ToString();
			//DataTable dtData = new DataTable();
			//string query = DataProvider.GetQueryText("last_date");
			//DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtData);
			//return dtData == null ? null : dtData.Rows[0][0].ToString();
		}

        // --------------------------------------------------------------------

        protected void web_grid1_DataBinding(object sender, EventArgs e)
        {
            DataTable grid_master = new DataTable();
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid1"));

            grid_master.Columns.Add();
			grid_master.Columns.Add("Показатель");
			grid_master.Columns.Add(UserComboBox.getLastBlock(last_year.Value));
			//grid_master.Columns.Add("Значение");

            foreach (Position pos in cs.Axes[1].Positions)
            {   // создание списка значений для строки UltraWebGrid
                object[] values = {
                    pos.Members[0].MemberProperties[0].Value.ToString(),
                    cs.Axes[1].Positions[pos.Ordinal].Members[0].Caption  + ", " + cs.Cells[1, pos.Ordinal].Value.ToString().ToLower(),
                    string.Format("{0:N0}", cs.Cells[0, pos.Ordinal].Value)
                };
                // заполнение строки данными
                grid_master.Rows.Add(values);
            }
            web_grid1.DataSource = grid_master.DefaultView;
        }

        protected void web_grid1_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            // настройка столбцов
            e.Layout.Bands[0].Columns[0].Hidden = true;

            double tempWidth = e.Layout.FrameStyle.Width.Value - 14;
            e.Layout.RowSelectorStyleDefault.Width = 20 - 3;
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 20) * 0.78) - 5;

            e.Layout.Bands[0].Columns[2].Width = (int)((tempWidth - 20) * 0.22) - 5;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "### ### ### ##0");
            e.Layout.Bands[0].Columns[2].Header.Style.HorizontalAlign = HorizontalAlign.Center;

        }

        private void webGrid1ActiveRowChange(int index, bool active)
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
			// Придется название показателя разбивать на части
			string[] split_string = row.Cells[1].Value.ToString().Split(',');
			string label1_text = split_string[0];
			for (int i = 1; i < (split_string.Length - 1); ++i)
			{
				label1_text += "," + split_string[i];
			}
			Label1.Text = String.Format(chart1_title_caption, label1_text, split_string[split_string.Length - 1]);
            int mer = row.Cells[1].Value.ToString().Split(',').Length-1;
            UltraChart1.Tooltips.FormatString = "<b><DATA_VALUE:###,##0.##></b>, " + row.Cells[1].Value.ToString().Split(',')[mer];
/*			if (Label1.Text.Length > 85)
			{
				UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.50 - 17);
			}
			else
			{
				UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.50);
			}*/
        }

        protected void web_grid1_ActiveRowChange(object sender, RowEventArgs e)
        {
            webGrid1ActiveRowChange(e.Row.Index, false);
        }


        // --------------------------------------------------------------------

        protected void web_grid2_DataBinding(object sender, EventArgs e)
        {
            DataTable grid_master = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid2"), "Показатели", grid_master);
            web_grid2.DataSource = grid_master.DefaultView;
        }

        protected void web_grid2_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            // настройка столбцов
            double tempWidth = e.Layout.FrameStyle.Width.Value - 16;
            e.Layout.RowSelectorStyleDefault.Width = 0;
            e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth) * 0.4) - 5;
            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = (int)((tempWidth) * 0.6 / (e.Layout.Bands[0].Columns.Count - 2)) - 5;
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                // установка формата отображения данных в UltraWebGrid
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ### ###.##");
            }
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
			e.Layout.Bands[0].Columns[1].Hidden = true;
		}

        protected void web_grid2_ActiveCellChange(object sender, CellEventArgs e)
        {
            UltraGridCell cell = e.Cell;
			int CellIndex = cell.Column.Index;
			if (CellIndex == 0)
			{
				CellIndex = 2;
			}
			if (CellIndex > 0)
            {
                grid2Manual_ActiveCellChange(CellIndex);
            }
            else
            {
                grid2Manual_ActiveCellChange(1);
            }
        }

        /// <summary>
        /// Обновление данных для UltraChart при изменении активной ячейки в UltraWebGrid
        /// </summary>
        /// <param name="CellIndex">индекс активной ячейки</param>
        protected void grid2Manual_ActiveCellChange(int CellIndex)
        {
			//CRHelper.SaveToUserAgentLog(String.Format("CellIndex = {0}", CellIndex));
            web_grid2.Columns[CellIndex].Selected = true;
            selected_year.Value = CellIndex == 0 ? web_grid2.Columns[2].Header.Key.ToString() : web_grid2.Columns[CellIndex].Header.Key.ToString();
            

            UltraChart2.DataBind();
            UltraChart3.DataBind();
            Label2.Text = String.Format(chart2_title_caption, selected_year.Value);
            Label3.Text = String.Format(chart3_title_caption, selected_year.Value);
        }

        protected void web_grid2_Click(object sender, ClickEventArgs e)
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
			if (CellIndex == 0)
			{
				CellIndex = 2;
			}
			if (CellIndex > 0)
            {
                grid2Manual_ActiveCellChange(CellIndex);
            }
            else
                grid2Manual_ActiveCellChange(1);
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
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(query_name), "sfn", table);
            return table == null ? null : table.DefaultView;
        }


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


        protected void UltraChart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
			string chart_labels = e.SceneGraph.ToString();
			CRHelper.SaveToUserAgentLog(chart_labels);
/*            int xOct = 0;
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
            }*/
        }

		protected void web_grid2_InitializeRow(object sender, RowEventArgs e)
		{
			e.Row.Cells[0].Text = e.Row.Cells[0].Text + ", " + e.Row.Cells[1].Text.ToLower();

		}

    }
}

