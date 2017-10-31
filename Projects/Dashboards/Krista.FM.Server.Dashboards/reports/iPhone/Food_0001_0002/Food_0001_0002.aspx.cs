using System;
using System.Data;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Web;
using System.IO;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class Food_0001_0002 : CustomReportPage
    {
        private DateTime lastDate;
        private DateTime currentDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            string filename = HttpContext.Current.Server.MapPath("~/TemporaryImages/Food_0001_0002/") + "TouchElementBounds.xml";
            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/TemporaryImages/Food_0001_0002/"));
            File.WriteAllText(filename, String.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?><touchElements><element id=\"Food_0001_0003\" bounds=\"x=0;y=400;width=768;height=100\" openMode=\"incomes\"/></touchElements>"));
            
            string multitouchIcon = String.Empty;
            multitouchIcon = "<img src='../../../images/detail.png'>";
            detalizationIconDiv.InnerHtml = String.Format("<table><tr><td><a href='webcommand?showPinchReport=Food_0001_0002_v'>{0}</a></td>", multitouchIcon);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Food_0001_0002_incomes_date");
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, " ", dtDate);

			lastDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0]["ДанныеНа"].ToString(), 3);
			currentDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[1]["ДанныеНа"].ToString(), 3);

			UserParams.PeriodCurrentDate.Value = dtDate.Rows[1]["ДанныеНа"].ToString();
			UserParams.PeriodLastDate.Value = dtDate.Rows[0]["ДанныеНа"].ToString();

            InitializeDescription();
            InitializeChart();
        }

        #region Текст

        private DataTable dt;

        private void InitializeDescription()
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("Food_0001_0002_Text");
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Товар", dt);

            string grownGoods = String.Empty;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
				if (!(dt.Rows[i]["Темп прироста (динамика к предыдущему периоду) модуль"].ToString().Contains("-")))
                {
					grownGoods += String.Format(" {0}&nbsp;<nobr>(на&nbsp;<span class='DigitsValue'>{1:N2}%</span>)</nobr>,", dt.Rows[i]["Товар"].ToString().ToLower(), dt.Rows[i]["Темп прироста (динамика к предыдущему периоду) модуль"]);
                }
            }
			if (String.IsNullOrEmpty(grownGoods))
				grownGoods = "На отчетную дату роста цен на продукты питания не наблюдалось";

            string fallingGoods = String.Empty;
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
				if (dt.Rows[i]["Темп прироста (динамика к предыдущему периоду) модуль"].ToString().Contains("-"))
                {
					fallingGoods += String.Format(" {0}&nbsp;<nobr>(на&nbsp;<span class='DigitsValue'>{1:N2}%</span>)</nobr>,", dt.Rows[i]["Товар"].ToString().ToLower(), dt.Rows[i]["Темп прироста (динамика к предыдущему периоду) модуль"]);
                }
            }
			if (String.IsNullOrEmpty(fallingGoods))
				fallingGoods = "На отчетную дату снижения цен на продукты питания не наблюдалось";

            lbDescription.Text = String.Format("<div style='padding-left: 29px; clear: both'>Розничные цены на продукты питания на&nbsp;<span class='DigitsValue'>{1:dd.MM.yyyy}</span>&nbsp;(изменение за период с&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>)</div><table style='margin-top: 12px'><tr><td align='right' style='color: white'><span class='DigitsValue'>Наибольшее увеличение&nbsp;цен</span></td><td style='padding-left: 15px'><img src='../../../images/arrowRedUpBB.png'></td><td style='padding-left: 15px'>{2}</td></tr><tr><td align='right' style='color: white'><span  class='DigitsValue' style='padding-top: 5px'>Наибольшее снижение&nbsp;цен</span></td><td style='padding-left: 15px; padding-top: 5px'><img src='../../../images/arrowGreenDownBB.png'></td><td style='padding-left: 15px; padding-top: 5px'>{3}</td></tr></table>", lastDate, currentDate, grownGoods.TrimEnd(','), fallingGoods.TrimEnd(','));
        }

        #endregion
     
        #region Диаграммы

        private DataTable dtChart;

        private void InitializeChart()
        {
            dtChart = new DataTable();
            string query = DataProvider.GetQueryText("Food_0001_0002_charts");
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
			
			CustomParam foodParam = UserParams.CustomParam("food");
			foreach (DataRow row in dtChart.Rows)
			{
				foodParam.Value = row["MDX"].ToString();
				DataTable dtData = new DataTable();
				query = DataProvider.GetQueryText("Food_0001_0002_chart_data");
				DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Продукт", dtData);
                query = DataProvider.GetQueryText("Food_0001_0002_beloyarskiy");
                DataRow beloyarRow = DataProvider.GetDataTableForChart("Food_0001_0002_beloyarskiy", DataProvidersFactory.SpareMASDataProvider).Rows[0];
				object[] data = new object[8];
                ParseData(dtData, data, beloyarRow);
				double minValue, maxValue;
                if (Double.TryParse(data[0].ToString(), out minValue) && (minValue < Convert.ToDouble(row["Минимум "].ToString())))
				{
                    row["Минимум "] = data[0];
                    row["Темп прироста (динамика к предыдущему периоду) минимум"] = data[1];
                    row["Абсолютное отклонение к прошлой дате минимум"] = data[2];
                    row["Минимум город"] = data[3];
				}
                if (Double.TryParse(data[4].ToString(), out maxValue) && (maxValue > Convert.ToDouble(row["Максимум "].ToString())))
				{
                    row["Максимум "] = data[4];
                    row["Темп прироста (динамика к предыдущему периоду) максимум"] = data[5];
                    row["Абсолютное отклонение к прошлой дате максимум"] = data[6];
                    row["Максимум город"] = data[7];
				}
			}

			dtChart.Columns.Remove("MDX");

            IPadElementHeader1.Text = dtChart.Rows[0][0].ToString();
            IPadElementHeader2.Text = dtChart.Rows[1][0].ToString();
            IPadElementHeader3.Text = dtChart.Rows[2][0].ToString();
            IPadElementHeader4.Text = dtChart.Rows[3][0].ToString();
            IPadElementHeader5.Text = dtChart.Rows[4][0].ToString();
            IPadElementHeader6.Text = dtChart.Rows[5][0].ToString();
			
			#region Настройка высоты подписей к диаграммам, чтобы влезали двухстрочные названия

			UltraChartFood_0001_0002_Chart1.LabelAreaHeight = 100;
			UltraChartFood_0001_0002_Chart2.LabelAreaHeight = 100;
			UltraChartFood_0001_0002_Chart3.LabelAreaHeight = 100;
			UltraChartFood_0001_0002_Chart4.LabelAreaHeight = 100;
			UltraChartFood_0001_0002_Chart5.LabelAreaHeight = 100;
			UltraChartFood_0001_0002_Chart6.LabelAreaHeight = 100;
			UltraChartFood_0001_0002_Chart1.City = "ХМАО";
			UltraChartFood_0001_0002_Chart2.City = "ХМАО";
			UltraChartFood_0001_0002_Chart3.City = "ХМАО";
			UltraChartFood_0001_0002_Chart4.City = "ХМАО";
			UltraChartFood_0001_0002_Chart5.City = "ХМАО";
			UltraChartFood_0001_0002_Chart6.City = "ХМАО";

			#endregion
			
			UltraChartFood_0001_0002_Chart1.Source = dtChart.Rows[0];
            UltraChartFood_0001_0002_Chart2.Source = dtChart.Rows[1];
            UltraChartFood_0001_0002_Chart3.Source = dtChart.Rows[2];
            UltraChartFood_0001_0002_Chart4.Source = dtChart.Rows[3];
            UltraChartFood_0001_0002_Chart5.Source = dtChart.Rows[4];
            UltraChartFood_0001_0002_Chart6.Source = dtChart.Rows[5];
        }

        #endregion

		#region Расчет геометрического среднего

		/// <summary>
		/// Разбирает таблицу с данными. На выходе массив: минимальное и максимальное значение среднего по МР
		/// </summary>
		/// <param name="dtData">Таблица с данными</param>
		/// <param name="results">Массив с результатами</param>
        protected void ParseData(DataTable dtData, object[] results, DataRow beloyarRow)
		{
			if (dtData == null || dtData.Rows.Count == 0)
			{
				for (int i = 0; i < results.Length; ++i)
					results[i] = DBNull.Value;
				return;
			}
			double[] gmValues = new double[dtData.Rows.Count / 2];
			string[] gmRegions = new string[dtData.Rows.Count / 2];
			for (int i = 0; i < dtData.Rows.Count - 1; i += 2)
			{
				int index = i / 2;
				DataRow row = dtData.Rows[i];
				DataRow prevRow = dtData.Rows[i + 1];
				double value = 0, prevValue = 0, delta = 0, rate = 0;
                if (row[0].ToString().Contains("Белоярский муниципальный район"))
                {
                    value = Convert.ToDouble(MathHelper.GeoMean(row, 1, 1, 0, beloyarRow["Текущая цена"]));
                    prevValue = Convert.ToDouble(MathHelper.GeoMean(prevRow, 1, 1, 0, beloyarRow["Цена на предыдущую дату"]));
                }
                else
                {
                    value = MathHelper.GeoMean(row, 1, row.ItemArray.Length - 1, 1, 0);
                    prevValue = MathHelper.GeoMean(prevRow, 1, prevRow.ItemArray.Length - 1, 1, 0);
                }
				delta = value - prevValue;
				rate = (value / prevValue - 1) * 100;
				gmValues[index] = value;
				string region = row[0].ToString().Split(';')[0].Replace("Город ", String.Empty).Replace(" муниципальный район", String.Empty);
				gmRegions[index] = MergeTriple(region, rate, delta);
			}
			Array.Sort(gmValues, gmRegions);
			SplitTriple(gmRegions[0], out results[3], out results[1], out results[2]);
			results[0] = gmValues[0];
			int n = gmValues.Length - 1;
			SplitTriple(gmRegions[n], out results[7], out results[5], out results[6]);
			results[4] = gmValues[n];
		}

		protected string MergeTriple(string name, double rate, double delta)
		{
			return String.Format("{0};{1:N2};{2:N2}", name, rate, delta);
		}

		protected void SplitTriple(string triple, out object name, out object rate, out object delta)
		{
			string[] data = triple.Split(';');
			name = data[0];
			rate = Convert.ToDouble(data[1]);
			delta = Convert.ToDouble(data[2]);
		}

		#endregion
	}
}
