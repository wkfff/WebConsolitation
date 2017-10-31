using System;
using System.Data;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.SkkSupport 
{

	public class ChartPointsTransport : ChartPointsBase
	{
		public ChartPointsTransport(UltraChart chart)
			: base(chart)
		{
			TextLabels = "Досмотрено ТС";
			TextHints = "Досмотрено ТС";
		}
	}

	public class ChartPointsPeople : ChartPointsBase
	{
		public ChartPointsPeople(UltraChart chart)
			: base(chart)
		{
			TextLabels = "Досмотрено лиц";
			TextHints = "Досмотрено лиц";
		}
	}

	public class ChartPointsGoods : ChartPointsBase
	{
		public ChartPointsGoods(UltraChart chart)
			: base(chart)
		{
			TextLabels = "Партий грузов";
			TextHints = "Досмотрено партий грузов";
		}
	}
	
	public abstract class ChartPointsBase : ChartBase
	{
		protected ChartPointsBase(UltraChart chart) 
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

			CRHelper.FillCustomColorModelLight(Chart, 300, true);

			Chart.DoughnutChart.InnerRadius = 40;
			Chart.DoughnutChart.OthersCategoryText = "Прочие";
			Chart.DoughnutChart.OthersCategoryPercent = 5;
			Chart.DoughnutChart.Labels.Font = defaultFont;
			Chart.DoughnutChart.Labels.FormatString = String.Format("<ITEM_LABEL>\n{0}: <DATA_VALUE:N0>\n<PERCENT_VALUE:#0.00>%", TextLabels);
			Chart.Tooltips.FormatString = String.Format("&nbsp;<ITEM_LABEL>&nbsp;\n&nbsp;{0}: <b><DATA_VALUE:N0></b>&nbsp;\n&nbsp;<b><PERCENT_VALUE:#0.00>%</b>&nbsp;", TextHints);
			
			Chart.FillSceneGraph += FillSceneGraph;
		}

		/// <summary>
		/// установить данные
		/// </summary>
		public override void SetData(string queryName)
		{
			DataTable dtChart = (new Query(queryName)).GetDataTable();

			foreach (DataRow row in dtChart.Rows)
			{
				row[0] = row[0].ToString().Replace("\"", "''");
				row[0] = SKKHelper.WrapString(row[0].ToString(), SKKHelper.maxStringLength, "\n");
			}

			Chart.Series.Clear();
			Chart.Series.Add(CRHelper.GetNumericSeries(1, dtChart));
		}

		/// <summary>
		/// доп обработка
		/// </summary>
		private static void FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];
				
				// хинты
				if (primitive is Wedge)
				{
					Wedge hint = (Wedge)primitive;
					if (hint.DataPoint != null)
					{
						hint.DataPoint.Label = hint.DataPoint.Label.Replace("\n", "&nbsp;\n&nbsp;");
					}
				}

			}
		}


	}
}
