using System.Data;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
	// обертка над диаграммой по видам сообщения

	public class ChartHelpTransportGoods : ChartHelpTransportBase
	{
		public ChartHelpTransportGoods()
		{
			TextLabels = "Досмотрено партий";
			TextHints = "Досмотрено партий";
		}
	}

	public class ChartHelpTransportPeople : ChartHelpTransportBase
	{
		public ChartHelpTransportPeople()
		{
			TextLabels = "Досмотрено лиц";
			TextHints = "Досмотрено лиц";
		}
	}

	public class ChartHelpTransportTransport : ChartHelpTransportBase
	{
		public ChartHelpTransportTransport()
		{
			TextLabels = "Досмотрено TC";
			TextHints = "Досмотрено TC";
		}
	}

	public class ChartHelpTransportBase : ChartHelpBase
	{
		public string TextLabels { set; get; }
		public string TextHints { set; get; }

		public override void Init(int chartID, string queryName)
		{
			base.Init(chartID, queryName, typeof(ChartHelpTransport));

			helper.TextLabels = TextLabels;
			helper.TextHints = TextHints;

			helper.SetStyle(SKKHelper.defaultHelpItemWidth, SKKHelper.defaultChartHeight);
			helper.Chart.Width = SKKHelper.defaultHelpItemWidth;
			helper.SetData(queryName);
		}
	}

	public class ChartHelpTransport : ChartTransportBase
	{
		public ChartHelpTransport(UltraChart chart) 
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
