using System;
using System.Data;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.SkkSupport
{

	public class ChartSubjectsTransport : ChartSubjectsBase
	{
		public ChartSubjectsTransport(UltraChart chart)
			: base(chart)
		{
			TextLabels = "Досмотрено ТС";
			TextHints = "Досмотрено ТС";
		}
	}

	public class ChartSubjectsPeople : ChartSubjectsBase
	{
		public ChartSubjectsPeople(UltraChart chart)
			: base(chart)
		{
			TextLabels = "Досмотрено лиц";
			TextHints = "Досмотрено лиц";
		}
	}

	public class ChartSubjectsGoods : ChartSubjectsBase
	{
		public ChartSubjectsGoods(UltraChart chart)
			: base(chart)
		{
			TextLabels = "Партий грузов";
			TextHints = "Досмотрено партий грузов";
		}
	}

	public abstract class ChartSubjectsBase : ChartBase
	{
		protected ChartSubjectsBase(UltraChart chart) 
			: base(chart)
		{
			// empty
		}

		/// <summary>
		/// настройка внешнего вида
		/// </summary>
		public override void SetStyle()
		{
			SetStyle(SKKHelper.default2ItemsWidth, SKKHelper.defaultChartHeight);
		}

		/// <summary>
		/// настройка внешнего вида
		/// </summary>
		public override void SetStyle(double width, double height)
		{
			base.SetStyle(width, height);

			Chart.ChartType = ChartType.DoughnutChart;

			CRHelper.FillCustomColorModelLight(Chart, 100, true);

			Chart.DoughnutChart.InnerRadius = 40;
			Chart.DoughnutChart.OthersCategoryText = "Прочие";
			Chart.DoughnutChart.OthersCategoryPercent = 4;
			Chart.DoughnutChart.Labels.Font = defaultFont;
			Chart.DoughnutChart.Labels.FormatString = String.Format("<ITEM_LABEL>\n{0}: <DATA_VALUE:N0>\n<PERCENT_VALUE:#0.00>%", TextLabels);
			Chart.Tooltips.FormatString = String.Format("&nbsp;<ITEM_LABEL>&nbsp;\n&nbsp;{0}: <b><DATA_VALUE:N0></b>&nbsp;\n&nbsp;<b><PERCENT_VALUE:#0.00>%</b>&nbsp;", TextHints);
		}

		/// <summary>
		/// установить данные
		/// </summary>
		public override void SetData(string queryName)
		{
			DataTable dtChart = (new Query(queryName)).GetDataTable();
			Chart.Series.Clear();
			Chart.Series.Add(CRHelper.GetNumericSeries(1, dtChart));
		}

	}
}
