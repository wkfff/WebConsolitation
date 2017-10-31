using System.Data;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
	// обертка над диаграммой по федеральным округам

	public class ChartHelpDistrictsGoods : ChartHelpDistrictsBase
	{
		public ChartHelpDistrictsGoods()
		{
			TextLabels = "Досмотрено партий";
			TextHints = "Досмотрено партий";
		}
	}

	public class ChartHelpDistrictsPeople : ChartHelpDistrictsBase
	{
		public ChartHelpDistrictsPeople()
		{
			TextLabels = "Досмотрено лиц";
			TextHints = "Досмотрено лиц";
		}
	}

	public class ChartHelpDistrictsTransport : ChartHelpDistrictsBase
	{
		public ChartHelpDistrictsTransport()
		{
			TextLabels = "Досмотрено TC";
			TextHints = "Досмотрено TC";
		}
	}

	public abstract class ChartHelpDistrictsBase : ChartHelpBase
	{
		public string TextLabels { set; get; }
		public string TextHints { set; get; }

		public override void Init(int chartID, string queryName)
		{
			base.Init(chartID, queryName, typeof(ChartHelpDistricts));

			helper.TextLabels = TextLabels;
			helper.TextHints = TextHints;

			helper.SetStyle(SKKHelper.defaultHelpItemWidth, SKKHelper.defaultChartHeight);
			helper.Chart.Width = SKKHelper.defaultHelpItemWidth;
			helper.Chart.DoughnutChart.OthersCategoryPercent = 0;

			helper.SetData(queryName);
		}
	}

	public class ChartHelpDistricts : ChartDistrictsBase
	{
		public ChartHelpDistricts(UltraChart chart) 
			: base(chart)
		{
			// empty
		}

		/// <summary>
		/// установить данные
		/// </summary>
		public override void SetData(string queryName)
		{
			DataTable dtChart = (new Query(queryName)).GetDataTable();
			if (dtChart.Rows.Count > 1)
			{
				dtChart.Rows.RemoveAt(0);

				Chart.Series.Clear();
				Chart.Series.Add(CRHelper.GetNumericSeries(1, dtChart));
			}
			else
			{
				SetChartHeight(SKKHelper.defaultChartNoDataHeight);
			}
		}

	}
}
